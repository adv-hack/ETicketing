Imports Microsoft.VisualBasic
Imports System.IO
Imports System.web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Image Path
'
'       Date                        07/03/07
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      ACPLN- 
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       07/03/07    /000    Ben     Imported from V8_4
'--------------------------------------------------------------------------------------------------
Namespace Talent.eCommerce

    Public Class ImagePath

        Const groupPromo As String = "GROUPPROMO"
        Const groupThumb As String = "GROUPTHUMB"
        Const productThumb As String = "PRODTHUMB"
        Const productList As String = "PRODLIST"
        Const productZoom As String = "PRODZOOM"
        Const productMain As String = "PRODMAIN"
        Const productAdditional As String = "PRODADD"
        Const productSwatch As String = "PRODSWATCH"
        Const productIcon As String = "PRODICON"
        Const productSuggest As String = "PRODSUGGEST"
        Const productAssoc As String = "PRODASSOC"
        Const productTicketingOpposition As String = "PRODTICKETING"
        Const productCompetition As String = "PRODCOMPETITION"
        Const productSponsor As String = "PRODSPONSOR"
        Const productCorporate As String = "PRODCORPORATE"
        Const productPackage As String = "PRODPACKAGE"
        Const productSubType As String = "PRODSUBTYPE"
        Const promoImage As String = "PROMOMOTION"
        Const altImage1 As String = "ALTIMAGE1"
        Const altImage2 As String = "ALTIMAGE2"
        Const altImage3 As String = "ALTIMAGE3"
        Const altImage4 As String = "ALTIMAGE4"
        Const altImage5 As String = "ALTIMAGE5"
        Const altImage6 As String = "ALTIMAGE6"
        Const altImage7 As String = "ALTIMAGE7"
        Const altImage8 As String = "ALTIMAGE8"
        Const altImage9 As String = "ALTIMAGE9"
        Const altImage10 As String = "ALTIMAGE10"
        Const appTheme As String = "APPTHEME"
        Const magicZoomDefault As String = "MAGICZOOMDEFAULT"
        Const magicZoom1S As String = "MAGICZOOM1S"
        Const magicZoom1M As String = "MAGICZOOM1M"
        Const magicZoom1L As String = "MAGICZOOM1L"
        Const magicZoom2S As String = "MAGICZOOM2S"
        Const magicZoom2M As String = "MAGICZOOM2M"
        Const magicZoom2L As String = "MAGICZOOM2L"
        Const magicZoom3S As String = "MAGICZOOM3S"
        Const magicZoom3M As String = "MAGICZOOM3M"
        Const magicZoom3L As String = "MAGICZOOM3L"
        Const magicZoom4S As String = "MAGICZOOM4S"
        Const magicZoom4M As String = "MAGICZOOM4M"
        Const magicZoom4L As String = "MAGICZOOM4L"
        Const magicZoom5S As String = "MAGICZOOM5S"
        Const magicZoom5M As String = "MAGICZOOM5M"
        Const magicZoom5L As String = "MAGICZOOM5L"
        Const magicZoom6S As String = "MAGICZOOM6S"
        Const magicZoom6M As String = "MAGICZOOM6M"
        Const magicZoom6L As String = "MAGICZOOM6L"
        Const magicZoom7S As String = "MAGICZOOM7S"
        Const magicZoom7M As String = "MAGICZOOM7M"
        Const magicZoom7L As String = "MAGICZOOM7L"
        Const magicZoom8S As String = "MAGICZOOM8S"
        Const magicZoom8M As String = "MAGICZOOM8M"
        Const magicZoom8L As String = "MAGICZOOM8L"
        Const magicZoom9S As String = "MAGICZOOM9S"
        Const magicZoom9M As String = "MAGICZOOM9M"
        Const magicZoom9L As String = "MAGICZOOM9L"
        Const magicZoom10S As String = "MAGICZOOM10S"
        Const magicZoom10M As String = "MAGICZOOM10M"
        Const magicZoom10L As String = "MAGICZOOM10L"
        Const magicZoom11S As String = "MAGICZOOM11S"
        Const magicZoom11M As String = "MAGICZOOM11M"
        Const magicZoom11L As String = "MAGICZOOM11L"
        Const magicZoom12S As String = "MAGICZOOM12S"
        Const magicZoom12M As String = "MAGICZOOM12M"
        Const magicZoom12L As String = "MAGICZOOM12L"
        Const alerts As String = "ALERTS"
        Const STADIUMCODE As String = "STADIUMCODE"
        Const productGroup As String = "PRODGROUP"

        Public Shared Function getImagePath(ByVal type As String, ByVal item As String, ByVal businessUnit As String, ByVal Partner As String) As String
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay
            If item Is Nothing Then item = String.Empty
            '---------------------------------
            ' GetImagePath (e.g. for products)
            '---------------------------------
            Dim ucr As New Talent.Common.UserControlResource
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "ImagePath.vb"
                .PageCode = "*ALL"
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            End With

            '---------------------------------
            'Get Default Values
            '---------------------------------
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()

            Dim returnString As String = String.Empty

            '-----------------------
            ' Check if it's in cache
            '-----------------------
            Dim cacheKey As String = "IMAGEPATH" &
                                    Common.Utilities.FixStringLength(type, 20) &
                                    Common.Utilities.FixStringLength(item, 20) &
                                    Common.Utilities.FixStringLength(businessUnit, 50) &
                                    Common.Utilities.FixStringLength(Partner, 50)


            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                returnString = CType(HttpContext.Current.Cache.Item(cacheKey), String)
            Else

                Select Case type
                    Case productThumb
                        returnString = checkImageExists("/Product/Thumb/", item, businessUnit, Partner)
                        If returnString = "/Product/Thumb/" Then returnString = getMagicZoomOrMainImage(item, businessUnit, Partner)
                    Case productList
                        returnString = checkImageExists("/Product/List/", item, businessUnit, Partner)
                        If returnString = "/Product/List/" Then returnString = getMagicZoomOrMainImage(item, businessUnit, Partner)
                    Case productAssoc
                        returnString = checkImageExists("/Product/Associated/", item, businessUnit, Partner)
                        If returnString = "/Product/Associated/" Then returnString = getMagicZoomOrMainImage(item, businessUnit, Partner)
                    Case productMain
                        returnString = checkImageExists(def.ProductMainImagePathRelative, item, businessUnit, Partner)
                        If returnString = def.ProductMainImagePathRelative Then returnString = String.Empty
                    Case productAdditional
                        returnString = checkImageExists("/Product/Additional/", item, businessUnit, Partner)
                    Case productSwatch
                        returnString = checkImageExists("/Product/Swatch/", item, businessUnit, Partner)
                    Case productIcon
                        returnString = checkImageExists("/Product/Icon/", item, businessUnit, Partner)
                    Case productSuggest
                        returnString = checkImageExists("/Product/Suggest/", item, businessUnit, Partner)
                    Case productZoom
                        returnString = checkImageExists("/Product/Zoomify/", item, businessUnit, Partner)
                    Case productTicketingOpposition
                        returnString = checkImageExists("/Product/Ticketing/Opposition/", item, businessUnit, Partner)
                        If returnString = "/Product/Ticketing/Opposition/" Then returnString = String.Empty
                    Case productCompetition
                        returnString = checkImageExists("/Product/Ticketing/Competition/", item, businessUnit, Partner)
                        If returnString = "/Product/Ticketing/Competition/" Then returnString = String.Empty
                    Case productSponsor
                        returnString = checkImageExists("/Product/Ticketing/Sponsor/", item, businessUnit, Partner)
                        If returnString = "/Product/Ticketing/Sponsor/" Then returnString = String.Empty
                    Case promoImage
                        returnString = checkImageExists("/Promo/", item, businessUnit, Partner)
                    Case altImage1
                        returnString = checkImageExists("/Product/Alt/1/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/1/" Then returnString = String.Empty
                    Case altImage2
                        returnString = checkImageExists("/Product/Alt/2/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/2/" Then returnString = String.Empty
                    Case altImage3
                        returnString = checkImageExists("/Product/Alt/3/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/3/" Then returnString = String.Empty
                    Case altImage4
                        returnString = checkImageExists("/Product/Alt/4/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/4/" Then returnString = String.Empty
                    Case altImage5
                        returnString = checkImageExists("/Product/Alt/5/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/5/" Then returnString = String.Empty
                    Case altImage6
                        returnString = checkImageExists("/Product/Alt/6/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/6/" Then returnString = String.Empty
                    Case altImage7
                        returnString = checkImageExists("/Product/Alt/7/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/7/" Then returnString = String.Empty
                    Case altImage8
                        returnString = checkImageExists("/Product/Alt/8/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/8/" Then returnString = String.Empty
                    Case altImage9
                        returnString = checkImageExists("/Product/Alt/9/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/9/" Then returnString = String.Empty
                    Case altImage10
                        returnString = checkImageExists("/Product/Alt/10/", item, businessUnit, Partner)
                        If returnString = "/Product/Alt/10/" Then returnString = String.Empty
                    Case appTheme
                        returnString = checkImageExists("/AppTheme/", item, businessUnit, Partner)
                        If returnString = "/AppTheme/" Then returnString = String.Empty
                    Case magicZoomDefault
                        returnString = checkImageExists("/Product/MagicZoom/Default/", item, businessUnit, Partner)
                        If returnString = "/Product/MagicZoom/Default/" Then returnString = String.Empty
                    Case magicZoom1S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/1/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/1/S/") Then returnString = String.Empty
                    Case magicZoom1M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/1/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/1/M/") Then returnString = String.Empty
                    Case magicZoom1L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/1/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/1/L/") Then returnString = String.Empty
                    Case magicZoom2S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/2/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/2/S/") Then returnString = String.Empty
                    Case magicZoom2M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/2/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/2/M/") Then returnString = String.Empty
                    Case magicZoom2L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/2/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/2/L/") Then returnString = String.Empty
                    Case magicZoom3S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/3/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/3/S/") Then returnString = String.Empty
                    Case magicZoom3M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/3/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/3/M/") Then returnString = String.Empty
                    Case magicZoom3L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/3/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/3/L/") Then returnString = String.Empty
                    Case magicZoom4S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/4/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/4/S/") Then returnString = String.Empty
                    Case magicZoom4M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/4/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/4/M/") Then returnString = String.Empty
                    Case magicZoom4L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/4/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/4/L/") Then returnString = String.Empty
                    Case magicZoom5S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/5/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/5/S/") Then returnString = String.Empty
                    Case magicZoom5M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/5/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/5/M/") Then returnString = String.Empty
                    Case magicZoom5L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/5/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/5/L/") Then returnString = String.Empty
                    Case magicZoom6S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/6/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/6/S/") Then returnString = String.Empty
                    Case magicZoom6M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/6/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/6/M/") Then returnString = String.Empty
                    Case magicZoom6L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/6/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/6/L/") Then returnString = String.Empty
                    Case magicZoom7S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/7/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/7/S/") Then returnString = String.Empty
                    Case magicZoom7M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/7/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/7/M/") Then returnString = String.Empty
                    Case magicZoom7L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/7/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/7/L/") Then returnString = String.Empty
                    Case magicZoom8S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/8/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/8/S/") Then returnString = String.Empty
                    Case magicZoom8M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/8/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/8/M/") Then returnString = String.Empty
                    Case magicZoom8L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/8/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/8/L/") Then returnString = String.Empty
                    Case magicZoom9S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/9/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/9/S/") Then returnString = String.Empty
                    Case magicZoom9M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/9/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/9/M/") Then returnString = String.Empty
                    Case magicZoom9L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/9/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/9/L/") Then returnString = String.Empty
                    Case magicZoom10S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/10/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/10/S/") Then returnString = String.Empty
                    Case magicZoom10M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/10/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/10/M/") Then returnString = String.Empty
                    Case magicZoom10L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/10/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/10/L/") Then returnString = String.Empty
                    Case magicZoom11S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/11/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/11/S/") Then returnString = String.Empty
                    Case magicZoom11M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/11/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/11/M/") Then returnString = String.Empty
                    Case magicZoom11L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/11/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/11/L/") Then returnString = String.Empty
                    Case magicZoom12S
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/12/S/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/12/S/") Then returnString = String.Empty
                    Case magicZoom12M
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/12/M/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/12/M/") Then returnString = String.Empty
                    Case magicZoom12L
                        returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + "/12/L/", item, businessUnit, Partner)
                        If returnString = (def.ProductMagicZoomImagePathRelative + "/12/L/") Then returnString = String.Empty
                    Case productCorporate
                        returnString = checkImageExists("/Product/Corporate/", item, businessUnit, Partner)
                        If returnString = "/Product/Corporate/" Then returnString = String.Empty
                    Case productPackage
                        returnString = checkImageExists("/Product/Package/", item, businessUnit, Partner)
                        If returnString = "/Product/Package/" Then returnString = String.Empty
                    Case productSubType
                        returnString = checkImageExists("/Product/SubType/", item, businessUnit, Partner)
                        If returnString = "/Product/SubType/" Then returnString = String.Empty
                    Case alerts
                        returnString = checkImageExists("/Alerts/", item)
                        If returnString = "/Alerts/" Then returnString = String.Empty
                    Case STADIUMCODE
                        returnString = checkImageExists("/Stadiums/", item, businessUnit, Partner)
                        If returnString = "/Stadiums/" Then returnString = String.Empty
                    Case productGroup
                        returnString = checkImageExists("/Product/Ticketing/ProductGroup/", item, businessUnit, Partner)
                        If returnString = "/Product/Ticketing/ProductGroup/" Then returnString = String.Empty
                End Select

                'If there is no image found, retrieve the default blank image.
                If returnString = String.Empty Then
                    Select Case type
                        Case productThumb
                            returnString = def.RetailMissingImagePath
                        Case productList
                            returnString = def.RetailMissingImagePath
                        Case Else
                            returnString = def.MissingImagePath
                    End Select

                Else
                    'Do we need to add a query string to flush the cache
                    If Not String.IsNullOrEmpty(ucr.Attribute(type & "_querystring")) Then
                        returnString += ucr.Attribute(type & "_querystring")
                    End If
                End If

                Utilities.TalentLogging.LoadTestLog("ImagePath.vb", "getImagePath", timeSpan)
                If String.IsNullOrWhiteSpace(returnString) Then returnString = ""

                '-----------------------
                ' Put in cache
                '-----------------------
                HttpContext.Current.Cache.Insert(cacheKey,
                        returnString,
                        Nothing,
                        System.DateTime.Now.AddMinutes(30),
                        Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
            Return returnString

        End Function

        Public Shared Function getGroupImagePath(ByVal type As String, ByVal item As String, ByVal businessUnit As String, ByVal Partner As String) As String
            '---------------------------
            ' GetGroupImagePath - Cached
            '---------------------------

            Dim returnString As String = ""
            '----------------- 
            ' Build group path
            '-----------------
            Dim group1 As String = String.Empty, group2 As String = String.Empty, group3 As String = String.Empty, group4 As String = String.Empty, group5 As String = String.Empty
            Dim group6 As String = String.Empty, group7 As String = String.Empty, group8 As String = String.Empty, group9 As String = String.Empty, group10 As String = String.Empty
            Dim arrGroup(10) As String
            arrGroup(0) = HttpContext.Current.Request("group1")
            arrGroup(1) = HttpContext.Current.Request("group2")
            arrGroup(2) = HttpContext.Current.Request("group3")
            arrGroup(3) = HttpContext.Current.Request("group4")
            arrGroup(4) = HttpContext.Current.Request("group5")
            arrGroup(5) = HttpContext.Current.Request("group6")
            arrGroup(6) = HttpContext.Current.Request("group7")
            arrGroup(7) = HttpContext.Current.Request("group8")
            arrGroup(8) = HttpContext.Current.Request("group9")
            arrGroup(9) = HttpContext.Current.Request("group10")

            Dim count As Integer = 0
            Dim sbGroups As New StringBuilder
            Dim groupFolderPath As String = String.Empty
            Dim sbGroupFolders As New StringBuilder
            With sbGroups
                Do While count < 10
                    If Not arrGroup(count) Is Nothing AndAlso arrGroup(count) <> "*EMPTY" Then
                        .Append(Common.Utilities.FixStringLength(arrGroup(count), 20))
                        sbGroupFolders.Append("/").Append(arrGroup(count).Trim)
                    End If
                    count += 1
                Loop
            End With
            If sbGroupFolders.ToString <> String.Empty Then
                sbGroupFolders.Append("/")
            End If
            groupFolderPath = sbGroupFolders.ToString
            '-----------------------
            ' Check if it's in cache
            '-----------------------
            Dim cacheKey As String = "GROUPIMAGEPATH" & _
                                     Common.Utilities.FixStringLength(type, 20) & _
                                     Common.Utilities.FixStringLength(item, 20) & _
                                    Common.Utilities.FixStringLength(businessUnit, 50) & _
                                    Common.Utilities.FixStringLength(Partner, 50) & _
                                    sbGroups.ToString

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
                returnString = CType(HttpContext.Current.Cache.Item(cacheKey), String)
            Else
                '-------------------------------------
                ' Check folders with groups, then root
                '-------------------------------------
                Dim groupPromoPath As String = "/group" & groupFolderPath & "Promo/"
                Dim groupThumbPath As String = "/group" & groupFolderPath & "Thumb/"

                Select Case type
                    Case groupPromo
                        returnString = checkImageExists(groupPromoPath, item, businessUnit, Partner)
                        If returnString = groupPromoPath Then
                            returnString = checkImageExists("/group/Promo/", item, businessUnit, Partner)
                            ' Return a blank string if not found. Stops broken image links
                            If returnString = "/group/Promo/" Then
                                returnString = String.Empty
                            End If
                        End If
                    Case groupThumb
                        returnString = checkImageExists(groupThumbPath, item, businessUnit, Partner)
                        If returnString = groupThumbPath Then
                            returnString = checkImageExists("/group/Thumb/", item, businessUnit, Partner)
                            ' Return a blank string if not found. Stops broken image links
                            If returnString = "/group/Thumb/" Then
                                returnString = String.Empty
                            End If
                        End If
                End Select
                '-----------------------
                ' If found, put in cache
                '-----------------------
                ' BF- Cache it regardless otherwise the itemsInCache will hang if it's 
                ' not found
                ' If returnString <> String.Empty Then
                HttpContext.Current.Cache.Insert(cacheKey, _
                        returnString, _
                        Nothing, _
                        System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                        Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                ' End If
            End If
            Return returnString

        End Function

        Private Shared Function checkImageExists(ByVal pathString As String, ByVal item As String, ByVal businessUnit As String, ByVal partner As String) As String
            '----------------------------------------------------------------
            '   CheckImageExists - Check for image by BU/PAR, then BU then root
            '
            Dim returnString As String = String.Empty
            Dim context As HttpContext = HttpContext.Current
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim rootPathString As String = def.ImagePathVirtual
            Dim rootPathAbsolute As String = def.ImagePathAbsolute
            Dim rootPathVirtual As String = def.ImagePathVirtual
            Dim rootSslPathVirtual As String = def.ImageSslPathVirtual
            Dim imageInGlobal As Boolean = False
            Dim globalImagePath As String = def.GlobalImagePath
            Dim globalPathVirtual As String = def.GlobalPathVirtual
            Dim globalSslPathVirtual As String = def.GlobalSslPathVirtual

            Dim pathItem As String = String.Empty

            ' The absolute path always overrides the path string when entered
            If rootPathAbsolute.Length > 0 Then
                rootPathString = rootPathAbsolute
            End If
            rootPathString = (rootPathString.Replace("\", "/")).Replace("//", "/")

            ' replace slashes in image name
            If def.ImageNameReplaceSlash <> String.Empty AndAlso item.Contains("/") Then
                item = item.Replace("/", def.ImageNameReplaceSlash)
            End If

            ' replace double quotes in image name
            If def.ImageNameReplaceDoubleQuote <> String.Empty AndAlso item.Contains("""") Then
                item = item.Replace("""", def.ImageNameReplaceDoubleQuote)
            End If

            ' replace single quotes
            If item.Contains("'") Then
                item = item.Replace("'", "")
            End If

            ' Check if exists for BU/PARTNER
            pathItem = rootPathString & "/" & businessUnit & "/" & partner & pathString.Trim & item
            returnString = CheckImageIsInThePath(pathItem)
            If returnString = String.Empty Then
                '
                pathItem = rootPathString & "/" & businessUnit & "/" & pathString.Trim & item
                returnString = CheckImageIsInThePath(pathItem)
                If returnString = String.Empty Then
                    '
                    pathItem = rootPathString & "/" & pathString.Trim & item
                    returnString = CheckImageIsInThePath(pathItem)
                End If
            End If

            ' Check for the image in the global settings if still not found
            If returnString = String.Empty Then
                '
                rootPathString = (globalImagePath.Replace("\", "/")).Replace("//", "/")
                pathItem = rootPathString & "/" & pathString.Trim & item
                returnString = CheckImageIsInThePath(pathItem)
                imageInGlobal = True
            End If

            ' Override to the virtual paths when defined
            If rootPathAbsolute.Length > 0 And returnString <> String.Empty Then

                ' Was the image found in the global path
                If imageInGlobal = True Then
                    rootSslPathVirtual = globalSslPathVirtual
                    rootPathVirtual = globalPathVirtual
                End If

                ' Override to the virtual paths
                Dim securePort As String = ""
                If Talent.Common.TalentThreadSafe.ItemIsInCache("SecurePort")  Then
                    securePort = HttpContext.Current.Cache.Item("SecurePort").ToString.Trim
                End If
                If (HttpContext.Current.Request.IsSecureConnection Or HttpContext.Current.Request.Url.Port.ToString.Trim = securePort) And rootSslPathVirtual.Trim <> "" Then
                    returnString = returnString.Replace(rootPathString, rootSslPathVirtual)
                Else
                    returnString = returnString.Replace(rootPathString, rootPathVirtual)
                End If

                ' Format the path
                returnString = (returnString.Replace("\", "/")).Replace("//", "/")
                returnString = returnString.Replace("http:/", "http://")
                returnString = returnString.Replace("https:/", "https://")

            End If
            If returnString = String.Empty Then returnString = pathString
            '-----------------------------------------------------------------------------------------------
            Return returnString
        End Function

        Private Shared Function checkImageExists(ByVal pathString As String, ByVal item As String) As String
            '----------------------------------------------------------------
            '   CheckImageExists - Check for image by BU/PAR, then BU then root
            '
            Dim returnString As String = String.Empty
            Dim context As HttpContext = HttpContext.Current
            '------------------------------------
            Dim myDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

            Dim rootPathString As String = ConfigurationManager.AppSettings("imagesRootPath").ToString.Trim
            Dim rootPathAbsolute As String = def.ImagePathAbsolute
            Dim rootPathVirtual As String = def.ImagePathVirtual
            Dim rootSslPathVirtual As String = def.ImageSslPathVirtual
            Dim imageInGlobal As Boolean = False
            Dim globalImagePath As String = def.GlobalImagePath
            Dim globalPathVirtual As String = def.GlobalPathVirtual
            Dim globalSslPathVirtual As String = def.GlobalSslPathVirtual

            Dim pathItem As String = String.Empty

            ' The absolute path always overrides the path string when entered
            If rootPathAbsolute.Length > 0 Then
                rootPathString = rootPathAbsolute
            End If
            rootPathString = (rootPathString.Replace("\", "/")).Replace("//", "/")

            ' replace slashes in image name
            If def.ImageNameReplaceSlash <> String.Empty AndAlso item.Contains("/") Then
                item = item.Replace("/", def.ImageNameReplaceSlash)
            End If

            ' replace double quotes in image name
            If def.ImageNameReplaceDoubleQuote <> String.Empty AndAlso item.Contains("""") Then
                item = item.Replace("""", def.ImageNameReplaceDoubleQuote)
            End If

            ' Check if exists in given path
            pathItem = rootPathString & "/" & pathString.Trim & item
            returnString = CheckImageIsInThePath(pathItem)

            ' Check for the image in the global settings if still not found
            If returnString = String.Empty Then
                '
                rootPathString = (globalImagePath.Replace("\", "/")).Replace("//", "/")
                pathItem = rootPathString & "/" & pathString.Trim & item
                returnString = CheckImageIsInThePath(pathItem)
                imageInGlobal = True
            End If

            ' Override to the virtual paths when defined
            If rootPathAbsolute.Length > 0 And returnString <> String.Empty Then

                ' Was the image found in the global path
                If imageInGlobal = True Then
                    rootSslPathVirtual = globalSslPathVirtual
                    rootPathVirtual = globalPathVirtual
                End If

                ' Override to the virtual paths
                Dim securePort As String = ""
                If Talent.Common.TalentThreadSafe.ItemIsInCache("SecurePort") Then
                    securePort = HttpContext.Current.Cache.Item("SecurePort").ToString.Trim
                End If
                If (HttpContext.Current.Request.IsSecureConnection Or HttpContext.Current.Request.Url.Port.ToString.Trim = securePort) And rootSslPathVirtual.Trim <> "" Then
                    returnString = returnString.Replace(rootPathString, rootSslPathVirtual)
                Else
                    returnString = returnString.Replace(rootPathString, rootPathVirtual)
                End If

                ' Format the path
                returnString = (returnString.Replace("\", "/")).Replace("//", "/")
                returnString = returnString.Replace("http:/", "http://")
                returnString = returnString.Replace("https:/", "https://")

            End If
            If returnString = String.Empty Then returnString = pathString
            '-----------------------------------------------------------------------------------------------
            Return returnString
        End Function

        Private Shared Function CheckImageIsInThePath(ByVal pathString As String) As String

            '---------------------------------------------------------------------------------------
            '   As we are concatinating from various places could end up with the slashes going every 
            '   which way and duplicated, so we need to sort out.
            '
            pathString = (pathString.Replace("\", "/")).Replace("//", "/")
            '---------------------------------------------------------------------------------------
            '    Check if a Gif or Jpg exists for either the virtual (~) or absolute path specified
            '
            Try
                If pathString.Substring(0, 1) = "~" Then
                    If File.Exists(HttpContext.Current.Server.MapPath(pathString.Trim & ".jpg")) Then _
                        Return pathString.Trim & ".jpg"

                    If File.Exists(HttpContext.Current.Server.MapPath(pathString.Trim + ".png")) Then _
                       Return pathString.Trim & ".png"

                    If File.Exists(HttpContext.Current.Server.MapPath(pathString.Trim + ".gif")) Then _
                       Return pathString.Trim & ".gif"
                Else
                    If File.Exists(pathString.Trim & ".jpg") Then _
                        Return pathString.Trim & ".jpg"

                    If File.Exists(pathString.Trim & ".png") Then _
                        Return pathString.Trim & ".png"

                    If File.Exists(pathString.Trim + ".gif") Then _
                       Return pathString.Trim & ".gif"
                End If

            Catch ex As Exception
            End Try

            Return String.Empty

        End Function

        Public Shared Function getMagicZoomOrMainImage(ByVal item As String, ByVal businessUnit As String, ByVal Partner As String) As String
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()

            Dim returnString As String = String.Empty
            Dim magicZoomPaths() As String = {"/1/M/", "/1/S/", "/1/L/", "/2/M/", "/2/S/", "/2/L/", "/3/M/", "/3/S/", "/3/L/", "/4/M/", "/4/S/", "/4/L/"}
            Dim magiczoomPath As String = String.Empty
            For Each magiczoomPath In magicZoomPaths
                returnString = checkImageExists(def.ProductMagicZoomImagePathRelative + magiczoomPath, item, businessUnit, Partner)
                If returnString <> (def.ProductMagicZoomImagePathRelative + magiczoomPath) Then
                    Exit For
                End If
            Next

            If returnString = (def.ProductMagicZoomImagePathRelative + magiczoomPath) Then
                returnString = checkImageExists(def.ProductMainImagePathRelative, item, businessUnit, Partner)
                If returnString = def.ProductMainImagePathRelative Then
                    returnString = String.Empty
                End If
            End If
            Return returnString
        End Function
    End Class
End Namespace