<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuiviEdit.aspx.cs" Inherits="SPEEDEAU.Layouts.SPEEDEAU.SuiviEdit" DynamicMasterPageFile="~masterurl/default.master" %>

<%--add this for taxonomy fields--%>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%--add this for taxonomy fields--%>
<%@ Register TagPrefix="Taxonomy" Namespace="Microsoft.SharePoint.Taxonomy" Assembly="Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">


    <link rel="Stylesheet" type="text/css" href="/_layouts/15/speedeau/js/jquery/jquery-ui.min.css" />
    <link rel="Stylesheet" type="text/css" href="/_layouts/15/speedeau/js/jquery/jquery-ui.structure.min.css" />
    <link rel="Stylesheet" type="text/css" href="/_layouts/15/speedeau/js/jquery/jquery-ui.theme.min.css" />

    <script type="text/javascript" src="/_layouts/15/speedeau/js/jquery/external/jquery/jquery.js"></script>
    <script type="text/javascript" src="/_layouts/15/speedeau/js/jquery/jquery-ui.js"></script>
    <script type="text/javascript">

        $(function () {

            // set date picker
            $(function () {
                $("#DateCibleTextBox").datepicker({dateFormat:"dd/mm/yy"});
            });

            // set accordion
            $("#accordion").accordion({
                heightStyle: "content",
                collapsible: true
            });

            // check checkbox status and set matching panel accordingly
            $(".spdeau-checkbox-diffusion").click(function () {
                var id = $(this).attr("title");
                var checked = $(this).children(":first").is(":checked");
                if (checked) $("." + id).show({ duration: 'slow', easing: "easeOutExpo" });
                else $("." + id).hide();
            });

            $(".spdeau-checkbox-diffusion").click();
        });


        function cancelDialog() {
            SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, "0");
        }

    </script>

    <style type="text/css">
        .Validation-Summary {
            color: red;
        }


        /** diffusion du documents */
        #Table5 {
            width: 100%;
        }

            #Table5 > table {
                margin-bottom: 10px;
            }

        .speedeau-row {
            width: 170px;
        }

        /* buttons */

        #FormTable {
            width: 100%;
            margin-top: 20px;
        }

        .spdeau-form-btn-cancel {
            text-align: left;
        }

        .spdeau-form-btn-ok {
            text-align: right;
        }
    </style>


