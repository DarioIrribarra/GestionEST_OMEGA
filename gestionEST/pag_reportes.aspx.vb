Imports DevExpress.Export
Imports DevExpress.XtraPrinting
Imports DevExpress
Imports System.Data.SqlClient
Imports DevExpress.XtraCharts
Imports DevExpress.Web.ASPxPivotGrid
Imports System.Reflection
Imports DevExpress.Web

Public Class pag_reportes
    Inherits System.Web.UI.Page

    'VALOR QUE MIDE SI SE HIZO CLICK O NO

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Page.Form.DefaultFocus = cbxTipo.UniqueID
        Page.Form.DefaultButton = Nothing

        If IsPostBack = False Then
            Call Ocultar()
            gridDatos.Visible = False
            gridSemanal.Visible = False
            gridDotacionMensual.Visible = False
            UpdatePanel1.Visible = False
            updatePanel2.Visible = False
            gridSexo.Visible = False
            gridBucarTrabajadores.Visible = False
        End If

        If IsPostBack Then
            If Session("DotacionMensual") = Nothing Then
                'ShowPopUpMsg("Vuelva a realizar la búsqueda")
            End If

            If Session("BusquedaRealizada") = 1 Then
                Call CargarReportes()
            End If
        End If

    End Sub

    Private Sub pag_reportes_Init(sender As Object, e As EventArgs) Handles Me.Init

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

        If Session("pubUnidadesUsuario") = Nothing Then
            Response.Redirect("pag_index.aspx")
            Return
        End If

        Page.Form.DefaultFocus = cbxTipo.UniqueID
        Page.Form.DefaultButton = Nothing

        Call CargarDotacionCausal()
        Call CargarDotacion()

        'CARGAR EMPRESA
        Dim sqlCombo = $"SELECT DISTINCT id, descripcion from ccEmpUsuaria WHERE id <> 'TMPO'"
        Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresaPagina, "id", "descripcion")

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE id IN (" & Session("pubUnidadesUsuario") & ") order by descripcion"
                    Call spLlenaComboBox(sqlStr, txtUnidadEmpresa, "idUnidad", "descripcion")
                    Call spLlenaComboBoxFecha("SELECT id as idMes, UPPER(descripcion) as descripcion from ccMeses", txtMesEmpresa, "idMes", "descripcion")
                    Call spLlenaComboBoxFecha("SELECT vano as idYear, vano as descripcion FROM ccAnos WHERE vano >= 2018", txtAnoEmpresa, "idYear", "descripcion")
                    txtMesEmpresa.SelectedIndex = DateTime.Now.Month - 1
                    txtAnoEmpresa.SelectedIndex = DateTime.Now.Year - 2018
                Catch ex As Exception
                    ShowPopUpMsg("Error carga superior: " + ex.ToString)
                End Try
            End Using
        End If
        If IsPostBack = False Then
            Session("BusquedaRealizada") = 0
        End If

        'CARGAR UNIDADES DE EMPRESA SOLO PARA PERFILES CORRECTOS
        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
            lblEmpresa.Visible = False
            cbxEmpresaPagina.Visible = False
            'CARGO LA UNIDAD INICIAL
            If IsPostBack = False Then
                cbxEmpresaPagina.Value = Session("pubEmpUsuariaUsuario")

                'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                            Call spLlenaComboBox(sqlStr, txtUnidadEmpresa, "idUnidad", "descripcion")
                        Catch ex As Exception
                            ShowPopUpMsg("Error 1: {0}" + ex.ToString)
                        End Try
                    End Using
                End If
            End If
        Else

            lblEmpresa.Visible = False
            cbxEmpresaPagina.Visible = False
        End If
        lblDesde.Visible = False
    End Sub

    'SELECCION DE REPORTES
    Protected Sub cbxTipo_SelectedIndexChanged(sender As Object, e As EventArgs)
        'BUSQUEDA DE REPORTES
        Session("BusquedaRealizada") = 0

        'ELIMINO LA TABLA ACTUAL SI EXISTE
        If Session("DotacionMensual") <> Nothing Then
            'ESCONDO GRILLA
            gridDotacionMensual.Visible = False
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
            Else
                ShowPopUpMsg("No hay conexión a la base de datos")
            End If
        Else
            'ShowPopUpMsg("Vuelva a realizar la búsqueda")
        End If


        Form.DefaultButton = Nothing
        Select Case cbxTipo.SelectedIndex
            'SIN SELECCION
            Case "0"
                Session("BusquedaRealizada") = 0
                Form.DefaultButton = Nothing
                gridSemanal.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                lblDesde.Visible = False
                lblEmpresa.Visible = False
                cbxEmpresaPagina.Visible = False
                gridDotacionMensual.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                Call Ocultar()

            'REPORTE ARAUCO
            Case "7"
                Session("BusquedaRealizada") = 0
                lblDesde.Visible = False
                gridSemanal.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                Call Mostrar()
                tr_excel.Visible = False
                Form.DefaultButton = btnConsultar.UniqueID
                txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    lblEmpresa.Visible = True
                    cbxEmpresaPagina.Visible = True
                End If

                'DOTACION SEMANAL
            Case "1"
                Session("BusquedaRealizada") = 0
                lblDesde.Visible = False
                gridSemanal.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                Call Ocultar()
                tr_excel.Visible = False
                tr_unidad.Visible = True
                tr_consultar.Visible = True
                Form.DefaultButton = btnConsultar.UniqueID
                txtUnidadEmpresa.ValidationSettings.CausesValidation = True
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = True
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    lblEmpresa.Visible = True
                    cbxEmpresaPagina.Visible = True
                End If

                'DOTACION MENSUAL
            Case "2"
                Session("BusquedaRealizada") = 0
                lblDesde.Visible = False
                gridSemanal.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                Call Mostrar()
                tr_excel.Visible = False
                Form.DefaultButton = btnConsultar.UniqueID
                txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    lblEmpresa.Visible = True
                    cbxEmpresaPagina.Visible = True
                End If

                'DOTACION ONLINE
            Case "3"
                Session("BusquedaRealizada") = 0
                lblDesde.Visible = False
                updatePanel2.Visible = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridSemanal.Visible = False
                UpdatePanel1.Visible = True
                Call CargarDotacion()
                Call Ocultar()
                tr_excel.Visible = False
                Form.DefaultButton = btnConsultar.UniqueID

                'DOTACION ONLINE CAUSAL
            Case "4"
                Session("BusquedaRealizada") = 0
                lblDesde.Visible = False
                updatePanel2.Visible = True
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridSemanal.Visible = False
                UpdatePanel1.Visible = False
                Call CargarDotacionCausal()
                Call Ocultar()
                tr_excel.Visible = False
                Form.DefaultButton = btnConsultar.UniqueID

                'REPORTE POR GENERO
            Case "5"
                lblDesde.Visible = False
                gridSemanal.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                Call Mostrar()
                tr_excel.Visible = False
                Form.DefaultButton = btnConsultar.UniqueID
                txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    lblEmpresa.Visible = True
                    cbxEmpresaPagina.Visible = True
                End If

            Case "6"
                lblDesde.Visible = True
                gridSemanal.Visible = False
                gridDatos.Visible = False
                gridSexo.Visible = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                Call Ocultar()
                tr_excel.Visible = False
                tr_unidad.Visible = True
                tr_consultar.Visible = True
                txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                Form.DefaultButton = btnConsultar.UniqueID
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    lblEmpresa.Visible = True
                    cbxEmpresaPagina.Visible = True
                End If
        End Select
    End Sub

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)
        Session("BusquedaRealizada") = 1

        'BORRO LA TABLA AL VOLVER A BUSCAR
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

        'REMUNERACIONES
        tr_excel.Visible = True
        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 10 Then
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = True
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = True
                        gridDotacionMensual.Visible = False
                        gridBucarTrabajadores.Visible = False
                        gridSemanal.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        gridDatos.PageIndex = 0
                        Dim sqlBase = String.Format("SELECT id as ID,unidad as Unidad,UPPER(sexo) as Sexo,idCentro as 'ID Centro',UPPER(descCentro) as Centro,codContrato as 
                                        'Código Contrato', division as División,UPPER(descArea) as Area,cargo as 'ID Cargo',UPPER(descCargo) as Cargo,cencos as 
                                        'ID Centro Costo', UPPER(descCentroCosto) as 'Centro Costo',clasif as Clasificación,UPPER(descCausaLegal) as 'Causa Legal',
                                        UPPER(ocupacion) as Ocupación,horario as Horario,jornada as Jornada,fechaNac as 'Fecha Nacimiento',fechaIng as 'Inicio Contrato Vigente',
                                        " & "fechaRet as 'Fecha Retiro',duracion as Duración, Codigo as Código,Rut,UPPER(Nombre) as Nombre,Estado,Ames as 'Año/Mes',
                                        SUBASE,DiasTr as 'Días Trabajados',CANHRSEXT as 'Cant. Horas Extras',HRSEXTRAS as 'Horas Extras',CANHRSEXT100 as 
                                        'Cant. Horas Extras 100', HRSEXT100 as 'Horas Extras 100',BonoFijo as 'Bono Fijo', BONARA,ASIESP,DIFSMA,BONCUM,DIFBON,BONRESP,
                                        GRATIFICACION as Gratificación," & "BONPRO,BONOTARDE,BONO6x2,BON4TU,BONONOCHE,ASICASA,BONASIST,OTROSBONOS,IMPONIBLE,COLACI,MOVILI,
                                        OTROSNOIMPONIBLES,Ktex,Ktfm,Ktha,RENTATRIBUT,IMPUES,RENTALIQUIDA,AGUINA," & "BONO18,BONVAC,BONNAV 
                                        FROM ccReporteArauco 
                                        WHERE idCentro='{0}' AND Ames='{1}'",
                                                    txtUnidadEmpresa.Value, txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString)

                        sqlSource = sqlBase
                        spllenaGridView(gridDatos, sqlBase)

                        If gridDatos.VisibleRowCount > 0 Then
                            'tr_excel.Visible = True
                            gridDatos.Visible = True
                        Else
                            'tr_excel.Visible = False
                            gridDatos.Visible = False
                        End If

                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error en Reporte 1: " + ex.ToString)
                End Try
            End Using
        End If

        'SEMANAL
        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 1 Then
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = True
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = True
                        gridDotacionMensual.Visible = False
                        gridBucarTrabajadores.Visible = False
                        gridDatos.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        gridSemanal.ClearSort()
                        gridSemanal.PageIndex = 0
                        Dim sqlBaseSemanal = String.Format("SELECT Usuaria,Planta,Area,Jefe,Nombre,Rut,Tarjeta, Cargo,Justificacion as Justificación,Motivo,
                                                [Fecha Inicio], [Fecha Ingreso],[Fecha Termino] as 'Fecha Término', Duracion as Duración, [Centro Costo],Localidad, 
                                                SueldoBase as 'Sueldo Base' 
                                                FROM ccReporteSemanal WHERE idCentro='{0}'", txtUnidadEmpresa.Value)
                        sqlSource = sqlBaseSemanal
                        spllenaGridView(gridSemanal, sqlBaseSemanal)

                        If gridSemanal.VisibleRowCount > 0 Then
                            'tr_excel.Visible = True
                            gridSemanal.Visible = True
                        Else
                            'tr_excel.Visible = False
                            gridSemanal.Visible = False
                        End If
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error en Reporte 2: " + ex.ToString)
                End Try
            End Using
        End If

        'DATO DE FECHA
        Dim connObtencionFecha As New SqlConnection
        Dim fecha = ""
        If ConectaSQLServerConn(connObtencionFecha) Then
            Using connObtencionFecha
                Try
                    sqlStr = "SELECT mesActual FROM ccMesProceso"
                    Dim sqlcmd As New SqlCommand(sqlStr, connObtencionFecha)
                    sqlcmd = New SqlCommand(sqlStr, connObtencionFecha)
                    fecha = sqlcmd.ExecuteScalar()
                    If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                        fecha = fecha
                    Else
                        fecha = ""
                    End If
                Catch ex As Exception

                End Try
            End Using
        End If

        'DOTACION MENSUAL
        'DECLARO DONDE VOY A ALMACENAR EL NOMBRE DE LA NUEVA TABLA
        Session("DotacionMensual") = Nothing

        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 2 Then
                        gridDatos.Visible = False
                        gridSexo.Visible = False
                        gridSemanal.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                        gridBucarTrabajadores.Visible = False
                        gridDotacionMensual.ClearSort()
                        gridDotacionMensual.PageIndex = 0
                        Dim sqlstr2 As String

                        Dim nombreTabla = Session("pubIdUsuario").ToString.Replace(".", "0")
                        Session("DotacionMensual") = "tmpDM" + nombreTabla + Now().ToString("HHmmss")

                        Dim connCreacionTablas As New SqlConnection

                        If ConectaSQLServerConn(connCreacionTablas) Then

                            Using connCreacionTablas
                                Try
                                    If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                                        'TODAS LAS EMPRESAS - FECHA ES IGUAL A FECHA ACTUAL TRABAJADA EN BD
                                        If txtUnidadEmpresa.SelectedIndex = 0 Then

                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensual WHERE idEmpusuaria IN ('{1}') ", Session("DotacionMensual"), cbxEmpresaPagina.Value)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHis WHERE idEmpusuaria IN ('{1}') and AMES ={2}", Session("DotacionMensual"), cbxEmpresaPagina.Value, txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        Else
                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensual WHERE idCentro='{1}'", Session("DotacionMensual"), txtUnidadEmpresa.Value)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHis WHERE idCentro='{1}' and AMES ={2}", Session("DotacionMensual"), txtUnidadEmpresa.Value, txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        End If
                                    Else
                                        'CLIENTES Y OPERACIONES
                                        'TODAS LAS EMPRESAS
                                        If txtUnidadEmpresa.SelectedIndex = 0 Then

                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensual WHERE idCentro IN ({1}) ", Session("DotacionMensual"), Session("pubUnidadesUsuario"))
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHis WHERE idCentro IN ({1}) and AMES ={2}", Session("DotacionMensual"), Session("pubUnidadesUsuario"), txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        Else
                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensual WHERE idCentro='{1}'", Session("DotacionMensual"), txtUnidadEmpresa.Value)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHis WHERE idCentro='{1}' and AMES ={2}", Session("DotacionMensual"), txtUnidadEmpresa.Value, txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        End If
                                    End If


                                Catch ex As Exception
                                    ShowPopUpMsg("Error en Reporte 3 - Crear Tablas " + ex.ToString)
                                    Return
                                End Try
                            End Using

                        End If

                        sqlStr = String.Format("SELECT * FROM {0}", Session("DotacionMensual"))

                        If ConectaSQLServer() Then
                            Using conn
                                Try

                                    Dim cmd As New SqlCommand(sqlStr, conn)
                                    Dim rdr As SqlDataReader = cmd.ExecuteReader()

                                    If rdr.HasRows Then
                                        While rdr.Read

                                            'CONSULTO LOS CONTRATOS PARA ESE RUT
                                            Dim conn2 As New SqlConnection
                                            If ConectaSQLServerConn(conn2) Then
                                                Using conn2

                                                    Dim sumDias As Integer = 0
                                                    Dim sumContratos As Integer = 1
                                                    Dim fechaIngAnterior As Date
                                                    Dim motivoAnterior As Integer = 0
                                                    Dim esPrimerRegistro As Boolean = True


                                                    sqlstr2 = $"SELECT * FROM ccViewContratosHis WHERE rut='{rdr("rut")}' order by rut, actual DESC, ames desc, fecha_ret DESC"
                                                    Try


                                                        Dim cmd2 As New SqlCommand(sqlstr2, conn2)
                                                        Dim rdr2 As SqlDataReader = cmd2.ExecuteReader()

                                                        If rdr2.HasRows Then
                                                            While rdr2.Read
                                                                If esPrimerRegistro Then
                                                                    fechaIngAnterior = rdr2("fecha_ing")
                                                                    sumDias = CInt(rdr2("duracion"))
                                                                    motivoAnterior = CInt(rdr2("clasif"))
                                                                    esPrimerRegistro = vbFalse
                                                                Else
                                                                    If motivoAnterior = CInt(rdr2("clasif")) Then
                                                                        If fechaIngAnterior <> rdr2("fecha_ing") Then
                                                                            If (DateDiff(DateInterval.Day, fechaIngAnterior, rdr2("fecha_ret")) + 1 = 0) Then
                                                                                fechaIngAnterior = rdr2("fecha_ing")
                                                                                sumDias = CInt(rdr2("duracion")) + sumDias
                                                                                sumContratos = sumContratos + 1
                                                                            End If
                                                                        End If
                                                                    Else
                                                                        Exit While
                                                                    End If
                                                                End If

                                                            End While

                                                        End If
                                                    Catch ex As Exception
                                                        ShowPopUpMsg("Error en Reporte 3 - Consultar Contratos Rut " + ex.ToString)
                                                    End Try

                                                    'ACTUALIZAR VALOR DE DIAS

                                                    Dim conn4 As New SqlConnection
                                                    If ConectaSQLServerConn(conn4) Then
                                                        Using conn4
                                                            Try
                                                                sqlStr = $"UPDATE {Session("DotacionMensual")} SET diasCausal={sumDias}, nroContratos={sumContratos} WHERE rut='{rdr("rut")}' "
                                                                Dim sqlcmd3 As New SqlCommand(sqlStr, conn4)
                                                                sqlcmd3.CommandTimeout = 480
                                                                sqlcmd3.ExecuteNonQuery()

                                                            Catch ex As Exception
                                                                ShowPopUpMsg("Error en Reporte 3 - Actualizar Valor Días " + ex.ToString)
                                                                'Return
                                                            End Try
                                                        End Using
                                                    End If

                                                    '*****************************
                                                End Using
                                            End If

                                        End While
                                    End If

                                Catch ex As Exception
                                    ShowPopUpMsg("Error en Reporte 3 - Conecta SQL " + ex.ToString)
                                End Try
                            End Using
                        End If

                        'ARAUCO
                        'Dim sqlBase As String = $"SELECT Usuaria,Planta,Area,Jefe,Nombre,Rut,Cargo,Justificacion,Motivo,
                        'Convert(varchar, fechaini, 105)  As 'Fecha Inicio',	convert(varchar, fecha_ing, 105) as 'Fecha Ingreso', convert(varchar, fecha_ret, 105) as 'Fecha Termino', Duracion, diasCausal, nroContratos, [Centro Costo],Localidad, SueldoBase FROM {strName} order by Nombre"

                        If fecha = "" Then
                            Dim sqlBase As String = $"SELECT Usuaria, Planta,Area,Jefe,  Nombre,Rut,Tarjeta,Cargo,Justificacion as Justificación,Motivo, 
                                    convert(varchar,fecha_nac,105)  as 'Fecha Nacimiento', nacionalidad as Nacionalidad,
                                    convert(varchar,fechaini,105)  as 'Inicio Contrato Histórico ',	convert(varchar, fecha_ing, 105) as 'Inicio Contrato Vigente', 
                                    convert(varchar, fecha_ret, 105) as 'Término Contrato Vigente', Duracion as 'Días Contrato Vigente', diasCausal as 'Días Acumulados Misma Causal',
                                    nroContratos as 'N° Contratos Acumulados  Misma Causal', [Centro Costo],Localidad, SueldoBase as 'Sueldo Base' 
                                    FROM {Session("DotacionMensual")}
                                    WHERE ames = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString} 
                                    order by Nombre"
                            Dim conn5 As New SqlConnection
                            If ConectaSQLServerConn(conn5) Then
                                Using conn5
                                    Try
                                        sqlSource = sqlBase
                                        spllenaGridView(gridDotacionMensual, sqlBase)
                                        If gridDotacionMensual.VisibleRowCount > 0 Then
                                            'tr_excel.Visible = True
                                            gridDotacionMensual.Visible = True
                                        Else
                                            'tr_excel.Visible = False
                                            gridDotacionMensual.Visible = False
                                        End If

                                        If cbxTipo.SelectedIndex = 5 Then

                                        End If
                                    Catch ex As Exception
                                        ShowPopUpMsg(ex.ToString)
                                    End Try
                                End Using
                            End If
                        Else
                            Dim sqlBase As String = $"SELECT Usuaria, Planta,Area,Jefe,  Nombre,Rut,Tarjeta,Cargo,Justificacion as Justificación,Motivo, 
                                    convert(varchar,fecha_nac,105)  as 'Fecha Nacimiento', nacionalidad as Nacionalidad,
                                    convert(varchar,fechaini,105)  as 'Inicio Contrato Histórico ',	convert(varchar, fecha_ing, 105) as 'Inicio Contrato Vigente', 
                                    convert(varchar, fecha_ret, 105) as 'Término Contrato Vigente', Duracion as 'Días Contrato Vigente', diasCausal as 'Días Acumulados Misma Causal',
                                    nroContratos as 'N° Contratos Acumulados  Misma Causal', [Centro Costo],Localidad, SueldoBase as 'Sueldo Base' 
                                    FROM {Session("DotacionMensual")}
                                    order by Nombre"
                            Dim conn5 As New SqlConnection
                            If ConectaSQLServerConn(conn5) Then
                                Using conn5
                                    Try
                                        sqlSource = sqlBase
                                        spllenaGridView(gridDotacionMensual, sqlBase)
                                        If gridDotacionMensual.VisibleRowCount > 0 Then
                                            'tr_excel.Visible = True
                                            gridDotacionMensual.Visible = True
                                        Else
                                            'tr_excel.Visible = False
                                            gridDotacionMensual.Visible = False
                                        End If

                                        If cbxTipo.SelectedIndex = 5 Then

                                        End If
                                    Catch ex As Exception
                                        ShowPopUpMsg(ex.ToString)
                                    End Try
                                End Using
                            End If
                        End If
                    End If
                Catch ex As Exception
                    'ShowPopUpMsg("Error en Reporte 3 - Primer Conecta SQL " + ex.ToString)
                End Try
            End Using
        End If

        '********************* REPORTE ARAUCO********************************************************************

        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 7 Then
                        gridDatos.Visible = False
                        gridSexo.Visible = False
                        gridSemanal.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                        gridBucarTrabajadores.Visible = False
                        gridDotacionMensual.ClearSort()
                        gridDotacionMensual.PageIndex = 0
                        Dim sqlstr2 As String

                        Dim nombreTabla = Session("pubIdUsuario").ToString.Replace(".", "0")
                        Session("DotacionMensual") = "tmpDM" + nombreTabla + Now().ToString("HHmmss")

                        Dim connCreacionTablas As New SqlConnection

                        If ConectaSQLServerConn(connCreacionTablas) Then

                            Using connCreacionTablas
                                Try
                                    If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                                        'TODAS LAS EMPRESAS - FECHA ES IGUAL A FECHA ACTUAL TRABAJADA EN BD
                                        If txtUnidadEmpresa.SelectedIndex = 0 Then

                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualArauco WHERE DiasTrabajados>0 AND idEmpusuaria IN ('{1}') ", Session("DotacionMensual"), cbxEmpresaPagina.Value)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHisArauco WHERE DiasTrabajados>0 AND idEmpusuaria IN ('{1}') and ANIOMES ={2}", Session("DotacionMensual"), cbxEmpresaPagina.Value, txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        Else
                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualArauco WHERE DiasTrabajados>0 AND idCentro='{1}'", Session("DotacionMensual"), txtUnidadEmpresa.Value)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHisArauco WHERE DiasTrabajados>0 AND idCentro='{1}' and ANIOMES ={2}", Session("DotacionMensual"), txtUnidadEmpresa.Value, txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        End If
                                    Else
                                        'CLIENTES Y OPERACIONES
                                        'TODAS LAS EMPRESAS
                                        If txtUnidadEmpresa.SelectedIndex = 0 Then

                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualArauco WHERE DiasTrabajados>0 AND idCentro IN ({1}) ", Session("DotacionMensual"), Session("pubUnidadesUsuario"))
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHisArauco WHERE DiasTrabajados>0 AND idCentro IN ({1}) and ANIOMES ={2}", Session("DotacionMensual"), Session("pubUnidadesUsuario"), txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        Else
                                            If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualArauco WHERE DiasTrabajados>0 AND idCentro='{1}'", Session("DotacionMensual"), txtUnidadEmpresa.Value)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            Else
                                                sqlStr = String.Format("SELECT * INTO {0} FROM ccReporteDotacionMensualHisArauco WHERE DiasTrabajados>0 AND idCentro='{1}' and ANIOMES ={2}", Session("DotacionMensual"), txtUnidadEmpresa.Value, txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString)
                                                Dim sqlcmd As New SqlCommand(sqlStr, connCreacionTablas)
                                                sqlcmd.CommandTimeout = 480
                                                sqlcmd.ExecuteNonQuery()
                                            End If

                                        End If
                                    End If


                                Catch exc As Exception
                                    ShowPopUpMsg("Error en Reporte 7 - Crear Tablas " + exc.ToString)
                                    Return
                                End Try
                            End Using

                        End If

                        'CORREGIR FECHA_INI DE LA TABLA TEMPORAL
                        sqlStr = String.Format("UPDATE {0} SET fechaPrimerContrato=fecha_ing WHERE fechaPrimerContrato IS NULL", Session("DotacionMensual"))
                        Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                        sqlcmd2.CommandTimeout = 480
                        sqlcmd2.ExecuteNonQuery()

                        'ACTUALIZAR INFO DE CONTRATOS HISTORICOS
                        sqlStr = String.Format("SELECT * FROM {0}", Session("DotacionMensual"))

                        If ConectaSQLServer() Then
                            Using conn
                                Try

                                    Dim cmd As New SqlCommand(sqlStr, conn)
                                    Dim rdr As SqlDataReader = cmd.ExecuteReader()

                                    If rdr.HasRows Then
                                        While rdr.Read

                                            'CONSULTO LOS CONTRATOS PARA ESE RUT
                                            Dim conn2 As New SqlConnection
                                            If ConectaSQLServerConn(conn2) Then
                                                Using conn2

                                                    Dim sumDias As Integer = 0
                                                    Dim sumContratos As Integer = 1
                                                    Dim fechaIngAnterior As Date
                                                    Dim motivoAnterior As Integer = 0
                                                    Dim esPrimerRegistro As Boolean = True


                                                    sqlstr2 = $"SELECT * FROM ccViewContratosHis WHERE rut='{rdr("rut")}' order by rut, actual DESC, ames desc, fecha_ret DESC"
                                                    Try


                                                        Dim cmd2 As New SqlCommand(sqlstr2, conn2)
                                                        Dim rdr2 As SqlDataReader = cmd2.ExecuteReader()

                                                        If rdr2.HasRows Then
                                                            While rdr2.Read
                                                                If esPrimerRegistro Then
                                                                    fechaIngAnterior = rdr2("fecha_ing")
                                                                    sumDias = CInt(rdr2("duracion"))
                                                                    motivoAnterior = CInt(rdr2("clasif"))
                                                                    esPrimerRegistro = vbFalse
                                                                Else
                                                                    If motivoAnterior = CInt(rdr2("clasif")) Then
                                                                        If fechaIngAnterior <> rdr2("fecha_ing") Then
                                                                            If (DateDiff(DateInterval.Day, fechaIngAnterior, rdr2("fecha_ret")) + 1 = 0) Then
                                                                                fechaIngAnterior = rdr2("fecha_ing")
                                                                                sumDias = CInt(rdr2("duracion")) + sumDias
                                                                                sumContratos = sumContratos + 1
                                                                            End If
                                                                        End If
                                                                    Else
                                                                        Exit While
                                                                    End If
                                                                End If

                                                            End While

                                                        End If
                                                    Catch ex As Exception
                                                        ShowPopUpMsg("Error en Reporte 3 - Consultar Contratos Rut " + ex.ToString)
                                                    End Try

                                                    'ACTUALIZAR VALOR DE DIAS

                                                    Dim conn4 As New SqlConnection
                                                    If ConectaSQLServerConn(conn4) Then
                                                        Using conn4
                                                            Try
                                                                sqlStr = $"UPDATE {Session("DotacionMensual")} SET nroRenovaciones={sumContratos} WHERE rut='{rdr("rut")}' "
                                                                Dim sqlcmd3 As New SqlCommand(sqlStr, conn4)
                                                                sqlcmd3.CommandTimeout = 480
                                                                sqlcmd3.ExecuteNonQuery()

                                                            Catch ex As Exception
                                                                ShowPopUpMsg("Error en Reporte 3 - Actualizar Valor Días " + ex.ToString)
                                                                'Return
                                                            End Try
                                                        End Using
                                                    End If

                                                    '*****************************
                                                End Using
                                            End If

                                        End While
                                    End If

                                Catch ex As Exception
                                    ShowPopUpMsg("Error en Reporte 3 - Conecta SQL " + ex.ToString)
                                End Try
                            End Using
                        End If

                        'ARAUCO
                        'Dim sqlBase As String = $"SELECT Usuaria,Planta,Area,Jefe,Nombre,Rut,Cargo,Justificacion,Motivo,
                        'Convert(varchar, fechaini, 105)  As 'Fecha Inicio',	convert(varchar, fecha_ing, 105) as 'Fecha Ingreso', convert(varchar, fecha_ret, 105) as 'Fecha Termino', Duracion, diasCausal, nroContratos, [Centro Costo],Localidad, SueldoBase FROM {strName} order by Nombre"

                        If fecha = "" Then
                            Dim sqlBase As String
                            sqlBase = $"SELECT AreaPlanta as 'AREA PLANTA', aniomes as 'Mes', Usuaria as 'Operaciones', idCentro as 'Centro', Planta, Unidad, Area,
                                TipoServicio as 'Tipo Servicio', RutE as 'Rut E.', RazonSocial as ' Razón Social', rut as 'Rut P.', nombre as 'Empleado',
                                DiasTrabajados as 'DIAS TRABAJADOS', HH, perMes as 'Per/Mes', Justificacion as 'Servicio' ,NroRenovaciones as 'Nro Renovaciones',
                                FechaPrimerContrato as 'Fecha Inicio 1er Contrato', FechaTermino as 'Fecha Término Contrato'
                            FROM {Session("DotacionMensual")}
                            WHERE ANIOMES = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString} 
                            order by idCentro, Nombre"

                            ' sqlBase = $"SELECT * 
                            'FROM {Session("DotacionMensual")}
                            'WHERE ames = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString} 
                            'order by Nombre"


                            Dim conn5 As New SqlConnection
                            If ConectaSQLServerConn(conn5) Then
                                Using conn5
                                    Try
                                        sqlSource = sqlBase
                                        spllenaGridView(gridDotacionMensual, sqlBase)
                                        If gridDotacionMensual.VisibleRowCount > 0 Then
                                            'tr_excel.Visible = True
                                            gridDotacionMensual.Visible = True
                                        Else
                                            'tr_excel.Visible = False
                                            gridDotacionMensual.Visible = False
                                        End If

                                        If cbxTipo.SelectedIndex = 5 Then

                                        End If
                                    Catch ex As Exception
                                        ShowPopUpMsg(ex.ToString)
                                    End Try
                                End Using
                            End If
                        Else
                            ' Dim sqlBase As String = $"SELECT Usuaria, Planta,Area,Jefe,  Nombre,Rut,Tarjeta,Cargo,Justificacion as Justificación,Motivo, 
                            'Convert(varchar,fecha_nac,105)  as 'Fecha Nacimiento', nacionalidad as Nacionalidad,
                            'Convert(varchar,fechaini,105)  as 'Inicio Contrato Histórico ',	convert(varchar, fecha_ing, 105) as 'Inicio Contrato Vigente', 
                            'Convert(varchar, fecha_ret, 105) as 'Término Contrato Vigente', Duracion as 'Días Contrato Vigente', diasCausal as 'Días Acumulados Misma Causal',
                            'nroContratos as 'N° Contratos Acumulados  Misma Causal', [Centro Costo],Localidad, SueldoBase as 'Sueldo Base' 
                            'FROM {Session("DotacionMensual")}
                            'order by Nombre"

                            Dim sqlBase As String = $"SELECT AreaPlanta as 'AREA PLANTA', aniomes as 'Mes', Usuaria as 'Operaciones', idCentro as 'Centro', Planta, Unidad, Area,
                                TipoServicio as 'Tipo Servicio', RutE as 'Rut E.', RazonSocial as ' Razón Social', rut as 'Rut P.', nombre as 'Empleado',
                                DiasTrabajados as 'DIAS TRABAJADOS', HH, perMes as 'Per/Mes', Justificacion as 'Servicio' ,NroRenovaciones as 'Nro Renovaciones',
                                FechaPrimerContrato as 'Fecha Inicio 1er Contrato', FechaTermino as 'Fecha Término Contrato'
                            FROM {Session("DotacionMensual")}                           
                            order by idCentro, Nombre"

                            ' Dim sqlBase As String = $"SELECT *
                            'FROM {Session("DotacionMensual")}
                            'order by Nombre"

                            Dim conn5 As New SqlConnection
                            If ConectaSQLServerConn(conn5) Then
                                Using conn5
                                    Try
                                        sqlSource = sqlBase
                                        spllenaGridView(gridDotacionMensual, sqlBase)
                                        If gridDotacionMensual.VisibleRowCount > 0 Then
                                            'tr_excel.Visible = True
                                            gridDotacionMensual.Visible = True
                                        Else
                                            'tr_excel.Visible = False
                                            gridDotacionMensual.Visible = False
                                        End If

                                        If cbxTipo.SelectedIndex = 5 Then

                                        End If
                                    Catch ex As Exception
                                        ShowPopUpMsg(ex.ToString)
                                    End Try
                                End Using
                            End If
                        End If
                    End If
                Catch ex As Exception
                    'ShowPopUpMsg("Error en Reporte 3 - Primer Conecta SQL " + ex.ToString)
                End Try
            End Using
        End If


        '*****************************************************************************************

        Try
            If cbxTipo.SelectedIndex = 5 Then
                txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                gridSemanal.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                gridSexo.ClearSort()
                gridSexo.PageIndex = 0

                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    If txtUnidadEmpresa.SelectedIndex = 0 Then

                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA, 
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idEmpusuaria IN ('{cbxEmpresaPagina.Value}')
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ('{cbxEmpresaPagina.Value}') and REMPLESh.AMES ={txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        End If
                        'SOLO 1 EMPRESA
                    Else
                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA,
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idCentro IN ('{txtUnidadEmpresa.Value}')
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ('{txtUnidadEmpresa.Value}') AND REMPLESH.AMES = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"

                            spllenaGridView(gridSexo, sqlStr)
                        End If

                    End If
                Else
                    'TODAS LAS EMPRESAS
                    If txtUnidadEmpresa.SelectedIndex = 0 Then

                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA, 
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idCentro IN ({Session("pubUnidadesUsuario")})
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ({Session("pubUnidadesUsuario")}) and REMPLESh.AMES ={txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        End If
                        'SOLO 1 EMPRESA
                    Else
                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA,
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idCentro IN ('{txtUnidadEmpresa.Value}')
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ('{txtUnidadEmpresa.Value}') AND REMPLESH.AMES = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"

                            spllenaGridView(gridSexo, sqlStr)
                        End If

                    End If
                End If

                If gridSexo.VisibleRowCount > 0 Then
                    'tr_excel.Visible = True
                    gridSexo.Visible = True
                Else
                    'tr_excel.Visible = False
                    gridSexo.Visible = False
                End If

            End If
        Catch ex As Exception
            ShowPopUpMsg("Error en Reporte 1: " + ex.ToString)
        End Try

        Try
            If cbxTipo.SelectedIndex = 6 Then
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                gridSemanal.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False
                gridBucarTrabajadores.ClearSort()
                gridBucarTrabajadores.PageIndex = 0

                'QUERY QUE AGREGA UNIDADES EN CICLO
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    'SIN SELECCION DE UNIDAD
                    If txtUnidadEmpresa.SelectedIndex = 0 Then

                        sqlStr = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE idEmpusuaria = '{cbxEmpresaPagina.Value}'
                                    ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, sqlStr)

                        'SOLO 1 UNIDAD
                    Else
                        sqlStr = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE id = '{txtUnidadEmpresa.Value}'
                                    ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, sqlStr)

                    End If
                Else
                    'SOLO UNIDADES CORRESPONDIENTES A CLIENTE
                    If txtUnidadEmpresa.SelectedIndex = 0 Then
                        'TODAS LAS UNIDADES

                        Dim arreglounidades() As String = Session("pubUnidadesUsuario").ToString().Split(",")
                        Dim cadena1sql = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE id = {arreglounidades(0)}"

                        If arreglounidades.Count > 1 Then
                            For index = 1 To arreglounidades.Count - 1
                                cadena1sql = cadena1sql + $" UNION SELECT * FROM [dbo].[diViewContrHisActivosMaxames] where id = {arreglounidades(index)}"
                            Next

                        End If
                        cadena1sql = cadena1sql + " ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, cadena1sql)
                        'SOLO 1 EMPRESA
                    Else

                        sqlStr = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE id = '{txtUnidadEmpresa.Value}'
                                    ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, sqlStr)

                    End If
                End If

                If gridBucarTrabajadores.VisibleRowCount > 0 Then
                    'tr_excel.Visible = True
                    gridBucarTrabajadores.Visible = True

                Else
                    'tr_excel.Visible = False
                    gridBucarTrabajadores.Visible = False
                End If
            End If
        Catch ex As Exception
            ShowPopUpMsg("Error en Reporte 6: " + ex.ToString)
        End Try
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs)

        Call CargarReportes()


        If cbxTipo.SelectedIndex = 1 Then
            gridExportarSemanal.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
        End If

        If cbxTipo.SelectedIndex = 2 Then
            GridExportarDotacionMensual.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
        End If

        If cbxTipo.SelectedIndex = 7 Then
            GridExportarDotacionMensual.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
        End If

        If cbxTipo.SelectedIndex = 5 Then
            GridExportarSexo.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
        End If

        If cbxTipo.SelectedIndex = 6 Then
            GridExportarBuscarTrabajadores.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
        End If

    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Sub Mostrar()
        tr_unidad.Visible = True
        tr_ano.Visible = True
        tr_mes.Visible = True
        tr_consultar.Visible = True
        tr_excel.Visible = True
    End Sub

    Sub Ocultar()
        tr_unidad.Visible = False
        tr_ano.Visible = False
        tr_mes.Visible = False
        tr_consultar.Visible = False
        tr_excel.Visible = False
    End Sub

    Sub CargarDotacionCausal()
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA DOTACION CAUSAL
                    Dim restrictedTypes() As ViewType = {ViewType.PolarArea, ViewType.PolarLine, ViewType.ScatterPolarLine, ViewType.SideBySideGantt, ViewType.SideBySideRangeBar, ViewType.RangeBar, ViewType.Gantt, ViewType.PolarPoint, ViewType.Stock, ViewType.CandleStick, ViewType.Bubble}
                    ChartTypeCausal.Items.Clear()
                    ChartTypeCausal.Items.Add("Bar")
                    ChartTypeCausal.Items.Add("Line")
                    ChartTypeCausal.Items.Add("Pie")
                    ChartTypeCausal.SelectedItem = ChartTypeCausal.Items.FindByText(ViewType.Bar.ToString())
                    SetChartCausalType(ChartTypeCausal.SelectedItem.Text)

                    PointLabelsCausal.Checked = WebChartCausal.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True
                    ASPxPivotGridCausal.OptionsChartDataSource.ProvideColumnGrandTotals = ShowColumnGrandTotalsCausal.Checked
                    ASPxPivotGridCausal.OptionsChartDataSource.ProvideRowGrandTotals = ShowRowGrandTotalsCausal.Checked
                    ASPxPivotGridCausal.OptionsChartDataSource.ProvideDataByColumns = ChartDataVerticalCausal.Checked
                    SqlDataSource2.SelectCommand = String.Format("SELECT [id], [ames], [descripcion], [Descrip], [dotacion] FROM [ccRptDotacionCausal] WHERE [idUnidad] IN({0})", Session("pubUnidadesUsuario"))

                Catch ex As Exception
                    ShowPopUpMsg("Dotación Causal: " + ex.ToString)
                End Try
            End Using
        End If
    End Sub

    Sub CargarDotacion()
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA DOTACION'
                    Dim restrictedTypes() As ViewType = {ViewType.PolarArea, ViewType.PolarLine, ViewType.ScatterPolarLine, ViewType.SideBySideGantt, ViewType.SideBySideRangeBar, ViewType.RangeBar, ViewType.Gantt, ViewType.PolarPoint, ViewType.Stock, ViewType.CandleStick, ViewType.Bubble}

                    ChartType.Items.Clear()
                    ChartType.Items.Add("Bar")
                    ChartType.Items.Add("Line")
                    ChartType.Items.Add("Pie")

                    ChartType.SelectedItem = ChartType.Items.FindByText(ViewType.Bar.ToString())
                    SetChartType(ChartType.SelectedItem.Text)
                    PointLabels.Checked = WebChart.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True
                    ASPxPivotGrid.OptionsChartDataSource.ProvideColumnGrandTotals = ShowColumnGrandTotals.Checked
                    ASPxPivotGrid.OptionsChartDataSource.ProvideRowGrandTotals = ShowRowGrandTotals.Checked
                    ASPxPivotGrid.OptionsChartDataSource.ProvideDataByColumns = ChartDataVertical.Checked
                    SqlDataSource1.SelectCommand = String.Format("SELECT [id], [ames], [descripcion], [dotacion] FROM [ccRptDotacion] WHERE [idUnidad] IN({0})", Session("pubUnidadesUsuario"))

                Catch ex As Exception
                    ShowPopUpMsg("Dotación : " + ex.ToString)
                End Try
            End Using
        End If
    End Sub

    'UPDATE PANEL DOTACION
