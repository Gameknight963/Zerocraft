using UnityEngine;

namespace mszcubemod
{
    public static class BlockPreviewRenderer
    {
        public static Texture2D CreateIsometricPreview(IBlockTexture blockTexture, int size = 64)
        {
            // size=64, half=32
            int half = size / 2;
            int width = size;
            int height = size;

            // Tuning variables

            // topSlant is the vertical rise of top face per unit of u+v
            float topSlant = size / 6f;

            // sideHeight is the vertical height of side faces
            float sideHeight = size / 6f * 4f;

            // Derived
            int totalHeight = Mathf.RoundToInt(topSlant * 2 + sideHeight);
            int padding = (height - totalHeight) / 2;
            int seam = padding + Mathf.RoundToInt(sideHeight);

            Texture2D result = new(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

            Color[] topPixels = SampleTexture(blockTexture.Top, size, size);
            Color[] leftPixels = SampleTexture(blockTexture.Left, size, size);
            Color[] rightPixels = SampleTexture(blockTexture.Right, size, size);

            // Top face
            for (int pu = 0; pu < size; pu++)
            {
                for (int pv = 0; pv < size; pv++)
                {
                    float u = pu / (float)(size - 1);
                    float v = pv / (float)(size - 1);
                    int sx = Mathf.RoundToInt((u - v) * half + half);
                    int sy = Mathf.RoundToInt(seam + (u + v) * topSlant);
                    if (sx >= 0 && sx < width && sy >= 0 && sy < height)
                        BlendPixel(pixels, width, sx, sy, topPixels[pv * size + pu]);
                }
            }

            // Left face
            for (int pu = 0; pu < size; pu++)
            {
                for (int pv = 0; pv < size; pv++)
                {
                    float u = pu / (float)(size - 1);
                    float v = pv / (float)(size - 1);
                    int sx = Mathf.RoundToInt(u * half);
                    int sy = Mathf.RoundToInt(seam + (1f - u) * topSlant - v * sideHeight);
                    if (sx >= 0 && sx < width && sy >= 0 && sy < height)
                    {
                        Color source = leftPixels[pv * size + pu];
                        Color c = source * 0.75f;
                        c.a = source.a;
                        BlendPixel(pixels, width, sx, sy, c);
                    }
                }
            }

            // Right face
            for (int pu = 0; pu < size; pu++)
            {
                for (int pv = 0; pv < size; pv++)
                {
                    float u = pu / (float)(size - 1);
                    float v = pv / (float)(size - 1);
                    int sx = Mathf.RoundToInt(half + u * half);
                    int sy = Mathf.RoundToInt(seam + u * topSlant - v * sideHeight);
                    if (sx >= 0 && sx < width && sy >= 0 && sy < height)
                    {
                        Color source = rightPixels[pv * size + pu];
                        Color c = source * 0.88f;
                        c.a = source.a;
                        BlendPixel(pixels, width, sx, sy, c);
                    }
                }
            }

            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        public static Sprite CreateIsometricSprite(IBlockTexture blockTexture, int size = 64)
        {
            Texture2D preview = CreateIsometricPreview(blockTexture, size);
            return Sprite.Create(preview, new Rect(0, 0, preview.width, preview.height), new Vector2(0.5f, 0.5f));
        }

        private static Color[] SampleTexture(Texture2D src, int w, int h)
        {
            Color[] result = new Color[w * h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    result[y * w + x] = src.GetPixelBilinear(x / (float)w, y / (float)h);
            return result;
        }

        private static void BlendPixel(Color[] pixels, int width, int x, int y, Color src)
        {
            if (src.a <= 0f) return;
            int i = y * width + x;
            Color dst = pixels[i];
            float a = src.a + dst.a * (1 - src.a);
            if (a <= 0f) return;
            pixels[i] = new Color(
                (src.r * src.a + dst.r * dst.a * (1 - src.a)) / a,
                (src.g * src.a + dst.g * dst.a * (1 - src.a)) / a,
                (src.b * src.a + dst.b * dst.a * (1 - src.a)) / a,
                a
            );
        }
    }
}
