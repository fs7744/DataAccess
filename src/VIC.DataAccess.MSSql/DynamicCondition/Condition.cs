using System.Text;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.DynamicCondition
{
    public class Condition
    {
        private IDataCommand _Command;
        private string _Placeholder;
        private StringBuilder _SB = new StringBuilder("WHERE");

        public Condition(IDataCommand command, string placeholder)
        {
            _Command = command;
            _Placeholder = placeholder;
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

        public void Apply()
        {
            if (string.IsNullOrEmpty(_Command.CommandText)) return;
            _Command.CommandText = _Command.CommandText.Replace(_Placeholder, _SB.ToString());
        }
    }
}