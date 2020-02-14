using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject
{
    public enum State
    {
        Start,
        Identifier,

        Integer,
        Real,
        UnsignedInteger,
        UnsignedReal,
        String,
        Char,

        Equals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
        Add,
        Subtract,
        Multiply,
        Divide,
        Comma,
        EndOfFile,
        InvalidChar
    }

    // Do comments, just search for slash, then slash again, then we know its a comment. Go until new line symbol
    // Or search for slash, then star, then keep going until star-slash is found again. End of token

    // Strings, search for " and keep going until " found again. Then we have the string
    // Chars, search for ', go one char, make sure 3rd char is ' again, or else its invalid

    // signed ints?
    // floats with the f?

    public class Block1Scanner : Scanner
    {
        private readonly List<string> _keywords = new List<string>()
        {
            "begin", "end", "let", "put", "for", "get", "to", "do", "real"
        };

        protected override IToken getNextToken()
        {
            var state = State.Start;
            IToken token = null;
            int startLine = 0, startCol = 0;
            var strBuilder = new StringBuilder();

            while (token == null)
            {
                Console.WriteLine($"Current char is {currentChar}");

                switch (state)
                {
                    case State.Start:
                        Console.WriteLine("In State.Start");
                        if (char.IsWhiteSpace(currentChar))
                        {
                            state = State.Start;
                            break;
                        }                            

                        startLine = line;
                        startCol = column;
                        strBuilder.Clear();

                        if (char.IsLetter(currentChar)) state = State.Identifier;

                        else if (char.IsDigit(currentChar)) state = State.Integer;

                        else if (currentChar == '=') state = State.Equals;

                        else if (currentChar == '>') state = State.GreaterThan;

                        else if (currentChar == '<') state = State.LessThan;

                        else if (currentChar == '+') state = State.Add;

                        else if (currentChar == '-') state = State.Subtract;

                        else if (currentChar == '*') state = State.Multiply;

                        else if (currentChar == '/') state = State.Divide;

                        else if (currentChar == ',') state = State.Comma;

                        else if (currentChar == '"') state = State.String;

                        else if (currentChar == eofChar) state = State.EndOfFile;

                        else state = State.InvalidChar;

                        break;

                    case State.Identifier:
                        Console.WriteLine("In State.Identifier");

                        if (char.IsLetterOrDigit(currentChar))
                        {
                            state = State.Identifier;
                            break;
                        }

                        var identifier = strBuilder.ToString();
                        if (_keywords.Contains(identifier))
                            token = new Token(identifier, startLine, startCol);
                        else
                            token = new Token(Token.IdentifierToken, identifier, startLine, startCol);
                        break;

                    case State.Integer:
                        Console.WriteLine("In State.Integer");
                        if (char.IsDigit(currentChar))
                        {
                            state = State.Integer;
                            break;
                        }

                        if (currentChar == '.')
                        {
                            state = State.Real;
                            break;
                        }

                        // If it's a letter next and not whitespace, it's an invalid int
                        if (char.IsLetter(currentChar))
                        {
                            token = new Token(Token.InvalidToken, strBuilder.ToString(), startLine, startCol);
                            break;
                        }

                        token = new Token(Token.IntegerToken, strBuilder.ToString(), startLine, startCol);
                        break;

                    case State.UnsignedInteger:
                        Console.WriteLine("In State.UnsignedInteger");
                        if (char.IsDigit(currentChar))
                        {
                            state = State.UnsignedInteger;
                            break;
                        }

                        if (currentChar == '.')
                        {
                            state = State.UnsignedReal;
                            break;
                        }

                        // If it's a letter next and not whitespace, it's an invalid int
                        if (char.IsLetter(currentChar))
                        {
                            token = new Token(Token.InvalidToken, strBuilder.ToString(), startLine, startCol);
                            break;
                        }

                        token = new Token("UnsignedInteger", strBuilder.ToString(), startLine, startCol);
                        break;

                    case State.Real:
                        if (char.IsDigit(currentChar))
                        {
                            state = State.Real;
                            break;
                        }

                        // If it's a letter next and not whitespace, it's an invalid real
                        if (char.IsLetter(currentChar))
                        {
                            token = new Token(Token.InvalidToken, strBuilder.ToString(), startLine, startCol);
                            break;
                        }

                        token = new Token(Token.RealToken, strBuilder.ToString(), startLine, startCol);
                        break;

                    case State.UnsignedReal:
                        if (char.IsDigit(currentChar))
                        {
                            state = State.UnsignedReal;
                            break;
                        }

                        // If it's a letter next and not whitespace, it's an invalid real
                        if (char.IsLetter(currentChar))
                        {
                            token = new Token(Token.InvalidToken, strBuilder.ToString(), startLine, startCol);
                            break;
                        }

                        token = new Token("UnsignedReal", strBuilder.ToString(), startLine, startCol);
                        break;

                    case State.String:
                        if (currentChar != '"')
                        {
                            state = State.String;
                            break;
                        }

                        token = new Token(Token.StringToken, strBuilder.ToString(), startLine, startCol);
                        break;

                    case State.Equals:
                        Console.WriteLine("In State.Equals");
                        token = new Token("=", startLine, startCol);
                        break;

                    case State.GreaterThan:
                        Console.WriteLine("In State.GreaterThan");
                        if (currentChar == '=')
                        {
                            state = State.GreaterThanOrEquals;
                            break;
                        }

                        token = new Token(">", startLine, startCol);
                        break;

                    case State.LessThan:
                        Console.WriteLine("In State.LessThan");
                        if (currentChar == '=')
                        {
                            state = State.LessThanOrEquals;
                            break;
                        }

                        token = new Token("=", startLine, startCol);
                        break;

                    case State.GreaterThanOrEquals:
                        Console.WriteLine("In State.GreaterThanOrEquals");
                        token = new Token(">=", startLine, startCol);
                        break;

                    case State.Add:
                        token = new Token("+", startLine, startCol);
                        break;

                    case State.Subtract:
                        if (char.IsDigit(currentChar))
                        {
                            state = State.UnsignedInteger;
                            break;
                        }

                        token = new Token("-", startLine, startCol);
                        break;

                    case State.Multiply:
                        token = new Token("*", startLine, startCol);
                        break;

                    case State.Divide:
                        token = new Token("/", startLine, startCol);
                        break;

                    case State.Comma:
                        Console.WriteLine("In State.Comma");
                        token = new Token(",", startLine, startCol);
                        break;

                    case State.EndOfFile:
                        Console.WriteLine("In State.EndOfFile");
                        token = new Token(Token.EndOfFile, startLine, startCol);
                        break;

                    case State.InvalidChar:
                        Console.WriteLine("In State.InvalidChar");
                        token = new Token(Token.InvalidChar, strBuilder.ToString(), startLine, startCol);
                        break;
                }

                // If the token hasn't been created, we are still reading the current
                // token
                if (token == null)
                {
                    // If it's not in the start state, append the current char
                    // to build the token name
                    if (state != State.Start)
                        strBuilder.Append(currentChar);

                    getNextChar();
                }
            }

            Console.WriteLine($"*** Token is [{token.ToString()}] ***");
            return token;
        }
    }
}
