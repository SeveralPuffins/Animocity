using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Animocity.Cities
{
    public class FleaCircusManager
    {
        private Transform _cachedPrefab;
        private string prefabPath = "People/Mechizen";
        public Transform GetPrefab()
        {
            if (!_cachedPrefab)
            {
                _cachedPrefab = Resources.Load<Transform>(prefabPath);
            }
            return _cachedPrefab;
        }
        public void MakeCommuter(Commute commute)
        {
            MonoBehaviour.print("Making commuter!");
            var commuter = Transform.Instantiate(GetPrefab()).AddComponent<CommuteComp>();
            commuter.commute = commute;
        }
    }
}
