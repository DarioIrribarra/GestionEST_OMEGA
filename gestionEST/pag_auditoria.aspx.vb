Imports System.Data.SqlClient
Imports DevExpress.Export
Imports DevExpress.Web
Imports DevExpress.XtraPrinting

Public Class pag_auditoria
    Inherits System.Web.UI.Page

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

        If IsPostBack Then
            gridDocumentacion.ClientVisible = True
            lblDefinicionAuditado.Visible = True
        End If
    End Sub

    Private Sub pag_auditoria_Init(sender As Object, e As EventArgs) Handles Me.Init
        If IsPostBack = False Then
            CargarGridDocumentacion()
            gridDocumentacion.ClientVisible = False
            lblDefinicionAuditado.Visible = False
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

        'ACCESOS DE PERFIL: ESTE ACCESO SOLO BLOQUEA QUE PERFIL WEB CLICKEE EL CHECKBOX. 
        'POR ESO SE DA EN EL CHECKBOX

    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Protected Sub txtUnidadEmpresa_Init(sender As Object, e As EventArgs)
        '++++++ACTIVAR EN CASO DE QUERER QUE AUDITOR SOLO CARGUE UNIDADES ASIGNADAS
        'CARGA DE SOLO UNIDADES QUE CORRESPONDEN AL PERFIL SI NO ES ADMIN, WEB 
        'If Session("Auditor") = True Then
        '    sqlStr = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion 
        '            FROM ccViewEmpleados 
        '            WHERE id IN (" & TraeUnidadesAuditoria() & ") 
        '            ORDER BY descripcion"
        '    Call spLlenaComboBox(sqlStr, cbxUnidad, "idUnidad", "descripcion")
        'Else
        sqlStr = "SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion 
                    FROM ccViewEmpleados 
                    WHERE id IN (" & Session("pubUnidadesUsuario") & ") 
                    ORDER BY descripcion"
        Call spLlenaComboBox(sqlStr, cbxUnidad, "idUnidad", "descripcion")
        'End If

    End Sub

    Private Sub CargarGridDocumentacion()
        'QUERY FINAL
        sqlStr = "SELECT ccde.rut as rut, ccve.nombre as nombre, ccve.id as planta, ccve.estado"
        'SE OBTIENEN TODAS LAS COLUMNAS TIPO DOCTO Y SE AÑADEN A LA QUERY FINAL
        Dim queryTipoDocto = "  SELECT COLUMN_NAME
                                FROM INFORMATION_SCHEMA.COLUMNS
                                WHERE TABLE_NAME = N'ccDocEmpleado'
                                AND COLUMN_NAME NOT LIKE '%FCREACION%'
                                AND COLUMN_NAME NOT LIKE '%FVENCIMIENTO%'
                                AND COLUMN_NAME != 'rut'
                                AND COLUMN_NAME != 'bAuditado'
                                ORDER BY COLUMN_NAME ASC"
        Dim reader As SqlDataReader
        If ConectaSQLServer() Then
            Using conn
                Dim cmd As New SqlCommand(queryTipoDocto, conn)
                reader = cmd.ExecuteReader
                While reader.Read()
                    sqlStr = sqlStr + ", " + "ccde." + reader.Item(0)
                End While
            End Using
        End If
        'SE AÑADE LO RESTANTE A LA QUERY SIN EL FILTRO
        sqlStr = sqlStr + ", ccde.bauditado FROM [dbo].[ccDocEmpleado] ccde
                            INNER JOIN ccViewEmpleados ccve
                            ON ccde.rut = ccve.rut"

        '++++++ACTIVAR EN CASO DE QUERER QUE AUDITOR SOLO CARGUE UNIDADES ASIGNADAS

        'PARA AUDITOR SE SACAN DIRECTAMENTE DE LA BD
        'If Session("Auditor") = True Then

        '    If cbxUnidad.Value = Nothing Then
        '        sqlStr = sqlStr + " WHERE ccve.estado IN ('A', 'E', 'S') AND 
        '                ccve.id IN (" & TraeUnidadesAuditoria() & ")
        '                ORDER BY ccve.estado ASC, ccve.nombre asc;"
        '        Call spllenaGridViewAuditoria(gridDocumentacion, sqlStr)
        '    Else
        '        'PARA UNA SOLA PLANTA
        '        sqlStr = sqlStr + " WHERE ccve.id = '" + cbxUnidad.SelectedItem.Value + "'
        '            AND ccve.estado IN ('A', 'E', 'S')
        '            ORDER BY ccve.estado ASC, ccve.nombre asc;"
        '        Call spllenaGridViewAuditoria(gridDocumentacion, sqlStr)
        '    End If

        'Else
        If cbxUnidad.Value = Nothing Then
            sqlStr = sqlStr + " WHERE ccve.estado IN ('A', 'E', 'S') AND 
                        ccve.id IN (" & Session("pubUnidadesUsuario") & ")
                        ORDER BY ccve.estado ASC, ccve.nombre asc;"
            Call spllenaGridViewAuditoria(gridDocumentacion, sqlStr)
        Else
            'PARA UNA SOLA PLANTA
            sqlStr = sqlStr + " WHERE ccve.id = '" + cbxUnidad.SelectedItem.Value + "'
                    AND ccve.estado IN ('A', 'E', 'S')
                    ORDER BY ccve.estado ASC, ccve.nombre asc;"
            Call spllenaGridViewAuditoria(gridDocumentacion, sqlStr)
        End If
        'End If

    End Sub

    Protected Function TraeUnidadesAuditoria()
        Dim unidades = ""
        Dim sqlUnidades = $"SELECT unidades 
                                FROM ccUsuarioWeb
                                WHERE usuario = '{Session("pubIdUsuario")}'"
        If ConectaSQLServer() Then
            Using conn
                Dim sqlcmd As New SqlCommand(sqlUnidades, conn)
                Try
                    Using readerUnidades As SqlDataReader = sqlcmd.ExecuteReader
                        While readerUnidades.Read()
                            unidades = readerUnidades("unidades")
                        End While
                    End Using
                    If unidades = "" Then
                        unidades = "''"
                    End If
                Catch ex As Exception
                    unidades = "''"
                End Try
            End Using

        End If
        Return unidades
    End Function

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)
        Call ActualizaAuditoria()
        Call CargarGridDocumentacion()
        gridDocumentacion.ClearSort()
        gridDocumentacion.PageIndex = 0
        gridDocumentacion.ClientVisible = True
        lblDefinicionAuditado.Visible = True
        tr_excel.Attributes("style") = "visibility: visible;"
    End Sub

    'Protected Sub gridDocumentacion_BeforeGetCallbackResult(sender As Object, e As EventArgs)
    '    Call CargarGridDocumentacion()
    'End Sub

    Protected Sub gridDocumentacion_PageIndexChanged(sender As Object, e As EventArgs)
        Call CargarGridDocumentacion()
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs)

        If gridDocumentacion.Visible = False Then
            ShowPopUpMsg("ERROR: NO TIENE UNIDADES ASIGNADAS")
            Exit Sub
        End If

        'DICCIONARIO PARA ALMACENAR DATOS
        Dim diccionarioTipoDoctoYControlaVencimiento As New Dictionary(Of String, String)
        'Dim listado As New List(Of Object)
        'OBTENER TODOS LOS TIPO DOCTO Y VALOR DE VENCIMIENTO DE DOCUMENTOS QUE SE MUESTRAN EN AUDITORIA
        sqlStr = "SELECT id, controlaVencimiento, nombreEnAuditoria
                    FROM ccTIpoDocto
                    WHERE activoEnAuditoria = 1"
        Try
            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    Using reader = sqlcmd.ExecuteReader
                        While reader.Read()
                            diccionarioTipoDoctoYControlaVencimiento.Add(reader.Item(0), reader.Item(2).ToString + reader.Item(1).ToString)
                            'listado.Add(reader.Item)
                        End While
                    End Using
                End Using

            End If
        Catch ex As Exception
            ShowPopUpMsg(ex.ToString)
        End Try

        'SI NO TIENE FECHA VENCIMIENTO PERO SI PRESENTA DOCUMENTO, SE CAMBIA A 'VIGENTE', SINO A ''
        sqlStr = "SELECT ccde.rut as rut, ccve.nombre as nombre, ccve.id as planta, ccve.estado"
        For Each item As Object In diccionarioTipoDoctoYControlaVencimiento.Keys
            If diccionarioTipoDoctoYControlaVencimiento(item).Substring(diccionarioTipoDoctoYControlaVencimiento(item).Length - 1, 1) = "0" Then
                sqlStr = sqlStr + ", CASE WHEN " + item + " LIKE '%SCAN%' THEN 'VIGENTE' ELSE '' END AS " + item
            Else
                sqlStr = sqlStr + ", CASE WHEN CONVERT(varchar, " + item + "_FVENCIMIENTO, 112) < convert(varchar, getdate(), 112) THEN 'CADUCADO' 
                                    WHEN convert(varchar, " + item + "_FVENCIMIENTO, 112) BETWEEN convert(varchar, getdate(), 112) AND convert(varchar, DATEADD(dd, 20, getdate()), 112) THEN '20 DIAS VIGENCIA'
                                    WHEN convert(varchar, " + item + "_FVENCIMIENTO, 112) >= convert(varchar, DATEADD(dd, 20, getdate()), 112) THEN 'VIGENTE'
                                    WHEN " + item + "_FVENCIMIENTO IS NULL THEN ''
                                    END AS " + item + ""
            End If

        Next

        '++++++ACTIVAR EN CASO DE QUERER QUE AUDITOR SOLO CARGUE UNIDADES ASIGNADAS
        'PARA AUDITOR SE SACAN DIRECTAMENTE DE LA BD
        'If Session("Auditor") = True Then
        '    If cbxUnidad.Value = Nothing Then
        '        sqlStr = sqlStr + $", CASE WHEN ccde.bauditado = 0 THEN 'NO' ELSE 'SI' END AS 'bauditado'
        '                FROM [dbo].[ccDocEmpleado] ccde 
        '                INNER JOIN ccViewEmpleados ccve
        '                ON ccde.rut = ccve.rut
        '                WHERE ccve.id IN (" & TraeUnidadesAuditoria() & ")
        '                AND ccve.estado IN ('A', 'E', 'S')
        '                ORDER BY ccve.estado ASC, ccve.nombre asc;"
        '    Else
        '        'PARA UNA SOLA PLANTA
        '        sqlStr = sqlStr + ", CASE WHEN ccde.bauditado = 0 THEN 'NO' ELSE 'SI' END AS 'bauditado'
        '                FROM [dbo].[ccDocEmpleado] ccde
        '                INNER JOIN ccViewEmpleados ccve
        '                ON ccde.rut = ccve.rut
        '                WHERE ccve.id = '" + cbxUnidad.SelectedItem.Value + "'
        '                AND ccve.estado IN ('A', 'E', 'S')
        '                ORDER BY ccve.estado ASC, ccve.nombre asc;"
        '    End If
        'Else
        If cbxUnidad.Value = Nothing Then
            sqlStr = sqlStr + ", CASE WHEN ccde.bauditado = 0 THEN 'NO' ELSE 'SI' END AS 'bauditado'
                        FROM [dbo].[ccDocEmpleado] ccde 
                        INNER JOIN ccViewEmpleados ccve
                        ON ccde.rut = ccve.rut
                        WHERE ccve.id IN (" & Session("pubUnidadesUsuario") & ")
                        AND ccve.estado IN ('A', 'E', 'S')
                        ORDER BY ccve.estado ASC, ccve.nombre asc;"
        Else
            'PARA UNA SOLA PLANTA
            sqlStr = sqlStr + ", CASE WHEN ccde.bauditado = 0 THEN 'NO' ELSE 'SI' END AS 'bauditado'
                        FROM [dbo].[ccDocEmpleado] ccde
                        INNER JOIN ccViewEmpleados ccve
                        ON ccde.rut = ccve.rut
                        WHERE ccve.id = '" + cbxUnidad.SelectedItem.Value + "'
                        AND ccve.estado IN ('A', 'E', 'S')
                        ORDER BY ccve.estado ASC, ccve.nombre asc;"
        End If

        'End If

        Call spllenaGridViewAuditoria(gridDocumentacion, sqlStr)

        For Each item In diccionarioTipoDoctoYControlaVencimiento.Keys
            gridDocumentacion.DataColumns.Item(item).Caption = diccionarioTipoDoctoYControlaVencimiento(item).Substring(0, diccionarioTipoDoctoYControlaVencimiento(item).Length - 1)
        Next
        'SE CREA EL REPORTE
        gridExport.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
    End Sub

    Protected Function imgEstado_A_Load(sender As Object, grid As ASPxGridView)
        'aca es el evento de la imagen dentro del itemtemplate

        Dim image As ASPxImage = sender
        Dim container As GridViewDataItemTemplateContainer = image.NamingContainer
        Dim myValue As String = grid.GetRowValues(container.ItemIndex, container.Column.FieldName)

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
        Return True
    End Function

    Public Function ASPxHyperLink1_Init(sender As Object, grid As ASPxGridView, DiccionarioTipoYVencimiento As Dictionary(Of String, String))
        Dim link As ASPxHyperLink = sender
        Dim gridn = grid
        Dim container As GridViewDataItemTemplateContainer = link.NamingContainer
        Dim rut = gridn.GetRowValues(container.ItemIndex, "rut")
        Dim myValue As String = gridn.GetRowValues(container.ItemIndex, container.Column.FieldName)



        Try
            'COLOR VERDE PARA LOS QUE NO TIENEN FECHA VENCIMIENTO
            If myValue <> "" Then

                If DiccionarioTipoYVencimiento(container.Column.FieldName) = 0 Then
                    link.ImageUrl = "~/images/15.png"
                    link.NavigateUrl = myValue
                    link.Target = "_blank"
                Else
                    If ConectaSQLServer() Then
                        Using conn
                            Try
                                sqlStr = $" SELECT vctodocto
                                        FROM [dbo].[ccDocumento] 
                                        INNER JOIN ccDocEmpleado 
                                        ON ccDocumento.rut = ccDocEmpleado.rut
                                        WHERE ccDocumento.rut like '%{rut}%' and ccDocumento.ruta = '{myValue}'"
                                Dim sqlcmd As New SqlCommand(sqlStr, conn)
                                Dim fecha = sqlcmd.ExecuteScalar()
                                If fecha <> Nothing Then
                                    fecha = DateTime.ParseExact(fecha, "yyyyMMdd", Nothing)
                                Else
                                    link.ImageUrl = "~/images/12.png"
                                    link.NavigateUrl = myValue
                                    link.Target = "_blank"
                                End If

                                'SI LA FECHA DE VENCIMIENTO ES MENOR A HOY, COLOR ROJO
                                If fecha < DateTime.Today Then
                                    link.ImageUrl = "~/images/12.png"
                                    link.NavigateUrl = myValue
                                    link.Target = "_blank"

                                    'SI LA FECHA ESTÁ ENTRE HOY Y 20 DÍAS MÁS AMARILLO
                                ElseIf (fecha >= DateTime.Today And fecha < DateTime.Today.AddDays(20)) Then
                                    link.ImageUrl = "~/images/36.png"
                                    link.NavigateUrl = myValue
                                    link.Target = "_blank"

                                    'SI LA FECHA POR DEFECTO ES MAYOR A A 20 DíAS MÁS, VERDE
                                ElseIf fecha >= DateTime.Today.AddDays(20) Then
                                    link.ImageUrl = "~/images/15.png"
                                    link.NavigateUrl = myValue
                                    link.Target = "_blank"
                                End If

                            Catch ex As Exception
                                ShowPopUpMsg(ex.ToString)
                            End Try
                        End Using
                    End If
                End If
            Else
                link.ImageUrl = "~/images/14.png"
                link.NavigateUrl = ""
            End If
        Catch ex As Exception
            ShowPopUpMsg(ex.ToString)
        End Try
        Return True
    End Function

    Protected Function chkAuditado_Init(sender As Object, grid As ASPxGridView)

        Dim cbox As ASPxCheckBox = sender
        Dim container As GridViewDataItemTemplateContainer = cbox.NamingContainer
        Dim myValue As String = grid.GetRowValues(container.ItemIndex, "rut")
        'ACCESO WEB DE SOLO VISUALIZACIÓN
        If Session("Web") = True Or Session("Operaciones") = True Then
            cbox.Enabled = False
        End If
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = $" SELECT bAuditado 
                                    FROM ccDocEmpleado
                                    WHERE rut = '{myValue}'"
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    Dim valor = sqlcmd.ExecuteScalar
                    If valor = 1 Then
                        cbox.Checked = True
                    Else
                        cbox.Checked = False
                    End If
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using
        End If
        Return True
    End Function

    Protected Sub chkAuditado_CheckedChanged(sender As Object, e As CallbackEventArgs)
        Dim datosSeparados() = e.Parameter.Split("-")
        'Dim cbox As ASPxCheckBox = sender
        'Dim container As GridViewDataItemTemplateContainer = cbox.NamingContainer
        Dim myValue As String = gridDocumentacion.GetRowValues(datosSeparados(0), "rut")

        If datosSeparados(1) = "true" Then
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" UPDATE ccDocEmpleado 
                                    SET bAuditado = 1
                                    WHERE rut = '{myValue}'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.ExecuteNonQuery()
                        Call CargarGridDocumentacion()
                        'ShowPopUpMsg($"¡Auditoría de {gridDocumentacion.GetRowValues(datosSeparados(0), "nombre")} actualizada!")
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            End If
        Else
            If ConectaSQLServer() Then
                Using conn
                    Try
                        sqlStr = $" UPDATE ccDocEmpleado 
                                    SET bAuditado = 0
                                    WHERE rut = '{myValue}'"
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.ExecuteNonQuery()
                        Call CargarGridDocumentacion()
                        'ShowPopUpMsg($"¡Auditoría de {gridDocumentacion.GetRowValues(datosSeparados(0), "nombre")} actualizada!")
                    Catch ex As Exception
                        ShowPopUpMsg(ex.ToString)
                    End Try
                End Using
            End If
        End If
    End Sub

    'ACTUALIZACIÓN DE AUDITORÍA
    Protected Sub ActualizaAuditoria()
        Dim sqlcmd As New SqlCommand
        Try
            If ConectaSQLServer() Then
                Using conn
                    sqlcmd.Connection = conn
                    sqlcmd.CommandType = CommandType.StoredProcedure
                    sqlcmd.CommandText = "prActualizaAuditoría"
                    sqlcmd.ExecuteNonQuery()
                    sqlcmd.Dispose()
                End Using
            End If
        Catch ex As Exception
            ShowPopUpMsg("No se puede actualizar audítoría. Revise el procedimiento almacenado prActualizaAuditoría")
        End Try
    End Sub

    Protected Sub gridDocumentacion_Init(sender As Object, e As EventArgs)
        Call CargarGridDocumentacion()
    End Sub

    Protected Sub gridDocumentacion_Load(sender As Object, e As EventArgs)
        '1.- LISTADO PARA OBTENER TODOS LOS TIPO DOCTO
        Dim listadoTipoDoc As New List(Of String)
        sqlStr = "  SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = N'ccDocEmpleado'
                    AND COLUMN_NAME NOT LIKE '%FCREACION%'
                    AND COLUMN_NAME NOT LIKE '%FVENCIMIENTO%'
                    AND COLUMN_NAME != 'rut'
                    AND COLUMN_NAME != 'bAuditado'
                    ORDER BY COLUMN_NAME ASC"
        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Using reader = cmd.ExecuteReader
                        While reader.Read()
                            listadoTipoDoc.Add("'" + reader.Item(0).ToString() + "'")
                        End While
                    End Using
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString())
                End Try
            End Using
        End If
        'LISTADO EN STRING PARA PREPARAR QUERY
        Dim listadoEnString = String.Join(",", listadoTipoDoc)

        '2.- DICCIONARIO PARA OBTENER CADA NOMBRE DEPENDIENDO DEL TIPO DOCTO (HACER UN FOR EACH Y RELLENAR)
        Dim diccionarioTipoYDescripcion As New Dictionary(Of String, String)
        sqlStr = "SELECT id, nombreEnAuditoria, controlaVencimiento
                    FROM ccTipoDocto
                    WHERE id IN (" + listadoEnString + ")
                    AND activoEnAuditoria = 1
                    ORDER BY nombreEnAuditoria ASC"

        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Using reader = cmd.ExecuteReader
                        While reader.Read()
                            'SI TIENE UN ESPACIO SE SEPARA EL TEXTO EN 2 LÍNEAS
                            If InStr(reader.Item(1), " ") > 0 Then
                                Dim nombreSeparado() As String = reader.Item(1).ToString.Split(" ")
                                Dim listado As List(Of String) = nombreSeparado.ToList()
                                Dim nombreJunto = String.Join("</br>", listado)
                                diccionarioTipoYDescripcion.Add(reader.Item(0), nombreJunto)
                            Else
                                diccionarioTipoYDescripcion.Add(reader.Item(0), reader.Item(1))
                            End If
                        End While
                    End Using
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString())
                End Try
            End Using

        End If

        '3.- DICCIONARIO DE SOLO TIPODOCTO Y VENCIMIENTO PARA PASARLO AL DATATEMPLATE
        Dim diccionarioTipoYVencimiento As New Dictionary(Of String, String)
        sqlStr = "SELECT id, controlaVencimiento
                    FROM ccTipoDocto
                    WHERE id IN (" + listadoEnString + ")
                    AND activoEnAuditoria = 1
                    ORDER BY nombreEnAuditoria ASC"

        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Using reader = cmd.ExecuteReader
                        While reader.Read()
                            diccionarioTipoYVencimiento.Add(reader.Item(0), reader.Item(1))
                        End While
                    End Using
                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString())
                End Try
            End Using
        End If

        'TODAS INVISIBLES PARA ELEGIR SOLO ALGUNAS
        For Each columna As GridViewColumn In gridDocumentacion.Columns
            columna.Visible = False
        Next

        Try
            '--------COLUMNAS BASE----------
            'PROPIEDADES DE COLUMNA RUT
            gridDocumentacion.Columns("rut").Visible = True
            gridDocumentacion.Columns("rut").Width = 83%
            gridDocumentacion.Columns("rut").Caption = "RUT"
            'PROPIEDADES DE COLUMNA NOMBRE
            gridDocumentacion.Columns("nombre").Visible = True
            gridDocumentacion.Columns("nombre").Width = 150%
            gridDocumentacion.Columns("nombre").Caption = "NOMBRE"
            'PROPIEDADES DE COLUMNA PLANTA
            gridDocumentacion.Columns("planta").Visible = True
            gridDocumentacion.Columns("planta").Caption = "PLANTA"
            gridDocumentacion.Columns("planta").CellStyle.HorizontalAlign = HorizontalAlign.Center
            gridDocumentacion.Columns("planta").Width = 60%

            'AÑADO IMÁGENES Y CONFIGURACIONES A COLUMNA ESTADO
            gridDocumentacion.Columns("estado").Visible = True
            gridDocumentacion.Columns("estado").Caption = "ESTADO"
            gridDocumentacion.Columns("estado").CellStyle.HorizontalAlign = HorizontalAlign.Center
            gridDocumentacion.Columns("estado").Width = 73%
            TryCast(gridDocumentacion.Columns("estado"), GridViewDataColumn).Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
            TryCast(gridDocumentacion.Columns("estado"), GridViewDataColumn).Settings.FilterMode = ColumnFilterMode.DisplayText
            TryCast(gridDocumentacion.Columns("estado"), GridViewDataColumn).Settings.AllowSort = DevExpress.Utils.DefaultBoolean.False
            TryCast(gridDocumentacion.Columns("estado"), GridViewDataColumn).Settings.AllowGroup = DevExpress.Utils.DefaultBoolean.False
            TryCast(gridDocumentacion.Columns("estado"), GridViewDataColumn).DataItemTemplate = New ImagenesEstado(gridDocumentacion)

            'AÑADIR COLUMNAS DE TIPOS DE DOCUMENTOS
            For Each nombreColumna As String In diccionarioTipoYDescripcion.Keys
                gridDocumentacion.Columns(nombreColumna).Visible = True
                gridDocumentacion.Columns(nombreColumna).Caption = diccionarioTipoYDescripcion(nombreColumna)
                gridDocumentacion.Columns(nombreColumna).CellStyle.HorizontalAlign = HorizontalAlign.Center
                'LE PASO LA GRID A LA CLASE QUE CREA EL DATATEMPLATE
                TryCast(gridDocumentacion.Columns(nombreColumna), GridViewDataTextColumn).DataItemTemplate = New ImagenesArchivosGrid(gridDocumentacion, diccionarioTipoYVencimiento)
                gridDocumentacion.Columns(nombreColumna).Width = 95%
            Next

            'PROPIEDADES DE COLUMNA AUDITADO
            gridDocumentacion.Columns("bauditado").Visible = True
            gridDocumentacion.Columns("bauditado").Caption = "AUDITADO"
            gridDocumentacion.Columns("bauditado").CellStyle.HorizontalAlign = HorizontalAlign.Center
            gridDocumentacion.Columns("bauditado").Width = 73%
            TryCast(gridDocumentacion.Columns("bauditado"), GridViewDataTextColumn).DataItemTemplate = New CheckBoxAuditado(gridDocumentacion)

            gridDocumentacion.Visible = True
        Catch ex As Exception
            gridDocumentacion.Visible = False
            ShowPopUpMsg("NO POSEE UNIDADES ASIGNADAS")
        End Try


    End Sub

    'CREA EL TEMPLATE DE HYPERLINK
    Friend Class ImagenesArchivosGrid
        Implements ITemplate
        Dim gridantigua As ASPxGridView
        Dim dictTipoyVencimiento As Dictionary(Of String, String)
        Public Sub New(ByVal grid As ASPxGridView, ByVal DicDoctoYVencimiento As Dictionary(Of String, String))
            gridantigua = grid
            dictTipoyVencimiento = DicDoctoYVencimiento
        End Sub

        Public Sub InstantiateIn(ByVal container As UI.Control) Implements ITemplate.InstantiateIn
            Dim pagina As New pag_auditoria()
            Dim gridContainer As GridViewDataItemTemplateContainer = CType(container, GridViewDataItemTemplateContainer)
            Dim link As New ASPxHyperLink()
            link.ID = "ASPxHyperLink1"
            link.Text = "texto"
            'AÑADO EL EVENTO Y UTILIZO LAMBDA PARA PASAR PARÁMETROS
            'SE DEBE CREAR UNA FUNCION QUE DEVUELVA ALGUN VALOR PARA LAMBDA OBLIGATORIAMENTE!!!
            'EN ESTE CASO, SE DEVUELVE TRUE
            AddHandler link.Init, Function(sender, e) pagina.ASPxHyperLink1_Init(sender, gridantigua, dictTipoyVencimiento)
            'link.Value = gridantigua.GetRowValues(gridContainer.ItemIndex, gridantigua.Columns.F)
            container.Controls.Add(link)
        End Sub
    End Class

    'CREA EL TEMPLATE DE IMAGEN
    Friend Class ImagenesEstado
        Implements ITemplate
        Dim gridantigua As ASPxGridView
        Public Sub New(ByVal grid As ASPxGridView)
            gridantigua = grid
        End Sub

        Public Sub InstantiateIn(ByVal container As UI.Control) Implements ITemplate.InstantiateIn
            Dim pagina As New pag_auditoria()
            Dim gridContainer As GridViewDataItemTemplateContainer = CType(container, GridViewDataItemTemplateContainer)
            Dim img As New ASPxImage()
            img.ID = "ASPxImage1"
            'link.Value = "Sin Imagen"
            'AÑADO EL EVENTO Y UTILIZO LAMBDA PARA PASAR PARÁMETROS
            'SE DEBE CREAR UNA FUNCION QUE DEVUELVA ALGUN VALOR PARA LAMBDA OBLIGATORIAMENTE!!!
            'EN ESTE CASO, SE DEVUELVE TRUE
            AddHandler img.Init, Function(sender, e) pagina.imgEstado_A_Load(sender, gridantigua)
            'link.Value = gridantigua.GetRowValues(gridContainer.ItemIndex, gridantigua.Columns.F)
            container.Controls.Add(img)
        End Sub
    End Class

    'CREA EL TEMPLATE DE CHECKBOX
    Friend Class CheckBoxAuditado
        Implements ITemplate
        Dim gridantigua As ASPxGridView
        Public Sub New(ByVal grid As ASPxGridView)
            gridantigua = grid
        End Sub

        Public Sub InstantiateIn(ByVal container As UI.Control) Implements ITemplate.InstantiateIn
            Dim pagina As New pag_auditoria()
            Dim gridContainer As GridViewDataItemTemplateContainer = CType(container, GridViewDataItemTemplateContainer)
            Dim chkbox As New ASPxCheckBox()
            chkbox.ID = "chkAuditado"
            chkbox.Theme = "MetropolisBlue"
            chkbox.ClientInstanceName = "chkAuditado"
            chkbox.ClientSideEvents.CheckedChanged = "ClickAuditado"
            'link.Value = "Sin Imagen"
            'AÑADO EL EVENTO Y UTILIZO LAMBDA PARA PASAR PARÁMETROS
            'SE DEBE CREAR UNA FUNCION QUE DEVUELVA ALGUN VALOR PARA LAMBDA OBLIGATORIAMENTE!!!
            'EN ESTE CASO, SE DEVUELVE TRUE
            AddHandler chkbox.Init, Function(sender, e) pagina.chkAuditado_Init(sender, gridantigua)
            'link.Value = gridantigua.GetRowValues(gridContainer.ItemIndex, gridantigua.Columns.F)
            container.Controls.Add(chkbox)
        End Sub
    End Class
End Class


