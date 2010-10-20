Minecraft Server Manager
========================

Requirements
============

  * Microsoft .NET Framework 3.5 (not tested on Mono, but might work)
  * The Sun "JVM" (Tested with 1.6 update 21)
  * Minecraft Server (Java version)

Setup
=====
Suggested installation is to have a master "server" folder somewhere on the server, for example C:\Minecraft, which contains the necessary files and folders.

Here's the bare minimum to get started.  A lot of other files will be created at the first run, not least the world itself...

C:\Minecraft\

C:\Minecraft\Server\                         <-- The actual Minecraft server
C:\Minecraft\Server\minecraft_server.jar

C:\Minecraft\ServerManager\                  <-- Put the EMM files here
C:\Minecraft\ServerManager\emm.dll
C:\Minecraft\ServerManager\emmserver.exe


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