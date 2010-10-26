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
	maps-all
	maps-av
	maps-avextra
	maps-overviewer


Any other commands are passed straight to the Minecraft server instance.
Examples:

	save-all
	save-on
	save-off



Configuration
=============
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
			Server_Mod.jar) and EMM will auto-detec it's running and enable certain
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

Requirements
============

  * Microsoft .NET Framework 3.5 (not tested on Mono, but might work)
  * The Sun "JVM" (Tested with 1.6 update 21)
  * Minecraft Server (Java version)
	* Optionally Hey0 Server mod
	* Optionally AlphaVespucci mapper


Setup
=====
Suggested installation is to have a master "server" folder somewhere on the server, for example C:\Minecraft, which contains the necessary files and folders.

Here's the bare minimum to get started.  A lot of other files will be created at the first run, not least the world itself...

C:\Minecraft\

C:\Minecraft\Minecraft\                         <-- The actual Minecraft server
C:\Minecraft\Minecraft\minecraft_server.jar

C:\Minecraft\ServerManager\                  <-- Put the EMM files here
C:\Minecraft\ServerManager\emm.dll
C:\Minecraft\ServerManager\emmserver.exe
C:\Minecraft\ServerManager\emmclient.exe
C:\Minecraft\ServerManager\settings.xml

C:\Minecraft\Maps
C:\Minecraft\Maps\History


So the setup process would be something like:

  1) Install the Sun JDK if you don't already have it

  2) Install the .NET Framework if you don't already have it

  3) Create folders to put everything in:
    a) C:\Minecraft\
    b) C:\Minecraft\Server\
    c) C:\Minecraft\ServerManager\

  4) Download the official Minecraft Server (Java version!) and put it in
     C:\Minecraft\Server\

  5) Build or download the Server Manager and put it in
     C:\Minecraft\ServerManager\


Solution
========
   * Build - All projects' build output goes here
   * Deploy - Final deployment files go here
   * EMM - The main Minecraft interfacing library, and common helpers
   * Server - The main Server Manager