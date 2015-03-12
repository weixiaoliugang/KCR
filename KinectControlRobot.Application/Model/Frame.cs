using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;

namespace KinectControlRobot.Application.Model
{
    public struct FrameToSend : IFrameToSend
    {
        static readonly byte[] Head = new byte[12];
        static readonly byte[] Control = new byte[64];
        static readonly byte[] Crc = new byte[16];
        static readonly byte[] Tail = new byte[4];

        public FrameToSend(FrameToSendFlag frameToSendFlag)
        {
            switch (frameToSendFlag)
            {
                case FrameToSendFlag.Requesting:
                    Head[10] = 0x00;break;
                case FrameToSendFlag.Performing:
                    Head[10] = 0x0f;break;
                    case FrameToSendFlag.Halting:
                    Head[10] = 0xf0;break;
            }
        }

        public byte[] ToBytes()
        {
            return Head.Concat(Control).Concat(Crc).Concat(Tail).ToArray();
        }
    }

    public struct ReceivedFrame : IReceivedFrame
    {
        static readonly byte[] Head = new byte[12];
        static readonly byte[] State = new byte[1];
        static readonly byte[] Crc = new byte[16];
        static readonly byte[] Tail = new byte[3];
        private readonly bool _isBroken;

        public ReceivedFrame(byte[] receivedBytes)
        {
            if (receivedBytes.Length!=32)
            {
                _isBroken = true;
                return;
            }

            Array.Copy(receivedBytes, Head, 12);
            Array.Copy(receivedBytes, 12 , State, 0, 1);
            Array.Copy(receivedBytes, 13, Crc, 0, 16);
            Array.Copy(receivedBytes, 29, Tail, 0, 3);
            _isBroken = false;
        }

        public ReceivedFrameFlag Parse()
        {
            if (!_isBroken)
            {
                return (ReceivedFrameFlag) State[0];
            }

            return ReceivedFrameFlag._Broken_;
        }
    }
}
