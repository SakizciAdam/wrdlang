using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using WrdLang.Lang.Lex;
using WrdLang.Lang.Utils;
using WrdLang.Lang.VM.Types;
using Z.Expressions;


namespace WrdLang.Lang.VM {
    public class WrdVM {
        private List<Variable> variables=new List<Variable>();
        private List<Method> methods=new List<Method>();

        public List<LangClass> langClasses=new List<LangClass>();

        private EvalContext context = new EvalContext();

        public int end=0;

        public WrdVM(){
            context.ForceCharAsString = true;
            context.RegisterStaticMethod(typeof(WrdUtils));	
            context.RegisterMember(typeof(WrdVM));
            context.RegisterLocalVariable("vm",this);
            context.RegisterAlias("\"","\'");
         
            context.RegisterType(typeof(WrdLang.Lang.VM.Types.Array<>));
            context.RegisterType(typeof(WrdLang.Lang.VM.Types.LClass));
           
   
           
        }

        public Dictionary<string, object> GetGlobalVariables(){
            var values = new Dictionary<string, object>() { };
            foreach(Variable v in variables){
                if(!v.global) continue;
                values[v.name]=v.value;
            }
            return values;
        }

        public Dictionary<string, object> GetVariables(){
            var values = new Dictionary<string, object>() { };
            foreach(Variable v in variables){
                values[v.name]=v.value;
            }
            return values;
        }

        public object ExecuteMethod(string name,object[]? parameters){
            if(GetMethod(name)==null){
                Error("Invalid method: "+name);
            }
            Method method=GetMethod(name);
            Dictionary<string,object> values=GetGlobalVariables();
            int o=0;
            Debug(method.parameters.Count+" blow");
            foreach(var parameter in method.parameters){

                
                context.RegisterLocalVariable(parameter.name,parameters[o]);
                o++;
            }

        
            
            
            object x=context.Execute(ExecuteWithEnd(method.code,end+1),values);
            context.AliasLocalVariables.Clear();
           
            return x;
        }

        public void AddMethod(string name,string type,List<Parameter> paramaters){
            string str=@"public ";
            str+=type+" ";

            str+=name;
            str+=@"(";
          
            foreach(var parameter in paramaters){
                str+=parameter.type+" "+parameter.name+",";
            }
            if(paramaters.Count>0)
                str=str.Substring(0,str.Length-1);
            

    
            string haha="\""+name+"\"";
            str+=@") {";
            if(type!="void"){
                str+=" return";
            }
            str+=@" vm.ExecuteMethod("+haha+",";
            if(paramaters.Count==0){
                str+="null";
            } else {
                string z="new object[]{";
                foreach(var parameter in paramaters){
               
                    z+=parameter.name+",";
                }

                z=z.Substring(0,z.Length-1);

                z+="}";
                str+=z;
            }
            str+=");}";
            



            Debug(str);
            context.AddMethod(str);

        }

      

        public void AddVariable(string name,string value,string type){
            type=type.ToUpper();
            Debug("Add variable "+type+" "+name+" "+value);
            
            if(value==""){
                variables.Add(new Variable(type,name,null));
                return;
            }

            bool global=false;

            if(GetVariable(name)!=null){
                global=GetVariable(name).global;
            }
          
            var values=GetVariables();

            if(GetVariable(name)!=null){
                variables.Remove(GetVariable(name));
            }
            
            variables.Add(new Variable(type,name,context.Execute(value,values)));
            if(variables[variables.Count-1].value.GetType()==typeof(LClass)){
                LangClass? cls=this.langClasses.Find(b => value.Contains(b.name));

                if(cls==null){
                    WrdVM.Error("Could not find class");
                }
                LClass bruh=(LClass)variables[variables.Count-1].value;
                foreach(Variable v in cls.variables){
                    bruh._fields.Add(v.name,new KeyValuePair<Type, object>(v.value.GetType(),v.value));
                }
            }
            
            variables[variables.Count-1].global=global;
            Debug(variables[variables.Count-1].value);
  
        }

        public Method? GetMethod(string name){
            return methods.Find(x => x.name==name);
        }

        public Variable? GetVariable(string name){
            return variables.Find(x => x.name==name);
        }

