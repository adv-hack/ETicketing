<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VoucherDetails.aspx.vb" Inherits="PagesLogin_Promotions_VoucherDetails" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceholderID="ContentPlaceHolder1" runat="Server">

    <asp:PlaceHolder ID="plhVoucherNotFound" runat="server">
        <div class="alert alert-box voucher-not-found">
            <asp:Literal ID="ltlVoucherNotFound" runat="server" />
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhVoucherFound" runat="server">
        <div class="panel ebiz-voucher-details">
            <div class="row">
                <div class="large-4 columns">
                    <asp:Image ID="imgVoucher" runat="server" ImageUrl='<%=ImagePath%>'  />
                </div>
                <div class="large-8 columns">
                    <h3><%= VoucherTitle%></h3>
                    <p><%= VoucherDescription%></p>
                    <h3><%= VoucherCode%></h3>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>

