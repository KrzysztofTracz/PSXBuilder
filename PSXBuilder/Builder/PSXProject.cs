using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;
using ApplicationFramework;
using System.Collections;

namespace PSXBuilder
{
    public class PSXProject
    {
        public enum FileType
        {
            Unknown,
            Header,
            Source
        }

        public class File
        {
            public String   Path         { get; protected set; }
            public String   LocalPath    { get; protected set; }

            public String   Name         { get; protected set; }
            public String   Extension    { get; protected set; }
            public String   FullName     { get; protected set; }

            public FileType     Type         { get; protected set; }
            public DateTime     LastModified { get; protected set; }
            public List<String> Dependencies { get; protected set; }

            public const String Include = "#include";

            public File(String root, String localPath, FileType type)
            {
                Path      = Utils.Path(root, localPath);
                LocalPath = localPath;

                Name      = Utils.GetFileNameExcludingExtension(Path);
                Extension = Utils.GetFileExtension(Path);
                FullName  = Utils.GetFileName(Path);

                Type         = type;
                LastModified = new System.IO.FileInfo(Path).LastWriteTime;
                Dependencies = ReadDependencies(Path);
            }

            protected List<String> ReadDependencies(String path)
            {
                var result = new List<String>();
                var text   = Utils.TrimComments(System.IO.File.ReadAllText(path));

                int searchStartIndex = 0;
                int index            = -1;

                while ((index = text.IndexOf(Include, searchStartIndex)) != -1)
                {
                    var lineStart = index + Include.Length;
                    var lineEnd   = text.IndexOf('\n', index);

                    if (lineEnd  == -1) break;

                    searchStartIndex = lineEnd;

                    var line = text.Substring(lineStart, lineEnd - lineStart);

                    var quoteStart = line.IndexOf('"');
                    if(quoteStart != -1 && quoteStart < line.Length - 1)
                    {
                        var quoteEnd = line.IndexOf('"', quoteStart + 1);
                        if(quoteEnd != -1)
                        {
                            result.Add(line.Substring(quoteStart + 1, quoteEnd - (quoteStart + 1)));
                        }
                    }
                }

                return result;
            }

            public static String GetClType(FileType type)
            {
                String result = "";

                switch (type)
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
        }

        public class FileDictionary : IEnumerable<File>
        {
            public FileDictionary()
            {
                files = new Dictionary<FileType, List<File>>();
            }

            public IEnumerator<File> GetEnumerator()
            {
                return new FileEnumerator(this);
            }

            public void Add(FileType fileType, File file)
            {
                if(!files.ContainsKey(fileType))
                {
                    files.Add(fileType, new List<File>());
                }

                files[fileType].Add(file);
            }

            public List<File> this[FileType fileType]
            {
                get
                {
                    return files.ContainsKey(fileType) ? files[fileType] : null;
                }
            }

            protected Dictionary<FileType, List<File>> files;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }
        };

        public class FileEnumerator : IEnumerator<PSXProject.File>
        {
            private FileDictionary files;
            private FileType       fileType;
            private int            index;

            public FileEnumerator(FileDictionary enumerable)
            {
                files = enumerable;
                Reset();
            }

            public File Current
            {
                get
                {
                    return files[fileType][index];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                files = null;
            }

            public bool MoveNext()
            {
                bool result = false;

                var fileTypes = Enum.GetValues(typeof(FileType)).Cast<FileType>().ToList();
                var typeIndex = fileTypes.IndexOf(fileType);

                bool incrementIndex = true;
                while(typeIndex < fileTypes.Count)
                {
                    fileType = fileTypes[typeIndex];

                    if (files[fileType] != null)                        
                    {
                        if (incrementIndex)
                        {
                            index++;
                        }

                        if (index < files[fileType].Count)
                        {
                            result = true;
                            break;
                        }
                    }

                    typeIndex++;
                    incrementIndex = false;
                    index = 0;
                }               

                return result;
            }

            public void Reset()
            {
                fileType = Enum.GetValues(typeof(FileType)).Cast<FileType>().First();
                index    = 0;
            }
        }

        public String Name       { get; protected set; }
        public String FileName   { get; protected set; }
        public String Directory  { get; protected set; }

        public String IntermediateDir { get; protected set; }

        public String OutputDir      { get; protected set; }
        public String OutputFile     { get; protected set; }
        public String OutputFileName { get; protected set; }

        public FileDictionary Files { get; protected set; }

        public String Configuration     { get; protected set; }
        public String VideoFormat       { get; protected set; }
        public int    OptimisationLevel { get; protected set; }

        public List<String> PreprocessorDefinitions { get; protected set; }

        public PSXProject()
        {
            Name     = "";
            FileName = "";

            OutputDir      = "";
            OutputFile     = "";
            OutputFileName = "";

            Files = null;
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

                Name     = Utils.GetFileNameExcludingExtension(fileName);
                FileName = fileName;

                Directory       = project.GetPropertyValue("MSBuildProjectDirectory");

                OutputFile      = project.GetPropertyValue("TargetPath");
                OutputFileName  = project.GetPropertyValue("TargetFileName");

                OutputDir       = Utils.CorrectDirectoryPath(project.GetPropertyValue("TargetDir"));
                IntermediateDir = Utils.CorrectDirectoryPath(project.GetPropertyValue("IntDir"));

                Files = LoadFiles(project);
                ProjectCollection.GlobalProjectCollection.UnloadProject(project);

                Configuration = configuration;

                PreprocessorDefinitions = new List<String>(project.GetPropertyValue("NMakePreprocessorDefinitions").Replace(" ", "").Split(';'));

                if (PreprocessorDefinitions.Contains("_NTSC"))
                {
                    VideoFormat = "NTSC";
                }
                else if (PreprocessorDefinitions.Contains("_PAL"))
                {
                    VideoFormat = "PAL";
                }

                OptimisationLevel = PreprocessorDefinitions.Contains("_DEBUG") ? 0 : 2;
            }           

            return result;
        }

        public bool Contains(String localPath)
        {
            return GetFile(localPath) != null;
        }

        public File GetFile(String localPath)
        {
            File result = null;
            foreach (var file in Files)
            {
                if (file.LocalPath == localPath)
                {
                    result = file;
                    break;
                }
            }
            return result;
        }

        protected FileDictionary LoadFiles(Project project)
        {
            FileDictionary result = new FileDictionary();

            foreach(var fileType in Enum.GetValues(typeof(FileType)).Cast<FileType>())
            {
                var itemType = File.GetClType(fileType);
                if (!String.IsNullOrEmpty(itemType))
                {
                    var items = project.GetItems(itemType);
                    foreach (var item in items)
                    {
                        result.Add(fileType, new File(project.DirectoryPath,
                                                      item.EvaluatedInclude,
                                                      fileType));
                    }
                }
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
