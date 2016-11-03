using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using System.Xml;

namespace PSXBuildService
{
    public class CompilationInfo
    {
        public const String FileExtension = "compile";

        public const String XMLRootElement = "Compilation";
        public const String XMLFileElement = "File";

        public const String XMLIndentChars = "\t";

        public List<String> Files         { get; protected set; }
        public List<String> CompiledFiles { get; protected set; }

        public ILogger Logger { get; protected set; }

        public String FileName { get; protected set; }

        public CompilationInfo()
        {
            Files         = new List<String>();
            CompiledFiles = new List<String>();
            Logger        = null;
            FileName      = "";
        }

        public void Initialize(String projectName,
                               String projectDirectory,
                               ILogger logger)
        {
            FileName = Utils.Path(projectDirectory, Utils.FileName(projectName, FileExtension));
            Logger = logger;
        }

        public void MarkAsCompiled(String file)
        {
            CompiledFiles.Add(file);
        }

        public void Add(String file)
        {
            if (!Files.Contains(file))
            {
                Files.Add(file);
                Flush();
            }
        }

        public void Remove(String file)
        {
            if(Files.Contains(file))
            {
                Files.Remove(file);
                Flush();
            }
        }

        public void FlushCompiledFiles()
        {
            foreach(var file in CompiledFiles)
            {
                if(Files.Contains(file))
                {
                    Files.Remove(file);
                }
            }
            CompiledFiles.Clear();
            Flush();
        }

        public void Load()
        {
            try
            {
                XmlReader xmlReader = XmlReader.Create(FileName);
                if (xmlReader != null)
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xmlReader.Name == XMLFileElement)
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
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }
#endif
        }

        public void Flush()
        {
            Save();
        }

        public void Save()
        {
#if !DEBUG
            try
            {
#endif
                var xmlWriterSettings = new XmlWriterSettings();

                xmlWriterSettings.Indent = true;
                xmlWriterSettings.IndentChars = XMLIndentChars;

                var xmlWriter = XmlWriter.Create(FileName, xmlWriterSettings);
                if (xmlWriter != null)
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement(XMLRootElement);
                    foreach (var file in Files)
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
                Logger.Log(e.Message);
            }
#endif
        }
    }
}
