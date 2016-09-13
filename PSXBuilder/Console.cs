using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSXBuilder
{
    static class Console
    {
        public static void Write(String format, params object[] args)
        {
            System.Console.Write(format, args);
#if DEBUG
            System.Diagnostics.Debug.Write(String.Format(format, args));
            System.Diagnostics.Debug.Flush();
#endif    
        }

        public static void WriteLine(String format, params object[] args)
        {
            System.Console.WriteLine(format, args);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format(format, args));
            System.Diagnostics.Debug.Flush();
#endif 
        }
    }
}
