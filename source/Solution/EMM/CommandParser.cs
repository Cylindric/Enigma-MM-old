using System;
using EnigmaMM.Interfaces;
using System.Collections.Generic;
using System.Xml;

namespace EnigmaMM
{
    /// <summary>
    /// The CommandParser is a simple tool for converting user input into Server Manager or
    /// Minecraft commands.
    /// </summary>
    /// <remarks>Any unrecognised commands are passed up to Minecraft to process directly.</remarks>
    public class CommandParser
    {
        private IServer mMinecraft;
        private static List<Item> sItems = new List<Item>();

        private class Item
        {
            private string mItemCode;
            private int mItemId;
            private int mItemQty;
            private int mMaxQty;

            public string Code
            {
                get { return mItemCode; }
            }

            public int Id
            {
                get { return mItemId; }
            }

            public int Quantity
            {
                get { return mItemQty; }
            }
            public int MaxQuantity
            {
                get { return mMaxQty; }
            }

            public Item(string id, string qty, string max, string code)
            {
                mItemId = int.Parse(id);
                mItemQty = int.Parse(qty);
                mMaxQty = int.Parse(max);
                mItemCode = code;
            }

        }
        
        /// <summary>
        /// Creates a new <c>CommandParser</c> and connects it to the specified <see cref="IServer"/>.
        /// </summary>
        /// <param name="minecraft"></param>
        public CommandParser(IServer minecraft)
        {
            mMinecraft = minecraft;
        }


        /// <summary>
        /// Handle a command from the CLI.
        /// Commands for the server manager are prefixed with the command-character.
        /// </summary>
        /// <param name="Command">The command to parse.</param>
        public bool ParseCommand(String Command)
        {
            bool executed = true;
            string[] args = Command.Trim().Replace("  ", " ").Split(' ');
            switch (args[0])
            {
                case ("start"):
                    mMinecraft.StartServer();
                    break;

                case ("restart"):
                    mMinecraft.RestartServer(true);
                    break;

                case ("restart now"):
                    ParseCommand("stop");
                    mMinecraft.StartServer();
                    break;

                case ("stop"):
                    mMinecraft.StopServer(true);
                    break;

                case ("stop now"):
                    mMinecraft.StopServer(false);
                    break;

                case ("abort"):
                    mMinecraft.AbortPendingOperations();
                    break;

                case ("maps"):
                    mMinecraft.GenerateMaps(args);
                    break;

                case ("backup"):
                    mMinecraft.Backup();
                    break;

                case ("get"):
                    ParseServerCommand(args);
                    break;

                case ("sys.importitems"):
                    mMinecraft.System_ImportItems();
                    break;

                default:
                    executed = false;
                    break;
            }
            return executed;
        }

        public static void PopulateItems(string fileName)
        {
            sItems = new List<Item>();

            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            XmlNodeList nodeList = xml.DocumentElement.SelectNodes("/items/item");
            foreach (XmlNode message in nodeList)
            {
                XmlNode codeNode = message.SelectSingleNode("code");
                XmlNode qtyNode = message.SelectSingleNode("quantity");
                XmlNode maxNode = message.SelectSingleNode("max");
                XmlNode idNode = message.SelectSingleNode("id");
                Item p = new Item(idNode.InnerText, qtyNode.InnerText, maxNode.InnerText, codeNode.InnerText);
                sItems.Add(p);
            }
        }

        public void ParseServerCommand(string[] args)
        {
            string username = args[args.Length-1];
            string command = args[0];

            if (command.Equals("get"))
            {
                switch (args.Length)
                {
                    case 3:
                        ParseRequestCommand(username, args[1]);
                        break;
                    case 4:
                        ParseRequestCommand(username, args[1], args[2]);
                        break;
                }
            }
        }

        private void ParseRequestCommand(string username, string item, string quantity = null)
        {
            int finalQuantity = 0;
            int finalItemId = 0;
         
            Item foundItem = sItems.Find(i => i.Code == item);
            if (foundItem != null)
            {
                finalItemId = foundItem.Id;
                int.TryParse(quantity, out finalQuantity);
                if (quantity == null)
                {
                    finalQuantity = foundItem.Quantity;
                }
                finalQuantity = Math.Min(finalQuantity, foundItem.MaxQuantity);
            }
            
            mMinecraft.GiveItem(username, finalItemId, finalQuantity);
        }

    }

}
