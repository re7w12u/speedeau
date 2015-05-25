



function Init() {
};

/**** cmd for deploiement ***********************/
function showValidationForm_FromDep() {
    var item = getSelectedItem();
    if (item != null) {
        // cannot use param 'ID' in url - throw error for some reason
        var url = _spPageContextInfo.webAbsoluteUrl + "/RNVO_Setup/pages/ValidationForm.aspx?currentId=" + item.id;
        openDialogSize(url, "Validation IH1600", 600, 250);
    }
}

function showCommentForm_FromDep() {
    var item = getSelectedItem();
    if (item != null) {
        var url = _spPageContextInfo.webAbsoluteUrl + "/Lists/obs-dep/NewForm.aspx?DOCID=" + item.id;
        openDialog(url, "Ajouter un commentaire", 800, 600);
        //var url = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/suivi.svc/getDocID/" + item.id;
        //$.when($.ajax({ "url": url, "headers": { "Accept": "application/json; odata=verbose" } }))
        //.done(function (data) {
        //    // expected result : {"getDocIDResult":4}
        //    if (data.getDocIDResult == 0) {
        //        alert("Aucun fichier lié pour cet élément de suivi.");
        //    }
        //    else {
        //        var url = _spPageContextInfo.webAbsoluteUrl + "/Lists/obs-dep/NewForm.aspx?DOCID=" + data.getDocIDResult;
        //        openDialog(url, "Saisir un commentaire");
        //    }
        //});
    }
}

function showUploadDocumentForm_FromDep() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 0) {
        var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/check_codification.aspx?new=false";
        openDialog(url, "Nouveau document de déploiement");
    }
    else {
        alert("Veuillez désélectionner les fichiers.");
        return null;
    }
}

function showUploadVersionForm_FromDep() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 1) {
        var item = selectedItems[0];
        url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/newdeploiement.aspx?ID=" + item.id;
        openDialog(url, "Nouvelle version document de déploiement");
    }
    else {
        alert("Veuillez sélectionner un fichier.");
        return null;
    }
}

function showUpdateForm_FromDep() {
    var item = getSelectedItem();
    var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/newdeploiement.aspx?UID=" + item.id;
    openDialog(url, "Mettre à jour les propriétés");
}

/**** cmd for referentiel ***********************/

function showUploadDocumentForm_FromRef() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 0) {
        var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/NewReferentiel.aspx?blank=true";
        openDialog(url, "Nouveau document de référence");
    }
    else {
        alert("Veuillez désélectionner le ou les fichiers.");
        return null;
    }


}

function showUploadVersionForm_FromRef() {

    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 1) {
        var item = selectedItems[0];
        url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/NewReferentiel.aspx?ID=" + item.id;
        openDialog(url, "Nouvelle version document de référence");
    }
    else {
        alert("Veuillez sélectionner un fichier.");
        return null;
    }


}

function showUpdateForm_FromRef() {
    var item = getSelectedItem();
    var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/NewReferentiel.aspx?UID=" + item.id;
    openDialog(url, "Mettre à jour les métadonnées");
}

function showObservationsList_FromDep() {
    var item = getSelectedItem();

    var url = _spPageContextInfo.webAbsoluteUrl + "/SitePages/observations.aspx?DID=" + item.id;
    var options = SP.UI.$create_DialogOptions();
    options.title = "Commentaires";
    options.width = 800;
    options.height = 600;
    options.url = url;
    options.allowMaximize = true;
    options.showClose = true;

    SP.UI.ModalDialog.showModalDialog(options);

    //if (item != null) {

    //    var restUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists(guid'" + _spPageContextInfo.pageListId.replace('{', '').replace('}', '') + "')/items("+item.id+")?$select=Title,EDFRevision,EDFVersion";

    //    $.ajax({ "url": restUrl, "headers": { "Accept": "application/json; odata=verbose" } }).done(function (data) {
    //        var obsListUrl = _spPageContextInfo.webAbsoluteUrl + "/SitePages/observations.aspx?FilterField1=document&FilterValue1=" + encodeURI(data.d.Title) + "&FilterField2=EDFVersion&FilterValue2=" + data.d.EDFVersion + "&FilterField2=EDFRevision&FilterValue2=" + data.d.EDFRevision + "&PageView=Shared";
    //        openDialog(obsListUrl, "Observations pour " + data.d.Title);
    //    });
    //}
    //var url = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/deploiement.svc/getDeploiementForSuiviItem/" + item.id;
    //$.when($.ajax({ "url": url, "headers": { "Accept": "application/json; odata=verbose" } }))
    //    .done(function (data) {
    //        if (data.GetDeploiementForSuiviItemResult != null) {
    //            var d = data.GetDeploiementForSuiviItemResult;
    //            var obsListUrl = _spPageContextInfo.webAbsoluteUrl + "/SitePages/observations.aspx?FilterField1=document&FilterValue1=" + encodeURI(d.Title) + "&FilterField2=EDFRevision&FilterValue2=" + d.Revision + "&FilterField2=EDFRevision&FilterValue2=" + d.Indice + "&PageView=Shared";
    //            openDialog(obsListUrl, "Observations pour " + d.Title);
    //        } else {
    //            alert("Aucune observations trouvées pour ce document.");
    //        }
    //    });
}

