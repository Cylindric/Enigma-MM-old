'==============================================================================
' Common options
'==============================================================================

'------------------------------------------------------------------------------
' The path to the server system.
' Defaults to the parent of the script's location.
'------------------------------------------------------------------------------
'ROOT = ..\

'------------------------------------------------------------------------------
' The path to the utility programs.
' Defaults to the Utils folder in script's location.
'------------------------------------------------------------------------------
'UTILPATH = .\Utils

'------------------------------------------------------------------------------
' The path to the actual Minecraft server
'------------------------------------------------------------------------------
'SERVERROOT = "{ROOT}\Server1"

'------------------------------------------------------------------------------
' The path to use for temporary files and caching.
' Anything in this folder can be deleted, but note that this can GREATLY increase
' the running-time of some actions
'------------------------------------------------------------------------------
'CACHEDIR = "{ROOT}\Cache"

'------------------------------------------------------------------------------
' The full path to the Python executable.
'------------------------------------------------------------------------------
'PYTHON = "C:\Program Files\Python27\python.exe"
'PYTHON = "C:\Program Files (x86)\Python27\python.exe"

'------------------------------------------------------------------------------
' The full path to the multiplexer client.
'------------------------------------------------------------------------------
'MULTIPLEXER = "{ROOT}\Server1\multiplex_client.py"


'==============================================================================
' Backup-specific options
'==============================================================================

'------------------------------------------------------------------------------
' The path to save the backups in
'------------------------------------------------------------------------------
'BACKUPS = {ROOT}\Backups

'------------------------------------------------------------------------------
' Whether or not to pause server auto-saves while backing up.
' Requires MULTIPLEXER to be set, and working.
'------------------------------------------------------------------------------
'PAUSEAUTOSAVE = True

'------------------------------------------------------------------------------
' Path to save a copy of the latest backup.
' Useful for putting the latest backup into a cloud-storage folder.
'------------------------------------------------------------------------------
'BACKUPDUPLICATE = ""

'------------------------------------------------------------------------------
' How many backups to keep.
'------------------------------------------------------------------------------
'BackupsToKeep = 5

'------------------------------------------------------------------------------
' Format for backups.
' Valid options are "7z" or "zip"
'------------------------------------------------------------------------------
'BackupFormat = "7z"




'==============================================================================
' Mapping-specific options
'==============================================================================

'------------------------------------------------------------------------------
' The full path to the ImageMagick convert and animate executables.
'------------------------------------------------------------------------------
'CONVERTER = "C:\Program Files\ImageMagick-6.6.4-Q16\convert.exe"
'ANIMATOR = "C:\Program Files\ImageMagick-6.6.4-Q16\animate.exe"

'------------------------------------------------------------------------------
' The full path to the Alpha Vespucci mapper.
' Defaults to being in our Minecraft Server root.
'------------------------------------------------------------------------------
'VESPUCCIMAPPER = "{ROOT}\AlphaVespucci\AlphaVespucci.exe"

'------------------------------------------------------------------------------
' The full path to the Minecraft-Overview Python script.
' Defaults to being in our Minecraft Server root
'------------------------------------------------------------------------------
'OVERVIEWMAPPER = "{ROOT}\Overviewer\gmap.py"

'------------------------------------------------------------------------------
' The full path to the pngout.exe image compressor.
'------------------------------------------------------------------------------
'PNGC = "{UTILS}\pngout.exe"

'------------------------------------------------------------------------------
' The path to the world to process.
'------------------------------------------------------------------------------
'WORLD = "{ROOT}\server1\world"


'------------------------------------------------------------------------------
' The path to the web root for the output files.
'------------------------------------------------------------------------------
'WEBROOT = "{ROOT}\www"


'------------------------------------------------------------------------------
' The path to the web root for the actual maps.
'------------------------------------------------------------------------------
'OUTPUT = "{ROOT}\www\maps"


'------------------------------------------------------------------------------
' Image optimisation level
' 0 - perform no image compression
' 1 - compress low-volume 'single' images
' 2 - compress the Alpha Vespucci map parts
' 3 - compress the Overviewer map parts
'------------------------------------------------------------------------------
'OPTIMISE = 0


'------------------------------------------------------------------------------
' Range of layers to slice for the slice maps.
'------------------------------------------------------------------------------
'SLICE_MIN = 1
'SLICE_MAX = 127


'------------------------------------------------------------------------------
' Sizes for the reduced images
'------------------------------------------------------------------------------
'ThumbSize = 120
'SmallSize = 800


'------------------------------------------------------------------------------
' Actions to perform
'------------------------------------------------------------------------------
'CreateSingleMaps = True
'CreateLayeredMap = True
'CreateGoogleMap = True
'CreateSliceMaps = False
'CreateHistoryAnim = False



' Server layout should be like this:
'
' MCServerPath\
' |
' +-+- AlphaVespucci\
' | +--- AlphaVespucci.exe
' |
' +-+-- Scripts\
' | +--- Utils\
' | | +--- pngout.exe
' | |
' | +--- mapper.vbs
' |
' +-+-- Cache\
' | +--- History\
' | +--- Overviewer\
' | +-+- Slices\
' |   +--- Thumbs\
' |
' +-+- Server\
' | +--- minecraft_server.jar
' |
' +-+- www\
' | +--- Maps\
' |
