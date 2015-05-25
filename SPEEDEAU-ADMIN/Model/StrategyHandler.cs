using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Model
{

    public interface IStrategyHanlder
    {
        void Get(SPListItem item, PropertyInfo pInfo, IH1600DOC doc);
        void Set(PropertyInfo pInfo, IH1600DOC doc, ref SPListItem item);

    }
    //public class StrategyHandler<T> where T : IStrategyHanlder
    //{

    //    readonly T _handler;

    //    public StrategyHandler(T handler)
    //    {
    //        this._handler = handler;
    //    }

    //    public void Compute()
    //    {
    //        // _handler.Read();
    //    }
    //}


    public class PickerEntityHandler : IStrategyHanlder
    {

        public static SPFieldUserValueCollection GetUserValueCollection(ArrayList resolvedEntities, SPWeb web)
        {
            SPFieldUserValueCollection users = new SPFieldUserValueCollection();
            foreach (PickerEntity entity in resolvedEntities)
            {
                string login = entity.Key;
                SPUser user = web.EnsureUser(login);
                SPFieldUserValue userValue = new SPFieldUserValue(web, user.ID, user.LoginName);
                users.Add(userValue);
            }

            return users;
        }

        /// <summary>
        /// get info from SPListItem and fill property
        /// </summary>
        /// <param name="item"></param>
        /// <param name="doc"></param>
        public void Get(SPListItem item, PropertyInfo pInfo, IH1600DOC doc)
        {
            SpeedeauFieldAttribute attr = pInfo.GetCustomAttribute<SpeedeauFieldAttribute>();
            string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);
            //string users = item[fieldName].ToString();
            string users = item.EnsureValue<string>(fieldName);
            
            List<PickerEntity> result = new List<PickerEntity>();
            SPFieldUserValueCollection usersColl = new SPFieldUserValueCollection(item.Web, users);
            PeopleEditor pe = new PeopleEditor();
            foreach (SPFieldUserValue user in usersColl)
            {
                PickerEntity entity = new PickerEntity();
                entity.Key = user.User.LoginName;
                entity = pe.ValidateEntity(entity);
                entity.IsResolved = true;
                result.Add(entity);
            }

            pInfo.SetValue(doc, result);
        }

        /// <summary>
        /// Get info from property and set SPListItem 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="doc"></param>
        public void Set(PropertyInfo pInfo, IH1600DOC doc, ref SPListItem item)
        {
            List<PickerEntity> data = pInfo.GetValue(doc) as List<PickerEntity>;
            SPFieldUserValueCollection users = new SPFieldUserValueCollection();
            if (data != null)
            {
                foreach (PickerEntity picker in data)
                {
                    string login = picker.Key;
                    SPUser user = item.Web.EnsureUser(login);
                    SPFieldUserValue userValue = new SPFieldUserValue(item.Web, user.ID, user.LoginName);
                    users.Add(userValue);
                }
            }
            SpeedeauFieldAttribute attr = pInfo.GetCustomAttribute<SpeedeauFieldAttribute>();
            string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

            item[fieldName] = users;            
        }
    }




}
