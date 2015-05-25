Type.registerNamespace("VAL");

//$(function () {
//    var scriptbase = _spPageContextInfo.webServerRelativeUrl + "/_layouts/15/";
//    $.getScript(scriptbase + "SP.Runtime.js",
//        function () {
//            $.getScript(scriptbase + "SP.js", Init);
//        }
//    );
//});

//SP.SOD.executeFunc('sp.js', 'SP.ClientContext', Init);


// 12/05/2015 - using executeOrDelayUntilScriptLoaded instead of executeFunc because click handler fails to trigger otherwise (actually works with developper tools enable...)
SP.SOD.executeOrDelayUntilScriptLoaded(Init, "SP.js");



function Init() {
    // disable select control - value are not up to the user.
    $("select").prop("disabled", true)

    var queryString = $.getQueryParameters();
    if (queryString.currentId == undefined) RNVO.notify.error("Missing parameter exception");
    else {
        VAL.core.context = SP.ClientContext.get_current();
        VAL.core.library = VAL.core.context.get_web().get_lists().getByTitle(IH1600.param.depLibraryName);
        VAL.core.obsList = VAL.core.context.get_web().get_lists().getByTitle(IH1600.param.depObsListName);
        VAL.form.init(queryString.currentId);
    }
}


//------------------------- CORE -------------------------------
VAL.core = function () { };
VAL.core.context = null;
VAL.core.library = null;
VAL.core.obsList = null;

VAL.form = function () { };
VAL.form.obsListItemColl = null;
VAL.form.selectedId = null;
VAL.form.targetRevision = null;
VAL.form.targetVersion = null;
VAL.form.obsColl = [];
VAL.form.obsViewUrl = null;
VAL.form.depItem = null;

VAL.form.init = function (id) {
    VAL.form.selectedId = id;
    VAL.form.checkCurrentValidation();
    VAL.form.setBehavior();
}

VAL.form.checkCurrentValidation = function () {

    //get current validation status for selected item
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml('<View><Query><Where><Eq><FieldRef Name="ID" /><Value Type="Counter">' + VAL.form.selectedId + '</Value></Eq></Where></Query>' +
                            '<ViewFields>' +
                                '<FieldRef Name="' + IH1600.field.info.Verification.internalName + '" />' +
                                '<FieldRef Name="' + IH1600.field.info.Indice.internalName + '" />' +
                                '<FieldRef Name="' + IH1600.field.info.EDFRevision.internalName + '" />' +
                             '</ViewFields>' +
                            '<RowLimit>1</RowLimit></View>');
    VAL.form.depItem = VAL.core.library.getItems(camlQuery);
    VAL.core.context.load(VAL.form.depItem);
    VAL.core.context.executeQueryAsync(function () {
        var enumerator = VAL.form.depItem.getEnumerator();
        enumerator.moveNext();
        var item = enumerator.get_current();
        VAL.form.targetVersion = item.get_item(IH1600.field.info.Indice.internalName);
        VAL.form.targetRevision = item.get_item(IH1600.field.info.EDFRevision.internalName);

        // cannot validate a document with no indice
        if (VAL.form.targetVersion == null) {
            RNVO.notify.warn("Ce document n'a pas d'indice et de ce fait ne peux pas être valider.");
            $("#val-loading").hide();
            $("#rnvo-val-MenuContainer").show({ complete: RNVO.common.resizeDialog });
            $("#rnvo-val-btn-close").show();
            return;
        }
        
        var validation = item.get_item(IH1600.field.info.Verification.internalName);
        if (validation == "None") {
            // no validation status yet, proceeding
            VAL.form.getObservation();
            $("#rnvo-val-btns").show();
        }
        else {
            RNVO.notify.warn("Ce document a déjà été validé en " + validation);
            $("#val-loading").hide();
            $("#rnvo-val-MenuContainer").show({ complete: RNVO.common.resizeDialog });
            $("#rnvo-val-btn-close").show();
        }
    }
        , RNVO.common.onQueryFailed);
};

