using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* The possible ground options */
public enum Area
{
    FAIRWAY,
    ROUGH,
    EXTRA_ROUGH,
    GREEN,
    SAND,
    ASPHAULT,
    DIRT,
    UNSET,
    START
}

public struct BoundingBox
{
    public Area type;

    public int x_min;
    public int x_max;
    public int y_min;
    public int y_max;

    public BoundingBox(Area contents)
    {
        type = contents;

        x_min = int.MaxValue;
        x_max = int.MinValue;
        y_min = int.MaxValue;
        y_max = int.MinValue;
    }

    public void TestPointForUpdate(int x, int y)
    {
        if (x < x_min)
            x_min = x;

        if (x > x_max)
            x_max = x;

        if (y < y_min)
            y_min = y;

        if (y > y_max)
            y_max = y;
    }
}

public class CreateGolfTerrain : MonoBehaviour {

    public GameObject squareSprite;
    
    public int size_width;
    public float size_depth;
    public int size_height;
    
    public int mapWidth;
    public int mapHeight;
    public float[,] heightmap;

    private Area[,] groundType;

    // Procedural Parameters
    public int initialPointMin;
    public int initialPointMax;
    public int radiusMin;
    public int radiusMax;
    public int distanceMin;
    public int distanceMax;
    public int angleVariance;
    public float defaultHeight;

    public float par3Min = 95;
    public float par4Min = 251;
    public float par5Min = 451;

    // [Xmin, Xmax, Ymin, Ymax]
    public BoundingBox greenBox;
    public BoundingBox fairwayBox;
    public BoundingBox roughBox;
    public BoundingBox extraRoughBox;
    public BoundingBox sandBox;

    // In the creation of Sand traps, min a max # of circles to place
    public int minSandCircles;
    public int maxSandCircles;
    public int minSandRadius;
    public int maxSandRadius;


    /**
     * float Point2PointDistance(int[] p1, int[] p2)
     * 
     */
    public float PointDistance(int[] p1, int[] p2)
    {
        float x_dist = p2[0] - p1[0];
        float y_dist = p2[1] - p1[1]; 

        return Mathf.Sqrt((x_dist * x_dist) + (y_dist * y_dist));

    }

    /**
     *  void SetRadiusToValue - For each point in ground array, set the radius around the given point to 'type'
     * 
     *      int[] point - [x, y], position to place the centre of the circle.
     *      int radius  - The radius of the circle to make.
     *      Area type   - Area enum type to set the circle to.
     */
    public void SetRadiusToValue(int[] point, int radius, Area type)
    {
        BoundingBox myBounds;

        // We need a 2 point array
        if(point.Length != 2)
        {
            throw new System.Exception("CreateGolfTerrain:SetRadiusToValue:'point.Length is not = 2!'");
        }

        // Find the boundaeies that we're using
        switch (type)
        {
            case Area.FAIRWAY:
                myBounds = fairwayBox;
                break;
            case Area.ROUGH:
                myBounds = roughBox;
                break;
            case Area.EXTRA_ROUGH:
                myBounds = extraRoughBox;
                break;
            case Area.GREEN:
                myBounds = greenBox;
                break;
            case Area.SAND:
                myBounds = sandBox;
                break;
            default:
                myBounds = new BoundingBox(Area.UNSET);

                myBounds.x_min = 0;
                myBounds.x_max = mapWidth;
                myBounds.y_min = 0;
                myBounds.y_max = mapHeight;
                break;
        }

        // Test the extremes of the circle against the bounds, update if necessary
        myBounds.TestPointForUpdate(point[0] - radius, point[1] - radius);
        myBounds.TestPointForUpdate(point[0] + radius, point[1] + radius);

        // For every point in the boundary...  
        for (int y = myBounds.y_min; y < myBounds.y_max; y++)
        {
            for (int x = myBounds.x_min; x < myBounds.x_max; x++)
            {
                // Find the distance from here to the centre
                float dist = PointDistance(new int[2] { x, y }, point);

                // If the point's in the circle...
                if (dist < radius)
                {
                    // Set it to type
                    groundType[x, y] = type;
                }
            }
        }
    }

