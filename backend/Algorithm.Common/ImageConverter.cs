using System.Drawing;

namespace Algorithms.Common {
    public static class ImageConverter {


        public static float[] LoadBmpAsFloatArray(string path) {
            using var bmp = new Bitmap(path);
            // Resize if needed (e.g., 224x224)
            //using var resized = new Bitmap(bmp, new Size(1224, 1224));

            var floats = new float[bmp.Height * bmp.Width * 3];
            int idx = 0;

            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < bmp.Width; x++) {
                    var pixel = bmp.GetPixel(x, y);
                    floats[idx++] = pixel.R / 255f;
                    floats[idx++] = pixel.G / 255f;
                    floats[idx++] = pixel.B / 255f;
                }
            }

            return floats;
        }
    }
}
