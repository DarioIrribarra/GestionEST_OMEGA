Imports System.Data.SqlClient
Imports DevExpress.Export
Imports DevExpress.Web
Imports DevExpress.XtraPrinting

Public Class pag_EvaluacionArauco
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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

        'Actualiza datos de ccEvaluación la primera vez que se entra a la página
        If IsPostBack = False Then
            Dim sqlcmd As New SqlCommand
            Try
                If ConectaSQLServer() Then
                    Using conn
                        sqlcmd.Connection = conn
                        sqlcmd.CommandType = CommandType.StoredProcedure
                        sqlcmd.CommandText = "prActualizaccEvaluacion"
                        sqlcmd.ExecuteNonQuery()
                        sqlcmd.Dispose()
                    End Using
                End If
            Catch ex As Exception
                ShowPopUpMsg("No se puede actualizar Evaluación. Revise el procedimiento almacenado 'prActualizaccEvaluacion'")
            End Try

        End If

        CargaGrid()



    End Sub

    Protected Sub cbxUnidad_Load(sender As Object, e As EventArgs)
        If IsPostBack = False Then
            sqlStr = $"SELECT DISTINCT id as idUnidad, UPPER(dbo.LimpiarCaracteres(descripcion)) as descripcion 
                    FROM ccUnidades 
                    WHERE idEmpUsuaria = 'PANL'
                    AND id IN ({Session("pubUnidadesUsuario")})    
                    ORDER BY descripcion"
            Call spLlenaComboBoxPopUp(sqlStr, cbxUnidad, "idUnidad", "descripcion")
            cbxUnidad.Items.Insert(Nothing, New ListEditItem("SELECCIONE", Nothing))
        End If

    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs)
        'CargaGrid()
        sqlStr = $"SELECT * 
                        FROM Vw_ccEvaluacion
                        WHERE id='{cbxUnidad.Value}'
                        AND ames = dbo.fn_MesProceso()
                        ORDER BY ames DESC, nombre ASC"
        spllenaGridView(EvaluacionExcel, sqlStr)
        If EvaluacionExcel.GetRowValues(2, "tipoEvaluacion") = 0 Then

            EvaluacionExcel.DataColumns.Item("userEvalua").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota01").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota02").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota03").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota04").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC01").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC02").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC03").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC04").Visible = False
            EvaluacionExcel.DataColumns.Item("esRecomendado").Visible = False
            EvaluacionExcel.DataColumns.Item("Observacion").Visible = False
        End If

        If EvaluacionExcel.GetRowValues(2, "tipoEvaluacion") = 1 Then

            EvaluacionExcel.DataColumns.Item("userEvalua").Visible = True
            EvaluacionExcel.DataColumns.Item("Nota01").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota02").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota03").Visible = False
            EvaluacionExcel.DataColumns.Item("Nota04").Visible = False
            EvaluacionExcel.DataColumns.Item("Promedio").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC01").Visible = True
            EvaluacionExcel.DataColumns.Item("NotaC02").Visible = True
            EvaluacionExcel.DataColumns.Item("NotaC03").Visible = True
            EvaluacionExcel.DataColumns.Item("NotaC04").Visible = True
            EvaluacionExcel.DataColumns.Item("esRecomendado").Visible = True
            EvaluacionExcel.DataColumns.Item("Observacion").Visible = True
        End If

        If EvaluacionExcel.GetRowValues(2, "tipoEvaluacion") = 2 Then

            EvaluacionExcel.DataColumns.Item("userEvalua").Visible = True
            EvaluacionExcel.DataColumns.Item("Nota01").Visible = True
            EvaluacionExcel.DataColumns.Item("Nota02").Visible = True
            EvaluacionExcel.DataColumns.Item("Nota03").Visible = True
            EvaluacionExcel.DataColumns.Item("Nota04").Visible = True
            EvaluacionExcel.DataColumns.Item("Promedio").Visible = True
            EvaluacionExcel.DataColumns.Item("NotaC01").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC02").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC03").Visible = False
            EvaluacionExcel.DataColumns.Item("NotaC04").Visible = False
            EvaluacionExcel.DataColumns.Item("esRecomendado").Visible = True
            EvaluacionExcel.DataColumns.Item("Observacion").Visible = True
        End If

        EvaluacionExcel.DataColumns.Item("rut").Caption = "Rut"
        EvaluacionExcel.DataColumns.Item("nombre").Caption = "Nombre"
        EvaluacionExcel.DataColumns.Item("ames").Caption = "Fecha"
        EvaluacionExcel.DataColumns.Item("id").Caption = "Planta"
        EvaluacionExcel.DataColumns.Item("area").Caption = "Area"
        EvaluacionExcel.DataColumns.Item("cargo").Caption = "Cargo"
        EvaluacionExcel.DataColumns.Item("userEvalua").Caption = "Evaluador"
        EvaluacionExcel.DataColumns.Item("Nota01").Caption = "Procedimientos de seguridad"
        EvaluacionExcel.DataColumns.Item("Nota02").Caption = "Responsabilidad en tareas"
        EvaluacionExcel.DataColumns.Item("Nota03").Caption = "Realización de labores"
        EvaluacionExcel.DataColumns.Item("Nota04").Caption = "Relación respetuosa"
        EvaluacionExcel.DataColumns.Item("NotaC01").Caption = "Dominio técnico"
        EvaluacionExcel.DataColumns.Item("NotaC02").Caption = "Superación de obstáculos"
        EvaluacionExcel.DataColumns.Item("NotaC03").Caption = "Cumplimiento de reglas"
        EvaluacionExcel.DataColumns.Item("NotaC04").Caption = "Actitud respetuosa"
        EvaluacionExcel.DataColumns.Item("esRecomendado").Caption = "Se recomienda"
        EvaluacionExcel.DataColumns.Item("idArea").Visible = False
        EvaluacionExcel.DataColumns.Item("tipoEvaluacion").Visible = False
        gridExport.WriteXlsxToResponse(New XlsxExportOptionsEx() With {.ExportType = ExportType.WYSIWYG})
    End Sub

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)
        gridEvaluaciones.PageIndex = 0
        'Obtengo el valor del tipo de evaluación
        sqlStr = $" SELECT tipoEvalua
                        FROM ccUnidades 
                        WHERE id= '{cbxUnidad.Value}'"
        Dim reader As SqlDataReader
        If ConectaSQLServer() Then
            Using conn
                Dim cmd As New SqlCommand(sqlStr, conn)
                reader = cmd.ExecuteReader
                While reader.Read()
                    If reader.Item("tipoEvalua") = 0 Then
                        'Si el valor es 0 no mostrar ningún campo a evaluar
                        gridEvaluaciones.Columns.Item("userEvalua").Visible = False
                        gridEvaluaciones.Columns.Item("Nota01").Visible = False
                        gridEvaluaciones.Columns.Item("Nota02").Visible = False
                        gridEvaluaciones.Columns.Item("Nota03").Visible = False
                        gridEvaluaciones.Columns.Item("Nota04").Visible = False
                        'gridEvaluaciones.Columns.Item("PromedioUnbound").Visible = False
                        gridEvaluaciones.Columns.Item("Promedio").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC01").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC02").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC03").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC04").Visible = False
                        gridEvaluaciones.Columns.Item("esRecomendado").Visible = False
                        gridEvaluaciones.Columns.Item("Observacion").Visible = False
                        gridEvaluaciones.Columns.Item("btnGuardar").Visible = False
                    End If
                    '
                    If reader.Item("tipoEvalua") = 1 Then
                        ' Ocultar columnas de número
                        gridEvaluaciones.Columns.Item("userEvalua").Visible = True
                        gridEvaluaciones.Columns.Item("Nota01").Visible = False
                        gridEvaluaciones.Columns.Item("Nota02").Visible = False
                        gridEvaluaciones.Columns.Item("Nota03").Visible = False
                        gridEvaluaciones.Columns.Item("Nota04").Visible = False
                        'gridEvaluaciones.Columns.Item("PromedioUnbound").Visible = False
                        gridEvaluaciones.Columns.Item("Promedio").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC01").Visible = True
                        gridEvaluaciones.Columns.Item("NotaC02").Visible = True
                        gridEvaluaciones.Columns.Item("NotaC03").Visible = True
                        gridEvaluaciones.Columns.Item("NotaC04").Visible = True
                    End If
                    If reader.Item("tipoEvalua") = 2 Then
                        'Mostrar columnas de campo de texto
                        gridEvaluaciones.Columns.Item("userEvalua").Visible = True
                        gridEvaluaciones.Columns.Item("NotaC01").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC02").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC03").Visible = False
                        gridEvaluaciones.Columns.Item("NotaC04").Visible = False
                        gridEvaluaciones.Columns.Item("Nota01").Visible = True
                        gridEvaluaciones.Columns.Item("Nota02").Visible = True
                        gridEvaluaciones.Columns.Item("Nota03").Visible = True
                        gridEvaluaciones.Columns.Item("Nota04").Visible = True
                        'gridEvaluaciones.Columns.Item("PromedioUnbound").Visible = True
                        gridEvaluaciones.Columns.Item("Promedio").Visible = True
                    End If
                End While
            End Using
        End If
        gridEvaluaciones.Visible = True
        If gridEvaluaciones.VisibleRowCount > 0 Then
            tr_excel.Attributes("style") = "visibility: visible;"
        End If
    End Sub


    Protected Sub cbxEvaluacion_Init(sender As Object, e As EventArgs)
        Dim evaluacion As ASPxComboBox = sender
        Dim container As GridViewDataItemTemplateContainer = evaluacion.NamingContainer

        For index = 0 To 7
            evaluacion.Items.Insert(index, New ListEditItem(index, index))
        Next
        evaluacion.Items.RemoveAt(0)
    End Sub

    Protected Sub cbxEvaluacion_Load(sender As Object, e As EventArgs)
        Dim evaluacion As ASPxComboBox = sender
        Dim container As GridViewDataItemTemplateContainer = evaluacion.NamingContainer
        Dim myValue As String = ""
        myValue = gridEvaluaciones.GetRowValues(container.ItemIndex, container.Column.FieldName)
        evaluacion.Value = myValue
    End Sub

    Protected Sub cbxEvaluacionConceptual_Init(sender As Object, e As EventArgs)
        sqlStr = "SELECT descEvaluacion FROM ccConceptoEvaluacion"
        Dim evaluacion As ASPxComboBox = sender
        Dim container As GridViewDataItemTemplateContainer = evaluacion.NamingContainer
        If ConectaSQLServer() Then
            Using conn
                spLlenaComboBoxPopUp(sqlStr, evaluacion, "descEvaluacion", "descEvaluacion")
            End Using
        End If
    End Sub

    Protected Sub cbxEvaluacionConceptual_Load(sender As Object, e As EventArgs)
        Dim evaluacion As ASPxComboBox = sender
        Dim container As GridViewDataItemTemplateContainer = evaluacion.NamingContainer
        Dim myValue As String = ""
        evaluacion.Value = gridEvaluaciones.GetRowValues(container.ItemIndex, container.Column.FieldName)

    End Sub

    Protected Sub cbxRecomendado_Init(sender As Object, e As EventArgs)
        Dim combobox As ASPxComboBox = sender
        Dim container As GridViewDataItemTemplateContainer = combobox.NamingContainer
        combobox.Items.Insert(0, New ListEditItem("SI", "SI"))
        combobox.Items.Insert(1, New ListEditItem("NO", "NO"))
    End Sub

    Protected Sub cbxRecomendado_Load(sender As Object, e As EventArgs)
        Dim combobox As ASPxComboBox = sender
        Dim container As GridViewDataItemTemplateContainer = combobox.NamingContainer
        combobox.Value = gridEvaluaciones.GetRowValues(container.ItemIndex, container.Column.FieldName)
    End Sub

    Protected Sub txtObservacion_Load(sender As Object, e As EventArgs)
        Dim evaluacion As ASPxMemo = sender
        Dim container As GridViewDataItemTemplateContainer = evaluacion.NamingContainer
        Dim myValue As String = gridEvaluaciones.GetRowValues(container.ItemIndex, container.Column.FieldName)
        evaluacion.Value = gridEvaluaciones.GetRowValues(container.ItemIndex, container.Column.FieldName)
    End Sub

    Sub CargaGrid()
        If cbxUnidad.SelectedIndex > 0 Then
            sqlStr = $" SELECT * 
                        FROM Vw_ccEvaluacion
                        WHERE id='{cbxUnidad.Value}'
                        AND ames = dbo.fn_MesProceso()
                        ORDER BY ames DESC, nombre ASC"
            If ConectaSQLServer() Then
                Using conn
                    spllenaGridView(gridEvaluaciones, sqlStr)
                    If gridEvaluaciones.VisibleRowCount > 0 Then
                        gridEvaluaciones.Visible = True
                    End If
                End Using

            End If
        Else
            gridEvaluaciones.Visible = False
        End If

    End Sub


    Protected Sub cbpGrid_Callback(sender As Object, e As CallbackEventArgsBase)

        'OBTENGO DATOS DE PARAMETRO Y OBJETO
        Dim valor = sender
        Dim valores = e.Parameter.Split("~")

        'DIVIDO EL ARREGLO QUE VIENE POR CLIENTE Y OBTENGO VALORES
        Dim Identificador = valores(0)
        Dim Nota1 = valores(1)
        Dim Nota2 = valores(2)
        Dim Nota3 = valores(3)
        Dim Nota4 = valores(4)
        Dim esRecomendado = valores(5)
        Dim Observacion = valores(6)
        Dim Index = Integer.Parse(valores(7))

        'OBTENGO DATOS DE TABLA PARA INSERCIÓN
        Dim rut = gridEvaluaciones.GetRowValues(Index, "rut")
        Dim idUnidad = gridEvaluaciones.GetRowValues(Index, "id")
        Dim idArea = gridEvaluaciones.GetRowValues(Index, "idArea")
        Dim ames = gridEvaluaciones.GetRowValues(Index, "ames")
        Dim evaluador = Session("pubIdUsuario")

        'VERIFICO EL PRIMER DATO PARA INSERTAR VALORES CORRECTOS
        If Identificador = "Numeros" Then
            sqlStr = $" UPDATE ccEvaluacion 
                        SET NOTA01 = {Nota1}, 
                        userEvalua = '{evaluador}',
                        NOTA02 = {Nota2},
                        NOTA03 = {Nota3}, 
                        NOTA04 = {Nota4}, 
                        esRecomendado = '{esRecomendado}', 
                        Observacion = '{Observacion}'
                        WHERE rut = '{rut}' 
                        AND area = '{idArea}' 
                        AND unidad = '{idUnidad}' 
                        AND ames = '{ames}'"
        Else
            sqlStr = $" UPDATE ccEvaluacion 
                        SET NOTAC01 = '{Nota1}',
                        userEvalua = '{evaluador}',
                        NOTAC02 = '{Nota2}',
                        NOTAC03 = '{Nota3}', 
                        NOTAC04 = '{Nota4}', 
                        esRecomendado = '{esRecomendado}', 
                        Observacion = '{Observacion}'
                        WHERE rut = '{rut}' 
                        AND area = '{idArea}' 
                        AND unidad = '{idUnidad}' 
                        AND ames = '{ames}'"
        End If

        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.ExecuteNonQuery()
                Catch ex As Exception

                End Try
            End Using
        End If

        CargaGrid()

    End Sub

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

End Class