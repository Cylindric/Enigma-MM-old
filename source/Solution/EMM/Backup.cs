using System.IO;
using Ionic.Zip;
using System;

namespace EnigmaMM
{
    class Backup : IDisposable
    {
        private MCServer mMinecraft;
        private int BackupsToKeep = 5;

        public Backup(MCServer server)
        {
            mMinecraft = server;
        }

        public void Dispose() {
            mMinecraft = null;
        }

        public void PerformBackup()
        {
            mMinecraft.ServerProperties.Load();
            if (!Directory.Exists(Settings.BackupRoot))
            {
                mMinecraft.RaiseServerMessage(string.Format("ERROR: Specified backup location doesn't exist! {0}", Settings.BackupRoot));
                return;
            }

            string backupFile = Path.Combine(Settings.BackupRoot, string.Format("backup-{0:yyyyMMdd-HHmmss}.zip", DateTime.Now));

            RotateFiles();
            using (ZipFile zip = new ZipFile())
            {
                //zip.AddDirectory(Settings.MinecraftRoot, "Minecraft");
                zip.AddSelectedFiles("*.txt", Settings.MinecraftRoot, @"minecraft");
                zip.AddSelectedFiles("*.jar", Settings.MinecraftRoot, @"minecraft");
                zip.AddSelectedFiles("*.properties", Settings.MinecraftRoot, @"minecraft");
                zip.AddDirectory(mMinecraft.ServerProperties.WorldPath, @"minecraft\" + Path.GetFileName(mMinecraft.ServerProperties.WorldPath));
                try
                {
                    zip.Save(backupFile);
                }
                catch (Exception e)
                {
                    mMinecraft.RaiseServerMessage(string.Format("ERROR: Unable to save backup! {0}", e.Message));
                }
            }
        }


        private void RotateFiles()
        {
            // Get a list of current backups, sorted by created-date
            string[] fileNames = Directory.GetFiles(Settings.BackupRoot, "*.zip");
            DateTime[] creationTimes = new DateTime[fileNames.Length];
            for (int i = 0; i < fileNames.Length; i++)
            {
                creationTimes[i] = new FileInfo(fileNames[i]).CreationTime;
            }
            Array.Sort(creationTimes, fileNames);

            // Delete any older ones needed to keep the number to the configured value
            for (int i = 0; i < fileNames.Length - (BackupsToKeep - 1); i++)
            {
                File.Delete(fileNames[i]);
            }
        }

    }
}
