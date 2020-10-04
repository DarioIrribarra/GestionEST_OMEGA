Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports DevExpress.Web
Imports Ionic.Zip
Imports System.Collections.Generic

Public Class pag_colaboradores
    Inherits System.Web.UI.Page

    'EVENTOS DE LOAD E INNIT DE CONTROLES Y PÁGINA
#Region "EVENTOS LOAD E INNIT"

    Private Sub pag_colaboradores_Init(sender As Object, e As EventArgs) Handles Me.Init

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

        cbxUnidad.NullText = "SELECCIONE"

        '*******CARGA LISTBOX (MENU INFERIOR)*******
        Dim sqlListBox = "  SELECT CONCAT(id, controlaVencimiento) AS id, nombreEnMenu
                            FROM ccTipoDocto 
                            WHERE activoEnMenu = 1
                            ORDER BY nombreEnMenu"
        Call spLlenaListBox(sqlListBox, listBoxInferior, "id", "nombreEnMenu")

        'VISTA COMPLETA GRID INFERIOR
        Call visibilidadGridInferior(listBoxInferior.Value)

        'CARGA COMBOBOX
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccViewEmpleados WHERE id IN (" & Session("pubUnidadesUsuario") & ") ORDER BY descripcion"
                    Call spLlenaComboBox(sqlStr, cbxUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    'ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If

        sqlStr = "  SELECT id, descripcion, controlaVencimiento, activo
                            FROM ccTipoDocto 
                            WHERE activo = 1
                            ORDER BY descripcion"
        Dim listadoColumnas As New List(Of String)({"id", "descripcion", "controlaVencimiento", "activo"})
        Call spLlenaComboBoxPopUpTodosDatosBd(sqlStr, txtTipoDocto, listadoColumnas, "id", "1")
        'Call spLlenaComboBoxPopUp(sqlStr, txtTipoDocto, "id", "descripcion")
        'Call spLlenaComboBox(sqlStr, cbxDescargarDocumento, "id", "descripcion")
        Call spLlenaComboBoxPopUpTodosDatosBd(sqlStr, cbxDescargarDocumento, listadoColumnas, "id", "1")
        cbxDescargarDocumento.Items.Insert(Nothing, New ListEditItem("TODO", Nothing))

        '*****ACCESO DE DISTINTOS PERFILES*******

        'ADMINISTRADOR Y AUDITOR PUEDEN SUBIR ARCHIVOS
        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Then
            btnSubeArchivo.Visible = True
            btnVerContrato.Visible = True
            btnDocumentos.Visible = True
            gridEmpleado.DataColumns("estado").Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
            gridEmpleado.Columns.Item("contrato").Visible = True
            GridInferior.Columns.Item("eliminar").Visible = True
            GridInferior.Columns.Item("editar").Visible = True
            GridInferiorVencimiento.Columns.Item("eliminar").Visible = True
            GridInferiorVencimiento.Columns.Item("editar").Visible = True
            'ACÁ PUEDE SER EL CAMBIO DE NOMBRE DEPENDIENDO DE LA EMPRESA QUE REPRESENTE
            Try
                'sqlStr = "  SELECT id, 
                '            CASE
                '                WHEN descripcion LIKE '%Liquidaciones de Sueldos%' THEN 'LIQUIDACIÓN DE SUELDO'
                '                WHEN descripcion LIKE '%Pacto de Hora Extra%' THEN 'PACTO DE HORA EXTRA'
                '                WHEN descripcion LIKE '%Certificados de Estudios%' THEN 'CERTIFICADO DE ESTUDIO'
                '              WHEN descripcion LIKE '%Charla Masso%' THEN 'CHARLA MASSO'
                '                WHEN descripcion LIKE '%Contratos y Anexos%' THEN 'CONTRATOS Y ANEXOS'
                '                WHEN descripcion LIKE '%Antecedentes Curriculares%' THEN 'CURRICULUM VITAE'
                '                WHEN descripcion LIKE '%Derecho a Saber%' THEN 'DERECHO A SABER'
                '                WHEN descripcion LIKE '%Entrega de Elementos Protección%' THEN 'ELEMENTOS PROTECCIÓN PERSONAL'
                '                WHEN descripcion LIKE '%Entrega de Reglamento Interno%' THEN 'REGLAMENTO INTERNO'
                '                WHEN descripcion LIKE '%Examenes Preocupacionales%' THEN 'EXAMEN PREOCUPACIONAL'
                '                WHEN descripcion LIKE '%Exámenes Sicologicos%' THEN 'EVALUACIÓN PSICOLÓGICA'
                '            END as descripcion 
                '            FROM ccTipoDocto 
                '            WHERE id IN ('CEREST', 'CHAMAS', 'CONTRA', 'CURRIC', 'DERSAB', 'ENTEPP', 'ENTREG', 'EXAPRE', 'EXASIC', 'LIQSUE', 'PACHOR')"

                'If IsPostBack = False Then
                '    Dim listadoColumnas As New List(Of String)({"id", "descripcion", "cvDigitalDoc", "activo"})
                '    Call spLlenaComboBoxPopUpTodosDatosBd(sqlStr, txtTipoDocto, listadoColumnas, "id", "1")
                '    'Call spLlenaComboBoxPopUp(sqlStr, txtTipoDocto, "id", "descripcion")

                '    Call spLlenaComboBox(sqlStr, cbxDescargarDocumento, "id", "descripcion")
                'End If


            Catch ex As Exception
                ShowPopUpMsg(ex.ToString)
            End Try
        End If

        'WEB SOLO VE TODO. NO SUBE ARCHIVO
        If Session("Web") = True Then
            btnSubeArchivo.Visible = False
            btnVerContrato.Visible = False
            btnDocumentos.Visible = False
            tb_BotonesCargar.Visible = False
            gridEmpleado.DataColumns("estado").Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
            gridEmpleado.Columns.Item("contrato").Visible = True
            GridInferior.Columns.Item("eliminar").Visible = False
            GridInferior.Columns.Item("editar").Visible = False
            GridInferiorVencimiento.Columns.Item("eliminar").Visible = False
            GridInferiorVencimiento.Columns.Item("editar").Visible = False
        End If

        'OPERACIONES SOLO VEN SUS UNIDADES PERO PUEDE FILTRAR
        If Session("Operaciones") = True Then
            btnVerContrato.Visible = False
            tb_BotonesCargar.Visible = True
            btnDocumentos.Visible = True
            ASPxImage1.Visible = False
            btnSubeArchivo.Visible = False
            gridEmpleado.DataColumns("estado").Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
            gridEmpleado.Columns.Item("contrato").Visible = True
            GridInferior.Columns.Item("eliminar").Visible = False
            GridInferior.Columns.Item("editar").Visible = False
            GridInferiorVencimiento.Columns.Item("eliminar").Visible = False
            GridInferiorVencimiento.Columns.Item("editar").Visible = False
        End If

        'CLIENTE SOLO VEN SUS UNIDADES
        If Session("Cliente") = True Then
            btnSubeArchivo.Visible = False
            btnVerContrato.Visible = False
            tb_BotonesCargar.Visible = False
            gridEmpleado.DataColumns("estado").Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.False
            gridEmpleado.Columns.Item("contrato").Visible = False
            GridInferior.Columns.Item("eliminar").Visible = False
            GridInferior.Columns.Item("editar").Visible = False
            GridInferiorVencimiento.Columns.Item("eliminar").Visible = False
            GridInferiorVencimiento.Columns.Item("editar").Visible = False

        End If

        If divGridInferior.Visible = True Then
            divPanelInferior.Visible = True
            divlistBoxInferior.Visible = True
            listBoxInferior.Visible = True
        End If

        'CARGA DATOS DEL POPUP
        hlVerArchivo.Visible = False

        'CARGAR EMPRESA
        Dim sqlCombo = $"SELECT DISTINCT id, descripcion from ccEmpUsuaria WHERE id <> 'TMPO'"
        Call spLlenaComboBoxPopUp(sqlCombo, cbxEmpresaPagina, "id", "descripcion")

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
                            sqlStr = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                            Call spLlenaComboBox(sqlStr, cbxUnidad, "idUnidad", "descripcion")
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

        'LA COLUMNA DE EVALUACION ES VISIBLE SI LA EMPRESA ES ARAUCO Y NO ES CLIENTE
        OcultarColumnaEvaluacion()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'BUSQUEDA DE REPORTES
        Session("BusquedaRealizada") = 0

        'ELIMINO LA TABLA DE LA BD CREADA EN LA DOTACION MENSUAL SI EXISTE
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

        'ELIJO EN QUE CONTROL MOSTRAR EL LOADING PANEL
        lpCallBack.ContainerElementID = btnSubeArchivo.ID

        'CARGAR ADMI, AUFITOR Y WEB
        If (IsPostBack = False Or IsCallback = False) And Session("SuperAdmin") = True Or Session("Administrador") = True Or Session("Auditor") = True Or Session("Web") = True Then
            If gridEmpleado.FocusedRowIndex >= 0 Then
                divPanelInferior.Visible = True
                divlistBoxInferior.Visible = True
                listBoxInferior.Visible = True
                divGridInferior.Visible = True
                'VISTA COMPLETA GRID INFERIOR
                Call visibilidadGridInferior(listBoxInferior.Value)
                'If listBoxInferior.SelectedItem.Value = "Pacto_HE" Or
                '    listBoxInferior.SelectedItem.Value = "Masso" Or
                '    listBoxInferior.SelectedItem.Value = "Contratos" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Preoc" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Psico" Then
                '    GridInferiorVencimiento.Visible = True
                '    GridInferior.Visible = False
                'Else
                '    GridInferiorVencimiento.Visible = False
                '    GridInferior.Visible = True
                'End If

            End If
            Call CargarGridAdministrador()
            'CargarGridInferior()
        End If

        'CARGAR OPERACIONES
        If (IsPostBack = False Or IsCallback = False) And Session("Operaciones") = True Then
            If gridEmpleado.FocusedRowIndex >= 0 Then
                divPanelInferior.Visible = True
                divlistBoxInferior.Visible = True
                listBoxInferior.Visible = True
                divGridInferior.Visible = True
                'VISTA COMPLETA GRID INFERIOR
                Call visibilidadGridInferior(listBoxInferior.Value)
                'If listBoxInferior.SelectedItem.Value = "Pacto_HE" Or
                '    listBoxInferior.SelectedItem.Value = "Masso" Or
                '    listBoxInferior.SelectedItem.Value = "Contratos" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Preoc" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Psico" Then
                '    GridInferiorVencimiento.Visible = True
                '    GridInferior.Visible = False
                'Else
                '    GridInferiorVencimiento.Visible = False
                '    GridInferior.Visible = True
                'End If

            End If
            Call CargarGridEmpleado()
            'CargarGridInferior()
        End If

        'CARGAR CLIENTE
        If (IsPostBack = False Or IsCallback = False) And Session("Cliente") = True Then
            If gridEmpleado.FocusedRowIndex >= 0 Then
                divPanelInferior.Visible = True
                divlistBoxInferior.Visible = True
                listBoxInferior.Visible = True
                divGridInferior.Visible = True
                'VISTA COMPLETA GRID INFERIOR
                Call visibilidadGridInferior(listBoxInferior.Value)
                'If listBoxInferior.SelectedItem.Value = "Pacto_HE" Or
                '    listBoxInferior.SelectedItem.Value = "Masso" Or
                '    listBoxInferior.SelectedItem.Value = "Contratos" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Preoc" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Psico" Then
                '    GridInferiorVencimiento.Visible = True
                '    GridInferior.Visible = False
                'Else
                '    GridInferiorVencimiento.Visible = False
                '    GridInferior.Visible = True
                'End If
            End If
            Call CargarGridEmpleadoCliente()
            'CargarGridInferior()
        End If

    End Sub

    Protected Sub ASPxHyperLink1_Load(sender As Object, e As EventArgs)
        Dim link As ASPxHyperLink = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer
        Dim myValue As String = ""

        If gridEmpleado.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Elija un colaborador")
            Return
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    'Select Case listBoxInferior.SelectedItem.Value
                    'SE OBTIENE EL VALOR PARA VER SI TIENE FECHA DE VENCIMIENTO O NO 
                    Dim manejaVencimiento = listBoxInferior.Value.ToString()
                    manejaVencimiento = manejaVencimiento.Substring(manejaVencimiento.Length - 1, 1)
                    If manejaVencimiento = 0 Then
                        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    Else
                        myValue = GridInferiorVencimiento.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    End If

                    'Select Case listBoxInferior.Value
                    '    Case "Liquidaciones"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Pacto_HE"
                    '        myValue = GridInferiorVencimiento.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Estudios"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Masso"
                    '        myValue = GridInferiorVencimiento.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Contratos"
                    '        myValue = GridInferiorVencimiento.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Curriculum"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Der_Saber"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "EPP"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Reg_Interno"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Ev_Desempeño"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Ex_Preoc"
                    '        myValue = GridInferiorVencimiento.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Ex_Psico"
                    '        myValue = GridInferiorVencimiento.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    '    Case "Finiquito"
                    '        myValue = GridInferior.GetRowValues(container.ItemIndex, container.Column.FieldName)
                    'End Select
                Catch ex As Exception
                    ShowPopUpMsg("Error: " & ex.ToString & "")
                End Try
            End Using
        End If

        If myValue <> "" Then
            link.ImageUrl = "~/images/15.png"
            link.NavigateUrl = myValue
            link.Target = "_blank"

        Else
            link.ImageUrl = "~/images/12.png"
            link.NavigateUrl = ""
        End If

    End Sub

    Protected Sub imgEstado_A_Load(sender As Object, e As EventArgs)
        'aca es el evento de la imagen dentro del itemtemplate

        Dim image As ASPxImage = sender
        Dim container As GridViewDataItemTemplateContainer = image.NamingContainer
        Dim myValue As String = ""

        myValue = gridEmpleado.GetRowValues(container.ItemIndex, container.Column.FieldName)
        If myValue.Equals("A") Then
            image.Visible = True
            image.ImageUrl = "images/15.png"
        ElseIf myValue.Equals("E") Then
            image.Visible = True
            image.ImageUrl = "images/12.png"
        ElseIf myValue.Equals("S") Then
            image.Visible = True
            image.ImageUrl = "images/14.png"
        ElseIf myValue.Equals("O") Then
            image.Visible = True
            image.ImageUrl = "images/13.png"
        Else
            image.Visible = False
        End If

    End Sub

    Protected Sub cbxUnidad_Load(sender As Object, e As EventArgs)
        'CARGA COMBOBOX UNIDADES DEPENDIENDO DE LA SESION
        If Session("Administrador") = True Or Session("Auditor") = True Or Session("SuperAdmin") = True Or Session("Web") Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                        Call spLlenaComboBox(sqlCombo, cbxUnidad, "idUnidad", "descripcion")
                    Catch ex As Exception
                        ShowPopUpMsg("Error 1: {0}" + ex.ToString)
                    End Try
                End Using
            End If
        End If
    End Sub

#End Region

    'CARGA DE GRILLAS
#Region "CARGA DE GRILLAS"
    'GRILLA CON PERMISOS ADMINISTRADOR
    Protected Sub CargarGridAdministrador()
        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxUnidad.Value <> Nothing Then

                        sqlStr = "  Select rut, nombre, ccViewEmpleados.id As idUnidad, UPPER(dbo.LimpiarCaracteres(ccViewEmpleados.descripcion)) As descUnidad, estado
                                    FROM ccViewEmpleados 
                                    INNER Join ccUnidades ON ccViewEmpleados.id = ccUnidades.id
                                    WHERE ccUnidades.idEmpusuaria IN ('" & cbxEmpresaPagina.Value & "') AND ccViewEmpleados.id = '" & cbxUnidad.Value.ToString & "' 
                                    ORDER BY estado ASC"
                        spllenaGridView(gridEmpleado, sqlStr)
                    Else
                        sqlStr = "  Select rut, nombre, ccViewEmpleados.id As idUnidad, UPPER(dbo.LimpiarCaracteres(ccViewEmpleados.descripcion)) As descUnidad, estado
                                    FROM ccViewEmpleados 
                                    INNER Join ccUnidades ON ccViewEmpleados.id = ccUnidades.id
                                    WHERE ccUnidades.idEmpusuaria IN ('" & cbxEmpresaPagina.Value & "') 
                                    ORDER BY estado ASC"
                        spllenaGridView(gridEmpleado, sqlStr)
                    End If
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using

        End If
    End Sub
    'GRILLA CON PERMISOS TRABAJADOR INTERNO
    Protected Sub CargarGridEmpleado()
        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxUnidad.Value <> Nothing Then

                        sqlStr = "  SELECT rut, nombre, id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descUnidad, estado 
                                        FROM ccViewEmpleados 
                                        WHERE id IN (" & Session("pubUnidadesUsuario") & ") AND id = '" & cbxUnidad.Value.ToString & "' 
                                        ORDER BY estado ASC"
                        spllenaGridView(gridEmpleado, sqlStr)
                    Else
                        sqlStr = "  SELECT rut, nombre, id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descUnidad, estado 
                                        FROM ccViewEmpleados WHERE id IN (" & Session("pubUnidadesUsuario") & ") 
                                        ORDER BY estado ASC"
                        spllenaGridView(gridEmpleado, sqlStr)
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error: " & ex.ToString & "")
                End Try
            End Using

        End If
    End Sub
    'GRILLA TRABAJADOR CLIENTE
    Protected Sub CargarGridEmpleadoCliente()
        If ConectaSQLServer() Then
            Using conn
                Try
                    If cbxUnidad.Value = Nothing Then
                        sqlStr = "  SELECT rut, nombre, id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descUnidad, estado 
                                    FROM ccViewEmpleados 
                                    WHERE id IN (" & Session("pubUnidadesUsuario") & ") 
                                    AND estado = 'A'
                                    ORDER BY estado ASC"
                        spllenaGridView(gridEmpleado, sqlStr)
                    Else
                        sqlStr = "  SELECT rut, nombre, id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descUnidad, estado 
                                    FROM ccViewEmpleados 
                                    WHERE id IN (" & Session("pubUnidadesUsuario") & ") AND id = '" & cbxUnidad.Value.ToString & "' AND estado = 'A'
                                    ORDER BY estado ASC"
                        spllenaGridView(gridEmpleado, sqlStr)
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("Error: " & ex.ToString & "")
                End Try
            End Using
        End If
    End Sub
    'CARGA DE GRILLA INFERIOR
    Private Sub CargarGridInferior()
        If gridEmpleado.FocusedRowIndex >= 0 Then
            If gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "rut").ToString() <> Nothing Then
                Dim nroRut = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "rut").ToString()
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            'OBTENGO VALOR DE SELECCIÓN PARA HACER SUBSTRING
                            Dim valor = listBoxInferior.Value.ToString()
                            Dim tipoDocto = valor.Substring(0, valor.Length - 1)
                            Dim sqlLlenadoGridView = "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strAFechaDocumentos(vctodocto) as vence, id 
                                                        from ccDocumento 
                                                        WHERE tipodocto='" & tipoDocto & "' 
                                                        AND rut='" & nroRut & "' 
                                                        ORDER BY fechadocto DESC"

                            'MANEJA VISIBILIDAD DE GRIDS INDEPENDIENTES EN BASE A VENCIMIENTO
                            Dim manejaVencimiento = valor.Substring(valor.Length - 1, 1)
                            If manejaVencimiento = 0 Then
                                GridInferior.Visible = True
                                GridInferiorVencimiento.Visible = False
                                'LLENO GRIDVIEW SIN FECHA VENCIMIENTO
                                Call spllenaGridView(GridInferior, sqlLlenadoGridView)
                            Else
                                GridInferiorVencimiento.Visible = True
                                GridInferior.Visible = False
                                'LLENO GRIDVIEW CON FECHA VENCIMIENTO
                                Call spllenaGridView(GridInferiorVencimiento, sqlLlenadoGridView)
                            End If

                            'Select Case listBoxInferior.SelectedItem.Value
                            '    Case "Liquidaciones"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        'spllenaGridView(GridInferior, "Select ruta, RIGHT(CONVERT(VARCHAR(10), fechadocto, 103), 7) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='LIQSUE' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='LIQSUE' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Pacto_HE"
                            '        GridInferiorVencimiento.Visible = True
                            '        GridInferior.Visible = False
                            '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='PACHOR' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Estudios"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='CEREST' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Masso"
                            '        GridInferiorVencimiento.Visible = True
                            '        GridInferior.Visible = False
                            '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='CHAMAS' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Contratos"
                            '        GridInferiorVencimiento.Visible = True
                            '        GridInferior.Visible = False
                            '        spllenaGridView(GridInferiorVencimiento, "Select ruta,convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='CONTRA' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Curriculum"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='CURRIC' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Der_Saber"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='DERSAB' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "EPP"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='ENTEPP' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Reg_Interno"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta,convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='ENTREG' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Ev_Desempeño"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='EVADES' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Ex_Preoc"
                            '        GridInferiorVencimiento.Visible = True
                            '        GridInferior.Visible = False
                            '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='EXAPRE' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Ex_Psico"
                            '        GridInferiorVencimiento.Visible = True
                            '        GridInferior.Visible = False
                            '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='EXASIC' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                            '    Case "Finiquito"
                            '        GridInferior.Visible = True
                            '        GridInferiorVencimiento.Visible = False
                            '        spllenaGridView(GridInferior, "Select ruta,convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='FINIQU' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")

                            'End Select
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                End If
            End If

        End If
    End Sub
    ''CAMBIO DE PÁGINA EN GRILLA SUPERIOR
    'Protected Sub gridEmpleado_PageIndexChanged(sender As Object, e As EventArgs)
    '    If divGridInferior.Visible = True Then

    '        divlistBoxInferior.Visible = True
    '        listBoxInferior.Visible = True
    '        divPanelInferior.Visible = True

    '        Call CargarGridInferior()
    '    End If
    'End Sub
    'CAMBIO DE PÁGINA EN GRILLA INFERIOR
    Protected Sub GridInferior_PageIndexChanged(sender As Object, e As EventArgs)
        Call CargarGridInferior()
    End Sub
    'ANTES DE OBTENER OBTENER RESULTADOS GRILLA SUPERIOR
    Protected Sub gridEmpleado_BeforeGetCallbackResult(sender As Object, e As EventArgs)
        'CARGAR ADMI, AUFITOR Y WEB
        If IsPostBack And Session("SuperAdmin") = True Or Session("Administrador") = True Or Session("Auditor") = True Or Session("Web") = True Then
            If gridEmpleado.FocusedRowIndex >= 0 Then
                divPanelInferior.Visible = True
                divlistBoxInferior.Visible = True
                listBoxInferior.Visible = True
                divGridInferior.Visible = True
                'VISTA COMPLETA GRID INFERIOR
                Call visibilidadGridInferior(listBoxInferior.Value)
                'If listBoxInferior.SelectedItem.Value = "Pacto_HE" Or
                '    listBoxInferior.SelectedItem.Value = "Masso" Or
                '    listBoxInferior.SelectedItem.Value = "Contratos" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Preoc" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Psico" Then
                '    GridInferiorVencimiento.Visible = True
                '    GridInferior.Visible = False
                'Else
                '    GridInferiorVencimiento.Visible = False
                '    GridInferior.Visible = True
                'End If

            End If
            Call CargarGridAdministrador()
            CargarGridInferior()
        End If

        'CARGAR OPERACIONES
        If IsPostBack And Session("Operaciones") = True Then
            If gridEmpleado.FocusedRowIndex >= 0 Then
                divPanelInferior.Visible = True
                divlistBoxInferior.Visible = True
                listBoxInferior.Visible = True
                divGridInferior.Visible = True
                'VISTA COMPLETA GRID INFERIOR
                Call visibilidadGridInferior(listBoxInferior.Value)
                'If listBoxInferior.SelectedItem.Value = "Pacto_HE" Or
                '    listBoxInferior.SelectedItem.Value = "Masso" Or
                '    listBoxInferior.SelectedItem.Value = "Contratos" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Preoc" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Psico" Then
                '    GridInferiorVencimiento.Visible = True
                '    GridInferior.Visible = False
                'Else
                '    GridInferiorVencimiento.Visible = False
                '    GridInferior.Visible = True
                'End If

            End If
            Call CargarGridEmpleado()
            CargarGridInferior()
        End If

        'CARGAR CLIENTE
        If IsPostBack And Session("Cliente") = True Then
            If gridEmpleado.FocusedRowIndex >= 0 Then
                divPanelInferior.Visible = True
                divlistBoxInferior.Visible = True
                listBoxInferior.Visible = True
                divGridInferior.Visible = True
                'VISTA COMPLETA GRID INFERIOR
                Call visibilidadGridInferior(listBoxInferior.Value)
                'If listBoxInferior.SelectedItem.Value = "Pacto_HE" Or
                '    listBoxInferior.SelectedItem.Value = "Masso" Or
                '    listBoxInferior.SelectedItem.Value = "Contratos" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Preoc" Or
                '    listBoxInferior.SelectedItem.Value = "Ex_Psico" Then
                '    GridInferiorVencimiento.Visible = True
                '    GridInferior.Visible = False
                'Else
                '    GridInferiorVencimiento.Visible = False
                '    GridInferior.Visible = True
                'End If
            End If
            Call CargarGridEmpleadoCliente()
            CargarGridInferior()
        End If
    End Sub
    'ANTES DE OBTENER RESULTADOS GRILLA INFERIOR
    Protected Sub GridInferior_BeforeGetCallbackResult(sender As Object, e As EventArgs)
        If gridEmpleado.FocusedRowIndex >= 0 Then
            Dim nroRut As String = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "rut").ToString()
            If ConectaSQLServer() Then
                Using conn
                    Try

                        'OBTENGO VALOR DE SELECCIÓN PARA HACER SUBSTRING
                        Dim valor = listBoxInferior.Value.ToString()
                        Dim tipoDocto = valor.Substring(0, valor.Length - 1)
                        Dim sqlLlenadoGridView = "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strAFechaDocumentos(vctodocto) as vence, id 
                                                        from ccDocumento 
                                                        WHERE tipodocto='" & tipoDocto & "' 
                                                        AND rut='" & nroRut & "' 
                                                        ORDER BY fechadocto DESC"

                        'MANEJA VISIBILIDAD DE GRIDS INDEPENDIENTES EN BASE A VENCIMIENTO
                        Dim manejaVencimiento = valor.Substring(valor.Length - 1, 1)
                        If manejaVencimiento = 0 Then
                            GridInferior.Visible = True
                            GridInferiorVencimiento.Visible = False
                            'LLENO GRIDVIEW SIN FECHA VENCIMIENTO
                            Call spllenaGridView(GridInferior, sqlLlenadoGridView)
                        Else
                            GridInferiorVencimiento.Visible = True
                            GridInferior.Visible = False
                            'LLENO GRIDVIEW CON FECHA VENCIMIENTO
                            Call spllenaGridView(GridInferiorVencimiento, sqlLlenadoGridView)
                        End If

                        'Select Case listBoxInferior.SelectedItem.Value
                        '    Case "Liquidaciones"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta, RIGHT(CONVERT(VARCHAR(10), fechadocto, 103), 7) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='LIQSUE' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Pacto_HE"
                        '        GridInferiorVencimiento.Visible = True
                        '        GridInferior.Visible = False
                        '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='PACHOR' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Estudios"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='CEREST' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Masso"
                        '        GridInferiorVencimiento.Visible = True
                        '        GridInferior.Visible = False
                        '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='CHAMAS' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Contratos"
                        '        GridInferiorVencimiento.Visible = True
                        '        GridInferior.Visible = False
                        '        spllenaGridView(GridInferiorVencimiento, "Select ruta,convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='CONTRA' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Curriculum"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='CURRIC' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Der_Saber"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='DERSAB' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "EPP"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='ENTEPP' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Reg_Interno"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta,convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='ENTREG' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Ev_Desempeño"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='EVADES' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Ex_Preoc"
                        '        GridInferiorVencimiento.Visible = True
                        '        GridInferior.Visible = False
                        '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='EXAPRE' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Ex_Psico"
                        '        GridInferiorVencimiento.Visible = True
                        '        GridInferior.Visible = False
                        '        spllenaGridView(GridInferiorVencimiento, "Select ruta, convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, dbo.fn_strToFecha(vctodocto) as vence, id from ccDocumento WHERE tipodocto='EXASIC' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")
                        '    Case "Finiquito"
                        '        GridInferior.Visible = True
                        '        GridInferiorVencimiento.Visible = False
                        '        spllenaGridView(GridInferior, "Select ruta,convert(varchar(10) ,fechadocto,105) as fechadcto, descdocto, id from ccDocumento WHERE tipodocto='FINIQU' AND rut='" & nroRut & "' ORDER BY fechadocto DESC")

                        'End Select
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            End If
        End If
    End Sub