#Region "Reporte ONLINE"



    Private Sub SetFilter(ByVal field As PivotGridField, ByVal selectNumber As Integer)
        Try
            Dim values() As Object = field.GetUniqueValues()
            Dim includedValues As New List(Of Object)(values.Length / selectNumber)
            For i As Integer = 0 To values.Length - 1
                If i Mod selectNumber = 0 Then
                    includedValues.Add(values(i))
                End If
            Next i
            field.FilterValues.ValuesIncluded = includedValues.ToArray()
        Catch ex As Exception
            ShowPopUpMsg("UPDATE DOTACION PANEL : " + ex.ToString)
        End Try

    End Sub

    Private Sub SetChartType(ByVal text As String)
        Try
            WebChart.SeriesTemplate.ChangeView(CType(System.Enum.Parse(GetType(ViewType), text), ViewType))
            If WebChart.SeriesTemplate.Label IsNot Nothing Then
                PointLabels.Enabled = True
                WebChart.SeriesTemplate.LabelsVisibility = If(PointLabels.Checked, DevExpress.Utils.DefaultBoolean.True, DevExpress.Utils.DefaultBoolean.False)
            Else
                PointLabels.Enabled = False
            End If
        Catch ex As Exception
            ShowPopUpMsg("CHART TYPE : " + ex.ToString)
        End Try

    End Sub

    Protected Sub ChartType_ValueChanged(sender As Object, e As EventArgs)
        Try
            SetChartType(ChartType.SelectedItem.Text)
        Catch ex As Exception
            ShowPopUpMsg("CHART TYPE VALUE : " + ex.ToString)
        End Try


    End Sub

    Protected Sub ShowColumnGrandTotals_CheckedChanged(sender As Object, e As EventArgs)
        Try
            ASPxPivotGrid.OptionsChartDataSource.ProvideColumnGrandTotals = ShowColumnGrandTotals.Checked

        Catch ex As Exception
            ShowPopUpMsg("COLUMN GRAND TOTAL: " + ex.ToString)
        End Try

    End Sub

    Protected Sub ChartDataVertical_CheckedChanged(sender As Object, e As EventArgs)
        Try
            ASPxPivotGrid.OptionsChartDataSource.ProvideDataByColumns = ChartDataVertical.Checked
        Catch ex As Exception
            ShowPopUpMsg("CHART DATA VERTICAL: " + ex.ToString)
        End Try


    End Sub

    Protected Sub PointLabels_CheckedChanged(sender As Object, e As EventArgs)
        Try
            WebChart.SeriesTemplate.LabelsVisibility = If(PointLabels.Checked, DevExpress.Utils.DefaultBoolean.True, DevExpress.Utils.DefaultBoolean.False)

        Catch ex As Exception
            ShowPopUpMsg("POINT LABELS: " + ex.ToString)
        End Try

    End Sub

    Protected Sub ShowRowGrandTotals_CheckedChanged(sender As Object, e As EventArgs)
        Try
            ASPxPivotGrid.OptionsChartDataSource.ProvideRowGrandTotals = ShowRowGrandTotals.Checked

        Catch ex As Exception
            ShowPopUpMsg("ROW GRAND TOTAL: " + ex.ToString)
        End Try

    End Sub

    Protected Sub ASPxPivotGrid_PreRender(sender As Object, e As EventArgs)
        '      If (Not IsPostBack) Then
        '     SetFilter(fielddescripcion, 4)
        '    fieldOrderYear.FilterValues.SetValues(New Object() {1996}, DevExpress.XtraPivotGrid.PivotFilterType.Included, False)
        '   End If
    End Sub
    'UPDATE PANEL DOTACION

    'UPDATE PANEL DOTACION-CAUSAL
    Private Sub SetChartCausalType(ByVal text As String)
        Try
            WebChartCausal.SeriesTemplate.ChangeView(CType(System.Enum.Parse(GetType(ViewType), text), ViewType))
            If WebChartCausal.SeriesTemplate.Label IsNot Nothing Then
                PointLabelsCausal.Enabled = True
                WebChartCausal.SeriesTemplate.LabelsVisibility = If(PointLabelsCausal.Checked, DevExpress.Utils.DefaultBoolean.True, DevExpress.Utils.DefaultBoolean.False)
            Else
                PointLabelsCausal.Enabled = False
            End If
        Catch ex As Exception
            ShowPopUpMsg("UPDTE DOTACION PANEL 2: " + ex.ToString)
        End Try

    End Sub

    Protected Sub ChartTypeCausal_ValueChanged(sender As Object, e As EventArgs)
        Try
            SetChartCausalType(ChartTypeCausal.SelectedItem.Text)

        Catch ex As Exception
            ShowPopUpMsg("CHART TYPE 2: " + ex.ToString)
        End Try

    End Sub

    Protected Sub ShowColumnGrandTotalsCausal_CheckedChanged(sender As Object, e As EventArgs)
        Try
            ASPxPivotGridCausal.OptionsChartDataSource.ProvideColumnGrandTotals = ShowColumnGrandTotalsCausal.Checked

        Catch ex As Exception
            ShowPopUpMsg("COLUMN GRAND TOITAL 2: " + ex.ToString)
        End Try

    End Sub

    Protected Sub ChartDataVerticalCausal_CheckedChanged(sender As Object, e As EventArgs)
        Try
            ASPxPivotGridCausal.OptionsChartDataSource.ProvideDataByColumns = ChartDataVerticalCausal.Checked

        Catch ex As Exception
            ShowPopUpMsg("vertical data 2: " + ex.ToString)

        End Try

    End Sub

    Protected Sub PointLabelsCausal_CheckedChanged(sender As Object, e As EventArgs)
        Try
            WebChartCausal.SeriesTemplate.LabelsVisibility = If(PointLabelsCausal.Checked, DevExpress.Utils.DefaultBoolean.True, DevExpress.Utils.DefaultBoolean.False)

        Catch ex As Exception
            ShowPopUpMsg("POINT LABEL 2: " + ex.ToString)

        End Try

    End Sub

    Protected Sub ShowRowGrandTotalsCausal_CheckedChanged(sender As Object, e As EventArgs)
        Try
            ASPxPivotGridCausal.OptionsChartDataSource.ProvideRowGrandTotals = ShowRowGrandTotalsCausal.Checked

        Catch ex As Exception
            ShowPopUpMsg("TOTAL CAUSAL 2: " + ex.ToString)

        End Try

    End Sub

    Protected Sub ASPxPivotGridCausal_PreRender(sender As Object, e As EventArgs)
        '      If (Not IsPostBack) Then
        '     SetFilter(fielddescripcion, 4)
        '    fieldOrderYear.FilterValues.SetValues(New Object() {1996}, DevExpress.XtraPivotGrid.PivotFilterType.Included, False)
        '   End If
    End Sub

    'SOLUCION SCRIPT REGISTER -- AGREGAR EVENTO UNLOAD EN LOS UPDATEPANELS AFECTADOS
    Protected Sub UpdatePanel1_Unload(sender As Object, e As EventArgs)
        Try
            RegisterUpdatePanel(TryCast(sender, UpdatePanel))
        Catch ex As Exception
            ShowPopUpMsg(ex.ToString)
        End Try

    End Sub

    Protected Sub RegisterUpdatePanel(panel As UpdatePanel)
        Try
            For Each methodInfo As Reflection.MethodInfo In GetType(ScriptManager).GetMethods(BindingFlags.NonPublic Or BindingFlags.Instance)
                If methodInfo.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel") Then
                    methodInfo.Invoke(ScriptManager.GetCurrent(Page), New Object() {panel})
                End If
            Next
        Catch ex As Exception
            ShowPopUpMsg("REGISTER UPDATE PANEL: " + ex.ToString)

        End Try

    End Sub
