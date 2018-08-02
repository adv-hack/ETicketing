Imports Talent.Common
Imports Talent.eCommerce

Partial Class PagesAdmin_DataTransfer
    Inherits TalentBase01

    Dim moduleDefs As ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
    Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefs.GetDefaults

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        DataTransfer.DoExtract_EBGPPR()
    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.Click
        DataTransfer.DoExtract_EBGL01()
    End Sub

    Protected Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
        DataTransfer.DoExtract_EBGL02()
    End Sub

    Protected Sub Button4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button4.Click
        DataTransfer.DoExtract_EBGL03()
    End Sub

    Protected Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
        DataTransfer.DoExtract_EBGL04()
    End Sub

    Protected Sub Button6_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button6.Click
        DataTransfer.DoExtract_EBGL05()
    End Sub

    Protected Sub Button22_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button22.Click
        DataTransfer.DoExtract_EBGL07()
    End Sub

    Protected Sub Button23_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button23.Click
        DataTransfer.DoExtract_EBGL08()
    End Sub

    Protected Sub Button24_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button24.Click
        DataTransfer.DoExtract_EBGL09()
    End Sub

    Protected Sub Button25_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button25.Click
        DataTransfer.DoExtract_EBGL10()
    End Sub

    Protected Sub Button7_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button7.Click
        DataTransfer.DoExtract_EBPROD(def.UpdateProductDescriptionsDT)
    End Sub

    Protected Sub Button8_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button8.Click
        DataTransfer.DoExtract_EBPRRL()
    End Sub

    Protected Sub Button9_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button9.Click
       
        DataTransfer.DoExtract_EBPLDT(def.ReplacePriceListDetailDT)
    End Sub

    Protected Sub Button10_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button10.Click
        DataTransfer.DoExtract_EBPLHD()
    End Sub

    Protected Sub Button11_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button11.Click
        DataTransfer.DoExtract_EBGROU()
    End Sub

    Protected Sub Button12_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button12.Click
        DataTransfer.DoExtract_EBPRST()
    End Sub

    Protected Sub Button13_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button13.Click
       
        DataTransfer.DoExtract_All(def.ReplacePriceListDetailDT, def.UpdateProductDescriptionsDT)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DataTransfer.OleDbConnectionString = ConfigurationManager.ConnectionStrings("TALENTEBOleDBConnectionString").ToString 'Insert code to retrieve from web.config here
        DataTransfer.SQLConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString 'Insert code to retrieve from web.config here
        DataTransfer.BusinessUnit = TalentCache.GetBusinessUnit()

        DataTransfer.StockLocation = def.StockLocation

    End Sub

    Protected Sub Button14_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button14.Click
        'Order Header
        DataTransfer.DoExtract_EBORDHT()
        'Order Detail
        DataTransfer.DoExtract_EBORDDT()
    End Sub

    Protected Sub Button15_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button15.Click
        DataTransfer.DoExtract_EBINVHT()
    End Sub

    Protected Sub Button16_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button16.Click
        DataTransfer.DoExtract_EBPRDO()
        DataTransfer.DoExtract_EBPRDD()
    End Sub

    Protected Sub Button17_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button17.Click
        DataTransfer.DoExtract_EBUSERT(def.DataTransfer_BackendDatabaseType, def.DataTransfer_CreatePartnerOnAddCustomer)
    End Sub

    Protected Sub Button18_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button18.Click
        DataTransfer.DoExtract_EBGL06()
    End Sub

    Protected Sub Button19_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button19.Click
        DataTransfer.DoExtract_EBECMB()
    End Sub
    Protected Sub Button21_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button21.Click
        DataTransfer.DoExtract_EBPLDTP()
    End Sub
    Protected Sub Button26_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button26.Click
        ' Order template header
        DataTransfer.DoExtract_EBOTHD()
        ' Order template details
        DataTransfer.DoExtract_EBOTDT()
    End Sub

    Protected Sub Button20_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button20.Click
        DataTransfer.DoExtract_EBCNEHT()
    End Sub

    Protected Sub btnPromotions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPromotions.Click
        DataTransfer.DoExtract_EBPMF()
        DataTransfer.DoExtract_EBPML()
        DataTransfer.DoExtract_EBPMD()
        DataTransfer.DoExtract_EBPMR()
        DataTransfer.DoExtract_EBPM()
    End Sub
End Class