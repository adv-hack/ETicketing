<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="UnreserveTickets.aspx.vb" Inherits="PagesAgent_Reservations_UnreserveTickets" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder runat="server" ID="plhErrorMessage" Visible="false">
        <div class="alert-box alert ebiz-reservation-error-message">
            <asp:BulletedList id="blErrorDetails" runat="server"></asp:BulletedList>
        </div>
    </asp:PlaceHolder>

     <asp:PlaceHolder ID="plhConfirmationMessage" runat="server" Visible="false">
        <div class="alert-box success ebiz-confirmation-details">
            <asp:Literal id="ltlConfirmationDetails" runat="server"></asp:Literal>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhUnreserveTickets" runat="server" Visible="false">
        <div>
            <div class="row">
                <div class="medium-3 columns">
                    <asp:Label ID="lblComment" AssociatedControlID="txtComment" runat="server" CssClass="middle" />
                </div>
               <div class="medium-9 columns">
                    <asp:TextBox ID="txtComment" runat="server" />
               </div>
            </div>
            <ul class="button-group">               
                <li><asp:button ID="btnUnreserve" runat="server" CssClass="button ebiz-unreserve" /></li>
            </ul>
        </div>
    </asp:PlaceHolder> 
</asp:Content>

