using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.DynamicCondition
{
    public static class ConditionExtensions
    {
        public static Condition Where(this IDataCommand command, string placeholder = "#where#")
        {
            return new Condition(command, placeholder);
        }
    }
}