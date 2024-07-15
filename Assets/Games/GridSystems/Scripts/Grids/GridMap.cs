using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PL.Systems.Grids
{
    public class GridMap
    {
        public enum GridDirection
        {
            Up, Down, Left, Right, None
        }

        public class Tile
        {
            public int x;
            public int y;
            public Passage upPassage;
            public Passage downPassage;
            public Passage leftPassage;
            public Passage rightPassage;
            public bool valid;

            public Tile(int x, int y)
            {
                this.x = x;
                this.y = y;
                this.valid = true;
            }
        }

        public class Passage
        {
            public enum PassageType
            {
                Vertical,
                Horizontal
            }

            public float x;
            public float y;
            public PassageType type;
            public Tile firstTile;
            public Tile secondTile;
            public bool movable;

            public Passage(Tile firstTile, Tile secondTile, bool valid, PassageType type)
            {
                this.firstTile = firstTile;
                this.secondTile = secondTile;
                this.movable = valid;
                this.type = type;
                this.x = (firstTile.x + secondTile.x) * 0.5f;
                this.y = (firstTile.y + secondTile.y) * 0.5f;
            }


        }



        public int width;
        public int height;
        public Tile[,] tiles;
        public Passage[,] verticalPassages;
        public Passage[,] horizontalPassages;


        public int centerWidth => width / 2;
        public int centerHeight => height / 2;

        public Vector3 center => new Vector3(centerWidth, 0f, centerHeight);

        public GridMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            tiles = new Tile[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }

            horizontalPassages = new Passage[width - 1, height];

            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    horizontalPassages[i, j] = new Passage(tiles[i, j], tiles[i + 1, j], true, Passage.PassageType.Horizontal);
                }
            }

            verticalPassages = new Passage[width, height - 1];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    verticalPassages[i, j] = new Passage(tiles[i, j], tiles[i, j + 1], true, Passage.PassageType.Vertical);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i != 0)
                    {
                        tiles[i, j].leftPassage = horizontalPassages[i - 1, j];
                    }

                    if (i != width - 1)
                    {
                        tiles[i, j].rightPassage = horizontalPassages[i, j];
                    }

                    if (j != 0)
                    {
                        tiles[i, j].upPassage = verticalPassages[i, j - 1];
                    }

                    if (j != height - 1)
                    {
                        tiles[i, j].downPassage = verticalPassages[i, j];
                    }
                }
            }

        }


        public void BlockPassage(float ratio)
        {
            foreach (var passage in verticalPassages)
            {
                if (Random.Range(0f, 1f) < ratio)
                {
                    passage.movable = false;
                }
            }

            foreach (var passage in horizontalPassages)
            {
                if (Random.Range(0f, 1f) < ratio)
                {
                    passage.movable = false;
                }
            }
        }

        public static GridMap GenerateGrid(int width, int height)
        {
            var grid = new GridMap(width, height);

            return grid;
        }

        public static bool IsNearTile(int x1, int x2, int y1, int y2, GridMap gridMap)
        {
            if (x1 < 0 || x1 >= gridMap.width)
            {
                return false;
            }

            if (x2 < 0 || x2 >= gridMap.width)
            {
                return false;
            }

            if (y1 < 0 || y1 >= gridMap.height)
            {
                return false;
            }

            if (y2 < 0 || y2 >= gridMap.height)
            {
                return false;
            }

            if (x1 == x2 && Mathf.Abs(y1 - y2) == 1)
            {
                return true;
            }

            if (y1 == y2 && Mathf.Abs(x1 - x2) == 1)
            {
                return true;
            }

            return false;
        }

        public bool IsNearTile(int x1, int y1, int x2, int y2)
        {
            return IsNearTile(x1, x2, y1, y2, this);
        }

        public bool IsPassAvailable(int x1, int y1, int x2, int y2)
        {
            if (IsNearTile(x1, y1, x2, y2))
            {
                Debug.Log((x1, y1, x2, y2, GetDirection(x1, y1, x2, y2)));
                switch (GetDirection(x1, y1, x2, y2))
                {
                    case GridDirection.Up:
                        return verticalPassages[x1, y2].movable;
                    case GridDirection.Down:
                        return verticalPassages[x1, y1].movable;
                    case GridDirection.Left:
                        return horizontalPassages[x1, y1].movable;
                    case GridDirection.Right:
                        return horizontalPassages[x2, y1].movable;
                    case GridDirection.None:
                        break;
                }
            }
            return false;
        }

        public GridDirection GetDirection(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2)
            {
                if (y1 > y2)
                {
                    return GridDirection.Up;
                }
                else if (y1 < y2)
                {
                    return GridDirection.Down;
                }
            }
            if (y1 == y2)
            {
                if (x1 < x2)
                {
                    return GridDirection.Left;
                }
                else if (x1 > x2)
                {
                    return GridDirection.Right;
                }
            }

            return GridDirection.None;
        }

        public bool IsValidTile(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public List<(int, int)> GetEffectiveGrid(int leftX, int downY, int maskWidth, int maskLength)
        {
            var result = new List<(int, int)>();
            for (int i = 0; i < maskWidth; i++)
            {
                for (int j = 0; j < maskLength; j++)
                {
                    var maskableX = leftX + i;
                    var maskableY = downY + j;


                    if (maskableX >= 0 && maskableX < width && maskableY >= 0 && maskableY < height)
                    {
                        result.Add((maskableX, maskableY));
                    }
                }
            }

            return result;
        }

        public List<(int, int)> GetEffectiveGrid(int leftX, int downY, List<(int, int)> masks)
        {
            var result = new List<(int, int)>();
            foreach (var mask in masks)
            {
                var maskableX = leftX + mask.Item1;
                var maskableY = downY + mask.Item2;

                if (maskableX >= 0 && maskableX < width && maskableY >= 0 && maskableY < height)
                {
                    result.Add((maskableX, maskableY));
                }
            }

            return result;
        }
        
        public static List<(int, int)> GenerateMask(int x, int y)
        {
            var result = new List<(int, int)>();
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    result.Add((i, j));
                }
            }
            return result;
        }
        
        public List<(int, int)> AvailableGrids(int markerWidth, int markerHeight)
        {
            var result = new List<(int, int)>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i + markerWidth - 1 < width && j + markerHeight - 1 < height)
                    {
                        result.Add((i, j));
                    }
                }
            }
            return result;
        }
    }

}
