
//------------------------- CORE -------------------------------
IH1600.alerte = function () { };
IH1600.alerte.create = function (title, body, categorie) {
    var webRelativeUrl = _spPageContextInfo.webServerRelativeUrl;

    var context = IH1600.core.context;
    var oList = context.get_web().get_lists().getByTitle(IH1600.param.alerteListName);

    var itemCreateInfo = new SP.ListItemCreationInformation();
    var oListItem = oList.addItem(itemCreateInfo);
    oListItem.set_item('Title', title);
    oListItem.set_item('Body', body);
    oListItem.set_item('Categorie_Document', categorie);
    oListItem.update();

    context.load(oListItem);
    context.executeQueryAsync(
        Function.createDelegate(this, function () {
            RNVO.notify.log("Une alerte a été créée.")
        }),
        Function.createDelegate(this, RNVO.common.onQueryFailed)
    );
    
};






IH1600.alerte.registerClass("IH1600.alerte");



//# sourceURL=rnvo.ih1600.alertes.js