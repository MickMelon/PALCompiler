using System;
using System.IO;

namespace Assessment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Error: No filename arg specified.");
                return;
            }

            var fileName = args[0];
            StreamReader streamReader;
            try
            {
                streamReader = new StreamReader(fileName);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: Cannot open '{fileName}': {ex}");
                return;
            }

            var parser = new PALParser(new PALScanner());
            parser.Parse(streamReader);

            parser.Errors.ForEach(e => Console.WriteLine(e.ToString()));
        }
    }
}
