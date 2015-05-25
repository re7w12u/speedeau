Type.registerNamespace("OBS");

//$(function () {
//    var scriptbase = _spPageContextInfo.webServerRelativeUrl + "/_layouts/15/";
//    $.getScript(scriptbase + "SP.js", Init);
//});

function Init() {
    // disable select control - value are not up to the user.
    $("select").prop("disabled", true)

    var queryString = $.getQueryParameters();
    if (queryString.DOCID == undefined) RNVO.notify.error("Missing parameter exception");
    else {
        OBS.core.context = SP.ClientContext.get_current();
        OBS.core.library = OBS.core.context.get_web().get_lists().getByTitle(IH1600.param.depLibraryName);
        OBS.core.obsList = OBS.core.context.get_web().get_lists().getByTitle(IH1600.param.depObsListName);
        OBS.form.init(queryString.DOCID);
    }
}


//------------------------- CORE -------------------------------
OBS.core = function () { };
OBS.core.context = null;
OBS.core.obsList = null;
OBS.core.serverAbsoluteUrl = _spPageContextInfo.webAbsoluteUrl;


OBS.form = function () { };
OBS.form.listItemColl = null;
OBS.form.selectedId = null;
OBS.form.init = function (id) {
    OBS.form.selectedId = id;
    OBS.form.getSelectedFileData();
}

//OBS.form.hideSelectDocument = function () {
//    var field = OBS.form.getFileFieldObj();
//    field.hide();
//}
OBS.form.getSelectedFileData = function () {
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml('<View><Query><Where><Eq><FieldRef Name="ID" /><Value Type="Counter">' + OBS.form.selectedId + '</Value></Eq></Where></Query>' +
                                '<ViewFields>'+
                                    '<FieldRef Name="' + IH1600.field.info.Verification.internalName + '" />'+
                                    '<FieldRef Name="' + IH1600.field.info.Indice.internalName + '" />' +
                                    '<FieldRef Name="' + IH1600.field.info.EDFRevision.internalName + '" />' +
                                    '<FieldRef Name="' + IH1600.field.info.StatusCurrent.internalName + '" />'+
                                '</ViewFields>' +
                                '<RowLimit>1</RowLimit>' +
                          '</View>');
    OBS.form.listItemColl = OBS.core.library.getItems(camlQuery);
    OBS.core.context.load(OBS.form.listItemColl);
    OBS.core.context.executeQueryAsync(OBS.form.onFileDataReceived, RNVO.common.onQueryFailed);
}

OBS.form.onFileDataReceived = function () {
    var listItemEnumerator = OBS.form.listItemColl.getEnumerator();
    // file DocCollection with data from existing docs
    listItemEnumerator.moveNext();
    var oListItem = listItemEnumerator.get_current();
    var value = oListItem.get_item(IH1600.field.info.Verification.internalName);
    var version = oListItem.get_item(IH1600.field.info.Indice.internalName);

    if (value != "None") {
        RNVO.notify.error("Ce document a été validé. De ce fait, vous ne pouvez plus faire de commentaire sur cette version.");
    }
    else if (version == null) {
        RNVO.notify.error("Ce document n'a pas d'indice. De ce fait, vous ne pouvez pas faire de commentaire sur cette version.");
    }
    else {
        // show form
        $("#obs-loading").closest(".ms-webpartzone-cell").next().slideDown();
        OBS.form.setLookupValue();        

        $(IH1600.field.info.Indice.selector).val(version);
        $(IH1600.field.info.Indice.selector).prop("disabled", true);

        var revision = oListItem.get_item(IH1600.field.info.EDFRevision.internalName);
        $(IH1600.field.info.EDFRevision.selector).val(revision);
        $(IH1600.field.info.EDFRevision.selector).prop("disabled", true);

        var status = oListItem.get_item(IH1600.field.info.StatusCurrent.internalName);
        $(IH1600.field.info.StatusCurrent.selector).closest("table").find("input[value=" + status + "]").prop("checked", "checked");
        //IH1600.field.info.StatusCurrent.selector only points to select radio button. We need to disable the sibling as well.
        //so we go up the DOM tree to the closest wrapping table and then get all radio button within
        $(IH1600.field.info.StatusCurrent.selector).closest("table").find("input").prop("disabled", true);
        
    }

    $("#obs-loading").hide();
}

OBS.form.setLookupValue = function () {
    var select = $("select[title=" + RNVO.def.fields.obs.doc + "]");
    select.children("[selected=selected]").removeAttr("selected");
    select.children("[value=" + OBS.form.selectedId + "]").attr("selected", "selected");
}


//# sourceURL=rnvo.obs.main.js