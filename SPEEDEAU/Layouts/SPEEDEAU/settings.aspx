<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="settings.aspx.cs" Inherits="SPEEDEAU.Layouts.Settings" DynamicMasterPageFile="~masterurl/default.master" %>
<%--add this for taxonomy fields--%>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>


<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
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

        .spdeau-form-description{
            color:#9C9999;
            font-size: smaller;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <asp:Table ID="siteParam" runat="server">
         <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Projet</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyProjet" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="false"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false" />
                <br />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>Ce paramétre sera automatiquement repris dans le champ projet des formulaires</span>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Déploiement</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyFamilleDocDep" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="false"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false" />
                <br />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>Ce paramétre sera automatiquement repris dans le champ Famille Documentaire de la liste de déploiement</span>
            </asp:TableCell>
        </asp:TableRow>


        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Référentiel</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyFamilleDocRef" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="false"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false" />
                <br />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>Ce paramétre sera automatiquement repris dans le champ Famille Documentaire de la liste de référentiel</span>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Url du site MSH</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="SiteMSHTextBox" runat="server" /><br />             
            </asp:TableCell>
        </asp:TableRow>

         <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>l'url du site qui contient la liste de suivi</span>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Email</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="EmailTextBox" runat="server" /><br />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="EmailTextBox" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                    ErrorMessage="Le format de l'adresse email est incorrect."></asp:RegularExpressionValidator>                
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>L'adresse email est utilisée en tant qu'émetteur pour les 2 types d'alertes <br />+ en tant que destinataire pour la notification à tous les membres du site.<br />
                     Si non renseigné, c'est l'adresse des paramétres smtp qui sera utilisée.</span>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Activer les alertes</span></asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="AlerteCheckBox" runat="server" /><br />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>Les alertes ne sont pas créées si la case est décochée.</span>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Validation de la codification</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="RegexCodificationTextBox" runat="server" /><br />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-form-description">
            <asp:TableCell ColumnSpan="2">
                <span>Entrez une expression régulière permettant de vérifier la codification. <br />ex : ^IH(?=.{15,}$)([-\s_\.]?)([-a-zA-Z0-9\s_\.*]{6,22})([-_\s\.*]?)([\.\d]{3,6})$ </span>
            </asp:TableCell>
        </asp:TableRow>

        <%--buttons --%>
        <asp:TableFooterRow>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-cancel" > 
                <asp:Button runat="server" ID="CancelBtn" Text="Annuler"  OnClick="CancelBtn_Click" CausesValidation="false"/></asp:TableCell>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="SaveBtn" Text="Enregistrer" OnClick="SaveBtn_Click" CausesValidation="true" /></asp:TableCell>
        </asp:TableFooterRow>

    </asp:Table>


</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
SPEEDEAU - Paramétrage du site
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
SPEEDEAU - Paramétrage du site
</asp:Content>
