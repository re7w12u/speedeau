
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace SPEEDEAU.ADMIN.Services
{
    public class AlerteService : IAlerteService
    {
        /// <summary>
        /// notifiy document author 
        /// </summary>
        /// <param name="listname"></param>
        /// <param name="itemID"></param>
        /// <param name="newStatus"></param>
        public bool NotifyAuthor(string webUrl, string listname, int itemID, string newStatus)
        {
            using (SPSite site = new SPSite(webUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    if (!IsAlerteEnabled(web)) { return false; }
                    return NotifyAuthor(web, listname, itemID, newStatus);
                }
            }
        }

        private bool NotifyAuthor(SPWeb web, string listname, int itemID, string newStatus)
        {
            try
            {
                SPList depList = web.Lists[listname];
                SPListItem item = depList.GetItemById(itemID);

                //object author = null;
                //author = item[SPBuiltInFieldId.Modified_x0020_By];
                //SPUser user = web.EnsureUser(author.ToString());

                SPUser user = item.Versions.Cast<SPListItemVersion>().First(x => x.FileVersion != null).CreatedBy.User;
                
                XElement xml = null;
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (SPSite site = new SPSite(web.Url))
                    {
                        using (SPWeb w = site.OpenWeb())
                        {
                            string data = new WebClient { UseDefaultCredentials = true }.DownloadString(w.Site.Url + "/RNVO_Assets/AlerteContent_Verification.xml");
                            xml = XElement.Parse(data);
                        }
                    }
                });

              
                #region get info
                string titre = item.Title;
                string codif = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.CODIFICATION, ResourceFiles.FIELDS));
                string indice = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS));
                string revision = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS));
                string statut = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.STATUS_DOC, ResourceFiles.FIELDS));
                string fileUrl = Uri.EscapeUriString(String.Concat(item.Web.Url, "/", item.Url));
                string fullVStatus = String.Empty;
                bool checkComments = false;
                switch (newStatus.ToLower())
                {
                    case "vso":
                        fullVStatus = Localization.GetResource(ResourceFieldsKeys.VSO, ResourceFiles.FIELDS);
                        break;
                    case "vso-sc":
                        fullVStatus = Localization.GetResource(ResourceFieldsKeys.VSOSC, ResourceFiles.FIELDS);
                        checkComments = true;
                        break;
                    case "vso-sv":
                        fullVStatus = Localization.GetResource(ResourceFieldsKeys.VSOSV, ResourceFiles.FIELDS);
                        break;
                    case "vao":
                        fullVStatus = Localization.GetResource(ResourceFieldsKeys.VAO, ResourceFiles.FIELDS);
                        checkComments = true;
                        break;
                    default:
                        break;
                }
                #endregion

                #region get commentaires
                StringBuilder commentaires = new StringBuilder();
                if (checkComments)
                {
                    SPQuery q = new SPQuery();
                    q.Query = String.Format(@"<Where>
                              <And>
                                 <And>
                                    <And>
                                       <Eq>
                                          <FieldRef Name='document' />
                                          <Value Type='Lookup'>{0}</Value>
                                       </Eq>
                                       <Eq>
                                          <FieldRef Name='EDFRevision' />
                                          <Value Type='Text'>{1}</Value>
                                       </Eq>
                                    </And>
                                    <Eq>
                                       <FieldRef Name='Status' />
                                       <Value Type='Choice'>{2}</Value>
                                    </Eq>
                                 </And>
                                 <Eq>
                                    <FieldRef Name='EDFVersion' />
                                    <Value Type='Text'>{3}</Value>
                                 </Eq>
                              </And>
                           </Where>", titre, revision, statut, indice);
                    SPList obsList = web.Lists[Localization.GetResource(ResourceListKeys.OBSERVATIONS_DEP_LISTNAME, ResourceFiles.CORE)];
                    SPListItemCollection obsItems = obsList.GetItems(q);

                    commentaires.Append ("<p>Liste des commentaires:<br/><ul>");

                    foreach (SPListItem i in obsItems)
                    {
                        commentaires.AppendFormat("<li><h3>{0}</h3><p>{1}</p></li>", i.Title, i[Localization.GetResource(ResourceFieldsKeys.OBSERVATION, ResourceFiles.FIELDS)]);
                    }
                    commentaires.Append("</ul></p>");
                }
                #endregion

                string subjectPattern = String.Format(xml.Descendants("subject").FirstOrDefault().Value, titre, indice, revision, statut, newStatus);                
                string bodyPattern = String.Format(xml.Descendants("email_body").FirstOrDefault().Value,
                    titre,
                    codif,
                    indice,
                    revision,
                    statut,
                    newStatus,
                    fullVStatus,
                    fileUrl,
                    commentaires.ToString());


#if DEBUG
                user.Email = GetDefaultEmail(web);
#endif

                if (String.IsNullOrWhiteSpace(user.Email)) throw new NullReferenceException(String.Format("Utilisateur {0} n'a pas d'adresse email !!", user.LoginName));

                AddValidationAlerte(web, subjectPattern, bodyPattern, user);
                return SPUtility.SendEmail(web, true, false, user.Email, subjectPattern, bodyPattern);
            }
            catch (Exception err)
            {
                LoggerManager.Logger.TraceToDeveloper("SPEEDEAU - Notify Author - error occured",
                    LoggerManager.EventId, Microsoft.SharePoint.Administration.TraceSeverity.Unexpected,
                    LoggerManager.AreaFullName(LoggerCategory.Alertes));
                LoggerManager.Logger.TraceToDeveloper(err);
                return false;
            }
        }

        private void AddValidationAlerte(SPWeb web, string subjectPattern, string bodyPattern, SPUser user)
        {
            // create item in alerte list
            try
            {
                web.AllowUnsafeUpdates = true;
                string alerteListName = Localization.GetResource(ResourceListKeys.ALERTE_VALIDATION_LISTNAME, ResourceFiles.CORE);
                SPList alerteList = web.Lists[alerteListName];
                SPListItem item = alerteList.AddItem();
                item[SPBuiltInFieldId.Title] = subjectPattern;
                item[SPBuiltInFieldId.Body] = bodyPattern;
                item[SPBuiltInFieldId.AssignedTo] = new SPFieldUserValue(web, user.ID, user.Name);
                item.Update();
            }
            catch (Exception err)
            {
                LoggerManager.Logger.TraceToDeveloper("SPEEDEAU - Notify Author > AddValidationAlerte : error occured", LoggerManager.EventId, TraceSeverity.Unexpected, LoggerManager.AreaFullName(LoggerCategory.Alertes));
                LoggerManager.Logger.TraceToDeveloper(err);
            }
            finally
            {
                web.AllowUnsafeUpdates = false;
            }
        }


        /// <summary>
        /// notify all site members
        /// </summary>
        /// <param name="itemID"></param>
        public bool NotifySiteMembers(string webUrl, string listname, int itemID)
        {
            using (SPSite site = new SPSite(webUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    if (!IsAlerteEnabled(web)) { return false; }

                    SPList depList = web.Lists[listname];
                    SPListItem item = depList.GetItemById(itemID);
                    #region fetch template
                    XElement xml = null;
                    SPSecurity.RunWithElevatedPrivileges(() =>
                    {
                        using (SPSite s = new SPSite(web.Url))
                        {
                            using (SPWeb w = s.OpenWeb())
                            {
                                string data = new WebClient { UseDefaultCredentials = true }.DownloadString(w.Site.Url + "/RNVO_Assets/AlerteContent_Referentiel.xml");
                                xml = XElement.Parse(data);
                            }
                        }
                    });
                    #endregion
                    string title = item.Title;
                    string url = Uri.EscapeUriString(String.Concat(item.Web.Url, "/", item.Url));
                    string fileName = item.File.Name;
                    object indice = item["EDFVersion"];
                    object revision = item["EDFRevision"];
                    object statut = item["Status"];


                    string subject = String.Format(xml.Descendants("subject").FirstOrDefault().Value, item.Title);
                    string email_body = String.Format(xml.Descendants("email_body").FirstOrDefault().Value, title, indice, revision, statut, web.Title, url, fileName);
                    string alerte_body = String.Format(xml.Descendants("alerte_body").FirstOrDefault().Value, title, url, fileName);
                    AddAlerteItem(web, subject, alerte_body);
                    return NotifySiteMembers(web, subject, email_body);
                }
            }
        }

        /// <summary>
        /// check speedeau properties to see if alerte are enabled or not
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        private bool IsAlerteEnabled(SPWeb web)
        {
            IWebProperties webProp = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
            string checkBoxPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_ALERTE_CHECKBOX, ResourceFiles.CORE);
            string checkBoxValue = webProp.Get(checkBoxPropertyName, web);

            if (!String.IsNullOrWhiteSpace(checkBoxValue) && bool.Parse(checkBoxValue) == false)
            {
                LoggerManager.Logger.TraceToDeveloper("SPEEDEAU - Notify Site Members - Les alertes sont désactivées", LoggerManager.EventId, TraceSeverity.High, LoggerManager.AreaFullName(LoggerCategory.Alertes));
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool NotifySiteMembers(SPWeb web, string title, string body)
        {
            try
            {
                string sender = GetDefaultEmail(web);

                StringDictionary headers = new StringDictionary();
                headers.Add("subject", title);
                headers.Add("content-type", "text/html");
                headers.Add("To", sender);
                headers.Add("From", sender);

                string recipients = GetRecipients(web);
                headers.Add("bcc", recipients);

                LoggerManager.Logger.TraceToDeveloper("SPEEDEAU - Notify Site Members - " + title + " - ready to send email to " + recipients.ToString(),
                   LoggerManager.EventId, Microsoft.SharePoint.Administration.TraceSeverity.Verbose,
                   LoggerManager.AreaFullName(LoggerCategory.Alertes));

                return SPUtility.SendEmail(web, headers, body);
            }
            catch (Exception err)
            {
                LoggerManager.Logger.TraceToDeveloper("SPEEDEAU - Notify Site Members - error occured", LoggerManager.EventId, TraceSeverity.Unexpected, LoggerManager.AreaFullName(LoggerCategory.Alertes));
                LoggerManager.Logger.TraceToDeveloper(err);
                return false;
            }
        }

        private static string GetRecipients(SPWeb web)
        {
            List<string> recipients = new List<string>();
            SPGroupCollection groups = web.Groups;
            foreach (SPGroup g in groups)
            {
                SPUserCollection users = g.Users;
                foreach (SPUser u in users)
                {
                    if (!String.IsNullOrWhiteSpace(u.Email))
                    {
                        recipients.Add(u.Email);
                    }
                }
            }

            // remove duplicates
            recipients = recipients.Distinct().ToList();

            return String.Join(",", recipients.ToArray());
        }

        private void AddAlerteItem(SPWeb web, string title, string body)
        {
            // create item in alerte list
            try
            {
                string alerteListName = Localization.GetResource(ResourceListKeys.ALERTE_LISTNAME, ResourceFiles.CORE);
                SPList alerteList = web.Lists[alerteListName];
                SPListItem item = alerteList.AddItem();
                item["Title"] = title;
                item["Body"] = body;
                item.Update();
            }
            catch (Exception err)
            {
                LoggerManager.Logger.TraceToDeveloper("SPEEDEAU - Notify Site Members > AddAlerteItem : error occured", LoggerManager.EventId, TraceSeverity.Unexpected, LoggerManager.AreaFullName(LoggerCategory.Alertes));
                LoggerManager.Logger.TraceToDeveloper(err);
            }
        }

        /// <summary>
        /// look up  in current web parameter or get the default farm email address
        /// </summary>
        /// <returns></returns>
        private string GetDefaultEmail(SPWeb web)
        {
            IWebProperties webProp = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
            string EmailPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_ALERTE_EMAIL, ResourceFiles.CORE);
            string emailValue = webProp.Get(EmailPropertyName, web);
            if (!String.IsNullOrWhiteSpace(emailValue)) return emailValue;

            return web.Site.WebApplication.OutboundMailSenderAddress;
        }

    }
}
