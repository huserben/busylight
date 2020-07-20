using System;
using System.Collections.Generic;
using busylight.Model;

namespace busylight.Services.ClewareControl
{
   public interface IClewareLampControl : IDisposable
   {
      /// <summary>
      /// Prints all the available devices found.
      /// </summary>
      IEnumerable<TrafficLight> DiscoverConnectedDevices();

      void SwitchLight(int deviceId, int lampId, bool isOn);
   }
}
