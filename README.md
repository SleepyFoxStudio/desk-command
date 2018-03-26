# desk-command

## Streaming companion to control streaming software from tablet on your desk.
Software runs on the pc, and then tablets phones etc connect just using a web browser, to control your streaming software, focusing on OBS to begin with.


## Getting started

PreBuilt source available in the releases section https://github.com/tridionted/desk-command/releases


1. Clone this repo/grab the latest built release
2. If you grabbed pre-built release jump to step 5 
3. Open in visual studio 
4. Click _Build/Publish_ and publish to local file system
5. Take the published contents and put it somewhere on the filesystem you want the app to run from.
6. open the folder and run the "Desk Command Core.exe"
7. when the app starts it should tell you how to access the remote interface saying something like *Now listening on: http://localhost:5000*
8. start by testing in a local browser that the interface loads and works
9. In the sample configuration it defaults the two OBS scene buttons to sending the HotKey SHIFT+F1 for scene 1 and SHIFT+F2 for scene 2 make sure you go in to OBS and open settings/Hotkeys and makse sure you fill in the same values in order for it to work.
10. if you want to access from another Tablet etc on your same network then Open Windows firewall and make an exception for the EXE or port that the Desk Command software is using
11. access the interface using the IP of PC running the software e.g if it's running on http://localhost:5000 and your internal IP is 192.168.55.2 then you would point your tablet web browser to http://192.168.55.2:5000 or use the machine name if your PC is called DesktopBob use http://DesktopBob:5000 make sure to use the port defined in the app when it starts as it may not always be 5000
