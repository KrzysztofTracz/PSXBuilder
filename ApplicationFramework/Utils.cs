using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public class Utils
    {
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
                    if (d != "." && d != "..")
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

        public static String GetAcceptableFileName(String filename)
        {
            var result = filename;
            foreach(var unacceptableCharacter in UnacceptableFileNameCharacters)
            {
                result = result.Replace(unacceptableCharacter, '-');
            }
            return result;
        }

        public const char DirectorySeparator = '\\';

        public static readonly char[] UnacceptableFileNameCharacters = { '/', '\\', ':', '|', '*', '?', '"', '<', '>' };
    }
}
