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
    public partial class KBTable : IKBTypeInterface//UserControl
    {
        public bool m_KBlayout_enable = false;
        //Grid_KB mouse parameter
        private bool isLeftMouseButtonDownOnWindow = false;
        private bool isDraggingSelectionRect = false;
        private Point origMouseDownPoint;
        private static readonly double DragThreshold = 5;

        public List<key> m_KeyList = null;
        public List<key> m_WelKeyList = null;
        public bool KBTI_KBlayout_enable
        {
            get
            {
                return m_KBlayout_enable;
            }

            set
            {
                m_KBlayout_enable = value;
            }

        }
        public List<key> KBTI_KeyList
        {
            get
            {
                return m_KeyList;
            }

            set
            {
                m_KeyList = value;
            }
        }
        public List<key> KBTI_WelKeyList
        {
            get
            {
                return m_WelKeyList;
            }

            set
            {
                m_WelKeyList = value;
            }
        }

        private uint m_mode = (uint)mode.windows;

        private void InitializeParameter()
        {
            m_KeyList = new List<key>
            {
                new key("esc"         ,       0x01, false, false, 0x00, 0x00, 0x00,  11, 11,  45, 38,    6,   0),
                new key("f1"          ,       0x3B, false, false, 0x00, 0x00, 0x00,  47, 11,  81, 38,   12,   0),
                new key("f2"          ,       0x3C, false, false, 0x00, 0x00, 0x00,  83, 11, 117, 38,   18,   0),
                new key("f3"          ,       0x3D, false, false, 0x00, 0x00, 0x00, 119, 11, 154, 38,   24,   0),
                new key("f4"          ,       0x3E, false, false, 0x00, 0x00, 0x00, 155, 11, 189, 38,   30,   0),
                new key("f5"          ,       0x3F, false, false, 0x00, 0x00, 0x00, 191, 11, 225, 38,   36,   0),
                new key("f6"          ,       0x40, false, false, 0x00, 0x00, 0x00, 227, 11, 261, 38,   42,   0),
                new key("f7"          ,       0x41, false, false, 0x00, 0x00, 0x00, 263, 11, 297, 38,   48,   0),
                new key("f8"          ,       0x42, false, false, 0x00, 0x00, 0x00, 299, 11, 333, 38,   54,   0),
                new key("f9"          ,       0x43, false, false, 0x00, 0x00, 0x00, 335, 11, 369, 38,   60,   0),
                new key("f10"         ,       0x44, false, false, 0x00, 0x00, 0x00, 371, 11, 405, 38,   66,   0),
                new key("f11"         ,       0x45, false, false, 0x00, 0x00, 0x00, 407, 11, 441, 38,   72,   0),
                new key("f12"         ,       0x46, false, false, 0x00, 0x00, 0x00, 443, 11, 477, 38,   78,   0),
                new key("prtsc"       ,       0x37, false, false, 0x00, 0x00, 0x00, 479, 11, 513, 38,   84,   0),
                new key("insert"      ,       0x52, false, false, 0x00, 0x00, 0x00, 515, 11, 549, 38,   90,   0),
                new key("del"         ,       0x53, false, false, 0x00, 0x00, 0x00, 550, 11, 584, 38,   96,   0),
                new key("home"        ,       0x47, false, false, 0x00, 0x00, 0x00, 587, 11, 622, 38,  102,   0),
                new key("end"         ,       0x4F, false, false, 0x00, 0x00, 0x00, 623, 11, 658, 38,  108,   0),
                new key("pgup"        ,       0x49, false, false, 0x00, 0x00, 0x00, 659, 11, 694, 38,  114,   0),
                new key("pgdn"        ,       0x51, false, false, 0x00, 0x00, 0x00, 695, 11, 730, 38,  120,   0),
                new key("tilde"       ,       0x29, false, false, 0x00, 0x00, 0x00,  11, 40,  40, 77,    5,   0),
                new key("exclamation" ,       0x02, false, false, 0x00, 0x00, 0x00,  42, 40,  79, 77,   11,   0),
                new key("at"          ,       0x03, false, false, 0x00, 0x00, 0x00,  81, 40, 118, 77,   17,   0),
                new key("pound"       ,       0x04, false, false, 0x00, 0x00, 0x00, 120, 40, 157, 77,   23,   0),
                new key("dollar"      ,       0x05, false, false, 0x00, 0x00, 0x00, 159, 40, 196, 77,   29,   0),
                new key("percent"     ,       0x06, false, false, 0x00, 0x00, 0x00, 198, 40, 235, 77,   35,   0),
                new key("caret"       ,       0x07, false, false, 0x00, 0x00, 0x00, 237, 40, 274, 77,   41,   0),
                new key("and"         ,       0x08, false, false, 0x00, 0x00, 0x00, 276, 40, 313, 77,   47,   0),
                new key("asterisk"    ,       0x09, false, false, 0x00, 0x00, 0x00, 315, 40, 352, 77,   53,   0),
                new key("openbracket" ,       0x0A, false, false, 0x00, 0x00, 0x00, 354, 40, 391, 77,   59,   0),
                new key("closebracket",       0x0B, false, false, 0x00, 0x00, 0x00, 393, 40, 430, 77,   65,   0),
                new key("dash" ,              0x0C, false, false, 0x00, 0x00, 0x00, 432, 40, 469, 77,   71,   0),
                new key("equals",             0x0D, false, false, 0x00, 0x00, 0x00, 471, 40, 508, 77,   77,   0),
                new key("backspace",          0x0E, false, false, 0x00, 0x00, 0x00, 511, 40, 585, 77,   89,  95),
                new key("numlock",            0x45, false, false, 0x00, 0x00, 0x00, 587, 40, 622, 77,  101,   0),
                new key("division", (ushort)0xE035, false, false, 0x00, 0x00, 0x00, 623, 40, 658, 77,  107,   0),
                new key("star",               0x37, false, false, 0x00, 0x00, 0x00, 659, 40, 694, 77,  113,   0),
                new key("minus",              0x4A, false, false, 0x00, 0x00, 0x00, 695, 40, 730, 77,  119,   0),
                new key("tab",                0x0F, false, false, 0x00, 0x00, 0x00,  11, 79,  59, 116,   4,   0),
                new key("q",                  0x10, false, false, 0x00, 0x00, 0x00,  61, 79,  98, 116,  16,   0),
                new key("w",                  0x11, false, false, 0x00, 0x00, 0x00, 100, 79, 137, 116,  22,   0),
                new key("e",                  0x12, false, false, 0x00, 0x00, 0x00, 139, 79, 176, 116,  28,   0),
                new key("r",                  0x13, false, false, 0x00, 0x00, 0x00, 178, 79, 215, 116,  34,   0),
                new key("t",                  0x14, false, false, 0x00, 0x00, 0x00, 217, 79, 254, 116,  40,   0),
                new key("y",                  0x15, false, false, 0x00, 0x00, 0x00, 256, 79, 293, 116,  46,   0),
                new key("u",                  0x16, false, false, 0x00, 0x00, 0x00, 295, 79, 332, 116,  52,   0),
                new key("i",                  0x17, false, false, 0x00, 0x00, 0x00, 334, 79, 371, 116,  58,   0),
                new key("o",                  0x18, false, false, 0x00, 0x00, 0x00, 373, 79, 410, 116,  64,   0),
                new key("p",                  0x19, false, false, 0x00, 0x00, 0x00, 412, 79, 449, 116,  70,   0),
                new key("openbrace",          0x1A, false, false, 0x00, 0x00, 0x00, 451, 79, 488, 116,  76,   0),
                new key("closebrace",         0x1B, false, false, 0x00, 0x00, 0x00, 490, 79, 527, 116,  82,   0),
                new key("backslash",          0x2B, false, false, 0x00, 0x00, 0x00, 529, 79, 585, 116,  94,   0),
                new key("7",                  0x47, false, false, 0x00, 0x00, 0x00, 587, 79, 622, 116, 100,   0),
                new key("8",                  0x48, false, false, 0x00, 0x00, 0x00, 623, 79, 658, 116, 106,   0),
                new key("9",                  0x49, false, false, 0x00, 0x00, 0x00, 659, 79, 694, 116, 112,   0),
                new key("plus",               0x4E, false, false, 0x00, 0x00, 0x00, 695, 79, 730, 155, 118, 117),
                new key("capslock",           0x3A, false, false, 0x00, 0x00, 0x00,  11,118,  67, 155,   3,   0),
                new key("a",                  0x1E, false, false, 0x00, 0x00, 0x00,  69,118, 106, 155,  15,   0),
                new key("s",                  0x1F, false, false, 0x00, 0x00, 0x00, 108,118, 145, 155,  21,   0),
                new key("d",                  0x20, false, false, 0x00, 0x00, 0x00, 147,118, 184, 155,  27,   0),
                new key("f",                  0x21, false, false, 0x00, 0x00, 0x00, 186,118, 223, 155,  33,   0),
                new key("g",                  0x22, false, false, 0x00, 0x00, 0x00, 225,118, 262, 155,  39,   0),
                new key("h",                  0x23, false, false, 0x00, 0x00, 0x00, 264,118, 301, 155,  45,   0),
                new key("j",                  0x24, false, false, 0x00, 0x00, 0x00, 303,118, 340, 155,  51,   0),
                new key("k",                  0x25, false, false, 0x00, 0x00, 0x00, 342,118, 379, 155,  57,   0),
                new key("l",                  0x26, false, false, 0x00, 0x00, 0x00, 381,118, 418, 155,  63,   0),
                new key("colon",              0x27, false, false, 0x00, 0x00, 0x00, 420,118, 457, 155,  69,   0),
                new key("quote",              0x28, false, false, 0x00, 0x00, 0x00, 459,118, 497, 155,  75,   0),
                new key("enter1",             0x1c, false, false, 0x00, 0x00, 0x00, 501,118, 585, 155,  87,  93),
                new key("4",                  0x4b, false, false, 0x00, 0x00, 0x00, 587,118, 622, 155,  99,   0),
                new key("5",                  0x4c, false, false, 0x00, 0x00, 0x00, 623,118, 658, 155, 105,   0),
                new key("6",                  0x4d, false, false, 0x00, 0x00, 0x00, 659,118, 694, 155, 111,   0),
                new key("shift1",             0x2a, false, false, 0x00, 0x00, 0x00,  11,157,  86, 195,   2,   8),
                new key("z",                  0x2c, false, false, 0x00, 0x00, 0x00,  88,157, 125, 195,  14,   0),
                new key("x",                  0x2d, false, false, 0x00, 0x00, 0x00, 127,157, 164, 195,  20,   0),
                new key("c",                  0x2e, false, false, 0x00, 0x00, 0x00, 166,157, 203, 195,  26,   0),
                new key("v",                  0x2f, false, false, 0x00, 0x00, 0x00, 205,157, 242, 195,  32,   0),
                new key("b",                  0x30, false, false, 0x00, 0x00, 0x00, 244,157, 281, 195,  38,   0),
                new key("n",                  0x31, false, false, 0x00, 0x00, 0x00, 283,157, 320, 195,  44,   0),
                new key("m",                  0x32, false, false, 0x00, 0x00, 0x00, 322,157, 359, 195,  50,   0),
                new key("lessthan",           0x33, false, false, 0x00, 0x00, 0x00, 361,157, 398, 195,  56,   0),
                new key("greaterthan",        0x34, false, false, 0x00, 0x00, 0x00, 400,157, 437, 195,  62,   0),
                new key("question",           0x35, false, false, 0x00, 0x00, 0x00, 439,157, 476, 195,  68,   0),
                new key("shift",              0x36, false, false, 0x00, 0x00, 0x00, 478,157, 543, 195,  74,   0),
                new key("up",               0xe048, false, false, 0x00, 0x00, 0x00, 546,157, 585, 195,  92,   0),
                new key("1",                  0x4f, false, false, 0x00, 0x00, 0x00, 587,157, 622, 195,  98,   0),
                new key("2",                  0x50, false, false, 0x00, 0x00, 0x00, 623,157, 658, 195, 104,   0),
                new key("3",                  0x51, false, false, 0x00, 0x00, 0x00, 659,157, 694, 195, 110,   0),
                new key("enter2",           0xe01c, false, false, 0x00, 0x00, 0x00, 695,157, 730, 235, 116, 115),
                new key("ctrl1",              0x1d, false, false, 0x00, 0x00, 0x00,  11,197,  49, 235,   1,   0),
                new key("fn",               0xffff, false, false, 0x00, 0x00, 0x00,  49,197,  86, 235,   7,   0),
                new key("windows",          0xffff, false, false, 0x00, 0x00, 0x00,  88,197, 126, 235,  13,   0),
                new key("alt1",               0x38, false, false, 0x00, 0x00, 0x00, 127,197, 165, 235,  19,   0),
                new key("spacebar",           0x39, false, false, 0x00, 0x00, 0x00, 166,197, 359, 235,  31,  37),
                new key("alt2",             0xe038, false, false, 0x00, 0x00, 0x00, 362,197, 398, 235,  55,   0),
                new key("note",             0xffff, false, false, 0x00, 0x00, 0x00, 400,197, 438, 235,  61,   0),
                new key("ctrl2",            0xe01d, false, false, 0x00, 0x00, 0x00, 440,197, 505, 235,  73,   0),
                new key("left",             0xe046, false, false, 0x00, 0x00, 0x00, 508,197, 544, 235,  85,   0),
                new key("down",             0xe050, false, false, 0x00, 0x00, 0x00, 546,197, 583, 235,  91,   0),
                new key("right",            0xe04d, false, false, 0x00, 0x00, 0x00, 587,197, 622, 235,  97,   0),
                new key("0",                  0x52, false, false, 0x00, 0x00, 0x00, 623,197, 658, 235, 103,   0),
                new key("del2",             0x0053, false, false, 0x00, 0x00, 0x00, 659,197, 694, 235, 109,   0),

             };
            m_WelKeyList = new List<key>
            {
                new key("esc"         ,       0x01, false, false, 0x00, 0x00, 0x00,  11, 11,  45, 38,    6,   0),
                new key("f1"          ,       0x3B, false, false, 0x00, 0x00, 0x00,  47, 11,  81, 38,   12,   0),
                new key("f2"          ,       0x3C, false, false, 0x00, 0x00, 0x00,  83, 11, 117, 38,   18,   0),
                new key("f3"          ,       0x3D, false, false, 0x00, 0x00, 0x00, 119, 11, 154, 38,   24,   0),
                new key("f4"          ,       0x3E, false, false, 0x00, 0x00, 0x00, 155, 11, 189, 38,   30,   0),
                new key("f5"          ,       0x3F, false, false, 0x00, 0x00, 0x00, 191, 11, 225, 38,   36,   0),
                new key("f6"          ,       0x40, false, false, 0x00, 0x00, 0x00, 227, 11, 261, 38,   42,   0),
                new key("f7"          ,       0x41, false, false, 0x00, 0x00, 0x00, 263, 11, 297, 38,   48,   0),
                new key("f8"          ,       0x42, false, false, 0x00, 0x00, 0x00, 299, 11, 333, 38,   54,   0),
                new key("f9"          ,       0x43, false, false, 0x00, 0x00, 0x00, 335, 11, 369, 38,   60,   0),
                new key("f10"         ,       0x44, false, false, 0x00, 0x00, 0x00, 371, 11, 405, 38,   66,   0),
                new key("f11"         ,       0x45, false, false, 0x00, 0x00, 0x00, 407, 11, 441, 38,   72,   0),
                new key("f12"         ,       0x46, false, false, 0x00, 0x00, 0x00, 443, 11, 477, 38,   78,   0),
                new key("prtsc"       ,       0x37, false, false, 0x00, 0x00, 0x00, 479, 11, 513, 38,   84,   0),
                new key("insert"      ,       0x52, false, false, 0x00, 0x00, 0x00, 515, 11, 549, 38,   90,   0),
                new key("del"         ,       0x53, false, false, 0x00, 0x00, 0x00, 550, 11, 584, 38,   96,   0),
                new key("home"        ,       0x47, false, false, 0x00, 0x00, 0x00, 587, 11, 622, 38,  102,   0),
                new key("end"         ,       0x4F, false, false, 0x00, 0x00, 0x00, 623, 11, 658, 38,  108,   0),
                new key("pgup"        ,       0x49, false, false, 0x00, 0x00, 0x00, 659, 11, 694, 38,  114,   0),
                new key("pgdn"        ,       0x51, false, false, 0x00, 0x00, 0x00, 695, 11, 730, 38,  120,   0),
                new key("tilde"       ,       0x29, false, false, 0x00, 0x00, 0x00,  11, 40,  40, 77,    5,   0),
                new key("exclamation" ,       0x02, false, false, 0x00, 0x00, 0x00,  42, 40,  79, 77,   11,   0),
                new key("at"          ,       0x03, false, false, 0x00, 0x00, 0x00,  81, 40, 118, 77,   17,   0),
                new key("pound"       ,       0x04, false, false, 0x00, 0x00, 0x00, 120, 40, 157, 77,   23,   0),
                new key("dollar"      ,       0x05, false, false, 0x00, 0x00, 0x00, 159, 40, 196, 77,   29,   0),
                new key("percent"     ,       0x06, false, false, 0x00, 0x00, 0x00, 198, 40, 235, 77,   35,   0),
                new key("caret"       ,       0x07, false, false, 0x00, 0x00, 0x00, 237, 40, 274, 77,   41,   0),
                new key("and"         ,       0x08, false, false, 0x00, 0x00, 0x00, 276, 40, 313, 77,   47,   0),
                new key("asterisk"    ,       0x09, false, false, 0x00, 0x00, 0x00, 315, 40, 352, 77,   53,   0),
                new key("openbracket" ,       0x0A, false, false, 0x00, 0x00, 0x00, 354, 40, 391, 77,   59,   0),
                new key("closebracket",       0x0B, false, false, 0x00, 0x00, 0x00, 393, 40, 430, 77,   65,   0),
                new key("dash" ,              0x0C, false, false, 0x00, 0x00, 0x00, 432, 40, 469, 77,   71,   0),
                new key("equals",             0x0D, false, false, 0x00, 0x00, 0x00, 471, 40, 508, 77,   77,   0),
                new key("backspace",          0x0E, false, false, 0x00, 0x00, 0x00, 511, 40, 585, 77,   89,  95),
                new key("numlock",            0x45, false, false, 0x00, 0x00, 0x00, 587, 40, 622, 77,  101,   0),
                new key("division", (ushort)0xE035, false, false, 0x00, 0x00, 0x00, 623, 40, 658, 77,  107,   0),
                new key("star",               0x37, false, false, 0x00, 0x00, 0x00, 659, 40, 694, 77,  113,   0),
                new key("minus",              0x4A, false, false, 0x00, 0x00, 0x00, 695, 40, 730, 77,  119,   0),
                new key("tab",                0x0F, false, false, 0x00, 0x00, 0x00,  11, 79,  59, 116,   4,   0),
                new key("q",                  0x10, false, false, 0x00, 0x00, 0x00,  61, 79,  98, 116,  16,   0),
                new key("w",                  0x11, false, false, 0x00, 0x00, 0x00, 100, 79, 137, 116,  22,   0),
                new key("e",                  0x12, false, false, 0x00, 0x00, 0x00, 139, 79, 176, 116,  28,   0),
                new key("r",                  0x13, false, false, 0x00, 0x00, 0x00, 178, 79, 215, 116,  34,   0),
                new key("t",                  0x14, false, false, 0x00, 0x00, 0x00, 217, 79, 254, 116,  40,   0),
                new key("y",                  0x15, false, false, 0x00, 0x00, 0x00, 256, 79, 293, 116,  46,   0),
                new key("u",                  0x16, false, false, 0x00, 0x00, 0x00, 295, 79, 332, 116,  52,   0),
                new key("i",                  0x17, false, false, 0x00, 0x00, 0x00, 334, 79, 371, 116,  58,   0),
                new key("o",                  0x18, false, false, 0x00, 0x00, 0x00, 373, 79, 410, 116,  64,   0),
                new key("p",                  0x19, false, false, 0x00, 0x00, 0x00, 412, 79, 449, 116,  70,   0),
                new key("openbrace",          0x1A, false, false, 0x00, 0x00, 0x00, 451, 79, 488, 116,  76,   0),
                new key("closebrace",         0x1B, false, false, 0x00, 0x00, 0x00, 490, 79, 527, 116,  82,   0),
                new key("backslash",          0x2B, false, false, 0x00, 0x00, 0x00, 529, 79, 585, 116,  94,   0),
                new key("7",                  0x47, false, false, 0x00, 0x00, 0x00, 587, 79, 622, 116, 100,   0),
                new key("8",                  0x48, false, false, 0x00, 0x00, 0x00, 623, 79, 658, 116, 106,   0),
                new key("9",                  0x49, false, false, 0x00, 0x00, 0x00, 659, 79, 694, 116, 112,   0),
                new key("plus",               0x4E, false, false, 0x00, 0x00, 0x00, 695, 79, 730, 155, 118, 117),
                new key("capslock",           0x3A, false, false, 0x00, 0x00, 0x00,  11,118,  67, 155,   3,   0),
                new key("a",                  0x1E, false, false, 0x00, 0x00, 0x00,  69,118, 106, 155,  15,   0),
                new key("s",                  0x1F, false, false, 0x00, 0x00, 0x00, 108,118, 145, 155,  21,   0),
                new key("d",                  0x20, false, false, 0x00, 0x00, 0x00, 147,118, 184, 155,  27,   0),
                new key("f",                  0x21, false, false, 0x00, 0x00, 0x00, 186,118, 223, 155,  33,   0),
                new key("g",                  0x22, false, false, 0x00, 0x00, 0x00, 225,118, 262, 155,  39,   0),
                new key("h",                  0x23, false, false, 0x00, 0x00, 0x00, 264,118, 301, 155,  45,   0),
                new key("j",                  0x24, false, false, 0x00, 0x00, 0x00, 303,118, 340, 155,  51,   0),
                new key("k",                  0x25, false, false, 0x00, 0x00, 0x00, 342,118, 379, 155,  57,   0),
                new key("l",                  0x26, false, false, 0x00, 0x00, 0x00, 381,118, 418, 155,  63,   0),
                new key("colon",              0x27, false, false, 0x00, 0x00, 0x00, 420,118, 457, 155,  69,   0),
                new key("quote",              0x28, false, false, 0x00, 0x00, 0x00, 459,118, 497, 155,  75,   0),
                new key("enter1",             0x1c, false, false, 0x00, 0x00, 0x00, 501,118, 585, 155,  87,  93),
                new key("4",                  0x4b, false, false, 0x00, 0x00, 0x00, 587,118, 622, 155,  99,   0),
                new key("5",                  0x4c, false, false, 0x00, 0x00, 0x00, 623,118, 658, 155, 105,   0),
                new key("6",                  0x4d, false, false, 0x00, 0x00, 0x00, 659,118, 694, 155, 111,   0),
                new key("shift1",             0x2a, false, false, 0x00, 0x00, 0x00,  11,157,  86, 195,   2,   8),
                new key("z",                  0x2c, false, false, 0x00, 0x00, 0x00,  88,157, 125, 195,  14,   0),
                new key("x",                  0x2d, false, false, 0x00, 0x00, 0x00, 127,157, 164, 195,  20,   0),
                new key("c",                  0x2e, false, false, 0x00, 0x00, 0x00, 166,157, 203, 195,  26,   0),
                new key("v",                  0x2f, false, false, 0x00, 0x00, 0x00, 205,157, 242, 195,  32,   0),
                new key("b",                  0x30, false, false, 0x00, 0x00, 0x00, 244,157, 281, 195,  38,   0),
                new key("n",                  0x31, false, false, 0x00, 0x00, 0x00, 283,157, 320, 195,  44,   0),
                new key("m",                  0x32, false, false, 0x00, 0x00, 0x00, 322,157, 359, 195,  50,   0),
                new key("lessthan",           0x33, false, false, 0x00, 0x00, 0x00, 361,157, 398, 195,  56,   0),
                new key("greaterthan",        0x34, false, false, 0x00, 0x00, 0x00, 400,157, 437, 195,  62,   0),
                new key("question",           0x35, false, false, 0x00, 0x00, 0x00, 439,157, 476, 195,  68,   0),
                new key("shift",              0x36, false, false, 0x00, 0x00, 0x00, 478,157, 543, 195,  74,   0),
                new key("up",               0xe048, false, false, 0x00, 0x00, 0x00, 546,157, 585, 195,  92,   0),
                new key("1",                  0x4f, false, false, 0x00, 0x00, 0x00, 587,157, 622, 195,  98,   0),
                new key("2",                  0x50, false, false, 0x00, 0x00, 0x00, 623,157, 658, 195, 104,   0),
                new key("3",                  0x51, false, false, 0x00, 0x00, 0x00, 659,157, 694, 195, 110,   0),
                new key("enter2",           0xe01c, false, false, 0x00, 0x00, 0x00, 695,157, 730, 235, 116, 115),
                new key("ctrl1",              0x1d, false, false, 0x00, 0x00, 0x00,  11,197,  49, 235,   1,   0),
                new key("fn",               0xffff, false, false, 0x00, 0x00, 0x00,  49,197,  86, 235,   7,   0),
                new key("windows",          0xffff, false, false, 0x00, 0x00, 0x00,  88,197, 126, 235,  13,   0),
                new key("alt1",               0x38, false, false, 0x00, 0x00, 0x00, 127,197, 165, 235,  19,   0),
                new key("spacebar",           0x39, false, false, 0x00, 0x00, 0x00, 166,197, 359, 235,  31,  37),
                new key("alt2",             0xe038, false, false, 0x00, 0x00, 0x00, 362,197, 398, 235,  55,   0),
                new key("note",             0xffff, false, false, 0x00, 0x00, 0x00, 400,197, 438, 235,  61,   0),
                new key("ctrl2",            0xe01d, false, false, 0x00, 0x00, 0x00, 440,197, 505, 235,  73,   0),
                new key("left",             0xe046, false, false, 0x00, 0x00, 0x00, 508,197, 544, 235,  85,   0),
                new key("down",             0xe050, false, false, 0x00, 0x00, 0x00, 546,197, 583, 235,  91,   0),
                new key("right",            0xe04d, false, false, 0x00, 0x00, 0x00, 587,197, 622, 235,  97,   0),
                new key("0",                  0x52, false, false, 0x00, 0x00, 0x00, 623,197, 658, 235, 103,   0),
                new key("del2",             0x0053, false, false, 0x00, 0x00, 0x00, 659,197, 694, 235, 109,   0),

             };
        }

        private void InitializeLayout()
        {
      
        }
        public KBTable()
        {
            InitializeParameter();
            InitializeComponent();
            InitializeLayout();
        }

        public void SetKeyBoardColor(uint current_mode, byte R, byte G, byte B)
        {
            List<key> currentKeyList;
            if (current_mode == (uint)mode.windows)
                currentKeyList = m_KeyList;
            else if (current_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList;

            foreach (key akey in currentKeyList)
            {
                if (akey.forcused == true)
                {
                    akey.R = R;
                    akey.G = G;
                    akey.B = B;
                    string controlname = "rec_kb_" + akey.key_name;
                    Rectangle control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                }
            }

        }

        public void EnableKBLayout(bool enable, uint current_mode)
        {
            //if (m_KBlayout_enable == enable)   // !!!!!!! This will impact profile layout update.... !!!!! TBD item
            //    return;
            m_mode = current_mode;
            m_KBlayout_enable = enable;
            if (m_KBlayout_enable == false)
            {
                foreach (key akey in m_KeyList)
                {
                    akey.forcused = false;
                    

                    //back to keybaord normal status.
                    string uriname = "..\\Image\\key_" + akey.key_name + "_n.png";
                    string controlname = "img_kb_" + akey.key_name;
                    var uriSource = new Uri(uriname, UriKind.Relative);
                    Image img = (Image)FindName(controlname);
                    img.Source = new BitmapImage(uriSource);
                    img.Visibility = Visibility.Visible;
                    
                    controlname = "rec_kb_" + akey.key_name;
                    Rectangle rec = (Rectangle)FindName(controlname);
                    rec.Fill = null;
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
                    foreach (key akey in m_KeyList)
                    {

                        string controlname = "rec_kb_" + akey.key_name;
                        Rectangle rec = (Rectangle)FindName(controlname);
                        rec.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
                    }
                }
                else if(current_mode == (uint)mode.welcome)
                {
                    foreach (key akey in m_WelKeyList)
                    {

                        string controlname = "rec_kb_" + akey.key_name;
                        Rectangle rec = (Rectangle)FindName(controlname);
                        rec.Fill = new SolidColorBrush(Color.FromRgb(akey.R, akey.G, akey.B));
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
                currentKeyList = m_KeyList;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList;


            // bool cursored = false;
            string uriname;
            string controlname;

            foreach (key akey in currentKeyList)
            {
                if ((akey.x1 < x) && (akey.x2 > x) && (akey.y1 < y) && (akey.y2 > y))
                {
                    if (akey.forcused == false)
                    {
                        uriname = "..\\Image\\key_" + akey.key_name + "_h.png";
                    }
                    else
                    {
                        uriname = "..\\Image\\key_" + akey.key_name + "_fh.png";
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
                            uriname = "..\\Image\\key_" + akey.key_name + "_f.png";
                        else
                            uriname = "..\\Image\\key_" + akey.key_name + "_n.png";
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
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList;

            foreach (key akey in currentKeyList)
            {
                if (((akey.x1 > x) && (akey.x1 < _x) && (akey.y1 > y) && (akey.y1 < _y)) ||
                    ((akey.x1 > x) && (akey.x1 < _x) && (akey.y2 > y) && (akey.y2 < _y)) ||
                    ((akey.x2 > x) && (akey.x2 < _x) && (akey.y1 > y) && (akey.y1 < _y)) ||
                    ((akey.x2 > x) && (akey.x2 < _x) && (akey.y2 > y) && (akey.y2 < _y)))
                {
                    if (akey.forcused == false)
                    {
                        string uriname = "..\\Image\\key_" + akey.key_name + "_f.png";
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
                        string uriname = "..\\Image\\key_" + akey.key_name + "_n.png";
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
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList;

            bool clickedonKeys = false;
            Point p = e.GetPosition(Grid_KB);
            foreach (key akey in currentKeyList)
            {
                if ((p.X >= akey.x1) && (p.X <= akey.x2) && (p.Y >= akey.y1) && (p.Y <= akey.y2))
                {
                    string uriname;
                    if (akey.forcused == false)
                    {
                        uriname = "..\\Image\\key_" + akey.key_name + "_f.png";
                        akey.forcused = true;
                    }
                    else
                    {
                        uriname = "..\\Image\\key_" + akey.key_name + "_n.png";
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
                        string uriname = "..\\Image\\key_" + akey.key_name + "_n.png";
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
            List<key> currentKeyList;
            if (m_mode == (uint)mode.windows)
                currentKeyList = m_KeyList;
            else if (m_mode == (uint)mode.welcome)
                currentKeyList = m_WelKeyList;
            else
                currentKeyList = m_KeyList;

            foreach (key akey in currentKeyList)
            {
                if (akey.forcused == true)
                {
                    akey.forcused = false;
                    string uriname = "..\\Image\\key_" + akey.key_name + "_n.png";
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
