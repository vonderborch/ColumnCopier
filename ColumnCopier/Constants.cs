using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnCopier
{
    public class Constants
    {
        private static Constants instance = new Constants();
        private Dictionary<char, string> characterReplacements;
        private Dictionary<string, string> stringReplacements;

        private Constants()
        {
            var replacements = new Dictionary<char, string>()
            {
                {'!', "&#33;" },
                {'"', "&#34;" },
                {'#', "&#35;" },
                {'$', "&#36;" },
                {'%', "&#37;" },
                {'&', "&#38;" },
                {'\'', "&#39;" },
                {'(', "&#40;" },
                {')', "&#41;" },
                {'*', "&#42;" },
                {'+', "&#43;" },
                {',', "&#44;" },
                {'-', "&#45;" },
                {'.', "&#46;" },
                {'/', "&#47;" },
                {':', "&#58;" },
                {';', "&#59;" },
                {'<', "&#60;" },
                {'=', "&#61;" },
                {'>', "&#62;" },
                {'?', "&#63;" },
                {'@', "&#64;" },
                {'[', "&#91;" },
                {'\\', "&#92;" },
                {']', "&#93;" },
                {'^', "&#94;" },
                {'_', "&#95;" },
                {'`', "&#96;" },
                {'{', "&#123;" },
                {'|', "&#124;" },
                {'}', "&#125;" },
                {'~', "&#126;" },
            };

            characterReplacements = new Dictionary<char, string>();
            stringReplacements = new Dictionary<string, string>();
            StringReplacementPattern = string.Empty;
            var pattern = new StringBuilder();
            pattern.Append("\b(");
            var i = 0;
            var max = replacements.Count - 1;
            foreach (var pair in replacements)
            {
                characterReplacements.Add(pair.Key, pair.Value);
                stringReplacements.Add(pair.Value, pair.Key.ToString());

                pattern.Append(string.Format("{0}{1}", pair.Value, i == max ? "" : "|"));
                i++;
            }
            pattern.Append(")\b");
            StringReplacementPattern = pattern.ToString();
        }

        public static Constants Instance
        {
            get { return instance; }
        }

        public Dictionary<char, string> CharacterReplacements
        {
            get { return characterReplacements; }
        }

        public Dictionary<string, string> StringReplacements
        {
            get { return stringReplacements; }
        }

        public string StringReplacementPattern { get; private set; }
    }
}
