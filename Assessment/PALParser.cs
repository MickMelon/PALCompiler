using AllanMilne.Ardkit;
using Assessment.Errors;
using Assessment.Errors.Syntax;
using System.Collections.Generic;

namespace Assessment
{
    public class PALParser : RecoveringRdParser
    {
        private PALSemantics semantics;

        public PALParser(IScanner scan) : base(scan)
        {
            semantics = new PALSemantics(this);
        }

        /// <summary>
        /// 
        /// <Program> ::= PROGRAM Identifier
        ///               WITH <VarDecls>
        ///               IN (<Statement>)+
        ///               END ;
        ///               
        /// </summary>
        protected override void recStarter()
        {
            Scope.OpenScope();

            mustBe("PROGRAM");
            mustBe(Token.IdentifierToken);

            mustBe("WITH");
            RecVarDecls();

            mustBe("IN");
            if (!HaveStatement())
                syntaxError(new MissingStatementsError(scanner.CurrentToken));
            else
                RecStatements();

            mustBe("END");

            Scope.CloseScope();
        }

        /// <summary>
        /// 
        /// <VarDecls> ::= (<IdentList> AS <Type>)* ;
        /// 
        /// </summary>
        private void RecVarDecls()
        {
            // Var declarations will continue while there are identifiers
            while (have(Token.IdentifierToken))
            {
                var identifiers = RecIdentList();
                mustBe("AS");
                var type = RecType();
                identifiers.ForEach(i => semantics.DeclareIdentifier(i, type));
            }            
        }

        /// <summary>
        /// 
        /// <IdentList> ::= Identifier ( , Identifier)* ;
        /// 
        /// </summary>
        private List<IToken> RecIdentList()
        {
            var identifiers = new List<IToken>();

            // There must be at least one identifier
            identifiers.Add(scanner.CurrentToken);
            mustBe(Token.IdentifierToken);
            

            // If there is a comma, then there must be more identifiers specified
            while (have(","))
            {
                mustBe(",");
                identifiers.Add(scanner.CurrentToken);
                mustBe(Token.IdentifierToken);                
            }

            return identifiers;
        }

        /// <summary>
        /// 
        /// <Type> ::= REAL | INTEGER ;
        /// 
        /// </summary>
        private int RecType()
        {
            if (have("REAL"))
            {
                mustBe("REAL");
                return LanguageType.Real;
            }
                
            if (have("INTEGER"))
            {
                mustBe("INTEGER");
                return LanguageType.Integer;
            }
                
            syntaxError(new InvalidTypeError(scanner.CurrentToken));
            return LanguageType.Undefined;
        }

        /// <summary>
        /// Keeps reading statements until there is none left.
        /// </summary>
        private void RecStatements()
        {
            // If any of the below tokens are found, then
            // the statement list must've ended.
            while (!have("ELSE") &&
                   !have("ENDIF") &&
                   !have("ENDLOOP") &&
                   !have("END") &&
                   !have(Token.EndOfFile) &&
                   !have(Token.InvalidChar) &&
                   !have(Token.InvalidToken))
            {
                // Parse assignment
                if (!HaveStatement()) break;
                RecStatement();
            }
        }

        /// <summary>
        /// Checks if we current token is a valid statement
        /// </summary>
        private bool HaveStatement()
        {
            return have(Token.IdentifierToken) ||
                   have("UNTIL") ||
                   have("IF") ||
                   have("INPUT") ||
                   have("OUTPUT");
        }

        /// <summary>
        /// 
        /// <Statement> ::= <Assignment> | <Loop> | <Conditional> | <I-o> ;
        /// 
        /// </summary>
        private void RecStatement()
        {
            if (have(Token.IdentifierToken))
            {
                RecAssignment();
            }

            // Parse loop
            else if (have("UNTIL"))
            {
                RecLoop();
            }

            // Parse conditional
            else if (have("IF"))
            {
                RecConditional();
            }

            // Parse IO
            else if (have("INPUT") || have("OUTPUT"))
            {
                RecIo();
            }

            // Must be one of the above, if not then there is an
            // error in the syntax.
            else
            {
                syntaxError(new InvalidStatementError(scanner.CurrentToken));
            }

        }

        /// <summary>
        /// 
        /// <Assignment> ::= Identifier = <Expression> ;
        /// 
        /// </summary>
        private void RecAssignment()
        {
            var type = semantics.CheckIdentifier(scanner.CurrentToken);
            mustBe(Token.IdentifierToken);
            mustBe("=");
            RecExpression();
        }

        /// <summary>
        /// 
        /// <Loop> ::= UNTIL <BooleanExpr> REPEAT (<Statement>)* ENDLOOP ;
        /// 
        /// </summary>
        private void RecLoop()
        {
            mustBe("UNTIL");
            RecBooleanExpr();
            mustBe("REPEAT");
            RecStatements();
            mustBe("ENDLOOP");
        }

