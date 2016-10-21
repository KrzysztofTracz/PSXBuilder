using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ApplicationFramework
{
    public class SettingsField
    {
        public String DefaultValue { get; protected set; }
        public String Value
        {
            get
            {
                return value == null ? DefaultValue : value;
            }

            set
            {
                this.value = value;
            }
        }

        public SettingsField(String defaultValue)
        {
            DefaultValue = defaultValue;
        }

        protected String value = null;
    }

    public class Settings
    {
        public const String SettingsFileName = "Settings.xml";
        public const String XMLRootElement   = "Settings";
        public const String XMLIndentChars   = "\t";

        public Dictionary<String, SettingsField> Fields { get; protected set; }

        public Settings()
        {
            Fields = null;
        }

        public virtual void Initialize(object settingsOwner, ILogger logger)
        {
            this.logger = logger;

            Fields = new Dictionary<String,SettingsField>();

            var ownerType = settingsOwner.GetType();
            var fields    = ownerType.GetFields();

            foreach(var field in fields)
            {
                if(field.FieldType.Equals(typeof(SettingsField)))
                {
                    Fields.Add(field.Name,
                               field.GetValue(settingsOwner) as SettingsField);
                }
            }
        }

        public virtual bool Load()
        {
            bool result = false;

            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(SettingsFileName);
                if (xmlReader != null)
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if(xmlReader.Name != XMLRootElement)
                                {
                                    SetValue(xmlReader.Name, xmlReader.ReadString());
                                }                                
                                break;
                        }
                    }
                    xmlReader.Close();
                    result = true;
                }
            }
            catch(System.IO.IOException e)
            {
                logger.Log(e.Message);
            }
#if !DEBUG
            catch(Exception e)
            {
                logger.Log(e.Message);
            } 
#endif
            return result;
        }

        public virtual bool Save()
        {
            bool result = false;

            if(Fields.Count > 0)
            {
                XmlWriter xmlWriter = null;
#if !DEBUG
                try
                {
#endif
                    var xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent      = true;
                    xmlWriterSettings.IndentChars = XMLIndentChars;

                    xmlWriter = XmlWriter.Create(SettingsFileName, xmlWriterSettings);
                    if (xmlWriter != null)
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement(XMLRootElement);
                        foreach (var key in Fields.Keys)
                        {
                            xmlWriter.WriteElementString(key, Fields[key].Value);
                        }
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                        result = true;
                    }
#if !DEBUG
                }
                catch (Exception e)
                {
                    logger.Log(e.Message);
                }   
#endif
            }

            return result;
        }

        public bool SetValue(String name, String value)
        {
            bool result = false;
            if (Fields.ContainsKey(name))
            {
                Fields[name].Value = value;
            }
            return result;
        }

        protected ILogger logger = null;
    }
}
