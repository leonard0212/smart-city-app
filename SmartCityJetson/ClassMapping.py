OBJ_MAPPING = {
    "Trash": [
        "TrashBagUncollected",
        "TrashCanUncollected",
        "TrashCanOk",
        "TrashDumpstersUncollected",
        "TrashDumpstersOk",
        "TrashUncollected",
    ],
    "Pothole": ["Pothole"],
    "Billboard": ["Billboard"],
    "TrafficSign": [
        "TrafficSignDamaged",
        "TrafficSignOk",
        "TrafficSignObstructed",
    ],
   "Crosswalk": [
        "CrosswalkOk",
        "CrosswalkDamaged",
    ],
}

# COCO dataset mapping
VEHICLES = ["car", "bus", "truck", "train", "motorcycle"]
COCO_MAPPING = {"person": ["face"], **{vehicle: ["license_plate"] for vehicle in VEHICLES}}

# Combined class mapping
CLASS_MAPPING = {**OBJ_MAPPING, **COCO_MAPPING}


# Threshold on SUBCLASS
CONF_THRESHOLDS_SUB = {


    # ——— Trash ———
    "TrashBagUncollected": 0.86,
    "TrashCanUncollected": 0.86,
    # "TrashCanOk": 0.80,
    "TrashDumpstersUncollected": 0.86,
    # "TrashDumpstersOk": 0.80,
    "TrashUncollected": 0.86,

    # ——— Pothole ———
    "Pothole": 0.86,

    # ——— Billboard ———
    # "Billboard": 0.85,

    # ——— Traffic signs ———
    "TrafficSignDamaged": 0.90,
    #"TrafficSignOk": 0.72,
    "TrafficSignObstructed": 0.86,

    # ——— Crosswalk ———
    # "CrosswalkOk": 0.80,
    # "CrosswalkDamaged": 0.70,
}

# Threshold on MAIN – applies to all subclasses of that main
CONF_THRESHOLDS_MAIN = {
    # "Trash": 0.58,
    # "Pothole": 0.55,
    # "Billboard": 0.60,
    # "TrafficSign": 0.60,
    # "Crosswalk": 0.58,

}

def get_min_conf_for_class(sub_class: str, main_class: str, default_threshold: float) -> float:
    """
Returns the confidence threshold for a detection:
-  if there is a threshold on the subclass -> uses it
- otherwise if there is a threshold on the main class -> uses it
- otherwise -> uses the global threshold (default_threshold)
    """
    if sub_class in CONF_THRESHOLDS_SUB:
        return float(CONF_THRESHOLDS_SUB[sub_class])
    if main_class in CONF_THRESHOLDS_MAIN:
        return float(CONF_THRESHOLDS_MAIN[main_class])
    return float(default_threshold)


def get_main_class(class_name: str) -> str:
    """
    Returns the main class if the input is a subclass.
    If the input is a main class, returns the class name itself.
      Args:
        class_name (str) = input ->  The name of the class to find the main class for.
    """
    for main_class, sub_class in CLASS_MAPPING.items():
        # If the class name is found in the subclass list, return the main class
        if class_name in sub_class:
            return main_class
    return class_name  # If it's already a main class or not found, return it as is


def get_sub_class(class_name: str) -> str:
    """
    Returns the subclass name if found inside class_mapping.
    If not found, returns the class_name itself.
    Args:
        class_name (str) = input -> The name of the class to find the subclass for.
    """
    for sub_class in CLASS_MAPPING.values():
        if class_name in sub_class:
            return class_name # Found the exact subclass
    return class_name  # If it's not in the subclass list, return it as is



def ignore_ok_subclasses(
    main_class: str,
    subclass_name: str,
    send_ok: bool,
    is_on_crosswalk: bool
) -> bool:
    """
    Decide if a subclass ending with 'Ok' should be ignored:
      - If send_ok=True, do not ignore any.
      - CrosswalkOk is not ignored if it's overlapping.
      - All other 'Ok' subclasses are ignored when send_ok=False.

    Args:
        main_class (str): the main class (e.g., "Crosswalk").
        subclass_name (str): the subclass (e.g., "CrosswalkOk").
        send_ok (bool): global flag from config (send_ok_images).
        is_on_crosswalk (bool): whether overlap was detected.
    """
    # If any Overlap in subclass, NEVER ignore!
    if "Overlap" in subclass_name:
        return False

    # If we allow all 'Ok' subclasses, do not ignore
    if send_ok:
        return False

    # Allow CrosswalkOk if overlapping (legacy, but will be caught by Overlap above)
    if main_class == "Crosswalk" and subclass_name == "CrosswalkOk" and is_on_crosswalk:
        return False

    # Otherwise, ignore any subclass ending with 'Ok'
    return subclass_name.endswith("Ok")

