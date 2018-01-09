using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour {

    Terrain myTerrain;
    TerrainData myData;

    public int numPeaks;                // How many random peaks
    public int smoothingIterations;     // How many times to average out peaks
    public float noiseVariance;         // +-Noise to add, 0 - 1

    public int width;                   // Width of the terrain map
    public int height;                  // height of the map
    public int depth;                   // scale of terrain to heightmap

    public bool useSeed;                // Whether or not a seed is given
    public int seed;                    // Seed to use

	// Use this for initialization
	void Start ()
    {
        /* Set seed if given */
        if (useSeed)
            Random.InitState(seed);
        
        /* Variable init */
        float[,] heightmap = new float[width, height];
        myData = new TerrainData();
        myData.heightmapResolution = width;
        
        /* Randomly generate peaks */
        MonteCarloTerrain(numPeaks, heightmap);

        /* spread peaks out using an average, for N iterations */
        heightmap = SimpleSmoother(heightmap, smoothingIterations);

        /* Add a noise  */
        float[,] noise = GenerateNoiseArray(-noiseVariance, noiseVariance);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heightmap[x, y] += noise[x, y];
            }
        }
        heightmap = SimpleSmoother(heightmap, 1);

        myData.SetHeights(0, 0, heightmap);
        
        myTerrain = Terrain.CreateTerrainGameObject(myData).GetComponent<Terrain>();
        myTerrain.gameObject.transform.position = Vector3.zero;
	}


    /**
     * Select n points and randomly assign a value over 0, less than 1
     */
    public void MonteCarloTerrain(int numPoints, float[,] arr)
    {
        for(int i = 0; i < numPoints; i++)
        {
            arr[Random.Range(0, width), Random.Range(0, height)] = Random.Range(float.Epsilon, 1.0F);
        }
    }

    /**
     * Give each point of the array a value (0 < x <= 1] 
     */
    public float[,] GenerateNoiseArray(float min, float max)
    {
        float[,] noise = new float[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noise[x, y] = Random.Range(min, max);
            }
        }

        return noise;
    }


    /**  
     *  Apply a variably weaighted average between each point and its tallest neighbor
     */
    public float[,] SimpleSmoother(float[,] arr, int iterations)
    {
        
        for(int i = 0; i < iterations; i++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if(x == 0)
                    {
                        arr[x, y] = arr[x + 1, y] * Random.Range(0.93F, 0.98F);
                    }
                    else if (x == width)
                    {
                        arr[x, y] = arr[x - 1, y] * Random.Range(0.93F, 0.98F);
                    }
                    else if (y == 0)
                    {
                        arr[x, y] = arr[x, y + 1] * Random.Range(0.93F, 0.98F);
                    }
                    else if (y == height)
                    {
                        arr[x, y] = arr[x, y - 1] * Random.Range(0.93F, 0.98F);
                    }
                    else if (arr[x, y] < .6)
                    {
                        arr[x, y] = Mathf.Max(arr[x - 1, y], arr[x + 1, y], arr[x, y - 1], arr[x, y + 1]) * Random.Range(.6F, .95F);
                    }
                }
            }
        }

        return arr;
    }
    
	// Update is called once per frame
	void Update () {
        /* Rescale on change */
        myData.size = new Vector3(width, depth, height);
	}
}