</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">


    <asp:HiddenField ID="CurrentModeHiddenField" runat="server" />
    <asp:HiddenField ID="IDHiddenField" runat="server" />
    <asp:HiddenField ID="SiteHiddenField" runat="server" />
    <asp:HiddenField ID="ProjectHiddenField" runat="server" />
    <%--<Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyProjet" Visible="false"       
                            IsDisplayPickerButton="true"                            
                            AllowFillIn="false"
                            IsAddTerms="false"
                            IsIncludePathData="false" />

    <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomySite" Visible="false"       
                            IsDisplayPickerButton="true"                            
                            AllowFillIn="false"
                            IsAddTerms="false"
                            IsIncludePathData="false" />--%>


    <!-- Validation Summary -->
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="Validation-Summary" />


    <!--Main Form -->

    <div id="accordion">
        <!-- SECTION 1 -->
        <h3>Général</h3>
        <div>
            <asp:Table runat="server" ID="Table1" ClientIDMode="Static">

                <asp:TableRow CssClass="spdeau-hide">
                    <asp:TableCell runat="server" ID="Cell1"><span class="speedeau-label">Opération</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell2">
                        <asp:TextBox runat="server" ID="OperationTextBox" Width="352px" ClientIDMode="Static" ReadOnly="true" Disabled="Disabled" /><br />
                        <asp:RequiredFieldValidator ID="OperationTextBoxValidator" runat="server" ErrorMessage="Merci de renseigner le champ 'Opération'" ControlToValidate="OperationTextBox" Display="None" />
                    </asp:TableCell>
                </asp:TableRow>

                <%--     Business rule says : value is référentiel and is not editable by user. So in the first place, I used code behind to set the value and disabled the field 
                with the attribute 'Enabled=false'. But it seems that there's a bug in IE that erase the value.
                So, I simply keep the attribute as Enabled=true, set the field in code behind and hide the field from user--%>
                <asp:TableRow CssClass="spdeau-hide">
                    <asp:TableCell runat="server" ID="Cell3"><span class="speedeau-label">Famille Documentaire</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell4">
                        <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyFamilleDoc" Visible="true"
                            IsDisplayPickerButton="true"
                            IsMulti="true"
                            AllowFillIn="false"
                            IsAddTerms="false"
                            IsIncludePathData="false"
                            Disabled="false"
                            Enabled="true" />
                        <asp:CustomValidator ID="TaxonomyFamilleDocValidator" runat="server" ErrorMessage="Merci de renseigner le champ 'Famille Documentaire'" OnServerValidate="TaxonomyFamilleDocValidator_ServerValidate" Display="None" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>

        <!-- SECTION IDENTIFICATION DU DOCUMENT -->
        <h3>Identification du document</h3>
        <div>
            <asp:Table runat="server" ID="Table2" ClientIDMode="Static">
                <asp:TableRow runat="server" ID="Row1">
                    <asp:TableCell runat="server" ID="Cell5"><span class="speedeau-label">Titre</span><span class="spdeau-required">*</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell6">
                        <asp:TextBox runat="server" ID="Titre" Width="352px" ClientIDMode="Static" /><br />
                        <asp:RequiredFieldValidator ID="ValidateTitre" runat="server" ErrorMessage="Merci de renseigner le titre du document" ControlToValidate="Titre" Display="None" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server" ID="Row2">
                    <asp:TableCell runat="server" ID="Cell7"><span class="speedeau-label">Nature Documentaire</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell8">
                        <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyNatureDoc" Visible="true"
                            IsDisplayPickerButton="true"
                            IsMulti="false"
                            AllowFillIn="false"
                            IsAddTerms="false"
                            IsIncludePathData="false" />
                        <asp:CustomValidator ID="TaxonomyNatureDocValidator" runat="server" ErrorMessage="Merci de renseigner le champ 'Nature Documentaire'" OnServerValidate="TaxonomyNatureDocValidator_ServerValidate" Display="None" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server" ID="Row3">
                    <asp:TableCell runat="server" ID="Cell9"><span class="speedeau-label">Requis</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell10">
                        <asp:CheckBox ID="RequisCheckBox" runat="server" Checked="true" ClientIDMode="Static" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowCodification" runat="server">
                    <asp:TableCell runat="server" ID="Cell11"><span class="speedeau-label ">Codification</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell12">
                        <asp:TextBox runat="server" ID="Codification" Width="352px" /><br />
                        <asp:RegularExpressionValidator ID="ValidateCodification" runat="server" ErrorMessage="La codification saisie n'est pas valide." ControlToValidate="Codification"  Display="None"/>
                        <asp:RequiredFieldValidator ID="ValidateCodification2" runat="server" ErrorMessage="Merci de renseigner le champ 'Codification'" ControlToValidate="Codification" Display="None" />
                        <asp:CustomValidator ID="ValidateionCodification3" runat="server" ErrorMessage="Cette codification existe déjà." ControlToValidate="Codification" Display="None" OnServerValidate="ValidateionCodification3_ServerValidate" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowIndice" runat="server">
                    <asp:TableCell runat="server" ID="Cell13"><span class="speedeau-label ">Indice</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell14">
                        <asp:TextBox runat="server" ID="Indice" ClientIDMode="Static" Width="10px" />
                        <asp:RegularExpressionValidator ID="ValidateIncide" runat="server" ControlToValidate="Indice" Display="None" ValidationExpression="^[a-zA-Z]{1}$" ErrorMessage="L'indice n'est pas valide. Il se compose d'une lettre" />
                        <asp:CustomValidator ID="ValidateIncide2" runat="server" ControlToValidate="Indice" ValidateEmptyText="true" OnServerValidate="ValidateIncide2_ServerValidate" Display="None" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowObservations" runat="server">
                    <asp:TableCell runat="server" ID="Cell15"><span class="speedeau-label ">Observations RNVO</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell16">
                        <asp:TextBox runat="server" ID="ObservationsTextBox" ClientIDMode="Static" TextMode="MultiLine" Rows="3" Columns="42" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>

        <!-- SECTION CHARGES ET DELAI -->
        <h3>Charges et délai</h3>
        <div>
            <asp:Table runat="server" ID="Table3" ClientIDMode="Static">

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowFourniture" runat="server">
                    <asp:TableCell runat="server" ID="Cell17"><span class="speedeau-label ">Fourniture</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell18">
                        <asp:TextBox runat="server" ID="FournitureTextBox" ClientIDMode="Static" MaxLength="10" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowDateCible" runat="server">
                    <asp:TableCell runat="server" ID="Cell86"><span class="speedeau-label ">Date Cible</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell85">
                        <asp:TextBox runat="server" ID="DateCibleTextBox" ClientIDMode="Static" />
                        <asp:CustomValidator ID="DateCibleValidator" runat="server" ControlToValidate="DateCibleTextBox" OnServerValidate="DateCibleValidator_ServerValidate" ErrorMessage="la date n'est pas valide" />
                    </asp:TableCell>                    
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowFormat" runat="server">
                    <asp:TableCell runat="server" ID="Cell19"><span class="speedeau-label ">Format demandé</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell20">
                        <asp:TextBox runat="server" ID="FormatTextBox" ClientIDMode="Static" MaxLength="20" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowTemps" runat="server">
                    <asp:TableCell runat="server" ID="Cell21"><span class="speedeau-label ">Temps Estimé (h)</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell22">
                        <asp:TextBox runat="server" ID="TempsEstimeTextBox" ClientIDMode="Static" TextMode="Number" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow CssClass="spdeau-ih1600" ID="RowResteAFaire" runat="server">
                    <asp:TableCell runat="server" ID="Cell23"><span class="speedeau-label ">Reste à faire (h)</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell24">
                        <asp:TextBox runat="server" ID="ResteAFaireTextBox" ClientIDMode="Static" TextMode="Number" />
                    </asp:TableCell>
                </asp:TableRow>

            </asp:Table>
        </div>

        <!-- SECTION INTERVENANTS -->
        <h3>Intervenants</h3>
        <div>
            <asp:Table runat="server" ID="Table4" ClientIDMode="Static">

                <asp:TableRow runat="server" ID="Row4">
                    <asp:TableCell runat="server" ID="Cell25"><span class="speedeau-label">Rédaction</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell26">
                        <Taxonomy:TaxonomyWebTaggingControl runat="server" ID="TaxonomyRedaction" Visible="true"
                            IsDisplayPickerButton="true"
                            IsMulti="true"
                            AllowFillIn="false"
                            IsAddTerms="false"
                            IsIncludePathData="false" />
                        <asp:CustomValidator ID="TaxonomyRedactionValidator" runat="server" ErrorMessage="Merci de renseigner le champ 'Rédaction'" OnServerValidate="TaxonomyRedactionValidator_ServerValidate"  Display="None" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server" ID="Row5">
                    <asp:TableCell runat="server" ID="Cell27"><span class="speedeau-label">Vérificateur</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell28">
                        <SharePoint:ClientPeoplePicker ID="VerificateurPicker" UseLocalSuggestionCache="true" PrincipalAccountType="User" runat="server" Rows="1" AllowMultipleEntities="false" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server" ID="Row6">
                    <asp:TableCell runat="server" ID="Cell29"><span class="speedeau-label">Approbateur</span></asp:TableCell>
                    <asp:TableCell runat="server" ID="Cell30">
                        <SharePoint:ClientPeoplePicker ID="ApprobateurPicker" UseLocalSuggestionCache="true" PrincipalAccountType="User" runat="server" Rows="1" AllowMultipleEntities="false" />
                    </asp:TableCell>
                </asp:TableRow>

            </asp:Table>
        </div>

        <!-- SECTION DIFFUSION DU DOCUMENT -->
        <h3>Diffusion du document</h3>
        <div>
            <asp:Table runat="server" ID="Table5" ClientIDMode="Static">
                
                <%--Intégrateur--%>
                <asp:TableRow runat="server" ID="Row9">
                    <asp:TableCell runat="server" ID="Cell49">
                        <asp:Table runat="server">
                            <asp:TableRow runat="server" ID="Cell88">
                                <asp:TableCell runat="server" ID="Cell50" CssClass="speedeau-row"><span class="speedeau-label">Diffusion Intégrateur</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell51">
                                    <asp:CheckBox ID="IntegrateurCheckBox" runat="server" ClientIDMode="Static" CssClass="spdeau-checkbox-diffusion" ToolTip="spdeau-Integrateur" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Integrateur" runat="server">
                                <asp:TableCell runat="server" ID="Cell52" CssClass="speedeau-row"><span class="speedeau-label ">Intégrateur Date</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell53">
                                    <asp:TextBox runat="server" ID="IntegrateurDateTextBox" ClientIDMode="Static" TextMode="SingleLine" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Integrateur" runat="server">
                                <asp:TableCell runat="server" ID="Cell54" CssClass="speedeau-row"><span class="speedeau-label ">Intégrateur Format</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell55">
                                    <asp:TextBox runat="server" ID="IntegrateurFormatTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Integrateur" runat="server">
                                <asp:TableCell runat="server" ID="Cell56" CssClass="speedeau-row"><span class="speedeau-label ">Intégrateur Stockage</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell57">
                                    <asp:TextBox runat="server" ID="IntegrateurStockageTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2"><hr /></asp:TableCell>
                </asp:TableRow>
                
                <%--Tableautier--%>
                <asp:TableRow runat="server" ID="Row11">
                    <asp:TableCell runat="server" ID="Cell66">
                        <asp:Table runat="server">
                            <asp:TableRow runat="server" ID="Cell90">
                                <asp:TableCell runat="server" ID="Cell67" CssClass="speedeau-row"><span class="speedeau-label">Diffusion Tableautier</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell68">
                                    <asp:CheckBox ID="TableautierCheckBox" runat="server" ClientIDMode="Static" CssClass="spdeau-checkbox-diffusion" ToolTip="spdeau-Tableautier" />
                                </asp:TableCell>
                            </asp:TableRow>


                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Tableautier" runat="server">
                                <asp:TableCell runat="server" ID="Cell69" CssClass="speedeau-row"><span class="speedeau-label ">Tableautier Date</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell83">
                                    <asp:TextBox runat="server" ID="TableautierDateTextBox" ClientIDMode="Static" TextMode="SingleLine" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Tableautier" runat="server">
                                <asp:TableCell runat="server" ID="Cell70" CssClass="speedeau-row"><span class="speedeau-label ">Tableautier Format</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell80">
                                    <asp:TextBox runat="server" ID="TableautierFormatTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Tableautier" runat="server">
                                <asp:TableCell runat="server" ID="Cell81" CssClass="speedeau-row"><span class="speedeau-label ">Tableautier Stockage</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell82">
                                    <asp:TextBox runat="server" ID="TableautierStockageTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2"><hr /></asp:TableCell>
                </asp:TableRow>

                <%--DTG--%>
                <asp:TableRow runat="server" ID="Row7">
                    <asp:TableCell runat="server" ID="Cell31">
                        <asp:Table runat="server">
                            <asp:TableRow runat="server" ID="Row12">
                                <asp:TableCell runat="server" ID="Cell32" CssClass="speedeau-row"><span class="speedeau-label">Diffusion DTG</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell33">
                                    <asp:CheckBox ID="DTGCheckBox" runat="server" ClientIDMode="Static" CssClass="spdeau-checkbox-diffusion" ToolTip="spdeau-dtg" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-dtg" runat="server">
                                <asp:TableCell runat="server" ID="Cell34" CssClass="speedeau-row"><span class="speedeau-label ">DTG Date</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell35">
                                    <asp:TextBox runat="server" ID="DTGDateTextBox" ClientIDMode="Static" TextMode="SingleLine" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-dtg" runat="server">
                                <asp:TableCell runat="server" ID="Cell36" CssClass="speedeau-row"><span class="speedeau-label ">DTG Format</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell37">
                                    <asp:TextBox runat="server" ID="DTGFormatTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-dtg" runat="server">
                                <asp:TableCell runat="server" ID="Cell38" CssClass="speedeau-row"><span class="speedeau-label ">DTG Stockage</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell39">
                                    <asp:TextBox runat="server" ID="DTGStcokageTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2"><hr /></asp:TableCell>
                </asp:TableRow>

                <%--Exploitant--%>
                <asp:TableRow runat="server" ID="Row8">
                    <asp:TableCell runat="server" ID="Cell40">
                        <asp:Table runat="server">
                            <asp:TableRow runat="server" ID="Row13">
                                <asp:TableCell runat="server" ID="Cell41" CssClass="speedeau-row"><span class="speedeau-label">Diffusion Exploitant</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell42">
                                    <asp:CheckBox ID="ExploitantCheckBox" runat="server" ClientIDMode="Static" CssClass="spdeau-checkbox-diffusion" ToolTip="spdeau-Exploitant" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Exploitant" runat="server">
                                <asp:TableCell runat="server" ID="Cell43" CssClass="speedeau-row"><span class="speedeau-label ">Exploitant Date</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell44">
                                    <asp:TextBox runat="server" ID="ExploitantDateTextBox" ClientIDMode="Static" TextMode="SingleLine" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Exploitant" runat="server">
                                <asp:TableCell runat="server" ID="Cell45" CssClass="speedeau-row"><span class="speedeau-label ">Exploitant Format</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell46">
                                    <asp:TextBox runat="server" ID="ExploitantFormatTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-Exploitant" runat="server">
                                <asp:TableCell runat="server" ID="Cell47" CssClass="speedeau-row"><span class="speedeau-label ">Exploitant Stockage</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell48">
                                    <asp:TextBox runat="server" ID="ExploitantStockageTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2"><hr /></asp:TableCell>
                </asp:TableRow>

                <%--MCO--%>
                <asp:TableRow runat="server" ID="Row10">
                    <asp:TableCell runat="server" ID="Cell84">
                        <asp:Table runat="server">
                            <asp:TableRow runat="server" ID="Cell89">
                                <asp:TableCell runat="server" ID="Cell58" CssClass="speedeau-row"><span class="speedeau-label">Diffusion MCO</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell59">
                                    <asp:CheckBox ID="MCOCheckBox" runat="server" ClientIDMode="Static" CssClass="spdeau-checkbox-diffusion" ToolTip="spdeau-MCO" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-MCO" runat="server">
                                <asp:TableCell runat="server" ID="Cell60" CssClass="speedeau-row"><span class="speedeau-label ">MCO Date</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell61">
                                    <asp:TextBox runat="server" ID="MCODateTextBox" ClientIDMode="Static" TextMode="SingleLine" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-MCO" runat="server">
                                <asp:TableCell runat="server" ID="Cell62" CssClass="speedeau-row"><span class="speedeau-label ">MCO Format</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell63">
                                    <asp:TextBox runat="server" ID="MCOFormatTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow CssClass="spdeau-ih1600 spdeau-MCO" runat="server">
                                <asp:TableCell runat="server" ID="Cell64" CssClass="speedeau-row"><span class="speedeau-label ">MCO Stockage</span></asp:TableCell>
                                <asp:TableCell runat="server" ID="Cell65">
                                    <asp:TextBox runat="server" ID="MCOStockageTextBox" ClientIDMode="Static" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>

            </asp:Table>
        </div>


    </div>

    <asp:Table runat="server" ID="FormTable" ClientIDMode="Static">

        <%--buttons --%>
        <asp:TableFooterRow>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-cancel">
                <asp:Button runat="server" ID="CancelBtn" Text="Annuler" OnClientClick="javascript:cancelDialog();" CausesValidation="false" />
            </asp:TableCell>
            <asp:TableCell CssClass="spdeau-form-btn spdeau-form-btn-ok">
                <asp:Button runat="server" ID="SaveBtn" Text="Enregistrer" OnClick="SaveBtn_Click" CausesValidation="true" />
            </asp:TableCell>
        </asp:TableFooterRow>

    </asp:Table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Speedeau - Liste de suivi - formulaire d'ajout et de modification
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Speedeau - Liste de suivi - formulaire d'ajout et de modification
</asp:Content>
