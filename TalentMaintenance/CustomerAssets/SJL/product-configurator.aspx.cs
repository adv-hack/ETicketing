using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using System.Net;

public partial class productConfigurator : System.Web.UI.Page {
	
	public int i;
	public int roundelsSelectedCount;
	public int coloursSelectedCount;
	public bool boo360;
	public bool booVideo;
	public bool booLarge;
	public bool liveSite;
	public bool externalAssets;
	public string assetPath;
	public string username;
	public string password;
	public string domain;
	public string strColoursText;
	
	protected void Page_Load(object sender, EventArgs e) {

        //liveSite = false;
        //externalAssets = false;
        //if (liveSite == true && externalAssets == true) {
        //    assetPath = "https://a248.e.akamai.net/images.talentarena.co.uk/simonjerseylive/SJL/";
        //} else if (liveSite == false && externalAssets == true) {
        //    assetPath = "https://a248.e.akamai.net/images.talentarena.co.uk/simonjersey/SJL/";
        //} else if (liveSite == true && externalAssets == false) {
        //    assetPath = "http://www.simonjersey.com/Assets/SJL/";
        //} else {
        //    assetPath = "http://www.simonjerseytest.talent-ebusiness.co.uk/Assets/SJL/";
        //}
        //username = "SJL";
        //password = "sjl";
        //domain = "EBIIS1";

        assetPath = ConfigurationManager.AppSettings["SimonJerseyProductConfiguratorAssetPath"];
        username = ConfigurationManager.AppSettings["SimonJerseyProductConfiguratorUsername"];
        password = ConfigurationManager.AppSettings["SimonJerseyProductConfiguratorPassword"];
        domain = ConfigurationManager.AppSettings["SimonJerseyProductConfiguratorDomain"];
    }    
	
	protected void btnDoLoad_Click(object sender, EventArgs e) {
		configEntry.Visible = false;
    configDisplay.Visible = false;
    configConfirm.Visible = false;
	}
	
	protected void btnLoad_Click(object sender, EventArgs e) {

//		string filePath = Request.PhysicalPath.Substring(0,Request.PhysicalPath.LastIndexOf('\\')) + "\\";
        string filePath = ConfigurationManager.AppSettings["SimonJerseyProductConfiguratorFilePath"];
		
		if (File.Exists(filePath + txtProductCode.Text + "\\" + txtProductCode.Text + ".txt")) {
			litError.Text = "<p class=\"error\">The details of your product are below.</p>";
			// create reader & open file
      TextReader trStored = new StreamReader(filePath + txtProductCode.Text + "\\" + txtProductCode.Text + ".txt");
      // read a line of text
      txtProductCode.Text = trStored.ReadLine();
			txtDescription.Text = trStored.ReadLine();
			
			string storedRoundels = trStored.ReadLine();
			if (storedRoundels.Length > 0) {
				string[] arrRoundels = storedRoundels.Split(',');
				// Read array items using foreach loop
				foreach (string roundel in arrRoundels) {
					lbxRoundels.Items[Convert.ToInt32(roundel)].Selected = true;
				}
			}
			
			if (trStored.ReadLine() == "true") {
				chkImage1.Checked = true;
			} else {
				chkImage1.Checked = false;
			}
			
			if (trStored.ReadLine() == "true") {
				chkImage2.Checked = true;
			} else {
				chkImage2.Checked = false;
			}
			
			if (trStored.ReadLine() == "true") {
				chkImage3.Checked = true;
			} else {
				chkImage3.Checked = false;
			}
			
			if (trStored.ReadLine() == "true") {
				chkImage4.Checked = true;
			} else {
				chkImage4.Checked = false;
			}
			
			if (trStored.ReadLine() == "true") {
				chkImage5.Checked = true;
			} else {
				chkImage5.Checked = false;
			}
			
			string storedColours = trStored.ReadLine();
			if (storedColours.Length > 0) {
				string[] arrColours = storedColours.Split(',');
				// Read array items using foreach loop
				foreach (string colour in arrColours) {
					lbxColours.Items[Convert.ToInt32(colour)].Selected = true;
				}
			}
			
			if (trStored.ReadLine() == "true") {
				chkLarge.Checked = true;
			} else {
				chkLarge.Checked = false;
			}
			
			if (trStored.ReadLine() == "true") {
				chk360.Checked = true;
			} else {
				chk360.Checked = false;
			}
			
			if (trStored.ReadLine() == "true") {
				chkVideo.Checked = true;
			} else {
				chkVideo.Checked = false;
			}
			
			ddlSize.SelectedIndex = Convert.ToInt32(trStored.ReadLine());
			txtReviewCode.Text = trStored.ReadLine();
			
      // close the stream
      trStored.Close();
		} else {
			litError.Text = "<p class=\"error\">Sorry, this product has not yet been configured or we can't find the files. Please use the form below to configure the product from scratch.</p>";
		}
		
		configEntry.Visible = true;
    configDisplay.Visible = false;
    configConfirm.Visible = false;
	}
	
