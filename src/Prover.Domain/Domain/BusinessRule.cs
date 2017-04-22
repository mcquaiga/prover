namespace Prover.Domain.Model.Domain
{
    public class BusinessRule
    {
        public BusinessRule(string ruleDescription)
        {
            RuleDescription = ruleDescription;
        }

        public string RuleDescription { get; }
    }
}