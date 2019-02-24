using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyColor2
{
    /// <summary>
    /// Tip.xaml 的互動邏輯
    /// </summary>
    public partial class Tips : UserControl
    {
        private uint m_step = 0;
        public MainWindow parentWindow = null;

        public Tips()
        {
            InitializeComponent();
            UpdateSteps(0);
        }
        public void UpdateSteps(uint step)
        {
            switch (step)
            {
                case 0:
                    {

                        img_Tip_f.Margin = img_Tip_0.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips0Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips0Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Visibility = Visibility.Hidden;
                        img_Tip_top_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 22, 0, 0);
                        img_Tip_top_arrow.Visibility = Visibility.Visible;

                        WindowGrid.Margin = new Thickness(img_Tip_top_arrow.Margin.Left - 192, img_Tip_top_arrow.Margin.Top + 12, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 125;
                        TipsPreviousBtn.Visibility = Visibility.Hidden;
                        TipsNextBtn.Visibility = Visibility.Visible;

                    }
                    break;
                case 1:
                    {
                        img_Tip_f.Margin = img_Tip_1.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips1Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips1Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Visibility = Visibility.Hidden;
                        img_Tip_top_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 22, 0, 0);
                        img_Tip_top_arrow.Visibility = Visibility.Visible;

                        WindowGrid.Margin = new Thickness(img_Tip_top_arrow.Margin.Left - 192, img_Tip_top_arrow.Margin.Top + 12, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 150;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 2:
                    {
                        img_Tip_f.Margin = img_Tip_2.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips2Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips2Description1");
                        text_Tips_Title2.SetResourceReference(Run.TextProperty, "strTips2Title2");
                        text_Tips_Description2.SetResourceReference(Run.TextProperty, "strTips2Description2");

                        img_Tip_bottom_arrow.Visibility = Visibility.Hidden;
                        img_Tip_top_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 22, 0, 0);
                        img_Tip_top_arrow.Visibility = Visibility.Visible;

                        WindowGrid.Margin = new Thickness(img_Tip_top_arrow.Margin.Left - 192, img_Tip_top_arrow.Margin.Top + 12, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 200;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 3:
                    {
                        img_Tip_f.Margin = img_Tip_3.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips3Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips3Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Visibility = Visibility.Hidden;
                        img_Tip_top_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 22, 0, 0);
                        img_Tip_top_arrow.Visibility = Visibility.Visible;

                        WindowGrid.Margin = new Thickness(img_Tip_top_arrow.Margin.Left - 192, img_Tip_top_arrow.Margin.Top + 12, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 170;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 4:
                    {
                        img_Tip_f.Margin = img_Tip_4.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips4Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips4Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Visibility = Visibility.Hidden;
                        img_Tip_top_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 22, 0, 0);
                        img_Tip_top_arrow.Visibility = Visibility.Visible;

                        WindowGrid.Margin = new Thickness(img_Tip_top_arrow.Margin.Left - 382, img_Tip_top_arrow.Margin.Top + 12, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 120;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 5:
                    {
                        img_Tip_f.Margin = img_Tip_5.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips5Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips5Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Visibility = Visibility.Hidden;
                        img_Tip_top_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 22, 0, 0);
                        img_Tip_top_arrow.Visibility = Visibility.Visible;

                        WindowGrid.Margin = new Thickness(img_Tip_top_arrow.Margin.Left - 382, img_Tip_top_arrow.Margin.Top + 12, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 125;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 6:
                    {
                        img_Tip_f.Margin = img_Tip_6.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips6Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips6Description1");
                        text_Tips_Title2.SetResourceReference(Run.TextProperty, "strTips6Title2");
                        text_Tips_Description2.SetResourceReference(Run.TextProperty, "strTips6Description2");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left -192, img_Tip_bottom_arrow.Margin.Top - 200, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 200;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 7:
                    {
                        img_Tip_f.Margin = img_Tip_7.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips7Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips7Description1");
                        text_Tips_Title2.SetResourceReference(Run.TextProperty, "strTips7Title2");
                        text_Tips_Description2.SetResourceReference(Run.TextProperty, "strTips7Description2");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left -382, img_Tip_bottom_arrow.Margin.Top - 200, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 200;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 8:
                    {
                        img_Tip_f.Margin = img_Tip_8.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips8Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips8Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left, img_Tip_bottom_arrow.Margin.Top - 180, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 180;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 9:
                    {
                        img_Tip_f.Margin = img_Tip_9.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips9Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips9Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left - 192, img_Tip_bottom_arrow.Margin.Top - 120, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 120;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 10:
                    {
                        img_Tip_f.Margin = img_Tip_10.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips10Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips10Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, ""); ;
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left - 382, img_Tip_bottom_arrow.Margin.Top - 150, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 150;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 11:
                    {
                        img_Tip_f.Margin = img_Tip_11.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips11Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips11Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left, img_Tip_bottom_arrow.Margin.Top - 120, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 120;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 12:
                    {
                        img_Tip_f.Margin = img_Tip_12.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips12Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips12Description1");
                        text_Tips_Title2.SetValue(Run.TextProperty, "");
                        text_Tips_Description2.SetValue(Run.TextProperty, "");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left, img_Tip_bottom_arrow.Margin.Top - 180, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 180;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Visible;
                    }
                    break;
                case 13:
                    {
                        img_Tip_f.Margin = img_Tip_13.Margin;

                        text_Tips_Title1.SetResourceReference(Run.TextProperty, "strTips13Title1");
                        text_Tips_Description1.SetResourceReference(Run.TextProperty, "strTips13Description1");
                        text_Tips_Title2.SetResourceReference(Run.TextProperty, "strTips13Title2");
                        text_Tips_Description2.SetResourceReference(Run.TextProperty, "strTips13Description2");

                        img_Tip_bottom_arrow.Margin = new Thickness(img_Tip_f.Margin.Left + 14, img_Tip_f.Margin.Top + 10, 0, 0);
                        img_Tip_bottom_arrow.Visibility = Visibility.Visible;
                        img_Tip_top_arrow.Visibility = Visibility.Hidden;

                        WindowGrid.Margin = new Thickness(img_Tip_bottom_arrow.Margin.Left - 382, img_Tip_bottom_arrow.Margin.Top - 200, 0, 0);
                        WindowGrid.Width = 400;
                        WindowGrid.Height = 200;
                        TipsPreviousBtn.Visibility = Visibility.Visible;
                        TipsNextBtn.Visibility = Visibility.Hidden;
                    }
                    break;
                default:
                    break;
            }
            m_step = step;
        }
        private void TipsCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void TipsPreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateSteps(m_step - 1);
        }

        private void TipsNextBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateSteps(m_step + 1);
        }

        private void TipsSteip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image img = (Image)sender;
                uint uid = Convert.ToUInt16(img.Uid);
                UpdateSteps(uid);

            }
            catch
            {
                UpdateSteps(0);
            }

            e.Handled = true;
        }
    }
}
