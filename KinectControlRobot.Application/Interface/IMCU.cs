using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlRobot.Application.Interface
{
    public interface IMCU
    {
        MCUStatus Status { get; }

        void Connect();

        void DisConnect();
    }

    public enum MCUStatus
    {
        DisConnected,
        SystemNormal,
        SystemAbnormal,
        Working,
    }
}
