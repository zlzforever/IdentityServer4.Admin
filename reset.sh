#!/usr/bin/env bash
sudo docker stop ids4admin2
sudo docker rm ids4admin2
sudo docker pull zlzforever/ids4admin:latest
sudo docker run -d --name ids4admin2 --restart always -e ADMIN_PASSWORD=1qazZAQ! -v ~/ids4admin2:/ids4admin2 -p 5566:7896 zlzforever/ids4admin /ids4admin2/appsettings.json