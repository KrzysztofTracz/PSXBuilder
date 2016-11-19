using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    class SetupProgram : Program<Application>
    {
        public override bool Start(params String[] arguments)
        {
            bool result = true;

            foreach(var argument in arguments)
            {
                String settingsName;
                String settingsValue;

                bool isArgumentValid = false;
                if (ParseArgument(argument, out settingsName, out settingsValue))
                {
                    if (Application.Settings.Contains(settingsName))
                    {
                        Application.Settings.SetValue(settingsName, settingsValue);
                        isArgumentValid = true;
                    }
                }

                if (!isArgumentValid)
                {
                    Log("Invalid argument {0}.", argument);
                    DisplayHelp();
                    result = false;
                    break;
                }
            }

            if(result) Application.Settings.Save();

            return result;
        }

        public override bool Start()
        {
            return false;
        }

        protected bool ParseArgument(String argument, out String name, out String value)
        {
            var result = false;
            name  = null;
            value = null;

            argument = argument.Substring(1);
            var nameEndIndex = argument.IndexOf('=');
            if(nameEndIndex > 0 && nameEndIndex + 1 < argument.Length)
            {
                name   = argument.Substring(0, nameEndIndex);
                value  = argument.Substring(nameEndIndex + 1);
                result = true;
            }

            return result; 
        }

        protected override String[] GetArguments()
        {
            var result = new String[Application.Settings.Count];
            
            for (int i = 0; i < Application.Settings.Count; i++)
            {
                var buffer = new StringBuilder();
                buffer.Append("? ");
                buffer.Append(ProgramArgument.Prefix);
                buffer.Append(Application.Settings.GetName(i));
                var defaultValue = Application.Settings.GetDefaultValue(i);
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    buffer.Append('=');
                    buffer.Append(Utils.Quotes(Application.Settings.GetDefaultValue(i)));
                }
                result[i] = buffer.ToString();
            }
            return result;
        }

        protected override String GetDescription()
        {
            return "Application setup";
        }

        protected override String GetSpecifier()
        {
            return "s";
        }
    }
}
