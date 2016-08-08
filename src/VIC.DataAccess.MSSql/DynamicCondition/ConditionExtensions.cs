using VIC.DataAccess.DynamicCondition;

namespace VIC.DataAccess.MSSql
{
    public static class ConditionExtensions
    {
        public static Condition Where(this string sql, IConditionOperater op, string placeholder = "#where#")
        {
            return new Condition(sql, placeholder, op);
        }
    }
}