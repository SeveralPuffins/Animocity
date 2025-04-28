using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities
{
    public class BuildRequirementBlueprint : Blueprint
    {
        private BuildRequirementWorker worker;
        public BuildRequirementWorker Worker
        {
            get
            {
                if (worker == null) worker = (BuildRequirementWorker)Activator.CreateInstance(this.requirementCheckWorker);
                return worker;
            }
        }
        public Type requirementCheckWorker;
    }
}
