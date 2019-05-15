namespace Domain.Entities
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