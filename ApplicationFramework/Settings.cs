using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ApplicationFramework
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SettingsFieldAttribute : Attribute
    {
        public String DefaultValue { get; private set; }

        public SettingsFieldAttribute(String defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }

    public class Settings
    {
        public const String SettingsFileNameSuffix = "Settings.xml";
        public const String XMLRootElement         = "Settings";
        public const String XMLIndentChars         = "\t";

        public virtual void Initialize(object settingsOwner, ILogger logger)
        {
            this.settingsOwner = settingsOwner;
            this.logger        = logger;

            fields = new Dictionary<String,SettingsField>();

            var ownerType   = settingsOwner.GetType();
            var ownerFields = ownerType.GetFields();

            foreach(var field in ownerFields)
            {
                if(field.FieldType.Equals(typeof(String)))
                {
                    var attributes = field.GetCustomAttributes(typeof(SettingsFieldAttribute), true);
                    if(attributes.Length > 0)
                    {
                        var settingsFieldAttribute = attributes[0] as SettingsFieldAttribute;
                        var settingsField = new SettingsField(field, settingsFieldAttribute.DefaultValue);

                        settingsField.SetValue(settingsField.DefaultValue, settingsOwner);

                        fields.Add(settingsField.Name, settingsField);
                    }                   
                }
            }
        }

        public String GetSettingsFileName()
        {
            return String.Format("{0}\\{1}{2}",
                                 Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                 settingsOwner.GetType().Name,
                                 SettingsFileNameSuffix);
        }

        public virtual bool Load()
        {
            bool result = false;

            XmlReader xmlReader = null;

            try
            {
                xmlReader = XmlReader.Create(GetSettingsFileName());
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

            if(Count > 0)
            {
                XmlWriter xmlWriter = null;
#if !DEBUG
                try
                {
#endif
                    var xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent      = true;
                    xmlWriterSettings.IndentChars = XMLIndentChars;

                    xmlWriter = XmlWriter.Create(GetSettingsFileName(), xmlWriterSettings);
                    if (xmlWriter != null)
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement(XMLRootElement);
                        for(int i=0;i<Count;i++)
                        {
                            var name = GetName(i);
                            xmlWriter.WriteElementString(name, GetValue(name));
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
            if (fields.ContainsKey(name))
            {
                fields[name].SetValue(value, settingsOwner);
            }
            return result;
        }

        public bool SetValue(int index, String value)
        {
            bool result = false;
            var name = GetName(index);
            if(!String.IsNullOrEmpty(name) && fields.ContainsKey(name))
            {
                fields[name].SetValue(value, settingsOwner);
            }
            return result;
        }

        public String GetValue(String name)
        {
            String result = null;
            if (fields.ContainsKey(name))
            {
                result = fields[name].GetValue(settingsOwner);
            }
            return result;
        }

        public String GetValue(int index)
        {
            String result = null;
            var name = GetName(index);
            if (!String.IsNullOrEmpty(name) && fields.ContainsKey(name))
            {
                result = fields[name].GetValue(settingsOwner);
            }
            return result;
        }

        public String GetName(int index)
        {
            String result = null;
            if(index < Count)
            {
                result = fields.Keys.ElementAt(index);
            }
            return result;
        }

        public String this[String name]
        {
            get
            {
                return GetValue(name);
            }

            set
            {
                SetValue(name, value);
            }
        }

        public String this[int index]
        {
            get
            {
                return GetValue(GetName(index));
            }

            set
            {
                SetValue(GetName(index), value);
            }
        }

        public int Count
        {
            get
            {
                return fields.Keys.Count;
            }
        }

        protected ILogger logger        = null;
        protected object  settingsOwner = null;

        protected Dictionary<String, SettingsField> fields = null;

        protected class SettingsField
        {
            public String    Name         { get; protected set; }
            public String    DefaultValue { get; protected set; }
            public FieldInfo FieldInfo    { get; protected set; }

            public SettingsField(FieldInfo fieldInfo, String defaultValue)
            {
                DefaultValue = defaultValue;
                FieldInfo    = fieldInfo;
                Name         = fieldInfo.Name;
            }

            public void SetValue(String value, object fieldOwner)
            {
                FieldInfo.SetValue(fieldOwner, value);
            }

            public String GetValue(object fieldOwner)
            {
                return FieldInfo.GetValue(fieldOwner) as String;
            }
        }
    }
}
