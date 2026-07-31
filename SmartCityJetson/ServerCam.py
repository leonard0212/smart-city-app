from DetectionClass import Integrated_detector

# 0) Silent GPIO cleanup so you never hit “device busy”
import warnings
try:
    import Jetson.GPIO as GPIO
    warnings.filterwarnings(
        "ignore",
        message="No channels have been set up yet.*"
    )
    GPIO.cleanup()
except ImportError:
    pass


# region Execution and Workflow
# Instantiate the integrated detector with specified models and paths
detector = Integrated_detector(
    object_detector_model="best.engine",
    coco_detector_model="yolo11s.engine",
    plate_detector_model ="my_license_plate_detector.engine",
    face_detector_model = "my_face_detector.engine",
    segmentation_model="yolo11s-seg.engine",
)

# Process the video with the integrated detector
detector.process_video(use_camera=True, sync=True) #source="TESTE/Video/4K.mp4", use_camera=True,source="TESTE/Img/poza4k.jpeg",
# endregion