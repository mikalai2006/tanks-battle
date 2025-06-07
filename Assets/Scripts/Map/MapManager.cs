using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    private GameSetting _gameSetting => GameManager.Instance.Settings;
    public GridTileHelper gridTileHelper;
    [SerializeField] Tilemap map;
    public Tilemap Map => map;
    [SerializeField] Tilemap mapBorder;
    [SerializeField] Tilemap mapObjects;
    [SerializeField] Tilemap mapDamages;

    public void CreateMap()
    {
        gridTileHelper = new GridTileHelper(_gameManager.LevelConfig.gridSize.x, _gameManager.LevelConfig.gridSize.y);

        // Random value for noise.
        var xOffSet = Random.Range(-10000f, 10000f);
        var zOffSet = Random.Range(-10000f, 10000f);

        for (int x = 0; x < _gameManager.LevelConfig.gridSize.x; x++)
        {
            for (int y = 0; y < _gameManager.LevelConfig.gridSize.y; y++)
            {
                // TileBase tile = map.GetTile(new Vector3Int(x, y));
                var position = new Vector3Int(x, y);

                GridTileNode node = gridTileHelper.GetNode(new Vector3Int(x, y));

                float noiseValue = Mathf.PerlinNoise(
                    x * _gameManager.LevelConfig.noiseScaleKoof + xOffSet,
                    y * _gameManager.LevelConfig.noiseScaleKoof + zOffSet
                );

                bool isFirstLand = noiseValue < _gameManager.LevelConfig.noiseMaxKoof;
                if (isFirstLand)
                {
                    map.SetTile(position, _gameManager.LevelConfig.tileLandscape);
                }
                else
                {
                    map.SetTile(position, _gameManager.LevelConfig.tileSecondLandscape);

                }

                float noiseForBorder = Mathf.PerlinNoise(
                    x * _gameManager.LevelConfig.noiseScaleObstacleKoof + xOffSet,
                    y * _gameManager.LevelConfig.noiseScaleObstacleKoof + zOffSet
                );

                bool isBorderCenter = noiseForBorder > _gameManager.LevelConfig.noiseObstacleMaxKoof;

                bool isBorder = x == 0 || y == 0 || x == _gameManager.LevelConfig.gridSize.x - 1 || y == _gameManager.LevelConfig.gridSize.y - 1;
                if (isBorder || isBorderCenter)
                {
                    mapBorder.SetTile(position, _gameManager.LevelConfig.tileBorder);
                    node.SetDisableNode();
                    // mapBorder.SetColor(position, Color.black);
                }


                bool isObstacle = Random.Range(0f, 1f) < 0.01f;
                if (isObstacle && (x > 2 || y > 2 || x < _gameManager.LevelConfig.gridSize.x - 2 || y < _gameManager.LevelConfig.gridSize.y - 2))
                {
                    mapBorder.SetTile(position, _gameManager.LevelConfig.tileObstcles[Random.Range(0, _gameManager.LevelConfig.tileObstcles.Count - 1)]);
                    node.SetDisableNode();
                    // mapBorder.SetColor(position, Color.black);
                }
            }
        }
    }

    public void OnSetColor(GridTileNode node, Color color)
    {
        Vector3Int posTile = map.WorldToCell(node.position);

        map.SetColor(posTile, color);
    }
}
