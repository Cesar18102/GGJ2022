using Assets.Scripts;
using System;
using System.Text;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject _darkFloorTile;
    public GameObject _darkSpawn;

    public GameObject _lightFloorTile;
    public GameObject _lightSpawn;

    public GameObject _middleTile;

    public int _width;
    public int _length;

    public static GameObject[,] _darkMap;
    public static GameObject[,] _lightMap;

    private class FloorSideSpawnInfo
    {
        public GameObject FloorTilePrefab { get; set; }
        public GameObject SpawnObject { get; set; }
        public float TileCenterY { get; set; }
        public int Length { get; set; }
        public bool InverseDirection { get; set; }
    }

    private class FloorSideSpawnResult
    {
        public int CountOfTilesInFrontOfSpawn { get; set; }
        public GameObject[,] OutputMap { get; set; }
    }

    //X-axis is left to right
    //Z-axis is length
    // Start is called before the first frame update
    void Start()
    {
        //<Scaling light spawn to fir dark spawn size>
        var darkSpawn = Instantiate(_darkSpawn, new Vector3(0, 0, 0), _darkSpawn.transform.rotation, gameObject.transform);
        var lightSpawn = Instantiate(_lightSpawn, new Vector3(0, 0, 0), _lightSpawn.transform.rotation, gameObject.transform);

        var darkSpawnBounds = darkSpawn.GetComponent<BoxCollider>().bounds;
        var lightSpawnBounds = lightSpawn.GetComponent<BoxCollider>().bounds;

        var lightSpawnScale = darkSpawnBounds.size.DivideBy(lightSpawnBounds.size);
        Destroy(lightSpawn);
        //</Scaling light spawn to fir dark spawn size>

        //<Calculating scale for light tile to fit size of a dark tile>
        var darkFloorTileBounds = _darkFloorTile.GetComponent<MeshRenderer>().bounds;
        var lightFloorTileBounds = _lightFloorTile.GetComponent<MeshRenderer>().bounds;

        var lightFloorTileScale = darkFloorTileBounds.size.DivideBy(lightFloorTileBounds.size);
        //</Calculating scale for light tile to fit size of a dark tile>

        var darkFloorSpawnInfo = new FloorSideSpawnInfo()
        {
            FloorTilePrefab = _darkFloorTile,
            SpawnObject = darkSpawn,
            Length = _length / 2,
            TileCenterY = darkFloorTileBounds.center.y
        };
        var darkFloorSpawnResult = InstantiateFloorForSide(darkFloorSpawnInfo);
        _darkMap = darkFloorSpawnResult.OutputMap;

        //<Instantiation light spawn at its place on the other side of the map>
        var lightSpawnPosition = new Vector3(0, 0, 2 * darkFloorSpawnResult.CountOfTilesInFrontOfSpawn * darkFloorTileBounds.size.z + darkSpawnBounds.size.z);
        var lightSpawnRotation = _lightSpawn.transform.rotation * new Quaternion(0, 180, 0, 0);

        lightSpawn = Instantiate(_lightSpawn, lightSpawnPosition, lightSpawnRotation, gameObject.transform);

        lightSpawn.transform.localScale = lightSpawn.transform.localScale.ScaleBy(lightSpawnScale);
        //</Instantiation light spawn at its place on the other side of the map>

        var lightFloorSpawnInfo = new FloorSideSpawnInfo()
        {
            FloorTilePrefab = _lightFloorTile,
            SpawnObject = lightSpawn,
            Length = _length / 2,
            TileCenterY = darkFloorTileBounds.center.y,
            InverseDirection = true
        };
        var lightFloorSpawnResult = InstantiateFloorForSide(lightFloorSpawnInfo);
        _lightMap = lightFloorSpawnResult.OutputMap;

        //<Spawning middle tile decoration>
        var middleTileRelyFloorTileBounds = _darkMap[0, _darkMap.GetLength(1) / 2].GetComponent<BoxCollider>().bounds;
        var middleTileLocation = new Vector3(
            middleTileRelyFloorTileBounds.center.x,
            middleTileRelyFloorTileBounds.max.y,
            middleTileRelyFloorTileBounds.max.z
        );
        Instantiate(_middleTile, middleTileLocation, Quaternion.identity, gameObject.transform);
        //<Spawning middle tiledecoration>

        var darkFloorString = GetMapString(darkFloorSpawnResult.OutputMap);
        var lightFloorString = GetMapString(lightFloorSpawnResult.OutputMap);
    }

    private string GetMapString(GameObject[,] map)
    {
        StringBuilder text = new StringBuilder();

        for(int i = 0; i < map.GetLength(0); ++i)
        {
            for(int j = 0; j < map.GetLength(1); ++j)
            {
                text.Append(map[i, j] == null ? "0" : "1");
            }
            text.Append("\n");
        }

        return text.ToString();
    }

    private FloorSideSpawnResult InstantiateFloorForSide(FloorSideSpawnInfo spawnInfo)
    {
        var outputMap = new GameObject[spawnInfo.Length, _width];

        var tileBounds = spawnInfo.FloorTilePrefab.GetComponent<MeshRenderer>().bounds;
        var spawnBounds = spawnInfo.SpawnObject.GetComponent<BoxCollider>().bounds;

        var scaledSpawnBounds = spawnBounds.ScaleBy(spawnInfo.SpawnObject.transform.localScale);

        int countOfTilesBySpawnWidth = (int)(scaledSpawnBounds.size.x / tileBounds.size.x);
        int widthAtTheSideOfSpawn = (_width - countOfTilesBySpawnWidth) / 2;

        Func<int, float> zCalculator = null;

        if (spawnInfo.InverseDirection)
            zCalculator = i => scaledSpawnBounds.max.z - (i + 0.5f) * tileBounds.size.z;
        else
            zCalculator = i => scaledSpawnBounds.min.z + (i + 0.5f) * tileBounds.size.z;

        for (int i = 0; i < spawnInfo.Length; ++i)
        {
            for (int j = 0; j < widthAtTheSideOfSpawn; ++j)
            {
                float centerZ = zCalculator(i);
                float leftSideTileCenterX = scaledSpawnBounds.min.x - (j + 0.5f) * tileBounds.size.x;
                float rightSideTileCenterX = scaledSpawnBounds.max.x + (j + 0.5f) * tileBounds.size.x;

                var leftSideTilePosition = new Vector3(leftSideTileCenterX, spawnInfo.TileCenterY, centerZ);
                outputMap[spawnInfo.Length - i - 1, widthAtTheSideOfSpawn - j - 1] = Instantiate(
                    spawnInfo.FloorTilePrefab, leftSideTilePosition, Quaternion.identity, gameObject.transform
                );

                var rightSideTilePosition = new Vector3(rightSideTileCenterX, spawnInfo.TileCenterY, centerZ);
                outputMap[spawnInfo.Length - i - 1, widthAtTheSideOfSpawn + countOfTilesBySpawnWidth + j] = Instantiate(
                    spawnInfo.FloorTilePrefab, rightSideTilePosition, Quaternion.identity, gameObject.transform
                );
            }
        }

        int countOfTilesBySpawnLength = (int)(scaledSpawnBounds.size.z / tileBounds.size.z);
        int countOfTilesInFrontOfSpawn = spawnInfo.Length - countOfTilesBySpawnLength;

        if (spawnInfo.InverseDirection)
            zCalculator = i => scaledSpawnBounds.min.z - (i + 0.5f) * tileBounds.size.z;
        else
            zCalculator = i => scaledSpawnBounds.max.z + (i + 0.5f) * tileBounds.size.z;

        for (int i = 0; i < countOfTilesInFrontOfSpawn; ++i)
        {
            for (int j = 0; j < countOfTilesBySpawnWidth; ++j)
            {
                float centerZ = zCalculator(i);

                var middleTilePosition = new Vector3(scaledSpawnBounds.min.x + (j + 0.5f) * tileBounds.size.x, spawnInfo.TileCenterY, centerZ);
                outputMap[spawnInfo.Length - countOfTilesBySpawnLength - i - 1, widthAtTheSideOfSpawn + j] = Instantiate(
                    spawnInfo.FloorTilePrefab, middleTilePosition, Quaternion.identity, gameObject.transform
                );
            }
        }

        return new FloorSideSpawnResult()
        {
            CountOfTilesInFrontOfSpawn = countOfTilesInFrontOfSpawn,
            OutputMap = outputMap
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
