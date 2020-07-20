using busylight.Model;
using System.Collections.Generic;

namespace busylight.Services
{
   public interface ITrafficLightRepository
   {
      IEnumerable<TrafficLight> GetTrafficLights();

      TrafficLight GetTrafficLightById(int id);
   }
}