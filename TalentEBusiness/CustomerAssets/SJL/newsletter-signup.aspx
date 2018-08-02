<%@ Page Language="C#" AutoEventWireup="true" CodeFile="newsletter-signup.aspx.cs" Inherits="dress_explorer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">






<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Simon Jersey Corporate - Dress Explorer</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<meta name="description" content="Simon Jersey Corporate Site" />
		<link href="newsletter-signup.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">

        <div id="container">
          <div id="newsletter">
	        <asp:Panel ID="NewsletterSignupForm" runat="server" DefaultButton="Button1" >          
            <h1>Sign up to free newsletter</h1>
                <p>Please enter your details below to receive the latest news and offers via email and or text message.<br />
	        <span class="required"><span class="ast">*</span> Required fields</span></p>
                <fieldset>
                    <label for="">Name <span class="ast">*</span> </label>
                    <asp:TextBox ID="Fullname" name="Fullname" CssClass="text" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ReqVal1" ErrorMessage="Name" runat="server" ValidationGroup="NewsletterSignupForm" ControlToValidate="Fullname" Display="None"> </asp:RequiredFieldValidator>
                    <br />
                    <label for="">Email <span class="ast">*</span> </label>
                    <asp:TextBox ID="Email" name="Email" CssClass="text" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ReqVal2" ErrorMessage="Email" runat="server" ValidationGroup="NewsletterSignupForm" ControlToValidate="Email" Display="None"></asp:RequiredFieldValidator>
                    <br />
                    <label for="">Mobile</label>
                    <asp:TextBox ID="Mobile" name="Mobile" CssClass="text" runat="server"></asp:TextBox>
                </fieldset>
                <fieldset class="buttons">
                    <asp:Button ID="Button1" runat="server" Text="Sign Up" CssClass="button" OnClick="Button1_Click" ValidationGroup="NewsletterSignupForm" />
                </fieldset>
                <asp:ValidationSummary runat="server" EnableClientScript="True" ShowMessageBox="True"
        ValidationGroup="NewsletterSignupForm" DisplayMode="BulletList" HeaderText="Please enter a"
        ShowSummary="False" ID="Validationsummary1"></asp:ValidationSummary>
            </asp:Panel>
            
            <asp:Panel ID="NewsletterError" runat="server" Visible="false">
                <h1>Sign up to free newsletter</h1>
                <p>Sorry, there appears to have been an error. Please close this box and re-open, ensure you fill out all of the madatory fields.</p><br />
                <p><asp:Literal ID="ErrorMessage" runat="server" /></p>
            </asp:Panel>    
            
            <asp:Panel ID="NewsletterThankYou" runat="server" Visible="false">
                <h1>Sign up to free newsletter</h1>
                <p>Thank you, your details have been submitted. You will now receive the latest news and offers from Simon Jersey.</p><br />
            </asp:Panel>           
            
          </div>
        </div>


    </form>    
    
</body>
</html>
