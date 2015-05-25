
/******************** fields *******************************************/
// DisplayName, InternalName, isTaxonomyField, jquery selector
IH1600.field = function () { };
IH1600.field.info = {
    Name: new FieldInfo('Nom Champ obligatoire', 'Nom Champ obligatoire', false, '.ms-formbody [title*=Nom]'),
    Title: new FieldInfo('Titre', 'Title', false, '.ms-formbody [title=Titre]'),
    Codification: new FieldInfo('Codification', 'Codification', false, '.ms-formbody [title=\'Codification\']'),
    Site: new FieldInfo('Site DPIH', 'EDF_Site', true, '.ms-formbody [title=Site DPIH] div'),
    Projet: new FieldInfo('Projet', 'EDF_Projet', true, '.ms-formbody [title=Projet] div'),
    Typologie: new FieldInfo('Famille documentaire', 'EDF_Typologie', true, '.ms-taxonomy-writeableregion'),
    StatusBPE: new FieldInfo('Status Documentaire', 'Status', true, '.ms-RadioText[title=BPE] input'),
    StatusPREL: new FieldInfo('Status Documentaire', 'Status', false, '.ms-RadioText[title=PREL] input'),
    StatusCurrent: new FieldInfo('Status Documentaire', 'Status', false, '.ms-RadioText input[type=radio]:checked'),
    Indice: new FieldInfo('Indice', 'EDFVersion', false, '.ms-formbody [title=Indice]'),
    EDFRevision: new FieldInfo('Révision', 'EDFRevision', false, '.ms-formbody [title=Révision]'),
    Verification: new FieldInfo('Vérification', 'validation', false, 'select[title=Vérification]'),
    ProcessIH1600: new FieldInfo('Process IH1600', 'ProcessIH1600', false, '.ms-formbody [title="Process IH1600"]'),
    Save: new FieldInfo('Save', '', false, 'input[value=Enregistrer]:last'),
    Cancel: new FieldInfo('Cancel', '', false, 'input[value=Annuler]')
};

IH1600.field.validation = {
    Title: false,
    Codification: false,
    Site: false,
    Projet: false,
    Status: false,
    Typologie: false,
    Indice: false,
    EDFRevision: false,
};

/// check that current form value follow rules definied for current content type
// if not the save button is disabled
IH1600.field.validate = function () {
    var contentType = IH1600.core.contentType;
    var rules = IH1600.rules.contentType[contentType];
    var sum = 0;
    if (IH1600.field.validation.Codification && rules.codeMandatory) {
        sum += 1;
    }

    if (sum == 1) IH1600.ui.disableSave(false)
    else IH1600.ui.disableSave(true);
}

IH1600.field.registerClass("IH1600.field");


//*************************** Messages *****************************/
IH1600.msg = function () { };
IH1600.msg.emptyIH1600WithBPE = 'le code IH1600 doit être renseigné pour une version BPE';
IH1600.msg.invalidCodeIH = "code ih1600 invalide";
IH1600.msg.inconsistentCodeIH = "Le code IH1600 saisi ne correspond pas à celui associé au fichier. Merci de vérifier.";
IH1600.msg.fileUploaded = "Votre fichier a bien été téléchargé.";
IH1600.msg.fileError = "Une erreur est survenue lors de l'enregistrement.";
IH1600.msg.noFile = "Veuillez sélectionner un fichier.";
IH1600.msg.wrongIndice = "L'indice de cette nouvelle version doit être supérieure à celui de l'ancienne version."
IH1600.msg.wrongRevision = "La révision doit être supérieure à celle de la version précédente";
IH1600.msg.registerClass("IH1600.msg");


/********************* régles de gestion *************************/

IH1600.rules = function () { };
IH1600.rules.contentType = {
    "Deploiement": {
        codeMandatory: true,
        defaultStatus: IH1600.enum.status.BPE,
        prelEnable: false
    },
    "Referentiel": {
        codeMandatory: true,
        defaultStatus: IH1600.enum.status.BPE,
        prelEnable: true
    },
    "equipe": {
        codeMandatory: true,
        defaultStatus: IH1600.enum.status.BPE,
        prelEnable: true
    },
    "affaire": {
        codeMandatory: true,
        defaultStatus: IH1600.enum.status.BPE,
        prelEnable: true
    }
};

IH1600.rules.registerClass("IH1600.rules");

SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('rnvo.ih1600.validation');

//# sourceURL=rnvo.ih1600.validation.js