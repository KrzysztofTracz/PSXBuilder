﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ApplicationFramework;

namespace PSXBuilder.Programs
{
    class CreateProjectProgram : Program<PSXBuilder>
    {
        public const String SourceDirectory       = "Source";
        public const String SolutionFileExtension = "sln";
        public const String ProjectFileExtension  = "vcproj";
        public const String FiltersExtension      = ProjectFileExtension + ".filters";
        public const String UserFileExtension     = ProjectFileExtension + ".user";

        [ProgramArgument]
        public String Target = null;

        [ProgramArgument(Optional=true)]
        public String Source = null;
        
        public override bool Start()
        {
            bool result = false;

            var sourceDirectory = Source;
            var targetDirectory = Target;

            var files = new List<String>();

            EnumerateFiles(sourceDirectory, "*.h", files);
            EnumerateFiles(sourceDirectory, "*.c", files);

            var projectName = Utils.GetFileName(sourceDirectory);

            var solutionDirectory    = Utils.Path(targetDirectory,   projectName);
            var projectDirectory     = Utils.Path(solutionDirectory, projectName);
            var sourceFilesDirectory = Utils.Path(projectDirectory,  SourceDirectory);



            //Utils.CreateDirectory(solutionDirectory);
            //Utils.CreateDirectory(Utils.Path(targetDirectory, projectName, ));


            return result;
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
