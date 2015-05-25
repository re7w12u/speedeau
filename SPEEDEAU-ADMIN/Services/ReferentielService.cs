using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Taxonomy;

namespace SPEEDEAU.ADMIN.Services
{
    public class ReferentielService : DocServiceBase, IReferentielService
    {
        #region IDeploiementService implementation

        public Referentiel GetReferentielFromRefLibItem(int id, SPWeb web)
        {
            string listName = Localization.GetResource(ResourceListKeys.REFERENTIEL_LISTNAME, ResourceFiles.CORE);
            SPList list = web.Lists[listName];
            SPListItem item = list.GetItemById(id);

            Referentiel result = new Referentiel();
            HydrateIH1600FromDocLibraryItem(item, result);
            return result;
        }

        public SPListItem SaveReferentielToList(Referentiel referentiel, string filename, SPWeb web)
        {
            string listName = Localization.GetResource(ResourceListKeys.REFERENTIEL_LISTNAME, ResourceFiles.CORE);
            SPDocumentLibrary docLib = web.Lists[listName] as SPDocumentLibrary;

            SPFile file;
            bool createNewVersion = false;
            if (referentiel.File != null && referentiel.File.Count() > 0)
            {
                if (String.IsNullOrWhiteSpace(filename))
                {
                    // new file
                    file = docLib.RootFolder.Files.Add(referentiel.FileName, referentiel.File, true);
                }
                else
                {
                    //createNewVersion = true;
                    // existing file - first use previous name to keep version tracking
                    file = docLib.RootFolder.Files.Add(filename, referentiel.File, true);
                    // then rename file with new name
                    file.MoveTo(file.ParentFolder.Url + "/" + referentiel.FileName);
                }
            }
            else
            {
                file = docLib.RootFolder.Files[filename];
            }
            SPListItem item = file.Item;
            SetItemValues(item, referentiel);

            if (createNewVersion) item.Update();
            else item.SystemUpdate(false);

            return item;
        }

        #endregion

    }
}
