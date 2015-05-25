
function GetListInformation() {
    var url = _spPageContextInfo.webAbsoluteUrl + "/_api/Lists('" + _spPageContextInfo.pageListId + "')/?$select=basetemplate";
    $.ajax({ "url": url, "headers": { "Accept": "application/json; odata=verbose" } })
        .done(onQuerySucceeded);
}

function onQuerySucceeded(data) {

    var basetype = data.d.BaseTemplate;
    switch (basetype) {
        case 10100:
            HideNewDocumentLink();
            DisableDragAndDrop();
            break;
        case 10101:
            HideNewDocumentLink();
            DisableDragAndDrop();
            break;
        case 20300:
            HideNewDocumentLink();
            break;
        default:
            break;
    }
}

function HideNewDocumentLink() {
    $("#Hero-WPQ2").css("visibility", "hidden");
}

function checkUserPerm(tabId) {
    var restUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/permissions.svc/CanUploadToDeploiement";

    $.ajax({ "url": restUrl, "headers": { "Accept": "application/json; odata=verbose" } }).done(function (data) {

        if (!data.CanUploadToDeploiementResult) {
            // show ribbon in list de suivi
            HideTab(tabId);
        }
    });
}

function HideTab(tabId) {

    $("#" + tabId + "-title").hide();
}

function SetRibbonFocus(tabId) {
    try {
        var ribbon = SP.Ribbon.PageManager.get_instance().get_ribbon();
        SelectRibbonTab(tabId, true);
    } catch (e)
    { }
}

function SetLinkAsBlank() {
    $(".ms-listlink").attr("target", "_blank")
}

function HideHelpButton() {
    $("#ms-help").hide();
}

function DisableDragAndDrop() {
    // disable drag and drop because we want user to fill in metadata
    g_uploadType = DragDropMode.NOTSUPPORTED;
    SPDragDropManager.DragDropMode = DragDropMode.NOTSUPPORTED;
}



$(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        HideHelpButton();
    }, "sp.js");


    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {

            try {
                var pm = SP.Ribbon.PageManager.get_instance();

                pm.add_ribbonInited(function () {
                    GetListInformation();
                });

                var ribbon = null;
                try {
                    ribbon = pm.get_ribbon();
                }
                catch (e) { }

                if (!ribbon) {
                    if (typeof (_ribbonStartInit) == "function")
                        _ribbonStartInit(_ribbon.initialTabId, false, null);
                }
                else {
                    GetListInformation();
                }
            } catch (e)
            { }
        }, "sp.ribbon.js");
    }, "DragDrop.js");

});