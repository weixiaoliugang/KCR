using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlRobot.Application.Interface
{
    public interface IMCUService
    {
        IMCU CurrentMCU { get; }

        MCUStatus CheckMCUStatus();

        void StopMCU();
    }
}
