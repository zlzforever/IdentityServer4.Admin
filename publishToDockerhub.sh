#!/usr/bin/env bash
cd src/IdentityServer4.Admin
yarn install
cd ../..
docker build -t ids4admin .
docker tag ids4admin zlzforever/ids4admin:latest
docker push zlzforever/ids4admin:latest
tag=$(date +%Y%m%d%H%M%S)
docker tag ids4admin zlzforever/ids4admin:$tag
docker push zlzforever/ids4admin:$tag