using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;

namespace PSXBuilder
{
    class PSXProject
    {
        public enum FileType
        {
            Header,
            Source
        }

        public String File       { get; protected set; }
        public String Name       { get; protected set; }
        public String ShortName  { get; protected set; }

        public String OutputDir      { get; protected set; }
        public String OutputFile     { get; protected set; }
        public String OutputFileName { get; protected set; }

        public List<String> HeaderFiles { get; protected set; }
        public List<String> SourceFiles { get; protected set; }

        public PSXProject()
        {
            Name       = "";
            ShortName  = "";
            File       = "";

            OutputDir      = "";
            OutputFile     = "";
            OutputFileName = "";

            SourceFiles = null;
        }

        public bool Load(String fileName, String configuration, String toolsVersion)
        {
            bool result = false;

            var project = new Project(fileName, 
                                      GetProjectGlobalProperties(configuration),
                                      toolsVersion);
            if(project != null)
            {
                result = true;

                File        = fileName;
                Name        = GetName(File);
                ShortName   = GetShortName(Name);

                OutputDir      = GetOutputDir(project);
                OutputFile     = GetOutputFile(project);
                OutputFileName = GetOutputFileName(project);

                HeaderFiles = GetFiles(project, FileType.Header);
                SourceFiles = GetFiles(project, FileType.Source);
            }

            return result;
        }

        protected String GetName(String fileName)
        {
            return fileName.Split('\\').Last().Split('.').First();
        }

        protected String GetShortName(String name)
        {
            String result = name;

            if(result.Length > 8)
            {
                result  = result.Substring(0, 7);
                result += '~';
            }

            return result;
        }

        protected String GetOutputDir(Project project)
        {
            return project.GetPropertyValue("TargetDir");
        }

        protected String GetOutputFile(Project project)
        {
            return project.GetPropertyValue("TargetPath");
        }

        protected String GetOutputFileName(Project project)
        {
            return project.GetPropertyValue("TargetFileName");
        }

        protected List<String> GetFiles(Project project, FileType fileType)
        {
            List<String> result = new List<String>();

            var items = project.GetItems(GetItemType(fileType));
            foreach(var item in items)
            {
                result.Add(item.EvaluatedInclude);
            }

            return result;
        }

        protected String GetItemType(FileType fileType)
        {
            String result = "";

            switch(fileType)
            {
                case FileType.Header:
                    result = "ClInclude";
                    break;
                case FileType.Source:
                    result = "ClCompile";
                    break;
            }

            return result;
        }

        protected IDictionary<String, String> GetProjectGlobalProperties(String configuration)
        {
            var result = new Dictionary<String, String>();

            result.Add("Configuration", configuration);

            return result;
        }
    }
}
