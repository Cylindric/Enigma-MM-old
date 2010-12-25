using System.IO;
using Ionic.Zip;
using System;
using EnigmaMM.Interfaces;
using System.Threading;

namespace EnigmaMM
{
    class Backup : IDisposable
    {
        private IServer mServer;
        private int mBackupsToKeep = 5;
        private string mWorldPath;

        public Backup(IServer server)
        {
            mServer = server;
            mServer.MinecraftSettings.Load();
            mWorldPath = mServer.MinecraftSettings.WorldPath;
        }

        public void Dispose() {
            mServer = null;
        }

        /// <summary>
        /// Perform environment checks to make sure backups are reaady to run.
        /// </summary>
        /// <returns>True if system is ready; else false.</returns>
        public bool CheckRequirements()
        {
            bool status = true;
            if (!Directory.Exists(mServer.Settings.BackupRoot))
            {
                mServer.RaiseServerMessage(string.Format("ERROR: Specified backup location doesn't exist! {0}", mServer.Settings.BackupRoot));
                status = false;
            }
            return status;
        }

        /// <summary>
        /// Perform a backup.
        /// </summary>
        public void PerformBackup()
        {
            mServer.BlockAutoSave();
            RotateFiles();
            BackupFiles();
            mServer.UnblockAutoSave();
        }

        private void BackupFiles()
        {
            string backupFile = Path.Combine(mServer.Settings.BackupRoot, string.Format("backup-{0:yyyyMMdd-HHmmss}.zip", DateTime.Now));
            using (ZipFile zip = new ZipFile())
            {
                zip.AddSelectedFiles("*.txt", mServer.Settings.MinecraftRoot, @"minecraft");
                zip.AddSelectedFiles("*.jar", mServer.Settings.MinecraftRoot, @"minecraft");
                zip.AddSelectedFiles("*.properties", mServer.Settings.MinecraftRoot, @"minecraft");
                zip.AddDirectory(mWorldPath, @"minecraft\" + Path.GetFileName(mWorldPath));
                try
                {
                    zip.Save(backupFile);
                }
                catch (Exception e)
                {
                    mServer.RaiseServerMessage(string.Format("ERROR: Unable to save backup! {0}", e.Message));
                }
            }
            mServer.RaiseServerMessage("Backup complete.");
        }

        private void RotateFiles()
        {
            // Get a list of current backups, sorted by created-date
            string[] fileNames = Directory.GetFiles(mServer.Settings.BackupRoot, "*.zip");
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
