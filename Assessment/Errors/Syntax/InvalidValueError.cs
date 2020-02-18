using AllanMilne.Ardkit;

namespace Assessment.Errors.Syntax
{
    public class InvalidValueError : CompilerError
    {
        public InvalidValueError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Found '{token.TokenValue}' where 'Identifier', 'Integer', or 'Real' expected.";
    }
}
