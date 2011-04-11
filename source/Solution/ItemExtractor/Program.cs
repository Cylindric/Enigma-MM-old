using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM;
using EnigmaMM.Data;
using System.Data.Linq;

namespace ItemExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            EMMServer server = new EMMServer();
            EnigmaMM.Data.EMMDataContext db = server.Database;

            // Delete all items!
            db.Items.DeleteAllOnSubmit(db.Items);
            db.SubmitChanges();

            // Add in all the new items
            Item item = new Item();
            item.Code = "dirt";
            item.Name = "Dirt Block";
            item.Max = 256;
            item.Stack_Size = 64;
            db.Items.InsertOnSubmit(item);

            db.SubmitChanges();            
        }
    }
}
