IH1600.ui = function () { };

IH1600.ui.showLoading = function () {
    $("#ih1600-loading").parents(".ms-webpartzone-cell").show();
    RNVO.common.resizeDialog();
}

IH1600.ui.hideCancel = function () {
    $(IH1600.field.info.Cancel.selector).hide();

}

IH1600.ui.hideNameField = function () {
    $(IH1600.field.info.Name.selector).parents("TR:first").hide()
}

IH1600.ui.hideValidationField = function () {
    $(IH1600.field.info.Verification.selector).parents("TR:first").hide()
}

//
IH1600.ui.hideProcessIH1600Field = function () {
    $(IH1600.field.info.ProcessIH1600.selector).parents("TR:first").hide();
}


IH1600.ui.enableForm = function () {
    $("#ih1600-loading").hide();
    $("#ih1600-loading").parents(".ms-webpartzone-cell").next().show();
    RNVO.common.resizeDialog();
}

IH1600.ui.setUpTitleField = function () {
    var titleField = IH1600.ui.getField(IH1600.field.info.Title);
    titleField.val(IH1600.core.currentDoc.filename);
    titleField.parents("TR:first").hide();
    // hide extra top save and cancel buttons
    $(".ms-toolbar:first").hide();
}

IH1600.ui.setUpStatusField = function (status) {    
    var statusField = null;
    if (status == IH1600.enum.status.PREL) statusField = IH1600.ui.getField(IH1600.field.info.StatusPREL);
    else statusField = IH1600.ui.getField(IH1600.field.info.StatusBPE);

    statusField.prop("checked", true);

    if (!IH1600.core.currentDoc.isNewFile() && status == IH1600.enum.status.PREL) {
        // hide BPE
        $(".ms-RadioText[title=BPE]").parents("TR:first").hide()
    }
}

IH1600.ui.setUpVersion = function (status) {

    var doc = IH1600.core.currentDoc;
    var version = IH1600.ui.getField(IH1600.field.info.Indice);
    var revision = IH1600.ui.getField(IH1600.field.info.EDFRevision);

    var v1 = doc.getNextVersion();
    var v2 = doc.getNextRevision();
    version.val(v1);

    if (v1 != "A" || (v1 == "A" && v2 != "0")) {
      // disabled field except on first upload to allow adding existing version
        version.prop("disabled", true);
        revision.prop("disabled", true);
    }

    if (status == IH1600.enum.status.BPE) {
        //revision.val(doc.getNextRevision());
        // no revision whatsoever for BPE anyway
        revision.val('');
        IH1600.ui.hideShowRevision(false);
    }
    else if (status == IH1600.enum.status.PREL) {
        IH1600.ui.hideShowRevision(true);
        var r = doc.getNextRevision(true);
        if (r == "") r = 2;
        revision.val(r);
    }
}

IH1600.ui.hideShowRevision = function (show) {
    var target = IH1600.ui.getField(IH1600.field.info.EDFRevision).parents("TR:first");
    if (show) target.slideDown();
    else target.slideUp();
}

IH1600.ui.logError = function (msg) {
    $("#ih1600-messageWrapper").show();
    $("#ih1600-messageLog").append($("<div>" + msg + "</div>"));
}



IH1600.ui.disableSave = function (prevent) {
    $(".ms-toolbar input").prop("disabled", prevent);
}

IH1600.ui.getField = function (field_info) {
    var field = $(field_info.selector);
    return field;
}

IH1600.ui.getFieldValue = function (field_info) {
    var result = null;
    var field = IH1600.ui.getField(field_info);
    switch (field_info.fieldName) {
        case IH1600.field.info.Codification.fieldName:
        case IH1600.field.info.Title.fieldName:
        case IH1600.field.info.Indice.fieldName:
        case IH1600.field.info.EDFRevision.fieldName:
        case IH1600.field.info.StatusBPE.fieldName:
        case IH1600.field.info.StatusPREL.fieldName:
        case IH1600.field.info.StatusCurrent.fieldName:
            result = field.val();
            break;
        case IH1600.field.info.Site.fieldName:
        case IH1600.field.info.Projet.fieldName:
        case IH1600.field.info.Typologie.fieldName:
            result = field.children().text();
            break;
        default:
            break;

    }
    return result;    
}

IH1600.ui.registerClass("IH1600.ui");



//# sourceURL=rnvo.ih1600.ui.js