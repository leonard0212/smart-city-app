
import cv2
print("OpenCV version:", cv2.__version__)
print("OpenCV module location:", cv2.__file__)

#print(cv2.getBuildInformation())  # “GStreamer” in Video I/O

# Open the camera
cap = cv2.VideoCapture(0)  # Or the path to a video file

if not cap.isOpened():
    print("The camera cannot be accessed!")
    exit()

while True:
    ret, frame = cap.read()

    if not ret:
        print("It's not working!")
        break

    cv2.imshow("Live Video", frame)

    # 'q' to exit
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
