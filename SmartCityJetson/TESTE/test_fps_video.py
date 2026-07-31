import cv2

video_path = "../Video3.mp4"
cap = cv2.VideoCapture(video_path)

fps = cap.get(cv2.CAP_PROP_FPS)
print(f"The video's FPS is: {fps}")

cap.release()
