'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Hash tables having scope outside the 
'                                   CMS enviroment
'
'       Date                        Apr 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TCHASHT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
'   Friend      Specifies that one or more declared programming elements are accessible only from 
'               within the assembly that contains their declaration.
'
'--------------------------------------------------------------------------------------------------
Module Hashtable_Cache
    '
    Private _attHashtable As Hashtable = New Hashtable

    Private _multiLingualContent As Hashtable = New Hashtable
    Private _multilingualTitles As Hashtable = New Hashtable
    Private _multilingualHeaderTexts As Hashtable = New Hashtable
    Private _multilingualMetaKeywords As Hashtable = New Hashtable
    Private _multilingualMetaDescription As Hashtable = New Hashtable

    Private _cmsMnuHashtable As Hashtable = New Hashtable
    Private _cmsUcrHashtable As Hashtable = New Hashtable
    Private _cmsWfrHashtable As Hashtable = New Hashtable
    '
    Private _mnuHashtable As Hashtable = New Hashtable
    Private _ucrHashtable As Hashtable = New Hashtable
    Private _wfrHashtable As Hashtable = New Hashtable

    Friend Sub ClearHashTableCache()
        _attHashtable = New Hashtable
        _multiLingualContent = New Hashtable
        _multilingualTitles = New Hashtable
        _multilingualHeaderTexts = New Hashtable
        _multilingualMetaKeywords = New Hashtable
        _multilingualMetaDescription = New Hashtable
        _cmsMnuHashtable = New Hashtable
        _cmsUcrHashtable = New Hashtable
        _cmsWfrHashtable = New Hashtable
        _mnuHashtable = New Hashtable
        _ucrHashtable = New Hashtable
        _wfrHashtable = New Hashtable
    End Sub

    Friend Property AttHashtable() As Hashtable
        Get
            Return _attHashtable
        End Get
        Set(ByVal value As Hashtable)
            _attHashtable = value
        End Set
    End Property

    Friend Property MultiLingualContent() As Hashtable
        Get
            Return _multiLingualContent
        End Get
        Set(ByVal value As Hashtable)
            _multiLingualContent = value
        End Set
    End Property
    Friend Property MultilingualTitles() As Hashtable
        Get
            Return _multilingualTitles
        End Get
        Set(ByVal value As Hashtable)
            _multilingualTitles = value
        End Set
    End Property
    Friend Property MultilingualHeaderTexts() As Hashtable
        Get
            Return _multilingualHeaderTexts
        End Get
        Set(ByVal value As Hashtable)
            _multilingualHeaderTexts = value
        End Set
    End Property
    Friend Property MultilingualMetaKeywords() As Hashtable
        Get
            Return _multilingualMetaKeywords
        End Get
        Set(ByVal value As Hashtable)
            _multilingualMetaKeywords = value
        End Set
    End Property
    Friend Property MultilingualMetaDescription() As Hashtable
        Get
            Return _multilingualMetaDescription
        End Get
        Set(ByVal value As Hashtable)
            _multilingualMetaDescription = value
        End Set
    End Property

    Friend Property CmsMnuHashtable() As Hashtable
        Get
            Return _cmsMnuHashtable
        End Get
        Set(ByVal value As Hashtable)
            _cmsMnuHashtable = value
        End Set
    End Property
    Friend Property CmsUcrHashtable() As Hashtable
        Get
            Return _cmsUcrHashtable
        End Get
        Set(ByVal value As Hashtable)
            _cmsUcrHashtable = value
        End Set
    End Property
    Friend Property CmsWfrHashtable() As Hashtable
        Get
            Return _cmsWfrHashtable
        End Get
        Set(ByVal value As Hashtable)
            _cmsWfrHashtable = value
        End Set
    End Property
    Friend Property MnuHashtable() As Hashtable
        Get
            Return _mnuHashtable
        End Get
        Set(ByVal value As Hashtable)
            _mnuHashtable = value
        End Set
    End Property
    Friend Property UcrHashtable() As Hashtable
        Get
            Return _ucrHashtable
        End Get
        Set(ByVal value As Hashtable)
            _ucrHashtable = value
        End Set
    End Property
    Friend Property WfrHashtable() As Hashtable
        Get
            Return _wfrHashtable
        End Get
        Set(ByVal value As Hashtable)
            _wfrHashtable = value
        End Set
    End Property

End Module
