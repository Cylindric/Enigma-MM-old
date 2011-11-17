using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace EnigmaMM.Engine.Data
{
    class CreateDb
    {
        public static void DoCreate(string datafile)
        {
            string constring = string.Format("Data Source = \"{0}\"", datafile);

            using (SqlCeEngine engine = new SqlCeEngine(constring))
            {
                engine.CreateDatabase();
            }

            using (SqlCeConnection con = new SqlCeConnection(constring))
            {
                con.Open();

                List<string> commands = new List<string>();

                commands.Add(@"
                CREATE TABLE Config (
                    [Config_ID] int IDENTITY PRIMARY KEY NOT NULL, 
                    [Key] nvarchar(50) NOT NULL, 
                    [Value] nvarchar(50) NOT NULL
                )");

                commands.Add(@"
                CREATE TABLE MessageTypes
                (
                    [Message_Type_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Name] nvarchar(50) NOT NULL,
                    [Expression] nvarchar(50) NOT NULL
                )");

                commands.Add(@"
                CREATE TABLE Ranks
                (
                    [Rank_ID] int PRIMARY KEY NOT NULL,
                    [Name] nvarchar(50) NOT NULL
                )");

                commands.Add(@"
                CREATE TABLE Items
                (
                    [Item_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Code] nvarchar(50) NOT NULL,
                    [Block_ID] int NOT NULL,
                    [Name] nvarchar(50) NOT NULL,
                    [Stack_Size] int NOT NULL DEFAULT 1,
                    [Max] int NOT NULL DEFAULT 64,
                    [Min_Rank_ID] int NOT NULL DEFAULT 1,
                    CONSTRAINT [FK_Rank] FOREIGN KEY (Min_Rank_ID) REFERENCES Ranks (Rank_ID) ON DELETE CASCADE ON UPDATE CASCADE
                )");

                commands.Add(@"
                CREATE TABLE Users
                (
                    [User_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Rank_ID] int NOT NULL,
                    [Username] nvarchar(50) NOT NULL,
                    CONSTRAINT [FK_Rank] FOREIGN KEY (Rank_ID) REFERENCES Ranks (Rank_ID) ON DELETE CASCADE ON UPDATE CASCADE
                )");

                commands.Add(@"
                CREATE TABLE ItemHistory
                (
                    [ItemHistory_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Item_ID] int NOT NULL,
                    [User_ID] int NOT NULL,
                    [Quantity] int NOT NULL DEFAULT 0,
                    [CreateDate] datetime NOT NULL,
                    CONSTRAINT [FK_Item] FOREIGN KEY (Item_ID) REFERENCES Items (Item_ID),
                    CONSTRAINT [FK_User] FOREIGN KEY (User_ID) REFERENCES Users (User_ID)
                )");

                foreach (string sql in commands)
                {
                    using (SqlCeCommand cmd = new SqlCeCommand(sql, con))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
    }
}
