using AllanMilne.Ardkit;

namespace Assessment.Errors.Syntax
{
    public class InvalidTypeError : CompilerError
    {
        public InvalidTypeError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Found '{token.TokenValue}' where 'INTEGER' or 'REAL' expected.";
    }
}
