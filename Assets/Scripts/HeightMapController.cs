using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class HeightMapController : MonoBehaviour {

    //public Texture2D hMap;
    //public Texture2D soilMap;

    public static float defaultLength = 1024f;
    public static float defaultWidth = 1024f;
    public static float defaultHeight = 300f;
    private float[,] heights;

    public float[,,] splats;

    public GameObject vegetationPrefab;

    enum SoilTexture {
        Clay = 0,
        Sand = 1,
        Rock = 2,
        Urban = 3
    }

    

    //Terrain Building
    public void BuildTerrain(Texture2D hMap) {
        BuildTerrain(hMap, defaultHeight);
    }

    public void BuildTerrain(Texture2D hMap, float customHeight) {
        Debug.Log("Building Terrain");
        TerrainData tdata = this.GetComponent<Terrain>().terrainData;
        heights = new float[hMap.width, hMap.height];

        int z = hMap.width - 1;
        for (int x = 0; x < hMap.width; x++) {
            for (int y = 0; y < hMap.height; y++) {
                heights[z, y] = hMap.GetPixel(x, y).grayscale;
            }
            z--;
        }
        tdata.size = new Vector3(defaultLength, customHeight, defaultWidth);
        tdata.SetHeights(0, 0, heights);
    }

    public void UpdateTerrainHeight(float newHeight) {
        TerrainData tdata = this.GetComponent<Terrain>().terrainData;
        tdata.size = new Vector3(defaultLength, newHeight, defaultWidth);
    }

    public void Flatten(int x1, int x2, int z1, int z2) {
        TerrainData tdata = this.GetComponent<Terrain>().terrainData;
        float averageHeight = ((heights[x1, z1] + heights[x2, z1] + heights[x1, z2] + heights[x2, z2]) / 4);

        for (int y = z1; y < z2; y++) {
            for (int x = x1; x < x2; x++) {
                heights[x, y] = averageHeight;
            }
        }
        tdata.SetHeights(0, 0, heights);
    }

    //Soil Maps
    public void SoilMapLoad(Texture2D soilMap) {
        Debug.Log("loadingSoil");

        TerrainData tdata = this.GetComponent<Terrain>().terrainData;

        float[,,] map = new float[soilMap.width, soilMap.height, 4];

        int z = soilMap.width - 1;
        for (int x = 0; x < soilMap.width; x++) {
            for (int y = 0; y < soilMap.height; y++) {

                int targetTexture = 0;

                if (soilMap.GetPixel(x, y).grayscale > 0.18f && soilMap.GetPixel(x, y).grayscale < 0.22f) {
                    targetTexture = (int)SoilTexture.Clay;
                } else if (soilMap.GetPixel(x, y).grayscale > 0.38f && soilMap.GetPixel(x, y).grayscale < 0.42f) {
                    targetTexture = (int)SoilTexture.Sand;
                } else if (soilMap.GetPixel(x, y).grayscale > 0.58f && soilMap.GetPixel(x, y).grayscale < 0.62f) {
                    targetTexture = (int)SoilTexture.Rock;
                } else if (soilMap.GetPixel(x, y).grayscale > 0.78f && soilMap.GetPixel(x, y).grayscale < 0.82f) {
                    targetTexture = (int)SoilTexture.Urban;
                }
                map[z, y, targetTexture] = 1f;
            }
            z--;
        }
        splats = map;
        tdata.SetAlphamaps(0, 0, map);
    }

    public float[,,] GetSplatMap() {

        TerrainData tdata = this.GetComponent<Terrain>().terrainData;
        return tdata.GetAlphamaps(0, 0, tdata.alphamapWidth, tdata.alphamapHeight);
    }


    //Vegetation Maps

    public void VegetationMapLoad(Texture2D vMap) {
        TerrainData tdata = this.GetComponent<Terrain>().terrainData;
        Debug.Log("loadingVeg");
        float[,] map = new float[vMap.width, vMap.height];

        for (int x = 0; x < vMap.width; x+=10) {
            for (int y = 0; y < vMap.height; y+=10) {
                if (vMap.GetPixel(x, y).grayscale < 0.1f) {
                    map[x, y] = 1f;
                    Vector3 position = new Vector3(x, tdata.GetHeight(x,y), y);
                    Instantiate(vegetationPrefab, position, Quaternion.identity);
                }
            }
        }
    }
}
