Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.Export
Imports DevExpress.Web
Imports DevExpress.XtraPrinting
Imports MailKit.Net.Smtp
Imports MimeKit


Public Class pag_requerimientos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub pag_requerimientos_Init(sender As Object, e As EventArgs) Handles Me.Init

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

        'CARGA DE UNIDADES
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT id as idUnidad,UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion 
                    FROM ccUnidades 
                    WHERE id IN (" & Session("pubUnidadesUsuario") & ") 
                    ORDER BY descripcion"

                    Call spLlenaComboBoxPopUp(sqlStr, cbxPlanta, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT id as idUnidad,UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion 
                    FROM ccUnidades 
                    WHERE id IN (" & Session("pubUnidadesUsuario") & ") 
                    ORDER BY descripcion"

                    Call spLlenaComboBox(sqlStr, cbxUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA DE CAUSALES
                    sqlStr = "SELECT Codigo, Descrip
                    FROM cpCausasLegales 
                    WHERE causalContrato IS NOT NULL"

                    Call spLlenaComboBoxPopUp(sqlStr, cbxCausal, "Codigo", "Descrip")
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If



        'CARGAR GRID
        If IsPostBack = False Then
            Call cargarGridSolicitudes()
        End If

        'Administrador
        If Session("Cliente") = False Then
            tr_popupSolicitud.Visible = False
        Else
            tr_popupSolicitud.Visible = True
        End If

        If Session("SuperAdmin") = True Then
            tr_popupSolicitud.Visible = True
        End If


        gridSolicitudes.FilterExpression = Nothing
        gridSolicitudes.ClearSort()
        div_solicitudes.Visible = True
        btnpopUpSolicitud.Text = "Crear Solicitud"
        tr_excel.Visible = True
        Me.Form.DefaultButton = btnpopUpSolicitud.UniqueID
    End Sub

    Sub cargarGridSolicitudes()

        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA GRID CON ACCESO A TODOS LOS HISTORIALES
                    If Session("Administrador") = True Or Session("SuperAdmin") = True Or Session("Auditor") = True Or Session("Web") = True Then
                        If cbxUnidad.Value <> Nothing Then
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                            UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                            UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                            UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                            archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                            FROM diSolicitud
                            WHERE idUnidad = '{cbxUnidad.SelectedItem.Value}'
                            ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        Else
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                            UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                            UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                            UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                            archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                            FROM diSolicitud
                            ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        End If

                    End If
                    'CARGA GRID CON ACCESO A HISTORIALES DE LA PLANTA QUE ESTÁ A CARGO
                    If Session("Operaciones") = True Then
                        If cbxUnidad.SelectedItem.Value <> Nothing Then
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE idUnidad IN ({Session("pubUnidadesUsuario")}) AND idUnidad = '{cbxUnidad.SelectedItem.Value}'
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        Else
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE idUnidad IN ({Session("pubUnidadesUsuario")})
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        End If
                    End If
                    'CARGA GRID CON ACCESO A HISTORIAL DE SOLICITUDES QUE LA PERSONA HA REALIZADO
                    If Session("Cliente") = True Then
                        If cbxUnidad.SelectedItem.Value <> Nothing Then
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE nombreUsuarioSolicitud ='{Session("pubIdUsuario")}' AND idUnidad = '{cbxUnidad.SelectedItem.Value}'
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        Else
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE nombreUsuarioSolicitud = '" & Session("pubIdUsuario") & "'
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        End If
                    End If

                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If

    End Sub

    Protected Sub ASPxHyperLink1_Load(sender As Object, e As EventArgs)
        Dim link As ASPxHyperLink = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer
        Dim myValue As String = gridSolicitudes.GetRowValues(container.ItemIndex, container.Column.FieldName)

        If myValue <> "" Then
            link.ImageUrl = "~/images/15.png"
            link.NavigateUrl = myValue
            link.Target = "_blank"

        Else
            link.ImageUrl = "~/images/12.png"
            link.NavigateUrl = ""
        End If
    End Sub

    Private Const TempDirectory As String = "~/Temp/"
    Protected Sub UploadControl_FileUploadComplete(sender As Object, e As FileUploadCompleteEventArgs)

        Dim resultExtension As String = Path.GetExtension(e.UploadedFile.FileName)
        Dim resultFileName As String = Path.ChangeExtension(Path.GetRandomFileName(), resultExtension)
        Dim resultFileUrl As String = TempDirectory & resultFileName
        Dim resultFilePath As String = MapPath(resultFileUrl)
        e.UploadedFile.SaveAs(resultFilePath)

        Dim name As String = e.UploadedFile.FileName
        Dim url As String = ResolveClientUrl(resultFileUrl)
        Dim sizeInKilobytes As Long = e.UploadedFile.ContentLength / 1024
        Dim sizeText As String = sizeInKilobytes.ToString() & " KB"


        Dim uploadControl As ASPxUploadControl = TryCast(sender, ASPxUploadControl)

        If uploadControl.UploadedFiles IsNot Nothing AndAlso uploadControl.UploadedFiles.Length > 0 Then
            For i As Integer = 0 To uploadControl.UploadedFiles.Length - 1
                Dim file As UploadedFile = uploadControl.UploadedFiles(i)
                If file.FileName <> "" Then
                    Session("pubFileName") = Trim(txtNombreReferido.Text) + "-" + Now().ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName)
                    Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                    file.SaveAs(fileName, True)
                End If
            Next i
        End If

    End Sub

    Protected Sub btnCrearSolicitud_Click(sender As Object, e As EventArgs)
        Dim snumeroSolicitudes As Integer = 0

        If Session("pModoPopupSolicitud") = "C" Then
            If Session("pubFileName") <> "" Then
                Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                System.IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
                System.IO.File.Delete(fileName)
            End If

            If ConectaSQLServer() Then
                Using conn
                    Try
                        Using cmd As New SqlCommand("pr_diAgregaSolicitud")
                            Using sda As New SqlDataAdapter()
                                cmd.CommandType = CommandType.StoredProcedure
                                cmd.Parameters.AddWithValue("@idUnidad", cbxPlanta.SelectedItem.Value)
                                cmd.Parameters.AddWithValue("@nombreUsuarioSolicitud", Session("pubIdUsuario"))
                                cmd.Parameters.AddWithValue("@empUsuaria", Session("pubEmpUsuariaUsuario"))
                                cmd.Parameters.AddWithValue("@area", txtArea.Text.Trim)
                                cmd.Parameters.AddWithValue("@planta", cbxPlanta.SelectedItem.Text.Trim)
                                cmd.Parameters.AddWithValue("@centroCosto", cbxCentroCosto.Text.Trim)
                                cmd.Parameters.AddWithValue("@cargo", txtCargo.Text.Trim)
                                cmd.Parameters.AddWithValue("@causal", cbxCausal.SelectedItem.Text.Trim)
                                cmd.Parameters.AddWithValue("@motivoCausal", txtMotivoCausal.Text.Trim)
                                cmd.Parameters.AddWithValue("@fechaInicio", dtFechaDesde.Date.ToString("yyyy-MM-dd"))
                                cmd.Parameters.AddWithValue("@dias", txtDias.Text.Trim)
                                cmd.Parameters.AddWithValue("@fechaTermino", dtFechaHasta.Date.ToString("yyyy-MM-dd"))
                                cmd.Parameters.AddWithValue("@sueldo", txtSuedoBase.Text.Trim)
                                cmd.Parameters.AddWithValue("@turno", txtTurno.Text.Trim)
                                cmd.Parameters.AddWithValue("@jefe", txtJefeACargo.Text.Trim)
                                cmd.Parameters.AddWithValue("@cantRequerida", spnCargaMasiva.Value)
                                If chkReferido.Checked = True Then
                                    cmd.Parameters.AddWithValue("@referido", "SI")
                                    cmd.Parameters.AddWithValue("@datosReferido", txtNombreReferido.Text.Trim)
                                Else
                                    cmd.Parameters.AddWithValue("@referido", "NO")
                                    cmd.Parameters.AddWithValue("@datosReferido", "S/D")
                                End If

                                If Session("pubFileName") <> "" Then
                                    cmd.Parameters.AddWithValue("@archivo", "~/SCAN/" + Session("pubFileName"))
                                Else
                                    cmd.Parameters.AddWithValue("@archivo", "")
                                End If

                                'PARAMETROS OUT
                                cmd.Parameters.Add("@snumeroSolicitudes", SqlDbType.Int).Direction = ParameterDirection.Output

                                cmd.Connection = conn
                                cmd.ExecuteNonQuery()

                                snumeroSolicitudes = cmd.Parameters("@snumeroSolicitudes").Value
                                If snumeroSolicitudes = Nothing Then
                                    snumeroSolicitudes = snumeroSolicitudes + 1
                                End If


                                'YA NO SOLICITA EMAIL
                                Call subSendMail(cbxPlanta.SelectedItem.Value, Session("pubIdUsuario"), snumeroSolicitudes.ToString)

                                ShowPopUpMsg("¡Solicitud Creada Satisfactoriamente!\nRecuerde, Tiene 1 hora como MÁXIMO para realizar cambios\n*Si el Correo de confirmación no le llega dentro de los próximos 10 minutos, favor coordinar con la persona encargada directamente*")
                                Call cargarGridSolicitudes()
                                '-------LIMPIAR PANTALLA DE SOLICITUD-----
                                popUpCrearSolicitud.ShowOnPageLoad = False
                                Form.DefaultButton = Nothing
                                cbxPlanta.SelectedIndex = -1
                                cbxCentroCosto.Text = Nothing
                                txtArea.Text = Nothing
                                txtCargo.Text = Nothing
                                cbxCausal.SelectedIndex = -1
                                txtMotivoCausal.Text = Nothing
                                dtFechaDesde.Text = Nothing
                                txtDias.Value = 1
                                dtFechaHasta.Text = Nothing
                                txtSuedoBase.Text = Nothing
                                txtTurno.Text = Nothing
                                txtJefeACargo.Text = Nothing
                                chkReferido.Checked = False
                                txtNombreReferido.Text = Nothing
                                Session("pubFileName") = ""
                                cbxPlanta.Focus()
                            End Using
                        End Using
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try

                End Using
            Else
                ShowPopUpMsg("No hay conexión a la base de datos")
            End If

        End If

        If Session("pModoPopupSolicitud") = "M" Then
            If Session("pubFileName") <> "" Then
                If Session("pubFileName").ToString.Contains("SCAN") Then
                    'HACER NADA SI YA VIENE EL MISMO ARCHIVO
                Else
                    Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                    System.IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
                    System.IO.File.Delete(fileName)
                End If
            End If

            Dim pCodigo As String = gridSolicitudes.GetRowValues(gridSolicitudes.FocusedRowIndex, "idSolicitud").ToString()
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Using cmd As New SqlCommand("pr_diModificaSolicitud")
                            Using sda As New SqlDataAdapter()
                                cmd.CommandType = CommandType.StoredProcedure
                                cmd.Parameters.AddWithValue("@idSolicitud", pCodigo)
                                cmd.Parameters.AddWithValue("@idUnidad", cbxPlanta.SelectedItem.Value)
                                cmd.Parameters.AddWithValue("@nombreUsuarioSolicitud", Session("pubIdUsuario"))
                                cmd.Parameters.AddWithValue("@empUsuaria", Session("pubEmpUsuariaUsuario"))
                                cmd.Parameters.AddWithValue("@area", txtArea.Text.Trim)
                                cmd.Parameters.AddWithValue("@planta", cbxPlanta.SelectedItem.Text.Trim)
                                cmd.Parameters.AddWithValue("@centroCosto", cbxCentroCosto.Text.Trim)
                                cmd.Parameters.AddWithValue("@cargo", txtCargo.Text.Trim)
                                cmd.Parameters.AddWithValue("@causal", cbxCausal.Value)
                                cmd.Parameters.AddWithValue("@motivoCausal", txtMotivoCausal.Text.Trim)
                                cmd.Parameters.AddWithValue("@fechaInicio", dtFechaDesde.Date.ToString("yyyy-MM-dd"))
                                cmd.Parameters.AddWithValue("@dias", txtDias.Text.Trim)
                                cmd.Parameters.AddWithValue("@fechaTermino", dtFechaHasta.Date.ToString("yyyy-MM-dd"))
                                cmd.Parameters.AddWithValue("@sueldo", txtSuedoBase.Text.Trim)
                                cmd.Parameters.AddWithValue("@turno", txtTurno.Text.Trim)
                                cmd.Parameters.AddWithValue("@jefe", txtJefeACargo.Text.Trim)
                                cmd.Parameters.AddWithValue("@cantRequerida", spnCargaMasiva.Value)
                                If chkReferido.Checked = True Then
                                    cmd.Parameters.AddWithValue("@referido", "SI")
                                    cmd.Parameters.AddWithValue("@datosReferido", txtNombreReferido.Text.Trim)
                                Else
                                    cmd.Parameters.AddWithValue("@referido", "NO")
                                    cmd.Parameters.AddWithValue("@datosReferido", "S/D")
                                End If

                                If Session("pubFileName") <> "" Then
                                    If Session("pubFileName").ToString.Contains("~/SCAN/") Then
                                        cmd.Parameters.AddWithValue("@archivo", Session("pubFileName"))
                                    Else
                                        cmd.Parameters.AddWithValue("@archivo", "~/SCAN/" + Session("pubFileName"))
                                    End If
                                Else
                                    cmd.Parameters.AddWithValue("@archivo", "")
                                End If

                                'PARAMETROS OUT
                                cmd.Parameters.Add("@snumeroSolicitudes", SqlDbType.Int).Direction = ParameterDirection.Output

                                cmd.Connection = conn
                                cmd.ExecuteNonQuery()

                                snumeroSolicitudes = cmd.Parameters("@snumeroSolicitudes").Value
                                If snumeroSolicitudes = Nothing Then
                                    snumeroSolicitudes = snumeroSolicitudes + 1
                                End If


                                'YA NO SOLICITA EMAIL
                                Call subSendMail(cbxPlanta.SelectedItem.Value, Session("pubIdUsuario"), snumeroSolicitudes.ToString)

                                ShowPopUpMsg("¡Solicitud Modificada Satisfactoriamente!")
                                Call cargarGridSolicitudes()
                                '-------LIMPIAR PANTALLA DE SOLICITUD-----
                                popUpCrearSolicitud.ShowOnPageLoad = False
                                Form.DefaultButton = Nothing
                                cbxPlanta.SelectedIndex = -1
                                cbxCentroCosto.Text = Nothing
                                txtArea.Text = Nothing
                                txtCargo.Text = Nothing
                                cbxCausal.SelectedIndex = -1
                                txtMotivoCausal.Text = Nothing
                                dtFechaDesde.Text = Nothing
                                txtDias.Value = 1
                                dtFechaHasta.Text = Nothing
                                txtSuedoBase.Text = Nothing
                                txtTurno.Text = Nothing
                                txtJefeACargo.Text = Nothing
                                chkReferido.Checked = False
                                txtNombreReferido.Text = Nothing
                                Session("pubFileName") = ""
                                cbxPlanta.Focus()
                            End Using
                        End Using
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try

                End Using
            Else
                ShowPopUpMsg("No hay conexión a la base de datos")
            End If
        End If

    End Sub

    'ENVIO DE CORREOS
    Private Const HOST_ADDRESS As String = "hermes.ssc.cl" 'XXXXXXX    "200.54.170.244"
    Private Const HOST_PORT As Integer = 465
    Private Const HOST_USERNAME As String = "plataforma.it@serviciosest.cl"
    Private Const HOST_PASSWORD As String = "Plat1t-*"

    Sub subSendMail(ByVal pUnidad As String, ByVal pUsuario As String, ByVal pSolicitud As String)

        Dim strEmailUsuaria As String = ""
        Dim strEmailEST As String = ""
        Dim strEmailUsuarioWeb As String = ""
        Dim strcorreoSupervisor As String = ""
        Dim idSolicitud As Integer = 0
        Dim archivoReferido As String = ""

        'TRAE CORREOS CORRESPONDIENTE A UNIDAD
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim SqlComando As New SqlCommand
                    SqlComando.Connection = conn
                    SqlComando.CommandType = CommandType.StoredProcedure
                    SqlComando.CommandText = "pr_diTraeCorreosUnidad"


                    'PARAMETROS IN
                    SqlComando.Parameters.Add("@pUnidad", SqlDbType.NVarChar, 4).Value = pUnidad
                    SqlComando.Parameters.Add("@pUsuario", SqlDbType.NVarChar, 50).Value = pUsuario

                    'PARAMETROS OUT
                    SqlComando.Parameters.Add("@sEmailUsuaria", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sEmailEst", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sEmailUsuarioWeb", SqlDbType.VarChar, 60).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@scorreoSupervisor", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sidSolicitud", SqlDbType.Int).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@archivoReferido", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output

                    SqlComando.ExecuteNonQuery()

                    strEmailUsuaria = SqlComando.Parameters("@sEmailUsuaria").Value.ToString
                    strEmailEST = SqlComando.Parameters("@sEmailEst").Value.ToString
                    strEmailUsuarioWeb = SqlComando.Parameters("@sEmailUsuarioWeb").Value.ToString
                    strcorreoSupervisor = SqlComando.Parameters("@scorreoSupervisor").Value.ToString
                    idSolicitud = SqlComando.Parameters("@sidSolicitud").Value
                    archivoReferido = SqlComando.Parameters("@archivoReferido").Value


                Catch mirror As Exception
                    ShowPopUpMsg("¡Ha ocurrido un Error al traer Correos de la Unidad!")
                    GrabaLog(Session("pubIdUsuario"), "TRAE_CORREOS", mirror.ToString)
                    Return
                End Try
            End Using

        End If

        Dim strEmailsTo As String = strEmailEST
        Dim strArray As Array = strEmailsTo.Split(";")

        Dim strEmailsCC As String = strEmailUsuaria
        If strEmailUsuarioWeb <> "" Then
            strEmailsCC = strEmailUsuarioWeb + ";" + strEmailsCC
        End If
        If strcorreoSupervisor <> "" Then
            strEmailsCC = strcorreoSupervisor + ";" + strEmailsCC
        End If

        Dim strArrayCC As Array = strEmailsCC.Split(";")


        Try

            Dim mail As New MimeMessage()
            mail.From.Add(New MailboxAddress("plataforma.it@serviciosest.cl"))

            For count = 0 To strArray.Length - 1
                mail.To.Add(New MailboxAddress(strArray(count)))
            Next

            For count2 = 0 To strArrayCC.Length - 1
                mail.Cc.Add(New MailboxAddress(strArrayCC(count2)))
            Next

            'mail.To.Add(New MailboxAddress("dario.irribarra@sopytec.cl"))
            Dim builder = New BodyBuilder
            Dim referido As String
            Dim datosReferido = ""
            If chkReferido.Checked = True Then
                referido = "Si"
                If txtNombreReferido.Text.Trim = "" Then
                    datosReferido = "Datos de  Referido " & vbTab + vbTab & ": S/A"
                Else
                    datosReferido = "Datos de  Referido " & vbTab + vbTab & ": " & txtNombreReferido.Text.Trim & ""
                End If
            Else
                referido = "No"
                datosReferido = ""
                archivoReferido = ""
            End If
            If Session("pModoPopupSolicitud") = "C" Then
                mail.Subject = "Se ha INGRESADO una solicitud Nro: " + pSolicitud + " - por Empresa: " + Session("pubEmpUsuariaUsuario")
                builder.TextBody = String.Format("Se ha procesado una solicitud a nombre de: " & Session("pubNombreUsuario") & " " + vbCrLf + vbCrLf +
                                          "Cantidad EST Requerido" & vbTab & ": " & spnCargaMasiva.Value & "" + vbCrLf +
                                          "Nombre de Planta" & vbTab + vbTab & ": " & cbxPlanta.SelectedItem.Text.Trim & "" + vbCrLf +
                                          "Centro de Costo" & vbTab + vbTab & ": " & cbxCentroCosto.Text.Trim & "" + vbCrLf +
                                          "Cargo" & vbTab + vbTab + vbTab + vbTab & ": " & txtCargo.Text.Trim & "" + vbCrLf +
                                          "Area" & vbTab + vbTab + vbTab + vbTab & ": " & txtArea.Text.Trim & "" + vbCrLf +
                                          "Causal" & vbTab + vbTab + vbTab + vbTab & ": " & cbxCausal.Value & "" + vbCrLf +
                                          "Motivo de Causal" & vbTab + vbTab & ": " & txtMotivoCausal.Text.Trim & "" + vbCrLf +
                                          "Fecha de Inicio" & vbTab + vbTab + vbTab & ": " & dtFechaDesde.Date.ToString("yyyy-MM-dd") & "" + vbCrLf +
                                          "Días" & vbTab + vbTab + vbTab + vbTab & ": " & txtDias.Text & "" + vbCrLf +
                                          "Fecha de Término" & vbTab + vbTab & ": " & dtFechaHasta.Date.ToString("yyyy-MM-dd") & "" + vbCrLf +
                                          "Sueldo" & vbTab + vbTab + vbTab + vbTab & ": $" & txtSuedoBase.Text.Trim & "" + vbCrLf +
                                          "Turno" & vbTab + vbTab + vbTab + vbTab & ": " & txtTurno.Text.Trim & "" + vbCrLf +
                                          "Jefe a Cargo" & vbTab + vbTab + vbTab & ": " & txtJefeACargo.Text.Trim & "" + vbCrLf +
                                          "Referido" & vbTab + vbTab + vbTab & ": " & referido & "" + vbCrLf +
                                          datosReferido + vbCrLf + vbCrLf +
                                          "Favor coordinar a la brevedad")
            End If

            If Session("pModoPopupSolicitud") = "M" Then
                mail.Subject = "Se ha MODIFICADO la solicitud Nro: " + pSolicitud + " - por Empresa: " + Session("pubEmpUsuariaUsuario")
                builder.TextBody = String.Format("Se ha procesado una MODIFICACIÓN de solicitud a nombre de: " & Session("pubNombreUsuario") & " " + vbCrLf + vbCrLf +
                                          "Cantidad EST Requerido" & vbTab & ": " & spnCargaMasiva.Value & "" + vbCrLf +
                                          "Nombre de Planta" & vbTab + vbTab & ": " & cbxPlanta.SelectedItem.Text.Trim & "" + vbCrLf +
                                          "Centro de Costo" & vbTab + vbTab & ": " & cbxCentroCosto.Text.Trim & "" + vbCrLf +
                                          "Cargo" & vbTab + vbTab + vbTab + vbTab & ": " & txtCargo.Text.Trim & "" + vbCrLf +
                                          "Area" & vbTab + vbTab + vbTab + vbTab & ": " & txtArea.Text.Trim & "" + vbCrLf +
                                          "Causal" & vbTab + vbTab + vbTab + vbTab & ": " & cbxCausal.Value & "" + vbCrLf +
                                          "Motivo de Causal" & vbTab + vbTab & ": " & txtMotivoCausal.Text.Trim & "" + vbCrLf +
                                          "Fecha de Inicio" & vbTab + vbTab + vbTab & ": " & dtFechaDesde.Date.ToString("yyyy-MM-dd") & "" + vbCrLf +
                                          "Días" & vbTab + vbTab + vbTab + vbTab & ": " & txtDias.Text & "" + vbCrLf +
                                          "Fecha de Término" & vbTab + vbTab & ": " & dtFechaHasta.Date.ToString("yyyy-MM-dd") & "" + vbCrLf +
                                          "Sueldo" & vbTab + vbTab + vbTab + vbTab & ": $" & txtSuedoBase.Text.Trim & "" + vbCrLf +
                                          "Turno" & vbTab + vbTab + vbTab + vbTab & ": " & txtTurno.Text.Trim & "" + vbCrLf +
                                          "Jefe a Cargo" & vbTab + vbTab + vbTab & ": " & txtJefeACargo.Text.Trim & "" + vbCrLf +
                                          "Referido" & vbTab + vbTab + vbTab & ": " & referido & "" + vbCrLf +
                                          datosReferido + vbCrLf + vbCrLf +
                                          "Favor revisar cambios y coordinar a la brevedad")
            End If

            Try
                Dim client = New SmtpClient()
                client.Connect(HOST_ADDRESS, HOST_PORT, True)
                client.Authenticate(HOST_USERNAME, HOST_PASSWORD)
                'ENVIO DE ATTACHMENT
                If archivoReferido <> "" Then
                    Try
                        'Dim adjunto = New Attachment(Server.MapPath(archivoReferido))
                        builder.Attachments.Add(Server.MapPath(archivoReferido)) 'attachment
                        mail.Body = builder.ToMessageBody
                        client.Send(mail)
                    Catch ex As Exception
                        client.Send(mail)
                    End Try
                Else
                    client.Send(mail)
                End If
                client.Disconnect(True)
            Catch ex As Exception
                ShowPopUpMsg(ex.ToString)
            End Try

        Catch ex As Exception
            ShowPopUpMsg(ex.ToString)
            GrabaLog(Session("pubIdUsuario"), "ENVIAR CORREOS", ex.ToString)
            Return
        End Try

    End Sub


    Protected Sub btnpopUp_Click(sender As Object, e As EventArgs)
        popUpCrearSolicitud.ShowOnPageLoad = True
        Form.DefaultButton = btnCrearSolicitud.UniqueID
        cbxPlanta.SelectedIndex = -1
        cbxCentroCosto.Text = Nothing
        txtArea.Text = Nothing
        txtCargo.Text = Nothing
        cbxCausal.SelectedIndex = -1
        txtMotivoCausal.Text = Nothing
        dtFechaDesde.Text = Nothing
        txtDias.Value = 1
        dtFechaHasta.Text = Nothing
        txtSuedoBase.Text = Nothing
        txtTurno.Text = Nothing
        txtJefeACargo.Text = Nothing
        chkReferido.Checked = False
        txtNombreReferido.Text = Nothing
        chkCargaMasiva.Checked = False
        spnCargaMasiva.Value = 1
        Session("pubFileName") = ""
        cbxPlanta.Focus()
        Session("pModoPopupSolicitud") = "C"
    End Sub

    Private Sub cargarExcel()
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA GRID CON ACCESO A TODOS LOS HISTORIALES
                    If Session("Administrador") = True Or Session("SuperAdmin") = True Or Session("Auditor") = True Or Session("Web") = True Then
                        If cbxUnidad.Value <> Nothing Then
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                            UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                            UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                            UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                            archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                            FROM diSolicitud
                            WHERE idUnidad = '{cbxUnidad.SelectedItem.Value}'
                            ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        Else
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                            UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                            UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                            UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                            archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                            FROM diSolicitud
                            ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        End If

                    End If
                    'CARGA GRID CON ACCESO A HISTORIALES DE LA PLANTA QUE ESTÁ A CARGO
                    If Session("Operaciones") = True Then
                        If cbxUnidad.SelectedItem.Value <> Nothing Then
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE idUnidad IN ({Session("pubUnidadesUsuario")}) AND idUnidad = '{cbxUnidad.SelectedItem.Value}'
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        Else
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE idUnidad IN ({Session("pubUnidadesUsuario")})
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        End If
                    End If
                    'CARGA GRID CON ACCESO A HISTORIAL DE SOLICITUDES QUE LA PERSONA HA REALIZADO
                    If Session("Cliente") = True Then
                        If cbxUnidad.SelectedItem.Value <> Nothing Then
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE nombreUsuarioSolicitud ='{Session("pubIdUsuario")}' AND idUnidad = '{cbxUnidad.SelectedItem.Value}'
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        Else
                            sqlStr = $"SELECT idSolicitud, numeroSolicitudes, UPPER(dbo.LimpiarCaracteres(nombreUsuarioSolicitud)) as nombreUsuarioSolicitud, idUnidad,
                        UPPER(dbo.LimpiarCaracteres(planta)) as planta, UPPER(dbo.LimpiarCaracteres(centroCosto)) as centroCosto, UPPER(dbo.LimpiarCaracteres(cargo)) as cargo,
                        UPPER(dbo.LimpiarCaracteres(causal)) as causal, UPPER(dbo.LimpiarCaracteres(motivoCausal)) as motivoCausal, fechaInicio, dias, fechaTermino, sueldo,
                        UPPER(dbo.LimpiarCaracteres(turno)) as turno, UPPER(dbo.LimpiarCaracteres(jefe)) as jefe, referido, UPPER(dbo.LimpiarCaracteres(datosReferido)) as datosReferido,
                        archivo, fechaCreacion, empUsuaria, UPPER(dbo.LimpiarCaracteres(area)) as area, cantRequerida
                        FROM diSolicitud
                        WHERE nombreUsuarioSolicitud = '" & Session("pubIdUsuario") & "'
                        ORDER BY idSolicitud"

                            Call spllenaGridView(gridSolicitudes, sqlStr)
                        End If
                    End If

                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs)
        Call cargarExcel()

        gridSolicitudes.Columns.Item(0).Caption = "CODIGO UNICO"
        gridSolicitudes.Columns.Item(1).Caption = "NUMERO DE SOLICITUDES"
        gridSolicitudes.Columns.Item(2).Caption = "EMPRESA USUARIA"
        gridSolicitudes.Columns.Item(3).Caption = "CANTIDAD EST SOLICITADO"
        gridSolicitudes.Columns.Item(4).Caption = "FECHA CREACION DE SOLICITUD"
        gridSolicitudes.Columns.Item(5).Caption = "UNIDAD"
        gridSolicitudes.Columns.Item(6).Caption = "PLANTA"
        gridSolicitudes.Columns.Item(7).Caption = "CENTRO COSTO"
        gridSolicitudes.Columns.Item(8).Caption = "AREA"
        gridSolicitudes.Columns.Item(9).Caption = "CARGO"
        gridSolicitudes.Columns.Item(10).Caption = "CAUSAL"
        gridSolicitudes.Columns.Item(11).Caption = "MOTIVO CAUSAL"
        gridSolicitudes.Columns.Item(12).Caption = "FECHA INICIO"
        gridSolicitudes.Columns.Item(13).Caption = "DIAS"
        gridSolicitudes.Columns.Item(14).Caption = "FECHA TERMINO"
        gridSolicitudes.Columns.Item(15).Caption = "SUELDO"
        gridSolicitudes.Columns.Item(16).Caption = "TURNO"
        gridSolicitudes.Columns.Item(17).Caption = "JEFE A CARGO"
        gridSolicitudes.Columns.Item(18).Caption = "REFERIDO"
        gridSolicitudes.Columns.Item(19).Caption = "DATOS REFERIDO"
        gridSolicitudes.Columns.Item(20).Caption = "ARCHIVO REFERIDO"
        gridSolicitudes.Columns.Item(21).Visible = False
        gridExport.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})

    End Sub
    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)
        Call cargarGridSolicitudes()
        gridSolicitudes.PageIndex = 0

    End Sub

    Protected Sub btnEditarDocumento_Click(sender As Object, e As EventArgs)

        If gridSolicitudes.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione una SOLICITUD y vuelva a clicar EDITAR")
            Return
        End If

        Dim btn As ASPxButton = TryCast(sender, ASPxButton)
        Dim container As GridViewDataItemTemplateContainer = TryCast(btn.NamingContainer, GridViewDataItemTemplateContainer)
        Dim prueba = gridSolicitudes.GetRowValues(container.VisibleIndex, "idSolicitud").ToString()
        'btn.ClientSideEvents.Click = String.Format("function(s, e) {{cbShowCheckoutDetail({0})}}", container.VisibleIndex)

        'TRAER CODIGO
        Dim pCodigo As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "idSolicitud").ToString()
        Dim pPlanta As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "planta").ToString()
        Dim pCentro As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "centroCosto").ToString()
        Dim pArea As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "area").ToString()
        Dim pCargo As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "cargo").ToString()
        Dim pCausal As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "causal").ToString()
        Dim pMotivo As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "motivoCausal").ToString()
        'Dim pMasiva As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "codigo").ToString()
        Dim pCantidad = gridSolicitudes.GetRowValues(container.VisibleIndex, "cantRequerida")
        Dim pFechaDesde As Date = gridSolicitudes.GetRowValues(container.VisibleIndex, "fechaInicio")
        Dim pFechaHasta As Date = gridSolicitudes.GetRowValues(container.VisibleIndex, "fechaTermino")
        Dim pDias As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "dias").ToString()
        Dim pSueldo As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "sueldo").ToString()
        Dim pJornada As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "turno").ToString()
        Dim pJefe As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "jefe").ToString()
        Dim pReferido As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "referido").ToString()
        Dim pDatosReferido As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "datosReferido").ToString()
        Dim pArchivo As String = gridSolicitudes.GetRowValues(container.VisibleIndex, "archivo").ToString()

        cbxPlanta.Text = pPlanta
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA DE CAUSALES
                    sqlStr = $"SELECT codUnidad, descCCosto
                    FROM ccCentroCosto 
                    WHERE codUnidad = '{cbxPlanta.Value}' OR codUnidad = 'AAAA'"

                    Call spLlenaComboBoxPopUp(sqlStr, cbxCentroCosto, "codUnidad", "descCCosto")
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If
        cbxCentroCosto.Text = pCentro
        txtArea.Text = pArea
        txtCargo.Text = pCargo
        cbxCausal.Text = pCausal
        txtMotivoCausal.Text = pMotivo
        If pCantidad >= 2 Then
            chkCargaMasiva.Checked = True
            td_CargaMasiva.Attributes("style") = "visibility : visible;"
        Else
            chkCargaMasiva.Checked = False
            td_CargaMasiva.Attributes("style") = "visibility: hidden;"
        End If
        spnCargaMasiva.Value = pCantidad
        dtFechaDesde.Value = pFechaDesde
        txtDias.Value = pDias
        dtFechaHasta.Value = pFechaHasta
        txtSuedoBase.Text = pSueldo.Remove(0, 1)
        txtTurno.Text = pJornada
        txtJefeACargo.Text = pJefe
        If pReferido = "SI" Then
            chkReferido.Checked = True
            txtNombreReferido.Text = pDatosReferido
            Session("pubFileName") = pArchivo
            td_NombreReferido.Attributes("style") = "visibility : visible;"
            td_txtArchivoCargado.Attributes("style") = "visibility : visible;"
            td_UploadControl.Attributes("style") = "visibility : visible;"
            lblNombreReferido.Attributes("style") = "visibility : visible;"
        Else
            chkReferido.Checked = False
            txtNombreReferido.Text = pDatosReferido
            Session("pubFileName") = pArchivo
            td_NombreReferido.Attributes("style") = "visibility : hidden;"
            td_txtArchivoCargado.Attributes("style") = "visibility : hidden;"
            td_UploadControl.Attributes("style") = "visibility : hidden;"
            lblNombreReferido.Attributes("style") = "visibility : hidden;"
        End If
        cbxPlanta.Focus()

        gridSolicitudes.FocusedRowIndex = container.VisibleIndex

        popUpCrearSolicitud.ShowOnPageLoad = True
        Form.DefaultButton = btnCrearSolicitud.UniqueID
        Session("pModoPopupSolicitud") = "M"
    End Sub

    Protected Sub btnEditarDocumento_Load(sender As Object, e As EventArgs)
        Dim btnEditarDocumento As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = btnEditarDocumento.NamingContainer
        Dim pCodigo As String = ""
        'Dim fechaAumentada As DateTime
        pCodigo = gridSolicitudes.GetRowValues(container.ItemIndex, "idSolicitud")
        Dim fechaCreacion As DateTime
        sqlStr = $" SELECT fechaCreacion 
                                FROM diSolicitud
                                WHERE idSolicitud = {pCodigo}"
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    fechaCreacion = sqlcmd.ExecuteScalar()
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If
        'fechaAumentada = fechaCreacion.AddHours(1)
        If Date.Now >= fechaCreacion.AddHours(1) Then
            btnEditarDocumento.Enabled = False
            btnEditarDocumento.Image.Url = "Images/candado2.png"
        Else
            btnEditarDocumento.Enabled = True
            btnEditarDocumento.Image.Url = "images/39.png"
        End If
    End Sub

    Protected Sub cbpCentroCosto_Callback(sender As Object, e As CallbackEventArgsBase)
        cbxCentroCosto.Value = Nothing
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA DE CAUSALES
                    sqlStr = $"SELECT codUnidad, descCCosto
                    FROM ccCentroCosto 
                    WHERE codUnidad = '{cbxPlanta.Value}' OR codUnidad = 'AAAA'"

                    Call spLlenaComboBoxPopUp(sqlStr, cbxCentroCosto, "codUnidad", "descCCosto")
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If
    End Sub

    Protected Sub gridSolicitudes_PageIndexChanged(sender As Object, e As EventArgs)
        Call cargarGridSolicitudes()
    End Sub

    Protected Sub gridSolicitudes_BeforeGetCallbackResult(sender As Object, e As EventArgs)
        Call cargarGridSolicitudes()
    End Sub
End Class