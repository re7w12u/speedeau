<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestServices.aspx.cs" Inherits="SPEEDEAU.ADMIN.Layouts.TestServices" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
        <style type="text/css">
        label {
            float: left;
            width: 120px;
            font-weight: bold;
        }

        input, textarea {
            width: 180px;
            margin-bottom: 5px;
        }

        textarea {
            width: 250px;
            height: 150px;
        }

        .boxes {
            width: 1em;
        }

        #submitbutton {
            margin-left: 120px;
            margin-top: 5px;
            width: 90px;
        }

        .clear {
            clear: left;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <SharePoint:FormDigest ID="FormDigest1" runat="server">
    </SharePoint:FormDigest>

    <div>
        <fieldset>
            <legend>Test Logging</legend>
            <asp:TextBox runat="server" ID="LoggingText" /> <asp:Button runat="server" ID="LoggingButton" Text="Log" OnClick="LoggingButton_Click" /><br />            
            <asp:Label runat="server" ID="LoggingLabel" />
        </fieldset>

        <fieldset>
            <legend>Test Web Properties</legend>
            <asp:TextBox runat="server" ID="WebPropertiesUrl" Text="http://rdits-sp13-dev/sites/rnvo/ext1" /> <br />
            <asp:TextBox runat="server" ID="WebPropertiesValue" Text="MSH" /> <br />
            <asp:Button runat="server" ID="WebProperties" Text="Get" OnClick="GetWebProperties_Click" />
            <asp:Button runat="server" ID="Button3" Text="Set" OnClick="SetWebProperties_Click" /><br />     
            <asp:Label runat="server" ID="WebPropertiesLabel" />
        </fieldset>

        <fieldset>
            <legend>Custom Logging Categories </legend>
            <asp:Button runat="server" ID="Button1" Text="Get Event Id" OnClick="GetEventID" /><asp:Label runat="server" ID="EventIdLabel" />
            <asp:Button runat="server" ID="Button7" Text="Get Logging Cat" OnClick="GetLoggingCategories" /><asp:TreeView ID="LogCategoriesList1" runat="server">
            </asp:TreeView>
            <br />
        </fieldset>

        <fieldset>
            <legend>Admin Services</legend>
            <asp:Button runat="server" ID="Button2" Text="Get Custom Services" OnClick="GetCustomServices_Click" />
                <asp:GridView ID="TypeMappingGV" runat="server">
                    <%--<Columns>
                        <asp:BoundField DataField="FromType"  HeaderText="from type"/>
                        <asp:BoundField DataField="ToType" HeaderText="to type" />
                    </Columns> --%>                   
                </asp:GridView>
            <br />
        </fieldset>

    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
