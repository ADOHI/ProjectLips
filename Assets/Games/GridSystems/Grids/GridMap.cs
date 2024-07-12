using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Grids
{
    public class GridMap
    {
        public enum GridDirection
        {
            Up, Down, Left, Right
        }

        public class Tile
        {
            public int x;
            public int y;
            public Passage upPassage;
            public Passage downPassage;
            public Passage leftPassage;
            public Passage rightPassage;

            public Tile(int x, int y)
            {
                this.x = x;
                this.y = y;
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
            public bool valid;

            public Passage(Tile firstTile, Tile secondTile, bool valid, PassageType type)
            {
                this.firstTile = firstTile;
                this.secondTile = secondTile;
                this.valid = valid;
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
                    passage.valid = false;
                }
            }

            foreach (var passage in horizontalPassages)
            {
                if (Random.Range(0f, 1f) < ratio)
                {
                    passage.valid = false;
                }
            }
        }

        public static GridMap GenerateGrid(int width, int height)
        {
            var grid = new GridMap(width, height);

            return grid;
        }

    }

}
