<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"
    MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/jquery-1.11.1.min.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.common.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.params.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.notify.js%>'/>"></script>

    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.setup.taxonomy.js%>'/>"></script>
    <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$SPURL:~sitecollection/RNVO_Assets/js/rnvo.setup.fields.js%>'/>"></script>

    <style type="text/css">

        #rnvo-val-MenuContainer button{
            margin:10px;
        }

    </style>

</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Setup SPIDEAU Solution
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="val-loading">
        
    </div>

    <div id="rnvo-val-MenuContainer">
        <button id="setup-DepTaxonomyField" >Set Up Deploiement Taxonomy fields</button><span class="result"></span><br />
    </div>
</asp:Content>