#End Region

    'DESCARGA DE DOCUMENTOS
#Region "Descarga de Documentos"
    Protected Sub cbpDescargarArchivos_Callback(sender As Object, e As CallbackEventArgsBase)
        txtEmpleadoDescargarDocumento.Text = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "nombre")
        txtIdEmpleadoDescargarDocumento.Text = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "rut")
        cbxDescargarDocumento.Value = Nothing
    End Sub


    Protected Sub btnDescargar_Click(sender As Object, e As EventArgs)
        Dim diccionarioDocumentos As New Dictionary(Of String, String)
        If gridEmpleado.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Error: Seleccione un colaborador y luego descargue documentos")
            Return
        End If

        Dim filePath As String = ""

        'OBTENER EL ARCHIVO SI SE SELECCIONÓ
        If cbxDescargarDocumento.Value <> Nothing Then
            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                'zip.AddDirectoryByName(folderpath)
                'ASIGNO LOS ARCHIVOS A COMPRIMIR
                If ConectaSQLServer() Then
                    Try
                        sqlStr = $"  SELECT ruta
                                    FROM ccDocumento
                                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                                    AND tipodocto = '{cbxDescargarDocumento.Value}'"
                        Using conn
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader
                            While reader.Read
                                filePath = Server.MapPath(reader.Item(0))
                                zip.AddFile(filePath, "\")
                            End While
                            sqlcmd.Dispose()
                        End Using
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End If

                Response.Clear()
                Response.BufferOutput = False
                Dim zipName As String = [String].Format("{0}_{1}_{2}.zip", cbxDescargarDocumento.SelectedItem.Text, Trim(txtEmpleadoDescargarDocumento.Text.Replace(",", "_")), DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                Response.ContentType = "application/zip"
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
                Try
                    zip.Save(Response.OutputStream)
                    Response.End()
                Catch ex As Exception
                    ShowPopUpMsg("Error: Uno o más archivos faltantes")
                    Response.End()
                End Try

            End Using

        Else
            'DESTINO CARPETA PARA COMPRIMIR
            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary

                If ConectaSQLServer() Then
                    Try
                        Dim sqlNombresArchivos = "SELECT id, descripcion
                                                    FROM ccTipoDocto 
                                                    WHERE activo = 1
                                                    ORDER BY descripcion"
                        Using conn
                            Dim sqlcmd As New SqlCommand(sqlNombresArchivos, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader
                            While reader.Read
                                zip.AddDirectoryByName(reader.Item("descripcion"))
                                diccionarioDocumentos.Add(reader.Item("id"), reader.Item("descripcion"))
                            End While
                            sqlcmd.Dispose()
                        End Using
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End If

                'zip.AddDirectoryByName("LIQUIDACIÓN DE SUELDO")
                'zip.AddDirectoryByName("PACTO DE HORA EXTRA")
                'zip.AddDirectoryByName("CERTIFICADO DE ESTUDIO")
                'zip.AddDirectoryByName("CHARLA MASSO/PE/S&S")
                'zip.AddDirectoryByName("CONTRATOS Y ANEXOS")
                'zip.AddDirectoryByName("CURRICULUM VITAE")
                'zip.AddDirectoryByName("DERECHO A SABER")
                'zip.AddDirectoryByName("ELEMENTOS PROTECCIÓN PERSONAL")
                'zip.AddDirectoryByName("REGLAMENTO INTERNO")
                'zip.AddDirectoryByName("EXAMEN PREOCUPACIONAL")
                'zip.AddDirectoryByName("EVALUACIÓN PSICOLÓGICA")

                'ASIGNO LOS ARCHIVOS A COMPRIMIR

                For Each item As String In diccionarioDocumentos.Keys
                    If ConectaSQLServer() Then
                        Try
                            sqlStr = $"  SELECT ruta
                                    FROM ccDocumento
                                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                                    AND tipodocto LIKE '%{item}%'"
                            Using conn
                                Dim sqlcmd As New SqlCommand(sqlStr, conn)
                                Dim reader As IDataReader = sqlcmd.ExecuteReader
                                While reader.Read
                                    filePath = Server.MapPath(reader.Item(0))
                                    zip.AddFile(filePath, diccionarioDocumentos(item))
                                End While
                                sqlcmd.Dispose()
                            End Using
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End If
                Next



                ''LIQUIDACIONES
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%LIQSUE%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "LIQUIDACIÓN DE SUELDO")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''PACTO HE
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%PACHOR%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "PACTO DE HORA EXTRA")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''CERTIFICADO ESTUDIOS
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%CEREST%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "CERTIFICADO DE ESTUDIO")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''CHARLA MASSO
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%CHAMAS%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "CHARLA MASSO/PE/S&S")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''CONTRATOS
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%CONTRA%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "CONTRATOS Y ANEXOS")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''CURRICULUM
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%CURRIC%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "CURRICULUM VITAE")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''DERECHO A SABER
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%DERSAB%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "DERECHO A SABER")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''EPP
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%ENTEPP%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "ELEMENTOS PROTECCIÓN PERSONAL")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''REGLAMENTO INTERNO
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%ENTREG%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "REGLAMENTO INTERNO")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''EXAMEN PREOCUPACIONAL
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%EXAPRE%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "EXAMEN PREOCUPACIONAL")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                ''EVALUACIÓN PSICOLÓGICA
                'If ConectaSQLServer() Then
                '    Try
                '        sqlStr = $"  SELECT ruta
                '                    FROM ccDocumento
                '                    WHERE rut LIKE '%{txtIdEmpleadoDescargarDocumento.Text}%'
                '                    AND tipodocto LIKE '%EXASIC%'"
                '        Using conn
                '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                '            Dim reader As IDataReader = sqlcmd.ExecuteReader
                '            While reader.Read
                '                filePath = Server.MapPath(reader.Item(0))
                '                zip.AddFile(filePath, "EVALUACIÓN PSICOLÓGICA")
                '            End While
                '            sqlcmd.Dispose()
                '        End Using
                '    Catch ex As Exception
                '        ShowPopUpMsg(ex.ToString)
                '    End Try
                'End If

                Response.Clear()
                Response.BufferOutput = False
                Dim zipName As String = [String].Format("{0}_{1}_{2}.zip", "DOCUMENTOS", Trim(txtEmpleadoDescargarDocumento.Text.Replace(",", "_")), DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                Response.ContentType = "application/zip"
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
                popUpDescargarArchivos.ShowOnPageLoad = False
                Try
                    Dim dato = Response.OutputStream
                    zip.Save(dato)
                    Response.End()
                Catch ex As Exception
                    ShowPopUpMsg("Error: Uno o más archivos faltantes")
                End Try

            End Using
        End If
    End Sub
#End Region

    'BOTONES
#Region "BOTONES"
    'BTN VER CONTRATO
    Protected Sub btnVerContrato_Click(sender As Object, e As EventArgs)
        Dim link As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer

        Page.ClientScript.RegisterStartupScript([GetType](), "OpenWindow", "window.open('pag_contrato.aspx', '_blank');", True)
        Session("pubRutContrato") = gridEmpleado.GetRowValues(container.ItemIndex, "rut").ToString()
        'link.Target = "_blank"
        'Response.Redirect("~/contrato.aspx")

    End Sub

    'BTN BUSCAR
    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)

        'LA COLUMNA DE EVALUACION ES VISIBLE SI LA EMPRESA ES ARAUCO Y NO ES CLIENTE
        OcultarColumnaEvaluacion()

        'PARA NO CLIENTES VER LOS ACTIVOS

        If Session("SuperAdmin") = True Or Session("Administrador") = True Or Session("Auditor") = True Or Session("Web") = True Then
            Call CargarGridAdministrador()
        End If

        If Session("Operaciones") = True Then
            Call CargarGridEmpleado()
        End If

        If Session("Cliente") = True Then
            Call CargarGridEmpleadoCliente()
        End If

        gridEmpleado.PageIndex = 0
        gridEmpleado.FocusedRowIndex = 0
        Call CargarGridInferior()

    End Sub

    'BTN GUARDAR
    Protected Sub btnGrabarArchivo_Click(sender As Object, e As EventArgs)
        'Dim valor = txtTipoDocto.Value
        Dim existeColumna = ""
        Dim rut = ""
        Dim ultimodoc = ""
        Dim idAntigua = ""
        Dim pCuenta As Integer = 0
        If txtIdEmpleado.Text = Nothing Then
            ShowPopUpMsg("Seleccione un Colaborador")
            Return
        End If

        'INSERCIÓN DE DATOS
        'VEO SI LA COLUMNA EXISTE EN ccDocEmpleado
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = $" IF COL_LENGTH('dbo.ccDocEmpleado', '{txtTipoDocto.Value}') IS NOT NULL 
                                        SELECT '1'
                                    ELSE
                                        SELECT '0'"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    existeColumna = sqlcmd.ExecuteScalar()

                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        Else
            ShowPopUpMsg("No hay conexion a base datos")
        End If
        'SI LA COLUMNA NO EXISTE, SE DEBEN CREAR LAS 3 (TIPO DE DOCUMENTO, FECHA INGRESO, FECHA VENCIMIENTO)
        If existeColumna = "0" Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        'Dim tTransaction As SqlTransaction
                        sqlStr = $" ALTER TABLE ccDocEmpleado
                                        ADD {txtTipoDocto.Value} VARCHAR(250) CONSTRAINT nuevo_doc_{txtTipoDocto.Value} DEFAULT '' NOT NULL, 
                                        {txtTipoDocto.Value}_FCREACION DATETIME,
                                        {txtTipoDocto.Value}_FVENCIMIENTO VARCHAR(10) CONSTRAINT nuevo_doc_fechavencimiento_{txtTipoDocto.Value} DEFAULT '' NOT NULL"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        'sqlcmd.Transaction = tTransaction
                        sqlcmd.ExecuteNonQuery()

                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            Else
                ShowPopUpMsg("No hay conexion a base datos")
            End If
        End If
