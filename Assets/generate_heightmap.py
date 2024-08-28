import cv2
import numpy as np

def generate_heightmap(input_image_path, output_image_path):
    print("Generating heightmap from image:", input_image_path)

    img = cv2.imread(input_image_path)
    gray_img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    heightmap = cv2.normalize(gray_img, None, 0, 255, cv2.NORM_MINMAX)
    cv2.imwrite(output_image_path, heightmap)

if __name__ == "__main__":
    import sys
    input_image_path = sys.argv[1]
    output_image_path = "models/heightmap.png"
    generate_heightmap(input_image_path, output_image_path)
