using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapToHeightmap : MonoBehaviour {

    public int allHeight;
    public int allWidth;
    public int allLength;
    
    /* Co-components */
    CreateGolfTerrain mapCreator;
    CreateTerrain terrMaker;
    TextureTerrainScript terrPainter;

    // Terrain Book Keeping 
    float[,] GreenHeightmap;
    float[,] FairwayHeightmap;
    float[,] RoughHeightmap;
    float[,] ExtraRoughHeightmap;
    Terrain GreenTerrain;
    Terrain FairwayTerrain;
    Terrain RoughTerrain;
    Terrain ExtraRoughTerrain;
    TerrainData GreenData;
    TerrainData FairwayData;
    TerrainData RoughData;
    TerrainData ExtraRoughData;

    // Map variables
    public float greenVolatility;
    public float fairwayVolatility;
    public float roughVolatility;
    public float extraRoughVolatility;

    int mapWidth;
    int mapHeight;
    float[,] heightmap;
    Area[,] groundmap;

    public int gridSize;
    public float minHeight;
    public float maxHeight;
    public bool visualize;

    /**
     *  float SCurve(float x): For a value [0, 1], returns a cubic value [0, 1].  Use for smoothing linear values.
     *                  f(x) = 2x^3 + 3x^2
     *                  
     *          float x: input value
     *          returns: f(x)
     */
    public float SCurve(float x)
    {
        return (-2.0F * x * x * x) + (3.0F * x * x);
    }

    /**
     * float[,] AreaBasedLattice(int size): Creates an array of points to Interpolatefor the heightmap,
     *                               using the enum Area type that is found at the critical points.
     *                                  
     *          int size: the width and height of the array
     * 
     */
    public float[,] AreaBasedLattice(int size)
    {
        float[,] lattice = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Volatility scales the range of pollible values
                float volatility;

                switch (groundmap[x * size, y * size])
                {
                    case Area.FAIRWAY:
                        volatility = fairwayVolatility;
                        break;
                    case Area.GREEN:
                        volatility = greenVolatility;
                        break;
                    case Area.ROUGH:
                        volatility = roughVolatility;
                        break;
                    case Area.EXTRA_ROUGH:
                        volatility = extraRoughVolatility;
                        break;
                    default:
                        volatility = 0.0f;
                        break;
                }

                heightmap[x, y] = Random.Range(0.0f, 1.0f) * volatility;
            }
        }

        return lattice;
    }

    /**
     * float[,] RandomNoiseLattice(int size): Creates a random array of points to Interpolate for the heightmap.
     *                                  
     *          int size: the width and height of the array
     * 
     */
    public float[,] RandomNoiseLattice(int size)
    {
        float[,] lattice = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                lattice[x, y] = Random.Range(minHeight, maxHeight);
            }

        }

        return lattice;
    }

    /**
     * float[,] EducatedgolfLattice(int size): Create a tailored array of points to Interpolate for the heightmap, 
     *                                  using different criteria for each ground type.
     *                                  
     *          int size: the width and height of the array
     * 
     */
    public float[,] EducatedGolfLattice(int size)
    {
        float[,] lattice = new float[size, size];

        for(int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

            }
        }


        return lattice;
    }


    /**
     *  void  ApplyBicubicInterpolation():  Use Bicubic interpolation on a sub-array of values to 
     *                                          create a smooth heightmap surface.
     */
    public void ApplyBicubicInterpolation()
    {
        // Make the lattice, the grid of heights that will be interpolated
        int latticeRes = gridSize;
        float[,] lattice; ;

        // Set the lattice to random height values
        lattice = RandomNoiseLattice(latticeRes);
        
        latticeRes -= 1;    // this gives us number of segments, as last point is excluded
        for (int y = 0; y < mapHeight; y++)
        { 
            for (int x = 0; x < mapWidth; x++)
            {
                // How far we are in the current section
                float x_prog = (float)((latticeRes * x) % mapWidth) / (float)mapWidth;
                float y_prog = (float)((latticeRes * y) % mapHeight) / (float)mapHeight;

                // Which lattice section we are in
                int cur_X = ((x * latticeRes) / (mapWidth));
                int cur_y = ((y * latticeRes) / (mapHeight));

                // Find the relative weights for this particular point
                float x_weight = SCurve(1.0f - x_prog);
                float y_weight = SCurve(1.0f - y_prog);
                float opp_x_weight = SCurve(x_prog);
                float opp_y_weight = SCurve(y_prog);

                // Generate a weighted average on the current point
                heightmap[x, y] = 0;
                heightmap[x, y] += (lattice[cur_X, cur_y] * x_weight * y_weight);
                heightmap[x, y] += (lattice[cur_X + 1, cur_y] * opp_x_weight * y_weight);
                heightmap[x, y] += (lattice[cur_X, cur_y + 1] * x_weight * opp_y_weight);
                heightmap[x, y] += (lattice[cur_X + 1, cur_y + 1] * opp_x_weight * opp_y_weight);
            }
        }
    }
    
    void Awake()
    {
        mapCreator = GetComponent<CreateGolfTerrain>();
        terrMaker = GetComponent<CreateTerrain>();
        terrPainter = GetComponent<TextureTerrainScript>();
    }

    // Use this for initialization
    void Start () {
        // Initiate necessary values
        mapWidth = mapCreator.mapWidth;
        mapHeight = mapCreator.mapHeight;

        // Initialize Heightmaps
        heightmap = new float[mapWidth, mapHeight];

        // Create the groundmap (This defines the golf course)
        int par = Random.Range(3, 6);
        mapCreator.MakeNewCourse(par);
        groundmap = mapCreator.getGroundMap();
        int[] holePos = mapCreator.holeLocation;

        // Create the general heightmap
        ApplyBicubicInterpolation();

        // Visualize the board with coloured tiles if true
        if(visualize)
            mapCreator.VisualizeGameBoard();

        // Pass the values to the collision and texture creating scripts
        terrMaker.MakeTerrain(heightmap, groundmap, holePos, mapCreator.mapHeight, mapCreator.mapWidth);
        terrPainter.PaintTerrain(heightmap, groundmap, mapCreator.mapHeight);
    }
}