#Region "Editar"
        'SI VIENE CON ID Y NO RECARGAN DOC HAGO UN UPDATE SIN DOCUMENTO ' DEBO VER LA FECHA DEL DOC
        If Session("IdDocumento") <> Nothing And Session("Editar") = 1 Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" SELECT id 
                                    FROM ccDocumento 
                                    WHERE id = '{Session("IdDocumento")}'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        idAntigua = sqlcmd.ExecuteScalar
                    Catch ex As Exception

                    End Try
                End Using
            End If


            'VEO SI EL RUT ESTÁ EN ccDocEmpleado
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" SELECT rut 
                                    FROM ccDocEmpleado
                                    WHERE rut like '%{txtIdEmpleado.Text}%'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        rut = sqlcmd.ExecuteScalar()
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            Else
                ShowPopUpMsg("No hay conexion a base datos")
            End If

            'SI EL RUT EXISTE OBTENGO EL ARCHIVO CON LA FECHA MÁXIMA EN ccDocumento
            If rut <> Nothing Then
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" SELECT max(fechadocto) 
                                    FROM [dbo].[ccDocumento] 
                                    where rut like '%{txtIdEmpleado.Text}%' and tipodocto = '{txtTipoDocto.Value}';"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader
                            While reader.Read
                                If reader.IsDBNull(0) Then
                                    ultimodoc = ""
                                Else
                                    ultimodoc = reader.Item(0)
                                End If
                            End While

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
                'SI RUT NO EXISTE SE INGRESA EN ccDocEmpleado e Igual se busca en ccDocuemtno
            Else
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"INSERT INTO ccDocEmpleado (rut, {txtTipoDocto.Value}, {txtTipoDocto.Value}_FCREACION, {txtTipoDocto.Value}_FVENCIMIENTO)
                                                        VALUES (@rut, @ruta, @fecha, @vcto)"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd.Parameters.Add(New SqlParameter("@ruta", Session("pubFileName")))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            'txtTipoDocto.Value = "CONTRA" Or
                            'txtTipoDocto.Value = "EXAPRE" Or
                            'txtTipoDocto.Value = "EXASIC" Or
                            'txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If
                            sqlcmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
                'BUSCO ULTIMA FECHA EN cDocumento
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" SELECT max(fechadocto) 
                                    FROM [dbo].[ccDocumento] 
                                    where rut like '%{txtIdEmpleado.Text}%' and tipodocto = '{txtTipoDocto.Value}';"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader
                            While reader.Read
                                If reader.IsDBNull(0) Then
                                    ultimodoc = ""
                                Else
                                    ultimodoc = reader.Item(0)
                                End If
                            End While

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
            End If

            ' SI QUIEREN PONER ESTE ULTIMO DOC COMO LA MAYOR FECHA, SE HACE UN UPDATE de la ruta en ccDocumento y ccDocEmpleado
            If txtFechaDocto.Date >= ultimodoc Then
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"  UPDATE ccDocumento 
                                                SET rut = @rut, tipodocto = @tipo, fechadocto = @fecha, nrodocto = @nroDoc, 
                                                    vctodocto = @vcto, fechascan = @fscan,horascan = @hscan,usuarioscan = @user,descdocto = @desc
                                                    WHERE id = '{Session("IdDocumento")}'"

                            Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                            sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '            txtTipoDocto.Value = "CONTRA" Or
                            '            txtTipoDocto.Value = "EXAPRE" Or
                            '            txtTipoDocto.Value = "EXASIC" Or
                            '            txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If

                            sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                            sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                            sqlcmd2.ExecuteNonQuery()
                        Catch ex As Exception

                        End Try
                    End Using
                End If
                'ACtUALIZO LA RUTA DEL ccDOcEmpleado
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" UPDATE ccDocEmpleado 
                                    SET {txtTipoDocto.Value} = @ruta, {txtTipoDocto.Value}_FCREACION = @fecha, {txtTipoDocto.Value}_FVENCIMIENTO = @fechaVencimiento
                                    WHERE rut like '%{rut}%'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.Parameters.Add(New SqlParameter("@ruta", Session("pubFileName")))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '        txtTipoDocto.Value = "CONTRA" Or
                            '        txtTipoDocto.Value = "EXAPRE" Or
                            '        txtTipoDocto.Value = "EXASIC" Or
                            '        txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                            'End If
                            sqlcmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                Call CargarGridInferior()
                ShowPopUpMsg("¡Archivo Editado Exitosamente!")
                Session("pubFileName") = ""
                Session("Editar") = 0
                Session("IdDocumento") = Nothing
                txtArchivoCargado.Text = "Falta Cargar Archivo"
                popupCargaArchivo.ShowOnPageLoad = False
                Return
                'SI NO ES MAYOR SOLO SE ACTUALIZA EN ccDocumento
            Else
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"  UPDATE ccDocumento 
                                                SET rut = @rut, tipodocto = @tipo, fechadocto = @fecha, nrodocto = @nroDoc, 
                                                    vctodocto = @vcto, fechascan = @fscan,horascan = @hscan,usuarioscan = @user,descdocto = @desc
                                                    WHERE id = '{Session("IdDocumento")}'"

                            Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                            sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '            txtTipoDocto.Value = "CONTRA" Or
                            '            txtTipoDocto.Value = "EXAPRE" Or
                            '            txtTipoDocto.Value = "EXASIC" Or
                            '            txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If

                            sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                            sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                            sqlcmd2.ExecuteNonQuery()

                        Catch ex As Exception

                        End Try
                    End Using
                End If

                Call CargarGridInferior()
                ShowPopUpMsg("¡Archivo Editado Exitosamente!")
                Session("pubFileName") = ""
                Session("Editar") = 0
                Session("IdDocumento") = Nothing
                txtArchivoCargado.Text = "Falta Cargar Archivo"
                popupCargaArchivo.ShowOnPageLoad = False
                Return
            End If


        End If

        'SI VIENE CON ID Y CAMBIAN EL DOCUMENTO
        If Session("IdDocumento") <> Nothing And Session("Editar") = 2 Then

            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" SELECT id 
                                    FROM ccDocumento 
                                    WHERE id = '{Session("IdDocumento")}'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        idAntigua = sqlcmd.ExecuteScalar
                    Catch ex As Exception

                    End Try
                End Using
            End If

            'VEO SI EL RUT ESTÁ EN ccDocEmpleado
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" SELECT rut 
                                    FROM ccDocEmpleado
                                    WHERE rut like '%{txtIdEmpleado.Text}%'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        rut = sqlcmd.ExecuteScalar()
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            Else
                ShowPopUpMsg("No hay conexion a base datos")
            End If

            'SI EL RUT EXISTE OBTENGO EL ARCHIVO CON LA FECHA MÁXIMA EN ccDocumento
            If rut <> Nothing Then
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" SELECT max(fechadocto) 
                                    FROM [dbo].[ccDocumento] 
                                    where rut like '%{txtIdEmpleado.Text}%' and tipodocto = '{txtTipoDocto.Value}';"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader
                            While reader.Read
                                If reader.IsDBNull(0) Then
                                    ultimodoc = ""
                                Else
                                    ultimodoc = reader.Item(0)
                                End If
                            End While

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
            Else
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"INSERT INTO ccDocEmpleado (rut, {txtTipoDocto.Value}, {txtTipoDocto.Value}_FCREACION, {txtTipoDocto.Value}_FVENCIMIENTO)
                                                        VALUES (@rut, @ruta, @fecha, @vcto)"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            'txtTipoDocto.Value = "CONTRA" Or
                            'txtTipoDocto.Value = "EXAPRE" Or
                            'txtTipoDocto.Value = "EXASIC" Or
                            'txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If
                            sqlcmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" SELECT max(fechadocto) 
                                    FROM [dbo].[ccDocumento] 
                                    where rut like '%{txtIdEmpleado.Text}%' and tipodocto = '{txtTipoDocto.Value}';"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader
                            While reader.Read
                                If reader.IsDBNull(0) Then
                                    ultimodoc = ""
                                Else
                                    ultimodoc = reader.Item(0)
                                End If
                            End While

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
            End If

            If ConectaSQLServer() Then
                Using conn
                    Try
                        If Session("pubFileName") <> "" Then
                            Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                            IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
                            IO.File.Delete(fileName)
                        Else
                            ShowPopUpMsg("Favor cargue archivo")
                            Return
                        End If
                    Catch ex As Exception

                    End Try
                End Using
            Else
                ShowPopUpMsg("No hay conexion a base datos")
            End If

            'SI EL ARCHIVO ES MAYOR AL ULTIMO DOC EXISTENTE SE ACTUALIZA EL DOC EN ccDocEmpleado y en ccDocumento
            If txtFechaDocto.Date >= ultimodoc Then
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"  UPDATE ccDocumento 
                                                SET rut = @rut, tipodocto = @tipo, ruta = @ruta, fechadocto = @fecha, nrodocto = @nroDoc, 
                                                    vctodocto = @vcto, fechascan = @fscan,horascan = @hscan,usuarioscan = @user,descdocto = @desc
                                                    WHERE id = '{Session("IdDocumento")}'"

                            Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                            sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))
                            sqlcmd2.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))
                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '            txtTipoDocto.Value = "CONTRA" Or
                            '            txtTipoDocto.Value = "EXAPRE" Or
                            '            txtTipoDocto.Value = "EXASIC" Or
                            '            txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If

                            sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                            sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                            sqlcmd2.ExecuteNonQuery()

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                'ACtUALIZO LA RUTA DEL ccDOcEmpleado
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" UPDATE ccDocEmpleado 
                                    SET {txtTipoDocto.Value} = @ruta, {txtTipoDocto.Value}_FCREACION = @fecha, {txtTipoDocto.Value}_FVENCIMIENTO = @fechaVencimiento
                                    WHERE rut like '%{rut}%'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '        txtTipoDocto.Value = "CONTRA" Or
                            '        txtTipoDocto.Value = "EXAPRE" Or
                            '        txtTipoDocto.Value = "EXASIC" Or
                            '        txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                            'End If
                            sqlcmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                Call CargarGridInferior()
                ShowPopUpMsg("¡Archivo Editado Exitosamente!")
                Session("pubFileName") = ""
                Session("Editar") = 0
                Session("IdDocumento") = Nothing
                txtArchivoCargado.Text = "Falta Cargar Archivo"
                popupCargaArchivo.ShowOnPageLoad = False
                Return
                'ACA SOLO SE ACTUALIZA EL ccDocumento
            Else
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"  UPDATE ccDocumento 
                                                SET rut = @rut, tipodocto = @tipo, ruta = @ruta, fechadocto = @fecha, nrodocto = @nroDoc, 
                                                    vctodocto = @vcto, fechascan = @fscan,horascan = @hscan,usuarioscan = @user,descdocto = @desc
                                                    WHERE id = '{Session("IdDocumento")}'"

                            Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                            sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))
                            sqlcmd2.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))
                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '            txtTipoDocto.Value = "CONTRA" Or
                            '            txtTipoDocto.Value = "EXAPRE" Or
                            '            txtTipoDocto.Value = "EXASIC" Or
                            '            txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If

                            sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                            sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                            sqlcmd2.ExecuteNonQuery()

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                Call CargarGridInferior()
                ShowPopUpMsg("¡Archivo Editado Exitosamente!")
                Session("pubFileName") = ""
                Session("Editar") = 0
                Session("IdDocumento") = Nothing
                txtArchivoCargado.Text = "Falta Cargar Archivo"
                popupCargaArchivo.ShowOnPageLoad = False
                Return
            End If


        End If
