Imports System.Data.SqlClient
Imports DevExpress.Export
Imports DevExpress.Web
Imports DevExpress.XtraPrinting

Public Class pag_contrataciones
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Form.DefaultButton = btnConsultar.UniqueID

        'ELIJO EN QUE CONTROL MOSTRAR EL LOADING PANEL
        lpCallBack.ContainerElementID = gridMaster.ID

        'CARGAR LAS EMPRESAS DEPENDIENDO DE LA SESIONEMPRESA QUE HAYA UTILIZADO
        'cbxEmpresaPagina.Value = Session("pubEmpUsuariaUsuario")

        'CARGA COMBOBOX EMPRESA EN PAGINA
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim sqlCombo = $"SELECT DISTINCT id, descripcion from ccEmpUsuaria WHERE id <> 'TMPO'"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresaPagina, "id", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}")
                End Try
            End Using
        End If

        ''ACTUALIZO JORNADAS
        'If ConectaSQLServer() Then
        '    Using conn
        '        Dim SqlComando As New SqlCommand
        '        Try
        '            SqlComando.Connection = conn
        '            SqlComando.CommandType = CommandType.StoredProcedure
        '            SqlComando.CommandText = "pr_ActualizaJornadas"
        '            SqlComando.ExecuteNonQuery()
        '            SqlComando.Dispose()
        '        Catch ex As Exception
        '            ShowPopUpMsg("Error 6: {0}")
        '        End Try
        '    End Using
        'End If

    End Sub

    Private Sub pag_contrataciones_Init(sender As Object, e As EventArgs) Handles Me.Init
        'BUSQUEDA DE REPORTES
        Session("BusquedaRealizada") = 0

        'ELIMINO LA TABLA DE LA BD CREADA EN LA DOTACION MENSUAL
        If Session("DotacionMensual") <> Nothing Then
            Try
                sqlStr = $"DROP TABLE {Session("DotacionMensual")}"
                If ConectaSQLServer() Then
                    Using conn
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.ExecuteNonQuery()
                        sqlcmd.Dispose()
                    End Using
                End If
                Session("DotacionMensual") = Nothing
            Catch ex As Exception
                ShowPopUpMsg("Error en Reporte 3 - Drop Table " + ex.ToString)
                Return
            End Try
        End If

        'CARGA COMBOBOX EMPRESA EN PAGINA
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim sqlCombo = $"SELECT DISTINCT id, descripcion from ccEmpUsuaria WHERE id <> 'TMPO'"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresaPagina, "id", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" )
                End Try
            End Using
        End If

        'CARGA COMBOBOX EMPRESA DENTRO DE POPUP
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim sqlCombo = $"SELECT DISTINCT id, descripcion from ccEmpUsuaria WHERE id <> 'TMPO'"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresa, "id", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" )
                End Try
            End Using
        End If

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

        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Then
            gridMaster.Columns.Item("eliminar").Visible = True
        End If

        Try
            sqlStr = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccViewEmpleados WHERE id IN (" & Session("pubUnidadesUsuario") & ") ORDER BY descripcion"
            Call spLlenaComboBoxPopUp(sqlStr, txtUnidad, "idUnidad", "descripcion")
        Catch ex As Exception
            ShowPopUpMsg("Error 1.5: {0}" )
        End Try


        Try
            sqlStr = "SELECT Codigo, UPPER(dbo.LimpiarCaracteres(Descrip)) as Descrip from ccCargo ORDER BY Descrip"
            Call spLlenaComboBoxPopUp(sqlStr, txtCargo, "Codigo", "Descrip")
        Catch ex As Exception
            ShowPopUpMsg("Error 2: {0}" )
        End Try


        Try
            sqlStr = "SELECT Codigo, UPPER(dbo.LimpiarCaracteres(Descrip)) as Descrip from ccCausaLegal ORDER BY Descrip"
            Call spLlenaComboBoxPopUp(sqlStr, txtCausa, "Codigo", "Descrip")
        Catch ex As Exception
            ShowPopUpMsg("Error 3: {0}" )
        End Try

        'CARGO LA UNIDAD INICIAL
        If IsPostBack = False Then
            cbxEmpresaPagina.Value = Session("pubEmpUsuariaUsuario")

            'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                        Call spLlenaComboBox(sqlCombo, cbxUnidad, "idUnidad", "descripcion")
                    Catch ex As Exception
                        ShowPopUpMsg("Error 1: {0}" )
                    End Try
                End Using
            End If
        End If

    End Sub

    Sub spCargaGridMaster()

        Try
            If cbxUnidad.Value = Nothing Then
                sqlStr = "  SELECT Remples.Nombre AS asociado, Remples.rut as rutasociado, ccContratoMarco.id AS id, ccContratoMarco.codContrato AS codContrato, ccContratoMarco.empUsuaria AS empUsuaria, ccContratoMarco.unidad AS unidad, UPPER(dbo.LimpiarCaracteres(ccUnidades.descripcion)) as descUnidad, dbo.fn_strToFecha(ccContratoMarco.fechaContrato) as fecha, ccContratoMarco.causaLegal AS causaLegal, ccCausaLegal.descrip as descCausa, ccContratoMarco.detalleCausal AS detalleCausal, ccContratoMarco.cargo AS cargo, UPPER(dbo.LimpiarCaracteres(ccCargo.descrip)) as descCargo,
                            ccContratoMarco.cantidadTrab AS cantidadTrab, dbo.fn_strToFecha(ccContratoMarco.fechainicio) as fini, dbo.fn_strToFecha(ccContratoMarco.fechafin) as ffin, ccContratoMarco.cantDias AS cantDias, ccContratoMarco.valorPagar AS valorPagar
                            
                            FROM ccContratoMarco INNER JOIN ccUNidades ON ccContratoMarco.unidad=ccUnidades.id
                            INNER JOIN ccCausaLegal ON ccCausaLegal.codigo=ccContratoMarco.causaLegal
                            INNER JOIN ccCargo ON ccCargo.codigo=ccContratoMarco.cargo
                            LEFT OUTER JOIN Remples ON ccContratoMarco.CodContrato = Remples.Credenc

                            WHERE ccContratoMarco.empUsuaria IN ('" & cbxEmpresaPagina.Value & "')
                            ORDER BY ccContratoMarco.id DESC"

                spllenaGridView(gridMaster, sqlStr)
            Else
                sqlStr = "  SELECT Remples.Nombre AS asociado, Remples.rut as rutasociado, ccContratoMarco.id AS id, ccContratoMarco.codContrato AS codContrato, ccContratoMarco.empUsuaria AS empUsuaria, ccContratoMarco.unidad AS unidad, UPPER(dbo.LimpiarCaracteres(ccUnidades.descripcion)) as descUnidad, dbo.fn_strToFecha(ccContratoMarco.fechaContrato) as fecha, ccContratoMarco.causaLegal AS causaLegal, ccCausaLegal.descrip as descCausa, ccContratoMarco.detalleCausal AS detalleCausal, ccContratoMarco.cargo AS cargo, UPPER(dbo.LimpiarCaracteres(ccCargo.descrip)) as descCargo,
                            ccContratoMarco.cantidadTrab AS cantidadTrab, dbo.fn_strToFecha(ccContratoMarco.fechainicio) as fini, dbo.fn_strToFecha(ccContratoMarco.fechafin) as ffin, ccContratoMarco.cantDias AS cantDias, ccContratoMarco.valorPagar AS valorPagar
                            
                            FROM ccContratoMarco INNER JOIN ccUNidades ON ccContratoMarco.unidad=ccUnidades.id
                            INNER JOIN ccCausaLegal ON ccCausaLegal.codigo=ccContratoMarco.causaLegal
                            INNER JOIN ccCargo ON ccCargo.codigo=ccContratoMarco.cargo
                            LEFT OUTER JOIN Remples ON ccContratoMarco.CodContrato = Remples.Credenc

                            WHERE ccContratoMarco.empUsuaria IN ('" & cbxEmpresaPagina.Value & "') AND ccContratoMarco.unidad = '" & cbxUnidad.Value.ToString & "' 
                            ORDER BY ccContratoMarco.id DESC"

                spllenaGridView(gridMaster, sqlStr)
            End If
        Catch ex As Exception
            ShowPopUpMsg("Error 4: {0}" )
        End Try


        Dim lineasMaster As Integer = gridMaster.VisibleRowCount
        Dim idContrato As String = ""

    End Sub

    Protected Sub btnNuevo_Click(sender As Object, e As EventArgs)
        ''ACTUALIZO JORNADAS
        'If ConectaSQLServer() Then
        '    Using conn
        '        Dim SqlComando As New SqlCommand
        '        Try
        '            SqlComando.Connection = conn
        '            SqlComando.CommandType = CommandType.StoredProcedure
        '            SqlComando.CommandText = "pr_ActualizaJornadas"
        '            SqlComando.ExecuteNonQuery()
        '            SqlComando.Dispose()
        '        Catch ex As Exception
        '            ShowPopUpMsg("Error 6: {0}" )
        '        End Try
        '    End Using
        'End If
        Call cargarGridBuscarTrabajador()
        txtUnidad.Value = Nothing
        txtFecha.Date = Now()
        txtFechaIni.Date = Now()
        txtFechaFin.Date = Now()
        txtCausa.Value = Nothing
        txtDetalleCausal.Text = ""
        txtCargo.Value = Nothing
        txtTrabajadores.Text = 1
        txtDias.Text = 1
        txtValor.Text = 250000
        cbpUnidades.Visible = True
        cbxEmpresa.Value = Session("pubEmpUsuariaUsuario")
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA COMBOBOX
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & cbxEmpresa.Value & "') ORDER BY descripcion"
                    Call spLlenaComboBoxPopUp(sqlCombo, txtUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" )
                End Try
            End Using

        End If

        Session("pModoPopup") = "C"
        'pModoPopup = "C"
        popupContrato.HeaderText = "Nuevo Contrato"
        popupContrato.ShowOnPageLoad = True
        Form.DefaultButton = btnGrabar.UniqueID
    End Sub

    Protected Sub btnBorra_Click(sender As Object, e As EventArgs)
        Session("pModoPopup") = ""
        'pModoPopup = ""
        Dim idContrato As Integer = CInt(gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "id"))

        If gridMaster.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione un Contrato")
            Exit Sub
        End If


        Try
            sqlStr = String.Format("DELETE FROM ccContratoMarco WHERE id={0}", idContrato)
            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.ExecuteNonQuery()
                    sqlcmd.Dispose()
                End Using
            End If

            ShowPopUpMsg(String.Format("¡Se ha Borrado el Contrato!"))

            Call spCargaGridMaster()

        Catch ex As Exception
            ShowPopUpMsg("Error 5: {0}" )
            ShowPopUpMsg("Ocurrió Un error en btnBorrarAsociar_Click")
            Return
        End Try

    End Sub

    Protected Sub btnModifica_Click(sender As Object, e As EventArgs)

        If gridMaster.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione un Contrato y VUELVA a clicar el botón")
            Exit Sub
        Else

            Dim pId As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "id").ToString()
            Dim pEmpresa As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "empUsuaria").ToString()
            Dim pUnidad As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "unidad").ToString()
            Dim pFecha As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "fecha").ToString()
            Dim pCausa As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "causaLegal").ToString()
            Dim PDetalleCausal As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "detalleCausal").ToString()
            Dim pCargo As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "cargo").ToString()
            Dim pTrabajadores As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "cantidadTrab").ToString()
            Dim pFechaIni As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "fini").ToString()
            Dim pFechaFin As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "ffin").ToString()
            Dim pDias As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "cantDias").ToString()
            Dim pValor As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "valorPagar").ToString()

            'ccContratoMarco.id, codContrato, empUsuaria, unidad, ccUnidades.descripcion as descUnidad, dbo.fn_strToFecha(fechaContrato) as fecha,
            'causaLegal, ccCausaLegal.descrip As descCausa, detalleCausal, cargo, ccCargo.descrip As descCargo,
            ' cantidadTrab, dbo.fn_strToFecha(fechainicio) As fini, dbo.fn_strToFecha(fechafin) As ffin, cantDias, valorPagar 


            txtUnidad.Value = pUnidad
            txtFecha.Date = pFecha
            txtCausa.Value = pCausa
            txtDetalleCausal.Text = PDetalleCausal
            txtCargo.Value = pCargo
            txtTrabajadores.Text = pTrabajadores
            txtDias.Text = pDias
            txtFechaIni.Value = pFechaIni
            txtFechaFin.Value = pFechaFin
            txtValor.Text = pValor

            ''OCULTO LA BÚSQUEDA DE TRABAJADOR
            lblBusquedaTrabajador.Visible = False
            gridBucarTrabajadores.Visible = False

            cbxEmpresa.Value = Session("pubEmpUsuariaUsuario")
            If ConectaSQLServer() Then
                Using conn
                    Try
                        'CARGA COMBOBOX
                        Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & cbxEmpresa.Value & "') ORDER BY descripcion"
                        Call spLlenaComboBoxPopUp(sqlCombo, txtUnidad, "idUnidad", "descripcion")
                    Catch ex As Exception
                        ShowPopUpMsg("Error 1: {0}" )
                    End Try
                End Using

            End If

            Session("pModoPopup") = "M"
            'pModoPopup = "M"
            popupContrato.HeaderText = "Modificar Contrato"
            popupContrato.ShowOnPageLoad = True
            Form.DefaultButton = btnGrabar.UniqueID
        End If

    End Sub


    Protected Sub btnAsociar_Click(sender As Object, e As EventArgs)

        If gridMaster.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione un Contrato")
            Exit Sub
        End If

        If gridMaster.VisibleRowCount = 0 Then
            ShowPopUpMsg("¡No hay Contrato Marco Seleccionado!")
            Return
        End If

        popupAsocia.ShowOnPageLoad = True
        Form.DefaultButton = btnOkAsociar.UniqueID

    End Sub

    Protected Sub btnGrabar_Click(sender As Object, e As EventArgs)
        'CONEXIÓN EXTRA PARA CAMBIAR CUANDO UNA ESTÉ OCUPADA
        Dim connExtra As New SqlConnection
        Dim conex_usable As SqlConnection

        'If ConectaSQLServer() Then
        '    Using conn
        '        Dim SqlComando As New SqlCommand
        '        Try
        '            SqlComando.Connection = conn
        '            SqlComando.CommandType = CommandType.StoredProcedure
        '            SqlComando.CommandText = "pr_ActualizaJornadas"
        '            SqlComando.ExecuteNonQuery()
        '            SqlComando.Dispose()
        '        Catch ex As Exception
        '            ShowPopUpMsg("Error 6: {0}" )
        '        End Try
        '    End Using
        'End If

        'VALIDACION MANUAL DE LIMITES
        If txtCausa.Value = 1 Then
            If CInt(txtDias.Text) > 90 Then
                ShowPopUpMsg("¡Cantidad Máxima de Días por esta Causal es de 90 días!")
                Return
            End If
        End If

        If txtCausa.Value = 4 Then
            If CInt(txtDias.Text) > 180 Then
                ShowPopUpMsg("¡Cantidad Máxima de Días por esta Causal es de 180 días!")
                Return
            End If
        End If

        Dim ames As String = txtFecha.Date.ToString("yyyyMM")

        If Session("pModoPopup") = "C" Then
            Dim conn2 As New SqlConnection

            Try
                If ConectaSQLServerConn(conn2) Then
                    Using conn2
                        Dim SqlComando As New SqlCommand
                        SqlComando.CommandType = CommandType.StoredProcedure
                        SqlComando.CommandText = "pr_ccAgregaContrato"
                        'PARAMETROS IN
                        SqlComando.Parameters.Add("@empUsuaria", SqlDbType.NVarChar, 4).Value = Session("pubEmpUsuariaUsuario")
                        SqlComando.Parameters.Add("@unidad", SqlDbType.NVarChar, 4).Value = txtUnidad.Value
                        SqlComando.Parameters.Add("@fecha", SqlDbType.NVarChar, 8).Value = txtFecha.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@causaLegal", SqlDbType.Int).Value = txtCausa.Value
                        SqlComando.Parameters.Add("@detalleCausal", SqlDbType.NVarChar, 500).Value = txtDetalleCausal.Text
                        SqlComando.Parameters.Add("@cargo", SqlDbType.Int).Value = txtCargo.Value
                        SqlComando.Parameters.Add("@cantidadTrab", SqlDbType.Int).Value = txtTrabajadores.Value
                        SqlComando.Parameters.Add("@fechaini", SqlDbType.NVarChar, 8).Value = txtFechaIni.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@fechafin", SqlDbType.NVarChar, 8).Value = txtFechaFin.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@cantDias", SqlDbType.Int).Value = txtDias.Value
                        SqlComando.Parameters.Add("@valor", SqlDbType.Int).Value = txtValor.Value
                        SqlComando.Parameters.Add("@ames", SqlDbType.NVarChar, 6).Value = ames

                        'PARAMETROS OUT
                        SqlComando.Parameters.Add("@contrato", SqlDbType.NVarChar, 13).Direction = ParameterDirection.Output

                        If conn2.State <> ConnectionState.Open Then
                            conex_usable = connExtra
                            SqlComando.Connection = conex_usable
                            SqlComando.ExecuteNonQuery()
                            SqlComando.Dispose()
                            SqlComando.Connection.Dispose()
                        Else
                            conex_usable = conn2
                            SqlComando.Connection = conex_usable
                            SqlComando.ExecuteNonQuery()
                            SqlComando.Dispose()
                            SqlComando.Connection.Dispose()
                        End If

                        ShowPopUpMsg(String.Format("¡Se ha Grabado el Contrato {0}!", SqlComando.Parameters("@contrato").Value.ToString))

                    End Using

                Else
                    ShowPopUpMsg("¡No hay conexion con la Base de Datos!")
                    Return
                End If


            Catch ex As Exception
                ShowPopUpMsg("Error 6: {0}" )
                ShowPopUpMsg("Ha ocurrido un Error al grabar Detalle Requerimiento!")
                GrabaLog(Session("pubIdUsuario"), "GRABA_CONTRATO", ex.ToString)
                Return
            End Try

            Call spCargaGridMaster()
            popupContrato.ShowOnPageLoad = False
            Form.DefaultButton = Nothing
            Session("pModoPopup") = ""

        End If

        If Session("pModoPopup") = "M" Then
            Dim conn3 As New SqlConnection
            Try
                If ConectaSQLServerConn(conn3) Then
                    Using conn3
                        Dim SqlComando As New SqlCommand
                        SqlComando.CommandType = CommandType.StoredProcedure
                        SqlComando.CommandText = "pr_ccModificaContrato"

                        Dim id As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "id").ToString
                        Dim oldContrato As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "codContrato").ToString()

                        'PARAMETROS IN
                        SqlComando.Parameters.Add("@id", SqlDbType.Int).Value = CInt(id)
                        SqlComando.Parameters.Add("@empUsuaria", SqlDbType.NVarChar, 4).Value = Session("pubEmpUsuariaUsuario")
                        SqlComando.Parameters.Add("@unidad", SqlDbType.NVarChar, 4).Value = txtUnidad.Value
                        SqlComando.Parameters.Add("@fecha", SqlDbType.NVarChar, 8).Value = txtFecha.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@causaLegal", SqlDbType.Int).Value = txtCausa.Value
                        SqlComando.Parameters.Add("@detalleCausal", SqlDbType.NVarChar, 500).Value = txtDetalleCausal.Text
                        SqlComando.Parameters.Add("@cargo", SqlDbType.Int).Value = txtCargo.Value
                        SqlComando.Parameters.Add("@cantidadTrab", SqlDbType.Int).Value = txtTrabajadores.Value
                        SqlComando.Parameters.Add("@fechaini", SqlDbType.NVarChar, 8).Value = txtFechaIni.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@fechafin", SqlDbType.NVarChar, 8).Value = txtFechaFin.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@cantDias", SqlDbType.Int).Value = txtDias.Value
                        SqlComando.Parameters.Add("@valor", SqlDbType.Int).Value = txtValor.Value
                        SqlComando.Parameters.Add("@ames", SqlDbType.NVarChar, 6).Value = ames
                        SqlComando.Parameters.Add("@oldContrato", SqlDbType.NVarChar, 15).Value = oldContrato

                        'PARAMETROS OUT
                        SqlComando.Parameters.Add("@newContrato", SqlDbType.NVarChar, 15).Direction = ParameterDirection.Output

                        If conn3.State <> ConnectionState.Open Then
                            conex_usable = connExtra
                            SqlComando.Connection = conex_usable
                            SqlComando.ExecuteNonQuery()
                            SqlComando.Dispose()
                            SqlComando.Connection.Dispose()
                        Else
                            conex_usable = conn3
                            SqlComando.Connection = conex_usable
                            SqlComando.ExecuteNonQuery()
                            SqlComando.Dispose()
                            SqlComando.Connection.Dispose()
                        End If

                        Dim newContrato As String = SqlComando.Parameters("@newContrato").Value.ToString

                        'REASOCIACION
                        Dim connReAsociar As New SqlConnection
                        sqlStr = String.Format("UPDATE Remples SET CREDENC='{0}' WHERE CREDENC='{1}'", newContrato, oldContrato)
                        Try
                            If ConectaSQLServerConn(connReAsociar) Then
                                Using connReAsociar
                                    Dim sqlcmd As New SqlCommand(sqlStr, connReAsociar)
                                    SqlComando.Connection = conex_usable
                                    sqlcmd.ExecuteNonQuery()
                                    SqlComando.Dispose()
                                End Using
                            End If
                        Catch ex As Exception
                            ShowPopUpMsg("Error 7: {0}" )
                            ShowPopUpMsg("Ocurrió Un error en ReAsociar_Click")
                            Return
                        End Try


                        ShowPopUpMsg(String.Format("¡Se ha Modificado el Contrato!"))

                    End Using

                Else
                    ShowPopUpMsg("¡No hay conexion con la Base de Datos!")
                    Return
                End If

                Call spCargaGridMaster()
                popupContrato.ShowOnPageLoad = False
                Form.DefaultButton = Nothing
                Session("pModoPopup") = ""

            Catch mirror As Exception
                ShowPopUpMsg("Error 8: {0}")
                ShowPopUpMsg("Ha ocurrido un Error al Modificar!")
                GrabaLog(Session("pubIdUsuario"), "MODIFICA_CONTRATO", mirror.ToString)
                Return
            End Try

        End If

    End Sub

    Protected Sub btnOkAsociar_Click(sender As Object, e As EventArgs)
        'CONEXIÓN EXTRA PARA CAMBIAR CUANDO UNA ESTÉ OCUPADA
        Dim connExtra As New SqlConnection
        Dim conex_usable As SqlConnection

        Session("pModoPopup") = ""
        Dim sqlrdr As SqlDataReader
        Dim conn4 As New SqlConnection
        Try
            If ConectaSQLServerConn(conn4) Then
                Using conn4
                    sqlStr = String.Format("SELECT Codigo, Nombre FROM remples WHERE estado='A' AND dbo.TRIM(Codigo)='{0}'", txtCodigoTrabajador.Text.Trim)
                    If conn4.State <> ConnectionState.Open Then
                        conex_usable = connExtra
                    Else
                        conex_usable = conn4
                    End If

                    Dim sqlcmd As New SqlCommand(sqlStr, conex_usable)
                    sqlrdr = sqlcmd.ExecuteReader()
                    If sqlrdr.HasRows Then
                        While sqlrdr.Read
                            txtIdEmpleado.Text = sqlrdr("Codigo").ToString
                            txtNombreEmpleado.Text = sqlrdr("Nombre").ToString
                        End While

                    Else
                        ShowPopUpMsg("Código Empleado No Válido")
                        txtCodigoTrabajador.Focus()
                        Return
                    End If
                End Using

            End If

        Catch ex As Exception
            ShowPopUpMsg("Error 9: {0}" )
            ShowPopUpMsg("Ocurrió Un error en btnOkAsociar_Click")
            Return
        End Try

        popupAsocia.ShowOnPageLoad = False
        Form.DefaultButton = Nothing

        popupGrabarAsociar.ShowOnPageLoad = True
        Form.DefaultButton = btnGrabarAsociar.UniqueID

    End Sub

    Protected Sub btnGrabarAsociar_Click(sender As Object, e As EventArgs)
        'CONEXIÓN EXTRA PARA CAMBIAR CUANDO UNA ESTÉ OCUPADA
        Dim connExtra As New SqlConnection
        Dim conex_usable As SqlConnection

        Session("pModoPopup") = ""
        Dim pContrato As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "codContrato").ToString()

        Dim conn5 As New SqlConnection
        Try
            If ConectaSQLServerConn(conn5) Then
                Using conn5
                    sqlStr = String.Format("UPDATE Remples SET CREDENC='{0}' WHERE Codigo='{1}'", pContrato, txtIdEmpleado.Text)
                    If conn5.State <> ConnectionState.Open Then
                        conex_usable = connExtra
                    Else
                        conex_usable = conn5
                    End If
                    Dim sqlcmd As New SqlCommand(sqlStr, conex_usable)
                    sqlcmd.ExecuteNonQuery()
                    sqlcmd.Dispose()
                End Using

                popupGrabarAsociar.ShowOnPageLoad = False
                Form.DefaultButton = Nothing

                popupAsocia.ShowOnPageLoad = True
                Form.DefaultButton = btnAsociar.UniqueID
            End If

        Catch ex As Exception
            ShowPopUpMsg("Ocurrió Un error en btnGrabarAsociar_Click")
            Return
        End Try

        ShowPopUpMsg(String.Format("¡Se Asocio Correctamente el Contrato al Trabajador"))
        Call spCargaGridMaster()
    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Sub fExecuteJavaScript(ByVal pScript As String)

        Dim sb As New StringBuilder

        sb.Append(pScript)

        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ver", sb.ToString, True)

    End Sub

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)
        Call spCargaGridMaster()
        gridMaster.PageIndex = 0
    End Sub

    Protected Sub btnEliminarArchivo_Click(sender As Object, e As EventArgs)
        'CONEXIÓN EXTRA PARA CAMBIAR CUANDO UNA ESTÉ OCUPADA
        Dim connExtra As New SqlConnection
        Dim conex_usable As SqlConnection

        Session("pModoPopup") = ""
        Dim link As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer
        Dim idContrato As Integer = CInt(gridMaster.GetRowValues(container.ItemIndex, "id"))

        If gridMaster.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione un Contrato")
            Exit Sub
        End If
        Dim conn6 As New SqlConnection
        Try
            If ConectaSQLServerConn(conn6) Then
                Using conn6
                    sqlStr = String.Format("DELETE FROM ccContratoMarco WHERE id={0}", idContrato)

                    If conn6.State <> ConnectionState.Open Then
                        conex_usable = connExtra
                    Else
                        conex_usable = conn6
                    End If

                    Dim sqlcmd As New SqlCommand(sqlStr, conn6)
                    sqlcmd.ExecuteNonQuery()
                    sqlcmd.Dispose()
                End Using

            End If

        Catch ex As Exception
            ShowPopUpMsg("Ocurrió Un error al Eliminar el Contrato")
            Return
        End Try

        ShowPopUpMsg(String.Format("¡Se ha Borrado el Contrato!"))
        Call spCargaGridMaster()

    End Sub

    Protected Sub gridBucarTrabajadores_Init(sender As Object, e As EventArgs)
        Call cargarGridBuscarTrabajador()
    End Sub

    Sub cargarGridBuscarTrabajador()

        Try
            sqlStr = "SELECT DISTINCT (Remples.Codigo) AS codigo, Remples.Nombre AS Nombre, Remples.rut as rutasociado, ccContratoMarco.detalleCausal AS detalleCausal, 
                                    ccCausaLegal.Descrip AS descrip, dbo.fn_strToFecha(ccContratoMarco.fechainicio) as fini, 
                                    dbo.fn_strToFecha(ccContratoMarco.fechafin) as ffin, ccContratoMarco.cantDias AS cantDias

                                    FROM ccContratoMarco INNER JOIN ccUNidades ON ccContratoMarco.unidad=ccUnidades.id
                                    INNER JOIN ccCausaLegal ON ccCausaLegal.codigo=ccContratoMarco.causaLegal
                                    RIGHT OUTER JOIN Remples ON ccContratoMarco.CodContrato = Remples.Credenc

									WHERE ccContratoMarco.detalleCausal IS NOT NULL
                                    ORDER BY Remples.Codigo DESC"

            spllenaGridView(gridBucarTrabajadores, sqlStr)
        Catch ex As Exception
            ShowPopUpMsg("Error 10: {0}" )
        End Try


    End Sub

    Protected Sub btnDuplicar_Click(sender As Object, e As EventArgs)
        'CONEXIÓN EXTRA PARA CAMBIAR CUANDO UNA ESTÉ OCUPADA
        Dim connExtra As New SqlConnection
        Dim conex_usable As SqlConnection

        'ACTUALIZO JORNADAS
        'Dim conn7 As New SqlConnection
        'If ConectaSQLServerConn(conn7) Then
        '    Using conn7
        '        Dim SqlComando As New SqlCommand
        '        Try

        '            SqlComando.CommandType = CommandType.StoredProcedure
        '            SqlComando.CommandText = "pr_ActualizaJornadas"
        '            If conn7.State <> ConnectionState.Open Then
        '                conex_usable = connExtra
        '                SqlComando.Connection = conex_usable
        '                SqlComando.ExecuteNonQuery()
        '                SqlComando.Dispose()
        '                SqlComando.Connection.Dispose()
        '            Else
        '                conex_usable = connExtra
        '                SqlComando.Connection = conn7
        '                SqlComando.ExecuteNonQuery()
        '                SqlComando.Dispose()
        '                SqlComando.Connection.Dispose()
        '            End If

        '        Catch ex As Exception
        '            ShowPopUpMsg("Error 1.3: {0}" )
        '        End Try
        '    End Using
        'End If

        Session("pModoPopup") = ""
        If gridMaster.FocusedRowIndex < 0 Or gridMaster.VisibleRowCount <= 0 Then
            ShowPopUpMsg("Seleccione un Contrato y VUELVA a clicar el botón")
            Exit Sub
        Else
            Dim pId As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "id").ToString()
            Dim pUnidad As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "unidad").ToString()
            Dim pFecha As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "fecha").ToString()
            Dim pCausa As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "causaLegal").ToString()
            Dim PDetalleCausal As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "detalleCausal").ToString()
            Dim pCargo As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "cargo").ToString()
            Dim pTrabajadores As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "cantidadTrab").ToString()
            Dim pFechaIni As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "fini").ToString()
            Dim pFechaFin As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "ffin").ToString()
            Dim pDias As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "cantDias").ToString()
            Dim pValor As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "valorPagar").ToString()
            Dim pcontrato As String = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "codContrato").ToString()

            txtUnidad.Value = pUnidad
            txtFecha.Date = pFecha
            txtCausa.Value = pCausa
            txtDetalleCausal.Text = PDetalleCausal
            txtCargo.Value = pCargo
            txtTrabajadores.Text = pTrabajadores
            txtDias.Text = pDias
            txtFechaIni.Date = pFechaIni
            txtFechaFin.Date = pFechaFin
            txtValor.Text = pValor



            Dim ames As String = txtFecha.Date.ToString("yyyyMM")
            Dim conn8 As New SqlConnection
            Try
                If ConectaSQLServerConn(conn8) Then
                    Using conn8
                        Dim SqlComando As New SqlCommand
                        SqlComando.CommandType = CommandType.StoredProcedure
                        SqlComando.CommandText = "pr_ccAgregaContrato"
                        'PARAMETROS IN

                        SqlComando.Parameters.Add("@empUsuaria", SqlDbType.NVarChar, 4).Value = Session("pubEmpUsuariaUsuario")
                        SqlComando.Parameters.Add("@unidad", SqlDbType.NVarChar, 4).Value = txtUnidad.Value
                        SqlComando.Parameters.Add("@fecha", SqlDbType.NVarChar, 8).Value = txtFecha.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@causaLegal", SqlDbType.Int).Value = txtCausa.Value
                        SqlComando.Parameters.Add("@detalleCausal", SqlDbType.NVarChar, 500).Value = txtDetalleCausal.Text
                        SqlComando.Parameters.Add("@cargo", SqlDbType.Int).Value = txtCargo.Value
                        SqlComando.Parameters.Add("@cantidadTrab", SqlDbType.Int).Value = txtTrabajadores.Value
                        SqlComando.Parameters.Add("@fechaini", SqlDbType.NVarChar, 8).Value = txtFechaIni.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@fechafin", SqlDbType.NVarChar, 8).Value = txtFechaFin.Date.ToString("yyyyMMdd")
                        SqlComando.Parameters.Add("@cantDias", SqlDbType.Int).Value = txtDias.Value
                        SqlComando.Parameters.Add("@valor", SqlDbType.Int).Value = txtValor.Value
                        SqlComando.Parameters.Add("@ames", SqlDbType.NVarChar, 6).Value = ames

                        'PARAMETROS OUT
                        SqlComando.Parameters.Add("@contrato", SqlDbType.NVarChar, 13).Direction = ParameterDirection.Output
                        If conn8.State <> ConnectionState.Open Then
                            conex_usable = connExtra
                            SqlComando.Connection = conex_usable
                            SqlComando.ExecuteNonQuery()
                            SqlComando.Dispose()
                            SqlComando.Connection.Dispose()
                        Else
                            conex_usable = conn8
                            SqlComando.Connection = conex_usable
                            SqlComando.ExecuteNonQuery()
                            SqlComando.Dispose()
                            SqlComando.Connection.Dispose()
                        End If

                    End Using

                Else
                    ShowPopUpMsg("¡No hay conexion con la Base de Datos!")
                    Return
                End If

            Catch mirror As Exception
                ShowPopUpMsg("Ha ocurrido un Error al grabar Detalle Requerimiento!")
                GrabaLog(Session("pubIdUsuario"), "GRABA_CONTRATO", mirror.ToString)
                Return
            End Try

            ShowPopUpMsg("¡Se ha Duplicado el Contrato Satisfactoriamente!")
            Call spCargaGridMaster()
        End If

    End Sub

    Protected Sub gridMaster_PageIndexChanged(sender As Object, e As EventArgs)
        Call spCargaGridMaster()
    End Sub

    Protected Sub cbpGridMaster_Callback(source As Object, e As CallbackEventArgs)
        If gridMaster.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Seleccione un Contrato y VUELVA a clicar el botón")
            Return
        End If

        Dim conn9 As New SqlConnection
        If ConectaSQLServerConn(conn9) Then
            Using conn9
                Try
                    'Page.ClientScript.RegisterStartupScript([GetType](), "OpenWindow", "window.open('pag_contrato.aspx', '_blank');", True)
                    Session("pubRutContrato") = gridMaster.GetRowValues(gridMaster.FocusedRowIndex, "rutasociado").ToString()
                    cbpGridMaster.JSProperties("cpNewWindowUrl") = "pag_contrato.aspx"
                Catch ex As Exception
                    ShowPopUpMsg("Error 11: {0}" )
                End Try
            End Using
        End If
        'Call spCargaGridMaster()
    End Sub

    Protected Sub cbpUnidades_Callback(sender As Object, e As CallbackEventArgsBase)
        txtUnidad.Value = Nothing
        If Session("pModoPopup") = "C" Then
            lblBusquedaTrabajador.Visible = True
            gridBucarTrabajadores.Visible = True
        End If

        If Session("pModoPopup") = "M" Then
            lblBusquedaTrabajador.Visible = False
            gridBucarTrabajadores.Visible = False
        End If
        Session("pubEmpUsuariaUsuario") = cbxEmpresa.Value
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA COMBOBOX
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & cbxEmpresa.Value & "') ORDER BY descripcion"
                    Call spLlenaComboBoxPopUp(sqlCombo, txtUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" )
                End Try
            End Using

        End If
    End Sub

    Protected Sub cbpPanelPagina_Callback(sender As Object, e As CallbackEventArgsBase)
        cbxUnidad.Value = Nothing
        Session("pubEmpUsuariaUsuario") = cbxEmpresaPagina.Value
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA COMBOBOX
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" )
                End Try
            End Using

        End If

    End Sub

    Protected Sub gridMaster_BeforeGetCallbackResult(sender As Object, e As EventArgs)
        Call spCargaGridMaster()
    End Sub

    Protected Sub cbxUnidad_Load(sender As Object, e As EventArgs)
        'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                    Call spLlenaComboBox(sqlCombo, cbxUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" )
                End Try
            End Using
        End If
    End Sub

    Protected Sub cbpGrid_Callback(sender As Object, e As CallbackEventArgsBase)
        Call spCargaGridMaster()
        gridMaster.PageIndex = 0
    End Sub

    Protected Sub btnExportarGrid_Click(sender As Object, e As EventArgs)
        Try
            cargarGridBuscarTrabajador()
            gridBucarTrabajadores.Columns.Item(2).Caption = "Causa Legal"
            gridBucarTrabajadores.Columns.Item(4).Caption = "Inicio Contrato"
            gridBucarTrabajadores.Columns.Item(5).Caption = "Fin Contrato"
            gridExport.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
        Catch ex As Exception
            ShowPopUpMsg("Error al exportar: " + ex.ToString())
        End Try

    End Sub
End Class