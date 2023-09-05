using System;
using System.Collections.Generic;
using WrdLang.Lang.Lex;

namespace WrdLang.Lang.VM.Types {
    public class Array<T> {

        public T[] arr;

        public int len => arr.Length;



        public Array(params T[] arr){
            this.arr=arr;
        }

        public Array(){
            this.arr=new T[0];
        }

        public Array(int z){
            this.arr=new T[z];
        }

        public bool contains(T x){
            return arr.Contains(x);
        }

        public T get(int index){
            if(index>=arr.Length||index<0){
                WrdVM.Error("Invalid index "+index);
            }
            return this.arr[index];
            
        }

        public void set(int index,T value){
            this.arr[index]=value;
        }

        public static bool operator!= (Array<T> lhs, Array<T>  rhs) {
            return lhs.arr!=rhs.arr;
        }

        public static bool operator== (Array<T> lhs, Array<T>  rhs) {
             return lhs.arr==rhs.arr;
        }

        public void add(T value){
            var x=new T[arr.Length+1];

            for(int i=0;i<arr.Length;i++){
                x[i]=arr[i];
            }
            x[arr.Length]=value;
            this.arr=x;
        }

     

    }
}