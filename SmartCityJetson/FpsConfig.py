import cv2

def configure_fps(cap=None, target_fps=None, frame_skip=None):# target_fps= None for cam, 15 for video
    """
    Dynamically calculates FPS settings for video and image processing.
    - If `cap` is None (processing an image), the FPS is set to 30.
    - If `cap` is a video, it retrieves the FPS from the video source.
    - If `target_fps` is None, it defaults to the original FPS.
    - If `frame_skip` is None, it is calculated as `max(1, int(fps / target_fps))`.
    - If `frame_skip` is manually set, it is used as is.

    """

    if cap is None:
        # Default FPS for images
        fps = 30.0
    else:
        # Get FPS from video source, fallback to 30 if not available
        fps = cap.get(cv2.CAP_PROP_FPS)
        if not fps or fps <= 0:
            fps = 30.0

    # If target_fps is None, use the original FPS
    if target_fps is None:
        target_fps = fps

    # Ensure target_fps is not greater than the actual FPS
    target_fps = min(target_fps, fps)

    # If frame_skip is None, calculate it dynamically
    if frame_skip is None:
        frame_skip = max(1, int(fps / target_fps))

    return fps, target_fps, frame_skip
