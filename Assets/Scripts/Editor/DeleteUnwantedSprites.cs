using UnityEngine;
using UnityEditor;
using System.Linq;

public class DeleteUnwantedSprites
{
    [MenuItem("Tools/Delete Player1_* Sprites")]
    public static void DeleteUnwantedSpriteSlices()
    {
        string spritePath = "Assets/Sprites/Characters/Player1.png";

        // Get the texture importer
        TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("Could not find texture importer for " + spritePath);
            return;
        }

        if (importer.spriteImportMode != SpriteImportMode.Multiple)
        {
            Debug.LogError("Sprite is not in Multiple mode");
            return;
        }

        // Get current sprite metadata
        SpriteMetaData[] originalSprites = importer.spritesheet;

        // Filter out unwanted sprites (Player1_*, Unused, Bonus_*)
        SpriteMetaData[] filteredSprites = originalSprites
            .Where(sprite => !sprite.name.StartsWith("Player1_")
                          && sprite.name != "Unused"
                          && !sprite.name.StartsWith("Bonus_"))
            .ToArray();

        int removedCount = originalSprites.Length - filteredSprites.Length;

        Debug.Log($"Original sprite count: {originalSprites.Length}");
        Debug.Log($"Removing {removedCount} unwanted sprites");
        Debug.Log($"New sprite count: {filteredSprites.Length}");

        // Update the sprite sheet metadata
        importer.spritesheet = filteredSprites;

        // Save and reimport
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log("Sprite cleanup completed successfully!");
    }
}
