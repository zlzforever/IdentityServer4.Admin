#!/usr/bin/env bash
docker stop ids4admin2
docker rm ids4admin2
docker pull registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin:latest
docker run -d --name ids4admin2 --restart always -v /ids4admin2:/ids4admin2 -p 5566:5566 registry.cn-shanghai.aliyuncs.com/zlzforever/ids4admin /ids4admin2/appsettings.json