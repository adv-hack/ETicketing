<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DirectDebitSummary.ascx.vb" Inherits="UserControls_DirectDebitSummary" %>

<asp:PlaceHolder id="plhDirectDebitSummary" runat="server" Visible="false">
    <div class="c-dd-sum">
        <h2><asp:Literal ID="ltlTitle" runat="server" /></h2>

        <asp:BulletedList ID="ErrorList" runat="server" CssClass="error" />

        <div class="c-dd-sum__guar">
            <div class="c-dd-sum__logo">
                <asp:Image ID="imgDirectDebit" runat="server" />
            </div>
            <div class="c-dd-sum__guar-lab">
                <asp:Label ID="lblGuarantee" runat="server" />
            </div>
        </div>

        <div class="row c-dd-sum__dd-ref">
            <div class="columns c-dd-sum__lab">
                <asp:Label ID="lblDDIRef" runat="server" />
            </div>
            <div class="columns c-dd-sum__dta">
                <asp:Label ID="lblDDIRefDetail" runat="server" />
            </div>
        </div>

        <div class="row c-dd-sum__acc-n"> 
            <div class="columns c-dd-sum__lab">
                <asp:Label ID="lblAccountName" runat="server" />
            </div>
            <div class="columns c-dd-sum__dta">
                <asp:Label ID="lblAccountNameDetail" runat="server" />
            </div>
        </div>

        <div class="row c-dd-sum__s-c">
            <div class="columns c-dd-sum__lab">
                <asp:Label ID="lblSortCode" runat="server" />
            </div>
            <div class="columns c-dd-sum__dta">
                <asp:Label ID="lblSortCodeDetail" runat="server" />
            </div>
        </div>

        <div class="row c-dd-sum__acc-nbr">
            <div class="columns c-dd-sum__lab">
                <asp:Label ID="lblAccountNumber" runat="server" />
            </div>
            <div class="columns c-dd-sum__dta">
                <asp:Label ID="lblAccountNumberDetail" runat="server" />
            </div>
        </div>
        

        <div class="row c-dd-sum__det">
            <div class="columns c-dd-sum__det-d">
                <asp:Repeater ID="rptPaymentDate" runat="server">
                    <HeaderTemplate>
                        <h2>
                            <asp:label ID="lblPaymentDateHeader" runat="server" OnPreRender="GetText"></asp:label>
                        </h2>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="c-dd-sum__det-det">
                            <asp:label ID="lblPaymentDateDetail" runat="server"><%#DataBinder.Eval(Container.DataItem, "PaymentDate")%></asp:label>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="columns c-dd-sum__det-pmt">
                <asp:Repeater ID="rptPaymentAmount" runat="server">
                    <HeaderTemplate>
                        <h2>
                            <asp:label ID="lblPaymentAmountHeader" runat="server" OnPreRender="GetText"></asp:label>
                        </h2>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="c-dd-sum__det-det">
                            <asp:label ID="lblPaymentAmountDetail" runat="server"><%#DataBinder.Eval(Container.DataItem, "PaymentAmount")%></asp:label>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="columns c-dd-sum__pmt-stat">
                <asp:Repeater ID="rptPaymentStatus" runat="server">
                    <HeaderTemplate>
                        <h2>
                            <asp:label ID="lblPaymentStatustHeader" runat="server" OnPreRender="GetText"></asp:label>
                        </h2>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="c-dd-sum__det-det">
                            <asp:label ID="lblPaymentStatusDetail" runat="server"><%#DataBinder.Eval(Container.DataItem, "ScheduledPaymentStatusDescription")%></asp:label>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:PlaceHolder>

    