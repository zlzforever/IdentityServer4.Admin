using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TokenResponseGenerator = IdentityServer4.Admin.Infrastructure.TokenResponseGenerator;

namespace IdentityServer4.Admin
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly AdminOptions _options;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _configuration = configuration;
            _hostingEnvironment = env;
            _options = new AdminOptions(_configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AdminOptions));

            // Add configuration
            services.AddScoped<AdminOptions>();

            // Add MVC
            services.AddControllersWithViews()
                //.AddMvcOptions(o => o.Filters.Add<HttpGlobalExceptionFilter>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorRuntimeCompilation();

            services.AddHealthChecks();


            services.AddResponseCompression();
            services.AddResponseCaching();

            services.AddAuthorization();

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

            var key = string.IsNullOrWhiteSpace(_configuration["MountFolder"])
                ? "signing_key.rsa"
                : Path.Combine(_configuration["MountFolder"], "signing_key.rsa");

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
                }).AddResourceStore<EfResourceStore>();
            builder.AddProfileService<ProfileService>();
            builder.Services.AddTransient<ITokenResponseGenerator, TokenResponseGenerator>();
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            if (_options.StorageRoot == "wwwroot")
            {
                app.UseStaticFiles();
            }
            else
            {
                app.UseFileServer(new FileServerOptions
                {
                    FileProvider = new PhysicalFileProvider(_options.StorageRoot)
                });
                DirectoryHelper.Move("wwwroot", _options.StorageRoot);
            }

            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseResponseCompression();
            app.UseResponseCaching();
        }

        private void PrePareDatabase(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<AdminDbContext>().Database;

                var migrations = database.GetPendingMigrations().ToArray();
                if (migrations.Length > 0)
                {
                    database.Migrate();
                    logger.LogInformation($"已提交{migrations.Length}条挂起的迁移记录：{string.Join(",", migrations)}");
                }

                logger.LogInformation("Migrate database success");
            }

            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                new SeedData(logger, scope.ServiceProvider).EnsureData();
            }
        }
    }
}