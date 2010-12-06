using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace EnigmaMM
{
    class Overviewer : Mapper
    {
        public Overviewer(MCServer server) : base(server, "overviewer")
        {
            mExePath = Path.Combine(Settings.OverviewerRoot, "gmap.exe");
            mOutputPath = Path.Combine(Settings.MapRoot, "Overviewer");
        }

        public override void Render(string type)
        {
            RenderMap();
        }

        public override void RenderMap()
        {
            base.RenderMap();
            mMinecraft.RaiseServerMessage("OVERVIEWER: Creating map");

            if (!Directory.Exists(Settings.CacheRoot))
            {
                throw new DirectoryNotFoundException("Cache path missing: " + Settings.CacheRoot);
            }
            if (!Directory.Exists(mCachePath))
            {
                Directory.CreateDirectory(mCachePath);
            }

            string cmd = string.Format(
                "-p 1 --cachedir \"{1}\" \"{2}\" \"{3}\"",
                mCachePath, mMinecraft.ServerProperties.WorldPath, mOutputPath
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            CreateMarkersFromWarpLocations();

            mMinecraft.RaiseServerMessage("OVERVIEWER: Done.");
        }


        private void CreateMarkersFromWarpLocations()
        {
            StringBuilder markers = new StringBuilder();
            markers.Append("var markerData=[\n");
            foreach (KeyValuePair<string, string> kvp in mMinecraft.ServerWarps.Values)
            {
                string[] warp = kvp.Value.Split(':');

                string name = kvp.Key;
                float x = 0f;
                float y = 0f;
                float z = 0f;
                string a = "";
                string b = "";
                string group = "";

                float.TryParse(warp[0], out x);
                float.TryParse(warp[1], out y);
                float.TryParse(warp[2], out z);
                a = warp[3];
                b = warp[4];
                group = warp[5];

                if ((group == "") || (group == "users"))
                {
                    markers.Append("    {");
                    markers.AppendFormat("\"msg\": \"{0}\", ", name);
                    markers.AppendFormat("\"y\": {0:f0}, \"z\": {1:f0}, \"x\": {2:f0}", y, z, x);
                    markers.Append("}, \n");
                }
            }
            markers.Append("];\n");

            StreamWriter sw = File.CreateText(Path.Combine(mOutputPath, "markers.js"));
            sw.Write(markers.ToString());
            sw.Close();
        }

    }
}