	protected void btnSubmit_Click(object sender, EventArgs e) {
		configEntry.Visible = false;
    configDisplay.Visible = true;
    configConfirm.Visible = false;
		
		litProductCode.Text = txtProductCode.Text;
		litDesc.Text = txtDescription.Text;
		
		
		i = 0;
		roundelsSelectedCount = 0;
		for (i = 0; i < lbxRoundels.Items.Count; i++) {
			if (lbxRoundels.Items[i].Selected) {
				roundelsSelectedCount++;
			}
		}
		
		i = 0;
		if (roundelsSelectedCount > 0) {
			litRoundels.Text = "<ul>";
			for (i = 0; i < lbxRoundels.Items.Count; i++) {
				if (lbxRoundels.Items[i].Selected) {
					litRoundels.Text = litRoundels.Text + "<li><img src=\"" + assetPath + "HTML/Images/roundels/" + lbxRoundels.Items[i].Value + "\" alt=\"" + lbxRoundels.Items[i].Text + "\" /></li>";
				}
			}
			litRoundels.Text = litRoundels.Text + "</ul>";
		}

		
		/*chkImage1*/
		if (chkImage1.Checked || chkImage2.Checked || chkImage3.Checked || chkImage4.Checked || chkImage5.Checked) {
			litImages.Text = "<ul>";
			
			if (chkImage1.Checked) {
				string imageExt = "";
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/1/" + txtProductCode.Text + ".gif"); 
					if (liveSite == false && externalAssets == false) {
						//Response.Write(username);
						myHttpWebRequest.Credentials = new NetworkCredential(username, password, domain);
					}
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
					//Response.Write(myHttpWebResponse.StatusCode);
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "gif";
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch (Exception excep) {
					// Response.Write(excep.Message);
					 
				} finally {
					
				}
				
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/1/" + txtProductCode.Text + ".jpg"); 
					if (liveSite == false && externalAssets == false) {
						//Response.Write(username);
						myHttpWebRequest.Credentials = new NetworkCredential(username, password, domain);
					}
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
					//Response.Write(myHttpWebResponse.StatusCode);
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "jpg";
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch (Exception excep) {
					// Response.Write(excep.Message);
					 
				} finally {
					
				}
				
				if (imageExt == "gif") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/1/" + txtProductCode.Text + ".gif\" alt=\"" + txtProductCode.Text + " alternate image 1\" /></li>";
				} else if (imageExt == "jpg") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/1/" + txtProductCode.Text + ".jpg\" alt=\"" + txtProductCode.Text + " alternate image 1\" /></li>";
				} else {
					litImages.Text += "<li class=\"message\">You have selected alternative image 1 to be shown but this does not exist. This image will not be shown on the product detail page.</li>";
				}
			}
			
