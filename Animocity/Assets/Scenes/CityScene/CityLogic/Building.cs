using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities
{
    public class Building : MonoBehaviour
    {
        public BuildingBlueprint Blue { get; private set; }
        public Vector2Int GridLocation { get; private set; }

        public static Building AddToGameObject(GameObject go, BuildingBlueprint blue, Vector2Int loc)
        {
            var building = go.AddComponent<Building>();
            building.Blue = blue;
            building.GridLocation = loc;
            return building;
        }

        public Vector2Int rootPosition;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
