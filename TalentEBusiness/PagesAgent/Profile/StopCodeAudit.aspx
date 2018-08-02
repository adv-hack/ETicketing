<%@ Page Language="VB" AutoEventWireup="false" CodeFile="StopCodeAudit.aspx.vb" Inherits="PagesLogin_Profile_OnAccount" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
  
    <div class="stopcodeaudit_ascx default-form">
        <asp:PlaceHolder ID="plhNoStopCodeAudit" runat="server">
            <div class="alert-box info">
              <asp:Literal ID="ltlNoStopCodeAudit" runat="server" />
            </div>
        </asp:PlaceHolder>
               
        <asp:Repeater ID="rptStopCodeAudit" runat="server">
            <HeaderTemplate>
              <div class="ebiz-stopcode-audit-wrap">
                <table class="ebiz-responsive-table">
                  <thead>
                    <tr>
                        <th scope="col" class="date"><%=DateHeaderText%></th>
                        <th scope="col" class="time"><%=TimeHeaderText%></th>
                        <th scope="col" class="text"><%=Usertext%></th>
                        <th scope="col" class="from"><%=FromCodeText%></th>
                        <th scope="col" class="to"><%=ToCodeText%></th>
                    </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
             <ItemTemplate>
               <tr>
                   <td class="date"><asp:Label ID="lblDate" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Date"))%>'></asp:Label></td> 
                   <td class="time"><asp:Label ID="lblTime" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Time"))%>'></asp:Label></td> 
                   <td class="text"><asp:Label ID="lblUser" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "User"))%>'></asp:Label></td> 
                   <td class="from"><asp:Label ID="Label1"  runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "FromCode"))%>'></asp:Label></td>
                   <td class="to"><asp:Label ID="Label2" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "ToCode"))%>'></asp:Label></td>
               </tr>
             </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
              </div>
            </FooterTemplate>
        </asp:Repeater>    
    </div>
    <button class="close-button" data-close aria-label="Close modal" type="button">
      <span aria-hidden="true"><i class="fa fa-times"></i></span>
    </button>
</asp:Content>