import React from 'react';
import * as signalR from "@microsoft/signalr";

import TrafficLight from 'react-trafficlight';
import { Lamp } from '../model/Lamp';

import DataService from '../Services/DataService';

interface TrafficLightContainerProps {
  trafficLightId: number;
}

const TrafficLightContainer = (props: TrafficLightContainerProps) => {
  const initialData: { [id: string]: Lamp } = {};
  initialData['Red'] = { id: 0, color: 'Red', isOn: false };
  initialData['Orange'] = { id: 1, color: 'Orange', isOn: false };
  initialData['Green'] = { id: 2, color: 'Green', isOn: false };

  const [lampStates, setLampStates]: [{ [id: string]: Lamp }, any] = React.useState(initialData);
  const [loaded, setLoaded]: [boolean, any] = React.useState(false);

  const setupSignalRConnection = async () => {
    const connection: signalR.HubConnection = await DataService.connectToHub(props.trafficLightId);
    connection.on("trafficLightUpdate", async (id: number) => {
      if (id === props.trafficLightId) {
        await updateLightStatus();
      }
    });
  }

  const updateLightStatus = async () => {
    const lamps = await DataService.getLampsOfTrafficLight(props.trafficLightId);

    var states: { [id: string]: Lamp } = {}
    lamps.forEach((lamp: Lamp) => {
      states[lamp.color] = lamp;
    });

    setLampStates(states);
  };

  React.useEffect(() => {
    if (!loaded) {
      setLoaded(true);
      updateLightStatus();

      setupSignalRConnection();
    }
  });

  return (
    <div>

      <TrafficLight
        RedOn={lampStates['Red'].isOn}
        onRedClick={() => DataService.changeLightStatus(props.trafficLightId, lampStates['Red'])}
        YellowOn={lampStates['Orange'].isOn}
        onYellowClick={() => DataService.changeLightStatus(props.trafficLightId, lampStates['Orange'])}
        GreenOn={lampStates['Green'].isOn}
        onGreenClick={() => DataService.changeLightStatus(props.trafficLightId, lampStates['Green'])}
      />

      <div>
        {lampStates['Red'].isOn ? <h1 style={{ background: 'red' }}>Camera On</h1> : <div />}
        {lampStates['Orange'].isOn ? <h1 style={{ background: 'orange' }}>Audio On</h1> : <div />}
        {lampStates['Green'].isOn ? <h1 style={{ background: 'green' }}>Work in Progress</h1> : <div />}
      </div>
    </div>
  );
};

export default TrafficLightContainer;