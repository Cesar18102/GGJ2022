using Assets.Scripts;
using System;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject[] _lightBuildings;
    public GameObject[] _darkBuildings;

    public static GameObject[,] _darkBuildingMap;
    public static GameObject[,] _lightBuildingMap;

    public int _widthPadding;
    public int _lengthPadding;

    private System.Random rand = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        var mapGenerator = GetComponent<MapGenerator>();

        _darkBuildingMap = CreateBuildingMap(MapGenerator._darkMap, mapGenerator._darkSpawn);
        _lightBuildingMap = CreateBuildingMap(MapGenerator._lightMap, mapGenerator._lightSpawn);

        var mapBoundSpawner = GetComponent<SpawnMapBounds>();

        _widthPadding = mapBoundSpawner._widthPadding;
        _lengthPadding = mapBoundSpawner._lengthPadding;

        SpawnBuildings(MapGenerator._darkMap, _darkBuildingMap, _darkBuildings, 1);
        SpawnBuildings(MapGenerator._lightMap, _lightBuildingMap, _lightBuildings, -1);
    }

    private void SpawnBuildings(GameObject[,] map, GameObject[,] buildingMap, GameObject[] buildingsToSpawn, int zDirection)
    {
        var tileBounds = map[0, 0].GetComponent<BoxCollider>().bounds;

        foreach (var buildingToSpawn in buildingsToSpawn)
        {
            var buildingSize = buildingToSpawn.GetComponent<BoxCollider>().size;
            var scaledBuildingSize = buildingSize.ScaleBy(buildingToSpawn.transform.localScale);

            var tilesPerWidth = (int)Math.Round(scaledBuildingSize.x / tileBounds.size.x);
            var tilesPerLength = (int)Math.Round(scaledBuildingSize.z / tileBounds.size.z);

            var scaledWidth = tilesPerWidth * tileBounds.size.x;
            var scaledLength = tilesPerLength * tileBounds.size.z;

            var scaleX = scaledWidth / scaledBuildingSize.x;
            var scaleZ = scaledLength / scaledBuildingSize.z;
            var scale = VectorExtensions.GetAveragedByYScale(scaleX, scaleZ);

            var emptyPlace = GetRandomEmptyPlaceOfSize(tilesPerLength, tilesPerWidth, buildingMap);
            var locationTile = map[emptyPlace.y + tilesPerLength / 2, emptyPlace.x + tilesPerWidth / 2];
            var locationTileBounds = locationTile.GetComponent<BoxCollider>().bounds;

            var locationZ = zDirection == 1 ?
                locationTileBounds.min.z - (tilesPerLength % 2 == 0 ? 0 : locationTileBounds.size.z / 2) :
                locationTileBounds.max.z + (tilesPerLength % 2 == 0 ? 0 : locationTileBounds.size.z / 2);


            var location = new Vector3(
                locationTileBounds.max.x + (tilesPerWidth % 2 == 0 ? 0 : locationTileBounds.size.x / 2),
                locationTileBounds.max.y,
                locationZ
            );

            var spawnedBuilding = Instantiate(buildingToSpawn, location, Quaternion.identity, gameObject.transform);
            buildingToSpawn.transform.localScale = buildingToSpawn.transform.localScale.ScaleBy(scale);

            FillBuildingMapWithObject(emptyPlace, new Vector2Int(tilesPerWidth, tilesPerLength), spawnedBuilding, buildingMap);
        }
    }

    private GameObject[,] CreateBuildingMap(GameObject[,] map, GameObject spawn)
    {
        var length = map.GetLength(0);
        var width = map.GetLength(1);

        var buildingMap = new GameObject[length, width];

        for (int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                if (map[i, j] == null)
                {
                    buildingMap[i, j] = spawn;
                }
            }
        }

        return buildingMap;
    }

    private void FillBuildingMapWithObject(Vector2Int pivot, Vector2Int size, GameObject item, GameObject[,] map)
    {
        for(int i = 0; i < size.y; ++i)
        {
            for(int j = 0; j < size.x; ++j)
            {
                map[pivot.y + i, pivot.x + j] = item;
            }
        }
    }

    private Vector2Int GetRandomEmptyPlaceOfSize(int length, int width, GameObject[,] map)
    {
        var y = rand.Next(_lengthPadding + 1, map.GetLength(0) - length - _lengthPadding - 1); //z-axis
        var x = rand.Next(_widthPadding + 1, map.GetLength(1) - width - _widthPadding - 1); // extra padding made to avoid placing buildings ajecent to the border

        for(int i = -1; i <= length; ++i) //should be at leat one block distance from other objects
        { 
            for(int j = -1; j <= width; ++j)
            {
                if(map[y + i, x + j] != null)
                {
                    return GetRandomEmptyPlaceOfSize(length, width, map);
                }
            }
        }

        return new Vector2Int(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
