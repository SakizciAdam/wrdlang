using System;
using System.Collections.Generic;

namespace WrdLang.Lang.VM {
    public class Parameter {
        public string type;
        public string name;

        public Parameter(string type,string token){
            this.type=type;
            this.name=token;

        }

    }
}