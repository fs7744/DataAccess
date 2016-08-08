using VIC.DataAccess.DynamicCondition;

namespace VIC.DataAccess.MSSql
{
    public static class ConditionOperaterExtensions
    {
        public static IConditionOperater Equal(this string field, string paramter)
        {
            return new InfixConditionOperater(field, "=", paramter);
        }

        public static IConditionOperater NotEqual(this string field, string paramter)
        {
            return new InfixConditionOperater(field, "<>", paramter);
        }

        public static IConditionOperater LessThan(this string field, string paramter)
        {
            return new InfixConditionOperater(field, "<", paramter);
        }

        public static IConditionOperater LessThanOrEqual(this string field, string paramter)
        {
            return new InfixConditionOperater(field, "<=", paramter);
        }

        public static IConditionOperater GreaterThan(this string field, string paramter)
        {
            return new InfixConditionOperater(field, ">", paramter);
        }

        public static IConditionOperater GreaterThanOrEqual(this string field, string paramter)
        {
            return new InfixConditionOperater(field, ">=", paramter);
        }

        public static IConditionOperater Between(this string field, string left, string right)
        {
            return new InfixConditionOperater(field, "BETWEEN", $"{left} AND {right}");
        }

        public static IConditionOperater Like(this string field, string paramter)
        {
            return new InfixConditionOperater(field, "LIKE", paramter);
        }

        public static IConditionOperater In(this string field, params string[] paramters)
        {
            var ps = string.Join(",", paramters);
            return new InfixConditionOperater(field, "IN", $"({ps})");
        }

        public static IConditionOperater Exists(this string field, string subQuery)
        {
            return new InfixConditionOperater(field, "EXISTS", subQuery);
        }

        public static IConditionOperater Not(this IConditionOperater op)
        {
            return new PrefixConditionOperater("NOT", op.ToString());
        }

        public static string ToDbStr(this string paramter)
        {
            return $"'{paramter}'";
        }
    }
}