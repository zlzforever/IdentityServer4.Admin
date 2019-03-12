using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Account;
using IdentityServer4.Admin.ViewModels.Role;
using IdentityServer4.Admin.ViewModels.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace IdentityServer4.Admin
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AdminOptions _options;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _hostingEnvironment = env;
            _options = new AdminOptions(_configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add configuration
            services.AddSingleton<AdminOptions>();

            // Add MVC
            services.AddMvc()
                //.AddMvcOptions(o => o.Filters.Add<HttpGlobalExceptionFilter>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization();

            services.AddSession();

            // Add DbContext            
            Action<DbContextOptionsBuilder> dbContextOptionsBuilder;
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            switch (_options.DatabaseProvider.ToLower())
            {
                case "mysql":
                {
                    dbContextOptionsBuilder = b =>
                        b.UseMySql(_options.ConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                }
                case "sqlserver":
                {
                    dbContextOptionsBuilder = b =>
                        b.UseSqlServer(_options.ConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                }
                default:
                {
                    throw new Exception($"Unsupported database provider: {_options.DatabaseProvider}");
                }
            }

            services.AddDbContext<IDbContext, AdminDbContext>(dbContextOptionsBuilder);

            // Add aspnetcore identity
            IdentityBuilder idBuilder = services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireUppercase = _options.RequireUppercase;
                options.Password.RequireNonAlphanumeric = _options.RequireNonAlphanumeric;
                options.Password.RequireDigit = _options.RequireDigit;
                options.Password.RequiredLength = _options.RequiredLength;
                options.User.RequireUniqueEmail = _options.RequireUniqueEmail;
            }).AddErrorDescriber<CustomIdentityErrorDescriber>();

            idBuilder.AddDefaultTokenProviders();
            idBuilder.AddEntityFrameworkStores<AdminDbContext>();

            // Add ids4
            var builder = services.AddIdentityServer()
                .AddAspNetIdentity<User>();

            var key = Directory.Exists("/ids4admin") ? "/ids4admin/signing_key.rsa" : "signing_key.rsa";
            builder.AddDeveloperSigningCredential(true, key);

            builder.AddConfigurationStore<AdminDbContext>(options =>
                {
                    options.ResolveDbContextOptions = (provider, b) => dbContextOptionsBuilder(b);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore<AdminDbContext>(options =>
                {
                    options.ResolveDbContextOptions = (provider, b) => dbContextOptionsBuilder(b);
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = _options.EnableTokenCleanup;
                });
            builder.AddProfileService<ProfileService>();

            // Configure AutoMapper
            ConfigureAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            PrePareDatabase(app.ApplicationServices);
            if (_hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            if (_options.StorageRoot == "wwwroot")
            {
                app.UseStaticFiles();
            }
            else
            {
                app.UseFileServer(new FileServerOptions
                {
                    FileProvider = new PhysicalFileProvider(_options.StorageRoot),
                });
                DirectoryHelper.Move("wwwroot", _options.StorageRoot);
            }

            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void PrePareDatabase(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
            logger.LogInformation("Configuration: " + _options.Version);

            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                if (scope.ServiceProvider.GetRequiredService<AdminDbContext>().Database.EnsureCreated())
                {
                    logger.LogInformation("Created database success");
                }
            }

            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                new SeedData(logger, scope.ServiceProvider).EnsureData();
            }
        }

        private void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CreateUserViewModel, User>();
                cfg.CreateMap<Role, ViewRoleViewModel>();
                cfg.CreateMap<Role, ViewUserRoleViewModel>();
                cfg.CreateMap<CreateRoleViewModel, Role>();     
                cfg.CreateMap<UpdateProfileViewModel, User>();
                cfg.CreateMap<UpdateProfileViewModel, ProfileViewModel>();
                cfg.CreateMap<User, ListUserItemViewModel>();
                cfg.CreateMap<ViewUserViewModel, User>();
            });
        }
    }
}