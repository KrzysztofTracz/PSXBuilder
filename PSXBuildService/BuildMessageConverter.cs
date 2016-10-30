using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;

namespace PSXBuildService
{
    public static class BuildMessageConverter
    {
        public static String ConvertMessage(String message, String file)
        {
            var result = new StringBuilder();
            var output = new Dictionary<int, StringBuilder>();
            
            var lines    = message.Split('\n');
            var fileName = Utils.GetFileName(file);

            file = file.Replace('\\', '/');

            System.Console.WriteLine(">> lines: " + lines.Length);
            System.Console.WriteLine(">> " + file);

            foreach (var line in lines)
            {
                if (line.StartsWith(file))
                {
                    System.Console.WriteLine(">> " + line);

                    var index  = file.Length + 1;
                    var buffer = new StringBuilder();
                    while(index < line.Length && line[index] != ':')
                    {
                        buffer.Append(line[index]);
                        index++;
                    }

                    System.Console.WriteLine(">> " + buffer.ToString());

                    int lineNumber;
                    if(int.TryParse(buffer.ToString(), out lineNumber) && index < line.Length)
                    {
                        

                        var str = line.Substring(index + 1, line.Length - (index + 1));
                        if(output.ContainsKey(lineNumber))
                        {
                            output[lineNumber].Append(' ');
                            output[lineNumber].Append(str);
                        }
                        else
                        {
                            output.Add(lineNumber, new StringBuilder(str));
                        }
                    }
                }
            }

            var keys = output.Keys.ToList();
            keys.Sort();

            foreach (var key in keys)
            {
                result.Append(String.Format("{0}({1}) : error : {2} \n", fileName, 
                                                                         key, 
                                                                         output[key].ToString()));
            }

            return result.ToString();
        }
    }
}
