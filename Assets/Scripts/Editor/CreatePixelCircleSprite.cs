using UnityEngine;
using UnityEditor;

namespace InternationalKarate.Editor
{
    public class CreatePixelCircleSprite
    {
        [MenuItem("Tools/Create Pixelated Circle Sprite")]
        public static void CreateSprite()
        {
            // Create a small 16x16 texture for a pixelated circle
            int size = 16;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point; // No smoothing - pixelated!

            // Define circle pixels (8-bit style circle)
            Color transparent = new Color(0, 0, 0, 0);
            Color white = Color.white;

            // Initialize all pixels to transparent
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            // Draw a pixelated circle (C64 style)
            // This pattern creates a nice C64-like circle
            int[][] circlePixels = new int[][]
            {
                // Format: {x, y}
                // Row 0 (top)
                new int[] {6, 0}, new int[] {7, 0}, new int[] {8, 0}, new int[] {9, 0},
                // Row 1
                new int[] {5, 1}, new int[] {6, 1}, new int[] {7, 1}, new int[] {8, 1}, new int[] {9, 1}, new int[] {10, 1},
                // Row 2
                new int[] {4, 2}, new int[] {5, 2}, new int[] {6, 2}, new int[] {7, 2}, new int[] {8, 2}, new int[] {9, 2}, new int[] {10, 2}, new int[] {11, 2},
                // Row 3
                new int[] {3, 3}, new int[] {4, 3}, new int[] {5, 3}, new int[] {6, 3}, new int[] {7, 3}, new int[] {8, 3}, new int[] {9, 3}, new int[] {10, 3}, new int[] {11, 3}, new int[] {12, 3},
                // Row 4-11 (middle section - full width)
                new int[] {3, 4}, new int[] {4, 4}, new int[] {5, 4}, new int[] {6, 4}, new int[] {7, 4}, new int[] {8, 4}, new int[] {9, 4}, new int[] {10, 4}, new int[] {11, 4}, new int[] {12, 4},
                new int[] {3, 5}, new int[] {4, 5}, new int[] {5, 5}, new int[] {6, 5}, new int[] {7, 5}, new int[] {8, 5}, new int[] {9, 5}, new int[] {10, 5}, new int[] {11, 5}, new int[] {12, 5},
                new int[] {3, 6}, new int[] {4, 6}, new int[] {5, 6}, new int[] {6, 6}, new int[] {7, 6}, new int[] {8, 6}, new int[] {9, 6}, new int[] {10, 6}, new int[] {11, 6}, new int[] {12, 6},
                new int[] {3, 7}, new int[] {4, 7}, new int[] {5, 7}, new int[] {6, 7}, new int[] {7, 7}, new int[] {8, 7}, new int[] {9, 7}, new int[] {10, 7}, new int[] {11, 7}, new int[] {12, 7},
                new int[] {3, 8}, new int[] {4, 8}, new int[] {5, 8}, new int[] {6, 8}, new int[] {7, 8}, new int[] {8, 8}, new int[] {9, 8}, new int[] {10, 8}, new int[] {11, 8}, new int[] {12, 8},
                new int[] {3, 9}, new int[] {4, 9}, new int[] {5, 9}, new int[] {6, 9}, new int[] {7, 9}, new int[] {8, 9}, new int[] {9, 9}, new int[] {10, 9}, new int[] {11, 9}, new int[] {12, 9},
                new int[] {3, 10}, new int[] {4, 10}, new int[] {5, 10}, new int[] {6, 10}, new int[] {7, 10}, new int[] {8, 10}, new int[] {9, 10}, new int[] {10, 10}, new int[] {11, 10}, new int[] {12, 10},
                new int[] {3, 11}, new int[] {4, 11}, new int[] {5, 11}, new int[] {6, 11}, new int[] {7, 11}, new int[] {8, 11}, new int[] {9, 11}, new int[] {10, 11}, new int[] {11, 11}, new int[] {12, 11},
                // Row 12
                new int[] {3, 12}, new int[] {4, 12}, new int[] {5, 12}, new int[] {6, 12}, new int[] {7, 12}, new int[] {8, 12}, new int[] {9, 12}, new int[] {10, 12}, new int[] {11, 12}, new int[] {12, 12},
                // Row 13
                new int[] {4, 13}, new int[] {5, 13}, new int[] {6, 13}, new int[] {7, 13}, new int[] {8, 13}, new int[] {9, 13}, new int[] {10, 13}, new int[] {11, 13},
                // Row 14
                new int[] {5, 14}, new int[] {6, 14}, new int[] {7, 14}, new int[] {8, 14}, new int[] {9, 14}, new int[] {10, 14},
                // Row 15 (bottom)
                new int[] {6, 15}, new int[] {7, 15}, new int[] {8, 15}, new int[] {9, 15},
            };

            // Set the circle pixels
            foreach (int[] pixel in circlePixels)
            {
                texture.SetPixel(pixel[0], pixel[1], white);
            }

            texture.Apply();

            // Save the texture
            byte[] bytes = texture.EncodeToPNG();
            string path = "Assets/Sprites/PixelCircle.png";

            // Create Sprites folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
            {
                AssetDatabase.CreateFolder("Assets", "Sprites");
            }

            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();

            // Configure the texture import settings
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Point; // Pixelated look
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spritePixelsPerUnit = 16; // Match texture size
                importer.mipmapEnabled = false;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log("Pixelated circle sprite created at: " + path);
        }
    }
}
