using KinectControlRobot.Application.Interface;

namespace KinectControlRobot.Application.Model
{
    public interface IFrameToSend
    {
        byte[] ToBytes();
    }

    public interface IReceivedFrame
    {
        ReceivedFrameFlag Parse();
    }
}