using Animocity.Cities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BlueprintSystem;
using System.Linq;
using System;
using UnityEditorInternal;
using Animocity;

public class CityOverview : MonoBehaviour
{

    public PowerGrid PowerGrid { get; private set; }
    public HousingManager HousingManager { get; private set; }
    public WorkforceManager WorkforceManager { get; private set; }
    public TransportManager TransportManager { get; private set; }

    public static CityOverview Current;

    private const float POPULATION_GROWTH_RATE_PER_MIN = 0.5f;

    private Dictionary<PopulationBlue, int> _population;
    private Dictionary<ResourceBlue, float> _resources;

    public List<CityGrid> cityGrids;

    private float _fractionalPopulationGrowth = 0f;

    private PopulationBlue birthPop;

    private bool _init;
    
    // Start is called before the first frame update
    void Awake()
    {
        DataLoader.OnDataLoaded += this.Init;
        DataLoader.OnDataCleared += this.Clear;

        this._population = new Dictionary<PopulationBlue, int>();
        this._resources = new Dictionary<ResourceBlue, float>();
        this.PowerGrid = new(new(), new(), new(), new());
        this.HousingManager = new HousingManager(cityGrids);
        this.WorkforceManager = new();
        this.TransportManager = new TransportManager(this);

        Current = this;
    }

    private void OnDestroy()
    {
        DataLoader.OnDataLoaded -= this.Init;
        DataLoader.OnDataCleared -= this.Clear;
    }

    private void Clear(PlayerProfile profile, DataLoader.LoadStatus Status)
    {
        _population.Clear();
    }

    private void Init(PlayerProfile profile, DataLoader.LoadStatus Status)
    {
        var scenario = BlueprintDatabase<ScenarioBlue>.FetchAllWhere((blue)=>blue.isDefault).FirstOrDefault();
        InitialisePopulation(scenario);
        InitialiseResources(scenario);
        _init = true;
    }

    private void InitialisePopulation(ScenarioBlue scenario)
    {
        
        foreach (var blue in BlueprintDatabase<PopulationBlue>.FetchAll())
        {
            int pop = 0;
            scenario.startingPopulations.TryGetValue(blue, out pop);

            _population.Add(blue, pop);
            if(blue.birthType) birthPop = blue;
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
        if(! _init) return;
        UpdatePopulation();
        ticks++;
        if(ticks >= 100)
        {
            ticks = 0;
            WorkforceManager.UpdateWorkforceCommutes();
        }
    }

    private void UpdatePopulation()
    {
        int capacity = HousingManager.GetHousingCapacity();

        float housingSatisfaction;

        float tp = TotalPopulation;


        _fractionalPopulationGrowth += tp * GetSatisfaction() * POPULATION_GROWTH_RATE_PER_MIN * Time.deltaTime / 60f;

        if(_fractionalPopulationGrowth > 1f) 
        {
            int newPops = (int)_fractionalPopulationGrowth;

            _fractionalPopulationGrowth -= newPops;

            _population[birthPop] += newPops;
        }
    }

    public float GetSatisfaction()
    {
        return HousingManager.GetHousingSatisfaction(TotalPopulation);
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
            if(_population.Count == 0) return 0;
            return _population.Values.Sum();
        }
    }

    public int GetPopulationByClass(PopulationBlue blue)
    {
        if(_population.TryGetValue(blue, out var population))
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
        return _resources.Keys.Sum((k)=> k.value * GetResourceAmount(k));
    }

    public float GetTotalWhere(Func<ResourceBlue,bool> pred)
    {
        return _resources.Keys.Where((k)=>pred(k)).Sum((k) => GetResourceAmount(k));
    }

    internal bool HasResources(Dictionary<ResourceBlue, float> inputs)
    {
        foreach(var resource in inputs.Keys)
        {
            if (_resources[resource] < inputs[resource])
            {
                return false;
            }
        }
        return true;
    }

    internal void TakeResource(Vector2Int gridLocation, ResourceBlue resource, float v)
    {
        _resources[resource] -= v;
    }

    internal void PushResource(Vector2Int gridLocation, ResourceBlue resource,  float v)
    {
        _resources[resource] += v;
    }
}
