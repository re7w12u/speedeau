<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%--<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentPicker.aspx.cs" Inherits="SPEEDEAU.Layouts.SPEEDEAU.DocumentPicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="https://code.jquery.com/jquery-2.1.3.min.js"></script>
    <script type="text/javascript"></script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <asp:Panel ID="pageStatusBar" ClientIDMode="Static" runat="server" Visible="false">
        <asp:Literal ID="MsgLiteral" runat="server" />
    </asp:Panel>

      <asp:ListView ID="ListView1" runat="server">
        <LayoutTemplate>
            <table border="0" cellpadding="1">
                <%--<tr style="background-color: #E5E5FE">
                    <th align="left">
                        <asp:LinkButton ID="lnkId" runat="server">Titre</asp:LinkButton></th>
                    <th align="left">
                        <asp:LinkButton ID="lnkName" runat="server">Statut documentaire</asp:LinkButton></th>
                    <th align="left">
                        <asp:LinkButton ID="lnkType" runat="server">Indice</asp:LinkButton></th>
                     <th align="left">
                        <asp:LinkButton ID="LinkButton1" runat="server">Revision</asp:LinkButton></th>
                     <th align="left">
                        <asp:LinkButton ID="LinkButton2" runat="server">Auteur</asp:LinkButton></th>
                     <th align="left">
                        <asp:LinkButton ID="LinkButton3" runat="server">Date de modification</asp:LinkButton></th>                    
                </tr>--%>
                <tr id="itemPlaceholder" runat="server"></tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td><%# GetFileName(Container.DataItem) %></td>
				<td><%# ((SPListItem)Container.DataItem)["Status Documentaire"] %></td>
                <td><%# ((SPListItem)Container.DataItem)["Indice"] %></td>				
                <td><%# ((SPListItem)Container.DataItem)["Révision"] %></td>
                <td><%# GetUserName(((SPListItem)Container.DataItem)[SPBuiltInFieldId.Modified_x0020_By]) %></td>
                <td><%# ((SPListItem)Container.DataItem)[SPBuiltInFieldId.Modified].ToString().Split(new char[]{' '})[0] %></td>
                <td><asp:Button ID="SelectBtn" runat="server" Text="Selectionner" OnClick="SelectBtn_Click" ToolTip=<%# Eval("ID") %> /></td>
            </tr>
        </ItemTemplate>
    </asp:ListView>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Speedeau
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Speedeau
</asp:Content>
