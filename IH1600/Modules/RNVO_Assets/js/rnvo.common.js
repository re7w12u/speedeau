//--------------------------------------------------------------------
Type.registerNamespace("RNVO");



RNVO.common = function () { };
RNVO.common.onQueryFailed = function (sender, args) {
    /// <summary>generic function handling async request error</summary>
    //console.log('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
}

RNVO.common.resizeDialog = function AutosizeDialog() {
    //resize dialog if we are in one
    var dlg = SP.UI.ModalDialog.get_childDialog();
    if (dlg != null) {
        dlg.autoSize();
    }
}

RNVO.common.ensureValue = function (listItem, columnName) {
    try {
        return listItem.get_item(columnName);
    } catch (e) {
        //console.log('Could not get value for '+columnName+' in listitem id ='+listItem.get_item('Title')+' - ' + e.message);
    }
}

RNVO.common.registerClass("RNVO.common");


// -------------- jquery extension ----------------------------------

jQuery.extend({

    getQueryParameters: function (str) {
        return (str || document.location.search).replace(/(^\?)/, '').split("&").map(function (n) { return n = n.split("="), this[n[0]] = n[1], this }.bind({}))[0];
    }

});


SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('rnvo.common');