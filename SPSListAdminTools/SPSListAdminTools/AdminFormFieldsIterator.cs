using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
    public class AdminFormFieldsIterator : ListFieldIterator
    {
        private string _show;
        private string _where;
        private string _condition;
        private string _group;

        protected override bool IsFieldExcluded(SPField field)
        {
            if (CheckIfFeatureIsEnabled())
            {
                string key = String.Format("DisplaySetting{0}", SPContext.Current.List.ID);
                
                if (SPContext.Current.List.ParentWeb.Properties.ContainsKey(key))
                {
                    var fieldProperties = SPSWebPropertiesHelper.Decode(
                        SPContext.Current.List.ParentWeb.Properties[key]);

                    string displaySettings;

                    if (!fieldProperties.ContainsKey(field.InternalName))
                        return base.IsFieldExcluded(field);

                    switch (SPContext.Current.FormContext.FormMode)
                    {
                        case SPControlMode.Display:
                            {
                                displaySettings = fieldProperties[field.InternalName]["Display"];

                                GetAttributes(displaySettings);

                                return NecesaryRender(field);
                            }
                        case SPControlMode.Edit:
                            {
                                displaySettings = fieldProperties[field.InternalName]["Edit"];

                                GetAttributes(displaySettings);

                                return NecesaryRender(field);
                            }

                        case SPControlMode.New:
                            {
                                if (!fieldProperties[field.InternalName].ContainsKey("New"))
                                    return base.IsFieldExcluded(field);

                                displaySettings = fieldProperties[field.InternalName]["New"];
                                GetAttributes(displaySettings);

                                return NecesaryRender(field);
                            }

                        default:
                            return base.IsFieldExcluded(field);
                    }
                }
                return base.IsFieldExcluded(field);
            }
            return base.IsFieldExcluded(field);
        }

        private bool NecesaryRender(SPField field)
        {
            if (_show == "always")
                return false;

            if (_show == "never")
                return true;

            return !RenderField(field, _where, _condition, _group);
        }

        private void GetAttributes(string displaySettings)
        {
            _show = displaySettings.Split(";".ToCharArray())[0];
            _where = displaySettings.Split(";".ToCharArray())[1];
            _condition = displaySettings.Split(";".ToCharArray())[2];
            _group = displaySettings.Split(";".ToCharArray())[3];
        }

        private bool RenderField(SPField field, IEquatable<string> where, IEquatable<string> condition, string group)
        {
            bool result = where.Equals("[Me]") ? WhereUser(condition, group) : WhereField(field);

            return result;
        }

        private static bool WhereUser(IEquatable<string> condition, string group)
        {
            SPUser user = SPContext.Current.Web.CurrentUser;
            SPGroupCollection userGroups = user.Groups;
            bool userInGroup = false;

            foreach (SPGroup groupItem in userGroups)
            {
                if (groupItem.Name.Equals(group))
                {
                    userInGroup = true;
                    continue;
                }
            }

            if (condition.Equals("IsInGroup"))
                return userInGroup;

            return !userInGroup;
        }

        private bool WhereField(SPField field)
        {
            return !base.IsFieldExcluded(field);
        }

        private static bool CheckIfFeatureIsEnabled()
        {
            SPFeature feature =
                SPContext.Current.Site.Features[new Guid("C509490D-0EAC-46E5-971B-F90093F9D182")];

            if ((feature != null) &&
                (feature.Definition.Status == SPObjectStatus.Online))
                return true;

            return false;
        }
    }

}