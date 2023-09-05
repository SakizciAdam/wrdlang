using System;

namespace WrdLang.Lang.Lex {
    public class Token {
        public string type;
        public string token;


        public Token(string token){
            this.token=token;
        }
        public Token(string token,string type){
            this.token=token;
            this.type=type;
        }

        public bool IsSpace => type=="SPACE"&&type==token;
        public bool IsLineEnding => type=="SPACE"&&type!=token;

        public bool IsNumber => type=="NUMBER"||type=="INT"||type=="FLOAT"||type=="DOUBLE"||type=="LONG";

        public bool IsUnknown(){
            return !Lexer.IsLetterOnly(token)&&!Lexer.IsNumber(token)&&!Lexer.IsSymbolOnly(token);
        }
    }
}