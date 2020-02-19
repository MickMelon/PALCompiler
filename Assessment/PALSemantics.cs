using AllanMilne.Ardkit;
using System;

namespace Assessment
{
    public class PALSemantics : Semantics
    {
        public PALSemantics(IParser parser) : base(parser)
        {
        }

        public void DeclareIdentifier(IToken identifier, int type)
        {
            if (!identifier.Is(Token.IdentifierToken))
                return;

            if (Scope.CurrentScope.IsDefined(identifier.TokenValue))
                semanticError(new AlreadyDeclaredError(identifier, Scope.CurrentScope.Get(identifier.TokenValue)));
            else
                Scope.CurrentScope.Add(new VarSymbol(identifier, type));
        }

        public bool IsIdentifierDeclared(IToken identifier)
        {
            return Scope.CurrentScope.IsDefined(identifier.TokenValue);
        }

        public int GetIdentifierType(IToken identifier)
        {
            if (!IsIdentifierDeclared(identifier)) 
                return LanguageType.Undefined;

            return Scope.CurrentScope.Get(identifier.TokenValue).Type;
        }

        public int CheckIdentifier(IToken identifier)
        {
            if (!identifier.Is(Token.IdentifierToken))
                return LanguageType.Undefined;

            if (!Scope.CurrentScope.IsDefined(identifier.TokenValue))
            {
                semanticError(new NotDeclaredError(identifier));
                return LanguageType.Undefined;
            }

            return Scope.CurrentScope.Get(identifier.TokenValue).Type;
        }

        public void VerifyType(IToken token, int type, int expected)
        {
            if (type != expected)
                semanticError(new TypeConflictError(token, type, expected));
        }

        public void CheckBooleanExpr(IToken leftToken, IToken rightToken)
        {
            if (leftToken.TokenType != PALType.ToString(LanguageType.Boolean) ||
                rightToken.TokenType != PALType.ToString(LanguageType.Boolean))
            {
             //   semanticError(new TypeConflictError(leftToken, , LanguageType.Boolean));

            }
        }
    }
}
