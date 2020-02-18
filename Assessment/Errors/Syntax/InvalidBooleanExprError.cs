using AllanMilne.Ardkit;

namespace Assessment.Errors.Syntax
{
    public class InvalidBooleanExprError : CompilerError
    {
        public InvalidBooleanExprError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} '{token.TokenValue}' found where boolean expected.";
    }
}
