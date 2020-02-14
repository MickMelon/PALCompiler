using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Errors
{
    public class InvalidTypeError : CompilerError
    {
        public InvalidTypeError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Expected 'INTEGER' or 'REAL' but found {token.TokenValue}";
    }
}
