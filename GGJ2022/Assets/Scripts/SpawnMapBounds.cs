using UnityEngine;
using Assets.Scripts;

public class SpawnMapBounds : MonoBehaviour
{
    public GameObject[] _darkBoundaryObjects;
    public GameObject[] _lightBoundarybjects;

    public int _widthPadding; //TODO: take this variables into account in this script
    public int _lengthPadding;

    private int darkMapWidth => MapGenerator._darkMap.GetLength(0);
    private int darkMapHight => MapGenerator._darkMap.GetLength(1);

    private int lightMapWidth => MapGenerator._lightMap.GetLength(0);
    private int lightMapHight => MapGenerator._lightMap.GetLength(1);

    private readonly System.Random rand = new System.Random();

    private class TileBoundInfo
    {
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public float MaxZ { get; set; }
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MinZ { get; set; }
    }

    // Start is called before the first frame update
    void Start()
    { 
        for (int i = 0; i < darkMapWidth; i++)
        {
            if (i == darkMapWidth - 1)
            {
                for (int j = 0; j < darkMapHight; j++)
                {
                    if(MapGenerator._darkMap[i, j] != null)
                    {
                        InstantiateBoundaryObject(MapGenerator._darkMap[i, j], _darkBoundaryObjects, Quaternion.Euler(0, 0, 0));
                    }                    
                }
            }
            else
            {
                InstantiateBoundaryObject(MapGenerator._darkMap[i, 0], _darkBoundaryObjects, Quaternion.Euler(0, 90, 0));
                InstantiateBoundaryObject(MapGenerator._darkMap[i, darkMapHight - 1], _darkBoundaryObjects, Quaternion.Euler(0, -90, 0));
            }
        }

        for (int i = 0; i < lightMapWidth; i++)
        {
            if (i == lightMapWidth - 1)
            {
                for (int j = 0; j < lightMapHight; j++)
                {
                    if (MapGenerator._lightMap[i, j] != null)
                    {
                        InstantiateBoundaryObject(MapGenerator._lightMap[i, j], _lightBoundarybjects);
                    }
                }
            }
            else
            {
                InstantiateBoundaryObject(MapGenerator._lightMap[i, 0], _lightBoundarybjects);
                InstantiateBoundaryObject(MapGenerator._lightMap[i, lightMapHight - 1], _lightBoundarybjects);
            }
        }
    }

    private TileBoundInfo GetTileBoundInfo(GameObject obj)
    {
        var boxCollider = obj.GetComponent<BoxCollider>();

        return new TileBoundInfo()
        {
            MaxX = boxCollider.bounds.max.x,
            MaxY = boxCollider.bounds.max.y,
            MinX = boxCollider.bounds.min.x,
            MinY = boxCollider.bounds.min.y,
            MaxZ = boxCollider.bounds.max.z,
            MinZ = boxCollider.bounds.min.z
        };
    }

    private void InstantiateBoundaryObject(GameObject tile, GameObject[] BoundaryObjects, Quaternion? quaternion = null)
    {
        var TileInfo = GetTileBoundInfo(tile);
        var index = rand.Next(0, BoundaryObjects.Length);
        var spawnedBoundaryObject = Instantiate(
            BoundaryObjects[index],
            new Vector3
            (
                (TileInfo.MaxX + TileInfo.MinX) / 2,
                TileInfo.MaxY,
                (TileInfo.MaxZ + TileInfo.MinZ) / 2
            ),
            quaternion ?? BoundaryObjects[index].transform.rotation,
            gameObject.transform
        );
        spawnedBoundaryObject.AddComponent<BoxCollider>();

        var tileSize = tile.GetComponent<BoxCollider>().bounds.size;
        var BoundaryObjectSize = spawnedBoundaryObject.GetComponent<BoxCollider>().bounds.size;

        var scaleCoefficient = tileSize.DivideBy(BoundaryObjectSize);
        var scale = VectorExtensions.GetAveragedByYScale(scaleCoefficient.x, scaleCoefficient.z);
        spawnedBoundaryObject.transform.localScale = spawnedBoundaryObject.transform.localScale.ScaleBy(scale);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