        public Token[] Parse(List<Token> tokens){
            List<Token> tokens2=new List<Token>();
            bool stringMode=false;
            string sr="";
            int skip=0;
            for(int i=0;i<tokens.Count;i++){

                Token t=tokens[i];
                Token? nextToken=i>=tokens.Count-1 ? null : tokens[i+1];
                Token? next2Token=i>=tokens.Count-2 ? null : tokens[i+2];
                Token? next3Token=i>=tokens.Count-3 ? null : tokens[i+3];

                if(skip>0){
                    skip--;
                    continue;
                }
                if(t.type=="SYMBOL"&&t.token=="\'"){
                    if(stringMode){
                        Lexer.AddString(ref tokens2,sr);
                        sr="";
                    }
                    stringMode=!stringMode;
                }
                else if(stringMode){
                    if(t.IsSpace){
                        sr+=" ";
                        continue;
                    }
                    sr+=t.token;
                } else {
                    if(t.type=="NUMBER"){
                        if(nextToken!=null){
                            if(nextToken.type=="WORD"){
                                string lower=nextToken.token.ToLower();
                                if(lower=="d"){
                                    t.type="DOUBLE"; 
                                    skip=1;
                                }
                                if(lower=="f"){
                                    t.type="FLOAT"; 
                                    skip=1;
                                }
                                if(lower=="l"){
                                    t.type="LONG"; 
                                    skip=1;
                                }
                                
                            } else if(next2Token!=null&&nextToken.token=="."){
                                if(next2Token.type=="NUMBER"){
                              
                                    if(next3Token!=null){
                                        if(next3Token.type=="WORD"){
                                            string lower=next3Token.token.ToLower();
                                            t.token+="."+next2Token.token;
                                            skip=3;
                                            if(lower=="d"){
                                                t.type="DOUBLE"; 
                                            }
                                            else if(lower=="f"){
                                                t.type="FLOAT"; 
                                            } else {
                                                t.type="DOUBLE";
                                                skip=2;
                                            }
                                            
                                
                                        } else {
                                            t.token+="."+next2Token.token;
                                            t.type="DOUBLE"; 
                                            skip=2;
                                        }
                                    } else {
                                        t.token+="."+next2Token.token;
                                        t.type="DOUBLE"; 
                                        skip=2;
                                    }
                                }
                            }
                        }
                    } else if(t.type=="WORD"){
                        if(Lexer.IsPrimitive(t.token)){
                            t.type=t.token.ToUpper();
                        } else if(t.token=="true"||t.token=="false"){
                            t.type="TF";
                        } else if(Char.IsUpper(t.token[0])){
                            t.type="TYPE";
                        } else if(t.token=="end"&&nextToken.IsNumber){
                            skip++;
                            t.token="end"+nextToken.token;
                        }
                    } else if(t.type=="SYMBOL"){
                        if(t.token=="+"&&nextToken.token=="="){
                            skip++;
                            t.token="+=";
                        }
                        if(t.token=="-"&&nextToken.token=="="){
                            skip++;
                            t.token="-=";
                        }
                        if(t.token=="/"&&nextToken.token=="="){
                            skip++;
                            t.token="/=";
                        }
                         if(t.token=="*"&&nextToken.token=="="){
                            skip++;
                            t.token="*=";
                        }
                    }
                    tokens2.Add(t);
                }
            }
         
            return tokens2.ToArray();
        }

        public string ExecuteWithEnd(Token[] tokens,int newEnd){
            int old=end;

            end=newEnd;

            string a=Execute(tokens);

            end=old;

            return a;
        }

