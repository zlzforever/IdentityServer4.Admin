#!/usr/bin/env bash
cd src/IdentityServer4.Admin || exit
yarn install
cd ../..
docker build -t ids4admin .
docker tag ids4admin registry-docker.pamirs.com/ids4admin:latest
docker push registry-docker.pamirs.com/ids4admin:latest
tag=$(date +%Y%m%d)
docker tag ids4admin registry-docker.pamirs.com/ids4admin:$tag
docker push registry-docker.pamirs.com/ids4admin:$tag