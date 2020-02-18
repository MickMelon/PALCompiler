using AllanMilne.Ardkit;

namespace Assessment.Errors.Syntax
{
    public class InvalidFactorError : CompilerError
    {
        public InvalidFactorError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} '{token.TokenValue}' found where factor expected.";
    }
}
