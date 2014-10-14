using GalaSoft.MvvmLight.Threading;

namespace KinectControlRobot.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
