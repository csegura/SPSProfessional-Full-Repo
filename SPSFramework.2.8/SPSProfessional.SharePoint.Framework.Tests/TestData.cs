using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace SPSProfessional.SharePoint.Framework.Tests
{
    public class TestData
    {
        public static IEnumerable<object[]> AllSystemTypes
        {
            get
            {
                yield return new object[]
                             {
                                     typeof(Boolean)
                             };
                yield return new object[]
                             {
                                     typeof(Byte)
                             };
                yield return new object[]
                             {
                                     typeof(Char)
                             };
                yield return new object[]
                             {
                                     typeof(DateTime)
                             };
                yield return new object[]
                             {
                                     typeof(Decimal)
                             };
                yield return new object[]
                             {
                                     typeof(Double)
                             };
                yield return new object[]
                             {
                                     typeof(Int16)
                             };
                yield return new object[]
                             {
                                     typeof(Int32)
                             };
                yield return new object[]
                             {
                                     typeof(Int64)
                             };
                yield return new object[]
                             {
                                     typeof(object)
                             };
                yield return new object[]
                             {
                                     typeof(SByte)
                             };
                yield return new object[]
                             {
                                     typeof(Single)
                             };
                yield return new object[]
                             {
                                     typeof(String)
                             };
                yield return new object[]
                             {
                                     typeof(UInt16)
                             };
                yield return new object[]
                             {
                                     typeof(UInt32)
                             };
                yield return new object[]
                             {
                                     typeof(UInt64)
                             };
                yield return new object[]
                             {
                                     typeof(Single)
                             };
                yield return new object[]
                             {
                                     typeof(Single)
                             };
            }
        }

        public static IEnumerable<object[]> AllSystemTypesDefaultValues1
        {
            get
            {
                yield return new object[]
                             {
                                     typeof(Boolean), false
                             };
                yield return new object[]
                             {
                                     typeof(Byte), Byte.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Char), Char.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(DateTime), DateTime.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Decimal), Decimal.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Double), Double.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Int16), Int16.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Int32), Int32.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Int64), Int64.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(SByte), SByte.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(Single), Single.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(String), string.Empty
                             };
                yield return new object[]
                             {
                                     typeof(UInt16), UInt16.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(UInt32), UInt32.MinValue
                             };
                yield return new object[]
                             {
                                     typeof(UInt64), UInt64.MinValue
                             };
                //yield return new object[] { typeof(object), DBNull.Value };
            }
        }

        public static IEnumerable<object[]> SqlDbTratedTypes
        {
            get
            {
                yield return new object[]
                             {
                                     SqlDbType.BigInt
                             };
                yield return new object[]
                             {
                                     SqlDbType.Binary
                             };
                yield return new object[]
                             {
                                     SqlDbType.VarBinary
                             };
                yield return new object[]
                             {
                                     SqlDbType.Image
                             };
                yield return new object[]
                             {
                                     SqlDbType.Timestamp
                             };
                yield return new object[]
                             {
                                     SqlDbType.Bit
                             };
                yield return new object[]
                             {
                                     SqlDbType.Char
                             };
                yield return new object[]
                             {
                                     SqlDbType.NChar
                             };
                yield return new object[]
                             {
                                     SqlDbType.NText
                             };
                yield return new object[]
                             {
                                     SqlDbType.NVarChar
                             };
                yield return new object[]
                             {
                                     SqlDbType.Text
                             };
                yield return new object[]
                             {
                                     SqlDbType.Xml
                             };
                yield return new object[]
                             {
                                     SqlDbType.DateTime
                             };
                yield return new object[]
                             {
                                     SqlDbType.SmallDateTime
                             };
                yield return new object[]
                             {
                                     SqlDbType.Decimal
                             };
                yield return new object[]
                             {
                                     SqlDbType.Money
                             };
                yield return new object[]
                             {
                                     SqlDbType.SmallMoney
                             };
                yield return new object[]
                             {
                                     SqlDbType.Float
                             };
                yield return new object[]
                             {
                                     SqlDbType.Int
                             };
                yield return new object[]
                             {
                                     SqlDbType.Real
                             };
                yield return new object[]
                             {
                                     SqlDbType.SmallInt
                             };
                yield return new object[]
                             {
                                     SqlDbType.TinyInt
                             };
                yield return new object[]
                             {
                                     SqlDbType.UniqueIdentifier
                             };
                yield return new object[]
                             {
                                     SqlDbType.Variant
                             };
            }
        }

        public static IEnumerable<object[]> SqlDbNoTratedTypes
        {
            get
            {
                yield return new object[]
                             {
                                     SqlDbType.Udt
                             };
                yield return new object[]
                             {
                                     SqlDbType.DateTime2
                             };
                yield return new object[]
                             {
                                     SqlDbType.DateTimeOffset
                             };
                yield return new object[]
                             {
                                     SqlDbType.Structured
                             };
            }
        }

        public static IEnumerable<object[]> SqlDbTratedTypesCastSystemTypes
        {
            get
            {
                yield return new object[]
                             {
                                     SqlDbType.BigInt, typeof(Int64)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Binary, typeof(Byte[])
                             };
                yield return new object[]
                             {
                                     SqlDbType.VarBinary, typeof(Byte[])
                             };
                yield return new object[]
                             {
                                     SqlDbType.Image, typeof(Byte[])
                             };
                yield return new object[]
                             {
                                     SqlDbType.Timestamp, typeof(Byte[])
                             };
                yield return new object[]
                             {
                                     SqlDbType.Bit, typeof(Boolean)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Char, typeof(string)
                             };
                yield return new object[]
                             {
                                     SqlDbType.NChar, typeof(string)
                             };
                yield return new object[]
                             {
                                     SqlDbType.NText, typeof(string)
                             };
                yield return new object[]
                             {
                                     SqlDbType.NVarChar, typeof(string)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Text, typeof(string)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Xml, typeof(string)
                             };
                yield return new object[]
                             {
                                     SqlDbType.DateTime, typeof(DateTime)
                             };
                yield return new object[]
                             {
                                     SqlDbType.SmallDateTime, typeof(DateTime)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Decimal, typeof(Decimal)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Money, typeof(Decimal)
                             };
                yield return new object[]
                             {
                                     SqlDbType.SmallMoney, typeof(Decimal)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Float, typeof(Double)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Int, typeof(Int32)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Real, typeof(Single)
                             };
                yield return new object[]
                             {
                                     SqlDbType.SmallInt, typeof(Int16)
                             };
                yield return new object[]
                             {
                                     SqlDbType.TinyInt, typeof(Byte)
                             };
                yield return new object[]
                             {
                                     SqlDbType.UniqueIdentifier, typeof(Guid)
                             };
                yield return new object[]
                             {
                                     SqlDbType.Variant, typeof(object)
                             };
            }
        }

        public static IEnumerable<object[]> EvaluatorFunctions
        {
            get
            {
                yield return new object[]
                             {
                                     "DateNow()",
                             };

                yield return new object[]
                             {
                                     "DateTimeNow()",
                             };
                yield return new object[]
                             {
                                     "MonthNumber()",
                             };
                yield return new object[]
                             {
                                     "DayNumber()",
                             };
                yield return new object[]
                             {
                                     "YearNumber()",
                             };
                yield return new object[]
                             {
                                     "Empty()",
                             };
                yield return new object[]
                             {
                                     "QueryStringNull('var',value)",
                             };
                yield return new object[]
                             {
                                     "QueryString('var')",
                             };
                yield return new object[]
                             {
                                     "DateFormat('s')",
                             };
                yield return new object[]
                             {
                                     "DateCalcFormat('a',M+1)",
                             };
                yield return new object[]
                             {
                                     "DateCalc(M+1)",
                             };               

            }
        }


        public static IDictionary<Type,object> AllSystemTypesDefaultValues()
        {
            Dictionary<Type,object> dictionary = new Dictionary<Type, object>();
            dictionary.Add(typeof(Boolean), false);
            dictionary.Add(typeof(Byte), Byte.MinValue);
            dictionary.Add(typeof(Char), Char.MinValue);
            dictionary.Add(typeof(DateTime), DateTime.MinValue);
            dictionary.Add(typeof(Decimal), Decimal.MinValue);
            dictionary.Add(typeof(Double), Double.MinValue);
            dictionary.Add(typeof(Int16), Int16.MinValue);
            dictionary.Add(typeof(Int32), Int32.MinValue);
            dictionary.Add(typeof(Int64), Int64.MinValue);
            dictionary.Add(typeof(SByte), SByte.MinValue);
            dictionary.Add(typeof(Single), Single.MinValue);
            dictionary.Add(typeof(String), string.Empty);
            dictionary.Add(typeof(UInt16), UInt16.MinValue);
            dictionary.Add(typeof(UInt32), UInt32.MinValue);
            dictionary.Add(typeof(UInt64), UInt64.MinValue);

            return dictionary;
        }

        public static string GetResource(string resourceName)
        {
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            TextReader textReader = null;
            if (assembly != null)
            {
                try
                {
                    resourceName = "SPSProfessional.SharePoint.Framework.Tests.Resources" + "." + resourceName;
                    textReader = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                }
                catch (ArgumentNullException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (BadImageFormatException)
                {
                }
                catch (FileLoadException)
                {
                }
                catch (FileNotFoundException)
                {
                }

                if (textReader != null)
                {
                    string result = textReader.ReadToEnd();
                    textReader.Close();

                    return result;
                }
            }
            return string.Empty;
        }

    }
}