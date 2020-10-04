Imports System.Data
Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports DevExpress.Web
Imports System.Globalization


Module funciones

    'Public gIPServer As String = "DESKTOP-ON8N7S7"
    'Public gDatabase As String = "EST2016"
    ''Public gDatabase As String = "EST-DESA"
    'Public gUserBD As String = "sa"
    'Public gUserPass As String = "admin"
    'Public gDirFiles As String = "~/Archivos/"
    ''Public gDirFiles As String = "c:/sites/gestionEST/Archivos/"
    'Public gExtFiles As String = " png jpg pdf xls xlsx"

    Public gIPServer As String = "OMEGA"
    Public gDatabase As String = "EST2016"
    'Public gDatabase As String = "EST-DESA"
    Public gUserBD As String = "sa"
    Public gUserPass As String = "soporte123"
    Public gDirFiles As String = "~/Archivos/"
    'Public gDirFiles As String = "c:/sites/gestionEST/Archivos/"
    Public gExtFiles As String = " png jpg pdf xls xlsx"

    Public sqlStr As String
    Public conn As SqlConnection
    Public data As DataTable
    Public da As SqlDataAdapter
    Public cb As SqlCommandBuilder
    Public cmd As SqlCommand
    Public re As SqlDataReader
    Public pModoPopup As String = ""
    Public pModoPopupSolicitud As String = ""
    Public pModoPopupDetalleSolicitud As String = ""
    Public sqlSource As String = ""

    Function ConectaSQLServer() As Boolean

        Dim StrConn As String

        If Not conn Is Nothing Then
            'conn.Close()
        End If

        StrConn = String.Format("Server={0};Database={1};User Id={2};Password={3};", gIPServer, gDatabase, gUserBD, gUserPass)

        Try
            conn = New SqlConnection(StrConn)
            conn.Open()
            Return True

        Catch ex As SqlException
            ''MsgBox(ex.ToString)
            Return False
        End Try

    End Function
    Function ConectaSQLServerConn(ByRef myConn As SqlConnection) As Boolean

        Dim StrConn As String

        If Not myConn Is Nothing Then
            'myConn.Close()
        End If

        StrConn = String.Format("Server={0};Database={1};User Id={2};Password={3};", gIPServer, gDatabase, gUserBD, gUserPass)

        Try
            myConn = New SqlConnection(StrConn)
            myConn.Open()
            Return True

        Catch ex As SqlException
            'MsgBox(ex.ToString)
            Return False
        End Try

    End Function


    Sub GrabaLog(pUsuario As String, pSector As String, pLog As String)
        If ConectaSQLServer() Then
            Using conn
                Try

                    pLog = pLog.Replace("'", "|")
                    sqlStr = String.Format("INSERT INTO ccLOG (idUsuario,sector, texto) SELECT '{0}','{1}','{2}'", pUsuario, pSector, pLog)

                    If ConectaSQLServer() Then
                        Using conn
                            Try
                                cmd = New SqlCommand(sqlStr, conn)
                                cmd.ExecuteNonQuery()
                            Catch ex As Exception
                                'MsgBox(ex.ToString)
                            End Try
                        End Using

                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                    Return
                End Try
            End Using
        End If



    End Sub


    Function fnNumeroSolicitudNueva() As Integer

        fnNumeroSolicitudNueva = -1

        sqlStr = "SELECT max(id) as idactual FROM ccSolicitud "

        If ConectaSQLServer() Then
            Using conn
                Try

                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Dim rdr As SqlDataReader = cmd.ExecuteReader()

                    If rdr.HasRows Then
                        While rdr.Read
                            fnNumeroSolicitudNueva = rdr("idactual")
                            fnNumeroSolicitudNueva = fnNumeroSolicitudNueva + 1
                        End While
                    End If

                    rdr.Close()


                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using


        End If

    End Function

    Function strToDate(ByVal pStr As String) As Date


        Return DateTime.ParseExact(pStr, "dd-MM-yyyy", CultureInfo.InvariantCulture)

    End Function

    Function fnCargaTodasUnidades() As String

        fnCargaTodasUnidades = ""

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT ccUnidades.id FROM ccUnidades INNER JOIN ccEmpUsuaria ON ccUnidades.idEmpUsuaria=ccEmpUsuaria.id "

                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Dim rdr As SqlDataReader = cmd.ExecuteReader()

                    If rdr.HasRows Then
                        While rdr.Read
                            If fnCargaTodasUnidades = "" Then
                                fnCargaTodasUnidades = fnCargaTodasUnidades + "'" + rdr("id") + "'"
                            Else
                                fnCargaTodasUnidades = fnCargaTodasUnidades + ",'" + rdr("id") + "'"
                            End If

                        End While
                    End If

                    rdr.Close()


                Catch ex As Exception
                    'MsgBox(ex.ToString)

                End Try
            End Using


        End If

    End Function


    Function fnCargaUnidades(ByRef pUsuaria As String) As String

        fnCargaUnidades = ""

        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = "SELECT ccUnidades.id FROM ccUnidades INNER JOIN ccEmpUsuaria ON ccUnidades.idEmpUsuaria=ccEmpUsuaria.id WHERE ccEmpUsuaria.id ='" & pUsuaria & "'"

                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Dim rdr As SqlDataReader = cmd.ExecuteReader()

                    If rdr.HasRows Then
                        While rdr.Read
                            If fnCargaUnidades = "" Then
                                fnCargaUnidades = fnCargaUnidades + "'" + rdr("id") + "'"
                            Else
                                fnCargaUnidades = fnCargaUnidades + ",'" + rdr("id") + "'"
                            End If

                        End While
                    End If

                    rdr.Close()


                Catch ex As Exception
                    'MsgBox(ex.ToString)

                End Try
            End Using


        End If

    End Function
    Sub spCargarListView(ByRef ListView As ListView, ByVal sql As String, ByVal db As String)
        If ConectaSQLServer() Then
            Using conn
                Try
                    '

                    ' propiedades del SqlCommand  
                    Dim comando As New SqlCommand

                    With comando
                        .CommandType = CommandType.Text
                        .CommandText = sql

                        .Connection = conn
                    End With

                    Dim da As New SqlDataAdapter ' Crear nuevo SqlDataAdapter  
                    Dim dataset As New DataSet ' Crear nuevo dataset  

                    da.SelectCommand = comando

                    ' llenar el dataset  
                    da.Fill(dataset, "Tabla")

                    ' Propiedades del ListView  
                    With ListView
                        '       .Items.Clear()
                        '      .c.Columns.Clear()
                        '     .View = View.Details
                        '    .GridLines = True
                        '   .FullRowSelect = True
                        ' añadir los nombres de columnas  
                        For c As Integer = 0 To dataset.Tables("tabla").Columns.Count - 1
                            '      .Columns.Add(dataset.Tables("tabla").Columns(c).Caption)
                        Next
                    End With

                    ' Añadir los registros de la tabla  
                    ListView.Items.Clear()
                    With dataset.Tables("tabla")
                        For f As Integer = 0 To .Rows.Count - 1

                            Dim dato As New ListViewItem(.Rows(f).Item(0).ToString)
                            ' recorrer las columnas  
                            For c As Integer = 1 To .Columns.Count - 1
                                '         dato.SubItems.Add(.Rows(f).Item(c).ToString())
                            Next
                            ListView.Items.Add(dato)
                        Next
                    End With

                Catch ex As Exception
                    'MsgBox(ex.Message.ToString)
                End Try
            End Using
        End If

    End Sub

    Public Function fnFechaTexto(ByVal pFecha As String) As String

        Return (Mid(pFecha, 7, 4) & Mid(pFecha, 4, 2) & Mid(pFecha, 1, 2))

    End Function
    Public Function fnFormatoHora24(ByVal pHora As Date) As String

        Return pHora.ToString("HH:mm")

        '     Dim xHora As String = "00"
        '    Dim xMin As String = "00"

        'xHora = Trim(Mid(pHora, 1, InStr(":", pHora) - 1))
        '        xMin = Trim(Mid(pHora, InStr(":", pHora) + 1, 2))


        'xHora = pHora.ToString("HH:mm")


    End Function

    Sub spLlenaListBox(ByRef pSqlStr As String, ByVal pListBox As ASPxListBox, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    pListBox.Items.Clear()
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    pListBox.DataSource = ds
                    pListBox.ValueType = GetType(String)
                    pListBox.ValueField = pCampoDependiente.ToString
                    pListBox.TextField = pCampoInfo.ToString
                    pListBox.DataBind()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using

        End If

    End Sub

    Sub spLlenatxtIdUnidad(ByRef pSqlStr As String, ByVal ptxt As ASPxTextBox, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()

        If ConectaSQLServer() Then
            Using conn
                Try
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    ptxt.DataSource = ds
                    ptxt.Value = pCampoDependiente.ToString
                    ptxt.Text = pCampoInfo.ToString
                    ptxt.DataBind()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If

    End Sub

    Sub spLlenaComboBoxPopUp(ByRef pSqlStr As String, ByVal pComboBox As ASPxComboBox, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    pComboBox.Items.Clear()
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    pComboBox.DataSource = ds
                    pComboBox.ValueType = GetType(String)
                    pComboBox.ValueField = pCampoDependiente.ToString
                    pComboBox.TextField = pCampoInfo.ToString
                    pComboBox.DataBind()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If


    End Sub

    Sub spLlenaComboBoxPopUpTodosDatosBd(ByRef pSqlStr As String, ByVal pComboBox As ASPxComboBox, ByRef pListadoColumnas As List(Of String), ByRef pValorDeCampo As String, ByRef pColumnaTextoAMostrarEnCombo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    pComboBox.Items.Clear()
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")
                    Dim dato = ds.Tables(0).Columns(0)
                    Dim d = dato
                    pComboBox.DataSource = ds
                    pComboBox.ValueType = GetType(String)
                    pComboBox.ValueField = pValorDeCampo
                    For Each columna As String In pListadoColumnas
                        pComboBox.Columns.Add(columna)
                    Next
                    pComboBox.DataBind()
                    pComboBox.Columns(0).ClientVisible = False
                    pComboBox.Columns(2).ClientVisible = False
                    pComboBox.Columns(3).ClientVisible = False
                    pComboBox.TextFormatString = "{" + pColumnaTextoAMostrarEnCombo + "}"
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If
    End Sub

    Sub spLlenaComboBox(ByRef pSqlStr As String, ByVal pComboBox As ASPxComboBox, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    pComboBox.Items.Clear()
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    pComboBox.DataSource = ds
                    pComboBox.ValueType = GetType(String)
                    pComboBox.ValueField = pCampoDependiente.ToString
                    pComboBox.TextField = pCampoInfo.ToString
                    pComboBox.DataBind()
                    pComboBox.Items.Insert(Nothing, New ListEditItem("TODAS", Nothing))
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If

    End Sub

    Sub spLlenaCheckBoxList(ByRef pSqlStr As String, ByVal vl_diCheckBoxList As ASPxCheckBoxList, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    vl_diCheckBoxList.Items.Clear()
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    vl_diCheckBoxList.DataSource = ds
                    vl_diCheckBoxList.ValueType = GetType(String)
                    vl_diCheckBoxList.ValueField = pCampoDependiente.ToString
                    vl_diCheckBoxList.TextField = pCampoInfo.ToString
                    vl_diCheckBoxList.DataBind()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If

    End Sub

    Sub spLlenaComboBoxFecha(ByRef pSqlStr As String, ByVal pComboBox As ASPxComboBox, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    pComboBox.Items.Clear()
                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    pComboBox.DataSource = ds
                    pComboBox.ValueType = GetType(String)
                    pComboBox.ValueField = pCampoDependiente.ToString
                    pComboBox.TextField = pCampoInfo.ToString
                    pComboBox.DataBind()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If

    End Sub

    Sub spLlenaComboBoxReport(ByRef pSqlStr As String, ByVal pComboBox As Object, ByRef pCampoDependiente As String, ByRef pCampoInfo As String)
        Dim ds As New DataSet()
        If ConectaSQLServer() Then
            Using conn
                Try
                    pComboBox.Items.Clear()
                    pComboBox.appenddatabounditems = True
                    pComboBox.items.add(New ListItem("--TODOS--", "%"))

                    da = New SqlDataAdapter(pSqlStr, conn)
                    da.Fill(ds, "Datos")

                    pComboBox.DataSource = ds
                    pComboBox.DataValueField = pCampoDependiente.ToString
                    pComboBox.DataTextField = pCampoInfo.ToString
                    pComboBox.clearselection()
                    pComboBox.DataBind()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If

    End Sub

    Sub spllenaGridView(ByVal pGridView As ASPxGridView, ByRef pSQL As String)

        Dim connNueva As New SqlConnection()
        If ConectaSQLServerConn(connNueva) Then
            Using connNueva
                Try
                    Dim adapter As New SqlDataAdapter()
                    Dim ds As New DataSet()

                    Dim command As New SqlCommand(pSQL, connNueva)
                    adapter.SelectCommand = command
                    adapter.Fill(ds)
                    adapter.Dispose()
                    command.Dispose()
                    If ds.Tables(0).Rows.Count > 0 Then
                        pGridView.DataSource = ds.Tables(0)
                        pGridView.DataBind()
                    Else
                        pGridView.DataSource = Nothing
                        pGridView.DataBind()
                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If
    End Sub

    Sub spllenaGridViewAuditoria(ByVal pGridView As ASPxGridView, ByRef pSQL As String)
        Dim ds As New DataSet()
        Dim connNueva As New SqlConnection()
        If ConectaSQLServerConn(connNueva) Then
            Using connNueva
                Try
                    Dim adapter As New SqlDataAdapter()
                    Dim command As New SqlCommand(pSQL, connNueva)
                    adapter.SelectCommand = command
                    adapter.Fill(ds)
                    adapter.Dispose()
                    command.Dispose()
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using
        End If
        'DATOS DE COLUMNAS
        Try
            If ds.Tables(0).Rows.Count > 0 Then
                pGridView.DataSource = ds.Tables(0)
                pGridView.KeyFieldName = "rut"
                pGridView.DataBind()
                ''PROPIEDADES DE RUT
                'pGridView.Columns(0).Width = 10%


                ''AÑADO IMÁGENES Y CONFIGURACIONES A ESTADO
                'TryCast(pGridView.Columns("estado"), GridViewDataColumn).Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
                'TryCast(pGridView.Columns("estado"), GridViewDataColumn).Settings.FilterMode = ColumnFilterMode.DisplayText
                'TryCast(pGridView.Columns("estado"), GridViewDataColumn).Settings.AllowSort = DevExpress.Utils.DefaultBoolean.False
                'TryCast(pGridView.Columns("estado"), GridViewDataColumn).Settings.AllowGroup = DevExpress.Utils.DefaultBoolean.False
            Else
                pGridView.DataSource = Nothing
                pGridView.DataBind()
            End If
        Catch ex As Exception
            'MsgBox(ex.ToString())
        End Try
    End Sub

    Sub spllenaGridViewAsp(ByVal pGridView As GridView, ByRef pSQL As String)

        If ConectaSQLServer() Then
            Using conn
                Try
                    Dim adapter As New SqlDataAdapter()
                    Dim ds As New DataSet()

                    Dim command As New SqlCommand(pSQL, conn)
                    adapter.SelectCommand = command
                    adapter.Fill(ds)
                    adapter.Dispose()
                    command.Dispose()

                    If ds.Tables(0).Rows.Count > 0 Then
                        pGridView.DataSource = ds.Tables(0)
                        pGridView.DataBind()
                    Else
                        pGridView.DataSource = Nothing
                        pGridView.DataBind()
                    End If
                Catch ex As Exception
                    'MsgBox(ex.ToString)
                End Try
            End Using

        End If


    End Sub
    Public Function generarClaveSHA1(ByVal nombre As String) As String
        ' Crear una clave SHA1 como la generada por 
        ' FormsAuthentication.HashPasswordForStoringInConfigFile
        ' Adaptada del ejemplo de la ayuda en la descripción de SHA1 (Clase)
        Dim enc As New UTF8Encoding
        Dim data() As Byte = enc.GetBytes(nombre)
        Dim result() As Byte

        Dim sha As New SHA1CryptoServiceProvider
        ' This is one implementation of the abstract class SHA1.
        result = sha.ComputeHash(data)
        '
        ' Convertir los valores en hexadecimal
        ' cuando tiene una cifra hay que rellenarlo con cero
        ' para que siempre ocupen dos dígitos.
        Dim sb As New StringBuilder
        For i As Integer = 0 To result.Length - 1
            If result(i) < 16 Then
                sb.Append("0")
            End If
            sb.Append(result(i).ToString("x"))
        Next
        '
        Return sb.ToString.ToUpper
    End Function

    '    Private void GuardarArchivo(HttpPostedFile file) {
    '  // Se carga la ruta física de la carpeta temp del sitio
    '  String ruta = Server.MapPath("~/temp");

    '  // Si el directorio no existe, crearlo
    '  If (!Directory.Exists(ruta))
    '    Directory.CreateDirectory(ruta);

    '  String archivo = String.Format("{0}\\{1}", ruta, file.FileName);

    '  // Verificar que el archivo no exista
    '  If (File.Exists(archivo))
    '    MensajeError(String.Format(
    '      "Ya existe una imagen con nombre\"{0}\".", file.FileName));
    '  Else {
    '    file.SaveAs(archivo);
    '  }
    '}


    Public Sub spGuardarArchivo(file As HttpPostedFile, ruta As String)
        'Dim ruta As String = Server.MapPath("~/Archivos")
        Dim archivo As String
        archivo = String.Format("{0}\\{1}", ruta, file.FileName)
        file.SaveAs(archivo)

    End Sub


    Public Function fnIntToBoolean(ByVal pInt As Integer) As Boolean

        If Not IsNumeric(pInt) Then Return False

        If pInt = 1 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function verItemEvaluacionArauco(ByVal id) As Boolean
        Dim veItemEvaluacionArauco = False
        sqlStr = $"SELECT EmpUsuaria, esEvalua FROM ccUsuarioWeb WHERE usuario = '{id}'"
        Dim reader As SqlDataReader
        If ConectaSQLServer() Then
            Using conn
                Dim cmd As New SqlCommand(sqlStr, conn)
                reader = cmd.ExecuteReader
                While reader.Read()
                    'If reader.Item("empUsuaria") = "PANL" Then
                    'If reader.Item("esAdmin") = 1 Then
                    'veItemEvaluacionArauco = True
                    'End If
                    'End If
                    If reader.Item("esEvalua") = 1 Then
                        veItemEvaluacionArauco = True
                    End If
                End While
            End Using
        End If
        Return veItemEvaluacionArauco
    End Function

End Module
