using EnigmaMM.Interfaces;

namespace EnigmaMM.Plugin.Implementation
{
    public class OverviewerMapper : IMapper
    {
        private string mName;
        private Overviewer M;

        string IPlugin.Name
        {
            get { return mName; }
        }

        public OverviewerMapper()
        {
            mName = "Overviewer";
        }

        void IPlugin.Initialise(IServer server)
        {
            M = new Overviewer(server);            
        }

        void IMapper.Render()
        {
            M.RenderMap();
        }

        void IMapper.Render(string type)
        {
            M.RenderMap();
        }

    }
}
