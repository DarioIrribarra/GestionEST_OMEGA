Imports System.Data.SqlClient
Imports System.IO
Imports DevExpress.Web

Public Class pag_empresa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'BUSQUEDA DE REPORTES
        Session("BusquedaRealizada") = 0

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

        Page.Form.DefaultButton = btnConsultar.UniqueID
        gridEmpresa.Visible = False
        lblDefinicion.Visible = False

        If IsPostBack Then
            gridEmpresa.Visible = True
            If gridEmpresa.VisibleRowCount > 0 Then
                lblDefinicion.Visible = True
            Else
                lblDefinicion.Visible = False
            End If
        End If

    End Sub

    Private Sub empresa_Init(sender As Object, e As EventArgs) Handles Me.Init

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

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = $"SELECT id as idUnidad,UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE id IN ({Session("pubUnidadesUsuario")}) order by descripcion"
                    Call spLlenaComboBox(sqlStr, cbxPlanta, "idUnidad", "descripcion")

                    'CARGAR EMPRESA
                    Dim sqlCombo = $"SELECT DISTINCT id, descripcion from ccEmpUsuaria WHERE id <> 'TMPO'"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresaPagina, "id", "descripcion")

                    'CARGA EMPRESA CONTRATO
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresaContrato, "id", "descripcion")

                    'CARGA POPUPS
                    Dim sqlStrUnidad = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccViewEmpleados WHERE id IN (" & Session("pubUnidadesUsuario") & ") ORDER BY descripcion"
                    Call spLlenaComboBoxPopUp(sqlStrUnidad, cbxUnidadCarga, "idUnidad", "descripcion")
                    Call spLlenaComboBoxFecha("SELECT id as idMes, UPPER(descripcion) as descripcion from ccMeses", txtMesEmpresa, "idMes", "descripcion")
                    Call spLlenaComboBoxFecha("SELECT vano as idYear, vano as descripcion FROM ccAnos WHERE vano >= 2018", txtAnoEmpresa, "idYear", "descripcion")
                    'CARGAR FECHAS CONTRATO
                    Call spLlenaComboBoxFecha("SELECT id as idMes, UPPER(descripcion) as descripcion from ccMeses", cbxMesContrato, "idMes", "descripcion")
                    Call spLlenaComboBoxFecha("SELECT vano as idYear, vano as descripcion FROM ccAnos WHERE vano >= 2018", cbxAñoContrato, "idYear", "descripcion")

                Catch ex As Exception

                End Try
            End Using
        End If
        cbxPlanta.NullText = "SELECCIONE UNIDAD"
        txtMesEmpresa.SelectedIndex = DateTime.Now.Month - 1
        txtAnoEmpresa.SelectedIndex = DateTime.Now.Year - 2018


        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Then
            btnSubeArchivo.Visible = True
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = "SELECT id, descripcion from ccTipoDocto WHERE clasif=0"
                        Call spLlenaComboBoxPopUp(sqlStr, txtTipoDocto, "id", "descripcion")
                        gridEmpresa.SettingsBehavior.AllowFocusedRow = True

                    Catch ex As Exception

                    End Try
                End Using
            End If
        Else
            tb_BotonesCargar.Visible = False
            btnSubeArchivo.Visible = False
            gridEmpresa.SettingsBehavior.AllowFocusedRow = False
        End If

        'CARGAR UNIDADES DE EMPRESA SOLO PARA PERFILES CORRECTOS
        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
            lblEmpresa.Visible = True
            cbxEmpresaPagina.Visible = True
            'CARGO LA UNIDAD INICIAL
            If IsPostBack = False Then
                cbxEmpresaPagina.Value = Session("pubEmpUsuariaUsuario")

                'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                            Call spLlenaComboBox(sqlCombo, cbxPlanta, "idUnidad", "descripcion")
                        Catch ex As Exception
                            ShowPopUpMsg("Error 1: {0}")
                        End Try
                    End Using
                End If
            End If
        Else
            lblEmpresa.Visible = False
            cbxEmpresaPagina.Visible = False
        End If

    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Protected Sub ASPxHyperLink1_Load(sender As Object, e As EventArgs)
        Dim link As ASPxHyperLink = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer
        Dim myValue As String = gridEmpresa.GetRowValues(container.ItemIndex, container.Column.FieldName)

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


        Dim name As String = e.UploadedFile.FileName
        Dim url As String = ResolveClientUrl(resultFileUrl)
        Dim sizeInKilobytes As Long = e.UploadedFile.ContentLength / 1024
        Dim sizeText As String = sizeInKilobytes.ToString() & " KB"


        Dim uploadControl As ASPxUploadControl = TryCast(sender, ASPxUploadControl)

        If uploadControl.UploadedFiles IsNot Nothing AndAlso uploadControl.UploadedFiles.Length > 0 Then
            For i As Integer = 0 To uploadControl.UploadedFiles.Length - 1
                Dim file As UploadedFile = uploadControl.UploadedFiles(i)
                If file.FileName <> "" Then
                    Session("pubFileName") = Trim(txtIdUnidad.Text) + "-" + txtTipoDocto.Value + "-" + Now().ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName)
                    Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                    file.SaveAs(fileName, True)

                End If
            Next i
        End If
    End Sub

    Protected Sub uploadControlContrato_FileUploadComplete(sender As Object, e As FileUploadCompleteEventArgs)
        Dim resultExtension As String = Path.GetExtension(e.UploadedFile.FileName)
        Dim resultFileName As String = Path.ChangeExtension(Path.GetRandomFileName(), resultExtension)
        Dim resultFileUrl As String = TempDirectory & resultFileName
        Dim resultFilePath As String = MapPath(resultFileUrl)


        Dim name As String = e.UploadedFile.FileName
        Dim url As String = ResolveClientUrl(resultFileUrl)
        Dim sizeInKilobytes As Long = e.UploadedFile.ContentLength / 1024
        Dim sizeText As String = sizeInKilobytes.ToString() & " KB"


        Dim uploadControl As ASPxUploadControl = TryCast(sender, ASPxUploadControl)

        If uploadControl.UploadedFiles IsNot Nothing AndAlso uploadControl.UploadedFiles.Length > 0 Then
            For i As Integer = 0 To uploadControl.UploadedFiles.Length - 1
                Dim file As UploadedFile = uploadControl.UploadedFiles(i)
                If file.FileName <> "" Then
                    Session("pubFileNameContrato") = "CONTRATO-" + cbxEmpresaContrato.Text + "-" + txtTipoDocto.Value + "-" + Now().ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName)
                    Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileNameContrato"))
                    file.SaveAs(fileName, True)

                End If
            Next i
        End If

    End Sub

    Private Sub CargarGridEmpresa()
        'GRID PARA ADMINSTRADOR, WEB Y AUDITOR
        Try
            If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                If cbxPlanta.Value = Nothing Then
                    sqlStr = "SELECT ccunidades.descripcion as centro, cotiza, asiste, cerdeu, finpag, form29, form30, librem, sueldo, unidad, contrato FROM ccDocEmpresa INNER JOIN ccUnidades ON ccDocEmpresa.unidad=ccUNidades.id " &
                          "WHERE ccUnidades.idEmpUsuaria IN ('" & cbxEmpresaPagina.Value & "')" &
                           " And AMES ='" & txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString & "'"
                    spllenaGridView(gridEmpresa, sqlStr)
                Else
                    sqlStr = "SELECT ccunidades.descripcion as centro, cotiza, asiste, cerdeu, finpag, form29, form30, librem, sueldo, unidad, contrato FROM ccDocEmpresa INNER JOIN ccUnidades ON ccDocEmpresa.unidad=ccUNidades.id " &
                      "WHERE ccUnidades.idEmpUsuaria IN ('" & cbxEmpresaPagina.Value & "') AND unidad = '" & cbxPlanta.Value.ToString & "' " & " 
                        And AMES ='" & txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString & "'"
                    spllenaGridView(gridEmpresa, sqlStr)
                End If

            Else
                If cbxPlanta.Value = Nothing Then
                    sqlStr = "SELECT ccunidades.descripcion as centro, cotiza, asiste, cerdeu, finpag, form29, form30, librem, sueldo, unidad, contrato FROM ccDocEmpresa INNER JOIN ccUnidades ON ccDocEmpresa.unidad=ccUNidades.id " &
                          "WHERE unidad IN (" & Session("pubUnidadesUsuario") & ")" &
                           " And AMES ='" & txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString & "'"
                    spllenaGridView(gridEmpresa, sqlStr)
                Else
                    sqlStr = "SELECT ccunidades.descripcion as centro, cotiza, asiste, cerdeu, finpag, form29, form30, librem, sueldo, unidad, contrato FROM ccDocEmpresa INNER JOIN ccUnidades ON ccDocEmpresa.unidad=ccUNidades.id " &
                      "WHERE unidad IN (" & Session("pubUnidadesUsuario") & ") AND unidad = '" & cbxPlanta.Value.ToString & "' " & " 
                        And AMES ='" & txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString & "'"
                    spllenaGridView(gridEmpresa, sqlStr)
                End If

            End If
        Catch ex As Exception
            ShowPopUpMsg("Error CargarGridEmpresa: " + ex.ToString())
        End Try


    End Sub

    Protected Sub btnGrabarArchivo_Click(sender As Object, e As EventArgs)
        Dim sqlStrArchivo As String

        If Session("pubFileName") <> "" Then
            Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
            IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
            IO.File.Delete(fileName)
        Else
            ShowPopUpMsg("Favor cargue archivo")
            Return
        End If

        'AGREGAR A BASE DE DATOS
        If ConectaSQLServer() Then
            Using conn
                Try
                    'COMPROBAR QUE LA FECHA Y UNIDAD EXISTAN
                    sqlStr = "SELECT ames 
                        FROM ccDocEmpresa
                        WHERE ames = @ames 
                        AND unidad=@unidad"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.Parameters.Add(New SqlParameter("@ames", txtAnoMes.Text))
                    sqlcmd.Parameters.Add(New SqlParameter("@unidad", txtIdUnidad.Text))

                    Dim reader As IDataReader = sqlcmd.ExecuteReader
                    'SI EXISTEN SE ACTUALIZAN CON ARCHIVOS
                    If reader.Read() Then
                        sqlStrArchivo = String.Format("UPDATE ccDocEmpresa SET {0}=@archivo WHERE unidad=@unidad AND ames=@ames", LCase(txtTipoDocto.Value))
                        Dim sqlcmdArchivo As New SqlCommand(sqlStrArchivo, conn)
                        sqlcmdArchivo.Parameters.Add(New SqlParameter("@unidad", txtIdUnidad.Text))
                        sqlcmdArchivo.Parameters.Add(New SqlParameter("@ames", txtAnoMes.Text))
                        sqlcmdArchivo.Parameters.Add(New SqlParameter("@archivo", "~/SCAN/" + Session("pubFileName")))
                        reader.Close()
                        sqlcmdArchivo.ExecuteNonQuery()

                        'SI NO EXISTEN SE INSERTAN ARCHIVOS, UNIDAD Y FECHA
                    Else
                        sqlStrArchivo = String.Format("INSERT INTO ccDocEmpresa ({0}, unidad, ames)
                                                VALUES (@archivo, @unidad, @ames)", LCase(txtTipoDocto.Value))
                        Dim sqlcmdArchivo As New SqlCommand(sqlStrArchivo, conn)
                        sqlcmdArchivo.Parameters.Add(New SqlParameter("@unidad", txtIdUnidad.Text))
                        sqlcmdArchivo.Parameters.Add(New SqlParameter("@ames", txtAnoMes.Text))
                        sqlcmdArchivo.Parameters.Add(New SqlParameter("@archivo", "~/SCAN/" + Session("pubFileName")))
                        reader.Close()
                        sqlcmdArchivo.ExecuteNonQuery()
                    End If
                Catch ex As Exception

                End Try


            End Using
        Else

            ShowPopUpMsg("No hay conexion a base datos")

        End If

        ShowPopUpMsg("!Archivo Cargado Exitosamente¡")

        '********* REFRESCAR GRID
        Call CargarGridEmpresa()

        '***********************

        Session("pubFileName") = ""
        txtArchivoCargado.Text = "Falta Cargar Archivo"
        popupCargaArchivo.ShowOnPageLoad = True
        Form.DefaultButton = Nothing
    End Sub

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)

        Call CargarGridEmpresa()

        gridEmpresa.Visible = True
        If gridEmpresa.VisibleRowCount > 0 Then
            lblDefinicion.Visible = True
        Else
            lblDefinicion.Visible = False
        End If
    End Sub

    Protected Sub btnSubeArchivo_Click(sender As Object, e As EventArgs)
        'txtUnidad.Text = gridEmpresa.GetRowValues(gridEmpresa.FocusedRowIndex, "centro")
        txtIdUnidad.NullText = "SELECCIONE UNIDAD"
        If cbxPlanta.SelectedIndex > 1 Then
            cbxUnidadCarga.SelectedIndex = cbxPlanta.SelectedIndex
        Else
            cbxUnidadCarga.SelectedIndex = -1
        End If

        txtAnoMes.Text = txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString
        'txtIdUnidad.Text = gridEmpresa.GetRowValues(gridEmpresa.FocusedRowIndex, "unidad")

        If gridEmpresa.FocusedRowIndex >= 0 Then
            cbxUnidadCarga.Value = gridEmpresa.GetRowValues(gridEmpresa.FocusedRowIndex, "unidad")
            txtIdUnidad.Text = gridEmpresa.GetRowValues(gridEmpresa.FocusedRowIndex, "unidad")
        End If

        If cbxUnidadCarga.Value = Nothing Then
            txtIdUnidad.Value = Nothing
            txtIdUnidad.Text = ""

        Else
            txtIdUnidad.Text = cbxUnidadCarga.Value.ToString
        End If

        'txtTipoDocto.Value = "ASISTE"
        txtTipoDocto.SelectedIndex = -1
        txtArchivoCargado.Text = "Falta Cargar Archivo"
        Session("pubFileName") = ""
        popupCargaArchivo.ShowOnPageLoad = True
        Form.DefaultButton = btnGrabarArchivo.UniqueID
    End Sub

    Protected Sub cbxUnidadCarga_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cbxUnidadCarga.Value = Nothing Then
            txtIdUnidad.Value = Nothing
            txtIdUnidad.Text = ""
        Else
            txtIdUnidad.Text = cbxUnidadCarga.Value.ToString
        End If
    End Sub

    Protected Sub cbpPanelIzquierdo_Callback(sender As Object, e As CallbackEventArgsBase)
        cbxPlanta.Value = Nothing
        Session("pubEmpUsuariaUsuario") = cbxEmpresaPagina.Value
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA COMBOBOX
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxPlanta, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}")
                End Try
            End Using
        End If
    End Sub

    Protected Sub cbxPlanta_Load(sender As Object, e As EventArgs)
        'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                        Call spLlenaComboBox(sqlCombo, cbxPlanta, "idUnidad", "descripcion")
                    Catch ex As Exception
                        ShowPopUpMsg("Error 1: {0}")
                    End Try
                End Using
            End If
        End If

    End Sub

    Protected Sub cbpEmpresa_Callback(sender As Object, e As CallbackEventArgsBase)
        'NO ES NECESARIO CARGAR LA GRILLA ACA
        'Call CargarGridEmpresa()
    End Sub

    Protected Sub btnSubirContrato_Click(sender As Object, e As EventArgs)
        'Conexión para consultar si existe el contrato
        cbxEmpresaContrato.Value = Session("pubEmpUsuariaUsuario")
        cbxMesContrato.SelectedIndex = DateTime.Now.Month - 1
        cbxAñoContrato.SelectedIndex = DateTime.Now.Year - 2018
        Dim contrato = ""
        Try
            If ConectaSQLServer() Then
                sqlStr = $"  SELECT contrato 
                            FROM ccDocEmpresa
                            INNER JOIN ccUnidades 
                            ON ccDocEmpresa.unidad = ccUnidades.id
                            WHERE ccUnidades.idEmpUsuaria = '{Session("pubEmpUsuariaUsuario")}' AND ames ={cbxAñoContrato.Value + cbxMesContrato.Value}"
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    contrato = sqlcmd.ExecuteScalar()
                End Using

            End If
        Catch ex As Exception
            ShowPopUpMsg(ex.ToString)
        End Try

        If contrato = "" Then
            'hlVerArchivo.NavigateUrl = filename
            'hlVerArchivo.Target = "_blank"
            hlVerArchivo.Text = "¡NO EXISTE CONTRATO EN ESTA FECHA!"
            hlVerArchivo.Visible = True
        Else
            hlVerArchivo.NavigateUrl = contrato
            hlVerArchivo.Target = "_blank"
            hlVerArchivo.Text = "Haga Click AQUÍ para ver contrato en Base de Datos"
            hlVerArchivo.Visible = True
        End If

        txtContrato.Text = "Falta Cargar Contrato"
        popUpContrato.ShowOnPageLoad = True
        Form.DefaultButton = btnGuardarContrato.UniqueID
    End Sub

    Protected Sub btnGuardarContrato_Click(sender As Object, e As EventArgs)
        If Session("pubFileNameContrato") <> "" Then
            Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileNameContrato"))
            IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
            IO.File.Delete(fileName)
        Else
            ShowPopUpMsg("Favor cargue archivo")
            Return
        End If

        Try
            Dim ruta As String = "~/SCAN/" + Session("pubFileNameContrato")
            'AGREGAR CONTRATO
            sqlStr = $"UPDATE ccDocEmpresa Set contrato = '{ruta}' WHERE unidad IN (SELECT distinct unidad 
                    FROM ccDocEmpresa
                    INNER JOIN ccUnidades 
                    ON ccDocEmpresa.unidad = ccUnidades.id
                    WHERE ccUnidades.idEmpUsuaria = '{cbxEmpresaContrato.Value}') AND ames >= {cbxAñoContrato.Value.ToString & cbxMesContrato.Value.ToString}"

            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd = New SqlCommand(sqlStr, conn)
                    sqlcmd.ExecuteNonQuery()
                End Using
            End If

        Catch ex As Exception
            ShowPopUpMsg("Error al registrar contrato Actualizado")
            ShowPopUpMsg(ex.ToString())
            Return
        End Try
        Session("pubFileNameContrato") = ""
        popUpContrato.ShowOnPageLoad = False
    End Sub

    Protected Sub cbpContrato_Callback(sender As Object, e As CallbackEventArgsBase)
        Dim contrato = ""
        Try
            If ConectaSQLServer() Then
                sqlStr = $"  SELECT contrato 
                            FROM ccDocEmpresa
                            INNER JOIN ccUnidades 
                            ON ccDocEmpresa.unidad = ccUnidades.id
                            WHERE ccUnidades.idEmpUsuaria = '{cbxEmpresaContrato.Value}' AND ames ={cbxAñoContrato.Value + cbxMesContrato.Value}"
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    contrato = sqlcmd.ExecuteScalar()
                End Using

            End If
        Catch ex As Exception
            ShowPopUpMsg(ex.ToString)
        End Try

        If contrato = "" Then
            hlVerArchivo.Text = "¡NO EXISTE CONTRATO EN ESTA FECHA!"
            hlVerArchivo.Visible = True
        Else
            hlVerArchivo.NavigateUrl = contrato
            hlVerArchivo.Target = "_blank"
            hlVerArchivo.Text = "Haga Click AQUÍ para ver contrato en Base de Datos"
            hlVerArchivo.Visible = True
        End If
    End Sub
End Class