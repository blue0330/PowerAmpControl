/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:PowerAmpControl.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PowerAmpControl.Model;

namespace PowerAmpControl.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PowerAmplifierViewModel>();
            SimpleIoc.Default.Register<SocketViewModel>();
            //SimpleIoc.Default.Register<SocketClientViewModel>();
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

        public PowerAmplifierViewModel PowerAmplifier
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PowerAmplifierViewModel>();
            }
        }
        public SocketViewModel Socket
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SocketViewModel>();
            }
        }

        //public SocketClientViewModel SocketClient
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<SocketClientViewModel>();
        //    } 
        //}
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}