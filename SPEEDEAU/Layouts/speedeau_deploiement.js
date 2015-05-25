(function () {
    var overrideContext = {};
    overrideContext.BaseViewID = 1;
    overrideContext.ListTemplateType = 10100;
    overrideContext.Templates = {};
    //overrideContext.Templates.Header = "<h2><a href='#' onclick='javascript:OpenDeploiementForm();'>Télécharger un nouveau fichier de déploiement</a></h2>";
    //overrideContext.Templates.Fields = { 'PopularityPercent': { 'View': PopularityViewTemplate } };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideContext);
})();
function PopularityViewTemplate(ctx) {
    var popularity = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    var color = getColor(popularity);
    return "&nbsp;&nbsp;&nbsp;&nbsp;<span style='background-color : " + color + "' >&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;" + popularity + "%";
}

function OpenDeploiementForm(){
    var url = _spPageContextInfo.webServerRelativeUrl + "/" + _spPageContextInfo.layoutsUrl + "/speedeau/check_codification.aspx?";

    //var options = SP.UI.$create_DialogOptions();
    //options.title = "My Dialog Title";
    //options.width = 400;
    //options.height = 600;
    //options.url = url;
    //SP.UI.ModalDialog.showModalDialog(options);

    //STSNavigate(url);    
}