    /**
     * int[] FindPointFromVector - Given a point, an angle, and a magnitude, return an array point
     * 
     *      returns     - int [x, y]: the integer coordinates result of the calculation
     *      
     *      point       - int [x, y]: the initial point from which to start
     *      angleDeg    - The angle in degrees to apply
     *      magnitude   - How the vector is from point.
     */
    public int[] FindPointFromVector(int [] point, int angleDeg, int magnitude)
    {
        int[] pointTwo = new int[2];
        
        pointTwo[0] = point[0] + Mathf.FloorToInt(Mathf.Sin(angleDeg * Mathf.Deg2Rad) * magnitude);
        pointTwo[1] = point[1] + Mathf.FloorToInt(Mathf.Cos(angleDeg * Mathf.Deg2Rad) * magnitude);

        if (pointTwo[0] > mapWidth || pointTwo[1] > mapHeight)
        {
            print("Invalid Point:" + pointTwo[0] + " " + pointTwo[1]);
            throw new System.Exception("SetRadiusToValue:FindPointFromVector:'New point is out of bounds: (" + 
                pointTwo[0].ToString() + ", " + pointTwo[1].ToString() + ")'");
        }

        return pointTwo;
    }

    /**
     * PointToLineTest - Returns negative if point is to the left/above, positive if to right/below
     * 
     *      returns      - a float, negative or positive for either side of the line
     * 
     *      int[] line1  - int[x, y]: one point fo the line
     *      int[] line2  - int[x, y]: second point fo the line
     *      int[] point  - int[x, y]: point to test
     */
    public float PointToLineTest(int[] line1, int[] line2, int[] point)
    {
        float ans;
        // Equation that does as advertised:
        // (x1 - x0)(y2 - y0) - (x2 - x0)(y1 - y0)
        ans = ((line2[0] - line1[0]) * (point[1] - line1[1])) - ((line2[1] - line1[1]) * (point[0] - line1[0]));
        
        return ans;
    }

