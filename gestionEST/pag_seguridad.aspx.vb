Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Windows.Forms
Imports DevExpress.Export
Imports DevExpress.Web
Imports DevExpress.XtraPrinting
Imports System.Data
Imports System.Configuration

Public Class pag_seguridad
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



        Form.DefaultButton = Nothing
        'Call cargarGridUsuarios()

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
            image.Src = "images/10.png"
            imagenCambioClave.ImageUrl = "images/10.png"
        Else
            image.Src = datosimagen
            imagenCambioClave.ImageUrl = datosimagen
            imagenCambioClave.Height = 50
            imagenCambioClave.Width = 80
        End If
    End Sub

    Private Sub pag_seguridad_Init(sender As Object, e As EventArgs) Handles Me.Init

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

        Me.Form.DefaultButton = Nothing

        '---------PAGINA USUARIOS---------
        Call cargarGridUsuarios()
        'PÁGINA VISIBLE PARA USUARIOS CON PERMISOS ADMINISTRADOR Y WEB

        If Session("Administrador") = True Or Session("Web") = True Or Session("SuperAdmin") = True Then
            'If Session("Administrador") = True Or Session("Auditor") = True Or Session("Web") = True Or Session("SuperAdmin") = True Then
            menuSeguridad.Items.FindByName("usuarios").Visible = True
        Else
            menuSeguridad.Items.FindByName("usuarios").Visible = False
        End If

        '--------------------------------------PAGINA USUARIOS'--------------------------------------

        '--------------------------------------PAGINA CAMBIO DE CLAVE'--------------------------------------
        'SI VIENE DESDE EL BOTÓN CAMBIAR CLAVE
        If Session("CambiarClave") = 1 Then
            menuSeguridad.Items.FindByName("clave").Selected = True
            div_clave.Visible = True
            Me.Form.DefaultButton = btnCambiar.UniqueID
            Session("CambiarClave") = 0
        Else
            menuSeguridad.Items.FindByName("clave").Selected = False
            div_clave.Visible = False
            Me.Form.DefaultButton = Nothing
        End If


        Page.Form.DefaultButton = btnCambiar.UniqueID
        tr_rut.Visible = False
        lblNombreUsuario.InnerText = Session("pubNombreUsuario")
        lblNombreEmpresa.InnerText = Session("pubEmpUsuariaUsuario")

        txtAnterior.Text = Nothing
        txtNueva.Text = Nothing
        txtNuevaRe.Text = Nothing
        txtAnterior.Focus()
        '--------------------------------------PAGINA CAMBIO DE CLAVE'--------------------------------------
    End Sub
    'ACCESO A DISTINTAS OPCIONES DE LA PAGINA Y MENU
    Protected Sub menuSeguridad_ItemClick(source As Object, e As MenuItemEventArgs)
        If e.Item.Name = "perfil" Then
            div_perfil.Visible = True
            Call CargarDatosPerfil()
            Me.Form.DefaultButton = btnAgregarDatosPerfil.UniqueID
        Else
            div_perfil.Visible = False
            Me.Form.DefaultButton = Nothing

        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    If e.Item.Name = "clave" Then
                        div_clave.Visible = True
                        Me.Form.DefaultButton = btnCambiar.UniqueID
                    Else
                        div_clave.Visible = False
                        Me.Form.DefaultButton = Nothing
                    End If
                Catch ex As Exception

                End Try
            End Using
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    If e.Item.Name = "usuarios" Then
                        gridUsuariosWeb.FilterExpression = Nothing
                        gridUsuariosWeb.ClearSort()
                        div_usuarios.Visible = True
                        btnPopUpUsuarios.Text = "Crear Usuario"
                        tr_popupUsuarios.Visible = True
                        tr_popupModificarUsuario.Visible = True
                        Me.Form.DefaultButton = btnPopUpUsuarios.UniqueID
                    Else
                        div_usuarios.Visible = False
                        tr_popupUsuarios.Visible = False
                        tr_popupModificarUsuario.Visible = False
                        Me.Form.DefaultButton = Nothing
                    End If
                Catch ex As Exception

                End Try
            End Using
        End If



    End Sub
    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    '--------------------------------------------------------------------PAGINA PERFIL -----------------------------------------------
    Sub CargarDatosPerfil()

        If ConectaSQLServer() Then
            Using conn
                Dim SqlComando As New SqlCommand
                Try
                    SqlComando.Connection = conn
                    SqlComando.CommandType = CommandType.StoredProcedure
                    SqlComando.CommandText = "pr_diTraeDatosUsuarioWeb"


                    'PARAMETROS IN
                    SqlComando.Parameters.Add("@pUsuario", SqlDbType.NVarChar, 50).Value = Session("pubIdUsuario")

                    'PARAMETROS OUT
                    SqlComando.Parameters.Add("@sNombre", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sEmail", SqlDbType.VarChar, 60).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sTelefono", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sDireccionWebEmpresa", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output
                    SqlComando.Parameters.Add("@sDireccionEmpresa", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output

                    SqlComando.ExecuteNonQuery()

                    'VALORES A COMPLETAR DEL PERFIL
                    txtNombrePerfil.Text = SqlComando.Parameters("@sNombre").Value.ToString
                    txtEmailPerfil.Text = SqlComando.Parameters("@sEmail").Value.ToString
                    If IsDBNull(SqlComando.Parameters("@sTelefono").Value.ToString) Then
                        txtTelefonoPerfil.Text = ""
                    Else
                        txtTelefonoPerfil.Text = SqlComando.Parameters("@sTelefono").Value.ToString
                    End If

                    If IsDBNull(SqlComando.Parameters("@sDireccionWebEmpresa").Value.ToString) Then
                        txtSitioWebEmpresaPerfil.Text = ""
                    Else
                        txtSitioWebEmpresaPerfil.Text = SqlComando.Parameters("@sDireccionWebEmpresa").Value.ToString
                    End If

                    If IsDBNull(SqlComando.Parameters("@sDireccionEmpresa").Value) Then
                        txtDireccionEmpresaPerfil.Text = ""
                    Else
                        txtDireccionEmpresaPerfil.Text = SqlComando.Parameters("@sDireccionEmpresa").Value
                    End If

                Catch mirror As Exception
                    ShowPopUpMsg("¡Ha ocurrido un Error al traer Correos de la Unidad!")
                    GrabaLog(Session("pubIdUsuario"), "TRAE_CORREOS", mirror.ToString)
                    Return
                End Try
            End Using

        End If
    End Sub

    'AGREGAR IMAGEN
    Protected Sub ASPxUploadControl1_FileUploadComplete1(sender As Object, e As FileUploadCompleteEventArgs)
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim uploadControl As ASPxUploadControl = TryCast(sender, ASPxUploadControl)
                    If uploadControl.UploadedFiles IsNot Nothing AndAlso uploadControl.UploadedFiles.Length > 0 Then
                        Session("imagen") = String.Format("Picture{0}.jpg", DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss"))
                        e.CallbackData = "Temp/" + Session("imagen")
                        Dim path As String = Page.MapPath("~/") + e.CallbackData
                        e.UploadedFile.SaveAs(path)
                    End If
                Catch ex As Exception

                End Try
            End Using
        End If
    End Sub

    'BOTON AGREGAR DATOS PERFIL
    Protected Sub btnAgregarDatosPerfil_Click(sender As Object, e As EventArgs)

        If Session("imagen") <> "" Then
            Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("imagen"))
            IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
            IO.File.Delete(fileName)
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    If Session("imagen") IsNot Nothing Then
                        sqlStr = "UPDATE ccUsuarioWeb
                            SET nombre = @nombre, email = @email, telefono = @telefono, direccionwebempresa = @direccionwebempresa, direccionempresa = @direccionempresa, imagen = @imagen
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"

                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.Parameters.Add(New SqlParameter("@nombre", txtNombrePerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@email", txtEmailPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@telefono", txtTelefonoPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@direccionwebempresa", txtSitioWebEmpresaPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@direccionempresa", txtDireccionEmpresaPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@imagen", "~/SCAN/" + Session("imagen")))
                        sqlcmd.ExecuteNonQuery()
                    Else
                        sqlStr = "UPDATE ccUsuarioWeb
                            SET nombre = @nombre, email = @email, telefono = @telefono, direccionwebempresa = @direccionwebempresa, direccionempresa = @direccionempresa
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"

                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.Parameters.Add(New SqlParameter("@nombre", txtNombrePerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@email", txtEmailPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@telefono", txtTelefonoPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@direccionwebempresa", txtSitioWebEmpresaPerfil.Value))
                        sqlcmd.Parameters.Add(New SqlParameter("@direccionempresa", txtDireccionEmpresaPerfil.Value))
                        sqlcmd.ExecuteNonQuery()
                    End If

                    ShowPopUpMsg("¡Datos Actualizados Satisfactoriamente")

                    Response.Redirect(Request.RawUrl)
                Catch ex As Exception
                    ShowPopUpMsg("Error al actualizar datos")
                End Try

            End Using
            Call CargarDatosPerfil()
        End If
    End Sub

    '----------------------------------------FIN PAGINA PERFIL -------------------------------------------

    '---------------------------------------- PAGINA CAMBIO DE CLAVE----------------------------------------
    Protected Sub btnCambiar_Click(sender As Object, e As EventArgs)
        If txtNueva.Text <> txtNuevaRe.Text Then
            ShowPopUpMsg("Clave Reingresada no corresponde")
            txtAnterior.Focus()
            Return
        End If

        If ConectaSQLServer() Then
            Using conn

                Try
                    Dim SqlComando As New SqlCommand
                    SqlComando.Connection = conn
                    SqlComando.CommandType = CommandType.StoredProcedure
                    SqlComando.CommandText = "pr_CambiaPassword"

                    'PARAMETROS IN

                    SqlComando.Parameters.Add("@pUser", SqlDbType.NVarChar, 50).Value = Session("pubIdUsuario")
                    SqlComando.Parameters.Add("@pAnterior", SqlDbType.NVarChar, 250).Value = txtAnterior.Text
                    SqlComando.Parameters.Add("@pNueva", SqlDbType.NVarChar, 250).Value = txtNueva.Text

                    'PARAMETROS OUT
                    SqlComando.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output


                    SqlComando.ExecuteNonQuery()
                    If SqlComando.Parameters("@status").Value = 1 Then
                        ShowPopUpMsg("Clave Cambiada")
                        txtAnterior.Text = Nothing
                        txtNueva.Text = Nothing
                        txtNuevaRe.Text = Nothing
                        txtAnterior.Focus()
                    Else
                        ShowPopUpMsg("Clave Actual Ingresada No Corresponde")
                    End If

                Catch mirror As Exception
                    ShowPopUpMsg("¡Ha ocurrido un Error al Cambiar Contraseña!")
                    Return
                End Try
            End Using

        Else
            ShowPopUpMsg("¡No hay Conexión a la Base de Datos!")
        End If
    End Sub

    Sub fExecuteJavaScript(ByVal pScript As String)

        Dim sb As New StringBuilder

        sb.Append(pScript)

        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ver", sb.ToString, True)

    End Sub
    '----------------------------------------FIN PAGINA CAMBIO DE CLAVE----------------------------------------



    '----------------------------------------PAGINA USUARIOS---------------------------------------------------
    'CARGAR GRILLA
    Sub cargarGridUsuarios()
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT usuario, empUsuaria, email, CONCAT(esAdmin, esAdminEST, esAdminUsuarios) as permisos
                  FROM ccUsuarioWeb
                  ORDER BY usuario ASC"
                    spllenaGridView(gridUsuariosWeb, sqlStr)
                Catch ex As Exception

                End Try
            End Using
        End If

    End Sub

    'LLENAR CBX EMPRESAS 
    Protected Sub cbxEmpresaUsuaria_Load1(sender As Object, e As EventArgs)

        sqlStr = "SELECT DISTINCT id
                    FROM ccEmpUsuaria
                    ORDER BY id ASC"

        spLlenaComboBoxPopUp(sqlStr, sender, "id", "id")
    End Sub

    'Llenar GRIDVIEW AL CAMBIO DE PAGINA
    Protected Sub gridUsuariosWeb_PageIndexChanged(sender As Object, e As EventArgs)
        Call cargarGridUsuarios()
    End Sub

    Protected Sub ddlPermisos_Init(sender As Object, e As EventArgs)
        Dim ddlPermisos As DropDownList = sender
        Dim container As GridViewDataItemTemplateContainer = ddlPermisos.NamingContainer
        Dim myValue As String = ""

        myValue = gridUsuariosWeb.GetRowValues(container.ItemIndex, container.Column.FieldName)
        'CARGAR DROPDOWN CON VALORES CORRESPONDIENTES A LA CONCATENACIÓN DE VALORES DE TABLAS esAdmin, esAdminEST y esAdminUsuarios.

        'ADMINISTRADOR
        If myValue = "111" Or myValue = "114" Then
            ddlPermisos.SelectedIndex = 1
            'AUDITOR
        ElseIf myValue = "112" Then
            ddlPermisos.SelectedIndex = 3
            'WEB
        ElseIf myValue = "101" Then
            ddlPermisos.SelectedIndex = 2
            'OPERACIONES
        ElseIf myValue = "003" Then
            ddlPermisos.SelectedIndex = 4
            'CLIENTE
        ElseIf myValue = "000" Then
            ddlPermisos.SelectedIndex = 0
        End If
    End Sub

    'AL CAMBIAR EL PERMISO, LO INGRESO A LA BASE DE DATOS
    Protected Sub ddlPermisos_SelectedIndexChanged(sender As Object, e As EventArgs)


        'Encuentro el dropdown dentro de la gridview y le agrego el index
        Dim ddlPermisos As DropDownList = sender
        Dim container As GridViewDataItemTemplateContainer = ddlPermisos.NamingContainer
        Dim myValue As String = ddlPermisos.SelectedIndex
        Dim usuario As String = gridUsuariosWeb.GetRowValues(container.ItemIndex, "usuario")
        'Dim ddlPermisos As DropDownList = gridUsuariosWeb.FindRowCellTemplateControl(index, gridUsuariosWeb.DataColumns(3), "ddlPermisos")
        If IsNothing(ddlPermisos) Then
            Return
        Else
            'Dim usuario = gridUsuariosWeb.GetRowValues(Index, "usuario")

            Dim permiso = ddlPermisos.SelectedItem.Value
            If ConectaSQLServer() Then
                Using conn

                    Try
                        'CLIENTE
                        If permiso = 0 Then
                            sqlStr = "UPDATE ccUsuarioWeb
                                        SET esAdmin = 0, esAdminEst = 0, esAdminUsuarios = 0
                                        WHERE usuario = '" & usuario & "'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                        End If
                        'ADMINISTRADOR
                        If permiso = 1 Then
                            sqlStr = "UPDATE ccUsuarioWeb
                                        SET esAdmin = 1, esAdminEst = 1, esAdminUsuarios = 1
                                        WHERE usuario = '" & usuario & "'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                        End If
                        'WEB
                        If permiso = 2 Then
                            sqlStr = "UPDATE ccUsuarioWeb
                                        SET esAdmin = 1, esAdminEst = 0, esAdminUsuarios = 1
                                        WHERE usuario = '" & usuario & "'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                        End If
                        'AUDITOR
                        If permiso = 3 Then
                            sqlStr = "UPDATE ccUsuarioWeb
                                        SET esAdmin = 1, esAdminEst = 1, esAdminUsuarios = 2
                                        WHERE usuario = '" & usuario & "'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                        End If
                        'OPERACIONES
                        If permiso = 4 Then
                            sqlStr = "UPDATE ccUsuarioWeb
                                        SET esAdmin = 0, esAdminEst = 0, esAdminUsuarios = 3
                                        WHERE usuario = '" & usuario & "'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                        End If

                        ShowPopUpMsg("¡Permisos actualizados exitosamente!")
                    Catch ex As Exception
                        ShowPopUpMsg("No hay conexión a base de datos")
                    End Try

                End Using
            End If

        End If
        'For index = gridUsuariosWeb.VisibleStartIndex To gridUsuariosWeb.VisibleRowCount

        'Next
    End Sub

    'LLENAR CHECKLIST UNIDADES
    Protected Sub checkListPanl_Init(sender As Object, e As EventArgs)
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "  SELECT ccu.id, CONCAT(cceu.id,' - ', ccu.descripcion) as descripcion
                                FROM  ccUnidades ccu 
                                INNER JOIN ccEmpUsuaria cceu 
                                ON ccu.idEmpUsuaria = cceu.id
                                order by ccu.id asc"

                    spLlenaCheckBoxList(sqlStr, sender, "id", "descripcion")
                Catch ex As Exception

                End Try
            End Using
        End If

    End Sub


    Protected Sub btnCrear_Click(sender As Object, e As EventArgs)
        Dim message As String = String.Empty

        If True Then
            If txtContraseñaPermisosUsuario.Text <> txtConfirmarContraseñaPermisosUsuario.Text Then
                ShowPopUpMsg("¡Clave Reingresada no corresponde!")
                txtContraseñaPermisosUsuario.Focus()
                Exit Sub
            End If
        End If

        Dim userId As Integer = 0
        Dim valores As String
        Dim resultado = ""

        If ConectaSQLServer() Then
            Using conn
                Try
                    Using cmd As New SqlCommand("pr_diCrearUsuario")
                        Using sda As New SqlDataAdapter()
                            cmd.CommandType = CommandType.StoredProcedure
                            cmd.Parameters.AddWithValue("@Nombre", txtNombrePermisos.Text.Trim())
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim())
                            cmd.Parameters.AddWithValue("@Password", txtContraseñaPermisosUsuario.Text.Trim())
                            cmd.Parameters.AddWithValue("@Email", txtCorreo.Text.Trim())
                            cmd.Parameters.AddWithValue("@correoSupervisor", txtCorreoSupervisorUsuarios.Text.Trim())
                            cmd.Parameters.AddWithValue("@Empresa", cbxEmpresaUsuaria.SelectedItem.Value)
                            'TOMO LOS VALORES DEL LISTBOX EMPRESA NO LOS TEXTOS DEL DROPDOWN

                            'RECORRER EL CHECKLIST Y AGREGAR VALORES
                            For index = 0 To checkListPanl.SelectedValues.Count() - 1
                                valores = checkListPanl.SelectedValues.Item(index).ToString()
                                If resultado = "" Then
                                    resultado = Chr(39) + valores + Chr(39)
                                Else
                                    resultado = resultado + ", " + Chr(39) + valores + Chr(39)
                                End If
                            Next
                            cmd.Parameters.AddWithValue("@Unidades", resultado)

                            'DAR PERMISOS CORRECTOS A LOS TIPOS DE USUARIOS
                            'CLIENTE
                            If cbxPermisoCrearUsuario.SelectedItem.Value = 0 Then
                                cmd.Parameters.AddWithValue("@PermisosesAdmin", "0")
                                cmd.Parameters.AddWithValue("@PermisosesAdminEST", "0")
                                cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "0")
                            End If

                            'ADMINISTRADOR
                            If cbxPermisoCrearUsuario.SelectedItem.Value = 1 Then
                                cmd.Parameters.AddWithValue("@PermisosesAdmin", "1")
                                cmd.Parameters.AddWithValue("@PermisosesAdminEST", "1")
                                cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "1")
                            End If

                            'WEB
                            If cbxPermisoCrearUsuario.SelectedItem.Value = 2 Then
                                cmd.Parameters.AddWithValue("@PermisosesAdmin", "1")
                                cmd.Parameters.AddWithValue("@PermisosesAdminEST", "0")
                                cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "1")
                            End If

                            'AUDITOR
                            If cbxPermisoCrearUsuario.SelectedItem.Value = 3 Then
                                cmd.Parameters.AddWithValue("@PermisosesAdmin", "1")
                                cmd.Parameters.AddWithValue("@PermisosesAdminEST", "1")
                                cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "2")
                            End If

                            'AUDITOR
                            If cbxPermisoCrearUsuario.SelectedItem.Value = 4 Then
                                cmd.Parameters.AddWithValue("@PermisosesAdmin", "0")
                                cmd.Parameters.AddWithValue("@PermisosesAdminEST", "0")
                                cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "3")
                            End If

                            If chkEvaluacion.Value = True Then
                                cmd.Parameters.AddWithValue("@veEvaluacion", 1)
                            Else
                                cmd.Parameters.AddWithValue("@veEvaluacion", 0)
                            End If

                            cmd.Connection = conn
                            userId = Convert.ToInt32(cmd.ExecuteScalar())
                        End Using
                    End Using
                    Select Case userId
                        Case -1
                            message = "¡Usuario ya existe!\nPor favor escoja un nombre de usuario diferente."
                            txtUsername.Focus()
                            Exit Select
                        Case Else
                            message = "¡Registro exitoso!"
                            popUpCrearUsuarios.ShowOnPageLoad = False
                            Form.DefaultButton = Nothing
                            Exit Select
                    End Select
                    ClientScript.RegisterStartupScript([GetType](), "alert", (Convert.ToString("alert('") & message) + "');", True)

                Catch ex As Exception

                End Try
            End Using
        End If

        'REDRESCAR GRID
        Call cargarGridUsuarios()
        'LIMPIAR PANTALLA
    End Sub

    Protected Sub btnEliminarUsuario_Click(sender As Object, e As EventArgs)
        Dim btnEliminar As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = btnEliminar.NamingContainer
        Dim usuario As String = ""

        'OBTENGO EL USUARIO A ELIMINAR
        usuario = gridUsuariosWeb.GetRowValues(container.ItemIndex, "usuario")

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "DELETE from ccUsuarioWeb WHERE usuario=@usuario"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.Parameters.Add(New SqlParameter("@usuario", usuario))
                    sqlcmd.ExecuteNonQuery()
                    ShowPopUpMsg("¡Usuario Eliminado!")
                Catch ex As Exception

                End Try
            End Using

        Else
            ShowPopUpMsg("No hay conexion a base datos")
            Return
        End If
        Call cargarGridUsuarios()
    End Sub


    Protected Sub btnPopUpUsuarios_Click(sender As Object, e As EventArgs)
        cbxEmpresaUsuaria.SelectedIndex = -1
        txtNombrePermisos.Text = ""
        txtUsername.Text = ""
        txtContraseñaPermisosUsuario.Text = ""
        txtConfirmarContraseñaPermisosUsuario.Text = ""
        txtCorreo.Text = ""
        txtCorreoSupervisorUsuarios.Text = ""
        chkEvaluacion.Value = False
        cbxPermisoCrearUsuario.SelectedItem = Nothing
        Dim index = 0
        checkListPanl.UnselectAll()
        popUpCrearUsuarios.ShowOnPageLoad = True
        Form.DefaultButton = btnCrear.UniqueID
    End Sub

    Protected Sub btnModificaUsuarios_Click(sender As Object, e As EventArgs)
        checkListPanlModificar.UnselectAll()
        Dim pUnidad As String = ""
        Dim pUsuario As String = ""
        Dim pUsername As String = ""
        Dim pEmail As String = ""
        Dim pEmailSupervisor As String = ""
        Dim pPassword As String = ""
        Dim pConfirmarPassword As String = ""
        Dim esAdmin As String = ""
        Dim esAdminEST As String = ""
        Dim esAdminUsuarios As String = ""
        Dim pPermisos As String = ""
        Dim pUnidades As String = ""
        Dim veEvaluacion As Int32
        Dim unidadesElegidas() As String

        If gridUsuariosWeb.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione un Usuario y VUELVA a clicar el botón")
            Exit Sub
        Else
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = "  SELECT * 
                                FROM ccUsuarioWeb 
                                WHERE usuario = '" & gridUsuariosWeb.GetRowValues(gridUsuariosWeb.FocusedRowIndex, "usuario").ToString() & "'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        Dim reader As IDataReader = sqlcmd.ExecuteReader

                        While reader.Read()
                            pUnidad = reader.Item("empUsuaria")
                            pUnidades = reader.Item("unidades")
                            pUsuario = reader.Item("nombre")
                            pUsername = reader.Item("usuario")
                            pEmail = reader.Item("email")
                            veEvaluacion = reader.Item("esEvalua")
                            If IsDBNull(reader.Item("correoSupervisor")) = False Then
                                pEmailSupervisor = reader.Item("correoSupervisor")
                            Else
                                pEmailSupervisor = ""
                            End If
                            'pPassword = reader.Item("password")
                            'pConfirmarPassword = reader.Item("password")
                            esAdmin = reader.Item("esAdmin")
                            esAdminEST = reader.Item("esAdminEST")
                            esAdminUsuarios = reader.Item("esAdminUsuarios")
                            pPermisos = esAdmin + esAdminEST + esAdminUsuarios
                        End While
                        reader.Close()
                    Catch ex As Exception
                    End Try
                End Using
            End If

            'UNIDADES
            unidadesElegidas = Split(pUnidades.Replace("'", "").Trim, ",")
            If unidadesElegidas(0) Is "" Then
                checkListPanlModificar.SelectAll()
            Else
                'RECORRO TODO EL CHECKLIST
                For index = 0 To unidadesElegidas.Count - 1
                    'BUSCO DENTRO DEL CHECKLIST SI EXISTE Y LA SELECCIONO
                    If checkListPanlModificar.Items.FindByValue(unidadesElegidas(index).Trim) IsNot Nothing Then
                        checkListPanlModificar.Items.FindByValue(unidadesElegidas(index).Trim).Selected = True
                    End If
                Next
            End If

            'VALORES A MOSTRAR
            cbxUnidadModificar.Value = pUnidad
            txtNombreUsuarioModificar.Text = pUsuario
            txtUsernameModificar.Text = pUsername
            txtCorreoUsuarioModificar.Text = pEmail
            txtCorreoSupervisorModificar.Text = pEmailSupervisor
            If veEvaluacion = 1 Then
                chkEvaluacionModificar.CheckState = DevExpress.Web.CheckState.Checked
            Else
                chkEvaluacionModificar.CheckState = DevExpress.Web.CheckState.Unchecked

            End If
            'txtContraseñaModificar.Text = pPassword
            'txtContraseñaConfirmarModificar.Text = pConfirmarPassword

            'ADMINISTRADOR
            If pPermisos = "111" Or pPermisos = "114" Then
                cbxPermisosModificar.SelectedIndex = 1
                'AUDITOR
            ElseIf pPermisos = "112" Then
                cbxPermisosModificar.SelectedIndex = 3
                'WEB
            ElseIf pPermisos = "101" Then
                cbxPermisosModificar.SelectedIndex = 2
                'OPERACIONES
            ElseIf pPermisos = "003" Then
                cbxPermisosModificar.SelectedIndex = 4
                'CLIENTE
            ElseIf pPermisos = "000" Then
                cbxPermisosModificar.SelectedIndex = 0
            End If

        End If
        popUpModificar.ShowOnPageLoad = True
    End Sub

    Protected Sub btnGuardarModificar_Click(sender As Object, e As EventArgs)

        Dim valores As String
        Dim resultado = ""


        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "UPDATE ccUsuarioWeb
                    SET nombre = @Nombre, 
                    email = @Email, 
                    correoSupervisor = @correoSupervisor, 
                    empUsuaria = @Empresa, 
                    unidades = @Unidades, 
                    esAdmin = @PermisosesAdmin, 
                    esAdminEST = @PermisosesAdminEST, 
                    esAdminUsuarios = @PermisosesAdminUsuarios,
                    esEvalua = @veEvaluacion
                    WHERE usuario = '" & gridUsuariosWeb.GetRowValues(gridUsuariosWeb.FocusedRowIndex, "usuario").ToString() & "'"

                    Using cmd As New SqlCommand(sqlStr, conn)
                        cmd.Parameters.AddWithValue("@Nombre", txtNombreUsuarioModificar.Text.Trim())
                        cmd.Parameters.AddWithValue("@Email", txtCorreoUsuarioModificar.Text.Trim())
                        cmd.Parameters.AddWithValue("@correoSupervisor", txtCorreoSupervisorModificar.Text.Trim())
                        cmd.Parameters.AddWithValue("@Empresa", cbxUnidadModificar.SelectedItem.Value)


                        'RECORRER EL CHECKLIST Y AGREGAR VALORES
                        For index = 0 To checkListPanlModificar.SelectedValues.Count() - 1
                            valores = checkListPanlModificar.SelectedValues.Item(index).ToString()
                            If resultado = "" Then
                                resultado = Chr(39) + valores + Chr(39)
                            Else
                                resultado = resultado + ", " + Chr(39) + valores + Chr(39)
                            End If
                        Next
                        cmd.Parameters.AddWithValue("@Unidades", resultado)

                        'DAR PERMISOS CORRECTOS A LOS TIPOS DE USUARIOS
                        'CLIENTE
                        If cbxPermisosModificar.SelectedItem.Value = 0 Then
                            cmd.Parameters.AddWithValue("@PermisosesAdmin", "0")
                            cmd.Parameters.AddWithValue("@PermisosesAdminEST", "0")
                            cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "0")
                        End If

                        'ADMINISTRADOR
                        If cbxPermisosModificar.SelectedItem.Value = 1 Then
                            cmd.Parameters.AddWithValue("@PermisosesAdmin", "1")
                            cmd.Parameters.AddWithValue("@PermisosesAdminEST", "1")
                            cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "1")
                        End If

                        'WEB
                        If cbxPermisosModificar.SelectedItem.Value = 2 Then
                            cmd.Parameters.AddWithValue("@PermisosesAdmin", "1")
                            cmd.Parameters.AddWithValue("@PermisosesAdminEST", "0")
                            cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "1")
                        End If

                        'AUDITOR
                        If cbxPermisosModificar.SelectedItem.Value = 3 Then
                            cmd.Parameters.AddWithValue("@PermisosesAdmin", "1")
                            cmd.Parameters.AddWithValue("@PermisosesAdminEST", "1")
                            cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "2")
                        End If

                        'AUDITOR
                        If cbxPermisosModificar.SelectedItem.Value = 4 Then
                            cmd.Parameters.AddWithValue("@PermisosesAdmin", "0")
                            cmd.Parameters.AddWithValue("@PermisosesAdminEST", "0")
                            cmd.Parameters.AddWithValue("@PermisosesAdminUsuarios", "3")
                        End If

                        If chkEvaluacionModificar.Value = True Then
                            cmd.Parameters.AddWithValue("@veEvaluacion", 1)
                        Else
                            cmd.Parameters.AddWithValue("@veEvaluacion", 0)
                        End If

                        cmd.ExecuteNonQuery()
                    End Using
                    ShowPopUpMsg("¡Usuario " & gridUsuariosWeb.GetRowValues(gridUsuariosWeb.FocusedRowIndex, "usuario").ToString() & " Modificado!")

                Catch ex As Exception
                    ShowPopUpMsg("¡Error al modificar! '" & ex.ToString & "'")
                End Try
            End Using
        End If
        Call cargarGridUsuarios()
        popUpModificar.ShowOnPageLoad = False
    End Sub

    Protected Sub cbpUnidades_Callback(source As Object, e As CallbackEventArgs)

    End Sub
    '----------------------------------------FIN PAGINA USUARIOS----------------------------------------


End Class