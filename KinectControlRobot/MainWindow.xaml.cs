using System.Windows;
using KinectControlRobot.ViewModel;
using CustomChrome;

namespace KinectControlRobot
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
            : base(new Thickness(6), 24, new CornerRadius(3), new Thickness(0))
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}