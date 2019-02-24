    using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyColor2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IntPtr m_hPowerNotify;
        private static IntPtr m_hwnd;
        
        private static IntPtr m_hAudio;
        private static IntPtr m_pStartMonitorAudio;
        private static IntPtr m_pStopMonitorAudio;
        private static NativeMethod.STARTMONITORAUDIO m_delStartMonitorAudio;
        private static NativeMethod.STOPMONITORAUDIO m_delStopMonitorAudio;
        
        private static DLLBuffer m_windows_DLLBuffer;
        private static DLLBuffer m_welcome_DLLBuffer;
        private static FOURZONE_DLLBUFFER m_windows_FOURZONE_DLLBuffer;
        private static FOURZONE_DLLBUFFER m_welcome_FOURZONE_DLLBuffer;
        private static layoutStatus m_windows_layoutstatus;
        private static layoutStatus m_welcome_layoutstatus;

        private static uint m_sendflag = (uint)type_SendLayoutFlag.Clear;

        private static int m_devie_type = Type_RGBKY_DLL.ME_KB_FAIL;
        private static int m_FWVer = 0;
        private static int m_profile = 0;
        private static uint m_mode = (uint)mode.windows;

        //Light Ball mouse parameter
        private static bool m_isLightBall_MouseDown = false;

        //Tempo Ball mouse parameter
        private static bool m_isTempoBall_MouseDown = false;

        private static uint m_trigger = (uint)tirgger.auto;

        private static bool m_isPowerOn = status.on;
        private static bool m_isPowerSaving = status.off;

        private static int s_MouseHookHandle;
        private static int s_KeyboardHookHandle;
        private static NativeMethod.HookProc s_MouseDelegate;
        private static NativeMethod.HookProc s_KeyBoardDelegate;
        private static AutoResetEvent Hook_Event = new AutoResetEvent(false);
        private static Thread HookThread;
        private static bool HookThread_Exit = false;
        private static bool m_bHookLedOnOff = true;
        

        //private static ManagementEventWatcher m_Sound_InsertWatcher;
        //private static ManagementEventWatcher m_Sound_DeleteWatcher;
        private bool bWKD_Panel_OnOffEvent_Initialized = false; // Workaround bit to avoid windows flash and enable Music Mode twice when AP start. This is because panel on event will trigger anyway when AP start.

        /*##############################################*/
        //add by Hans,at 20170708
        //for Second generation keyboard
        /*##############################################*/
        IKBTypeInterface KBType = null;
        int m_iKBType = (int) enum_KBTypeXamlID._1st_FKBT;
        uint level = 2U;

        private void CheckRGBDevice()
        {
            /*
             * Sigle Zone : 0766h.2 = 1
             * Four Zone : 0766h.2 = 0  073Ch.1 = 1
             * Full Zone : 0766.2 =0 073Ch.1 = 0  073h.0 = 0  <== Notice that the magic number
             * Normal Keyboard : 0766.2 =0 073Ch.1 = 0  073h.0 = 1            
             */
             /*
            byte m_0766 = 0x00;
            byte m_073C = 0x00;
            NativeRWEC.Load();
            NativeRWEC.READ(ECSpec.ADDR_SUPPORT_BYTE2, ref m_0766);
            System.Threading.Thread.Sleep(200);
            NativeRWEC.READ(ECSpec.ADDR_SUPPORT_BYTE3, ref m_073C);
            System.Threading.Thread.Sleep(200);
            NativeRWEC.Unload();

            if ((m_0766 & (byte)ECSpec.SupportByteTwoFlag.RGBKeyBoard) == (byte)ECSpec.SupportByteTwoFlag.RGBKeyBoard)
                Environment.Exit(0);
            else if ((m_073C & (byte)ECSpec.SupportByteThreeFlag.FourZone) == (byte)ECSpec.SupportByteThreeFlag.FourZone)
                { }//Four Zone
            else if ((m_073C & (byte)ECSpec.SupportByteThreeFlag.FullZone) == 0x00) //************** Notice that bit0 = 0 =-> support fullzone **************
                { }//Full zone
            else
                Environment.Exit(0);
            */
            try
            {
                //if (NativeMethod.GRGBDLL_InitialDLL() == Err_RGBKB_DLL.USB_DEVICE_NOTCONNECTED)

                /*###################################################
                 //add by Hans.
                 //at 20170711, for Detect KB Type in the Get Oem Svc 
                ###################################################*/
                try
                {
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_101;
                    //GetOEMSVC_KBType();
                }
                catch
                {
                    MessageBox.Show("Get OemService Failed");
                    m_devie_type = NativeMethod.GRGBDLL_GetMEKBTYPE();
                    m_iKBType = (int)enum_KBTypeXamlID._1st_FKBT;
                }

            //    MessageBox.Show(String.Format("[Initialize]CheckRGBDevice m_iKBType ==> {0}", m_iKBType));
             //   MessageBox.Show(String.Format("[Initialize]CheckRGBDevice GRGBDLL_GetMEKBTYPE ==> {0}", m_devie_type));

                if (m_devie_type == Type_RGBKY_DLL.ME_KB_FAIL)
                {
                    Utility.Log("Error]USB_DEVICE_NOTCONNECTED");
#if DEMO			
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB; // For UI demo
                  //  Environment.Exit(0);
#else				  		
                    Environment.Exit(0);
#endif					
                }
                Utility.Log("[Initialize]GRGBDLL_InitialDLL Finished");     

                m_hAudio = NativeMethod.LoadLibrary("audiostealer.dll");
                Utility.Log("[Initialize]LoadLibrary audiostealer.dll Finished");
                if (m_hAudio != IntPtr.Zero)
                {
                    m_pStartMonitorAudio = NativeMethod.GetProcAddress(m_hAudio, "StartMonitorAudio");
                    m_pStopMonitorAudio = NativeMethod.GetProcAddress(m_hAudio, "StopMonitorAudio");
                    if ((m_pStartMonitorAudio != IntPtr.Zero) && (m_pStopMonitorAudio != IntPtr.Zero))
                    {
                        m_delStartMonitorAudio = (NativeMethod.STARTMONITORAUDIO)Marshal.GetDelegateForFunctionPointer(
                                                    m_pStartMonitorAudio,
                                                    typeof(NativeMethod.STARTMONITORAUDIO));
                        m_delStopMonitorAudio = (NativeMethod.STOPMONITORAUDIO)Marshal.GetDelegateForFunctionPointer(
                                                    m_pStopMonitorAudio,
                                                    typeof(NativeMethod.STOPMONITORAUDIO));
                        if ((m_delStartMonitorAudio == null) && (m_delStopMonitorAudio == null))
                        {
                            Utility.Log("[Initialize]audiostealer Delegate mapping failed");
                        }

                    }
                    else
                    {
                        Utility.Log("[Error]audiostealer GetProcAddress  failed");
                    }
                    

                }
                else
                {
                    Utility.Log("[Error]audiostealer LoadLibrary  failed");
                }
           

            }
            catch (Exception e)
            {
                Utility.Log(String.Format("[Error]CheckRGBDevice exception {0}", e.ToString()));
#if DEMO
                //  Environment.Exit(0);
#else
                    Environment.Exit(0);
#endif
            }
        }

        /*
        private void GetOEMSVC_KBType()
        {
            string[] av = new string[] { "OemServiceWinApp.exe", "KBLBID", "/GetKBL" };
            IntPtr buffer = Marshal.AllocHGlobal(128);
            char cHChar = '0';
            char cLChar = '0';
            int iCMD = 0;
            Utility.Log("[Trace]GetOemServiceKBType OemSvcHook start");
            //NativeMethod.OemSvcHook(3, av, buffer, 128);
            Utility.Log("[Trace]GetOemServiceKBType OemSvcHook end");
            byte[] managedData = new byte[128];
            Marshal.Copy(buffer, managedData, 0, managedData.Length);
            Marshal.FreeHGlobal(buffer);

            cHChar = Convert.ToChar(managedData[0]);
            cLChar = Convert.ToChar(managedData[1]);
            managedData[0] = (byte)char.GetNumericValue(cHChar);
            managedData[1] = (byte)char.GetNumericValue(cLChar);
            iCMD = (managedData[0] << 4) + managedData[1];
            string sCMD = string.Format("{0}{1}", cHChar, cLChar);
            //if(managedData[0])
            //string hexString = "1";
            //int num = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

            //m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
            //m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_101;
            Utility.Log(string.Format("[Trace]managedData[0] = {0}", managedData[0].ToString()));
            switch (iCMD)
            {
                case (Byte)enum_KBTypeXamlID._1st_FKBT:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._1st_FKBT;
                    break;
                case (Byte)enum_KBTypeXamlID._1st_4KBT:
                    m_devie_type = Type_RGBKY_DLL.FOURZONE_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._1st_4KBT2;
                    break;
                case (Byte)enum_KBTypeXamlID._1st_4KBT2:
                    m_devie_type = Type_RGBKY_DLL.FOURZONE_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._1st_4KBT2;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_101:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_102;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_101M:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_101M;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_102:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_101;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_102M:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_102M;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_87:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_87;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_87M:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_87M;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_88:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_88;
                    break;
                case (Byte)enum_KBTypeXamlID._2nd_KBT_88M:
                    m_devie_type = Type_RGBKY_DLL.FULL_ME_KB;
                    m_iKBType = (int)enum_KBTypeXamlID._2nd_KBT_88M;
                    break;
                default:
                    m_devie_type = NativeMethod.GRGBDLL_GetMEKBTYPE();
                    m_iKBType = (int)enum_KBTypeXamlID._1st_FKBT;
                    break;
            }
        }
        */
        private void InitializeParameter()
        {
            if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                this.WindowStyle = System.Windows.WindowStyle.None;
                m_windows_DLLBuffer = new DLLBuffer();
                m_windows_DLLBuffer.singleRGB = new byte[4];
                m_windows_DLLBuffer.userRGB = new NativeKBRGB[16];
                m_windows_DLLBuffer.KBRGB = new NativeKBRGB[126];
                m_windows_DLLBuffer.bkeyCount = 126; // reserved

                m_welcome_DLLBuffer = new DLLBuffer();
                m_welcome_DLLBuffer.singleRGB = new byte[4];
                m_welcome_DLLBuffer.userRGB = new NativeKBRGB[16];
                m_welcome_DLLBuffer.KBRGB = new NativeKBRGB[126];
                m_welcome_DLLBuffer.bkeyCount = 126; // reserved

                m_windows_layoutstatus = new layoutStatus();
                m_windows_layoutstatus.effect = (uint)effect.Rainbow;
                m_windows_layoutstatus.brightness_level = 4;
                m_windows_layoutstatus.tempo_level = 5;
                m_windows_layoutstatus.effectColorList = new List<EffectColor>()
                {
                    new EffectColor((uint)effect.Static, 1, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Breathing, 7, true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Wave, 7, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Ripple, 7, true, true, 0xff, 0x00, 0x00 ),
                    new EffectColor((uint)effect.Reactive, 7, true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Marquee, 7, false, false, 0x00, 0x00, 0x00 ),
                    new EffectColor((uint)effect.Raindrop, 7,  true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Stacker, 3, false, false, 0x00, 0x00, 0x00 ),
                    new EffectColor((uint)effect.Impact, 3, false, false, 0x00, 0x00, 0x00 ),
                    new EffectColor((uint)effect.Aurora, 7,  true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Spark, 7,  true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Neon, 7,  false, false, 0x00, 0x00, 0x00),

                };
                m_windows_layoutstatus.effectdirection = new List<EffectDirection>()
                {
                    new EffectDirection((uint)effect.Static, 0),
                    new EffectDirection((uint)effect.Wave,   0),
                    new EffectDirection((uint)effect.Ripple,   0),
                    new EffectDirection((uint)effect.Reactive,   0),
                    new EffectDirection((uint)effect.Spark,   0),
                    new EffectDirection((uint)effect.Aurora,   0),
                };

                m_welcome_layoutstatus = new layoutStatus();
                m_welcome_layoutstatus.effect = (uint)effect.Breathing;
                m_welcome_layoutstatus.brightness_level = 4;
                m_welcome_layoutstatus.tempo_level = 5;
                m_welcome_layoutstatus.effectColorList = new List<EffectColor>()
                {
                    new EffectColor((uint)effect.Breathing, 7, true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Wave, 7, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Marquee, 7, false, false, 0x00, 0x00, 0x00 ),
                    new EffectColor((uint)effect.Raindrop, 7,  true, true, 0xff, 0x00, 0x00),

                };
                m_welcome_layoutstatus.effectdirection = new List<EffectDirection>()
                {
                    new EffectDirection((uint)effect.Wave,   0),
                    new EffectDirection((uint)effect.Ripple,   0),
                    new EffectDirection((uint)effect.Reactive,   0),
                };

            }
            else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                this.WindowStyle = System.Windows.WindowStyle.None;
                m_windows_FOURZONE_DLLBuffer = new FOURZONE_DLLBUFFER();
                m_windows_FOURZONE_DLLBuffer.userRGB = new NativeKBRGB[16];

                m_welcome_FOURZONE_DLLBuffer = new FOURZONE_DLLBUFFER();
                m_welcome_FOURZONE_DLLBuffer.userRGB = new NativeKBRGB[16];

                m_windows_layoutstatus = new layoutStatus();
                m_windows_layoutstatus.effect = (uint)effect.Rainbow;
                m_windows_layoutstatus.brightness_level = 4;
                m_windows_layoutstatus.tempo_level = 5;
                m_windows_layoutstatus.effectColorList = new List<EffectColor>()
                {
                    new EffectColor((uint)effect.Static, 4, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Breathing, 7, true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Wave, 7, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Flash, 7, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Mix, 7, false, false, 0x00, 0x00, 0x00),

                };
                m_windows_layoutstatus.effectdirection = new List<EffectDirection>()
                {
                    new EffectDirection((uint)effect.Wave,   0),
                    new EffectDirection((uint)effect.Flash, 0),
                };

                m_welcome_layoutstatus = new layoutStatus();
                m_welcome_layoutstatus.effect = (uint)effect.Breathing;
                m_welcome_layoutstatus.brightness_level = 4;
                m_welcome_layoutstatus.tempo_level = 5;
                m_welcome_layoutstatus.effectColorList = new List<EffectColor>()
                {
                    new EffectColor((uint)effect.Static, 4, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Breathing, 7, true, true, 0xff, 0x00, 0x00),
                    new EffectColor((uint)effect.Wave, 7, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Flash, 7, false, false, 0x00, 0x00, 0x00),
                    new EffectColor((uint)effect.Mix, 7, false, false, 0x00, 0x00, 0x00),

                };
                m_welcome_layoutstatus.effectdirection = new List<EffectDirection>()
                {
                    new EffectDirection((uint)effect.Wave,   0),
                    new EffectDirection((uint)effect.Flash, 0),
                };
            }
            else
            {
                //other devicde
            }
			
			// Check registry setting
            Utility.Log("[Initialize]MyColor2 finished parameter initialized");

            string version = null;
            try
            {
                object obj = Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "version", "x.xx");
                version = obj.ToString();
            }
            catch
            {
                Utility.Log("[Error]Read MyColor2 registry version failed");
            }
            finally
            {
                if (version != "1.07")
                {
                    Utility.Log("[Initialize]MyColor2 registry version is old and then update");
                    string filepath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                    string arg = "/s MyColor2.reg " + filepath;
                    Process regeditProcess = Process.Start("regedit.exe", arg);
                    regeditProcess.WaitForExit();
                    Utility.Log("[Initialize]MyColor2 update finished");
                }
            }
        }
        private void LoadProfileStatus()
        {
            Utility.Log("[Trace]LoadProfileStatus start ");
            try
            {
                string profilePath = @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\" + m_profile.ToString();
                if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                {
                    Brightness_Offset.Value = Convert.ToDouble(Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "Brightness_perc", "0"));
                    Speed_Offset.Value = Convert.ToDouble(Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "Speed_perc", "0"));
                    int effect = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "effect", 0);
                    m_windows_layoutstatus.effect = (uint)effect;

                    int Brightnesslevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "Brightnesslevel", 4);
                    m_windows_layoutstatus.brightness_level = (byte)Brightnesslevel;

                    int Tempolevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "Tempolevel", 5);
                    m_windows_layoutstatus.tempo_level = (byte)Tempolevel;

                    byte[] Direction = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "Direction", 0);
                    int i = 0;
                    foreach (EffectDirection d in m_windows_layoutstatus.effectdirection)
                    {
                        d.direction_index = Direction[i];
                        i++;
                    }

                    byte[] RGB = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "RGBColor", 0);
                    int j = 0;
                    foreach (EffectColor d in m_windows_layoutstatus.effectColorList)
                    {
                        d.color_single_status = (RGB[j] > 0) ? true : false;
                        d.color_single_r = RGB[j + 1];
                        d.color_single_g = RGB[j + 2];
                        d.color_single_b = RGB[j + 3];
                        j += 4;

                        foreach (ColorBlock r in d.colorblock)
                        {
                            r.R = RGB[j];
                            r.G = RGB[j + 1];
                            r.B = RGB[j + 2];
                            j += 3;
                        }
                    }
                    byte[] KeyBoardRGB = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "KeyBoardRGB", 0);
                    int k = 0;
                    //foreach (key key in KBTable.m_KeyList)
                    foreach(key key in KBType.KBTI_KeyList)
                    {
                        //key.df_id1 = KeyBoardRGB[k];
                        //key.df_id2 = KeyBoardRGB[k + 1];
                        key.R = KeyBoardRGB[k + 2];
                        key.G = KeyBoardRGB[k + 3];
                        key.B = KeyBoardRGB[k + 4];
                        k += 5;
                    }

                    effect = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "WelEffect", 0);
                    m_welcome_layoutstatus.effect = (uint)effect;

                    //Get OemService Effect here, and follow oemservice define.

                    int oemservice_effect = AntiTranslateDarfonEffect(GetOemServiceEffectStatus());
                    if (m_welcome_layoutstatus.effect != (uint)oemservice_effect)
                    {
                        m_welcome_layoutstatus.effect = (uint)oemservice_effect;
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, profilePath, "WelEffect", oemservice_effect, RegistryValueKind.DWord);
                    }


                    Brightnesslevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "WelBrightnesslevel", 4);
                    m_welcome_layoutstatus.brightness_level = (byte)Brightnesslevel;

                    Tempolevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "WelTempolevel", 5);
                    m_welcome_layoutstatus.tempo_level = (byte)Tempolevel;

                    Direction = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "WelDirection", 0);
                    i = 0;
                    foreach (EffectDirection d in m_welcome_layoutstatus.effectdirection)
                    {
                        d.direction_index = Direction[i];
                        i++;
                    }

                    RGB = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "WelRGBColor", 0);
                    j = 0;
                    foreach (EffectColor d in m_welcome_layoutstatus.effectColorList)
                    {
                        d.color_single_status = (RGB[j] > 0) ? true : false;
                        d.color_single_r = RGB[j + 1];
                        d.color_single_g = RGB[j + 2];
                        d.color_single_b = RGB[j + 3];
                        j += 4;

                        foreach (ColorBlock r in d.colorblock)
                        {
                            r.R = RGB[j];
                            r.G = RGB[j + 1];
                            r.B = RGB[j + 2];
                            j += 3;
                        }
                    }

                    KeyBoardRGB = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "WelKeyBoardRGB", 0);
                    k = 0;
                    //foreach (key key in KBTable.m_WelKeyList)
                    foreach (key key in KBType.KBTI_WelKeyList)
                    {
                        //key.df_id1 = KeyBoardRGB[k];
                        //key.df_id2 = KeyBoardRGB[k + 1];
                        key.R = KeyBoardRGB[k + 2];
                        key.G = KeyBoardRGB[k + 3];
                        key.B = KeyBoardRGB[k + 4];
                        k += 5;
                    }

                }
                else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                {

                    int effect = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_effect", 0);
                    m_windows_layoutstatus.effect = (uint)effect;

                    int Brightnesslevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_Brightnesslevel", 4);
                    m_windows_layoutstatus.brightness_level = (byte)Brightnesslevel;

                    int Tempolevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_Tempolevel", 5);
                    m_windows_layoutstatus.tempo_level = (byte)Tempolevel;

                    byte[] Direction = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_Direction", 0);
                    int i = 0;
                    foreach (EffectDirection d in m_windows_layoutstatus.effectdirection)
                    {
                        d.direction_index = Direction[i];
                        i++;
                    }

                    byte[] RGB = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_RGBColor", 0);
                    int j = 0;
                    foreach (EffectColor d in m_windows_layoutstatus.effectColorList)
                    {
                        d.color_single_status = (RGB[j] > 0) ? true : false;
                        d.color_single_r = RGB[j + 1];
                        d.color_single_g = RGB[j + 2];
                        d.color_single_b = RGB[j + 3];
                        j += 4;

                        foreach (ColorBlock r in d.colorblock)
                        {
                            r.R = RGB[j];
                            r.G = RGB[j + 1];
                            r.B = RGB[j + 2];
                            j += 3;
                        }
                    }

                    effect = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_WelEffect", 0);
                    m_welcome_layoutstatus.effect = (uint)effect;

                    //Get OemService Effect here, and follow oemservice define.

                    //Get OemService Effect here, and follow oemservice define.

                    int oemservice_effect = AntiTranslateDarfonEffect(GetOemServiceEffectStatus());
                    if (m_welcome_layoutstatus.effect != (uint)oemservice_effect)
                    {
                        m_welcome_layoutstatus.effect = (uint)oemservice_effect;
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, profilePath, "4Zone_WelEffect", oemservice_effect, RegistryValueKind.DWord);
                    }



                    Brightnesslevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_WelBrightnesslevel", 4);
                    m_welcome_layoutstatus.brightness_level = (byte)Brightnesslevel;

                    Tempolevel = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_WelTempolevel", 5);
                    m_welcome_layoutstatus.tempo_level = (byte)Tempolevel;

                    Direction = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_WelDirection", 0);
                    i = 0;
                    foreach (EffectDirection d in m_welcome_layoutstatus.effectdirection)
                    {
                        d.direction_index = Direction[i];
                        i++;
                    }

                    RGB = (byte[])Utility.RegistryKeyRead(RegistryHive.LocalMachine, profilePath, "4Zone_WelRGBColor", 0);
                    j = 0;
                    foreach (EffectColor d in m_welcome_layoutstatus.effectColorList)
                    {
                        d.color_single_status = (RGB[j] > 0) ? true : false;
                        d.color_single_r = RGB[j + 1];
                        d.color_single_g = RGB[j + 2];
                        d.color_single_b = RGB[j + 3];
                        j += 4;

                        foreach (ColorBlock r in d.colorblock)
                        {
                            r.R = RGB[j];
                            r.G = RGB[j + 1];
                            r.B = RGB[j + 2];
                            j += 3;
                        }
                    }
                }

            }
            catch(Exception e)
            {
                Utility.Log(String.Format("[Error]LoadProfileStatus failed = {0} ", e.ToString()));
            }

            Utility.Log("[Trace]LoadProfileStatus end ");
        }
        public void InitializeLayout()
        {/*
            try
            {
                ###################################################
                //add by Hans.
                //at 20170711, for Detect KB Type in the xaml  
                ###################################################*/
         /*  switch (m_iKBType)
           {
               case (int)enum_KBTypeXamlID._1st_FKBT:
               case (int)enum_KBTypeXamlID._1st_4KBT2:
                   //KBTable.Visibility = Visibility.Visible;
                   //KBType = KBTable;
                   break;
               case (int)enum_KBTypeXamlID._2nd_KBT_101:
                 //  MessageBox.Show("1");
                   KBTable102.Visibility = Visibility.Visible;
                   KBType = KBTable102;
                   break;
               case (int)enum_KBTypeXamlID._2nd_KBT_102:
                 //  MessageBox.Show("2");

                   KBTable102.Visibility = Visibility.Visible;
                   KBType = KBTable102;
                   break;
               case (int)enum_KBTypeXamlID._2nd_KBT_88:

                   break;
               case (int)enum_KBTypeXamlID._2nd_KBT_87:

                   break;
               default:
                   break;
           }
       }
       catch(Exception e)
       {
           Utility.Log(String.Format("[Error]MyColor2 UI sync to status failed {0}", e.ToString()));
           EndSendLayoutData((uint)type_SendLayoutFlag.Initialize, false);
       }
       */


            if (Convert.ToInt32(Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "Layout_usa", "0")) == 0)
            {
                KBType = KBTable102;
                KBTable102.Visibility = Visibility.Visible;
                KBTable101.Visibility = Visibility.Hidden;

            }
            else
            {
                KBType = KBTable101;
                KBTable102.Visibility = Visibility.Hidden;
                KBTable101.Visibility = Visibility.Visible;
            }
            //Import window / welcome combolist
            if ( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                ComboBoxItem_Windows_Effect_0.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_0.IsEnabled = true;
                ComboBoxItem_Windows_Effect_1.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_1.IsEnabled = true;
                ComboBoxItem_Windows_Effect_2.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_2.IsEnabled = true;
                ComboBoxItem_Windows_Effect_3.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_3.IsEnabled = true;
                ComboBoxItem_Windows_Effect_4.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_4.IsEnabled = true;
                ComboBoxItem_Windows_Effect_5.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_5.IsEnabled = true;
                ComboBoxItem_Windows_Effect_6.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_6.IsEnabled = true;
                ComboBoxItem_Windows_Effect_7.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_7.IsEnabled = true;
                ComboBoxItem_Windows_Effect_8.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_8.IsEnabled = true;
                ComboBoxItem_Windows_Effect_9.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_9.IsEnabled = true;
                ComboBoxItem_Windows_Effect_11.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_11.IsEnabled = true;
                ComboBoxItem_Windows_Effect_12.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_12.IsEnabled = true;
                ComboBoxItem_Windows_Effect_13.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_13.IsEnabled = true;
                ComboBoxItem_Windows_Effect_15.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_15.IsEnabled = true;
                ComboBoxItem_Windows_Effect_16.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_16.IsEnabled = true;

                ComboBoxItem_Welcome_Effect_1.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_1.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_2.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_2.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_5.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_5.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_6.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_6.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_7.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_7.IsEnabled = true;
                //ComboBoxItem_Welcome_Effect_13.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_13.IsEnabled = true;
            }
            else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB )
            {
                ComboBoxItem_Windows_Effect_0.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_0.IsEnabled = true;
                ComboBoxItem_Windows_Effect_1.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_1.IsEnabled = true;
                ComboBoxItem_Windows_Effect_2.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_2.IsEnabled = true;
                ComboBoxItem_Windows_Effect_5.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_5.IsEnabled = true;
                ComboBoxItem_Windows_Effect_17.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_17.IsEnabled = true;
                ComboBoxItem_Windows_Effect_18.Visibility = Visibility.Visible; ComboBoxItem_Windows_Effect_18.IsEnabled = true;

                ComboBoxItem_Welcome_Effect_0.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_0.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_1.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_1.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_2.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_2.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_5.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_5.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_17.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_17.IsEnabled = true;
                ComboBoxItem_Welcome_Effect_18.Visibility = Visibility.Visible; ComboBoxItem_Welcome_Effect_18.IsEnabled = true;


            }
            Utility.Log("[Initialize]MyColor2 UI effect list updated");

            StartSendLayoutData((uint)type_SendLayoutFlag.Initialize);
            try
            {
                m_profile = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "Profile", 0);
                comboBox_Profile.SelectedIndex = m_profile;  //Triger updae layout
                Utility.Log(String.Format("[Initialize]MyColor2 UI set Profile =  {0}", m_profile.ToString()));
                int intPS = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerSaving", 0);
                Utility.Log(String.Format("[Initialize]MyColor2 UI set Power Saving =  {0}", intPS.ToString()));
                m_isPowerSaving = (intPS > 0) ? status.on : status.off;
                string str_img_trigger_url;
                if (m_isPowerSaving)
                {
                    CreateListenThread();
                    CreateHookDelegate();
                    str_img_trigger_url = "..\\Image\\check_box_on.png";
                    Utility.Log("[Initialize] power saving on, trigger listen thread");

                }
                else
                {
                    DeleteHookDelegate();
                    DeleteListenThread();
                    str_img_trigger_url = "..\\Image\\check_box_off.png";
                    Utility.Log("[Initialize] power saving off, stop listen thread");
                }
                var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
                img_powersaving.Source = new BitmapImage(uriSource);

                //1st. Get registry power status.
                bool registryPower = ((int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerStatus", 1) > 0) ? true : false;
                //2nd. Get OemService power status. The condition is prior to registry !
                bool welcomePower = GetOemServicePowerStatus();
                Utility.Log(String.Format("[Initialize] registry_power_status = {0}, welcome_power_status = {1}", registryPower.ToString(), welcomePower.ToString()));

                if ((registryPower == status.on) && (welcomePower == status.on))
                {
                    m_isPowerOn = status.on;
                    //PowerOn(false); not need to update since initialization first.
                    EndSendLayoutData((uint)type_SendLayoutFlag.Initialize, true);
                }
                else if ((registryPower == status.on) && (welcomePower == status.off))
                {
                    m_isPowerOn = status.off;
                    PowerOff(true, false, false);
                    EndSendLayoutData((uint)type_SendLayoutFlag.Initialize, false);
                }
                else if ((registryPower == status.off) && (welcomePower == status.on))
                {
                    m_isPowerOn = status.on;
                    bool Power = m_isPowerOn;
                    Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerStatus", Power, RegistryValueKind.DWord);
                    EndSendLayoutData((uint)type_SendLayoutFlag.Initialize, true);
                }
                else
                {
                    m_isPowerOn = status.off;
                    PowerOff(false, false, false);
                    EndSendLayoutData((uint)type_SendLayoutFlag.Initialize, false);
                }
                Utility.Log( "[Initialize] set current setting to FW update effect and color");
            }
            catch(Exception e)
            {
                Utility.Log(String.Format("[Error]MyColor2 UI sync to status failed {0}", e.ToString()));
                EndSendLayoutData((uint)type_SendLayoutFlag.Initialize, false);
            }
 
        }
        private void SetLanguageDictionary()
        {
            int Lang = 0x409;
      //      string strLang = "en-US";
            try
            {
                // Lang = (int)Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\OEM\MyAPP\", "LCID", 0x409);
                //*****Important********
                // Sicne MyAPP serive is 32bit program, but MyColor is 64bit program. Must read from 32bit registry here for language info.
                Lang = (int)RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\OEM\MyAPP\").GetValue("LCID", 0x409);
            }
            catch
            {

            }
            ResourceDictionary dict = new ResourceDictionary();
            switch (Lang)
            {
                case 0x409:
                    dict.Source = new Uri("..\\Language\\en-US.xaml",
                                  UriKind.Relative);
               //     strLang = "en-US";
                    break;
                case 0x804:
                    dict.Source = new Uri("..\\Language\\zh-CN.xaml",
                                       UriKind.Relative);
               //     strLang = "zh-CN";
                    break;
                default:
                    dict.Source = new Uri("..\\Language\\en-US.xaml",
                                  UriKind.Relative);
               //     strLang = "en-US";
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);

        }
        private void SetBrightnessLayout(uint level, bool bSend)
        {
            Utility.Log(string.Format("[Trace]SetBrightnessLayout level={0}, bSend={1}", level, bSend));
            if (level == 0)
            {
                img_Brightness_ball.Margin = new Thickness(brightness_ball_margin.level0_x, brightness_ball_margin.height, 0, 0);
                img_Brightness_bar.Margin = new Thickness(140, 560, 692 + 47 * 4, 0);
            }
            else if (level == 1)
            {
                img_Brightness_ball.Margin = new Thickness(brightness_ball_margin.level1_x, brightness_ball_margin.height, 0, 0);
                img_Brightness_bar.Margin = new Thickness(140, 560, 692 + 47 * 3, 0);
            }
            else if (level == 2)
            {
                img_Brightness_ball.Margin = new Thickness(brightness_ball_margin.level2_x, brightness_ball_margin.height, 0, 0);
                img_Brightness_bar.Margin = new Thickness(140, 560, 692 + 47 * 2, 0);
            }
            else if (level == 3)
            {
                img_Brightness_ball.Margin = new Thickness(brightness_ball_margin.level3_x, brightness_ball_margin.height, 0, 0);
                img_Brightness_bar.Margin = new Thickness(140, 560, 692 + 47 * 1, 0);
            }
            else if (level == 4)
            {
                img_Brightness_ball.Margin = new Thickness(brightness_ball_margin.level4_x, brightness_ball_margin.height, 0, 0);
                img_Brightness_bar.Margin = new Thickness(140, 560, 0, 0);
            }

            if (bSend)
            {
                StartSendLayoutData((uint)type_SendLayoutFlag.Brightness);
                EndSendLayoutData((uint)type_SendLayoutFlag.Brightness, true);
            }
            Utility.Log("[Trace]SetBrightnessLayout finished");
        }
        private void SetTempoLayout(uint level, bool bSend)
        {
            Utility.Log(string.Format("[Trace]SetTempoLayout level={0}, bSend={1}", level, bSend));
            if (level == 0)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level0_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 9, 0);
            }
            else if (level == 1)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level1_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 8, 0);
            }
            else if (level == 2)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level2_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 7, 0);
            }
            else if (level == 3)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level3_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 6, 0);
            }
            else if (level == 4)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level4_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 5, 0);
            }
            else if (level == 5)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level5_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 4, 0);
            }
            else if (level == 6)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level6_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 3, 0);
            }
            else if (level == 7)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level7_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 2, 0);
            }
            else if (level == 8)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level8_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 335 + 21 * 1, 0);
            }
            else if (level == 9)
            {
                img_Tempo_ball.Margin = new Thickness(tempo_ball_margin.level9_x, tempo_ball_margin.height, 0, 0);
                img_Tempo_bar.Margin = new Thickness(498, 560, 0, 0);
            }
            if(bSend)
            {
                StartSendLayoutData((uint)type_SendLayoutFlag.Tempo);
                EndSendLayoutData((uint)type_SendLayoutFlag.Tempo, true);
            }
            Utility.Log("[Trace]SetTempoLayout finished");
        }
        private void EnableTrigger(bool enable)
        {
            if (enable)
            {
                Img_trigger_auto.IsEnabled = status.enable;
                Img_trigger_onkeypressed.IsEnabled = status.enable;
                string str_on_url = "..\\Image\\btn_on_n.png";
                Uri uri_on_Source = new Uri(str_on_url, UriKind.Relative);
                string str_off_url = "..\\Image\\btn_off_n.png";
                Uri uri_off_Source = new Uri(str_off_url, UriKind.Relative);

                if (m_trigger == (uint)tirgger.auto)
                {
                    Img_trigger_auto.Source = new BitmapImage(uri_on_Source);
                    Img_trigger_onkeypressed.Source = new BitmapImage(uri_off_Source);
                }
                else
                {
                    Img_trigger_auto.Source = new BitmapImage(uri_off_Source);
                    Img_trigger_onkeypressed.Source = new BitmapImage(uri_on_Source);
                }

            }
            else
            {
                Img_trigger_auto.IsEnabled = status.disable;
                Img_trigger_onkeypressed.IsEnabled = status.disable;
                string str_on_url = "..\\Image\\btn_on_d.png";
                Uri uri_on_Source = new Uri(str_on_url, UriKind.Relative);
                string str_off_url = "..\\Image\\btn_off_d.png";
                Uri uri_off_Source = new Uri(str_off_url, UriKind.Relative);

                if (m_trigger == (uint)tirgger.auto)
                {
                    Img_trigger_auto.Source = new BitmapImage(uri_on_Source);
                    Img_trigger_onkeypressed.Source = new BitmapImage(uri_off_Source);
                }
                else
                {
                    Img_trigger_auto.Source = new BitmapImage(uri_off_Source);
                    Img_trigger_onkeypressed.Source = new BitmapImage(uri_on_Source);
                }
            }
        }
        /*
        private void EnableSecEditor(bool enable)
        {
            edit_Second.IsEnabled = enable;
            if (enable)
            {
                foreach(ColorChangeTiming t in m_windows_layoutstatus.colorTimerList)
                {
                    if(t.effect_id == m_windows_layoutstatus.effect)
                    {
                        edit_Second.Text = t.seconds.ToString();
                        break;
                    }
                }
            }
            else
            {
                edit_Second.Text = "";
            }

        }
        */
        private void EnableDirection(bool enable, uint _type_direction)
        {
            Utility.Log(string.Format("[Trace]EnableDirection level={0}, bSend={1}", enable, _type_direction));
            if (enable)
            {
                comboBox_Direction.IsEnabled = true;
                comboBox_Direction.Items.Clear();
                if ((_type_direction == (uint)type_direction.direction) || (_type_direction == (uint)type_direction.direction_sync))
                {
                    if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                    {
                        comboBox_Direction.Items.Add(ComboBoxItem_Direction_LR);
                        comboBox_Direction.Items.Add(ComboBoxItem_Direction_RL);
                        comboBox_Direction.Items.Add(ComboBoxItem_Direction_TB);
                        comboBox_Direction.Items.Add(ComboBoxItem_Direction_BT);
                    }
                    else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                    {
                        comboBox_Direction.Items.Add(ComboBoxItem_Direction_LR);
                        comboBox_Direction.Items.Add(ComboBoxItem_Direction_RL);
                        if(_type_direction == (uint)type_direction.direction_sync)
                            comboBox_Direction.Items.Add(ComboBoxItem_Direction_Sync);
                    }                    
                }
                else if (_type_direction == (uint)type_direction.clockwise)
                {
                    comboBox_Direction.Items.Add(ComboBoxItem_Direction_CW);
                    comboBox_Direction.Items.Add(ComboBoxItem_Direction_CCW);

                }
                else if (_type_direction == (uint)type_direction.onkeypress)
                {
                    comboBox_Direction.Items.Add(ComboBoxItem_Direction_Auto);
                    comboBox_Direction.Items.Add(ComboBoxItem_Direction_OnKeyPressed);

                }

                layoutStatus l_layoutstatus = GetCurrentLayoutStatus();
                foreach (EffectDirection d in l_layoutstatus.effectdirection)
                {
                    if (d.effect_id == l_layoutstatus.effect)
                    {
                        int index = d.direction_index;
                        comboBox_Direction.SelectedIndex = index;
                        break;
                    }
                }
            }
            else
            {
                comboBox_Direction.Items.Clear();
                comboBox_Direction.IsEnabled = false;

            }
            Utility.Log("[Trace]EnableDirection finished");
        }
        private void EnableColorBlock(uint uid)
        {
            layoutStatus _layoutStatus = GetCurrentLayoutStatus();
            //Clear status
            for (uint i = 0; i < 7; i++)
            {
                string controlname = "rec_color_" + i.ToString();
                Rectangle control = (Rectangle)FindName(controlname);
                //control.Fill = null;
                control.Fill = new SolidColorBrush(Color.FromRgb(0x3C, 0x45, 0x50));
                //control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                control.Stroke = null;
                control.StrokeThickness = 1;
                foreach (EffectColor effectColor in _layoutStatus.effectColorList)
                {
                    foreach (ColorBlock colorBlock in effectColor.colorblock)
                    {
                        colorBlock.selected = false;
                        colorBlock.enable = false;
                    }
                }
            }
            rec_color_single.Fill = null;
            string str_disable_url = "..\\Image\\check_box_d.png";
            Uri uri_disable_Source = new Uri(str_disable_url, UriKind.Relative);
            img_color_mode.Source = new BitmapImage(uri_disable_Source);
            img_color_mode.IsEnabled = false;

            rec_color_single.Visibility = Visibility.Hidden;

            //Hard Code color to rainbow mode.....
            if (uid == (uint)effect.Rainbow)
            {
                if( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                {
                    string controlname = "rec_color_0";
                    Rectangle control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00));
                    controlname = "rec_color_1";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x7D, 0x00));
                    controlname = "rec_color_2";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0x00));
                    controlname = "rec_color_3";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x00));
                    controlname = "rec_color_4";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF));
                    controlname = "rec_color_5";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0xFF));
                    controlname = "rec_color_6";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0xFF));
                }
                else if( m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                {
                    string controlname = "rec_color_0";
                    Rectangle control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00));
                    controlname = "rec_color_1";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0x00));
                    controlname = "rec_color_2";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF));
                    controlname = "rec_color_3";
                    control = (Rectangle)FindName(controlname);
                    control.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0xFF));
                }
                return;
            }

            foreach (EffectColor effectColor in _layoutStatus.effectColorList)
            {
                if (uid == effectColor.effect_id)  //find the effect had color block
                {
                    // if radom color is enable, show radom mode or show color blocks.
                    if (effectColor.color_single_enable && (effectColor.color_single_status == status.on))
                    {
                        rec_color_single.Visibility = Visibility.Visible;
                        rec_color_single.Fill = new SolidColorBrush(Color.FromRgb(effectColor.color_single_r,
                            effectColor.color_single_g, effectColor.color_single_b));

                        string str_on_url = "..\\Image\\check_box_on.png";
                        Uri uri_on_Source = new Uri(str_on_url, UriKind.Relative);
                        img_color_mode.Source = new BitmapImage(uri_on_Source);
                        img_color_mode.IsEnabled = true;
                    }
                    else
                    {
                        if (effectColor.color_single_enable && (effectColor.color_single_status == status.off))
                        {
                            string str_off_url = "..\\Image\\check_box_off.png";
                            Uri uri_off_Source = new Uri(str_off_url, UriKind.Relative);
                            img_color_mode.Source = new BitmapImage(uri_off_Source);
                            img_color_mode.IsEnabled = true;
                        }


                        foreach (ColorBlock r in effectColor.colorblock)
                        {
                            int blocks = effectColor.colorblock.Count;
                            string controlname = "rec_color_" + r.sid.ToString();
                            Rectangle control = (Rectangle)FindName(controlname);
                            if (r.sid == 0)
                            {
                                control.Fill = new SolidColorBrush(Color.FromRgb(r.R, r.G, r.B));
                                control.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                                control.StrokeThickness = 3;
                                r.selected = true;
                                r.enable = true;
                            }
                            else if (r.sid < blocks)
                            {
                                control.Fill = new SolidColorBrush(Color.FromRgb(r.R, r.G, r.B));
                                control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                                control.StrokeThickness = 1;
                                r.selected = false;
                                r.enable = true;
                            }
                            else
                            {
                                control.Fill = null;
                                control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                                control.StrokeThickness = 1;
                                r.selected = false;
                                r.enable = false;
                            }

                        }
                    }


                    return;
                }
            }

        }

        private layoutStatus GetCurrentLayoutStatus()
        {
            return (m_mode == (uint)mode.windows) ? m_windows_layoutstatus : m_welcome_layoutstatus;
        }
        private List<ColorBlock> GetCurrentEffectColorBlocks()
        {
            List<ColorBlock> currentEffectColor = null;
            layoutStatus l_layoutStatus = GetCurrentLayoutStatus();
            foreach (EffectColor r in l_layoutStatus.effectColorList)
            {
                if (l_layoutStatus.effect == r.effect_id)
                    currentEffectColor = r.colorblock;
            }
            return currentEffectColor;

        }



        private void PowerSavingOn(bool bUpdateKeepStatus)
        {
            Utility.Log(string.Format("[Trace]PowerSavingOn, update keep status = {0}", bUpdateKeepStatus));
            CreateListenThread();
            CreateHookDelegate();
            string str_img_trigger_url = "..\\Image\\check_box_on.png";
            var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            img_powersaving.Source = new BitmapImage(uriSource);
            if(bUpdateKeepStatus)
            {
                m_isPowerSaving = status.on;
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerSaving", m_isPowerSaving, RegistryValueKind.DWord);
            }
            Utility.Log("[Trace]PowerSavingOn Finished");
        }
        private void PowerSavingOff(bool bUpdateKeepStatus)
        {
            Utility.Log(string.Format("[Trace]PowerSavingOff Finished, update keep status = {0}", bUpdateKeepStatus));
            DeleteHookDelegate();
            DeleteListenThread();
            string str_img_trigger_url = "..\\Image\\check_box_off.png";
            var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            img_powersaving.Source = new BitmapImage(uriSource);
            if(bUpdateKeepStatus)
            {
                m_isPowerSaving = status.off;
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerSaving", m_isPowerSaving, RegistryValueKind.DWord);
            }
            Utility.Log("[Trace]PowerSavingOff Finished");
        }
        private void PowerOn(bool bSyncToRegistry, bool bSyncToWelcome)
        {
            Utility.Log(string.Format("[Trace]PowerOn, sync to registry ={0}, sync to welcom={1}", bSyncToRegistry, bSyncToWelcome));
            comboBox_Profile.IsEnabled = true;
            comboBox_Profile_Option.IsEnabled = true;
            combobox_Windows_Effect.IsEnabled = true;
            combobox_Welcome_Effect.IsEnabled = true;

            string str_on_url = "..\\Image\\btn_on_n.png";
            Uri uri_on_Source = new Uri(str_on_url, UriKind.Relative);
            if (m_mode == (uint)mode.windows)
                img_windows_mode.Source = new BitmapImage(uri_on_Source);
            else
                img_welcome_mode.Source = new BitmapImage(uri_on_Source);
            img_windows_mode.IsEnabled = true;
            img_welcome_mode.IsEnabled = true;

            if ((m_mode == (uint)mode.windows) && (m_windows_layoutstatus.effect == (uint)effect.UserMode))
                //KBTable.EnableKBLayout(true, m_mode);
                KBType.EnableKBLayout(true, m_mode);

            Btn_Clear.IsEnabled = true;
            Img_color_pad.Visibility = Visibility.Visible;
            img_color_mode.Visibility = Visibility.Visible;
            text_color_mode.Visibility = Visibility.Visible;
            //rec_color_red.Visibility = Visibility.Visible;
            //rec_color_orange.Visibility = Visibility.Visible;
            //rec_color_yellow.Visibility = Visibility.Visible;
            //rec_color_green.Visibility = Visibility.Visible;
            //rec_color_blue.Visibility = Visibility.Visible;
            //rec_color_indigo.Visibility = Visibility.Visible;
            //rec_color_violet.Visibility = Visibility.Visible;

            if ((m_mode == (uint)mode.windows) && (m_windows_layoutstatus.effect ==(uint)effect.Music))
            {
                // Brightness and Temp not show ...
                // Hidden Welcome Mode 
                img_welcome_mode.Visibility = Visibility.Hidden;
                text_welcome_mode.Visibility = Visibility.Hidden;
            }
            else
            {
                img_Brightness_ball.Visibility = Visibility.Visible;
                img_Brightness_bar.Visibility = Visibility.Visible;
                img_Brightness_bar_bg.IsEnabled = true;
                img_Tempo_ball.Visibility = Visibility.Visible;
                img_Tempo_bar.Visibility = Visibility.Visible;
                img_Tempo_bar_bg.IsEnabled = true;
            }


            comboBox_Direction.Foreground = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            if(comboBox_Direction.SelectedIndex != -1)
                comboBox_Direction.IsEnabled = true;

            string controlname = "rec_color_single";
            Rectangle control = (Rectangle)FindName(controlname);
            control.Visibility = Visibility.Visible;
            for (uint i = 0; i < 7; i++)
            {
                controlname = "rec_color_" + i.ToString();
                control = (Rectangle)FindName(controlname);
                control.Visibility = Visibility.Visible;
            }
            if (m_isPowerSaving)
                PowerSavingOn(false);         
            img_powersaving.Visibility = Visibility.Visible;
            text_SavingMode.Visibility = Visibility.Visible;
            Btn_Save.IsEnabled = true;
            text_Save.Visibility = Visibility.Visible;

            string str_img_trigger_url = "..\\Image\\check_box_on.png";
            var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            img_power.Source = new BitmapImage(uriSource);

            m_isPowerOn = status.on;
            if(bSyncToRegistry)
            {
                bool Power = m_isPowerOn;
                try
                {
                    Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerStatus", Power, RegistryValueKind.DWord);
                }
                catch(Exception e)
                {
                    Utility.Log(string.Format("[Error]PowerOn sync powerstatus to registry failed, exception {0}", e.ToString()));
                }

            }
            if(bSyncToWelcome)
            {
                SetWelcomePower(status.on);
            }

            if( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                if (m_windows_DLLBuffer.Effect == 0) // Need to assign current layout setting.
                    SetWindowsEffect();
                else
                {
                    Utility.Log("GRGBDLL_SwitchLight start");
                    NativeMethod.GRGBDLL_SwitchLight(status.on, ref m_windows_DLLBuffer);
                    Utility.Log("GRGBDLL_SwitchLight finished");
                }

            }
            else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
            {
                if (m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bEffectType ==0) // Need to assign current layout setting.
                    SetWindowsEffect();
                else
                {
                    Utility.Log("[Trace]GRGBDLL_SwitchLight_4Z start");
                    NativeMethod.GRGBDLL_SwitchLight_4Z(status.on, ref m_windows_FOURZONE_DLLBuffer);
                    Utility.Log("[Trace]GRGBDLL_SwitchLight_4Z finished");
                }

            }
            //Hard code to enable music mode
            if (m_windows_layoutstatus.effect == (uint)effect.Music)
                SetMusicMode(status.on);
            //
            Utility.Log("[Trace]PowerOn Finished");
        }
        private void PowerOff(bool bSyncToRegistry, bool bSyncToWelcome, bool bSendToDFFirmware)
        {
            Utility.Log(string.Format("[Trace]PowerOff, sync to registry ={0}, sync to welcom={1}, send to FW={2}", bSyncToRegistry, bSyncToWelcome, bSendToDFFirmware));
            //Hard code to disbloe music mode
            if (m_windows_layoutstatus.effect == (uint)effect.Music)
                SetMusicMode(status.off);
            //

            comboBox_Profile.IsEnabled = false;
            comboBox_Profile_Option.IsEnabled = false;  
            combobox_Windows_Effect.IsEnabled = false;
            combobox_Welcome_Effect.IsEnabled = false;

            string str_off_url = "..\\Image\\btn_off_n.png";
            Uri uri_off_Source = new Uri(str_off_url, UriKind.Relative);
            if (m_mode == (uint)mode.windows)
                img_windows_mode.Source = new BitmapImage(uri_off_Source);
            else
                img_welcome_mode.Source = new BitmapImage(uri_off_Source);
            img_windows_mode.IsEnabled = false;
            img_welcome_mode.IsEnabled = false;

            //KBTable.EnableKBLayout(false, m_mode);
            KBType.EnableKBLayout(false, m_mode);

            Btn_Clear.IsEnabled = false;
            Img_color_pad.Visibility = Visibility.Hidden;
            img_color_mode.Visibility = Visibility.Hidden;
            text_color_mode.Visibility = Visibility.Hidden;
            //rec_color_red.Visibility = Visibility.Hidden;
            //rec_color_orange.Visibility = Visibility.Hidden;
            //rec_color_yellow.Visibility = Visibility.Hidden;
            //rec_color_green.Visibility = Visibility.Hidden;
            //rec_color_blue.Visibility = Visibility.Hidden;
            //rec_color_indigo.Visibility = Visibility.Hidden;
            //rec_color_violet.Visibility = Visibility.Hidden;

            string controlname = "rec_color_single";
            Rectangle control = (Rectangle)FindName(controlname);
            control.Visibility = Visibility.Hidden;
            for (uint i = 0; i < 7; i++)
            {
                controlname = "rec_color_" + i.ToString();
                control = (Rectangle)FindName(controlname);
                control.Visibility = Visibility.Hidden;
            }

            img_Brightness_ball.Visibility = Visibility.Hidden;
            img_Brightness_bar.Visibility = Visibility.Hidden;
            img_Brightness_bar_bg.IsEnabled = false;
            img_Tempo_ball.Visibility = Visibility.Hidden;
            img_Tempo_bar.Visibility = Visibility.Hidden;
            img_Tempo_bar_bg.IsEnabled = false;
            comboBox_Direction.Foreground = new SolidColorBrush(Color.FromRgb(0x14, 0x1D, 0x24));
            comboBox_Direction.IsEnabled = false;

            if (m_isPowerSaving)
                PowerSavingOff(false);
            img_powersaving.Visibility = Visibility.Hidden;
            text_SavingMode.Visibility = Visibility.Hidden;
            Btn_Save.IsEnabled = false;
            text_Save.Visibility = Visibility.Hidden;

            if ((m_mode == (uint)mode.windows) && (m_windows_layoutstatus.effect == (uint)effect.Music))
            {
                // Show Welcome Mode 
                img_welcome_mode.Visibility = Visibility.Visible;
                text_welcome_mode.Visibility = Visibility.Visible;
            }

            string str_img_trigger_url = "..\\Image\\check_box_off.png";
            var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            img_power.Source = new BitmapImage(uriSource);
            m_isPowerOn = status.off;

            if(bSyncToRegistry)
            {
                bool Power = m_isPowerOn;
                try
                {
                    Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "PowerStatus", Power, RegistryValueKind.DWord);
                }
                catch(Exception e)
                {
                    Utility.Log(string.Format("[Error]Write PowerStatus to registry failed, exception = {0}", e.ToString()));
                }

            }
            if (bSyncToWelcome)
            {
                SetWelcomePower(status.off);
            }
            if(bSendToDFFirmware)
            {
                if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                {
                    DLLBuffer bufNULL = new DLLBuffer();
                    Utility.Log("[Trace]GRGBDLL_SwitchLight start");
                    NativeMethod.GRGBDLL_SwitchLight(status.off, ref bufNULL);
                    Utility.Log("[Trace]GRGBDLL_SwitchLight end");

                }
                else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                {
                    FOURZONE_DLLBUFFER bufNULL = new FOURZONE_DLLBUFFER();
                    Utility.Log("[Trace]GRGBDLL_SwitchLight_4Z start");
                    NativeMethod.GRGBDLL_SwitchLight_4Z(status.off, ref bufNULL);
                    Utility.Log("[Trace]GRGBDLL_SwitchLight_4Z end");
                }

            }
            Utility.Log("[Trace]PowerOff Finished");
        }

        public MainWindow()
        {
            Utility.EnableLog();
            CheckRGBDevice();
            Utility.Log("[Initialize]CheckRGBDevice Finished");
            InitializeParameter();
            Utility.Log("[Initialize]InitializeParameter Finished");
            InitializeComponent();
            Utility.Log("[Initialize]InitializeComponent Finished");
            this.SourceInitialized += new EventHandler(MainWindow_SourceInitialized);
            

            // SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            SetLanguageDictionary();
            Utility.Log("[Initialize]SetLanguageDictionary Finished");
            InitializeLayout();
            Utility.Log("[Initialize]InitializeLayout Finished");

        }

        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            Hook_Event.Set();
            return NativeMethod.CallNextHookEx(s_MouseHookHandle, nCode, wParam, lParam);
        }

        private static int KeyboardProc(int nCode, int wParam, IntPtr lParam)
        {
            Hook_Event.Set();
            return NativeMethod.CallNextHookEx(s_KeyboardHookHandle, nCode, wParam, lParam);
        }

        private static void CreateHookDelegate()
        {
            Utility.Log("[Trace]CreateHookDelegate start");
            if (s_MouseHookHandle == 0)
            {
                s_MouseDelegate = MouseHookProc;
                //install hook
                //s_MouseHookHandle = SetWindowsHookEx( WH_MOUSE_LL, s_MouseDelegate, Marshal.GetHINSTANCE(           Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                s_MouseHookHandle = NativeMethod.SetWindowsHookEx(NativeMethod.WH_MOUSE_LL, s_MouseDelegate, IntPtr.Zero, 0);
                if (s_MouseHookHandle == 0)
                {
                    Console.WriteLine("SetWindowsHookEx Failed {0}", Marshal.GetLastWin32Error());
                    Utility.Log(String.Format("[Error]SetWindowsHookEx Failed {0}", Marshal.GetLastWin32Error()));
                }
            }
            if (s_KeyboardHookHandle == 0)
            {
                s_KeyBoardDelegate = KeyboardProc;
                //install hook
                //s_MouseHookHandle = SetWindowsHookEx( WH_MOUSE_LL, s_MouseDelegate, Marshal.GetHINSTANCE(           Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                s_KeyboardHookHandle = NativeMethod.SetWindowsHookEx(NativeMethod.WH_KEYBOARD_LL, s_KeyBoardDelegate, IntPtr.Zero, 0);
                if (s_KeyboardHookHandle == 0)
                {
                    Console.WriteLine("SetWindowsHookEx Failed {0}", Marshal.GetLastWin32Error());
                    Utility.Log(String.Format("[Error]SetWindowsHookEx Failed {0}", Marshal.GetLastWin32Error()));
                }
            }
            Utility.Log("[Trace]CreateHookDelegate finished");
        }
        private static void DeleteHookDelegate()
        {
            Utility.Log("[Trace]DeleteHookDelegate start");
            if (s_MouseHookHandle != 0)
            {
                //uninstall hook
                int result = NativeMethod.UnhookWindowsHookEx(s_MouseHookHandle);
                //reset invalid handle
                s_MouseHookHandle = 0;
                //Free up for GC
                s_MouseDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    Console.WriteLine("UnhookWindowsHookEx failed {0}", Marshal.GetLastWin32Error());
                    Utility.Log(String.Format("[Error]UnhookWindowsHookEx failed = {0}", Marshal.GetLastWin32Error()));
                }
            }

            if (s_KeyboardHookHandle != 0)
            {
                //uninstall hook
                int result = NativeMethod.UnhookWindowsHookEx(s_KeyboardHookHandle);
                //reset invalid handle
                s_KeyboardHookHandle = 0;
                //Free up for GC
                s_KeyBoardDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    Console.WriteLine("UnhookWindowsHookEx failed", Marshal.GetLastWin32Error());
                    Utility.Log(String.Format("[Error]UnhookWindowsHookEx failed = {0}", Marshal.GetLastWin32Error()));
                }
            }
            Utility.Log("[Trace]DeleteHookDelegate finished");
        }

        private static void HookThreadProc()
        {
            while (!HookThread_Exit)
            {
                bool bsignal = Hook_Event.WaitOne(PowerSaving.CloseTiming);  // 10 min
                if (bsignal)
                {
                    if (HookThread_Exit)
                        break;
                    else
                    {
                        if(m_bHookLedOnOff == false)
                        {
                            Console.WriteLine("Light on becasue IO active");
                            if( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                            {
                                Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight on");
                                NativeMethod.GRGBDLL_SwitchLight(status.on, ref m_windows_DLLBuffer);
                                Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight on finished");
                            }
                            else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB )
                            {
                                Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight_4Z on");
                                NativeMethod.GRGBDLL_SwitchLight_4Z(status.on, ref m_windows_FOURZONE_DLLBuffer);
                                Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight_4Z on finished");
                            }

                            m_bHookLedOnOff = true;

                            //Hard code to enable music mode
                            if (m_windows_layoutstatus.effect == (uint)effect.Music)
                                m_delStartMonitorAudio(0);
                            //
                        }
                    }
                }
                else // timeout
                {
                    if ((m_isPowerSaving) && ( m_bHookLedOnOff == true))
                    {
                        //Hard code to disbloe music mode
                        if (m_windows_layoutstatus.effect == (uint)effect.Music)
                            m_delStopMonitorAudio();
                        //
                        Console.WriteLine("Light off becasue IO not active under timer");
                        Utility.Log("[Trace]HookThreadProc Backlight off becasue IO not active under timer");
                        if ( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                        {
                            DLLBuffer bufNULL = new DLLBuffer();
                            Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight off");
                            NativeMethod.GRGBDLL_SwitchLight(status.off, ref bufNULL);
                            Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight off finished");
                        }
                        else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                        {
                            FOURZONE_DLLBUFFER bufNULL = new FOURZONE_DLLBUFFER();
                            Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight_4Z off");
                            NativeMethod.GRGBDLL_SwitchLight_4Z(status.off, ref bufNULL);
                            Utility.Log("[Trace]HookThreadProc GRGBDLL_SwitchLight_4Z finished");
                        }
                        m_bHookLedOnOff = false;
                    }
                }

            }
            HookThread = null;
        }

        private static void CreateListenThread()
        {
            Utility.Log("[Trace]CreateListenThread start");
            if (HookThread == null)
            {
                HookThread_Exit = false;
                HookThread = new Thread(HookThreadProc);
                HookThread.Start();
            }
            Utility.Log("[Trace]CreateListenThread end");
        }
        private static void DeleteListenThread()
        {
            Utility.Log("[Trace]DeleteListenThread start");
            HookThread_Exit = true;
            Hook_Event.Set();
            Utility.Log("[Trace]DeleteListenThread end");
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            //Resume
            //Poewr Battery
            //Load user setting from registry

            if (e.Mode == PowerModes.Resume)
            {
                Console.WriteLine("Resume");

            }
            else if(e.Mode == PowerModes.Suspend)
            {
                Console.WriteLine("Suspend");
            }
        }
        private void WMIHandleEvent(object sender, EventArrivedEventArgs e)
        {
            try
            {
                int scancode = Convert.ToInt32(((ManagementBaseObject)e.NewEvent).SystemProperties["ULong"].Value.ToString());
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    switch (scancode)
                    {
                        case ECSpec.OSD_RGBKBBKL_LEVEL_UPDATE:
                            {
                                Utility.Log("[Trace]WMIHandleEvent OSD_RGBKBBKL_LEVEL_UPDATE start");
                                byte DFLightLevel = NativeMethod.GRGBDLL_GetBacklight();
                                Utility.Log("[Trace]WMIHandleEvent OSD_RGBKBBKL_LEVEL_UPDATE end");
                                byte level = 0;
                                if (DFLightLevel > 0x24)
                                {
                                    level = 4;
                                }
                                else if ((DFLightLevel > 0x16) && (DFLightLevel <= 0x24))
                                {
                                    level = 3;
                                }
                                else if ((DFLightLevel > 0x08) && (DFLightLevel <= 0x16))
                                {
                                    level = 2;
                                }
                                else if ((DFLightLevel > 0x00) && (DFLightLevel <= 0x08))
                                {
                                    level = 1;
                                }
                                else
                                {
                                    level = 0;
                                }
                                if (m_mode == (uint)mode.windows)
                                    SetBrightnessLayout(level, false);
                                m_windows_layoutstatus.brightness_level = level;

                                if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                                    m_windows_DLLBuffer.Brightnesslevel = TranslateDarfonLightlevel();
                                else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                                    m_windows_FOURZONE_DLLBuffer._TSLedType_08H.blight = TranslateDarfonLightlevel();

                                if (m_isPowerOn == status.off)
                                {
                                    PowerOn(true, true);
                                }
                                Utility.Log("[Trace]WMIHandleEvent OSD_RGBKBBKL_LEVEL_UPDATE finshed");
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Utility.Log(string.Format("[Error]WMIHandleEvent get exception = {0}",ex.ToString()));
            }

        }
        /*
        private void SoundDeviceInsertedAndRemoveEvent(object sender, EventArrivedEventArgs e)
        {
            if (m_windows_DLLBuffer.Effect == 0x24)
            {
                m_delStopMonitorAudio();
                Thread.Sleep(1000);
                if(m_windows_DLLBuffer.Effect == 0x24)  // Check again to avoid UI changed.
                {
                    m_delStopMonitorAudio();
                    m_delStartMonitorAudio(0);
                }
            }

            Console.WriteLine("DeviceInsertedAndRemoveEvent");
        }
        */
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == APMessage.WM_MSG_MYCOLOR2)  //Brightness Up
            {
                //
            }
            else if(msg == APMessage.WM_MSG_MYAPP)
            {
                Utility.Log("[Trace]WndProc WM_MSG_MYAPP");
                int define = (int)wParam;
                if (define == APMessage.MSG_MYCOLOR2_AP_SHOW)
                {
                    this.Topmost = true;
                    this.Visibility = Visibility.Visible;
                    this.WindowState = WindowState.Normal;
                    this.ShowInTaskbar = true;
                    this.Topmost = false;

                }
                Utility.Log("[Trace]WndProc WM_MSG_MYAPP finished");
            }
            else if(msg == APMessage.WM_MSG_OOBE)
            {
                Utility.Log("[Trace]WndProc WM_MSG_OOBE");
                int define = (int)wParam;
                if (define == APMessage.MSG_MYCOLOR2_AP_TIPSHOW)
                {
                    Tips.UpdateSteps(0);
                    Tips.Visibility = Visibility.Visible;
                    this.Topmost = true;
                    this.Visibility = Visibility.Visible;
                    this.WindowState = WindowState.Normal;
                    this.ShowInTaskbar = true;
                    this.Topmost = false;
                }
                else if (define == APMessage.MSG_MYCOLOR2_AP_ENABLE_BREATHING)
                {
                    DLLBuffer tip_buffer = new DLLBuffer();
                    tip_buffer.singleRGB = new byte[4];
                    tip_buffer.userRGB = new NativeKBRGB[16];
                    tip_buffer.KBRGB = new NativeKBRGB[126];
                    tip_buffer.bkeyCount = 126; // reserved

                    tip_buffer.Mode = 0; // Hans's spec,  0= windows, 1 = Welcome
                    tip_buffer.Effect = 0x20;  //Static Mode
                    tip_buffer.Brightnesslevel = 0x32;
                    tip_buffer.Tempolevel = 0x05;
                    tip_buffer.Direction = 0;
                    tip_buffer.NVsaving = false;
                    //TranslateDarfonWindowsRGB();
                    //m_DLLBuffer.user_sec = TranslateDarfonSeconds();

                    //Single Color Blocks  DA70D6 = Purple colr
                    tip_buffer.userRGB[1].ID = 1;
                    tip_buffer.userRGB[1].R = 0xDA;
                    tip_buffer.userRGB[1].G = 0x70;
                    tip_buffer.userRGB[1].B = 0xD6;

                    tip_buffer.bSingleDisplay = true;
                    Utility.Log("[Trace]WndProc WM_MSG_OOBE GRGBDLL_CaptureCMDBuffer start");
                    NativeMethod.GRGBDLL_CaptureCMDBuffer(ref tip_buffer);
                    Utility.Log("[Trace]WndProc WM_MSG_OOBE GRGBDLL_CaptureCMDBuffer finished");
                }
                else if (define == APMessage.MSG_MYCOLOR2_AP_DISABLE_BREATHING)
                {
                    SetWindowsEffect();
                }
                Utility.Log("[Trace]WndProc WM_MSG_OOBE finished");

            }

            else if (msg == 0x218) //WM_POWERBROADCAST
            {
                //Console.WriteLine("WM_POWERBROADCAST");
                if ((int)wParam == 0x8013) //PBT_POWERSETTINGCHANGE
                {
                    try
                    {
                        //Console.WriteLine("PBT_POWERSETTINGCHANGE");
                        Utility.Log("[Trace]WndProc power setting changed");
                        NativeMethod.POWERBROADCAST_SETTING ps = (NativeMethod.POWERBROADCAST_SETTING)
                            Marshal.PtrToStructure(lParam, typeof(NativeMethod.POWERBROADCAST_SETTING));
                        //if (ps.PowerSetting == GUID_MONITOR_POWER_ON )
                        if (ps.PowerSetting == NativeMethod.GUID_CONSOLE_DISPLAY_STATE)
                        {
                            Utility.Log("[Trace]WndProc console disaply  state updated");
                            //0x0 - The monitor is off.
                            //0x1 - The monitor is on.
                            //0x2 - The display is dimmed.
                            //Console.WriteLine("Power = {0}", ps.Data);
                            if ((ps.Data == 0x00))
                            {
                                Utility.Log("[Trace]WndProc Monitor Off");

                                //Hard code to disbloe music mode
                                if (m_windows_layoutstatus.effect == (uint)effect.Music)
                                    m_delStopMonitorAudio();
                                //

                                if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                                {
                                    DLLBuffer bufNULL = new DLLBuffer();
                                    Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight strart");
                                    NativeMethod.GRGBDLL_SwitchLight(status.off, ref bufNULL);
                                    Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight end");
                                }
                                else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                                {
                                    FOURZONE_DLLBUFFER bufNULL = new FOURZONE_DLLBUFFER();
                                    Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight_4Z start");
                                    NativeMethod.GRGBDLL_SwitchLight_4Z(status.off, ref bufNULL);
                                    Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight_4Z end");
                                }
                                //  }
                            }
                            else if (ps.Data == 0x001) // Enable LED anyway to avoid status incorrect
                            {
                                Utility.Log("[Trace]WndProc Monitor On");
                                if (m_isPowerOn == status.on)
                                {
                                    Console.WriteLine("Light on becasue open panel");
                                    if (bWKD_Panel_OnOffEvent_Initialized)
                                    {
                                        if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                                        {
                                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight start");
                                            NativeMethod.GRGBDLL_SwitchLight(status.on, ref m_windows_DLLBuffer);
                                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight end");
                                        }
                                        else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                                        {
                                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight_4Z start");
                                            NativeMethod.GRGBDLL_SwitchLight_4Z(status.on, ref m_windows_FOURZONE_DLLBuffer);
                                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight_4Z end");
                                        }
                                        //Hard code to enable music mode
                                        if (m_windows_layoutstatus.effect == (uint)effect.Music)
                                            m_delStartMonitorAudio(0);
                                        //
                                    }
                                }
                                //else{do nothing}  // Power off

                                bWKD_Panel_OnOffEvent_Initialized = true;

                            }
                        }
                    }
                    catch(Exception e)
                    {
                        if( !bWKD_Panel_OnOffEvent_Initialized)
                            bWKD_Panel_OnOffEvent_Initialized = true;
                        Utility.Log(String.Format("[Error]WndProc power setting changed process failed {0}",e.ToString()));
                    }

                }
                else if((int)wParam == 0x04) //PBT_APMSUSPEND
                {
                    Utility.Log("[Trace]WndProc Suspend");

                    //Hard code to disbloe music mode
                    if (m_windows_layoutstatus.effect == (uint)effect.Music)
                        m_delStopMonitorAudio();
                    //

                    if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                    {
                        DLLBuffer bufNULL = new DLLBuffer();
                        Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight start");
                        NativeMethod.GRGBDLL_SwitchLight(status.off, ref bufNULL);
                        Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight end");
                    }
                    else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                    {
                        FOURZONE_DLLBUFFER bufNULL = new FOURZONE_DLLBUFFER();
                        Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight_4Z start");
                        NativeMethod.GRGBDLL_SwitchLight_4Z(status.off, ref bufNULL);
                        Utility.Log("[Trace]RGB backlight off becasue Panel off GRGBDLL_SwitchLight_4Z end");
                    }
                }
                else if((int)wParam == 0x12)//PBT_APMRESUMEAUTOMATIC 
                {
                    Utility.Log("[Trace]WndProc Resume");
                    if (m_isPowerOn == status.on)
                    {
                        if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                        {
                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight start");
                            NativeMethod.GRGBDLL_SwitchLight(status.on, ref m_windows_DLLBuffer);
                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight end");
                        }
                        else if (m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                        {
                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight_4Z start ");
                            NativeMethod.GRGBDLL_SwitchLight_4Z(status.on, ref m_windows_FOURZONE_DLLBuffer);
                            Utility.Log("[Trace]RGB backlight on becasue Panel on GRGBDLL_SwitchLight_4Z end");
                        }
                        //Hard code to enable music mode
                        if (m_windows_layoutstatus.effect == (uint)effect.Music)
                            m_delStartMonitorAudio(0);
                        //
                    }
                }
            }
            return IntPtr.Zero;
            //throw new NotImplementedException();
        }
        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
//#if DEMO
            // this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = true;
//#else
            //this.Visibility = Visibility.Hidden;
//#endif
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            m_hwnd = new WindowInteropHelper(this).Handle;
            m_hPowerNotify = NativeMethod.RegisterPowerSettingNotification(m_hwnd, ref NativeMethod.GUID_CONSOLE_DISPLAY_STATE, NativeMethod.DEVICE_NOTIFY_WINDOW_HANDLE);
            if(m_hPowerNotify == IntPtr.Zero)
            {
                Utility.Log("[Error]RegisterPowerSettingNotification failed, handle is null");
            }
            WMIEC.StartWMIReceiveEvent(WMIHandleEvent);
            Utility.Log("[Initialize]StartWMIReceiveEvent finished");
            /*
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_SoundDevice'");
            m_Sound_InsertWatcher = new ManagementEventWatcher(insertQuery);
            m_Sound_InsertWatcher.EventArrived += new EventArrivedEventHandler(SoundDeviceInsertedAndRemoveEvent);
            m_Sound_InsertWatcher.Start();
            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_SoundDevice'");
            m_Sound_DeleteWatcher = new ManagementEventWatcher(removeQuery);
            m_Sound_DeleteWatcher.EventArrived += new EventArrivedEventHandler(SoundDeviceInsertedAndRemoveEvent);
            m_Sound_DeleteWatcher.Start();
            */
            //MyAPPShell.Connect();


            //show for the first time the keyboard layout settings
            if (Convert.ToInt32(Utility.RegistryKeyRead(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "First_start", "1")) == 1)
            {
                setting.Show();
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "First_start", "0", RegistryValueKind.DWord);
            }
            Utility.Log("[Initialize]MainWindow_Loaded finished");
        }
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(KBType.KBTI_KBlayout_enable && e.ChangedButton == MouseButton.Left)
            {
                KBType.SetKeyForcusedClear();
            }
            /*if (KBTable.m_KBlayout_enable && e.ChangedButton == MouseButton.Left)
            {
                KBTable.SetKeyForcusedClear();
            }*/

            Point p = e.GetPosition(this);
            if(p.Y < line_upper.Y1)
                this.DragMove();
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NativeMethod.UnregisterPowerSettingNotification(m_hPowerNotify);
            DeleteHookDelegate();
            DeleteListenThread();
            //m_delStopMonitorAudio();
            //NativeMethod.GRGBDLL_DestroyDLL();
            WMIEC.EndWMIRecieveEvent();
            //MyAPPShell.Disconnect();
            Utility.DisableLog();
            Process.Start("Rev.Center2.exe");
            Environment.Exit(0);
        }
        private void Btn_Tips_Click(object sender, RoutedEventArgs e)
        {
            // Tips tips = new Tips();
            // tips.Left = this.Left;
            // tips.Top = this.Top;
            // tips.ShowDialog();
            Tips.UpdateSteps(0);
            Tips.Visibility = Visibility.Visible;

        }
        private void Btn_Small_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }





        private void CancelKeyEvent(object sender, KeyEventArgs e)
        {
            if (comboBox_Direction.IsEnabled == false)
                return;

            ComboBoxItem item = (ComboBoxItem)comboBox_Direction.SelectedItem;
            byte direction_uid = 0;
            try
            {
                direction_uid = Convert.ToByte(item.Uid);
            }
            catch
            {
                direction_uid = 0x00;
            }

            if (direction_uid == (uint)id_direction.OnKeyPressed)
                e.Handled = true;


        }
        private void ComboBox_Profile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.Log("[Trace]ComboBox_Profile_SelectionChanged start ");
            if (comboBox_Profile.SelectedIndex == -1)  // not selected
                return;

            StartSendLayoutData((uint)type_SendLayoutFlag.Profile);
            try
            {
                m_profile = comboBox_Profile.SelectedIndex;
                //Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "Profile", m_profile, RegistryValueKind.DWord);
                LoadProfileStatus();

                //****Triger Effect to update layout here*****
                /*
                string controlname = "ComboBoxItem_Windows_Effect_" + m_windows_layoutstatus.effect.ToString();
                ComboBoxItem control = (ComboBoxItem)FindName(controlname);
                if (combobox_Windows_Effect.SelectedItem == control)
                {
                    ComboBox_Windows_Effect_SelectionChanged(null, null);
                }
                else
                    combobox_Windows_Effect.SelectedItem = control;
                */
                string str_on_url = "..\\Image\\btn_on_n.png";
                Uri uri_on_Source = new Uri(str_on_url, UriKind.Relative);
                string str_off_url = "..\\Image\\btn_off_n.png";
                Uri uri_off_Source = new Uri(str_off_url, UriKind.Relative);
                m_mode = (uint)mode.windows;
                img_windows_mode.Source = new BitmapImage(uri_on_Source);
                img_welcome_mode.Source = new BitmapImage(uri_off_Source);
                text_Welcome_Effect.Visibility = Visibility.Hidden;
                text_Windows_Effect.Visibility = Visibility.Visible;
                combobox_Welcome_Effect.Visibility = Visibility.Hidden;
           //     combobox_Welcome_Effect.SelectedIndex = -1;

                uint index = (uint)m_windows_layoutstatus.effect;
                string controlname = "ComboBoxItem_Windows_Effect_" + index.ToString();
                ComboBoxItem control = (ComboBoxItem)FindName(controlname);
                Utility.Log("[Trace-Dummy]Trigger combobox update that differnet profile but have same window combobox ");
                combobox_Windows_Effect.SelectedIndex = -1; //Trigger combobox update(differnet profile but have same window combobox)
                combobox_Windows_Effect.SelectedItem = control;
                Utility.Log("[Trace-Dummy]Set Combox_windows_effect visibility = visible ");
                combobox_Windows_Effect.Visibility = Visibility.Visible;

                SetBrightnessLayout(m_windows_layoutstatus.brightness_level,true);
                SetTempoLayout(m_windows_layoutstatus.tempo_level, true);
            }
            catch(Exception ex)
            {
                //Recovery
                Utility.Log(String.Format("[Error]ComboBox_Profile_SelectionChanged failed = {0} ",ex.ToString()));
            }
            EndSendLayoutData((uint)type_SendLayoutFlag.Profile, true);
            Utility.Log("[Trace]ComboBox_Profile_SelectionChanged end ");
        }
        private void comboBox_Profile_Option_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Profile_Option.SelectedIndex == -1)  // not selected
                return;
            else if (comboBox_Profile_Option.SelectedIndex == (uint)profiles_option.Import)
            {
                try
                {
                    MyAPPShell.Connect();
                    string FileName = MyAPPShell.GetStream("Import");
                    MyAPPShell.Disconnect();
                    if (FileName != "")
                    {
                        string arg = "/s " + FileName;
                        Process regeditProcess = Process.Start("regedit.exe", arg);
                        regeditProcess.WaitForExit();
                    }
                    //Flash to import setting
                    comboBox_Profile.SelectedIndex = -1;
                    comboBox_Profile.SelectedIndex = 0;  //Proflie 0
                }
                catch (Exception ex)
                {
                    Utility.Log(String.Format("[Error]comboBox_Profile_Option_SelectionChanged import profile failed {0} ", ex.ToString()));
                }
            }
            else if (comboBox_Profile_Option.SelectedIndex == (uint)profiles_option.Export)
            {
                try
                {
                    MyAPPShell.Connect();
                    string FileName = MyAPPShell.GetStream("Export");
                    MyAPPShell.Disconnect();
                    if (FileName != "")
                    {
                        string arg = @"/e " + FileName + @" HKEY_LOCAL_MACHINE\SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard";
                        Process regeditProcess = Process.Start("regedit.exe", arg);
                        regeditProcess.WaitForExit();

                    }
                }
                catch (Exception ex)
                {
                    Utility.Log(String.Format("[Error]comboBox_Profile_Option_SelectionChanged export profile failed {0} ", ex.ToString()));
                }
            }
            comboBox_Profile_Option.SelectedIndex = -1;  //flash to trigger next selection.
        }

        private void img_mode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Utility.Log("[Trace]img_mode_MouseDown start ");
            Image img = (Image)sender;
            string str_on_url = "..\\Image\\btn_on_n.png";
            Uri uri_on_Source = new Uri(str_on_url, UriKind.Relative);
            string str_off_url = "..\\Image\\btn_off_n.png";
            Uri uri_off_Source = new Uri(str_off_url, UriKind.Relative);

            if (img.Name == "img_windows_mode")
            {
                m_mode = (uint)mode.windows;
                img_windows_mode.Source = new BitmapImage(uri_on_Source);
                img_welcome_mode.Source = new BitmapImage(uri_off_Source);
                text_Welcome_Effect.Visibility = Visibility.Hidden;
                text_Windows_Effect.Visibility = Visibility.Visible;
                combobox_Welcome_Effect.Visibility = Visibility.Hidden;
                combobox_Windows_Effect.SelectedIndex = -1;
                uint index = (uint)m_windows_layoutstatus.effect;
                string controlname = "ComboBoxItem_Windows_Effect_" + index.ToString();
                ComboBoxItem control = (ComboBoxItem)FindName(controlname);
                combobox_Windows_Effect.SelectedItem = control;
                combobox_Windows_Effect.Visibility = Visibility.Visible;
                SetBrightnessLayout(m_windows_layoutstatus.brightness_level,false);
                SetTempoLayout(m_windows_layoutstatus.tempo_level, false);
            }
            else
            {
                m_mode = (uint)mode.welcome;
                img_windows_mode.Source = new BitmapImage(uri_off_Source);
                img_welcome_mode.Source = new BitmapImage(uri_on_Source);
                text_Windows_Effect.Visibility = Visibility.Hidden;
                text_Welcome_Effect.Visibility = Visibility.Visible;
                combobox_Windows_Effect.Visibility = Visibility.Hidden;
                combobox_Welcome_Effect.SelectedIndex = -1;
                uint index = (uint)m_welcome_layoutstatus.effect;
                string controlname = "ComboBoxItem_Welcome_Effect_" + index.ToString();
                ComboBoxItem control = (ComboBoxItem)FindName(controlname);
                combobox_Welcome_Effect.SelectedItem = control;
                combobox_Welcome_Effect.Visibility = Visibility.Visible;

                //**********************************************************                
                //Workaround to sync Music Mode to show Brightness and Tempo
                img_Brightness_ball.Visibility = Visibility.Visible;
                img_Brightness_bar.Visibility = Visibility.Visible;
                img_Brightness_bar_bg.IsEnabled = true;
                img_Tempo_ball.Visibility = Visibility.Visible;
                img_Tempo_bar.Visibility = Visibility.Visible;
                img_Tempo_bar_bg.IsEnabled = true;
                //******************************************************
                //


                SetBrightnessLayout(m_welcome_layoutstatus.brightness_level, false);
                SetTempoLayout(m_welcome_layoutstatus.tempo_level, false);
            }
            e.Handled = true;
            Utility.Log("[Trace]img_mode_MouseDown end ");
        }
        private void ComboBox_Windows_Effect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.Log("[Trace]ComboBox_Windows_Effect_SelectionChanged start ");
            if (combobox_Windows_Effect.SelectedIndex == -1)  // not selected
            {
                Utility.Log("[Trace]ComboBox_Windows_Effect_SelectionChanged index = -1 and return ");
                return;
            }
            ComboBoxItem item = (ComboBoxItem)combobox_Windows_Effect.SelectedItem;
            if (item == null)
            {
                Utility.Log("[Trace]ComboBox_Windows_Effect_SelectionChanged item is null and return ");
                return;
            }
            uint uid = 0;
            try
            {
                uid = Convert.ToUInt32(item.Uid);
                m_windows_layoutstatus.effect = uid;
            }
            catch
            {
                // error log.
                Utility.Log("[Error]ComboBox_Windows_Effect_SelectionChanged conver item uid failed");
                return;
            }
            //hard code to hidden welecome mode under Music effect
            if(uid == (uint)effect.Music)
            {
                img_welcome_mode.Visibility = Visibility.Hidden;
                text_welcome_mode.Visibility = Visibility.Hidden;
            }
            else
            {
                img_welcome_mode.Visibility = Visibility.Visible;
                text_welcome_mode.Visibility = Visibility.Visible;
            }
            // 
            Utility.Log(String.Format("[Trace]ComboBox_Windows_Effect_SelectionChanged uid  ={0}",uid));
            StartSendLayoutData((uint)type_SendLayoutFlag.Effect);
            switch (uid)
            {
                case (uint)effect.Static:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    if( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                        EnableDirection(status.enable, (uint)type_direction.direction);
                    else  //FOUR ZONE
                        EnableDirection(status.disable, (uint)type_direction.NA);
                    KBType.EnableKBLayout(false, m_mode);
                    //KBTable.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Breathing:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Wave:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.enable, (uint)type_direction.direction);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Ripple:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.enable, (uint)type_direction.onkeypress);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Reactive:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.enable, (uint)type_direction.onkeypress);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Rainbow:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Marquee:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Raindrop:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Stacker:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Impact:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Aurora:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.enable, (uint)type_direction.onkeypress);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Spark:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.enable, (uint)type_direction.onkeypress);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Music:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.UserMode:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(true, m_mode);
                    KBType.EnableKBLayout(true, m_mode);
                    //KBTable101.EnableKBLayout(true, m_mode);
                    break;
                case (uint)effect.Neon:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Flash:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.enable, (uint)type_direction.direction_sync);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Mix:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                default:
                    break;

            }
            EndSendLayoutData((uint)type_SendLayoutFlag.Effect, true);
            Utility.Log("[Trace]ComboBox_Windows_Effect_SelectionChanged end");
        }
        private void ComboBox_Welcome_Effect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.Log("[Trace]ComboBox_Welcome_Effect_SelectionChanged start");
            if (combobox_Welcome_Effect.SelectedIndex == -1)  // not selected
            {
                Utility.Log("[Trace]ComboBox_Welcome_Effect_SelectionChanged index = -1 and return ");
                return;
            }
            ComboBoxItem item = (ComboBoxItem)combobox_Welcome_Effect.SelectedItem;
            if (item == null)
            {
                Utility.Log("[Trace]ComboBox_Welcome_Effect_SelectionChanged item is null and return ");
                return;
            }
            uint uid = 0;
            try
            {
                uid = Convert.ToUInt32(item.Uid);
                m_welcome_layoutstatus.effect = uid;
            }
            catch
            {
                // error log.
                Utility.Log("[Error]ComboBox_Welcome_Effect_SelectionChanged convert uid failed");
                return;
            }
            StartSendLayoutData((uint)type_SendLayoutFlag.Effect);
            Utility.Log(String.Format("[Trace]ComboBox_Welcome_Effect_SelectionChanged uid =  {0}", uid));
            switch (uid)
            {
                case (uint)effect.Static:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB )
                        EnableDirection(status.enable, (uint)type_direction.NA);
                    else
                        EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Breathing:
                    EnableColorBlock(uid);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Wave:
                    EnableColorBlock(uid);
                    EnableDirection(status.enable, (uint)type_direction.direction);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Rainbow:
                    EnableColorBlock(uid);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Marquee:
                    EnableColorBlock(uid);
                    //EnableTrigger(status.disable);
                    //EnableSecEditor(false);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Raindrop:
                    EnableColorBlock(uid);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Flash:
                    EnableColorBlock(uid);
                    EnableDirection(status.enable, (uint)type_direction.direction_sync);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.Mix:
                    EnableColorBlock(uid);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(false, m_mode);
                    KBType.EnableKBLayout(false, m_mode);
                    break;
                case (uint)effect.UserMode:
                    EnableColorBlock(uid);
                    EnableDirection(status.disable, (uint)type_direction.NA);
                    //KBTable.EnableKBLayout(true, m_mode);
                    KBType.EnableKBLayout(true, m_mode);
                    break;
                default:
                    break;

            }
            EndSendLayoutData((uint)type_SendLayoutFlag.Effect, true);
            Utility.Log("[Trace]ComboBox_Welcome_Effect_SelectionChanged end");
        }
        private void Img_trigger_auto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Img_trigger_auto.IsEnabled == status.disable)
                return;

            m_trigger = (uint)tirgger.auto;

            string str_img_trigger_url = "..\\Image\\btn_on_n.png";
            var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            Img_trigger_auto.Source = new BitmapImage(uriSource);
            str_img_trigger_url = "..\\Image\\btn_off_n.png";
            uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            Img_trigger_onkeypressed.Source = new BitmapImage(uriSource);
        }
        private void Img_trigger_onkeypressed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Img_trigger_onkeypressed.IsEnabled == status.disable)
                return;

            m_trigger = (uint)tirgger.onkeypressed;

            string str_img_trigger_url = "..\\Image\\btn_off_n.png";
            var uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            Img_trigger_auto.Source = new BitmapImage(uriSource);
            str_img_trigger_url = "..\\Image\\btn_on_n.png";
            uriSource = new Uri(str_img_trigger_url, UriKind.Relative);
            Img_trigger_onkeypressed.Source = new BitmapImage(uriSource);
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            StartSendLayoutData((uint)type_SendLayoutFlag.ColorClear);

            //if (KBTable.m_KBlayout_enable)
            //{
            //    KBTable.SetKeyBoardColor(m_mode,0x00, 0x00, 0x00);
            //}
            if (KBType.KBTI_KBlayout_enable)
            {
                KBType.SetKeyBoardColor(m_mode, 0x00, 0x00, 0x00);
            }
            else
            {

                //check random color or color blocks
                layoutStatus l_layoutStatus = GetCurrentLayoutStatus();
                foreach (EffectColor effectColor in l_layoutStatus.effectColorList)
                {
                    if (effectColor.effect_id == l_layoutStatus.effect)  //find the effect had color block
                    {
                        if (effectColor.color_single_enable && (effectColor.color_single_status == status.on))
                        {
                            rec_color_single.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                            effectColor.color_single_r = 0x00;
                            effectColor.color_single_g = 0x00;
                            effectColor.color_single_b = 0x00;
                            EndSendLayoutData((uint)type_SendLayoutFlag.ColorClear, true);
                            return;
                        }
                    }
                }

                int select_id = -1;
                int enable_id = -1;

                List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
                if (currentEffectColor == null)
                {
                    EndSendLayoutData((uint)type_SendLayoutFlag.ColorClear, false);
                    e.Handled = true;
                    return;
                }

                foreach (ColorBlock r in currentEffectColor)
                {
                    if (r.enable)
                    {
                        if (r.selected)
                        {
                            string controlname = "rec_color_" + r.sid.ToString();
                            Rectangle control = (Rectangle)FindName(controlname);
                            control.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                            r.R = 0x00;
                            r.G = 0x00;
                            r.B = 0x00;

                            // clear selected satus
                            r.selected = false;
                            control.StrokeThickness = 1;
                            control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            select_id = (int)r.sid;
                        }
                        enable_id = (int)r.sid;
                    }

                }
                if ((enable_id >= 0) && (select_id >= 0))
                {
                    select_id = (select_id == enable_id) ? 0 : select_id + 1;
                    string controlname = "rec_color_" + select_id.ToString();
                    Rectangle control = (Rectangle)FindName(controlname);
                    currentEffectColor[select_id].selected = true;
                    control.StrokeThickness = 3;
                    control.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }


            }
            EndSendLayoutData((uint)type_SendLayoutFlag.ColorClear, true);
        }
        private void color_pad_MouseDown(object sender, MouseButtonEventArgs e, uint uid)
        {
            //uid = 0(color pad) / 1(red) / 2(orange) .....7(violed) 
            byte[] pix = new byte[4];
            if (uid == (uint)colorpad.pad)
            {
                BitmapSource bmp = Img_color_pad.Source as BitmapSource;

                /// get mouse down position
                Point pos = e.GetPosition(Img_color_pad);

                /// map position to pixel base
                //int x = (int)((pos.X / Img_color_pad.ActualWidth) *
                //   bmp.PixelWidth);
                //int y = (int)((pos.Y / Img_color_pad.ActualHeight) *
                // bmp.PixelHeight);
                int x = (int)(pos.X);
                int y = (int)(pos.Y);
                if (x >= bmp.PixelWidth)
                    x = bmp.PixelWidth - 1;
                if (y >= bmp.PixelHeight)
                    y = bmp.PixelHeight - 1;
                /// pick the color to a byte array
                CroppedBitmap cb = new CroppedBitmap(bmp, new Int32Rect(x, y, 1, 1));

                cb.CopyPixels(pix, 4, 0);
            }
            else if (uid == (uint)colorpad.red)    { pix[2] = 0xFF; pix[1] = 0x00; pix[0] = 0x00; }
            else if (uid == (uint)colorpad.orange) { pix[2] = 0xFF; pix[1] = 0x7D; pix[0] = 0x00; }
            else if (uid == (uint)colorpad.yellow) { pix[2] = 0xFF; pix[1] = 0xFF; pix[0] = 0x00; }
            else if (uid == (uint)colorpad.greeen) { pix[2] = 0x00; pix[1] = 0xFF; pix[0] = 0x00; }
            else if (uid == (uint)colorpad.blue)   { pix[2] = 0x00; pix[1] = 0x00; pix[0] = 0xFF; }
            else if (uid == (uint)colorpad.indigo) { pix[2] = 0x00; pix[1] = 0xFF; pix[0] = 0xFF; }
            else if (uid == (uint)colorpad.violet) { pix[2] = 0xFF; pix[1] = 0x00; pix[0] = 0xFF; }

            if((pix[2] >= 250) && (pix[0] < 128))
            {
                if (pix[0] < 100)
                    pix[0] = (byte)(pix[0] / 10);
                else
                    pix[0] = (byte)(pix[0] - 90);
            }
            // show the picked color
            StartSendLayoutData((uint)type_SendLayoutFlag.ColorPad);

            if (KBType.KBTI_KBlayout_enable) 
            {
                KBType.SetKeyBoardColor(m_mode, pix[2], pix[1], pix[0]);            
            }
            //if (KBTable.m_KBlayout_enable) 
            //{
            //    KBTable.SetKeyBoardColor(m_mode, pix[2], pix[1], pix[0]);            
            //}
            else 
            {
                // single color or block color(or null)
                layoutStatus l_layoutStatus = GetCurrentLayoutStatus();
                foreach (EffectColor effectColor in l_layoutStatus.effectColorList)
                {
                    if (effectColor.effect_id == l_layoutStatus.effect)  //find the effect had color block
                    {
                        if(effectColor.color_single_enable&&(effectColor.color_single_status == status.on))
                        {
                            rec_color_single.Fill = new SolidColorBrush(Color.FromRgb(pix[2], pix[1], pix[0]));
                            effectColor.color_single_r = pix[2];
                            effectColor.color_single_g = pix[1];
                            effectColor.color_single_b = pix[0];
                            EndSendLayoutData((uint)type_SendLayoutFlag.ColorPad, true);
                            e.Handled = true;
                            return;
                        }
                    }
                }

                int select_id = -1;
                int enable_id = -1;

                List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
                if (currentEffectColor == null)
                {
                    EndSendLayoutData((uint)type_SendLayoutFlag.ColorPad, false);
                    e.Handled = true;
                    return;
                }

                foreach (ColorBlock r in currentEffectColor)
                {
                    if (r.enable)
                    {
                        if (r.selected)
                        {
                            string controlname = "rec_color_" + r.sid.ToString();
                            Rectangle control = (Rectangle)FindName(controlname);
                            control.Fill = new SolidColorBrush(Color.FromRgb(pix[2], pix[1], pix[0]));
                            r.R = pix[2];
                            r.G = pix[1];
                            r.B = pix[0];

                            // clear selected satus
                            r.selected = false;
                            control.StrokeThickness = 1;
                            control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            select_id = (int)r.sid;
                        }
                        enable_id = (int)r.sid;
                    }

                }
                if ((enable_id >= 0) && (select_id >= 0))
                {
                    select_id = (select_id == enable_id) ? 0 : select_id + 1;
                    string controlname = "rec_color_" + select_id.ToString();
                    Rectangle control = (Rectangle)FindName(controlname);
                    currentEffectColor[select_id].selected = true;
                    control.StrokeThickness = 3;
                    control.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }


            }
            EndSendLayoutData((uint)type_SendLayoutFlag.ColorPad, true);

            e.Handled = true;
        }

        private void Img_color_pad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            color_pad_MouseDown(sender, e, (uint)colorpad.pad);
        }

        private void Rec_color_pad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            uint rect_id = Convert.ToUInt32(rect.Uid);
            color_pad_MouseDown(sender, e, rect_id);

        }
        private void comboBox_Template_Option_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBox_Template_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ColorBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
            if (currentEffectColor == null)
                return;
            Rectangle rect = (Rectangle)sender;
            int rect_id = Convert.ToInt32(rect.Uid);
            if (rect_id >= currentEffectColor.Count)
                return;
            else if (currentEffectColor[rect_id].enable == false)
                return;
            else
            {
                currentEffectColor[rect_id].selected = !currentEffectColor[rect_id].selected;
                if (currentEffectColor[rect_id].selected)
                {
                    rect.StrokeThickness = 3;
                    rect.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else
                {
                    rect.StrokeThickness = 1;
                    rect.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                }
            }
            
        }
        private void img_color_mode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (img_color_mode.IsEnabled == false)
                return;
            layoutStatus l_layoutStatus = GetCurrentLayoutStatus();
            foreach (EffectColor effectColor in l_layoutStatus.effectColorList)
            {
                if (l_layoutStatus.effect == effectColor.effect_id)  //find the effect had color block
                {
                    if (effectColor.color_single_enable == status.disable)
                        return;

                    string str_url = "..\\Image\\check_box_off.png";
                    Uri uri_Source = new Uri(str_url, UriKind.Relative);

                    if (effectColor.color_single_status == status.on)
                    {
                        effectColor.color_single_status = status.off;
                        img_color_mode.Source = new BitmapImage(uri_Source);
                        rec_color_single.Visibility = Visibility.Hidden;

                        foreach (ColorBlock r in effectColor.colorblock)
                        {
                            int blocks = effectColor.colorblock.Count;
                            string controlname = "rec_color_" + r.sid.ToString();
                            Rectangle control = (Rectangle)FindName(controlname);
                            if (r.sid == 0)
                            {
                                control.Fill = new SolidColorBrush(Color.FromRgb(r.R, r.G, r.B));
                                control.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                                control.StrokeThickness = 3;
                                r.selected = true;
                                r.enable = true;
                            }
                            else if (r.sid < blocks)
                            {
                                control.Fill = new SolidColorBrush(Color.FromRgb(r.R, r.G, r.B));
                                control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                                control.StrokeThickness = 1;
                                r.selected = false;
                                r.enable = true;
                            }
                            else
                            {
                                control.Fill = null;
                                control.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                                control.StrokeThickness = 1;
                                r.selected = false;
                                r.enable = false;
                            }

                        }
                    }
                    else
                    {
                        foreach (ColorBlock colorBlock in effectColor.colorblock)//colorblock又執行七次
                        {
                            colorBlock.selected = false;
                            colorBlock.enable = false;
                        }
                        for (uint i = 0; i < 7; i++)//為何要做七次
                        {
                            string controlname = "rec_color_" + i.ToString();
                            Rectangle control = (Rectangle)FindName(controlname);
                            //control.Fill = null;
                            control.Fill = new SolidColorBrush(Color.FromRgb(0x3C, 0x45, 0x50));
                            control.Stroke = null;// new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            control.StrokeThickness = 1;
                            /*
                            foreach (ColorBlock colorBlock in effectColor.colorblock)//colorblock又執行七次
                            {
                                control.Fill = new SolidColorBrush(Color.FromRgb(0x3C, 0x45, 0x50));
                                colorBlock.selected = false;
                                colorBlock.enable = false;
                            }
                            */
                        }
                        effectColor.color_single_status = status.on;
                        str_url = "..\\Image\\check_box_on.png";
                        uri_Source = new Uri(str_url, UriKind.Relative);
                        img_color_mode.Source = new BitmapImage(uri_Source);
                        rec_color_single.Fill = new SolidColorBrush(Color.FromRgb(effectColor.color_single_r,
                            effectColor.color_single_g, effectColor.color_single_b));
                        rec_color_single.Visibility = Visibility.Visible;
                    }
                    StartSendLayoutData((uint)type_SendLayoutFlag.ColorSingle);
                    EndSendLayoutData((uint)type_SendLayoutFlag.ColorSingle, true);
                    e.Handled = true;
                    return;
                }
            }
        }
        /*
        private void edit_Second_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (edit_Second.IsEnabled == false)
                return;
              string pattern = @"^[0-9]$";
              Regex regex = new Regex(pattern);
              if (!regex.IsMatch(e.Text))
              {
                e.Handled = true;
              }

        }
        private void edit_Second_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (edit_Second.IsEnabled == false)
                return;
            TextBox textbox = (TextBox)sender;
            uint sec = 5;
            try
            {
                uint num = Convert.ToUInt32(textbox.Text);
                if (num > 60)
                {
                    sec = 60;
                    edit_Second.Text = "60";
                }
                else if( num <= 0)
                {
                    sec = 5;
                    edit_Second.Text = "5";
                }
                else
                    sec = num;
            }
            catch
            {
                if(textbox.IsEnabled)
                    edit_Second.Text = "5";
                else
                    edit_Second.Text = "";
            }

            //store
            foreach(ColorChangeTiming t in m_windows_layoutstatus.colorTimerList)
            {
                if(t.effect_id == m_windows_layoutstatus.effect)
                {
                    t.seconds = sec;
                    break;
                }
            }

             StartSendLayoutData((uint)type_SendLayoutFlag.Seconds);
             EndSendLayoutData((uint)type_SendLayoutFlag.Seconds, true);
        }
        */
        private void img_Brightness_ball_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_Brightness_ball.CaptureMouse();
            m_isLightBall_MouseDown = true;
            e.Handled = true;
        }
        private void img_Brightness_ball_MouseUp(object sender, MouseButtonEventArgs e)
        {
            byte level = 0;
            img_Brightness_ball.ReleaseMouseCapture();
            double x = img_Brightness_ball.Margin.Left;
            x = x + 15;
            if( x< 164)
            {
                level = 0;
            }
            else if( x < 211)
            {
                level = 1;
            }
            else if (x < 258)
            {
                level = 2;
            }
            else if (x < 305)
            {
                level = 3;
            }
            else if (x < 352)
            {
                level = 4;
            }
            if(m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, true);
            }
            else if(m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, false);
            }
            m_isLightBall_MouseDown = false;
            e.Handled = true;

        }
        private void img_Brightness_ball_MouseMove(object sender, MouseEventArgs e)
        {
            if(m_isLightBall_MouseDown)
            {
                Point curMouseDownPoint = e.GetPosition(ControlGrid);
                if ((curMouseDownPoint.Y > 590) || (curMouseDownPoint.Y < 530))
                {
                    m_isLightBall_MouseDown = false;
                    return;
                }

                curMouseDownPoint.X = curMouseDownPoint.X - 15;
                if (curMouseDownPoint.X > 313)
                    curMouseDownPoint.X = 313;
                else if (curMouseDownPoint.X < 125)
                    curMouseDownPoint.X = 125;
                else
                {

                }

                img_Brightness_ball.Margin = new Thickness(curMouseDownPoint.X, brightness_ball_margin.height, 0, 0);

            }

        }
        private void img_Brightness_bar_bg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_Brightness_bar.CaptureMouse();
            img_Brightness_bar_bg.CaptureMouse();
            byte level = 0;
            Point p = e.GetPosition(this);
            double x = p.X;
            if (x < 164)
            {
                level = 0;
            }
            else if (x < 211)
            {
                level = 1;
            }
            else if (x < 258)
            {
                level = 2;
            }
            else if (x < 305)
            {
                level = 3;
            }
            else if (x < 352)
            {
                level = 4;
            }

            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, false);
            }

            e.Handled = true;
        }
        private void img_Brightness_bar_bg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_Brightness_bar_bg.ReleaseMouseCapture();
            img_Brightness_bar.ReleaseMouseCapture();
            e.Handled = true;
        }
        private void img_Brightness_line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            byte level = 0;
            Image img = (Image)sender;
            img.CaptureMouse();
            if (img.Name == "img_Brightness_line_1")
                level = 1;
            else if (img.Name == "img_Brightness_line_2")
                level = 2;
            else if (img.Name == "img_Brightness_line_3")
                level = 3;

            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, false);
            }

            e.Handled = true;
        }
        private void img_Brightness_line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            img.ReleaseMouseCapture();
            e.Handled = true;
        }
        private void img_Tempo_ball_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_Tempo_ball.CaptureMouse();
            m_isTempoBall_MouseDown = true;
            e.Handled = true;
        }
        private void img_Tempo_ball_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_Tempo_ball.ReleaseMouseCapture();
            byte level = 0;
            double x = img_Tempo_ball.Margin.Left;
            x = x + 15;
            if (x < 505)
            {
                level = 0;
            }
            else if (x < 519)
            {
                level = 1;
            }
            else if (x < 540)
            {
                level = 2;
            }
            else if (x < 561)
            {
                level = 3;
            }
            else if (x < 582)
            {
                level = 4;
            }
            else if (x < 603)
            {
                level = 5;
            }
            else if (x < 624)
            {
                level = 6;
            }
            else if (x < 645)
            {
                level = 7;
            }
            else if (x < 666)
            {
                level = 8;
            }
            else if (x < 687)
            {
                level = 9;
            }
            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.tempo_level = level;
                SetTempoLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.tempo_level = level;
                SetTempoLayout(level, false);
            }

            m_isTempoBall_MouseDown = false;
            e.Handled = true;

        }
        private void img_Tempo_ball_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isTempoBall_MouseDown)
            {
                Point curMouseDownPoint = e.GetPosition(ControlGrid);
                if ((curMouseDownPoint.Y > 590) || (curMouseDownPoint.Y < 530))
                {
                    m_isTempoBall_MouseDown = false;
                    return;
                }

                curMouseDownPoint.X = curMouseDownPoint.X - 15;
                if (curMouseDownPoint.X > 670)
                    curMouseDownPoint.X = 670;
                else if (curMouseDownPoint.X < 488)
                    curMouseDownPoint.X = 488;
                else
                {

                }

                img_Tempo_ball.Margin = new Thickness(curMouseDownPoint.X, tempo_ball_margin.height, 0, 0);

            }
        }
        private void img_Tempo_bar_bg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            img_Tempo_bar.CaptureMouse();
            img_Tempo_bar_bg.CaptureMouse();
            Point p = e.GetPosition(this);
            double x = p.X;
            byte level = 0;
            if (x < 505)
            {
                level = 0;
            }
            else if (x < 519)
            {
                level = 1;
            }
            else if (x < 540)
            {
                level = 2;
            }
            else if (x < 561)
            {
                level = 3;
            }
            else if (x < 582)
            {
                level = 4;
            }
            else if (x < 603)
            {
                level = 5;
            }
            else if (x < 624)
            {
                level = 6;
            }
            else if (x < 645)
            {
                level = 7;
            }
            else if (x < 666)
            {
                level = 8;
            }
            else if (x < 687)
            {
                level = 9;
            }
            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.tempo_level = level;
                SetTempoLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.tempo_level = level;
                SetTempoLayout(level, false);
            }
            e.Handled = true;
        }
        private void img_Tempo_bar_bg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            img_Tempo_bar_bg.ReleaseMouseCapture();
            img_Tempo_bar.ReleaseMouseCapture();
            e.Handled = true;
        }
        private void img_Tempo_line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            img.CaptureMouse();
            byte level = 0;
            if (img.Name == "img_Tempo_line_1")
                level = 1;
            else if (img.Name == "img_Tempo_line_2")
                level = 2;
            else if (img.Name == "img_Tempo_line_3")
                level = 3;
            else if (img.Name == "img_Tempo_line_4")
                level = 4;
            else if (img.Name == "img_Tempo_line_5")
                level = 5;
            else if (img.Name == "img_Tempo_line_6")
                level = 6;
            else if (img.Name == "img_Tempo_line_7")
                level = 7;
            else if (img.Name == "img_Tempo_line_8")
                level = 8;
            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.tempo_level = level;
                SetTempoLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.tempo_level = level;
                SetTempoLayout(level, false);
            }

            e.Handled = true;
        }
        private void img_Tempo_line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            img.ReleaseMouseCapture();
            e.Handled = true;
        }
        private void comboBox_Direction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Direction.SelectedIndex == -1)  // not selected
                return;
            layoutStatus l_layoutStatus = GetCurrentLayoutStatus();
            foreach (EffectDirection d in l_layoutStatus.effectdirection)
            {
                if (d.effect_id == l_layoutStatus.effect)
                {
                    d.direction_index = comboBox_Direction.SelectedIndex;
                    break;
                }
            }

            StartSendLayoutData((uint)type_SendLayoutFlag.Direction);
            EndSendLayoutData((uint)type_SendLayoutFlag.Direction, true);

        }

        private void img_powerOnOff_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_isPowerOn == status.on)
            {
                PowerOff(true, true, true);
            }
            else
            {
                PowerOn(true, true);
            }
            e.Handled = true;
        }
        private void img_powersaving_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_isPowerSaving)
            {
                PowerSavingOff(true);
            }
            else
            {
                PowerSavingOn(true);
            }
            e.Handled = true;
        }
        private uint DFKeyboardCS(uint[] data , uint len)
        {
            uint i;
            uint sum;

            sum = 0;

            for (i = 0; i < len; i++)            
                sum += data[i];
            
            return (uint)(0xFF - sum);
        }		
        private byte TranslateDarfonEffect()
        {
            ComboBoxItem item;

            if(m_mode == (uint)mode.windows)
               item = (ComboBoxItem)combobox_Windows_Effect.SelectedItem;
            else
                item = (ComboBoxItem)combobox_Welcome_Effect.SelectedItem;

            if (item == null)
                return 0xFF;
            uint uid = 0;
            try
            {
                uid = Convert.ToUInt32(item.Uid);
            }
            catch
            {
                return 0xFF;
            }
            switch (uid)
            {
                case (uint)effect.Static:
                    {
                        if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                        { return 0x20; }
                        else  // FOUR_ZONE
                        { return 0x01; }
                    }
                case (uint)effect.Breathing:
                    return 0x02;
                case (uint)effect.Wave:
                    return 0x03;
                case (uint)effect.Ripple:
                    return 0x06;
                case (uint)effect.Reactive:
                    return 0x04;
                case (uint)effect.Rainbow:
                    return 0x05;
                case (uint)effect.Marquee:
                    return 0x09;
                case (uint)effect.Raindrop:
                    return 0x0A;
                case (uint)effect.Stacker:
                    return 0x23;
                case (uint)effect.Impact:
                    return 0x22;
                //case (uint)effect.Whirling:
                //    return 0x0D;
                case (uint)effect.Aurora:
                    return 0x0E;
                case (uint)effect.Spark:
                    return 0x11;
                case (uint)effect.Music:
                    return 0x24;
                case (uint)effect.UserMode:
                    {
                        if (m_mode == (uint)mode.windows)
                        { return 0x25; }
                        else  // Welcome Mode
                        { return 0x33; }
                    }
                case (uint)effect.Neon:
                    return 0x26;
                case (uint)effect.Flash:
                    return 0x12;
                case (uint)effect.Mix:
                    return 0x13;
                //case (uint)effect.Close: //TBD
                //    return 0xFF;
                default:
                    return 0x05;  //Rainbow mode

            }
            
        }
        private byte TranslateDarfonTrigger()
        {
            if ((m_trigger == (uint)tirgger.auto) && (Img_trigger_auto.IsEnabled == status.enable))
                return 0;
            else if ((m_trigger == (uint)tirgger.onkeypressed) && (Img_trigger_onkeypressed.IsEnabled == status.enable))
                return 1;
            else
                return 0;
        }
        private byte TranslateDarfonLightlevel()
        {
            byte level = (m_mode == (uint)mode.windows) ? m_windows_layoutstatus.brightness_level : m_welcome_layoutstatus.brightness_level;

           switch(level)
            {
                case 0:
                    return 0x00;
                case 1:
                    return 0x08;
                case 2:
                    return 0x16;
                case 3:
                    return 0x24;
                case 4:
                    return 0x32;
                default:
                    break;
            }
            return 255;
        }
        private byte TranslateDarfonTempolevel()
        {
            byte level = (m_mode == (uint)mode.windows) ? m_windows_layoutstatus.tempo_level : m_welcome_layoutstatus.tempo_level;
            switch (level)
            {
                case 0:
                    return 0x0A;   // 0X0B, Speed Stop
                case 1:
                    return 0x09;
                case 2:
                    return 0x08;
                case 3:
                    return 0x07;
                case 4:
                    return 0x06;
                case 5:
                    return 0x05;
                case 6:
                    return 0x04;
                case 7:
                    return 0x03;
                case 8:
                    return 0x02;
                case 9:
                    return 0x01;
                default:
                    break;
            }
            return 255;
        }
        private byte TranslateDarfonDirection()
        {
            if (comboBox_Direction.IsEnabled == false)
                return 0;

            ComboBoxItem item = (ComboBoxItem)comboBox_Direction.SelectedItem;
            byte direction_uid = 0;
            try
            {
                direction_uid = Convert.ToByte(item.Uid);
            }
            catch
            {
                direction_uid = 0x00;
            }

            if (direction_uid == (uint)id_direction.LR)
                return 1;
            else if (direction_uid == (uint)id_direction.RL)
                return 2;
            else if (direction_uid == (uint)id_direction.TB)
                return 4;
            else if (direction_uid == (uint)id_direction.BT)
                return 3;
            else if (direction_uid == (uint)id_direction.CW)
                return 1;
            else if (direction_uid == (uint)id_direction.CCW)
                return 2;
            else if (direction_uid == (uint)id_direction.Auto)
                return 0; // Auto
            else if (direction_uid == (uint)id_direction.OnKeyPressed)
                return 1;
            else if (direction_uid == (uint)id_direction.Sync)
                return 3;
            else
                return 0;

        }

        private void TranslateDarfonWindowsRGB()
        {
            //SetRGB Color
            if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                List<key> currentKeyList;
                //if (KBTable.m_KBlayout_enable)
                if (KBType.KBTI_KBlayout_enable)
                {
                    //Clear user RGB
                    for (int i = 0; i < m_windows_DLLBuffer.userRGB.Length; i++)
                    {
                        m_windows_DLLBuffer.userRGB[i].ID = 0;
                        m_windows_DLLBuffer.userRGB[i].R = 0;
                        m_windows_DLLBuffer.userRGB[i].G = 0;
                        m_windows_DLLBuffer.userRGB[i].B = 0;
                    }
                    if (m_mode == (uint)mode.windows)
                        //currentKeyList = KBTable.m_KeyList;
                        currentKeyList = KBType.KBTI_KeyList;
                    else if (m_mode == (uint)mode.welcome)
                        //currentKeyList = KBTable.m_WelKeyList;
                        currentKeyList = KBType.KBTI_WelKeyList;
                    else
                        //currentKeyList = KBTable.m_KeyList;
                        currentKeyList = KBType.KBTI_KeyList;
                    for (int i = 0, kb_i = 0; (i < m_windows_DLLBuffer.KBRGB.Length) && (kb_i < currentKeyList.Count); i++, kb_i++)
                        {
                            m_windows_DLLBuffer.KBRGB[i].ID = currentKeyList[kb_i].df_id1;
                            m_windows_DLLBuffer.KBRGB[i].R = currentKeyList[kb_i].R;
                            m_windows_DLLBuffer.KBRGB[i].G = currentKeyList[kb_i].G;
                            m_windows_DLLBuffer.KBRGB[i].B = currentKeyList[kb_i].B;

                            if (currentKeyList[kb_i].df_id2 > 0)
                            {
                                m_windows_DLLBuffer.KBRGB[i + 1].ID = currentKeyList[kb_i].df_id2;
                                m_windows_DLLBuffer.KBRGB[i + 1].R = currentKeyList[kb_i].R;
                                m_windows_DLLBuffer.KBRGB[i + 1].G = currentKeyList[kb_i].G;
                                m_windows_DLLBuffer.KBRGB[i + 1].B = currentKeyList[kb_i].B;
                                i++;

                                //Workaround to match space 3 kbid...
                                if (currentKeyList[kb_i].df_id2 == 37) // space 3 threee is is 31,37,43
                                {
                                    m_windows_DLLBuffer.KBRGB[i + 1].ID = 43;
                                    m_windows_DLLBuffer.KBRGB[i + 1].R = currentKeyList[kb_i].R;
                                    m_windows_DLLBuffer.KBRGB[i + 1].G = currentKeyList[kb_i].G;
                                    m_windows_DLLBuffer.KBRGB[i + 1].B = currentKeyList[kb_i].B;
                                    i++;
                                }

                            }

                        }


                }
                else
                {
                    //Clear Keyboard RGB for Full ME KEB
                    if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                    {
                        for (int i = 0; i < m_windows_DLLBuffer.KBRGB.Length; i++)
                        {
                            m_windows_DLLBuffer.KBRGB[i].ID = 0;
                            m_windows_DLLBuffer.KBRGB[i].R = 0;
                            m_windows_DLLBuffer.KBRGB[i].G = 0;
                            m_windows_DLLBuffer.KBRGB[i].B = 0;
                        }
                    }


                    //Feed Rain bow color
                    if (m_windows_layoutstatus.effect == (uint)effect.Rainbow)
                    {
                        m_windows_DLLBuffer.userRGB[1].ID = 1; m_windows_DLLBuffer.userRGB[1].R = 0xFF; m_windows_DLLBuffer.userRGB[1].G = 0x00; m_windows_DLLBuffer.userRGB[1].B = 0x00;
                        m_windows_DLLBuffer.userRGB[2].ID = 1; m_windows_DLLBuffer.userRGB[2].R = 0xFF; m_windows_DLLBuffer.userRGB[2].G = 0x7D; m_windows_DLLBuffer.userRGB[2].B = 0x00;
                        m_windows_DLLBuffer.userRGB[3].ID = 1; m_windows_DLLBuffer.userRGB[3].R = 0xFF; m_windows_DLLBuffer.userRGB[3].G = 0xFF; m_windows_DLLBuffer.userRGB[3].B = 0x00;
                        m_windows_DLLBuffer.userRGB[4].ID = 1; m_windows_DLLBuffer.userRGB[4].R = 0x00; m_windows_DLLBuffer.userRGB[4].G = 0xFF; m_windows_DLLBuffer.userRGB[4].B = 0x00;
                        m_windows_DLLBuffer.userRGB[5].ID = 1; m_windows_DLLBuffer.userRGB[5].R = 0x00; m_windows_DLLBuffer.userRGB[5].G = 0x00; m_windows_DLLBuffer.userRGB[5].B = 0xFF;
                        m_windows_DLLBuffer.userRGB[6].ID = 1; m_windows_DLLBuffer.userRGB[6].R = 0x00; m_windows_DLLBuffer.userRGB[6].G = 0xFF; m_windows_DLLBuffer.userRGB[6].B = 0xFF;
                        m_windows_DLLBuffer.userRGB[7].ID = 1; m_windows_DLLBuffer.userRGB[7].R = 0xFF; m_windows_DLLBuffer.userRGB[7].G = 0x00; m_windows_DLLBuffer.userRGB[7].B = 0xFF;


                        return;
                    }

                    //Check Single or Color Blocks
                    m_windows_DLLBuffer.bSingleDisplay = false;
                    foreach (EffectColor effectColor in m_windows_layoutstatus.effectColorList)
                    {
                        if (effectColor.effect_id == m_windows_layoutstatus.effect)  //find the effect had color block
                        {
                            if (effectColor.color_single_enable && (effectColor.color_single_status == status.on))
                            {
                                m_windows_DLLBuffer.userRGB[1].ID = 1;
                                m_windows_DLLBuffer.userRGB[1].R = effectColor.color_single_r;
                                m_windows_DLLBuffer.userRGB[1].G = effectColor.color_single_g;
                                m_windows_DLLBuffer.userRGB[1].B = effectColor.color_single_b;
                                for (int i = 2; i < 8; i++)
                                {
                                    m_windows_DLLBuffer.userRGB[i].ID = 0;
                                    m_windows_DLLBuffer.userRGB[i].R = 0;
                                    m_windows_DLLBuffer.userRGB[i].G = 0;
                                    m_windows_DLLBuffer.userRGB[i].B = 0;
                                }
                                m_windows_DLLBuffer.bSingleDisplay = true;
                                break;
                            }
                        }
                    }
                    if (!m_windows_DLLBuffer.bSingleDisplay)
                    {
                        List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
                        if (currentEffectColor != null)
                        {
                            for (int i = 1; i < 8; i++)
                            {
                                if (i < (1 + currentEffectColor.Count))
                                {
                                    m_windows_DLLBuffer.userRGB[i].ID = 1;
                                    m_windows_DLLBuffer.userRGB[i].R = currentEffectColor[i - 1].R;
                                    m_windows_DLLBuffer.userRGB[i].G = currentEffectColor[i - 1].G;
                                    m_windows_DLLBuffer.userRGB[i].B = currentEffectColor[i - 1].B;
                                }
                                else
                                {
                                    m_windows_DLLBuffer.userRGB[i].ID = 0;
                                    m_windows_DLLBuffer.userRGB[i].R = 0;
                                    m_windows_DLLBuffer.userRGB[i].G = 0;
                                    m_windows_DLLBuffer.userRGB[i].B = 0;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 1; i < 8; i++)
                            {
                                m_windows_DLLBuffer.userRGB[i].ID = 0;
                                m_windows_DLLBuffer.userRGB[i].R = 0;
                                m_windows_DLLBuffer.userRGB[i].G = 0;
                                m_windows_DLLBuffer.userRGB[i].B = 0;
                            }

                        }
                    }

                }
            }
            else if( m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
            {
                //Feed Rain bow color
                if (m_windows_layoutstatus.effect ==  (uint)effect.Rainbow)
                {

                    m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bColor = 0x08;

                    m_windows_FOURZONE_DLLBuffer.userRGB[1].ID = 1; m_windows_FOURZONE_DLLBuffer.userRGB[1].R = 0xFF; m_windows_FOURZONE_DLLBuffer.userRGB[1].G = 0x00; m_windows_FOURZONE_DLLBuffer.userRGB[1].B = 0x00;
                    m_windows_FOURZONE_DLLBuffer.userRGB[2].ID = 1; m_windows_FOURZONE_DLLBuffer.userRGB[2].R = 0xFF; m_windows_FOURZONE_DLLBuffer.userRGB[2].G = 0xFF; m_windows_FOURZONE_DLLBuffer.userRGB[2].B = 0x00;
                    m_windows_FOURZONE_DLLBuffer.userRGB[3].ID = 1; m_windows_FOURZONE_DLLBuffer.userRGB[3].R = 0x00; m_windows_FOURZONE_DLLBuffer.userRGB[3].G = 0x00; m_windows_FOURZONE_DLLBuffer.userRGB[3].B = 0xFF;
                    m_windows_FOURZONE_DLLBuffer.userRGB[4].ID = 1; m_windows_FOURZONE_DLLBuffer.userRGB[4].R = 0xFF; m_windows_FOURZONE_DLLBuffer.userRGB[4].G = 0x00; m_windows_FOURZONE_DLLBuffer.userRGB[4].B = 0xFF;

                    return;
                }

                //Check Single or Color Blocks
                m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bColor = 0x08;
                foreach (EffectColor effectColor in m_windows_layoutstatus.effectColorList)
                {
                    if (effectColor.effect_id == m_windows_layoutstatus.effect)  //find the effect had color block
                    {
                        if (effectColor.color_single_enable && (effectColor.color_single_status == status.on))
                        {
                            m_windows_FOURZONE_DLLBuffer.userRGB[1].ID = 1;
                            m_windows_FOURZONE_DLLBuffer.userRGB[1].R = effectColor.color_single_r;
                            m_windows_FOURZONE_DLLBuffer.userRGB[1].G = effectColor.color_single_g;
                            m_windows_FOURZONE_DLLBuffer.userRGB[1].B = effectColor.color_single_b;
                            for (int i = 2; i < 8; i++)
                            {
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].ID = 0;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].R = 0;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].G = 0;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].B = 0;
                            }
                            m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bColor = 0x01;//0x08:circular color   0x01: windows mode color index1
                            break;
                        }
                    }
                }
                if (m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bColor  == 0x08) //0x08:circular color   0x09:color index1
                {
                    List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
                    if (currentEffectColor != null)
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            if (i < (1 + currentEffectColor.Count))
                            {
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].ID = 1;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].R = currentEffectColor[i - 1].R;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].G = currentEffectColor[i - 1].G;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].B = currentEffectColor[i - 1].B;
                            }
                            else
                            {
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].ID = 0;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].R = 0;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].G = 0;
                                m_windows_FOURZONE_DLLBuffer.userRGB[i].B = 0;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            m_windows_FOURZONE_DLLBuffer.userRGB[i].ID = 0;
                            m_windows_FOURZONE_DLLBuffer.userRGB[i].R = 0;
                            m_windows_FOURZONE_DLLBuffer.userRGB[i].G = 0;
                            m_windows_FOURZONE_DLLBuffer.userRGB[i].B = 0;
                        }

                    }
                }
            }


        }
        private void TranslateDarfonWelcomeRGB()
        {
            if( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                //Feed Rain bow color
                if (m_welcome_layoutstatus.effect == (uint)effect.Rainbow)
                {
                    m_welcome_DLLBuffer.userRGB[9].ID = 1; m_welcome_DLLBuffer.userRGB[9].R = 0xFF; m_welcome_DLLBuffer.userRGB[9].G = 0x00; m_welcome_DLLBuffer.userRGB[9].B = 0x00;
                    m_welcome_DLLBuffer.userRGB[10].ID = 1; m_welcome_DLLBuffer.userRGB[10].R = 0xFF; m_welcome_DLLBuffer.userRGB[10].G = 0x7D; m_welcome_DLLBuffer.userRGB[10].B = 0x00;
                    m_welcome_DLLBuffer.userRGB[11].ID = 1; m_welcome_DLLBuffer.userRGB[11].R = 0xFF; m_welcome_DLLBuffer.userRGB[11].G = 0xFF; m_welcome_DLLBuffer.userRGB[11].B = 0x00;
                    m_welcome_DLLBuffer.userRGB[12].ID = 1; m_welcome_DLLBuffer.userRGB[12].R = 0x00; m_welcome_DLLBuffer.userRGB[12].G = 0xFF; m_welcome_DLLBuffer.userRGB[12].B = 0x00;
                    m_welcome_DLLBuffer.userRGB[13].ID = 1; m_welcome_DLLBuffer.userRGB[13].R = 0x00; m_welcome_DLLBuffer.userRGB[13].G = 0x00; m_welcome_DLLBuffer.userRGB[13].B = 0xFF;
                    m_welcome_DLLBuffer.userRGB[14].ID = 1; m_welcome_DLLBuffer.userRGB[14].R = 0x00; m_welcome_DLLBuffer.userRGB[14].G = 0xFF; m_welcome_DLLBuffer.userRGB[14].B = 0xFF;
                    m_welcome_DLLBuffer.userRGB[15].ID = 1; m_welcome_DLLBuffer.userRGB[15].R = 0xFF; m_welcome_DLLBuffer.userRGB[15].G = 0x00; m_welcome_DLLBuffer.userRGB[15].B = 0xFF;
                    return;
                }

                //Check Single or Color Blocks
                m_welcome_DLLBuffer.bSingleDisplay = false;
                foreach (EffectColor effectColor in m_welcome_layoutstatus.effectColorList)
                {
                    if (effectColor.effect_id == m_welcome_layoutstatus.effect)  //find the effect had color block
                    {
                        if (effectColor.color_single_enable && (effectColor.color_single_status == status.on))
                        {
                            m_welcome_DLLBuffer.userRGB[9].ID = 1;
                            m_welcome_DLLBuffer.userRGB[9].R = effectColor.color_single_r;
                            m_welcome_DLLBuffer.userRGB[9].G = effectColor.color_single_g;
                            m_welcome_DLLBuffer.userRGB[9].B = effectColor.color_single_b;
                            for (int i = 0x0a; i <= 0x0f; i++)
                            {
                                m_welcome_DLLBuffer.userRGB[i].ID = 0;
                                m_welcome_DLLBuffer.userRGB[i].R = 0;
                                m_welcome_DLLBuffer.userRGB[i].G = 0;
                                m_welcome_DLLBuffer.userRGB[i].B = 0;
                            }
                            m_welcome_DLLBuffer.bSingleDisplay = true;
                            break;
                        }
                    }
                }
                if (!m_welcome_DLLBuffer.bSingleDisplay)
                {
                    List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
                    if (currentEffectColor != null)
                    {
                        for (int i = 0x09; i <= 0x0f; i++)
                        {
                            if (i < (0x09 + currentEffectColor.Count))
                            {
                                m_welcome_DLLBuffer.userRGB[i].ID = 1;
                                m_welcome_DLLBuffer.userRGB[i].R = currentEffectColor[i - 9].R;
                                m_welcome_DLLBuffer.userRGB[i].G = currentEffectColor[i - 9].G;
                                m_welcome_DLLBuffer.userRGB[i].B = currentEffectColor[i - 9].B;
                            }
                            else
                            {
                                m_welcome_DLLBuffer.userRGB[i].ID = 0;
                                m_welcome_DLLBuffer.userRGB[i].R = 0;
                                m_welcome_DLLBuffer.userRGB[i].G = 0;
                                m_welcome_DLLBuffer.userRGB[i].B = 0;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0x09; i <= 0x0f; i++)
                        {
                            m_welcome_DLLBuffer.userRGB[i].ID = 0;
                            m_welcome_DLLBuffer.userRGB[i].R = 0;
                            m_welcome_DLLBuffer.userRGB[i].G = 0;
                            m_welcome_DLLBuffer.userRGB[i].B = 0;
                        }

                    }
                }
            }
            else if( m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
            {
                //Feed Rain bow color
                if (m_welcome_layoutstatus.effect == (uint)effect.Rainbow)
                {
                    m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bColor = 0x08;  // 08:circular color, 09: color index 1
                    m_welcome_FOURZONE_DLLBuffer.userRGB[9].ID = 1; m_welcome_FOURZONE_DLLBuffer.userRGB[9].R = 0xFF; m_welcome_FOURZONE_DLLBuffer.userRGB[9].G = 0x00; m_welcome_FOURZONE_DLLBuffer.userRGB[9].B = 0x00;
                    m_welcome_FOURZONE_DLLBuffer.userRGB[10].ID = 1; m_welcome_FOURZONE_DLLBuffer.userRGB[10].R = 0xFF; m_welcome_FOURZONE_DLLBuffer.userRGB[10].G = 0xFF; m_welcome_FOURZONE_DLLBuffer.userRGB[10].B = 0x00;
                    m_welcome_FOURZONE_DLLBuffer.userRGB[11].ID = 1; m_welcome_FOURZONE_DLLBuffer.userRGB[11].R = 0x00; m_welcome_FOURZONE_DLLBuffer.userRGB[11].G = 0x00; m_welcome_FOURZONE_DLLBuffer.userRGB[11].B = 0xFF;
                    m_welcome_FOURZONE_DLLBuffer.userRGB[12].ID = 1; m_welcome_FOURZONE_DLLBuffer.userRGB[12].R = 0xFF; m_welcome_FOURZONE_DLLBuffer.userRGB[12].G = 0x00; m_welcome_FOURZONE_DLLBuffer.userRGB[12].B = 0xFF;
                    return;
                }

                //Check Single or Color Blocks
                m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bColor = 0x08;  // 08:circular color, 09: color index 1
                foreach (EffectColor effectColor in m_welcome_layoutstatus.effectColorList)
                {
                    if (effectColor.effect_id == m_welcome_layoutstatus.effect)  //find the effect had color block
                    {
                        if (effectColor.color_single_enable && (effectColor.color_single_status == status.on))
                        {
                            m_welcome_FOURZONE_DLLBuffer.userRGB[9].ID = 1;
                            m_welcome_FOURZONE_DLLBuffer.userRGB[9].R = effectColor.color_single_r;
                            m_welcome_FOURZONE_DLLBuffer.userRGB[9].G = effectColor.color_single_g;
                            m_welcome_FOURZONE_DLLBuffer.userRGB[9].B = effectColor.color_single_b;
                            for (int i = 0x0a; i <= 0x0f; i++)
                            {
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].ID = 0;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].R = 0;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].G = 0;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].B = 0;
                            }
                            m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bColor = 0x09;// 08:circular color, 09: welcome mode color index 1
                            break;
                        }
                    }
                }
                if (m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bColor == 0x08) // 08:circular color, 09: color index 1
                {
                    List<ColorBlock> currentEffectColor = GetCurrentEffectColorBlocks();
                    if (currentEffectColor != null)
                    {
                        for (int i = 0x09; i <= 0x0f; i++)
                        {
                            if (i < (0x09 + currentEffectColor.Count))
                            {
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].ID = 1;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].R = currentEffectColor[i - 9].R;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].G = currentEffectColor[i - 9].G;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].B = currentEffectColor[i - 9].B;
                            }
                            else
                            {
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].ID = 0;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].R = 0;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].G = 0;
                                m_welcome_FOURZONE_DLLBuffer.userRGB[i].B = 0;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0x09; i <= 0x0f; i++)
                        {
                            m_welcome_FOURZONE_DLLBuffer.userRGB[i].ID = 0;
                            m_welcome_FOURZONE_DLLBuffer.userRGB[i].R = 0;
                            m_welcome_FOURZONE_DLLBuffer.userRGB[i].G = 0;
                            m_welcome_FOURZONE_DLLBuffer.userRGB[i].B = 0;
                        }

                    }
                }
            }
        }

        private int AntiTranslateDarfonEffect(byte DLLeffectID)
        {

            switch (DLLeffectID)
            {
                case 0x01:
                    return (int)effect.Static;
                case 0x20:
                    return (int)effect.Static;
                case 0x02:
                    return (int)effect.Breathing;
                case 0x03:
                    return (int)effect.Wave;
                case 0x06:
                    return (int)effect.Ripple;
                case 0x04:
                    return (int)effect.Reactive;
                case 0x05:
                    return (int)effect.Rainbow;
                case 0x09:
                    return (int)effect.Marquee;
                case 0x0A:
                    return (int)effect.Raindrop;
                case 0x23:
                    return (int)effect.Stacker;
                case 0x22:
                    return (int)effect.Impact;
                //case (uint)effect.Whirling:
                //    return 0x0D;
                case 0x0E:
                    return (int)effect.Aurora;
                case 0x11:
                    return (int)effect.Spark;
                case 0x24:
                    return (int)effect.Music;
                case 0x25: // Windows
                case 0x33: // Welcome
                    return (int)effect.UserMode;
                case 0x26:
                    return (int)effect.Neon;
                //case (uint)effect.Close: //TBD
                //    return 0xFF;
                case 0x12:
                    return (int)effect.Flash;
                case 0x13:
                    return (int)effect.Mix;
                default:
                    return (int)effect.Rainbow;

            }

        }

        /*
        private uint TranslateDarfonSeconds()
        {
            if (edit_Second.IsEnabled == false)
                return 0;

            try
            {
                return (uint)Convert.ToByte(edit_Second.Text);
            }
            catch
            {
                return 0;
            }


        }
        */
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if(m_mode == (uint)mode.windows)
            {
                try
                {
                    uint level = 0 ;
                    WMIEC.WMIWriteECRAM(1873UL, 128UL + (ulong)level);
                    int Profile = comboBox_Profile.SelectedIndex;
                    Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "Profile", Profile, RegistryValueKind.DWord);
                    string NamePath = @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\" + Profile.ToString();
                    uint Effect = m_windows_layoutstatus.effect;
                    uint Brightnesslevel = m_windows_layoutstatus.brightness_level;
                    uint Tempolevel = m_windows_layoutstatus.tempo_level;

                    byte[] directionArray = new byte[m_windows_layoutstatus.effectdirection.Count];
                    int i = 0;
                    foreach (EffectDirection d in m_windows_layoutstatus.effectdirection)
                    {
                        directionArray[i] = (byte)d.direction_index;
                        i ++;
                    }

                    byte[] RGBArray = new byte[m_windows_layoutstatus.effectColorList.Count * (4+7*3)  ];
                    int j = 0;
                    foreach (EffectColor d in m_windows_layoutstatus.effectColorList)
                    {
                        RGBArray[j] = (byte)(d.color_single_status ? 1 : 0);
                        RGBArray[j + 1] = (byte)d.color_single_r;
                        RGBArray[j + 2] = (byte)d.color_single_g;
                        RGBArray[j + 3] = (byte)d.color_single_b;
                        j +=4;

                        foreach (ColorBlock r in d.colorblock)
                        {
                            RGBArray[j] = r.R;
                            RGBArray[j + 1] = r.G;
                            RGBArray[j + 2] = r.B;
                            j += 3;                           
                        }

                    }

                    //byte[] KeyBoardRGB = new byte[KBTable.m_KeyList.Count * 5];
                    byte[] KeyBoardRGB = new byte[KBType.KBTI_KeyList.Count * 5];
                    int k = 0;
                    //foreach (key rgb in KBTable.m_KeyList)
                    foreach (key rgb in KBType.KBTI_KeyList)
                    {
                        KeyBoardRGB[k] = rgb.df_id1;
                        KeyBoardRGB[k + 1] = rgb.df_id2;
                        KeyBoardRGB[k + 2] = rgb.R;
                        KeyBoardRGB[k + 3] = rgb.G;
                        KeyBoardRGB[k + 4] = rgb.B;
                        k += 5;
                    }

                    if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                    {
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Effect", Effect, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Brightnesslevel", Brightnesslevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Tempolevel", Tempolevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Direction", directionArray, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "RGBColor", RGBArray, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "KeyBoardRGB", KeyBoardRGB, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Brightness_perc", Brightness_Offset.Value, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Speed_perc", Speed_Offset.Value, RegistryValueKind.DWord);

                        // Woarkaround : For Han's  saying, ITE chip need to know save flag for NV-memory life.....
                        m_windows_DLLBuffer.NVsaving = true;
                        Utility.Log("[Trace]Btn_Save_Click WM_MSG_OOBE GRGBDLL_CaptureCMDBuffer start");
                        NativeMethod.GRGBDLL_CaptureCMDBuffer(ref m_windows_DLLBuffer);
                        Utility.Log("[Trace]Btn_Save_Click WM_MSG_OOBE GRGBDLL_CaptureCMDBuffer finished");

                    }
                    else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
                    {
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_Effect", Effect, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_Brightnesslevel", Brightnesslevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_Tempolevel", Tempolevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_Direction", directionArray, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_RGBColor", RGBArray, RegistryValueKind.Binary);
                        
                        // Woarkaround : For Han's  saying, ITE chip need to know save flag for NV-memory life.....
                        m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bsavingMode = true;
                        Utility.Log("[Trace]GRGBDLL_Capture4ZONEBuffer start");
                        NativeMethod.GRGBDLL_Capture4ZONEBuffer(ref m_windows_FOURZONE_DLLBuffer);
                        Utility.Log("[Trace]GRGBDLL_Capture4ZONEBuffer finished");
                    }
                    
                }
                catch
                {
                    //TODO : Recovery
                }

            }
            else  // wlecome mode
            {
                try
                {
                    SetWelcomeEffect();

                    int Profile = comboBox_Profile.SelectedIndex;
                    Utility.RegistryKeyWrite(RegistryHive.LocalMachine, @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\", "Profile", Profile, RegistryValueKind.DWord);
                    string NamePath = @"SOFTWARE\Rev.Center\Rev.Center2.0\KeyBoard\" + Profile.ToString();
                    uint Effect = m_welcome_layoutstatus.effect;
                    uint Brightnesslevel = m_welcome_layoutstatus.brightness_level;
                    uint Tempolevel = m_welcome_layoutstatus.tempo_level;

                    byte[] directionArray = new byte[m_welcome_layoutstatus.effectdirection.Count];
                    int i = 0;
                    foreach (EffectDirection d in m_welcome_layoutstatus.effectdirection)
                    {
                        directionArray[i] = (byte)d.direction_index;
                        i++;
                    }

                    byte[] RGBArray = new byte[m_welcome_layoutstatus.effectColorList.Count * (4 + 7 * 3)];
                    int j = 0;
                    foreach (EffectColor d in m_welcome_layoutstatus.effectColorList)
                    {
                        RGBArray[j] = (byte)(d.color_single_status ? 1 : 0);
                        RGBArray[j + 1] = (byte)d.color_single_r;
                        RGBArray[j + 2] = (byte)d.color_single_g;
                        RGBArray[j + 3] = (byte)d.color_single_b;
                        j += 4;

                        foreach (ColorBlock r in d.colorblock)
                        {
                            RGBArray[j] = r.R;
                            RGBArray[j + 1] = r.G;
                            RGBArray[j + 2] = r.B;
                            j += 3;
                        }

                    }
                    //byte[] KeyBoardRGB = new byte[KBTable.m_WelKeyList.Count * 5];
                    byte[] KeyBoardRGB = new byte[KBType.KBTI_WelKeyList.Count * 5];
                    int k = 0;
                    //foreach (key rgb in KBTable.m_WelKeyList)
                    foreach (key rgb in KBType.KBTI_WelKeyList)
                    {
                        KeyBoardRGB[k] = rgb.df_id1;
                        KeyBoardRGB[k + 1] = rgb.df_id2;
                        KeyBoardRGB[k + 2] = rgb.R;
                        KeyBoardRGB[k + 3] = rgb.G;
                        KeyBoardRGB[k + 4] = rgb.B;
                        k += 5;
                    }

                    if ( m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
                    {
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "WelEffect", Effect, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "WelBrightnesslevel", Brightnesslevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "WelTempolevel", Tempolevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "WelDirection", directionArray, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "WelRGBColor", RGBArray, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "WelKeyBoardRGB", KeyBoardRGB, RegistryValueKind.Binary);
                    }
                    else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB )
                    {
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_WelEffect", Effect, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_WelBrightnesslevel", Brightnesslevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_WelTempolevel", Tempolevel, RegistryValueKind.DWord);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_WelDirection", directionArray, RegistryValueKind.Binary);
                        Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "4Zone_WelRGBColor", RGBArray, RegistryValueKind.Binary);
                    }
                }
                catch
                {
                    //TODO : Recovery
                }
            }
            exportRegistry();

        }



        void exportRegistry()
        {
            string userRoot = "HKEY_LOCAL_MACHINE";
            string strKey = userRoot + "\\SOFTWARE\\OEM\\MyColor2\\0";
            string filepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "reg.exe";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.Arguments = "export \"" + strKey + "\" \"" + filepath + "\" /y";
                    proc.Start();
                    string stdout = proc.StandardOutput.ReadToEnd();
                    string stderr = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                // handle exception
            }
        }





        private static void Export()
        {
            string userRoot = "HKEY_LOCAL_MACHINE";
            string key = userRoot + "\\SOFTWARE\\OEM\\MyColor2\\0";

            string exportPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = "\"" + exportPath + "\"";

            Process proc = new Process();

            try
            {
                proc.StartInfo.FileName = "regedit.exe";
                proc.StartInfo.UseShellExecute = false;

                proc = Process.Start("regedit.exe", "/e " + exportPath + " " + key);
                proc.WaitForExit();
            }
            catch (Exception)
            {
                proc.Dispose();
            }
        }


        private void StartSendLayoutData( uint _send_layout_type)
        {
            if (m_sendflag == (uint)type_SendLayoutFlag.Clear)
                m_sendflag = _send_layout_type;

        }
        private void EndSendLayoutData(uint _send_layout_type, bool _bSendToDFFirmware)
        {
            if(m_sendflag == _send_layout_type)
            {
                m_sendflag = (uint)type_SendLayoutFlag.Clear;
                if (_bSendToDFFirmware)
                {
                    if (m_mode == (uint)mode.windows)
                    {
                        Utility.Log(String.Format("[Trace]Start SetWindowsEffect UI, and send flag = {0}", _send_layout_type.ToString()));
                        SetWindowsEffect();
                    }

                   // else
                  //    SetWelcomeEffect();
                }

            }
        }

        private void SetMusicMode(bool enable)
        {
            try
            {
                if (enable) //Music Mode
                {
	                m_delStartMonitorAudio(0);
                    img_Brightness_ball.Visibility = Visibility.Hidden;
                    img_Brightness_bar.Visibility = Visibility.Hidden;
                    img_Brightness_bar_bg.IsEnabled = false;
                    img_Tempo_ball.Visibility = Visibility.Hidden;
                    img_Tempo_bar.Visibility = Visibility.Hidden;
                    img_Tempo_bar_bg.IsEnabled = false;
                }
                else
                {
                    m_delStopMonitorAudio();
                    img_Brightness_ball.Visibility = Visibility.Visible;
                    img_Brightness_bar.Visibility = Visibility.Visible;
                    img_Brightness_bar_bg.IsEnabled = true;
                    img_Tempo_ball.Visibility = Visibility.Visible;
                    img_Tempo_bar.Visibility = Visibility.Visible;
                    img_Tempo_bar_bg.IsEnabled = true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("SetMusicEffect failed {0}", e.ToString());
                Utility.Log(String.Format("[Error]SetMusicEffect failed {0}",e.ToString()));
            }
        }
        private void SetWindowsEffect()
        {
            Utility.Log("[Trace]SetWindowsEffect");
            if (m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                m_windows_DLLBuffer.Mode = 0; // Hans's spec,  0= windows, 1 = Welcome
                m_windows_DLLBuffer.Effect = TranslateDarfonEffect();
                m_windows_DLLBuffer.Brightnesslevel = TranslateDarfonLightlevel();
                m_windows_DLLBuffer.Tempolevel = TranslateDarfonTempolevel();
                m_windows_DLLBuffer.Direction = TranslateDarfonDirection();
                m_windows_DLLBuffer.NVsaving = false;
                TranslateDarfonWindowsRGB();
                //Set Music Disable
                if (m_windows_DLLBuffer.Effect != 0x24)
                {
                    SetMusicMode(status.disable);
                }
                else if (m_windows_DLLBuffer.Effect == 0x24)
                {
                    // Workaround to clear RGB lighting status.
                    m_windows_DLLBuffer.Effect = 0x25; //User Mode
                    for (int i = 0; i < 126; i++)
                    {
                        m_windows_DLLBuffer.KBRGB[i].R = 0x00;
                        m_windows_DLLBuffer.KBRGB[i].G = 0x00;
                        m_windows_DLLBuffer.KBRGB[i].B = 0x00;
                    }
                    NativeMethod.GRGBDLL_CaptureCMDBuffer(ref m_windows_DLLBuffer);

                    m_windows_DLLBuffer.Effect = 0x24;
                }
                Utility.Log(String.Format("[Trace]GRGBDLL_CaptureCMDBuffer, effect ={0}, brightness={1}, tempo={2}, direction={3}"
                    , m_windows_DLLBuffer.Effect, m_windows_DLLBuffer.Brightnesslevel, m_windows_DLLBuffer.Tempolevel, m_windows_DLLBuffer.Direction));
                NativeMethod.GRGBDLL_CaptureCMDBuffer(ref m_windows_DLLBuffer);
                Utility.Log("[Trace]GRGBDLL_CaptureCMDBuffer finished");
                //Set Music Enable
                if (m_windows_DLLBuffer.Effect == 0x24)
                    SetMusicMode(status.enable);
            }
            else if(m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
            {

                m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bEffectType = TranslateDarfonEffect();
                m_windows_FOURZONE_DLLBuffer._TSLedType_08H.blight = TranslateDarfonLightlevel();
                m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bSpeed = TranslateDarfonTempolevel();
                m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bdirection = TranslateDarfonDirection();
                m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bsavingMode = false;
                TranslateDarfonWindowsRGB();
                //Set Music Disable
                //if (m_windows_DLLBuffer.Effect != 0x24)
                //    SetMusicMode(status.disable);
                Utility.Log(String.Format("[Trace]GRGBDLL_Capture4ZONEBuffer, effect ={0}, brightness={1}, tempo={2}, direction={3}"
                    , m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bEffectType, m_windows_FOURZONE_DLLBuffer._TSLedType_08H.blight,
                    m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bSpeed, m_windows_FOURZONE_DLLBuffer._TSLedType_08H.bdirection));
                NativeMethod.GRGBDLL_Capture4ZONEBuffer(ref m_windows_FOURZONE_DLLBuffer);
                Utility.Log("[Trace]GRGBDLL_Capture4ZONEBuffer finished");
                //Set Music Enable
                //if (m_windows_DLLBuffer.Effect == 0x24)
                //    SetMusicMode(status.enable);
            }
            Utility.Log("[Trace]SetWindowsEffect finished");


        }
        private void SetWelcomeEffect()
        {
            Console.WriteLine("SetWelcomEffect");
            Utility.Log("[Trace]SetWelcomeEffect start");
            TranslateDarfonWelcomeRGB();
            if(m_devie_type == Type_RGBKY_DLL.FULL_ME_KB)
            {
                try
                {
                    m_welcome_DLLBuffer.Mode = 1; // Hans's spec,  0= windows, 1 = Welcome
                    m_welcome_DLLBuffer.Effect = TranslateDarfonEffect();
                    m_welcome_DLLBuffer.Brightnesslevel = TranslateDarfonLightlevel();
                    m_welcome_DLLBuffer.Tempolevel = TranslateDarfonTempolevel();
                    m_welcome_DLLBuffer.Direction = TranslateDarfonDirection();
                    m_welcome_DLLBuffer.NVsaving = true;
                    // NativeMethod.GRGBDLL_CaptureCMDBuffer(ref m_DLLBuffer);

                    //check Color index
                    uint index = (m_welcome_DLLBuffer.bSingleDisplay) ? (uint)0x09 : (uint)0x08;
                    uint Checksum = 0;
                    uint[] data = new uint[7] { 0x08, 0x03, (uint)m_welcome_DLLBuffer.Effect, (uint)m_welcome_DLLBuffer.Tempolevel, (uint)m_welcome_DLLBuffer.Brightnesslevel, index, (uint)m_welcome_DLLBuffer.Direction };
                    Checksum = DFKeyboardCS(data, 7);
                    string[] av = new string[] { "OemServiceWinApp.exe", "ledkb", "/setdata", "0x08", "0x03", m_welcome_DLLBuffer.Effect.ToString(),
                    m_welcome_DLLBuffer.Tempolevel.ToString(), m_welcome_DLLBuffer.Brightnesslevel.ToString(),index.ToString(), m_welcome_DLLBuffer.Direction.ToString(),
                    Checksum.ToString() };
                    IntPtr buffer = Marshal.AllocHGlobal(256);
#if DEMO
#else
                    Utility.Log("[Trace]OemSvcHook start");
                    NativeMethod.OemSvcHook(11, av, buffer, System.Runtime.InteropServices.Marshal.SizeOf(buffer));
                    Utility.Log("[Trace]OemSvcHook end");
#endif
                    Marshal.FreeHGlobal(buffer);

                    NativeKBRGB[] welColr = new NativeKBRGB[16];
                    welColr[0].ID = 0; welColr[0].R = 0; welColr[0].G = 0; welColr[0].B = 0;
                    for (int i = 1; i < 8; i++)
                    {
                        welColr[i].ID = m_welcome_DLLBuffer.userRGB[8 + i].ID;
                        welColr[i].R = m_welcome_DLLBuffer.userRGB[8 + i].R;
                        welColr[i].G = m_welcome_DLLBuffer.userRGB[8 + i].G;
                        welColr[i].B = m_welcome_DLLBuffer.userRGB[8 + i].B;
                    }
                    for (int i = 9; i < 16; i++)
                    {
                        welColr[i].ID = 0; welColr[i].R = 0; welColr[i].G = 0; welColr[i].B = 0;
                    }
                    Utility.Log("[Trace]GRGBDLL_iSetWelColor_14H start");
                    NativeMethod.GRGBDLL_iSetWelColor_14H(welColr);  // welcoomecolor index + windows effect(for keep color status)
                    Utility.Log("[Trace]GRGBDLL_iSetWelColor_14H end");

                }
                catch(Exception e)
                {
                    Console.WriteLine("Set OemService Failed");
                    Utility.Log(String.Format("[Trace]SetWelcomeEffect set oemserve failed {0}", e.ToString()));
                }
            }
            else if( m_devie_type == Type_RGBKY_DLL.FOURZONE_ME_KB)
            {
                try
                {
                    m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bEffectType = TranslateDarfonEffect();
                    m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.blight = TranslateDarfonLightlevel();
                    m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bSpeed = TranslateDarfonTempolevel();
                    m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bdirection = TranslateDarfonDirection();
                    m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bsavingMode = true;
                    // NativeMethod.GRGBDLL_CaptureCMDBuffer(ref m_DLLBuffer);


                    uint Checksum = 0;
                    uint[] data = new uint[7] { 0x08, 0x03,
                        (uint)m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bEffectType,
                        (uint)m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bSpeed,
                        (uint)m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.blight,
                        (uint)m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bColor,
                        (uint)m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bdirection };
                    Checksum = DFKeyboardCS(data, 7);
                    string[] av = new string[] { "OemServiceWinApp.exe", "ledkb", "/setdata", "0x08", "0x03",
                        m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bEffectType.ToString(),
                        m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bSpeed.ToString(),
                        m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.blight.ToString(),
                        m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bColor.ToString(),
                        m_welcome_FOURZONE_DLLBuffer._TSLedType_08H.bdirection.ToString(),
                        Checksum.ToString() };
                    IntPtr buffer = Marshal.AllocHGlobal(256);
#if DEMO
#else
                    Utility.Log("[Trace]OemSvcHook start");
                    NativeMethod.OemSvcHook(11, av, buffer, System.Runtime.InteropServices.Marshal.SizeOf(buffer));
                    Utility.Log("[Trace]OemSvcHook end");
#endif
                    Marshal.FreeHGlobal(buffer);

                    NativeKBRGB[] welColr = new NativeKBRGB[16];
                    welColr[0].ID = 0; welColr[0].R = 0; welColr[0].G = 0; welColr[0].B = 0;
                    for (int i = 1; i < 8; i++)
                    {
                        welColr[i].ID = m_welcome_FOURZONE_DLLBuffer.userRGB[8 + i].ID;
                        welColr[i].R = m_welcome_FOURZONE_DLLBuffer.userRGB[8 + i].R;
                        welColr[i].G = m_welcome_FOURZONE_DLLBuffer.userRGB[8 + i].G;
                        welColr[i].B = m_welcome_FOURZONE_DLLBuffer.userRGB[8 + i].B;
                    }
                    for (int i = 9; i < 16; i++)
                    {
                        welColr[i].ID = 0; welColr[i].R = 0; welColr[i].G = 0; welColr[i].B = 0;
                    }
                    Utility.Log("[Trace]GRGBDLL_iSetWelColor_14H start");
                    NativeMethod.GRGBDLL_iSetWelColor_14H(welColr);  // welcoomecolor index + windows effect(for keep color status)
                    Utility.Log("[Trace]GRGBDLL_iSetWelColor_14H end");

                }
                catch(Exception e)
                {
                    Console.WriteLine("Set OemService Failed");
                    Utility.Log(String.Format("[Trace]SetWelcomeEffect set oemserve failed {0}",e.ToString()));
                }

            }

            Utility.Log("[Trace]SetWelcomeEffect End");
        }

        private void SetWelcomePower(bool PowerStatus)
        {
            byte bPowerStatus = (PowerStatus )? (byte)0x01 : (byte)0x00;
            byte bTimer = (PowerStatus) ? (byte)0x14 : (byte)0x00;
            uint[] data = new uint[7] { 0x1a, 0x05, bPowerStatus, bTimer, 0x00, 0x00, 0x00 };
            uint Checksum = DFKeyboardCS(data, 7);
            string[] av = new string[] { "OemServiceWinApp.exe", "ledkb", "/setdata", "0x1a", "0x05", bPowerStatus.ToString(), bTimer.ToString(), "0x00", "0x00", "0x00",Checksum.ToString() };
            IntPtr buffer = Marshal.AllocHGlobal(128);
#if DEMO
#else
            Utility.Log("[Trace]OemSvcHook start");
            NativeMethod.OemSvcHook(11, av, buffer, System.Runtime.InteropServices.Marshal.SizeOf(buffer));
            Utility.Log("[Trace]OemSvcHook end");
#endif
            Marshal.FreeHGlobal(buffer);
        }
        private bool GetOemServicePowerStatus()
        {
#if DEMO
            return status.on;
#endif
            // LEDKB status from OemService
            // 08H           ==> 08 03 effect tempo brightness colorIndex direction CS    ==> 3x7 +2 = 23byte
            // 14H(not used) ==> 14 00 Index R G B 00 CS 0 <== ???　Why add 1 byte ??     ==> 3x8 +2 = 26byte
            // 1AH           ==> 1A 05 on/off 00 00 00 00 CS                              ==> 3x7 +2 = 23byte
            try
            {
                string[] av = new string[] { "OemServiceWinApp.exe", "LEDKB", "/GetStatus" };
                IntPtr buffer = Marshal.AllocHGlobal(128);

                Utility.Log("[Trace]GetOemServicePowerStatus OemSvcHook start");
                NativeMethod.OemSvcHook(3, av, buffer, 128);
                Utility.Log("[Trace]GetOemServicePowerStatus OemSvcHook end");
                byte[] managedData = new byte[128];
                Marshal.Copy(buffer, managedData, 0, managedData.Length);
                Marshal.FreeHGlobal(buffer);

                return (managedData[56] == 0x30) ? status.off : status.on;  // 0 = off, 1 = on

            }
            catch
            {
                Console.WriteLine("Get OemService Failed");
                return  status.on;
            }
        }

        private byte GetOemServiceEffectStatus()
        {

#if DEMO
            return 0x0A;  // Raindrop effect
#endif
            // LEDKB status from OemService
            // 08H           ==> 08 03 effect tempo brightness colorIndex direction CS    ==> 3x7 +2 = 23byte
            // 14H(not used) ==> 14 00 Index R G B 00 CS 0 <== ???　Why add 1 byte ??     ==> 3x8 +2 = 26byte
            // 1AH           ==> 1A 05 on/off 00 00 00 00 CS                              ==> 3x7 +2 = 23byte
            try
            {
                string[] av = new string[] { "OemServiceWinApp.exe", "LEDKB", "/GetStatus" };
                IntPtr buffer = Marshal.AllocHGlobal(128);
                Utility.Log("[Trace]GetOemServiceEffectStatus OemSvcHook start");
                NativeMethod.OemSvcHook(3, av, buffer, 128);
                Utility.Log("[Trace]GetOemServiceEffectStatus OemSvcHook end");
                byte[] managedData = new byte[128];
                Marshal.Copy(buffer, managedData, 0, managedData.Length);
                Marshal.FreeHGlobal(buffer);

                int hInt = 0;
                int lInt = 6;
                if ((managedData[6] >= 48) && (managedData[6] <= 57))
                {
                    hInt = managedData[6] - 48;
                }
                else if ((managedData[6] >= 65) && (managedData[6] <= 70))
                {
                    hInt = managedData[6] - 65 + 10;
                }
                else if ((managedData[6] >= 97) && (managedData[6] <= 102))
                {
                    hInt = managedData[6] - 97 + 10;
                }
                else
                {
                    hInt = 0;
                }

                if ((managedData[7] >= 48) && (managedData[7] <= 57))
                {
                    lInt = managedData[7] - 48;
                }
                else if ((managedData[7] >= 65) && (managedData[7] <= 70))
                {
                    lInt = managedData[7] - 65 + 10;
                }
                else if ((managedData[7] >= 97) && (managedData[7] <= 102))
                {
                    lInt = managedData[7] - 97 + 10;
                }
                else
                {
                    lInt = 3;
                }
                return (byte)(16 * hInt + lInt);


            }
            catch
            {
                Console.WriteLine("Get OemService Failed");
                return 0x0A;  // Raindrop effect
            }
        }


        private void Brightness_Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            byte level = 0;

            if (Brightness_Offset.Value < 10)
            {
                level = 0;
            }
            else if (Brightness_Offset.Value < 20)
            {
                level = 1;
            }
            else if (Brightness_Offset.Value < 30)
            {
                level = 2;
            }
            else if (Brightness_Offset.Value < 40)
            {
                level = 3;
            }
            else if (Brightness_Offset.Value <= 50)
            {
                level = 4;
            }
            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.brightness_level = level;
                SetBrightnessLayout(level, false);
            }
            e.Handled = true;
        }

        private void Speed_bar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            byte level = 0;
            if (Speed_Offset.Value < 10)
            {
                level = 0;
            }
            else if (Speed_Offset.Value < 20)
            {
                level = 1;
            }
            else if (Speed_Offset.Value < 30)
            {
                level = 2;
            }
            else if (Speed_Offset.Value < 40)
            {
                level = 3;
            }
            else if (Speed_Offset.Value < 50)
            {
                level = 4;
            }
            else if (Speed_Offset.Value < 60)
            {
                level = 5;
            }
            else if (Speed_Offset.Value < 70)
            {
                level = 6;
            }
            else if (Speed_Offset.Value < 80)
            {
                level = 7;
            }
            else if (Speed_Offset.Value < 90)
            {
                level = 8;
            }
            else if (Speed_Offset.Value <= 100)
            {
                level = 9;
            }
            if (m_mode == (uint)mode.windows)
            {
                m_windows_layoutstatus.tempo_level = level;
                SetTempoLayout(level, true);
            }
            else if (m_mode == (uint)mode.welcome)
            {
                m_welcome_layoutstatus.tempo_level = level;
                SetTempoLayout(level, false);
            }

            m_isTempoBall_MouseDown = false;
            e.Handled = true;
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        Setting setting = new Setting();
        private void BTN_settings_Click(object sender, RoutedEventArgs e)
        {
            setting.Owner = this;
            setting.Show();
        }
    }
}
