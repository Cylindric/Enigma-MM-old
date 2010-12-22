using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Mappers
{
    static class MapManager
    {
        static private Dictionary<string, IMapper> Mappers = new Dictionary<string, IMapper>();
        
        static public void Register(string tag, IMapper mapper)
        {
            Mappers.Add(tag, mapper);
        }

        static public void RenderMaps(string[] args)
        {
            string tag = "all";
            string type = "main";

            if (args.Length > 1)
            {
                tag = args[1];
            }
            if (args.Length > 2)
            {
                type = args[2];
            }
            if (tag == "all")
            {
                foreach (IMapper mapper in Mappers.Values)
                {
                    mapper.Render(type);
                }
            }
            else
            {
                if (Mappers.ContainsKey(tag))
                {
                    Mappers[tag].Render(type);
                }
            }
        }
    }
}
