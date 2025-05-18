using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponentData
    {

        public string componentClass;

        public BuildingComponent GetWorker(Building building)
        {
            MonoBehaviour.print($"Component Class is {componentClass}");
            Type componentType = Type.GetType(componentClass);
            MonoBehaviour.print($"Type is {componentType.FullName}");
            return (BuildingComponent) Activator.CreateInstance(componentType, new object[]{this, building});
        }
    }
}
