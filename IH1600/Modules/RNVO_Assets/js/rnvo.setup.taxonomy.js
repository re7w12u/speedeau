'use strict';

function TaxonBinder() {
    this.context;
    this.termStore;
    this.termSets;
    this.termSet;
    this.termSetName;
    this.fieldInternalName;
    this.termSetLocale;
    this.field;
    this.taxField;
    this.dialog;
    this.dialogTitle = "Please wait";
    this.dialogMessage = "We are getting your site ready..";
    this.dialogHeight = 500;
    this.dialogWidth = 500;

    this.execute = function (_fieldInternalName, _termSetName, _termSetLocale) {
        var self = this;
        SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
            //console.log("Loaded sp.js");
            SP.SOD.registerSod('sp.taxonomy.js', SP.Utilities.Utility.getLayoutsPageUrl('sp.taxonomy.js'));
            SP.SOD.executeFunc('sp.taxonomy.js', 'SP.Taxonomy.TaxonomySession', function () {
                //console.log("Loaded sp.taxonomy.js");
                SP.SOD.executeFunc('sp.ui.dialog.js', 'SP.UI.ModalDialog.showWaitScreenWithNoClose', function () {
                    //console.log("Loaded all files..");
                    self.initTaxObjects(_fieldInternalName, _termSetName, _termSetLocale);
                });
            });
        });
    };
    this.initTaxObjects = function (fieldToBindInternalName, termSetToBindName, termSetToBindLocale) {
       // console.log("In initTaxObjects()..");
        var self = this;
        this.fieldInternalName = fieldToBindInternalName;
        this.termSetName = termSetToBindName;
        this.termSetLocale = termSetToBindLocale;

        // show dialog during execution of our provisioning steps..
        this.dialog = SP.UI.ModalDialog.showWaitScreenWithNoClose(this.dialogTitle, this.dialogMessage, this.dialogHeight, this.dialogWidth);

        $('#jsomProvisioningMessage').append("<br /><div>Attempting to fetch term set '" + this.termSetName + "' and field '" + this.fieldInternalName + "'.");

        this.context = SP.ClientContext.get_current();
        var taxonomySession = SP.Taxonomy.TaxonomySession.getTaxonomySession(this.context);
        this.context.load(taxonomySession);

        this.termStore = taxonomySession.getDefaultSiteCollectionTermStore();
        this.termSets = taxonomySession.getTermSetsByName(this.termSetName, this.termSetLocale);
        this.context.load(this.termStore);
        this.context.load(this.termSets);

        this.field = this.context.get_site().get_rootWeb().get_fields().getByInternalNameOrTitle(this.fieldInternalName);
        this.context.load(this.field);
        this.context.executeQueryAsync(
            function () {
                //console.log("In onInitTaxObjectsSuccess()..");

                var termSetEnumerator = self.termSets.getEnumerator();

                while (termSetEnumerator.moveNext()) {
                    var currentTermSet = termSetEnumerator.get_current();
                    var currentTermSetName = currentTermSet.get_name();
                    if (currentTermSetName === self.termSetName) {
                        //console.log("Found term set " + self.termSetName);
                        self.termSet = currentTermSet;
                    }
                }

                if (self.termSet != undefined) {
                    //console.log("Term set identified, proceeding..");
                    self.bindTaxonomyField()
                }
                else {
                    var msg = "Failed to find term set '" + self.termSetName + "'. Please check this exists in your Term Store!";
                    //console.log(msg);
                    $('#jsomProvisioningMessage').append(msg);
                    if (self.dialog != null) {
                        self.dialog.close();
                    }
                }
            }
            , self.onInitTaxObjectsFail);
    };


    this.onInitTaxObjectsFail = function (sender, args) {
        alert('Failed to initialise taxonomy objects! Error:' + sender.statusCode);
    };

    this.bindTaxonomyField = function () {
        //console.log("In bindTaxonomyField()..");
        var self = this;
        this.taxField = this.context.castTo(this.field, SP.Taxonomy.TaxonomyField);
        this.taxField.set_sspId(this.termStore.get_id().toString());
        this.taxField.set_termSetId(this.termSet.get_id());

        this.taxField.updateAndPushChanges(true);
        this.context.executeQueryAsync(
            function () {
                //console.log("In onBindTaxFieldSuccess()..");
                // now we need to close the dialog, BUT I'm pausing for 4 seconds here for demo purposes using setTimeout() - things go too 
                // fast otherwise. The setTimeout() should be removed in real use.. 
                setTimeout(function () {
                    $('#jsomProvisioningMessage').append('<br /><div>Taxonomy field bound successfully..</div>');
                    if (self.dialog != null) {
                        self.dialog.close();
                    }
                }, 4000);
            }, self.onBindTaxFieldFail);
    };

    

    this.onBindTaxFieldFail = function (sender, args) {
        if (this.dialog != null) {
            this.dialog.close();
        }
        alert('Failed to bind taxonomy field! Error:' + sender.statusCode);
    };
}