#End Region

#Region "Cargar"
        'SI VIENE DESDE CARGAR ARCHIVO
        If Session("Editar") = 2 And Session("IdDocumento") = Nothing Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        If Session("pubFileName") <> "" Then
                            Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                            IO.File.Copy(fileName, fileName.Replace("Temp", "SCAN"))
                            IO.File.Delete(fileName)
                        Else
                            ShowPopUpMsg("Favor cargue archivo")
                            Return
                        End If
                    Catch ex As Exception

                    End Try
                End Using
            Else
                ShowPopUpMsg("No hay conexion a base datos")
            End If

            'VEO SI EL RUT ESTÁ EN ccDocEmpleado
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" SELECT rut 
                                    FROM ccDocEmpleado
                                    WHERE rut like '%{txtIdEmpleado.Text}%'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        rut = sqlcmd.ExecuteScalar()
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            Else
                ShowPopUpMsg("No hay conexion a base datos")
            End If

            'SI EL RUT EXISTE OBTENGO EL ULTIMO DOCUMENTO para luego actualizar ccDocEmp
            If rut <> "" Then
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" SELECT max(fechadocto) 
                                    FROM [dbo].[ccDocumento] 
                                    where rut like '%{txtIdEmpleado.Text}%' and tipodocto = '{txtTipoDocto.SelectedItem.Value}';"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            Dim reader As IDataReader = sqlcmd.ExecuteReader

                            While reader.Read
                                If reader.IsDBNull(0) Then
                                    ultimodoc = ""
                                Else
                                    ultimodoc = reader.Item(0)
                                End If
                            End While

                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                'SI NO EXISTE RUT INSERTO EN ccDocumento y en ccDocEmpleado
            Else
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = "INSERT INTO ccDocumento (rut, ruta, tipodocto, fechadocto, nrodocto,vctodocto, fechascan,horascan,usuarioscan,descdocto)  
                                VALUES(@rut, @ruta, @tipo, @fecha, @nrodoc, @vcto, @fscan, @hscan, @user, @desc)"

                            Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                            sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd2.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If

                            'If txtTipoDocto.Value = "CHAMAS" Or
                            'txtTipoDocto.Value = "CONTRA" Or
                            'txtTipoDocto.Value = "EXAPRE" Or
                            'txtTipoDocto.Value = "EXASIC" Or
                            'txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If

                            sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                            sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                            sqlcmd2.ExecuteNonQuery()

                        Catch ex As Exception

                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
                'INGRESO EN ccDocEmpleado
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $"INSERT INTO ccDocEmpleado (rut, {txtTipoDocto.Value}, {txtTipoDocto.Value}_FCREACION, {txtTipoDocto.Value}_FVENCIMIENTO)
                                                        VALUES (@rut, @ruta, @fecha, @vcto)"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            'txtTipoDocto.Value = "CONTRA" Or
                            'txtTipoDocto.Value = "EXAPRE" Or
                            'txtTipoDocto.Value = "EXASIC" Or
                            'txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If
                            sqlcmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If

                Call CargarGridInferior()
                ShowPopUpMsg("¡Archivo Cargado Exitosamente!")
                Session("pubFileName") = ""
                Session("Editar") = 0
                Session("IdDocumento") = Nothing
                cbxDescripcion.Text = Nothing
                txtArchivoCargado.Text = "Falta Cargar Archivo"
                popupCargaArchivo.ShowOnPageLoad = True
                Return
            End If

            'SI HAY ULTIMO DOCUMENTO SE ACTUALIZA EL ccDocEmpleado y el ccDocumento
            If ultimodoc <> "" Then
                ' SI EL DOC ACTUAL ES MAYOR AL ANTERIOR SE HACE UN UPDATE EN ccDocEmpleado Y UN INSERT EN ccDocumento
                If txtFechaDocto.Date >= ultimodoc Then
                    If ConectaSQLServer() Then
                        Using conn
                            Try
                                sqlStr = $" UPDATE ccDocEmpleado 
                                    SET {txtTipoDocto.Value} = @ruta, {txtTipoDocto.Value}_FCREACION = @fecha, {txtTipoDocto.Value}_FVENCIMIENTO = @fechaVencimiento
                                    WHERE rut like '%{rut}%'"
                                Dim sqlcmd As New SqlCommand(sqlStr, conn)
                                sqlcmd.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))

                                ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                                If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                    sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                                Else
                                    sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                                End If
                                'FECHA VENCIMIENTO DOCUMENTO 
                                If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                                Else
                                    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                                End If

                                'If txtTipoDocto.Value = "CHAMAS" Or
                                '    txtTipoDocto.Value = "CONTRA" Or
                                '    txtTipoDocto.Value = "EXAPRE" Or
                                '    txtTipoDocto.Value = "EXASIC" Or
                                '    txtTipoDocto.Value = "PACHOR" Then
                                '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                                'Else
                                '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                                'End If
                                sqlcmd.ExecuteNonQuery()
                            Catch ex As Exception
                                ShowPopUpMsg(ex.ToString)
                            End Try
                        End Using
                    Else
                        ShowPopUpMsg("No hay conexion a base datos")
                    End If
                    If ConectaSQLServer() Then
                        Using conn
                            Try
                                sqlStr = "INSERT INTO ccDocumento (rut, ruta, tipodocto, fechadocto, nrodocto,vctodocto, fechascan,horascan,usuarioscan,descdocto)  
                                VALUES(@rut, @ruta, @tipo, @fecha, @nrodoc, @vcto, @fscan, @hscan, @user, @desc)"

                                Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                                sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                                sqlcmd2.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))

                                ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                                If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                    sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                                Else
                                    sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                                End If
                                'FECHA VENCIMIENTO DOCUMENTO 
                                If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                                Else
                                    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                                End If
                                'If txtTipoDocto.Value = "CHAMAS" Or
                                '        txtTipoDocto.Value = "CONTRA" Or
                                '        txtTipoDocto.Value = "EXAPRE" Or
                                '        txtTipoDocto.Value = "EXASIC" Or
                                '        txtTipoDocto.Value = "PACHOR" Then
                                '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                                'Else
                                '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                                'End If

                                sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                                sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                                sqlcmd2.ExecuteNonQuery()

                            Catch ex As Exception

                            End Try
                        End Using
                    Else
                        ShowPopUpMsg("No hay conexion a base datos")
                    End If
                    Call CargarGridInferior()
                    ShowPopUpMsg("¡Archivo Cargado Exitosamente!")
                    Session("pubFileName") = ""
                    Session("Editar") = 0
                    Session("IdDocumento") = Nothing
                    cbxDescripcion.Text = Nothing
                    txtArchivoCargado.Text = "Falta Cargar Archivo"
                    popupCargaArchivo.ShowOnPageLoad = True
                    Return
                Else
                    'SINO; SOLO SE HACE UN INSERT EN ccDocumento
                    If ConectaSQLServer() Then
                        Using conn
                            Try
                                sqlStr = "INSERT INTO ccDocumento (rut, ruta, tipodocto, fechadocto, nrodocto,vctodocto, fechascan,horascan,usuarioscan,descdocto)  
                                VALUES(@rut, @ruta, @tipo, @fecha, @nrodoc, @vcto, @fscan, @hscan, @user, @desc)"

                                Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                                sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                                sqlcmd2.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))

                                ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                                If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                    sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                                Else
                                    sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                                End If
                                'FECHA VENCIMIENTO DOCUMENTO 
                                If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                                Else
                                    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                                End If
                                'If txtTipoDocto.Value = "CHAMAS" Or
                                '        txtTipoDocto.Value = "CONTRA" Or
                                '        txtTipoDocto.Value = "EXAPRE" Or
                                '        txtTipoDocto.Value = "EXASIC" Or
                                '        txtTipoDocto.Value = "PACHOR" Then
                                '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                                'Else
                                '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                                'End If

                                sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                                sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                                sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                                sqlcmd2.ExecuteNonQuery()


                            Catch ex As Exception

                            End Try
                        End Using
                    Else
                        ShowPopUpMsg("No hay conexion a base datos")
                    End If
                End If
                Call CargarGridInferior()
                ShowPopUpMsg("¡Archivo Cargado Exitosamente!")
                Session("pubFileName") = ""
                Session("Editar") = 0
                Session("IdDocumento") = Nothing
                cbxDescripcion.Text = Nothing
                txtArchivoCargado.Text = "Falta Cargar Archivo"
                popupCargaArchivo.ShowOnPageLoad = True
                Return
            Else
                'SI NO HAY DOC SE HACE UPDATE E INSERTA
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = $" UPDATE ccDocEmpleado 
                                    SET {txtTipoDocto.Value} = @ruta, {txtTipoDocto.Value}_FCREACION = @fecha, {txtTipoDocto.Value}_FVENCIMIENTO = @fechaVencimiento
                                    WHERE rut like '%{rut}%'"
                            Dim sqlcmd As New SqlCommand(sqlStr, conn)
                            sqlcmd.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '        txtTipoDocto.Value = "CONTRA" Or
                            '        txtTipoDocto.Value = "EXAPRE" Or
                            '        txtTipoDocto.Value = "EXASIC" Or
                            '        txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd.Parameters.Add(New SqlParameter("@fechaVencimiento", Now.ToString("yyyyMMdd")))
                            'End If
                            sqlcmd.ExecuteNonQuery()
                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
                If ConectaSQLServer() Then
                    Using conn
                        Try
                            sqlStr = "INSERT INTO ccDocumento (rut, ruta, tipodocto, fechadocto, nrodocto,vctodocto, fechascan,horascan,usuarioscan,descdocto)  
                                VALUES(@rut, @ruta, @tipo, @fecha, @nrodoc, @vcto, @fscan, @hscan, @user, @desc)"

                            Dim sqlcmd2 As New SqlCommand(sqlStr, conn)
                            sqlcmd2.Parameters.Add(New SqlParameter("@rut", txtIdEmpleado.Text))
                            sqlcmd2.Parameters.Add(New SqlParameter("@ruta", "~/SCAN/" + Session("pubFileName")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@tipo", txtTipoDocto.Value))

                            ' Dim fechaprueba = txtVenceDocto.Date.ToString("yyyyMMdd")
                            If txtFechaDocto.Date.ToString("yyyyMMdd") = "00010101" Or txtFechaDocto.Date.ToString("yyyyMMdd") = "30010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@fecha", txtFechaDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'FECHA VENCIMIENTO DOCUMENTO 
                            If txtVenceDocto.Date.ToString("yyyyMMdd") = "00010101" Then
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            Else
                                sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            End If
                            'If txtTipoDocto.Value = "CHAMAS" Or
                            '    txtTipoDocto.Value = "CONTRA" Or
                            '    txtTipoDocto.Value = "EXAPRE" Or
                            '    txtTipoDocto.Value = "EXASIC" Or
                            '    txtTipoDocto.Value = "PACHOR" Then
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", txtVenceDocto.Date.ToString("yyyyMMdd")))
                            'Else
                            '    sqlcmd2.Parameters.Add(New SqlParameter("@vcto", Now.ToString("yyyyMMdd")))
                            'End If

                            sqlcmd2.Parameters.Add(New SqlParameter("@nroDoc", "000000"))
                            sqlcmd2.Parameters.Add(New SqlParameter("@fscan", Now.ToString("yyyyMMdd")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@hscan", Now.ToString("HH:mm:ss")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@user", Session("pubIdUsuario")))
                            sqlcmd2.Parameters.Add(New SqlParameter("@desc", UCase(cbxDescripcion.Text)))

                            sqlcmd2.ExecuteNonQuery()

                        Catch ex As Exception
                            ShowPopUpMsg(ex.ToString)
                        End Try
                    End Using
                Else
                    ShowPopUpMsg("No hay conexion a base datos")
                End If
            End If
            Call CargarGridInferior()
            ShowPopUpMsg("¡Archivo Cargado Exitosamente!")
            Session("pubFileName") = ""
            Session("Editar") = 0
            Session("IdDocumento") = Nothing
            cbxDescripcion.Text = Nothing
            txtArchivoCargado.Text = "Falta Cargar Archivo"
            popupCargaArchivo.ShowOnPageLoad = True
            Return
        Else
            ShowPopUpMsg("FAVOR CARGUE ARCHIVO")
            Return
        End If


        Form.DefaultButton = Nothing

        If gridEmpleado.VisibleRowCount < 1 Then
            Return
        End If

        If gridEmpleado.FocusedRowIndex < 0 Then
            ShowPopUpMsg("Elija un colaborador")
            Return
        End If

        Call CargarGridInferior()
        ShowPopUpMsg("¡Archivo Cargado Exitosamente!")
        Session("pubFileName") = ""
        Session("Editar") = 0
        Session("IdDocumento") = Nothing
        cbxDescripcion.Text = Nothing
        txtArchivoCargado.Text = "Falta Cargar Archivo"
        popupCargaArchivo.ShowOnPageLoad = True
#End Region

    End Sub

    'BTN EDITAR
    Protected Sub btnEliminarArchivo_Click(sender As Object, e As EventArgs)

        Dim link As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer
        Dim archivo = ""
        Dim rut = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "rut")
        divlistBoxInferior.Visible = True
        listBoxInferior.Visible = True
        divGridInferior.Visible = True
        divPanelInferior.Visible = True
        'OBTENGO VALOR DE SELECCIÓN PARA HACER SUBSTRING
        Dim valor = listBoxInferior.Value.ToString()
        Dim tipoDocto = valor.Substring(0, valor.Length - 1)
        'MANEJA VISIBILIDAD DE GRIDS INDEPENDIENTES EN BASE A VENCIMIENTO
        Dim manejaVencimiento = valor.Substring(valor.Length - 1, 1)
        Dim id As Integer = 0
        Dim filename As String = ""

        If ConectaSQLServer() Then
            Using conn
                Try

                    If manejaVencimiento = 0 Then
                        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    Else
                        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    End If

                    'Select Case listBoxInferior.SelectedItem.Value
                    '    Case "Liquidaciones"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Pacto_HE"
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Estudios"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Masso"
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Contratos"
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Curriculum"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Der_Saber"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "EPP"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Reg_Interno"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Ev_Desempeño"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Ex_Preoc"
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Ex_Psico"
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Finiquito"
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    'End Select
                Catch ex As Exception

                End Try
            End Using
        End If

        'OBTENGO EL TIPODOCTO
        'If ConectaSQLServer() Then
        '    Using conn
        '        Try
        '            sqlStr = $" SELECT tipodocto 
        '                        FROM ccDocumento
        '                        WHERE ruta = '{filename}'"
        '            Dim sqlcmd As New SqlCommand(sqlStr, conn)
        '            tipoDocto = sqlcmd.ExecuteScalar
        '        Catch ex As Exception

        '        End Try
        '    End Using
        'End If

        'USO EL TIPODOCTO PARA OBTENER LA RUTA
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = $" SELECT {tipoDocto}
                                FROM ccDocEmpleado 
                                WHERE {tipoDocto} = '{filename}'"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    'ACA ME PUEDE DAR NULL
                    If IsDBNull(archivo = sqlcmd.ExecuteScalar) Then
                        Exit Try
                    Else
                        'VER MANEJO DE FECHA DE VENCIMIENTO
                        If manejaVencimiento = "1" Then
                            sqlStr = $" UPDATE ccDocEmpleado
                                    SET {tipoDocto} = ''
                                    WHERE {tipoDocto} = '{filename}'"
                            sqlcmd = New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                            ShowPopUpMsg("Documento Eliminado")
                        Else
                            Dim archivoVencimiento = tipoDocto + "_FVENCIMIENTO"
                            sqlStr = $" UPDATE ccDocEmpleado
                                    SET {tipoDocto} = '', {archivoVencimiento} = NULL
                                    WHERE {tipoDocto} = '{filename}'"
                            sqlcmd = New SqlCommand(sqlStr, conn)
                            sqlcmd.ExecuteNonQuery()
                            ShowPopUpMsg("Documento Eliminado")
                        End If

                    End If

                Catch ex As Exception

                End Try
            End Using
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "DELETE from ccDocumento WHERE id=@id"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.Parameters.Add(New SqlParameter("@id", id))
                    sqlcmd.ExecuteNonQuery()
                    'ELIMINACÍÓN DE ARCHIVO FÍSICO
                    IO.File.Delete(MapPath(filename))
                Catch ex As Exception
                    'ShowPopUpMsg(ex.ToString)
                End Try
            End Using

        Else
            ShowPopUpMsg("No hay conexion a base datos")
            Return
        End If

        If gridEmpleado.VisibleRowCount < 1 Then
            Return
        End If

        Call CargarGridInferior()

    End Sub

    'BTN EDITAR
    Protected Sub btnEditarDocumento_Click(sender As Object, e As EventArgs)
        'TRAER LOS DATOS
        Dim verVencimiento = 0
        Dim link As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer

        Dim id As Integer = 0
        Dim filename As String = ""

        If ConectaSQLServer() Then
            Using conn
                Try

                    'OBTENGO VALOR DE SELECCIÓN PARA HACER SUBSTRING
                    Dim valor = listBoxInferior.Value.ToString()

                    'MANEJA VISIBILIDAD DE GRIDS INDEPENDIENTES EN BASE A VENCIMIENTO
                    Dim manejaVencimiento = valor.Substring(valor.Length - 1, 1)
                    If manejaVencimiento = 0 Then
                        verVencimiento = 0
                        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    Else
                        verVencimiento = 1
                        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    End If

                    'Select Case listBoxInferior.SelectedItem.Value
                    '    Case "Liquidaciones"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Pacto_HE"
                    '        verVencimiento = 1
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Estudios"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Masso"
                    '        verVencimiento = 1
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Contratos"
                    '        verVencimiento = 1
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Curriculum"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Der_Saber"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "EPP"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Reg_Interno"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Ev_Desempeño"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Ex_Preoc"
                    '        verVencimiento = 1
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Ex_Psico"
                    '        verVencimiento = 1
                    '        id = CInt(GridInferiorVencimiento.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferiorVencimiento.GetRowValues(container.ItemIndex, "ruta")
                    '    Case "Finiquito"
                    '        verVencimiento = 0
                    '        id = CInt(GridInferior.GetRowValues(container.ItemIndex, "id"))
                    '        filename = GridInferior.GetRowValues(container.ItemIndex, "ruta")
                    'End Select
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If


        If ConectaSQLServer() Then
            Using conn
                Try
                    'DOCUMENTOS CON FECHA VENCIMIENTO
                    If verVencimiento = 1 Then
                        sqlStr = $" SELECT ccd.id, ccd.rut, ccve.nombre, ccd.tipodocto, ccd.fechadocto, ccd.descdocto, ccd.ruta, convert(datetime, dbo.fn_strAFechaDocumentos(vctodocto), 110) as vctodocto
                                FROM [dbo].[ccDocumento] ccd 
                                INNER JOIN ccViewEmpleados ccve 
                                ON ccd.rut = ccve.rut
                                WHERE ccd.id = '{id}'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        Dim reader As IDataReader = sqlcmd.ExecuteReader
                        While reader.Read()
                            txtEmpleado.Text = reader.Item("nombre")
                            txtIdEmpleado.Text = reader.Item("rut")
                            txtTipoDocto.Value = reader.Item("tipodocto")
                            txtFechaDocto.Date = reader.Item("fechadocto")
                            txtVenceDocto.Value = reader.Item("vctodocto")
                            If txtTipoDocto.Value = "CHAMAS" Then
                                cbxDescripcion.Items.Add("CHARLA MASSO")
                                cbxDescripcion.Items.Add("PROCEDIMIENTOS ESPECÍFICOS")
                                cbxDescripcion.Items.Add("FICHA SALUD Y SEGURIDAD")
                            End If
                            cbxDescripcion.Text = reader.Item("descdocto")
                            Session("Editar") = 1
                            Session("pubFileName") = reader.Item("ruta")
                            Session("IdDocumento") = reader.Item("id")
                        End While
                    Else
                        sqlStr = $" SELECT ccd.id, ccd.rut, ccve.nombre, ccd.tipodocto, ccd.fechadocto, ccd.descdocto, ccd.ruta
                                FROM [dbo].[ccDocumento] ccd 
                                INNER JOIN ccViewEmpleados ccve 
                                ON ccd.rut = ccve.rut
                                WHERE ccd.id = '{id}'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        Dim reader As IDataReader = sqlcmd.ExecuteReader
                        While reader.Read()
                            txtEmpleado.Text = reader.Item("nombre")
                            txtIdEmpleado.Text = reader.Item("rut")
                            txtTipoDocto.Value = reader.Item("tipodocto")
                            txtFechaDocto.Date = reader.Item("fechadocto")
                            If txtTipoDocto.Value = "DERSAB" Then
                                cbxDescripcion.Items.Add("OBLIGACIÓN DE INFORMAR")
                                cbxDescripcion.Items.Add("DERECHO A SABER")
                            End If
                            cbxDescripcion.Text = reader.Item("descdocto")
                            Session("Editar") = 1
                            Session("pubFileName") = reader.Item("ruta")
                            Session("IdDocumento") = reader.Item("id")
                        End While
                    End If

                Catch ex As Exception
                    ShowPopUpMsg("Error al Editar Archivo.\nRecargue la página. ")
                End Try
            End Using
        End If

        hlVerArchivo.NavigateUrl = filename
        hlVerArchivo.Target = "_blank"
        hlVerArchivo.Text = "Haga Click AQUÍ para ver archivo en Base de Datos"
        hlVerArchivo.Visible = True
        txtTipoDocto.ReadOnly = True
        txtTipoDocto.BackColor = Color.FromName("#EAEAEA")
        txtArchivoCargado.Text = "YA EXISTE UN ARCHIVO CARGADO"
        If verVencimiento = 1 Then
            tr_vencimiento.Attributes("style") = "visibility: visible;"
        Else
            tr_vencimiento.Attributes("style") = "visibility: hidden;"
        End If
        popupCargaArchivo.ShowOnPageLoad = True
    End Sub

#End Region

    'CALLBACKS
#Region "CALLBACKS"

    Protected Sub cbpInferior_Callback(sender As Object, e As CallbackEventArgsBase)
        Dim visibleRows As Integer = gridEmpleado.VisibleRowCount
        If visibleRows > 0 And gridEmpleado.FocusedRowIndex >= 0 Then
            divlistBoxInferior.Visible = True
            listBoxInferior.Visible = True
            divGridInferior.Visible = True
            divPanelInferior.Visible = True

            Call CargarGridInferior()
        End If

    End Sub

    Protected Sub cbpEditar_Callback(sender As Object, e As CallbackEventArgsBase)
        txtEmpleado.Text = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "nombre")
        txtIdEmpleado.Text = gridEmpleado.GetRowValues(gridEmpleado.FocusedRowIndex, "rut")
        hlVerArchivo.ClientVisible = False
        txtTipoDocto.ReadOnly = False
        txtTipoDocto.BackColor = Color.White
        txtTipoDocto.SelectedIndex = -1
        txtFechaDocto.Date = Now()
        tr_vencimiento.Attributes("style") = "visibility: hidden;"
        txtVenceDocto.Value = Nothing
        cbxDescripcion.Text = Nothing
        Session("IdDocumento") = Nothing
        Session("Editar") = 0
        Session("pubFileName") = Nothing
        txtArchivoCargado.Text = "Falta Cargar Archivo"
    End Sub

    Protected Sub cbpPanelIzquierdo_Callback(sender As Object, e As CallbackEventArgsBase)
        cbxUnidad.Value = Nothing
        Session("pubEmpUsuariaUsuario") = cbxEmpresaPagina.Value
        If ConectaSQLServer() Then
            Using conn
                Try
                    'CARGA COMBOBOX
                    Dim sqlCombo = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion from ccUnidades WHERE idEmpUsuaria IN ('" & Session("pubEmpUsuariaUsuario") & "') ORDER BY descripcion"
                    Call spLlenaComboBoxPopUp(sqlCombo, cbxUnidad, "idUnidad", "descripcion")
                Catch ex As Exception
                    ShowPopUpMsg("Error 1: {0}" + ex.ToString)
                End Try
            End Using
        End If
    End Sub

