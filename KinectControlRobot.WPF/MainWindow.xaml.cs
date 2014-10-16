using System.Windows;
using KinectControlRobot.Application.ViewModel;
using CustomChrome;

namespace KinectControlRobot.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
            : base(new Thickness(0), 24, new CornerRadius(6), new Thickness(0))
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}