using AllanMilne.Ardkit;
using System;

namespace Assessment
{
    public class PALType : LanguageType
    {
        private PALType() { }

        public static new double ToReal(string input)
            => Convert.ToSingle(input);

        public static new int ToInteger(string input)
            => Convert.ToInt32(input);

        public static new bool ToBoolean(string input)
            => Convert.ToBoolean(input);

        public static new string ToString(int type)
            => type switch
            {
                Undefined => "Undefined",
                Integer => "Integer",
                Real => "Real",
                Boolean => "Boolean",
                String => "String",
                _ => "Undefined",
            };
    }
}
