using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans
{
   
    public class ModelExplainerDataDto
    {
        // Feature Importance Chart: Key: Feature Name, Value: Score
        public Dictionary<string, decimal> FeatureImportance { get; set; } = new Dictionary<string, decimal>();

        // Loan Type - Feature Contribution Chart (Clustered Bar Chart)
        public List<FeatureContributionPoint> FeatureContribution { get; set; } = new List<FeatureContributionPoint>();
    }
}
