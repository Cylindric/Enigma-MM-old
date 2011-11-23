using System.Linq;
using System;

namespace EnigmaMM.Engine.Commands
{
    class MapsCommand: Command
    {
        private static bool AmRendering = false;

        public MapsCommand()
        {
            mPermissionsRequired.Add(Manager.GetContext.Permissions.Single(i => i.Name == "maps"));
        }

        protected override void ExecuteTask(EMMServerMessage command)
        {
            if (AmRendering)
            {
                Manager.Server.RaiseServerMessage("Cannot start mapper, already mapping");
                return;
            }

            AmRendering = true;

            Mappers.Mapper mapper;

            Manager.Server.SendCommand("save-all");
            Manager.Server.BlockAutoSave();
            System.Threading.Thread.Sleep(2000);

            try
            {
                mapper = new Mappers.C10t();
                mapper.RenderMap();
            }
            catch (Exception ex)
            {
                Manager.Server.RaiseServerMessage("Error generating C10t map");
                Manager.Server.RaiseServerMessage(ex.Message);
            }

            try
            {
                mapper = new Mappers.Overviewer();
                mapper.RenderMap();
            }
            catch (Exception ex)
            {
                Manager.Server.RaiseServerMessage("Error generating overviewer map");
                Manager.Server.RaiseServerMessage(ex.Message);
            }


            Manager.Server.Broadcast("The maps have been updated");

            Manager.Server.UnblockAutoSave();

            AmRendering = false;
        }
    }
}
