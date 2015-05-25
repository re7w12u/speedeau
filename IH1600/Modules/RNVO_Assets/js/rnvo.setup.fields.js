// provides function for Custom Action button


$(function () {
    var scriptbase = _spPageContextInfo.webServerRelativeUrl + "/_layouts/15/";
    $.getScript(scriptbase + "SP.Runtime.js",
        function () {
            $.getScript(scriptbase + "SP.js", InitSetup);
        }
    );
});


var $context = null;


function InitSetup() {
    $context = SP.ClientContext.get_current();



    $("#setup-DepTaxonomyField").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        SetupDeploiementTaxonmyFields(span);
    });
}

function SetupLog(msg) {
    statusId = SP.UI.Status.addStatus(msg);
    SP.UI.Status.setStatusPriColor(statusId, 'green');
}

function SetupErr(sender, args) {
    statusId = SP.UI.Status.addStatus('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
    SP.UI.Status.setStatusPriColor(statusId, 'red');
}


function SetupDeploiementTaxonmyFields(span) {
    var locale = SP.Res.lcid;
    new TaxonBinder().execute("EDF_Site", "Cartographie sites DPIH", locale);
    new TaxonBinder().execute("EDF_Projet", "Projets P＆S", locale);
    new TaxonBinder().execute("Famille_Documentaire", "Typologies docs P＆S", locale);
}
