using System;
using System.Collections.Generic;

namespace WrdLang.Lang.Lex {
    public static class Lexer {

        public static bool IsLetterOnly(string a){
            foreach(char c in a){
                if(!Char.IsLetter(c)){
                    return false;
                }
            }
            return true;
        }

        public static bool IsSymbolOnly(string a){
            foreach(char c in a){
                if(!IsSymbol(c)){
                    return false;
                }
            }
            return true;
        }

        public static bool IsSymbol(char c){
            return c=='.'||c==','||c=='+'||c=='-'||c=='\"'
            ||c=='*'||c=='/'||c=='&'||c==':'
            ||c=='|'||c=='%'||c=='^'||c=='!'||c=='='||c=='>'||c=='<'
            ||c=='('||c==')'||c=='{'||c=='}'||c=='['||c==']'||c=='\''||c=='\\';
        }

        public static bool IsNumber(string a){

            return int.TryParse(a, out _);
        }

        public static void AddLineEnding(ref List<Token> tokens){
            tokens.Add(new Token("LINE","SPACE"));
     
        }

        public static void AddSpace(ref List<Token> tokens){
            tokens.Add(new Token("SPACE","SPACE"));
     
        }

        public static void AddString(ref List<Token> tokens,string value){
            tokens.Add(new Token(value,"STRING"));
     
        }

        public static void AddToken(ref List<Token> tokens,ref string token,string type){
          
            if(token.Trim()==""){
                return;
            }
            tokens.Add(new Token(token,type));
            token="";
        }

        public static Token[] TokenizeLine(string str){
            List<Token> tokens=new List<Token>();
            string token="";
  
            for(int i=0;i<=str.Length;i++){
                bool ending=i==str.Length;
                char c=ending ? ' ':str[i] ;
                char next=i>=str.Length-1 ? ' ' : str[i+1];
                char previous=i==0 ? ' ' : str[i-1];
               
      
                
                if(token.Length>0){
      
                    if(IsSymbolOnly(token)){
                        AddToken(ref tokens,ref token,"SYMBOL");
                    }
                    if(IsLetterOnly(token)&&(c==' '||IsSymbol(c))){
                        AddToken(ref tokens,ref token,"WORD");
                       
                    }
                    if(IsLetterOnly(token)&&IsNumber(c+"")){
                        AddToken(ref tokens,ref token,"WORD");
                       
                    }
                    if(IsNumber(token)&&(!IsNumber(token+c)||c==' ')){
                        AddToken(ref tokens,ref token,"NUMBER");
                    }
                    
                
                    
                }
                if(c!=' '){
                    token+=c;
                }
                if(c==' '){
            
                    AddSpace(ref tokens);
                }

    

                

               
                
            


            }

            AddLineEnding(ref tokens);

            return tokens.ToArray();
        }

        public static bool IsPrimitive(string t){
            return t=="bool"||t=="string"||t=="int"||t=="float"||t=="long"||t=="double";
        }

        public static List<Token> Tokenize(string str){
            List<Token> tokens=new List<Token>();
            foreach(var line in str.Split("\n")){
                tokens.AddRange(TokenizeLine(line));
            }



            return tokens;
        }
    }
}