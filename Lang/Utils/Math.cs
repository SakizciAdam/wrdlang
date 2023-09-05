using System;
using System.Collections.Generic;

namespace WrdLang.Lang {
    public static class Math {
        
        public static double PI=System.Math.PI;

        public static double ceil(double x){
             return System.Math.Ceiling(x);
        }

        public static double floor(double x){
             return System.Math.Floor(x);
        }

        public static double sqrt(double x){
            return System.Math.Sqrt(x);
        }

        public static double sin(double x){
            return System.Math.Sin(x);
        }

        public static double cos(double x){
            return System.Math.Cos(x);
        }

        public static int pow(int a,int b){
            return (int)System.Math.Pow(a,b);
        }

        public static float pow(float a,float b){
            return (float)System.Math.Pow(a,b);
        }

        public static double pow(double a,double b){
            return System.Math.Pow(a,b);
        }

        public static int ParseInt(string x){
            return int.Parse(x);
        }

        public static float ParseFloat(string x){
            return float.Parse(x);
        }

        public static double ParseDouble(string x){
            return double.Parse(x);
        }

        public static long ParseLong(string x){
            return long.Parse(x);
        }

        public static float IntToFloat(int i){
            return (float)i;
        }

        public static double IntToDouble(int i){
            return (double)i;
        }

        public static long IntToLong(int i){
            return (long)i;
        }

        public static int FloatToInt(float i){
            return (int)i;
        }

        public static double FloatToDouble(float i){
            return (double)i;
        }

        public static long FloatToLong(float i){
            return (long)i;
        }

        public static int DoubleToInt(double i){
            return (int)i;
        }

        public static float DoubleToFloat(double i){
            return (float)i;
        }

        public static long DoubleToLong(double i){
            return (long)i;
        }
    


    }
}