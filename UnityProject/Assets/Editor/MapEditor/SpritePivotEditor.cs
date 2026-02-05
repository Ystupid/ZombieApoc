using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SpritePivotEditor
{
    [MenuItem("Assets/Set Auto Pivot", false, 20)]
    public static void SetGridAutoPivot()
    {
        var selecteds = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        foreach (var texture in selecteds)
        {
            SetTextureAutoPivot(new Vector2(16, 16), texture);
        }
    }

    public static void SetTextureAutoPivot(Vector2 gridSize, Texture2D texture)
    {
        var assetPath = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(assetPath);
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();

        var spriteRects = provider.GetSpriteRects();

        foreach (var spriteRect in spriteRects)
        {
            spriteRect.pivot = new Vector2
            {
                x = 0.5f,
                y = gridSize.y / 2 / texture.height
            };
            spriteRect.alignment = SpriteAlignment.Custom;
        }

        provider.SetSpriteRects(spriteRects);
        provider.Apply();

        importer.SaveAndReimport();

    }
}
