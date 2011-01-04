using EnigmaMM.Interfaces;

namespace EnigmaMM.Plugin.Implementation
{
    public class AlphaVespucciMapper : IMapper
    {
        private string mName;
        private AlphaVespucci M;

        string IPlugin.Name
        {
            get { return mName; }
        }

        public AlphaVespucciMapper()
        {
            mName = "AlphaVespucci";
        }

        void IPlugin.Initialise(IServer server)
        {
            M = new AlphaVespucci(server);            
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
