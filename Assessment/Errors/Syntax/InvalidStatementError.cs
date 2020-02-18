using AllanMilne.Ardkit;

namespace Assessment.Errors.Syntax
{
    public class InvalidStatementError : CompilerError
    {
        public InvalidStatementError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} '{token.TokenValue}' found where statement expected.";
    }
}
