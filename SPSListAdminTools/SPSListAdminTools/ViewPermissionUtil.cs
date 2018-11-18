using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
    public class ViewPermissionUtil
    {
        public static void ConvertFromString(ref Dictionary<int, Dictionary<Guid, bool>> roleProperties, ref Dictionary<int, Guid> defaultViews, string value, SPList currentList)
        {
            string[] groups = value.Split("|".ToCharArray());
            var groupValues = new Dictionary<int, string>();

            foreach (string group in groups)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    string[] values = group.Split("#".ToCharArray());
                    int groupId = int.Parse(values[0]);
                    groupValues.Add(groupId, group);
                }
            }

            foreach (SPGroup group in currentList.ParentWeb.Groups)
            {
                roleProperties.Add(group.ID, new Dictionary<Guid, bool>());
                Guid defaultViewId = GetDefautView(groupValues, group.ID);

                SetDefaultVue(defaultViewId, currentList.DefaultView.ID, group.ID, currentList, ref defaultViews);

                foreach (SPView view in currentList.Views)
                {
                    if ((!view.Hidden) && (!view.PersonalView))
                        roleProperties[group.ID].Add(view.ID, IsViewAllowed(groupValues, group.ID, view.ID));
                    else
                    {
                        if(view.PersonalView)
                            roleProperties[group.ID].Add(view.ID, true);
                    }
                }
            }
        }

        private static void SetDefaultVue(Guid defaultUserView, Guid listDefaultVue, int groupId, SPList currentList, ref Dictionary<int, Guid> defaultViews)
        {
            try
            {
                if ((defaultUserView != Guid.Empty) && (currentList.Views[defaultUserView] != null))
                    defaultViews.Add(groupId, defaultUserView);
                else
                    defaultViews.Add(groupId, listDefaultVue);
            }
            catch (Exception)
            {
                defaultViews.Add(groupId, listDefaultVue);
            }
        }

        public static void ConvertFromStringForPage(ref Dictionary<int, Dictionary<Guid, bool>> roleProperties, ref Dictionary<int, Guid> defaultViews, string value, SPList currentList)
        {
            string[] groups = value.Split("|".ToCharArray());
            var groupValues = new Dictionary<int, string>();

            foreach (string group in groups)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    string[] values = group.Split("#".ToCharArray());
                    int groupId = int.Parse(values[0]);
                    groupValues.Add(groupId, group);
                }
            }

            foreach (SPGroup group in currentList.ParentWeb.Groups)
            {
                roleProperties.Add(group.ID, new Dictionary<Guid, bool>());
                Guid defaultViewId = GetDefautView(groupValues, group.ID);

                SetDefaultVue(defaultViewId, currentList.DefaultView.ID, group.ID, currentList, ref defaultViews);

                foreach (SPView view in currentList.Views)
                {
                    if ((!view.Hidden) && (!view.PersonalView))
                        roleProperties[group.ID].Add(view.ID, IsViewAllowed(groupValues, group.ID, view.ID));
                }

            }
        }

        private static Guid GetDefautView(IDictionary<int, string> groupValues, int groupId)
        {
            if (groupValues.ContainsKey(groupId))
            {
                string[] values = groupValues[groupId].Split("#".ToCharArray());
                return new Guid(values[1]);
            }
            
            return Guid.Empty;
        }

        private static bool IsViewAllowed(IDictionary<int, string> groupValues, int groupId, Guid viewToTest)
        {
            if (groupValues.ContainsKey(groupId))
            {
                string[] values = groupValues[groupId].Split("#".ToCharArray());
                return values[2].Contains(viewToTest.ToString("D"));
            }
            
            return true;
        }
    }
}