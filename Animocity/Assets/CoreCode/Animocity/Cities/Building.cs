using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

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

        public float BuildingEfficiency
        {
            get
            {
                float eff = 1f;

                foreach (var component in Components)
                {
                    eff = component.ModifyEfficiency(eff);
                }
                return eff;
            }
        }

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
            if (Blue.components != null)
            {
                foreach (var data in Blue.components)
                {
                    var worker = data.GetWorker(this);
                    Components.Add(worker);
                    print($"Making Building Component of type {data.GetType().ToString()} with worker type {worker.GetType().ToString()}");
                }
            }
        }

        public List<T> GetComps<T>() where T : BuildingComponent
        {
            var found = Components.OfType<T>().ToList();
            if (found != null)
            {
                MonoBehaviour.print($"Found {found.Count()} comps");
            }

            foreach ( var component in Components)
            {
                MonoBehaviour.print($"Comp {component.GetType().Name} found on {this.Blue.label}");

                bool isAssignable = typeof(T).IsAssignableFrom(component.GetType());

                MonoBehaviour.print($"{typeof(T).Name} is assignable from {component.GetType().Name} ? -> {isAssignable}");
            }

            return found;
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
