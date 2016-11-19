using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApplicationFramework
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ProgramArgumentAttribute : Attribute
    {
        public bool Optional { get; set; }

        public ProgramArgumentAttribute()
        {
            Optional = false;
        }
    }

    public enum ProgramArgumentType
    {
        Unknown,
        Bool,
        String,
        StringList
    }

    public class ProgramArgument
    {
        public const String Prefix = "/";

        public String    Name         { get; protected set; }
        public String    DefaultValue { get; protected set; }
        public bool      Optional     { get; protected set; }
        public FieldInfo Field        { get; protected set; }
        public IProgram  Program      { get; protected set; }

        public ProgramArgumentType Type { get; protected set; }

        public ProgramArgument(ProgramArgumentAttribute attribute, 
                               FieldInfo fieldInfo,
                               IProgram program)
        {
            Type         = TypeToProgramArgumentType(fieldInfo.FieldType);
            Optional     = Type == ProgramArgumentType.Bool ? true : attribute.Optional;            
            Name         = Utils.Text(Prefix, fieldInfo.Name);
            Field        = fieldInfo;
            Program      = program;
            DefaultValue = Value;
        }

        public String Value
        {
            get
            {
                String result = null;
                switch(Type)
                {
                case ProgramArgumentType.Bool:
                    result = GetBoolValue();
                    break;
                case ProgramArgumentType.String:
                    result = GetStringValue();
                    break;
                case ProgramArgumentType.StringList:
                    result = GetStringListValue();
                    break;
                }
                return result;
            }

            set
            {
                switch (Type)
                {
                case ProgramArgumentType.Bool:
                    SetBoolValue(value);
                    break;
                case ProgramArgumentType.String:
                    SetStringValue(value);
                    break;
                case ProgramArgumentType.StringList:
                    SetStringListValue(value);
                    break;
                }
            }
        }

        protected String GetBoolValue()
        {
            return Field.GetValue(Program).ToString();
        }

        protected String GetStringValue()
        {
            String result = null;
            var value = Field.GetValue(Program);
            if (value != null)
            {
                result = value.ToString();
            }
            return result;
        }

        protected String GetStringListValue()
        {
            String result = null;
            var value = Field.GetValue(Program);
            if(value != null)
            {
                result = Utils.ConcatArguments(",", (Field.GetValue(Program) as List<String>).ToArray());
            }
            return result;
        }

        protected void SetBoolValue(String value)
        {
            bool result = false;
            bool.TryParse(value, out result);
            Field.SetValue(Program, result);
        }

        protected void SetStringValue(String value)
        {
            Field.SetValue(Program, value);
        }

        protected void SetStringListValue(String value)
        {
            Field.SetValue(Program, value != null ? new List<String>(value.Split(new[] { ',' })) : null);
        }

        public static ProgramArgumentType TypeToProgramArgumentType(Type type)
        {
            var result = ProgramArgumentType.Unknown;
            if (type.Equals(typeof(bool)))
            {
                result = ProgramArgumentType.Bool;
            }
            else if (type.Equals(typeof(String)))
            {
                result = ProgramArgumentType.String;
            }
            else if (type.Equals(typeof(List<String>)))
            {
                result = ProgramArgumentType.StringList;
            }
            return result;
        }
    }
}
