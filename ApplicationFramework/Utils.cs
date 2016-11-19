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

        public static void CreateDirectory(String path)
        {
            var split = path.Split(DirectorySeparator);

            if (split.Length > 1)
            {
                var dir = new StringBuilder();
                for (int i = 0; i < split.Length; i++)
                {
                    var d = split[i];
                    bool createDirectory = false;
                    if (d != "." &&
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
        }

        public static FileStream CreateFile(String path)
        {
            CreateDirectory(GetDirectory(path));
            return File.Create(path);
        }

        public static String GetDirectory(String file)
        {
            var result = "";
            var split = file.Split(DirectorySeparator);
            if(split.Length > 0)
            {
                var directories = new String[split.Length - 1];
                for (int i = 0; i < directories.Length; i++)
                {
                    directories[i] = split[i];
                }
                result = Path(directories);
            }
            return result;
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

        public static String Quotes(String text)
        {
            return String.Format("\"{0}\"", text);
        }

        public static String Text(params String[] text)
        {
            var buffer = new StringBuilder();
            foreach(var t in text)
            {
                buffer.Append(t);
            }
            return buffer.ToString();
        }

        public static String TrimComments(String text)
        {
            var buffer     = new StringBuilder();
            var skipBuffer = true;
            var prevChar   = (char)0;

            for (int i = 0; i < text.Length; ++i)
            {
                var currChar   = text[i];
                    skipBuffer = false;

                if (currChar == '/' && prevChar == '/')
                {
                    while (i < text.Length && text[i] != '\n') ++i;
                    skipBuffer = true;
                }
                else if(currChar == '*' && prevChar == '/')
                {
                    while (i < text.Length - 1 && text[i] != '*' && text[i + 1] != '/') ++i;
                    ++i;
                    skipBuffer = true;
                }

                if (!skipBuffer && prevChar != (char)0)
                {
                    buffer.Append(prevChar);
                }

                if(i < text.Length)
                {
                    prevChar = text[i];
                }                
            }

            if(!skipBuffer)
            {
                buffer.Append(text[text.Length - 1]);
            }

            return buffer.ToString();
        }

        public const char DirectorySeparator = '\\';

        public static readonly char[] UnacceptableFileNameCharacters = { '/', '\\', ':', '|', '*', '?', '"', '<', '>' };

        public static String TrimPath(String path)
        {
            var directories = new List<String>(path.Split(DirectorySeparator));

            for(int i = 1; i < directories.Count;)
            {
                var directory = directories[i];
                if(directory == "..")
                {
                    directories.RemoveAt(i - 1);
                    directories.RemoveAt(i - 1);
                }
                else
                {
                    ++i;
                }
            }
            
            return Utils.Path(directories.ToArray());
        }
    }
}
