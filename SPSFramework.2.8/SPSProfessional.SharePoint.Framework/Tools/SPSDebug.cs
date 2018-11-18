// File : SPSDebug.cs
// Date : 24/06/2008
// User : csegura
// Logs

using System;
using System.Diagnostics;
using System.Reflection;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    public static class SPSDebug
    {
        [Conditional("DEBUG")]
        public static void DumpException(string name, Exception ex)
        {
            Debug.WriteLine("PANIC:" + ex.TargetSite);
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }

        [Conditional("DEBUG")]
        public static void DumpException(Exception ex)
        {
            Debug.WriteLine("PANIC:"+ex.TargetSite);
            Debug.WriteLine(ex);
        }

        [Conditional("DEBUG")]
        public static void DumpObject(Object anyObject)
        {
            Type type = anyObject.GetType();

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetIndexParameters().Length == 0)
                {
                    Debug.WriteLine(string.Format("{0} = {1}", 
                        propertyInfo.Name, 
                        propertyInfo.GetValue(anyObject, null)));
                }
            }
        }
    }
}