function onDialogCloseAction(dialogResult, returnedValue) {
    if (dialogResult == SP.UI.DialogResult.OK && returnedValue == 1) {
        SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK);
    }
}

/// get info for selected item
function getItemInfo(id, callback) {
    //var d = $.Deferred();
    //var context = SP.ClientContext.get_current();
    //var listID = SP.ListOperation.Selection.getSelectedList();
    //var list = context.get_web().get_lists().getById(listID);
    //var item = list.getItemById(id);
    //var label = null;
    //context.load(item);
    //context.executeQueryAsync(
    //    Function.createDelegate(this, function () {
    //        var taxField = item.get_item(IH1600.field.info.Typologie.internalName);
    //        var target = taxField.get_label();
    //        if (target == undefined) target = taxField.Label;
    //        d.resolve(target);
    //    }),
    //    RNVO.common.onQueryFailed
    //);

    //return d.promise();


    var familleDocRestUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/suivi.svc/GetSuiviitem/" + id;
    return $.ajax({ "url": familleDocRestUrl, "headers": { "Accept": "application/json; odata=verbose" } });
    
    
    //.done(function (t) {
    //    var suivi = JSON.parse(t.GetSuiviItemResult);
    //    if (suivi.Term == "Déploiement") {
    //        var codif = suivi.CodificationSystem;
    //        if (codif == "") {
    //            alert("Cet élément n'a pas de codification.");
    //            return;
    //        }
    //        var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/DocumentPicker.aspx?CID=" + codif;
    //        openDialog(url, "Sélectionnez le document à mettre à jour");

    //    }
    //    else if (suivi.Term == "Référentiel") alert("Vous ne pouvez pas télécharger de document de référence à partir de la liste de suivi.");
    //    else alert("le système n'a pas pu déterminer quel formulaire ouvrir.\n Assurez-vous que la famille documentaire soit bien renseignée pour cette entrée.");
    //});
}



/**** cmd for suivi ***********************/
/**
* open dialog window for uploading a document in 'Déploiement' Library
* triggered from 'liste de suivi'
* Binded to ribbon button
*/
function showUploadDocumentForm_FromSuivi() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 1) {
        var item = selectedItems[0];
        // check if user has right to upload document on deploiement
        //var restUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/permissions.svc/CanUploadToDeploiement";
        //$.ajax({ "url": restUrl, "headers": { "Accept": "application/json; odata=verbose" } }).done(function (data) {
        //    if (data.CanUploadToDeploiementResult) {
                // check the 'Famille Doc' of selected item
        getItemInfo(item.id).done(showUploadDocumentForm_CallBack);
        //    } else {
        //        alert("Vous ne pouvez pas télécharger de document.");
        //    }
        //});
    }
    else {
        alert("La sélection d'une ligne (une seule) est nécessaire");
        return null;
    }
}
/**
* open dialog window for uploading a document in 'Déploiement' Library
* triggered from webpart displaying liste de suivi
* Binded to ribbon button
*/
function showUploadDocumentForm_FromSuiviWebPart() {
    // cannot user SP.ListOperation here because we're not in the context of the list... too bad !!
    var selectedItems = $("tr.s4-itm-selected input");
    if (selectedItems.length == 1) {
        var itemId = $(selectedItems[0]).attr("title");
        // check the 'Famille Doc' of selected item
        getItemInfo(itemId).done(showUploadDocumentForm_CallBack);
    }
    else {
        alert("La sélection d'une ligne (une seule) est nécessaire");
        return null;
    }
}

