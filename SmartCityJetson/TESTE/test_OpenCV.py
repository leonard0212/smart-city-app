import cv2
import numpy as np

print(cv2.__version__)
#print(cv2.getBuildInformation())


image = np.zeros((512, 512, 3), dtype=np.uint8)

cv2.circle(image, (256, 256), 100, (255, 0, 0), -1)


cv2.imshow("Test Image", image)
cv2.waitKey(0)
cv2.destroyAllWindows()
