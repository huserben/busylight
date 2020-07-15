import cv2
import time

while True:
    stream = cv2.VideoCapture(0)

    try:
        r, f = stream.read()

        if r == True:
            print("Camera is not in use!")
        else:
            print("Camera is in use!")

    except:
        print("Failed")

    finally:
        stream.release()
        cv2.destroyAllWindows()

    time.sleep(60)
