import { TrafficLightModel } from "../model/TrafficLight";
import { Lamp } from "../model/Lamp";

import { Utils } from './Utils';

import * as signalR from "@microsoft/signalr";

class DataService {
    baseUrl: string = Utils.getAbsoluteDomainUrl();

    openConnections: { [id: number]: signalR.HubConnection } = {}

    private static instance: DataService;

    private constructor() {
        console.log(`Detected following base url: ${this.baseUrl}`);
    }

    public static getInstance(): DataService {
        if (!DataService.instance) {
            DataService.instance = new DataService();
        }

        return DataService.instance;
    }

    async getAvailableTrafficLights(): Promise<TrafficLightModel[]> {
        const url: string = `${this.baseUrl}/api/trafficlight`;
        console.log(`Getting avialable traffic lights from ${url}`)

        const response = await fetch(`${url}`);
        const data = await response.json();

        return data;
    };

    async getLampsOfTrafficLight(trafficLightId: number): Promise<Lamp[]> {
        const url: string = `${this.baseUrl}/api/trafficlight/${trafficLightId}/lamps`;
        console.log(`Getting lamps of traffic light ${trafficLightId} from ${url}`)

        const response = await fetch(`${url}`);
        const data = await response.json();

        return data;
    }


    async changeLightStatus(trafficLightId: number, lamp: Lamp): Promise<void> {
        const newState: string = lamp.isOn ? 'false' : 'true';

        const url: string = `${this.baseUrl}/api/trafficlight/${trafficLightId}/lamps/${lamp.id}/${newState}`;

        console.log(`Changing light status of lamp ${lamp.id} for traffic light ${trafficLightId} via ${url}`)

        await fetch(`${url}`, { method: 'PUT' });
    };

    async connectToHub(trafficLightId: number): Promise<signalR.HubConnection> {
        if (this.openConnections[trafficLightId] !== undefined) {
            await this.disconnectFromHub(trafficLightId);
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/hub")
            .build();

        await connection.start().catch(err => console.log(err));

        this.openConnections[trafficLightId] = connection;
        return connection;
    }

    async disconnectFromHub(trafficLightId: number): Promise<void> {
        await this.openConnections[trafficLightId].stop();
        delete this.openConnections[trafficLightId];
    }
};

let service: DataService = DataService.getInstance();
export default service;