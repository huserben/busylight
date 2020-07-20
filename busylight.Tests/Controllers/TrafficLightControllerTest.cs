using busylight.Controllers;
using busylight.Hubs;
using busylight.Model;
using busylight.Services;
using busylight.Services.ClewareControl;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace busylight.Tests
{
    public class TrafficLightControllerTest
    {
        private readonly ITrafficLightRepository trafficLightRepo;
        private readonly TrafficLightController trafficLightController;

        public TrafficLightControllerTest()
        {
            trafficLightRepo = Mock.Of<ITrafficLightRepository>();

            trafficLightController = new TrafficLightController(
                Mock.Of<ILogger<TrafficLightController>>(),
                trafficLightRepo,
                Mock.Of<IClewareLampControl>(),
                Mock.Of<IHubContext<TrafficLightHub>>());
        }

        [Fact]
        public void Get_ReturnsTrafficLightsFromRepo()
        {
            var expected = new[] { new TrafficLight(0, Enumerable.Empty<Lamp>()), new TrafficLight(1, Enumerable.Empty<Lamp>()) };

            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLights()).Returns(expected);

            var actual = trafficLightController.Get();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(12)]
        public void Get_WithId_ReturnsMatchingTrafficLight(int trafficLightId)
        {
            var trafficLight = new TrafficLight(trafficLightId, Enumerable.Empty<Lamp>());
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(trafficLightId)).Returns(trafficLight);

            var actual = trafficLightController.Get(trafficLightId);

            Assert.Equal(trafficLight, actual);
        }
    }
}
