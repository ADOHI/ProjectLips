using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PL.Systems.Ingames
{
    public class IngameManager : Singleton<IngameManager>
    {
        private int lastPhase = 2;
        public IntReference phase;

        public void IncreasePhase()
        {
            if (phase.Value < lastPhase)
            {
                phase.Value++;
            }
        }

        public void EndGame()
        {
            SceneManager.LoadScene(2);
        }
    }

}
