using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private List<BuildingComponent_StaffRequirement> _workplaces = new();

        public delegate void WorkforceChangeEvent(List<BuildingComponent_StaffRequirement> locationsWithChange);

        public event WorkforceChangeEvent WorkforceUpdated;
        public event WorkforceChangeEvent WorkforceInsufficient;

        public Dictionary<PopulationBlue, int> unassignedWorkers = new();
        private int unassignedHousing = 0;

        public void UpdateWorkforceAssignments()
        {
            ResetAssignments();

            var workplacesInPriorityOrder = 

                _workplaces.Where((wp) => wp.Priority > 0 && wp.AcceptedPops.Count() > 0)
                       .GroupBy((wp) => wp.Priority)
                       .OrderByDescending(group => group.Key)
                       .Select(group=>group.AsEnumerable())
                       .ToList();

            foreach (var priorityWorkplaceGroup in workplacesInPriorityOrder)
            {
                if
                (
                    priorityWorkplaceGroup.Count() > 0
                    && unassignedHousing > 0
                    && unassignedWorkers.Values.Sum() > 0
                )
                {
                    BulkAssignWorkers(priorityWorkplaceGroup.ToList());
                }
            }
        }

        public void AddWorkplace(BuildingComponent_StaffRequirement workplace)
        {
            _workplaces.Add(workplace);
        }
        public void RemoveWorkplace(BuildingComponent_StaffRequirement workplace)
        {
            _workplaces.Remove(workplace);
        }

        private int GetTotalRemainingAvailableStaff()
        {
            return Math.Min(unassignedHousing, unassignedWorkers.Values.Sum());
        }

        private void BulkAssignWorkers(List<BuildingComponent_StaffRequirement> workplaces)
        {
            float totalStaffWanted = 1f * GetTotalStaffRequired(workplaces);
            // Here, we iterate through specialist jobs, in order of possible satisfaction 
            // so that those places that can't possibly meet the average due to a lack of specialists
            // can let us increase the target worker number for other jobs later

            List<BuildingComponent_StaffRequirement> finalAssignments = new();

            var specialistWorkplaces = workplaces.Where((workplace) => workplace.AcceptedPops.Count() == 1 && unassignedWorkers[workplace.AcceptedPops.FirstOrDefault()] > 0).ToList();


            var specialistJobDemand = GetSpecialistJobDemand(specialistWorkplaces);



            var orderedSpecialistWorkplaces = specialistWorkplaces.OrderBy(wp =>
                            {
                                var pop = wp.AcceptedPops.FirstOrDefault();
                                float jobCount = specialistJobDemand[pop];
                                float workers = unassignedWorkers[pop];

                                return workers / jobCount;
                            }).ToList();

            foreach( var wp in orderedSpecialistWorkplaces)
            { 
                float availableStaff = 1f * GetTotalRemainingAvailableStaff();

                if (availableStaff <= 0f) break;

                var pop = wp.AcceptedPops.FirstOrDefault();
                float jobCount = 0;
                if (specialistJobDemand.TryGetValue(pop, out var jobs))
                {
                    jobCount = 1f * jobs;
                }
                else continue;
                    
                    
                float specialists = unassignedWorkers[pop];

                if (specialists > 0)
                {
                    float maxSpecialistAvailability = specialists / jobCount;
                    float maxGeneralAvailability = availableStaff / totalStaffWanted;

                    float targetWorkerDensity = Math.Clamp(Math.Min(maxSpecialistAvailability, maxGeneralAvailability), 0, 1);
                    int targetNumberOfEmployees = (int)Math.Round(wp.StaffData.maxStaff * targetWorkerDensity);

                    if(AssignWorkers(wp, targetNumberOfEmployees, out int successes))
                    {
                        if(wp.CurrentStaff < wp.StaffData.maxStaff)
                        {
                            finalAssignments.Add(wp);
                        }
                    }
                }

                specialistJobDemand[pop] -= wp.StaffData.maxStaff;
                totalStaffWanted -= wp.StaffData.maxStaff;
            }

            //
            // Then assign remaining locations. We could try to order these.
            //

            var remainingWorkplaces = workplaces.Where((workplace) => workplace.AcceptedPops.Count() > 1).ToList();

            foreach (var wp in remainingWorkplaces)
            {
                float availableStaff = 1f * GetTotalRemainingAvailableStaff();

                if (availableStaff <= 0f) break;
                
                float targetDensity = Math.Clamp(availableStaff / totalStaffWanted, 0,1);
                int targetNumberOfEmployees = (int)Math.Round(wp.StaffData.maxStaff * targetDensity);

                if(AssignWorkers(wp, targetNumberOfEmployees, out int successes))
                {
                    if (wp.CurrentStaff < wp.StaffData.maxStaff)
                    {
                        finalAssignments.Add(wp);
                    }
                }

                totalStaffWanted -= wp.StaffData.maxStaff;
            }

            foreach (var wp in finalAssignments)
            {
                AssignWorkers(wp, wp.StaffData.maxStaff - wp.CurrentStaff, out var assigned);
            }
        }


        private bool AssignWorkers(BuildingComponent_StaffRequirement workplace, int targetNumberOfEmployees, out int successfullyAssigned)
        {
            int demandRemaining = targetNumberOfEmployees;
            successfullyAssigned = 0;
            foreach (var pop in workplace.StaffData.populationTypesAccepted.OrderByDescending((p) => unassignedWorkers[p]))
            {
                int assignedPopMax = Math.Min(demandRemaining, unassignedWorkers[pop]);
                if (assignedPopMax <= 0) break;

                if (CityOverview.Current.HousingManager.TryFindAcceptableCommute(workplace.Building.Grid, workplace.Building.GridLocation, pop, assignedPopMax, out int popsSuccessfullyHoused))
                {
                    successfullyAssigned += popsSuccessfullyHoused;
                    demandRemaining -= popsSuccessfullyHoused;
                    unassignedWorkers[pop] -= popsSuccessfullyHoused;
                    unassignedHousing -= popsSuccessfullyHoused;
                    workplace.AddStaff(popsSuccessfullyHoused);

                    //MonoBehaviour.print($"Assigned {popsSuccessfullyHoused} {pop.DisplayName} class workers to {workplace.Building.Blue.DisplayName}.");
                }
                else
                {
                    //MonoBehaviour.print("Unable to find path to housing");
                } 
            }
            return successfullyAssigned > 0;
        }

        private void ResetAssignments()
        {
            CityOverview.Current.HousingManager.ResetResidences();
            unassignedWorkers.Clear();
            unassignedWorkers = CityOverview.Current.GetPopulationsByClass();
            unassignedHousing = CityOverview.Current.HousingManager.GetHousingCapacity();
            foreach (var workplace in _workplaces)
            {
                workplace.ClearStaffForReassignment();
            }
        }

        private int GetTotalStaffRequired(List<BuildingComponent_StaffRequirement> workplaces)
        {
            return workplaces.Sum((wrk) => wrk.StaffData.maxStaff);
        }

        private Dictionary<PopulationBlue, int> GetSpecialistJobDemand(List<BuildingComponent_StaffRequirement> specialistWorkplaces)
        {
            Dictionary<PopulationBlue, int> specialistJobs = new Dictionary<PopulationBlue, int>();

            foreach (var specialistJob in specialistWorkplaces)
            {
                var pop = specialistJob.AcceptedPops.FirstOrDefault();
                if (specialistJobs.TryGetValue(pop, out var jobCount))
                {
                    specialistJobs[pop] = jobCount + specialistJob.StaffData.maxStaff;
                }
                else
                {
                    specialistJobs.Add(pop, specialistJob.StaffData.maxStaff);
                }
            }
            return specialistJobs;
        }
    }
}
