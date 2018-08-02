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
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Threading;

/// <summary>
/// Partial class for web page
/// </summary>
public partial class dress_explorer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string fileLocation = Server.MapPath(ConfigurationManager.AppSettings["FileLocation"]);
            fileLocation = Path.Combine(fileLocation, "_files/newsletter");

            string fileName = string.Format("NEWSLETTER{0}.csv", DateTime.Now.ToString("yyyyMMdd"));

            FileInfo fi = new FileInfo(Path.Combine(fileLocation, fileName));

            if (!fi.Directory.Exists)
                fi.Directory.Create();

            FileStream fs;
            StreamWriter writer;

            if (!fi.Exists)
            {
                fs = fi.Create();
                writer = new StreamWriter(fs);
                writer.WriteLine("\"Name\",\"Email Address\",\"Mobile\"");
            }
            else
            {
                fs = fi.OpenWrite();
                fs.Position = fs.Length;
                writer = new StreamWriter(fs);
            }

            writer.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\"", Fullname.Text.ToString(), Email.Text.ToString(), Mobile.Text.ToString()));
            writer.Dispose();
            fs.Dispose();

            string ftpLocation = ConfigurationManager.AppSettings["NewsletterFtpLocation"];
            string ftpUsername = ConfigurationManager.AppSettings["NewsletterFtpUsername"];
            string ftpPassword = ConfigurationManager.AppSettings["NewsletterFtpPassword"];

            string ftpResult = Utility.FtpFileUpload(ftpLocation, ftpUsername, ftpPassword, fi.FullName, fi.Name);
            if (string.IsNullOrEmpty(ftpResult))
            {
                NewsletterSignupForm.Visible = false;
                NewsletterThankYou.Visible = true;
                NewsletterError.Visible = false;
            }
            else
            {
                NewsletterSignupForm.Visible = false;
                NewsletterThankYou.Visible = false;
                NewsletterError.Visible = true;
                ErrorMessage.Text = ftpResult;
            }

        }
        else
        {
            NewsletterSignupForm.Visible = false;
            NewsletterError.Visible = true;
        }
    }
}


/// <summary>
/// Summary description for FtpState
/// </summary>
public class FtpState
{
    private ManualResetEvent wait;
    private FtpWebRequest request;
    private string fileName;
    private Exception operationException = null;
    string status;

    public FtpState()
    {
        wait = new ManualResetEvent(false);
    }

    public ManualResetEvent OperationComplete
    {
        get { return wait; }
    }

    public FtpWebRequest Request
    {
        get { return request; }
        set { request = value; }
    }

    public string FileName
    {
        get { return fileName; }
        set { fileName = value; }
    }
    public Exception OperationException
    {
        get { return operationException; }
        set { operationException = value; }
    }
    public string StatusDescription
    {
        get { return status; }
        set { status = value; }
    }
}


/// <summary>
/// Utility class for all common function
/// </summary>
public static class Utility
{
    #region"Validation"

    /// <summary>
    /// determent whether it is a number or not
    /// </summary>
    public static bool IsNumeric(object inputObject)
    {
        if (inputObject == null || inputObject == DBNull.Value)
        {
            return false;
        }
        else
        {
            Regex _isNumber = new System.Text.RegularExpressions.Regex("(^[-+]?\\d+(,?\\d*)*\\.?\\d*([Ee][-+]\\d*)?$)|(^[-+]?\\d?(,?\\d*)*\\.\\d+([Ee][-+]\\d*)?$)");
            return _isNumber.Match(inputObject.ToString()).Success;
        }

    }

    /// <summary>
    /// check item exists in a drop down
    /// </summary>
    /// <param name="ddl"></param>
    /// <param name="checkString"></param>
    /// <returns>bool</returns>
    public static bool DropDownItemExist(DropDownList ddl, string checkString)
    {
        bool exist = false;

        foreach (ListItem l in ddl.Items)
        {
            if (l.Text.ToLower() == checkString.ToLower())
            {
                exist = true;
                break;
            }
        }

        return exist;
    }


    #endregion

    #region"Convetor"

    #region"String"



    /// <summary>
    /// turn a object value into a string
    /// </summary>
    /// <param name="s"></param>
    /// <returns>string</returns>
    public static string NullStringReturn(object s)
    {
        if (s == null)
        {
            return "";
        }
        else
        {
            return s.ToString();
        }
    }

