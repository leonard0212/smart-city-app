from ultralytics import YOLO

model = YOLO("../best_s_v10_TrafficSgn.pt")
results = model("poza.jpg")

for result in results:
    result.show()