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
        public void MakeCommuter(Commute commute)
        {
            MonoBehaviour.print("Making commuter!");
            var commuter = Transform.Instantiate(commute.PopulationType.GetPrefab()).AddComponent<CommuteComp>();
            commuter.commute = commute;
        }
    }
}
