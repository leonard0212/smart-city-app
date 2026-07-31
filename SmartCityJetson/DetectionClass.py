
import os
import threading
import cv2
import numpy as np
from datetime import datetime
from ultralytics import YOLO
import torch
import configparser
from concurrent.futures import ThreadPoolExecutor
import uuid
import json
import time
import collections
from scoring import DEFAULT_SCORING_THRESHOLD, calculate_area_and_scoring
from threading import Lock
from azure.servicebus import ServiceBusClient, ServiceBusMessage
from ClassMapping import (CLASS_MAPPING, VEHICLES, ignore_ok_subclasses, get_main_class, get_sub_class,
                          get_min_conf_for_class, OBJ_MAPPING, )
from Utils.img_utils import BoundingBoxDrawer, VEHICLE_COLOR, OVERLAP_COLOR, DEFAULT_MASK_ALPHA
from Utils.azure_blob import (process_and_upload_image, upload_screenshot_image,)
from FpsConfig import configure_fps
from Utils.GPSModule import GPSCompassModule
from math import radians, sin, cos, sqrt, atan2
from Utils.VideoWriter import (
    init_video_writers,
    save_and_show,
    release_video_writers,
    RotatingVideoWriter,
    ensure_window_initialized,
    window_gui_events,
    show_full_with_crop,
)

# Set environment variables to avoid OpenMP conflicts used by other libraries
if not os.environ.get("KMP_DUPLICATE_LIB_OK"):
    os.environ["KMP_DUPLICATE_LIB_OK"] = "TRUE"
if not os.environ.get("OMP_NUM_THREADS"):
    os.environ["OMP_NUM_THREADS"] = "1"

