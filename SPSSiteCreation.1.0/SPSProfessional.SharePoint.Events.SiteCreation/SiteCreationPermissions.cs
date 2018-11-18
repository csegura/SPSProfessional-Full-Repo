using Microsoft.SharePoint;

namespace SPSProfessional.SharePoint.Events.SiteCreation
{
    public static class SiteCreationPermissions
    {
        private const string ASSOCIATE_OWNER_GROUP = "vti_associateownergroup";

        public enum DefaultGroups
        {
            Owners,
            Members,
            Vistors
        } ;

        public static void AddDefaultGroups(SPWeb spWeb, bool copyUsersFromParent)
        {
            spWeb.BreakRoleInheritance(false);

            SPGroup owners = AddGroup(spWeb, DefaultGroups.Owners, copyUsersFromParent);
            SPGroup members = AddGroup(spWeb, DefaultGroups.Members, copyUsersFromParent);
            SPGroup vistors = AddGroup(spWeb, DefaultGroups.Vistors, copyUsersFromParent);

            //SPGroup myGroup = AddGroup(spWeb, "My Group", "An example group.", null, null, null, false);

            SetAssociatedGroups(spWeb,
                                new[]
                                    {
                                        owners, members, vistors
                                    });
        }

        public static void SetAssociatedGroups(SPWeb spWeb, SPGroup[] groups)
        {
            string formatString = string.Empty;

            object[] ids = new object[groups.Length];

            for (int i = 0; i < groups.Length; i++)
            {
                formatString += string.Format("{{{0}}};", i);
                ids[i] = groups[i].ID;
            }

            spWeb.Properties["vti_associategroups"] = string.Format(formatString.TrimEnd(new[]
                                                                                             {
                                                                                                 ';'
                                                                                             }),
                                                                    ids);
            spWeb.Properties.Update();
        }

        public static SPGroup AddGroup(SPWeb web, DefaultGroups associateGroup, bool copyUsersFromParent)
        {
            return AddGroupInternal(associateGroup, web, copyUsersFromParent);
        }

        private static SPGroup AddGroupInternal(DefaultGroups associateGroup, SPWeb web, bool copyUsersFromParent)
        {
            switch (associateGroup)
            {
                case DefaultGroups.Owners:
                    return AddGroup(web,
                                    web.Name + " Owners",
                                    "Use this group to give people full control permissions to the SharePoint site: {0}",
                                    "Full Control",
                                    ASSOCIATE_OWNER_GROUP,
                                    web.ParentWeb.AssociatedOwnerGroup,
                                    copyUsersFromParent);
                case DefaultGroups.Members:
                    return AddGroup(web,
                                    web.Name + " Members",
                                    "Use this group to give people contribute permissions to the SharePoint site: {0}",
                                    "Contribute",
                                    "vti_associatemembergroup",
                                    web.ParentWeb.AssociatedMemberGroup,
                                    copyUsersFromParent);
                case DefaultGroups.Vistors:
                    return AddGroup(web,
                                    web.Name + " Vistors",
                                    "Use this group to give people read permissions to the SharePoint site: {0}",
                                    "Read",
                                    "vti_associatevisitorgroup",
                                    web.ParentWeb.AssociatedVisitorGroup,
                                    copyUsersFromParent);
                default:
                    return null;
            }
        }

        public static SPGroup AddGroup(SPWeb web,
                                       string groupName,
                                       string descriptionFormatString,
                                       string roleDefinitionName,
                                       string associatedGroupName,
                                       SPGroup parentAssociatedGroup,
                                       bool copyUsersFromParent)
        {
            return AddGroupInternal(web,
                                    groupName,
                                    descriptionFormatString,
                                    roleDefinitionName,
                                    associatedGroupName,
                                    parentAssociatedGroup,
                                    copyUsersFromParent);
        }

        private static SPGroup AddGroupInternal(SPWeb web,
                                                string groupName,
                                                string descriptionFormatString,
                                                string roleDefinitionName,
                                                string associatedGroupName,
                                                SPGroup parentAssociatedGroup,
                                                bool copyUsersFromParent)
        {
            SPGroup owner = parentAssociatedGroup;

            if (associatedGroupName != ASSOCIATE_OWNER_GROUP)
            {
                owner = web.SiteGroups.GetByID(int.Parse(web.Properties[ASSOCIATE_OWNER_GROUP]));
            }

            web.SiteGroups.Add(groupName, owner, null, string.Format(descriptionFormatString, web.Name));

            SPGroup group = web.SiteGroups[groupName];

            if (descriptionFormatString.IndexOf("{0}") != -1)
            {
                SPListItem item = web.SiteUserInfoList.GetItemById(group.ID);

                item["Notes"] = string.Format(descriptionFormatString,
                                              string.Format("<a href=\"{0}\">{1}</a>", web.Url, web.Name));
                item.Update();
            }

            if (roleDefinitionName != null)
            {
                SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
                SPRoleDefinition roleDefinition = web.RoleDefinitions[roleDefinitionName];
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                web.RoleAssignments.Add(roleAssignment);
            }

            if (copyUsersFromParent && parentAssociatedGroup != null)
            {
                foreach (SPUser user in parentAssociatedGroup.Users)
                {
                    group.AddUser(user);
                }
            }

            if (associatedGroupName != null)
            {
                web.Properties[associatedGroupName] = group.ID.ToString();
                web.Properties.Update();
            }

            web.Update();
            return group;
        }
    }
}