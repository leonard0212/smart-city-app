import cv2
import os
from ultralytics import YOLO
from concurrent.futures import ThreadPoolExecutor
import ffmpeg
import json

# Check if the YOLOv11 model file exists
model_path = '../yolo11s.pt'


# Load the YOLOv11 model
object_model = YOLO(model_path)
plate_model=YOLO('../my_license_plate_detector.pt')

# Open the video file
video_path = '../input_video.mp4'  # Replace with your video file path
#video_path='d://Video//video3.mp4'
#url="http://192.168.2.14:8080/video"
cap = cv2.VideoCapture(video_path)

# Check if the video was opened successfully
if not cap.isOpened():
    print("Error: Could not open video.")
    exit()


score_threshold = 0.5   
tracker_config = "../bytetrack.yaml"

# Extract GPS metadata using ffmpeg
probe = ffmpeg.probe(video_path)
metadata = probe.get('format', {}).get('tags', {})


def extract_gps_coordinates():
    location = metadata.get('location', '')
    if location:
        try:
            lat = float(location.split('+')[1])
            lng = float(location.split('+')[2].replace('/', ''))
            return lat, lng
        except (IndexError, ValueError):
            return 'N/A', 'N/A'
    return 'N/A', 'N/A'

frame_number = 0

print(json.dumps(metadata, indent=4))


while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break
    
    frame_number = frame_number + 1

    if frame_number % 3 != 0:
        continue

   

    frame = cv2.resize(frame, (1920, 1080))

    gps_latitude, gps_longitude = extract_gps_coordinates()

    with ThreadPoolExecutor(max_workers=2) as executor:
        object_future = executor.submit(object_model.track, frame, persist=True,
                                        tracker=tracker_config, conf=score_threshold)

        object_results = object_future.result()[0]

    # Process object detections (1st thread - OBJ)
    for box in object_results.boxes:
        # Only process if the detected class is 'car' (class ID 2 in COCO dataset)
        if int(box.cls[0]) == 2:
            x1, y1, x2, y2 = map(int, box.xyxy[0])
            confidence = box.conf[0]

            if confidence < 0.5:
                continue

            track_id = int(box.id.cpu().item()) if box.id is not None else -1

            gps_latitude, gps_longitude = 0,0 #get_gps_for_frame(frame_number)

            # Draw bounding box
            cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)
            label = f'Car: {confidence:.2f} ID: {track_id} Lat: {gps_latitude} Lon: {gps_longitude}'
            cv2.putText(frame, label, (x1, y1 - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)

            # Crop the car region and detect license plate
           
            vehicle_frame = frame[y1:y2, x1:x2]

            plate_results = plate_model.track(vehicle_frame, persist=True, tracker=tracker_config,
                                                        conf=score_threshold)[0]
            for plate_box in plate_results.boxes:
                plate_id = int(
                        plate_box.id.cpu().item()) if plate_box.id is not None else 1
                px1, py1, px2, py2 = map(int, plate_box.xyxy[0].cpu().tolist())
                adjusted_polygon = [
                        (px1 + x1, py1 + y1),
                        (px2 + x1, py1 + y1),
                        (px2 + x1, py2 + y1),
                        (px1 + x1, py2 + y1),
                    ]
                
                cv2.rectangle(frame, (px1 + x1, py1 + y1), (px2 + x1, py2 + y1), (0, 0, 255), 2)


           
            # Display the cropped car region
           

    # Display the frame
    cv2.imshow('Car Detection', frame)

    # Press 'q' to exit
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

    

# Release the video capture object and close display window
cap.release()
cv2.destroyAllWindows()

