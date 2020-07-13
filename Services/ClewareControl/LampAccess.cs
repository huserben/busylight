using System;
using System.Runtime.InteropServices;

namespace busylight.Services.ClewareControl
{
   public class LampAccess
   {
      // Options are:
      // 32 Bit: USBaccess.dll
      // 64 Bit: USBaccessx64.dll
      private const string UsbAccess = @"Services\ClewareControl\USBaccessx64.dll";

      public enum SWITCH_IDs : int
      {
         SWITCH_0 = 0x10, SWITCH_1 = 0x11, SWITCH_2 = 0x12, SWITCH_3 = 0x13,
         SWITCH_4 = 0x14, SWITCH_5 = 0x15, SWITCH_6 = 0x16, SWITCH_7 = 0x17,
         SWITCH_8 = 0x18, SWITCH_9 = 0x19, SWITCH_10 = 0x1a, SWITCH_11 = 0x1b,
         SWITCH_12 = 0x1c, SWITCH_13 = 0x1d, SWITCH_14 = 0x1e, SWITCH_15 = 0x1f
      };
      public enum USBtype_enum : int
      {
         ILLEGAL_DEVICE = 0,
         LED_DEVICE = 0x01,
         POWER_DEVICE = 0x02,
         DISPLAY_DEVICE = 0x03,
         WATCHDOG_DEVICE = 0x05,
         AUTORESET_DEVICE = 0x06,
         WATCHDOGXP_DEVICE = 0x07,
         SWITCH1_DEVICE = 0x08,
         SWITCH2_DEVICE = 0x09, SWITCH3_DEVICE = 0x0a, SWITCH4_DEVICE = 0x0b,
         SWITCH5_DEVICE = 0x0c, SWITCH6_DEVICE = 0x0d, SWITCH7_DEVICE = 0x0e, SWITCH8_DEVICE = 0x0f,
         TEMPERATURE_DEVICE = 0x10,
         TEMPERATURE2_DEVICE = 0x11,
         TEMPERATURE5_DEVICE = 0x15,
         HUMIDITY1_DEVICE = 0x20, HUMIDITY2_DEVICE = 0x21,
         SWITCHX_DEVICE = 0x28,     // new switch 3,4,8
         CONTACT00_DEVICE = 0x30, CONTACT01_DEVICE = 0x31, CONTACT02_DEVICE = 0x32, CONTACT03_DEVICE = 0x33,
         CONTACT04_DEVICE = 0x34, CONTACT05_DEVICE = 0x35, CONTACT06_DEVICE = 0x36, CONTACT07_DEVICE = 0x37,
         CONTACT08_DEVICE = 0x38, CONTACT09_DEVICE = 0x39, CONTACT10_DEVICE = 0x3a, CONTACT11_DEVICE = 0x3b,
         CONTACT12_DEVICE = 0x3c, CONTACT13_DEVICE = 0x3d, CONTACT14_DEVICE = 0x3e, CONTACT15_DEVICE = 0x3f,
         F4_DEVICE = 0x40,
         KEYC01_DEVICE = 0x41, KEYC16_DEVICE = 0x42, MOUSE_DEVICE = 0x43,
         ADC0800_DEVICE = 0x50, ADC0801_DEVICE = 0x51, ADC0802_DEVICE = 0x52, ADC0803_DEVICE = 0x53,
         COUNTER00_DEVICE = 0x60, COUNTER01_DEVICE = 0x61, COUNTER02_DEVICE = 0x62,
         CONTACTTIMER00_DEVICE = 0x70, CONTACTTIMER01_DEVICE = 0x71, CONTACTTIMER02_DEVICE = 0x72,
         ENCODER01_DEVICE = 0x80,
         BUTTON_NODEVICE = 0x1000
      };


      [DllImport(UsbAccess)]
      public static extern IntPtr FCWInitObject();
      [DllImport(UsbAccess)]
      public static extern void FCWUnInitObject(IntPtr cwHdl);
      [DllImport(UsbAccess)]
      public static extern int FCWOpenCleware(IntPtr cwHdl);
      [DllImport(UsbAccess)]
      public static extern int FCWCloseCleware(IntPtr cwHdl);
      [DllImport(UsbAccess)]
      public static extern int FCWGetUSBType(IntPtr cwHdl, int devNum);
      [DllImport(UsbAccess)]
      public static extern float FCWDGetTemperature(IntPtr cwHdl, int devNum);
      [DllImport(UsbAccess)]
      public static extern int FCWSetSwitch(IntPtr cwHdl, int devNum, int Switch, int On);	//	On: 0=off, 1=on
      [DllImport(UsbAccess)]
      public static extern int FCWGetSwitch(IntPtr cwHdl, int devNum, int Switch);
   }
}
