Imports Talent.eCommerce

Partial Class UserControls_ZoomifyControl
    Inherits ControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' BF - must load everytime otherwise 'Add to basket' on product options without entering a qte 
        ' makes the image dissapear
        '   If Not Page.IsPostBack Then
        If ModuleDefaults.ZoomifyInUse Then loadZoomifyImage()
        'End If
    End Sub

    Protected Sub loadZoomifyImage()
        'Dim zoomPath As String = ""
        'Dim mainImagePath As String = ""

        '' Call function to see whether zoom image exists
        'zoomPath = Utilities.GetImagePathAbsolute
        'If Not zoomPath.EndsWith("/") Then zoomPath += "/"
        'zoomPath += "Product/Zoomify"

        '' Call function to see whether main image exists
        'mainImagePath = ImagePath.getImagePath("PRODMAIN", Request("product"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

        'Dim flashType As String = "application/x-shockwave-flash"
        'Dim flashPluginsPage As String = "http://www.macromedia.com/go/getflashplayer"
        'Dim flashClassID As String = "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
        'Dim flashCodeBase As String = "http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0"
        'Dim zoomifyViewerFilePath As String = def.ZoomifyFlashFileURL
        'Dim zoomifyImagePath As String = Utilities.GetImagePathVirtual
        'If Not zoomifyImagePath.EndsWith("/") Then zoomifyImagePath += "/"
        'zoomifyImagePath += "Product/Zoomify"
        'zoomifyImagePath = zoomifyImagePath.Replace("~", "") & "/" & Request("product")

        'Dim imageWidth As String = def.ZoomifyWidth
        'Dim imageHeight As String = def.ZoomifyHeight
        'Dim bgColour As String = "white"

        'Dim script As New HtmlGenericControl("script")
        'Dim noscript As New HtmlGenericControl("noscript")

        '' If new image retrieval and zoom components exist
        ''If newImageRetrieval And Directory.Exists(MapPath(zoomPath)) Then
        'If System.IO.Directory.Exists(zoomPath) Then

        '    ' Output the javascript to determine whether Flash is installed
        '    script.Attributes.Add("language", "Javascript")
        '    script.Attributes.Add("type", "text/javascript")

        '    script.InnerHtml = "<!-- " & _
        '                        Environment.NewLine & _
        '                        "// Version check based upon the values entered above in ""Globals""" & _
        '                        Environment.NewLine & _
        '                        "var hasReqestedVersion = DetectFlashVer(requiredMajorVersion, requiredMinorVersion, requiredRevision);" & _
        '                        Environment.NewLine & _
        '                        "// Check to see if the version meets the requirements for playback" & _
        '                        Environment.NewLine & _
        '                        "if (hasReqestedVersion) {  // if we've detected an acceptable version" & _
        '                        Environment.NewLine & _
        '                        " var oeTags = '" & _
        '                        "<object classid=""" & flashClassID & """ width=""" & imageWidth & """ " & _
        '                            " height=""" & imageHeight & """ codebase=""" & flashCodeBase & """ ID=""theMovie"">" & _
        '                            " <param name=""movie"" value=""" & zoomifyViewerFilePath & """ \/>" & _
        '                            " <PARAM NAME=""FlashVars"" VALUE=""zoomifyImagePath=" & zoomifyImagePath & "&zoomifyX=0.0&zoomifyY=0.0&zoomifyZoom=-1&zoomifyToolbar=1&zoomifyNavWindow=1"" \/> " & _
        '                            " <param name=""quality"" value=""high"" \/>" & _
        '                            " <param name=""bgcolor"" value=""" & bgColour & """ \/>" & _
        '                            " <embed src=""" & zoomifyViewerFilePath & """ " & _
        '                                " FlashVars=""zoomifyImagePath=" & zoomifyImagePath & "&zoomifyX=0.0&zoomifyY=0.0&zoomifyZoom=-1&zoomifyToolbar=1&zoomifyNavWindow=1"" " & _
        '                                " quality=""high"" " & _
        '                                " bgcolor=""" & bgColour & """ " & _
        '                                " width=""" & imageWidth & """ " & _
        '                                " height=""" & imageHeight & """ " & _
        '                                " name=""detectiontest"" " & _
        '                                " align=""middle"" " & _
        '                                " play=""true"" " & _
        '                                " loop=""false"" " & _
        '                                " allowScriptAccess=""sameDomain"" " & _
        '                                " type=""" & flashType & """ " & _
        '                                " pluginspage=""" & flashPluginsPage & """ " & _
        '                                " >" & _
        '                            "<\/embed>" & _
        '                        "<\/object>';" & _
        '                        Environment.NewLine & _
        '                        " document.write(oeTags);   // embed the Flash Content SWF when all tests are passed" & _
        '                        Environment.NewLine & _
        '                        "}"

        '    ' Add javascript to display static image, only if main image exists
        '    If System.IO.File.Exists(mainImagePath) Then

        '        script.InnerHtml += " else { //flash is too old or we can't detect the plugin" & _
        '                            Environment.NewLine & _
        '                            " var alternateContent = '" & _
        '                            " <img src=""" & mainImagePath & """ " & _
        '                            "   height=""" & imageHeight & """ " & _
        '                            "   width=""" & imageWidth & """ " & _
        '                            " \/>';" & _
        '                            Environment.NewLine & _
        '                            "  document.write(alternateContent);  // insert non-flash content " & _
        '                            Environment.NewLine & _
        '                            "} " & _
        '                            Environment.NewLine & _
        '                            Environment.NewLine & _
        '                            "// -->"

        '        noscript.InnerHtml = "	<!-- Provide alternate content for browsers that do not support scripting" & _
        '                             Environment.NewLine & _
        '                             "	// or for those that have scripting disabled. -->" & _
        '                             Environment.NewLine & _
        '                             "  	<img src=""" & mainImagePath & """ width=""" & imageWidth & """ height=""" & imageHeight & """  \/> " & _
        '                             Environment.NewLine & _
        '                             " -->"
        '    End If
        'Else

        '    ' Image retrieval from file - only do if image found
        '    If System.IO.File.Exists(MapPath(mainImagePath)) Then
        '        ' Construct html image

        '        script.InnerHtml += "<img src="" " & mainImagePath & """ " & _
        '                            "   width=""" & imageWidth & """ " & _
        '                            "   height=""" & imageHeight & """ " & _
        '                            " \/>"

        '    End If

        'End If

        'Me.Controls.Add(script)
        'Me.Controls.Add(noscript)

        Dim div As New HtmlGenericControl("div")
        div.InnerHtml = productImage()
        Me.Controls.Add(div)

    End Sub

    Function productImage() As String
        Const doubleQuote As String = """"
        Const singleQuote As String = "'"
        Dim mainImageWidth As String = ModuleDefaults.ZoomifyWidth
        Dim mainImageHeight As String = ModuleDefaults.ZoomifyHeight

        Dim pathToTry As String = ""
        Dim sb As New StringBuilder
        Dim mainImagePath As String = ""
        Dim returnString As String = ""

        ' Call function to see whether zoom image exists
        pathToTry = Utilities.GetImagePathAbsolute
        If Not pathToTry.EndsWith("\") Then pathToTry += "\"
        pathToTry += "Product\Zoomify\" & Request("product")

        ' Call function to see whether main image exists
        mainImagePath = ImagePath.getImagePath("PRODMAIN", Request.QueryString("product"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

        Dim flashType As String = "application/x-shockwave-flash"
        Dim flashPluginsPage As String = "http://www.macromedia.com/go/getflashplayer"
        Dim flashClassID As String = "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
        Dim flashCodeBase As String = "http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0"
        Dim zoomifyViewerFilePath As String = ModuleDefaults.ZoomifyFlashFileURL

        Dim zoomifyImagePath As String = Utilities.GetImagePathVirtual
        If Not zoomifyImagePath.EndsWith("/") Then zoomifyImagePath += "/"
        zoomifyImagePath += "Product/Zoomify"
        zoomifyImagePath = zoomifyImagePath.Replace("~", "") & "/" & Request("product")

        Dim imageWidth As String = ModuleDefaults.ZoomifyWidth
        Dim imageHeight As String = ModuleDefaults.ZoomifyHeight
        Dim bgColour As String = "white"

        ' If new image retrieval and zoom components exist
        If System.IO.Directory.Exists(pathToTry) Then

            ' Output the javascript to determine whether Flash is installed
            With sb
                .Append("<script language=")
                .Append(doubleQuote)
                .Append("JavaScript")
                .Append(doubleQuote)
                .Append(" type=")
                .Append(doubleQuote)
                .Append("text/javascript")
                .Append(doubleQuote)
                .Append(">")
                .Append(ControlChars.CrLf)
                .Append("<!-- ")
                .Append(ControlChars.CrLf)
                .Append("// Version check based upon the values entered above in ")
                .Append(doubleQuote)
                .Append("Globals")
                .Append(doubleQuote)
                .Append(ControlChars.CrLf)
                .Append("var hasReqestedVersion = DetectFlashVer(requiredMajorVersion, requiredMinorVersion, requiredRevision);")
                .Append(ControlChars.CrLf)
                .Append("// Check to see if the version meets the requirements for playback")
                .Append(ControlChars.CrLf)
                .Append("if (hasReqestedVersion) {  // if we")
                .Append(singleQuote)
                .Append("ve detected an acceptable version")
                .Append(ControlChars.CrLf)
                .Append("    var oeTags = ")
                .Append(singleQuote)
                .Append("<object classid=")
                .Append(doubleQuote)
                .Append(flashClassID)
                .Append(doubleQuote)
                .Append(" width=")
                .Append(doubleQuote)
                .Append(mainImageWidth)
                .Append(doubleQuote)
                .Append(" height=")
                .Append(doubleQuote)
                .Append(mainImageHeight)
                .Append(doubleQuote)
                .Append(" codebase=")
                .Append(doubleQuote)
                .Append(flashCodeBase)
                .Append(doubleQuote)
                .Append("> <param name=")
                .Append(doubleQuote)
                .Append("movie")
                .Append(doubleQuote)
                .Append(" value=")
                .Append(doubleQuote)
                .Append(zoomifyViewerFilePath)
                .Append(doubleQuote & " />")
                .Append("<param name=" & doubleQuote & "FlashVars" & doubleQuote & " VALUE=" & doubleQuote & "zoomifyImagePath=" & zoomifyImagePath & "&zoomifyNavWindow=" & ModuleDefaults.ZoomifyDisplayNavValue & doubleQuote & " />")
                .Append("<param name=" & doubleQuote & "wmode" & doubleQuote & " VALUE=" & doubleQuote & "transparent" & doubleQuote & " />")
                .Append("<param name=")
                .Append(doubleQuote)
                .Append("quality")
                .Append(doubleQuote)
                .Append(" value=")
                .Append(doubleQuote)
                .Append("high")
                .Append(doubleQuote)
                .Append(" /><param name=")
                .Append(doubleQuote)
                .Append("bgcolor")
                .Append(doubleQuote)
                .Append(" value=")
                .Append(doubleQuote)
                .Append(bgColour)
                .Append(doubleQuote)
                .Append(" /> <embed src=")
                .Append(doubleQuote)
                .Append(zoomifyViewerFilePath)
                .Append(doubleQuote)
                .Append(" FlashVars=")
                .Append(doubleQuote)
                .Append("zoomifyImagePath=")
                .Append(zoomifyImagePath)
                .Append("&zoomifyNavWindow=" & ModuleDefaults.ZoomifyDisplayNavValue)
                .Append(doubleQuote)
                .Append(" quality=")
                .Append(doubleQuote)
                .Append("high")
                .Append(doubleQuote)
                .Append(" bgcolor=")
                .Append(doubleQuote)
                .Append(bgColour)
                .Append(doubleQuote)
                .Append(" width=")
                .Append(doubleQuote)
                .Append(mainImageWidth)
                .Append(doubleQuote)
                .Append(" height=")
                .Append(doubleQuote)
                .Append(mainImageHeight)
                .Append(doubleQuote)
                .Append(" name=")
                .Append(doubleQuote)
                .Append("detectiontest")
                .Append(doubleQuote)
                .Append(" aligh=")
                .Append(doubleQuote)
                .Append("middle")
                .Append(doubleQuote)
                .Append(" play=")
                .Append(doubleQuote)
                .Append("true")
                .Append(doubleQuote)
                .Append(" loop=")
                .Append(doubleQuote)
                .Append("false")
                .Append(doubleQuote)
                .Append(" quality=")
                .Append(doubleQuote)
                .Append("high")
                .Append(doubleQuote)
                .Append(" wmode=")
                .Append(doubleQuote)
                .Append("transparent")
                .Append(doubleQuote)
                .Append(" allowScriptAccess=")
                .Append(doubleQuote)
                .Append("sameDomain")
                .Append(doubleQuote)
                .Append(" type=")
                .Append(doubleQuote)
                .Append(flashType)
                .Append(doubleQuote)
                .Append(" pluginspage=")
                .Append(doubleQuote)
                .Append(flashPluginsPage)
                .Append(doubleQuote)
                .Append("> <\/embed> <\/object>")
                .Append(singleQuote)
                .Append(";")
                .Append(ControlChars.CrLf)
                .Append("    document.write(oeTags);   // embed the Flash Content SWF when all tests are passed")
                .Append(ControlChars.CrLf)
                .Append("  } ")

            End With

            ' Add javascript to display static image, only if main image exists
            'Try
            '    pathToTry = MapPath(mainImagePath)
            'Catch ex As Exception
            '    pathToTry = mainImagePath
            'End Try
            'If System.IO.File.Exists(pathToTry) Then
            '--------------------------------------
            ' Main image is already checked above..
            '--------------------------------------
            If mainImagePath.EndsWith(".jpg") Or mainImagePath.EndsWith(".gif") Or mainImagePath.EndsWith(".png") Then

                With sb

                    .Append("else {  // flash is too old or we can")
                    .Append(singleQuote)
                    .Append("t detect the plugin")
                    .Append(ControlChars.CrLf)
                    .Append("    var alternateContent = ")
                    .Append(singleQuote)
                    .Append("<img src=")
                    .Append(doubleQuote)
                    .Append(mainImagePath)
                    .Append(doubleQuote)
                    .Append(" width=")
                    .Append(doubleQuote)
                    .Append(mainImageWidth)
                    .Append(doubleQuote)
                    .Append(" height=")
                    .Append(doubleQuote)
                    .Append(mainImageHeight)
                    .Append(doubleQuote)
                    .Append("/>")
                    .Append(singleQuote)
                    .Append(";")
                    .Append(ControlChars.CrLf)
                    .Append("    document.write(alternateContent);  // insert non-flash content   ")
                    .Append(ControlChars.CrLf)
                    .Append("  }")

                End With

            End If

            With sb
                .Append(ControlChars.CrLf)
                .Append("// -->")
                .Append(ControlChars.CrLf)
                .Append("</script>")
                .Append(ControlChars.CrLf)
                .Append("<noscript>")
                .Append(ControlChars.CrLf)
                .Append("	<!-- Provide alternate content for browsers that do not support scripting")
                .Append(ControlChars.CrLf)
                .Append("	// or for those that have scripting disabled. -->")
                .Append(ControlChars.CrLf)
                .Append("  	<img src=")
                .Append(doubleQuote)
                .Append(mainImagePath)
                .Append(doubleQuote)
                .Append(" width=")
                .Append(doubleQuote)
                .Append(mainImageWidth)
                .Append(doubleQuote)
                .Append(" height=")
                .Append(doubleQuote)
                .Append(mainImageHeight)
                .Append(doubleQuote)
                .Append(" /> 	")
                .Append(ControlChars.CrLf)
                .Append("</noscript>")
                .Append(ControlChars.CrLf)
            End With

        Else
            Try
                pathToTry = MapPath(mainImagePath)
            Catch ex As Exception
                pathToTry = mainImagePath
            End Try
            'pathToTry = "C:\DEV\TalentEBusiness\Images\prod\" & Request("product") & ".png"
            'mainImagePath = "../../Images/prod/" & Request("product") & ".png"
            ' Image retrieval from file - only do if image found
            If System.IO.File.Exists(pathToTry) Then
                ' Construct html image
                With sb
                    sb.Append("<img src=")
                    .Append(doubleQuote)
                    .Append(mainImagePath)
                    .Append(doubleQuote)
                    .Append(" alt=")
                    .Append(doubleQuote)
                    .Append("")
                    .Append(doubleQuote)
                    .Append(" /> 	")
                End With
            End If

        End If

        returnString = sb.ToString

        Return returnString

    End Function

End Class
