using System;
using System.Collections;
using System.Collections.Generic;

namespace VPackager
{
    public class Language
    {
        public Language()
        {
            Words = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public int Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public bool IsRightToLeft { get; set; }

        public string FlagName { get; set; }

        public Dictionary<string, string> Words { get; private set; }

        public string this[string name]
        {
            get
            {
                if (Words.ContainsKey(name))
                    return Words[name];
                else
                    return name;
            }
        }

        public virtual void Merge(Language lang)
        {
            if (lang == null)
                throw new ArgumentNullException();

            foreach (var de in lang.Words)
            {
                Words[de.Key] = de.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Id, Name);
        }
    }
}
