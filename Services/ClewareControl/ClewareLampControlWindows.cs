using busylight.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace busylight.Services.ClewareControl
{
   public class ClewareLampControlWindows : IClewareLampControl
   {
      private readonly int deviceCount;
      private readonly IntPtr clewareObject;
      private readonly ILogger<ClewareLampControlWindows> logger;
      private Dictionary<int, LampAccess.SWITCH_IDs> idToSwitchIDMap = new Dictionary<int, LampAccess.SWITCH_IDs>();

      public ClewareLampControlWindows(ILogger<ClewareLampControlWindows> logger)
      {
         // Please note that OpenCleware should be called only once in the initialisation of your programm, 
         // not every time a cleware function is called
         clewareObject = LampAccess.FCWInitObject();
         deviceCount = LampAccess.FCWOpenCleware(clewareObject);

         idToSwitchIDMap.Add(0, LampAccess.SWITCH_IDs.SWITCH_0);
         idToSwitchIDMap.Add(1, LampAccess.SWITCH_IDs.SWITCH_1);
         idToSwitchIDMap.Add(2, LampAccess.SWITCH_IDs.SWITCH_2);

         this.logger = logger;
      }

      public IEnumerable<TrafficLight> DiscoverConnectedDevices()
      {
         var trafficLights = new List<TrafficLight>();

         for (var i = 0; i < deviceCount; i++)
         {
            var devType = LampAccess.FCWGetUSBType(clewareObject, i);
            logger.LogDebug("Device " + i + ": Type = " + devType);
            if (devType == (int)LampAccess.USBtype_enum.SWITCH1_DEVICE)
            {
               var state = LampAccess.FCWGetSwitch(clewareObject, i, (int)LampAccess.SWITCH_IDs.SWITCH_0);
               logger.LogDebug("Switch state is = " + state);
               LampAccess.FCWSetSwitch(clewareObject, i, (int)LampAccess.SWITCH_IDs.SWITCH_0, 1);
               System.Threading.Thread.Sleep(1000);     // wait one second
               LampAccess.FCWSetSwitch(clewareObject, i, (int)LampAccess.SWITCH_IDs.SWITCH_0, 0);

               LampAccess.FCWSetSwitch(clewareObject, i, (int)LampAccess.SWITCH_IDs.SWITCH_1, 0);
               LampAccess.FCWSetSwitch(clewareObject, i, (int)LampAccess.SWITCH_IDs.SWITCH_2, 0);

               var redLight = new Lamp(0, LampColor.Red);
               var orangeLight = new Lamp(1, LampColor.Orange);
               var greenLight = new Lamp(2, LampColor.Green);

               var trafficLight = new TrafficLight(i, new[] {greenLight, orangeLight, redLight});
               trafficLights.Add(trafficLight);
            }
            else
            {
               logger.LogDebug("Device is not switching device - skipping");
            }
         }

         return trafficLights;
      }

      public void Dispose()
      {
         if (deviceCount > 0)
         {
            LampAccess.FCWCloseCleware(clewareObject);
         }

         LampAccess.FCWUnInitObject(clewareObject);   // free the allocated object
      }

      public void SwitchLight(int deviceId, int lampId, bool isOn)
      {
         var switchId = idToSwitchIDMap[lampId];
         var lampState = isOn ? 1 : 0;

         SwitchLight(deviceId, lampState, switchId);
      }

      public void TurnOffLights(int deviceNr)
      {
         SwitchAllLights(deviceNr, 0);
      }

      public void TurnOnAllLights(int deviceNr)
      {
         SwitchAllLights(deviceNr, 1);
      }

      private void SwitchAllLights(int deviceNr, int state)
      {
         SwitchLight(deviceNr, state, LampAccess.SWITCH_IDs.SWITCH_0);
         System.Threading.Thread.Sleep(500);
         SwitchLight(deviceNr, state, LampAccess.SWITCH_IDs.SWITCH_1);
         System.Threading.Thread.Sleep(500);
         SwitchLight(deviceNr, state, LampAccess.SWITCH_IDs.SWITCH_2);
      }

      private void SwitchLight(int deviceNr, int state, LampAccess.SWITCH_IDs switchID)
      {
         LampAccess.FCWSetSwitch(clewareObject, deviceNr, (int)switchID, state);
      }
   }
}
