# Busylight
The Busylight project can be used to indicated your status with an actual traffic ligth that can be connected via USB).  
The concrete use case for me was to have an indication outside my office at home to indicate whether the Webcam is currently on (red), I'm in an audio-only conversation (orange) or it's safe to enter the room without any problem.

The system is based on a backend that is running on a machine that has the traffic light connected. This can either be Windows or Unix (e.g. a Raspberry Pi).

[![Build Status](https://dev.azure.com/huserben/busylight/_apis/build/status/huserben.busylight?branchName=master)](https://dev.azure.com/huserben/busylight/_build/latest?definitionId=48&branchName=master)
[![Dockerhub](https://img.shields.io/docker/pulls/huserben/busylight)](https://hub.docker.com/repository/docker/huserben/busylight)

## Hardware
- Cleware USB Traffic Light (https://www.cleware-shop.de/epages/63698188.sf/de_DE/?ObjectID=46834005)
- Raspberry Pi (any model with USB should do, I suggest a Model 3 or newer) for integrated Wifi

## Software
- Backend written in Asp.Net Core, using Signal R for coummunication with connected Clients
- Fronted using React.js

## Third Party Software
Thanks goes out to the following people/sites/projects that were used:
- [Cleware Control software for Linux](https://www.vanheusden.com/clewarecontrol/)
  - [Git Repo](https://github.com/flok99/clewarecontrol)
 - [Traffict Light Component for React.js](https://github.com/sgnh/react-trafficlight)

## Usage
You can either clone the repo and build everything from scratch with Visual Studio or VS Code.
If you just want to use it, you can use the docker image hosted at dockerhub: *[huserben/busylight](https://hub.docker.com/repository/docker/huserben/busylight)*

Run the following command to fetch the image and run a container that is exposing the web page to port 8080:  
*docker run -p 8080:80 -d --restart unless-stopped --privileged --name busylight huserben/busylight*  

**Note**: the privileged flag is needed as otherwise the cleware device will not be recognized  

If you combine it with [watchtower](https://github.com/containrrr/watchtower) then you will get automatically the newest versions as soon as they will be published on dockerhub:  
*docker run -d --name watchtower -v /var/run/docker.sock:/var/run/docker.sock --restart unless-stopped containrrr/watchtower busylight watchtower*


If you want to see some of the logging happening from the backend, you can also run it interactive (e.g. for debugging:):  
*docker run -p 8080:80 -it --privileged --name busylight huserben/busylight*
