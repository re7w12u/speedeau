<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="check_codification.aspx.cs" Inherits="SPEEDEAU.Layouts.SPEEDEAU.check_codification" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        .Validation-Summary{
            color : red;
        }
        .spdeau-required{
            margin-left:5px;
            color:red;
        }

        .spdeau-form-btn {
            width: 150px;
            height: 40px;
            margin-top: 45px;
        } 
        .spdeau-form-btn-cancel{
            text-align:left;
        }
        .spdeau-form-btn-ok{
            text-align:right;
        }
        .spdeau-form-text{

        }

        #FormTable tbody tr td{
            padding-bottom:12px; 
        }

    </style>

     <script type="text/javascript" src="https://code.jquery.com/jquery-2.1.3.min.js" ></script>
    <script type="text/javascript">

        function SkipCodification() {
            var url = _spPageContextInfo.webServerRelativeUrl + "/" + _spPageContextInfo.layoutsUrl + "/speedeau/newdeploiement.aspx?IsDlg=1&action=new";            
            //STSNavigate(url);
            window.location = url;
        }

    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%--<asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="Validation-Summary" />--%>

    <asp:Table runat="server" ID="FormTable" ClientIDMode="Static">
         <asp:TableRow>
            <asp:TableCell><span class="speedeau-label spdeau-bpe">Codification</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="Codification" Width="352px" /><br />
                <asp:RegularExpressionValidator ID="ValidateCodification" runat="server" ErrorMessage="La codification saisie n'est pas valide." ControlToValidate="Codification" />  <br />
                <asp:CustomValidator ID="CustomValidatorCodification" runat="server" ControlToValidate="Codification" ValidateEmptyText="true" OnServerValidate="CustomValidatorCodification_ServerValidate" />              
            </asp:TableCell>
        </asp:TableRow>


         <%--buttons --%>
        <asp:TableRow>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-cancel" > 
                <asp:Button runat="server" ID="IgnoreBtn" Text="Ignorer"  OnClick="IgnoreBtn_Click"  CausesValidation="false"/></asp:TableCell>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="SaveBtn" Text="Valider" OnClick="SaveBtn_Click" CausesValidation="true" /></asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="ForceCodeRow" Visible="false">
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-cancel" ></asp:TableCell>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="ForceBtn" Text="Continuer avec ce code" OnClick="ForceCode_Click" CausesValidation="false" /></asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <div class="spdeau-form-text">En saisissant une codification, le formulaire sera pré-rempli avec les informations disponibles dans la liste de suivi.</div>
                <div class="spdeau-form-text">Si vous ne souhaitez pas entrer une codification, cliquez sur 'ignorer'</div>
            </asp:TableCell>
        </asp:TableRow>

        </asp:Table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
