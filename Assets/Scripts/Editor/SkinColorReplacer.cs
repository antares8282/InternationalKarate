using UnityEngine;
using UnityEditor;
using System.IO;

public class SkinColorReplacer : Editor
{
    // Dark skin colors from Player1.png (brownish/tan tones)
    private static readonly Color32[] darkSkinColors = new Color32[]
    {
        new Color32(139, 90, 43, 255),   // Dark brown
        new Color32(166, 110, 58, 255),  // Medium brown
        new Color32(184, 126, 70, 255),  // Light brown
        new Color32(198, 143, 89, 255),  // Tan
        new Color32(156, 99, 50, 255),   // Brown variant
        new Color32(172, 115, 62, 255),  // Another brown
    };

    // Pink skin colors from greet0.png (peach/pink tones)
    private static readonly Color32[] pinkSkinColors = new Color32[]
    {
        new Color32(228, 166, 152, 255), // Light pink/peach
        new Color32(216, 136, 120, 255), // Medium pink
        new Color32(200, 116, 100, 255), // Darker pink
        new Color32(240, 184, 168, 255), // Very light pink
        new Color32(224, 152, 136, 255), // Pink variant
        new Color32(212, 144, 128, 255), // Another pink
    };

    [MenuItem("Tools/Replace Skin Color (Player1.png) - Preview")]
    public static void PreviewColors()
    {
        string path = "Assets/Sprites/Characters/Player1.png";

        // Make texture readable
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("Could not find Player1.png");
            return;
        }

        bool wasReadable = importer.isReadable;
        if (!wasReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        Color32[] pixels = tex.GetPixels32();

        // Find unique colors in skin tone range
        var skinColors = new System.Collections.Generic.HashSet<Color32>();
        foreach (var pixel in pixels)
        {
            if (pixel.a > 0 && IsSkinTone(pixel))
            {
                skinColors.Add(pixel);
            }
        }

        Debug.Log($"Found {skinColors.Count} potential skin tone colors:");
        foreach (var c in skinColors)
        {
            Debug.Log($"  RGB({c.r}, {c.g}, {c.b})");
        }

        if (!wasReadable)
        {
            importer.isReadable = false;
            importer.SaveAndReimport();
        }
    }

    private static bool IsSkinTone(Color32 c)
    {
        // Exact match for RGB(152, 90, 77) - no tolerance
        return c.r == 152 && c.g == 90 && c.b == 77;
    }

    [MenuItem("Tools/Replace Skin Color (From Backup) - Execute")]
    public static void ReplaceSkinColor()
    {
        string path = "Assets/Sprites/Characters/Player1.png";
        string backupPath = "Assets/Sprites/Characters/Player1_backup.png";

        // First restore from backup
        string fullBackupPath = Application.dataPath + "/../" + backupPath;
        if (System.IO.File.Exists(fullBackupPath))
        {
            byte[] backupBytes = System.IO.File.ReadAllBytes(fullBackupPath);
            System.IO.File.WriteAllBytes(Application.dataPath + "/../" + path, backupBytes);
            AssetDatabase.Refresh();
            Debug.Log("Restored from backup first");
        }

        // Make texture readable
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("Could not find Player1.png");
            return;
        }

        importer.isReadable = true;
        importer.SaveAndReimport();

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        // Create backup
        byte[] originalBytes = File.ReadAllBytes(Application.dataPath + "/../" + path);
        File.WriteAllBytes(Application.dataPath + "/../" + backupPath, originalBytes);
        Debug.Log($"Backup created at {backupPath}");

        // Get pixels
        Color32[] pixels = tex.GetPixels32();
        int replacedCount = 0;

        // Replace colors
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a == 0) continue;

            Color32 newColor;
            if (TryGetReplacementColor(pixels[i], out newColor))
            {
                pixels[i] = newColor;
                replacedCount++;
            }
        }

        // Create new texture and save
        Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        newTex.SetPixels32(pixels);
        newTex.Apply();

        byte[] pngData = newTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../" + path, pngData);

        AssetDatabase.Refresh();
        Debug.Log($"Replaced {replacedCount} pixels. Original backed up to {backupPath}");
    }

    private static bool TryGetReplacementColor(Color32 original, out Color32 replacement)
    {
        replacement = original;

        // Check if this is a skin tone that needs replacement
        if (!IsSkinTone(original)) return false;

        // Direct replacement: RGB(152,90,77) -> RGB(239,114,138)
        replacement = new Color32(239, 114, 138, original.a);
        return true;
    }

    [MenuItem("Tools/Restore Player1.png from Backup")]
    public static void RestoreBackup()
    {
        string path = "Assets/Sprites/Characters/Player1.png";
        string backupPath = "Assets/Sprites/Characters/Player1_backup.png";

        string fullBackupPath = Application.dataPath + "/../" + backupPath;
        if (!File.Exists(fullBackupPath))
        {
            Debug.LogError("No backup found!");
            return;
        }

        byte[] backupBytes = File.ReadAllBytes(fullBackupPath);
        File.WriteAllBytes(Application.dataPath + "/../" + path, backupBytes);
        AssetDatabase.Refresh();
        Debug.Log("Restored from backup");
    }
}
