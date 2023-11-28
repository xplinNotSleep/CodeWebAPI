//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//namespace AgCIM.Tools.Maths
//{

//    public class NumberBase<T> where T : struct, IEquatable<T>, IFormattable
//    {
//        public delegate TResult ValueSet<in TIn, out TResult>(TIn arg);
//        public delegate TResult ValueConvert<in TIn, out TResult>(TIn arg);
//        public delegate TResult ValueAggregate<in TIn, out TResult>(params TIn[] arg);
//        public virtual ValueSet<T, T> Set { get; }
//        public virtual Func<T, T, T> Add { get; }
//        public virtual ValueAggregate<T, T> AddMany { get; }
//        public virtual Func<T, T, T> Minus { get; }
//        public virtual Func<T, T, T> Multiply { get; }
//        public virtual ValueAggregate<T, T> MultiplyMany { get; }
//        public virtual Func<T, T, T> Divide { get; }
//        public virtual Func<T, T> Negate { get; }
//        public virtual T One { get; }
//        public virtual T Zero { get; }
//        public virtual MathBase<T> Math
//        {
//            get
//            {
//                var typeT = typeof(T);
//                MathBase<T> _math = null;
//                if (typeT == typeof(float))
//                {
//                    _math =new FloatMath() as MathBase<T>;
//                }
//                else if (typeT == typeof(double))
//                {
//                    _math =new DoubleMath() as MathBase<T>;
//                }
//                else
//                {
//                    _math = new IntMath() as MathBase<T>;
//                }
//                return _math;
//            }
//        }
//        public virtual ValueConvert<string, T> Parse { get; }
//        public virtual ValueConvert<T, float> ToFloat { get; }
//        public virtual ValueConvert<T, double> ToDouble { get; }
//        public virtual ValueConvert<dynamic, T> ToCurrent { get; }
//        public virtual ValueConvert<T, byte[]> ToBytes { get; }
//        public virtual Func<T, T, bool> IsEquals { get; }
//        public virtual Func<T, T, bool> IsGreater { get; }
//        public virtual Func<T, T, bool> IsLess { get; }
//        public virtual Func<T, T, bool> IsGreaterEquals { get; }
//        public virtual Func<T, T, bool> IsLessEquals { get; }
//        public static NumberBase<T> Num
//        {
//            get
//            {
//                var typeT = typeof(T);
//                NumberBase<T> _num = null;
//                if (typeT == typeof(int))
//                {
//                    _num = new IntSelt() as NumberBase<T>;
//                }
//                else if (typeT == typeof(float))
//                {
//                    _num = new FloatSelt() as NumberBase<T>;
//                }
//                else
//                {
//                    _num = new DoubleSelt() as NumberBase<T>;
//                }
//                return _num;
//            }
//        }
//    }

//    public class FloatSelt : NumberBase<float>
//    {
//        public override ValueSet<float, float> Set => a => a;
//        public override Func<float, float, float> Add { get => (p1, p2) => p1 + p2; }
//        public override ValueAggregate<float, float> AddMany { get => p => p.Sum(p1 => p1); }
//        public override Func<float, float, float> Minus { get => (p1, p2) => p1 - p2; }
//        public override Func<float, float, float> Multiply { get => (p1, p2) => p1 * p2; }
//        public override ValueAggregate<float, float> MultiplyMany { get => p => p.Aggregate((p1, p2) => p1 * p2); }
//        public override Func<float, float, float> Divide { get => (p1, p2) => p1 / p2; }
//        public override Func<float, float> Negate { get => p => -(float)p; }
//        public override float One { get => 1; }
//        public override float Zero { get => 0; }
//        public override ValueConvert<string, float> Parse { get => p => float.Parse(p); }
//        public override ValueConvert<float, float> ToFloat { get => p => p; }
//        public override ValueConvert<float, double> ToDouble { get => p => (double)p; }
//        public override ValueConvert<dynamic, float> ToCurrent { get => p => (float)p; }
//        public override ValueConvert<float, byte[]> ToBytes { get => p => BitConverter.GetBytes(p); }
//        public override Func<float, float, bool> IsEquals { get => (p1, p2) => Math.Abs(p1 - p2) < 1e-8; }
//        public override Func<float, float, bool> IsGreater { get => (p1, p2) => p1 > p2; }
//        public override Func<float, float, bool> IsLess { get => (p1, p2) => p1 < p2; }
//        public override Func<float, float, bool> IsGreaterEquals { get => (p1, p2) => p1 >= p2; }
//        public override Func<float, float, bool> IsLessEquals { get => (p1, p2) => p1 <= p2; }
//    }