        public string Execute(Token[] tokens){

            bool start=true;
            int skip=0;

            Variable? variable=null;
            string variableValue="";
            bool equalSign=false;
            
            bool ifStatement=false;
            bool lParentheses=false,rParentheses=false;
            string statement="";
            bool correctStatement=false;
            bool curlyL=false,curlyR=false;
            List<Token> ifTokens=new List<Token>();

    

            bool forStatement=false;
            string varName="";
            bool doubleDot=false;
            string startVarStr="";
            int startVar=0;
            int times=-1;
            string timesStr="";
            bool forLCurly=false;
            List<Token> forTokens=new List<Token>();

            bool methodStatement=false;
            string methodType="";
            string methodName="";
            List<Parameter> methodParameters=new List<Parameter>();
            string paraType="";
            bool methodRP=false,methodLP=false;
            bool methodLCurly=false;
            List<Token> methodTokens=new List<Token>();

            bool exec=false;
            string execCommand="";

            bool import=false;
            string package="";

            bool cls=false;
            string clsName="";
            bool clsLeftCurly=false;
            List<Token> clsTokens=new List<Token>();

            bool global=false;
            string globalName="";

            bool set=false;
            string setVariable="";
            int signType=-1;
            string setNewValue="";

            for(int i=0;i<tokens.Length;i++){

                Token token=tokens[i];
                Token? nextToken=i>=tokens.Length-1 ? null : tokens[i+1];
                Debug(token.type+" "+token.token);
                
                
                if(skip>0){
                    skip--;
                    continue;
                }

                if(import){
                    package+=token.type=="SPACE" ? " " : token.token;
                    if(!token.IsLineEnding){
                        continue;
                    }
                }

                if(curlyL&&token.token!="end"+end){
                    ifTokens.Add(token);
                    continue;
                }
                if(clsLeftCurly&&token.token!="}"){
                    clsTokens.Add(token);
                    continue;
                }
                if(forLCurly&&token.token!="end"+end){
                    Debug(token.token+" KEEP ADDING");
                    forTokens.Add(token);
                    continue;
                }
                if(methodLCurly&&token.token!="end"+end){
                    methodTokens.Add(token);
                    continue;
                }
                

                if(variable!=null){
                    

                    if(variable.name==""&&!token.IsSpace){
                        
                        if(token.IsLineEnding||token.type!="WORD"){
                            Error("Expected variable name");
                        }
                        variable.name=token.token;
                    } else if(!equalSign&&!token.IsSpace){
                        if(token.type=="SYMBOL"&&token.token=="="){
                            equalSign=true;
                        } else {
                            Error("Expected =, got "+token.token);
                        }
                    } else {
                        if(token.type=="STRING"){
                            variableValue+="\""+token.token+"\"";
                        } else {
                            variableValue+=token.IsSpace ? " " : token.token;
                        }
                        
                    }
                    
                
                }

                if(forStatement){
           
                    if(varName==""){
                        if(!token.IsSpace&&token.type!="WORD"){
                            Error("Expected var name, got "+token.token);
                        }
                        if(token.type=="WORD"){
                            varName=token.token;
                            Debug(varName+" xddd");
                        }
                        
                       
                     
                    } 
                    else if(!doubleDot){
              
                        if(token.token=="to"){
                            doubleDot=true;
                     
                            startVar=context.Execute<int>(startVarStr,GetVariables());
                            Debug(startVar+" starts at");
                        } else {
                            startVarStr+=token.IsSpace ? " " : token.token;
                        }
                        
                    } else if(!forLCurly){
                        
                        if(token.token==":"){
                            forLCurly=true;

                            times=context.Execute<int>(timesStr,GetVariables());
                            Debug(times+" ends at");
                        } else {
                            timesStr+=token.IsSpace ? " " : token.token;
                        }
                    } else {
                        if(token.token=="end"+end){
                            if(forTokens.Count>0){
                                WrdVM vm=new WrdVM();
                                vm.end=end+1;
                                vm.variables=this.variables;
                                vm.methods=methods;
                                vm.langClasses=langClasses;

                                for(int z=startVar;z<times;z++){
                                    vm.AddVariable(varName,z+"","INT");
                                    vm.Execute(forTokens.ToArray());
                                }
                            }
                        
                            forStatement=false;
                            forTokens.Clear();
                            forLCurly=false;
                            timesStr="";
                            startVarStr="";
                            times=-1;
                            doubleDot=false;
                            varName="";
                        } 
                    }
                }

                if(methodStatement){
                    if(methodName==""){
                        if(token.type=="WORD"){
                            methodName=token.token;
                            
                        } else if(!token.IsSpace){
                            Error("Expected a method name, got "+token.token);
                        }
                    }  else if(methodType==""){
                        if(!token.IsSpace){
                            methodType=token.token;
                        }
                    }
                    
                    else if(!methodLP){
                        if(token.token=="("){
                            methodLP=true;
                        } else if(!token.IsSpace){
                            Error("Expected (, got "+token.token);
                        }
                    } else if(!methodRP){
              
                        if(token.token==")"){
                            methodRP=true;
                        } else {
                            
                            if(!token.IsSpace&&token.token!=","){
                                if(paraType==""){
                                    paraType=token.token;
                                } else {
                                    methodParameters.Add(new Parameter(paraType,token.token));
                                    
                                    paraType="";
                                }
                            } 
                        }
                    } else if(!methodLCurly){
                        if(token.token==":"){
                            methodLCurly=true;
                        } else if(!token.IsSpace){
                            Error("Expected :, got "+token.token);
                        }
                    } else {
                        if(token.token=="end0"){
                           
                            Debug(methodParameters.Count+" da faq");
                            Method method=new Method(methodName,methodType,methodParameters);
                            method.code=methodTokens.ToArray();
                            methods.Add(method);
                            AddMethod(method.name,method.type,methodParameters);
                       
                            
                            methodLCurly=false;
                            methodLP=false;
                            methodName="";
                            methodParameters=new List<Parameter>();
                            methodRP=false;
                            methodStatement=false;
                            methodTokens.Clear();
                            methodType="";

                            



                        } 
                    }
                }
          
                if(ifStatement){
                    if(!curlyL){
                        if(token.token==":"){
                            curlyL=true;
                            statement=statement.Substring(1);
                            if(statement.Trim().Length==0){
                                Error("Expected a statement");
                            }
               
                            
                            correctStatement=context.Execute<bool>(statement,GetVariables());
                        } else {
                            Debug(statement+" + "+token.token);
                            if(token.IsSpace){
                                statement+=" ";
                            } else {
                                statement+=token.token;
                            }
                            
                        }

      
                        
                    } else if(!curlyR){
                        
                        if(token.token=="end"+end){
                            Debug("TEST");
                            if(ifTokens.Count>0&&correctStatement){
                                WrdVM vm=new WrdVM();
                                vm.end=end+1;
                                vm.variables=this.variables;
                                vm.methods=methods;
                                vm.langClasses=langClasses;

                                vm.Execute(ifTokens.ToArray());
                            }
                            ifTokens.Clear();
                            lParentheses=false;
                            rParentheses=false;
                            curlyL=false;
                            curlyR=false;
                            statement="";
                            correctStatement=false;
                        } 
                    }
                    
                }

                if(cls){
                    if(clsName==""){
                        if(token.type=="WORD"||token.type=="TYPE"){
                            clsName=token.token;
                        } else if(!token.IsSpace){
                            Error("Expected class name, got "+token.token);
                        }
                    } else if(!clsLeftCurly){
                        if(token.token=="{"){
                            clsLeftCurly=true;
                        } else if(!token.IsSpace){
                            Error("Expected {, got "+token.token);
                        }
                    } else {
                        if(token.token=="}"){
                            LangClass clz=new LangClass(clsName);
                            context.RegisterAlias(clsName,"LClass");
                            if(clsTokens.Count>0){
                                WrdVM vm=new WrdVM();
                             

                                vm.Execute(clsTokens.ToArray());
                                clz.variables=vm.variables;
                                clz.methods=vm.methods;
                                
                            }
                            
                            langClasses.Add(clz);
                            clsTokens.Clear();
                            cls=false;
                            clsLeftCurly=false;
                            clsName="";
                        }
                    }
                }

                if(set){
                    if(setVariable==""){
                        if(token.type=="WORD"){
                            setVariable=token.token;
                            Debug("SETTING "+setVariable);
                        }
                    } else if(signType==-1){
                       
                        if(token.token=="="){
                            signType=0;
                        } else if(token.token=="+="){
                            signType=1;
                        } else if(token.token=="-="){
                            signType=2;
                        }else if(token.token=="*="){
                            signType=3;
                        }else if(token.token=="/="){
                            signType=4;
                        }else if(!token.IsSpace){
                            Error("Expected =, got "+token.token);
                        }
                    } else {
                        setNewValue+=token.IsSpace ? " " : token.IsLineEnding ? "" : token.token;
                    }
                }

                if(import){
                    Debug(package);
                    Type type=WrdUtils.ByName(package.Trim());

                    if(type!=null){
                        context.RegisterType(type);
                       
                    } 
                    import=false;
                    package="";
                    
                
                }

                if(global){
                    if(token.type=="WORD"){
                        globalName="";
                        global=false;

                        if(GetVariable(token.token)==null){
                            Error("Expected variable name, got "+token.token);
                        }

                        GetVariable(token.token).global=true;
                    } else if(!token.IsSpace){
                        Error("Expected variable name, got "+token.token);
                    }
                }

                if(exec){
                    execCommand+=token.IsLineEnding ? "" : token.IsSpace ? " " : token.token;


                }

                if(start){
                    if(token.type=="WORD"){
                        if(token.token=="var"){
                            variable=new Variable("UNKNOWN","",null);
                        }
                        else if(token.token=="if"){
                            ifStatement=true;
                            statement="";
                            correctStatement=false;
                            curlyL=false;
                            curlyR=false;
                            lParentheses=false;
                            rParentheses=false;
                        }
                        else if(token.token=="for"){
                            forStatement=true;
                            varName="";
                            forLCurly=false;
                        }
                        else if(token.token=="return"){
                            string z="";
                            for(int j=i+1;j<tokens.Length;j++){
                                z+=tokens[j].type!="SPACE" ? tokens[j].token : " ";
                            }
                            Debug(z);
                            return z;
                            
                        }
                        else if(token.token=="struct"){
                            cls=true;
                            clsLeftCurly=false;
                            clsName="";
                            clsTokens.Clear();
                        } 
                        else if(token.token=="fn"){
                            
                            methodStatement=true;
                            methodTokens.Clear();
                            methodLP=false;
                            methodLCurly=false;
                            methodRP=false;
                            paraType="";
                            methodParameters.Clear();
                            methodName="";
                            methodType="";
                        } 
                        else if(token.token=="import"){
                            import=true;
                            package="";
                        }
                        else if(token.token=="global"){
                            global=true;
                            globalName="";
                        }
                        else if(token.token=="set"){
                            set=true;
                            setVariable="";
                            signType=-1;
                            setNewValue="";
                            Debug("SET MODE");
                        }
                        else if(token.token=="exec"){
                            Debug("exec mode");
                            exec=true;
                            execCommand=token.token;
                        }
                        else{
                            Debug("exec mode");
                            exec=true;
                            execCommand=token.token;
                        }
                    } 
                    
                }
                if(token.type!="SPACE"){
                    start=false;
                }
                




                if(token.IsSpace&&nextToken!=null&&nextToken.IsLineEnding){
                
                    if(variable!=null){
                        if(equalSign){
                            if(variableValue==""){
                                Error("Expected a variable value got none");
                            } else {
                                AddVariable(variable.name,variableValue,variable.type.ToLower());
                            }
                        } else {
                            AddVariable(variable.name,"",variable.type.ToLower());
                        }
                    }
                    if(set&&signType>=0){
                        if(setNewValue.Trim().Length==0){
                            Error("Expected a variable value got none");
                        }
                        Variable? v=GetVariable(setVariable);

                        if(v==null){
                            Error("Invalid variable name: "+setVariable);
                        }
                        
                        

                        

                        if(signType==0){
                            object val=context.Execute(setNewValue,GetVariables());
                                v.value=val;
                        } else if(signType==1){
                            object val=context.Execute(setNewValue,GetVariables());

                            v.value=context.Execute(v.value+"+"+val,GetVariables());
                        } else if(signType==2){
                            object val=context.Execute(setNewValue,GetVariables());

                            v.value=context.Execute(v.value+"-"+val,GetVariables());
                        } else if(signType==3){
                            object val=context.Execute(setNewValue,GetVariables());

                            v.value=context.Execute(v.value+"*"+val,GetVariables());
                        } else if(signType==4){
                            object val=context.Execute(setNewValue,GetVariables());

                            v.value=context.Execute(v.value+"/"+val,GetVariables());
                        }


                        
                        Debug("Set variable "+v.name+" "+v.value);
                        
                    }
                    if(exec){
                        context.Execute(execCommand,GetVariables());
                    }
                    exec=false;
                    execCommand="";
                    set=false;
                    signType=-1;
                    setNewValue="";
                    start=true;
                    exec=false;
                    execCommand="";
                    equalSign=false;
                    variableValue="";
                    variable=null;
                }



            }

    
            return "null";
        }
        public static bool DebugMode = false;
        public static void Debug(object x){
            if(DebugMode){
                Console.WriteLine("DEBUG: "+x);
            }
        }


        public static void Error(string error){
            Console.WriteLine("ERROR: "+error);
            Environment.Exit(1);
        }
    }
}