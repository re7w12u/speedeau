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

    $("#setup-dep-newObsForm").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        AddCEWPToNewObsForm(span);
        AddCEWPToDispObsForm(span);
    });

    $("#setup-EditDepForm").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        AddCEWPToEditDeploiementForm(span);
        AddCEWPToDispDeploiementForm(span);
    });

    $("#setup-LookupField").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        SetUpList();
    });

    $("#setup-RefList").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        setupReferentielLibrary(span);
    });

    $("#setup-editRefForm").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        AddCEWPToEditReferentielForm(span)
    });

    $("#setup-AlerteList").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        CreateAlertList(span);
    });

    $("#setup-MasterPage").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        ChangeMasterPage(span);
    });

    $("#setup-SuiviList").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        CreateListeType(span);
    });

    $("#setup-AlerteValidationList").click(function (evt) {
        evt.preventDefault();
        var span = $(this).next();
        span.text("loading");
        CreateAlertValidationList(span);
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

function AddCEWPToNewObsForm(span) {
    var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/Lists/obs-dep/NewForm.aspx"
    var oFile = $context.get_web().getFileByServerRelativeUrl(serverRelativeUrl);

    var limitedWebPartManager = oFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);

    var webPartXml = '<?xml version=\"1.0\" encoding=\"utf-8\"?>' +
        '<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' +
        ' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' +
        ' xmlns=\"http://schemas.microsoft.com/WebPart/v2\">' +
        '<Title>My Web Part</Title><FrameType>None</FrameType>' +
        '<Description>Use for formatted text, tables, and images.</Description>' +
        '<IsIncluded>true</IsIncluded><ZoneID></ZoneID><PartOrder>0</PartOrder>' +
        '<FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove>' +
        '<AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize>' +
        '<AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit>' +
        '<AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink />' +
        '<HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall />' +
        '<MissingAssembly>Cannot import this Web Part.</MissingAssembly>' +
        '<PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter />' +
        '<Assembly>Microsoft.SharePoint, Version=13.0.0.0, Culture=neutral, ' +
        'PublicKeyToken=94de0004b6e3fcc5</Assembly>' +
        '<TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>' +
        '<ContentLink xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor">' + _spPageContextInfo.siteServerRelativeUrl + '/RNVO_Assets/NewObsForm.html</ContentLink>' +
        '<Content xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor" />' +
        '<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>';

    var oWebPartDefinition = limitedWebPartManager.importWebPart(webPartXml);
    this.oWebPart = oWebPartDefinition.get_webPart();

    limitedWebPartManager.addWebPart(oWebPart, 'Main', 1);

    $context.load(oWebPart);

    $context.executeQueryAsync(
        function () {
            span.text("ok");
            SetupLog("Observation New form set up successfully");
        },
        function (sender, args) {
            span.text("failed");
            SetupErr(sender, args);
        }
     );

}

function AddCEWPToDispObsForm(span) {
    var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/Lists/obs-dep/DispForm.aspx"
    var oFile = $context.get_web().getFileByServerRelativeUrl(serverRelativeUrl);

    var limitedWebPartManager = oFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);

    var webPartXml = '<?xml version=\"1.0\" encoding=\"utf-8\"?>' +
        '<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' +
        ' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' +
        ' xmlns=\"http://schemas.microsoft.com/WebPart/v2\">' +
        '<Title>My Web Part</Title><FrameType>None</FrameType>' +
        '<Description>Use for formatted text, tables, and images.</Description>' +
        '<IsIncluded>true</IsIncluded><ZoneID></ZoneID><PartOrder>0</PartOrder>' +
        '<FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove>' +
        '<AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize>' +
        '<AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit>' +
        '<AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink />' +
        '<HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall />' +
        '<MissingAssembly>Cannot import this Web Part.</MissingAssembly>' +
        '<PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter />' +
        '<Assembly>Microsoft.SharePoint, Version=13.0.0.0, Culture=neutral, ' +
        'PublicKeyToken=94de0004b6e3fcc5</Assembly>' +
        '<TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>' +
        '<ContentLink xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor">' + _spPageContextInfo.siteServerRelativeUrl + '/RNVO_Assets/DispForm.html</ContentLink>' +
        '<Content xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor" />' +
        '<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>';

    var oWebPartDefinition = limitedWebPartManager.importWebPart(webPartXml);
    this.oWebPart = oWebPartDefinition.get_webPart();

    limitedWebPartManager.addWebPart(oWebPart, 'Main', 1);

    $context.load(oWebPart);

    $context.executeQueryAsync(
        function () {
            span.text("ok");
            SetupLog("Observation display form set up successfully");
        },
        function (sender, args) {
            span.text("failed");
            SetupErr(sender, args);
        }
     );
}