//    public class DoubleSelt : NumberBase<double>
//    {
//        public override ValueSet<double, double> Set => a => a;
//        public override Func<double, double, double> Add { get => (p1, p2) => p1 + p2; }
//        public override ValueAggregate<double, double> AddMany { get => p => p.Sum(p1 => p1); }

//        public override Func<double, double, double> Minus { get => (p1, p2) => p1 - p2; }

//        public override Func<double, double, double> Multiply { get => (p1, p2) => p1 * p2; }
//        public override ValueAggregate<double, double> MultiplyMany { get => p => p.Aggregate((p1, p2) => p1 * p2); }

//        public override Func<double, double, double> Divide { get => (p1, p2) => p1 / p2; }
//        public override Func<double, double> Negate { get => p => -p; }
//        public override double One { get => 1; }
//        public override double Zero { get => 0; }
//        public override ValueConvert<string, double> Parse { get => p => double.Parse(p); }
//        public override ValueConvert<double, float> ToFloat { get => p => (float)p; }
//        public override ValueConvert<double, double> ToDouble { get => p => p; }
//        public override ValueConvert<dynamic, double> ToCurrent { get => p => (double)p; }
//        public override ValueConvert<double, byte[]> ToBytes { get => p => BitConverter.GetBytes(p); }
//        public override Func<double, double, bool> IsEquals { get => (p1, p2) => Math.Abs(p1 - p2) < 1e-15; }
//        public override Func<double, double, bool> IsGreater { get => (p1, p2) => p1 > p2; }
//        public override Func<double, double, bool> IsLess { get => (p1, p2) => p1 < p2; }
//        public override Func<double, double, bool> IsGreaterEquals { get => (p1, p2) => p1 >= p2; }
//        public override Func<double, double, bool> IsLessEquals { get => (p1, p2) => p1 <= p2; }
//    }

//    public class IntSelt : NumberBase<int>
//    {
//        public override ValueSet<int, int> Set => a => a;
//        public override Func<int, int, int> Add { get => (p1, p2) => p1 + p2; }
//        public override ValueAggregate<int, int> AddMany { get => p => p.Sum(p1 => p1); }

//        public override Func<int, int, int> Minus { get => (p1, p2) => p1 - p2; }

//        public override Func<int, int, int> Multiply { get => (p1, p2) => p1 * p2; }
//        public override ValueAggregate<int, int> MultiplyMany { get => p => p.Aggregate((p1, p2) => p1 * p2); }

//        public override Func<int, int, int> Divide { get => (p1, p2) => p1 / p2; }
//        public override Func<int, int> Negate { get => p => -p; }
//        public override int One { get => 1; }
//        public override int Zero { get => 0; }
//        public override Func<int, int, bool> IsEquals { get => (p1, p2) => p1 == p2; }
//        public override ValueConvert<string, int> Parse { get => p => int.Parse(p); }
//        public override ValueConvert<int, float> ToFloat { get => p => (float)p; }
//        public override ValueConvert<int, double> ToDouble { get => p => (double)p; }
//        public override ValueConvert<int, byte[]> ToBytes { get => p => BitConverter.GetBytes(p); }
//        public override Func<int, int, bool> IsGreater { get => (p1, p2) => p1 > p2; }
//        public override Func<int, int, bool> IsLess { get => (p1, p2) => p1 < p2; }
//    }

