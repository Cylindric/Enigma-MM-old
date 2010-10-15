Pre-requisites
==============
For all features to work, the following must be installed and working:

	* Pre-requisites for these scripts:
			* Cygwin (for multiplexer) [1]
			* PNGOUT.EXE (for map compression) [2]
			* ImageMagick [3]
			* 7-zip (included) [4]

	* Pre-requisites for the external tools:
			* Python (for Overviewer and Multiplexer) [5]
			* Python Image Library (for Overviewer) [6]
			* Python Numpy (for Overviewer) [7]

	* The external tools:
	  * Flippeh Multiplexer
	  * Alpha Vespucci
	  * Minecraft Overviewer

[1] Cygwin: http://www.cygwin.com/
[2] PNGOUT.EXE: http://advsys.net/ken/utils.htm
[3] ImageMagick: http://www.imagemagick.org/script/binary-releases.php#windows
[4] 7-Zip: http://www.7-zip.org/download.html
[5] Python: http://www.python.org/download/
[6] Python Image Library: http://effbot.org/downloads/#pil
[7] Python Numpy: http://new.scipy.org/download.html

config.vbs
==========
Edit this to set any local options and to override the defaults.
Check the file for details on each option.


backup.vbs
==========
The main backup script.  Call this to perform backups of the server.


mapper.vbs
==========
The main mapper script.  Call this to generate the various available maps.
Default parameters are:
	/SingleMaps /LayeredMap /OverviewerMap

Available parameters:
	Parameters to enable a feature (these will be in addition to any defaults)
	  /SingleMaps
	  /LayeredMap
	  /OverviewerMap
	  /SliceMaps
	  /HistoryAnim

	Parameters to disable a feature
	  /NoSingleMaps
	  /NoLayeredMap
	  /NoOverviewerMap
	  /NoSliceMaps
	  /NoHistoryAnim

	Parameters to force only a single feature (overrides the defaults)
	  /OnlySingleMaps
	  /OnlyLayeredMap
	  /OnlyOverviewerMap
	  /OnlySliceMaps
	  /OnlyHistoryAnim
