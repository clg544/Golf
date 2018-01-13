using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTerrain : MonoBehaviour {

    public int tileSize;
    public int heightScale;
    public float overlapHeight;   // How far the dominate terrain lifts above non-active terrains

    public GameObject GreenTile;
    public GameObject FairwayTile;
    public GameObject RoughTile;
    public GameObject ExtraRoughTile;
    
    private int testFlag = 0XFF;   
    
    private int greenFlag       = 0x1;
    private int fairwayFlag     = 0x2;
    private int roughFlag       = 0x4;
    private int extraRoughFlag  = 0x8;


    public void MakeTerrain(float[,] heightmap, Area[,] groundmap, int height, int width)
    {
        // Make an array of boolean flags, and set all to 0 
        int[,] typesRequired = new int[width / tileSize, height / tileSize];
        for (int y = 0; y < height / tileSize; y++)
        {
            for (int x = 0; x < width / tileSize; x++)
            {
                typesRequired[x, y] = 0;
            }
        }
        
        //Find which terrains are in this tile space
        for (int y = 0; y < height / tileSize; y++)
        {
            for (int x = 0; x < width / tileSize; x++)
            {
                for(int i = y * tileSize; i < (y + 1) * tileSize; i++)
                {
                    for (int j = x * tileSize; j < (x + 1) * tileSize; j++)
                    {
                        switch(groundmap[i, j])
                        {
                            case (Area.GREEN):
                                typesRequired[x, y] = typesRequired[x, y] | greenFlag;
                                break;
                            case (Area.FAIRWAY):
                                typesRequired[x, y] = typesRequired[x, y] | fairwayFlag;
                                break;
                            case (Area.ROUGH):
                                typesRequired[x, y] = typesRequired[x, y] | roughFlag;
                                break;
                            case (Area.EXTRA_ROUGH):
                                typesRequired[x, y] = typesRequired[x, y] | extraRoughFlag;
                                break;
                        }
                    }
                }
            }
        }

        // Create series of Terrains
        int curFlags;
        GameObject curObject;
        Terrain curTerr;
        TerrainData curData;
        float[,] curHeights;
        for (int y = 0; y < height / tileSize; y++)
        {
            for (int x = 0; x < width / tileSize; x++)
            {
                // Init
                curFlags = typesRequired[x, y];
                curHeights = new float[tileSize + 1, tileSize + 1];
                
                // Create all the terrain types that exist in this area
                if ((curFlags & greenFlag) > 0)
                {
                    // Set heightmap values
                    for (int j = 0; j < tileSize + 1; j++)
                    {
                        for (int i = 0; i < tileSize + 1; i++)
                        {
                            // If this is our terrain...
                            if (groundmap[(tileSize * x) + i, (tileSize * y) + j] == Area.GREEN)
                            {
                                // Set to heightmap
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i];
                            }
                            else
                            {
                                // Else, this terrain gets tucked under the actual terrain
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i] - overlapHeight;
                            }

                        }
                    }

                    // Make the terrain object
                    curData = new TerrainData();
                    curData.name = "" + x + "x" + y + "y";
                    curData.size = new Vector3(tileSize, heightScale, tileSize);

                    curObject = Terrain.CreateTerrainGameObject(curData);
                    curTerr = curObject.GetComponent<Terrain>();

                    curObject.transform.position = new Vector3((x * tileSize), 0, (y * tileSize));
                    curData.SetHeights(0, 0, curHeights);

                    curTerr.transform.parent = gameObject.transform;
                    curTerr.editorRenderFlags = (TerrainRenderFlags)0;
                    curTerr.castShadows = false;

                    // Set names and tags
                    curObject.name = "GreenTerrainTile";
                    curObject.tag = "Green";
                    curObject.layer = LayerMask.NameToLayer("Ground");
                }

                if ((curFlags & fairwayFlag) > 0)
                {
                    // Set heightmap values
                    for (int j = 0; j < tileSize + 1; j++)
                    {
                        for (int i = 0; i < tileSize + 1; i++)
                        {
                            // If this is our terrain...
                            if (groundmap[(tileSize * x) + i, (tileSize * y) + j] == Area.FAIRWAY)
                            {
                                // Set to heightmap
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i];
                            }
                            else
                            {
                                // Else, this terrain gets tucked under the actual terrain
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i] - overlapHeight;
                            }

                        }
                    }

                    // Make the terrain object
                    curData = new TerrainData();
                    curData.name = "" + x + "x" + y + "y";
                    curData.size = new Vector3(tileSize, heightScale, tileSize);

                    curObject = Terrain.CreateTerrainGameObject(curData);
                    curTerr = curObject.GetComponent<Terrain>();

                    curObject.transform.position = new Vector3((x * tileSize), 0, (y * tileSize));
                    curData.SetHeights(0, 0, curHeights);

                    curTerr.transform.parent = gameObject.transform;
                    curTerr.editorRenderFlags = (TerrainRenderFlags)0;
                    curTerr.castShadows = false;

                    // Set names and tags
                    curObject.name = "FairwayTerrainTile";
                    curObject.tag = "Fairway";
                    curObject.layer = LayerMask.NameToLayer("Ground");
                }

                if ((curFlags & roughFlag) > 0)
                {

                    // Set heightmap values
                    for (int j = 0; j < tileSize + 1; j++)
                    {
                        for (int i = 0; i < tileSize + 1; i++)
                        {
                            // If this is our terrain...
                            if (groundmap[(tileSize * x) + i, (tileSize * y) + j] == Area.ROUGH)
                            {
                                // Set to heightmap
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i];
                            }
                            else
                            {
                                // Else, this terrain gets tucked under the actual terrain
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i] - overlapHeight;
                            }

                        }
                    }

                    // Make the terrain object
                    curData = new TerrainData();
                    curData.name = "" + x + "x" + y + "y";
                    curData.size = new Vector3(tileSize, heightScale, tileSize);

                    curObject = Terrain.CreateTerrainGameObject(curData);
                    curTerr = curObject.GetComponent<Terrain>();

                    curObject.transform.position = new Vector3((x * tileSize), 0, (y * tileSize));
                    curData.SetHeights(0, 0, curHeights);

                    curTerr.transform.parent = gameObject.transform;
                    curTerr.editorRenderFlags = (TerrainRenderFlags)0;
                    curTerr.castShadows = false;

                    // Set names and tags
                    curObject.name = "RoughTerrainTile";
                    curObject.tag = "Rough";
                    curObject.layer = LayerMask.NameToLayer("Ground");
                }

                if ((curFlags & extraRoughFlag) > 0)
                {
                    // Set heightmap values
                    for (int j = 0; j < tileSize + 1; j++)
                    {
                        for (int i = 0; i < tileSize + 1; i++)
                        {
                            // If this is our terrain...
                            if (groundmap[(tileSize * y) + j, (tileSize * x) + i] == Area.EXTRA_ROUGH)
                            {
                                // Set to heightmap
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i];
                            }
                            else
                            {
                                // Else, this terrain gets tucked under the actual terrain
                                curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i] - overlapHeight;
                            }

                        }
                    }

                    // Make the terrain object
                    curData = new TerrainData();
                    curData.name = "" + x + "x" + y + "y";
                    curData.size = new Vector3(tileSize, heightScale, tileSize);

                    curObject = Terrain.CreateTerrainGameObject(curData);
                    curTerr = curObject.GetComponent<Terrain>();
                    
                    curObject.transform.position = new Vector3((x * tileSize), 0, (y * tileSize));
                    curData.SetHeights(0, 0, curHeights);

                    curTerr.transform.parent = gameObject.transform;
                    curTerr.editorRenderFlags = (TerrainRenderFlags)0;
                    curTerr.castShadows = false;

                    // Set names and tags
                    curObject.name = "ExtraRoughTerrainTile";
                    curObject.tag = "ExtraRough";
                    curObject.layer = LayerMask.NameToLayer("Ground");
                }
            }
        }

        


                // Make all valid terrain tile


                //Set heightmaps

    }
}
