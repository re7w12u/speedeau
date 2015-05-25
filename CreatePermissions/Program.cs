using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatePermissions
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();

            string url = "http://rdits-sp13-dev/sites/rnvo";
            using (SPSite site = new SPSite(url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    Dictionary<string, SPGroup> groups = p.CreateAllGroups(web);
                    Dictionary<string, SPRoleDefinition> roles = p.CreateAllPermissions(web);

                    p.CreateQuickLaunch(web);
                }
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        #region permissions

        #region Create Groups
        private Dictionary<string, SPGroup> CreateAllGroups(SPWeb web)
        {
            string defaultUser = @"eur\sesa260501";
            SPUser defaultOwner = web.EnsureUser(defaultUser);
            Dictionary<string, SPGroup> result = new Dictionary<string, SPGroup>();

            SPDOGroup g1 = new SPDOGroup
            {
                GroupName = "EDF Administrateurs Fonctionnels",
                Description = "Administrateurs fonctionnels du site projet / équipe 'Modele': Agents EDF chargé de l'administration des données métier du site: référentiel, attribution de droits d'accès, etc",
            };
            result.Add(g1.GroupName, CreateNewGroup(web, g1, defaultOwner, defaultOwner));


            SPDOGroup g2 = new SPDOGroup
            {
                GroupName = "EDF Contributeurs Référentiel",
                Description = "Contributeurs du site projet / équipe. Agents EDF ayant la possibilité d'effectuer des publications et mises à jour diverses : documents, fonctions collaboratives, éléments de listes, etc.",
            };
            result.Add(g2.GroupName, CreateNewGroup(web, g2, defaultOwner, result[g1.GroupName]));

            SPDOGroup g3 = new SPDOGroup
            {
                GroupName = "EDF Contributeurs Déploiement",
                Description = "Contributeurs du site projet / équipe. Agents EDF ayant la possibilité d'effectuer des publications et mises à jour diverses : documents, fonctions collaboratives, éléments de listes, etc.",
            };
            result.Add(g3.GroupName, CreateNewGroup(web, g3, defaultOwner, result[g1.GroupName]));

            SPDOGroup g4 = new SPDOGroup
            {
                GroupName = "EDF Utilisateurs",
                Description = "Visiteurs du site dans l'espace interne EDF: Agents EDF ayant un accès en lecture seule au niveau du projet.",
            };
            result.Add(g4.GroupName, CreateNewGroup(web, g4, defaultOwner, result[g1.GroupName]));

            SPDOGroup g5 = new SPDOGroup
            {
                GroupName = "EDF Administrateurs Site",
                Description = "Administrateurs fonctionnels du site projet / équipe 'Modele': Agents EDF chargé de l'administration des données métier du site: référentiel, attribution de droits d'accès, etc",
            };
            result.Add(g5.GroupName, CreateNewGroup(web, g1, defaultOwner, defaultOwner));


            return result;
        }


        private SPGroup CreateNewGroup(SPWeb web, SPDOGroup group, SPUser defaultUser, SPMember groupOwner)
        {
            web.AllowUnsafeUpdates = true;
            string groupName = group.GroupName;
            web.Groups.Add(groupName, groupOwner, defaultUser, group.Description);
            web.AllowUnsafeUpdates = false;
            return web.Groups[groupName];
        }
        #endregion

        #region Create Permissions level
        private Dictionary<string, SPRoleDefinition> CreateAllPermissions(SPWeb web)
        {
            Dictionary<string, SPRoleDefinition> result = new Dictionary<string, SPRoleDefinition>();

            SPRoleDefinition reader = web.RoleDefinitions.GetByType(SPRoleType.Reader);
            result.Add(reader.Name, reader);

            SPDOPermission ps1 = new SPDOPermission
            {
                RoleName = "Administration EDF",
                RoleDescription = "Afficher, ajouter, mettre à jour et supprimer des éléments de liste et des documents",
                Permissions = SPBasePermissions.AddListItems | SPBasePermissions.EditListItems | SPBasePermissions.DeleteListItems | SPBasePermissions.ViewListItems |
                              SPBasePermissions.OpenItems | SPBasePermissions.ViewVersions | SPBasePermissions.CreateAlerts | SPBasePermissions.ViewFormPages | SPBasePermissions.ViewPages |
                              SPBasePermissions.BrowseUserInfo | SPBasePermissions.UseRemoteAPIs | SPBasePermissions.UseClientIntegration | SPBasePermissions.Open | SPBasePermissions.EditMyUserInfo
            };
            result.Add(ps1.RoleName, CreateRole(web, ps1));

            SPDOPermission ps2 = new SPDOPermission
            {
                RoleName = "Contribution EDF",
                RoleDescription = "Modification du niveau Collaboration pour les contributeurs EDF.",
                Permissions = SPBasePermissions.AddListItems | SPBasePermissions.EditListItems | SPBasePermissions.DeleteListItems | SPBasePermissions.ViewListItems |
                                     SPBasePermissions.OpenItems | SPBasePermissions.ViewVersions | SPBasePermissions.CreateAlerts | SPBasePermissions.ViewFormPages | SPBasePermissions.ViewPages |
                                     SPBasePermissions.BrowseUserInfo | SPBasePermissions.UseRemoteAPIs | SPBasePermissions.UseClientIntegration | SPBasePermissions.Open | SPBasePermissions.EditMyUserInfo
            };
            result.Add(ps2.RoleName, CreateRole(web, ps2));

            return result;
        }

        private SPRoleDefinition CreateRole(SPWeb web, SPDOPermission ps)
        {
            if (web.RoleDefinitions[ps.RoleName] != null) return web.RoleDefinitions[ps.RoleName];

            web.AllowUnsafeUpdates = true;

            SPRoleDefinition contributeDefinition = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
            SPRoleDefinition dlRole = new SPRoleDefinition(contributeDefinition);
            dlRole.Name = ps.RoleName;
            dlRole.Description = ps.RoleDescription;
            dlRole.BasePermissions = ps.Permissions;

            web.RoleDefinitions.Add(dlRole);
            web.Update();
            web.AllowUnsafeUpdates = false;

            return dlRole;
        }

        #endregion

        #region Bind permissions

        private void BindAll(SPWeb web, Dictionary<string, SPGroup> groups, Dictionary<string, SPRoleDefinition> roles)
        {
            // grant read access to site
            foreach (SPGroup g in groups.Values)
            {
                BindRoleAssignment(web, g, roles["Read"]);
            }

            // set liste de suivi
            SPList listSuivi = web.Lists["Liste de suivi"];
            listSuivi.BreakRoleInheritance(true);
            BindRoleAssignment(listSuivi, groups["EDF Utilisateurs"], roles["Read"]);
            BindRoleAssignment(listSuivi, groups["EDF Contributeurs Déploiement"], roles["Read"]);
            BindRoleAssignment(listSuivi, groups["EDF Contributeurs Référentiel"], roles["Read"]);
            BindRoleAssignment(listSuivi, groups["EDF Administrateurs Fonctionnels"], roles["Contribution EDF"]);
            BindRoleAssignment(listSuivi, groups["EDF Administrateurs Site"], roles["Contribution EDF"]);


            SPList referentiel = web.Lists["Référentiel"];
            listSuivi.BreakRoleInheritance(true);
            BindRoleAssignment(listSuivi, groups["EDF Utilisateurs"], roles["Read"]);
            BindRoleAssignment(listSuivi, groups["EDF Contributeurs Déploiement"], roles["Read"]);
            BindRoleAssignment(listSuivi, groups["EDF Contributeurs Référentiel"], roles["Read"]);
            BindRoleAssignment(listSuivi, groups["EDF Administrateurs Fonctionnels"], roles["Contribution EDF"]);
            BindRoleAssignment(listSuivi, groups["EDF Administrateurs Site"], roles["Contribution EDF"]);


            BindRoleAssignment(web, groups[""], roles["Read"]);
            BindRoleAssignment(web, groups[""], roles["Read"]);
            BindRoleAssignment(web, groups[""], roles["Read"]);
        }

        private void BindRoleAssignment(SPSecurableObject obj, SPGroup group, SPRoleDefinition roleDefinition)
        {
            SPRoleAssignment roleAssigment = new SPRoleAssignment(group);
            roleAssigment.RoleDefinitionBindings.Add(roleDefinition);
            obj.RoleAssignments.Add(roleAssigment);
            roleAssigment.Update();
        }
        #endregion

        #endregion

        #region Quick Launch
        private void CreateQuickLaunch(SPWeb web)
        {
            SPNavigationNodeCollection quickLaunch = web.Navigation.QuickLaunch;
            // delete all existing nodes
            int i = 0;
            while (quickLaunch.Count > 0)
            {
                i++;
                SPNavigationNode node = quickLaunch[i];
                node.Delete();
            }

            quickLaunch.AddAsLast(new SPNavigationNode("Espace Déploiement", web.ServerRelativeUrl + "/Deploiement", false));
            quickLaunch.AddAsLast(new SPNavigationNode("Espace Référentiel", web.ServerRelativeUrl + "/Referentiel", false));
            quickLaunch.AddAsLast(new SPNavigationNode("Espace d'Equipe", web.ServerRelativeUrl + "/equipe", false));
            quickLaunch.AddAsLast(new SPNavigationNode("Suivi d'opérations", web.ServerRelativeUrl + "/lists/suivi", false));

        }

        #endregion
    }
}
