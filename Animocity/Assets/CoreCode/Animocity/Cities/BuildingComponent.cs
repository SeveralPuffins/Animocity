using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Animocity.Cities
{
    public class BuildingComponent
    {
        public Building Building { get; protected set; }
        public BuildingComponentData Data
        {
            get; protected set;
        }
        public BuildingComponent(BuildingComponentData data, Building building)
        {
            Data = data;
            Building = building;
            Building.Tick += this.Tick;
            Building.LongTick += this.LongTick;
            OnBuild();
        }

        protected virtual bool HasInspector () => false;

        protected virtual void OnBuild()
        {

        }
        public virtual void OnDemolish()
        {

        }

        public virtual float ModifyEfficiency(float efficiency)
        {
            return efficiency;
        }

        protected virtual bool Tick(Building building)
        {
            return true;
        }

        protected virtual bool LongTick(Building building)
        {
            return true;
        }

        public virtual void AddInspectorInfo(BuildingInspectorComp inspector, bool select = false)
        {
            if (Building.IsPlan)
            {
                MonoBehaviour.print("PLAN building should not have comps");
                //PopulateBlueprintInspectorContentPane(inspector.contentPane);
            }
            else if (HasInspector())
            {
                Button tabButton = UIPrefabHelpers.Current.GetInspectorButton();
                tabButton.transform.GetChild(0).GetComponent<Image>().sprite = this.Data.GetSprite();
                tabButton.onClick.AddListener(() =>
                {
                    OnSelectInspectorTab(inspector);
                    tabButton.GetComponent<Image>().color = new Color(0.1f, 0.3f, 0.1f, 0.5f);
                });
                tabButton.transform.SetParent(inspector.tabPane);

                OnSelectInspectorTab(inspector);
            }
        }

        protected virtual void OnSelectInspectorTab(BuildingInspectorComp inspector)
        {
            inspector.UnselectAllTabs();
            inspector.ClearContentPane();
            this.PopulateInspectorContentPane(inspector.contentPane);
        }

        protected virtual void PopulateInspectorContentPane(Transform inspectorPane)
        {

        }

        protected virtual void PopulateBlueprintInspectorContentPane(Transform inspectorPane)
        {
            
        }
    }
}
