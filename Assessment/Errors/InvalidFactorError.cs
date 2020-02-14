using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Errors
{
    public class InvalidFactorError : CompilerError
    {
        public InvalidFactorError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Expected factor but found {token.TokenValue}";
    }
}
