using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NetBrowser_UWP.Helpers;

public static class XmlSerializer<T>
{
    // Serialize to xml  
    public static string ToXml(T value)
    {
        var serializer = new XmlSerializer(typeof(T));
        var stringBuilder = new StringBuilder();
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
        {
            serializer.Serialize(xmlWriter, value, ns);
        }

        return stringBuilder.ToString();
    }

    public static XmlNode ToXmlNode(string xmlInputString)
    {
        if (string.IsNullOrEmpty(xmlInputString.Trim())) throw new ArgumentNullException(nameof(xmlInputString));
        var xd = new XmlDocument();
        using var sr = new StringReader(xmlInputString);
        xd.Load(sr);
        return xd;
    }

    // Deserialize from xml  
    public static T FromXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(T));
        T value;
        using (var stringReader = new StringReader(xml))
        {
            var deserialized = serializer.Deserialize(stringReader);
            value = (T) deserialized;
        }

        return value;
    }
}