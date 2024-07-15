using PL.Systems.Grids.MapBlocks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Grids
{
    public class GridMapSpawner : MonoBehaviour
    {
        private GridMap gridMap => GridMapManager.Instance.gridMap;
        public MapBlock blackBlock;
        public MapBlock whiteBlock;
        public GameObject wall;
        public float widthScale = 0.95f;
        public float heightScale = 10f;

        //public Color blackCheckerBoardColor;
        //public Color whiteCheckerBoardColor;
        // Start is called before the first frame update
        void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            //Spawn blackBlock
            for (int i = 0; i < gridMap.width; i++)
            {
                for (int j = 0; j < gridMap.height; j++)
                {
                    
                    var spawnedBlock = ((i + j) % 2 == 0) ? Instantiate(blackBlock) : Instantiate(whiteBlock);

                    spawnedBlock.transform.SetParent(transform, false);
                    spawnedBlock.transform.position = new Vector3(i, 0f, j);
                    spawnedBlock.transform.localScale = new Vector3(widthScale, heightScale, widthScale);
                    spawnedBlock.x = i;
                    spawnedBlock.y = j;
                    spawnedBlock.gameObject.SetActive(true);
                }
            }

            foreach (var item in gridMap.horizontalPassages)
            {
                if (!item.movable)
                {
                    var spawnedWall = Instantiate(wall);

                    spawnedWall.transform.SetParent(transform, false);
                    spawnedWall.transform.position = new Vector3(item.x, 0.5f, item.y);
                    spawnedWall.transform.localScale = new Vector3(widthScale, 0.2f, 0.05f);
                }
            }

            foreach (var item in gridMap.verticalPassages)
            {
                if (!item.movable)
                {
                    var spawnedWall = Instantiate(wall);

                    spawnedWall.transform.SetParent(transform, false);
                    spawnedWall.transform.position = new Vector3(item.x, 0.5f, item.y);
                    spawnedWall.transform.localScale = new Vector3(0.05f, 0.2f, widthScale);
                }
            }
        }
    }

}
