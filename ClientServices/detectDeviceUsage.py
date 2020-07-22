# Microphone detection done via image recognition of microphone icon in sys tray
# Webcam Recognition done via trying to access it - if it's possible, no other application is using it

# Third Party Packages needed:
# pip3 install opencv-python python-imageseach-drov0 requests pyyaml

import cv2
import time
from python_imagesearch.imagesearch import imagesearch
import requests
import yaml
import sys, os

url = ''

pathname = os.path.dirname(sys.argv[0])        

with open("{0}/deviceUsageConfiguartion.yaml".format(os.path.abspath(pathname)), 'r') as stream:
    print('Reading configuration from yaml file...')

    try:
        configuration = yaml.safe_load(stream)['deviceConfiguration']
        url = configuration['endpoint']['url']

        print("Endpoint Url: {0}".format(url))

        traffic_light_id = configuration['endpoint']['trafficLightId']
        print("Traffic Light Id: {0}".format(traffic_light_id))

        imagePath = configuration['imageRecognition']['image']

        print("Image Path: {0}".format(imagePath))

        interval_between_checks = configuration['intervalBetweenChecks']
        print("Will wait {0} seconds between the camera and microphone checks".format(interval_between_checks))

        cycle_interval = configuration['intervalBetweenCycles']
        print("Will wait {0} seconds till checks rerun again".format(cycle_interval))

    except yaml.YAMLError as exc:
        print(exc)

print("Checking if endpoint is reachable...")
try:
    response = requests.get(url)
    if response.status_code == 200:
        print("Could access endpoint - continue")
    else:
        print(response.status_code)
        print("Could not reach endpoint, please check that it's online and accessible from this machine...")
        sys.exit()
except:
    print("Could not reach endpoint, please check that it's online and accessible from this machine...")
    sys.exit()

api_url = "{0}/api/trafficlight/{1}".format(url, traffic_light_id)
print("API Url: {0}".format(api_url))

while True:
    camera_on = False
    microphone_on = False

    print("Checking if camera is in use...")
    stream = cv2.VideoCapture(0)

    try:
        r, f = stream.read()
        camera_on = r == False

        if camera_on == True:
            print("Camera is in use!")
        else:
            print("Camera is not in use")

    except:
        print("Failed")

    finally:
        stream.release()
        cv2.destroyAllWindows()

    response = requests.put("{0}/lamps/{1}/{2}".format(api_url, 0, camera_on))    
    time.sleep(interval_between_checks)


    print("Checking if microphone is in use...")
    pos = imagesearch("{0}/{1}".format(os.path.abspath(pathname), imagePath))
    microphone_on = pos[0] != -1
    if microphone_on:
        print("Microphone is in use")
    else:
        print("Microphone is not in use")

    response = requests.put("{0}/lamps/{1}/{2}".format(api_url, 1, microphone_on))
    time.sleep(interval_between_checks)

    if (microphone_on or camera_on):
        response = requests.put("{0}/lamps/{1}/{2}".format(api_url, 2, False))
    else:
        response = requests.put("{0}/lamps/{1}/{2}".format(api_url, 2, True))
    
    print("Cycle completed, waiting {0} seconds till starting the next one...".format(cycle_interval))
    time.sleep(cycle_interval)
