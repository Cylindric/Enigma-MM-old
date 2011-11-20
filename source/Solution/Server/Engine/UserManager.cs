using System;
using System.IO;
using LibNbt.Tags;
using System.Linq;
using System.Diagnostics;

namespace EnigmaMM.Engine
{
    class UserManager
    {
        private EMMServer mServer;

        public UserManager(EMMServer server)
        {
            mServer = server;
        }

        public Data.User OnUserJoinedMessage(EMMServerMessage message)
        {
            // make sure user exists in database
            Data.User user = Manager.Database.Users.SingleOrDefault(i => i.Username == message.Data["username"]);
            if (user == null)
            {
                user = CreateDefaultUser(message.Data["username"]);
            }

            // parse their current location from the join message
            Coord position = new Coord(message);
            user.LocX = position.X;
            user.LocY = position.Y;
            user.LocZ = position.Z;
            user.LastSeen = DateTime.Now;
            Manager.Database.SubmitChanges();

            // save the current location to the tracking table
            TrackUserPosition(user);

            return user;
        }

        public void UpdateAllPositionsFromFile()
        {
            string playerpath = Path.Combine(mServer.MinecraftSettings.WorldPath, "players");
            if (!Directory.Exists(playerpath))
            {
                return;
            }
            foreach (string filename in Directory.GetFiles(playerpath, "*.dat"))
            {
                UpdatePositionFromFile(filename);
            }
        }

        public void MonitorUserFiles()
        {
            string playerpath = Path.Combine(mServer.MinecraftSettings.WorldPath, "players");
            if (!Directory.Exists(playerpath))
            {
                return;
            }
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = playerpath;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            watcher.Filter = "*.dat";
            watcher.Created += new FileSystemEventHandler(OnPlayerFileChanged);
            watcher.Changed += new FileSystemEventHandler(OnPlayerFileChanged);
            watcher.Deleted += new FileSystemEventHandler(OnPlayerFileChanged);
            watcher.EnableRaisingEvents = true;
        }

        private Data.User CreateDefaultUser(string username)
        {
            Data.User user = new Data.User();
            user.Username = username;
            user.Rank = Manager.Database.Ranks.Single(rank => rank.Name == "Everyone");
            Manager.Database.Users.InsertOnSubmit(user);
            Manager.Database.SubmitChanges();
            return user;
        }

        private void TrackUserPosition(Data.User user)
        {
            TrackUserPosition(user.User_ID, user.LocX, user.LocY, user.LocZ, DateTime.Now);
        }

        private void TrackUserPosition(int user_id, int x, int y, int z, DateTime time)
        {
            Data.Tracking lasttrack = Manager.Database.Trackings.OrderByDescending(t => t.PointTime).FirstOrDefault(i => i.User_ID == user_id);
            bool addTrack = false;
            if (lasttrack == null)
            {
                addTrack = true;
            }
            else
            {
                if ((lasttrack.LocX != x) || (lasttrack.LocY != y) || (lasttrack.LocZ != z))
                {
                    addTrack = true;
                }
            }
            if (addTrack)
            {
                Manager.Database.Trackings.InsertOnSubmit(new Data.Tracking() { User_ID = user_id, LocX = x, LocY = y, LocZ = z, PointTime = time });
                Manager.Database.SubmitChanges();
            }
        }

        private void UpdatePositionFromFile(string filename)
        {
            Coord coord = GetPositionFromFile(filename);
            string username = GetUsernameFromFile(filename);
            DateTime time = File.GetLastWriteTime(filename);
            Data.User user = Manager.Database.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                // file does not correspond to an existing user in the database
                user = CreateDefaultUser(username);
            }
            TrackUserPosition(user.User_ID, coord.X, coord.Y, coord.Z, time);
        }

        private void OnPlayerFileChanged(object source, FileSystemEventArgs e)
        {
            if (Path.GetFileName(e.FullPath) != "_tmp_.dat")
            {
                //UpdatePositionFromFile(e.FullPath);
            }
        }

        private Coord GetPositionFromFile(Data.User user)
        {
            return GetPositionFromFile(Path.Combine(mServer.MinecraftSettings.WorldPath, "players", user.Username + ".dat"));
        }

        private Coord GetPositionFromFile(string filename)
        {
            using (LibNbt.NbtFile userfile = new LibNbt.NbtFile(filename))
            {
                userfile.LoadFile();
                Coord position = new Coord(userfile.Query("//Pos/0"), userfile.Query("//Pos/1"), userfile.Query("//Pos/2"));
                return position;
            }
        }

        /// <summary>
        /// Returns the username from the data file
        /// </summary>
        /// <remarks>At the moment there doesn't seem to be any cleverer way than from the filename</remarks>
        /// <param name="filename">Full filename to the player.dat file</param>
        /// <returns>The username</returns>
        private string GetUsernameFromFile(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

    }
}
