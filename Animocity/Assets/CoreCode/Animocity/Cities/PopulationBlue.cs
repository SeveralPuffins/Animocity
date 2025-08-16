using Animocity.UI;
using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class PopulationBlue : Blueprint
    {
        public string description;
        public string iconPath;
        public PopulationBlue childType;

        public float minComfort;
        public float housingSatisfactionForMinComfort;
        public float housingSatisfactionForMaxComfort;

        private PopWorker worker;
        public PopWorker Worker
        {
            get
            {
                if (worker == null) worker = (PopWorker)Activator.CreateInstance(this.popWorker, new object[]{this});
                return worker;
            }
        }
        public Type popWorker;

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(iconPath);
        }
    }
}
