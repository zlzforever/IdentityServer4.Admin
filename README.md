# IdentityServer4.Admin

基于 IdentityServer4 开发的授权、用户管理、角色管理

| OS | Status |
|---|---|
| Ubuntu 16.04 | [![Build Status](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_apis/build/status/Ids4.Admin%20Build)](https://dev.azure.com/zlzforever/IdentityServer4.Admin/_build/latest?definitionId=2) |

### How to use

#### Install Docker-CE

1. Install docker-ce follow official document

        https://docs.docker.com/install/

2. *Change docker repository to Ali docker repository* because i only push to Ali repository

#### Prepare SqlServer/MySql

+ Right now only support SqlServer/MySql

#### Prepare configuration

        $ sudo mkdir ~/ids4admin2
        $ sudo cd ~/ids4admin2
        $ sudo curl https://raw.githubusercontent.com/zlzforever/IdentityServer4.Admin/master/src/IdentityServer4.Admin/appsettings.json -O

Then change `ConnectionString` to your database connection string in the appsettings.json, make sure `DatabaseProvider` is correct for your database

#### Pull & start docker images

        $ sudo docker pull registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin:latest
        $ sudo docker run -d --name ids4admin2 --restart always -e ADMIN_PASSWORD=1qazZAQ! -v ~/ids4admin2:/ids4admin2 -p 5566:7896 registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin /ids4admin2/appsettings.json
        
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







