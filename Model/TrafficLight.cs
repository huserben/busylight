using System.Collections.Generic;

namespace busylight.Model
{
   public class TrafficLight
   {
      public TrafficLight(int id, IEnumerable<Lamp> lamps)
      {
         ID = id;
         Lamps = lamps;
      }

      public int ID { get; }

      public IEnumerable<Lamp> Lamps { get; }

      public void TurnOff()
      {
         foreach (var lamp in Lamps)
         {
            lamp.SetLampState(false);
         }
      }
   }
}
