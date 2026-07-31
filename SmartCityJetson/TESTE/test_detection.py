import cv2
import os
import re
import torch
import numpy as np
from ultralytics import YOLO

class BoundingBoxDrawer:
    def __init__(self):
        pass

    def draw_bounding_box(self, frame, polygon, label, track_id=None,
                          box_color=(255, 255, 255), text_bg_color=(0, 0, 0),
                          text_color=(255, 255, 255)):

        cv2.polylines(frame, [np.array(polygon, dtype=np.int32)],
                      isClosed=True, color=box_color, thickness=2)#2

        if track_id is not None:
            label = f"{label} [ID: {track_id}]"


        font_scale = 2.0 #1.0
        font_thickness = 4 #2

        text_size, _ = cv2.getTextSize(label, cv2.FONT_HERSHEY_SIMPLEX, font_scale, font_thickness)
        text_width, text_height = text_size

        x_min, y_min = polygon[0]
        label_x = max(x_min, 0)
        label_y = max(y_min - 10, text_height)


        cv2.rectangle(frame,
                      (label_x, label_y - text_height - 5),
                      (label_x + text_width, label_y + 5),
                      color=text_bg_color,
                      thickness=cv2.FILLED)


        cv2.putText(frame, label, (label_x, label_y),
                    cv2.FONT_HERSHEY_SIMPLEX, font_scale, text_color, thickness=font_thickness)

        return frame

    def draw_bounding_boxes(self, frame, detections):
        annotated_frame = frame.copy()
        for det in detections:
            polygon = det.get('polygon')
            label = det.get('label', '')
            track_id = det.get('track_id', None)
            annotated_frame = self.draw_bounding_box(
                annotated_frame, polygon, label, track_id
            )
        return annotated_frame


def get_next_output_filename(output_dir, prefix="OUTPUT", ext=".png"):

    pattern = re.compile(rf"^{re.escape(prefix)}(\d+){re.escape(ext)}$")
    max_index = 0
    try:
        files = os.listdir(output_dir)
    except FileNotFoundError:
        files = []

    for fname in files:
        m = pattern.match(fname)
        if m:
            idx = int(m.group(1))
            if idx > max_index:
                max_index = idx
    next_index = max_index + 1
    return f"{prefix}{next_index}{ext}"


def main():
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    print("Current device:", device)
    print("PyTorch version:", torch.__version__)
    print("CUDA available:", torch.cuda.is_available())
    if torch.cuda.is_available():
        print("GPU Name:", torch.cuda.get_device_name(0))
        torch.cuda.empty_cache()


    model = YOLO("../best.pt")
    model.to(device)


    image_path = os.path.abspath("../Img/Pothole34.jpeg")
    print(" Image path (absolut):", image_path)
    frame = cv2.imread(image_path)
    if frame is None:
        print(f" The image could not be read {image_path}")
        return

    # Predict cu YOLO
    results = model.predict(source=frame, conf=0.45, device=device)
    print(f" Processed image: {image_path}")

    res = results[0]
    detections = []
    if hasattr(res, "boxes") and len(res.boxes) > 0:
        xyxy = res.boxes.xyxy.cpu().numpy()
        class_ids = res.boxes.cls.cpu().numpy().astype(int)
        scores = res.boxes.conf.cpu().numpy()
        names = model.names

        for i, box in enumerate(xyxy):
            x1, y1, x2, y2 = box
            polygon = [
                [int(x1), int(y1)],
                [int(x2), int(y1)],
                [int(x2), int(y2)],
                [int(x1), int(y2)],
            ]
            cls_id = class_ids[i]
            label_text = names.get(cls_id, str(cls_id))
            confidence = scores[i]
            label = f"{label_text} {confidence:.2f}"

            detections.append({
                "polygon": polygon,
                "label": label,
                "track_id": None
            })


    drawer = BoundingBoxDrawer()
    annotated = drawer.draw_bounding_boxes(frame, detections)


    project_root = os.path.dirname(os.path.abspath(__file__))
    output_dir = os.path.abspath(os.path.join(project_root, "..", "Img", "OUTPUT"))
    print(" Output directory (absolut):", output_dir)


    try:
        os.makedirs(output_dir, exist_ok=True)
        print(f" Director creat/verificat: {output_dir}")
    except Exception as e:
        print(f" Error creating directory {output_dir}: {e}")
        return


    output_filename = get_next_output_filename(output_dir)
    output_path = os.path.join(output_dir, output_filename)
    print("Saving as:", output_path)


    success = cv2.imwrite(output_path, annotated)
    if success:
        print(f" The image was saved successfully: {output_path}")
    else:
        print(f" Error saving image to: {output_path}")

    # Opțional: afișare
    display_width = 800
    h, w, _ = annotated.shape
    scale = display_width / w
    display_h = int(h * scale)
    resized = cv2.resize(annotated, (display_width, display_h))

    cv2.imshow("Annotated Image", resized)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()



# import torch
#
# os.environ["KMP_DUPLICATE_LIB_OK"] = "TRUE"
#
# def main():
#     # Set device
#     device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
#     print("Current device:", device)
#     print("PyTorch version:", torch.__version__)
#     print("CUDA available:", torch.cuda.is_available())
#     if torch.cuda.is_available():
#         print("GPU Name:", torch.cuda.get_device_name(0))
#         torch.cuda.empty_cache()  # Clear memory if GPU is available
#
#     # Load the Trained Model for testing
#     model = YOLO("../best.pt")
#
#     # Path to the image to be tested
#     image_path = r"../Img/Pothole1.jpeg"
#
#     # Perform prediction on the image
#     results = model.predict(source=image_path, conf=0.25)
#     print(f"Processed image: {image_path}")
#
#     # Get annotated image
#     annotated_image = results[0].plot()
#
#     # Resize image for display
#     display_width = 800
#     height, width, _ = annotated_image.shape
#     scale_ratio = display_width / width
#     display_height = int(height * scale_ratio)
#     resized_image = cv2.resize(annotated_image, (display_width, display_height))
#
#     # Display the image
#     cv2.imshow("Annotated Image", resized_image)
#     cv2.waitKey(0)  # Wait for a key press to close the window
#     cv2.destroyAllWindows()  # Close the OpenCV window
#
#     # #SAVE PHOTO
#     # output_image_path = os.path.join(os.path.dirname(image_path), "OUTPUT_test_detection.png")
#     # cv2.imwrite(output_image_path, annotated_image)
#     # print(f"Annotated image saved at: {output_image_path}")
#
# if __name__ == '__main__':
#     main()