function AddCEWPToEditDeploiementForm(span) {
    var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/deploiement/forms/editform.aspx"
    var oFile = $context.get_web().getFileByServerRelativeUrl(serverRelativeUrl);

    var limitedWebPartManager = oFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);

    var webPartXml = '<?xml version=\"1.0\" encoding=\"utf-8\"?>' +
        '<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' +
        ' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' +
        ' xmlns=\"http://schemas.microsoft.com/WebPart/v2\">' +
        '<Title>My Web Part</Title><FrameType>None</FrameType>' +
        '<Description>Use for formatted text, tables, and images.</Description>' +
        '<IsIncluded>true</IsIncluded><ZoneID></ZoneID><PartOrder>0</PartOrder>' +
        '<FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove>' +
        '<AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize>' +
        '<AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit>' +
        '<AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink />' +
        '<HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall />' +
        '<MissingAssembly>Cannot import this Web Part.</MissingAssembly>' +
        '<PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter />' +
        '<Assembly>Microsoft.SharePoint, Version=13.0.0.0, Culture=neutral, ' +
        'PublicKeyToken=94de0004b6e3fcc5</Assembly>' +
        '<TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>' +
        '<ContentLink xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor">' + _spPageContextInfo.siteServerRelativeUrl + '/RNVO_Assets/dep_IH1600.html</ContentLink>' +
        '<Content xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor" />' +
        '<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>';

    var oWebPartDefinition = limitedWebPartManager.importWebPart(webPartXml);
    this.oWebPart = oWebPartDefinition.get_webPart();

    limitedWebPartManager.addWebPart(oWebPart, 'Main', 1);

    $context.load(oWebPart);

    $context.executeQueryAsync(
       function () {
           span.text("ok");
           SetupLog("Deploiement Edit form set up successfully");
       },
        function (sender, args) {
            span.text("failed");
            SetupErr(sender, args);
        }
     );

}

function AddCEWPToDispDeploiementForm(span) {
    var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/deploiement/forms/dispform.aspx"
    var oFile = $context.get_web().getFileByServerRelativeUrl(serverRelativeUrl);

    var limitedWebPartManager = oFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);

    var webPartXml = '<?xml version=\"1.0\" encoding=\"utf-8\"?>' +
        '<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' +
        ' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' +
        ' xmlns=\"http://schemas.microsoft.com/WebPart/v2\">' +
        '<Title>My Web Part</Title><FrameType>None</FrameType>' +
        '<Description>Use for formatted text, tables, and images.</Description>' +
        '<IsIncluded>true</IsIncluded><ZoneID></ZoneID><PartOrder>0</PartOrder>' +
        '<FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove>' +
        '<AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize>' +
        '<AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit>' +
        '<AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink />' +
        '<HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall />' +
        '<MissingAssembly>Cannot import this Web Part.</MissingAssembly>' +
        '<PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter />' +
        '<Assembly>Microsoft.SharePoint, Version=13.0.0.0, Culture=neutral, ' +
        'PublicKeyToken=94de0004b6e3fcc5</Assembly>' +
        '<TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>' +
        '<ContentLink xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor">' + _spPageContextInfo.siteServerRelativeUrl + '/RNVO_Assets/DispForm.html</ContentLink>' +
        '<Content xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor" />' +
        '<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>';

    var oWebPartDefinition = limitedWebPartManager.importWebPart(webPartXml);
    this.oWebPart = oWebPartDefinition.get_webPart();

    limitedWebPartManager.addWebPart(oWebPart, 'Main', 1);

    $context.load(oWebPart);

    $context.executeQueryAsync(
       function () {
           span.text("ok");
           SetupLog("Deploiement Display form set up successfully");
       },
        function (sender, args) {
            span.text("failed");
            SetupErr(sender, args);
        }
     );

}

