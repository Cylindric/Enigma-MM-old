using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Interfaces
{
    interface IMapper
    {
        void Render();
        void Render(string type);
    }
}