VAL.form.getObservation = function () {

    var revisionCaml = '<Eq><FieldRef Name="' + IH1600.field.info.EDFRevision.internalName + '" /><Value Type="Text">' + VAL.form.targetRevision + '</Value></Eq>';
    if (VAL.form.targetRevision == null) revisionCaml = "<IsNull><FieldRef Name='EDFRevision' /></IsNull>";


    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml('<View>' +
                            '<Query>' +
                                '<Where>' +
                                    '<And>' +
                                        '<And>' + revisionCaml +
                                            '<Eq>' +
                                                '<FieldRef Name="' + IH1600.field.info.Indice.internalName + '" />' +
                                                '<Value Type="Text">' + VAL.form.targetVersion + '</Value>' +
                                            '</Eq>' +
                                        '</And>' +
                                        '<Eq>' +
                                            '<FieldRef Name="document" LookupId="True" />' +
                                            '<Value Type="Lookup">' + VAL.form.selectedId + '</Value>' +
                                        '</Eq>' +
                                    '</And>' +
                                '</Where>' +
                            '</Query>' +
                            '<ViewFields><FieldRef Name="Title" /><FieldRef Name="observation" /></ViewFields>' +
                          '</View>');
    VAL.form.obsListItemColl = VAL.core.obsList.getItems(camlQuery);
    VAL.core.context.load(VAL.form.obsListItemColl);
    VAL.core.context.load(VAL.core.obsList, 'DefaultDisplayFormUrl');
    VAL.core.context.executeQueryAsync(VAL.form.onObsReceived, RNVO.common.onQueryFailed);
};

VAL.form.onObsReceived = function () {
    VAL.form.obsViewUrl = VAL.core.obsList.get_defaultDisplayFormUrl();
    var listItemEnumerator = VAL.form.obsListItemColl.getEnumerator();
    while (listItemEnumerator.moveNext()) {
        var oListItem = listItemEnumerator.get_current();
        var obs = new Obs(oListItem);
        VAL.form.obsColl.push(obs);
    }
    VAL.form.displaySelect();
};

VAL.form.displaySelect = function () {
    if (VAL.form.obsColl.length == 0) {
        // no obs found
        $(".rnvo-val-noobs-menu").show();
    }
    else {
        // show menu
        $(".rnvo-val-obs-menu").show();
        // display obs
        var obsList = $("#rnvo-val-obs-container");
        obsList.show();
        for (var i = 0; i < VAL.form.obsColl.length; i++) {
            var curr = VAL.form.obsColl[i];
            obsList.append($('<li><a href="#" class="rnvo-val-obs-link" data-id="' + curr.id + '">' + curr.title + '</a></li>'));
        }

        $(".rnvo-val-obs-link").click(function () {
            var currLink = $(this);
            var id = currLink.attr("data-id");
            var options = SP.UI.$create_DialogOptions();
            options.title = currLink.text();
            options.width = 600;
            options.height = 600;
            options.url = VAL.form.obsViewUrl + "?ID=" + id;

            SP.UI.ModalDialog.showModalDialog(options);
        });
    }

    $("#val-loading").hide();
    $("#rnvo-val-MenuContainer").slideDown({ complete: RNVO.common.resizeDialog });
};

VAL.form.setBehavior = function () {

    $("#rnvo-val-fermer").click(function (result) {
        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel);
    })

    $("#rnvo-val-annuler").click(function () {
        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel);
    });

    $("#rnvo-val-valider").click(function () {

        var ok = confirm("La saisie de commentaires ne sera plus possible sur cette version du document.\r\nÊtes vous sûr de vouloir terminer la vérification et mettre à jour le statut de relecture ?");

        if (ok == false) return;

        // disable buttons
        $(this).prop("disabled", true);
        $("#rnvo-val-annuler").prop("disabled", true);

        var value = $('input[name=validation-value]:checked').val();

        if (value == undefined || value == "") {
            RNVO.notify.warn("Veuillez sélectionner une valeur.");
            return;
        }

        var oListItem = VAL.core.library.getItemById(VAL.form.selectedId);
        oListItem.set_item('validation', value);
        oListItem.update();

        VAL.core.context.executeQueryAsync(function () {
            RNVO.notify.log("Validation sauvegardée avec succès.");
            $.ajax(_spPageContextInfo.webAbsoluteUrl + "/_vti_bin/speedeau/alerte.svc/notifyauthor/" + IH1600.param.depLibraryName + "/" + VAL.form.selectedId + "/" + value)
                    .done(function (r) {
                        if (r.NotifyAuthorResult) {
                            var statusId = SP.UI.Status.addStatus("L'auteur du document a été notifié par email");
                            SP.UI.Status.setStatusPriColor(statusId, 'green');
                            setTimeout(function () { SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK); }, 3000);
                        }
                        else {
                            var statusId = SP.UI.Status.addStatus("Un problème est survenu lors de la notification!");
                            SP.UI.Status.setStatusPriColor(statusId, 'red');
                        }
                    });
        },
            RNVO.common.onQueryFailed
         );
    });
}

//# sourceURL=rnvo.validation.main.js
