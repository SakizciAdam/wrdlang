using System;
using System.Collections.Generic;
using WrdLang.Lang.Lex;

namespace WrdLang.Lang.VM {
    public class Method {
        public string name,type;
        public List<Parameter> parameters;
        public Token[] code;

        public Method(string name,string type){
            this.name=name;
            this.type=type;
        }

         public Method(string name,string type,List<Parameter> parameters){
            this.name=name;
            this.parameters=parameters;
            this.type=type;
        }

    }
}