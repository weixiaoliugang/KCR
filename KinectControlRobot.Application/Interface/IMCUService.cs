using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlRobot.Application.Interface
{
    public interface IMCUService
    {
        IMCU CurrentMCU { get; set; }

        event Action<MCUStatus> MCUStatusChanged;

        void Initialize();

        void InitializeAsynchronous();

        void Close();

        void StartMCU();

        void ResetMCU();

        void StopMCU();
    }
}
