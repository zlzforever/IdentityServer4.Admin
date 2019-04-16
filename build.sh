#!/usr/bin/env bash
cd src/IdentityServer4.Admin
yarn install
cd ../..
docker build -t ids4admin .