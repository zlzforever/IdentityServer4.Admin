#!/usr/bin/env bash
sudo docker stop ids4admin
sudo docker rm ids4admin
sudo docker pull zlzforever/ids4admin:latest
sudo docker run -d --name ids4admin --restart always -e ADMIN_PASSWORD=1qazZAQ! -v ~/ids4admin:/ids4admin -p 6566:6566 zlzforever/ids4admin /ids4admin/appsettings.json