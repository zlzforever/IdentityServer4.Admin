# IdentityServer4.Admin

基于 IdentityServer4 开发的授权、用户管理、角色管理

| OS | Status |
|---|---|
| Ubuntu 16.04 | [![Build Status](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_apis/build/status/Ids4.Admin%20Build)](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_build/latest?definitionId=2) |

### 说明

这是一个快速的开发的版本，没有设计、没有构架、没有性能优化。因为只是一个管理平台，所以对管理的性能需求并不高，并不会影响 IDS4 对授权接口、验证接口的性能。所以如果有代码洁癖的可以略过本项目。

在我有时间后可能会重构一版，或者有意向一起参于这个项目的可以发 PR 给我。

### How to use

#### Install Docker-CE

#### Prepare SqlServer/MySql

+ Right now only support SqlServer/MySql

#### Prepare configuration

        $ sudo mkdir ~/ids4admin2
        $ sudo cd ~/ids4admin2
        $ sudo curl https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/src/IdentityServer4.Admin/appsettings.json -O

Then change `ConnectionString` to your database connection string in the appsettings.json, make sure `DatabaseProvider` is correct for your database

#### Pull & start docker images

        $ sudo docker pull zlzforever/ids4admin:latest
        $ sudo docker run -d --name ids4admin2 --restart always -e ADMIN_PASSWORD=1qazZAQ! -v ~/ids4admin2:/ids4admin2 -p 5566:7896 zlzforever/ids4admin /ids4admin2/appsettings.json
        
#### Start from browser

        http://localhost:5566

 Default administrator account: admin  1qazZAQ!

### How to build the latest docker image

#### Prepare yarn

+ Install yarn

    https://yarn.bootcss.com/

+ Install js package

        $ cd src/IdentityServer4.Admin
        $ yarn install    
    
#### Execute build script
    
    $ cd src/IdentityServer4.Admin
    $ sh build.sh                     

### Images

![1](https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/images/001.png)
![2](https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/images/002.png)
![3](https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/images/003.png)
![4](https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/images/004.png)
![5](https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/images/005.png)
![6](https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/images/006.png)







