Minecraft Server Manager
========================

Usage
=====
Available commands within the Server Manager (SM) console:

  start            - starts the Minecraft server
  stop             - stops the Minecraft server
  stop-graceful    - same as stop, but waits until there are no users online
  restart          - stops and then restarts the Minecraft server
  restart-graceful - same as restart, but waits until there are no users online
  abort-graceful   - cancels any pending graceful actions
  quit             - stop the server if it's running, then quit the SM console
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
<<<<<<< HEAD
Copy the sample.settings.conf to just "settings.conf", and customise as required.
Details about each setting are in the sample file.


=======
Core Settings
-------------
  * ServerRoot
      Default: C:\Minecraft
      Full path to where the Minecraft server is.

  * MinecraftRoot
      Default: .\Server
      Full path to where the Minecraft server is.
      Either specify the full path, or if following the recommended directory
      layout, use a path relative to ServerRoot.  (That's what the default does)

  * ServerJar
      Default: minecraft_server.jar
      The filename of the server JAR file.
      If you're using the Hey0 server mod, put the name of the jar here (usually
      Server_Mod.jar) and EMM will auto-detect when it runs and enable certain
      features.

  * JavaHeapInit
      Default: 1024
      The initial memory-heap size for Java, in megabytes.

  * JavaHeapMax
      Default: 1024
      The maximum memory-heap size for Java, in megabytes.

  * JavaExec
      Default: java.exe
      The executable to use for Java.


Mapping Settings
----------------
  * MapOutputRoot
      Default: .\Maps
      Where to save the generated maps.

  * AlphaVespucciInstalled
      Default: false
      Whether or not to use the AlphaVespucci map output options.

  * AlphaVespucciRoot
      Default: .\AlphaVespucci
>>>>>>> 2809a97a603a11d15e556221170af41bba68f2e5

Requirements
============

  * Microsoft .NET Framework 3.5 (not tested on Mono, but might work)
  * The Sun "JVM" (Tested with 1.6 update 21)
  * Minecraft Server (Java version)
  * Optionally Hey0 Server mod
  * Optionally AlphaVespucci mapper
<<<<<<< HEAD
  * Optionally Minecraft Overviewer mapper
=======
>>>>>>> 2809a97a603a11d15e556221170af41bba68f2e5


Setup
=====
Suggested installation is to have a master "server" folder somewhere on the server, for example C:\EnigmaMM\, which contains the necessary files and folders.

Here's the bare minimum to get started.  A lot of other files will be created at the first run, not least the world itself...

C:\EnigmaMM\

C:\EnigmaMM\Minecraft\                      <-- The actual Minecraft server
C:\EnigmaMM\Minecraft\minecraft_server.jar

<<<<<<< HEAD
C:\EnigmaMM\ServerManager\                  <-- Put the EMM files here
C:\EnigmaMM\ServerManager\emm.dll
C:\EnigmaMM\ServerManager\emmclient.exe
C:\EnigmaMM\ServerManager\emmserver.exe
C:\EnigmaMM\ServerManager\LibNbt.dll
C:\EnigmaMM\ServerManager\LibNbt.txt
C:\EnigmaMM\ServerManager\optipng.exe
C:\EnigmaMM\ServerManager\optipng.txt
C:\EnigmaMM\ServerManager\settings.conf
=======
C:\Minecraft\Minecraft\                      <-- The actual Minecraft server
C:\Minecraft\Minecraft\minecraft_server.jar
>>>>>>> 2809a97a603a11d15e556221170af41bba68f2e5

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
    c) C:\EnigmaMM\ServerManager\

  4) Download the official Minecraft Server (Java version!) and put it in
     C:\EnigmaMM\Minecraft\

  5) Build or download the Server Manager and put it in
<<<<<<< HEAD
     C:\EnigmaMM\ServerManager\
=======
    C:\Minecraft\ServerManager\


Solution
========
  * Build - All projects' build output goes here
  * Deploy - Final deployment files go here
  * EMM - The main Minecraft interfacing library, and common helpers
  * Server - The main Server Manager


External Libraries and Tools
============================
Project:  OptiPNG
Homepage: http://sourceforge.net/projects/optipng/
License:  zlib/libpng License

Project:  LibNbt
Homepage: https://github.com/aphistic/libnbt
License:  lGPL
>>>>>>> 2809a97a603a11d15e556221170af41bba68f2e5