//function TaxonBinder() {
//    this.context;
//    this.termStore;
//    this.termSets;
//    this.termSet;
//    this.termSetName;
//    this.fieldInternalName;
//    this.termSetLocale;
//    this.field;
//    this.taxField;
//    this.dialog;
//    this.dialogTitle = "Please wait";
//    this.dialogMessage = "We are getting your site ready..";
//    this.dialogHeight = 500;
//    this.dialogWidth = 500;
//};
//TaxonBinder.prototype.execute = function (_fieldInternalName, _termSetName, _termSetLocale) {
//    var self = this;
//    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
//        console.log("Loaded sp.js");
//        SP.SOD.registerSod('sp.taxonomy.js', SP.Utilities.Utility.getLayoutsPageUrl('sp.taxonomy.js'));
//        SP.SOD.executeFunc('sp.taxonomy.js', 'SP.Taxonomy.TaxonomySession', function () {
//            console.log("Loaded sp.taxonomy.js");
//            SP.SOD.executeFunc('sp.ui.dialog.js', 'SP.UI.ModalDialog.showWaitScreenWithNoClose', function () {
//                console.log("Loaded all files..");
//                self.initTaxObjects(_fieldInternalName, _termSetName, _termSetLocale);
//            });
//        });
//    });
//}

//TaxonBinder.prototype.initTaxObjects = function (fieldToBindInternalName, termSetToBindName, termSetToBindLocale) {
//    console.log("In initTaxObjects()..");

//    this.fieldInternalName = fieldToBindInternalName;
//    this.termSetName = termSetToBindName;
//    this.termSetLocale = termSetToBindLocale;

//    // show dialog during execution of our provisioning steps..
//    this.dialog = SP.UI.ModalDialog.showWaitScreenWithNoClose(this.dialogTitle, this.dialogMessage, this.dialogHeight, this.dialogWidth);

//    $('#jsomProvisioningMessage').append("<br /><div>Attempting to fetch term set '" + this.termSetName + "' and field '" + this.fieldInternalName + "'.");

//    this.context = SP.ClientContext.get_current();
//    var taxonomySession = SP.Taxonomy.TaxonomySession.getTaxonomySession(this.context);
//    this.context.load(taxonomySession);

//    this.termStore = taxonomySession.getDefaultSiteCollectionTermStore();
//    this.termSets = taxonomySession.getTermSetsByName(this.termSetName, this.termSetLocale);
//    this.context.load(this.termStore);
//    this.context.load(this.termSets);

//    this.field = this.context.get_site().get_rootWeb().get_fields().getByInternalNameOrTitle(this.fieldInternalName);
//    this.context.load(this.field);
//    this.context.executeQueryAsync(this.onInitTaxObjectsSuccess, this.onInitTaxObjectsFail);
//};
//TaxonBinder.prototype.onInitTaxObjectsSuccess = function () {
//    console.log("In onInitTaxObjectsSuccess()..");

//    var termSetEnumerator = this.termSets.getEnumerator();

//    while (termSetEnumerator.moveNext()) {
//        var currentTermSet = termSetEnumerator.get_current();
//        var currentTermSetName = currentTermSet.get_name();
//        if (currentTermSetName === this.termSetName) {
//            console.log("Found term set " + this.termSetName);
//            this.termSet = currentTermSet;
//        }
//    }

//    if (this.termSet != undefined) {
//        console.log("Term set identified, proceeding..");
//        this.bindTaxonomyField()
//    }
//    else {
//        var msg = "Failed to find term set '" + this.termSetName + "'. Please check this exists in your Term Store!";
//        console.log(msg);
//        $('#jsomProvisioningMessage').append(msg);
//        if (this.dialog != null) {
//            this.dialog.close();
//        }
//    }
//};
//TaxonBinder.prototype.onInitTaxObjectsFail = function (sender, args) {
//    alert('Failed to initialise taxonomy objects! Error:' + sender.statusCode);
//};
//TaxonBinder.prototype.bindTaxonomyField = function () {
//    console.log("In window.COB.JsomProvisioning.bindTaxonomyField()..");

//    this.taxField = this.context.castTo(this.field, SP.Taxonomy.TaxonomyField);
//    this.taxField.set_sspId(this.termStore.get_id().toString());
//    this.taxField.set_termSetId(this.termSet.get_id());

//    this.taxField.updateAndPushChanges(true);
//    this.context.executeQueryAsync(this.onBindTaxFieldSuccess, this.onBindTaxFieldFail);
//};
//TaxonBinder.prototype.onBindTaxFieldSuccess = function () {
//    console.log("In window.COB.JsomProvisioning.onBindTaxFieldSuccess()..");
//    // now we need to close the dialog, BUT I'm pausing for 4 seconds here for demo purposes using setTimeout() - things go too 
//    // fast otherwise. The setTimeout() should be removed in real use.. 
//    setTimeout(function () {
//        $('#jsomProvisioningMessage').append('<br /><div>Taxonomy field bound successfully..</div>');
//        if (this.dialog != null) {
//            this.dialog.close();
//        }
//    }, 4000);
//};
//TaxonBinder.prototype.onBindTaxFieldFail = function (sender, args) {
//    if (this.dialog != null) {
//        this.dialog.close();
//    }
//    alert('Failed to bind taxonomy field! Error:' + sender.statusCode);
//};

