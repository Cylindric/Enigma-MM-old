using System;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace EnigmaMM.Engine.Commands
{
    class BackupCommand : Command
    {
        private int mBackupsToKeep = 5;
        private string mWorldPath;

        public BackupCommand()
        {
            mPermissionsRequired.Add(Manager.Database.Permissions.Single(i => i.Name == "backup"));
            Manager.Server.MinecraftSettings.Load();
            mWorldPath = Manager.Server.MinecraftSettings.WorldPath;
        }

        /// <summary>
        /// Perform a backup.
        /// </summary>
        protected override void ExecuteTask(EMMServerMessage command)
        {
            if (CheckRequirements())
            {
                Manager.Server.RaiseServerMessage("Backing up...");
                PerformBackup();
                Manager.Server.RaiseServerMessage("Backup complete.");
            }
        }

        /// <summary>
        /// Perform environment checks to make sure backups are reaady to run.
        /// </summary>
        /// <returns>True if system is ready; else false.</returns>
        private bool CheckRequirements()
        {
            EMMServer server = Manager.Server;
            bool status = true;
            if (!Directory.Exists(server.Settings.BackupRoot))
            {
                server.RaiseServerMessage(string.Format("ERROR: Specified backup location doesn't exist! {0}", server.Settings.BackupRoot));
                status = false;
            }
            return status;
        }

        private void PerformBackup()
        {
            Manager.Server.BlockAutoSave();
            RotateFiles();
            BackupFiles();
            Manager.Server.UnblockAutoSave();
        }

        private void BackupFiles()
        {
            EMMServer server = Manager.Server;
            string backupFile = Path.Combine(server.Settings.BackupRoot, string.Format("backup-{0:yyyyMMdd-HHmmss}.zip", DateTime.Now));
            using (ZipFile zip = new ZipFile())
            {
                zip.AddSelectedFiles("*.txt", server.Settings.MinecraftRoot, @"minecraft");
                zip.AddSelectedFiles("*.jar", server.Settings.MinecraftRoot, @"minecraft");
                zip.AddSelectedFiles("*.properties", server.Settings.MinecraftRoot, @"minecraft");
                zip.AddDirectory(mWorldPath, @"minecraft\" + Path.GetFileName(mWorldPath));
                try
                {
                    zip.Save(backupFile);
                }
                catch (Exception e)
                {
                    server.RaiseServerMessage(string.Format("ERROR: Unable to save backup! {0}", e.Message));
                }
            }
        }

        private void RotateFiles()
        {            
            // Get a list of current backups, sorted by created-date
            EMMServer server = Manager.Server;
            string[] fileNames = Directory.GetFiles(server.Settings.BackupRoot, "*.zip");
            DateTime[] creationTimes = new DateTime[fileNames.Length];
            for (int i = 0; i < fileNames.Length; i++)
            {
                creationTimes[i] = new FileInfo(fileNames[i]).CreationTime;
            }
            Array.Sort(creationTimes, fileNames);

            // Delete any older ones needed to keep the number to the configured value
            for (int i = 0; i < fileNames.Length - (mBackupsToKeep - 1); i++)
            {
                File.Delete(fileNames[i]);
            }
        }

    }
}
