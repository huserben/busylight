namespace busylight.Model
{
   public class Lamp
   {
      public Lamp(int id, LampColor color)
      {
         ID = id;
         Color = color;
      }

      public int ID { get; }

      public LampColor Color { get; }

      public bool IsOn { get; private set; }

      public void SetLampState(bool isOn)
      {
         IsOn = isOn;
      }
   }
}
