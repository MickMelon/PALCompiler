using AllanMilne.Ardkit;
using CompilersProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CrossReferenceIdentifiers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Check that the filename argument has been given
            if (args.Length != 1)
            {
                Console.WriteLine("ERROR: No filename specified.");
                return;
            }

            // Attempt to read the file
            var fileName = args[0];
            StreamReader streamReader;
            try
            {
                streamReader = new StreamReader(fileName);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"ERROR: Cannot open '{fileName}': {ex}");
                return;
            }

            // Create the scanner
            var scanner = new Block1Scanner();
            scanner.Init(streamReader, new List<ICompilerError>());

            // Count identifiers
            Console.WriteLine("Counting identifiers...");

            var identifiers = new List<Identifier>();
            while (!scanner.EndOfFile)
            {
                var token = scanner.NextToken();
                var existingIdentifier = identifiers.FirstOrDefault(i => i.Name.Equals(token.TokenType));

                if (existingIdentifier != null)
                {
                    existingIdentifier.LineNumbersUsed.Add(token.Line);
                }
                else
                {
                    identifiers.Add(new Identifier(token.TokenType, token.Line));
                }
            }

            Console.WriteLine("Finished counting identifiers!");
            foreach (var identifier in identifiers)
            {
                Console.WriteLine("---");
                Console.WriteLine($"Name: {identifier.Name}");
                Console.WriteLine($"Line Declared: {identifier.LineDeclared}");
                Console.WriteLine($"No Lines Appeared: {identifier.LineNumbersUsed.Count()}");
                var strBuilder = new StringBuilder();
                identifier.LineNumbersUsed.ForEach(l => strBuilder.Append($"{l}, "));
                Console.WriteLine(strBuilder.ToString());
                Console.WriteLine("---");
            }
        }
    }
}
