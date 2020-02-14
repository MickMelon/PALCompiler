using AllanMilne.Ardkit;
using System;
using System.Collections.Generic;
using System.IO;

namespace CompilersProject
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

            // Count lets
            Console.WriteLine($"Counting 'let' statements in '{fileName}'");

            var count = 0;
            while (!scanner.EndOfFile)
                if (scanner.NextToken().TokenType.Equals("let"))
                    count++;

            Console.WriteLine($"{count} 'let' statements found.");
        }
    }
}