			if (chkImage2.Checked) {
				string imageExt = "";
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/2/" + txtProductCode.Text + ".gif"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "gif";
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					 
				} finally {
					
				}
				
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/2/" + txtProductCode.Text + ".jpg"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "jpg";
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					 
				} finally {
					
				}
				
				if (imageExt == "gif") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/2/" + txtProductCode.Text + ".gif\" alt=\"" + txtProductCode.Text + " alternate image 2\" /></li>";
				} else if (imageExt == "jpg") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/2/" + txtProductCode.Text + ".jpg\" alt=\"" + txtProductCode.Text + " alternate image 2\" /></li>";
				} else {
					litImages.Text += "<li class=\"message\">You have selected alternative image 2 to be shown but this does not exist. This image will not be shown on the product detail page.</li>";
				}
			}
			
			if (chkImage3.Checked) {
				string imageExt = "";
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/3/" + txtProductCode.Text + ".gif"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "gif";	
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					 
				} finally {
					
				}	
				
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/3/" + txtProductCode.Text + ".jpg"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "jpg";	
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					 
				} finally {
					
				}	
				
				if (imageExt == "gif") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/3/" + txtProductCode.Text + ".gif\" alt=\"" + txtProductCode.Text + " alternate image 3\" /></li>";
				} else if (imageExt == "jpg") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/3/" + txtProductCode.Text + ".jpg\" alt=\"" + txtProductCode.Text + " alternate image 3\" /></li>";
				} else {
					litImages.Text += "<li class=\"message\">You have selected alternative image 3 to be shown but this does not exist. This image will not be shown on the product detail page.</li>";
				}
			}
			
			if (chkImage4.Checked) {
				string imageExt = "";
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/4/" + txtProductCode.Text + ".gif"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "gif";	
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					
				} finally {
					
				}
				
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/4/" + txtProductCode.Text + ".jpg"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "jpg";	
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					
				} finally {
					
				}
				
				if (imageExt == "gif") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/4/" + txtProductCode.Text + ".gif\" alt=\"" + txtProductCode.Text + " alternate image 4\" /></li>";
				} else if (imageExt == "jpg") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/4/" + txtProductCode.Text + ".jpg\" alt=\"" + txtProductCode.Text + " alternate image 4\" /></li>";
				} else {
					litImages.Text += "<li class=\"message\">You have selected alternative image 4 to be shown but this does not exist. This image will not be shown on the product detail page.</li>"; 
				}	
			}
			
			if (chkImage5.Checked) {
				string imageExt = "";
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/5/" + txtProductCode.Text + ".gif"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "gif";	
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					 
				} finally {
					
				}
				
				try {	
					// Creates an HttpWebRequest for the specified URL. 
					HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/5/" + txtProductCode.Text + ".jpg"); 
					// Sends the HttpWebRequest and waits for a response.
					HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
					if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
						imageExt = "jpg";	
					}
					// Releases the resources of the response.
					myHttpWebResponse.Close();
				} catch {
					 
				} finally {
					
				}
				
				if (imageExt == "gif") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/5/" + txtProductCode.Text + ".gif\" alt=\"" + txtProductCode.Text + " alternate image 5\" /></li>";
				} else if (imageExt == "jpg") {
					litImages.Text += "<li><img src=\"" + assetPath + "Images/Product/Alt/5/" + txtProductCode.Text + ".jpg\" alt=\"" + txtProductCode.Text + " alternate image 5\" /></li>";
				} else {
					litImages.Text += "<li class=\"message\">You have selected alternative image 5 to be shown but this does not exist. This image will not be shown on the product detail page.</li>"; 
				}
			}
			litImages.Text += "</ul>";
		}
		
		i = 0;
		coloursSelectedCount = 0;
		for (i = 0; i < lbxColours.Items.Count; i++) {
			if (lbxColours.Items[i].Selected) {
				coloursSelectedCount++;
			}
		}
		
		i = 0;
		if (coloursSelectedCount > 0) {
			litColours.Text = "<ul>";
			for (i = 0; i < lbxColours.Items.Count; i++) {
				if (lbxColours.Items[i].Selected) {
					litColours.Text = litColours.Text + "<li class=\"" + lbxColours.Items[i].Value + "\"><span class=\"colour\"></span><span class=\"text\">" + lbxColours.Items[i].Text.Replace(" / ", "/<br />") + "</span></li>";
				}
			}
			litColours.Text = litColours.Text + "</ul>";
		}
		
		if (chkLarge.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "HTML/Images/Large/" + txtProductCode.Text + ".jpg"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					litLarge.Text = "<div id=\"large\"><p>Large image <span>(scaled down)</span></p><img src=\"" + assetPath + "HTML/Images/Large/" + txtProductCode.Text + ".jpg\" /></div>";			
      	}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			} catch {
				litLarge.Text = "<div id=\"large\"><p>Large image <span>(scaled down)</span><p class=\"message\">You have selected a large image to be shown for this product but this does not exist. The large image will not be shown on the product detail page. You need to upload a large image.</p></div>"; 
			}
		} else {
			litLarge.Text = "";
		}
		
		if (chk360.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "360/" + txtProductCode.Text + ".html"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					lit360.Text = "<div id=\"view360\"><p>360&deg; view</p><iframe width=\"350\" height=\"450\" frameborder=\"0\" scrolling=\"auto\" src=\"" + assetPath + "360/" + txtProductCode.Text + ".html\"></iframe></div>";			
      	}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			} catch {
				lit360.Text = "<div id=\"view360\"><p>360&deg; view</p><p class=\"message\">You have selected a 360&deg; view to be shown for this product but this does not exist. The 360&deg; view will not be shown on the product detail page.</p></div>"; 
			}
		} else {
			lit360.Text = "";
		}
		
		
		if (chkVideo.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Video/" + txtProductCode.Text + ".flv"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					litVideo.Text = "<div id=\"video\"><p>Video</p><span id=\"videoHolder\"><script type=\"text/javascript\">var flashvars = {};	flashvars.videoID = \"" + assetPath + "Video/" + txtProductCode.Text + ".flv\";	var params = {}; swfobject.embedSWF(\"" + assetPath + "Video/video.swf\", \"videoHolder\", \"270\", \"480\", \"9.0.0\",\"expressInstall.swf\", flashvars, params);</script></span></div>";			
      	}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			} catch {
				litVideo.Text = "<div id=\"video\"><p>Video</p><p class=\"message\">You have selected a video to be shown for this product but this does not exist. The video view will not be shown on the product detail page.</p></div>"; 
			}
		} else {
			litVideo.Text = "";
		}
		
		if (ddlSize.SelectedValue == "") {
			litSize.Text = "";
		} else {
			litSize.Text = "<iframe width=\"800\" height=\"460\" scrolling=\"auto\" src=\"" + assetPath + "HTML/" + ddlSize.SelectedValue + "\"></iframe>";
		}
		
		//Response.Write(txtReviewCode.Text.Length);
		if (txtReviewCode.Text.Length > 0) {
			litReviewCode.Text = txtReviewCode.Text;
			litRating.Text = "<div id=\"ratingsHolder\"><script type=\"text/javascript\">var pr_style_sheet=\"http://cdn.shopzilla.com/aux/12022/4069/css/express.css\";</script><script type=\"text/javascript\" src=\"http://cdn.shopzilla.com/repos/12022/pr/pwr/engine/js/full.js\"></script><script type=\"text/javascript\">SHOPZILLAREVIEWS.display.snippet(document, { pr_page_id : \"" + txtReviewCode.Text + "\" });</script></div>";
			litReviews.Text = "<div id=\"reviewsHolder\"><script type=\"text/javascript\">SHOPZILLAREVIEWS.display.engine(document, { pr_page_id : \"" + txtReviewCode.Text + "\" });</script></div>";
		} else {
			litReviewCode.Text = "";
			litRating.Text = "";
			litReviews.Text ="";
		}
		
	}
	
	protected void btnConfirm_Click(object sender, EventArgs e) {
		configEntry.Visible = false;
    configDisplay.Visible = false;
    configConfirm.Visible = true;

//    string filePath = Request.PhysicalPath.Substring(0, Request.PhysicalPath.LastIndexOf('\\')) + "\\";
    string filePath = ConfigurationManager.AppSettings["SimonJerseyProductConfiguratorFilePath"];
		
		if (Directory.Exists(filePath + txtProductCode.Text)) {
		
		} else {
			System.IO.Directory.CreateDirectory(filePath + txtProductCode.Text);
		}

		
		// create a writer and open the backup file
    TextWriter twBackup = new StreamWriter(filePath + txtProductCode.Text + "\\" + txtProductCode.Text + ".txt");
		// write text to the file
		twBackup.WriteLine(txtProductCode.Text);
		twBackup.WriteLine(txtDescription.Text);
		
		string strRoundels = "";
		i = 0;
		for (i = 0; i < lbxRoundels.Items.Count; i++) {
			if (lbxRoundels.Items[i].Selected) {
				strRoundels += i + ",";
			}
		}
		if (strRoundels.Length > 0) {
			strRoundels = strRoundels.Substring(0,strRoundels.Length-1);
		}
		twBackup.WriteLine(strRoundels);
		
		if (chkImage1.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		if (chkImage2.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		if (chkImage3.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		if (chkImage4.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		if (chkImage5.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		string strColours = "";
		i = 0;
		for (i = 0; i < lbxColours.Items.Count; i++) {
			if (lbxColours.Items[i].Selected) {
				strColours += i + ",";
			}
		}
		if (strColours.Length > 0) {
			strColours = strColours.Substring(0,strColours.Length-1);
		}
		twBackup.WriteLine(strColours);
		
		if (chkLarge.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		if (chk360.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		if (chkVideo.Checked) {
			twBackup.WriteLine("true");
		} else {
			twBackup.WriteLine("false");
		}
		
		twBackup.WriteLine(ddlSize.SelectedIndex);
		twBackup.WriteLine(txtReviewCode.Text);
		// close the stream
		twBackup.Close();
		
		// create a writer and open the PRDDS2 file
    TextWriter twPRDDS2 = new StreamWriter(filePath + txtProductCode.Text.Replace("'", "&rsquo;") + "\\PRDDS2.js");
		// write string
		string strPRDDS2 = "if ($(\"#aspnetForm\").attr(\"class\") == \"form-product\") {document.write('<script type=\"text/javascript\">var pr_style_sheet=\"http://cdn.shopzilla.com/aux/12022/4069/css/express.css\";</script><script type=\"text/javascript\" src=\"http://cdn.shopzilla.com/repos/12022/pr/pwr/engine/js/full.js\"></script><script type=\"text/javascript\" src=\"" + assetPath + "HTML/Javascript/products.js\"></script><div class=\"star-rating\"><script type=\"text/javascript\">SHOPZILLAREVIEWS.display.snippet(document, { pr_page_id : \"" + txtReviewCode.Text.Replace("'", "&rsquo;") + "\" });</script></div><div class=\"productTabs\"><ul class=\"tabs\"><li><a class=\"tabDesc tabSection\" href=\"javascript:void(0);\">Description</a></li> <li><a class=\"tabImages";
		
		if (chkImage1.Checked || chkImage2.Checked || chkImage3.Checked || chkImage4.Checked || chkImage5.Checked) {
			strPRDDS2 += " tabSection";
		} else {
			strPRDDS2 += " tabDisable";
		}
		
		strPRDDS2 += "\" href=\"javascript:void(0);\">More images</a></li><li><a class=\"tabColours";
		
		i = 0;
		coloursSelectedCount = 0;
		for (i = 0; i < lbxColours.Items.Count; i++) {
			if (lbxColours.Items[i].Selected) {
				coloursSelectedCount++;
			}
		}

		if (coloursSelectedCount > 0) {
			strPRDDS2 += " tabSection";
		} else {
			strPRDDS2 += " tabDisable";
		}
		
		strPRDDS2 += "\" href=\"javascript:void(0);\">Colours</a></li><li><a class=\"tabReviews";
		
		if (txtReviewCode.Text.Length > 0) {
			strPRDDS2 += " tabSection";
		} else {
			strPRDDS2 += " tabDisable";
		}
		
		strPRDDS2 += "\" href=\"javascript:void(0);\">Reviews</a></li><li class=\"last\"><a class=\"tabSize";
		
		if (ddlSize.SelectedValue == "") {
			strPRDDS2 += " tabDisable\" href=\"javascript:void(0);";
		} else {
			strPRDDS2 += " thickbox\" href=\"" + assetPath + "HTML/" + ddlSize.SelectedValue.Replace("'", "&rsquo;") + "?TB_iframe=true&width=800&height=460";
		}

		strPRDDS2 += "\">Size Guide</a></li> </ul> <div class=\"productInfo\"><div class=\"infoSection description\"><div class=\"roundels\"> ";
		
		i = 0;
		for (i = 0; i < lbxRoundels.Items.Count; i++) {
			if (lbxRoundels.Items[i].Selected) {
				strPRDDS2 += "<img src=\"" + assetPath + "HTML/Images/roundels/" + lbxRoundels.Items[i].Value.Replace("'", "&rsquo;") + "\" alt=\"" + lbxRoundels.Items[i].Text.Replace("'", "&rsquo;") + "\" />";
			}
		}
		//strColoursText = txtDescription.Text;
		//Response.Write(strColoursText.Replace("'", "&rsquo;"));
		strPRDDS2 += "</div>" + txtDescription.Text.Replace("'", "&rsquo;");
		//Response.Write( txtDescription.Text.Replace("'", "&rsquo;"));
		strPRDDS2 += "</div><div class=\"infoSection moreImages\">";
		
		if (chkImage1.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/1/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/1/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 1\" style=\"border-width:0px;\" />";			
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}
			
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/1/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/1/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 1\" style=\"border-width:0px;\" />";			
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}
		}
		
		if (chkImage2.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/2/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/2/" +txtProductCode.Text.Replace("'", "&rsquo;") + ".gif\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 2\" style=\"border-width:0px;\" />";
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}
			
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/2/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/2/" +txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 2\" style=\"border-width:0px;\" />";
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}
		}
		
		if (chkImage3.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/3/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/3/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 3\" style=\"border-width:0px;\" />";
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}
			
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/3/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/3/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 3\" style=\"border-width:0px;\" />";
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}	
		}
		
		if (chkImage4.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/4/" +txtProductCode.Text.Replace("'", "&rsquo;") + ".gif"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/4/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 4\" style=\"border-width:0px;\" />";	
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}
			
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/4/" +txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/4/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 4\" style=\"border-width:0px;\" />";	
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}	
		}
		
		if (chkImage5.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/5/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/5/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".gif\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 5\" style=\"border-width:0px;\" />";
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}	
			
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Images/Product/Alt/5/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					strPRDDS2 += "<img src=\"" + assetPath + "Images/Product/Alt/5/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg\" alt=\"" + txtProductCode.Text.Replace("'", "&rsquo;") + " alternate image 5\" style=\"border-width:0px;\" />";
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			}	catch {
			
			}	
		}

		strPRDDS2 += "</div><div class=\"infoSection colours\"><ul>";
		
		i = 0;
		for (i = 0; i < lbxColours.Items.Count; i++) {
			if (lbxColours.Items[i].Selected) {
				strColoursText = lbxColours.Items[i].Text.Replace(" / ", "/<br />");
				strPRDDS2 += "<li class=\"" + lbxColours.Items[i].Value + "\"><span class=\"colour\"></span><span class=\"text\">" + strColoursText.Replace("'", "&rsquo;") + "</span></li>";
			}
		}

		strPRDDS2 += "</ul></div><div class=\"infoSection reviews\"><script type=\"text/javascript\">SHOPZILLAREVIEWS.display.engine(document, { pr_page_id : \"" + txtReviewCode.Text.Replace("'", "&rsquo;") + "\" });</script></div></div></div><p class=\"instructions\"><a href=\"javascript:void(0);\" onClick=\"document.location.hash=\\'ctl00_ContentPlaceHolder1_ProductInformation3_ProductControls1_ProductOptions1_Level1Options\\';\">Select size and quantity below</a></p>');}";
		// write text to the file
		twPRDDS2.WriteLine(strPRDDS2);
		// close the stream
		twPRDDS2.Close();
		
		
		
		// create a writer and open the PRDDS2 file
    TextWriter twPRDHT1 = new StreamWriter(filePath + txtProductCode.Text.Replace("'", "&rsquo;") + "\\PRDHT1.js");
		// write string
		string strPRDHT1 = "document.write(\'<a href=\"http://www.facebook.com/simonjersey\" target=\"_blank\" class=\"socialFacebook\"><img src=\"" + assetPath + "HTML/Images/social/facebook.gif\" width=\"44\" height=\"44\" alt=\"Follow Simon Jersey on Facebook for beauty industry news and exclusive offers\" /></a> <a href=\"http://twitter.com/simonjersey\" target=\"_blank\" class=\"socialTwitter\"><img src=\"" + assetPath + "HTML/Images/social/twitter.gif\" width=\"44\" height=\"44\" alt=\"Follow Simon Jersey on Twitter for beauty industry news and exclusive offers\" /></a> <a href=\"http://www.youtube.com/SimonJerseyUK\" target=\"_blank\" class=\"socialYoutube\"><img src=\"" + assetPath + "HTML/Images/social/youtube.gif\" width=\"44\" height=\"44\" alt=\"Follow Simon Jersey on YouTube for beauty industry news and exclusive offers\" /></a>\');";
		// write text to the file
		twPRDHT1.WriteLine(strPRDHT1);
		// close the stream
		twPRDHT1.Close();
		
		
		
		// create a writer and open the PRDDS2 file
    TextWriter twPRDHT3 = new StreamWriter(filePath + txtProductCode.Text.Replace("'", "&rsquo;") + "\\PRDHT3.js");
		
		try {	
			// Creates an HttpWebRequest for the specified URL. 
			HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "HTML/Images/Large/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg"); 
			// Sends the HttpWebRequest and waits for a response.
			HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
			if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
				booLarge = true;			
			}
			// Releases the resources of the response.
			myHttpWebResponse.Close();
		} catch {
			booLarge = false; 
		}
		
		string strPRDHT3 = "document.write(\'<a href=\"";
		if (booLarge == true) {
			// write string
			strPRDHT3 += "#\" class=\"largeImage\" onClick=\"MM_openBrWindow(\\'" + assetPath + "HTML/Images/Large/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".jpg\\',\\'largeImage\\',\\'width=800,height=800\\'); return false;\"";
		} else {
			strPRDHT3 += "javascript:void(0);\" class=\"largeImage noLarge\"";
		}

		strPRDHT3 += ">Enlarge</a> <a href=\"";
		if (chk360.Checked) {
			try {	
				// Creates an HttpWebRequest for the specified URL. 
				HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "360/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".html"); 
				// Sends the HttpWebRequest and waits for a response.
				HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
				if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
					boo360 = true;		
				}
				// Releases the resources of the response.
				myHttpWebResponse.Close();
			} catch {
				boo360 = false;
			}
		} else {
		
		}

		if (boo360 == true) {
			strPRDHT3 += assetPath + "360/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".html?TB_iframe=true&width=350&height=450";
		} else {
			strPRDHT3 += "javascript:void(0);";
		}
		
		if (boo360 == true) {
			strPRDHT3 += "\" class=\"thickbox image360";
		} else {
			strPRDHT3 += "\" class=\"image360";
		}
		
		if (boo360 == false) {
			strPRDHT3 += " no360";
		}
		
		
		try {	
			// Creates an HttpWebRequest for the specified URL. 
			HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(assetPath + "Video/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".flv"); 
			// Sends the HttpWebRequest and waits for a response.
			HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
			if (myHttpWebResponse.StatusCode == HttpStatusCode.OK) {
				booVideo = true;			
			}
			// Releases the resources of the response.
			myHttpWebResponse.Close();
		} catch {
			booVideo = false; 
		}
		
		strPRDHT3 += "\">360&deg;</a> <a href=\"";
		if (booVideo == true) {
			strPRDHT3 += assetPath + "Video/" + txtProductCode.Text.Replace("'", "&rsquo;") + ".html?TB_iframe=true&width=270&height=480";
		} else {
			strPRDHT3 += "javascript:void(0);";
		}
		
		if (booVideo == true) {
			strPRDHT3 += "\" class=\"thickbox imageVideo";
		} else {
			strPRDHT3 += "\" class=\"imageVideo";
		}

		if (booVideo == false) {
			strPRDHT3 += " noVideo";
		}
		//strPRDHT3 += "\">Play video</a> <img class=\"productBanner\" src=\"" + assetPath + "HTML/Images/banners/simon-jersey-cares.jpg\" alt=\"Simon Jersey Cares\" />\');";
		strPRDHT3 += "\">Play video</a>\');";
		// write text to the file
		twPRDHT3.WriteLine(strPRDHT3);
		// close the stream
		twPRDHT3.Close();
		
		
		litPRDDS2code.Text = "&lt;script type=\"text/javascript\" src=\"" + assetPath + "HTML/Products/" + txtProductCode.Text + "/PRDDS2.js\"&gt;&lt;/script&gt;";
		litPRDHT1code.Text = "&lt;script type=\"text/javascript\" src=\"" + assetPath + "HTML/Products/" + txtProductCode.Text + "/PRDHT1.js\"&gt;&lt;/script&gt;";
		litPRDHT3code.Text = "&lt;script type=\"text/javascript\" src=\"" + assetPath + "HTML/Products/" + txtProductCode.Text + "/PRDHT3.js\"&gt;&lt;/script&gt;";
	}
	
	protected void btnEdit_Click(object sender, EventArgs e) {
		configEntry.Visible = true;
    configDisplay.Visible = false;
    configConfirm.Visible = false;
	}
	
	protected void btnNewProduct_Click(object sender, EventArgs e) {
		Response.Redirect(Request.Path);
	}

}
