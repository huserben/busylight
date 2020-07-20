using busylight.Controllers;
using busylight.Hubs;
using busylight.Model;
using busylight.Services;
using busylight.Services.ClewareControl;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace busylight.Tests
{
    public class TrafficLightControllerTest
    {
        private readonly ITrafficLightRepository trafficLightRepo;
        private readonly ILogger<TrafficLightController> logger;
        private readonly IClewareLampControl clewareLampControl;
        private readonly TrafficLightController trafficLightController;
        private IClientProxy clientProxy;

        public TrafficLightControllerTest()
        {
            trafficLightRepo = Mock.Of<ITrafficLightRepository>();

            logger = Mock.Of<ILogger<TrafficLightController>>();
            clewareLampControl = Mock.Of<IClewareLampControl>();

            var hubContextMock = new Mock<IHubContext<TrafficLightHub>>();

            clientProxy = Mock.Of<IClientProxy>();
            hubContextMock.SetupGet(x => x.Clients.All).Returns(clientProxy);

            trafficLightController = new TrafficLightController(
                logger,
                trafficLightRepo,
                clewareLampControl,
                hubContextMock.Object);
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

        [Fact]
        public void GetLamps_GivenTrafficLightID_ReturnsLamps()
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            var redLamp = new Lamp(0, LampColor.Red);
            var lamps = new[] { greenLamp, redLamp };

            var trafficLight = new TrafficLight(0, lamps);
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            var actualLamps = trafficLightController.GetLamps(0);

            Assert.Equal(lamps, actualLamps);
        }

        [Fact]
        public void GetLamp_GivenTrafficLightAndLampID_ReturnsLamp()
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            var actualLamp = trafficLightController.GetLamp(0, 1);

            Assert.Equal(greenLamp, actualLamp);
        }

        [Fact]
        public void GetLamp_GivenNoMatchingLampID_ReturnsNull()
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            var actualLamp = trafficLightController.GetLamp(0, 12);

            Assert.Null(actualLamp);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoDifferentState_SetsLampStateToNewState(bool newLampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(!newLampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, newLampState);

            Assert.Equal(newLampState, greenLamp.IsOn);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoDifferentState_SwitchesLampState(bool newLampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(!newLampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, newLampState);

            Mock.Get(clewareLampControl).Verify(x => x.SwitchLight(0, 1, newLampState));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoDifferentState_NotifiesClients(bool newLampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(!newLampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, newLampState);

            Mock.Get(clientProxy).Verify(x => x.SendCoreAsync("trafficLightUpdate", new object[] { 0 }, It.IsAny<CancellationToken>()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoSameState_DoesNotChangeState(bool lampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(lampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, lampState);

            Assert.Equal(lampState, greenLamp.IsOn);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoSameState_DoesNotSwitchLights(bool lampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(lampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, lampState);

            Mock.Get(clewareLampControl).Verify(x => x.SwitchLight(0, 1, lampState), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoSameState_DoesNotNotifyClients(bool lampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(lampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, lampState);

            Mock.Get(clientProxy).Verify(x => x.SendCoreAsync("trafficLightUpdate", new object[] { 0 }, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GivenLampID_DoesNotFindLamp_Logs()
        {
            var trafficLight = new TrafficLight(0, Enumerable.Empty<Lamp>());
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 0, true);

            Mock.Get(logger).VerifyLog(Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenLamp_PutIntoSameState_LogsMessage(bool lampState)
        {
            var greenLamp = new Lamp(1, LampColor.Green);
            greenLamp.SetLampState(lampState);
            var trafficLight = new TrafficLight(0, new[] { greenLamp });
            Mock.Get(trafficLightRepo).Setup(x => x.GetTrafficLightById(0)).Returns(trafficLight);

            await trafficLightController.Put(0, 1, lampState);

            Mock.Get(logger).VerifyLog(Times.Once);
        }
    }
}
