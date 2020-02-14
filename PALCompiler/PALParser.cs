using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PALCompiler
{
    public class PALParser : RecoveringRdParser
    {
        public PALParser(IScanner scan) : base(scan)
        {
        }

        protected override void recStarter()
        {
            mustBe("PROGRAM");
            recIdentifier();

            mustBe("WITH");
            recVarDecls();

            mustBe("IN");
            recStatements();

            mustBe("END");
        }

        private void recVarDecls()
        {
            recIdentList();
            mustBe("AS");
            recType();
        }

        private void recIdentList()
        {
            while (have(Token.IdentifierToken))
            {
                recIdentifier();
            }
        }

        private void recIdentifier()
        {
            mustBe(Token.IdentifierToken);
        }

        private void recType()
        {
            if (have("REAL"))
                mustBe("REAL");
            else
                mustBe("INTEGER");
        }

        private void recStatements()
        {
            // assignment
            if (have(Token.IdentifierToken))
                recAssignment();

            // loop
            else if (have("UNTIL"))
                recLoop();

            // conditional
            else if (have("IF"))
                recConditional();

            // io
            else if (have("INPUT") || have("OUTPUT"))
                recIo();
        }

        private void recAssignment()
        {
            recIdentifier();

            mustBe("=");

            recExpression();
        }

        private void recLoop()
        {
            mustBe("UNTIL");
            recBooleanExpr();
            mustBe("REPEAT");
            recStatements();
            mustBe("ENDLOOP");
        }

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
                recExpression();
            }
        }

        private void recBooleanExpr()
        {
            recExpression();

            if (have("<"))
                mustBe("<");
            else if (have("="))
                mustBe("=");
            else
                mustBe(">");

            recExpression();
        }

        private void recExpression()
        {
            recTerm();

            if (have("+"))
                mustBe("+");
            else
                mustBe("-");

            recTerm();
        }

        private void recTerm()
        {
            recFactor();

            if (have("*"))
                mustBe("*");
            else
                mustBe("/");

            recFactor();
        }

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
            else
            {
                recExpression();
            }
        }

        private void recValue()
        {
            if (have(Token.IdentifierToken))
                mustBe(Token.IdentifierToken);
            else if (have(Token.IntegerToken))
                mustBe(Token.IntegerToken);
            else
                mustBe(Token.RealToken);
        }

        private bool haveValue()
        {
            return have(Token.IdentifierToken) || have(Token.IntegerToken) || have(Token.RealToken);
        }
    }
}
