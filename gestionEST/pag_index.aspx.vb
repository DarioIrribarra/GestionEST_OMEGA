Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.Web
Imports Ionic.Zip

Public Class pag_index1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'BUSQUEDA DE REPORTES
        Session("BusquedaRealizada") = 0

        'SI NO TIENE UNIDADES SE ENVIA MENSAJE
        If Session("pubUnidadesUsuario") = Nothing Then
            ShowPopUpMsg("Error: No tiene Unidades Asignadas.\nEl funcionamiento de la plataforma no será óptimo.")
            If Page.IsCallback Then
                ASPxWebControl.RedirectOnCallback("pag_login.aspx")
            Else
                Response.Redirect("/pag_login.aspx", False)
                Context.ApplicationInstance.CompleteRequest()
            End If
            Return
        End If

        form1.DefaultButton = btnDefault.UniqueID
        txtUsuario.InnerText = Session("pubNombreUsuario")
        lblNombreEmpresa.InnerText = Session("pubEmpUsuariaUsuario")

        'ELIMINO LA TABLA DE LA BD CREADA EN LA DOTACION MENSUAL
        If Session("DotacionMensual") <> Nothing Then
            If ConectaSQLServer() Then
                Using conn
                    Try

                        sqlStr = $"DROP TABLE {Session("DotacionMensual")}"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.ExecuteNonQuery()
                        Session("DotacionMensual") = Nothing
                    Catch ex As Exception
                        ShowPopUpMsg("Error en Reporte 3 - Drop Table " + ex.ToString)
                        Return
                    End Try
                End Using
            End If
        End If


        'IMAGEN
        Dim datosimagen As String = ""
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT imagen 
                            FROM ccUsuarioWeb 
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"
                    cmd = New SqlCommand(sqlStr, conn)
                    datosimagen = cmd.ExecuteScalar()
                Catch ex As Exception

                End Try
            End Using
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
        lpCallBack.ContainerElementID = LeftPanel.ID
    End Sub

    Private Sub pag_index_Init(sender As Object, e As EventArgs) Handles Me.Init

        If Session("Validado") = 0 Then
            If Page.IsCallback Then
                ASPxWebControl.RedirectOnCallback("pag_login.aspx")
                Return
            Else
                Response.Redirect("/pag_login.aspx", False)
                Context.ApplicationInstance.CompleteRequest()
                Return
            End If
        End If

        'ELIMINO CUALQUIER TABLA QUE PUEDA QUEDAR PENDIENTE
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim nombreTabla = Session("pubIdUsuario").ToString.Replace(".", "0")
                    sqlStr = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'tmpdm{nombreTabla}%'"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    Dim reader As IDataReader = sqlcmd.ExecuteReader
                    Dim dato = ""
                    Dim conn2 As New SqlConnection
                    While reader.Read()
                        dato = reader.Item(0).ToString
                        If ConectaSQLServerConn(conn2) Then
                            Using conn2
                                Try
                                    sqlStr = $"DROP TABLE {dato}"
                                    Dim sqlcmd2 As New SqlCommand(sqlStr, conn2)
                                    sqlcmd2.ExecuteNonQuery()
                                Catch ex As Exception

                                End Try
                            End Using
                        End If
                    End While
                Catch ex As Exception

                End Try
            End Using
        End If
        Session("DotacionMensual") = Nothing

        'PODER VER ÍTEM DE EVALUACIONES
        If Session("pubIdUsuario") <> Nothing Then
            If verItemEvaluacionArauco(Session("pubIdUsuario")) = False Then
                ASPxMenu1.Items.FindByName("evaluacionArauco").Visible = False
            End If
        End If

        'ACCESO DE PERFILES A MENU
        If Session("SuperAdmin") = True Then
            ASPxMenu1.Items.FindByName("auditoria").Visible = True
        End If

        If Session("Administrador") = True Then
            ASPxMenu1.Items.FindByName("auditoria").Visible = False
        End If

        If Session("Web") = True Or Session("Operaciones") = True Then
            ASPxMenu1.Items.FindByName("contrataciones").Visible = False
        End If

        If Session("Cliente") = True Then
            ASPxMenu1.Items.FindByName("contrataciones").Visible = False
            ASPxMenu1.Items.FindByName("crearDocs").Visible = False
            ASPxMenu1.Items.FindByName("auditoria").Visible = False
            ASPxMenu1.Items.FindByName("descarga").Visible = False
        End If

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
        Response.Redirect("~/pag_login.aspx")
    End Sub

    Protected Sub btnCambioClave_ServerClick(sender As Object, e As EventArgs)
        Session("CambiarClave") = 1
        Response.Redirect("pag_seguridad.aspx")
    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Protected Sub btnDescargar_Click(sender As Object, e As EventArgs)

        'DESCARGAR TODOS
        If cbxDescargarDocumento.SelectedItem.Value = 0 Then
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
        ElseIf cbxDescargarDocumento.SelectedItem.Value = 1 Then
            Dim filePath As String = "/documentos/docs_Gestion/Carta Ausencia a Trabajar_EST.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 2 Then
            Dim filePath As String = "/documentos/docs_Gestion/Carta Vencimiento de Plazo_EST.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 3 Then
            Dim filePath As String = "/documentos/docs_Gestion/Comprobante Entrega EPP_EST.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 4 Then
            Dim filePath As String = "/documentos/docs_Gestion/Ficha Personal_EST.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 5 Then
            Dim filePath As String = "/documentos/docs_Gestion/Inducción S&SSO_EST.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 6 Then
            Dim filePath As String = "/documentos/docs_Gestion/Inspeccion de EPP y Ropa de Trabajo_EST.xlsx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 7 Then
            Dim filePath As String = "/documentos/docs_Gestion/Observacion de Conducta_EST.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 8 Then
            Dim filePath As String = "/documentos/docs_Gestion/No Aplica Documentos - General.pdf"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 9 Then
            Dim filePath As String = "/documentos/docs_Gestion/No Aplica Documentos - Individual.pdf"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 10 Then
            Dim filePath As String = "/documentos/docs_Gestion/ODI - Obligación de Informar.docx"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 11 Then
            Dim filePath As String = "/documentos/docs_Gestion/ARAUCO CONTRATO MARCO.pdf"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 12 Then
            Dim filePath As String = "/documentos/docs_Gestion/MASONITE CONTRATO MARCO.pdf"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 13 Then
            Dim filePath As String = "/documentos/docs_Gestion/OXIQUIM CONTRATO MARCO.pdf"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        ElseIf cbxDescargarDocumento.SelectedItem.Value = 14 Then
            Dim filePath As String = "/documentos/docs_Gestion/RMD KWIKFORM CONTRATO MARCO.pdf"
            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(filePath)))
            Response.WriteFile(filePath)
            Response.End()

        End If
    End Sub

    Protected Sub cbpDescargarArchivos_Callback(sender As Object, e As CallbackEventArgsBase)

    End Sub
End Class