using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using busylight.Hubs;
using busylight.Model;
using busylight.Services;
using busylight.Services.ClewareControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace busylight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficLightController : ControllerBase
    {
        private readonly ILogger<TrafficLightController> logger;
        private readonly ITrafficLightRepository trafficLightRepository;
        private readonly IClewareLampControl clewareLampControl;
        private readonly IHubContext<TrafficLightHub> trafficLightHub;

        public TrafficLightController(
           ILogger<TrafficLightController> logger,
           ITrafficLightRepository trafficLightRepository,
           IClewareLampControl clewareLampControl,
           IHubContext<TrafficLightHub> trafficLightHub)
        {
            this.logger = logger;
            this.trafficLightRepository = trafficLightRepository;
            this.clewareLampControl = clewareLampControl;
            this.trafficLightHub = trafficLightHub;
        }

        // GET: api/<TrafficLightController>
        [HttpGet]
        public IEnumerable<TrafficLight> Get()
        {
            logger.LogDebug("Getting traffic light");
            return trafficLightRepository.GetTrafficLights();
        }

        // GET api/<TrafficLightController>/5
        [HttpGet("{id}")]
        public TrafficLight Get(int id)
        {
            return trafficLightRepository.GetTrafficLightById(id);
        }

        // GET api/<TrafficLightController>/5/lamps
        [HttpGet("{id}/lamps")]
        public IEnumerable<Lamp> GetLamps(int id)
        {
            return trafficLightRepository.GetTrafficLightById(id).Lamps;
        }

        // GET api/<TrafficLightController>/5/lamps/2
        [HttpGet("{id}/lamps/{lampId}")]
        public Lamp GetLamp(int id, int lampId)
        {
            return trafficLightRepository.GetTrafficLightById(id).Lamps.SingleOrDefault(l => l.ID == lampId);
        }

        // PUT api/<TrafficLightController>/5
        [HttpPut("{id}/lamps/{lampId}/{isOn}")]
        public async Task Put(int id, int lampId, bool isOn)
        {
            var lamp = GetLamp(id, lampId);

            if (lamp != null)
            {
                if (lamp.IsOn != isOn)
                {
                    await SetLampStateAsync(id, lampId, isOn, lamp);
                }
                else
                {
                   logger.LogInformation($"Lamp {lampId} is already in state {isOn} - skipping...");
                }
            }
            else
            {
                logger.LogInformation($"Failed to put lamp {lampId} into state {isOn} - Lamp could not be found");
            }
        }

        private async Task SetLampStateAsync(int id, int lampId, bool isOn, Lamp lamp)
        {
            lamp.SetLampState(isOn);
            var updateClients = trafficLightHub.Clients.All.SendAsync("trafficLightUpdate", id);

            clewareLampControl.SwitchLight(id, lampId, isOn);

            await updateClients;
        }
    }
}