function showUploadDocumentForm_CallBack(t) {
    var suivi = JSON.parse(t.GetSuiviItemResult);

    if (suivi.Term == "Déploiement") {
        var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/newdeploiement.aspx?FID=" + suivi.ID;
        //var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/DocumentPicker.aspx?SID=" + item.id;
        openDialog(url, "Nouveau document de déploiement");
    }
    else if (t == "Référentiel") alert("Vous ne pouvez pas télécharger de document de référence à partir de la liste de suivi.");
    else alert("le système n'a pas pu déterminer quel formulaire ouvrir.\n Assurez-vous que la famille documentaire soit bien renseignée pour cette entrée.");
}

/**
* open dialog window for uploading a new version in 'Déploiement' Library
* triggered from 'liste de suivi'
* Binded to ribbon button
*/
function showUploadVersionForm_FromSuivi() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 1) {
        var item = selectedItems[0];        
        getItemInfo(item.id).done(showUploadVersionForm_CallBack);
    }
    else {
        alert("La sélection d'une ligne (une seule) est nécessaire");
        return null;
    }
}

/**
* open dialog window for uploading a new version in 'Déploiement' Library
* triggered from webpart displaying liste de suivi
* Binded to ribbon button
*/
function showUploadVersionForm_FromSuiviWebPart() {
    var selectedItems = $("tr.s4-itm-selected input");
    if (selectedItems.length == 1) {
        var itemId = $(selectedItems[0]).attr("title");
        // check the 'Famille Doc' of selected item
        getItemInfo(itemId).done(showUploadVersionForm_CallBack);
    }
    else {
        alert("La sélection d'une ligne (une seule) est nécessaire");
        return null;
    }
}

function showUploadVersionForm_CallBack(t) {
    var suivi = JSON.parse(t.GetSuiviItemResult);
    if (suivi.Term == "Déploiement") {
        var codif = suivi.CodificationSystem;
        if (codif == "") {
            alert("Cet élément n'a pas de codification.");
            return;
        }
        var url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/DocumentPicker.aspx?CID=" + codif;
        openDialog(url, "Sélectionnez le document à mettre à jour");

    }
    else if (suivi.Term == "Référentiel") alert("Vous ne pouvez pas télécharger de document de référence à partir de la liste de suivi.");
    else alert("le système n'a pas pu déterminer quel formulaire ouvrir.\n Assurez-vous que la famille documentaire soit bien renseignée pour cette entrée.");
}

/*** display new form for liste de suivi ****/
function showNewSuiviForm_FromSuivi() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 0) {
        alert("Veuillez sélectionner un élément correspondant à l'opération souhaitée.");
        return null;
    }
    if (selectedItems.length > 1) {
        alert("Veuillez ne sélectionner qu'un seul élément.");
        return null;
    }
    else {
        var item = selectedItems[0];
        url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/suiviedit.aspx?mode=new&SID=" + item.id;
        openDialog(url, "Création d'un nouvel élément de suivi");
    }
}

/*** display modification form for liste de suivi *****/
function showModifSuiviForm_FromSuivi() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 0) {
        alert("Veuillez sélectionner l'élément à modifier.");
        return null;
    }
    if (selectedItems.length > 1) {
        alert("Veuillez ne sélectionner qu'un seul élément.");
        return null;
    }
    else {
        var item = selectedItems[0];
        url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/suiviedit.aspx?mode=edit&SID=" + item.id;
        openDialog(url, "Modification d'un élément de suivi");
    }
}

/*** display duplication form for liste de suivi ****/
function showDuplicateSuiviForm_FromSuivi() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 0) {
        alert("Veuillez sélectionner l'élément à dupliquer.");
        return null;
    }
    if (selectedItems.length > 1) {
        alert("Veuillez ne sélectionner qu'un seul élément.");
        return null;
    }
    else {
        var item = selectedItems[0];
        url = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/speedeau/suiviedit.aspx?mode=duplicate&SID=" + item.id;
        openDialog(url, "Dupliquer un élément de suivi");
    }

}

/**** common cmd ***********************/
function openDialog(url, title) {
    //Using the DialogOptions class.
    var options = SP.UI.$create_DialogOptions();
    options.title = title;
    options.width = 600;
    options.height = 600;
    options.url = url;
    options.allowMaximize = true;
    options.showClose = true;
    options.dialogReturnValueCallback = onDialogCloseAction;

    SP.UI.ModalDialog.showModalDialog(options);
}

function openDialogSize(url, title, w, h) {
    //Using the DialogOptions class.
    var options = SP.UI.$create_DialogOptions();
    options.title = title;
    options.width = w;
    options.height = h;
    options.url = url;
    options.allowMaximize = true;
    options.showClose = true;
    options.dialogReturnValueCallback = onDialogCloseAction;

    SP.UI.ModalDialog.showModalDialog(options);
}

