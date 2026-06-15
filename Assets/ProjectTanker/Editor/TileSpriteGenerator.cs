using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tools → ProjectTanker → Generate Tile Sprites を実行すると
/// 三角タイル 4 方向のスプライト PNG と Tile アセットを自動生成する。
/// </summary>
public static class TileSpriteGenerator
{
    private const int    Size         = 128;
    private const string TileFolder   = "Assets/ProjectTanker/Art/Tiles";

    [MenuItem("Tools/ProjectTanker/Generate Tile Sprites")]
    public static void Generate()
    {
        string absFolder = Application.dataPath + "/ProjectTanker/Art/Tiles";
        Directory.CreateDirectory(absFolder);

        // ──────────────────────────────────────────
        // 三角タイル 4 方向（直角の位置で命名）
        // BL = 直角が左下、BR = 右下、TL = 左上、TR = 右上
        // ──────────────────────────────────────────
        var triangles = new (string name, Vector2 a, Vector2 b, Vector2 c)[]
        {
            ("Triangle_BL", new Vector2(0,0), new Vector2(1,0), new Vector2(0,1)),
            ("Triangle_BR", new Vector2(1,0), new Vector2(0,0), new Vector2(1,1)),
            ("Triangle_TL", new Vector2(0,1), new Vector2(0,0), new Vector2(1,1)),
            ("Triangle_TR", new Vector2(1,1), new Vector2(1,0), new Vector2(0,1)),
        };

        foreach (var (name, a, b, c) in triangles)
            SavePng(absFolder, name, BuildTriangleTex(a, b, c));

        AssetDatabase.Refresh();

        // インポート設定を Sprite に変更
        foreach (var (name, _, _, _) in triangles)
            ApplySpriteImportSettings($"{TileFolder}/{name}.png");

        AssetDatabase.Refresh();

        // Tile アセットを生成（スプライトをアサイン済み）
        foreach (var (name, _, _, _) in triangles)
            CreateTileAsset(name);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[TileSpriteGenerator] 三角タイル 4 種（PNG + Tile アセット）を生成しました。\n" +
                  $"保存先: {TileFolder}");
    }

    // ── テクスチャ生成 ──────────────────────────────

    static Texture2D BuildTriangleTex(Vector2 a, Vector2 b, Vector2 c)
    {
        var tex    = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        var pixels = new Color[Size * Size];

        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            float u = (x + 0.5f) / Size;
            float v = (y + 0.5f) / Size;
            pixels[y * Size + x] = PointInTriangle(new Vector2(u, v), a, b, c)
                ? Color.white
                : Color.clear;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = Cross(p - a, b - a);
        float d2 = Cross(p - b, c - b);
        float d3 = Cross(p - c, a - c);
        return !((d1 < 0 || d2 < 0 || d3 < 0) && (d1 > 0 || d2 > 0 || d3 > 0));
    }

    static float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;

    // ── ファイル保存 ────────────────────────────────

    static void SavePng(string folder, string name, Texture2D tex)
    {
        File.WriteAllBytes(Path.Combine(folder, $"{name}.png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    // ── インポート設定 ──────────────────────────────

    static void ApplySpriteImportSettings(string assetPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null) return;

        importer.textureType          = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit  = Size;
        importer.filterMode           = FilterMode.Bilinear;
        importer.alphaIsTransparency  = true;
        importer.mipmapEnabled        = false;

        // アトラスパッキングを無効化（UV が 0-1 になるよう保証）
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);

        importer.SaveAndReimport();
    }

    // ── Tile アセット生成 ───────────────────────────

    static void CreateTileAsset(string spriteName)
    {
        string spritePath = $"{TileFolder}/{spriteName}.png";
        string tilePath   = $"{TileFolder}/{spriteName}_Tile.asset";

        if (File.Exists(Path.GetFullPath(tilePath.Replace("Assets/", Application.dataPath + "/"))))
        {
            Debug.Log($"[TileSpriteGenerator] {tilePath} は既に存在するためスキップ。");
            return;
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"[TileSpriteGenerator] スプライトが見つかりません: {spritePath}");
            return;
        }

        var tile             = ScriptableObject.CreateInstance<Tile>();
        tile.sprite          = sprite;
        tile.colliderType    = Tile.ColliderType.Sprite;

        AssetDatabase.CreateAsset(tile, tilePath);
    }
}
