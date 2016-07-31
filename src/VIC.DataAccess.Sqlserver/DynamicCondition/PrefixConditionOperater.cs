namespace VIC.DataAccess.DynamicCondition
{
    public class PrefixConditionOperater : IConditionOperater
    {
        private string _OP;
        private string _Right;

        public PrefixConditionOperater(string op, string right)
        {
            _OP = op;
            _Right = right;
        }

        public override string ToString()
        {
            return $"{_OP} {_Right}";
        }
    }
}