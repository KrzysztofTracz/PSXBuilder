using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using ApplicationFramework;

namespace PSXBuilder
{
    class PSXProjectGenerator
    {
        public const String ProjectFileExtension  = "vcxproj";
        public const String SolutionFileExtension = "sln";
        public const String UserFileExtension     = ProjectFileExtension + ".user";

        public const String Platform = "Win32";
        public static readonly String[] Configurations = { "NTSC-Debug",
                                                           "NTSC-Release",
                                                           "PAL-Debug",
                                                           "PAL-Release" };

        public const String XMLNamespace  = "http://schemas.microsoft.com/developer/msbuild/2003";
        public const String XMLIndent     = "  ";
        public const String ItemGroup     = "ItemGroup";
        public const String PropertyGroup = "PropertyGroup";
        public const String Import        = "Import";
        public const String ImportGroup   = "ImportGroup";

        public String ProjectName                  { get; set; }
        public String ProjectDirectory             { get; set; }
        public String SolutionDirectory            { get; set; }
        public String WindowsTargetPlatformVersion { get; set; }

        public List<String> ClIncludeFiles { get; set; }
        public List<String> ClCompileFiles { get; set; }

        public String      ProjectGUID  { get; protected set; }
        public String      SolutionGUID { get; protected set; }
        public Application Application  { get; protected set; }

        public String ProjectFile  { get; protected set; }
        public String UserFile     { get; protected set; }
        public String SolutionFile { get; protected set; }

        public PSXProjectGenerator(Application application)
        {
            Application = application;
        }

