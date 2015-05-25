(function () {
    if (typeof SPClientTemplates === 'undefined')
        return;

    $("<style type='text/css'> .SPDO_Suivi{ padding-left:10px;} </style>").appendTo("head");

    var siteCtx = {};

    siteCtx.Templates = {};

    siteCtx.Templates.Fields = {
        'SPDO_Suivi': { 'View': SPDO_SuiviView }
    };

    //register the template to render custom field
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(siteCtx);
})();

function SPDO_SuiviView(ctx) {
    var currentVal = '';
    //from the context get the current item and it's value
    if (ctx != null && ctx.CurrentItem != null) {
        var $id = "SPDO_Suivi" + ctx.CurrentItem.ID;
        var fileName = "";
        var siteUrl = _spPageContextInfo.siteServerRelativeUrl;
        var webUrl = _spPageContextInfo.webAbsoluteUrl;
        //Access current field value
        var codif = ctx.CurrentItem["Codification"];

        if (codif == "") return;

        codif = cleanUpCodification(codif);
        var familleDoc = ctx.CurrentItem["EDF_Typologie"].Label;

        if (familleDoc != "Déploiement" && familleDoc != "Référentiel") return;

        //get doc info
        $.getJSON(webUrl + "/_vti_bin/speedeau/suivi.svc/getalldocinfo/" + familleDoc + "/" + codif)
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
                    var statusDoc =     getSPDO_SuiviValue(result[i].StatutDoc, "none", true, true);
                    var indice =        getSPDO_SuiviValue(result[i].Indice, null, false, false);
                    var revision =      getSPDO_SuiviValue(result[i].Revision, "bpe", true, false);
                    var auteur =        getSPDO_SuiviValue(result[i].Auteur, null, true, true);
                    var dateModif =     getSPDO_SuiviValue(result[i].DateModif.split(" ")[0], null, true, true);
                    var verification =  getSPDO_SuiviValue(result[i].Verification, "none", true);
                    var link = $('<span class="SPDO_Suivi"><a href="' + result[i].DocUrl + '">' + result[i].DocName + '</a></span>')
                    
                    var div = $("<div/>");
                    div.append($('<span>' + indice + revision + '</span>'));
                    div.append(statusDoc);
                    div.append(verification);
                    div.append(link);
                    div.append(auteur);
                    div.append(dateModif);
                    target.append(div);
                }
            }
        })
        .fail(function () {
            var target = $("#" + $id);
            target.empty();
            target.append($('<span style="color:#807B7B;">NA</span>'));
        });

        return "<div id=" + $id + "><img src='" + siteUrl + "/rnvo_assets/img/ajax-loader-fb.gif' /></div>";

    }
}

function cleanUpCodification(c) {
    return c.replace(/[^0-9a-z]/ig, "");
}


function getSPDO_SuiviValue(value, invalidateValue, addclass, wrap) {
    if (value == null || value == undefined || value.toLowerCase() == invalidateValue) {
        return "";
    }
    if (wrap) {
        var css = addclass ? 'class="SPDO_Suivi"' : "";
        return $('<span ' + css + '>' + value + '</span>');
    } else {
        return value;
    }
}

// from https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/encodeURIComponent
// to be able to encode * in url
function fixedEncodeURIComponent(str) {
    return encodeURIComponent(str).replace(/[!'()*]/g, function (c) {
        return '%' + c.charCodeAt(0).toString(16);
    });
}