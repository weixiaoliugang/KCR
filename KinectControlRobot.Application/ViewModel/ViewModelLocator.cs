/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:KinectControlRobot.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight.Ioc;
using KinectControlRobot.Application.Interface;
using KinectControlRobot.Application.Model;
using KinectControlRobot.Application.Service;
using Microsoft.Practices.ServiceLocation;

namespace KinectControlRobot.Application.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the application and provides
    /// an entry point for the bindings.
    /// <para> See http://www.galasoft.ch/mvvm </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            #region Register Interface and Class

            SimpleIoc.Default.Register<IKinectService, KinectService>();
            SimpleIoc.Default.Register<IMCUService, MCUService>();

            #endregion Register Interface and Class

            SimpleIoc.Default.Register<MainViewModel>();
        }

        /// <summary>
        /// Gets the Main property. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources. 
        /// </summary>
        public static void Cleanup()
        {
            // call the cleanup() in the MainViewModel 
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
        }
    }
}