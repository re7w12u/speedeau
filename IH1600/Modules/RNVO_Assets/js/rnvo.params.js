Type.registerNamespace("IH1600");

// TODO : fetch param from custom list

IH1600.param = function () { };
IH1600.param.depLibraryName = "Déploiement";
IH1600.param.refLibraryName = "Référentiel";
IH1600.param.depObsListName = "Observations Déploiement";
IH1600.param.refObsListName = "Observations Référentiel";
IH1600.param.alerteListName = "Alertes";
IH1600.param.alerteValidationListName = "Alertes Validation";
IH1600.param.suiviListName = "Liste de Suivi";
IH1600.param.registerClass("IH1600.param");


//------------------------- ENUM -------------------------------
IH1600.enum = function () { };
IH1600.enum.validation = { VSO: "VSO", VSOSC: "VSO-SC", VSOSV: "VSO-SV", VAO: "VAO" };
IH1600.enum.status = { BPE: "BPE", PREL: "PREL" };
IH1600.enum.contentType = { OBS: "Observations", REF: "Referentiel", DEP: "Deploiement" };
IH1600.enum.registerClass("IH1600.enum");

//--------------------------------------------------------------
RNVO.def = function () { };
RNVO.def.fields = function () { };
RNVO.def.fields.obs = function () { };
RNVO.def.fields.obs.doc = "document";

SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('rnvo.params');