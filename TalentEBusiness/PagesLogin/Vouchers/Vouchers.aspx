<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Vouchers.aspx.vb" Inherits="PagesLogin_Promotions_Vouchers" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />

    <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorMessages" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="row">
        <div class="large-8 columns">
            <asp:PlaceHolder ID="plhVouchers" runat="server">
                <div class="panel ebiz-vouchers-wrap">
                    <h2><asp:Literal ID="VouchersLabel" runat="server" OnPreRender="GetText" /></h2>
                    <fieldset>
                        <legend></legend>
                        <div class="row">
                            <div class="large-3 columns">
                                <asp:Label ID="EnterVoucherCode" runat="server" OnPreRender="GetText" AssociatedControlID="txtVoucherCode" />
                            </div>
                            <div class="large-9 columns">  
                                <asp:TextBox ID="txtVoucherCode" runat="server" placeholder="Text Input" ClientIDMode="Static"/>
                            </div>
                        </div>
                        <div class="ebiz-add-voucher-wrap">
                            <asp:Button ID="btnAddVoucher" runat="server" class="button ebiz-add" OnPreRender="GetText" />
                        </div>
                    </fieldset>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhExternalVouchers" runat="server">
                <div class="panel ebiz-external-vouchers-wrap">
                    <h2><asp:Literal ID="ExternalVouchersLabel" runat="server" OnPreRender="GetText" /></h2>
                    <fieldset>
                        <legend></legend>
                        <div class="row">
                            <div class="large-3 columns">
                                <asp:Label ID="Company" runat="server" OnPreRender="GetText" AssociatedControlID="ddlcompany" />
                            </div>
                            <div class="large-9 columns">
                                <asp:DropDownList ID="ddlcompany" runat="server" AutoPostBack="true" ViewStateMode="Enabled" OnSelectedIndexChanged="ddlcompany_SelectedIndexChanged" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-3 columns">
                                <asp:Label ID="VoucherScheme" runat="server" OnPreRender="GetText" AssociatedControlID="ddlVoucherScheme" />
                            </div>
                            <div class="large-9 columns">
                                <asp:DropDownList ID="ddlVoucherScheme" runat="server" AutoPostBack="true" ViewStateMode="Enabled" OnSelectedIndexChanged="ddlVoucherScheme_SelectedIndexChanged" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="large-3 columns">
                                <asp:Label ID="Price" runat="server" OnPreRender="GetText" AssociatedControlID="ddlPrice" />
                            </div>
                            <div class="large-9 columns">
                                <asp:DropDownList ID="ddlPrice" runat="server" ViewStateMode="Enabled" />
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plhExternalVoucherCode" runat="server" Visible="false">
                            <div class="row">
                                <div class="large-3 columns ">
                                    <asp:Label ID="lblExternalVoucherCode" runat="server" OnPreRender="GetText" AssociatedControlID="txtExternalVoucherCode" />
                                </div>
                                <div  class="large-9 columns">
                                    <asp:TextBox ID="txtExternalVoucherCode" runat="server" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <div class="ebiz-add-external-voucher-wrap">
                            <asp:Button ID="btnAddExternalvouchers" runat="server" class="button ebiz-add" OnPreRender="GetText" />
                        </div>
                    </fieldset>
                </div>
            </asp:PlaceHolder>
        </div>

        <asp:PlaceHolder ID="plhAccountBalance" runat="server">
            <div class="large-4 columns">
                <div class="panel ebiz-your-account-balance-wrap">
                    <h2><asp:Literal ID="YourAccountBalance" runat="server" OnPreRender="GetText" /></h2>
                    <div class="ebiz-balance-amount"><asp:Literal ID="BalanceAmount" runat="server" /></div>
                    <asp:PlaceHolder ID="plhRecentlyAdded" runat="server">
                        <h2><asp:Literal ID="RecentlyAdded" runat="server" OnPreRender="GetText" /></h2>
                        <div class="row">
                            <div class="large-8 columns">
                                <asp:Literal ID="VoucherId" runat="server" />
                            </div>
                            <div class="large-4 columns">
                                <asp:Literal ID="VoucherAmount" runat="server" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <asp:PlaceHolder ID="plhRepeater" runat="server">
        <div class="row">
            <div class="large-12 columns">
                <h2><asp:Literal ID="YourExperience" runat="server" OnPreRender="GetText" /></h2>
            </div>
        </div>
        <asp:Repeater ID="rptExperiences" runat="server" EnableViewState="false">
            <ItemTemplate>
                <div class="row ebiz-your-experience">
                    <div class="large-12 columns">
                        <div class="panel">
                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:Image ID="imgVoucher" runat="server" ImageUrl='<%# DataBinder.Eval(Container.DataItem, "Image")%>' Visible='<%# IsVisible(DataBinder.Eval(Container.DataItem, "Image")) %>' />
                                    <asp:HiddenField ID="hdnVoucherID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "VoucherId")%>' />
                                </div>
                                <div class="large-9 columns">
                                    <h3><%# DataBinder.Eval(Container.DataItem, "Description").ToString()%> <%# GetFormattedDate(DataBinder.Eval(Container.DataItem, "ExpiryDate"))%></h3>
                                    <%# DataBinder.Eval(Container.DataItem, "DescriptionText").ToString()%>
                                    <h4><%# DataBinder.Eval(Container.DataItem, "VoucherCode").ToString()%></h4>
                                    <h4><%# DataBinder.Eval(Container.DataItem, "ExternalCompanyName").ToString()%></h4>
                                    
                                    <asp:Repeater ID="rptLinks" runat="server" OnItemCommand="rptLinks_ItemCommand" EnableViewState="false">
                                        <ItemTemplate>
                                            <div class="row ebiz-book-links">
                                                <div class="medium-8 columns">
                                                    <strong><%# DataBinder.Eval(Container.DataItem, "LinkText").ToString()%></strong>
                                                </div>
                                                <div class="medium-4 columns">
                                                    <asp:Button ID="btnBook" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LinkButtonText").ToString()%>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Link").ToString().Trim() %>' class="button ebiz-book" CommandName="Book"  />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <asp:PlaceHolder ID="plhExchange" runat="server" Visible='<%# IsExchangeContentVisible(Eval("VoucherCode").ToString(), Eval("IsExchangeToOnAccountActive"), Eval("VoucherSource"), Eval("SalePrice").ToString()) %>'>
                                    <div class="row ebiz-exchange">
                                        <div class="medium-8 columns">
                                            <p><strong><asp:Literal ID="lblExchange" runat="server" Text='<%# GetExchangeText(Eval("SalePrice").ToString(),Eval("ExchangeText").ToString())%>' /></strong></p>
                                        </div>
                                        <div class="medium-4 columns">
                                            <asp:Button ID="btnExchange" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "VoucherCode").ToString().Trim() %>' class="button ebiz-exchange" CommandName="Exchange" Text='<%# DataBinder.Eval(Container.DataItem, "ExchangeButtonText").ToString()%>' />
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <div class="row ebiz-delete">
                                        <div class="large-12 columns">
                                            <asp:Button ID="btnDelete" OnPreRender="GetText" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UniqueVoucherId").ToString().Trim() %>' class="button ebiz-delete" CommandName="Delete" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                     </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>

</asp:Content>