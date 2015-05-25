
//$(function () {
//    var scriptbase = _spPageContextInfo.webServerRelativeUrl + "/_layouts/15/";
//    $.getScript(scriptbase + "SP.js", function () {
//        $.getScript(scriptbase + "SP.Taxonomy.js", Init);
//    });
//});

function Init() {
    // display our custom tab by default
    //_ribbonStartInit("Ribbon.EDFTab", true);
    // disable drag and drop because we want user to fill in metadata
    //g_uploadType = DragDropMode.NOTSUPPORTED;
    //SPDragDropManager.DragDropMode = DragDropMode.NOTSUPPORTED;
    // get context
    IH1600.core.context = SP.ClientContext.get_current();
    // get list and librairies
    IH1600.core.library = IH1600.core.context.get_web().get_lists().getByTitle(IH1600.param.refLibraryName);
    // show loading image
    IH1600.ui.showLoading();
    // trigger data queries
    IH1600.form.GetAllDocumentInfo();
}

//------------------------- CORE -------------------------------
IH1600.core = function () { };
IH1600.core.context = null;
IH1600.core.library = null;// document library storing documents being watched and verified
IH1600.core.currentDoc = null; // reference of the document currently edited
IH1600.core.serverAbsoluteUrl = _spPageContextInfo.webAbsoluteUrl;

// ----------------------- FORM ---------------------------------
IH1600.form = function () {
    /// <summary>SharePoint IH1600 custom static class.</summary> 
}

IH1600.form.doc = new DocCollection(); // map collection containing metadata for each files (version, site, projects, etc) and identified by its code IH1600 as key
IH1600.form.listItemColl = null;


IH1600.form.GetAllDocumentInfo = function () {
    /// <summary>get file metadata from doc library</summary>
    if (IH1600.param.depLibraryName == undefined) return;

    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml('<View><Query></Query></View>');
    var camlQuery = SP.CamlQuery.createAllItemsQuery();
    IH1600.form.listItemColl = IH1600.core.library.getItems(camlQuery);
    IH1600.core.context.load(IH1600.form.listItemColl);
    IH1600.core.context.executeQueryAsync(IH1600.form.OnReceivingDocInfo, RNVO.common.onQueryFailed);
}

IH1600.form.OnReceivingDocInfo = function (sender, args) {
    var queryString = $.getQueryParameters();
    var listItemEnumerator = IH1600.form.listItemColl.getEnumerator();
    // file DocCollection with data from existing docs
    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        var doc = docFactory(oListItem, IH1600.enum.contentType.REF);
        IH1600.form.doc.add(doc);
        if (doc.id == queryString.ID) {
            IH1600.core.currentDoc = doc;
        }
    }

    if (IH1600.core.currentDoc == null) {
        // new document
        IH1600.core.currentDoc = docFactory(null, IH1600.enum.contentType.REF);
    }

    IH1600.form.setFields();
    IH1600.form.onChangeBehavior();
}

IH1600.form.setFields = function () {
    if (IH1600.core.currentDoc != null) {

        // hide title field and set same value as file name
        IH1600.ui.setUpTitleField();

        //set version
        var version = IH1600.ui.getField(IH1600.field.info.Indice);
        var v1 = IH1600.core.currentDoc.getNextVersion();
        version.val(v1);

        // hide some controls button
        IH1600.ui.hideCancel();
        IH1600.ui.hideNameField();

        // set up code IH1600
        IH1600.ui.enableForm();
    }
}

IH1600.form.onChangeBehavior = function () {

    $(IH1600.field.info.Codification.selector).keyup(function () {
        RNVO.notify.cleanUp();

        var field = $(this);
        var code = field.val();
        var isEmpty = code == "";
        var isValid = false;

        if (!isEmpty && !IH1600.form.ValidateInputCodification(code)) {
            //console.log("invalid code : " + code);
            RNVO.notify.warn(IH1600.msg.invalidCodeIH);
            IH1600.ui.disableSave(true);
            return;
        }

        field.val(code.toUpperCase());

        if (!isEmpty && IH1600.core.currentDoc.codeIh1600 != null && code != IH1600.core.currentDoc.codeIh1600) {
            // console.log("inconsistent code : " + code);
            RNVO.notify.error(IH1600.msg.inconsistentCodeIH);
            IH1600.ui.disableSave(true);
            return;
        }

        IH1600.ui.disableSave(false);
    });

    $(IH1600.field.info.Indice.selector).keyup(function () {
        // force upper case for indice
        $(this).val($(this).val().toUpperCase());
    });

    // inject validation before default sharepoint submit
    var defaultCmd = $(IH1600.field.info.Save.selector).attr("onclick");
    $(IH1600.field.info.Save.selector).attr("onclick", 'if(!IH1600.form.validateSubmit())return false;' + defaultCmd);
}

IH1600.form.validateSubmit = function () {
    RNVO.notify.cleanUp();

    // check Indice is greater than current version
    var currentIndice = IH1600.core.currentDoc.version;
    var currentInput = IH1600.ui.getFieldValue(IH1600.field.info.Indice);
    if (currentIndice != null && !(currentInput > currentIndice)) {
        RNVO.notify.error(IH1600.msg.wrongIndice);
        return false;
    }

    // check if Codification is OK
    var codification = IH1600.ui.getFieldValue(IH1600.field.info.Codification);
    if (!IH1600.form.ValidateInputCodification(codification)) {
        RNVO.notify.error(IH1600.msg.invalidCodeIH);
        return false;
    }


    // create alert
    //IH1600.form.createAlert();
    return true;
}

IH1600.form.createAlert = function () {
    var version = IH1600.ui.getFieldValue(IH1600.field.info.Indice);
    // create alerte
    var body = 'Nouveau document de référence '
        + '<a href="' + IH1600.core.currentDoc.fileurl + '">' + IH1600.core.currentDoc.filename + '</a>'
        + ' en version ' + version;
    var title = IH1600.core.currentDoc.filename + ' - ' + version;
    
    IH1600.alerte.create(title, body, IH1600.enum.contentType.REF);
}

// TODO change regex to skip version check

IH1600.form.ValidateInputCodification = function (code) {
    //IH-MSH-MODST-DLPT_COND 0018-C
    if (code == "" || code == null) return false;
    var re = /IH[\s-_][a-z-\s_]{0,20}[\s-_]?[0-9]{3,6}/ig;
    return re.test(code);
}


// ----------------------- REGISTRATION ---------------------------------
IH1600.core.registerClass("IH1600.core");
IH1600.enum.registerClass("IH1600.enum");
IH1600.form.registerClass("IH1600.form");



//# sourceURL=rnvo.IH1600.core.js