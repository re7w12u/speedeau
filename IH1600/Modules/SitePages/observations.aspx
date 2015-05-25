<%-- _lcid="1036" _version="15.0.4420" _dal="1" --%>
<%-- _LocalBinding --%>

<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" meta:webpartpageexpansion="full" meta:progid="SharePoint.WebPartPage.Document" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ListItemProperty Property="BaseName" MaxLength="40" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <meta name="GENERATOR" content="Microsoft SharePoint" />
    <meta name="ProgId" content="SharePoint.WebPartPage.Document" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="CollaborationServer" content="SharePoint Team Web Site" />
    <SharePoint:ScriptBlock runat="server">
        var navBarHelpOverrideKey = "WSSEndUser";
    </SharePoint:ScriptBlock>
    <SharePoint:StyleBlock runat="server" >
        body #s4-leftpanel {
	        display:none;
        }
        .s4-ca {
	        margin-left:0px;
        }

        <%--  hide stuff such a ribbon, title, etc. --%>
        <%--#s4-ribbonrow { display: none; }--%>
        
        <%--#Hero-WPQ2 { display: none; }--%>

        <%--.ms-webpart-chrome-title { display: none; }--%>

        <%--  override corev15.css rules --%>
        .ms-webpartPage-root { border-spacing: 5px; }

    </SharePoint:StyleBlock>
   <Sharepoint:ScriptLink runat="server" Name="SP.js" Localizable="false"  ID="s1" LoadAfterUI="true"/>
<Sharepoint:ScriptLink runat="server" Name="SP.Runtime.js" Localizable="false"  ID="s2" LoadAfterUI="true"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderSearchArea" runat="server">
    <SharePoint:DelegateControl runat="server"
        ControlId="SmallSearchInputBox" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
    <SharePoint:ProjectProperty Property="Description" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="ms-hide">
        <WebPartPages:WebPartZone runat="server" Title="loc:TitleBar" ID="TitleBar" AllowLayoutChange="false" AllowPersonalization="false" Style="display: none;"/>
    </div>
    <table class="ms-core-tableNoSpace ms-webpartPage-root" width="100%">
        <tr>
            <td id="_invisibleIfEmpty" name="_invisibleIfEmpty" valign="top" width="100%">
                <WebPartPages:WebPartZone runat="server" Title="loc:FullPage" ID="FullPage" FrameType="TitleBarOnly" />
            </td>
        </tr>
        <SharePoint:ScriptBlock runat="server">if(typeof(MSOLayout_MakeInvisibleIfEmpty) == "function") {MSOLayout_MakeInvisibleIfEmpty();}</SharePoint:ScriptBlock>
    </table>
</asp:Content>
