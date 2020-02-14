using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Errors
{
    public class InvalidBooleanExprError : CompilerError
    {
        public InvalidBooleanExprError(IToken where) : base(where)
        {
        }

        public override string ToString()
            => $"{base.ToString()} Expected boolean expression but found {token.TokenValue}";
    }
}