function SetUpList() {
    var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl;

    // create list observations
    var obsCreationInfo = new SP.ListCreationInformation();
    obsCreationInfo.set_title(IH1600.param.depObsListName);
    obsCreationInfo.set_templateType(20100);
    obsCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    obsCreationInfo.set_documentTemplateType("101");
    obsCreationInfo.set_url("Lists/obs-dep");
    obsCreationInfo.set_quickLaunchOption(SP.QuickLaunchOptions.off);
    $context.get_web().get_lists().add(obsCreationInfo);

    // create list déploiement
    var depCreationInfo = new SP.ListCreationInformation();
    depCreationInfo.set_title(IH1600.param.depLibraryName);
    depCreationInfo.set_templateType(10100);
    depCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    depCreationInfo.set_documentTemplateType("101");
    depCreationInfo.set_url("Deploiement");
    depCreationInfo.set_quickLaunchOption(SP.QuickLaunchOptions.on);
    $context.get_web().get_lists().add(depCreationInfo);



    $context.executeQueryAsync(
        function () {
            SetupLog("Lists 'Déploiement' et 'Observations' created successfully");

            var web = $context.get_web();
            var depList = web.get_lists().getByTitle(depCreationInfo.get_title());
            var obsList = web.get_lists().getByTitle(obsCreationInfo.get_title());
            var docField = obsList.get_fields().getByInternalNameOrTitle("document");
            $context.load(obsList);
            $context.load(depList);
            $context.load(docField);
            $context.load(web);
            $context.executeQueryAsync(
               function () {
                   //var deferred = $.Deferred();
                   //var docField = obsList.get_fields.getByInternalNameOrTitle("document");
                   var lookupField = $context.castTo(docField, SP.FieldLookup);
                   lookupField.set_lookupWebId(web.get_id());
                   lookupField.set_lookupList(depList.get_id().toString());
                   lookupField.set_lookupField("Title");
                   lookupField.update();
                   $context.executeQueryAsync(
                       function () {
                           SetupLog("déploiement lookup field set up successfully")
                           //deferred.resolve();
                       },
                       SetupErr
                   );
               },
               SetupErr
            );

            //return deferred.promise();
        },
        SetupErr
    );
}

function setupReferentielLibrary(span) {

    // create list observations
    var obsCreationInfo = new SP.ListCreationInformation();
    obsCreationInfo.set_title(IH1600.param.refObsListName);
    obsCreationInfo.set_templateType(20100);
    obsCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    obsCreationInfo.set_documentTemplateType("101");
    obsCreationInfo.set_url("Lists/obs-ref");
    obsCreationInfo.set_quickLaunchOption(SP.QuickLaunchOptions.off);
    $context.get_web().get_lists().add(obsCreationInfo);

    // create list référentiel
    var refCreationInfo = new SP.ListCreationInformation();
    refCreationInfo.set_title(IH1600.param.refLibraryName);
    refCreationInfo.set_templateType(10101);
    refCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    refCreationInfo.set_documentTemplateType("101");
    refCreationInfo.set_url("ref");
    refCreationInfo.set_quickLaunchOption(SP.QuickLaunchOptions.on);
    $context.get_web().get_lists().add(refCreationInfo);

    $context.executeQueryAsync(
        function () {
            span.text("ok");
            SetupLog("List 'Référentiel' created successfully");

            var web = $context.get_web();
            var refList = web.get_lists().getByTitle(refCreationInfo.get_title());
            var obsList = web.get_lists().getByTitle(obsCreationInfo.get_title());
            var docField = obsList.get_fields().getByInternalNameOrTitle("document");
            $context.load(obsList);
            $context.load(refList);
            $context.load(docField);
            $context.load(web);
            $context.executeQueryAsync(
               function () {
                   //var deferred = $.Deferred();
                   //var docField = obsList.get_fields.getByInternalNameOrTitle("document");
                   var lookupField = $context.castTo(docField, SP.FieldLookup);
                   lookupField.set_lookupWebId(web.get_id());
                   lookupField.set_lookupList(refList.get_id().toString());
                   lookupField.set_lookupField("Title");
                   lookupField.update();
                   $context.executeQueryAsync(
                       function () {
                           SetupLog("référentiel lookup field set up successfully")
                           //deferred.resolve();
                       },
                       SetupErr
                   );
               },
               SetupErr
            );
        },
        SetupErr
     );
}

