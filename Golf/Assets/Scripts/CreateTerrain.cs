using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTerrain : MonoBehaviour {

    public int tileSize;
    public int heightScale;
    public float overlapHeight;   // How far the dominate terrain lifts above non-active terrains
    public float cupHeight = .2f;

    Area[,] groundmap;
    float[,] heightmap;

    public GameObject GreenTile;
    public GameObject FairwayTile;
    public GameObject RoughTile;
    public GameObject ExtraRoughTile;
    public GameObject courseHole;

    private const int testFlag = 0XFF;   
    
    private const int greenFlag       = 0x1;
    private const int fairwayFlag     = 0x2;
    private const int roughFlag       = 0x4;
    private const int extraRoughFlag  = 0x8;


    void MakeNewTerrainTile(Area type, int x, int y)
    {
        int curFlag;
        Area curType;
        string curName;
        string curTag;
        
        switch(type){
            case Area.GREEN:
                curFlag = greenFlag;
                curType = Area.GREEN;
                curName = "GreenTerrainTile";
                curTag = "Green";
                break;
            case Area.FAIRWAY:
                curFlag = fairwayFlag;
                curType = Area.FAIRWAY;
                curName = "FairwayTerrainTile";
                curTag = "Fairway";
                break;
            case Area.ROUGH:
                curFlag = roughFlag;
                curType = Area.ROUGH;
                curName = "RoughTerrainTile";
                curTag = "Rough";
                break;
            case Area.EXTRA_ROUGH:
                curFlag = extraRoughFlag;
                curType = Area.EXTRA_ROUGH;
                curName = "ExtraRoughTerrainTile";
                curTag = "ExtraRough";
                break;
            default:
                curFlag = extraRoughFlag;
                curType = Area.EXTRA_ROUGH;
                curName = "ExtraRoughTerrainTile";
                curTag = "ExtraRough";
                break;
        }

        // Set heightmap values
        float[,] curHeights = new float[tileSize + 1, tileSize + 1];
        for (int j = 0; j < tileSize + 1; j++)
        {
            for (int i = 0; i < tileSize + 1; i++)
            {
                // If this is our terrain...
                if (groundmap[(tileSize * x) + i, (tileSize * y) + j] == curType)
                {
                    // Set to heightmap
                    curHeights[j, i] = heightmap[(tileSize * y) + j, (tileSize * x) + i];
                }
                else
                {
                    // Else, this terrain gets tucked under the actual terrain
                    curHeights[j, i] = 0;
                }
            }
        }
        
        // Instantiate Data
        GameObject curObject;
        Terrain curTerr;
        TerrainData curData;
        
        // Fill Data for the object
        curData = new TerrainData();
        curData.name = "" + x + "x" + y + "y";
        curData.size = new Vector3(tileSize, heightScale, tileSize);

        // Make the terrain object
        curObject = Terrain.CreateTerrainGameObject(curData);
        curTerr = curObject.GetComponent<Terrain>();
        curObject.transform.position = new Vector3((x * tileSize), 0, (y * tileSize));

        // Fill terrain options
        curTerr.transform.parent = gameObject.transform;
        curTerr.editorRenderFlags = (TerrainRenderFlags)0;
        curTerr.castShadows = false;

        // Apply data to terrain
        curData.SetHeights(0, 0, curHeights);

        // Set names and tags
        curObject.name = curName;
        curObject.tag = curTag;
        curObject.layer = LayerMask.NameToLayer("Ground");

        return;
    }
        
    
    public void MakeTerrain(float[,] heightmap, Area[,] groundmap, int[] holeCoors, int height, int width)
    {
        this.heightmap = heightmap;
        this.groundmap = groundmap;

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
        for (int y = 0; y < height / tileSize; y++)
        {
            for (int x = 0; x < width / tileSize; x++)
            {
                // Init
                curFlags = typesRequired[x, y];

                // Create all the terrain types that exist in this area
                if ((curFlags & greenFlag) > 0)
                    MakeNewTerrainTile(Area.GREEN, x, y);

                if ((curFlags & fairwayFlag) > 0)
                    MakeNewTerrainTile(Area.FAIRWAY, x, y);

                if ((curFlags & roughFlag) > 0)
                    MakeNewTerrainTile(Area.ROUGH, x, y);
                
                if ((curFlags & extraRoughFlag) > 0)
                    MakeNewTerrainTile(Area.EXTRA_ROUGH, x, y);
                    
            }
        }

        // Place the hole
        Vector3 holePos = new Vector3(0, 0, 0);
        
        // Find position
        holePos.x = holeCoors[0];
        holePos.z = holeCoors[1];
        holePos.y = heightmap[holeCoors[0], holeCoors[1]] * heightScale - cupHeight;

        // find Rotation
        float xAngle = heightmap[holeCoors[0] + 1, holeCoors[1]] - heightmap[holeCoors[0] - 1, holeCoors[1]];
        float yAngle = heightmap[holeCoors[0], holeCoors[1] + 1] - heightmap[holeCoors[0], holeCoors[1] - 1];

        xAngle = Mathf.Atan(xAngle);
        yAngle = Mathf.Atan(yAngle);

        GameObject curObj = Instantiate(courseHole, holePos, Quaternion.identity, gameObject.transform);
        
    }
}
