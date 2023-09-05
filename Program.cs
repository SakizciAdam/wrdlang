using System;
using WrdLang.Lang.Lex;
using WrdLang.Lang.VM;

namespace WrdLang {
    public static class Program {
 
        public static void Main(string[] args){

            string arg0=args.Length==0 ? "" : args[0];
            for(int i=1;i<args.Length;i++){
                if(args[i]=="--debug"){
                    WrdVM.DebugMode=true;
                }
            }
            if(arg0==""||arg0=="-h"){
                Console.WriteLine("wrdlang -h -> Shows help message");
                Console.WriteLine("wrdlang <file name> -> Executes file");
            } else {
                Run(arg0);
            }

           
           
        }

        public static Token[] getTokens(WrdVM vm,string fileName){
            using (StreamReader reader = new StreamReader(fileName))
            {
            
                Token[] tokens=vm.Parse(Lexer.Tokenize(reader.ReadToEnd()));

             
                return tokens;
            }
        }

        public static WrdVM Run(string fileName){
            WrdVM vm=new WrdVM();
         

            vm.Execute(getTokens(vm,fileName));
            return vm;
        }

    
    }
}
