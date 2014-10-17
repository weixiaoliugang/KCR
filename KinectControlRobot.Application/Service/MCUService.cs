using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;

namespace KinectControlRobot.Application.Service
{
    public class MCUService : IMCUService
    {
        private readonly IMCU _currentMCU;

        public IMCU CurrentMCU { get { return _currentMCU; } }

        public MCUService(IMCU mcu)
        {
            _currentMCU = mcu;
        }

        public MCUStatus CheckMCUStatus()
        {
            throw new NotImplementedException();
        }

        public void StopMCU()
        {
        }
    }
}
