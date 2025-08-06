using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;

namespace Animocity.Cities
{
    public class WorkforceManager
    {
        public static WorkforceManager Current { get; private set; }
        public WorkforceManager()
        {
            Current = this;
        }


        private List<BuildingComponent_StaffRequirement> _activeWorkplaces = new();
        private List<BuildingComponent_StaffRequirement> _priorityWorkplaces = new();

        public delegate void WorkforceChangeEvent(List<BuildingComponent_StaffRequirement> locationsWithChange);

        public event WorkforceChangeEvent WorkforceUpdated;
        public event WorkforceChangeEvent WorkforceInsufficient;

        private Dictionary<PopulationBlue, int> unassignedWorkers = new();
        private int unassignedHousing = 0;

        public void UpdateWorkforceCommutes()
        {
            ResetAssignments();
           
            if(_priorityWorkplaces.Count()>0) BulkAssignWorkers(_priorityWorkplaces);
            var remainingWorkplaces = _activeWorkplaces.Except(_priorityWorkplaces).ToList();
            if (remainingWorkplaces.Count() > 0) BulkAssignWorkers(remainingWorkplaces);
        }

        public void AddWorkplace(BuildingComponent_StaffRequirement workplace)
        {
            _activeWorkplaces.Add(workplace);
        }
        public void RemoveWorkplace(BuildingComponent_StaffRequirement workplace)
        {
            _activeWorkplaces.Remove(workplace);
        }

        private int GetRemainingAvailableStaff()
        {
            return Math.Min(unassignedHousing, unassignedWorkers.Values.Sum());
        }

        private void BulkAssignWorkers(List<BuildingComponent_StaffRequirement> workplaces)
        {
            int availableStaff = GetRemainingAvailableStaff();
            if (availableStaff <= 0) return;

            int staffDemand = GetTotalStaffRequired(workplaces);

            float approxAvgStaffingLevelGlobal = (1f * availableStaff) / (1f * staffDemand);
            var populationTypes = BlueprintDatabase<PopulationBlue>.FetchAll();

            MonoBehaviour.print($"Trying to assign {availableStaff} workers to {workplaces.Count()} workplaces (approx staffing level of {(int)(approxAvgStaffingLevelGlobal*100.0)}%).");
            //
            // First assign total specialists. If only one population type can do it, assign them.
            //
            var specialistWorkplaces = workplaces.Where((workplace) => workplace.StaffData.populationTypesAccepted.Count() == 1).ToList();

            var specialistAvailability = MaxAchievableSpecialistStaffLevels(specialistWorkplaces);
            foreach (var pop in specialistAvailability.Keys)
            {
                var targetWorkplaces = specialistWorkplaces.Where((workplace) => workplace.StaffData.populationTypesAccepted.Contains(pop));
                AssignWorkersUpToPercentage(targetWorkplaces, Math.Clamp(Math.Min(approxAvgStaffingLevelGlobal, specialistAvailability[pop]), 0f, 1f));
            }
            //
            // Then assign next-most-specialised locations, in order
            //
            for (int i = 2; i <= populationTypes.Count(); i++)
            {
                //availableStaff = GetRemainingAvailableStaff();
                //approxAvgStaffingLevelGlobal = (1f * availableStaff) / (1f * staffDemand);

                var nextMostSpecialisedWorkplaces = workplaces.Where((workplace) => workplace.StaffData.populationTypesAccepted.Count() == i);

                AssignWorkersUpToPercentage(nextMostSpecialisedWorkplaces, Math.Clamp(approxAvgStaffingLevelGlobal, 0f, 1f));
            }
        }

        private void AssignWorkersUpToPercentage(IEnumerable<BuildingComponent_StaffRequirement> targetWorkplaces, float v)
        {
            foreach(var workplace in targetWorkplaces)
            {
                int targetNumberOfEmployees = (int) Math.Round(workplace.StaffData.maxStaff * v);

                AssignWorkers(workplace, targetNumberOfEmployees);
            }
        }

        private void AssignWorkers(BuildingComponent_StaffRequirement workplace, int targetNumberOfEmployees)
        {
            MonoBehaviour.print($"Trying to assign {targetNumberOfEmployees} workers to {workplace.Building.Blue.DisplayName}.");
            int demandRemaining = targetNumberOfEmployees;
            foreach (var pop in workplace.StaffData.populationTypesAccepted.OrderByDescending((p) => unassignedWorkers[p]))
            {
                int assignedPopMax = Math.Min(demandRemaining, unassignedWorkers[pop]);

                if (CityInventory.Current.HousingManager.TryFindHousing(workplace.Building.Grid, workplace.Building.GridLocation, assignedPopMax, out int popsSuccessfullyHoused))
                {
                    demandRemaining -= popsSuccessfullyHoused;
                    unassignedWorkers[pop] -= popsSuccessfullyHoused;
                    unassignedHousing -= popsSuccessfullyHoused;
                    workplace.AddStaff(popsSuccessfullyHoused);

                    MonoBehaviour.print($"Assigned {popsSuccessfullyHoused} {pop.DisplayName} class workers to {workplace.Building.Blue.DisplayName}.");
                }
                else
                {
                    //MonoBehaviour.print("Unable to find path to housing");
                }
                if (demandRemaining <= 0) break;
            }
            
        }

        private void ResetAssignments()
        {
            unassignedWorkers.Clear();
            unassignedWorkers = CityInventory.Current.GetPopulationsByClass();
            unassignedHousing = CityInventory.Current.HousingManager.GetHousingCapacity();
            foreach (var workplace in _activeWorkplaces)
            {
                workplace.ClearStaffForReassignment();
            }
        }

        private int GetTotalStaffRequired(List<BuildingComponent_StaffRequirement> workplaces)
        {
            return workplaces.Sum((wrk) => wrk.StaffData.maxStaff);
        }

        private Dictionary<PopulationBlue, int> GetSpecialistDemand(List<BuildingComponent_StaffRequirement> workplaces)
        {
            var demands = new Dictionary<PopulationBlue, int>();
            foreach (var workplace in workplaces)
            {
                var pops = workplace.StaffData.populationTypesAccepted;

                if(pops.Count() == 1)
                {
                    int additionalDemand = workplace.StaffData.maxStaff;

                    if (demands.TryGetValue(pops[0], out var currentDemand))
                    {
                        demands[pops[0]] = currentDemand + additionalDemand;
                    }
                    else
                    {
                        demands.Add(pops[0], additionalDemand);
                    }
                }
            }
            return demands;
        }

        private Dictionary<PopulationBlue, float> MaxAchievableSpecialistStaffLevels(List<BuildingComponent_StaffRequirement> workplaces)
        {
            var fractionAvailable = new Dictionary<PopulationBlue, float>();
            var specialistSupply = CityInventory.Current.GetPopulationsByClass();
            var specialistDemand = GetSpecialistDemand(workplaces);

            foreach(var pop in specialistDemand.Keys)
            {
                fractionAvailable.Add(pop, (1f * specialistSupply[pop]) / (1f * specialistDemand[pop]));
            }

            return fractionAvailable;   
        }
    }
}
