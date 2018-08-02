Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBComments
    Inherits DBAccess

    Private _deComments As DEComments

    Public Property deComments() As DEComments
        Get
            Return _deComments
        End Get
        Set(ByVal value As DEComments)
            _deComments = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        err = AccessDatabaseWS068R()

        Return err

    End Function

    Public Function AccessDatabaseWS068R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim errorCount As Integer = 0
      
        Try

            PARAMOUT = CallWS068R()

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPP-03"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS068R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS068R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS068Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS068Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(_deComments.SessionID, 36) & _
                    Utilities.PadLeadingZeros(_deComments.CorporatePackageNumericID, 13) & _
                    Utilities.FixStringLength(_deComments.ProductCode, 6) & _
                    Utilities.FixStringLength(_deComments.Seat, 15) & _
                    Utilities.FixStringLength(_deComments.CommentText, 200) & _
                    Utilities.FixStringLength(_deComments.TempBasketID, 100) & _
                    Utilities.FixStringLength("", 652) & _
                    Utilities.FixStringLength("", 3)
        Return myString

    End Function

    
    
End Class