//    public class MathBase<T> where T : struct, IEquatable<T>, IFormattable
//    {
//        public virtual T PI { get; }
//        public virtual Func<T, T> Abs { get; }
//        public virtual Func<T, T> Acos { get; }
//        public virtual Func<T, T> Asin { get; }
//        public virtual Func<T, T> Atan { get; }
//        public virtual Func<T, T, T> Atan2 { get; }
//        public virtual Func<T, T> Ceiling { get; }
//        public virtual Func<T, T> Cos { get; }
//        public virtual Func<T, T> Cosh { get; }
//        public virtual Func<T, T> Exp { get; }
//        public virtual Func<T, T> Floor { get; }
//        public virtual Func<T, T, T> IEEERemainder { get; }
//        public virtual Func<T, T> Log { get; }
//        public virtual Func<T, T, T> Log2 { get; }
//        public virtual Func<T, T> Log10 { get; }
//        public virtual Func<T, T, T> Max { get; }
//        public virtual Func<T, T, T> Min { get; }
//        public virtual Func<T, T, T> Pow { get; }
//        public virtual Func<T, MidpointRounding, T> Round3 { get; }
//        public virtual Func<T, int, MidpointRounding, T> Round4 { get; }
//        public virtual Func<T, int, T> Round2 { get; }
//        public virtual Func<T, T> Round { get; }
//        public virtual Func<T, int> Sign { get; }
//        public virtual Func<T, T> Sin { get; }
//        public virtual Func<T, T> Sinh { get; }
//        public virtual Func<T, T> Sqrt { get; }
//        public virtual Func<T, T> Tan { get; }
//        public virtual Func<T, T> Tanh { get; }
//        public virtual Func<T, T> Truncate { get; }
//        /// <summary>
//        /// 通过加上适当的2pi倍数，将角度限制在-PI到PI之间
//        /// </summary>
//        public virtual Func<T, T> WrapPi { get; }
//        /// <summary>
//        /// 和acos(x)相同，但如果x超出范围将返回最为接近的有效值
//        /// </summary>
//        public virtual Func<T, T> SafeAcos { get; }
//    }

//    public class FloatMath : MathBase<float>
//    {
//        public override float PI { get => (float)Math.PI; }
//        public override Func<float, float> Abs { get => p => Math.Abs(p); }
//        public override Func<float, float> Acos { get => p => (float)Math.Acos((double)p); }
//        public override Func<float, float> Asin { get => p => (float)Math.Asin((double)p); }
//        public override Func<float, float> Atan { get => p => (float)Math.Asin((double)p); }
//        public override Func<float, float, float> Atan2 { get => (p1, p2) => (float)Math.Atan2((double)p1, (double)p2); }
//        public override Func<float, float> Ceiling { get => p => (float)Math.Ceiling((double)p); }
//        public override Func<float, float> Cos { get => p => (float)Math.Cos((double)p); }
//        public override Func<float, float> Cosh { get => p => (float)Math.Cosh((double)p); }
//        public override Func<float, float> Exp { get => p => (float)Math.Exp((double)p); }
//        public override Func<float, float> Floor { get => p => (float)Math.Floor((double)p); }
//        public override Func<float, float, float> IEEERemainder { get => (p1, p2) => (float)Math.IEEERemainder((double)p1, (double)p2); }
//        public override Func<float, float> Log { get => p => (float)Math.Log((double)p); }
//        public override Func<float, float, float> Log2 { get => (p1, p2) => (float)Math.Log((double)p1, (double)p2); }
//        public override Func<float, float> Log10 { get => p => (float)Math.Log10((double)p); }
//        public override Func<float, float, float> Max { get => (p1, p2) => Math.Max(p1, p2); }
//        public override Func<float, float, float> Min { get => (p1, p2) => (float)Math.Min((double)p1, (double)p2); }
//        public override Func<float, float, float> Pow { get => (p1, p2) => (float)Math.Pow((double)p1, (double)p2); }
//        public override Func<float, MidpointRounding, float> Round3 { get => (p1, p2) => (float)Math.Round((double)p1, p2); }
//        public override Func<float, int, MidpointRounding, float> Round4 { get => (p1, p2, p3) => (float)Math.Round((double)p1, p2, p3); }
//        public override Func<float, int, float> Round2 { get => (p1, p2) => (float)Math.Round((double)p1, p2); }
//        public override Func<float, float> Round { get => p => (float)Math.Round((double)p); }
//        public override Func<float, int> Sign { get => p => Math.Sign(p); }
//        public override Func<float, float> Sin { get => p => (float)Math.Sin((double)p); }
//        public override Func<float, float> Sinh { get => p => (float)Math.Sinh((double)p); }
//        public override Func<float, float> Sqrt { get => p => (float)Math.Sqrt((double)p); }
//        public override Func<float, float> Tan { get => p => (float)Math.Tan((double)p); }
//        public override Func<float, float> Tanh { get => p => (float)Math.Tanh((double)p); }
//        public override Func<float, float> Truncate { get => p => (float)Math.Truncate((double)p); }
//        /// <summary>
//        /// 通过加上适当的2pi倍数，将角度限制在-PI到PI之间
//        /// </summary>
//        public override Func<float, float> WrapPi
//        {
//            get => p =>
//        {
//            p += (float)Math.PI;
//            p -= (float)Math.Floor((double)p * 1.0 / Math.PI) * (float)Math.PI * 2.0f;
//            p -= (float)Math.PI;
//            return p;
//        };
//        }
//        /// <summary>
//        /// 和acos(x)相同，但如果x超出范围将返回最为接近的有效值
//        /// </summary>
//        public override Func<float, float> SafeAcos
//        {
//            get => x =>
//        {
//            //检查边界条件
//            if (x <= -1.0f)
//            {
//                return (float)Math.PI;
//            }
//            if (x >= 1.0f)
//            {
//                return 0.0f;
//            }
//            //使用标准的反余弦函数
//            return (float)Math.Acos((double)x);
//        };
//        }
//    }

