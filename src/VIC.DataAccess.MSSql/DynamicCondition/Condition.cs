using System.Text;

namespace VIC.DataAccess.DynamicCondition
{
    public class Condition
    {
        protected string _Sql;
        protected string _Placeholder;
        protected StringBuilder _SB = new StringBuilder("WHERE");

        public Condition(string sql, string placeholder, IConditionOperater op)
        {
            _Sql = sql;
            _Placeholder = placeholder;
            _SB.Append($" {op.ToString()}");
        }

        public Condition And(IConditionOperater op)
        {
            _SB.Append($" AND {op.ToString()}");
            return this;
        }

        public Condition Or(IConditionOperater op)
        {
            _SB.Append($" Or {op.ToString()}");
            return this;
        }

        public string Build()
        {
            return _Sql?.Replace(_Placeholder, _SB.ToString()) ?? string.Empty;
        }

        public string BuildToSubQuery()
        {
            return $"({Build()})";
        }
    }
}