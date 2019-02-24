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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyColor2
{
    /// <summary>
    /// KBTable.xaml 的互動邏輯
    /// </summary>
    public partial class KBTable102 : IKBTypeInterface
    {
        public bool m_KBlayout_enable = false;
         //Grid_KB mouse parameter
        private bool isLeftMouseButtonDownOnWindow = false;
        private bool isDraggingSelectionRect = false;
        private Point origMouseDownPoint;
        private static readonly double DragThreshold = 5;

        public List<key> m_KeyList102 = null;
        public List<key> m_WelKeyList = null;
        public bool KBTI_KBlayout_enable {
            get { return m_KBlayout_enable; }

            set { m_KBlayout_enable = value; }

        }
        public List<key> KBTI_KeyList {
            get
            {
                return m_KeyList102;
            }

            set
            {
                m_KeyList102 = value;
            }
        }
        public List<key> KBTI_WelKeyList {
            get
            {
                return m_WelKeyList;
            }

            set
            {
                m_WelKeyList = value;
            }
        }

        //private uint m_mode = (uint)mode.windows;
        private uint m_mode { get; set; }

    private void InitializeParameter()
        {
            m_KeyList102 = new List<key>
            {
                //new key("esc"         ,       0x01, false, false, 0x00, 0x00, 0x00,  11, 11,  45, 38,    6,   0),
                new key("esc"         ,       0x01, false, false, 0x00, 0x00, 0x00,  	img_kb_esc.Margin.Left + 2 , img_kb_esc.Margin.Top + 2 ,  img_kb_esc.Margin.Left+img_kb_esc.Width - 2, img_kb_esc.Margin.Top+img_kb_esc.Height - 2,    											 6,   0),
                new key("f1"          ,       0x3B, false, false, 0x00, 0x00, 0x00,     img_kb_f1.Margin.Left + 2 , img_kb_f1.Margin.Top + 2 ,  img_kb_f1.Margin.Left+img_kb_f1.Width - 2, img_kb_f1.Margin.Top+img_kb_f1.Height - 2,   												12,   0),
                new key("f2"          ,       0x3C, false, false, 0x00, 0x00, 0x00,     img_kb_f2.Margin.Left + 2 , img_kb_f2.Margin.Top + 2 ,  img_kb_f2.Margin.Left+img_kb_f2.Width - 2, img_kb_f2.Margin.Top+img_kb_f2.Height - 2,   												18,   0),
                new key("f3"          ,       0x3D, false, false, 0x00, 0x00, 0x00,     img_kb_f3.Margin.Left + 2 , img_kb_f3.Margin.Top + 2 ,  img_kb_f3.Margin.Left+img_kb_f3.Width - 2, img_kb_f3.Margin.Top+img_kb_f3.Height - 2,   												24,   0),
                new key("f4"          ,       0x3E, false, false, 0x00, 0x00, 0x00,     img_kb_f4.Margin.Left + 2 , img_kb_f4.Margin.Top + 2 ,  img_kb_f4.Margin.Left+img_kb_f4.Width - 2, img_kb_f4.Margin.Top+img_kb_f4.Height - 2,   												30,   0),
                new key("f5"          ,       0x3F, false, false, 0x00, 0x00, 0x00,     img_kb_f5.Margin.Left + 2 , img_kb_f5.Margin.Top + 2 ,  img_kb_f5.Margin.Left+img_kb_f5.Width - 2, img_kb_f5.Margin.Top+img_kb_f5.Height - 2,   												36,   0),
                new key("f6"          ,       0x40, false, false, 0x00, 0x00, 0x00,     img_kb_f6.Margin.Left + 2 , img_kb_f6.Margin.Top + 2 ,  img_kb_f6.Margin.Left+img_kb_f6.Width - 2, img_kb_f6.Margin.Top+img_kb_f6.Height - 2,   												42,   0),
                new key("f7"          ,       0x41, false, false, 0x00, 0x00, 0x00,     img_kb_f7.Margin.Left + 2 , img_kb_f7.Margin.Top + 2 ,  img_kb_f7.Margin.Left+img_kb_f7.Width - 2, img_kb_f7.Margin.Top+img_kb_f7.Height - 2,   												48,   0),
                new key("f8"          ,       0x42, false, false, 0x00, 0x00, 0x00,     img_kb_f8.Margin.Left + 2 , img_kb_f8.Margin.Top + 2 ,  img_kb_f8.Margin.Left+img_kb_f8.Width - 2, img_kb_f8.Margin.Top+img_kb_f8.Height - 2,   												54,   0),
                new key("f9"          ,       0x43, false, false, 0x00, 0x00, 0x00,     img_kb_f9.Margin.Left + 2 , img_kb_f9.Margin.Top + 2 ,  img_kb_f9.Margin.Left+img_kb_f9.Width - 2, img_kb_f9.Margin.Top+img_kb_f9.Height - 2,   												60,   0),//10
                new key("f10"         ,       0x44, false, false, 0x00, 0x00, 0x00,     img_kb_f10.Margin.Left + 2 , img_kb_f10.Margin.Top + 2 ,  img_kb_f10.Margin.Left+img_kb_f10.Width - 2, img_kb_f10.Margin.Top+img_kb_f10.Height - 2,   											66,   0),
                new key("f11"         ,       0x45, false, false, 0x00, 0x00, 0x00,     img_kb_f11.Margin.Left + 2 , img_kb_f11.Margin.Top + 2 ,  img_kb_f11.Margin.Left+img_kb_f11.Width - 2, img_kb_f11.Margin.Top+img_kb_f11.Height - 2,   											72,   0),
                new key("f12"         ,       0x46, false, false, 0x00, 0x00, 0x00,     img_kb_f12.Margin.Left + 2 , img_kb_f12.Margin.Top + 2 ,  img_kb_f12.Margin.Left+img_kb_f12.Width - 2, img_kb_f12.Margin.Top+img_kb_f12.Height - 2,   											78,   0),
                new key("prtsc"       ,       0x37, false, false, 0x00, 0x00, 0x00,     img_kb_prtsc.Margin.Left + 2 , img_kb_prtsc.Margin.Top + 2 ,  img_kb_prtsc.Margin.Left+img_kb_prtsc.Width - 2, img_kb_prtsc.Margin.Top+img_kb_prtsc.Height - 2,   								84,   0), //14
                //new key("insert"      ,       0x52, false, false, 0x00, 0x00, 0x00, 515, 11, 549, 38,   90,   0),
                new key("del"         ,       0x53, false, false, 0x00, 0x00, 0x00,     img_kb_del.Margin.Left + 2 , img_kb_del.Margin.Top + 2 ,  img_kb_del.Margin.Left+img_kb_del.Width - 2, img_kb_del.Margin.Top+img_kb_del.Height - 2,   											90,   0),
                new key("home"        ,       0x47, false, false, 0x00, 0x00, 0x00,     img_kb_home.Margin.Left + 2 , img_kb_home.Margin.Top + 2 ,  img_kb_home.Margin.Left+img_kb_home.Width - 2, img_kb_home.Margin.Top+img_kb_home.Height - 2,  										96,   0),
                new key("end"         ,       0x4F, false, false, 0x00, 0x00, 0x00,     img_kb_end.Margin.Left + 2 , img_kb_end.Margin.Top + 2 ,  img_kb_end.Margin.Left+img_kb_end.Width - 2, img_kb_end.Margin.Top+img_kb_end.Height - 2,  											102,   0),
                new key("pgup"        ,       0x49, false, false, 0x00, 0x00, 0x00,     img_kb_pgup.Margin.Left + 2 , img_kb_pgup.Margin.Top + 2 ,  img_kb_pgup.Margin.Left+img_kb_pgup.Width - 2, img_kb_pgup.Margin.Top+img_kb_pgup.Height - 2,  										108,   0),
                new key("pgdn"        ,       0x51, false, false, 0x00, 0x00, 0x00,     img_kb_pgdn.Margin.Left + 2 , img_kb_pgdn.Margin.Top + 2 ,  img_kb_pgdn.Margin.Left+img_kb_pgdn.Width - 2, img_kb_pgdn.Margin.Top+img_kb_pgdn.Height - 2,  										114,   0),
                new key("Quotes"      ,       0x29, false, false, 0x00, 0x00, 0x00,     img_kb_Quotes.Margin.Left + 2 , img_kb_Quotes.Margin.Top + 2 ,  img_kb_Quotes.Margin.Left+img_kb_Quotes.Width - 2, img_kb_Quotes.Margin.Top+img_kb_Quotes.Height - 2,    						  5,   0),//20
                new key("1"           ,       0x02, false, false, 0x00, 0x00, 0x00,     img_kb_1.Margin.Left + 2 , img_kb_1.Margin.Top + 2 ,  img_kb_1.Margin.Left+img_kb_1.Width - 2, img_kb_1.Margin.Top+img_kb_1.Height - 2,   														 11,   0),
                new key("2"           ,       0x03, false, false, 0x00, 0x00, 0x00,     img_kb_2.Margin.Left + 2 , img_kb_2.Margin.Top + 2 ,  img_kb_2.Margin.Left+img_kb_2.Width - 2, img_kb_2.Margin.Top+img_kb_2.Height - 2,   														 17,   0),
                new key("3"           ,       0x04, false, false, 0x00, 0x00, 0x00,     img_kb_3.Margin.Left + 2 , img_kb_3.Margin.Top + 2 ,  img_kb_3.Margin.Left+img_kb_3.Width - 2, img_kb_3.Margin.Top+img_kb_3.Height - 2,   														 23,   0),
                new key("4"           ,       0x05, false, false, 0x00, 0x00, 0x00,     img_kb_4.Margin.Left + 2 , img_kb_4.Margin.Top + 2 ,  img_kb_4.Margin.Left+img_kb_4.Width - 2, img_kb_4.Margin.Top+img_kb_4.Height - 2,   														 29,   0),
                new key("5"           ,       0x06, false, false, 0x00, 0x00, 0x00,     img_kb_5.Margin.Left + 2 , img_kb_5.Margin.Top + 2 ,  img_kb_5.Margin.Left+img_kb_5.Width - 2, img_kb_5.Margin.Top+img_kb_5.Height - 2,   														 35,   0),
                new key("6"           ,       0x07, false, false, 0x00, 0x00, 0x00,     img_kb_6.Margin.Left + 2 , img_kb_6.Margin.Top + 2 ,  img_kb_6.Margin.Left+img_kb_6.Width - 2, img_kb_6.Margin.Top+img_kb_6.Height - 2,   														 41,   0),
                new key("7"           ,       0x08, false, false, 0x00, 0x00, 0x00,     img_kb_7.Margin.Left + 2 , img_kb_7.Margin.Top + 2 ,  img_kb_7.Margin.Left+img_kb_7.Width - 2, img_kb_7.Margin.Top+img_kb_7.Height - 2,   														 47,   0),
                new key("8"           ,       0x09, false, false, 0x00, 0x00, 0x00,     img_kb_8.Margin.Left + 2 , img_kb_8.Margin.Top + 2 ,  img_kb_8.Margin.Left+img_kb_8.Width - 2, img_kb_8.Margin.Top+img_kb_8.Height - 2,   														 53,   0),
                new key("9"           ,       0x0A, false, false, 0x00, 0x00, 0x00,     img_kb_9.Margin.Left + 2 , img_kb_9.Margin.Top + 2 ,  img_kb_9.Margin.Left+img_kb_9.Width - 2, img_kb_9.Margin.Top+img_kb_9.Height - 2,   														 59,   0),
                new key("0"           ,       0x0B, false, false, 0x00, 0x00, 0x00,     img_kb_0.Margin.Left + 2 , img_kb_0.Margin.Top + 2 ,  img_kb_0.Margin.Left+img_kb_0.Width - 2, img_kb_0.Margin.Top+img_kb_0.Height - 2,   														 65,   0),  //30
                new key("underline"   ,       0x0C, false, false, 0x00, 0x00, 0x00,     img_kb_underline.Margin.Left + 2 , img_kb_underline.Margin.Top + 2 ,  img_kb_underline.Margin.Left+img_kb_underline.Width - 2, img_kb_underline.Margin.Top+img_kb_underline.Height - 2,   		 71,   0),
                new key("equal"      ,       0x0D, false, false, 0x00, 0x00, 0x00,      img_kb_equal.Margin.Left + 2 , img_kb_equal.Margin.Top + 2 ,  img_kb_equal.Margin.Left+img_kb_equal.Width - 2, img_kb_equal.Margin.Top+img_kb_equal.Height - 2,   77,   0),
                new key("backspace"   ,       0x0E, false, false, 0x00, 0x00, 0x00,     img_kb_backspace.Margin.Left + 2 , img_kb_backspace.Margin.Top + 2 ,  img_kb_backspace.Margin.Left+img_kb_backspace.Width - 2, img_kb_backspace.Margin.Top+img_kb_backspace.Height - 2,   		 89,   0),
                new key("numlock"     ,       0x45, false, false, 0x00, 0x00, 0x00,     img_kb_numlock.Margin.Left + 2 , img_kb_numlock.Margin.Top + 2 ,  img_kb_numlock.Margin.Left+img_kb_numlock.Width - 2, img_kb_numlock.Margin.Top+img_kb_numlock.Height - 2,   					 95,   0),
                new key("ndevide"     , (ushort)0xE035, false, false, 0x00, 0x00, 0x00, img_kb_ndevide.Margin.Left + 2 , img_kb_ndevide.Margin.Top + 2 ,  img_kb_ndevide.Margin.Left+img_kb_ndevide.Width - 2, img_kb_ndevide.Margin.Top+img_kb_ndevide.Height - 2,  					101,   0),
                new key("nmultiply"   ,       0x37, false, false, 0x00, 0x00, 0x00,     img_kb_nmultiply.Margin.Left + 2 , img_kb_nmultiply.Margin.Top + 2 ,  img_kb_nmultiply.Margin.Left+img_kb_nmultiply.Width - 2, img_kb_nmultiply.Margin.Top+img_kb_nmultiply.Height - 2,  		107,   0),
                new key("nminus"       ,       0x4A, false, false, 0x00, 0x00, 0x00,    img_kb_nminus.Margin.Left + 2 , img_kb_nminus.Margin.Top + 2 ,  img_kb_nminus.Margin.Left+img_kb_nminus.Width - 2, img_kb_nminus.Margin.Top+img_kb_nminus.Height - 2,  							113,   0),
                new key("tab"         ,       0x0F, false, false, 0x00, 0x00, 0x00,     img_kb_tab.Margin.Left + 2 , img_kb_tab.Margin.Top + 2 ,  img_kb_tab.Margin.Left+img_kb_tab.Width - 2, img_kb_tab.Margin.Top+img_kb_tab.Height - 2,   											  4,   0),
                new key("q"			  ,       0x10, false, false, 0x00, 0x00, 0x00,     img_kb_q.Margin.Left + 2 , img_kb_q.Margin.Top + 2 ,  img_kb_q.Margin.Left+img_kb_q.Width - 2, img_kb_q.Margin.Top+img_kb_q.Height - 2,  														 16,   0),
                new key("w"			  ,       0x11, false, false, 0x00, 0x00, 0x00,     img_kb_w.Margin.Left + 2 , img_kb_w.Margin.Top + 2 ,  img_kb_w.Margin.Left+img_kb_w.Width - 2, img_kb_w.Margin.Top+img_kb_w.Height - 2,  														 22,   0),   //40
                new key("e"			  ,       0x12, false, false, 0x00, 0x00, 0x00,     img_kb_e.Margin.Left + 2 , img_kb_e.Margin.Top + 2 ,  img_kb_e.Margin.Left+img_kb_e.Width - 2, img_kb_e.Margin.Top+img_kb_e.Height - 2,  														 28,   0),
                new key("r"			  ,       0x13, false, false, 0x00, 0x00, 0x00,     img_kb_r.Margin.Left + 2 , img_kb_r.Margin.Top + 2 ,  img_kb_r.Margin.Left+img_kb_r.Width - 2, img_kb_r.Margin.Top+img_kb_r.Height - 2,  														 34,   0),
                new key("t"			  ,       0x14, false, false, 0x00, 0x00, 0x00,     img_kb_t.Margin.Left + 2 , img_kb_t.Margin.Top + 2 ,  img_kb_t.Margin.Left+img_kb_t.Width - 2, img_kb_t.Margin.Top+img_kb_t.Height - 2,  														 40,   0),
                new key("y"			  ,       0x15, false, false, 0x00, 0x00, 0x00,     img_kb_y.Margin.Left + 2 , img_kb_y.Margin.Top + 2 ,  img_kb_y.Margin.Left+img_kb_y.Width - 2, img_kb_y.Margin.Top+img_kb_y.Height - 2,  														 46,   0),
                new key("u"			  ,       0x16, false, false, 0x00, 0x00, 0x00,     img_kb_u.Margin.Left + 2 , img_kb_u.Margin.Top + 2 ,  img_kb_u.Margin.Left+img_kb_u.Width - 2, img_kb_u.Margin.Top+img_kb_u.Height - 2,  														 52,   0),
                new key("i"			  ,       0x17, false, false, 0x00, 0x00, 0x00,     img_kb_i.Margin.Left + 2 , img_kb_i.Margin.Top + 2 ,  img_kb_i.Margin.Left+img_kb_i.Width - 2, img_kb_i.Margin.Top+img_kb_i.Height - 2,  														 58,   0),
                new key("o"			  ,       0x18, false, false, 0x00, 0x00, 0x00,     img_kb_o.Margin.Left + 2 , img_kb_o.Margin.Top + 2 ,  img_kb_o.Margin.Left+img_kb_o.Width - 2, img_kb_o.Margin.Top+img_kb_o.Height - 2,  														 64,   0),
                new key("p"			  ,       0x19, false, false, 0x00, 0x00, 0x00,     img_kb_p.Margin.Left + 2 , img_kb_p.Margin.Top + 2 ,  img_kb_p.Margin.Left+img_kb_p.Width - 2, img_kb_p.Margin.Top+img_kb_p.Height - 2,  														 70,   0),
                new key("Lparantheses",       0x1A, false, false, 0x00, 0x00, 0x00,     img_kb_Lparantheses.Margin.Left + 2 , img_kb_Lparantheses.Margin.Top + 2 ,  img_kb_Lparantheses.Margin.Left+img_kb_Lparantheses.Width - 2, img_kb_Lparantheses.Margin.Top+img_kb_Lparantheses.Height - 2,  76,   0),
                new key("Rparantheses",       0x1B, false, false, 0x00, 0x00, 0x00,     img_kb_Rparantheses.Margin.Left + 2 , img_kb_Rparantheses.Margin.Top + 2 ,  img_kb_Rparantheses.Margin.Left+img_kb_Rparantheses.Width - 2, img_kb_Rparantheses.Margin.Top+img_kb_Rparantheses.Height - 2,  82,   0),//50
                new key("enter"		  ,       0x2B, false, false, 0x00, 0x00, 0x00,     img_kb_enter.Margin.Left + 2 , img_kb_enter.Margin.Top + 2 ,  img_kb_enter.Margin.Left+img_kb_enter.Width - 2, img_kb_enter.Margin.Top+img_kb_enter.Height - 2,  								 88,   0),
                new key("n7"		  ,       0x47, false, false, 0x00, 0x00, 0x00,     img_kb_n7.Margin.Left + 2 , img_kb_n7.Margin.Top + 2 ,  img_kb_n7.Margin.Left+img_kb_n7.Width - 2, img_kb_n7.Margin.Top+img_kb_n7.Height - 2, 													 94,   0),
                new key("n8"		  ,       0x48, false, false, 0x00, 0x00, 0x00,     img_kb_n8.Margin.Left + 2 , img_kb_n8.Margin.Top + 2 ,  img_kb_n8.Margin.Left+img_kb_n8.Width - 2, img_kb_n8.Margin.Top+img_kb_n8.Height - 2, 													100,   0),
                new key("n9"		  ,       0x49, false, false, 0x00, 0x00, 0x00,     img_kb_n9.Margin.Left + 2 , img_kb_n9.Margin.Top + 2 ,  img_kb_n9.Margin.Left+img_kb_n9.Width - 2, img_kb_n9.Margin.Top+img_kb_n9.Height - 2, 													106,   0),
                new key("nplus"       ,       0x4E, false, false, 0x00, 0x00, 0x00,     img_kb_nplus.Margin.Left + 2 , img_kb_nplus.Margin.Top + 2 ,  img_kb_nplus.Margin.Left+img_kb_nplus.Width - 2, img_kb_nplus.Margin.Top+img_kb_nplus.Height - 2, 								112,   0),
                new key("capslock"    ,       0x3A, false, false, 0x00, 0x00, 0x00,     img_kb_capslock.Margin.Left + 2 , img_kb_capslock.Margin.Top + 2 ,  img_kb_capslock.Margin.Left+img_kb_capslock.Width - 2, img_kb_capslock.Margin.Top+img_kb_capslock.Height - 2,   			  3,   0),
                new key("a"			  ,       0x1E, false, false, 0x00, 0x00, 0x00,     img_kb_a.Margin.Left + 2 , img_kb_a.Margin.Top + 2 ,  img_kb_a.Margin.Left+img_kb_a.Width - 2, img_kb_a.Margin.Top+img_kb_a.Height - 2,  														 15,   0),
                new key("s"			  ,       0x1F, false, false, 0x00, 0x00, 0x00,     img_kb_s.Margin.Left + 2 , img_kb_s.Margin.Top + 2 ,  img_kb_s.Margin.Left+img_kb_s.Width - 2, img_kb_s.Margin.Top+img_kb_s.Height - 2,  														 21,   0),
                new key("d"			  ,       0x20, false, false, 0x00, 0x00, 0x00,     img_kb_d.Margin.Left + 2 , img_kb_d.Margin.Top + 2 ,  img_kb_d.Margin.Left+img_kb_d.Width - 2, img_kb_d.Margin.Top+img_kb_d.Height - 2,  														 27,   0),
                new key("f"			  ,       0x21, false, false, 0x00, 0x00, 0x00,     img_kb_f.Margin.Left + 2 , img_kb_f.Margin.Top + 2 ,  img_kb_f.Margin.Left+img_kb_f.Width - 2, img_kb_f.Margin.Top+img_kb_f.Height - 2,  														 33,   0),//60
                new key("g"			  ,       0x22, false, false, 0x00, 0x00, 0x00,     img_kb_g.Margin.Left + 2 , img_kb_g.Margin.Top + 2 ,  img_kb_g.Margin.Left+img_kb_g.Width - 2, img_kb_g.Margin.Top+img_kb_g.Height - 2,  														 39,   0),
                new key("h"			  ,       0x23, false, false, 0x00, 0x00, 0x00,     img_kb_h.Margin.Left + 2 , img_kb_h.Margin.Top + 2 ,  img_kb_h.Margin.Left+img_kb_h.Width - 2, img_kb_h.Margin.Top+img_kb_h.Height - 2,  														 45,   0),
                new key("j"			  ,       0x24, false, false, 0x00, 0x00, 0x00,     img_kb_j.Margin.Left + 2 , img_kb_j.Margin.Top + 2 ,  img_kb_j.Margin.Left+img_kb_j.Width - 2, img_kb_j.Margin.Top+img_kb_j.Height - 2,  														 51,   0),
                new key("k"			  ,       0x25, false, false, 0x00, 0x00, 0x00,     img_kb_k.Margin.Left + 2 , img_kb_k.Margin.Top + 2 ,  img_kb_k.Margin.Left+img_kb_k.Width - 2, img_kb_k.Margin.Top+img_kb_k.Height - 2,  														 57,   0),
                new key("l"			  ,       0x26, false, false, 0x00, 0x00, 0x00,     img_kb_l.Margin.Left + 2 , img_kb_l.Margin.Top + 2 ,  img_kb_l.Margin.Left+img_kb_l.Width - 2, img_kb_l.Margin.Top+img_kb_l.Height - 2,  														 63,   0),
                new key("colon"		  ,       0x27, false, false, 0x00, 0x00, 0x00,     img_kb_colon.Margin.Left + 2 , img_kb_colon.Margin.Top + 2 ,  img_kb_colon.Margin.Left+img_kb_colon.Width - 2, img_kb_colon.Margin.Top+img_kb_colon.Height - 2,  								 69,   0),
                new key("at"          ,       0x28, false, false, 0x00, 0x00, 0x00,     img_kb_at.Margin.Left + 2 , img_kb_at.Margin.Top + 2 ,  img_kb_at.Margin.Left+img_kb_at.Width - 2, img_kb_at.Margin.Top+img_kb_at.Height - 2,  													 75,   0),
                new key("hashtag"	  ,       0x1c, false, false, 0x00, 0x00, 0x00,     img_kb_hashtag.Margin.Left + 2 , img_kb_hashtag.Margin.Top + 2 ,  img_kb_hashtag.Margin.Left+img_kb_hashtag.Width - 2, img_kb_hashtag.Margin.Top+img_kb_hashtag.Height - 2,  					 81,  87),
                new key("n4"		  ,       0x4b, false, false, 0x00, 0x00, 0x00,     img_kb_n4.Margin.Left + 2 , img_kb_n4.Margin.Top + 2 ,  img_kb_n4.Margin.Left+img_kb_n4.Width - 2, img_kb_n4.Margin.Top+img_kb_n4.Height - 2, 													 93,   0),
                new key("n5"		  ,       0x4c, false, false, 0x00, 0x00, 0x00,     img_kb_n5.Margin.Left + 2 , img_kb_n5.Margin.Top + 2 ,  img_kb_n5.Margin.Left+img_kb_n5.Width - 2, img_kb_n5.Margin.Top+img_kb_n5.Height - 2, 													 99,   0),//70
                new key("n6"		  ,       0x4d, false, false, 0x00, 0x00, 0x00,     img_kb_n6.Margin.Left + 2 , img_kb_n6.Margin.Top + 2 ,  img_kb_n6.Margin.Left+img_kb_n6.Width - 2, img_kb_n6.Margin.Top+img_kb_n6.Height - 2, 													105,   0),
                new key("Lshift"	  ,       0x2a, false, false, 0x00, 0x00, 0x00,     img_kb_Lshift.Margin.Left + 2 , img_kb_Lshift.Margin.Top + 2 ,  img_kb_Lshift.Margin.Left+img_kb_Lshift.Width - 2, img_kb_Lshift.Margin.Top+img_kb_Lshift.Height - 2,   						  2,   0),
                new key("backslash"   ,       0x2a, false, false, 0x00, 0x00, 0x00,     img_kb_backslash.Margin.Left + 2 , img_kb_backslash.Margin.Top + 2 ,  img_kb_backslash.Margin.Left+img_kb_backslash.Width - 2, img_kb_backslash.Margin.Top+img_kb_backslash.Height - 2,   		 14,   0),
                new key("z"           ,       0x2c, false, false, 0x00, 0x00, 0x00,     img_kb_z.Margin.Left + 2 , img_kb_z.Margin.Top + 2 ,  img_kb_z.Margin.Left+img_kb_z.Width - 2, img_kb_z.Margin.Top+img_kb_z.Height - 2,  														 20,   0),
                new key("x"           ,       0x2d, false, false, 0x00, 0x00, 0x00,     img_kb_x.Margin.Left + 2 , img_kb_x.Margin.Top + 2 ,  img_kb_x.Margin.Left+img_kb_x.Width - 2, img_kb_x.Margin.Top+img_kb_x.Height - 2,  														 26,   0),
                new key("c"           ,       0x2e, false, false, 0x00, 0x00, 0x00,     img_kb_c.Margin.Left + 2 , img_kb_c.Margin.Top + 2 ,  img_kb_c.Margin.Left+img_kb_c.Width - 2, img_kb_c.Margin.Top+img_kb_c.Height - 2,  														 32,   0),
                new key("v"           ,       0x2f, false, false, 0x00, 0x00, 0x00,     img_kb_v.Margin.Left + 2 , img_kb_v.Margin.Top + 2 ,  img_kb_v.Margin.Left+img_kb_v.Width - 2, img_kb_v.Margin.Top+img_kb_v.Height - 2,  														 38,   0),
                new key("b"           ,       0x30, false, false, 0x00, 0x00, 0x00,     img_kb_b.Margin.Left + 2 , img_kb_b.Margin.Top + 2 ,  img_kb_b.Margin.Left+img_kb_b.Width - 2, img_kb_b.Margin.Top+img_kb_b.Height - 2,  														 44,   0),
                new key("n"           ,       0x31, false, false, 0x00, 0x00, 0x00,     img_kb_n.Margin.Left + 2 , img_kb_n.Margin.Top + 2 ,  img_kb_n.Margin.Left+img_kb_n.Width - 2, img_kb_n.Margin.Top+img_kb_n.Height - 2,  														 50,   0),
                new key("m"           ,       0x32, false, false, 0x00, 0x00, 0x00,     img_kb_m.Margin.Left + 2 , img_kb_m.Margin.Top + 2 ,  img_kb_m.Margin.Left+img_kb_m.Width - 2, img_kb_m.Margin.Top+img_kb_m.Height - 2,  														 56,   0),//80
                new key("less"		  ,       0x33, false, false, 0x00, 0x00, 0x00,     img_kb_less.Margin.Left + 2 , img_kb_less.Margin.Top + 2 ,  img_kb_less.Margin.Left+img_kb_less.Width - 2, img_kb_less.Margin.Top+img_kb_less.Height - 2,  										 62,   0),
                new key("more"		  ,       0x34, false, false, 0x00, 0x00, 0x00,     img_kb_more.Margin.Left + 2 , img_kb_more.Margin.Top + 2 ,  img_kb_more.Margin.Left+img_kb_more.Width - 2, img_kb_more.Margin.Top+img_kb_more.Height - 2,  										 68,   0),
                new key("question"	  ,       0x35, false, false, 0x00, 0x00, 0x00,     img_kb_question.Margin.Left + 2 , img_kb_question.Margin.Top + 2 ,  img_kb_question.Margin.Left+img_kb_question.Width - 2, img_kb_question.Margin.Top+img_kb_question.Height - 2,  				 74,   0),
                new key("Rshift"	  ,       0x36, false, false, 0x00, 0x00, 0x00,     img_kb_Rshift.Margin.Left + 2 , img_kb_Rshift.Margin.Top + 2 ,  img_kb_Rshift.Margin.Left+img_kb_Rshift.Width - 2, img_kb_Rshift.Margin.Top+img_kb_Rshift.Height - 2,  							 80,   0),
                new key("up"		  ,       0xe048, false, false, 0x00, 0x00, 0x00,   img_kb_up.Margin.Left + 2 , img_kb_up.Margin.Top + 2 ,  img_kb_up.Margin.Left+img_kb_up.Width - 2, img_kb_up.Margin.Top+img_kb_up.Height - 2, 													 86,   0),
                new key("n1"		  ,       0x4f, false, false, 0x00, 0x00, 0x00,     img_kb_n1.Margin.Left + 2 , img_kb_n1.Margin.Top + 2 ,  img_kb_n1.Margin.Left+img_kb_n1.Width - 2, img_kb_n1.Margin.Top+img_kb_n1.Height - 2, 													 92,   0),
                new key("n2"		  ,       0x50, false, false, 0x00, 0x00, 0x00,     img_kb_n2.Margin.Left + 2 , img_kb_n2.Margin.Top + 2 ,  img_kb_n2.Margin.Left+img_kb_n2.Width - 2, img_kb_n2.Margin.Top+img_kb_n2.Height - 2, 													 98,    0),
                new key("n3"		  ,       0x51, false, false, 0x00, 0x00, 0x00,     img_kb_n3.Margin.Left + 2 , img_kb_n3.Margin.Top + 2 ,  img_kb_n3.Margin.Left+img_kb_n3.Width - 2, img_kb_n3.Margin.Top+img_kb_n3.Height - 2, 													104,   0),
                new key("nenter"	  ,       0xe01c, false, false, 0x00, 0x00, 0x00,   img_kb_nenter.Margin.Left + 2 , img_kb_nenter.Margin.Top + 2 ,  img_kb_nenter.Margin.Left+img_kb_nenter.Width - 2, img_kb_nenter.Margin.Top+img_kb_nenter.Height - 2, 							110,   0),
                new key("Lctrl"		  ,       0x1d, false, false, 0x00, 0x00, 0x00,     img_kb_Lctrl.Margin.Left + 2 , img_kb_Lctrl.Margin.Top + 2 ,  img_kb_Lctrl.Margin.Left+img_kb_Lctrl.Width - 2, img_kb_Lctrl.Margin.Top+img_kb_Lctrl.Height - 2,   								  1,   0),//90
                new key("fn"		  ,       0xffff, false, false, 0x00, 0x00, 0x00,   img_kb_fn.Margin.Left + 2 , img_kb_fn.Margin.Top + 2 ,  img_kb_fn.Margin.Left+img_kb_fn.Width - 2, img_kb_fn.Margin.Top+img_kb_fn.Height - 2,  												     13,   0),
                new key("windows"	  ,       0xffff, false, false, 0x00, 0x00, 0x00,   img_kb_windows.Margin.Left + 2 , img_kb_windows.Margin.Top + 2 ,  img_kb_windows.Margin.Left+img_kb_windows.Width - 2, img_kb_windows.Margin.Top+img_kb_windows.Height - 2,  					 19,   0),
                new key("Lalt"		  ,       0x38, false, false, 0x00, 0x00, 0x00,     img_kb_Lalt.Margin.Left + 2 , img_kb_Lalt.Margin.Top + 2 ,  img_kb_Lalt.Margin.Left+img_kb_Lalt.Width - 2, img_kb_Lalt.Margin.Top+img_kb_Lalt.Height - 2,  										 25,   0),
                new key("space"		  ,       0x39, false, false, 0x00, 0x00, 0x00,     img_kb_space.Margin.Left + 2 , img_kb_space.Margin.Top + 2 ,  img_kb_space.Margin.Left+img_kb_space.Width - 2, img_kb_space.Margin.Top+img_kb_space.Height - 2,  								 43,   0),
                new key("altgr"		  ,       0xe038, false, false, 0x00, 0x00, 0x00,   img_kb_altgr.Margin.Left + 2 , img_kb_altgr.Margin.Top + 2 ,  img_kb_altgr.Margin.Left+img_kb_altgr.Width - 2, img_kb_altgr.Margin.Top+img_kb_altgr.Height - 2,  								 61,   0),
                new key("list"		  ,       0xffff, false, false, 0x00, 0x00, 0x00,   img_kb_list.Margin.Left + 2 , img_kb_list.Margin.Top + 2 ,  img_kb_list.Margin.Left+img_kb_list.Width - 2, img_kb_list.Margin.Top+img_kb_list.Height - 2,  										 67,   0),
                new key("Rctrl"		  ,       0xe01d, false, false, 0x00, 0x00, 0x00,   img_kb_Rctrl.Margin.Left + 2 , img_kb_Rctrl.Margin.Top + 2 ,  img_kb_Rctrl.Margin.Left+img_kb_Rctrl.Width - 2, img_kb_Rctrl.Margin.Top+img_kb_Rctrl.Height - 2,  								 73,   0),
                new key("left"		  ,       0xe046, false, false, 0x00, 0x00, 0x00,   img_kb_left.Margin.Left + 2 , img_kb_left.Margin.Top + 2 ,  img_kb_left.Margin.Left+img_kb_left.Width - 2, img_kb_left.Margin.Top+img_kb_left.Height - 2,     									 79,   0),
                new key("down"		  ,       0xe050, false, false, 0x00, 0x00, 0x00,   img_kb_down.Margin.Left + 2 , img_kb_down.Margin.Top + 2 ,  img_kb_down.Margin.Left+img_kb_down.Width - 2, img_kb_down.Margin.Top+img_kb_down.Height - 2,     									 85,   0),
                new key("right"		  ,       0xe04d, false, false, 0x00, 0x00, 0x00,   img_kb_right.Margin.Left + 2 , img_kb_right.Margin.Top + 2 ,  img_kb_right.Margin.Left+img_kb_right.Width - 2, img_kb_right.Margin.Top+img_kb_right.Height - 2,  								 91,   0),//100
                new key("n0"		  ,       0x52, false, false, 0x00, 0x00, 0x00,     img_kb_n0.Margin.Left + 2 , img_kb_n0.Margin.Top + 2 ,  img_kb_n0.Margin.Left+img_kb_n0.Width - 2, img_kb_n0.Margin.Top+img_kb_n0.Height - 2,  													 97,   0),
                new key("ndot"		  ,       0x0053, false, false, 0x00, 0x00, 0x00,   img_kb_ndot.Margin.Left + 2 , img_kb_ndot.Margin.Top + 2 ,  img_kb_ndot.Margin.Left+img_kb_ndot.Width - 2, img_kb_ndot.Margin.Top+img_kb_ndot.Height - 2,  										103,   0),  //102

             };
            m_WelKeyList = new List<key>
            {
                 new key("esc"         ,       0x01, false, false, 0x00, 0x00, 0x00,    img_kb_esc.Margin.Left + 2 , img_kb_esc.Margin.Top + 2 ,  img_kb_esc.Margin.Left+img_kb_esc.Width - 2, img_kb_esc.Margin.Top+img_kb_esc.Height - 2,                                              6,   0),
                new key("f1"          ,       0x3B, false, false, 0x00, 0x00, 0x00,     img_kb_f1.Margin.Left + 2 , img_kb_f1.Margin.Top + 2 ,  img_kb_f1.Margin.Left+img_kb_f1.Width - 2, img_kb_f1.Margin.Top+img_kb_f1.Height - 2,                                                   12,   0),
                new key("f2"          ,       0x3C, false, false, 0x00, 0x00, 0x00,     img_kb_f2.Margin.Left + 2 , img_kb_f2.Margin.Top + 2 ,  img_kb_f2.Margin.Left+img_kb_f2.Width - 2, img_kb_f2.Margin.Top+img_kb_f2.Height - 2,                                                   18,   0),
                new key("f3"          ,       0x3D, false, false, 0x00, 0x00, 0x00,     img_kb_f3.Margin.Left + 2 , img_kb_f3.Margin.Top + 2 ,  img_kb_f3.Margin.Left+img_kb_f3.Width - 2, img_kb_f3.Margin.Top+img_kb_f3.Height - 2,                                                   24,   0),
                new key("f4"          ,       0x3E, false, false, 0x00, 0x00, 0x00,     img_kb_f4.Margin.Left + 2 , img_kb_f4.Margin.Top + 2 ,  img_kb_f4.Margin.Left+img_kb_f4.Width - 2, img_kb_f4.Margin.Top+img_kb_f4.Height - 2,                                                   30,   0),
                new key("f5"          ,       0x3F, false, false, 0x00, 0x00, 0x00,     img_kb_f5.Margin.Left + 2 , img_kb_f5.Margin.Top + 2 ,  img_kb_f5.Margin.Left+img_kb_f5.Width - 2, img_kb_f5.Margin.Top+img_kb_f5.Height - 2,                                                   36,   0),
                new key("f6"          ,       0x40, false, false, 0x00, 0x00, 0x00,     img_kb_f6.Margin.Left + 2 , img_kb_f6.Margin.Top + 2 ,  img_kb_f6.Margin.Left+img_kb_f6.Width - 2, img_kb_f6.Margin.Top+img_kb_f6.Height - 2,                                                   42,   0),
                new key("f7"          ,       0x41, false, false, 0x00, 0x00, 0x00,     img_kb_f7.Margin.Left + 2 , img_kb_f7.Margin.Top + 2 ,  img_kb_f7.Margin.Left+img_kb_f7.Width - 2, img_kb_f7.Margin.Top+img_kb_f7.Height - 2,                                                   48,   0),
                new key("f8"          ,       0x42, false, false, 0x00, 0x00, 0x00,     img_kb_f8.Margin.Left + 2 , img_kb_f8.Margin.Top + 2 ,  img_kb_f8.Margin.Left+img_kb_f8.Width - 2, img_kb_f8.Margin.Top+img_kb_f8.Height - 2,                                                   54,   0),
                new key("f9"          ,       0x43, false, false, 0x00, 0x00, 0x00,     img_kb_f9.Margin.Left + 2 , img_kb_f9.Margin.Top + 2 ,  img_kb_f9.Margin.Left+img_kb_f9.Width - 2, img_kb_f9.Margin.Top+img_kb_f9.Height - 2,                                                   60,   0),//10
                new key("f10"         ,       0x44, false, false, 0x00, 0x00, 0x00,     img_kb_f10.Margin.Left + 2 , img_kb_f10.Margin.Top + 2 ,  img_kb_f10.Margin.Left+img_kb_f10.Width - 2, img_kb_f10.Margin.Top+img_kb_f10.Height - 2,                                             66,   0),
                new key("f11"         ,       0x45, false, false, 0x00, 0x00, 0x00,     img_kb_f11.Margin.Left + 2 , img_kb_f11.Margin.Top + 2 ,  img_kb_f11.Margin.Left+img_kb_f11.Width - 2, img_kb_f11.Margin.Top+img_kb_f11.Height - 2,                                             72,   0),
                new key("f12"         ,       0x46, false, false, 0x00, 0x00, 0x00,     img_kb_f12.Margin.Left + 2 , img_kb_f12.Margin.Top + 2 ,  img_kb_f12.Margin.Left+img_kb_f12.Width - 2, img_kb_f12.Margin.Top+img_kb_f12.Height - 2,                                             78,   0),
                new key("prtsc"       ,       0x37, false, false, 0x00, 0x00, 0x00,     img_kb_prtsc.Margin.Left + 2 , img_kb_prtsc.Margin.Top + 2 ,  img_kb_prtsc.Margin.Left+img_kb_prtsc.Width - 2, img_kb_prtsc.Margin.Top+img_kb_prtsc.Height - 2,                                 84,   0), //14
                //new key("insert"      ,       0x52, false, false, 0x00, 0x00, 0x00, 515, 11, 549, 38,   90,   0),
                new key("del"         ,       0x53, false, false, 0x00, 0x00, 0x00,     img_kb_del.Margin.Left + 2 , img_kb_del.Margin.Top + 2 ,  img_kb_del.Margin.Left+img_kb_del.Width - 2, img_kb_del.Margin.Top+img_kb_del.Height - 2,                                             90,   0),
                new key("home"        ,       0x47, false, false, 0x00, 0x00, 0x00,     img_kb_home.Margin.Left + 2 , img_kb_home.Margin.Top + 2 ,  img_kb_home.Margin.Left+img_kb_home.Width - 2, img_kb_home.Margin.Top+img_kb_home.Height - 2,                                       96,   0),
                new key("end"         ,       0x4F, false, false, 0x00, 0x00, 0x00,     img_kb_end.Margin.Left + 2 , img_kb_end.Margin.Top + 2 ,  img_kb_end.Margin.Left+img_kb_end.Width - 2, img_kb_end.Margin.Top+img_kb_end.Height - 2,                                             102,   0),
                new key("pgup"        ,       0x49, false, false, 0x00, 0x00, 0x00,     img_kb_pgup.Margin.Left + 2 , img_kb_pgup.Margin.Top + 2 ,  img_kb_pgup.Margin.Left+img_kb_pgup.Width - 2, img_kb_pgup.Margin.Top+img_kb_pgup.Height - 2,                                       108,   0),
                new key("pgdn"        ,       0x51, false, false, 0x00, 0x00, 0x00,     img_kb_pgdn.Margin.Left + 2 , img_kb_pgdn.Margin.Top + 2 ,  img_kb_pgdn.Margin.Left+img_kb_pgdn.Width - 2, img_kb_pgdn.Margin.Top+img_kb_pgdn.Height - 2,                                       114,   0),
                new key("Quotes"      ,       0x29, false, false, 0x00, 0x00, 0x00,     img_kb_Quotes.Margin.Left + 2 , img_kb_Quotes.Margin.Top + 2 ,  img_kb_Quotes.Margin.Left+img_kb_Quotes.Width - 2, img_kb_Quotes.Margin.Top+img_kb_Quotes.Height - 2,                             5,   0),//20
                new key("1"           ,       0x02, false, false, 0x00, 0x00, 0x00,     img_kb_1.Margin.Left + 2 , img_kb_1.Margin.Top + 2 ,  img_kb_1.Margin.Left+img_kb_1.Width - 2, img_kb_1.Margin.Top+img_kb_1.Height - 2,                                                          11,   0),
                new key("2"           ,       0x03, false, false, 0x00, 0x00, 0x00,     img_kb_2.Margin.Left + 2 , img_kb_2.Margin.Top + 2 ,  img_kb_2.Margin.Left+img_kb_2.Width - 2, img_kb_2.Margin.Top+img_kb_2.Height - 2,                                                          17,   0),
                new key("3"           ,       0x04, false, false, 0x00, 0x00, 0x00,     img_kb_3.Margin.Left + 2 , img_kb_3.Margin.Top + 2 ,  img_kb_3.Margin.Left+img_kb_3.Width - 2, img_kb_3.Margin.Top+img_kb_3.Height - 2,                                                          23,   0),
                new key("4"           ,       0x05, false, false, 0x00, 0x00, 0x00,     img_kb_4.Margin.Left + 2 , img_kb_4.Margin.Top + 2 ,  img_kb_4.Margin.Left+img_kb_4.Width - 2, img_kb_4.Margin.Top+img_kb_4.Height - 2,                                                          29,   0),
                new key("5"           ,       0x06, false, false, 0x00, 0x00, 0x00,     img_kb_5.Margin.Left + 2 , img_kb_5.Margin.Top + 2 ,  img_kb_5.Margin.Left+img_kb_5.Width - 2, img_kb_5.Margin.Top+img_kb_5.Height - 2,                                                          35,   0),
                new key("6"           ,       0x07, false, false, 0x00, 0x00, 0x00,     img_kb_6.Margin.Left + 2 , img_kb_6.Margin.Top + 2 ,  img_kb_6.Margin.Left+img_kb_6.Width - 2, img_kb_6.Margin.Top+img_kb_6.Height - 2,                                                          41,   0),
                new key("7"           ,       0x08, false, false, 0x00, 0x00, 0x00,     img_kb_7.Margin.Left + 2 , img_kb_7.Margin.Top + 2 ,  img_kb_7.Margin.Left+img_kb_7.Width - 2, img_kb_7.Margin.Top+img_kb_7.Height - 2,                                                          47,   0),
                new key("8"           ,       0x09, false, false, 0x00, 0x00, 0x00,     img_kb_8.Margin.Left + 2 , img_kb_8.Margin.Top + 2 ,  img_kb_8.Margin.Left+img_kb_8.Width - 2, img_kb_8.Margin.Top+img_kb_8.Height - 2,                                                          53,   0),
                new key("9"           ,       0x0A, false, false, 0x00, 0x00, 0x00,     img_kb_9.Margin.Left + 2 , img_kb_9.Margin.Top + 2 ,  img_kb_9.Margin.Left+img_kb_9.Width - 2, img_kb_9.Margin.Top+img_kb_9.Height - 2,                                                          59,   0),
                new key("0"           ,       0x0B, false, false, 0x00, 0x00, 0x00,     img_kb_0.Margin.Left + 2 , img_kb_0.Margin.Top + 2 ,  img_kb_0.Margin.Left+img_kb_0.Width - 2, img_kb_0.Margin.Top+img_kb_0.Height - 2,                                                          65,   0),  //30
                new key("underline"   ,       0x0C, false, false, 0x00, 0x00, 0x00,     img_kb_underline.Margin.Left + 2 , img_kb_underline.Margin.Top + 2 ,  img_kb_underline.Margin.Left+img_kb_underline.Width - 2, img_kb_underline.Margin.Top+img_kb_underline.Height - 2,          71,   0),
                new key("equal"      ,       0x0D, false, false, 0x00, 0x00, 0x00,      img_kb_equal.Margin.Left + 2 , img_kb_equal.Margin.Top + 2 ,  img_kb_equal.Margin.Left+img_kb_equal.Width - 2, img_kb_equal.Margin.Top+img_kb_equal.Height - 2,   77,   0),
                new key("backspace"   ,       0x0E, false, false, 0x00, 0x00, 0x00,     img_kb_backspace.Margin.Left + 2 , img_kb_backspace.Margin.Top + 2 ,  img_kb_backspace.Margin.Left+img_kb_backspace.Width - 2, img_kb_backspace.Margin.Top+img_kb_backspace.Height - 2,          89,   0),
                new key("numlock"     ,       0x45, false, false, 0x00, 0x00, 0x00,     img_kb_numlock.Margin.Left + 2 , img_kb_numlock.Margin.Top + 2 ,  img_kb_numlock.Margin.Left+img_kb_numlock.Width - 2, img_kb_numlock.Margin.Top+img_kb_numlock.Height - 2,                      95,   0),
                new key("ndevide"     , (ushort)0xE035, false, false, 0x00, 0x00, 0x00, img_kb_ndevide.Margin.Left + 2 , img_kb_ndevide.Margin.Top + 2 ,  img_kb_ndevide.Margin.Left+img_kb_ndevide.Width - 2, img_kb_ndevide.Margin.Top+img_kb_ndevide.Height - 2,                     101,   0),
                new key("nmultiply"   ,       0x37, false, false, 0x00, 0x00, 0x00,     img_kb_nmultiply.Margin.Left + 2 , img_kb_nmultiply.Margin.Top + 2 ,  img_kb_nmultiply.Margin.Left+img_kb_nmultiply.Width - 2, img_kb_nmultiply.Margin.Top+img_kb_nmultiply.Height - 2,         107,   0),
                new key("nminus"       ,       0x4A, false, false, 0x00, 0x00, 0x00,    img_kb_nminus.Margin.Left + 2 , img_kb_nminus.Margin.Top + 2 ,  img_kb_nminus.Margin.Left+img_kb_nminus.Width - 2, img_kb_nminus.Margin.Top+img_kb_nminus.Height - 2,                           113,   0),
                new key("tab"         ,       0x0F, false, false, 0x00, 0x00, 0x00,     img_kb_tab.Margin.Left + 2 , img_kb_tab.Margin.Top + 2 ,  img_kb_tab.Margin.Left+img_kb_tab.Width - 2, img_kb_tab.Margin.Top+img_kb_tab.Height - 2,                                               4,   0),
                new key("q"           ,       0x10, false, false, 0x00, 0x00, 0x00,     img_kb_q.Margin.Left + 2 , img_kb_q.Margin.Top + 2 ,  img_kb_q.Margin.Left+img_kb_q.Width - 2, img_kb_q.Margin.Top+img_kb_q.Height - 2,                                                          16,   0),
                new key("w"           ,       0x11, false, false, 0x00, 0x00, 0x00,     img_kb_w.Margin.Left + 2 , img_kb_w.Margin.Top + 2 ,  img_kb_w.Margin.Left+img_kb_w.Width - 2, img_kb_w.Margin.Top+img_kb_w.Height - 2,                                                          22,   0),   //40
                new key("e"           ,       0x12, false, false, 0x00, 0x00, 0x00,     img_kb_e.Margin.Left + 2 , img_kb_e.Margin.Top + 2 ,  img_kb_e.Margin.Left+img_kb_e.Width - 2, img_kb_e.Margin.Top+img_kb_e.Height - 2,                                                          28,   0),
                new key("r"           ,       0x13, false, false, 0x00, 0x00, 0x00,     img_kb_r.Margin.Left + 2 , img_kb_r.Margin.Top + 2 ,  img_kb_r.Margin.Left+img_kb_r.Width - 2, img_kb_r.Margin.Top+img_kb_r.Height - 2,                                                          34,   0),
                new key("t"           ,       0x14, false, false, 0x00, 0x00, 0x00,     img_kb_t.Margin.Left + 2 , img_kb_t.Margin.Top + 2 ,  img_kb_t.Margin.Left+img_kb_t.Width - 2, img_kb_t.Margin.Top+img_kb_t.Height - 2,                                                          40,   0),
                new key("y"           ,       0x15, false, false, 0x00, 0x00, 0x00,     img_kb_y.Margin.Left + 2 , img_kb_y.Margin.Top + 2 ,  img_kb_y.Margin.Left+img_kb_y.Width - 2, img_kb_y.Margin.Top+img_kb_y.Height - 2,                                                          46,   0),
                new key("u"           ,       0x16, false, false, 0x00, 0x00, 0x00,     img_kb_u.Margin.Left + 2 , img_kb_u.Margin.Top + 2 ,  img_kb_u.Margin.Left+img_kb_u.Width - 2, img_kb_u.Margin.Top+img_kb_u.Height - 2,                                                          52,   0),
                new key("i"           ,       0x17, false, false, 0x00, 0x00, 0x00,     img_kb_i.Margin.Left + 2 , img_kb_i.Margin.Top + 2 ,  img_kb_i.Margin.Left+img_kb_i.Width - 2, img_kb_i.Margin.Top+img_kb_i.Height - 2,                                                          58,   0),
                new key("o"           ,       0x18, false, false, 0x00, 0x00, 0x00,     img_kb_o.Margin.Left + 2 , img_kb_o.Margin.Top + 2 ,  img_kb_o.Margin.Left+img_kb_o.Width - 2, img_kb_o.Margin.Top+img_kb_o.Height - 2,                                                          64,   0),
                new key("p"           ,       0x19, false, false, 0x00, 0x00, 0x00,     img_kb_p.Margin.Left + 2 , img_kb_p.Margin.Top + 2 ,  img_kb_p.Margin.Left+img_kb_p.Width - 2, img_kb_p.Margin.Top+img_kb_p.Height - 2,                                                          70,   0),
                new key("Lparantheses",       0x1A, false, false, 0x00, 0x00, 0x00,     img_kb_Lparantheses.Margin.Left + 2 , img_kb_Lparantheses.Margin.Top + 2 ,  img_kb_Lparantheses.Margin.Left+img_kb_Lparantheses.Width - 2, img_kb_Lparantheses.Margin.Top+img_kb_Lparantheses.Height - 2,  76,   0),
                new key("Rparantheses",       0x1B, false, false, 0x00, 0x00, 0x00,     img_kb_Rparantheses.Margin.Left + 2 , img_kb_Rparantheses.Margin.Top + 2 ,  img_kb_Rparantheses.Margin.Left+img_kb_Rparantheses.Width - 2, img_kb_Rparantheses.Margin.Top+img_kb_Rparantheses.Height - 2,  82,   0),//50
                new key("enter"       ,       0x2B, false, false, 0x00, 0x00, 0x00,     img_kb_enter.Margin.Left + 2 , img_kb_enter.Margin.Top + 2 ,  img_kb_enter.Margin.Left+img_kb_enter.Width - 2, img_kb_enter.Margin.Top+img_kb_enter.Height - 2,                                  88,   0),
                new key("n7"          ,       0x47, false, false, 0x00, 0x00, 0x00,     img_kb_n7.Margin.Left + 2 , img_kb_n7.Margin.Top + 2 ,  img_kb_n7.Margin.Left+img_kb_n7.Width - 2, img_kb_n7.Margin.Top+img_kb_n7.Height - 2,                                                    94,   0),
                new key("n8"          ,       0x48, false, false, 0x00, 0x00, 0x00,     img_kb_n8.Margin.Left + 2 , img_kb_n8.Margin.Top + 2 ,  img_kb_n8.Margin.Left+img_kb_n8.Width - 2, img_kb_n8.Margin.Top+img_kb_n8.Height - 2,                                                   100,   0),
                new key("n9"          ,       0x49, false, false, 0x00, 0x00, 0x00,     img_kb_n9.Margin.Left + 2 , img_kb_n9.Margin.Top + 2 ,  img_kb_n9.Margin.Left+img_kb_n9.Width - 2, img_kb_n9.Margin.Top+img_kb_n9.Height - 2,                                                   106,   0),
                new key("nplus"       ,       0x4E, false, false, 0x00, 0x00, 0x00,     img_kb_nplus.Margin.Left + 2 , img_kb_nplus.Margin.Top + 2 ,  img_kb_nplus.Margin.Left+img_kb_nplus.Width - 2, img_kb_nplus.Margin.Top+img_kb_nplus.Height - 2,                                 112,   0),
                new key("capslock"    ,       0x3A, false, false, 0x00, 0x00, 0x00,     img_kb_capslock.Margin.Left + 2 , img_kb_capslock.Margin.Top + 2 ,  img_kb_capslock.Margin.Left+img_kb_capslock.Width - 2, img_kb_capslock.Margin.Top+img_kb_capslock.Height - 2,                 3,   0),
                new key("a"           ,       0x1E, false, false, 0x00, 0x00, 0x00,     img_kb_a.Margin.Left + 2 , img_kb_a.Margin.Top + 2 ,  img_kb_a.Margin.Left+img_kb_a.Width - 2, img_kb_a.Margin.Top+img_kb_a.Height - 2,                                                          15,   0),
                new key("s"           ,       0x1F, false, false, 0x00, 0x00, 0x00,     img_kb_s.Margin.Left + 2 , img_kb_s.Margin.Top + 2 ,  img_kb_s.Margin.Left+img_kb_s.Width - 2, img_kb_s.Margin.Top+img_kb_s.Height - 2,                                                          21,   0),
                new key("d"           ,       0x20, false, false, 0x00, 0x00, 0x00,     img_kb_d.Margin.Left + 2 , img_kb_d.Margin.Top + 2 ,  img_kb_d.Margin.Left+img_kb_d.Width - 2, img_kb_d.Margin.Top+img_kb_d.Height - 2,                                                          27,   0),
                new key("f"           ,       0x21, false, false, 0x00, 0x00, 0x00,     img_kb_f.Margin.Left + 2 , img_kb_f.Margin.Top + 2 ,  img_kb_f.Margin.Left+img_kb_f.Width - 2, img_kb_f.Margin.Top+img_kb_f.Height - 2,                                                          33,   0),//60
                new key("g"           ,       0x22, false, false, 0x00, 0x00, 0x00,     img_kb_g.Margin.Left + 2 , img_kb_g.Margin.Top + 2 ,  img_kb_g.Margin.Left+img_kb_g.Width - 2, img_kb_g.Margin.Top+img_kb_g.Height - 2,                                                          39,   0),
                new key("h"           ,       0x23, false, false, 0x00, 0x00, 0x00,     img_kb_h.Margin.Left + 2 , img_kb_h.Margin.Top + 2 ,  img_kb_h.Margin.Left+img_kb_h.Width - 2, img_kb_h.Margin.Top+img_kb_h.Height - 2,                                                          45,   0),
                new key("j"           ,       0x24, false, false, 0x00, 0x00, 0x00,     img_kb_j.Margin.Left + 2 , img_kb_j.Margin.Top + 2 ,  img_kb_j.Margin.Left+img_kb_j.Width - 2, img_kb_j.Margin.Top+img_kb_j.Height - 2,                                                          51,   0),
                new key("k"           ,       0x25, false, false, 0x00, 0x00, 0x00,     img_kb_k.Margin.Left + 2 , img_kb_k.Margin.Top + 2 ,  img_kb_k.Margin.Left+img_kb_k.Width - 2, img_kb_k.Margin.Top+img_kb_k.Height - 2,                                                          57,   0),
                new key("l"           ,       0x26, false, false, 0x00, 0x00, 0x00,     img_kb_l.Margin.Left + 2 , img_kb_l.Margin.Top + 2 ,  img_kb_l.Margin.Left+img_kb_l.Width - 2, img_kb_l.Margin.Top+img_kb_l.Height - 2,                                                          63,   0),
                new key("colon"       ,       0x27, false, false, 0x00, 0x00, 0x00,     img_kb_colon.Margin.Left + 2 , img_kb_colon.Margin.Top + 2 ,  img_kb_colon.Margin.Left+img_kb_colon.Width - 2, img_kb_colon.Margin.Top+img_kb_colon.Height - 2,                                  69,   0),
                new key("at"          ,       0x28, false, false, 0x00, 0x00, 0x00,     img_kb_at.Margin.Left + 2 , img_kb_at.Margin.Top + 2 ,  img_kb_at.Margin.Left+img_kb_at.Width - 2, img_kb_at.Margin.Top+img_kb_at.Height - 2,                                                    75,   0),
                new key("hashtag"     ,       0x1c, false, false, 0x00, 0x00, 0x00,     img_kb_hashtag.Margin.Left + 2 , img_kb_hashtag.Margin.Top + 2 ,  img_kb_hashtag.Margin.Left+img_kb_hashtag.Width - 2, img_kb_hashtag.Margin.Top+img_kb_hashtag.Height - 2,                      81,  87),
                new key("n4"          ,       0x4b, false, false, 0x00, 0x00, 0x00,     img_kb_n4.Margin.Left + 2 , img_kb_n4.Margin.Top + 2 ,  img_kb_n4.Margin.Left+img_kb_n4.Width - 2, img_kb_n4.Margin.Top+img_kb_n4.Height - 2,                                                    93,   0),
                new key("n5"          ,       0x4c, false, false, 0x00, 0x00, 0x00,     img_kb_n5.Margin.Left + 2 , img_kb_n5.Margin.Top + 2 ,  img_kb_n5.Margin.Left+img_kb_n5.Width - 2, img_kb_n5.Margin.Top+img_kb_n5.Height - 2,                                                    99,   0),//70
                new key("n6"          ,       0x4d, false, false, 0x00, 0x00, 0x00,     img_kb_n6.Margin.Left + 2 , img_kb_n6.Margin.Top + 2 ,  img_kb_n6.Margin.Left+img_kb_n6.Width - 2, img_kb_n6.Margin.Top+img_kb_n6.Height - 2,                                                   105,   0),
                new key("Lshift"      ,       0x2a, false, false, 0x00, 0x00, 0x00,     img_kb_Lshift.Margin.Left + 2 , img_kb_Lshift.Margin.Top + 2 ,  img_kb_Lshift.Margin.Left+img_kb_Lshift.Width - 2, img_kb_Lshift.Margin.Top+img_kb_Lshift.Height - 2,                             2,   0),
                new key("backslash"   ,       0x2a, false, false, 0x00, 0x00, 0x00,     img_kb_backslash.Margin.Left + 2 , img_kb_backslash.Margin.Top + 2 ,  img_kb_backslash.Margin.Left+img_kb_backslash.Width - 2, img_kb_backslash.Margin.Top+img_kb_backslash.Height - 2,          14,   0),
                new key("z"           ,       0x2c, false, false, 0x00, 0x00, 0x00,     img_kb_z.Margin.Left + 2 , img_kb_z.Margin.Top + 2 ,  img_kb_z.Margin.Left+img_kb_z.Width - 2, img_kb_z.Margin.Top+img_kb_z.Height - 2,                                                          20,   0),
                new key("x"           ,       0x2d, false, false, 0x00, 0x00, 0x00,     img_kb_x.Margin.Left + 2 , img_kb_x.Margin.Top + 2 ,  img_kb_x.Margin.Left+img_kb_x.Width - 2, img_kb_x.Margin.Top+img_kb_x.Height - 2,                                                          26,   0),
                new key("c"           ,       0x2e, false, false, 0x00, 0x00, 0x00,     img_kb_c.Margin.Left + 2 , img_kb_c.Margin.Top + 2 ,  img_kb_c.Margin.Left+img_kb_c.Width - 2, img_kb_c.Margin.Top+img_kb_c.Height - 2,                                                          32,   0),
                new key("v"           ,       0x2f, false, false, 0x00, 0x00, 0x00,     img_kb_v.Margin.Left + 2 , img_kb_v.Margin.Top + 2 ,  img_kb_v.Margin.Left+img_kb_v.Width - 2, img_kb_v.Margin.Top+img_kb_v.Height - 2,                                                          38,   0),
                new key("b"           ,       0x30, false, false, 0x00, 0x00, 0x00,     img_kb_b.Margin.Left + 2 , img_kb_b.Margin.Top + 2 ,  img_kb_b.Margin.Left+img_kb_b.Width - 2, img_kb_b.Margin.Top+img_kb_b.Height - 2,                                                          44,   0),
                new key("n"           ,       0x31, false, false, 0x00, 0x00, 0x00,     img_kb_n.Margin.Left + 2 , img_kb_n.Margin.Top + 2 ,  img_kb_n.Margin.Left+img_kb_n.Width - 2, img_kb_n.Margin.Top+img_kb_n.Height - 2,                                                          50,   0),
                new key("m"           ,       0x32, false, false, 0x00, 0x00, 0x00,     img_kb_m.Margin.Left + 2 , img_kb_m.Margin.Top + 2 ,  img_kb_m.Margin.Left+img_kb_m.Width - 2, img_kb_m.Margin.Top+img_kb_m.Height - 2,                                                          56,   0),//80
                new key("less"        ,       0x33, false, false, 0x00, 0x00, 0x00,     img_kb_less.Margin.Left + 2 , img_kb_less.Margin.Top + 2 ,  img_kb_less.Margin.Left+img_kb_less.Width - 2, img_kb_less.Margin.Top+img_kb_less.Height - 2,                                        62,   0),
                new key("more"        ,       0x34, false, false, 0x00, 0x00, 0x00,     img_kb_more.Margin.Left + 2 , img_kb_more.Margin.Top + 2 ,  img_kb_more.Margin.Left+img_kb_more.Width - 2, img_kb_more.Margin.Top+img_kb_more.Height - 2,                                        68,   0),
                new key("question"    ,       0x35, false, false, 0x00, 0x00, 0x00,     img_kb_question.Margin.Left + 2 , img_kb_question.Margin.Top + 2 ,  img_kb_question.Margin.Left+img_kb_question.Width - 2, img_kb_question.Margin.Top+img_kb_question.Height - 2,                74,   0),
                new key("Rshift"      ,       0x36, false, false, 0x00, 0x00, 0x00,     img_kb_Rshift.Margin.Left + 2 , img_kb_Rshift.Margin.Top + 2 ,  img_kb_Rshift.Margin.Left+img_kb_Rshift.Width - 2, img_kb_Rshift.Margin.Top+img_kb_Rshift.Height - 2,                            80,   0),
                new key("up"          ,       0xe048, false, false, 0x00, 0x00, 0x00,   img_kb_up.Margin.Left + 2 , img_kb_up.Margin.Top + 2 ,  img_kb_up.Margin.Left+img_kb_up.Width - 2, img_kb_up.Margin.Top+img_kb_up.Height - 2,                                                    86,   0),
                new key("n1"          ,       0x4f, false, false, 0x00, 0x00, 0x00,     img_kb_n1.Margin.Left + 2 , img_kb_n1.Margin.Top + 2 ,  img_kb_n1.Margin.Left+img_kb_n1.Width - 2, img_kb_n1.Margin.Top+img_kb_n1.Height - 2,                                                    92,   0),
                new key("n2"          ,       0x50, false, false, 0x00, 0x00, 0x00,     img_kb_n2.Margin.Left + 2 , img_kb_n2.Margin.Top + 2 ,  img_kb_n2.Margin.Left+img_kb_n2.Width - 2, img_kb_n2.Margin.Top+img_kb_n2.Height - 2,                                                    98,    0),
                new key("n3"          ,       0x51, false, false, 0x00, 0x00, 0x00,     img_kb_n3.Margin.Left + 2 , img_kb_n3.Margin.Top + 2 ,  img_kb_n3.Margin.Left+img_kb_n3.Width - 2, img_kb_n3.Margin.Top+img_kb_n3.Height - 2,                                                   104,   0),
                new key("nenter"      ,       0xe01c, false, false, 0x00, 0x00, 0x00,   img_kb_nenter.Margin.Left + 2 , img_kb_nenter.Margin.Top + 2 ,  img_kb_nenter.Margin.Left+img_kb_nenter.Width - 2, img_kb_nenter.Margin.Top+img_kb_nenter.Height - 2,                           110,   0),
                new key("Lctrl"       ,       0x1d, false, false, 0x00, 0x00, 0x00,     img_kb_Lctrl.Margin.Left + 2 , img_kb_Lctrl.Margin.Top + 2 ,  img_kb_Lctrl.Margin.Left+img_kb_Lctrl.Width - 2, img_kb_Lctrl.Margin.Top+img_kb_Lctrl.Height - 2,                                   1,   0),//90
                new key("fn"          ,       0xffff, false, false, 0x00, 0x00, 0x00,   img_kb_fn.Margin.Left + 2 , img_kb_fn.Margin.Top + 2 ,  img_kb_fn.Margin.Left+img_kb_fn.Width - 2, img_kb_fn.Margin.Top+img_kb_fn.Height - 2,                                                    13,   0),
                new key("windows"     ,       0xffff, false, false, 0x00, 0x00, 0x00,   img_kb_windows.Margin.Left + 2 , img_kb_windows.Margin.Top + 2 ,  img_kb_windows.Margin.Left+img_kb_windows.Width - 2, img_kb_windows.Margin.Top+img_kb_windows.Height - 2,                      19,   0),
                new key("Lalt"        ,       0x38, false, false, 0x00, 0x00, 0x00,     img_kb_Lalt.Margin.Left + 2 , img_kb_Lalt.Margin.Top + 2 ,  img_kb_Lalt.Margin.Left+img_kb_Lalt.Width - 2, img_kb_Lalt.Margin.Top+img_kb_Lalt.Height - 2,                                        25,   0),
                new key("space"       ,       0x39, false, false, 0x00, 0x00, 0x00,     img_kb_space.Margin.Left + 2 , img_kb_space.Margin.Top + 2 ,  img_kb_space.Margin.Left+img_kb_space.Width - 2, img_kb_space.Margin.Top+img_kb_space.Height - 2,                                  43,   0),
                new key("altgr"       ,       0xe038, false, false, 0x00, 0x00, 0x00,   img_kb_altgr.Margin.Left + 2 , img_kb_altgr.Margin.Top + 2 ,  img_kb_altgr.Margin.Left+img_kb_altgr.Width - 2, img_kb_altgr.Margin.Top+img_kb_altgr.Height - 2,                                  61,   0),
                new key("list"        ,       0xffff, false, false, 0x00, 0x00, 0x00,   img_kb_list.Margin.Left + 2 , img_kb_list.Margin.Top + 2 ,  img_kb_list.Margin.Left+img_kb_list.Width - 2, img_kb_list.Margin.Top+img_kb_list.Height - 2,                                        67,   0),
                new key("Rctrl"       ,       0xe01d, false, false, 0x00, 0x00, 0x00,   img_kb_Rctrl.Margin.Left + 2 , img_kb_Rctrl.Margin.Top + 2 ,  img_kb_Rctrl.Margin.Left+img_kb_Rctrl.Width - 2, img_kb_Rctrl.Margin.Top+img_kb_Rctrl.Height - 2,                                  73,   0),
                new key("left"        ,       0xe046, false, false, 0x00, 0x00, 0x00,   img_kb_left.Margin.Left + 2 , img_kb_left.Margin.Top + 2 ,  img_kb_left.Margin.Left+img_kb_left.Width - 2, img_kb_left.Margin.Top+img_kb_left.Height - 2,                                        79,   0),
                new key("down"        ,       0xe050, false, false, 0x00, 0x00, 0x00,   img_kb_down.Margin.Left + 2 , img_kb_down.Margin.Top + 2 ,  img_kb_down.Margin.Left+img_kb_down.Width - 2, img_kb_down.Margin.Top+img_kb_down.Height - 2,                                        85,   0),
                new key("right"       ,       0xe04d, false, false, 0x00, 0x00, 0x00,   img_kb_right.Margin.Left + 2 , img_kb_right.Margin.Top + 2 ,  img_kb_right.Margin.Left+img_kb_right.Width - 2, img_kb_right.Margin.Top+img_kb_right.Height - 2,                                  91,   0),//100
                new key("n0"          ,       0x52, false, false, 0x00, 0x00, 0x00,     img_kb_n0.Margin.Left + 2 , img_kb_n0.Margin.Top + 2 ,  img_kb_n0.Margin.Left+img_kb_n0.Width - 2, img_kb_n0.Margin.Top+img_kb_n0.Height - 2,                                                    97,   0),
                new key("ndot"        ,       0x0053, false, false, 0x00, 0x00, 0x00,   img_kb_ndot.Margin.Left + 2 , img_kb_ndot.Margin.Top + 2 ,  img_kb_ndot.Margin.Left+img_kb_ndot.Width - 2, img_kb_ndot.Margin.Top+img_kb_ndot.Height - 2,                                       103,   0),  //102

             };
        }

        private void InitializeLayout()
        {
      
        }
        public KBTable102()
        {
            InitializeComponent();
            InitializeParameter();
            InitializeLayout();
        }

        public void SetKeyBoardColor(uint current_mode, byte R, byte G, byte B)
        {
            List<key> currentKeyList;
            if (current_mode == (uint)mode.windows)
                currentKeyList = m_KeyList102;
            else if (current_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList102;

            foreach (key akey in currentKeyList)
            {
                if (akey.forcused == true)
                {
                    akey.R = R;
                    akey.G = G;
                    akey.B = B;
                    string controlname = "rec_kb_" + akey.key_name;
                    if (akey.key_name != "enter")
                    {
                        Rectangle control = (Rectangle)FindName(controlname);
                        control.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                    }
                    else
                    {
                        Rectangle control = (Rectangle)FindName(controlname);
                        control.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                        Rectangle control1 = (Rectangle)FindName("rec_kb_enter1");
                        control1.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                    }


                    //Rectangle control = (Rectangle)FindName(controlname);
                    //control.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                }
            }

        }

        public void EnableKBLayout(bool enable, uint current_mode)
        {
            //if (m_KBlayout_enable == enable)   // !!!!!!! This will impact profile layout update.... !!!!! TBD item
            //    return;
            m_mode = current_mode;
            m_KBlayout_enable = enable;
            string sImgPath = "..\\image\\2ndKB_Img\\UK\\";
            if (m_KBlayout_enable == false)
            {
                foreach (key akey in m_KeyList102)
                {
                    akey.forcused = false;
                    

                    //back to keybaord normal status.
                    string uriname = sImgPath + "normal\\" + akey.key_name + "_n.png";
                    string controlname = "img_kb_" + akey.key_name;
                    var uriSource = new Uri(uriname, UriKind.Relative);
                    Image img = (Image)FindName(controlname);
                    img.Source = new BitmapImage(uriSource);
                    img.Visibility = Visibility.Visible;
                    
                    controlname = "rec_kb_" + akey.key_name;
                    if(akey.key_name != "enter"){
                        Rectangle rec = (Rectangle)FindName(controlname);
                        rec.Fill = null;
                    }
                    else
                    {
                        Rectangle rec = (Rectangle)FindName(controlname);
                        rec.Fill = null;
                        Rectangle rec1 = (Rectangle)FindName("rec_kb_enter1");
                        rec1.Fill = null;
                    }
                }
                foreach (key akey in m_WelKeyList)
                {
                    akey.forcused = false;
                    // Layout has been cleaned before ...
                }
                line_upper.Visibility = Visibility.Collapsed;
                line_lower.Visibility = Visibility.Collapsed;
                line_lefter.Visibility = Visibility.Collapsed;
                line_righter.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(current_mode == (uint)mode.windows)
                {
                    foreach (key akey in m_KeyList102)
                    {
                        //string controlname = "img_kb_" + akey.key_name;
                        //Image img = (Image)FindName(controlname);
                        //img.OpacityMask = OpacityMask(0xffffff);
                        string controlname = "rec_kb_" + akey.key_name;
                        if (akey.key_name != "enter")
                        {
                            Rectangle rec2 = (Rectangle)FindName(controlname);
                            rec2.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                        }
                        else
                        {
                            Rectangle rec3 = (Rectangle)FindName(controlname);
                            rec3.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                            Rectangle rec4 = (Rectangle)FindName("rec_kb_enter1");
                            rec4.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                        }
                    }
                }
                else if(current_mode == (uint)mode.welcome)
                {
                    foreach (key akey in m_WelKeyList)
                    {
                        string controlname = "rec_kb_" + akey.key_name;
                        if (akey.key_name != "enter")
                        {
                            Rectangle rec2 = (Rectangle)FindName(controlname);
                            rec2.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                        }
                        else
                        {
                            Rectangle rec3 = (Rectangle)FindName(controlname);
                            rec3.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                            Rectangle rec4 = (Rectangle)FindName("rec_kb_enter1");
                            rec4.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                        }
                    }
                }
                line_upper.Visibility = Visibility.Visible;
                line_lower.Visibility = Visibility.Visible;
                line_lefter.Visibility = Visibility.Visible;
                line_righter.Visibility = Visibility.Visible;
            }
        }

        private void setKeyCusored(double x, double y)
        {
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList102;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList102;


            // bool cursored = false;
            string uriname;
            string controlname;
            string sImgPath = "..\\image\\2ndKB_Img\\UK\\";
            foreach (key akey in currentKeyList)
            {
                if ((akey.x1 < x) && (akey.x2 > x) && (akey.y1 < y) && (akey.y2 > y))
                {
                    if (akey.forcused == false)
                    {
                        uriname = sImgPath + "hover\\" + akey.key_name + "_h.png";
                    }
                    else
                    {
                        uriname = sImgPath + "focus_hover\\" + akey.key_name + "_fh.png";
                    }
                    controlname = "img_kb_" + akey.key_name;
                    var uriSource = new Uri(uriname, UriKind.Relative);
                    Image img = (Image)FindName(controlname);
                    img.Source = new BitmapImage(uriSource);
                    akey.cursored = true;
                    // cursored = true;
                }
                else
                {
                    if (akey.cursored == true)
                    {

                        if (akey.forcused == true)
                            uriname = sImgPath + "focus\\" + akey.key_name + "_f.png";
                        else
                            uriname = sImgPath + "normal\\" + akey.key_name + "_n.png";
                        controlname = "img_kb_" + akey.key_name;
                        var uriSource = new Uri(uriname, UriKind.Relative);
                        Image img = (Image)FindName(controlname);
                        img.Source = new BitmapImage(uriSource);
                        akey.cursored = false;

                    }
                }
            }
        }
        private void setKeyRangeForcused(double x, double _x, double y, double _y)
        {
            string sImgPath = "..\\image\\2ndKB_Img\\UK\\";
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList102;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList102;

            foreach (key akey in currentKeyList)
            {
                if (((akey.x1 > x) && (akey.x1 < _x) && (akey.y1 > y) && (akey.y1 < _y)) ||
                    ((akey.x1 > x) && (akey.x1 < _x) && (akey.y2 > y) && (akey.y2 < _y)) ||
                    ((akey.x2 > x) && (akey.x2 < _x) && (akey.y1 > y) && (akey.y1 < _y)) ||
                    ((akey.x2 > x) && (akey.x2 < _x) && (akey.y2 > y) && (akey.y2 < _y)))
                {
                    if (akey.forcused == false)
                    {
                        string uriname = sImgPath + "focus\\" + akey.key_name + "_f.png";
                        string controlname = "img_kb_" + akey.key_name;
                        var uriSource = new Uri(uriname, UriKind.Relative);
                        Image img = (Image)FindName(controlname);
                        img.Source = new BitmapImage(uriSource);
                        akey.forcused = true;
                    }
                }
                else
                {
                    if (akey.forcused == true)
                    {
                        string uriname = sImgPath + "normal\\" + akey.key_name + "_n.png";
                        string controlname = "img_kb_" + akey.key_name;
                        var uriSource = new Uri(uriname, UriKind.Relative);
                        Image img = (Image)FindName(controlname);
                        img.Source = new BitmapImage(uriSource);
                        akey.forcused = false;
                    }
                }
            }
        }
        private void SetKeyForcused(object sender, MouseButtonEventArgs e)
        {
            string sImgPath = "..\\image\\2ndKB_Img\\UK\\";
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList102;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList102;

            bool clickedonKeys = false;
            Point p = e.GetPosition(Grid_KB);
            foreach (key akey in currentKeyList)
            {
                if ((p.X >= akey.x1) && (p.X <= akey.x2) && (p.Y >= akey.y1) && (p.Y <= akey.y2))
                {
                    string uriname;
                    if (akey.forcused == false)
                    {
                        uriname = sImgPath + "focus\\" + akey.key_name + "_f.png";
                        akey.forcused = true;
                    }
                    else
                    {
                        uriname = sImgPath + "normal\\" + akey.key_name + "_n.png";
                        akey.forcused = false;

                    }
                    string controlname = "img_kb_" + akey.key_name;
                    var uriSource = new Uri(uriname, UriKind.Relative);
                    Image img = (Image)FindName(controlname);
                    img.Source = new BitmapImage(uriSource);
                    clickedonKeys = true;
                    break;
                }
            }
            if (clickedonKeys == false)  // clear all key
            {
                foreach (key akey in currentKeyList)
                {
                    if (akey.forcused == true)
                    {
                        akey.forcused = false;
                        string uriname = sImgPath + "normal\\" + akey.key_name + "_n.png";
                        string controlname = "img_kb_" + akey.key_name;
                        var uriSource = new Uri(uriname, UriKind.Relative);
                        Image img = (Image)FindName(controlname);
                        img.Source = new BitmapImage(uriSource);
                    }
                }
            }
        }

        public void SetKeyForcusedClear()
        {
            string sImgPath = "..\\image\\2ndKB_Img\\UK\\";
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList102;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList102;

            foreach (key akey in currentKeyList)
            {
                if (akey.forcused == true)
                {
                    akey.forcused = false;
                    string uriname = sImgPath + "normal\\" + akey.key_name + "_n.png";
                    string controlname = "img_kb_" + akey.key_name;
                    var uriSource = new Uri(uriname, UriKind.Relative);
                    Image img = (Image)FindName(controlname);
                    img.Source = new BitmapImage(uriSource);
                }
            }
        }
        private void ApplyDragSelectionRect()
        {
            dragSelectionCanvas.Visibility = Visibility.Collapsed;

            double x = Canvas.GetLeft(dragSelectionBorder);
            double y = Canvas.GetTop(dragSelectionBorder);
            double width = dragSelectionBorder.Width;
            double height = dragSelectionBorder.Height;
            Rect dragRect = new Rect(x, y, width, height);

            //
            // Inflate the drag selection-rectangle by 1/10 of its size to 
            // make sure the intended item is selected.
            //
            dragRect.Inflate(width / 10, height / 10);

            //
            // Clear the current selection.
            //
            //listBox.SelectedItems.Clear();

            //
            // Find and select all the list box items.
            //
            //foreach (RectangleViewModel rectangleViewModel in this.ViewModel.Rectangles)
            //{
            //   Rect itemRect = new Rect(rectangleViewModel.X, rectangleViewModel.Y, rectangleViewModel.Width, rectangleViewModel.Height);
            //    if (dragRect.Contains(itemRect))
            //    {
            //        listBox.SelectedItems.Add(rectangleViewModel);
            //    }
            //}
        }
        private void UpdateDragSelectionRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Determine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle used for drag selection.
            //
            Canvas.SetLeft(dragSelectionBorder, x);
            Canvas.SetTop(dragSelectionBorder, y);
            dragSelectionBorder.Width = width;
            dragSelectionBorder.Height = height;

            setKeyRangeForcused(x, x + width, y, y + height);

        }
        private void InitDragSelectionRect(Point pt1, Point pt2)
        {
            UpdateDragSelectionRect(pt1, pt2);

            dragSelectionCanvas.Visibility = Visibility.Visible;
        }
        private void Grid_KB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_KBlayout_enable)
                return;
            if (e.ChangedButton == MouseButton.Left)
            {

                SetKeyForcused(sender, e);
                isLeftMouseButtonDownOnWindow = true;
                origMouseDownPoint = e.GetPosition(Grid_KB);
                Grid_KB.CaptureMouse();
                e.Handled = true;
            }
        }
        private void Grid_KB_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!m_KBlayout_enable)
                return;
            if (e.ChangedButton == MouseButton.Left)
            {
                bool wasDragSelectionApplied = false;

                if (isDraggingSelectionRect)
                {
                    //
                    // Drag selection has ended, apply the 'selection rectangle'.
                    //

                    isDraggingSelectionRect = false;
                    ApplyDragSelectionRect();

                    e.Handled = true;
                    wasDragSelectionApplied = true;
                }

                if (isLeftMouseButtonDownOnWindow)
                {
                    isLeftMouseButtonDownOnWindow = false;
                    Grid_KB.ReleaseMouseCapture();

                    e.Handled = true;
                }

                if (!wasDragSelectionApplied)
                {
                    //
                    // A click and release in empty space clears the selection.
                    //
                    //listBox.SelectedItems.Clear();
                }
            }
        }
        private void Grid_KB_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_KBlayout_enable)
                return;
            Point curMouseDownPoint = e.GetPosition(Grid_KB);
            if (isDraggingSelectionRect)
            {
                //
                // Drag selection is in progress.
                //                
                UpdateDragSelectionRect(origMouseDownPoint, curMouseDownPoint);

                e.Handled = true;
            }
            else if (isLeftMouseButtonDownOnWindow)
            {
                //
                // The user is left-dragging the mouse,
                // but don't initiate drag selection until
                // they have dragged past the threshold value.
                //
                var dragDelta = curMouseDownPoint - origMouseDownPoint;
                double dragDistance = Math.Abs(dragDelta.Length);
                if (dragDistance > DragThreshold)
                {
                    //
                    // When the mouse has been dragged more than the threshold value commence drag selection.
                    //
                    isDraggingSelectionRect = true;
                    InitDragSelectionRect(origMouseDownPoint, curMouseDownPoint);
                }

                e.Handled = true;
            }
            else
            {
                setKeyCusored(curMouseDownPoint.X, curMouseDownPoint.Y);
            }


        }
        private void Grid_KB_MouseLeave(object sender, MouseEventArgs e)
        {
            setKeyCusored(0, 0); // clear cursor status.
        }
    }


}
