using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Level level;
    public static LevelGenerator Instance;
    enum gridSpace { empty, floor, wall, bottomWall }
    gridSpace[,] grid;

    int roomHeight, roomWidth;
    [SerializeField] GameObject levelHolderPrefab;
    [SerializeField] Transform levelHolder;
    //[SerializeField] Vector2 roomSizeWorldUnits = new Vector2(30, 30);
    //[SerializeField] float worldUnitsInOneGridCell = 1;
    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }
    List<walker> walkers;
    //[SerializeField] int maxWalkers = 10;
    //[Range(0, 1)]
    //[SerializeField] float chanceWalkerChangeDir = .5f, chanceWalkerSpawn = .05f, chanceWalkerDestroy = .05f, percentToFill = .2f;
    //[SerializeField] GameObject wallObj, floorObj, floorObj1, floorObj2, bottomWall;

    [SerializeField] bool cupgradeSpawned;
    [SerializeField] GameObject cupgrade;

    [SerializeField] bool spawnpointSet;
    [SerializeField] GameObject spawnPoint;

    [SerializeField] bool endLevelSet;
    [SerializeField] GameObject endLevel;

    [SerializeField] GameObject chest;
    //[Range(0, 1)]
    //[SerializeField] float chanceChestSpawn;
    //[SerializeField] int maxChestNumber;
    int chestNumber;

    int numberOfFloors = 0;
    int numberOfFloorsSpawned = 0;

    //[SerializeField] GameObject[] enemies;
    //[Range(0, 1)]
    //[SerializeField] float chanceIncreaseAmount;
    float chanceEnemySpawn;
    //[SerializeField] int maxEnemies;
    int totalEnemies;
    private void Awake()
    {
        Instance = this;
    }
    public void LoadLevel(Level newLevel = null)
    {
        if (newLevel != null)
            level = newLevel;
        
        if (levelHolder != null)
            Destroy(levelHolder.gameObject);
        levelHolder = Instantiate(levelHolderPrefab, transform).transform;

        cupgradeSpawned = spawnpointSet = endLevelSet = false;
        totalEnemies = numberOfFloors = numberOfFloorsSpawned = chestNumber = 0;
        chanceEnemySpawn = level.startingChanceEnemySpawn;

        Setup();
        CreateFloors();
        CreateWalls();
        SpawnLevel();
        GameManager.Instance.GenerationComplete();
    }

    private void Setup()
    {
        // Find grid size
        roomHeight = Mathf.RoundToInt(level.roomSizeWorldUnits.x / level.worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(level.roomSizeWorldUnits.y / level.worldUnitsInOneGridCell);

        // Create grid
        grid = new gridSpace[roomWidth, roomHeight];

        // Set grid's default state
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                // Make every cell "empty"
                grid[x, y] = gridSpace.empty;
            }
        }
        // Set first walker
        // Init list
        walkers = new List<walker>();

        // Create a walker
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();

        // Find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2f), Mathf.RoundToInt(roomHeight / 2f));
        newWalker.pos = spawnPos;

        // Add walker to lsit
        walkers.Add(newWalker);
    }

    private Vector2 RandomDirection()
    {
        // Pick random int between 0 and 3
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.90f);
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }

    private void CreateFloors()
    {
        int iteration = 0;
        do
        {
            // Create floor at position of every walker
            foreach (walker myWalker in walkers)
            {
                grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
            }

            // Chance: destroy walker
            int numberChecks = walkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                if (UnityEngine.Random.value < level.chanceWalkerDestroy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break;
                }
            }

            // Chance: walker pick new direction
            for (int i = 0; i < walkers.Count; i++)
            {
                if (UnityEngine.Random.value < level.chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }

            // Chance: spawn new walker
            numberChecks = walkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                if (UnityEngine.Random.value < level.chanceWalkerSpawn && walkers.Count < level.maxWalkers)
                {
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }

            // Move walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }

            // Avoid boarder of grid
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }

            // Check to exit loop
            if ((float)NumberOfFloors() / (float)grid.Length > level.percentToFill)
            {
                break;
            }
            iteration++;
        } while (iteration < 100000);
    }

    private float NumberOfFloors()
    {
        numberOfFloors = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                numberOfFloors++;
            }
        }
        return numberOfFloors;
    }

    private void CreateWalls()
    {
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                if (grid[x, y] == gridSpace.floor)
                {
                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }

                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }

                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }

                    if (grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y + 1] = gridSpace.bottomWall;
                    }
                }
            }
        }
    }
    private void SpawnLevel()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.empty:
                        //Spawn(x, y, level.wallObj);
                        break;
                    case gridSpace.floor:
                        numberOfFloorsSpawned++;
                        SpawnFloor(x, y);
                        SetSpawnpoint(x, y);
                        SetEndLevel(x, y);
                        SpawnCupgrade(x, y);
                        SpawnEnemies(x, y);
                        SpawnChest(x, y);
                        break;
                    case gridSpace.wall:
                        Spawn(x, y, level.wallObj);
                        break;
                    case gridSpace.bottomWall:
                        Spawn(x, y, level.bottomWall);
                        break;
                }
            }
        }
        
    }

    private void SpawnChest(int x, int y)
    {
        float chance = UnityEngine.Random.value;
        if (numberOfFloorsSpawned > numberOfFloors / 3 && chestNumber < level.maxChestNumber && chance < level.chanceChestSpawn)
        {
            chestNumber++;
            Spawn(x, y, chest);
        }
    }

    void SpawnEnemies(int x, int y)
    {
        float random = UnityEngine.Random.value;
        if (random < chanceEnemySpawn && totalEnemies < level.maxEnemies && numberOfFloorsSpawned > numberOfFloors / 3)
        {
            foreach (Enemy enemy in level.enemies)
            {
                float roll = UnityEngine.Random.value;
                if (roll < enemy.chanceToSpawn)
                {
                    chanceEnemySpawn = level.resetChanceEnemySpawn;
                    totalEnemies++;
                    GameManager.Instance.AddEnemy(Spawn(x, y, enemy.prefab));
                    return;
                }
            }
        }

        chanceEnemySpawn += level.chanceIncreaseAmount;

    }

    void SpawnCupgrade(int x, int y)
    {
        if (!cupgradeSpawned && numberOfFloorsSpawned > numberOfFloors / 2 &&
            grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x + 1, y] == gridSpace.floor && grid[x - 1, y] == gridSpace.floor)
        {
            cupgradeSpawned = true;
            Spawn(x, y, cupgrade);
        }
    }
    private void SpawnFloor(int x, int y)
    {
        float random = UnityEngine.Random.value;
        if (random < 0.075f)
            Spawn(x, y, level.floorObj1);
        else if (random < 0.15f)
            Spawn(x, y, level.floorObj2);
        else
            Spawn(x, y, level.floorObj);
    }
    private void SetSpawnpoint(int x, int y)
    {
        if (!spawnpointSet && grid[x, y + 1] == gridSpace.bottomWall)
        {
            spawnpointSet = true;
            Spawn(x, y + 1, spawnPoint);
            Vector2 offset = level.roomSizeWorldUnits / 2f;
            GameManager.Instance.SetSpawnPointPosition(new Vector2(x, y) * level.worldUnitsInOneGridCell - offset);
        }
    }
    private void SetEndLevel(int x, int y)
    {
        if (!endLevelSet && numberOfFloorsSpawned > numberOfFloors / 2 && grid[x + 1, y] == gridSpace.wall)
        {
            endLevelSet = true;
            GameManager.Instance.SetEndLevelGO(Spawn(x, y, endLevel));
        }
    }
    private GameObject Spawn(int x, int y, GameObject obj)
    {
        Vector2 offset = level.roomSizeWorldUnits / 2f;
        Vector2 spawnPos = new Vector2(x, y) * level.worldUnitsInOneGridCell - offset;

        return Instantiate(obj, spawnPos, Quaternion.identity, levelHolder);
    }
    public Transform GetLevelHolder()
    {
        return levelHolder;
    }
}
[Serializable]
public class Level
{
    public Vector2 roomSizeWorldUnits = new Vector2(30, 30);
    public float worldUnitsInOneGridCell = 1;
    public int maxWalkers = 10;
    [Range(0, 1)]
    public float chanceWalkerChangeDir = .5f, chanceWalkerSpawn = .05f, chanceWalkerDestroy = .05f, percentToFill = .2f;
    public GameObject wallObj, floorObj, floorObj1, floorObj2, bottomWall;
    [Range(0, 1)]
    public float chanceChestSpawn;
    public int maxChestNumber;
    public Enemy[] enemies;
    public int maxEnemies;
    [Range(0, 1)]
    public float chanceIncreaseAmount;
    [Range(0, 1)]
    public float startingChanceEnemySpawn;
    [Range(0, 1)]
    public float resetChanceEnemySpawn;
}
[Serializable]
public class Enemy
{
    public GameObject prefab;
    [Range(0,1)]
    public float chanceToSpawn;
}