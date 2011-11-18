using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine.Data
{
    class InsertData: UpdateDb
    {
        public override void DoUpdate()
        {
            DoInsertConfig();
            DoInsertMessageTypes();
            DoInsertRanks();
            DoInsertPermissions();
            DoInsertUsers();
            DoInsertItems();
        }

        private void DoInsertConfig()
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
            mDb.SubmitChanges();
        }

        private void DoInsertMessageTypes()
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
            mDb.SubmitChanges();
        }

        private void DoInsertPermissions()
        {
            InsertPermission(1, "get-item");
            InsertPermission(4, "backup");
            InsertPermission(6, "reboot");
            InsertPermission(5, "stop");
            InsertPermission(5, "start");
            InsertPermission(5, "restart");
            InsertPermission(5, "abort");
            InsertPermission(3, "maps");
            mDb.SubmitChanges();
        }

        private void DoInsertRanks()
        {
            InsertRank(1, "Everyone");
            InsertRank(2, "Authorised");
            InsertRank(3, "Operator");
            InsertRank(4, "Administrator");
            InsertRank(5, "Console");
            InsertRank(6, "System");
            mDb.SubmitChanges();
        }

        private void DoInsertItems()
        {
            InsertItem(0, "air", "Air", 1, 64, 6);
            InsertItem(1, "stone", "Stone", 64, 256, 4);
            InsertItem(2, "grass", "Grass", 64, 256, 4);
            InsertItem(3, "dirt", "Dirt", 64, 256, 4);
            InsertItem(4, "cobblestone", "Cobblestone", 64, 256, 4);
            InsertItem(5, "woodenplank", "Wooden Plank", 64, 256, 4);
            InsertItem(6, "sapling", "Sapling", 64, 256, 4);
            InsertItem(7, "bedrock", "bedrock", 1, 64, 6);
            InsertItem(8, "water", "Water", 1, 64, 5);
            InsertItem(9, "stationarywater", "Stationary Water", 1, 64, 5);
            InsertItem(10, "lava", "Lava", 1, 64, 5);
            InsertItem(11, "stationarylava", "Stationary Lava", 1, 64, 5);
            InsertItem(12, "sand", "Sand", 64, 256, 4);
            InsertItem(13, "gravel", "Gravel", 64, 256, 4);
            InsertItem(14, "goldore", "Gold (Ore)", 64, 256, 5);
            InsertItem(15, "ironore", "Iron (Ore)", 64, 256, 5);
            InsertItem(16, "coalore", "Coal (Ore)", 1, 64, 5);
            InsertItem(17, "wood", "Wood", 64, 256, 4);
            InsertItem(18, "leaves", "Leaves", 1, 64, 5);
            InsertItem(19, "sponge", "Sponge", 64, 256, 5);
            InsertItem(20, "glass", "Glass", 64, 256, 4);
            InsertItem(21, "lapislazuliore", "Lapis Lazuli (Ore)", 1, 64, 5);
            InsertItem(22, "lapislazuliblock", "Lapis Lazuli (Block)", 64, 256, 5);
            InsertItem(24, "sandstone", "Sandstone", 64, 256, 4);
            InsertItem(26, "bedblock", "Bed (Block)", 1, 64, 6);
            InsertItem(27, "poweredrail", "Powered Rail", 64, 256, 4);
            InsertItem(28, "detectorrail", "Detector Rail", 64, 256, 4);
            InsertItem(29, "stickypiston", "Sticky Piston", 64, 256, 4);
            InsertItem(30, "cobweb", "Cobweb", 64, 256, 4);
            InsertItem(31, "tallgrass", "Tall Grass", 64, 256, 5);
            InsertItem(32, "deadbush", "Dead Bush", 64, 256, 5);
            InsertItem(33, "piston", "Piston", 64, 256, 4);
            InsertItem(34, "pistonextension", "Piston Extension", 1, 1, 6);
            InsertItem(35, "wool", "Wool", 64, 256, 4);
            InsertItem(36, "blockmovedbypiston", "Block Moved By Piston", 1, 1, 6);
            InsertItem(37, "dandelion", "Dandelion", 1, 64, 4);
            InsertItem(38, "rose", "Rose", 1, 64, 4);
            InsertItem(39, "brownmushroom", "Brown Mushroom", 1, 64, 4);
            InsertItem(40, "redmushroom", "Red Mushroom", 1, 64, 4);
            InsertItem(41, "goldblock", "Gold (Block)", 64, 256, 5);
            InsertItem(42, "ironblock", "Iron (Block)", 64, 256, 5);
            InsertItem(43, "doubleslab", "Double Slab", 64, 256, 4);
            InsertItem(44, "slab", "Slab", 64, 256, 4);
            InsertItem(45, "brickblock", "Brick (Block)", 64, 256, 4);
            InsertItem(46, "tnt", "TNT", 0, 0, 5);
            InsertItem(49, "obsidian", "Obsidian", 64, 256, 4);
            InsertItem(50, "torch", "Torch", 64, 256, 4);
            InsertItem(51, "fire", "Fire", 1, 64, 5);
            InsertItem(52, "monsterspawner", "Monster Spawner", 1, 64, 5);
            InsertItem(53, "woodenstairs", "Wooden Stairs", 64, 256, 4);
            InsertItem(54, "chest", "Chest", 1, 64, 4);
            InsertItem(55, "redstonewire", "Redstone Wire", 1, 64, 6);
            InsertItem(56, "diamondore", "Diamond (Ore)", 1, 64, 5);
            InsertItem(57, "diamondblock", "Diamond (Block)", 64, 256, 5);
            InsertItem(58, "craftingtable", "Crafting Table", 1, 64, 4);
            InsertItem(59, "seedblock", "Seeds (Block)", 1, 64, 5);
            InsertItem(60, "farmland", "Farmland", 1, 64, 5);
            InsertItem(61, "furnace", "Furnace", 1, 64, 4);
            InsertItem(62, "burningfurnace", "Burning Furnace", 1, 64, 6);
            InsertItem(63, "signblock", "Sign (Block)", 1, 64, 6);
            InsertItem(64, "woodendoorblock", "Wooden Door (Block)", 1, 64, 6);
            InsertItem(65, "ladder", "Ladder", 64, 256, 4);
            InsertItem(66, "rails", "Rails", 64, 256, 4);
            InsertItem(67, "cobblestonestairs", "Cobblestone Stairs", 64, 256, 4);
            InsertItem(68, "wallsign", "Sign (Wall)", 1, 64, 5);
            InsertItem(69, "lever", "Lever", 1, 64, 4);
            InsertItem(70, "stonepressureplates", "Stone Pressure Plates", 1, 64, 4);
            InsertItem(71, "irondoorblock", "Iron Door (Block)", 1, 64, 6);
            InsertItem(72, "woodenpressureplates", "Wooden Pressure Plates", 1, 64, 4);
            InsertItem(73, "redstoneore", "Redstone (Ore)", 1, 64, 5);
            InsertItem(74, "glowingredstoneore", "Redstone (Glowing Ore)", 1, 64, 5);
            InsertItem(75, "redstonetorchoffstate", "Redstone Torch (\"off\" state)", 1, 64, 6);
            InsertItem(76, "redstonetorch", "Redstone Torch (\"on\" state)", 64, 256, 4);
            InsertItem(77, "stonebutton", "Stone Button", 1, 64, 4);
            InsertItem(78, "snow", "Snow", 64, 256, 4);
            InsertItem(80, "snowblock", "Snow (Block)", 64, 256, 5);
            InsertItem(81, "cactus", "Cactus", 1, 64, 5);
            InsertItem(82, "clayblock", "Clay (Block)", 64, 256, 4);
            InsertItem(83, "sugarcaneblock", "Sugarcane (Block)", 64, 256, 5);
            InsertItem(85, "fence", "Fence", 64, 256, 4);
            InsertItem(86, "pumpkin", "Pumpkin", 1, 64, 4);
            InsertItem(89, "glowstoneblock", "Glowstone (Block)", 64, 256, 5);
            InsertItem(90, "portal", "Portal", 1, 1, 6);
            InsertItem(92, "cakeblock", "Cake (Block)", 64, 256, 6);
            InsertItem(93, "redstonerepeater", "Redstone Repeater (\"off\" state)", 1, 64, 4);
            InsertItem(94, "redstonerepeateron", "Redstone Repeater (\"on\" state)", 1, 64, 6);
            InsertItem(95, "lockedchest", "Locked Chest", 1, 64, 6);
            InsertItem(96, "trapdoor", "Trapdoor", 64, 64, 4);
            InsertItem(98, "stonebricks", "Stone Bricks", 64, 64, 4);
            InsertItem(101, "ironbars", "", 64, 256, 4);
            InsertItem(102, "glasspane", "", 64, 256, 4);
            InsertItem(104, "pumkinstem", "", 1, 64, 5);
            InsertItem(105, "melonstem", "", 1, 64, 5);
            InsertItem(106, "vines", "", 64, 256, 5);
            InsertItem(107, "fencegate", "", 64, 256, 4);
            InsertItem(108, "brickstairs", "Brick Stairs", 64, 256, 4);
            InsertItem(109, "stonebrickstairs", "Stone Brick Stairs", 64, 64, 4);
            InsertItem(115, "netherwartblock", "", 64, 256, 5);
            InsertItem(117, "brewingstandblock", "Brewing Stand (Block)", 1, 64, 6);
            InsertItem(118, "cauldronblock", "Cauldron (Block)", 1, 64, 6);
            InsertItem(119, "endportal", "", 1, 64, 6);
            InsertItem(120, "endportalframe", "", 1, 64, 6);
            InsertItem(256, "ironshovel", "Iron Shovel", 1, 64, 4);
            InsertItem(257, "ironpickaxe", "Iron Pickaxe", 1, 64, 4);
            InsertItem(258, "ironaxe", "Iron Axe", 1, 64, 4);
            InsertItem(259, "flintandsteel", "Flint and Steel", 1, 64, 4);
            InsertItem(260, "apple", "Apple", 1, 64, 4);
            InsertItem(261, "bow", "Bow", 1, 64, 4);
            InsertItem(262, "arrow", "Arrow", 64, 256, 4);
            InsertItem(263, "coal", "Coal", 1, 64, 4);
            InsertItem(264, "diamond", "Diamond (gem)", 64, 256, 4);
            InsertItem(265, "ironingot", "Iron Ingot", 1, 64, 4);
            InsertItem(266, "goldingot", "Gold Ingot", 1, 64, 4);
            InsertItem(267, "ironsword", "Iron Sword", 1, 64, 4);
            InsertItem(268, "planks", "Planks", 1, 64, 4);
            InsertItem(269, "woodenshovel", "Wooden Shovel", 1, 64, 4);
            InsertItem(270, "woodenpickaxe", "Wooden Pickaxe", 1, 64, 4);
            InsertItem(271, "woodenaxe", "Wooden Axe", 1, 64, 4);
            InsertItem(272, "stonesword", "Stone Sword", 1, 64, 4);
            InsertItem(273, "stoneshovel", "Stone Shovel", 1, 64, 4);
            InsertItem(274, "stonepickaxe", "Stone Pickaxe", 1, 64, 4);
            InsertItem(275, "stoneaxe", "Stone Axe", 1, 64, 4);
            InsertItem(276, "diamondsword", "Diamond Sword", 1, 64, 4);
            InsertItem(277, "diamondshovel", "Diamond Shovel", 1, 64, 4);
            InsertItem(278, "diamondpickaxe", "Diamond Pickaxe", 1, 64, 4);
            InsertItem(279, "diamondaxe", "Diamond Axe", 1, 64, 4);
            InsertItem(280, "stick", "Stick", 64, 256, 4);
            InsertItem(281, "bowl", "Bowl", 1, 64, 4);
            InsertItem(282, "mushroomsoup", "Mushroom Soup", 1, 64, 4);
            InsertItem(283, "goldsword", "Gold Sword", 1, 64, 4);
            InsertItem(284, "goldshovel", "Gold Shovel", 1, 64, 4);
            InsertItem(285, "goldpickaxe", "Gold Pickaxe", 1, 64, 4);
            InsertItem(286, "goldaxe", "Gold Axe", 1, 64, 4);
            InsertItem(287, "string", "String", 1, 64, 4);
            InsertItem(288, "feather", "Feather", 1, 64, 4);
            InsertItem(289, "gunpowder", "Gunpowder", 1, 64, 4);
            InsertItem(290, "woodenhoe", "Wooden Hoe", 1, 64, 4);
            InsertItem(291, "stonehoe", "Stone Hoe", 1, 64, 4);
            InsertItem(292, "ironhoe", "Iron Hoe", 1, 64, 4);
            InsertItem(293, "diamondhoe", "Diamond Hoe", 1, 64, 4);
            InsertItem(294, "goldhoe", "Gold Hoe", 1, 64, 4);
            InsertItem(295, "seeds", "Seeds", 1, 64, 4);
            InsertItem(296, "wheat", "Wheat", 1, 64, 4);
            InsertItem(297, "bread", "Bread", 1, 64, 4);
            InsertItem(298, "leatherhelmet", "Leather Helmet", 1, 64, 4);
            InsertItem(299, "leatherchestplate", "Leather Chestplate", 1, 64, 4);
            InsertItem(300, "leatherleggings", "Leather Leggings", 1, 64, 4);
            InsertItem(301, "leatherboots", "Leather Boots", 1, 64, 4);
            InsertItem(302, "chainarmorhelmet", "Chain Armor Helmet", 1, 64, 4);
            InsertItem(303, "chainarmorchestplate", "Chain Armor Chestplate", 1, 64, 4);
            InsertItem(304, "chainarmorleggings", "Chain Armor Leggings", 1, 64, 4);
            InsertItem(305, "chainarmorboots", "Chain Armor Boots", 1, 64, 4);
            InsertItem(306, "ironhelmet", "Iron Helmet", 1, 64, 4);
            InsertItem(307, "ironchestplate", "Iron Chestplate", 1, 64, 4);
            InsertItem(308, "ironleggings", "Iron Leggings", 1, 64, 4);
            InsertItem(309, "ironboots", "Iron Boots", 1, 64, 4);
            InsertItem(310, "diamondhelmet", "Diamond Helmet", 1, 64, 4);
            InsertItem(311, "diamondchestplate", "Diamond Chestplate", 1, 64, 4);
            InsertItem(312, "diamondleggings", "Diamond Leggings", 1, 64, 4);
            InsertItem(313, "diamondboots", "Diamond Boots", 1, 64, 4);
            InsertItem(314, "goldhelmet", "Gold Helmet", 1, 64, 4);
            InsertItem(315, "goldchestplate", "Gold Chestplate", 1, 64, 4);
            InsertItem(316, "goldleggings", "Gold Leggings", 1, 64, 4);
            InsertItem(317, "goldboots", "Gold Boots", 1, 64, 4);
            InsertItem(318, "flint", "Flint", 1, 64, 4);
            InsertItem(319, "rawporkchop", "Raw Porkchop", 1, 64, 4);
            InsertItem(320, "cookedporkchop", "Cooked Porkchop", 1, 64, 4);
            InsertItem(322, "goldenapple", "Golden Apple", 1, 64, 4);
            InsertItem(323, "sign", "Sign", 1, 64, 4);
            InsertItem(324, "woodendoor", "Wooden Door", 1, 64, 4);
            InsertItem(325, "bucket", "Bucket", 1, 64, 4);
            InsertItem(326, "waterbucket", "Water Bucket", 1, 64, 5);
            InsertItem(327, "lavabucket", "Lava Bucket", 1, 64, 5);
            InsertItem(328, "minecart", "Minecart", 1, 64, 4);
            InsertItem(330, "irondoor", "Iron door", 1, 64, 4);
            InsertItem(331, "redstonedust", "Redstone (dust)", 64, 256, 4);
            InsertItem(332, "snowball", "Snowball", 1, 64, 4);
            InsertItem(333, "boat", "Boat", 1, 64, 4);
            InsertItem(334, "leather", "Leather", 1, 64, 4);
            InsertItem(336, "claybrick", "Clay Brick", 1, 64, 4);
            InsertItem(337, "clay", "Clay", 1, 64, 4);
            InsertItem(338, "sugarcane", "Sugar Cane", 1, 64, 4);
            InsertItem(341, "slimeball", "Slimeball", 1, 64, 4);
            InsertItem(342, "storageminecart", "Storage Minecart", 1, 64, 4);
            InsertItem(343, "poweredminecart", "Powered Minecart", 1, 64, 4);
            InsertItem(344, "egg", "Egg", 1, 64, 4);
            InsertItem(345, "compass", "Compass", 1, 64, 4);
            InsertItem(346, "fishingrod", "Fishing Rod", 1, 64, 4);
            InsertItem(348, "glowstonedust", "Glowstone (Dust)", 1, 64, 4);
            InsertItem(349, "fish", "Fish", 1, 64, 4);
            InsertItem(350, "cookedfish", "Cooked Fish", 1, 64, 4);
            InsertItem(351, "dye", "Dye", 1, 64, 4);
            InsertItem(352, "bone", "Bone", 1, 64, 4);
            InsertItem(354, "cake", "Cake", 1, 64, 4);
            InsertItem(355, "bed", "Bed", 1, 64, 4);
            InsertItem(356, "redstonerepeater", "Redstone Repeater", 1, 64, 4);
            InsertItem(357, "cookie", "Cookie", 1, 64, 4);
            InsertItem(359, "shears", "Shears", 64, 256, 4);
            InsertItem(360, "melonslice", "", 64, 256, 4);
            InsertItem(361, "pumpkinseeds", "", 64, 256, 4);
            InsertItem(362, "melonseeds", "", 64, 256, 4);
            InsertItem(363, "rawbeef", "", 64, 256, 4);
            InsertItem(364, "steak", "Steak", 64, 64, 4);
            InsertItem(365, "rawchicken", "", 64, 256, 4);
            InsertItem(366, "cookedchicken", "Cooked Chicken", 64, 256, 4);
            InsertItem(367, "rottenflesh", "", 64, 256, 4);
            InsertItem(368, "enderpearl", "", 64, 256, 4);
            InsertItem(371, "goldnugget", "Gold Nugget", 64, 256, 4);
            InsertItem(373, "potions", "", 1, 64, 6);
            InsertItem(375, "spidereye", "Spider Eye", 64, 256, 4);
            InsertItem(380, "cauldron", "Cauldron", 1, 64, 4);
            InsertItem(381, "eyeofender", "", 1, 64, 5);
            InsertItem(2256, "goldmusicdisc", "Gold Music Disc", 1, 64, 4);
            InsertItem(2257, "greenmusicdisc", "Green Music Disc", 1, 64, 4);
            InsertItem(2258, "blocksdisc", "Blocks Disc", 1, 64, 4);
            InsertItem(2259, "chirpdisc", "Chirp Disc", 1, 64, 4);
            InsertItem(2260, "fardisc", "", 1, 64, 4);
            InsertItem(2261, "malldisc", "Mall Disc", 1, 64, 4);
            InsertItem(2262, "mellohidisc", "Mellohi Disc", 1, 64, 4);
            InsertItem(2263, "staldisc", "Stal Disc", 1, 64, 4);
            InsertItem(2264, "straddisc", "Strad Disc", 64, 64, 4);
            InsertItem(2265, "warddisc", "Ward Disc", 1, 64, 4);
            InsertItem(2266, "11disc", "11 Disc", 1, 64, 4);
            mDb.SubmitChanges();
        }

        private void DoInsertUsers()
        {
            InsertUser("console", mDb.Ranks.First(r => r.Name == "Console"));
            mDb.SubmitChanges();
        }

    }
}