function getSelectedItem() {
    var context = SP.ClientContext.get_current();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    if (selectedItems.length == 1) {
        return selectedItems[0];
    } else if (selectedItems.length == 0) {
        alert("Veuillez sélectionner un fichier.");
        return null;
    }
    else if (selectedItems.length > 1) {
        alert("Veuillez ne sélectionner qu'un seul fichier.");
        return null;
    }
}


/******** enable disable ribbon button ****************/



var _hasValidateAndCommentPermission = false;
/**
editlist + edituserinfo = contributeur and more
editlist + ! edituserinfo = redacteur
!editlist = visitor
*/
function CanValidateAndComment() {
    if (_hasValidateAndCommentPermission) return _hasValidateAndCommentPermission;

    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Déploiement')/EffectiveBasePermissions";
    $.ajax({ "url": url, "headers": { "Accept": "application/json; odata=verbose" } }).done(
        function (data) {
            var permissions = new SP.BasePermissions();
            permissions.fromJson(data.d.EffectiveBasePermissions);
            _hasValidateAndCommentPermission = permissions.has(SP.PermissionKind.editListItems) && permissions.has(SP.PermissionKind.editMyUserInfo);
            if (_hasValidateAndCommentPermission) RefreshCommandUI();
        }
    );

    return false;
}


var _hasRibbonPermission = false;
function EnableRibbonBtn() {
    if (_hasRibbonPermission) return _hasRibbonPermission;

    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Déploiement')/EffectiveBasePermissions";
    $.ajax({ "url": url, "headers": { "Accept": "application/json; odata=verbose" } }).done(
        function (data) {
            var permissions = new SP.BasePermissions();
            permissions.fromJson(data.d.EffectiveBasePermissions);
            _hasRibbonPermission = permissions.has(SP.PermissionKind.editListItems);
            if (_hasRibbonPermission) RefreshCommandUI();
        }
    );

    return false;
}


var _canUploadToDeploiement = false;
function EnableUploadFromListSuivi() {
    if (_canUploadToDeploiement) return _canUploadToDeploiement;
    var restUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/permissions.svc/CanUploadToDeploiement";
    $.ajax({ "url": restUrl, "headers": { "Accept": "application/json; odata=verbose" } }).done(
        function (data) {
            _canUploadToDeploiement = data.CanUploadToDeploiementResult;
            if (_canUploadToDeploiement) RefreshCommandUI();
        });

    return false;
}


var _canChangeListeDeSuivi = false;
function EnableChangeListSuivi() {
    if (_canChangeListeDeSuivi) return _canChangeListeDeSuivi;
    var restUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/permissions.svc/CanChangeListeDeSuivi";
    $.ajax({ "url": restUrl, "headers": { "Accept": "application/json; odata=verbose" } }).done(
        function (data) {
            _canChangeListeDeSuivi = data.CanChangeListeDeSuiviResult;
            if (_canChangeListeDeSuivi) RefreshCommandUI();
        });

    return false;
}

/******************* init ***********************/

$(function () {

    var scriptbase = _spPageContextInfo.webServerRelativeUrl + "/_layouts/15/";
    var assets = _spPageContextInfo.siteServerRelativeUrl + "/RNVO_Assets/js/";

    SP.SOD.registerSod("rnvo.common", assets + "rnvo.common.js");
    SP.SOD.registerSod("rnvo.ih1600.model", assets + "rnvo.ih1600.model.js");
    SP.SOD.registerSod("rnvo.params", assets + "rnvo.params.js");
    SP.SOD.registerSod("rnvo.ih1600.validation", assets + "rnvo.ih1600.validation.js");
    SP.SOD.registerSod("spdo.taxonomy.", assets + "spdo.taxonomy.js");

    SP.SOD.registerSodDep("rnvo.params", "rnvo.common");
    SP.SOD.registerSodDep("rnvo.ih1600.validation", "rnvo.params");
    SP.SOD.registerSodDep("rnvo.ih1600.validation", "rnvo.ih1600.model");

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.executeFunc('sp.taxonomy.js', null, null);
        SP.SOD.executeFunc("rnvo.common", null, null);
        SP.SOD.executeFunc("rnvo.ih1600.model", null, null);
        SP.SOD.executeFunc("rnvo.params", null, null);
        SP.SOD.executeFunc("rnvo.ih1600.validation", null, null);
        SP.SOD.executeFunc("spdo.taxonomy", null, null);
    }, "SP.js");

});