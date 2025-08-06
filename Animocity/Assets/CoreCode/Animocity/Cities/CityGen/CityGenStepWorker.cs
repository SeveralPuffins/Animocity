using Animocity.Cities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities.CityGen
{
    public abstract class CityGenStepWorker
    {
        public CityGeneratorStepBlue Blue { get; protected set; }

        public CityGenStepWorker(CityGeneratorStepBlue blue)
        {
            this.Blue = blue;   
        }

        public virtual void Run(List<CityGrid> cityGrids)
        {

        }
    }
}