#End Region

    Private Const TempDirectory As String = "~/Temp/"
    Protected Sub UploadControl_FileUploadComplete(sender As Object, e As DevExpress.Web.FileUploadCompleteEventArgs)
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
                    Session("pubFileName") = Trim(txtIdEmpleado.Text) + "-" + txtTipoDocto.Value + "-" + Now().ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName)
                    Dim fileName As String = String.Format("{0}{1}", MapPath("~/Temp/"), Session("pubFileName"))
                    file.SaveAs(fileName, True)
                    Session("Editar") = 2
                End If
            Next i
        End If

    End Sub

    'MENSAJES EN PANTALLA
    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Sub visibilidadGridInferior(ByVal id As String)
        Dim controlaVencimiento = id.Substring(id.Length - 1, 1)
        If controlaVencimiento = 1 Then
            GridInferiorVencimiento.Visible = True
            GridInferior.Visible = False
        Else
            GridInferiorVencimiento.Visible = False
            GridInferior.Visible = True
        End If
    End Sub


    Protected Sub cbpVerEvaluaciones_Callback(sender As Object, e As CallbackEventArgsBase)
        'SE TOMA EL PARÁMETRO QUE VIENE DESDE EL LADO CLIENTE
        Dim celda = e.Parameter
        'SE OBTIENE EL RUT
        Dim rut = gridEmpleado.GetRowValues(celda, "rut")
        'SE CARGA LA VARIABLE DE SESIÓN PARA EL RUT CORRESPONDIENTE
        Session("rutBusquedaEvaluacionColaboradores") = rut
        'SE CARGA LA GRID ENTREGÁNDOLE EL RUT EN LA VARIABLE SESIÓN
        CargaGridEvaluacion(Session("rutBusquedaEvaluacionColaboradores"))
    End Sub

    Protected Sub gridVerEvaluacion_PageIndexChanged(sender As Object, e As EventArgs)
        CargaGridEvaluacion(Session("rutBusquedaEvaluacionColaboradores"))
    End Sub

    'PROCEDIMIENTO QUE OCULTA COLUMNA EVALUACION
    Protected Sub OcultarColumnaEvaluacion()
        'LA COLUMNA DE EVALUACION ES VISIBLE SI LA EMPRESA ES ARAUCO
        If Session("pubEmpUsuariaUsuario").Equals("PANL") Then
            gridEmpleado.Columns.Item("evaluacion").Visible = True
        Else
            gridEmpleado.Columns.Item("evaluacion").Visible = False
        End If
        'If Session("Cliente") = True Then
        '    gridEmpleado.Columns.Item("evaluacion").Visible = False
        'Else
        '    If Session("pubEmpUsuariaUsuario").Equals("PANL") Then
        '        gridEmpleado.Columns.Item("evaluacion").Visible = True
        '    Else
        '        gridEmpleado.Columns.Item("evaluacion").Visible = False
        '    End If
        'End If
    End Sub

    Sub CargaGridEvaluacion(ByVal rut)
        'SE CARGA LA GRID CON DATOS DE BD Y SELECCIÓN DE CBXUNIDAD
        sqlStr = $" SELECT * 
                    FROM Vw_ccEvaluacion
                    WHERE rut='{rut}'"
        If ConectaSQLServer() Then
            Using conn
                spllenaGridView(gridVerEvaluacion, sqlStr)
                If gridVerEvaluacion.VisibleRowCount > 0 Then
                    gridVerEvaluacion.Visible = True
                End If
            End Using
        End If

        'CAMBIO NOMBRES DE COLUMNAS
        gridVerEvaluacion.DataColumns.Item("rut").Caption = "Rut"
        gridVerEvaluacion.DataColumns.Item("nombre").Caption = "Nombre"
        gridVerEvaluacion.DataColumns.Item("ames").Caption = "Fecha"
        gridVerEvaluacion.DataColumns.Item("id").Caption = "Planta"
        gridVerEvaluacion.DataColumns.Item("area").Caption = "Area"
        gridVerEvaluacion.DataColumns.Item("cargo").Caption = "Cargo"
        gridVerEvaluacion.DataColumns.Item("userEvalua").Caption = "Evaluador"
        gridVerEvaluacion.DataColumns.Item("Nota01").Caption = "Procedimientos de seguridad"
        gridVerEvaluacion.DataColumns.Item("Nota02").Caption = "Responsabilidad en tareas"
        gridVerEvaluacion.DataColumns.Item("Nota03").Caption = "Realización de labores"
        gridVerEvaluacion.DataColumns.Item("Nota04").Caption = "Relación respetuosa"
        gridVerEvaluacion.DataColumns.Item("NotaC01").Caption = "Dominio técnico"
        gridVerEvaluacion.DataColumns.Item("NotaC02").Caption = "Superación de obstáculos"
        gridVerEvaluacion.DataColumns.Item("NotaC03").Caption = "Cumplimiento de reglas"
        gridVerEvaluacion.DataColumns.Item("NotaC04").Caption = "Actitud respetuosa"
        gridVerEvaluacion.DataColumns.Item("esRecomendado").Caption = "Se recomienda"
        gridVerEvaluacion.DataColumns.Item("idArea").Visible = False
        gridVerEvaluacion.DataColumns.Item("tipoEvaluacion").Visible = False
    End Sub

End Class