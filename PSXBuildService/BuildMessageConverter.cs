using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;

namespace PSXBuildService
{
    public class BuildMessageConverter
    {
        protected class Message
        {
            public enum EFileOrigin
            {
                Unknown,
                Project,
                SDK
            }

            public enum EType
            {
                Unknown,
                Warning,
                Error
            }

            public int           Line       = -1;
            public String        File       = "";
            public EFileOrigin   FileOrigin = EFileOrigin.Unknown; 
            public EType         Type       = EType.Unknown;
            public StringBuilder Buffer     = new StringBuilder();

            public static String ToString(EType type)
            {
                var result = "";
                switch(type)
                {
                case EType.Error:
                    result = "error :";
                    break;
                case EType.Warning:
                    result = "warning :";
                    break;
                }
                return result;
            }
        }

        public const String WarningSignature = " warning:";

        public String LocalProjectPath { get; protected set; }
        public String LocalSDKPath     { get; protected set; }

        public String OriginalProjectPath { get; protected set; }
        public String OriginalSDKPath     { get; protected set; }

        public BuildMessageConverter(String localProjectPath,
                                     String originalProjectPath,
                                     String localSDKPath,
                                     String originalSDKPath)
        {
            LocalProjectPath    = localProjectPath.Replace('\\', '/') + "/";
            OriginalProjectPath = originalProjectPath + "\\";

            LocalSDKPath    = localSDKPath + "\\";
            OriginalSDKPath = originalSDKPath + "\\";
        }

        public String ConvertMessage(String message)
        {
            var result   = new StringBuilder();
            var messages = new List<Message>();
            
            var lines    = message.Split('\n');

            foreach (var line in lines)
            {
                var fileOrigin = Message.EFileOrigin.Unknown;
                var index      = -1;

                if (line.StartsWith(LocalProjectPath))
                {
                    fileOrigin = Message.EFileOrigin.Project;
                    index = LocalProjectPath.Length;
                }
                else if(line.StartsWith(LocalSDKPath))
                {
                    fileOrigin = Message.EFileOrigin.SDK;
                    index = LocalSDKPath.Length;
                }

                if (fileOrigin != Message.EFileOrigin.Unknown)
                {
                    var buffer = new StringBuilder();
                    while (index < line.Length && line[index] != ':')
                    {
                        buffer.Append(line[index]);
                        index++;
                    }

                    var fileName = buffer.ToString();
                    buffer.Clear();
                    index++;

                    while (index < line.Length && line[index] != ':')
                    {
                        buffer.Append(line[index]);
                        index++;
                    }

                    int lineNumber = -1;
                    if(int.TryParse(buffer.ToString(), out lineNumber) && index < line.Length)
                    {
                        var str = line.Substring(index + 1, line.Length - (index + 1));
                        var messageType = Message.EType.Error;


                        if(str.StartsWith(WarningSignature))
                        {
                            messageType = Message.EType.Warning;
                            str = str.Substring(WarningSignature.Length, str.Length - WarningSignature.Length);
                        }

                        bool createNew = true;
                        foreach (var m in messages)
                        {
                            if (m.FileOrigin == fileOrigin  &&
                                m.File       == fileName    &&
                                m.Type       == messageType &&
                                m.Line       == lineNumber)
                            {
                                m.Buffer.Append(str);
                                createNew = false;
                                break;
                            }
                        }

                        if(createNew)
                        {
                            var newMessage = new Message();
                            newMessage.FileOrigin = fileOrigin;
                            newMessage.File       = fileName;
                            newMessage.Type       = messageType;
                            newMessage.Line       = lineNumber;
                            newMessage.Buffer.Append(str);

                            messages.Add(newMessage);
                        }
                    }
                }
            }

            foreach (var m in messages)
            {
                m.File = m.File.Replace('/', '\\');

                var str = String.Format("{0}{1}({2}) : {3}{4} \n", GetOriginalRootPath(m.FileOrigin),
                                                                   m.File,
                                                                   m.Line,
                                                                   Message.ToString(m.Type),
                                                                   m.Buffer.ToString());
                //System.Console.WriteLine(str);
                result.Append(str);
            }

            return result.ToString();
        }

        protected String GetOriginalRootPath(Message.EFileOrigin fileOrigin)
        {
            var result = "";
            switch(fileOrigin)
            {
            case Message.EFileOrigin.Project:
                result = OriginalProjectPath;
                break;
            case Message.EFileOrigin.SDK:
                result = OriginalSDKPath;
                break;
            }
            return result;
        }

    }
}
