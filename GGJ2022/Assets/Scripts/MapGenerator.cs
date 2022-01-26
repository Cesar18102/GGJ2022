using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject _floorTile;
    public GameObject _spawnFloor;

    public int _width;
    public int _length;

    private Bounds _spawnBounds;
    private Bounds _floorBounds;

    //X-axis is left to right
    //Z-axis is length
    // Start is called before the first frame update
    void Start()
    {
        _spawnBounds = _spawnFloor.GetComponent<MeshRenderer>().bounds;
        _floorBounds = _floorTile.GetComponent<MeshRenderer>().bounds;

        int countOfTilesBySpawnWidth = (int)(_spawnBounds.size.x / _floorBounds.size.x);
        int widthAtTheSideOfSpawn = (_width - countOfTilesBySpawnWidth) / 2;

        for (int i = 0; i < _length; ++i)
        {
            for(int j = 0; j < widthAtTheSideOfSpawn; ++j)
            {
                InstantiateFloorTile(
                    _spawnBounds.min.x - (j + 0.5f) * _floorBounds.size.x,
                    _spawnBounds.min.z + (i + 0.5f) * _floorBounds.size.z
                );

                InstantiateFloorTile(
                    _spawnBounds.max.x + (j + 0.5f) * _floorBounds.size.x,
                    _spawnBounds.min.z + (i + 0.5f) * _floorBounds.size.z
                );
            }
        }

        int countOfTilesBySpawnLength = (int)(_spawnBounds.size.z / _floorBounds.size.z);
        int lengthInCenter = _length - countOfTilesBySpawnLength;

        for(int i = 0; i < lengthInCenter; ++i)
        {
            for(int j = 0; j < countOfTilesBySpawnWidth; ++j)
            {
                InstantiateFloorTile(
                    _spawnBounds.min.x + (j + 0.5f) * _floorBounds.size.x,
                    _spawnBounds.max.z + (i + 0.5f) * _floorBounds.size.z
                );
            }
        }
    }

    private void InstantiateFloorTile(float centerX, float centerZ)
    {
        var rightSideTilePosition = new Vector3(centerX, _floorBounds.center.y, centerZ);
        Instantiate(_floorTile, rightSideTilePosition, Quaternion.identity, this.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
