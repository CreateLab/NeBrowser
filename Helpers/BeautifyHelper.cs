using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace NeBrowser.Helpers
{
    public class BeautifyHelper
    {
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
    }
}