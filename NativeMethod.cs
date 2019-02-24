using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyColor2
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeKBRGB
    {
        public byte ID;
        public byte R;
        public byte G;
        public byte B;
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct DLLBuffer
    {
        public byte Mode;
        public byte Effect;
        public byte Brightnesslevel;
        public byte Tempolevel;
        public byte Direction;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte [] singleRGB;

        public uint user_sec;

        public bool bSingleDisplay;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public NativeKBRGB [] userRGB;   // circle 0 = nost used 1 ~ 7 = windows color
                                         // circle 8 = nost used 9 ~ f = welcome color

        public byte bkeyCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 126)]
        public NativeKBRGB[] KBRGB;

        public bool savingmode; //reserved
        public bool NVsaving; //reserved

    };


    [StructLayout(LayoutKind.Sequential)]
    public struct LEDTYPE_08_SET
    {
        public byte bEffectType;
        public byte bSpeed;
        public byte blight;
        public byte bColor;
        public byte bdirection;
        public bool bsavingMode;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct FOURZONE_DLLBUFFER
    {
        public byte bMode;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public LEDTYPE_08_SET _TSLedType_08H;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public NativeKBRGB[] userRGB;   // circle 0 = nost used 1 ~ 7 = windows color
                                        // circle 8 = nost used 9 ~ f = welcome color

    };

    static class NativeMethod
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool STARTMONITORAUDIO(ushort nParam);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void STOPMONITORAUDIO();

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        //public static extern int PostMessage(IntPtr hwnd, uint wMsg, int wParam, IntPtr lParam);
        public static extern int PostMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("OemServiceWinApp.dll",CharSet =CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OemSvcHook(int argc, String[] argv, IntPtr key, int keysize);
        //public static extern bool Test(int argc, String[] argv);

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        //  public static extern bool Test(int ac, String[] av, byte[] buffer, uint buffersize);
        public static extern int GRGBDLL_GetMEKBTYPE();

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        //  public static extern bool Test(int ac, String[] av, byte[] buffer, uint buffersize);
        public static extern bool GRGBDLL_CaptureCMDBuffer(ref DLLBuffer colorbuf);

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        //  public static extern bool Test(int ac, String[] av, byte[] buffer, uint buffersize);
        public static extern int GRGBDLL_Capture4ZONEBuffer(ref FOURZONE_DLLBUFFER colorbuf);

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GRGBDLL_SwitchLight_4Z(bool onoff, ref FOURZONE_DLLBUFFER colorbuf);
		
		[DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GRGBDLL_SwitchLight(bool onoff, ref DLLBuffer colorbuf);

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GRGBDLL_InitialDLL();
		//public static extern bool GRGBDLL_InitialDLL(int a_VendorID, int a_ProductID);

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GRGBDLL_DestroyDLL();

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern byte GRGBDLL_GetBacklight();

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern byte GRGBDLL_iSetWelColor_14H(NativeKBRGB [] kbRGB);

        [DllImport("GRGBKB_DLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GRGBDLL_GET_Firmware_Version(ref byte bHVer, ref byte bLVer);

        [DllImport(@"User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);
        [DllImport(@"User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }
        public static Guid GUID_MONITOR_POWER_ON = new Guid(0x02731015, 0x4510, 0x4526, 0x99, 0xE6, 0xE5, 0xA1, 0x7E, 0xBD, 0x1A, 0xEA);
        public static Guid GUID_CONSOLE_DISPLAY_STATE = new Guid(0x6fe69556, 0x704a, 0x47a0, 0x8f, 0x24, 0xc2, 0x8d, 0x93, 0x6f, 0xda, 0x47);
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;

        public const int WH_MOUSE_LL = 14;
        public const int WH_KEYBOARD_LL = 13;
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto,CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int UnhookWindowsHookEx(int idHook);
    }
}
