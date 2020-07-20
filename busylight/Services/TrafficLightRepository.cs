using busylight.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using busylight.Services.ClewareControl;

namespace busylight.Services
{
   public class TrafficLightRepository : ITrafficLightRepository
   {
      private readonly ILogger<TrafficLightRepository> logger;
      private readonly IEnumerable<TrafficLight> trafficeLights;

      public TrafficLightRepository(ILogger<TrafficLightRepository> logger, IClewareLampControl clewareLampControl)
      {
         this.logger = logger;
         logger.LogDebug("Created Traffic Light Repo");

         trafficeLights = clewareLampControl.DiscoverConnectedDevices();
      }

      public IEnumerable<TrafficLight> GetTrafficLights()
      {
         logger.LogDebug("Getting Traffic Lights");
         return trafficeLights;
      }

      public TrafficLight GetTrafficLightById(int id)
      {
         return GetTrafficLights().SingleOrDefault(x => x.ID == id);
      }
   }
}
