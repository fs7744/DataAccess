namespace VIC.DataAccess.DynamicCondition
{
    public class InfixConditionOperater : IConditionOperater
    {
        private string _Left;
        private string _OP;
        private string _Right;

        public InfixConditionOperater(string left, string op, string right)
        {
            _Left = left;
            _OP = op;
            _Right = right;
        }

        public override string ToString()
        {
            return $"{_Left} {_OP} {_Right}";
        }
    }
}