//    public class DoubleMath : MathBase<double>
//    {
//        public override double PI { get => Math.PI; }
//        public override Func<double, double> Abs { get => p => Math.Abs(p); }
//        public override Func<double, double> Acos { get => p => Math.Acos(p); }
//        public override Func<double, double> Asin { get => p => Math.Asin(p); }
//        public override Func<double, double> Atan { get => p => Math.Asin(p); }
//        public override Func<double, double, double> Atan2 { get => (p1, p2) => Math.Atan2(p1, p2); }
//        public override Func<double, double> Ceiling { get => p => Math.Ceiling(p); }
//        public override Func<double, double> Cos { get => p => Math.Cos(p); }
//        public override Func<double, double> Cosh { get => p => Math.Cosh(p); }
//        public override Func<double, double> Exp { get => p => Math.Exp(p); }
//        public override Func<double, double> Floor { get => p => Math.Floor(p); }
//        public override Func<double, double, double> IEEERemainder { get => (p1, p2) => Math.IEEERemainder(p1, p2); }
//        public override Func<double, double> Log { get => p => Math.Log(p); }
//        public override Func<double, double, double> Log2 { get => (p1, p2) => Math.Log(p1, p2); }
//        public override Func<double, double> Log10 { get => p => Math.Log10(p); }
//        public override Func<double, double, double> Max { get => (p1, p2) => Math.Max(p1, p2); }
//        public override Func<double, double, double> Min { get => (p1, p2) => Math.Min(p1, p2); }
//        public override Func<double, double, double> Pow { get => (p1, p2) => Math.Pow(p1, p2); }
//        public override Func<double, MidpointRounding, double> Round3 { get => (p1, p2) => Math.Round(p1, p2); }
//        public override Func<double, int, MidpointRounding, double> Round4 { get => (p1, p2, p3) => Math.Round(p1, p2, p3); }
//        public override Func<double, int, double> Round2 { get => (p1, p2) => Math.Round(p1, p2); }
//        public override Func<double, double> Round { get => p => Math.Round(p); }
//        public override Func<double, int> Sign { get => p => Math.Sign(p); }
//        public override Func<double, double> Sin { get => p => Math.Sin(p); }
//        public override Func<double, double> Sinh { get => p => Math.Sinh(p); }
//        public override Func<double, double> Sqrt { get => p => Math.Sqrt(p); }
//        public override Func<double, double> Tan { get => p => Math.Tan(p); }
//        public override Func<double, double> Tanh { get => p => Math.Tanh(p); }
//        public override Func<double, double> Truncate { get => p => Math.Truncate(p); }
//        /// <summary>
//        /// 通过加上适当的2pi倍数，将角度限制在-PI到PI之间
//        /// </summary>
//        public override Func<double, double> WrapPi
//        {
//            get => p =>
//            {
//                p += Math.PI;
//                p -= Math.Floor(p * 1.0 / Math.PI) * Math.PI * 2.0;
//                p -= Math.PI;
//                return p;
//            };
//        }
//        /// <summary>
//        /// 和acos(x)相同，但如果x超出范围将返回最为接近的有效值
//        /// </summary>
//        public override Func<double, double> SafeAcos
//        {
//            get => x =>
//            {
//                    //检查边界条件
//                if (x <= -1.0)
//                {
//                    return Math.PI;
//                }
//                if (x >= 1.0)
//                {
//                    return 0.0;
//                }
//                    //使用标准的反余弦函数
//                return Math.Acos(x);
//            };
//        }
//    }

//    public class IntMath : MathBase<int>
//    {
//        public override Func<int, int, int> Max { get => (p1, p2) => Math.Max(p1, p2); }
//        public override Func<int, int, int> Min { get => (p1, p2) => Math.Min(p1, p2); }
//        public override Func<int, int> Sign { get => p => Math.Sign(p); }
//    }
//}