        public void CreateProject()
        {
            ProjectGUID = Guid.NewGuid().ToString("B").ToUpper();
            ProjectFile = Utils.Path(ProjectDirectory, Utils.FileName(ProjectName, ProjectFileExtension));

            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent      = true;
            xmlWriterSettings.IndentChars = XMLIndent;

            var xmlWriter = XmlWriter.Create(ProjectFile, xmlWriterSettings);
            if (xmlWriter != null)
            {
                xmlWriter.WriteStartDocument();

                /********************************************************/
                // <Project>
                xmlWriter.WriteStartElement("Project", XMLNamespace);
                xmlWriter.WriteAttributeString("DefaultTargets", "Build");
                xmlWriter.WriteAttributeString("ToolsVersion", "14.0");
                xmlWriter.WriteAttributeString("xmlns", XMLNamespace);


                /********************************************************/
                // <ProjectConfigurations>
                xmlWriter.WriteStartElement(ItemGroup);
                xmlWriter.WriteAttributeString("Label", "ProjectConfigurations");

                foreach (var configuration in Configurations)
                {
                    /****************************************************/
                    // <ProjectConfiguration>
                    xmlWriter.WriteStartElement("ProjectConfiguration");
                    xmlWriter.WriteAttributeString("Include", String.Format("{0}|{1}", configuration, Platform));

                    xmlWriter.WriteElementString("Configuration", configuration);
                    xmlWriter.WriteElementString("Platform", Platform);

                    xmlWriter.WriteEndElement();
                    // </ProjectConfiguration>
                    /****************************************************/
                }

                xmlWriter.WriteEndElement();
                // </ProjectConfigurations>
                /********************************************************/


                /********************************************************/
                // <Globals>
                xmlWriter.WriteStartElement(PropertyGroup);
                xmlWriter.WriteAttributeString("Label", "Globals");

                xmlWriter.WriteElementString("ProjectGuid", ProjectGUID);
                xmlWriter.WriteElementString("Keyword", "MakeFileProj");
                xmlWriter.WriteElementString("WindowsTargetPlatformVersion", WindowsTargetPlatformVersion);
                xmlWriter.WriteElementString("ProjectName", ProjectName);

                xmlWriter.WriteEndElement();
                // </Globals>
                /********************************************************/


                /********************************************************/
                // <Microsoft.Cpp.Default.props>
                xmlWriter.WriteStartElement(Import);
                xmlWriter.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.Default.props");
                xmlWriter.WriteEndElement();
                // </Microsoft.Cpp.Default.props>
                /********************************************************/


                /********************************************************/
                // <ConfigurationsProperties>
                foreach (var configuration in Configurations)
                {
                    xmlWriter.WriteStartElement(PropertyGroup);
                    xmlWriter.WriteAttributeString("Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, Platform));
                    xmlWriter.WriteAttributeString("Label", "Configuration");

                    xmlWriter.WriteElementString("ConfigurationType", "Makefile");
                    xmlWriter.WriteElementString("UseDebugLibraries", "false");
                    xmlWriter.WriteElementString("PlatformToolset", "v140");

                    xmlWriter.WriteEndElement();
                }
                // <ConfigurationsProperties>
                /********************************************************/


                /********************************************************/
                // <Microsoft.Cpp.props>
                xmlWriter.WriteStartElement(Import);
                xmlWriter.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.props");
                xmlWriter.WriteEndElement();
                // </Microsoft.Cpp.props>
                /********************************************************/


                /********************************************************/
                // <ConfigurationsProperties>
                foreach (var configuration in Configurations)
                {
                    xmlWriter.WriteStartElement(PropertyGroup);
                    xmlWriter.WriteAttributeString("Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, Platform));

                    xmlWriter.WriteElementString("NMakeOutput", String.Format("$(Configuration)\\bin\\{0}.exe", ProjectName));
                    xmlWriter.WriteElementString("NMakePreprocessorDefinitions", GetPreprocessorDefinitions(configuration));
                    xmlWriter.WriteElementString("NMakeIncludeSearchPath", "$(PSX_INCLUDES)");

                    xmlWriter.WriteElementString("NMakeBuildCommandLine",   Utils.Text("%PSX_BUILDER% ", GetProgramArguments<BuildProgram>()));
                    xmlWriter.WriteElementString("NMakeReBuildCommandLine", Utils.Text("%PSX_BUILDER% ", GetProgramArguments<RebuildProgram>()));
                    xmlWriter.WriteElementString("NMakeCleanCommandLine",   Utils.Text("%PSX_BUILDER% ", GetProgramArguments<CleanProgram>()));

                    xmlWriter.WriteElementString("OutDir", "$(SolutionDir)$(Configuration)\\bin\\");
                    xmlWriter.WriteElementString("IntDir", "$(SolutionDir)$(Configuration)\\");

                    xmlWriter.WriteEndElement();
                }
                // <ConfigurationsProperties>
                /********************************************************/


                /********************************************************/
                // <ClCompile>
                xmlWriter.WriteStartElement(ItemGroup);
                foreach (var file in ClCompileFiles)
                {
                    xmlWriter.WriteStartElement("ClCompile");
                    xmlWriter.WriteAttributeString("Include", file);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                // </ClCompile>
                /********************************************************/


                /********************************************************/
                // <ClInclude>
                xmlWriter.WriteStartElement(ItemGroup);
                foreach (var file in ClIncludeFiles)
                {
                    xmlWriter.WriteStartElement("ClInclude");
                    xmlWriter.WriteAttributeString("Include", file);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                // </ClInclude>
                /********************************************************/


                /********************************************************/
                // <Microsoft.Cpp.targets>
                xmlWriter.WriteStartElement(Import);
                xmlWriter.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets");
                xmlWriter.WriteEndElement();
                // </Microsoft.Cpp.targets>
                /********************************************************/


                xmlWriter.WriteEndElement();
                // </Project>
                /********************************************************/

                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }

        public void CreateSolution()
        {
            SolutionGUID = Guid.NewGuid().ToString("B").ToUpper();
            SolutionFile = Utils.Path(SolutionDirectory, Utils.FileName(ProjectName, SolutionFileExtension));

            var buffer = new StringBuilder();
            buffer.AppendLine();
            buffer.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            buffer.AppendLine("# Visual Studio 14");
            buffer.AppendLine("VisualStudioVersion = 14.0.25420.1");
            buffer.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
            buffer.AppendLine(String.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                                            SolutionGUID,
                                            ProjectName,
                                            Utils.ConvertPathToLocal(ProjectFile, SolutionDirectory),
                                            ProjectGUID));
            buffer.AppendLine("EndProject");
            buffer.AppendLine("Global");
            buffer.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            foreach(var configuration in Configurations)
            {
                buffer.AppendLine(String.Format("\t\t{0}|PSX = {0}|PSX", configuration));
            }
            buffer.AppendLine("\tEndGlobalSection");
            buffer.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (var configuration in Configurations)
            {
                buffer.AppendLine(String.Format("\t{0}.{1}|PSX.ActiveCfg = {1}|{2}", ProjectGUID, configuration, Platform));
                buffer.AppendLine(String.Format("\t{0}.{1}|PSX.Build.0 = {1}|{2}", ProjectGUID, configuration, Platform));
            }
            buffer.AppendLine("\tEndGlobalSection"); 
            buffer.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
            buffer.AppendLine("\t\tHideSolutionNode = FALSE");
            buffer.AppendLine("\tEndGlobalSection");
            buffer.AppendLine("EndGlobal");

            File.WriteAllText(SolutionFile, buffer.ToString());
        }

        public void CreateUserFile()
        {
            UserFile = Utils.Path(ProjectDirectory, Utils.FileName(ProjectName, UserFileExtension));

            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = XMLIndent;

            var xmlWriter = XmlWriter.Create(UserFile, xmlWriterSettings);
            if (xmlWriter != null)
            {
                xmlWriter.WriteStartDocument();

                /********************************************************/
                // <Project>
                xmlWriter.WriteStartElement("Project", XMLNamespace);
                xmlWriter.WriteAttributeString("ToolsVersion", "14.0");
                xmlWriter.WriteAttributeString("xmlns", XMLNamespace);


                /********************************************************/
                // <ConfigurationsProperties>
                foreach (var configuration in Configurations)
                {
                    xmlWriter.WriteStartElement(PropertyGroup);
                    xmlWriter.WriteAttributeString("Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, Platform));

                    xmlWriter.WriteElementString("LocalDebuggerCommand", "$(PSX_BUILDER)");
                    xmlWriter.WriteElementString("LocalDebuggerCommandArguments", "-e \"$(TargetPath)\"");
                    xmlWriter.WriteElementString("DebuggerFlavor", "WindowsLocalDebugger");

                    xmlWriter.WriteEndElement();
                }
                // </ConfigurationsProperties>
                /********************************************************/


                /********************************************************/
                // <ShowAllFiles>
                xmlWriter.WriteStartElement(PropertyGroup);
                xmlWriter.WriteElementString("ShowAllFiles", "true");
                xmlWriter.WriteEndElement();
                // </ShowAllFiles>
                /********************************************************/


                xmlWriter.WriteEndElement();
                // </Project>
                /********************************************************/

                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }

        protected String GetPreprocessorDefinitions(String configuration)
        {
            var result = "";
            var definitions = new List<String>();

            if (configuration.Contains("NTSC"))
            {
                definitions.Add("_NTSC");
            }

            if (configuration.Contains("PAL"))
            {
                definitions.Add("_PAL");
            }

            if (configuration.Contains("Debug"))
            {
                definitions.Add("_DEBUG");
            }

            if (definitions.Count > 0)
            {
                result = Utils.ConcatArguments("; ", definitions.ToArray());
            }

            return result;
        }

        protected String GetProgramArguments<T>() where T : IProgram
        {
            var program = Application.Program<T>();
            var buffer  = new StringBuilder();

            buffer.Append(program.Specifier);
            buffer.Append(' ');

            var arguments = program.ArgumentsCount;

            for (int i=0;i< arguments; i++)
            {
                var argument = program.GetArgument(i);
                buffer.Append(argument.Name);
                if(!String.IsNullOrEmpty(argument.DefaultValue))
                {
                    buffer.Append('=');
                    buffer.Append(argument.DefaultValue);
                }
                
                if(i < arguments - 1)
                {
                    buffer.Append(' ');
                }                
            }

            return buffer.ToString();
        }
    }
}
