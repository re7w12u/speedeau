﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewDeploiement.aspx.cs" Inherits="SPEEDEAU.Layouts.NewDeploiement" DynamicMasterPageFile="~masterurl/default.master" EnableSessionState="True" %>

<%--add this for taxonomy fields--%>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <style type="text/css">
        .Validation-Summary {
            color: red;
        }

        .spdeau-required {
            margin-left: 5px;
            color: red;
        }

        .spdeau-form-btn {
            width: 150px;
            height: 40px;
            margin-top: 45px;
        }

        .spdeau-form-btn-cancel {
            text-align: left;
        }

        .spdeau-form-btn-ok {
            text-align: right;
        }

        #FormTable {
            border: 0px;
            border-spacing: 0px;
        }

            #FormTable tbody tr td {
                padding: 5px;
                /*padding-bottom:12px; */
            }

        .spdeau-ih1600 {
            background-color: rgba( 156,206,240,0.3 );
        }

        .spdeau-hide {
            display: none;
        }

        #ReloadFormTable {
            border: 1px solid gray;
            padding: 10px;
            margin: 10px;
            background-color: #DAFBD6;
        }
    </style>

    <script type="text/javascript" src="https://code.jquery.com/jquery-2.1.3.min.js" ></script>
    <script type="text/javascript">
        $(function () {
            setFormBehavior();
            init();
        });

        function init() {
            if ($("#StatusDoc input:checked").val() == "BPE") $("#StatusDoc input:checked").change();

            $("#GestionIH1600CheckBox").change();
        }

        function setFormBehavior() {

            $("#StatusDoc input").change(function () {
                var val = $("#StatusDoc input:checked").val();
                // add asterik for required field in BPE mode                
                if (val == "BPE") $(".spdeau-bpe").after('<span class="spdeau-required bpe-dependant">*</span>');
                else $(".bpe-dependant").remove();

                // hide revision if BPE
                if (val == "BPE") {
                    $("#RowRevision").hide();
                    $("#Revision").val("");
                }
                else $("#RowRevision").show();
            });

            $("#GestionIH1600CheckBox").change(function () {
                var clss = 'spdeau-hide';
                var isChecked = $("#GestionIH1600CheckBox").is(":checked");
                if (isChecked) {
                    // show
                    $(".spdeau-ih1600").removeClass(clss);
                }
                else {
                    // hide
                    $(".spdeau-ih1600").addClass(clss);
                    // empty input and uncheck radio btn
                    $(".spdeau-ih1600").find("input").val("").prop("checked", "");
                }
            });

        }

        function cancelDialog() {
            SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, "0");
        }

    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <asp:HiddenField ID="UpdateOnlyHiddenField" runat="server" />
    <asp:HiddenField ID="SourceHiddenField" runat="server" />
    <asp:HiddenField ID="FIDHiddenField" runat="server" />
    <asp:HiddenField ID="HasPropertyBagHiddenField" runat="server" />
    <asp:HiddenField ID="ValidationStatusHiddenField" runat="server" />
    <asp:HiddenField ID="CodififcationSystem" runat="server" />
    <asp:HiddenField ID="NewDocumentHiddenField" runat="server" />

    <%-- Validation Summary --%>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="Validation-Summary" />
    
    <%--relaod suivi data form--%>
    <asp:Table runat="server" ID="ReloadFormTable" ClientIDMode="Static" Visible="false">             
        <asp:TableRow>
            <asp:TableCell RowSpan="2">
                <img src="/_layouts/images/centraladmin_configurationwizards_48x48.png" alt="wizard" />
            </asp:TableCell>
            <asp:TableCell CssClass="" >
                <div>Vous pouvez recharger les informations de la liste de suivi en cliquant sur le bouton 'corriger' ci-dessous.</div>
            </asp:TableCell>
        </asp:TableRow>  

        <asp:TableRow>
            <asp:TableCell CssClass="spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="Button2" Text="Corriger" OnClick="ReloadSuiviData_Click" CausesValidation="false" /></asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    <%--Same file name warning --%>
    <asp:Table runat="server" ID="SameFileNameFormTable" ClientIDMode="Static" Visible="false">             
        <asp:TableRow>
            <asp:TableCell RowSpan="2">
                <img src="/_layouts/images/warning32by32.gif" alt="Warning" />
            </asp:TableCell>
            <asp:TableCell CssClass="" >
                <div>Un fichier portant le même nom existe de déjà. Voulez-vous l'écraser</div>
            </asp:TableCell>
        </asp:TableRow>  

        <asp:TableRow>
            <asp:TableCell CssClass="spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="Button1" Text="Non" OnClientClick="javascript:cancelDialog();"  CausesValidation="false" /></asp:TableCell>
            <asp:TableCell CssClass="spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="Button3" Text="Oui" OnClick="ForceUpload_Click" CausesValidation="true" /></asp:TableCell>
        </asp:TableRow>
    </asp:Table>


    <%--Main Form--%>
    <asp:Table runat="server" ID="FormTable" ClientIDMode="Static">
        <asp:TableRow ID="FileUploadRow">
            <asp:TableCell><span class="speedeau-label">Votre fichier</span><span class="spdeau-required">*</span></asp:TableCell>
            <asp:TableCell>
                <asp:FileUpload runat="server" ID="FileUpload" ClientIDMode="Predictable" /><br />
                <asp:RequiredFieldValidator ID="ValidateFileUpload" runat="server" ErrorMessage="Merci de sélectionner un fichier" ControlToValidate="FileUpload" Display="None"/>
                <asp:CustomValidator ID="ValidateFileUpload2" runat="server" ErrorMessage="Le nom du fichier doit être " ControlToValidate="FileUpload" Display="None"  OnServerValidate="ValidateFileUpload2_ServerValidate"/>
                <asp:CustomValidator ID="ValidateFileUpload3" runat="server" ControlToValidate="FileUpload" Display="None" OnServerValidate="ValidateFileUpload3_ServerValidate" />                
                <asp:CustomValidator ID="ValidateFileUpload4" runat="server" ControlToValidate="FileUpload" Display="None" OnServerValidate="ValidateFileUpload4_ServerValidate" />
            </asp:TableCell>
        </asp:TableRow>

        <%--Business rule says : value is référentiel and is not editable by user. So in the first place, I used code behind to set the value and disabled the field 
        with the attribute 'Enabled=false'. But it seems that there's a bug in IE that erase the value.
        So, I simply keep the attribute as Enabled=true, set the field in code behind and hide the field from user--%>
         <asp:TableRow CssClass="spdeau-hide">
            <asp:TableCell><span class="speedeau-label">Famille Documentaire</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyFamilleDoc" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="true"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false"
                    Enabled="true"
                     />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Titre</span><span class="spdeau-required">*</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="Titre" Width="352px" /><br />
                <asp:RequiredFieldValidator ID="ValidateTitre" runat="server" ErrorMessage="Merci de renseigner le titre du document" ControlToValidate="Titre" Display="None"/>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Nature Documentaire</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyNatureDoc" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="false"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false"/>                
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Gestion IH1600</span></asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="GestionIH1600CheckBox" Checked="true" runat="server" ClientIDMode="Static" />             
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-ih1600" ID="RowCodification" runat="server">
            <asp:TableCell><span class="speedeau-label spdeau-bpe">Codification</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="Codification" Width="352px" /><br />
                <asp:RegularExpressionValidator ID="ValidateCodification" runat="server" ErrorMessage="La codification saisie n'est pas valide." ControlToValidate="Codification"  Display="None"/>
                <asp:CustomValidator ID="ValidateCodification2" runat="server" ControlToValidate="StatusDoc"
                    OnServerValidate="ValidateCodification_ServerValidate" ErrorMessage="Merci de renseigner la codification pour un document BPE"  Display="None"/>
                <asp:CustomValidator ID="ValidateSuiviConsitency" runat="server" ControlToValidate="Codification" Display="None" OnServerValidate="ValidateSuiviConsitency_ServerValidate" />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-ih1600" ID="RowStatusDoc" runat="server">
            <asp:TableCell><span class="speedeau-label">statut Documentaire</span><span class="spdeau-required">*</span></asp:TableCell>
            <asp:TableCell>
                <asp:RadioButtonList runat="server" ID="StatusDoc" ClientIDMode="Static">
                    <asp:ListItem Text="PREL" Value="PREL" />
                    <asp:ListItem Text="BPE" Value="BPE" />
                </asp:RadioButtonList>                
                <asp:CustomValidator ID="ValidateStatusDoc" runat="server" ControlToValidate="StatusDoc" Display="None" ValidateEmptyText="true"
                    OnServerValidate="ValidateStatusDoc_ServerValidate" ErrorMessage="Les observations faites précédemment ne permettent pas de télécharger un document BPE." />
            </asp:TableCell>            
        </asp:TableRow>

        <asp:TableRow CssClass="spdeau-ih1600" ID="RowIndice" runat="server">
            <asp:TableCell><span class="speedeau-label spdeau-bpe">Indice</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="Indice" ClientIDMode="Static" Width="10px" />
                <asp:RegularExpressionValidator ID="ValidateIncide" runat="server" ControlToValidate="Indice" Display="None"
                    ValidationExpression="^[a-zA-Z]{1}$" ErrorMessage="L'indice n'est pas valide. Il se compose d'une lettre" />
                <asp:CustomValidator ID="ValidateIncideMandatory" runat="server" ControlToValidate="Indice" ValidateEmptyText="true"  Display="None"
                    OnServerValidate="ValidateIndice_ServerValidate" />
                <asp:CustomValidator ID="validateIndiceIncrement" runat="server" ControlToValidate="Indice" ValidateEmptyText="true" Display="None" 
                    OnServerValidate="ValidateIndiceIncrement_ServerValidate" ErrorMessage="La valeur de l'indice n'est pas cohérente avec la valeur précédente" />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="RowRevision" ClientIDMode="Static" CssClass="spdeau-ih1600">
            <asp:TableCell><span class="speedeau-label">Revision</span></asp:TableCell>
            <asp:TableCell>
                <asp:TextBox runat="server" ID="Revision" ClientIDMode="Static"  Width="15px"/><br />
                <asp:RegularExpressionValidator ID="ValidateRevision" runat="server" ControlToValidate="Revision" Display="None"
                    ValidationExpression="^[0-9]+$" ErrorMessage="La révision n'est pas valide. Elle se compose d'une nombre." />
                <asp:CustomValidator ID="ValidateRevision2" runat="server" ControlToValidate="Revision" ValidateEmptyText="true"
                    OnServerValidate="ValidateRevision_ServerValidate" ErrorMessage="Un document BPE n'a pas de numéro de révision" Display="None" />
                <asp:CustomValidator ID="ValidateRevisionIncrement" runat="server" ControlToValidate="Revision" ValidateEmptyText="true"
                    OnServerValidate="ValidateRevisionIncrement_ServerValidate" ErrorMessage="La valeur de la révision n'est pas cohérente avec la valeur précédente." Display="None" />
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Projet</span><span class="spdeau-required">*</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyProjet" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="false"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false"/>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Theme</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyTheme" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="true"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false" />
            </asp:TableCell>
        </asp:TableRow>


        <asp:TableRow>
            <asp:TableCell><span class="speedeau-label">Site DPIH</span></asp:TableCell>
            <asp:TableCell>
                <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomySiteDPIH" Visible="true"
                    IsDisplayPickerButton="true"
                    IsMulti="false"
                    AllowFillIn="false"
                    IsAddTerms="false"
                    IsIncludePathData="false" />
            </asp:TableCell>
        </asp:TableRow>

        <%--buttons --%>
        <asp:TableFooterRow>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-cancel" > 
                <asp:Button runat="server" ID="CancelBtn" Text="Annuler"  OnClientClick="javascript:cancelDialog();"  CausesValidation="false"/></asp:TableCell>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-ok" >
                <asp:Button runat="server" ID="SaveBtn" Text="Enregistrer" OnClick="SaveBtn_Click" CausesValidation="true" /></asp:TableCell>
        </asp:TableFooterRow>

    </asp:Table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Speedeau - Déploiement - formulaire de téléchargement
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Téléchargez un document de déploiement
</asp:Content>

