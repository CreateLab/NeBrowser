using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
//using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace NeBrowser.Helpers
{
    public static class BeautifyHelper
    {
        private static readonly char[] SpaceSymbols = {' ', '\t'};

        public static bool TryBeautifyJson(ref string s)
        {
        	var options = new JsonSerializerOptions()
        	{
        		WriteIndented = true,
        		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        	};
        
        	JsonElement jsonElement;
        	try
        	{
        		jsonElement = JsonSerializer.Deserialize<JsonElement>(s);
        	}
        	catch (JsonException)
        	{
        		return false;
        	}
        
        	s = JsonSerializer.Serialize(jsonElement, options);
        	return true;
        }

        public static bool TryBeautifyXml(ref string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                xml = doc.ToString();
                return true;
            }
            catch (XmlException)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return false;
            }
        }

        public static bool TryBeautifyText(ref string text)
        {
            if (text.Length < 2000) return true;
            var blockCount = text.Length / 500;
            for (var i = 0; i < blockCount; i++)
            {
                var pos = GetMaxValue(text, i);
                switch (pos)
                {
                    case 0:
                        continue;
                    case -1:
                        text = text.Insert(i*500-1, "\n");
                        continue;
                    default:
                        text = text.Remove(pos, 1).Insert(pos, "\n");
                        break;
                }
            }

            return true;
        }

        private static int GetMaxValue(in string text, in int i)
        {
            const int count = 500;
            var start = i * count;
            var enterPosition =
                text.IndexOf("\n", start, (start + count > text.Length) ? text.Length-count : count, StringComparison.Ordinal);
            if (enterPosition > 0) return 0;
            var startIndex = ((i + 1) * count > text.Length) ? text.Length : (i + 1) * count;
            var position = text.LastIndexOfAny(SpaceSymbols, startIndex, count);
            if (position > 0) return position;
            return -1;
        }
    }
}