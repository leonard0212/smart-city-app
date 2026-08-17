import tornado.web
import json
import io
from PIL import Image, ImageDraw, ImageFont
from datetime import datetime

# Arial exists only on Windows hosts; the Docker image ships DejaVu Sans instead.
FONT_CANDIDATES = [
    "arial.ttf",
    "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
]


def _load_font(size):
    for path in FONT_CANDIDATES:
        try:
            return ImageFont.truetype(path, size)
        except OSError:
            continue
    return ImageFont.load_default()


class AddDetectionToImage(tornado.web.RequestHandler):

    def post(self):
        # Retrieve image and detections from request
        image_file = self.request.files['image_file'][0]['body']
        detections_json = self.get_body_argument('detections', default=None)
        detections = json.loads(detections_json)

        image = Image.open(io.BytesIO(image_file))
        draw = ImageDraw.Draw(image)
        font = _load_font(20)

        # Group detections by GroupId
        grouped_detections = {}
        for det in detections:
            group_id = det['GroupId']
            if group_id not in grouped_detections:
                grouped_detections[group_id] = []
            grouped_detections[group_id].append(det)

        # Process each group of detections
        for group_id, group in grouped_detections.items():
            # Sort points by PointsOrder
            sorted_points = sorted(group, key=lambda x: x['PointsOrder'])
            points = [(point['PosX'], point['PosY']) for point in sorted_points]

            if len(points) == 4:  # Ensure we have exactly 4 points for the rectangle
                # Draw rectangle using the points
                draw.polygon(points, outline="red", width=2)

                # Draw label at the top-left corner
                obj_name = group[0]['ObjectName']
                acc = group[0]['Accuracy']
                label = f"{obj_name} ({acc:.2f})"

                bbox = draw.textbbox((0, 0), label, font=font)
                bbox_width = bbox[2] - bbox[0]
                bbox_height = bbox[3] - bbox[1]

                x1, y1 = points[0]
                text_bg_coords = [x1, y1 - bbox_height, x1 + bbox_width, y1]
                draw.rectangle(text_bg_coords, fill="red")
                draw.text((x1, y1 - bbox_height), label, fill="white", font=font)

        # Save and send updated image
        output_buffer = io.BytesIO()
        image.save(output_buffer, format='PNG')
        output_buffer.seek(0)

        self.set_header('Content-Type', 'image/png')
        print("BB Done")
        print(datetime.now())
        self.write(output_buffer.getvalue())
