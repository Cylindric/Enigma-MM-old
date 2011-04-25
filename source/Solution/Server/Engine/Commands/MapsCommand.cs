using System.Linq;

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
            Mappers.Mapper mapper;
                
            mapper = new Mappers.C10t();
            mapper.RenderMap();

            mapper = new Mappers.Overviewer();
            mapper.RenderMap();
        }
    }
}
