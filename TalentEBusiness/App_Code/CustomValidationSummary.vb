Imports Microsoft.VisualBasic
Imports System.IO
Imports System.web
Imports System.Xml
Imports System.data
Imports System.Net
Imports Talent.Common

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    CustomValidatioSummary - Gives the ability to add any error
'                                   message to a validation summary
'
'       Date                        160307
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------
Namespace Talent.eCommerce

    Public Class CustomValidationSummary
        Inherits System.Web.UI.WebControls.ValidationSummary

        ' Allows the caller to place custom text messages inside the validation
        ' summary control

        ' param name="msg" -The message you want to appear in the summary
        ' param name= "pageRef"- A reference to the current page
        Public Sub AddErrorMessage(ByVal msg As String, ByVal pageRef As System.Web.UI.Page)
            pageRef.Validators.Add(New DummyValidator(msg))
        End Sub
    End Class

    ' The validation summary control works by iterating over the Page.Validators
    ' collection and displaying the ErrorMessage property of each validator
    ' that return false for the IsValid() property.  This class will act 
    ' like all the other validators except it always is invalid and thus the 
    ' ErrorMessage property will always be displayed.
    Friend Class DummyValidator
        Implements IValidator

        Private errorMsg As String

        Public Sub New(ByVal msg As String)
            errorMsg = msg
        End Sub
        Public Sub DummyValidator(ByVal msg As String)
            errorMsg = msg
        End Sub

        Public Property ErrorMessage() As String Implements IValidator.ErrorMessage
            Get
                Return errorMsg
            End Get
            Set(ByVal Value As String)
                errorMsg = Value
            End Set
        End Property

        Overridable Property IsValid() As Boolean Implements IValidator.IsValid
            Get
                Return False
            End Get
            Set(ByVal value As Boolean)
            End Set
        End Property

        Public Sub Validate() Implements IValidator.Validate
        End Sub
    End Class
End Namespace

