import cv2
import requests
import numpy as np

#Preview-RES IMAGE URL (from ImageUrlFull)
url = "https://***.blob.core.windows.net/pragencies/56c07748-e193-42aa-9a72-73622acd30bb.jpg"

# The JSON detections you got back (only the 'DetectionsFull' list)
#    In practice you would load this from your service response.
detections_preview_full = [
    {"Type":"Rectangle","Accuracy":0.9734375,"ObjectName":"TrafficSign","GroupId":1,"Order":1,"PosX":2383,"PosY":990},
    {"Type":"Rectangle","Accuracy":0.9734375,"ObjectName":"TrafficSign","GroupId":1,"Order":2,"PosX":2483,"PosY":990},
    {"Type":"Rectangle","Accuracy":0.9734375,"ObjectName":"TrafficSign","GroupId":1,"Order":3,"PosX":2483,"PosY":1090},
    {"Type":"Rectangle","Accuracy":0.9734375,"ObjectName":"TrafficSign","GroupId":1,"Order":4,"PosX":2383,"PosY":1090},
]

# Download the image
resp = requests.get(url)
img = cv2.imdecode(np.frombuffer(resp.content, np.uint8), cv2.IMREAD_COLOR)

# Overlay the boxes
#    We know each rectangle detection comes in order, so points 1→2→3→4 form the corners.
#    We'll just draw the polygon they define.
for i in range(0, len(detections_preview_full), 4):
    quad = detections_preview_full[i:i+4]
    pts = np.array([[d["PosX"], d["PosY"]] for d in quad], dtype=np.int32)
    cv2.polylines(img, [pts], isClosed=True, color=(0,255,0), thickness=3)

# 5. Show it
cv2.imshow("Preview-res with Detections", img)
cv2.waitKey(0)
cv2.destroyAllWindows()
