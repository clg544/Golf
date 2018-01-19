using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTerrainScript : MonoBehaviour {

    public int x, y, z;

    MapToHeightmap heightmapMaker;
    
    float[,] heightmap;
    Area[,] groundmap;
    
    public Terrain textureTerrain;
    TerrainData myData;
    float[,,] myAlpha;


    /**
     *  void PaintTerrain: Generate and apply a splatmap and heightmap to the terrain attached to this script.
     *  
     *      float[,] heightmap:     the heightmap to apply
     *      enum Area[,] groundmap: the ground type array that chooses what texture to pick at each point
     *      int size:               the length and width of both arrays
     */
    public void PaintTerrain(float[,] heightmap, Area[,] groundmap, int size)
    {
        this.heightmap = heightmap;
        this.groundmap = groundmap;

        // Set heightmap
        myData.SetHeights(0, 0, heightmap);

        // define splatmap based on ground array
        for(int y = 0; y < myData.alphamapHeight - 1; y++)
        {
            for(int x = 0; x < myData.alphamapWidth - 1; x++)
            {
                switch (groundmap[x, y])
                {
                    case (Area.GREEN):
                        myAlpha[y, x, 0] = 1.0f;
                        break;
                    case (Area.FAIRWAY):
                        myAlpha[y, x, 3] = 1.0f;
                        break;
                    case (Area.ROUGH):
                        myAlpha[y, x, 1] = 1.0f;
                        break;
                    case (Area.EXTRA_ROUGH):
                        myAlpha[y, x, 2] = 1.0f;
                        break;
                    case (Area.SAND):
                        myAlpha[y, x, 4] = 1.0f;
                        break;
                }
            }
        }

        myData.SetAlphamaps(0, 0, myAlpha);
    }

    void Awake()
    {
        heightmapMaker = gameObject.GetComponent<MapToHeightmap>();
        myData = textureTerrain.terrainData;

        myAlpha = new float[myData.alphamapWidth, myData.alphamapHeight, myData.alphamapLayers];
    }
}
