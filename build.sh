#!/usr/bin/env bash
cd src/IdentityServer4.Admin
bower update
cd ../..
docker build -t ids4admin .