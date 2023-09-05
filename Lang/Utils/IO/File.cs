using System;
using System.Collections.Generic;

namespace WrdLang.Lang.IO {
    public class File {
        public readonly string name;
        public File(string fileName){
            this.name=fileName;
        }

        public static bool operator!= (File lhs, File  rhs) {
            return lhs.name!=rhs.name;
        }

        public static bool operator== (File lhs, File  rhs) {
             return lhs.name==rhs.name;
        }

        public bool exists(){
          
            return System.IO.File.Exists(this.name);
        }

        public bool create(){
          
            System.IO.File.Create(name);
            return exists();
        }

        public bool delete(){
            System.IO.File.Delete(name);
            return !exists();
        }

        public void write(string str){
           
            System.IO.File.WriteAllText(this.name,str);
        }

        public void write(VM.Types.Array<byte> x){
    

            System.IO.File.WriteAllBytes(this.name,x.arr);
        }

        public VM.Types.Array<byte> readAllBytes(){
            return new VM.Types.Array<byte>(System.IO.File.ReadAllBytes(this.name));
        }

        public string read(){
            return System.IO.File.ReadAllText(this.name);
        }

        public VM.Types.Array<string> readLines(){
            return new VM.Types.Array<string>(System.IO.File.ReadLines(this.name).ToArray<string>());
        }





    }
}