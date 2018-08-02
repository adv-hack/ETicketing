Module DataTransfer

    Public Sub Main()

        If Command.Split(",").Length < 5 Then
            Console.Out.WriteLine("Missing Arguments!")
            Console.Out.WriteLine("Please enter the following arguments (seperated by commas):")
            Console.Out.WriteLine("1) OLEDB Connection String")
            Console.Out.WriteLine("2) SQL Connection String")
            Console.Out.WriteLine("3) Name of the table to update (or ALL to update all tables)")
            Console.Out.WriteLine("4) Update Product Descriptions - True/False")
            Console.Out.WriteLine("5) Replace Price List - True/False")
            Exit Sub
        End If

        Dim OleDbConnStr As String = Command.Split(",")(0)
        Dim SQLConnStr As String = Command.Split(",")(1)
        Dim TableName As String = Command.Split(",")(2)
        Dim updateProductDescriptions As Boolean = CBool(CStr(Command.Split(",")(3)).Trim)
        Dim replacePriceList As Boolean = CBool(CStr(Command.Split(",")(4)).Trim)


        Talent.Common.DataTransfer.OleDbConnectionString = OleDbConnStr
        Talent.Common.DataTransfer.SQLConnectionString = SQLConnStr

        Console.WriteLine("Attempting to update " & TableName)
        Console.WriteLine("Please wait...")

        'MsgBox(TableName, MsgBoxStyle.Exclamation, TableName)
        Select Case LCase(TableName.Trim)
            Case Is = "all"
                Talent.Common.DataTransfer.DoExtract_All(replacePriceList, updateProductDescriptions)

            Case Is = "tbl_group_level_01"
                Talent.Common.DataTransfer.DoExtract_EBGL01()
            Case Is = LCase("EBGL01")
                Talent.Common.DataTransfer.DoExtract_EBGL01()

            Case Is = "tbl_group_level_02"
                Talent.Common.DataTransfer.DoExtract_EBGL02()
            Case Is = LCase("EBGL02")
                Talent.Common.DataTransfer.DoExtract_EBGL02()

            Case Is = "tbl_group_level_03"
                Talent.Common.DataTransfer.DoExtract_EBGL03()
            Case Is = LCase("EBGL03")
                Talent.Common.DataTransfer.DoExtract_EBGL03()

            Case Is = "tbl_group_level_04"
                Talent.Common.DataTransfer.DoExtract_EBGL04()
            Case Is = LCase("EBGL04")
                Talent.Common.DataTransfer.DoExtract_EBGL04()

            Case Is = "tbl_group_level_05"
                Talent.Common.DataTransfer.DoExtract_EBGL05()
            Case Is = LCase("EBGL05")
                Talent.Common.DataTransfer.DoExtract_EBGL05()

            Case Is = "tbl_group"
                Talent.Common.DataTransfer.DoExtract_EBGROU()
            Case Is = LCase("EBGROU")
                Talent.Common.DataTransfer.DoExtract_EBGROU()

            Case Is = "tbl_group_product"
                Talent.Common.DataTransfer.DoExtract_EBGPPR()
            Case Is = LCase("EBGPPR")
                Talent.Common.DataTransfer.DoExtract_EBGPPR()

            Case Is = "tbl_price_list_header"
                Talent.Common.DataTransfer.DoExtract_EBPLHD()
            Case Is = LCase("EBPLHD")
                Talent.Common.DataTransfer.DoExtract_EBPLHD()

            Case Is = "tbl_price_list_detail"
                Talent.Common.DataTransfer.DoExtract_EBPLDT(replacePriceList)
            Case Is = LCase("EBPLDT")
                Talent.Common.DataTransfer.DoExtract_EBPLDT(replacePriceList)

            Case Is = "tbl_product"
                Talent.Common.DataTransfer.DoExtract_EBPROD(updateProductDescriptions)
            Case Is = LCase("EBPROD")
                Talent.Common.DataTransfer.DoExtract_EBPROD(updateProductDescriptions)

            Case Is = "tbl_product_relations"
                Talent.Common.DataTransfer.DoExtract_EBPRRL()
            Case Is = LCase("EBPRRL")
                Talent.Common.DataTransfer.DoExtract_EBPRRL()

            Case Is = "tbl_product_stock"
                Talent.Common.DataTransfer.DoExtract_EBPRST()
            Case Is = LCase("EBPRST")
                Talent.Common.DataTransfer.DoExtract_EBPRST()

            Case Is = "tbl_order_header"
                Talent.Common.DataTransfer.DoExtract_EBORDHT()
            Case Is = LCase("EBORDHT")
                Talent.Common.DataTransfer.DoExtract_EBORDHT()

            Case Is = "tbl_order_detail"
                Talent.Common.DataTransfer.DoExtract_EBORDDT()
            Case Is = LCase("EBORDDT")
                Talent.Common.DataTransfer.DoExtract_EBORDDT()

            Case Is = "tbl_invoice_header"
                Talent.Common.DataTransfer.DoExtract_EBINVHT()
            Case Is = LCase("EBINVHT")
                Talent.Common.DataTransfer.DoExtract_EBINVHT()

            Case Is = "tbl_product_options"
                Talent.Common.DataTransfer.DoExtract_EBPRDO()
            Case Is = LCase("EBPRDO")
                Talent.Common.DataTransfer.DoExtract_EBPRDO()

            Case Is = "tbl_product_option_defaults"
                Talent.Common.DataTransfer.DoExtract_EBPRDD()
            Case Is = LCase("EBPRDO")
                Talent.Common.DataTransfer.DoExtract_EBPRDD()

            Case Is = "tbl_ecommerce_module_defaults_bu"
                Talent.Common.DataTransfer.DoExtract_EBECMB()
            Case Is = LCase("EBECMB")
                Talent.Common.DataTransfer.DoExtract_EBECMB()

            Case Else
                Console.WriteLine("")
                Console.WriteLine("Table not found")
                Exit Sub

        End Select

        Console.WriteLine("Completed updating " & TableName)

    End Sub

End Module
