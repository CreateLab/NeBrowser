using System;
using System.Collections.Generic;

namespace NeBrowser.Models
{
    public class Memento
    {
        public string Url { get; set; }
        public IEnumerable<Pair> Headers { get; set; }
    }

    public class Pair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}