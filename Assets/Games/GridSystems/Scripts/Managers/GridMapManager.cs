using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PL.Systems.Grids
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        public int width;
        public int height;

        [HideInInspector] public GridMap gridMap;

        private void Awake()
        {
            gridMap = new GridMap(width, height);
        }
    }


}

