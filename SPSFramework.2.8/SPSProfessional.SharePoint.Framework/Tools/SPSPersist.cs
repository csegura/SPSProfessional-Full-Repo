using System.Web.SessionState;
using System.Web.UI;
using SPSProfessional.SharePoint.Framework.Common;

namespace SPSProfessional.SharePoint.Framework.Tools
{
    public class SPSPersist
    {

        public static void TrySaveInSessionState<T>(Page page, StateBag alternate, string key, T value)
        {
            if (page.Session.Mode != SessionStateMode.Off)
            {                
                if (Equals(value, default(T)))
                {
                    page.Session[key] = null;
                }
                else
                {
                    page.Session[key] = SPSSerialization.Serialize(value);
                }
            }
            else
            {
                TrySaveInViewState(alternate, key, value);
            }
        }

        public static void TrySaveInSessionState<T>(Page page, Control control, StateBag alternate, string key, T value)
        {
            key += control.UniqueID;
            TrySaveInSessionState(page,alternate,key,value);
        }

        public static T TryGetFromSessionState<T>(Page page, StateBag alternate, string key, T value)
        {

            if (Equals(value, default(T)))
            {
                if (page.Session.Mode != SessionStateMode.Off)
                {
                    if (page.Session[key] != null)
                    {
                        value = (T) SPSSerialization.Deserialize((string) page.Session[key]);
                    }
                }
                else
                {
                    value = TryGetFromViewState(alternate, key, value);
                }
            }
          
            return value;
        }

        public static T TryGetFromSessionState<T>(Page page, StateBag alternate, string key)
        {
            T value = default(T);
            value = TryGetFromSessionState(page, alternate, key, value);
            return value;
        }

        public static T TryGetFromSessionState<T>(Page page, Control control, StateBag alternate, string key)
        {
            T value = default(T);
            key += control.UniqueID;
            value = TryGetFromSessionState(page, alternate, key, value);
            return value;
        }

        public static T TryGetFromSessionState<T>(Page page, Control control, StateBag alternate, string key, T value)
        {
            key += control.UniqueID;
            value = TryGetFromSessionState(page, alternate, key, value);
            return value;
        }

        public static void TrySaveInViewState<T>(StateBag stateBag, string key, T value)
        {
            if (Equals(value, default(T)))
            {
                stateBag[key] = null;
            }
            else
            {
                stateBag[key] = SPSSerialization.Serialize(value);
            }
        }

        public static T TryGetFromViewState<T>(StateBag alternate, string key, T value)
        {
            if (Equals(value, default(T)))
            {
                if (alternate[key] != null)
                {
                    value = (T) SPSSerialization.Deserialize((string) alternate[key]);
                }
            }
            return value;
        }
    }
}
