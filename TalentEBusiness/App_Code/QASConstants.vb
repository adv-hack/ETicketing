'  QuickAddress Pro Web
'  (c) 2004 QAS Ltd. www.qas.com

Imports System


Namespace com.qas.prowebintegration


    ' Summary description for Constants.
    Public Class Constants


        ' Common configuration keys (reading from config.web)
        Public Const KEY_SERVER_URL As String = "com.qas.proweb.ServerURL"
        Public Const KEY_USER_NAME As String = "com.qas.proweb.username"
        Public Const KEY_PASSWORD As String = "com.qas.proweb.password"
        Public Const KEY_LAYOUT As String = "com.qas.proweb.Layout"
		' Configuration section used by Verification scenario for USA addresses
		Public Const KEY_USA_VERIFY_SECTION As String = "com.qas.proweb.USAVerify"

		' Common final page 
        Public Const PAGE_FINAL_ADDRESS As String = "Address.aspx"

        ' Shared page for Verification scenario
        Public Const PAGE_VERIFY_SEARCH As String = "VerifySearch.aspx"

        ' Common field IDs
        Public Const FIELD_DATA_ID As String = "DataID"
        Public Const FIELD_COUNTRY_NAME As String = "CountryName"
        Public Const FIELD_INPUT_LINES As String = "InputLine"
        Public Const FIELD_ADDRESS_LINES As String = "AddressLine"
        Public Const FIELD_ERROR_INFO As String = "ErrorInfo"
        Public Const FIELD_VERIFY_INFO As String = "VerifyInfo"
		Public Const FIELD_MONIKER As String = "Moniker"
		Public Const FIELD_ROUTE As String = "Route"

		' FIELD_ROUTE values: current search state, how we arrived at the final page
		Public Enum Routes
			Undefined				' State not defined
			Okay					' An address was successfully returned
			Failed					' An exception was thrown during the capture process
			PreSearchFailed			' CanSearch returned false
			UnsupportedCountry		' Country is not in page's list of installed countries
			TooManyMatches			' Picklist returned with TooManyMatches flag
			NoMatches				' Picklist returned empty
			Timeout					' Picklist returned with Timeout flag
		End Enum


	End Class


End Namespace
