using Animocity;
using Animocity.Cities;
using Animocity.Cities.CityGen;
using BlueprintSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ZLinq;

namespace Animocity.Cities
{
    public class CityOverview : MonoBehaviour
    {

        public PowerGrid PowerGrid { get; private set; }
        public HousingManager HousingManager { get; private set; }
        public WorkforceManager WorkforceManager { get; private set; }
        public TransportManager TransportManager { get; private set; }
        public FleaCircusManager FleaCircusManager { get; private set; }

        private float _realMinutesPerDay = 2;
        private float _timeOfDay = 6;
        public float TimeOfDay
        {
            get
            {
                return _timeOfDay;
            }
        }

        public static CityOverview Current;

        public bool HomelessAreFed { get; private set; } = true;

        private Dictionary<PopulationBlue, int> _population;
        private Dictionary<ResourceBlue, float> _resources;

        [SerializeField]
        private List<CityGrid> cityGrids;

        public MultiGrid CityMultiGrid;

        private Dictionary<PopulationBlue, float> _fractionalPopulationGrowth;

        public Dictionary<PopulationBlue, int> Homeless { get; private set; }

        private bool _init;

        // Start is called before the first frame update
        void Awake()
        {
            this.CityMultiGrid = new MultiGrid();
            foreach(var g in cityGrids)
            {
                CityMultiGrid.AddGrid(g);
            }
            CityMultiGrid.BuildBridges();

            DataLoader.OnDataLoaded += this.Init;
            DataLoader.OnDataCleared += this.Clear;


            this._population = new Dictionary<PopulationBlue, int>();
            this._resources = new Dictionary<ResourceBlue, float>();
            this.Homeless = new();
            this._fractionalPopulationGrowth = new();
            this.PowerGrid = new(new(), new(), new(), new());
            this.HousingManager = new HousingManager();
            this.WorkforceManager = new();
            this.TransportManager = new TransportManager(this);
            this.FleaCircusManager = new();

            Current = this;
        }

        private void OnDestroy()
        {
            DataLoader.OnDataLoaded -= this.Init;
            DataLoader.OnDataCleared -= this.Clear;
        }

        private bool _firstGen = true;
        private void RunGenSteps()
        {
            if (_firstGen)
            {
                var steps = BlueprintDatabase<CityGeneratorStepBlue>.FetchAll();
                print($"Initialising city with {steps.Count()} steps");
                _firstGen = false;
                foreach (var item in steps)
                {
                        print($"Running worker {item.displayName}..");
                        item.Worker.Run(CityMultiGrid.GetAllGrids().ToList());
                }
            }
        }

        private void Clear(PlayerProfile profile, DataLoader.LoadStatus Status)
        {
            _population.Clear();
        }

        private void Init(PlayerProfile profile, DataLoader.LoadStatus Status)
        {
            var scenario = BlueprintDatabase<ScenarioBlue>.FetchAllWhere((blue) => blue.isDefault).FirstOrDefault();
            InitialisePopulation(scenario);
            InitialiseResources(scenario);
            RunGenSteps();
            _init = true;
        }

        private void InitialisePopulation(ScenarioBlue scenario)
        {

            foreach (var blue in BlueprintDatabase<PopulationBlue>.FetchAll())
            {
                scenario.startingPopulations.TryGetValue(blue, out var pop);

                _population.Add(blue, pop);
                _fractionalPopulationGrowth.Add(blue, 0);
                Homeless.Add(blue, 0);
            }
        }
        private void InitialiseResources(ScenarioBlue scenario)
        {
            foreach (var blue in BlueprintDatabase<ResourceBlue>.FetchAll())
            {
                scenario.startingResources.TryGetValue(blue, out float inv);
                _resources.Add(blue, inv);
            }
        }

        int ticks = 0;
        private void Update()
        {

            if (!_init) return;

            _timeOfDay = (_timeOfDay + (24*Time.deltaTime / (60f*_realMinutesPerDay))) % 24;
            //MonoBehaviour.print($"Time is {(int)_timeOfDay}");

            ticks++;
            if (ticks == 50)
            {
                UpdatePopulation();
            }
            if (ticks >= 100)
            {
                ticks = 0;
                WorkforceManager.UpdateWorkforceAssignments();
                this.Homeless = HousingManager.GetHomelessAfterHousingUnemployed();
                HousingManager.GetFoodNeedRoutes();
                
            }
        }

        private void UpdatePopulationFedRate()
        {
            if (HousingManager.FeedTheHomeless(100f * Time.deltaTime / 60f))
            {

            }
        }

        private void UpdatePopulation()
        {
            float globalHomelessness = Homeless.Values.Sum() * 1f / this.TotalPopulation;

            foreach (var pop in this._population.Keys.ToArray())
            {
                int population = _population[pop];
                if (population == 0) continue;

                this.Homeless.TryGetValue(pop, out int homeless);

                float homelessFraction = homeless * 1f / (1f * population);
                float housedFractionSatisfaction = HousingManager.GetHousingSatisfaction(pop);
                float popSatisfaction = housedFractionSatisfaction * (1f - homelessFraction) * (1f - globalHomelessness);
                this._fractionalPopulationGrowth[pop.childType] += pop.Worker.GetChangeInPopulation(population, popSatisfaction) * Time.deltaTime;

                if (_fractionalPopulationGrowth[pop.childType] > 1f)
                {
                    _population[pop.childType] += 1;
                    _fractionalPopulationGrowth[pop.childType] -= 1;
                }
            }
        }


        public float PowerSupply
        {
            get
            {
                return PowerGrid.GetPowerProduced();
            }
        }

        public float PowerDemand
        {
            get
            {
                return PowerGrid.GetPowerUsed();
            }
        }

        public int TotalPopulation
        {
            get
            {
                if (_population.Count == 0) return 0;
                return _population.Values.Sum();
            }
        }

        public int GetPopulationByClass(PopulationBlue blue)
        {
            if (_population.TryGetValue(blue, out var population))
            {
                return population;
            }
            return 0;
        }
        public Dictionary<PopulationBlue, int> GetPopulationsByClass()
        {
            return new(_population);
        }

        public float GetResourceAmount(ResourceBlue blue)
        {
            return _resources[blue];
        }

        public float GetTotalResourceValue()
        {
            return _resources.Keys.AsValueEnumerable().Sum((k) => k.value * GetResourceAmount(k));
        }

        public float GetTotalWhere(Func<ResourceBlue, bool> pred)
        {
            return _resources.Keys.AsValueEnumerable().Where((k) => pred(k)).Sum((k) => GetResourceAmount(k));
        }

        internal bool HasResources(Dictionary<ResourceBlue, float> inputs)
        {
            foreach (var resource in inputs.Keys)
            {
                if (_resources[resource] < inputs[resource])
                {
                    return false;
                }
            }
            return true;
        }
        internal bool HasResource(ResourceBlue resource, float val)
        {
            return (_resources[resource] >= val);
        }

        internal void TakeResource(Vector2Int gridLocation, ResourceBlue resource, float v)
        {
            _resources[resource] -= v;
        }

        internal void PushResource(Vector2Int gridLocation, ResourceBlue resource, float v)
        {
            _resources[resource] += v;
        }
    }
}