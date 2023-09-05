using System;
using System.Collections.Generic;

namespace WrdLang.Lang.VM {
    public class Variable {
        public string type;
        public string name;
        public object? value;

        public bool global=false;

        public Variable(string type,string token,object? value){
            this.type=type;
            this.name=token;
            this.value=value;
        }

    }
}