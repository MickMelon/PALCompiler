using AllanMilne.Ardkit;
using System;

namespace Assessment
{
    public class PALSemantics : Semantics
    {
        public PALSemantics(IParser parser) : base(parser)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="type"></param>
        public void DeclareIdentifier(IToken identifier, int type)
        {
            if (!identifier.Is(Token.IdentifierToken))
                return;

            var symbols = Scope.CurrentScope;

            if (symbols.IsDefined(identifier.TokenValue))
                semanticError(new AlreadyDeclaredError(identifier, symbols.Get(identifier.TokenValue)));
            else
                symbols.Add(new VarSymbol(identifier, type));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool IsIdentifierDeclared(IToken identifier)
        {
            return Scope.CurrentScope.IsDefined(identifier.TokenValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public int GetIdentifierType(IToken identifier)
        {
            if (!IsIdentifierDeclared(identifier)) 
                return LanguageType.Undefined;

            return Scope.CurrentScope.Get(identifier.TokenValue).Type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <param name="expected"></param>
        public void VerifyType(IToken token, int type, int expected)
        {
            if (type != expected)
                semanticError(new TypeConflictError(token, type, expected));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftToken"></param>
        /// <param name="rightToken"></param>
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
