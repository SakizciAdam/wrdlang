using System;
using System.Collections.Generic;

namespace WrdLang.Lang.Utils {
    public static class WrdUtils {
        public static void print(object a){
            Console.Write(a);
      
            
        }
        public static void println(object a){
            Console.WriteLine(a);
        }

        public static void Exit(int i){
            Environment.Exit(i);
        }

        public static Type ByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }
            return null;
        }

        
    

        public static string ToString(object a){
            return a.ToString();
        }

    }
}