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
        public string iconPath;

        public BuildingComponent GetWorker(Building building)
        {
            Type componentType = Type.GetType(componentClass);
            return (BuildingComponent) Activator.CreateInstance(componentType, new object[]{this, building});
        }

        private Sprite _sprite;

        public Sprite GetSprite()
        {
            if( _sprite == null)
            {
                _sprite = Resources.Load<Sprite>(iconPath);
            }
            return _sprite;
        }
    }
}
