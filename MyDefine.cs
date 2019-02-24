using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyColor2
{
    interface  IKBTypeInterface
    {
        bool KBTI_KBlayout_enable { get; set; }
        List<key> KBTI_KeyList { get; set; }
        List<key> KBTI_WelKeyList { get; set; }

        void SetKeyForcusedClear();
        void SetKeyBoardColor(uint current_mode, byte R, byte G, byte B);
        void EnableKBLayout(bool enable, uint current_mode);

    }

    //for EC support byte3 status(0x073cH)
    public enum enum_KBTypeXamlID:byte
    {
        _1st_FKBT       = 0,
        _1st_4KBT       = 3,
        _1st_4KBT2      = 7,
        _2nd_KBT_102    = 17,
        _2nd_KBT_101    = 24,
        _2nd_KBT_102M   = 33,
        _2nd_KBT_101M   = 41,
        _2nd_KBT_88     = 49,
        _2nd_KBT_87     = 57,
        _2nd_KBT_88M    = 73,
        _2nd_KBT_87M    = 81, 
        ME_KB_FAIL      = 255

    }
    public class brightness_ball_margin
    {
        public const double height = 550;
        public const double level0_x = 129;
        public const double level1_x = 172;
        public const double level2_x = 219;
        public const double level3_x = 266;
        public const double level4_x = 313;
    }
    public class tempo_ball_margin
    {
        public const double height = 550;
        public const double level0_x = 487;//500;
        public const double level1_x = 501;//521;
        public const double level2_x = 522;//542;
        public const double level3_x = 543;//563;
        public const double level4_x = 564;//584;
        public const double level5_x = 585;//605;
        public const double level6_x = 606;//626;
        public const double level7_x = 627;//647;
        public const double level8_x = 648;//668;
        public const double level9_x = 669;//689;

    }
    public class key
    {
        public string key_name { get; set; }
        public ushort scan_code { get; set; }
        public bool cursored { get; set; }
        public bool forcused { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public double x1 { get; set; }
        public double x2 { get; set; }
        public double y1 { get; set; }
        public double y2 { get; set; }

        //Darfon Key defination
        public byte df_id1 { get; set; }
        public byte df_id2 { get; set; }
        public key(string _key_name, ushort _scan_code, bool _cursored,bool _forcused, byte _R, byte _G, byte _B, double _x1, double _y1, double _x2, double _y2, byte _df_id1, byte _df_id2)
        {
            key_name = _key_name;
            scan_code = _scan_code;
            cursored = _cursored;
            forcused = _forcused;
            R = _R;
            G = _G;
            B = _B;
            x1 = _x1;
            y1 = _y1;
            x2 = _x2;
            y2 = _y2;
            df_id1 = _df_id1;
            df_id2 = _df_id2;
        }
    }
    public class ColorBlock
    {
        public uint sid { get; set; }
        public bool enable { get; set; }
        public bool selected { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public ColorBlock( uint _sid, bool _enable, bool _selected, byte _R, byte _G, byte _B)
        {
            sid = _sid;
            enable = _enable;
            selected = _selected;
            R = _R;
            G = _G;
            B = _B;
        }

    }
    public class EffectColor
    {
        public uint effect_id { get; set; }

        public List<ColorBlock> colorblock { get; set; }
        public bool color_single_enable { get; set; }
        public bool color_single_status { get; set; }
        public byte color_single_r { get; set; }
        public byte color_single_g { get; set; }
        public byte color_single_b { get; set; }

        private void initColorBlock(uint _num)
        {
            colorblock = new List<ColorBlock>();
            for (uint i = 0; i < _num; i++)
                colorblock.Add(new ColorBlock(i, false, false, 0xFF, 0x00, 0x00));
        }
        public EffectColor(uint _effect_id, uint _num, bool _color_single_enable, bool _color_single_status,
          byte _color_single_r, byte _color_single_g, byte _color_single_b  )
        {
            effect_id = _effect_id;
            initColorBlock(_num);
            color_single_enable = _color_single_enable;
            color_single_status = _color_single_status;
            color_single_r = _color_single_r;
            color_single_g = _color_single_g;
            color_single_b = _color_single_b;

        }
    }
    /*
    internal class ColorChangeTiming
    {
        public uint effect_id { get; set; }
        public uint seconds { get; set; }
        public ColorChangeTiming(uint _effect_id, uint _seconds)
        {
            effect_id = _effect_id;
            seconds = _seconds;
        }
    }
    */
    public class EffectDirection
    {
        public uint effect_id { get; set; }
        public int direction_index { get; set; }
        public EffectDirection(uint _effect_id, int _direction_index)
        {
            effect_id = _effect_id;
            direction_index = _direction_index;
        }
    }

    public class status
    {
        public const bool disable = false;
        public const bool enable = true;

        public const bool off = false;
        public const bool on = true;

    }

    public class layoutStatus // for Welcome and Windows to store layout status
    {
        public uint effect;

        public byte tempo_level;
        public byte brightness_level;

        public List<EffectColor> effectColorList;
        public List<EffectDirection> effectdirection;
        //public List<ColorChangeTiming> colorTimerList;

        //TBD
        // Import  ColorRGBList, KBRGB, seconds, direction here......  

        //public byte R;
        //public byte G;
        //public byte B;

        /*
        // reserved for multi-color region 
        */

    }
    public enum colorpad
    {
        pad,
        red,
        orange,
        yellow,
        greeen,
        blue,
        indigo,
        violet
    }

    public enum profiles_option
    {
        Import,
        Export
    }

    public enum mode
    {
        windows,
        welcome
    }
    public enum effect
    {
        Static,
        Breathing,
        Wave,
        Ripple,
        Reactive,
        Rainbow,
        Marquee,
        Raindrop,
        Stacker,
        Impact,
        Whirling,
        Aurora,
        Spark,
        UserMode,
        Close,
        Music,
        Neon,
        Flash,
        Mix

    }

    public enum tirgger
    {
        auto,
        onkeypressed
    }

    public enum type_direction
    {
        NA,
        direction,
        clockwise,
        onkeypress,
        direction_sync,
    }

    public enum id_direction
    {
        LR,  //Left to Right
        RL,  //Right to Left 
        TB,  // Top to Bottom
        BT,  // Bottom to Top
        CW,  // Clock wise
        CCW, // Counter Clock Wise
        Auto, 
        OnKeyPressed,
        Sync,
    }


    public enum type_KBLayout
    {
        NA,
        Single,
        All,
    }

    public enum type_SendLayoutFlag
    {
        Clear,
        Initialize,
        Profile,
        Effect,
        Tempo,
        Brightness,
        Direction,
        ColorPad,
        ColorSingle,
        ColorClear,
        //Seconds,
    }

    public class MyDefine
    {
        public enum mouse_status { n,h,p,d }; // normal, highlight,pressed,disable

    }

    public class Type_RGBKY_DLL
    {
        public const int ME_KB_FAIL = 0;
        public const int FULL_ME_KB = 1;
        public const int FOURZONE_ME_KB = 2;
    }

    public class Err_RGBKB_DLL
    {
        public const int USB_DEVICE_NOTCONNECTED = -2;
    }

    public static class APMessage
    {
        public const string WINDOW_NAME_MYAPPTRAY = "MyAPPTray";
        public const string WINDOW_NAME_MYAPP = "MyAPP";
        public const string WINDOW_NAME_MYCOLOR = "MyColor";
        public const string WINDOW_NAME_MYMACROKEY = "MyMacrokey";
        public const string WINDOW_NAME_MYFANBOOST = "MyFanBoost";
        public const string WINDOW_NAME_MYCOLOR2 = "MyColor2";
        public const string WINDOW_NAME_OSD = "OEMOSD";

        // 0x5446 = TF OTA, 0x5447 = TF MYAPP, 0x5448 = TF MyColor2
        public const int WM_MSG_MYAPP = 0x5446 + 1;
        public const int WM_MSG_MYCOLOR2 = 0x5446 + 2;
        public const int WM_MSG_OOBE = 0x5446 + 3;
        //#define OSD_MYCOLOR2_EVENT      (WM_USER + 201)
        public const int WM_MSG_OSD = 0x0400 + 201;

        public const int MSG_MYAPP_AP_OPEN = 0x001;
        public const int MSG_MYAPP_AP_CLOSE = 0x002;
        public const int MSG_MYAPP_AP_SHOW = 0x003;
        public const int MSG_MYAPP_AP_HIDE = 0x004;
        public const int MSG_MYAPP_AP_MINIMIZE = 0x005;
        public const int MSG_MYAPP_EVENT_WMI = 0x020;

        public const int MSG_MYCOLOR_AP_OPEN = 0x101;
        public const int MSG_MYCOLOR_AP_CLOSE = 0x102;
        public const int MSG_MYCOLOR_AP_SHOW = 0x103;
        public const int MSG_MYCOLOR_AP_HIDE = 0x104;
        public const int MSG_MYCOLOR_AP_MINIMIZE = 0x105;

        public const int MSG_MYMACROKEY_AP_OPEN = 0x201;
        public const int MSG_MYMACROKEY_AP_CLOSE = 0x202;
        public const int MSG_MYMACROKEY_AP_SHOW = 0x203;
        public const int MSG_MYMACROKEY_AP_HIDE = 0x204;
        public const int MSG_MYMACROKEY_AP_MINIMIZE = 0x205;

        public const int MSG_MYFANBOOST_AP_OPEN = 0x301;
        public const int MSG_MYFANBOOST_AP_CLOSE = 0x302;
        public const int MSG_MYFANBOOST_AP_SHOW = 0x303;
        public const int MSG_MYFANBOOST_AP_HIDE = 0x304;
        public const int MSG_MYFANBOOST_AP_MINIMIZE = 0x305;
        public const int MSG_MYFANBOOST_EVENT_POWER = 0x320;

        public const int MSG_MYCOLOR2_AP_OPEN = 0x401;
        public const int MSG_MYCOLOR2_AP_CLOSE = 0x402;
        public const int MSG_MYCOLOR2_AP_SHOW = 0x403;
        public const int MSG_MYCOLOR2_AP_HIDE = 0x404;
        public const int MSG_MYCOLOR2_AP_MINIMIZE = 0x405;
        public const int MSG_MYCOLOR2_AP_RGBUP = 0x406;
        public const int MSG_MYCOLOR2_AP_RGBDOWN = 0x407;
        public const int MSG_MYCOLOR2_AP_TIPSHOW = 0x408;
        public const int MSG_MYCOLOR2_AP_TIPHIDE = 0x409;
        public const int MSG_MYCOLOR2_AP_ENABLE_BREATHING = 0x40A;
        public const int MSG_MYCOLOR2_AP_DISABLE_BREATHING = 0x40B;

    }

    public static class PowerSaving
    {
        public const int CloseTiming = 600000;  // 10 min to close LED

    }

    static class ECSpec
    {
        [Flags]
        public enum MyFanCTLByteFlag
        {
            Normal_Mode = 0x00, //default
            FanBoost_Mode = 0x40,
            User_Fan_Mode = 0x80,
            User_Fan_Level1 = 0x81,
            User_Fan_Level2 = 0x82,
            User_Fan_Level3 = 0x83,
            User_Fan_Level4 = 0x84,
            User_Fan_Level5 = 0x85
        }
        [Flags]
        public enum TriggerByteFlag
        {
            WinLock_Trigger = 0x01,
            LightBar_Trigger = 0x02,
            FanBoost_Trigger = 0x04,
            SilentMode_Trigger = 0x08,
            USBCharger_Trigger = 0x10,
            RGBKeybaord_Trigger = 0x20,
            RGBLogo_Trigger = 0x40,
            RGBKeybaordWelcome_Trigger = 0x80
        }
        [Flags]
        public enum SupportByteOneFlag
        {
            AirplaneMode = 0x01,
            GPSSwitch = 0x02,
            OverClock = 0x04,
            MacroKey = 0x08,
            ShortCutKey = 0x10,
            WinLockKey = 0x20,
            LightBar = 0x40,
            FanBoost = 0x80
        }
        [Flags]
        public enum SupportByteTwoFlag
        {
            SilentMode = 0x01,
            USBChargerMode = 0x02,
            RGBKeyBoard = 0x04,
            MyBat = 0x40
        }
        [Flags]
        public enum SupportByteThreeFlag
        {
            //**************************
            //IMPORTANT!!!!!!!!!!!!!!!!!
            FullZone = 0x01, //************** Notice that bit0 = 0 =-> support fullzone **************
            //***************************
            FourZone = 0x02,
            FourZoneReady = 0x04,
        }
        [Flags]
        public enum StatusByteOneFlag
        {
            WinLock = 0x01,
            BreathLed = 0x02,
            FanBoost = 0x04,
            MacroKey = 0x08,
            MyBatPowerBat = 0x10
        }
        public const ushort BIOSFuncReg = 0x0476;
        public const ushort ecPowSource = 0x0490;
        public const ushort ecBt1Temperature = 0x04A2;
        public const ushort ecBt1RSOC = 0x04AB;

        public const uint FAN_MODE_NORMAL = 0;
        public const uint FAN_MODE_BOOST = 1;
        public const uint FAN_MODE_CUSTOMIZE = 2;

        public const uint FAN_LEVEL_ONE = 1;
        public const uint FAN_LEVEL_TWO = 2;
        public const uint FAN_LEVEL_THREE = 3;
        public const uint FAN_LEVEL_FOUR = 4;
        public const uint FAN_LEVEL_FIVE = 5;


        public const ushort ADDR_MAFAN_CONTROL_BYTE = 0x0751;
        public const ushort ADDR_TRIGGER_BYTE2 = 0x075D;
        public const ushort ADDR_SUPPORT_BYTE1 = 0x0765;
        public const ushort ADDR_SUPPORT_BYTE2 = 0x0766;
        public const ushort ADDR_SUPPORT_BYTE3 = 0x073C;
        public const ushort ADDR_TRIGGER_BYTE = 0x0767;
        public const ushort ADDR_STAUTS_BYTE = 0x0768;

        public const ushort OSD_CAPSLOCK = 0x001;
        public const ushort OSD_NUMLOCK = 0x002;
        public const ushort OSD_SROLLOCK = 0x003;
        public const ushort OSD_TPON = 0x004;
        public const ushort OSD_TPOFF = 0x005;
        public const ushort OSD_SILENTON = 0x006;
        public const ushort OSD_SILENTOFF = 0x007;
        public const ushort OSD_WLANON = 0x008;
        public const ushort OSD_WLANOFF = 0x009;
        public const ushort OSD_WINMAXON = 0x00A;
        public const ushort OSD_WINMAXOFF = 0x00B;
        public const ushort OSD_BTON = 0x00C;
        public const ushort OSD_BTOFF = 0x00D;
        public const ushort OSD_RFON = 0x00E;
        public const ushort OSD_RFOFF = 0x00F;
        public const ushort OSD_3GON = 0x010;
        public const ushort OSD_3GOFF = 0x011;
        public const ushort OSD_WEBCAMON = 0x012;
        public const ushort OSD_WEBCAMOFF = 0x013;
        public const ushort OSD_BRIGHTNESSUP = 0x014;
        public const ushort OSD_BRIGHTNESSDOWN = 0x015;
        public const ushort OSD_RADIOON = 0x01A;
        public const ushort OSD_RADIOOFF = 0x01B;
        public const ushort OSD_POWERSAVEON = 0x031;
        public const ushort OSD_POWERSAVEOFF = 0x032;
        public const ushort OSD_MENU = 0x034;
        public const ushort OSD_MUTE = 0x035;
        public const ushort OSD_VOLUMEDOWN = 0x036;
        public const ushort OSD_VOLUMEUP = 0x037;
        public const ushort OSD_OSD_MENU_2 = 0x038;
        public const ushort OSD_BREATH_LED_ON = 0x039;
        public const ushort OSD_BREATH_LED_OFF = 0x03A;
        public const ushort OSD_KB_LED_LEVEL0 = 0x03B;
        public const ushort OSD_KB_LED_LEVEL1 = 0x03C;
        public const ushort OSD_KB_LED_LEVEL2 = 0x03D;
        public const ushort OSD_KB_LED_LEVEL3 = 0x03E;
        public const ushort OSD_KB_LED_LEVEL4 = 0x03F;
        public const ushort OSD_WINKEY_LOCK = 0x040;
        public const ushort OSD_WINKEY_UNLOCK = 0x041;
        public const ushort OSD_MENU_JP = 0x042;
        public const ushort OSD_CAMERAON = 0x090;
        public const ushort OSD_CAMERAOFF = 0x091;
        public const ushort OSD_LCD_SW = 0x0A9;
        public const ushort OSD_FAN_OVER_TEMP = 0x0AA;
        public const ushort OSD_MyBat_ACUpdate = 0x0AB;
        public const ushort OSD_MyBat_HPOff = 0x0AC;
        public const ushort OSD_RGBKBBKL_LEVEL_UPDATE = 0x0F0;  // RGB Keyboard Backlight Level

    }
}
