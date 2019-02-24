using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyColor2
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
      ///  static System.Threading.Mutex OneInstMutex = new System.Threading.Mutex(true, "{43621671-C528-4366-B44E-C7C79D6A5889}");
        protected override void OnStartup(StartupEventArgs e)
        {
          /*  OneInstMutex.ReleaseMutex();
            if (OneInstMutex.WaitOne(TimeSpan.Zero, true))
            {
            //TO-DO : If live app.exe on taskmanager, close it.
            */
                base.OnStartup(e);
            //    App.Current.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.RelativeOrAbsolute);
              /*  OneInstMutex.ReleaseMutex();
            }
            else
            {
                App.Current.Shutdown();
                return;
            }*/
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // if (OneInstMutex != null)
            //  {
            //      OneInstMutex.ReleaseMutex();
            //      OneInstMutex.Close();

            // }

            WMIEC.EndWMIRecieveEvent();

        }
        public App()
        {

        }
    }


}