    /// <summary>
    /// turn a object value into a string empty
    /// </summary>
    /// <param name="s"></param>
    /// <returns>string</returns>
    public static string StringEmptyNullReturn(object s)
    {
        if (NullStringReturn(s).Trim().Length == 0)
        {
            return null;
        }
        else
        {
            return s.ToString();
        }
    }

    /// <summary>
    /// Replace _ with -
    /// </summary>
    /// <param name="s"></param>
    /// <returns>string</returns>
    public static string HashStringReturn(string s)
    {
        return s.Replace("_", "-");
    }

    /// <summary>
    /// Replace - with _
    /// </summary>
    /// <param name="s"></param>
    /// <returns>string</returns>
    public static string HashStringRemove(string s)
    {
        return s.Replace(" ", "-");
    }

    /// <summary>
    /// Replace _ with space
    /// </summary>
    /// <param name="s"></param>
    /// <returns>string</returns>
    public static string SpaceStringReturn(string s)
    {
        return NullStringReturn(s).Replace("_", " ");
    }

    /// <summary>
    ///  Replace(" ", "_").Replace("&amp;", "and").Replace("&", "and") for combine in database
    /// </summary>
    /// <param name="s"></param>
    /// 
    public static string SpaceStringRemove(string s)
    {
        return NullStringReturn(s).Replace(" ", "_").Replace("&amp;", "and").Replace("&", "and");
    }

    #endregion

    #region"Number"

    /// <summary>
    /// turn a object value into an int32
    /// </summary>
    /// <param name="n"></param>
    /// <returns>int</returns>
    public static Int32 NullInt32Return(object n)
    {
        if (!IsNumeric(n))
        {
            return 0;
        }
        else
        {
            return Convert.ToInt32(n);
        }
    }


    /// <summary>
    /// turn a object value into an int32
    /// </summary>
    /// <param name="n"></param>
    /// <returns>int</returns>
    public static object NullInt32ReturnNull(object n)
    {
        if (!IsNumeric(n))
        {
            return null;
        }
        else
        {
            return Convert.ToInt32(n);
        }
    }


    public static object NullNumber(object n)
    {
        if (n == null)
        {
            return null;
        }
        else
        {
            return Convert.ToInt32(n);
        }
    }

    #endregion

    #endregion

    #region"Gridview"

    public static void GridViewDataBind(GridView gridView, DataTable dt, string cacheName)
    {
        DataView dataView = new DataView(dt);
        gridView.DataSource = dataView;
        gridView.DataBind();
        HttpContext.Current.Cache[cacheName] = gridView.DataSource;
    }


    public static void GridViewDataBindReload(GridView gridView, string cacheName)
    {
        if (HttpContext.Current.Cache[cacheName] != null)
        {
            gridView.DataSource = HttpContext.Current.Cache[cacheName];
        }
    }


