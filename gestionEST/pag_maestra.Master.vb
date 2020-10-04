Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports DevExpress.Web
Imports Ionic.Zip
Imports System.Collections.Generic
Imports System.Net.Mime

Public Class pag_maestra
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim connExtra As New SqlConnection
        Dim conex_usable As SqlConnection

        'BUSQUEDA DE REPORTES

        txtUsuario.InnerText = Session("pubNombreUsuario")
        lblNombreEmpresa.InnerText = Session("pubEmpUsuariaUsuario")
        form1.DefaultButton = btnDefault.UniqueID

        'IMAGEN
        Dim datosimagen As String = ""
        If ConectaSQLServer() Then
            If conn.State <> ConnectionState.Open Then
                Using connExtra
                    Try
                        sqlStr = "SELECT imagen 
                            FROM ccUsuarioWeb 
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"
                        cmd = New SqlCommand(sqlStr, connExtra)
                        datosimagen = cmd.ExecuteScalar()
                    Catch ex As Exception
                        ShowPopUpMsg("Error imgn: " + ex.ToString)
                    End Try
                End Using
            Else
                Using conn
                    Try
                        sqlStr = "SELECT imagen 
                            FROM ccUsuarioWeb 
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"
                        cmd = New SqlCommand(sqlStr, conn)
                        datosimagen = cmd.ExecuteScalar()
                    Catch ex As Exception
                        ShowPopUpMsg("Error imgn: " + ex.ToString)
                    End Try
                End Using
            End If
        End If


        If datosimagen = "" Then
            Image1.ImageUrl = "images/10.png"
            Image1.Height = 50
            Image1.Width = 80
        Else
            Image1.ImageUrl = datosimagen
            Image1.Height = 50
            Image1.Width = 80
        End If

        'LOADING PANEL
        lpCallBackEST.ContainerElementID = LeftPanel.ID
    End Sub

    Protected Sub btnCerrarSesion_ServerClick(sender As Object, e As EventArgs)
        Session("DotacionMensual") = Nothing
        Session("pubRutContrato") = Nothing
        Session("Validado") = 0
        Session("pubIdUsuario") = Nothing
        Session("pubEmpUsuariaUsuario") = Nothing
        Session("pubNombreUsuario") = Nothing
        Session("pubUnidadesUsuario") = Nothing
        Session("pubEsAdminEst") = Nothing
        Session("pubIsAdmin") = Nothing
        Session("CambiarClave") = 0
        Session("Administrador") = Nothing
        Session("Auditor") = Nothing
        Session("Web") = Nothing
        Session("Operaciones") = Nothing
        Session("Cliente") = Nothing
        Session("SuperAdmin") = Nothing
        Session("pModoPopup") = ""
        Session("pModoPopupSolicitud") = ""
        Session("pModoPopupCreaDoc") = ""
        Session("pubFileNameContrato") = Nothing
        Session("RecargarEvaluacion") = Nothing
        Response.Redirect("~/pag_login.aspx")
    End Sub

    Protected Sub btnCambioClave_ServerClick(sender As Object, e As EventArgs)
        Session("CambiarClave") = 1
        Response.Redirect("pag_seguridad.aspx")
    End Sub

    Private Sub pag_maestra_Init(sender As Object, e As EventArgs) Handles Me.Init

        'PODER VER ÍTEM DE EVALUACIONES
        If Session("pubIdUsuario") <> Nothing Then
            If verItemEvaluacionArauco(Session("pubIdUsuario")) = False Then
                ASPxMenu1.Items.FindByName("evaluacionArauco").Visible = False
            End If
        End If

        'ACCESO DE PERFILES A MENU
        If Session("Administrador") = True Then
            ASPxMenu1.Items.FindByName("auditoria").Visible = False
        End If

        If Session("Operaciones") = True Or Session("Web") = True Then
            ASPxMenu1.Items.FindByName("contrataciones").Visible = False
        End If

        If Session("Cliente") = True Then
            ASPxMenu1.Items.FindByName("contrataciones").Visible = False
            ASPxMenu1.Items.FindByName("crearDocs").Visible = False
            ASPxMenu1.Items.FindByName("auditoria").Visible = False
            ASPxMenu1.Items.FindByName("descarga").Visible = False
        End If

        ASPxSplitter1.Images.VerticalCollapseBackwardButton.Url = "images/01_Left.png"
        ASPxSplitter1.Images.VerticalCollapseBackwardButton.UrlHottracked = "images/01_Left.png"
        ASPxSplitter1.Images.VerticalCollapseBackwardButton.Width = Unit.Pixel(7)
        ASPxSplitter1.Images.VerticalCollapseForwardButton.Url = "images/01.png"
        ASPxSplitter1.Images.VerticalCollapseForwardButton.UrlHottracked = "images/01.png"
        ASPxSplitter1.Images.VerticalCollapseForwardButton.Width = Unit.Pixel(7)
    End Sub

    Protected Sub btnDescargarEST_Click(sender As Object, e As EventArgs)

        'DESCARGAR TODOS
        If cbxDescargarDocumentoEST.SelectedItem.Value = 0 Then
            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                zip.AddDirectoryByName("Documentación Corporativa EST")
                'ASIGNO LOS ARCHIVOS A COMPRIMIR
                Dim filePaths = Directory.GetFiles(Server.MapPath("/documentos/docs_Gestion/"))
                For Each archivo As String In filePaths
                    zip.AddFile(archivo, "Documentación Corporativa EST")
                Next
                Response.Clear()
                Response.BufferOutput = False
                Dim zipName As String = [String].Format("{0}_{1}.zip", "Documentación Corporativa EST", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                Response.ContentType = "application/zip"
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
                zip.Save(Response.OutputStream)
                Response.End()
            End Using

            'ARCHIVO DEPENDIENDO DE SELECCIÓN
        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 1 Then
            Dim filePath As String = "/documentos/docs_Gestion/Carta Ausencia a Trabajar_EST.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 2 Then
            Dim filePath As String = "/documentos/docs_Gestion/Carta Vencimiento de Plazo_EST.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 3 Then
            Dim filePath As String = "/documentos/docs_Gestion/Comprobante Entrega EPP_EST.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 4 Then
            Dim filePath As String = "/documentos/docs_Gestion/Ficha Personal_EST.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 5 Then
            Dim filePath As String = "/documentos/docs_Gestion/Inducción S&SSO_EST.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 6 Then
            Dim filePath As String = "/documentos/docs_Gestion/Inspeccion de EPP y Ropa de Trabajo_EST.xlsx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 7 Then
            Dim filePath As String = "/documentos/docs_Gestion/Observacion de Conducta_EST.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 8 Then
            Dim filePath As String = "/documentos/docs_Gestion/No Aplica Documentos - General.pdf"
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 9 Then
            Dim filePath As String = "/documentos/docs_Gestion/No Aplica Documentos - Individual.pdf"
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 10 Then
            Dim filePath As String = "/documentos/docs_Gestion/ODI - Obligación de Informar.docx"
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 11 Then
            Dim filePath As String = "/documentos/docs_Gestion/ARAUCO CONTRATO MARCO.pdf"
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 12 Then
            Dim filePath As String = "/documentos/docs_Gestion/MASONITE CONTRATO MARCO.pdf"
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 13 Then
            Dim filePath As String = "/documentos/docs_Gestion/OXIQUIM CONTRATO MARCO.pdf"
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumentoEST.SelectedItem.Value = 14 Then
            Dim filePath As String = "/documentos/docs_Gestion/RMD KWIKFORM CONTRATO MARCO.pdf"
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()
        End If
    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub
    Protected Sub cbpDescargarArchivosEST_Callback(sender As Object, e As CallbackEventArgsBase)

    End Sub
End Class