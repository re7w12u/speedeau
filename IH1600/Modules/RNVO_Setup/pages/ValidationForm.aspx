<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
    MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">


    <link rel="stylesheet" type="text/css" href="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/css/jquery-ui.min.css%>'/>" />
    <link rel="stylesheet" type="text/css" href="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/css/jquery-ui.structure.min.css%>'/> " />
    <link rel="stylesheet" type="text/css" href="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/css/jquery-ui.theme.min.css%>'/>" />
    <link rel="stylesheet" type="text/css" href="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/css/rnvo.val.css%>'/>" />

    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/jquery-1.11.1.min.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.common.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.ih1600.model.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.params.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.ih1600.validation.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.validation.main.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.notify.js%>'/>"></script>


<%--    <style type="text/css">
        /* hides the form while loading data*/
        #rnvo-val-MenuContainer {
            display: none;
        }

        #val-loading {
            text-align: center;
            text-align: center;
            width: 600px;
        }

        .rnvo-val-selectmenu {
            display: none;
            margin-top: 20px;
        }

        /*hide ribbon*/
        .ms-dialog #s4-ribbonrow {
            display: none;
        }
    </style>--%>

</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Valider un document
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="val-loading">
        <img src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/img/ajax-loader.gif%>'/>" alt="Loading..." /><h3>Chargement</h3>
    </div>

    <div id="rnvo-val-MenuContainer">
        <div class="rnvo-val-selectmenu rnvo-val-noobs-menu">
            <input type="radio" name="validation-value" value="VSO" />VSO (Validation sans observation)<br />
            <input type="radio" name="validation-value" value="VSO-SV" />VSO-SV (Validation sans observations, sans vérification)<br />
        </div>

        <div class="rnvo-val-selectmenu rnvo-val-obs-menu">
            <input type="radio" name="validation-value" value="VAO" />VAO (Validation avec observations)<br />
            <input type="radio" name="validation-value" value="VSO-SC" />VSO-SC (Validation sans observations, sous condition)<br />
        </div>

        <div id="rnvo-val-btns" class="rnvo-val-selectmenu">
            <input type="button" value="Annuler" id="rnvo-val-annuler" /><input type="button" value="Valider" id="rnvo-val-valider" />
        </div>

        <div id="rnvo-val-btn-close" class="rnvo-val-selectmenu">
            <input type="button" value="Fermer" id="rnvo-val-fermer" />
        </div>

       <%-- <div class="rnvo-val-selectmenu rnvo-val-obs-menu">
            <hr />
            <h3>listes des observations :</h3>
            <ul id="rnvo-val-obs-container" class="rnvo-val-selectmenu">
            </ul>
        </div>--%>
    </div>
</asp:Content>
