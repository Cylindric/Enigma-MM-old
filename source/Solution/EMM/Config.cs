using System;
using System.Data;
using System.IO;

namespace EnigmaMM
{
    // Class for managing configuration persistence
    // This is based almost entirely on the work done by Alberto Venditti at http://www.codeproject.com/KB/vb/ConfigOpt.aspx 
    public class Config
    {

        // This DataSet is used as a memory data
        // structure to hold config key/value pairs
        // Inside this DataSet, a single DataTable named ConfigValues is created
        private static DataSet DSoptions;

        // This is the filename for the DataSet XML serialization
        private static string mConfigFileName;
        
        // This property is read-only, because it is set
        // through Initialize or Store methods
        public static string ConfigFileName
        {
            get { return mConfigFileName; }
        }

        // This method has to be invoked before using
        // any other method of ConfigOpt class
        // ConfigFile parameter is the name of the config file to be read
        // (if that file doesn't exists, the method
        // simply initialize the data structure
        // and the ConfigFileName property)
        public static void Initialize(string ConfigFile)
        {
            mConfigFileName = ConfigFile;
            DSoptions = new DataSet("ConfigOpt");
            if (File.Exists(ConfigFile))
            {
                // If the specified config file exists,
                // it is read to populate the DataSet
                DSoptions.ReadXml(ConfigFile);
            }
            else
            {
                // If the specified config file doesn't exists, 
                // the DataSet is simply initialized (and left empty):
                // the ConfigValues DataTable is created
                // with two fields (to hold key/values pairs)
                DataTable dt = new DataTable("ConfigValues");
                dt.Columns.Add("OptionName", System.Type.GetType("System.String"));
                dt.Columns.Add("OptionValue", System.Type.GetType("System.String"));
                DSoptions.Tables.Add(dt);
            }
        }

        // This method serializes the memory data
        // structure holding the config parameters
        // The filename used is the one defined calling Initialize method
        public static void Store()
        {
            Store(mConfigFileName);
        }

        // Same as Store() method, but with the ability
        // to serialize on a different filename
        public static void Store(string ConfigFile)
        {
            mConfigFileName = ConfigFile;
            DSoptions.WriteXml(ConfigFile);
        }

        // Read a configuration Value (aka OptionValue),
        // given its Key (aka OptionName)
        // If the Key is not defined, an empty string is returned
        public static string GetOption(string OptionName)
        {
            DataView dv = DSoptions.Tables["ConfigValues"].DefaultView;
            dv.RowFilter = "OptionName='" + OptionName + "'";
            if (dv.Count > 0)
            {
                return Convert.ToString(dv[0]["OptionValue"]);
            }
            else
            {
                return "";
            }
        }

        // Write in the memory data structure a Key/Value
        // pair for a configuration setting
        // If the Key already exists, the Value is simply updated,
        // else the Key/Value pair is added
        // Warning: to update the written Key/Value pair
        // on the config file, you need to call Store
        public static void SetOption(string OptionName, string OptionValue)
        {
            DataView dv = DSoptions.Tables["ConfigValues"].DefaultView;
            dv.RowFilter = "OptionName='" + OptionName + "'";
            if (dv.Count > 0)
            {
                dv[0]["OptionValue"] = OptionValue;
            }
            else
            {
                DataRow dr = DSoptions.Tables["ConfigValues"].NewRow();
                dr["OptionName"] = OptionName;
                dr["OptionValue"] = OptionValue;
                DSoptions.Tables["ConfigValues"].Rows.Add(dr);
            }
        }


        // Some strongly-typed "well-known" options
        public static string ServerRoot {
            get { return GetOption("ServerRoot"); }
        }
        public static string JavaExec {
            get { return GetOption("JavaExec"); }
        }
        public static string ServerJar {
            get { return GetOption("ServerJar"); }
        }
        public static int JavaHeapInit {
            get { return int.Parse(GetOption("JavaHeapInit")); }
        }
        public static int JavaHeapMax {
            get { return int.Parse(GetOption("JavaHeapMax")); }
        }

    }

}