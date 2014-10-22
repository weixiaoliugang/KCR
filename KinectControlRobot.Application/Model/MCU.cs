using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using System.Timers;

namespace KinectControlRobot.Application.Model
{
    public class MCU : IMCU
    {
        public MCUStatus Status
        {
            get { return CheckStatus(); }
        }

        public MCU()
        {
        }

        private MCUStatus CheckStatus()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void DisConnect()
        {
            throw new NotImplementedException();
        }
    }
}
