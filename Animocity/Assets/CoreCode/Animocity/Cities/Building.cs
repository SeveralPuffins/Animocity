using Assets.CoreCode.Animocity.Cities;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Animocity.Utilities;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Animocity.Cities
{
    public class Building : MonoBehaviour
    {
        public const float SECONDS_PER_TICK = 0.25f;
        public const int TICKS_TO_LONGTICKS = 20;

        public BuildingBlueprint Blue { get; private set; }
        public CityGrid Grid { get; private set; }
        public Vector2Int GridLocation { get; private set; }

        public bool IsPlan { get; private set; }

        protected List<BuildingComponent> Components { get; private set; }
        private float _time;
        private int _ticks;

        public float BuildingEfficiency
        {
            get
            {
                if (IsPlan)
                {
                    return 0;
                }
                float eff = 1f;

                foreach (var component in Components)
                {
                    eff = component.ModifyEfficiency(eff);
                }
                return eff;
            }
        }

        public bool CanAfford()
        {
            if (IsPlan)
            {
                return CityOverview.Current.HasResources(this.Blue.resourceCosts);
            }
            return true;
        }

        // "Ghosts" "Selected"

        public void SetBuildingOutlined(bool isOutlined)
        {
            /*int iOut = isOutlined ? 1 : 0;

            MaterialPropertyBlock selectBlock = new MaterialPropertyBlock();

            selectBlock.SetInt( "_OutlineOn", iOut);

            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
            {
                r.SetPropertyBlock(selectBlock);
            }*/

            if (isOutlined) SetBuildingLayer("Selected");
            else SetBuildingLayer("Default");

        }

        public void SetBuildingBlueprint(bool isBlueprint)
        {
            int iBlue = isBlueprint ? 1 : 0;

            MaterialPropertyBlock selectBlock = new MaterialPropertyBlock();

            selectBlock.SetInt("_Hologram", iBlue);

            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
            {
                r.SetPropertyBlock(selectBlock);
            }
        }

        protected void SetBuildingLayer(string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            gameObject.layer = layer;

            MonoBehaviour.print($"Setting layer to {layer}");

            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = layer;
            }
        }

        private void PayFor()
        {
            foreach (var key in Blue.resourceCosts.Keys)
            {
                CityOverview.Current.TakeResource(GridLocation, key, Blue.resourceCosts[key]);
            }
        }
        private void Refund()
        {
            foreach (var key in Blue.resourceCosts.Keys)
            {
                CityOverview.Current.PushResource(GridLocation, key, Blue.resourceCosts[key]);
            }
        }

        public bool TryCommitBuild(bool isFree)
        {
            if (IsPlan)
            {
                if (isFree || CanAfford())
                {
                    IsPlan = false;
                    InitialiseBuilding();
                    SetBuildingBlueprint(false);
                    if (!isFree)
                    {
                        PayFor();
                    }
                }
            }
            return false;
        }

        private void InitialiseBuilding()
        {
            FillComponents();
            _time = Random.Range(0f, SECONDS_PER_TICK);
            _ticks = Random.Range(0, TICKS_TO_LONGTICKS);
        }

        public static Building AddToGameObject(GameObject go, BuildingBlueprint blue, CityGrid grid, Vector2Int loc)
        {
            var building = go.AddComponent<Building>();
            building.Blue = blue;
            building.Grid = grid;
            building.GridLocation = loc;
            building.IsPlan = true;
            building.SetBuildingBlueprint(true);
            foreach(var req in building.Blue.buildRequirements)
            {
                req.Worker.OnBuildAtLocation(loc, building, grid);
            }
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
                    //print($"Making Building Component of type {data.GetType().ToString()} with worker type {worker.GetType().ToString()}");
                }
            }
        }

        public List<T> GetComps<T>() where T : BuildingComponent
        {
            var found = Components.OfType<T>().ToList();

            /*
            foreach ( var component in Components)
            {
                MonoBehaviour.print($"Comp {component.GetType().Name} found on {this.Blue.label}");
                bool isAssignable = typeof(T).IsAssignableFrom(component.GetType());
                MonoBehaviour.print($"{typeof(T).Name} is assignable from {component.GetType().Name} ? -> {isAssignable}");
            }*/

            return found;
        }



        // Update is called once per frame
        void Update()
        {
            if (IsPlan) return;
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

        private void OnDisable()
        {
            if (Components != null)
            {
                Components.Clear();
            }
            Tick = null;
            LongTick = null;
            foreach(var b in supportBuildings)
            {
                b.CheckThreatenedBuildings -= this.ReportSupported;
            }
        }

        private void ReportSupported(Building supporter, DemolitionEventArgs e)
        {  
            e.buildingsThreatened.Add(this);
            this.CheckThreatenedBuildings?.Invoke(this, e);       
        }

        private HashSet<Building> supportBuildings = new();
        public void SubscribeToSupporters(HashSet<Building> supporters)
        {
            supportBuildings.AddRange(supporters);
            foreach(var supporter in supporters)
            {
                supporter.CheckThreatenedBuildings += this.ReportSupported;
            }
        }

        internal void DemolishSelf(HashSet<Building> allDemolishedBuildings)
        {
            DemolitionEventArgs threatenedLocations = new DemolitionEventArgs();
            CheckThreatenedBuildings?.Invoke(this, threatenedLocations);

            if (threatenedLocations.buildingsThreatened.All(threatened => allDemolishedBuildings.Contains(threatened)))
            {
                MonoBehaviour.print("All supported buildings are contained in demolish list");
                if (this.Grid.TryRemoveBuildingAt(this.GridLocation, this))
                {
                    this.Components.Clear();
                    this.Refund();
                    Destroy(this.gameObject);
                }
            }
            else
            {
                MonoBehaviour.print("Some threatened buildings are not in demolish list");
            }
        }
        public delegate void DemolitionCheckEvent(Building supporter, DemolitionEventArgs e);
        public event DemolitionCheckEvent CheckThreatenedBuildings;



        public delegate bool TickEvent(Building building);
        public event TickEvent Tick;
        public event TickEvent LongTick;

    }
}