    /**
     * void FillBitangent - Given two circles, calculate the bitangental rhombus and fill with type
     * 
     *      int[] p1    - int[x, y]: The x & y coordinates of circle one
     *      int[] p2    - int[x, y]: The x & y coordinates of circle two
     *      int r1      - The radius of circle one
     *      int r2      - The radius of circle two
     *      Area type   - What value to put in the area
     */
    public void FillBitangent(int[] p1, int[] p2, int r1, int r2, Area type)
    {
        // The path of the fairway
        float[] pointsOfLine = new float[8];

        // Get angle between centres
        float x_diff = p1[0] - p2[0];
        float y_diff = p1[1] - p2[1];
        int theta = Mathf.FloorToInt(Mathf.Atan(y_diff / x_diff) * Mathf.Rad2Deg);

        // Draw the Bitangental rhombus 
        int[,] rhombus = new int[2, 4];

        // Define the rhombus from the given values
        int[] curPoint;

        curPoint = FindPointFromVector(p1, theta + 90, r1 + 1);
        rhombus[0, 0] = curPoint[0];
        rhombus[1, 0] = curPoint[1];
        heightmap[rhombus[0, 0], rhombus[1, 0]] = 1;
        
        curPoint = FindPointFromVector(p1, theta - 90, r1 + 1);
        rhombus[0, 1] = curPoint[0];
        rhombus[1, 1] = curPoint[1];
        heightmap[rhombus[0, 1], rhombus[1, 1]] = 1;

        curPoint = FindPointFromVector(p2, theta - 90, r2 + 1);
        rhombus[0, 2] = curPoint[0];
        rhombus[1, 2] = curPoint[1];
        heightmap[rhombus[0, 2], rhombus[1, 2]] = 1;

        curPoint = FindPointFromVector(p2, theta + 90, r2 + 1);
        rhombus[0, 3] = curPoint[0];
        rhombus[1, 3] = curPoint[1];
        heightmap[rhombus[0, 3], rhombus[1, 3]] = 1;

        // Create the rhombus window for iteration
        int minX = 0;
        int minY = 0;
        int maxX = mapWidth;
        int maxY = mapHeight;
        
        for (int y = 0; y < 4; y++)
        {
            minX = Mathf.Min(minX, rhombus[0, y]);
            maxX = Mathf.Min(maxX, rhombus[0, y]);
            minY = Mathf.Min(minY, rhombus[1, y]);
            maxY = Mathf.Max(maxX, rhombus[1, y]);
        }

        // Bounding window is the centre positions +- the larger radius
        int maxRad = Mathf.Max(r1, r2);
        minX = Mathf.Max(minX - distanceMax, 0);
        maxX = Mathf.Min(maxX + distanceMax, mapWidth);
        minY = Mathf.Max(minY - distanceMax, 0);
        maxY = Mathf.Min(maxY + distanceMax, mapHeight);
        
        // Fill the rhombus
        bool test;
        int[] line1, line2;
        // For the entire min-max bound window...
        for (int y = minY; y < maxY; y++)
        {
            for(int x = minX; x < maxX; x++)
            {
                curPoint = new int[] { x, y };
                test = true;
                
                // left to top...
                line1 = new int[] { rhombus[0, 0], rhombus[1, 0] };
                line2 = new int[] { rhombus[0, 1], rhombus[1, 1] };
                test = test && (PointToLineTest(line1, line2, curPoint) <= 0);

                // top to right
                line1 = new int[] { rhombus[0, 1], rhombus[1, 1] };
                line2 = new int[] { rhombus[0, 2], rhombus[1, 2] };
                test = test && (PointToLineTest(line1, line2, curPoint) <= 0);

                // far right to bottom
                line1 = new int[] { rhombus[0, 2], rhombus[1, 2] };
                line2 = new int[] { rhombus[0, 3], rhombus[1, 3] };
                test = test && (PointToLineTest(line1, line2, curPoint) <= 0);

                // bottom to left
                line1 = new int[] { rhombus[0, 0], rhombus[1, 0] };
                line2 = new int[] { rhombus[0, 3], rhombus[1, 3] };
                test = test && (PointToLineTest(line1, line2, curPoint) >= 0);

                // if test is true after all four assertions...
                if (test)
                {
                    // Set the value,
                    groundType[x, y] = type;

                    // Update our bounding boxes
                    fairwayBox.TestPointForUpdate(x, y);
                }
            }
        }
    } 

