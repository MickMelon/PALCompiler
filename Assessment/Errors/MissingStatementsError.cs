using AllanMilne.Ardkit;

namespace Assessment.Errors
{
    public class MissingStatementsError : CompilerError
    {
        public MissingStatementsError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Missing statement(s) between 'IN' and 'END'";
    }
}
