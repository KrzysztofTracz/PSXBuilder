using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;
using CommunicationFramework.Messages;

namespace PSXBuilderNetworking.Messages
{
    public class CompilationStartMessage : FileListMessage
    {
        public List<String> PreprocessorDefinitions { get; set; }
        public int          OptimisationLevel       { get; set; }

        public CompilationStartMessage()
            : base()
        {
            PreprocessorDefinitions = new List<String>();
            OptimisationLevel       = 0;
        }

        public CompilationStartMessage(List<String> files, 
                                       List<String> preprocessorDefinitions,
                                       int          optimisationLevel)
            : base(files)
        {
            PreprocessorDefinitions = preprocessorDefinitions;
            OptimisationLevel       = optimisationLevel;
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(PreprocessorDefinitions.Count);
            foreach (var preprocessorDefinition in PreprocessorDefinitions)
            {
                arrayWriter.Append(GetStringSize(preprocessorDefinition));
                arrayWriter.Append(preprocessorDefinition);
            }
            arrayWriter.Append(OptimisationLevel);
            base.AppendData(arrayWriter);
        }

        protected override int GetDataSize()
        {
            int result = sizeof(int);
            foreach (var preprocessorDefinition in PreprocessorDefinitions)
            {
                result += sizeof(int) + GetStringSize(preprocessorDefinition);
            }
            result += sizeof(int);
            return result + base.GetDataSize();
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            var definitions = arrayReader.ReadInt();
            for(int i=0;i<definitions;i++)
            {
                PreprocessorDefinitions.Add(arrayReader.ReadString(arrayReader.ReadInt()));
            }
            OptimisationLevel = arrayReader.ReadInt();
            base.ReadData(arrayReader);
        }
    }
}
