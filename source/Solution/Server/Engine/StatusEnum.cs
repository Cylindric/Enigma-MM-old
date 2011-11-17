using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine
{
    public enum Status
    {
        Starting,
        Running,
        Busy,
        PendingRestart,
        PendingStop,
        Stopping,
        Stopped,
        Failed
    }
}
