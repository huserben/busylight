import React from 'react';
import './App.css';

import { TrafficLightModel } from './model/TrafficLight';
import TrafficLightContainer from './Components/TrafficLightContainer';

import DataService from './Services/DataService';

const App = () => {
  var [trafficLights, setTrafficLights]: [TrafficLightModel[], any] = React.useState([]);
  const [loaded, setLoaded]: [boolean, any] = React.useState(false);

  const fetchTrafficLights = async () => {
    var data = await DataService.getAvailableTrafficLights();
    setTrafficLights(data);
  };

  React.useEffect(() => {
    if (!loaded) {
      setLoaded(true);
      fetchTrafficLights();
    }
  });

  return (
    <div>
      {trafficLights.map((trafficLight: TrafficLightModel) => <TrafficLightContainer key={trafficLight.id} trafficLightId={trafficLight.id} />)}
    </div>
  );
};

export default App;
