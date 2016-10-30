using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ApplicationFramework
{
    public class DOSNamesConverter
    {
        public const String FileName              = "DOSNames.xml";
        public const String XMLRootElement        = "DOSNamesConverter";
        public const String XMLNameElement        = "name";
        public const String XMLShortNameAttribute = "short";
        public const String XMLLongNameAttribute  = "long";

        public const int MaxNameLength = 8;        

        public ILogger Logger { get; protected set; }

        public void Initialize(ILogger logger)
        {
            Logger = logger;
        }

        public String GetShortName(String fileName)
        {
            var extension = Utils.GetFileExtension(fileName);
            var name      = Utils.GetFileNameExcludingExtension(fileName);
            var result    = name;

            if(name.Length > MaxNameLength)
            {
                if (longToShort.ContainsKey(name))
                {
                    result = longToShort[name];
                }
                else
                {
                    var shortNameSize = MaxNameLength - 2;
                    var keys          = shortToLong.Keys;

                    while (shortNameSize > 0)
                    {
                        var shortName = name.Substring(0, shortNameSize) + "~";
                        var occupiedNames = new List<String>();

                        foreach (var key in keys)
                        {
                            if (key.StartsWith(shortName))
                            {
                                occupiedNames.Add(key);
                            }
                        }

                        int highestIndex = 0;
                        int digits       = MaxNameLength - (shortNameSize + 1);

                        foreach (var occupiedName in occupiedNames)
                        {
                            int value = int.Parse(occupiedName.Substring(shortNameSize + 1, digits));
                            if (value > highestIndex)
                            {
                                highestIndex = value;
                            }
                        }

                        if (highestIndex < Utils.GetMaximumValue(digits))
                        {
                            shortName += highestIndex.ToString();

                            result = shortName;
                            shortToLong.Add(shortName, name);
                            longToShort.Add(name, shortName);
                            Flush();
                            break;
                        }

                        shortNameSize--;
                    }
                }
            }

            return Utils.FileName(result,extension);
        }

        public String GetLongName(String fileName)
        {           
            var extension = Utils.GetFileExtension(fileName);
            var name      = Utils.GetFileNameExcludingExtension(fileName);
            var result    = name;

            if (shortToLong.ContainsKey(name))
            {
                result = Utils.FileName(shortToLong[name], extension);
            }

            return Utils.FileName(result, extension);
        }

        public String GetShortPath(String longPath)
        {
            var result    = "";
            var extension = Utils.GetFileExtension(longPath);
            var path      = longPath;

            if(!String.IsNullOrEmpty(extension))
            {
                path = path.Substring(0, path.Length - (extension.Length + 1));
            }

            var split = path.Split(Utils.DirectorySeparator);

            var convertedNames = new String[split.Length];
            for(int i=0; i<split.Length; i++)
            {
                convertedNames[i] = GetShortName(split[i]);
            }
            result = Utils.Path(convertedNames);
            result = Utils.FileName(result, extension);

            return result; 
        }

        public String GetLongPath(String shortPath)
        {
            var result = "";
            var extension = Utils.GetFileExtension(shortPath);
            var path      = shortPath;

            if (!String.IsNullOrEmpty(extension))
            {
                path = path.Substring(0, path.Length - (extension.Length + 1));
            }

            var split = path.Split(Utils.DirectorySeparator);

            var convertedNames = new String[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                convertedNames[i] = GetLongName(split[i]);
            }
            result = Utils.Path(convertedNames);
            result = Utils.FileName(result, extension);

            return result;
        }

        public void Load()
        {
            try
            {
                XmlReader xmlReader = XmlReader.Create(GetDataFilePath());
                if (xmlReader != null)
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xmlReader.Name == XMLNameElement)
                                {
                                    var shortName = xmlReader.GetAttribute(XMLShortNameAttribute);
                                    var longName  = xmlReader.GetAttribute(XMLLongNameAttribute);
                                    longToShort.Add(longName, shortName);
                                    shortToLong.Add(shortName, longName);
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
                Logger.Log(e.Message);
            } 
#endif
        }

        public void Flush()
        {
#if !DEBUG
            try
            {
#endif
            var xmlWriterSettings = new XmlWriterSettings();

            xmlWriterSettings.Indent      = true;
            xmlWriterSettings.IndentChars = "\t";

            var xmlWriter = XmlWriter.Create(GetDataFilePath(), xmlWriterSettings);
            if (xmlWriter != null)
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement(XMLRootElement);
                foreach (var key in shortToLong.Keys)
                {
                    xmlWriter.WriteStartElement(XMLNameElement);
                    xmlWriter.WriteAttributeString(XMLShortNameAttribute, key);
                    xmlWriter.WriteAttributeString(XMLLongNameAttribute, shortToLong[key]);
                    xmlWriter.WriteEndElement();
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
                Logger.Log(e.Message);
            }   
#endif
        }

        protected String GetDataFilePath()
        {
            return Utils.Path(Utils.GetExecutionPath(),
                              FileName);
        }
       
        protected Dictionary<String, String> shortToLong = new Dictionary<String, String>();
        protected Dictionary<String, String> longToShort = new Dictionary<String, String>();
    }
}
