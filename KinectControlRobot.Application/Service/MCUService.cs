using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;

namespace KinectControlRobot.Application.Service
{
    class MCUService:IMCUService
    {
        public IMCU CurrentMCU { get; private set; }

        public MCUService(Action callbackAction)
        {
            throw new NotImplementedException();

            callbackAction.Invoke();
        }

        public MCUStatus CheckMCUStatus()
        {
            throw new NotImplementedException();
        }

        public void StopMCU()
        {
            throw new NotImplementedException();
        }
    }
}
