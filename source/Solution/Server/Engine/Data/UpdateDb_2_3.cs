using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine.Data
{
    class UpdateDb_2_3: UpdateDb
    {
        public override void DoUpdate()
        {
            mCommandQueue.Clear();
            mCommandQueue.Add(@"
            CREATE TABLE Tracking
            (
                [Tracking_ID] int IDENTITY PRIMARY KEY NOT NULL,
                [User_ID] int NOT NULL,
                [LocX] Float NOT NULL DEFAULT 0,
                [LocY] Float NOT NULL DEFAULT 0,
                [LocZ] Float NOT NULL DEFAULT 0,
                [PointTime] DateTime
            )");

            mCommandQueue.Add(@"ALTER TABLE Tracking ADD CONSTRAINT [FK_Tracking_User] FOREIGN KEY (User_ID) REFERENCES Users (User_ID)");
            ExecuteCommands();
            mDb.Configs.First(c => c.Key == "db_version").Value = "3";
        }
    }
}
