﻿using System.Text.RegularExpressions;

/*
 *  Forthy.Variant
 * 
 *  ADT for Forthy supported data types.
 * 
 */

public partial class Forthy
{
    public class Variant
    {
        public enum Type
        {
            Word,       //  Forthy Word
            Action,     //  Forthy Runtime Action
            Chunk,      //  Forthy Chunk
            Bool,       //  Boolean
            Integer,    //  Integer, eg: 42, 43i
            Float,      //  Float, eg: 12.3, 12.3f
            String,     //  String Literal
        }

        public Type type { get; private set; }

        private object _value;  // boxed object to store anything

        private T Cast<T>()
        {
            return (T)_value;
        }

        //  note that both Word/String use this to read string
        public string AsCSharpString
        {
            get
            {
                return Cast<string>();
            }
        }

        public bool AsBool
        {
            get
            {
                return Cast<bool>();
            }
        }

        public int AsInt
        {
            get
            {
                return Cast<int>();
            }
        }

        public float AsFloat
        {
            get
            {
                return Cast<float>();
            }
        }

        public RuntimeAction AsAction
        {
            get
            {
                return Cast<RuntimeAction>();
            }
        }

        public Chunk AsChunk
        {
            get
            {
                return Cast<Chunk>();
            }
        }

        public static Variant TRUE = new Variant() { type = Type.Bool, _value = true };
        public static Variant FALSE = new Variant() { type = Type.Bool, _value = false };

        public static Variant Make(int value)
        {
            return new Variant()
            {
                _value = value,
                type = Type.Integer,
            };
        }

        public static Variant Make(float value)
        {
            return new Variant()
            {
                _value = value,
                type = Type.Float,
            };
        }

        public static Variant Make(string value)
        {
            ForthyUtils.Assert(value != null, "cant make null string");
            return new Variant()
            {
                _value = value,
                type = Type.String,
            };
        }

        public static Variant Make(bool value)
        {
            return value ? TRUE : FALSE;
        }

        public static Variant Make(RuntimeAction value)
        {
            ForthyUtils.Assert(value != null, "cant make null action");
            return new Variant()
            {
                _value = value,
                type = Type.Action,
            };
        }

        public static Variant Make(Chunk value)
        {
            ForthyUtils.Assert(value != null, "cant make null chunk");
            return new Variant()
            {
                _value = value,
                type = Type.Chunk,
            };
        }

        //  this one uses a seperate name to diff against Make String 
        public static Variant MakeWord(string token)
        {
            ForthyUtils.Assert(token != null, "cant make null word");
            return new Variant()
            {
                _value = token,
                type = Type.Word,
            };
        }

        private static Regex INT_PATTERN = new Regex(@"^(\d+)i?$");
        private static Regex FLOAT_PATTERN = new Regex(@"^([\d\.]+)f?$");
        private static Regex STRING_PATTERN = new Regex("^[\"'](.+)[\"']$");

        public static bool TryParse(string token, out Variant variant)
        {
            if (token == "true")
            {
                variant = TRUE;
                return true;
            }
            else if (token == "false")
            {
                variant = FALSE;
                return true;
            }

            Match match;

            match = INT_PATTERN.Match(token);
            if (match.Success)
            {
                int value;
                bool success = int.TryParse(match.Groups[1].Value, out value);
                if (success)
                {
                    variant = new Variant()
                    {
                        type = Type.Integer,
                        _value = value,
                    };
                    return true;
                }
            }

            match = FLOAT_PATTERN.Match(token);
            if (match.Success)
            {
                float value;
                bool success = float.TryParse(match.Groups[1].Value, out value);
                if (success)
                {
                    variant = new Variant()
                    {
                        type = Type.Float,
                        _value = value,
                    };
                    return true;
                }
            }

            match = STRING_PATTERN.Match(token);
            if (match.Success)
            {
                variant = new Variant()
                {
                    type = Type.String,
                    _value = match.Groups[1].Value,
                };
                return true;
            }

            variant = null;
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Variant);
        }

        public bool Equals(Variant other)
        {
            if (other == null || type != other.type)
                return false;

            switch (type)
            {
                case Type.Bool:
                    return AsBool == other.AsBool;
                case Type.Integer:
                    return AsInt == other.AsInt;
                case Type.Float:
                    return AsFloat == other.AsFloat;
                case Type.String:
                case Type.Word:
                    return AsCSharpString == other.AsCSharpString;
                case Type.Action:
                    return AsAction == other.AsAction;
                default:
                    ForthyUtils.Error(string.Format("bad type for comparison: {0}", type));
                    break;
            }

            return false;
        }

        public static bool operator ==(Variant lhs, Variant rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;

            if (((object)lhs == null) || (object)rhs == null)
                return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Variant lhs, Variant rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }


        public static Variant operator +(Variant lhs, Variant rhs)
        {
            ForthyUtils.Assert(lhs != null && rhs != null, "can't add null variant");
            if (lhs.type == Type.Integer && rhs.type == Type.Integer)
            {
                return Make(lhs.AsInt + rhs.AsInt);
            }
            if (lhs.type == Type.Float || rhs.type == Type.Float)
            {
                float lhsValue = lhs.type == Type.Integer ? lhs.AsInt : lhs.AsFloat;
                float rhsValue = rhs.type == Type.Integer ? rhs.AsInt : rhs.AsFloat;
                return Make(lhsValue + rhsValue);
            }

            ForthyUtils.Error(string.Format("bad type for add: {0} + {1}", lhs.type, rhs.type));
            return null;
        }

        public static Variant operator -(Variant lhs, Variant rhs)
        {
            ForthyUtils.Assert(lhs != null && rhs != null, "can't minus null variant");
            if (lhs.type == Type.Integer && rhs.type == Type.Integer)
            {
                return Make(lhs.AsInt - rhs.AsInt);
            }
            if (lhs.type == Type.Float || rhs.type == Type.Float)
            {
                float lhsValue = lhs.type == Type.Integer ? lhs.AsInt : lhs.AsFloat;
                float rhsValue = rhs.type == Type.Integer ? rhs.AsInt : rhs.AsFloat;
                return Make(lhsValue - rhsValue);
            }

            ForthyUtils.Error(string.Format("bad type for minus: {0} - {1}", lhs.type, rhs.type));
            return null;
        }

        public static Variant operator *(Variant lhs, Variant rhs)
        {
            ForthyUtils.Assert(lhs != null && rhs != null, "can't mul null variant");
            if (lhs.type == Type.Integer && rhs.type == Type.Integer)
            {
                return Make(lhs.AsInt * rhs.AsInt);
            }
            if (lhs.type == Type.Float || rhs.type == Type.Float)
            {
                float lhsValue = lhs.type == Type.Integer ? lhs.AsInt : lhs.AsFloat;
                float rhsValue = rhs.type == Type.Integer ? rhs.AsInt : rhs.AsFloat;
                return Make(lhsValue * rhsValue);
            }

            ForthyUtils.Error(string.Format("bad type for mul: {0} * {1}", lhs.type, rhs.type));
            return null;
        }

        public static Variant operator /(Variant lhs, Variant rhs)
        {
            ForthyUtils.Assert(lhs != null && rhs != null, "can't div null variant");
            if (lhs.type == Type.Integer && rhs.type == Type.Integer)
            {
                return Make(lhs.AsInt / rhs.AsInt);
            }
            if (lhs.type == Type.Float || rhs.type == Type.Float)
            {
                float lhsValue = lhs.type == Type.Integer ? lhs.AsInt : lhs.AsFloat;
                float rhsValue = rhs.type == Type.Integer ? rhs.AsInt : rhs.AsFloat;
                return Make(lhsValue / rhsValue);
            }

            ForthyUtils.Error(string.Format("bad type for mul: {0} / {1}", lhs.type, rhs.type));
            return null;
        }

        public static bool operator >(Variant lhs, Variant rhs)
        {
            ForthyUtils.Assert(lhs != null && rhs != null, "can't gt null variant");
            if (lhs.type == Type.Integer && rhs.type == Type.Integer)
            {
                return lhs.AsInt > rhs.AsInt;
            }
            if (lhs.type == Type.Float || rhs.type == Type.Float)
            {
                float lhsValue = lhs.type == Type.Integer ? lhs.AsInt : lhs.AsFloat;
                float rhsValue = rhs.type == Type.Integer ? rhs.AsInt : rhs.AsFloat;
                return lhsValue > rhsValue;
            }

            ForthyUtils.Error(string.Format("bad type for gt: {0} > {1}", lhs.type, rhs.type));
            return false;
        }

        public static bool operator <(Variant lhs, Variant rhs)
        {
            ForthyUtils.Assert(lhs != null && rhs != null, "can't lt null variant");
            if (lhs.type == Type.Integer && rhs.type == Type.Integer)
            {
                return lhs.AsInt < rhs.AsInt;
            }
            if (lhs.type == Type.Float || rhs.type == Type.Float)
            {
                float lhsValue = lhs.type == Type.Integer ? lhs.AsInt : lhs.AsFloat;
                float rhsValue = rhs.type == Type.Integer ? rhs.AsInt : rhs.AsFloat;
                return lhsValue < rhsValue;
            }

            ForthyUtils.Error(string.Format("bad type for lt: {0} < {1}", lhs.type, rhs.type));
            return false;
        }

        public override string ToString()
        {
            switch (type)
            {
                case Type.Action:
                    return string.Format("[Action]{0}", _value.GetType().Name);
                case Type.Word:
                    return string.Format("[Word]{0}", _value.ToString());
                case Type.Chunk:
                    return string.Format("[Chunk]{0}", _value.ToString());
                case Type.Bool:
                    return AsBool == true ? "true" : "false";
                case Type.Integer:
                case Type.Float:
                    return _value.ToString();
                case Type.String:
                    return string.Format("{0}", _value.ToString());
                default:
                    ForthyUtils.Error(string.Format("unhandled type {0}", type));
                    return null;
            }
        }
    }
}
