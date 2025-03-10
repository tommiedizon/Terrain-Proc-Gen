﻿using UnityEngine;

public static class TextureGenerator {
    public static int Convert2DIndexTo1D(int x, int y, int width) => y * width + x;
    public static Texture2D GetTextureFromColorMap(Color[] colorMap, int mapWidth, int mapHeight) {
        var texture = new Texture2D(mapWidth, mapHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colorMap); 
        texture.Apply();
        return texture;
    }

    public static Texture2D GetTextureFromHeightMap(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height]; //1D array

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // convert 2D array into 1D representation (think it through)
                colorMap[Convert2DIndexTo1D(x, y, width)] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return GetTextureFromColorMap(colorMap, width, height);
    }
}