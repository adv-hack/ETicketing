Imports TalentSystemDefaults
Imports TalentSystemDefaults.DataEntities

Partial Class MasterConfigurationResult
    Inherits System.Web.UI.Page
    Const NEWLINE As String = "<br>"
    Const TAB As String = "&nbsp;&nbsp;&nbsp;&nbsp;"
    Const OCB As String = "{"
    Const CCB As String = "}"
    Const LT As String = "&lt;"
    Const GT As String = "&gt;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim masterConfigs As ConfigurationEntity() = Session("MasterConfigs")
        If (masterConfigs IsNot Nothing) Then
            dvContent.InnerHtml = GetClassTemplate(masterConfigs)
        End If
    End Sub

    Private Function GetClassTemplate(masterConfigs As ConfigurationEntity()) As String
        Dim dataModule As DataModuleClass = Session("DataModule")
        Dim className As String = dataModule.ClassName
        Dim showAsModule As String = dataModule.ShowAsModule
        Dim moduleTitle As String = dataModule.ModuleTitle
        Dim content As New StringBuilder()
        content.Append("<pre>")
        content.AppendLine("using System.Collections.Generic;")
        content.AppendLine("namespace TalentSystemDefaults")
        content.AppendLine(OCB)
        content.AppendLine(TAB & "namespace TalentModules")
        content.AppendLine(TAB & OCB)
        'content.Append(TAB)
        content.Append(TAB & TAB & String.Format("public class {0}", className))
        content.AppendLine(" : DMBase")
        content.AppendLine(TAB & TAB & OCB)
        'content.Append(NEWLINE & TAB & TAB)

        content.AppendLine(TAB & TAB & TAB & String.Format("static private bool _EnableAsModule = {0};", showAsModule.ToLower))
        content.AppendLine(TAB & TAB & TAB & "public static bool EnableAsModule")
        content.AppendLine(TAB & TAB & TAB & OCB)
        content.AppendLine(TAB & TAB & TAB & TAB & "get")
        content.AppendLine(TAB & TAB & TAB & TAB & OCB)
        content.AppendLine(TAB & TAB & TAB & TAB & TAB & "return _EnableAsModule;")
        content.AppendLine(TAB & TAB & TAB & TAB & CCB)
        content.AppendLine(TAB & TAB & TAB & TAB & "set")
        content.AppendLine(TAB & TAB & TAB & TAB & OCB)
        content.AppendLine(TAB & TAB & TAB & TAB & TAB & "_EnableAsModule = value;")
        content.AppendLine(TAB & TAB & TAB & TAB & CCB)
        content.AppendLine(TAB & TAB & TAB & CCB)

        content.AppendLine(TAB & TAB & TAB & String.Format("static private string _ModuleTitle = ""{0}"";", moduleTitle))
        content.AppendLine(TAB & TAB & TAB & "public static string ModuleTitle")
        content.AppendLine(TAB & TAB & TAB & OCB)
        content.AppendLine(TAB & TAB & TAB & TAB & "get")
        content.AppendLine(TAB & TAB & TAB & TAB & OCB)
        content.AppendLine(TAB & TAB & TAB & TAB & TAB & "return _ModuleTitle;")
        content.AppendLine(TAB & TAB & TAB & TAB & CCB)
        content.AppendLine(TAB & TAB & TAB & TAB & "set")
        content.AppendLine(TAB & TAB & TAB & TAB & OCB)
        content.AppendLine(TAB & TAB & TAB & TAB & TAB & "_ModuleTitle = value;")
        content.AppendLine(TAB & TAB & TAB & TAB & CCB)
        content.AppendLine(TAB & TAB & TAB & CCB)

        For Each entity In masterConfigs
            content.Append(NEWLINE & TAB & TAB & TAB)
            content.Append(String.Format("public const string {0} = ""{1}"";", entity.DefaultName, entity.ConfigurationId))
        Next
        content.AppendLine(NEWLINE)
        content.Append(TAB & TAB & TAB)
        content.AppendLine(String.Format("public {0}(DESettings settings, bool initialiseData)", className))
        content.Append(TAB & TAB & TAB & TAB)
        content.AppendLine(": base(ref settings,initialiseData)")
        content.AppendLine(TAB & TAB & TAB & OCB)
        content.Append(TAB & TAB & TAB & CCB)

        content.Append(NEWLINE & TAB & TAB & TAB)
        content.AppendLine("public override void SetModuleConfiguration()")
        content.Append(TAB & TAB & TAB & OCB)
        content.AppendLine(NEWLINE)

        For Each entity In masterConfigs
            Dim validationGroupParam As String = String.Empty
            content.Append(NEWLINE & TAB & TAB & TAB)
            If (entity.ValidationGroup.Count > 0) Then
                Dim validationParams As New List(Of String)
                If (entity.MinLength IsNot Nothing) Then
                    validationParams.Add(String.Format("minLength:{0}", entity.MinLength))
                End If
                If (entity.MaxLength IsNot Nothing) Then
                    validationParams.Add(String.Format("maxLength:{0}", entity.MaxLength))
                End If
                Dim extraParms As String = String.Empty
                If validationParams.Count > 0 Then
                    extraParms = ", "
                End If
                validationGroupParam = String.Format(", FieldValidation.Add(new List{0} {1}{2}{3}{4}{5})", LT & "VG" & GT, OCB, String.Join(" , ", entity.ValidationGroup.ToArray), CCB, extraParms, String.Join(", ", validationParams.ToArray))
            End If
            content.Append(String.Format(TAB & "MConfigs.Add({0}, DataType.{1}, DisplayTabs.TabHeader{2}{3});", entity.DefaultName, entity.DataType, entity.DisplayTab, validationGroupParam))
        Next
        content.Append(NEWLINE & TAB & TAB & TAB & TAB)
        content.Append("Populate();")
        content.Append(NEWLINE & TAB & TAB & TAB & CCB)
        content.Append(NEWLINE & TAB & TAB & CCB)
        content.Append(NEWLINE & TAB & CCB)
        content.Append(NEWLINE & CCB)
        content.Append("</pre>")
        Return content.ToString()
    End Function

End Class
