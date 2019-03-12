# IdentityServer4.Admin

基于 IdentityServer4 开发的授权、用户管理、角色管理、权限管理

| OS | Status |
|---|---|
| Ubuntu 16.04 | [![Build Status](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_apis/build/status/Ids4.Admin%20Build)](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_build/latest?definitionId=2) |

### How to use

#### Prepare configuration

        $ mkdir /ids4admin2
        $ 

#### run a test instance

        $ docker run -d --name ids4admin --restart always  -e ADMIN_PASSWORD=1qazZAQ! -p 6566:6566 -p 6201:6201 zlzforever/ids4admin:latest /dev

+ try open http://localhost:6566 and login by user: admin, password: 1qazZAQ!
+ ids4admin runs in a memory storage with dev mode
        
#### run a production instance

        $ sudo mkdir /ids4admin
        $ sudo curl -o /ids4admin/appsettings.json https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/src/IdentityServer4.Admin/appsettings.json
        
change the configuration as you wish, for example database settings, support SqlServer and MySql right now:
        
        "DatabaseProvider": "MySql",
        "ConnectionString": "Database='ids4';Data Source=192.168.90.109;User ID=root;Password=xxxx;Port=60007;SslMode=None;",

then start a new instance          
                 
        $ docker run -d --name ids4admin --restart always -v /ids4admin:/ids4admin -p 6566:6566 -p 6201:6201 zlzforever/ids4admin:latest /ids4admin/appsettings.json

### How to build the latest docker image

        $ sh build.sh                     

### 功能说明

角色 

| feature | detail  | permission |   state |
|----|----|----|----|
| 添加角色    |  角色名不能重复, 不能添加名为 admin 的角色  | admin |   ☑    |
| 修改角色 |   不能修改 admin 角色       | admin |   ☑    |
| 删除角色 |  删除角色对象，删除用户角色记录，admin 角色不能删除          | admin |   ☑   |
| 查询角色    |           |   admin  |  ☑   |

用户 

| feature | detail  | permission |   state |
|----|----|----|----|
| 添加用户        |  | admin |   ☑    |
| 修改用户 |           | admin |   ☑    |
| 删除用户 |          | admin |   ☑   |
| 锁定用户 |  被锁定的用户无法登录         | admin |   ☑   |
| 解锁用户 |           | admin |   ☑   |
| 查询用户    |           |   admin  |  ☑   |
| 设置密码    |  不需要原始密码         |   admin  |  ☑   |
| 添加用户角色    |   admin 用户不能添加角色        |   admin  |  ☑   |
| 删除用户角色    |   admin 用户不能删除角色        |   admin  |  ☑   |   
| 用户 Profile    |           |   login  |  ☑    |  
| 修改密码    |   需要原始密码        |   login  |  ☑  |





