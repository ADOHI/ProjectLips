using PL.Systems.Grids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    private GridMap grid;

    public int width;
    public int height;
    public float wallRatio = 0.1f;
    public GameObject block;
    public GameObject wall;
    public float scale = 0.95f;
    // Start is called before the first frame update
    void Start()
    {
        grid = new GridMap(width, height);
        grid.BlockPassage(wallRatio);
        Spawn();
    }

    public void Spawn()
    {
        for (int i = 0; i < grid.width; i++)
        {
            for(int j = 0; j < grid.height; j++)
            {
                var spawnedBlock = Instantiate(block);

                spawnedBlock.transform.SetParent(transform, false);
                spawnedBlock.transform.position = new Vector3(i, 0f, j);
                spawnedBlock.transform.localScale = Vector3.one * scale;
            }
        }

        foreach (var item in grid.horizontalPassages)
        {
            if (!item.valid)
            {
                var spawnedWall = Instantiate(wall);

                spawnedWall.transform.SetParent(transform, false);
                spawnedWall.transform.position = new Vector3(item.x, 0.5f, item.y);
                spawnedWall.transform.localScale = new Vector3(scale, 0.2f, 0.05f);
            }
        }

        foreach (var item in grid.verticalPassages)
        {
            if (!item.valid)
            {
                var spawnedWall = Instantiate(wall);

                spawnedWall.transform.SetParent(transform, false);
                spawnedWall.transform.position = new Vector3(item.x, 0.5f, item.y);
                spawnedWall.transform.localScale = new Vector3(0.05f, 0.2f, scale);
            }
        }
    }
}
