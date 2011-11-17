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
                    [Name] nvarchar(20) NOT NULL,
                    [Expression] nvarchar(200) NOT NULL,
                    [MatchType] nvarchar(10) NOT NULL
                )");

                commands.Add(@"
                CREATE TABLE Ranks
                (
                    [Rank_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Level] int NOT NULL DEFAULT 0,
                    [Name] nvarchar(50) NOT NULL
                )");

                commands.Add(@"
                CREATE TABLE Items
                (
                    [Item_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Code] nvarchar(50) NOT NULL,
                    [Block_Decimal_ID] int NOT NULL,
                    [Name] nvarchar(50) NOT NULL,
                    [Stack_Size] int NOT NULL DEFAULT 1,
                    [Max] int NOT NULL DEFAULT 64,
                    [Min_Level] int NOT NULL DEFAULT 1
                )");

                commands.Add(@"
                CREATE TABLE Users
                (
                    [User_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Rank_ID] int NOT NULL,
                    [Username] nvarchar(50) NOT NULL,
                    [LocX] Float NOT NULL DEFAULT 0,
                    [LocY] Float NOT NULL DEFAULT 0,
                    [LocZ] Float NOT NULL DEFAULT 0,
                    [LastSeen] DateTime
                )");

                commands.Add(@"
                CREATE TABLE ItemHistory
                (
                    [ItemHistory_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Item_ID] int NOT NULL,
                    [User_ID] int NOT NULL,
                    [Quantity] int NOT NULL DEFAULT 0,
                    [CreateDate] datetime NOT NULL
                )");

                commands.Add(@"
                CREATE TABLE Permissions
                (
                    [Permission_ID] int IDENTITY PRIMARY KEY NOT NULL,
                    [Min_Level] int NOT NULL,
                    [Name] nvarchar(50) NOT NULL
                )");

                commands.Add(@"ALTER TABLE ItemHistory ADD CONSTRAINT [FK_ItemHistory_Item] FOREIGN KEY (Item_ID) REFERENCES Items (Item_ID)");
                commands.Add(@"ALTER TABLE ItemHistory ADD CONSTRAINT [FK_ItemHistory_User] FOREIGN KEY (User_ID) REFERENCES Users (User_ID)");
                commands.Add(@"ALTER TABLE Users ADD CONSTRAINT [FK_User_Rank] FOREIGN KEY (Rank_ID) REFERENCES Ranks (Rank_ID) ON DELETE CASCADE ON UPDATE CASCADE");

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
