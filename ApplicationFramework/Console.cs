using System;
using System.Text;

namespace ApplicationFramework
{
    public class Console : Logger
    {
        public override void Log(string text)
        {
            base.Log(text);
            WriteLine(text);
        }

        public void Write(String format, params object[] args)
        {
            if (isEmpty)
            {
                WriteLine(format, args);
            }
            else
            {
                System.Console.Write(format, args);
#if DEBUG
                System.Diagnostics.Debug.Write(String.Format(format, args));
                System.Diagnostics.Debug.Flush();
#endif
            }
        }

        public void WriteLine()
        {
            WriteLine("");
        }

        public void WriteLine(String format, params object[] args)
        {
            var line = String.Format("{0}{1}", GetTabs(), String.Format(format, args));
            System.Console.WriteLine(line);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format(line));
            System.Diagnostics.Debug.Flush();
#endif 
            isEmpty = false;
        }

        public void WriteLineSeparator()
        {
            WriteLine("--------------");
        }

        public void PushTab()
        {
            tabs++;
        }

        public void PopTab()
        {
            tabs--;
            if (tabs < 0) tabs = 0;
        }

        protected String GetTabs()
        {
            String result = "";
            if (tabs > 0)
            {
                StringBuilder sb = new StringBuilder();
                for(int i=0;i<tabs;i++)
                {
                    sb.Append('\t');
                }
                result = sb.ToString();
            }
            return result;
        }

        protected int  tabs    = 0;
        protected bool isEmpty = true;
    }
}