function AddCEWPToEditReferentielForm(span) {
    var serverRelativeUrl = _spPageContextInfo.webServerRelativeUrl + "/ref/forms/editform.aspx"
    var oFile = $context.get_web().getFileByServerRelativeUrl(serverRelativeUrl);

    var limitedWebPartManager = oFile.getLimitedWebPartManager(SP.WebParts.PersonalizationScope.shared);

    var webPartXml = '<?xml version=\"1.0\" encoding=\"utf-8\"?>' +
        '<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' +
        ' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' +
        ' xmlns=\"http://schemas.microsoft.com/WebPart/v2\">' +
        '<Title>Referentiel Web Part</Title><FrameType>None</FrameType>' +
        '<Description>Use for formatted text, tables, and images.</Description>' +
        '<IsIncluded>true</IsIncluded><ZoneID></ZoneID><PartOrder>0</PartOrder>' +
        '<FrameState>Normal</FrameState><Height /><Width /><AllowRemove>true</AllowRemove>' +
        '<AllowZoneChange>true</AllowZoneChange><AllowMinimize>true</AllowMinimize>' +
        '<AllowConnect>true</AllowConnect><AllowEdit>true</AllowEdit>' +
        '<AllowHide>true</AllowHide><IsVisible>true</IsVisible><DetailLink /><HelpLink />' +
        '<HelpMode>Modeless</HelpMode><Dir>Default</Dir><PartImageSmall />' +
        '<MissingAssembly>Cannot import this Web Part.</MissingAssembly>' +
        '<PartImageLarge>/_layouts/images/mscontl.gif</PartImageLarge><IsIncludedFilter />' +
        '<Assembly>Microsoft.SharePoint, Version=13.0.0.0, Culture=neutral, ' +
        'PublicKeyToken=94de0004b6e3fcc5</Assembly>' +
        '<TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>' +
        '<ContentLink xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor">' + _spPageContextInfo.siteServerRelativeUrl + '/RNVO_Assets/ref_IH1600.html</ContentLink>' +
        '<Content xmlns="http://schemas.microsoft.com/WebPart/v2/ContentEditor" />' +
        '<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>';

    var oWebPartDefinition = limitedWebPartManager.importWebPart(webPartXml);
    this.oWebPart = oWebPartDefinition.get_webPart();

    limitedWebPartManager.addWebPart(oWebPart, 'Main', 1);

    $context.load(oWebPart);

    $context.executeQueryAsync(
       function () {
           span.text("ok");
           SetupLog("Referentiel form set up successfully");
       },
        function (sender, args) {
            span.text("failed");
            SetupErr(sender, args);
        }
     );
}

function CreateAlertList(span) {
    var alertesCreationInfo = new SP.ListCreationInformation();
    alertesCreationInfo.set_title(IH1600.param.alerteListName);
    alertesCreationInfo.set_templateType(10104);
    alertesCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    alertesCreationInfo.set_url("Lists/alertes");
    $context.get_web().get_lists().add(alertesCreationInfo);
    $context.executeQueryAsync(
       function () {
           span.text("ok");
           SetupLog("List 'Alertes' created successfully");
       },
       function (sender, args) {
           span.text("failed");
           SetupErr(sender, args);
       }
    );
}

function CreateAlertValidationList(span) {
    var alertesCreationInfo = new SP.ListCreationInformation();
    alertesCreationInfo.set_title(IH1600.param.alerteValidationListName);
    alertesCreationInfo.set_templateType(10105);
    alertesCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    alertesCreationInfo.set_url("Lists/alertesvalidation");
    $context.get_web().get_lists().add(alertesCreationInfo);
    $context.executeQueryAsync(
       function () {
           span.text("ok");
           SetupLog("List 'Alertes Validation' created successfully");
       },
       function (sender, args) {
           span.text("failed");
           SetupErr(sender, args);
       }
    );
}

function CreateListeType(span) {
    var alertesCreationInfo = new SP.ListCreationInformation();
    alertesCreationInfo.set_title(IH1600.param.suiviListName);
    alertesCreationInfo.set_templateType(20300);
    alertesCreationInfo.set_templateFeatureId("9429761e-3558-4735-b902-bb09181aaca4");
    alertesCreationInfo.set_url("Lists/suivi");
    alertesCreationInfo.set_quickLaunchOption(SP.QuickLaunchOptions.on);
    $context.get_web().get_lists().add(alertesCreationInfo);
    $context.executeQueryAsync(
       function () {
           span.text("ok");
           SetupLog("List 'Suivi' created successfully");
       },
       function (sender, args) {
           span.text("failed");
           SetupErr(sender, args);
       }
    );
}

function ChangeMasterPage(span) {  
    var context;
    var web;
    var site;
    var url;
    var strMasterPageUrl = '/_catalogs/masterpage/speedeau.master';
    context = new SP.ClientContext.get_current();
    web = context.get_web();
    site = context.get_site();

    context.load(site);
    context.load(web);
    context.executeQueryAsync(
        Function.createDelegate(this, function(){
            url = site.get_serverRelativeUrl();
            //Update Site Master Page
            web.set_customMasterUrl(url + strMasterPageUrl);
            //Update System Master Page
            web.set_masterUrl(url + strMasterPageUrl);
            web.update();
            context.executeQueryAsync(
                function () {
                    span.text("ok");
                    SetupLog("Master Page set up successfully");
                },
                function (sender, args) {
                    span.text("failed");
                    SetupErr(sender, args);
                });
        }),
        Function.createDelegate(this, function(sender, args) {
            span.text("failed");
            SetupErr(sender, args);
        })
    );   
}