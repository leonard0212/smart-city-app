# SmartCityImageService

Serviciu minimal de adnotare imagini pentru SmartCity — expune un singur endpoint,
extras din NLPServices (fara chei sau alte credentiale).

## Endpoint

`POST /v1/adddetectionstoimage` — multipart/form-data:

- `image_file` — imaginea (jpeg/png)
- `detections` — JSON array cu detectii; fiecare punct are `GroupId`, `PointsOrder`,
  `PosX`, `PosY`, `ObjectName`, `Accuracy` (4 puncte per grup = un bounding box)

Raspuns: imaginea PNG cu bounding box-urile si etichetele desenate.

Consumat de `SmartCity.Core\Services\RawDetectionService.cs` (ProcessPictureAiAsync).

## Rulare

```bash
docker compose up -d --build
```

Containerul asculta intern pe 8888, publicat pe **8884** (portul apelat de SmartCity.Core).

Local, fara Docker:

```bash
pip install -r requirements.txt
python app.py   # porneste pe :8888
```

## Note

- CPU-only (Tornado + Pillow, fara inferenta locala).
- Pe Windows foloseste Arial; in container, DejaVu Sans (instalat in Dockerfile).
