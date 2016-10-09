using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class BuildEnvironment
    {
        public class DirectoryStructure
        {
            public const String SourceDirectoryName       = "src";
            public const String IntermediateDirectoryName = "obj";
            public const String OutputDirectoryName       = "bin";

            public String Source       { get; protected set; }
            public String Intermediate { get; protected set; }
            public String Output       { get; protected set; }

            public DirectoryStructure(String root)
            {
                Source       = GetDirectory(root, SourceDirectoryName);
                Intermediate = GetDirectory(root, IntermediateDirectoryName);
                Output       = GetDirectory(root, OutputDirectoryName);
            }

            protected String GetDirectory(String root, String DirectoryName)
            {
                return String.Format("{0}\\{1}", root, DirectoryName);
            }
        }

        public DirectoryStructure Remote { get; protected set; }

        public BuildEnvironment()
        {
            Remote = null;
        }

        public void Initialize()
        {
            var remoteDirectoryRoot = PSTools.CMD("echo %PSYQ_PROJECTS%");
            Remote = new DirectoryStructure(remoteDirectoryRoot);
        }

        public bool CopyFilesToRemote(List<String> files)
        {
            bool result = false;

            List<List<String>> directories = new List<List<String>>();
            foreach(var file in files)
            {
                var path = new List<String>(file.Split('\\'));
                if(path.Count > 1)
                {
                    directories.Add(path.GetRange(0, path.Count - 1));
                }
            }



            return result;
        }
    }
}
