using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Animocity.Cities
{
    public class Building : MonoBehaviour
    {
        public const float SECONDS_PER_TICK = 0.25f;
        public const int TICKS_TO_LONGTICKS = 20;

        public BuildingBlueprint Blue { get; private set; }
        public Vector2Int GridLocation { get; private set; }
        protected List<BuildingComponent> Components { get; private set; }
        private float _time;
        private int _ticks;
        public static Building AddToGameObject(GameObject go, BuildingBlueprint blue, Vector2Int loc)
        {
            var building = go.AddComponent<Building>();
            building.Blue = blue;
            building.GridLocation = loc;
            building.FillComponents();

            building._time = Random.Range(0f, SECONDS_PER_TICK);
            building._ticks = Random.Range(0, TICKS_TO_LONGTICKS);

            return building;
        }

        private void FillComponents()
        {
            Components = new List<BuildingComponent>();
            foreach(var data in Blue.components)
            {
                var worker = data.GetWorker(this);
                Components.Add(worker);
                print($"Making Building Component of type {data.GetType().ToString()} with worker type {worker.GetType().ToString()}");
            }
        }

        public List<T> GetComps<T>() where T : BuildingComponent
        {
           return Components.Where((comp)=> comp.GetType().IsAssignableFrom(typeof(T))).ToList() as List<T>;
        }


        // Update is called once per frame
        void Update()
        {
            UpdateTicks();
        }

        private void UpdateTicks()
        {
            var newTime = (_time + Time.deltaTime) % SECONDS_PER_TICK;
            if (newTime < _time)
            {
                Tick?.Invoke(this);
                _ticks++;
            }
            if (_ticks >= TICKS_TO_LONGTICKS)
            {
                _ticks = 0;
                LongTick?.Invoke(this);
            }
            _time = newTime;
        }

        public delegate bool TickEvent(Building building);
        public event TickEvent Tick;
        public event TickEvent LongTick;

    }
}
