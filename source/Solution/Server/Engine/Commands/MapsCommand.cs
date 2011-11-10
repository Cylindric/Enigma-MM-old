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

            Manager.Server.SendCommand("save-all");
            Manager.Server.BlockAutoSave();
            System.Threading.Thread.Sleep(2000);

            mapper = new Mappers.C10t();
            mapper.RenderMap();

            mapper = new Mappers.Overviewer();
            mapper.RenderMap();

            Manager.Server.Broadcast("The maps have been updated");

            Manager.Server.UnblockAutoSave();
        }
    }
}