class Integrated_detector:
    """
    Class to integrate multiple YOLO models for processing video frames,
    detecting objects, and generating both annotated video outputs and a JSON report of detections.
    """

    def __init__(
            self,
            object_detector_model,
            coco_detector_model,
            plate_detector_model,
            face_detector_model,
            segmentation_model=None,
            config_path="config.ini",
            tracker_config="bytetrack.yaml",
            target_fps=None,  # will be calculated in FpsConfig.py
            frame_skip=None,  # will be calculated in FpsConfig.py

    ):

        # Determine device based on GPU availability
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        print(
            "\n[STATUS] "
            + ("GPU is available. " if self.device.type == "cuda" else "GPU is not available. ")
            + f"Models will run on {self.device.type.upper()}."
        )

        # Load configuration from config.ini
        self.config = configparser.ConfigParser()
        self.config.read(config_path)

        self._prep_logged = False

        # SB singletons
        self.sb_client = ServiceBusClient.from_connection_string(
            conn_str=self.config["AZURE"]["CONNECTION_STR"]
        )
        self.sb_detect_sender = self.sb_client.get_queue_sender(
            queue_name=self.config["AZURE"]["QUEUE_NAME"]
        )
        self.sb_state_sender = self.sb_client.get_queue_sender(
            queue_name=self.config["AZURE"]["QUEUE_STATE_NAME"]
        )
        # upload thread
        upload_workers = self.config.getint("DEBUG", "upload_workers", fallback=2)
        self._send_executor = ThreadPoolExecutor(max_workers=upload_workers,
                                                 thread_name_prefix="sendexec")

        # FILTER settings
        self.send_ok = self.config.getboolean("FILTER", "send_ok_images", fallback=True)
        self.send_billboard = self.config.getboolean("FILTER", "send_billboard_images", fallback=True)
        self.send_crosswalk_damaged = self.config.getboolean("FILTER", "send_crosswalk_damaged", fallback=True)

        print("[INFO] send_ok_images flag is:", self.send_ok)
        print("[INFO] send_billboard_images flag is:", self.send_billboard)
        print("[INFO] send_crosswalk_damaged flag is:", self.send_crosswalk_damaged)


        # SEGMENTATION
        self.enable_crosswalk_seg = self.config.getboolean("SEGMENTATION", "crosswalk_segmentation_enable",fallback=False)
        print("[INFO] crosswalk_segmentation_enable flag is:", self.enable_crosswalk_seg)
        self.overlap_threshold  = self.config.getfloat("SEGMENTATION", "overlap_threshold", fallback=0.10)
        self.frame_ratio_threshold  = self.config.getfloat("SEGMENTATION", "frame_ratio_threshold", fallback=0.95)
        self.history_maxlen = self.config.getint("SEGMENTATION", "history_max_frames", fallback=30)

        # MODEL thresholds fallback
        self.score_threshold = self.config.getfloat("MODEL", "confidence_threshold", fallback=0.56)
        # print("[DEBUG] MODEL.confidence_threshold =", self.score_threshold)
        # print("[DEBUG] config_path =", os.path.abspath(config_path))
        self.min_percentage = self.config.getfloat("MODEL", "min_percentage", fallback=0.01)

        # DEBUG save and show video
        self.show_video = self.config.getboolean("DEBUG", "show_video", fallback=False)
        self.save_video = self.config.getboolean("DEBUG", "save_video", fallback=False)
        self.debug_files_path = self.config.get("DEBUG", "save_video_path", fallback="")

        # CAMERA section
        self.camera_enable = self.config.getboolean("CAMERA", "camera_settings_enabled", fallback=False)
        self.camera_id = self.config.getint("CAMERA", "camera_id", fallback=0)
        self.camera_width = self.config.getint("CAMERA", "camera_width", fallback=1920)  # 3840#1920
        self.camera_height = self.config.getint("CAMERA", "camera_height", fallback=1080)  # 2160#1080

        # CAMERA CROP settings
        self.camera_crop_enable = self.config.getboolean("CAMERA", "camera_crop_enabled", fallback=False)
        self.camera_crop_x = self.config.getint("CAMERA", "camera_crop_x", fallback=0)
        self.camera_crop_y = self.config.getint("CAMERA", "camera_crop_y", fallback=0)
        self.camera_crop_width = self.config.getint("CAMERA", "camera_crop_width", fallback=960)
        self.camera_crop_height = self.config.getint("CAMERA", "camera_crop_height", fallback=960)

        # Loading models on the determined device
        self.object_model = YOLO(object_detector_model, task="detect")  # .to('cuda')

        # model_names = self.object_model.names
        # pred_labels = set(model_names.values() if isinstance(model_names, dict) else model_names)
        # mapping_labels = set()
        # for subs in OBJ_MAPPING.values():
        #     mapping_labels.update(subs)
        #
        # mismatch_lower = {lbl for lbl in pred_labels if
        #                   lbl.lower() in {x.lower() for x in mapping_labels} and lbl not in mapping_labels}
        # missing = pred_labels - mapping_labels
        #
        # print("[MAP CHECK] model:", sorted(pred_labels))
        # print("[MAP CHECK] mapping:", sorted(mapping_labels))
        # print("[MAP CHECK] mismatch case:", mismatch_lower)
        # print("[MAP CHECK] missing in mapping:", missing)


        self.coco_model = YOLO(coco_detector_model, task="detect")  # .to('cuda')
        names = self.coco_model.names
        name_iter = names.items() if isinstance(names, dict) else enumerate(names)
        self._coco_keep = [i for i, n in name_iter if n == "person" or n in VEHICLES]

        self.plate_model = YOLO(plate_detector_model, task="detect")  # .to('cuda')
        self.face_model = YOLO(face_detector_model, task="detect")  # .to('cuda')
        if self.enable_crosswalk_seg:
            if segmentation_model is None:
                raise ValueError("SEGMENTATION enabled but no segmentation_model passed")
            self.seg_model = YOLO(segmentation_model, task="segment") # .to('cuda')



        # Detection parameters
        self.tracker_config = tracker_config
        self.target_fps = target_fps
        self.frame_skip = frame_skip
        self.fp16 = (self.device.type == "cuda")

        self._guid_seq_counter = 0
        self._inflight = set()

        #track + overlap
        self.track_history  = {}
        self.cross_history  = {}
        self.cross_reported = set()


        # DEVICE ID FROM /etc/smartcity/config.json
        json_cfg_path = "/etc/smartcity/config.json"
        self.device_id = None
        if os.path.isfile(json_cfg_path):
            try:
                with open(json_cfg_path, "r") as f:
                    cfg = json.load(f)
                self.device_id = cfg.get("device_id")
                print(f"[INFO] device_id = {self.device_id}")
            except Exception as e:
                print(f"[WARN] Can't read {json_cfg_path}: {e}")
        else:
            print(f"[WARN] File does not exist: {json_cfg_path}")


        # Initialize caches
        self.id_to_guid = {}
        self.json_detections_cache = {}
        self.frame_group_ids = {}
        self.cache_lock = Lock()
        self._state_lock = Lock()  # for last_device_state
        self._sb_lock = Lock()
        self.best_image = {}

        # GPS initialization
        self.gps = GPSCompassModule(config_path=config_path)
        self.gps.start()
        self.gps_drop_if_no_fix = self.config.getboolean("GPS", "drop_if_no_fix", fallback=False)
        self.last_device_state = None
        #print("[DEBUG] Waiting for GPS fix or static coordinates...")
        print(f"[INFO] GPS enable flag is: {self.gps.gps_enable}")

        # Screenshot worker state (will be used only if GPS is not None)
        self._last_ss_coord = None
        self._ss_distance_acc = 0.0
        self._current_frame = None
        self._stop_screenshot_thread = False

        # Wait until GPS returns a valid coordinate (or static fallback if disabled)
        if not self.gps.gps_enable:
            # gps_enable = False
            _ = self.gps.get_coordinates()
            self.device_state("GPS Manual Disabled")
        else:
            # gps_enable = True
            while True:
                coords = self.gps.get_coordinates()
                if coords:
                    self.device_state("GPS is Available")
                    break
                else:
                    print("[DEBUG] No valid GPS yet, retrying in 3 seconds...")
                    self.device_state("GPS Not Available")
                    time.sleep(3)

        # # Launch screenshot thread only if GPS is enabled
        # if self.gps.gps_enable:
        #     self._screenshot_thread = threading.Thread(
        #         target=self._screenshot_worker,
        #         daemon=True
        #     )
        #     self._screenshot_thread.start()
        # else:
        #     print("[INFO] GPS disabled, screenshot thread not started.")


    # region Utilities

    def generate_guid(self):
        """
        Generates a unique identifier (GUID) for tracking detected objects.
        """
        return str(uuid.uuid4())

    def get_guid(self, track_id):
        """
        Get or generate a GUID for a given track ID.
        """
        if track_id < 0:  # Handle invalid track_id by generating a temporary GUID
            return self.generate_guid()

        with self.cache_lock:  # Thread-safe access
            if track_id not in self.id_to_guid:
                new_guid = self.generate_guid()
                while new_guid in self.id_to_guid.values():
                    new_guid = self.generate_guid()  # Ensure uniqueness
                self.id_to_guid[track_id] = new_guid

        return self.id_to_guid[track_id]

    # endregion

    # region Image Processing


    def calculate_scale_factor(self, original_size, processed_size):
        """
        Calculate the scale factor between the original and processed image sizes.
        """
        original_width, original_height = original_size
        processed_width, processed_height = processed_size

        scale_x = original_width / processed_width
        scale_y = original_height / processed_height

        return {"scale_x": scale_x, "scale_y": scale_y}

    def apply_blur(self, frame, annotated_frame, coco_results):
        """
        - frame:         crop  (from original)
        - annotated_frame: the copy we are applying blur to
        - coco_results: COCO results for person/vehicle detection
        """
        person_boxes = []
        vehicle_boxes = []
        for res in coco_results:
            for box in res.boxes:
                x1, y1, x2, y2 = map(int, box.xyxy[0])
                cls = int(box.cls.cpu().item())
                name = self.coco_model.names[cls]
                if name == "person":
                    person_boxes.append((x1, y1, x2, y2))
                elif name in VEHICLES:
                    vehicle_boxes.append((x1, y1, x2, y2))

        # On each person_box, run the face detector and blur on each face
        for px1, py1, px2, py2 in person_boxes:
            roi = frame[py1:py2, px1:px2]
            if roi.size == 0:
                continue

            face_results = self.face_model.predict(
                roi, conf=0.3, device=self.device,half=self.fp16,imgsz=960
            )[0]
            for fb in face_results.boxes:
                fx1, fy1, fx2, fy2 = map(int, fb.xyxy[0])
                ax1, ay1 = px1 + fx1, py1 + fy1
                ax2, ay2 = px1 + fx2, py1 + fy2
                if ax2 > ax1 and ay2 > ay1:
                    face_roi = annotated_frame[ay1:ay2, ax1:ax2]
                    if face_roi.size > 0:
                        annotated_frame[ay1:ay2, ax1:ax2] = cv2.GaussianBlur(face_roi, (35, 35), 20)

        # On each vehicle_box, run the license plate detector and blur on each license plate
        for vx1, vy1, vx2, vy2 in vehicle_boxes:
            veh_roi = frame[vy1:vy2, vx1:vx2]
            if veh_roi.size == 0:
                continue

            plate_results = self.plate_model.predict(
                veh_roi, conf=0.3, device=self.device,half=self.fp16,imgsz=960
            )[0]
            for pb in plate_results.boxes:
                px1, py1, px2, py2 = map(int, pb.xyxy[0])
                bx1, by1 = vx1 + px1, vy1 + py1
                bx2, by2 = vx1 + px2, vy1 + py2
                if bx2 > bx1 and by2 > by1:
                    plate_roi = annotated_frame[by1:by2, bx1:bx2]
                    if plate_roi.size > 0:
                        annotated_frame[by1:by2, bx1:bx2] = cv2.GaussianBlur(plate_roi, (35, 35), 30)
    # endregion

    # region Box Processing

    def process_box(self, box, annotated_frame, default_main_class, model_source, custom_polygon=None):  # original_frame_buffer=None
        """
        Processes each detection box (by annotating the frame) and generates detection data.
        """
        confidence = float(box.conf.cpu().item())
        class_index = int(box.cls.cpu().item())
        names = model_source.names
        class_name = names[class_index] if isinstance(names, (list, tuple)) else names.get(class_index, "Unknown")

        track_id = int(box.id.cpu().item()) if box.id is not None else -1

        inferred_main_class = get_main_class(class_name) or default_main_class
        inferred_sub_class = get_sub_class(class_name)

        if custom_polygon:
            polygon = custom_polygon
        else:
            x1, y1, x2, y2 = map(int, box.xyxy[0].tolist())
            polygon = [(x1, y1), (x2, y1), (x2, y2), (x1, y2)]

        # if it is too small or below the scoring threshold
        area, scoring, is_valid = calculate_area_and_scoring(annotated_frame, polygon, confidence)
        if not is_valid:
            return None

        guid = self.get_guid(track_id)

        processed_size = (annotated_frame.shape[1], annotated_frame.shape[0])  # width, height


        detection_data = {
            "guid": guid,
            "main_class": inferred_main_class,
            "sub_class": inferred_sub_class,
            "confidence": confidence,
            "polygon": polygon,
            "track_id": track_id,
            "image_buffer": None,
            "scoring": scoring,
            "is_valid": is_valid,
            "area": area,
            "processed_size": processed_size,
        }

        return detection_data

    @staticmethod
    def resolve_sign_conflicts(detections, iou_thr=0.50, ioa_thr=0.70, delta=0.07):
        def iou_ioa(a, b):
            ax = [p[0] for p in a["polygon"]]
            ay = [p[1] for p in a["polygon"]]
            bx = [p[0] for p in b["polygon"]]
            by = [p[1] for p in b["polygon"]]
            ax1, ax2 = min(ax), max(ax)
            ay1, ay2 = min(ay), max(ay)
            bx1, bx2 = min(bx), max(bx)
            by1, by2 = min(by), max(by)
            x1, y1 = max(ax1, bx1), max(ay1, by1)
            x2, y2 = min(ax2, bx2), min(ay2, by2)
            inter = max(0, x2 - x1) * max(0, y2 - y1)
            if inter <= 0:
                return 0.0, 0.0
            a_area = (ax2 - ax1) * (ay2 - ay1)
            b_area = (bx2 - bx1) * (by2 - by1)
            iou = inter / (a_area + b_area - inter + 1e-9)
            ioa = inter / (min(a_area, b_area) + 1e-9)
            return iou, ioa

        keep = [True] * len(detections)

        for i in range(len(detections)):
            di = detections[i]
            if di["main_class"] != "TrafficSign" or di["sub_class"] not in ("TrafficSignDamaged", "TrafficSignOk"):
                continue

            for j in range(i + 1, len(detections)):
                if not keep[i] or not keep[j]:
                    continue
                dj = detections[j]
                if dj["main_class"] != "TrafficSign" or dj["sub_class"] not in ("TrafficSignDamaged", "TrafficSignOk"):
                    continue

                iou, ioa = iou_ioa(di, dj)
                if not (iou >= iou_thr or ioa >= ioa_thr):
                    continue

                if di["sub_class"] == "TrafficSignDamaged" and dj["sub_class"] == "TrafficSignOk":
                    conf_d, conf_o = di["confidence"], dj["confidence"]

                    if conf_d >= conf_o - delta:
                        keep[j] = False
                    else:
                        keep[i] = False
                elif di["sub_class"] == "TrafficSignOk" and dj["sub_class"] == "TrafficSignDamaged":
                    conf_o, conf_d = di["confidence"], dj["confidence"]
                    if conf_d >= conf_o - delta:
                        keep[i] = False
                    else:
                        keep[j] = False
        return [d for d, k in zip(detections, keep) if k]

    # region Cache Management
    def clear_caches(self, cache_types=None, detection_id=None):
        """
        Clears specified caches and buffers to free up memory.
        cache_types: any of ["img_cache", "json_detections_cache", "best_image"]
        """
        if cache_types is None:
            cache_types = [ "json_detections_cache", "best_image"]

        cleared = []

        with self.cache_lock:

            # json_detections_cache
            if "json_detections_cache" in cache_types:
                if detection_id:
                    self.json_detections_cache.pop(detection_id, None)
                else:
                    self.json_detections_cache.clear()
                cleared.append("json_detections_cache")

            # best_image
            if "best_image" in cache_types:
                if detection_id:
                    self.best_image.pop(detection_id, None)
                else:
                    self.best_image.clear()
                cleared.append("best_image")

        if cleared:
            print(f"Cleared caches: {', '.join(cleared)}.")
        else:
            print("No caches were cleared.")

    # endregion

    # region JSON and Azure Integration

    def _flush_cache(self, frame_id, sync=False):
        """
        Send to Azure the best scoring expired detections after timeout,
        following the rules for CrosswalkOverlap, CrosswalkDamaged, and CrosswalkOk.
        Then clear them from json_detections_cache.
        """
        fps = self.target_fps
        if not fps:
            _, fps, _ = configure_fps(None)  # fallback 30.0
        timeout_frames = max(1, int(fps * 4)) #3

        with self.cache_lock:
            cache_snapshot = []
            for guid, detections in self.json_detections_cache.items():
                cache_snapshot.append((guid,detections.get("last_seen_frame", frame_id - (timeout_frames + 1)),list(detections.get("entries", ())),detections.get("seq", 0)))


        ready_items = []  # (seq, guid, best_entry, last_seen_frame, track_id)
        for cached_guid, last_seen_frame, entries, seq in cache_snapshot:
            elapsed = frame_id - last_seen_frame
            if elapsed <= timeout_frames:
                continue

            valid = [entry for entry in entries if entry["scoring"] >= DEFAULT_SCORING_THRESHOLD]
            if not valid:
                with self.cache_lock:
                    self.json_detections_cache.pop(cached_guid, None)
                    self.best_image.pop(cached_guid, None)
                continue

            overlapped = [
                entry for entry in valid
                if entry.get("detection_data", {}).get("is_on_crosswalk", False)
                   or str(entry.get("detection_data", {}).get("main_class", "")).endswith("Overlap")]

            cands = overlapped if overlapped else valid
            # majority vote on subclass
            counts = {}
            for e in cands:
                s = e["detection_data"]["sub_class"]
                counts[s] = counts.get(s, 0) + 1
            maj_sub = max(counts.items(), key=lambda t: t[1])[0]
            maj_set = [e for e in cands if e["detection_data"]["sub_class"] == maj_sub]
            best = max(maj_set, key=lambda e: e["scoring"])

            main_class = best["detection_data"]["main_class"]
            sub_class = best["detection_data"]["sub_class"]
            track_id = best["detection_data"]["track_id"]

            send_it = False

            if main_class == "CrosswalkOverlap":
                send_it = True
            elif main_class != "Crosswalk":
                send_it = True

            ## CrosswalkDamaged → respect flag; still send if we have overlap from history
            elif sub_class == "CrosswalkDamaged":
                overlap_from_history = False
                if self.enable_crosswalk_seg:
                    history = self.cross_history.get(track_id, [])
                    if history and (sum(history) / len(history)) >= self.frame_ratio_threshold:
                         overlap_from_history = True
                         main_class = "CrosswalkOverlap"
                         sub_class = "CrosswalkDamagedOverlap"
                if not self.send_crosswalk_damaged and not overlap_from_history:
                    send_it = False
                else:
                    send_it = True

            # CrosswalkOk → send if overlap or if send_ok=True
            elif sub_class == "CrosswalkOk":
                if self.enable_crosswalk_seg:
                    history = self.cross_history.get(track_id, [])
                    if history and (sum(history) / len(history)) >= self.frame_ratio_threshold:
                        main_class = "CrosswalkOverlap"
                        sub_class = "CrosswalkOkOverlap"
                        send_it = True
                if self.send_ok:
                    send_it = True

            with self.cache_lock:
                detection_now = self.json_detections_cache.get(cached_guid)
                if not detection_now:
                    continue
                if (frame_id - detection_now.get("last_seen_frame", last_seen_frame)) <= timeout_frames:
                    continue
                if detection_now.get("queued") or (cached_guid in self._inflight):
                    continue
                best["detection_data"]["main_class"] = main_class
                best["detection_data"]["sub_class"] = sub_class
                detection_now["queued"] = True
                self._inflight.add(cached_guid)
                seq = detection_now.get("seq", seq)
            if send_it:
                ready_items.append((seq, cached_guid, best, last_seen_frame, track_id))
            else:
                with self.cache_lock:
                    self.json_detections_cache.pop(cached_guid, None)
                    self.best_image.pop(cached_guid, None)
                    self.cross_history.pop(track_id, None)

        ready_items.sort(key=lambda t: t[0])
        for seq, gid, best, last_seen_frame, track_id in ready_items:
            self._send_executor.submit(self.send_data_to_cloud, sync, gid, best, last_seen_frame)
            with self.cache_lock:
                self.cross_history.pop(track_id, None)



    def update_json(self, detection_data, frame_id, sync=False) -> object:
        """
        Updates the detection cache with detection data, ensuring the highest scoring detection
        per GUID is cached. Sends the detection with the highest scoring to Azure when a GUID
        is no longer active.
        """

        if detection_data is None:
            self._flush_cache(frame_id, sync)
            return

        guid = detection_data["guid"]
        scoring = detection_data["scoring"]
        original_size = detection_data["original_size"]
        processed_size = detection_data["processed_size"]
        track_id = detection_data["track_id"]  # Added for verification

        # We check if the track_id is valid (we avoid sending invalid IDs with -1 to Azure)
        if track_id == -1:
            print(f"Skipping detection {guid} - Track ID not assigned yet.")
            return

        scale_factor = self.calculate_scale_factor(original_size, processed_size)

        img_buf = detection_data.pop("image_buffer", None)
        full_buf = detection_data.pop("full_image_buffer", None)

        with self.cache_lock:
            prev = self.best_image.get(guid, {"score": -1})
            if scoring > prev["score"]:
                self.best_image[guid] = {"score": scoring, "img": img_buf, "full": full_buf}

        meta_fields = ("main_class", "sub_class", "confidence", "polygon", "track_id", "scoring",
                       "area", "processed_size", "original_size", "group_id", "is_on_crosswalk",
                       "gps", "time")

        meta = {k: detection_data[k] for k in meta_fields if k in detection_data}

        entry = {
            "detection_data": meta,
            "scoring": scoring,
            "frame_id": frame_id,
            "group_id": detection_data["group_id"],
            "scale_factor": scale_factor
        }

        with self.cache_lock:
            is_new_guid = guid not in self.json_detections_cache
            detections = self.json_detections_cache.setdefault(guid, {"entries": [], "last_seen_frame": frame_id})
            if is_new_guid or "seq" not in detections:
                self._guid_seq_counter += 1
                detections["seq"] = self._guid_seq_counter
                detections["queued"] = False
            detections["entries"].append(entry)
            detections["last_seen_frame"] = frame_id


        # Send immediately if processing a single image
        if frame_id == 0 and scoring >= DEFAULT_SCORING_THRESHOLD:
            with self.cache_lock:
                if guid in self._inflight:
                    return
                self._inflight.add(guid)
            self._send_executor.submit(self.send_data_to_cloud, sync, guid, entry, None)


    def send_data_to_cloud(self, sync, send_id, data, expected_last_seen=None):
        """
        Handles encoding, GPS retrieval, Blob upload (if sync), payload construction,
        Service Bus send, and cache cleanup for a single detection.
        """

        jpeg_bytes = None
        full_bytes = None

        try:
            with self.cache_lock:
                rec = self.best_image.get(send_id)
                if rec and rec.get("img") is not None:
                    jpeg_bytes = rec["img"]
                    full_bytes = rec.get("full")
                else:
                    rec = None

            if rec is None:
                print(f"[WARN] No best image buffer for GUID {send_id}.")
                return  # finally will run and clean the caches

            # Default URL fields (empty if no upload)
            image_url, image_url_preview = '', ''
            image_url_full, image_url_full_preview = '', ''

           # If sync, upload both buffers and get URLs
            if sync:
                image_url, image_url_preview = process_and_upload_image(jpeg_bytes, send_id)
                if full_bytes is not None:
                    image_url_full, image_url_full_preview = process_and_upload_image(
                        full_bytes, f"{send_id}_full"
                    )


            # GPS/TIME from detection moment (static or live)
            det_gps = data["detection_data"].get("gps")
            det_time = data["detection_data"].get("time")
            have_det_coords = bool(det_gps and ("Lat" in det_gps) and ("Lng" in det_gps))

            if have_det_coords:
                lat, lng = float(det_gps["Lat"]), float(det_gps["Lng"])
                if not self.gps.gps_enable:
                    self.device_state("GPS Manual Disabled")
                else:
                    self.device_state("GPS is Available")
            else:
                if not self.gps.gps_enable:
                    lat, lng = self.gps.get_coordinates()
                    self.device_state("GPS Manual Disabled")
                else:
                    if self.gps_drop_if_no_fix:
                        coords = self.gps.get_coordinates()
                        if not coords:
                            # No fix = drop
                            self.device_state("GPS Not Available")
                            with self.cache_lock:
                                self._inflight.discard(send_id)
                                self.best_image.pop(send_id, None)
                                self.json_detections_cache.pop(send_id, None)
                            return
                        lat, lng = coords
                        self.device_state("GPS is Available")
                    else:
                        while True:
                            coords = self.gps.get_coordinates()
                            if coords:
                                lat, lng = coords
                                self.device_state("GPS is Available")
                                break
                            else:
                                self.device_state("GPS Not Available")
                                time.sleep(1)

            # Time from the moment of detection, with fallback to the current time
            if det_time:
                formatted = det_time
            else:
                now = datetime.now()
                formatted = now.strftime("%Y-%m-%d %H:%M:%S")

            # translation offset only when crop is active
            offset_x = self.camera_crop_x if self.camera_crop_enable else 0
            offset_y = self.camera_crop_y if self.camera_crop_enable else 0

            # FULL (with offset)
            detections_full = [
                {
                    "Type": "Rectangle" if len(data["detection_data"]["polygon"]) == 4 else "Polygon",
                    "Accuracy": data["scoring"],
                    "ObjectName": data["detection_data"]["main_class"],
                    "GroupId": data["group_id"],
                    "Order": i + 1,
                    "PosX": point[0] + offset_x,
                    "PosY": point[1] + offset_y,
                }
                for i, point in enumerate(data["detection_data"]["polygon"])
            ]

            # CROP (without offset)
            detections_crop = [
                {
                    "Type": "Rectangle" if len(data["detection_data"]["polygon"]) == 4 else "Polygon",
                    "Accuracy": data["scoring"],
                    "ObjectName": data["detection_data"]["main_class"],
                    "GroupId": data["group_id"],
                    "Order": i + 1,
                    "PosX": point[0],
                    "PosY": point[1],
                }
                for i, point in enumerate(data["detection_data"]["polygon"])
            ]

            # Build the payload for Azure Service Bus
            common_entry = {
                "MainClass": data["detection_data"]["main_class"],  # Directly from detection_data
                "SubClass": data["detection_data"]["sub_class"],  # Directly from detection_data
                "Lat": lat,
                "Lng": lng,
                "state": "Detection OK",
                "time": formatted,
                "EdgeId": self.device_id,
                "ImageUrl": image_url,
                "ImageUrlPreview": image_url_preview,
                "ImageUrlFull": image_url_full,
                "ImageUrlFullPreview": image_url_full_preview,
                "ScaleFactor": data["scale_factor"],
                "Detections": detections_full,      # FULL
                "DetectionsCrop": detections_crop,  # CROP
            }
            detection_entry = {"DetectionId": send_id, **common_entry}


            # If sync is True, send to Service Bus
            if sync:
                self.azure(detection_entry)
                #print(f"[INFO] Detection payload ( sent to AZURE; sync = True): {json.dumps(detection_entry, indent=4)}")
            else:
                print(f"[DEBUG] Detection payload (NOT sent to AZURE; sent sync = False): {json.dumps(detection_entry, indent=4)}")

        except Exception as e:
            print(f"Error in sending detection for GUID {send_id}: {e}")

        finally:
            with self.cache_lock:
                detections = self.json_detections_cache.get(send_id)
                expired_now = (expected_last_seen is None) or (
                        detections and detections.get("last_seen_frame") == expected_last_seen)
                if expired_now:
                    self.json_detections_cache.pop(send_id, None)

                self.best_image.pop(send_id, None)
                self._inflight.discard(send_id)

                # retire the mapping ONLY if this GUID has actually expired
                if expired_now:
                    try:
                        tid = data["detection_data"].get("track_id")
                        if isinstance(tid, int) and tid >= 0:
                            self.id_to_guid.pop(tid, None)
                    except Exception:
                        pass

                    # fallback: clear any mapping to send_id
                    for tid2, guid in list(self.id_to_guid.items()):
                         if guid == send_id:
                             self.id_to_guid.pop(tid2, None)



    def device_state(self, state):
        """
        Send a device state update to Azure Service Bus,
        but only if 'state' is different from the last state sent.
        """

        with self._state_lock:
            if state == self.last_device_state:
                return
            self.last_device_state = state

        try:
            # Initialize Azure Service Bus client and queue sender
            queue_sender = self.sb_state_sender


            state_obj = {
                "EdgeId": self.device_id,
                "State": state,
                #"NEW detection": state,
                "Time": datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            }
            json_data = json.dumps(state_obj, indent=4)
            message = ServiceBusMessage(
                body=json_data,
                content_type="application/json",
                subject="DeviceState"
            )

            # Retry mechanism for sending messages
            for attempt in range(3):  # Maximum 3 attempts
                try:
                    with self._sb_lock:
                        queue_sender.send_messages(message)

                    print(f"[INFO] Device State {state} sent to Azure successfully!")
                    return  # Exit the loop after a successful send
                except Exception as e:
                    print(f"[WARN] Attempt {attempt + 1}: Failed to send detection: {e}")
                    if attempt == 2:  # Final attempt
                        print(f" [WARN] Failed to send device state {state} after 3 attempts.")
                        raise
        except Exception as e:
            print(f"[ERROR] General failure in sending detection to Azure: {e}")

    def azure(self, detection_entry):
        """
        Send a detection entry to Azure Service Bus using the initialized client and sender.
        """
        try:
            # Initialize Azure Service Bus client and queue sender
            queue_sender = self.sb_detect_sender

            # Serialize detection entry
            json_data = json.dumps(detection_entry, indent=4)
            message = ServiceBusMessage(
                body=json_data,
                content_type="application/json",
                subject=detection_entry.get("MainClass", "Unknown")
            )
            # Retry mechanism for sending messages
            for attempt in range(3):  # Maximum 3 attempts
                try:
                    with self._sb_lock:
                        queue_sender.send_messages(message)

                    print(f"Detection {detection_entry['DetectionId']} sent to Azure successfully!")
                    return  # Exit the loop after a successful send
                except Exception as e:
                    print(f"Attempt {attempt + 1}: Failed to send detection: {e}")
                    if attempt == 2:  # Final attempt
                        print(f"Failed to send detection {detection_entry['DetectionId']} after 3 attempts.")
                        raise
        except Exception as e:
            print(f"General failure in sending detection to Azure: {e}")

    # endregion

    # region frame/Video Processing

    def _screenshot_worker(self):
        """
        Background thread that watches GPS coordinates.  When cumulative distance 50 m,
        it takes the latest frame (self._current_frame) and calls upload_screenshot_image().

        The thread starts only if self.gps.gps_enable == True.
        """
        # Give process_frame 1 second to start updating self._current_frame
        time.sleep(1.0)

        while not self._stop_screenshot_thread and self.gps.gps_enable:
            try:
                coords = self.gps.get_coordinates()
                if coords is None:
                    # if we don't have a fix, we wait and retry
                    time.sleep(1.0)
                    continue

                lat, lng = coords

                # If it's the first valid fix, we store the coordinates
                if self._last_ss_coord is None:
                    self._last_ss_coord = (lat, lng)
                    time.sleep(1.0)
                    continue

                # Calculate the Haversine distance from the last called coordinate
                lat1, lon1 = map(radians, self._last_ss_coord)
                lat2, lon2 = map(radians, (lat, lng))
                dlat = lat2 - lat1
                dlon = lon2 - lon1
                a = sin(dlat / 2) ** 2 + cos(lat1) * cos(lat2) * sin(dlon / 2) ** 2
                c = 2 * atan2(sqrt(a), sqrt(1 - a))
                R = 6371000  # Earth radius (m)
                dist = R * c

                self._ss_distance_acc += dist

                # If we have traveled 50 m and there is a ready frame:
                if self._ss_distance_acc >= 50.0 and self._current_frame is not None:
                    frame_copy = self._current_frame.copy()
                    # # We aim not to block the worker, so we send the upload on a new thread
                    # threading.Thread(
                    #     target=upload_screenshot_image,
                    #     args=(frame_copy,),
                    #     daemon=True
                    # ).start()
                    # print("[INFO] Screenshot uploaded (distance 50 m)")

                    # Reset the counters for the next 50 m distance
                    self._last_ss_coord = (lat, lng)
                    self._ss_distance_acc = 0.0

                # Short break before we reread the coordinates
                time.sleep(1.0)
            except Exception as e:
                # Any error (serial, parsing, etc.) we log and retry
                print(f"[GPS Worker ERROR] {e}")
                time.sleep(1.0)

    def process_frame(self, frame, frame_count, sync=False, full_frame=None):
        """
        Processes a video frame (cropped/resized by _prepare_frame) by:
        - object detection + tracking
        - face & plate blurring
        - crosswalk segmentation
        - calling segmentation mask and bounding boxes
        Models run internally at imgsz=960.
        """

        # keep the full-resolution original for JSON/Azure upload
        original = full_frame if full_frame is not None else frame

        # lazy buffers: don't encode anything until at least one eligible detection
        safe_jpg_bytes = None
        full_buf = None

        # # if sync is enabled and GPS is active, update current frame for screenshot thread
        # if sync and self.gps.gps_enable:
        #     self._current_frame = original  # full-res or frame for crop

        # make a copy for drawing annotations
        annotated_frame = frame.copy()

        # OBJECT & COCO DETECTION (crop)
        object_results = self.object_model.track(
            frame, persist=True, tracker=self.tracker_config,
            conf=self.score_threshold, iou=0.5,agnostic_nms=True,  imgsz=960, device=self.device, half=self.fp16
        )[0]

        annotated_full = None

        processed_size = (annotated_frame.shape[1], annotated_frame.shape[0])
        original_size = (original.shape[1], original.shape[0])

        #  capture GPS & timestamp at detection moment (per frame)
        detection_time_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        det_coords = None
        try:
            if not self.gps.gps_enable:
                # static coords fallback
                det_coords = self.gps.get_coordinates()
            else:
                # non-blocking;
                det_coords = self.gps.get_coordinates()
        except Exception:
            det_coords = None



        annotated_frame_blurred = False

        # run seg only if there is Crosswalk or VEHICLES detected
        need_seg = False
        if self.enable_crosswalk_seg:
            for box in object_results.boxes:
                name = self.object_model.names[int(box.cls.cpu().item())]
                if get_main_class(name) == "Crosswalk" or name in VEHICLES:
                    need_seg = True
                    break

        vehicle_mask = None
        if self.enable_crosswalk_seg and need_seg:
            # letterbox the frame to 960×960, preserving aspect ratio
            ih, iw = frame.shape[:2]
            inp_size = 960
            scale = min(inp_size / iw, inp_size / ih)
            nw, nh = int(iw * scale), int(ih * scale)
            resized = cv2.resize(frame, (nw, nh), interpolation=cv2.INTER_LINEAR)
            canvas = np.full((inp_size, inp_size, 3), 114, dtype=np.uint8)
            top, left = (inp_size - nh) // 2, (inp_size - nw) // 2
            canvas[top:top + nh, left:left + nw] = resized

            # run segmentation on the 960×960 canvas
            seg_res = self.seg_model.predict(canvas, conf=0.3, device=self.device, half=self.fp16,imgsz=960)[0]

            # —— GUARD AGAINST NO MASKS ——
            if seg_res.masks is None or seg_res.masks.data is None:
                vehicle_mask = None
            else:
                md = seg_res.masks.data.cpu().numpy()  # (n_masks,960,960)
                cls_ids = seg_res.boxes.cls.cpu().numpy().astype(int)

                # combine all vehicle masks
                masks = [
                    md[i] > 0.5
                    for i, cid in enumerate(cls_ids)
                    if seg_res.names[cid].lower() in VEHICLES
                ]
                if masks:
                    combined = np.any(np.stack(masks, axis=0), axis=0)  # boolean mask 960×960
                    # remove the letterbox padding and resize back to original frame size
                    unpad = combined[top:top + nh, left:left + nw]
                    vehicle_mask = cv2.resize(
                        unpad.astype(np.uint8),
                        (iw, ih),
                        interpolation=cv2.INTER_NEAREST
                    ).astype(bool)
                else:
                    vehicle_mask = None

        # BUILD COLOR MAP FOR VEHICLES & OVERLAPS
        color_map = None
        if (self.show_video or self.save_video) and (vehicle_mask is not None):
            hm, wm = vehicle_mask.shape
            color_map = np.zeros((hm, wm, 3), dtype=np.uint8)
            color_map[vehicle_mask] = VEHICLE_COLOR
            # red - overlap with crosswalk in preview
            for box in object_results.boxes:
                idx = int(box.cls.cpu().item())
                name = self.object_model.names[idx]
                if get_main_class(name) == "Crosswalk":
                    x1, y1, x2, y2 = map(int, box.xyxy[0].tolist())
                    sub = vehicle_mask[y1:y2, x1:x2]
                    if sub.any():
                        region = color_map[y1:y2, x1:x2]
                        region[sub] = OVERLAP_COLOR
                        color_map[y1:y2, x1:x2] = region

        # Build crosswalk_mask only if segmentation is enabled and we have a vehicle_mask
        if self.enable_crosswalk_seg and vehicle_mask is not None:
            crosswalk_mask = np.zeros_like(vehicle_mask, dtype=bool)
            for cw_box in object_results.boxes:
                idx = int(cw_box.cls.cpu().item())
                name = self.object_model.names[idx]
                if get_main_class(name) == "Crosswalk":
                    x1, y1, x2, y2 = map(int, cw_box.xyxy[0].tolist())
                    crosswalk_mask[y1:y2, x1:x2] = True
        else:
            crosswalk_mask = None

        # PROCESS DETECTIONS & SEND TO JSON/AZURE
        detections = []

        for box in object_results.boxes:
            tid = int(box.id.cpu().item()) if box.id is not None else -1

            # Class filters
            cls_idx = int(box.cls.cpu().item())
            class_name = self.object_model.names[cls_idx]
            main_class = get_main_class(class_name)
            sub_class = get_sub_class(class_name)

            # Billboard filter
            if main_class == "Billboard" and not self.send_billboard:
                continue

            # extract mask for each detected vehicle
            mask = None
            if vehicle_mask is not None and class_name in VEHICLES:
                x1, y1, x2, y2 = map(int, box.xyxy[0].tolist())
                sub = vehicle_mask[y1:y2, x1:x2]
                m = np.zeros_like(vehicle_mask, dtype=bool)
                m[y1:y2, x1:x2] = sub
                mask = m

            #CROSS_HISTORY
            is_on_crosswalk = False
            if crosswalk_mask is not None and mask is not None and class_name in VEHICLES:
                overlap_area   = np.logical_and(mask, crosswalk_mask)
                mask_pixels    = np.count_nonzero(mask)
                overlap_pixels = np.count_nonzero(overlap_area)
                is_on_crosswalk = (overlap_pixels / mask_pixels) >= self.overlap_threshold
                self.cross_history.setdefault(tid, collections.deque(maxlen=self.history_maxlen)).append(is_on_crosswalk)

                # # DEBUG
                # if mask_pixels > 0:
                #     print(
                #         f"[OVERLAP DEBUG] Vehicle '{class_name}' (track_id={tid}): overlap={overlap_pixels}/{mask_pixels} ({overlap_pixels / mask_pixels:.2f}), threshold={self.overlap_threshold}, is_on_crosswalk={is_on_crosswalk}")

            # CrosswalkDamaged filter
            if (main_class == "Crosswalk"
                and sub_class == "CrosswalkDamaged"
                and not self.send_crosswalk_damaged
                and not is_on_crosswalk):
                # skip caching/sending non-overlap CrosswalkDamaged when flag is disabled
                continue


            # Ok-subclasses filter
            ignored_ok = False
            if not (sub_class == "CrosswalkOk" and self.enable_crosswalk_seg):
                if ignore_ok_subclasses(main_class, sub_class, self.send_ok, is_on_crosswalk):
                    if main_class == "TrafficSign" and sub_class == "TrafficSignOk":
                        ignored_ok = True
                    else:
                        continue

            # Process box
            data = self.process_box(box, annotated_frame, main_class, self.object_model) #original_frame_buffer=None

            if not data:
                continue


            data["processed_size"] = processed_size
            data["original_size"] = original_size

            # Overwrite detection_data for JSON
            data["main_class"] = main_class
            data["sub_class"] = sub_class
            data['is_on_crosswalk'] = is_on_crosswalk
            data["time"] = detection_time_str
            if det_coords:
                lat, lng = det_coords
                data["gps"] = {"Lat": float(lat), "Lng": float(lng)}
            else:
                data["gps"] = None
            data["_ignored_ok"] = ignored_ok

            base_label = f"{data['sub_class']} (Conf: {data['confidence']:.2f}, Score: {data.get('scoring', 0):.2f})"
            if is_on_crosswalk:
                data['label'] = f"{base_label} [OVERLAP]"
                data['color'] = OVERLAP_COLOR
            else:
                data['label'] = base_label

            # #DEBUG
            # if is_on_crosswalk:
            #     print(
            #         f"[DEBUG] Marked detection {data['guid']} as OVERLAP (main_class={main_class}, sub_class={sub_class})")

            # Confidence threshold per class/subclass (from ClassMapping)
            eff_min_conf = get_min_conf_for_class(sub_class, main_class, self.score_threshold)
            if data.get("confidence", 0.0) < eff_min_conf:
                continue

            detections.append(data)

        # CROSSWALK OVERLAP LOGIC
        if self.enable_crosswalk_seg and vehicle_mask is not None and crosswalk_mask is not None:
            for data in detections:
                if data['main_class'] == "Crosswalk":
                    polygon = data["polygon"]
                    x1, y1 = polygon[0]
                    x2, y2 = polygon[2]
                    crosswalk_box_mask = crosswalk_mask[y1:y2, x1:x2]
                    vehicle_box_mask = vehicle_mask[y1:y2, x1:x2]
                    overlap = np.logical_and(crosswalk_box_mask, vehicle_box_mask)
                    overlap_pixels = np.count_nonzero(overlap)
                    mask_pixels = np.count_nonzero(crosswalk_box_mask)
                    if mask_pixels > 0:
                        overlap_ratio = overlap_pixels / mask_pixels
                        if overlap_ratio >= self.overlap_threshold:
                            data["main_class"] = "CrosswalkOverlap"
                            if data["sub_class"] == "CrosswalkOk":
                                data["sub_class"] = "CrosswalkOkOverlap"
                            elif data["sub_class"] == "CrosswalkDamaged":
                                data["sub_class"] = "CrosswalkDamagedOverlap"
                            data["is_on_crosswalk"] = True
                            data[
                                "label"] = f"{data['sub_class']} (Conf: {data['confidence']:.2f}, Score: {data.get('scoring', 0):.2f}) [OVERLAP]"
                            data["color"] = OVERLAP_COLOR
                            # print(
                            #     f"[CROSSWALK OVERLAP] Crosswalk box ({x1},{y1},{x2},{y2}) overlap: {overlap_pixels}/{mask_pixels} ({overlap_ratio:.2f}) -> marked as OVERLAP")

        # CONFLICT RULE: TrafficSign Ok vs Damaged
        signs = [d for d in detections if d["main_class"] == "TrafficSign"]
        others = [d for d in detections if d["main_class"] != "TrafficSign"]
        if signs:
            signs = self.resolve_sign_conflicts(signs, iou_thr=0.50, ioa_thr=0.70, delta=0.07)


        # after arbitration, if send_ok=False we remove the 'Ok's that were marked as ignored
        signs = [
            d for d in signs
            if not (d.get("_ignored_ok", False) and not self.send_ok and "Overlap" not in d["sub_class"])
        ]
        detections = others + signs

        # assign group IDs
        for idx, data in enumerate(detections, 1):
            data['group_id'] = idx

        # JSON/Azure send
        for data in detections:
            if ignore_ok_subclasses(
                    data['main_class'],
                    data['sub_class'],
                    self.send_ok,
                    data.get('is_on_crosswalk', False)
            ):

                print(f"[IGNORED] Subclass '{data['sub_class']}' detected. Not caching.")
            else:
                # Encode only if the detection is eligible to send (>= DEFAULT_SCORING_THRESHOLD)
                if data.get("scoring", 0) >= DEFAULT_SCORING_THRESHOLD:
                    if safe_jpg_bytes is None:

                        # FACE & LICENSE PLATE BLUR (annotated_frame)
                        if (sync or self.show_video or self.save_video) and not annotated_frame_blurred:
                            coco_results = self.coco_model.predict(
                                frame, conf=0.35, imgsz=960, device=self.device, half=self.fp16,
                                classes=self._coco_keep, stream=False, verbose=False)
                            self.apply_blur(frame, annotated_frame, coco_results)
                            annotated_frame_blurred = True


                        ok_safe, enc_safe = cv2.imencode('.jpg', annotated_frame, [int(cv2.IMWRITE_JPEG_QUALITY), 97]) # 100
                        safe_jpg_bytes = enc_safe.tobytes() if ok_safe else None

                     # FACE & LICENSE PLATE BLUR (annotated_full)
                    if sync and full_buf is None:
                        if annotated_full is None:
                            annotated_full = original.copy()
                            coco_results_full = self.coco_model.predict(
                                original, conf=0.35, imgsz=960, device=self.device, half=self.fp16,
                                classes=self._coco_keep, stream=False, verbose=False)
                            self.apply_blur(original, annotated_full, coco_results_full)

                            if self.camera_crop_enable:
                                cx, cy = self.camera_crop_x, self.camera_crop_y
                                cw, ch = self.camera_crop_width, self.camera_crop_height
                                H, W = annotated_full.shape[:2]
                                x2 = min(cx + cw, W)
                                y2 = min(cy + ch, H)
                                target_w = x2 - cx
                                target_h = y2 - cy
                                if annotated_frame.shape[1] != target_w or annotated_frame.shape[0] != target_h:
                                    patch = cv2.resize(annotated_frame, (target_w, target_h),
                                                       interpolation=cv2.INTER_LINEAR)
                                else:
                                    patch = annotated_frame
                                annotated_full[cy:y2, cx:x2] = patch

                        ok_full, enc_full = cv2.imencode('.jpg', annotated_full,
                                                                 [int(cv2.IMWRITE_JPEG_QUALITY), 97]) # 100
                        full_buf = enc_full.tobytes() if ok_full else None


                    data["image_buffer"] = safe_jpg_bytes
                    if sync and full_buf is not None:
                        data["full_image_buffer"] = full_buf

                # records the current detection in the cache (per GUID)
                # keeps the best image (maximum score), calculates scale-factor, saves meta (main/sub class, polygon, track_id, etc.).
                self.update_json(data, frame_count, sync=sync)

        # start _flush_cache, check which GUIDs have "expired"
        self.update_json(None, frame_count, sync=sync)

        # SEGMENTATION MASK (annotated_frame)
        if (self.show_video or self.save_video) and self.enable_crosswalk_seg and (vehicle_mask is not None) and (color_map is not None):
            hf, wf = annotated_frame.shape[:2]
            mask_up = cv2.resize(
                vehicle_mask.astype(np.uint8),
                (wf, hf),
                interpolation=cv2.INTER_NEAREST
            ).astype(bool)
            cmap_up = cv2.resize(color_map, (wf, hf), interpolation=cv2.INTER_NEAREST)
            overlay = annotated_frame.copy()
            overlay[mask_up] = cmap_up[mask_up]
            annotated_frame = cv2.addWeighted(
                overlay, DEFAULT_MASK_ALPHA,
                annotated_frame, 1 - DEFAULT_MASK_ALPHA,0)


        # DRAW BOUNDING BOXES (annotated_frame)
        if self.show_video or self.save_video:
            drawer = BoundingBoxDrawer()
            annotated_frame = drawer.draw_bounding_boxes(annotated_frame, detections)

        return annotated_frame

    def _prepare_frame(self, frame):
        """
        Preprocess the frame:
        - if camera_enable is True, resize to configured resolution (camera_width x camera_height);
        - if camera_enable is False, keep original resolution;
        then apply crop if camera_crop_enable is True.
        """

        h0, w0 = frame.shape[:2]

        if not self._prep_logged:
            if self.camera_enable:
                if w0 != self.camera_width or h0 != self.camera_height:
                    print(f"[DEBUG] Resizing frame {w0}x{h0} ? {self.camera_width}x{self.camera_height}")
                else:
                    print("[DEBUG] Frame already at target configured resolution, skipping resize")
                if self.camera_crop_enable:
                    print(
                        f"[DEBUG] Cropping ENABLED: size {self.camera_crop_width}x{self.camera_crop_height}, "
                        f"offset ({self.camera_crop_x}, {self.camera_crop_y})"
                    )
            else:
                print("[DEBUG] Camera settings disabled: keeping original resolution")
                if self.camera_crop_enable:
                    print(
                        "[DEBUG] Cropping ENABLED on original resolution: "
                        f"size {self.camera_crop_width}x{self.camera_crop_height}, "
                        f"offset ({self.camera_crop_x}, {self.camera_crop_y})"
                    )
            self._prep_logged = True

        # Only resize if camera_enable==True
        if self.camera_enable and (w0 != self.camera_width or h0 != self.camera_height):
            if not self.camera_crop_enable:
                if self.camera_width < w0 or self.camera_height < h0:
                    frame = cv2.resize(
                        frame, (self.camera_width, self.camera_height),
                        interpolation=cv2.INTER_AREA)


        # Crop only if camera_crop_enable==True
        if self.camera_crop_enable:
            h, w = frame.shape[:2]
            cx = max(0, min(self.camera_crop_x, w - self.camera_crop_width))
            cy = max(0, min(self.camera_crop_y, h - self.camera_crop_height))
            frame = frame[cy:cy + self.camera_crop_height,
                    cx:cx + self.camera_crop_width]

        return frame

    def process_video(self, source=None, use_camera=False, sync=False):
        """
        Video, camera or image processing.
        - use_camera=True -> uses the camera.
        - source="file.mp4" -> processes a video.
        - source="image.jpg/png" -> processes a single image.
        """

        cap = None
        writer_clean, writer_annotated = None, None

        try:

            if use_camera:

                cap = cv2.VideoCapture(self.camera_id, cv2.CAP_V4L2)
                try:
                    cap.set(cv2.CAP_PROP_BUFFERSIZE, 1)
                except Exception:
                    pass


                if self.camera_enable:
                    cap.set(cv2.CAP_PROP_FRAME_WIDTH, self.camera_width)
                    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, self.camera_height)

                cap.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc(*'MJPG'))
                cap.set(cv2.CAP_PROP_FPS, 30)

                # We show the resolution the driver is working with anyway, even if you didn't force it
                w = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
                h = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)
                print(f"[DEBUG] Camera resolution : {int(w)}×{int(h)}")

                writer_clean, writer_annotated = init_video_writers(self.config)


            elif source and os.path.isfile(source):
                ext = source.split('.')[-1].lower()

                # Video file
                if ext in ["mp4", "avi", "mov"]:
                    cap = cv2.VideoCapture(source)
                    writer_clean, writer_annotated = init_video_writers(self.config)

                # Image
                elif ext in ["jpg", "jpeg", "png"]:
                    frame = cv2.imread(source)
                    if frame is None:
                        print(f"Error: Could not load image {source}")
                        return

                    # GPS fix
                    if not self.gps.gps_enable:
                        self.device_state("GPS Manual Disabled")
                    else:
                        if self.gps_drop_if_no_fix:
                            coords = self.gps.get_coordinates()
                            if coords:
                                self.device_state("GPS is Available")
                            else:
                                self.device_state("GPS Not Available")
                        else:
                            while True:
                                coords = self.gps.get_coordinates()
                                if coords:
                                    self.device_state("GPS is Available")
                                    break
                                else:
                                    self.device_state("GPS Not Available")
                                    time.sleep(1)

                    # crop/resize and process single image
                    orig_frame = frame.copy()
                    frame = self._prepare_frame(frame)
                    annotated_frame = self.process_frame(frame, 0, sync, full_frame=orig_frame)

                    save_and_show(
                        frame,
                        annotated_frame,
                        self.config,
                        is_image=True,
                        writer_clean=None,
                        writer_annotated=None
                    )
                    return
                else:
                    print("Unknown format!")
                    return
            else:
                print("Invalid source!")
                return

            ensure_window_initialized(self.show_video, self.config)


            #  FPS & frame skipping
            fps, self.target_fps, self.frame_skip = (
                configure_fps(cap) if cap else configure_fps(None)
            )
            print(f"[STATUS] Original FPS: {fps}, Target FPS: {self.target_fps}, Frame Skip: {self.frame_skip}")

            #  Main loop
            frame_count = 0
            while cap is not None and cap.isOpened():

                # GPS fix per frame
                if not self.gps.gps_enable:
                    self.device_state("GPS Manual Disabled")
                else:
                    if self.gps_drop_if_no_fix:
                        coords = self.gps.get_coordinates()
                        if coords:
                            self.device_state("GPS is Available")
                        else:
                            self.device_state("GPS Not Available")
                    else:
                        while True:
                            coords = self.gps.get_coordinates()
                            if coords:
                                self.device_state("GPS is Available")
                                break
                            else:
                                self.device_state("GPS Not Available")
                                time.sleep(1)

                try:
                    ret, frame = cap.read()
                except cv2.error as e:
                    window_gui_events(self.show_video)
                    continue

                # if no valid frame, skip
                if not ret or frame is None or frame.size == 0:
                    window_gui_events(self.show_video)
                    if use_camera:
                        continue
                    break

                # keep full-res
                full_frame = frame.copy()

                # crop/resize
                frame = self._prepare_frame(frame)

                # DEBUG: full frame + crop (only when show_video=True)
                if self.show_video:
                    show_full_with_crop(full_frame, self.config)

                # Skip frames
                if not use_camera and frame_count % self.frame_skip != 0:
                    frame_count += 1
                    window_gui_events(self.show_video)
                    continue

                # Process & display
                window_gui_events(self.show_video)
                annotated_frame = self.process_frame(frame, frame_count, sync, full_frame=full_frame)
                frame_count += 1

                should_quit = save_and_show(frame, annotated_frame, self.config,is_image=False,writer_clean=writer_clean,writer_annotated=writer_annotated)
                if should_quit:
                    break

            #  Cleanup caches
            self.clear_caches()

        finally:
            if cap is not None:
                cap.release()
                print("Camera/video released!")
            release_video_writers(writer_clean, writer_annotated)
            cv2.destroyAllWindows()
            # self._stop_screenshot_thread = True
            # if hasattr(self, "_screenshot_thread"):
            #     self._screenshot_thread.join(timeout=2.0)
            if hasattr(self, "_send_executor"):
                self._send_executor.shutdown(wait=True, cancel_futures=False)

            try:
                self.sb_detect_sender.close()
                self.sb_state_sender.close()
                self.sb_client.close()
            except Exception as e:
                print("[WARN] Failed to close Service Bus resources:", e)

    # endregion

