using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Errors
{
    public class InvalidStatementError : CompilerError
    {
        public InvalidStatementError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Expected statement but found {token.TokenValue} instead.";
    }
}
