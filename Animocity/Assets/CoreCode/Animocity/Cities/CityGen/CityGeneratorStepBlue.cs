using BlueprintSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities.CityGen
{
    public class CityGeneratorStepBlue : Blueprint
    {
        public string displayName;
        public override string DisplayName => displayName;
        private CityGenStepWorker worker;
        public CityGenStepWorker Worker
        {
            get
            {
                if (worker == null) worker = (CityGenStepWorker)Activator.CreateInstance(this.genStepWorker, new object[] { this });
                return worker;
            }
        }
        public Type genStepWorker;

        public float paramA;
        public float paramB;
        public float paramC;
        public float paramD;

        public string stringA;
        public string stringB;
        public string stringC;
    }
}