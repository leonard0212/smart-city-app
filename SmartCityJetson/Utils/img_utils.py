import cv2
import numpy as np

# colors for vehicle segmentation and overlap
VEHICLE_COLOR      = (0, 255, 0)   # green
OVERLAP_COLOR      = (0, 0, 255)   # red
DEFAULT_MASK_ALPHA = 0.4 # transparency 40%


class BoundingBoxDrawer:
    def __init__(self):
        pass

    def draw_bounding_box(self, frame, polygon, label, track_id=None,
                          box_color=(255, 255, 255), text_bg_color=(0, 0, 0),
                          text_color=(255, 255, 255),
                          mask=None, mask_color=OVERLAP_COLOR, mask_alpha=DEFAULT_MASK_ALPHA):
        """
        Draws a bounding box with a label and optionally a tracking ID.
        """

        # if there is a mask, overlay it semi-transparently
        if mask is not None:
            overlay = frame.copy()
            overlay[mask] = mask_color
            frame = cv2.addWeighted(overlay, mask_alpha, frame, 1 - mask_alpha, 0)

        cv2.polylines(frame, [np.array(polygon, dtype=np.int32)], isClosed=True, color=box_color, thickness=1)

        #Include Tracking ID in the label
        if track_id is not None:
            label = f"{label} [ID: {track_id}]"

        text_size, _ = cv2.getTextSize(label, cv2.FONT_HERSHEY_SIMPLEX, 0.5, 1)
        text_width, text_height = text_size
        x_min, y_min = polygon[0]
        label_x = max(x_min, 0)
        label_y = max(y_min - 10, text_height)

        # Draw background for text
        cv2.rectangle(frame, (label_x, label_y - text_height - 5), (label_x + text_width, label_y + 5),
                      color=text_bg_color, thickness=cv2.FILLED)

        # Draw the text
        cv2.putText(frame, label, (label_x, label_y), cv2.FONT_HERSHEY_SIMPLEX, 0.5, text_color, thickness=1)

        return frame

    def draw_bounding_boxes(self, frame, detections):
        """
        Draws bounding boxes for all detections.
        - `detections` is a list of dictionaries containing:
        - 'polygon': list of bounding box points
        - 'label': text to display
        - 'track_id' (optional): Tracking ID from object detection
        """
        annotated_frame = frame.copy()
        for det in detections:
            polygon = det['polygon']
            label = det['label']
            tid = det.get('track_id', None)
            mask = det.get('mask', None)
            color = det.get('color', (255, 255, 255))

            annotated_frame = self.draw_bounding_box(
                annotated_frame,
                polygon,
                label,
                track_id=tid,
                box_color=color,
                text_bg_color=(0, 0, 0),
                text_color=color,
                mask=mask,
                mask_color=color,
                mask_alpha=DEFAULT_MASK_ALPHA
            )

        return annotated_frame
