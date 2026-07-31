import os
import cv2

def ensure_window_initialized(show_video: bool, config=None):
    """Create the HighGUI window and place it right."""
    if not show_video:
        return
    if not hasattr(ensure_window_initialized, "_win_init"):
        try:
            cv2.namedWindow("Processed Frame", cv2.WINDOW_NORMAL)

            # dimensions from config (fallback 960x960)
            w = h = 960
            if config is not None and "CAMERA" in config:
                cam = config["CAMERA"]
                w = cam.getint("camera_crop_width", 960)
                h = cam.getint("camera_crop_height", 960)

            cv2.resizeWindow("Processed Frame", w, h)

            # move the window to the right
            try:
                import tkinter as tk
                root = tk.Tk(); root.withdraw()
                sw = root.winfo_screenwidth()

                x = max(0, sw - w - 24)
                y = 24
                cv2.moveWindow("Processed Frame", x, y)
            except Exception:
                pass

        except cv2.error:
            return
        ensure_window_initialized._win_init = True


def window_gui_events(show_video: bool):
    """Process window events (move/close) without blocking the loop."""
    if show_video:
        try:
            cv2.waitKey(1)
        except cv2.error:
            pass

def show_full_with_crop(full_frame, config):
    """
    Show full frame + crop rectangle in a separate window.
    """
    debug_cfg = config["DEBUG"]
    cam_cfg   = config["CAMERA"]

    if not debug_cfg.getboolean("show_video", False):
        return
    if not cam_cfg.getboolean("camera_crop_enabled", False):
        return

    camera_enable = cam_cfg.getboolean("camera_settings_enabled", False)
    cam_w  = cam_cfg.getint("camera_width", 1920)
    cam_h  = cam_cfg.getint("camera_height", 1080)

    x = cam_cfg.getint("camera_crop_x", 0)
    y = cam_cfg.getint("camera_crop_y", 0)
    w = cam_cfg.getint("camera_crop_width",  960)
    h = cam_cfg.getint("camera_crop_height", 960)

    # Bring full_frame into the same coordinate space as the camera settings
    debug_base = full_frame
    if camera_enable:
        if (debug_base.shape[1] != cam_w) or (debug_base.shape[0] != cam_h):
            debug_base = cv2.resize(debug_base, (cam_w, cam_h))

    H, W = debug_base.shape[:2]
    x = max(0, min(x, W - 1))
    y = max(0, min(y, H - 1))
    w = max(1, min(w, W - x))
    h = max(1, min(h, H - y))

    dbg = debug_base.copy()
    cv2.rectangle(dbg, (x, y), (x + w, y + h), (0, 255, 255), 2)

    if not hasattr(show_full_with_crop, "_win_init"):
        try:
            cv2.namedWindow("Full + CropBox", cv2.WINDOW_NORMAL)
            cv2.resizeWindow("Full + CropBox", w, h)
            try:
                import tkinter as tk
                root = tk.Tk();
                root.withdraw()
                cv2.moveWindow("Full + CropBox", 24, 24)
            except Exception:
                pass
        except cv2.error:
            return
        show_full_with_crop._win_init = True

    try:
        cv2.imshow("Full + CropBox", dbg)
        cv2.waitKey(1)
    except cv2.error:
        pass

class RotatingVideoWriter:
    def __init__(self, save_path, prefix, ext, max_files, max_size_mb,
                 fourcc, fps, frame_size):
        """
        save_path   : directory where videos are saved
        prefix      : filename prefix (e.g. "clean", "annot")
        ext         : file extension (e.g. "mp4")
        max_files   : maximum number of files before cycling (e.g. 10)
        max_size_mb : per-file size limit in MB (e.g. 500)
        fourcc, fps, frame_size : parameters for VideoWriter
        """
        self.save_path   = save_path
        self.prefix      = prefix
        self.ext         = ext
        self.max_files   = max_files
        self.max_bytes   = max_size_mb * 1024 * 1024
        self.fourcc      = fourcc
        self.fps         = fps
        self.frame_size  = frame_size

        os.makedirs(self.save_path, exist_ok=True)
        self.idx    = 1
        self.writer = self._open_writer(self.idx)



    def _make_filepath(self, idx):
        name = f"{self.prefix}_{idx}.{self.ext}"
        return os.path.join(self.save_path, name)

    def _open_writer(self, idx):
        path = self._make_filepath(idx)
        # If file exists, it will be overwritten automatically
        return cv2.VideoWriter(path, self.fourcc, self.fps, self.frame_size)

    def write(self, frame):
        # Write the frame
        self.writer.write(frame)
        # Check current file size
        path = self._make_filepath(self.idx)
        try:
            size = os.path.getsize(path)
        except OSError:
            size = 0
        # If exceeded max_bytes, rotate to next file
        if size >= self.max_bytes:
            self.writer.release()
            # advance index cyclically
            self.idx = self.idx + 1 if self.idx < self.max_files else 1
            self.writer = self._open_writer(self.idx)

    def release(self):
        self.writer.release()


# --- In init_video_writers() ---

def init_video_writers(config):
    debug_cfg  = config["DEBUG"]
    camera_cfg = config["CAMERA"]

    if not debug_cfg.getboolean("save_video", fallback=False):
        return None, None

    save_path    = debug_cfg.get("save_video_path", fallback="Saved_Video")
    prefix_clean = debug_cfg.get("prefix_clean",      fallback="Clean")
    prefix_ann   = debug_cfg.get("prefix_annotated",  fallback="Annotated")

    width  = camera_cfg.getint("camera_crop_width",  fallback=960)
    height = camera_cfg.getint("camera_crop_height", fallback=960)
    fps    = debug_cfg.getint("save_fps",             fallback=10)
    fourcc = cv2.VideoWriter_fourcc(*"mp4v")

    # Rotation parameters
    max_files   = debug_cfg.getint("max_video_files", fallback=10)
    max_size_mb = debug_cfg.getint("max_size_mb",     fallback=500)

    writer_clean     = RotatingVideoWriter(
        save_path, prefix_clean.lower(), "mp4",
        max_files, max_size_mb,
        fourcc, fps, (width, height)
    )
    writer_annotated = RotatingVideoWriter(
        save_path, prefix_ann.lower(), "mp4",
        max_files, max_size_mb,
        fourcc, fps, (width, height)
    )
    return writer_clean, writer_annotated

def save_and_show(
    frame,
    annotated_frame,
    config,
    is_image=False,
    writer_clean=None,
    writer_annotated=None
):
    debug_cfg  = config["DEBUG"]
    save_video = debug_cfg.getboolean("save_video", fallback=False)
    show_video = debug_cfg.getboolean("show_video", fallback=False)
    save_path  = debug_cfg.get("save_video_path", fallback="Saved_Video")
    prefix_ann = debug_cfg.get("prefix_annotated", fallback="Annotated")

    if is_image and save_video:
        os.makedirs(save_path, exist_ok=True)
        cv2.imwrite(os.path.join(save_path, f"{prefix_ann.lower()}.jpg"), annotated_frame)

    if not is_image and save_video and writer_clean and writer_annotated:
        writer_clean.write(frame)
        writer_annotated.write(annotated_frame)

    if not show_video:
        return False

    try:
        cv2.imshow("Processed Frame", annotated_frame)
    except cv2.error:
        return False


    key = cv2.waitKey(0 if is_image else 1) & 0xFF
    return key in (27, ord('q'))


def release_video_writers(writer_clean, writer_annotated):
    """
    Release and close any open video writers.
    """
    if writer_clean:
        writer_clean.release()
    if writer_annotated:
        writer_annotated.release()













