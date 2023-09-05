using System;
using System.Collections.Generic;

namespace WrdLang.Lang.VM {
    public class LangClass {
        public string name;
        public List<Variable> variables=new List<Variable>();

        public List<Method> methods=new List<Method>();

        public LangClass(string name){
            this.name=name;
        }

    

    }
}