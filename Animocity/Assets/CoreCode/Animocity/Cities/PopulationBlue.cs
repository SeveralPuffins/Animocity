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
        public string fleaPrefabPath;
        public PopulationBlue childType;

        public float minComfort;
        public float housingSatisfactionForMinComfort;
        public float housingSatisfactionForMaxComfort;

        private Transform _cachedPrefab;
        public Transform GetPrefab()
        {
            if (!_cachedPrefab)
            {
                MonoBehaviour.print($"Loading resource from path in mod Resource folder: {fleaPrefabPath}");
                _cachedPrefab = Resources.Load<Transform>(fleaPrefabPath);
            }
            return _cachedPrefab;
        }

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
