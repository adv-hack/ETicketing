Imports TalentSystemDefaults.DataEntities
Imports TalentSystemDefaults
Imports System.Data
Imports System.Data.SqlClient
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO

Partial Class DeSerialization
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

	End Sub

	Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		DeSerializeObject()
	End Sub

	Sub DeSerializeObject()
		Dim objTalentAccessDetail As TalentDataAccess = Nothing
		Dim conn As New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString)
		Dim comm As New SqlCommand("select top 1 * from tbl_config_detail_audit order by time_stamp desc")
		Dim da As New SqlDataAdapter
		Dim ds As New DataSet
		Dim byteArr() As Byte
		Dim bf As New BinaryFormatter
		Dim ms As MemoryStream = Nothing
		Dim overridePaymentType As String = TextBox1.Text

		' go.. get the data
		comm.Connection = conn
		da.SelectCommand = comm
		conn.Open()
		da.Fill(ds)
		conn.Close()

		' deserialize the object 
		byteArr = ds.Tables(0).Rows(0)("command")
		ms = New MemoryStream(byteArr)
		objTalentAccessDetail = DirectCast(bf.Deserialize(ms), TalentDataAccess)

		' manipulate the serialized object 
		For Each commandParam In objTalentAccessDetail.CommandElements.CommandParameter
			If commandParam.ParamName = "@PAYMENT_TYPE_CODE" Then
				commandParam.ParamValue = TextBox1.Text
				Exit For
			End If
		Next

		' execute the transaction for another payment type 
		If DropDownList1.SelectedValue = "1" Then
			objTalentAccessDetail.SQLAccess(DestinationDatabase.SQL2005)
		Else
			objTalentAccessDetail.SQLAccess(DestinationDatabase.TALENT_CONFIG)
		End If

		lblMsg.Text = "Transaction completed successfully !!"

	End Sub
	
End Class
