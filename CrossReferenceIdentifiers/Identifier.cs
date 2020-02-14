using System.Collections.Generic;

namespace CrossReferenceIdentifiers
{
    public class Identifier
    {
        public string Name { get; private set; }
        public int LineDeclared { get; private set; }
        public List<int> LineNumbersUsed { get; private set; }

        public Identifier(string name, int lineDeclared)
        {
            Name = name;
            LineDeclared = lineDeclared;
            LineNumbersUsed = new List<int>();
        }
    }
}