    public static string ConvertSortDirectionToSql(SortDirection sortDirection)
    {
        string newSortDirection = String.Empty;
        String direction = (String)HttpContext.Current.Session["sortDirection"];
        if (direction != null)
        {
            if (direction == "ASC")
            {
                newSortDirection = "DESC";
            }
            else
            {
                newSortDirection = "ASC";
            }
        }
        else
        {
            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = "ASC";
                    break;

                case SortDirection.Descending:
                    newSortDirection = "DESC";
                    break;
            }
        }
        HttpContext.Current.Session["sortDirection"] = newSortDirection;
        return newSortDirection;

    }

    public static void GridView_Sorting(GridView gridView, GridViewSortEventArgs e, string cacheName)
    {

        DataView dataView = gridView.DataSource as DataView;
        if (dataView != null)
        {
            dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection);

            gridView.DataSource = dataView.Table;
            gridView.DataSource = HttpContext.Current.Cache[cacheName];
            gridView.DataBind();
        }
    }
    #endregion

    #region"Url"

    /// <summary>
    /// inculde the virual path
    /// </summary>
    public static string VirtualPathInculde(string input)
    {
        if (input != null)
        {
            try
            {
                return VirtualPathUtility.ToAbsolute("~/" + input);
            }
            catch
            {
                //if the input is like http... it will catch here
                return input;
            }
        }
        else
        {
            return VirtualPathUtility.ToAbsolute("~/");
        }
    }

    /// <summary>
    /// Query String space replace
    /// note for javascript use replace(/ /g, "_")
    /// </summary>
    public static string QueryStringSpaceReplace(string queryString)
    {
        string sQueryString = queryString;

        if (sQueryString == null)
        {
            sQueryString = "";
        }
        return sQueryString.Replace(" ", "_");
    }

    /// <summary>
    /// Query String space return 
    /// </summary>
    public static string QueryStringSpaceReturn(string queryString)
    {
        string sQueryString = queryString;

        if (sQueryString == null)
        {
            sQueryString = "";
        }
        //space in query string is +
        return sQueryString.Replace(" ", "+").Replace("_", " ");
    }

    #endregion

    #region"Enumn"

    /// <summary>
    /// return Enum into hashTable
    /// </summary>  
    public static Hashtable EnumHashTable(Type enumType)
    {
        // get the names from the enumeration
        string[] names = Enum.GetNames(enumType);
        // get the values from the enumeration
        Array values = Enum.GetValues(enumType);


        // turn it into a hash table
        Hashtable ht = new Hashtable();
        for (int i = 0; i < names.Length; i++)
        {
            // note the cast to integer here is important
            // otherwise we'll just get the enum string back again
            ht.Add(names[i], (int)values.GetValue(i));
        }
        // return the dictionary to be bound to

        return ht;
    }

    /// <summary>
    /// return Enum into datatable
    /// </summary>  
    public static DataTable EnumDatatable(Type enumType)
    {
        // get the names from the enumeration
        string[] names = Enum.GetNames(enumType);
        // get the values from the enumeration
        Array values = Enum.GetValues(enumType);

        DataTable dt = new DataTable();
        dt.Columns.Add("Key");
        dt.Columns.Add("Value");
        for (int i = 0; i < names.Length; i++)
        {
            DataRow dr = dt.NewRow();
            dr["Key"] = SpaceStringReturn(names[i]);
            dr["Value"] = Convert.ToInt32(values.GetValue(i)).ToString();
            dt.Rows.Add(dr);
        }
        // return the dictionary to be bound to

        return dt;
    }

    //convert enum 
    public static int EnumInt(Type enumType)
    {
        return Convert.ToInt32(enumType);
    }

    //convert enum 
    public static string EnumIntString(Type enumType)
    {
        return Convert.ToInt32(enumType).ToString();
    }



    #endregion

    #region"Files"




    /// <summary>
    /// get the file name
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public static string GetExtension(string FileName)
    {
        string[] split = FileName.Split('.');
        string Extension = split[split.Length - 1];
        return Extension;
    }


    //copy file is other not exits
    public static string FileCheckCopy(string folderpath, string filename, string filenameCopy)
    {
        string fileFullPath = HttpContext.Current.Server.MapPath(folderpath) + filename;
        string fileFullPathCopy = HttpContext.Current.Server.MapPath(folderpath) + filenameCopy;
        //if file exits we remove it first
        if (!File.Exists(fileFullPath))
        {
            if (File.Exists(fileFullPathCopy))
            {
                //copy the copy file to the file
                File.Copy(fileFullPathCopy, fileFullPath);
            }

        }

        //no matter, still return 
        return filename;
    }


    public static void FileDelete(string folderpath, string filename)
    {
        string fileFullPath = HttpContext.Current.Server.MapPath(folderpath) + filename;
        //if file exits we remove it first
        if (File.Exists(fileFullPath))
        {
            File.Delete(fileFullPath);
        }
    }
    public static void FileImage(System.Web.UI.WebControls.Image image, string folderpath, string filename, string text)
    {
        if (File.Exists(HttpContext.Current.Server.MapPath(folderpath) + filename))
        {
            image.ImageUrl = folderpath + filename;
            image.Visible = true;
        }
        else
        {
            image.Visible = false;
        }

    }

    public static void FileDownLoadHyperLink( HyperLink hyperLink, string folderpath, string filename, string text)
    {
        if (File.Exists(HttpContext.Current.Server.MapPath(folderpath) + filename))
        {
            hyperLink.NavigateUrl = folderpath + filename;
            hyperLink.Text = text;
            hyperLink.Visible = true;
        }
        else
        {

            hyperLink.Visible = false;
        }

    }

    //upload a document file from a file upload control
    public static string UploadFileObject(FileUpload fu, string folderPath, string rename, string oldFilePath)
    {
        string filename = null;
        string fileFullPath = "";
        //check is file exits
        if (fu.HasFile)
        {
            //didnt put try and catch so easier to see the error.  
            if (Directory.Exists(folderPath))
            {
                rename += Path.GetExtension(fu.PostedFile.FileName);
                fileFullPath = folderPath + rename;

                if (Utility.NullStringReturn(oldFilePath).Length > 0)
                {
                    //delete the old file 
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                //if file exits we remove it first
                if (File.Exists(fileFullPath))
                {
                    File.Delete(fileFullPath);
                }
                fu.SaveAs(fileFullPath);
                filename = rename;
            }
        }
        //return file name
        return filename;

    }


    //upload a document file from a file upload control
    public static string UploadFileDocument(FileUpload fu, string folderPath, string rename, string oldFilePath)
    {
        string filename = null;
        string fileFullPath = "";
        //check is file exits
        if (fu.HasFile)
        {
            //didnt put try and catch so easier to see the error.  
            if (Directory.Exists(folderPath))
            {
                rename += Path.GetExtension(fu.PostedFile.FileName);
                fileFullPath = folderPath + rename;

                if (Utility.NullStringReturn(oldFilePath).Length > 0)
                {
                    //delete the old file 
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                //if file exits we remove it first
                if (File.Exists(fileFullPath))
                {
                    File.Delete(fileFullPath);
                }
                fu.SaveAs(fileFullPath);
                filename = rename;
            }
        }
        //return file name
        return filename;

    }


    //upload a image file from a file upload control
    public static string UploadFileImage(FileUpload fu, string folderPath, string rename, int? width, int? height)
    {
        string filename = "";
        string fileFullPath = "";
        //check is file exits
        if (fu.HasFile)
        {
            //didnt put try and catch so easier to see the error.  
            if (Directory.Exists(folderPath))
            {
                rename += Path.GetExtension(fu.PostedFile.FileName);
                fileFullPath = folderPath + rename;
                //if file exits we remove it first
                if (File.Exists(fileFullPath))
                {
                    File.Delete(fileFullPath);
                }
                fu.SaveAs(fileFullPath);
                ImageFileResize(fileFullPath, width, height);
                filename = rename;
            }
        }
        //return file name
        return filename;

    }

    //rezire the image
    public static void ImageFileResize(string fileFullPath, int? newWidth, int? newHeight)
    {
        int width = NullInt32Return(newWidth);
        int height = NullInt32Return(newHeight);
        System.Drawing.Image imgPhotoVert = System.Drawing.Image.FromFile(fileFullPath);
        if (imgPhotoVert.Width > width && width > 0)
        {
            System.Drawing.Image imgPhoto = ResizeBitmap(imgPhotoVert, width, (imgPhotoVert.Height * width) / imgPhotoVert.Width);
            imgPhotoVert.Dispose();
            imgPhoto.Save(fileFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgPhoto.Dispose();
            //call again to double check the size
            ImageFileResize(fileFullPath, newWidth, newHeight);

        }
        else if (imgPhotoVert.Height > height && height > 0)
        {
            System.Drawing.Image imgPhoto = ResizeBitmap(imgPhotoVert, (imgPhotoVert.Width * height) / imgPhotoVert.Height, height);
            imgPhotoVert.Dispose();
            imgPhoto.Save(fileFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgPhoto.Dispose();
            ImageFileResize(fileFullPath, newWidth, newHeight);

        }
        else
        {
            imgPhotoVert.Dispose();
        }
    }

    //resize the image
    public static Bitmap ResizeImage(System.Drawing.Image orgImage, int nWidth, int nHeight)
    {
        Bitmap thumb = new Bitmap(nWidth, nHeight);
        using (Graphics g = Graphics.FromImage((System.Drawing.Image)thumb))
        {
            // just in case it's a transparent GIF force the bg to white
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b, 0, 0, thumb.Width, thumb.Height);
            g.DrawImage(orgImage, 0, 0, thumb.Width, thumb.Height);
        }
        return thumb;
    }

    //convert to bitmap
    public static Bitmap ResizeBitmap(System.Drawing.Image imgPhoto, int nWidth, int nHeight)
    {
        Bitmap result = new Bitmap(nWidth, nHeight);
        using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgPhoto, 0, 0, nWidth, nHeight); return result;
        }
    }


    #endregion

    #region"Array"

    //return th array in certain point
    public static string StringArrayReturn(string[] stringArray, int numberStart, string betweenString)
    {
        StringBuilder st = new StringBuilder();

        for (int i = 0; i < stringArray.Length; i++)
        {
            if (i >= numberStart)
            {
                if (st.Length > 0)
                {
                    st.Append(betweenString);
                }
                st.Append(stringArray[i].ToString());
            }
        }
        return st.ToString();
    }

    #endregion




    #region"Mail"

    public static void SendMail(string sFrom, string sTo, string sSubject, string sBody, string sAttach, bool bHtml)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(sFrom);
        string[] aTo = sTo.Split(';');
        for (int i = 0; i < aTo.Length; i++)
            if (aTo[i] != "")
                mail.To.Add(aTo[i]);
        mail.Subject = sSubject;
        mail.Body = sBody;
        if (sAttach != "") mail.Attachments.Add((new Attachment(sAttach)));
        mail.IsBodyHtml = bHtml;
        SmtpClient smtp = new SmtpClient(ConfigurationSettings.AppSettings["Smtp"]);
        smtp.Send(mail);
    }     

    #endregion

    #region FTP

    public static string FtpFileUpload(string ftpUri, string ftpUsername, string ftpPassword, string fileFullPath, string fileName)
    {
        ManualResetEvent waitObject;

        FtpState state = new FtpState();
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri + fileName);
        request.Method = WebRequestMethods.Ftp.UploadFile;

        request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

        state.Request = request;
        state.FileName = fileFullPath;

        waitObject = state.OperationComplete;

        request.BeginGetRequestStream(
            new AsyncCallback(EndGetStreamCallback),
            state
        );

        waitObject.WaitOne();

        // The operations either completed or threw an exception.
        if (state.OperationException != null)
        {
            return state.OperationException.Message;
        }
        else
        {
            return string.Empty;
        }
    }

    private static void EndGetStreamCallback(IAsyncResult ar)
    {
        FtpState state = (FtpState)ar.AsyncState;

        Stream requestStream = null;
        // End the asynchronous call to get the request stream.
        try
        {
            requestStream = state.Request.EndGetRequestStream(ar);
            // Copy the file contents to the request stream.
            const int bufferLength = 2048;
            byte[] buffer = new byte[bufferLength];
            int count = 0;
            int readBytes = 0;
            FileStream stream = File.OpenRead(state.FileName);
            do
            {
                readBytes = stream.Read(buffer, 0, bufferLength);
                requestStream.Write(buffer, 0, readBytes);
                count += readBytes;
            }
            while (readBytes != 0);
            Console.WriteLine("Writing {0} bytes to the stream.", count);
            // IMPORTANT: Close the request stream before sending the request.
            requestStream.Dispose();
            stream.Dispose();
            // Asynchronously get the response to the upload request.
            state.Request.BeginGetResponse(
                new AsyncCallback(EndGetResponseCallback),
                state
            );
        }
        // Return exceptions to the main application thread.
        catch (Exception e)
        {
            Console.WriteLine("Could not get the request stream.");
            state.OperationException = e;
            state.OperationComplete.Set();
            return;
        }

    }

    private static void EndGetResponseCallback(IAsyncResult ar)
    {
        FtpState state = (FtpState)ar.AsyncState;
        FtpWebResponse response = null;
        try
        {
            response = (FtpWebResponse)state.Request.EndGetResponse(ar);
            response.Close();
            state.StatusDescription = response.StatusDescription;
            // Signal the main application thread that 
            // the operation is complete.
            state.OperationComplete.Set();
        }
        // Return exceptions to the main application thread.
        catch (Exception e)
        {
            Console.WriteLine("Error getting response.");
            state.OperationException = e;
            state.OperationComplete.Set();
        }
    }

    #endregion
}
