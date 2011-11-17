using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine.Data
{
    class InsertData: UpdateDb
    {
        public static void DoInsert()
        {
            DoInsertConfig();
            DoInsertMessageTypes();
            DoInsertRanks();
            DoInsertPermissions();
            DoInsertUsers();
        }

        private static void DoInsertConfig()
        {
            UpdateConfig("backup_path", @".\Backups\");
            UpdateConfig("biomeextractor_exe", @".\BiomeExtractor\BiomeExtractor.jar");
            UpdateConfig("c10t_exe", @".\c10t\c10t.exe");
            UpdateConfig("cache_path", @".\Cache\");
            UpdateConfig("db_version", DatabaseManager.CURRENT_VERSION.ToString());
            UpdateConfig("java_exe", @"java.exe");
            UpdateConfig("java_heap_init", @"1024");
            UpdateConfig("java_heap_max", @"1024");
            UpdateConfig("map_output_path", @".\Maps\");
            UpdateConfig("map_small_width", @"250");
            UpdateConfig("minecraft_path", @".\Minecraft\");
            UpdateConfig("overviewer_exe", @".\Overviewer\overviewer.exe");
        }

        private static void DoInsertMessageTypes()
        {
            InsertMessage(@"StartupComplete", @"^(?<timestamp>.+?)\ \[INFO]\ Done\ \((?<time>\d+)ns\)!\ For\ help,\ type\ ""help""\ or\ ""\?""$", @"Regex");
            InsertMessage(@"SaveComplete", @"[INFO] CONSOLE: Save complete", @"EndsWith");
            InsertMessage(@"ErrorPortBusy", @"[WARNING] **** FAILED TO BIND TO PORT!", @"EndsWith");
            InsertMessage(@"SaveStarted", @"[INFO] CONSOLE: Forcing save..", @"EndsWith");
            InsertMessage(@"UserLoggedIn", @"^(?<timestamp>.+?)\ \[INFO]\ (?<username>\w+?)\ \[(?<address>.+?)]\ logged\ in\ with\ entity\ id\ (?<entityid>\d+)\ at\ \((?<LocX>\-?\d+\.\d+),\ (?<LocY>\-?\d+\.\d+),\ (?<LocZ>\-?\d+\.\d+)\)$", @"Regex");
            InsertMessage(@"UserLoggedOut", @"^(?<timestamp>.+?)\[INFO]\ (?<username>\w+?)\ lost\ connection:\ (?<reason>.+?)$", @"Regex");
            InsertMessage(@"AutoSaveEnabled", @"[INFO] CONSOLE: Enabling level saving..", @"EndsWith");
            InsertMessage(@"AutoSaveDisabled", @"[INFO] CONSOLE: Disabling level saving..", @"EndsWith");
            InsertMessage(@"UserList", @"^(?<timestamp>.+?)\[INFO]\ Connected\ players:\ (?<userlist>.*?)$", @"Regex");
            InsertMessage(@"UserCount", @"^Player\ count:\ (?<count>\d+)$", @"Regex");
            InsertMessage(@"MinecraftBanner", @"^(?<timestamp>.+?)\[INFO]\ Starting\ minecraft\ server\ version\ (?<version>.*?)$", @"Regex");
            InsertMessage(@"ErrorInvalidMove", @"^(?<timestamp>.+?)\[WARNING]\ (?<username>\w+?)\ moved\ wrongly!$", @"Regex");
            InsertMessage(@"OpUser", @"^(?<timestamp>.+?)\[INFO]\ CONSOLE:\ Opping (?<username>\w+?)$", @"Regex");
            InsertMessage(@"DeopUser", @"^(?<timestamp>.+?)\[INFO]\ CONSOLE:\ De-opping (?<username>\w+?)$", @"Regex");
            InsertMessage(@"BanningUser", @"^(?<timestamp>.+?)\[INFO]\ CONSOLE:\ Banning (?<username>\w+?)$", @"Regex");
            InsertMessage(@"WhiteListOn", @"[INFO] CONSOLE: Turned on white-listing", @"EndsWith");
            InsertMessage(@"WhiteListOff", @"[INFO] CONSOLE: Turned off white-listing", @"EndsWith");
            InsertMessage(@"PardoningUser", @"^(?<timestamp>.+?)\[INFO]\ CONSOLE:\ Pardoning (?<username>\w+?)$", @"Regex");
            InsertMessage(@"ServerCommand", @"^(?<timestamp>.+?)\[INFO]\ (?<username>\w+?)\ issued\ server\ command:\ (?<command>.*?)$", @"Regex");
            InsertMessage(@"TriedServerCommand", @"^(?<timestamp>.+?)\[INFO]\ (?<username>\w+?)\ tried\ command:\ (?<command>.*?)$", @"Regex");
        }

        private static void DoInsertPermissions()
        {
            InsertPermission(1, "get-item");
            InsertPermission(4, "backup");
            InsertPermission(6, "reboot");
            InsertPermission(5, "stop");
            InsertPermission(5, "start");
            InsertPermission(5, "restart");
            InsertPermission(5, "abort");
            InsertPermission(3, "maps");
        }

        private static void DoInsertRanks()
        {
            InsertRank("Everyone");
            InsertRank("Authorised");
            InsertRank("Operator");
            InsertRank("Administrator");
            InsertRank("Console");
            InsertRank("System");
        }

        private static void DoInsertUsers()
        {
            InsertUser("console", mDb.Ranks.First(r => r.Name == "Console"));
        }

    }
}
