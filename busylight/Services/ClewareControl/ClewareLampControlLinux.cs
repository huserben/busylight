using busylight.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace busylight.Services.ClewareControl
{
   public class ClewareLampControlLinux : IClewareLampControl
   {
      private readonly ILogger<ClewareLampControlLinux> logger;

      public ClewareLampControlLinux(ILogger<ClewareLampControlLinux> logger)
      {
         this.logger = logger;

         // It's required to have "cleware-control" installed
         // See https://github.com/flok99/clewarecontrol and also https://blog.codecentric.de/en/2013/07/using-a-raspberry-pi-to-control-an-extreme-feedback-devices/)
      }

      public IEnumerable<TrafficLight> DiscoverConnectedDevices()
      {
         var trafficLights = new List<TrafficLight>();

         logger.LogInformation("Discovering connected devices...");

         var discoverDeviceCommand = "clewarecontrol -l";
         var devices = discoverDeviceCommand.Bash();

         foreach (var line in devices.Split(Environment.NewLine))
         {
            if (line.StartsWith("Device:"))
            {
               logger.LogInformation($"Found Device: {line}");
               var deviceId = int.Parse(line.Split(',').Last().Split(':').Last().Trim());

               logger.LogInformation($"Device ID Extracted: {deviceId}");

               var redLight = new Lamp(0, LampColor.Red);
               var orangeLight = new Lamp(1, LampColor.Orange);
               var greenLight = new Lamp(2, LampColor.Green);

               var trafficLight = new TrafficLight(deviceId, new[] { greenLight, orangeLight, redLight });
               trafficLights.Add(trafficLight);
            }
         }

         return trafficLights;
      }

      public void Dispose()
      {
      }

      public void SwitchLight(int deviceId, int lampId, bool isOn)
      {
         var lampState = isOn ? 1 : 0;

         logger.LogInformation($"Setting lamp state of lamp {lampId} (device {deviceId}) to {lampState}");

         var command = $"clewarecontrol -d {deviceId} -as {lampId} {lampState}";

         logger.LogDebug(command);

         command.Bash();
      }
   }
}