#End Region


    Protected Sub gridDatos_PageIndexChanged(sender As Object, e As EventArgs)
        Session("BusquedaRealizada") = 1
        Call CargarReportes()

    End Sub

    Protected Sub gridReporteArauco_PageIndexChanged(sender As Object, e As EventArgs)
        Session("BusquedaRealizada") = 1
        Call CargarReportes()

    End Sub

    Protected Sub gridBucarTrabajadores_PageIndexChanged(sender As Object, e As EventArgs)
        Call CargarReportes()
    End Sub

    'LLENA GRID
    Private Sub CargarReportes()

        'DATO DE FECHA
        Dim connObtencionFecha As New SqlConnection
        Dim fecha = ""
        If ConectaSQLServerConn(connObtencionFecha) Then
            Using connObtencionFecha
                Try
                    sqlStr = "SELECT mesActual FROM ccMesProceso"
                    Dim sqlcmd As New SqlCommand(sqlStr, connObtencionFecha)
                    sqlcmd = New SqlCommand(sqlStr, connObtencionFecha)
                    fecha = sqlcmd.ExecuteScalar()
                    'If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                    '    fecha = fecha
                    'Else
                    '    fecha = ""
                    'End If
                Catch ex As Exception

                End Try
            End Using
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 10 Then
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = True
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = True
                        gridDotacionMensual.Visible = False
                        gridBucarTrabajadores.Visible = False
                        gridSemanal.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        Dim sqlBase = String.Format("SELECT id as ID,unidad as Unidad,UPPER(sexo) as Sexo,idCentro as 'ID Centro',UPPER(descCentro) as Centro,codContrato as 
                                        'Código Contrato', division as División,UPPER(descArea) as Area,cargo as 'ID Cargo',UPPER(descCargo) as Cargo,cencos as 
                                        'ID Centro Costo', UPPER(descCentroCosto) as 'Centro Costo',clasif as Clasificación,UPPER(descCausaLegal) as 'Causa Legal',
                                        UPPER(ocupacion) as Ocupación,horario as Horario,jornada as Jornada,fechaNac as 'Fecha Nacimiento',fechaIng as 'Inicio Contrato Vigente',
                                        " & "fechaRet as 'Fecha Retiro',duracion as Duración, Codigo as Código,Rut,UPPER(Nombre) as Nombre,Estado,Ames as 'Año/Mes',
                                        SUBASE,DiasTr as 'Días Trabajados',CANHRSEXT as 'Cant. Horas Extras',HRSEXTRAS as 'Horas Extras',CANHRSEXT100 as 
                                        'Cant. Horas Extras 100', HRSEXT100 as 'Horas Extras 100',BonoFijo as 'Bono Fijo', BONARA,ASIESP,DIFSMA,BONCUM,DIFBON,BONRESP,
                                        GRATIFICACION as Gratificación," & "BONPRO,BONOTARDE,BONO5x2,BON4TU,BONONOCHE,ASICASA,BONASIST,OTROSBONOS,IMPONIBLE,COLACI,MOVILI,
                                        OTROSNOIMPONIBLES,Ktex,Ktfm,Ktha,RENTATRIBUT,IMPUES,RENTALIQUIDA,AGUINA," & "BONO18,BONVAC,BONNAV FROM ccReporteArauco 
                                        WHERE idCentro='{0}' AND Ames='{1}'",
                                                    txtUnidadEmpresa.Value, txtAnoEmpresa.Value.ToString & txtMesEmpresa.Value.ToString)

                        sqlSource = sqlBase
                        spllenaGridView(gridDatos, sqlBase)
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error en Reporte 1: " + ex.ToString)
                End Try
            End Using
        End If

        'REPORTE SEMANAL
        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 1 Then
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = True
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = True
                        gridDotacionMensual.Visible = False
                        gridBucarTrabajadores.Visible = False
                        gridDatos.Visible = False
                        gridSexo.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        Dim sqlBaseSemanal = String.Format("SELECT Usuaria,Planta,Area,Jefe,Nombre,Rut,Tarjeta, Cargo,Justificacion as Justificación,Motivo,
                                                [Fecha Inicio], [Fecha Ingreso],[Fecha Termino] as 'Fecha Término', Duracion as Duración, [Centro Costo],Localidad, 
                                                SueldoBase as 'Sueldo Base' FROM ccReporteSemanal WHERE idCentro='{0}'", txtUnidadEmpresa.Value)
                        sqlSource = sqlBaseSemanal
                        spllenaGridView(gridSemanal, sqlBaseSemanal)

                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error en Reporte 2: " + ex.ToString)
                End Try
            End Using
        End If

        'REPORTE DOTACION MENSUAL
        If ConectaSQLServer() And Session("DotacionMensual") <> Nothing Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 2 Then
                        gridDatos.Visible = False
                        gridSexo.Visible = False
                        gridSemanal.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        gridBucarTrabajadores.Visible = False
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                        Dim sqlBase As String = $"SELECT Usuaria, Planta, Area, Jefe, Nombre,Rut,Tarjeta,Cargo,Justificacion as Justificación, Motivo, 
                                    convert(varchar,fecha_nac,105)  as 'Fecha Nacimiento', nacionalidad as Nacionalidad,
                                    convert(varchar,fechaini,105)  as 'Inicio Contrato Histórico ', convert(varchar, fecha_ing, 105) as 'Inicio Contrato Vigente', 
                                    convert(varchar, fecha_ret, 105) as 'Término Contrato Vigente', duracion as 'Días Contrato Vigente', diasCausal as 'Días Acumulados Misma Causal', 
                                    nroContratos as 'N° Contratos Acumulados  Misma Causal', [Centro Costo],Localidad, SueldoBase as 'Sueldo Base' 
                                    FROM {Session("DotacionMensual")}"

                        sqlSource = sqlBase
                        spllenaGridView(gridDotacionMensual, sqlBase)
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error en Reporte 3 - Primer Conecta SQL " + ex.ToString)
                End Try
            End Using
        End If

        'REPORTE ARAUCO
        If ConectaSQLServer() And Session("DotacionMensual") <> Nothing Then
            Using conn
                Try
                    If cbxTipo.SelectedIndex = 7 Then
                        gridDatos.Visible = False
                        gridSexo.Visible = False
                        gridSemanal.Visible = False
                        UpdatePanel1.Visible = False
                        updatePanel2.Visible = False
                        gridBucarTrabajadores.Visible = False
                        txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                        txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                        ' Dim sqlBase As String = $"SELECT *
                        'FROM {Session("DotacionMensual")}"

                        Dim sqlBase As String = $"SELECT AreaPlanta as 'AREA PLANTA', aniomes as 'Mes', Usuaria as 'Operaciones', idCentro as 'Centro', Planta, Unidad, Area,
                                TipoServicio as 'Tipo Servicio', RutE as 'Rut E.', RazonSocial as ' Razón Social', rut as 'Rut P.', nombre as 'Empleado',
                                DiasTrabajados as 'DIAS TRABAJADOS', HH, perMes as 'Per/Mes', Justificacion as 'Servicio' ,NroRenovaciones as 'Nro Renovaciones',
                                FechaPrimerContrato as 'Fecha Inicio 1er Contrato', FechaTermino as 'Fecha Término Contrato'
                            FROM {Session("DotacionMensual")}
                            order by idCentro, Nombre"


                        sqlSource = sqlBase
                        spllenaGridView(gridDotacionMensual, sqlBase)
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error en Reporte 7 - Primer Conecta SQL " + ex.ToString)
                End Try
            End Using
        End If

        'REPORTE GENERO
        Try
            If cbxTipo.SelectedIndex = 5 Then
                txtUnidadEmpresa.ValidationSettings.CausesValidation = False
                txtUnidadEmpresa.ValidationSettings.RequiredField.IsRequired = False
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                gridSemanal.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False

                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    If txtUnidadEmpresa.SelectedIndex = 0 Then

                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA, 
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idEmpusuaria IN ('{cbxEmpresaPagina.Value}')
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ('{cbxEmpresaPagina.Value}') and REMPLESh.AMES ={txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        End If
                        'SOLO 1 EMPRESA
                    Else
                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA,
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idCentro IN ('{txtUnidadEmpresa.Value}')
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ('{txtUnidadEmpresa.Value}') AND REMPLESH.AMES = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"

                            spllenaGridView(gridSexo, sqlStr)
                        End If

                    End If
                Else
                    'TODAS LAS EMPRESAS
                    If txtUnidadEmpresa.SelectedIndex = 0 Then

                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA, 
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idCentro IN ({Session("pubUnidadesUsuario")})
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ({Session("pubUnidadesUsuario")}) and REMPLESh.AMES ={txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        End If
                        'SOLO 1 EMPRESA
                    Else
                        If txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString = fecha Then
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensual.rut as RUT, ccReporteDotacionMensual.nombre as NOMBRE, ccReporteDotacionMensual.Localidad as LOCALIDAD, ccReporteDotacionMensual.cargo as CARGO, 
                                        ccReporteDotacionMensual.planta as PLANTA, ccReporteDotacionMensual.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensual.nacionalidad as NACIONALIDAD, ccReporteDotacionMensual.area as AREA,
                                        CASE
                                            WHEN REMPLES.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLES.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensual
                                        LEFT JOIN REMPLES ON ccReporteDotacionMensual.rut = REMPLES.rut
                                        WHERE estado IN ('A', 'S') AND ccReporteDotacionMensual.idCentro IN ('{txtUnidadEmpresa.Value}')
                                        ORDER BY ccReporteDotacionMensual.nombre ASC"
                            spllenaGridView(gridSexo, sqlStr)
                        Else
                            sqlStr = $" SELECT DISTINCT ccReporteDotacionMensualhis.rut as RUT, ccReporteDotacionMensualhis.nombre as NOMBRE, ccReporteDotacionMensualhis.Localidad as LOCALIDAD, ccReporteDotacionMensualhis.cargo as CARGO, 
                                        ccReporteDotacionMensualhis.planta as PLANTA, ccReporteDotacionMensualhis.[Centro Costo] as 'CCOSTO', ccReporteDotacionMensualhis.nacionalidad as NACIONALIDAD, ccReporteDotacionMensualhis.area as AREA,
                                        CASE
                                            WHEN REMPLESh.Sexo = 'M' THEN 'Masculino'
                                            WHEN REMPLESh.Sexo = 'F' THEN 'Femenino'
                                        END AS sexo
                                        FROM ccReporteDotacionMensualhis
                                        LEFT JOIN REMPLESh ON ccReporteDotacionMensualhis.rut = REMPLESh.rut
                                        WHERE estado IN ('A', 'S') AND (ccReporteDotacionMensualhis.idCentro IN ('{txtUnidadEmpresa.Value}') AND REMPLESH.AMES = {txtAnoEmpresa.Value.ToString + txtMesEmpresa.Value.ToString})
                                        ORDER BY ccReporteDotacionMensualhis.nombre ASC"

                            spllenaGridView(gridSexo, sqlStr)
                        End If

                    End If
                End If

                If gridSexo.VisibleRowCount > 0 Then
                    'tr_excel.Visible = True
                    gridSexo.Visible = True
                Else
                    'tr_excel.Visible = False
                    gridSexo.Visible = False
                End If

            End If
        Catch ex As Exception
            ShowPopUpMsg("Error en Reporte 1: " + ex.ToString)
        End Try

        Try
            If cbxTipo.SelectedIndex = 6 Then
                gridDotacionMensual.Visible = False
                gridBucarTrabajadores.Visible = False
                gridSemanal.Visible = False
                UpdatePanel1.Visible = False
                updatePanel2.Visible = False

                'QUERY QUE AGREGA UNIDADES EN CICLO
                If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
                    'SIN SELECCION DE UNIDAD
                    If txtUnidadEmpresa.SelectedIndex = 0 Then

                        sqlStr = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE idEmpusuaria = '{cbxEmpresaPagina.Value}'
                                    ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, sqlStr)

                        'SOLO 1 UNIDAD
                    Else
                        sqlStr = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE id = '{txtUnidadEmpresa.Value}'
                                    ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, sqlStr)

                    End If
                Else
                    'SOLO UNIDADES CORRESPONDIENTES A CLIENTE
                    If txtUnidadEmpresa.SelectedIndex = 0 Then
                        'TODAS LAS UNIDADES

                        Dim arreglounidades() As String = Session("pubUnidadesUsuario").ToString().Split(",")
                        Dim cadena1sql = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE id = {arreglounidades(0)}"

                        If arreglounidades.Count > 1 Then
                            For index = 1 To arreglounidades.Count - 1
                                cadena1sql = cadena1sql + $" UNION SELECT * FROM [dbo].[diViewContrHisActivosMaxames] where id = {arreglounidades(index)}"
                            Next

                        End If
                        cadena1sql = cadena1sql + " ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, cadena1sql)
                        'SOLO 1 EMPRESA
                    Else

                        sqlStr = $" SELECT * 
                                    FROM [dbo].[diViewContrHisActivosMaxames] 
                                    WHERE id = '{txtUnidadEmpresa.Value}'
                                    ORDER BY ames desc, codigo DESC"
                        spllenaGridView(gridBucarTrabajadores, sqlStr)

                    End If
                End If
                If gridBucarTrabajadores.VisibleRowCount > 0 Then
                    'tr_excel.Visible = True
                    gridBucarTrabajadores.Visible = True

                Else
                    'tr_excel.Visible = False
                    gridBucarTrabajadores.Visible = False
                End If
            End If
        Catch ex As Exception
            ShowPopUpMsg("Error en Reporte 6: " + ex.ToString)
        End Try

    End Sub

    'ELIMINO LA TABLA AL CAMBIAR DE INDICE
    Protected Sub cbckDotacionmensual_Callback(source As Object, e As DevExpress.Web.CallbackEventArgs)

        gridDotacionMensual.PageIndex = 0

        If IsPostBack = False Then
            If Session("DotacionMensual") <> Nothing Then
                'ESCONDO GRILLA
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
                Else
                    ShowPopUpMsg("No hay conexión a la base de datos")
                End If
            End If
        End If

    End Sub

    Protected Sub cbpPanelIzquierdo_Callback(sender As Object, e As CallbackEventArgsBase)
        txtUnidadEmpresa.Value = Nothing
        Session("pubEmpUsuariaUsuario") = cbxEmpresaPagina.Value
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA COMBOBOX
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                    Call spLlenaComboBox(sqlCombo, txtUnidadEmpresa, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" + ex.ToString)
                End Try
            End Using
        End If
    End Sub

    Protected Sub txtUnidadEmpresa_Load(sender As Object, e As EventArgs)
        'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                        Call spLlenaComboBox(sqlCombo, txtUnidadEmpresa, "idUnidad", "descripcion")
                    Catch ex As Exception
                        ShowPopUpMsg("Error 1: {0}" + ex.ToString)
                    End Try
                End Using
            End If
        End If
    End Sub


End Class