using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using System.Timers;
using KinectControlRobot.Application.Model;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;

namespace KinectControlRobot.Application.Service
{
    public class MCUService : IMCUService
    {
        private IMCU _currentMCU;
        private MCUStatus _lastStatus;

        public IMCU CurrentMCU
        {
            get { return _currentMCU; }
            set
            {
                if (value is IMCU)
                {
                    _currentMCU = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private void _CheckCanExecute()
        {
            if (_currentMCU == null || _currentMCU.Status != MCUStatus.SystemNormal)
            {
                throw new InvalidOperationException();
            }
        }

        public event Action<MCUStatus> MCUStatusChanged;

        [PreferredConstructor]
        public MCUService() { }

        public MCUService(IMCU mcu)
        {
            CurrentMCU = mcu;
        }

        private void _StartQueryMCUStatus()
        {
            _CheckCanExecute();

            Timer checkStatusTimer = new Timer(200);
            checkStatusTimer.Elapsed += new ElapsedEventHandler((o, e) =>
            {
                MCUStatus currMCUStatus = _currentMCU.Status;
                if (currMCUStatus != _lastStatus)
                {
                    Action<MCUStatus> handler = MCUStatusChanged;
                    if (handler != null)
                    {
                        handler(currMCUStatus);
                    }

                    _lastStatus = currMCUStatus;
                }
            });
            checkStatusTimer.Start();
        }

        public void StartMCU()
        {
            _CheckCanExecute();

            throw new NotImplementedException();
        }

        public void ResetMCU()
        {
            _CheckCanExecute();

            throw new NotImplementedException();
        }

        public void StopMCU()
        {
            _CheckCanExecute();

            throw new NotImplementedException();
        }

        public void Initialize()
        {
            if (_currentMCU == null)
            {
                _currentMCU = ServiceLocator.Current.GetInstance<IMCU>();
                while (_currentMCU.Status != MCUStatus.SystemNormal)
                {
                    System.Threading.Thread.Sleep(200);
                    _currentMCU.Connect();
                }

                _lastStatus = _currentMCU.Status;
                _StartQueryMCUStatus();
            }
        }

        public void InitializeAsynchronous()
        {
            Task.Factory.StartNew(() =>
            {
                Initialize();
            });
        }

        public void Close()
        {
            _currentMCU = null;
        }
    }
}
