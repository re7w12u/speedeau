(function () {
    if (typeof SPClientTemplates === 'undefined')
        return;

    $("<style type='text/css'> .SPDO_Suivi{ padding-left:10px;} </style>").appendTo("head");

    var siteCtx = {};

    siteCtx.Templates = {};

    siteCtx.Templates.Fields = {
        //MyCustomField is the Name of our field
        'SPDO_Link_Suivi': {
            'View': SPDO_Link_Suivi_View
        }
    };

    //register the template to render custom field
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(siteCtx);
})();

function SPDO_Link_Suivi_View(ctx) {
    var currentVal = '';
    //from the context get the current item and it's value
    if (ctx != null && ctx.CurrentItem != null) {
        var $id = "SPDO_Link_Suivi_View" + ctx.CurrentItem.ID;
        var fileName = "";
        var siteUrl = _spPageContextInfo.siteServerRelativeUrl;
        var webUrl = _spPageContextInfo.webAbsoluteUrl;
        //Access current field value
        var codif = ctx.CurrentItem["Codification"];

        if (codif == "") {
            return;
        }
        
        //get property bag
        $.when($.getJSON(webUrl + "/_vti_bin/speedeau/suivi.svc/getalldocinfo/" + codif))
        .done(function (r) {
            if (r.GetAllDocInfoResult.length == 0) {
                var target = $("#" + $id);
                target.empty();
                target.append($('<span style="color:#807B7B;">aucun fichier</span>'));
            }
            else {
                var target = $("#" + $id);
                target.empty();
                var result = r.GetAllDocInfoResult
                for (var i = 0; i < result.length; i++) {
                    target.append($('<a href="' + result[i].DocUrl + '">' + result[i].DocName + '</a>'));
                    target.append($('<span class="SPDO_Suivi">' + result[i].Auteur + '</span>'));
                    target.append($('<span class="SPDO_Suivi">' + result[i].DateModif.split(" ")[0] + '</span>'));
                    target.append($('<span class="SPDO_Suivi">' + result[i].Indice + result[i].Revision +  " " + result[i].StatutDoc +'</span>'));
                    target.append($('<span class="SPDO_Suivi">' + result[i].Verification + '</span>'));
                }
            }
        });

        return "<div id=" + $id + "><img src='" + siteUrl + "/rnvo_assets/img/ajax-loader-fb.gif' /></div>";

    }
}