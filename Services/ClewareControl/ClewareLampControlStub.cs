using busylight.Model;
using System;
using System.Collections.Generic;

namespace busylight.Services.ClewareControl
{
    public class ClewareLampControlStub : IClewareLampControl
    {
        private readonly TrafficLight trafficLight;

        public ClewareLampControlStub()
        {
            trafficLight =
                new TrafficLight(0, new[]
                {
                    new Lamp(0, LampColor.Red),
                    new Lamp(1, LampColor.Orange),
                    new Lamp(2, LampColor.Green),
                });
        }

        public IEnumerable<TrafficLight> DiscoverConnectedDevices()
        {
            return new[]{ trafficLight };
        }

        public void Dispose()
        {
        }

        public void SwitchLight(int deviceId, int lampId, bool isOn)
        {
        }
    }
}
