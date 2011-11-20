using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace EnigmaMM.Engine.Data
{
    class CreateDb: UpdateDb
    {
        public override void DoUpdate()
        {
            using (SqlCeEngine engine = new SqlCeEngine(mConnectionString))
            {
                engine.CreateDatabase();
            }

            mCommandQueue.Clear();

            mCommandQueue.Add(@"
            CREATE TABLE Config (
                [Config_ID] int IDENTITY PRIMARY KEY NOT NULL, 
                [Key] nvarchar(50) NOT NULL, 
                [Value] nvarchar(50) NOT NULL
            )");

            mCommandQueue.Add(@"
            CREATE TABLE ItemHistory
            (
                [ItemHistory_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [Item_ID] int NOT NULL,
                [User_ID] int NOT NULL,
                [Quantity] int NOT NULL DEFAULT 0,
                [CreateDate] datetime NOT NULL
            )");

            mCommandQueue.Add(@"
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

            mCommandQueue.Add(@"
            CREATE TABLE MessageTypes
            (
                [Message_Type_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [Name] nvarchar(20) NOT NULL,
                [Expression] nvarchar(200) NOT NULL,
                [MatchType] nvarchar(10) NOT NULL
            )");

             mCommandQueue.Add(@"
            CREATE TABLE Permissions
            (
                [Permission_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [Min_Level] int NOT NULL,
                [Name] nvarchar(50) NOT NULL
            )");

             mCommandQueue.Add(@"
            CREATE TABLE Ranks
            (
                [Rank_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [Level] int NOT NULL DEFAULT 0,
                [Name] nvarchar(50) NOT NULL
            )");

           mCommandQueue.Add(@"
            CREATE TABLE Tracking
            (
                [Tracking_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [User_ID] int NOT NULL,
                [LocX] int NOT NULL DEFAULT 0,
                [LocY] int NOT NULL DEFAULT 0,
                [LocZ] int NOT NULL DEFAULT 0,
                [PointTime] DateTime 
            )"); 
            
            mCommandQueue.Add(@"
            CREATE TABLE Users
            (
                [User_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [Rank_ID] int NOT NULL,
                [Username] nvarchar(50) NOT NULL,
                [LocX] int NOT NULL DEFAULT 0,
                [LocY] int NOT NULL DEFAULT 0,
                [LocZ] int NOT NULL DEFAULT 0,
                [LastSeen] DateTime
            )");

            mCommandQueue.Add(@"ALTER TABLE ItemHistory ADD CONSTRAINT [FK_ItemHistory_Item] FOREIGN KEY (Item_ID) REFERENCES Items (Item_ID)");
            mCommandQueue.Add(@"ALTER TABLE ItemHistory ADD CONSTRAINT [FK_ItemHistory_User] FOREIGN KEY (User_ID) REFERENCES Users (User_ID)");
            mCommandQueue.Add(@"ALTER TABLE Tracking ADD CONSTRAINT [FK_Tracking_User] FOREIGN KEY (User_ID) REFERENCES Users (User_ID)");
            mCommandQueue.Add(@"ALTER TABLE Users ADD CONSTRAINT [FK_User_Rank] FOREIGN KEY (Rank_ID) REFERENCES Ranks (Rank_ID) ON DELETE CASCADE ON UPDATE CASCADE");

            ExecuteCommands();
        }
    }
}
