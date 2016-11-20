using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ApplicationFramework;

namespace PSXBuilder.Programs
{
    class CreateProjectProgram : Program<PSXBuilder>
    {
        public const String SourceDirectoryName   = "Source";
        
        
        [ProgramArgument]
        public String TargetDirectory = null;

        [ProgramArgument(Optional=true)]
        public String SourceDirectory = null;

        [ProgramArgument(Optional = true)]
        public String ProjectName = null;

        [ProgramArgument(Optional = true)]
        public String WindowsTargetPlatformVersion = "8.1";

        public override bool Start()
        {
            bool result = false;

            var includeFiles = new List<String>();
            var compileFiles = new List<String>();

            if (!String.IsNullOrEmpty(SourceDirectory))
            {
                EnumerateFiles(SourceDirectory, "*.h", includeFiles);
                EnumerateFiles(SourceDirectory, "*.c", compileFiles);
            }

            if (String.IsNullOrEmpty(ProjectName))
            {
                ProjectName = Utils.GetFileName(SourceDirectory);
            }

            var solutionDirectory    = Utils.Path(TargetDirectory,   ProjectName);
            var projectDirectory     = Utils.Path(solutionDirectory, ProjectName);
            var sourceFilesDirectory = Utils.Path(projectDirectory,  SourceDirectoryName);

            Utils.CreateDirectory(solutionDirectory);
            Utils.CreateDirectory(projectDirectory);
            Utils.CreateDirectory(sourceFilesDirectory);

            CopyFiles(includeFiles, sourceFilesDirectory, projectDirectory);
            CopyFiles(compileFiles, sourceFilesDirectory, projectDirectory);

            var projectGenerator = new PSXProjectGenerator(Application);
            projectGenerator.ProjectName                  = ProjectName;
            projectGenerator.ProjectDirectory             = projectDirectory;
            projectGenerator.SolutionDirectory            = solutionDirectory;
            projectGenerator.WindowsTargetPlatformVersion = WindowsTargetPlatformVersion;
            projectGenerator.ClIncludeFiles               = includeFiles;
            projectGenerator.ClCompileFiles               = compileFiles;

            projectGenerator.CreateProject();
            projectGenerator.CreateUserFile();
            projectGenerator.CreateSolution();

            return result;
        }

        protected void CopyFiles(List<String> files, String sourceFilesDirectory, String projectDirectory)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var targetPath = Utils.Path(sourceFilesDirectory, Utils.ConvertPathToLocal(files[i], SourceDirectory));
                File.WriteAllBytes(targetPath, File.ReadAllBytes(files[i]));
                files[i] = Utils.ConvertPathToLocal(targetPath, projectDirectory);
            }
        }

        protected void EnumerateFiles(String directory, String pattern, List<String> files)
        {
            var results = Directory.EnumerateFiles(directory,
                                                   pattern,
                                                   SearchOption.AllDirectories);
            
            foreach (var file in results)
            {
                files.Add(file);
            }
        }

        protected override String GetDescription()
        {
            return "Create new project";
        }

        protected override String GetSpecifier()
        {
            return "n";
        }
    }
}