        /// <summary>
        /// 
        /// <Conditional> ::= IF <BooleanExpr> THEN (<Statement>)*
        ///                       ( ELSE (<Statement>)* )?
        ///                       ENDIF ;
        /// 
        /// </summary>
        private void RecConditional()
        {
            mustBe("IF");
            RecBooleanExpr();
            mustBe("THEN");
            RecStatements();
            
            if (have("ELSE"))
            {
                mustBe("ELSE");
                RecStatements();
            }

            mustBe("ENDIF");
        }

        /// <summary>
        /// 
        /// <I-o> ::= INPUT <IdentList> |
        ///           OUTPUT <Expression> ( , <Expression>)* ;
        /// 
        /// </summary>
        private void RecIo()
        {
            if (have("INPUT"))
            {
                mustBe("INPUT");
                RecIdentList();
            }
            else
            {
                mustBe("OUTPUT");

                // Must be at least one expression.
                RecExpression();

                // If there is a comma, there must be at least
                // one more expression.
                while (have(","))
                {
                    mustBe(",");
                    RecExpression();
                }
            }
        }

        /// <summary>
        /// 
        /// <BooleanExpr> ::= <Expression> ("<" | "=" | ">") <Expression> ;
        /// 
        /// </summary>
        private void RecBooleanExpr()
        {
            RecExpression();

            if (have("<"))
                mustBe("<");
            else if (have("="))
                mustBe("=");
            else if (have(">"))
                mustBe(">");
            else
                syntaxError(new InvalidBooleanExprError(scanner.CurrentToken));

            RecExpression();
        }

        /// <summary>
        /// 
        /// <Expression> ::= <Term> ( (+|-) <Term>)* ;
        /// 
        /// </summary>
        private int RecExpression()
        {
            int leftType, rightType;
            IToken leftToken, rightToken;

            leftToken = scanner.CurrentToken;
            leftType = RecTerm();

            while (have("+") || have("-"))
            {
                if (have("+"))
                    mustBe("+");
                else
                    mustBe("-");

                rightToken = scanner.CurrentToken;
                rightType = RecTerm();

                semantics.VerifyType(rightToken, rightType, leftType);
            }

            return leftType;
        }

        /// <summary>
        /// 
        /// <Term> ::= <Factor> ( (*|/) <Factor>)* ;
        /// 
        /// </summary>
        private int RecTerm()
        {
            int leftType, rightType;
            IToken token;
            leftType = RecFactor();
            
            while (have("*") || have("/"))
            {
                if (have("*"))
                {
                    mustBe("*");
                    token = scanner.CurrentToken;
                    rightType = RecFactor();
                }
                else
                {
                    mustBe("/");
                    token = scanner.CurrentToken;
                    rightType = RecFactor();
                }

                semantics.VerifyType(token, leftType, rightType);
            }

            return leftType;
        }

        /// <summary>
        /// 
        /// <Factor> ::= (+|-)? ( <Value> | "(" <Expression> ")" ) ;
        /// 
        /// </summary>
        private int RecFactor()
        {
            var type = LanguageType.Undefined;

            if (have("+"))
                mustBe("+");
            else if (have("-"))
                mustBe("-");

            if (HaveValue())
            {
                type = RecValue();
            }
            else if (have("("))
            {
                mustBe("(");
                type = RecExpression();
                mustBe(")");
            }
            else
                syntaxError(new InvalidFactorError(scanner.CurrentToken));

            return type;
        }

        /// <summary>
        /// 
        /// <Value> ::= Identifier | IntegerValue | RealValue ;
        /// 
        /// </summary>
        private int RecValue()
        {
            if (have(Token.IdentifierToken))
            {
                int type = semantics.CheckIdentifier(scanner.CurrentToken);
                mustBe(Token.IdentifierToken);
                return type;
            }

            if (have(Token.IntegerToken))
            {
                mustBe(Token.IntegerToken);
                return LanguageType.Integer;
            }
                
            
            if (have(Token.RealToken))
            {
                mustBe(Token.RealToken);
                return LanguageType.Real;
            }

            syntaxError(new InvalidValueError(scanner.CurrentToken));
            return LanguageType.Undefined;
        }

        /// <summary>
        /// Checks if we have an identifier, integer, or real token.
        /// </summary>
        private bool HaveValue()
        {
            return have(Token.IdentifierToken) || have(Token.IntegerToken) || have(Token.RealToken);
        }

        private void syntaxError(ICompilerError error)
        {
            if (IsRecovering) return;
            Errors.Add(error);
        }
    }
}