    /**
     *  void FillPolygon
     * 
     *      points  - Four points that descripe the polygon.  Dimension one represents [x, y],
     *                      dimension two represents [p1, p2, p3, p4]
     *      type    - The Area enum value to place in the area
     */
    public void FillPolygon(int[,] points, Area type)
    {
        int minX, minY, maxX, maxY;

        minX = Mathf.Min(points[0, 0], points[0, 1], points[0, 2], points[0, 3]);
        minY = Mathf.Min(points[1, 0], points[1, 1], points[1, 2], points[1, 3]);
        maxX = Mathf.Max(points[0, 0], points[0, 1], points[0, 2], points[0, 3]);
        maxY = Mathf.Max(points[1, 0], points[1, 1], points[1, 2], points[1, 3]);
        
        // Fill the rhombus
        bool test;
        int[] line1, line2, curPoint;
        // For the entire min-max bound window...
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                curPoint = new int[] { x, y };
                test = true;

                // left to top...
                line1 = new int[] { points[0, 0], points[1, 0] };
                line2 = new int[] { points[0, 1], points[1, 1] };
                test = test && (PointToLineTest(line1, line2, curPoint) <= 0);

                // top to right
                line1 = new int[] { points[0, 1], points[1, 1] };
                line2 = new int[] { points[0, 2], points[1, 2] };
                test = test && (PointToLineTest(line1, line2, curPoint) <= 0);

                // far right to bottom
                line1 = new int[] { points[0, 2], points[1, 2] };
                line2 = new int[] { points[0, 3], points[1, 3] };
                test = test && (PointToLineTest(line1, line2, curPoint) <= 0);

                // bottom to left
                line1 = new int[] { points[0, 0], points[1, 0] };
                line2 = new int[] { points[0, 3], points[1, 3] };
                test = test && (PointToLineTest(line1, line2, curPoint) >= 0);

                // if test is true after all four assertions...
                if (test)
                {
                    // Set the value
                    groundType[x, y] = type;
                }
            }
        }
    }

    /**
     *  void MakeFairway - Set up the fairway based on the given par. If 3, make a large pad. If
     *                          4 || 5, create a chain of circles until required distance is met,
     *                          then join them in sequence bitangentally.
     *                          
     *      par     - Parameter that determines the length of the course, based on golfing conventions.
     */
    public void MakeFairway(int par)
    {
        // End conditions
        float parMin = 0;
        float totalDist = 0;

        switch (par)
        {
            case (3):
                parMin = par3Min;
                break;
            case (4):
                parMin = par4Min;
                break;
            default:
                parMin = par5Min;
                break;
        }

        // This means we need to make a fairway before our green
        if(par >= 3)
        {
            // Pick the Initial Point
            int initPoint = Random.Range(initialPointMin, initialPointMax);
            int[] curPoint = new int[2] { mapWidth / 3, mapHeight / 3 };
            int curRadius = Random.Range(radiusMin, radiusMax);
            int curDist;
            int curAngle = 90;

            // Apply adjustment to heightmap
            SetRadiusToValue(curPoint, curRadius, Area.FAIRWAY);
        
            // Iterate until par distance reached
            int[] nextPoint;
            int nextAngle, nextDist, nextRadius;
            while(totalDist < parMin)
            {
                // Next variables
                nextAngle = Random.Range(45 - angleVariance, 45 + angleVariance);
                nextDist = Random.Range(distanceMin, distanceMax);
                nextRadius = Random.Range(radiusMin, radiusMax);

                // Find the next point
                nextPoint = FindPointFromVector(curPoint, nextAngle, nextDist);

                // Apply to heightmap
                SetRadiusToValue(nextPoint, nextRadius, Area.FAIRWAY);

                //Fill between the two circle
                FillBitangent(curPoint, nextPoint, curRadius, nextRadius, Area.FAIRWAY);

                // Track path length
                totalDist += nextDist;

                curPoint = nextPoint;
                curRadius = nextRadius;
                curDist = nextDist;
                curAngle = nextAngle;
            }

            // If we're done making a fairway OR we're making a par 3, place our green.
            MakeGreen(curPoint, curRadius - 10);
        }
    }

    /**
     *  void MakeGreen(int[] point, int radius) - Define the bounding box of the green, then fill it
     *              within that boundary.
     *              
     *      point - The centre of the green circle
     *      radius - The radius of the green circle
     *      
     */
    public void MakeGreen(int[] point, int radius)
    {
        greenBox.x_min = point[0] - radius;
        greenBox.x_max = point[0] + radius;
        greenBox.y_min = point[1] - radius;
        greenBox.y_max = point[1] + radius;

        SetRadiusToValue(point, radius, Area.GREEN);
    }


    public void MakeSandTrap(BoundingBox curSandBox)
    {
        // Determin how many circles we're going to place
        int numCircles = Random.Range(minSandCircles, maxSandCircles + 1);

        // Iteration variables
        int curRadius;
        int[] curPoint;

        // for 0 to num circles
        for(int i = 0; i < numCircles; i++)
        {
            curPoint = new int[2] {Random.Range(curSandBox.x_min + (int)maxSandRadius,
                                                curSandBox.x_max - (int)maxSandRadius),
                                   Random.Range(curSandBox.y_min + (int)maxSandRadius,
                                                curSandBox.y_max - (int)maxSandRadius) };

            curRadius = Random.Range(minSandRadius, maxSandRadius);

            try
            {

                SetRadiusToValue(curPoint, curRadius, Area.SAND);
            }
            catch (System.IndexOutOfRangeException)
            {
                print("curPoint = " + curPoint[0] + "," + curPoint[1]);
                print("curRadius = " + curRadius);
            }
        }
    }


    public void PlaceSandTraps(int minTraps, int MaxTraps)
    {
        int numTraps = Random.Range(minTraps, MaxTraps + 1);
        List<int[]> pointList = new List<int[]>();
        int[] curPoint;

        // Build a list of all fairway transitions
        for (int y = fairwayBox.y_min; y < fairwayBox.y_max; y++)
        {
            for (int x = fairwayBox.x_min; x < fairwayBox.x_max; x++)
            {
                if (y < 1 || y >= mapHeight - 1 || x < 1 || x >= mapHeight - 1)
                    break;

                if(groundType[x, y] == Area.FAIRWAY)
                {
                    if(groundType[x - 1, y] != Area.FAIRWAY || groundType[x + 1, y] != Area.FAIRWAY ||
                        groundType[x, y - 1] != Area.FAIRWAY || groundType[x, y + 1] != Area.FAIRWAY)
                    {
                        curPoint = new int[2];

                        curPoint[0] = x;
                        curPoint[1] = y;

                        pointList.Add(curPoint);
                        break;
                    }
                }
            }
        }

        BoundingBox curSandBox;
        for (int i = 0; i < numTraps; i++)
        {
            curPoint = pointList[Random.Range(0, pointList.Count)];

            curSandBox = new BoundingBox(Area.SAND);
            
            curSandBox.x_min = Mathf.Max(0, curPoint[0] - (maxSandRadius + 1));
            curSandBox.x_max = Mathf.Min(mapHeight - 1, curPoint[0] + (maxSandRadius + 1));
            curSandBox.y_min = Mathf.Max(0, curPoint[1] - (maxSandRadius + 1));
            curSandBox.y_max = Mathf.Min(mapHeight - 1, curPoint[1] + (maxSandRadius + 1));

            MakeSandTrap(curSandBox);
        }

    }


    /**
     * void SurroundFairwayWithRough - For each point that is within resolution of fairway,
     *                                  set it to rough.
     *                                      
     *      resolution  - distance to check for fairway value.  
     */
    public void SurroundFairwayWithRough(int resolution)
    {
        bool updated;

        for (int y = fairwayBox.y_min - (2 * resolution); y < fairwayBox.y_max + (2 * resolution); y++)
        {
            for (int x = fairwayBox.x_min - (2 * resolution); x < fairwayBox.x_max + (2 * resolution); x++)
            {
                updated = false;

                // If we're within the array and at an unset element...
                if (x - resolution > 0 && x + resolution < mapWidth - 1 &&
                    y - resolution > 0 && y + resolution < mapHeight - 1 &&
                    groundType[x, y] == Area.UNSET)
                {
                    // For 'resolution' around curPoint... 
                    for (int i = x - resolution; i < x + resolution; i++)
                    {
                        for (int j = y - resolution; j < y + resolution; j++)
                        {
                            // If we find fairway...
                            if(groundType[i, j] == Area.FAIRWAY)
                            {
                                if(PointDistance(new int[2] { i, j }, new int[2] { x, y }) < (float)resolution)
                                {
                                    // Set to Rough, and update rough bounding box
                                    groundType[x, y] = Area.ROUGH;
                                    roughBox.TestPointForUpdate(x, y);
                                    updated = true;
                                    break;
                                }
                            }
                        }
                        // Break both loops if we were succesful
                        if (updated)
                            break;
                    }
                }
            }
        }
    }

    /**
     * void SurroundRoughWithExtraRough - For each point that is within resolution of rough,
     *                                      set it to ExtraRough.
     *                                      
     *      resolution  - distance to check for fairway value.  
     */
    public void SurroundRoughWithExtraRough(int resolution)
    {
        bool updated;

        for (int y = roughBox.y_min - resolution; y < roughBox.y_max + resolution; y++)
        {
            for (int x = roughBox.x_min - resolution; x < roughBox.x_max + resolution; x++)
            {
                updated = false;

                // If we're within the array and at an unset element...
                if (x - resolution > 0 && x + resolution < mapWidth - 1 &&
                    y - resolution > 0 && y + resolution < mapHeight - 1 &&
                    groundType[x, y] == Area.UNSET)
                {
                    // For 'resolution' around curPoint... 
                    for (int i = x - resolution; i < x + resolution; i++)
                    {
                        for (int j = y - resolution; j < y + resolution; j++)
                        {
                            // If we find rough...
                            if (groundType[i, j] == Area.ROUGH)
                            {
                                if (PointDistance(new int[2] { i, j }, new int[2] { x, y }) < (float)resolution)
                                {
                                    // Set to Rough, and update rough bounding box
                                    groundType[x, y] = Area.EXTRA_ROUGH;
                                    extraRoughBox.TestPointForUpdate(x, y);
                                    updated = true;
                                    break;
                                }
                            }
                        }
                        // Break both loops if we were succesful
                        if (updated)
                            break;
                    }
                }
            }
        }
    }

    /**
     *  void SetUnset - For Every point in groundArray that is unset, set it to type.
     *  
     *  type    - Type to set unset points to
     */
    public void SetUnsetArea(Area type)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(groundType[x,y] == Area.UNSET)
                groundType[x, y] = type;
            }
        }
    }

    public void CreateBorder(int width, Area type)
    {
        for(int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (x < width)
                    groundType[x, y] = type;
                if (y < width)
                    groundType[x, y] = type;
                if(x > mapWidth - width)
                    groundType[x, y] = type;
                if(y > mapHeight - width)
                    groundType[x, y] = type;
            }
        }
    }
    
    public void MakeNewCourse(int par)
    {
        CreateBorder(3, Area.ASPHAULT);
        MakeFairway(par);
        PlaceSandTraps(2, 3);
        SurroundFairwayWithRough(20);
        SurroundRoughWithExtraRough(20);
        SetUnsetArea(Area.EXTRA_ROUGH);

        int[,] points = new int[2,4];

        // p0
        points[0, 0] = 55;
        points[1, 0] = 92;

        //p1
        points[0, 1] = 102;
        points[1, 1] = 64;

        //p2
        points[0, 2] = 80;
        points[1, 2] = 18;

        //p3
        points[0, 3] = 31;
        points[1, 3] = 41;

        FillPolygon(points, Area.FAIRWAY);


    }

    /**
     *  void VisualizeGameBoard - visualize the generated terrain with coloured squares at (0, 0, 0)
     * 
     */
    public void VisualizeGameBoard()
    {
        GameObject curSprite;
        SpriteRenderer curSpriteRend;

        // For every point of the array...
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // If type is not unset...
                if (groundType[x, y] != Area.UNSET)
                {
                    // Make a unique tile,
                    curSprite = Instantiate(squareSprite, new Vector3(x, y, 0), Quaternion.identity, gameObject.transform);
                    curSpriteRend = curSprite.GetComponent<SpriteRenderer>();

                    // Color it to represent an Area Value
                    switch (groundType[x, y])
                    {
                        case Area.GREEN:
                            curSpriteRend.color = Color.white;
                            break;
                        case Area.FAIRWAY:
                            curSpriteRend.color = Color.green;
                            break;
                        case Area.ROUGH:
                            curSpriteRend.color = new Color(0.0f, .66f, 0.0f, 1.0f);
                            break;
                        case Area.EXTRA_ROUGH:
                            curSpriteRend.color = new Color(0.0f, .33f, 0.0f, 1.0f);
                            break;
                        case Area.SAND:
                            curSpriteRend.color = Color.yellow;
                            break;
                        case Area.DIRT:
                            curSpriteRend.color = new Color(.54f, .27f, .07f);
                            break;
                        case Area.ASPHAULT:
                            curSpriteRend.color = Color.grey;
                            break;
                        default:
                            curSpriteRend.color = Color.clear;
                            break;
                    }
                }
            }
        }
    }

    void Awake()
    {
        heightmap = new float[mapWidth, mapHeight];
        groundType = new Area[mapWidth, mapHeight];

        fairwayBox = new BoundingBox(Area.FAIRWAY);
        roughBox = new BoundingBox(Area.ROUGH);
        extraRoughBox = new BoundingBox(Area.EXTRA_ROUGH);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                groundType[x, y] = Area.UNSET;
            }
        }
    }

    /* Getters and Setters */
    public Area[,] getGroundMap()
    {
        return groundType;
    }
}
