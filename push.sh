#!/usr/bin/env bash
cd src/IdentityServer4.Admin
bower update
cd ../..
docker build -t ids4admin .
docker tag ids4admin registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin
docker push registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin
tag=$(date +%Y%m%d%H%M%S)
docker tag ids4admin registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin:$tag
docker push registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin:$tag