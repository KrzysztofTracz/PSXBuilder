using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public class Utils
    {
        public static String GetExecutionPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static String ConcatArguments(String separator, params String[] arguments)
        {
            String result = "";
            if (arguments != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < arguments.Length; i++)
                {
                    sb.Append(arguments[i]);
                    if (i < arguments.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
                result = sb.ToString();
            }
            return result;
        }

        public static FileStream CreateFile(String path)
        {
            var split = path.Split(DirectorySeparator);

            if (split.Length > 1)
            {
                var dir = new StringBuilder();
                for (int i = 0; i < split.Length - 1; i++)
                {
                    var d = split[i];
                    bool createDirectory = false;
                    if (d != "."  && 
                        d != ".." &&
                        !IsDrive(d))
                    {
                        createDirectory = true;
                    }
                    dir.Append(d);

                    if (createDirectory)
                    {
                        var dirStr = dir.ToString();
                        if (!Directory.Exists(dirStr))
                        {
                            Directory.CreateDirectory(dirStr);
                        }
                    }

                    dir.Append(DirectorySeparator);
                }
            }

            return File.Create(path);
        }

        public static String GetFileName(String path)
        {
            return path.Split(DirectorySeparator).Last();
        }

        public static String GetFileNameExcludingExtension(String path)
        {
            return GetFileName(path).Split('.').First();
        }

        public static String GetFileExtension(String path)
        {
            String result = "";
            var split = GetFileName(path).Split('.');
            if(split.Length > 1)
            {
                result = split.Last();
            }
            return result;
        }

        public static String FileName(String name, String extension)
        {
            String result = name;
            if (!String.IsNullOrEmpty(extension))
            {
                result = String.Format("{0}.{1}", name, extension);
            }
            return result;
        }

        public static int GetMaximumValue(int digits)
        {
            int result = 0;
            for(int i=0;i<digits;i++)
            {
                result += 9 * (int)Math.Pow(10, i);
            }
            return result;
        }

        public static String GetAcceptableFileName(String filename)
        {
            var result = filename;
            foreach(var unacceptableCharacter in UnacceptableFileNameCharacters)
            {
                result = result.Replace(unacceptableCharacter, '-');
            }
            return result;
        }

        public static String ConvertPathToLocal(String path, String root)
        {
            var result = path.Replace(root, "");

            if (result.StartsWith(DirectorySeparator.ToString()))
            {
                result = result.Substring(1, result.Length - 1);
            }

            return result;
        }

        public static bool IsDrive(String directory)
        {
            return directory.EndsWith(":");
        }

        public static String Path(params String[] elements)
        {
            return ConcatArguments(DirectorySeparator.ToString(), elements);
        }

        public static String CorrectDirectoryPath(String path)
        {
            var result = path;
            if(path.EndsWith(DirectorySeparator.ToString()))
            {
                result = path.Substring(0, path.Length - 1);
            }
            return result;
        }

        public const char DirectorySeparator = '\\';

        public static readonly char[] UnacceptableFileNameCharacters = { '/', '\\', ':', '|', '*', '?', '"', '<', '>' };
    }
}
