
import cv2
import numpy as np

# Minimum threshold to accept a detection on the composite score
DEFAULT_SCORING_THRESHOLD = 0.52   # 0.6

# Frame-related size filters
MIN_AREA_RATIO = 0.0065  # the object must cover at least  ~0.6% by crop ≈ 5 530 px (~74x74)
MIN_SIDE_FRAC  = 0.03     # minimum side =  min ~29 px of frame width/height

AREA_WEIGHT = 0.30
CONF_WEIGHT = 0.70

def calculate_area_and_scoring(frame, polygon, confidence, threshold=DEFAULT_SCORING_THRESHOLD):

    H, W = frame.shape[:2]

    # area in pixels + widths/heights for side filters
    if len(polygon) == 4: #rectangle
        x_min, y_min = polygon[0]
        x_max, y_max = polygon[2]
        w = max(0, x_max - x_min)
        h = max(0, y_max - y_min)
        area_pixels = w * h
    else:  # poligon
        cnt = np.array(polygon, dtype=np.int32)
        area_pixels = int(abs(cv2.contourArea(cnt)))
        xs = [p[0] for p in polygon]
        ys = [p[1] for p in polygon]
        w = max(xs) - min(xs)
        h = max(ys) - min(ys)

    # area relative to frame
    area = area_pixels / float(W * H + 1e-9)

    # anti small obj
    too_small_area = area < MIN_AREA_RATIO
    too_thin = (w / float(W + 1e-9)) < MIN_SIDE_FRAC or (h / float(H + 1e-9)) < MIN_SIDE_FRAC
    if too_small_area or too_thin:
        return area, 0.0, False

    scoring = AREA_WEIGHT * area + CONF_WEIGHT * confidence
    is_valid = scoring >= threshold
    return area, scoring, is_valid
