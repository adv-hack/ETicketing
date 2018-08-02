<%@ Page Language="C#" AutoEventWireup="true" CodeFile="product-configurator.aspx.cs" Inherits="productConfigurator" Debug="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Simon Jersey Corporate - Product Configurator</title>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<meta name="description" content="Simon Jersey Corporate Site" />
	<link href="product-configurator.css" rel="stylesheet" type="text/css" />
  <!--[if IE 7]>
  	<link rel="stylesheet" href="ie7.css" type="text/css" media="all" />
	<![endif]-->
  <script type="text/javascript" src="/Assets/SJL/HTML/JavaScript/jquery-1.3.2.min.js"></script>
  <script type="text/javascript" src="/Assets/SJL/HTML/JavaScript/swfobject21.js"></script>
  <script type="text/javascript">
		$(document).ready(function() {
			$("span.tooltip").hover( 
  			function () { 
    			$(this).find("span.tipText").show(); 
  			},  
  			function () { 
    			$(this).find("span.tipText").hide(); 
  			} 
			);
		});
	</script>
</head>

<body>

<form id="form1" runat="server">

<div id="container">
	<div id="header">
  	<img src="/Assets/SJL/HTML/Images/ccfc-logo.jpg" alt="Simon Jersey - Corporate clothing that people love to wear" style="border-width:0px;" />
  </div>
	
  

  <asp:Panel ID="configEntry" runat="server" DefaultButton="btnSubmit" > 
  <h1>Product configurator</h1>
  <div id="config" class="content">
    <asp:Literal ID="litError" runat="server" />
    
    <p>Please work your way through the form entering details as applicable. Click <span class="btnTextConfirm">Submit product</span> to check the details you have entered. You can load and edit an existing product by clicking <span class="btnTextConfirm">Load & edit product</span>. If you wish to clear the form and start again click <span class="btnTextEdit">Clear product</span>.<br />
      Additional information has been given to help you with the process, hover your mouse over <img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /> for help.</p>

    <div class="configSection prodCode">
    	
      <label for="txtProductCode">Product code <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please enter the Product Code of the product.</span></span></label>
      <asp:TextBox ID="txtProductCode" CssClass="txtBox" MaxLength="10" Runat="Server" />
      <asp:RequiredFieldValidator ID="valProductCode" ErrorMessage="Please enter a product code" runat="server" CssClass="validator" ValidationGroup="configEntry" ControlToValidate="txtProductCode"></asp:RequiredFieldValidator>
   		<asp:LinkButton ID="btnDoLoad" OnClick="btnLoad_Click" CssClass="link-button" Text="Load & edit product" runat="server" />    
    </div>

    <div class="configSection desc">
    	
      <label for="txtDescription">Description <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please enter the description of the product.</span></span></label>
      <asp:TextBox ID="txtDescription" CssClass="txtBox" TextMode="MultiLine" Runat="Server" />
      <asp:RequiredFieldValidator ID="valDescription" ErrorMessage="Please enter a description" runat="server" CssClass="validator" ValidationGroup="configEntry" ControlToValidate="txtDescription"></asp:RequiredFieldValidator>
    </div>
		
    <div class="clr">&nbsp;</div>
    
    <div class="configSection roundels">
    	
      <label for="lbxRoundels">Roundels <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please select which roundels will be displayed along with the product description. To select more than one, hold down CTRL when you click.</span></span></label>
      <asp:ListBox id="lbxRoundels" SelectionMode="Multiple" Rows="8" runat="server">
        <asp:ListItem Value="100-percent-cotton.gif">100% Cotton</asp:ListItem>
        <asp:ListItem Value="100-percent-wool.gif">100% Wool</asp:ListItem>
        <asp:ListItem Value="30_Wash.gif">30&deg; Wash</asp:ListItem>
        <asp:ListItem Value="70_wash.gif">Suitable For Industrial Laundering At 70&deg;</asp:ListItem>
        <asp:ListItem Value="90_Wash.gif">Suitable For Industrial Laundering At 90&deg;</asp:ListItem>
        <asp:ListItem Value="available-2-lengths.gif">Available In 2 Lengths</asp:ListItem>
        <asp:ListItem Value="available-3-lengths.gif">Available In 3 Lengths</asp:ListItem>
        <asp:ListItem Value="commercial-laundering.gif">Suitable For Commercial Laundering</asp:ListItem>
        <asp:ListItem Value="easy-care.gif">Easy Care</asp:ListItem>
        <asp:ListItem Value="extra-sizes-added.gif">Extra Sizes Added</asp:ListItem>
        <asp:ListItem Value="minimum-iron.gif">Minimum Iron</asp:ListItem>
        <asp:ListItem Value="new-colour.gif">New Colour</asp:ListItem>
        <asp:ListItem Value="new-lower-price.gif">New Lower Price</asp:ListItem>
        <asp:ListItem Value="new.gif">New</asp:ListItem>
        <asp:ListItem Value="personalise-your-wardrobe.gif">Personlise Your Wardrobe</asp:ListItem>
        <asp:ListItem Value="petite-size.gif">Petite Size</asp:ListItem>
        <asp:ListItem Value="plus.gif">Plus</asp:ListItem>
        <asp:ListItem Value="stretch-fabric.gif">2 Way Stretch Fabric</asp:ListItem>
        <asp:ListItem Value="teflon.gif">Teflon</asp:ListItem>
        <asp:ListItem Value="trousers-hemmed.gif">Trousers Hemmed To Order</asp:ListItem>
      </asp:ListBox>
    </div>

    <div class="configSection addImages">
      <p>Additional images <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please choose which additional images should be displayed along with the product by clicking the boxes.</span></span></p>
      <asp:CheckBox ID="chkImage1" runat="server" />
      <label for="chkImage1">Image 1</label>
      <asp:CheckBox ID="chkImage2" runat="server" />
      <label for="chkImage2">Image 2</label>
      <asp:CheckBox ID="chkImage3" runat="server" />
      <label for="chkImage3">Image 3</label>
      <asp:CheckBox ID="chkImage4" runat="server" />
      <label for="chkImage4">Image 4</label>
      <asp:CheckBox ID="chkImage5" runat="server" />
      <label for="chkImage5">Image 5</label>
    </div>
    
    <div class="configSection colours">
      <label for="lbxColours">Colours <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please select which colour options are available for the product. To select more than one, hold down CTRL when you click.</span></span></label>
      <asp:ListBox id="lbxColours" SelectionMode="Multiple" Rows="8" runat="server">
      	<asp:ListItem Value="Beige">Beige</asp:ListItem>
      	<asp:ListItem Value="Black">Black</asp:ListItem>
        <asp:ListItem Value="BrightBlue">Bright Blue</asp:ListItem>
        <asp:ListItem Value="Brown">Brown</asp:ListItem>
        <asp:ListItem Value="Coral">Coral</asp:ListItem>
        <asp:ListItem Value="DarkGreen">Dark Green</asp:ListItem>
        <asp:ListItem Value="DarkPink">Dark Pink</asp:ListItem>
        <asp:ListItem Value="DarkPurple">Dark Purple</asp:ListItem>
        <asp:ListItem Value="Grey">Grey</asp:ListItem>
        <asp:ListItem Value="Lavender">Lavender</asp:ListItem>
        <asp:ListItem Value="LightBlue">Light Blue</asp:ListItem>
        <asp:ListItem Value="LightBrown">Light Brown</asp:ListItem>
        <asp:ListItem Value="LightPurple">Light Purple</asp:ListItem>
        <asp:ListItem Value="Lime">Lime</asp:ListItem>
        <asp:ListItem Value="Olive">Olive</asp:ListItem>
        <asp:ListItem Value="MidBlue">Mid Blue</asp:ListItem>
        <asp:ListItem Value="Mulberry">Mulberry</asp:ListItem>
        <asp:ListItem Value="Navy">Navy</asp:ListItem>
        <asp:ListItem Value="Nutmeg">Nutmeg</asp:ListItem>
        <asp:ListItem Value="Orange">Orange</asp:ListItem>
        <asp:ListItem Value="PaleBlue">Pale Blue</asp:ListItem>
        <asp:ListItem Value="Pink">Pink</asp:ListItem>
        <asp:ListItem Value="Purple">Purple</asp:ListItem>
        <asp:ListItem Value="Red">Red</asp:ListItem>
        <asp:ListItem Value="Turquoise">Turquoise</asp:ListItem>
        <asp:ListItem Value="White">White</asp:ListItem>
        <asp:ListItem Value="Yellow">Yellow</asp:ListItem>
      </asp:ListBox>
    </div>
    
    <div class="configSection interact">
      <p>Interactivity <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please choose which interactive components should be displayed for the product by clicking the boxes.</span></span></p>
      <asp:CheckBox ID="chkLarge" runat="server" Checked="true" Enabled="false" />
      <label for="chkLarge">Large image</label>
      <asp:CheckBox ID="chk360" runat="server" />
      <label for="chk360">360&deg; view</label>
      <asp:CheckBox ID="chkVideo" runat="server" />
      <label for="chkVideo">Video</label>
    </div>
    
    <div class="configSection size">
      <label for="ddlSize">Size chart <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">Please select which size chart should be associated with the product. If there is no size chart you do not need to select one.</span></span></label>
      <asp:DropDownList id="ddlSize" runat="server">
        <asp:ListItem Value="">Please select...</asp:ListItem>
        <asp:ListItem Value="womensSizeChart.html">Women's</asp:ListItem>
        <asp:ListItem Value="mensSizeChart.html">Men's</asp:ListItem>
        <asp:ListItem Value="unisexSizeChart.html">Unisex</asp:ListItem>
        <asp:ListItem Value="workwearSizeChart.html">Workwear</asp:ListItem>
        <asp:ListItem Value="highVisibilitySizeChart.html">High Visibility</asp:ListItem>
        <asp:ListItem Value="maternitySizeChart.html">Maternity</asp:ListItem>
        <asp:ListItem Value="footwearSizeChart.html">Footwear</asp:ListItem>
        <asp:ListItem Value="chefsHatsSizeChart.html">Chef's Hats</asp:ListItem>
      </asp:DropDownList>
    </div>
    
    <div class="configSection reviewCode">
      <label for="txtReviewCode">Shopzilla Page ID <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">In order to pull in reviews please enter the unique Shopzilla Page ID which is associated with the product.</span></span></label>
      <asp:TextBox ID="txtReviewCode" CssClass="txtBox reviewCode" MaxLength="10" Runat="Server" />
    </div>

    
    <div class="configSection theButtons">
    	<asp:Button ID="btnClearProduct" runat="server" Text="Clear product" CssClass="button edit" OnClick="btnNewProduct_Click" />
      <asp:Button ID="btnSubmit" runat="server" Text="Submit product" CssClass="button" OnClick="btnSubmit_Click" ValidationGroup="configEntry" />
  	</div>
  </asp:Panel>
  
  <asp:Panel ID="configDisplay" runat="server" DefaultButton="btnConfirm" Visible="false"> 
  <h1>Product configurator - Asset check</h1>
  <div id="display" class="content">
    <p>Below is the result of the information you have entered on the Product Configurator form. Please check for any mistakes. If you are happy with the information you have entered please click <span class="btnTextConfirm">I confirm these details are correct</span> otherwise click <span class="btnTextEdit">Edit</span> to return to the form.</p>
    
    <div class="configSection theButtons">
      <asp:Button ID="btnEditAlt" runat="server" Text="Edit" CssClass="button edit" OnClick="btnEdit_Click" />
      <asp:Button ID="btnConfirmAlt" runat="server" Text="I confirm these details are correct" CssClass="button" OnClick="btnConfirm_Click" ValidationGroup="configDisplay" />
    </div>
    
    <div class="configSection prodCode">
      <p><strong>Product code</strong> : <br />
      <asp:Literal ID="litProductCode" Text="asdasd" runat="server" /></p>
    </div>
    
    <div class="configSection desc">
      <p><strong>Product description</strong> : <br />
        <asp:Literal ID="litDesc" runat="server" /></p>
    </div>
    
    <div class="configSection roundels">
      <p><strong>Roundels</strong> : </p>
      <asp:Literal ID="litRoundels" runat="server" />
    </div>
    
    <div class="configSection addImages">
      <p><strong>Additional images</strong> : </p>
      <asp:Literal ID="litImages" runat="server" />
    </div>
    
    <div class="configSection colours">
      <p><strong>Colour options</strong> : </p>
      <asp:Literal ID="litColours" runat="server" />
    </div>
    
    <div class="configSection interact clearfix">
      <p><strong>Interactivity</strong> : </p>
      <asp:Literal ID="litLarge" runat="server" />
      <asp:Literal ID="lit360" runat="server" />
      <asp:Literal ID="litVideo" runat="server" />
    </div>
    
    <div class="configSection size">
      <p><strong>Size chart</strong> : </p>
      <asp:Literal ID="litSize" runat="server" />
    </div>
    
    <div class="configSection reviewCode clearfix">
      <p><strong>Reviews</strong> : <asp:Literal ID="litReviewCode" runat="server" /> <asp:Literal ID="litRating" runat="server" /></p>
      <asp:Literal ID="litReviews" runat="server" />
    </div>
    
    <div class="configSection theButtons">
      <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="button edit" OnClick="btnEdit_Click" />
      <asp:Button ID="btnConfirm" runat="server" Text="I confirm these details are correct" CssClass="button" OnClick="btnConfirm_Click" ValidationGroup="configDisplay" />
    </div>
  </div>
  </asp:Panel>
  
  <asp:Panel ID="configConfirm" runat="server" Visible="false"> 
  <h1>Product configurator - Confirmation</h1>
  <div id="confirm" class="content">
    <p class="error">Your files have been successfully written.</p>
    <p>These are the three links <span class="link">(in blue)</span> that you will need to enter in the Product Spreadsheet for your product along with the name of the cell you you need to paste in to.</p>
    <p>If you wish to edit the product again please click <span class="btnTextEdit">Edit</span> to return to the form. You can review what you have entered by clicking <span class="btnTextReview">Review product</span>. To start a new product please click <span class="btnTextConfirm">Configure a new product</span> or click <span class="btnTextConfirm">Load product</span> to open an existing product.</p>
    
    <ul>
      <li>Product Description 2 <span class="instruct">(to be entered into cell <strong>PRDDS2</strong>)</span>: <span class="tooltip"><img src="/Assets/SJL/HTML/Images/info-icon.gif" alt="Information" width="20" height="20" /><span class="tipText">You can triple click on the link text to select the whole line.</span></span>
      	<span class="copyPaste"><asp:Literal id="litPRDDS2code" runat="server" /></span></li>
      <li>Product HTML 1 <span class="instruct">(to be entered into cell <strong>PRDHT1</strong>)</span>: 
      	<span class="copyPaste"><asp:Literal id="litPRDHT1code" runat="server" /></span></li>
      <li>Product HTML 2 <span class="instruct">(to be entered into cell <strong>PRDHT3</strong>)</span>: 
      	<span class="copyPaste"><asp:Literal id="litPRDHT3code" runat="server" /></span></li>
    </ul>
    
    <asp:Button ID="btnEditAgain" runat="server" Text="Edit" CssClass="button edit" OnClick="btnEdit_Click" />
    <asp:Button ID="btnReview" runat="server" Text="Review product" CssClass="button review" OnClick="btnSubmit_Click" />
    <asp:Button ID="btnNewProduct" runat="server" Text="Configure a new product" CssClass="button" OnClick="btnNewProduct_Click" />
  </div>
  </asp:Panel>
  
  <div id="footer">
  
  </div>
</div>
<!--container -->

</form>    
    
</body>
</html>
