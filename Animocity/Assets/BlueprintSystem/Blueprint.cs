using System;
using System.Reflection;

namespace BlueprintSystem
{
	public class Blueprint
	{
		public string label;
		internal ModInfo ParentMod; // First mod to create the bp. Note that fields may be overwritten by OTHER mods.

		public Blueprint (){}

		public override string ToString(){
			return string.Format("{0} labelled {1}", this.GetType().Name, label);
		}

		public virtual string DisplayName{
			get{
				return label;
			}
		}

		/*
		* Derived classes could speed this up by not using reflection...
		*/


		public virtual void Init(){}
	}
}

