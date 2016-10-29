using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using System.Xml;

namespace PSXBuilder
{
    public class BuildInfo
    {
        public const String FileName = "PSXBuilder.lastbuild";

        public const String XMLRootElement   = "LastBuild";
        public const String XMLFileElement   = "File";
        public const String XMLTimeAttribute = "time"; 

        public const String XMLIndentChars = "\t";

        public List<String> Files { get; set; }
        public DateTime     Time  { get; set; }

        public void Load(String directory)
        {
            Files = new List<String>();

            var path = Utils.Path(directory, FileName);

            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create(path);
                if (xmlReader != null)
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xmlReader.Name == XMLRootElement)
                                {
                                    Time = DateTime.Parse(xmlReader.GetAttribute(XMLTimeAttribute));
                                }
                                else if (xmlReader.Name == XMLFileElement)
                                {
                                    Files.Add(xmlReader.ReadString());
                                }
                                break;
                        }
                    }
                    xmlReader.Close();
                }
            }
            catch (System.IO.IOException) { }
#if !DEBUG
            catch(Exception e)
            {
                logger.Log(e.Message);
            } 
#endif
        }

        public void Save(String directory)
        {
            var path = Utils.Path(directory, FileName);
#if !DEBUG
            try
            {
#endif
            var xmlWriterSettings = new XmlWriterSettings();

            xmlWriterSettings.Indent      = true;
            xmlWriterSettings.IndentChars = XMLIndentChars;

            var xmlWriter = XmlWriter.Create(path, xmlWriterSettings);
            if (xmlWriter != null)
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement(XMLRootElement);
                xmlWriter.WriteAttributeString(XMLTimeAttribute, Time.ToString());
                foreach(var file in Files)
                {
                    xmlWriter.WriteElementString(XMLFileElement, file);
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
#if !DEBUG
            }
            catch (Exception e)
            {
                logger.Log(e.Message);
            }   
#endif
        }
    }
}
