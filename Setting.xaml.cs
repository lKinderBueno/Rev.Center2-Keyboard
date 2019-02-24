using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace MyColor2
{
    /// <summary>
    /// Logica di interazione per Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        private string NamePath = @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\";
        public Setting()
        {
            InitializeComponent();

        }


        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "Layout_usa", "0")) == 0)
                Key_uk.Stroke = new SolidColorBrush(Colors.Green);
            else Key_usa.Stroke = new SolidColorBrush(Colors.Green);
        }

        private void Key_uk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Key_uk.Stroke = new SolidColorBrush(Colors.Green);
            Key_usa.Stroke = new SolidColorBrush(Colors.Black);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Layout_usa", "0", RegistryValueKind.DWord);
            Process.Start("Rev.Center2 Keyboard");
            Environment.Exit(0);

        }

        private void Key_usa_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Key_usa.Stroke = new SolidColorBrush(Colors.Green);
            Key_uk.Stroke = new SolidColorBrush(Colors.Black);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Layout_usa", "1", RegistryValueKind.DWord);
            Process.Start("Rev.Center2 Keyboard");
            Environment.Exit(0);

        }

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            window.Topmost = true;
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            window.Topmost = false;

        }
    }
}
