using AllanMilne.Ardkit;
using Assessment.Errors;

namespace Assessment
{
    public class PALParser : RecoveringRdParser
    {
        public PALParser(IScanner scan) : base(scan)
        {
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
            mustBe("PROGRAM");
            recIdentifier();

            mustBe("WITH");
            recVarDecls();

            mustBe("IN");
            if (!haveStatement())
                syntaxError(new MissingStatementsError(scanner.CurrentToken));
            else
                recStatements();

            mustBe("END");
        }

        /// <summary>
        /// 
        /// <VarDecls> ::= (<IdentList> AS <Type>)* ;
        /// 
        /// </summary>
        private void recVarDecls()
        {
            // Var declarations will continue until "IN" is found
            while (have(Token.IdentifierToken))
            {
                recIdentList();
                mustBe("AS");
                recType();
            }            
        }

        /// <summary>
        /// 
        /// <IdentList> ::= Identifier ( , Identifier)* ;
        /// 
        /// </summary>
        private void recIdentList()
        {
            // There must be at least one identifier
            recIdentifier();

            // If there is a comma, then there must be more identifiers specified
            while (have(","))
            {
                mustBe(",");
                recIdentifier();
            }
        }

        /// <summary>
        /// 
        /// Identifier
        /// 
        /// </summary>
        private void recIdentifier()
        {
            mustBe(Token.IdentifierToken);
        }

        /// <summary>
        /// 
        /// <Type> ::= REAL | INTEGER ;
        /// 
        /// </summary>
        private void recType()
        {
            if (have("REAL"))
                mustBe("REAL");
            else if (have("INTEGER"))
                mustBe("INTEGER");
            else
                syntaxError(new InvalidTypeError(scanner.CurrentToken));
        }

        /// <summary>
        /// Keeps reading statements until there is none left.
        /// </summary>
        private void recStatements()
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
                // Console.WriteLine($"Current token is {scanner.CurrentToken.ToString()}");
                // Parse assignment
                if (!haveStatement()) break;
                recStatement();
            }
        }

        /// <summary>
        /// Checks if we current token is a valid statement
        /// </summary>
        private bool haveStatement()
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
        private void recStatement()
        {
            // Console.WriteLine($"rec Token is {scanner.CurrentToken.ToString()}");
            if (have(Token.IdentifierToken))
            {
                recAssignment();
            }

            // Parse loop
            else if (have("UNTIL"))
            {
                recLoop();
            }

            // Parse conditional
            else if (have("IF"))
            {
                recConditional();
            }

            // Parse IO
            else if (have("INPUT") || have("OUTPUT"))
            {
                recIo();
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
        private void recAssignment()
        {
            recIdentifier();
            mustBe("=");
            recExpression();
        }

        /// <summary>
        /// 
        /// <Loop> ::= UNTIL <BooleanExpr> REPEAT (<Statement>)* ENDLOOP ;
        /// 
        /// </summary>
        private void recLoop()
        {
            mustBe("UNTIL");
            recBooleanExpr();
            mustBe("REPEAT");
            recStatements();
            mustBe("ENDLOOP");
        }

        /// <summary>
        /// 
        /// <Conditional> ::= IF <BooleanExpr> THEN (<Statement>)*
        ///                       ( ELSE (<Statement>)* )?
        ///                       ENDIF ;
        /// 
        /// </summary>
        private void recConditional()
        {
            mustBe("IF");
            recBooleanExpr();
            mustBe("THEN");
            recStatements();
            
            if (have("ELSE"))
            {
                mustBe("ELSE");
                recStatements();
            }

            mustBe("ENDIF");
        }

        /// <summary>
        /// 
        /// <I-o> ::= INPUT <IdentList> |
        ///           OUTPUT <Expression> ( , <Expression>)* ;
        /// 
        /// </summary>
        private void recIo()
        {
            if (have("INPUT"))
            {
                mustBe("INPUT");
                recIdentList();
            }
            else
            {
                mustBe("OUTPUT");

                // Must be at least one expression.
                recExpression();

                // If there is a comma, there must be at least
                // one more expression.
                while (have(","))
                {
                    mustBe(",");
                    recExpression();
                }
            }
        }

        /// <summary>
        /// 
        /// <BooleanExpr> ::= <Expression> ("<" | "=" | ">") <Expression> ;
        /// 
        /// </summary>
        private void recBooleanExpr()
        {
            recExpression();

            if (have("<"))
                mustBe("<");
            else if (have("="))
                mustBe("=");
            else if (have(">"))
                mustBe(">");
            else
                syntaxError(new InvalidBooleanExprError(scanner.CurrentToken));

            recExpression();
        }

        /// <summary>
        /// 
        /// <Expression> ::= <Term> ( (+|-) <Term>)* ;
        /// 
        /// </summary>
        private void recExpression()
        {
            recTerm();

            while (have("+") || have("-"))
            {
                if (have("+"))
                {
                    mustBe("+");
                    recTerm();
                }
                else
                {
                    mustBe("-");
                    recTerm();
                }
            }          
        }

        /// <summary>
        /// 
        /// <Term> ::= <Factor> ( (*|/) <Factor>)* ;
        /// 
        /// </summary>
        private void recTerm()
        {
            recFactor();

            while (have("*") || have("/"))
            {
                if (have("*"))
                {
                    mustBe("*");
                    recFactor();
                }
                else
                {
                    mustBe("/");
                    recFactor();
                }
            }            
        }

        /// <summary>
        /// 
        /// <Factor> ::= (+|-)? ( <Value> | "(" <Expression> ")" ) ;
        /// 
        /// </summary>
        private void recFactor()
        {
            if (have("+"))
                mustBe("+");
            else if (have("-"))
                mustBe("-");

            if (haveValue())
            {
                recValue();
            }
            else if (have("("))
            {
                mustBe("(");
                recExpression();
                mustBe(")");
            }
            else
                syntaxError(new InvalidFactorError(scanner.CurrentToken));
        }

        /// <summary>
        /// 
        /// <Value> ::= Identifier | IntegerValue | RealValue ;
        /// 
        /// </summary>
        private void recValue()
        {
            if (have(Token.IdentifierToken))
                mustBe(Token.IdentifierToken);
            else if (have(Token.IntegerToken))
                mustBe(Token.IntegerToken);
            else
                mustBe(Token.RealToken);
        }

        /// <summary>
        /// Checks if we have an identifier, integer, or real token.
        /// </summary>
        private bool haveValue()
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
