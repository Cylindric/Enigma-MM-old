Minecraft Server Manager
========================

Usage
=====
Available commands within the Server Manager (emmserver or emmclient) console:

	start            - starts the Minecraft server
	stop             - stops the Minecraft server
	stop-graceful    - same as stop, but waits until there are no users online
	restart          - stops and then restarts the Minecraft server
	restart-graceful - same as restart, but waits until there are no users online
	abort-graceful   - cancels any pending graceful actions
	quit             - stop the server if it's running, then quit the EMM console
	maps-av          - generates the primary maps using AlphaVespucci
	maps-avextra     - generates additional maps using AlphaVespucci
	maps-overviewer  - generates the Overview map (the Google-maps style one)
	maps-all         - generates all available maps

Any other commands are passed straight to the Minecraft server instance.
Examples:

	save-all
	save-on
	save-off



Configuration
=============
Copy the sample.settings.conf to just "settings.conf", and customise as required.
Details about each setting are in the sample file.



Requirements
============

  * Microsoft .NET Framework 3.5 (not tested on Mono, but might work)
  * The Sun "JVM" (Tested with 1.6 update 21)
  * Minecraft Server (Java version)
  * Optionally Hey0 Server mod
  * Optionally AlphaVespucci mapper
  * Optionally Minecraft Overviewer mapper


Setup
=====
Suggested installation is to have a master "server" folder somewhere on the server, for example C:\EnigmaMM\, which contains the necessary files and folders.

Here's the bare minimum to get started.  A lot of other files will be created at the first run, not least the world itself...

C:\EnigmaMM\
C:\EnigmaMM\emm.dll
C:\EnigmaMM\emmclient.exe
C:\EnigmaMM\emmserver.exe
C:\EnigmaMM\LibNbt.dll
C:\EnigmaMM\LibNbt.txt
C:\EnigmaMM\optipng.exe
C:\EnigmaMM\optipng.txt
C:\EnigmaMM\settings.conf

C:\EnigmaMM\Minecraft\                      <-- The actual Minecraft server
C:\EnigmaMM\Minecraft\minecraft_server.jar

C:\EnigmaMM\ServerManager\                  <-- Put the EMM files here

C:\EnigmaMM\Cache         <-- The cache files will end up in here as needed
C:\EnigmaMM\Maps          <-- Maps will end up in here once generated

C:\EnigmaMM\AlphaVespucci <-- Put the AlphaVespucci executable in here if needed
C:\EnigmaMM\Overviewer    <-- Put the Overviewer files in here if needed



So the setup process would be something like:

  1) Install the Sun JDK if you don't already have it

  2) Install the .NET Framework if you don't already have it

  3) Create folders to put everything in:
    a) C:\EnigmaMM\
    b) C:\EnigmaMM\Minecraft\
    c) C:\EnigmaMM\AlphaVespucci\

  4) Download the official Minecraft Server (Java version!) and put it in
     C:\EnigmaMM\Minecraft\

  5) Build or download the Server Manager and put it in
     C:\EnigmaMM\
