import os
import uuid
from io import BytesIO
from PIL import Image, ImageOps
from azure.storage.blob import BlobServiceClient
import configparser

# Load config values
config = configparser.ConfigParser()
config.read(os.path.join(os.path.dirname(__file__), '..', 'config.ini'))


# Blob Storage connections and containers
STORAGE_CONN = config["AZURE"]["STORAGE_CONNECTION_STR"]
BLOB_STORAGE = config["AZURE"]["BLOB_STORAGE"]    # new organized container
BLOB_PREVIEW = config["AZURE"]["BLOB_PREVIEW"]    # legacy preview container

# singleton BlobServiceClient
BLOB_CLIENT = BlobServiceClient.from_connection_string(STORAGE_CONN)

def process_and_upload_image(image_bytes, guid=None):
    """
    Load the image into BOTH containers, with deterministic name on GUID.
    - BLOB_PREVIEW (pragencies): main + preview
    - BLOB_STORAGE (project-data): ONLY main (no preview)

    CROP
    pragencies/<guid>_crop.jpg = project-data/detections/<guid>_crop.jpg
    pragencies/<guid>_crop_preview.jpg
  

    FULL
    pragencies/<guid>_full.jpg = project-data/detections/<guid>_full.jpg
    pragencies/<guid>_full_preview.jpg
    
    Returns: (url_main_from_pragencies, url_preview_from_pragencies)
    """

    try:

        # decode & normalize
        img = Image.open(BytesIO(image_bytes))
        try:
            img = ImageOps.exif_transpose(img)
        except Exception:
            pass
        if img.mode != "RGB":
            img = img.convert("RGB")

        # main JPEG <= 3MB
        MAX_SIZE = 3 * 1024 * 1024
        is_jpeg = image_bytes[:2] == b'\xff\xd8'

        if is_jpeg and len(image_bytes) <= MAX_SIZE:
            # keep exactly the OpenCV bytes (no recompression)
            main_bytes = image_bytes
        else:
            # only recompress if it is NOT JPEG or if it is >3MB
            MIN_QUALITY = 90
            q = 97 # 100
            out_main = BytesIO()
            img.save(out_main, format="JPEG", quality=q)
            while len(out_main.getvalue()) > MAX_SIZE and q > MIN_QUALITY:
                q -= 2
                out_main.seek(0)
                img.save(out_main, format="JPEG", quality=q)
            out_main.seek(0)
            main_bytes = out_main.getvalue()


        # preview 300x200 (letterboxed)
        prev_img = ImageOps.fit(
            img, (300, 200),
            method=Image.Resampling.LANCZOS,
            centering=(0.5, 0.5))
        
        out_prev = BytesIO()
        prev_img.save(out_prev, format="JPEG", quality=95, optimize=True) # 100
        out_prev.seek(0)


        # deterministic names 
        name = str(guid)

        # (pragencies)
        prev_main_path = f"{name}.jpg"             # <guid>.jpg | <guid>_full.jpg
        prev_prev_path = f"{name}_preview.jpg"     # <guid>_preview.jpg | <guid>_full_preview.jpg

        # (project-data)
        stor_main_path = f"detections/{name}.jpg"  # <guid>.jpg | <guid>_full.jpg

        # upload
        client = BLOB_CLIENT

        # pragencies: main + preview 
        prev_main_client = client.get_blob_client(container=BLOB_PREVIEW, blob=prev_main_path)
        prev_prev_client = client.get_blob_client(container=BLOB_PREVIEW, blob=prev_prev_path)

        prev_main_client.upload_blob(main_bytes, overwrite=True)
        prev_prev_client.upload_blob(out_prev.getvalue(), overwrite=True)

        # project-data: no preview
        stor_main_client = client.get_blob_client(container=BLOB_STORAGE, blob=stor_main_path)

        stor_main_client.upload_blob(main_bytes, overwrite=True)

        # URLs (from pragencies)
        base_prev = f"https://{client.account_name}.blob.core.windows.net/{BLOB_PREVIEW}"
        url_main = f"{base_prev}/{prev_main_path}"
        url_prev = f"{base_prev}/{prev_prev_path}"
        return url_main, url_prev

    except Exception as e:
        print(f"[azure_blob] upload error: {e}")
        return "", ""


def upload_screenshot_image(frame):
    """
    Uploads an OpenCV frame as JPEG to the new container (BLOB_STORAGE) under screenshots/<uuid>.jpg
    """
    import cv2
    _, buf = cv2.imencode('.jpg', frame, [int(cv2.IMWRITE_JPEG_QUALITY), 97]) #100

    client = BLOB_CLIENT

    path = f"screenshots/{uuid.uuid4()}.jpg"
    blob_client = client.get_blob_client(container=BLOB_STORAGE, blob=path)
    blob_client.upload_blob(buf.tobytes(), overwrite=True)
    return f"https://{client.account_name}.blob.core.windows.net/{BLOB_STORAGE}/{path}"
