using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EnigmaMM.Commands
{
    class ItemExtractor: Command
    {
        private const string WIKI_FILE = "items.wiki.txt";
        private const int DEFAULT_STACK_SIZE = 1;
        private const int DEFAULT_MAX = 64;
        private const int BLOCK_STACK_SIZE = 64;
        private const int BLOCK_MAX = 256;


        private Dictionary<int, string> mBlackListItems = new Dictionary<int, string>();
        private List<string> mBlockItems = new List<string>();

        protected override void ExecuteTask(EMMServerMessage servermessage)
        {
            InitialiseBlackList();
            InitialiseBlockList();

            EnigmaMM.Data.EMMDataContext db = Manager.Database;

            Console.WriteLine("Using database {0}", db.Connection.Database);

            System.IO.StreamReader wiki = new System.IO.StreamReader(WIKI_FILE);
            Boolean blocksFound = false;
            while (!wiki.EndOfStream == true)
            {
                string line = wiki.ReadLine();
                if (line.Contains("== Block IDs =="))
                {
                    blocksFound = true;
                    break;
                }
            }

            if (blocksFound)
            {
                // Delete all items!
                db.Items.DeleteAllOnSubmit(db.Items);
                db.SubmitChanges();

                while (!wiki.EndOfStream == true)
                {
                    string line = wiki.ReadLine();                  
                    if (line.Contains("== Inventory Slot Number =="))
                    {
                        break;
                    }

                    int id = 0;
                    string name = "";
                    string code = "";
                    int max = DEFAULT_MAX;
                    int stack = DEFAULT_STACK_SIZE;

                    string regex = @"^\|\ .+?\|\ .+?\ \|\|\ (?<DecValue>\d+)\ \|\|\ (?<HexValue>\w+?)\ \|\|\ (?<ItemName>.+?)$";
                    MatchCollection matches = Regex.Matches(line, regex);
                    foreach (Match match in matches)
                    {
                        int.TryParse(CleanString(match.Groups["DecValue"].Value), out id);
                        name = CleanString(match.Groups["ItemName"].Value);
                        code = CodifyName(name);

                        if (IsBlock(code))
                        {
                            max = BLOCK_MAX;
                            stack = BLOCK_STACK_SIZE;
                        }

                        // Set any forbidden items to zero qty and max
                        if (mBlackListItems.ContainsKey(id))
                        {
                            stack = 0;
                            max = 0;
                        }

                        if (id > 0)
                        {
                            Data.Item item = new Data.Item();
                            item.Name = name;
                            item.Code = code;
                            item.Max = max;
                            item.Stack_Size = stack;
                            item.Block_Decimal_ID = id;
                            item.Min_Rank_ID = 1;
                            db.Items.InsertOnSubmit(item);
                            Console.WriteLine(string.Format("{0}: {1}", item.Block_Decimal_ID, item.Name));
                        }

                    }
                }
            }
            db.SubmitChanges();
        }

        private void InitialiseBlockList()
        {
            mBlockItems.Add("arrow");
            mBlockItems.Add("bedrock");
            mBlockItems.Add("coalore");
            mBlockItems.Add("cobblestone");
            mBlockItems.Add("cobblestonestairs");
            mBlockItems.Add("diamondore");
            mBlockItems.Add("dirt");
            mBlockItems.Add("doubleslabs");
            mBlockItems.Add("farmland");
            mBlockItems.Add("fence");
            mBlockItems.Add("glass");
            mBlockItems.Add("glowingredstoneore");
            mBlockItems.Add("goldore");
            mBlockItems.Add("grass");
            mBlockItems.Add("gravel");
            mBlockItems.Add("ice");
            mBlockItems.Add("ironore");
            mBlockItems.Add("ladders");
            mBlockItems.Add("lapislazuliore");
            mBlockItems.Add("lava");
            mBlockItems.Add("leaves");
            mBlockItems.Add("mossstone");
            mBlockItems.Add("netherrack");
            mBlockItems.Add("obsidian");
            mBlockItems.Add("rails");
            mBlockItems.Add("redstonedust");
            mBlockItems.Add("redstoneore");
            mBlockItems.Add("sand");
            mBlockItems.Add("sandstone");
            mBlockItems.Add("slabs");
            mBlockItems.Add("snow");
            mBlockItems.Add("soulsand");
            mBlockItems.Add("sponge");
            mBlockItems.Add("stationarylava");
            mBlockItems.Add("stationarywater");
            mBlockItems.Add("stick");
            mBlockItems.Add("stone");
            mBlockItems.Add("tnt");
            mBlockItems.Add("torch");
            mBlockItems.Add("water");
            mBlockItems.Add("wood");
            mBlockItems.Add("woodenplank");
            mBlockItems.Add("woodenstairs");
            mBlockItems.Add("wool");
        }

        private void InitialiseBlackList()
        {
            mBlackListItems.Add(0, "air");
            mBlackListItems.Add(7, "bedrock");
            mBlackListItems.Add(8, "water");
            mBlackListItems.Add(9, "stationarywater");
            mBlackListItems.Add(10, "lava");
            mBlackListItems.Add(11, "stationarylava");
            mBlackListItems.Add(26, "bed");
            mBlackListItems.Add(43, "double slab");
            mBlackListItems.Add(46, "tnt");
            mBlackListItems.Add(51, "fire");
            mBlackListItems.Add(52, "monster spawner");
            mBlackListItems.Add(62, "burning furnace");
            mBlackListItems.Add(74, "redstone ore (glowing)");
            mBlackListItems.Add(76, "redstone torch (on)");
            mBlackListItems.Add(90, "portal");
            mBlackListItems.Add(94, "redstone repeater (on)");
            mBlackListItems.Add(95, "locked chest");
        }

        private string CleanString(string input)
        {
            string output = input.Trim();
            output = output.Replace("[[", "");
            output = output.Replace("]]", "");
            output = output.Replace("<span style='color:red'>", "");
            output = output.Replace("<span style='color:green'>", "");
            output = output.Replace("</span>", "");
            if (output.ToLower().Contains("redstone torch ("))
            {
                output = output.Substring(output.IndexOf("|") + 1);
            }

            if (output.Contains("<sup"))
            {
                output = output.Substring(0, output.IndexOf("<sup"));
            }
            if (output.Contains("#"))
            {
                if (output.ToLower().Contains("mushroom") || output.ToLower().Contains("flowers"))
                {
                    output = output.Substring(output.IndexOf("#") + 1);
                }
                else
                {
                    output = output.Substring(0, output.IndexOf("#"));
                }
            }
            if (output.Contains("|"))
            {
                output = output.Substring(0, output.IndexOf("|"));
            }
            output = output.Trim();
            return output;
        }

        private string CodifyName(string input)
        {
            string code = input.Trim();
            code = code.ToLower();
            code = code.Replace(" ", "");
            code = code.Replace("(", "");
            code = code.Replace(")", "");
            code = code.Replace("\"", "");
            return code;
        }

        private Boolean IsBlock(string code)
        {
            if (code.EndsWith("block"))
            {
                return true;
            }

            if (mBlockItems.Contains(code))
            {
                return true;
            }
            return false;
        }

    }
}