/******************************************************************/
/*
window.COB = window.COB || {};

//window.COB.TermSetName = "Countries";
window.COB.TermSetLocale = "1033";
//window.COB.FieldInternalName = "COB_Countries";

window.COB.JsomProvisioning = function () {
    var context,
        termStore,
        termSets,
        termSet,
        termSetName,
        fieldInternalName,
        termSetLocale,
        field,
        taxField,
        dialog,
        dialogTitle = "Please wait",
        dialogMessage = "We are getting your site ready..",
        dialogHeight = 500,
        dialogWidth = 500,

    initTaxObjects = function (fieldToBindInternalName, termSetToBindName, termSetToBindLocale) {
        console.log("In window.COB.JsomProvisioning.initTaxObjects()..");

        fieldInternalName = fieldToBindInternalName;
        termSetName = termSetToBindName;
        termSetLocale = termSetToBindLocale;

        // show dialog during execution of our provisioning steps..
        dialog = SP.UI.ModalDialog.showWaitScreenWithNoClose(dialogTitle, dialogMessage, dialogHeight, dialogWidth);

        $('#jsomProvisioningMessage').append("<br /><div>Attempting to fetch term set '" + termSetName + "' and field '" + fieldInternalName + "'.");

        context = SP.ClientContext.get_current();
        var taxonomySession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
        context.load(taxonomySession);

        termStore = taxonomySession.getDefaultSiteCollectionTermStore();
        termSets = taxonomySession.getTermSetsByName(termSetName, termSetLocale);
        context.load(termStore);
        context.load(termSets);

        field = context.get_site().get_rootWeb().get_fields().getByInternalNameOrTitle(fieldInternalName);
        context.load(field);
        context.executeQueryAsync(onInitTaxObjectsSuccess, onInitTaxObjectsFail);
    },
    onInitTaxObjectsSuccess = function () {
        console.log("In window.COB.JsomProvisioning.onInitTaxObjectsSuccess()..");

        var termSetEnumerator = termSets.getEnumerator();

        while (termSetEnumerator.moveNext()) {
            var currentTermSet = termSetEnumerator.get_current();
            var currentTermSetName = currentTermSet.get_name();
            if (currentTermSetName === termSetName) {
                console.log("Found term set " + termSetName);
                termSet = currentTermSet;
            }
        }

        if (termSet != undefined) {
            console.log("Term set identified, proceeding..");
            bindTaxonomyField()
        }
        else {
            var msg = "Failed to find term set '" + termSetName + "'. Please check this exists in your Term Store!";
            console.log(msg);
            $('#jsomProvisioningMessage').append(msg);
            if (dialog != null) {
                dialog.close();
            }
        }
    },
    onInitTaxObjectsFail = function (sender, args) {
        alert('Failed to initialise taxonomy objects! Error:' + sender.statusCode);
    },

    bindTaxonomyField = function () {
        console.log("In window.COB.JsomProvisioning.bindTaxonomyField()..");

        taxField = context.castTo(field, SP.Taxonomy.TaxonomyField);
        taxField.set_sspId(termStore.get_id().toString());
        taxField.set_termSetId(termSet.get_id());

        taxField.updateAndPushChanges(true);
        context.executeQueryAsync(onBindTaxFieldSuccess, onBindTaxFieldFail);
    },
    onBindTaxFieldSuccess = function () {
        console.log("In window.COB.JsomProvisioning.onBindTaxFieldSuccess()..");
        // now we need to close the dialog, BUT I'm pausing for 4 seconds here for demo purposes using setTimeout() - things go too 
        // fast otherwise. The setTimeout() should be removed in real use.. 
        setTimeout(function () {
            $('#jsomProvisioningMessage').append('<br /><div>Taxonomy field bound successfully..</div>');
            if (dialog != null) {
                dialog.close();
            }
        }, 4000);
    },
    onBindTaxFieldFail = function (sender, args) {
        if (dialog != null) {
            dialog.close();
        }
        alert('Failed to bind taxonomy field! Error:' + sender.statusCode);
    }

    return {
        execute: function (_fieldInternalName, _termSetName) {
            console.log("In window.COB.JsomProvisioning.execute()..");

            SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
                console.log("Loaded sp.js");
                SP.SOD.registerSod('sp.taxonomy.js', SP.Utilities.Utility.getLayoutsPageUrl('sp.taxonomy.js'));
                SP.SOD.executeFunc('sp.taxonomy.js', 'SP.Taxonomy.TaxonomySession', function () {
                    console.log("Loaded sp.taxonomy.js");
                    SP.SOD.executeFunc('sp.ui.dialog.js', 'SP.UI.ModalDialog.showWaitScreenWithNoClose', function () {
                        console.log("Loaded all files..");
                        initTaxObjects(_fieldInternalName, _termSetName, window.COB.TermSetLocale);
                    });
                });
            });
        }
    }
}();
*/