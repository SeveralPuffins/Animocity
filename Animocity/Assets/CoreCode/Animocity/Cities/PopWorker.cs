using System;
using UnityEngine;

namespace Animocity.Cities
{
    public class PopWorker
    {
        public PopulationBlue Blue { get; set; }
        public PopWorker(PopulationBlue blue)
        {
            MonoBehaviour.print("MAKING POPWORKER FOR BLUE " + blue.DisplayName);
            this.Blue = blue;
        }
       
        public float GetHousingComfort(float currentHouseSatisfaction)
        {
            float t = (currentHouseSatisfaction - Blue.housingSatisfactionForMinComfort) / (Blue.housingSatisfactionForMaxComfort - Blue.housingSatisfactionForMinComfort);
            return Blue.minComfort + Mathf.Lerp(Blue.minComfort, 1f, t);
        }

        public float GetChangeInPopulation(int population, float popComfort)
        {
            return 0.01f * population * popComfort;
        }
    }
}
