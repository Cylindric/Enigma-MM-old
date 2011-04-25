using System;
using System.Linq;
using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine.Commands
{
    class MapsCommand: Command
    {
        public MapsCommand()
        {
            mPermissionsRequired.Add(Manager.Database.Permissions.Single(i => i.Name == "maps"));
        }

        protected override void ExecuteTask(EMMServerMessage command)
        {
            Mappers.C10t mapper = new Mappers.C10t();
            mapper.RenderMap();
        }
    }
}
