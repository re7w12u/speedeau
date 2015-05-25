(function () {
    if (typeof SPClientTemplates === 'undefined')
        return;

    var siteCtx = {};

    siteCtx.Templates = {};

    siteCtx.Templates.Fields = {
        //MyCustomField is the Name of our field
        'SPDO_Suivi_Verif': {
            'View': customView
        }
    };

    //register the template to render custom field
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(siteCtx);
})();

function customView(ctx) {
    var currentVal = '';
    //from the context get the current item and it's value
    if (ctx != null && ctx.CurrentItem != null) {
        var $id = "SPDO_Suivi_Verif" + ctx.CurrentItem.ID;
        var fileName = "";
        var siteUrl = _spPageContextInfo.siteServerRelativeUrl;
        var webUrl = _spPageContextInfo.webAbsoluteUrl;
        //Access current field value
        var codif = ctx.CurrentItem["Codification"];

        //get property bag
        $.when($.getJSON(webUrl + "/_vti_bin/speedeau/suivi.svc/getalldocinfo/" + codif))
        .done(function (r) {
            if (r.GetAllDocInfoResult.length == 0) {
                var target = $("#" + $id);
                target.empty();
                target.append($('<span style="color:#807B7B;">-</span>'));
            }
            else {
                var target = $("#" + $id);
                target.empty();
                var result = r.GetAllDocInfoResult
                for (var i = 0; i < result.length; i++) {
                    target.append($('<div class="SPDO_Suivi">' + result[i].Verification + '</div>'));
                }
            }
        });

        return "<span id=" + $id + "><img src='" + siteUrl + "/rnvo_assets/img/ajax-loader-fb.gif' /></span>";

    }
}