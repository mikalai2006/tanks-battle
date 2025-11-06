using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;
public class MazeGenerator : MonoBehaviour
{
    [SerializeField] MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeHeight;
    [SerializeField] private MazeCell[,] _mazeGrid;

    public void Create(int width, int height)
    {
        _mazeWidth = width;
        _mazeHeight = height;
        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeHeight; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0f, z), Quaternion.identity, transform);
                var gpuIEs = _mazeGrid[x, z].gameObject.GetComponentsInChildren<GPUInstanceEnabler>();
                for (int q = 0; q <  gpuIEs.Count(); q++)
                {
                    // go.gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    Color _color = new Color(0f, 1f, 0f, 1.0f);
                    _color.a = 1f;
                    gpuIEs[q].SetColor(_color);
                }
                _mazeGrid[x, z].transform.localPosition = new Vector3(x, 0f, z);
            }
        }

        GenerateMaze(null, _mazeGrid[0,0]);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.localPosition.x;
        int z = (int)currentCell.transform.localPosition.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
         if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }
         if (z + 1 < _mazeHeight)
        {
            var cellToFront = _mazeGrid[x, z+1];
            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
         if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z-1];
            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.localPosition.x < currentCell.transform.localPosition.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if (previousCell.transform.localPosition.x > currentCell.transform.localPosition.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }
        if (previousCell.transform.localPosition.z < currentCell.transform.localPosition.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.localPosition.z > currentCell.transform.localPosition.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
}
