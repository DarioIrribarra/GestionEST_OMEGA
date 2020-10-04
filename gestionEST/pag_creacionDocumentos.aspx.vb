Imports System.Data.SqlClient
Imports DevExpress.Web

Public Class pag_creacionDocumentos
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
    End Sub

    Protected Sub gridCrearDocumentos_Init(sender As Object, e As EventArgs)
        Call cargaGrid()
    End Sub

    Protected Sub gridCrearDocumentos_Load(sender As Object, e As EventArgs)
        '--------COLUMNAS BASE----------
        'PROPIEDADES DE COLUMNA CONTROLAVENCIMIENTO
        gridCrearDocumentos.Columns("CONTROLA VENCIMIENTO").Caption = "CONTROLA<BR/>VENCIMIENTO"
        gridCrearDocumentos.Columns("CONTROLA VENCIMIENTO").CellStyle.HorizontalAlign = HorizontalAlign.Center
        TryCast(gridCrearDocumentos.Columns("CONTROLA VENCIMIENTO"), GridViewDataColumn).DataItemTemplate = New ImagenesEstado(gridCrearDocumentos)
        'PROPIEDADES DE COLUMNA ACTIVO
        gridCrearDocumentos.Columns("ACTIVO").CellStyle.HorizontalAlign = HorizontalAlign.Center
        TryCast(gridCrearDocumentos.Columns("ACTIVO"), GridViewDataColumn).DataItemTemplate = New ImagenesEstado(gridCrearDocumentos)
        'PROPIEDADES DE COLUMNA NOMBRE EN MENU COLABORADORES
        gridCrearDocumentos.Columns("NOMBRE EN MENU COLABOLADORES").Caption = "NOMBRE EN MENU<BR/>COLABORADORES"
        'PROPIEDADES DE COLUMNA VISIBLE EN MENU COLABORADORES
        gridCrearDocumentos.Columns("VISIBLE EN MENU COLABORADORES").Caption = "VISIBLE EN MENU<BR/>COLABORADORES"
        gridCrearDocumentos.Columns("VISIBLE EN MENU COLABORADORES").CellStyle.HorizontalAlign = HorizontalAlign.Center
        TryCast(gridCrearDocumentos.Columns("VISIBLE EN MENU COLABORADORES"), GridViewDataColumn).DataItemTemplate = New ImagenesEstado(gridCrearDocumentos)
        'PROPIEDADES DE COLUMNA VISIBLE EN AUDITORIA
        gridCrearDocumentos.Columns("VISIBLE EN AUDITORÍA").Caption = "VISIBLE EN<BR/>AUDITORIA"
        gridCrearDocumentos.Columns("VISIBLE EN AUDITORÍA").CellStyle.HorizontalAlign = HorizontalAlign.Center
        TryCast(gridCrearDocumentos.Columns("VISIBLE EN AUDITORÍA"), GridViewDataColumn).DataItemTemplate = New ImagenesEstado(gridCrearDocumentos)
        'PROPIEDADES DE COLUMNA NOMBRE EN AUDITORIA
        gridCrearDocumentos.Columns("NOMBRE EN AUDITORÍA").Caption = "NOMBRE EN<BR/>AUDITORIA"
        'PROPIEDADES DE COLUMNA ELIMINAR
        If Session("SuperAdmin") = True Then
            gridCrearDocumentos.Columns("ELIMINAR").Visible = True
            gridCrearDocumentos.Columns("ELIMINAR").CellStyle.HorizontalAlign = HorizontalAlign.Center
            TryCast(gridCrearDocumentos.Columns("ELIMINAR"), GridViewDataColumn).DataItemTemplate = New BotonEliminar(gridCrearDocumentos, Me)
        Else
            gridCrearDocumentos.Columns("ELIMINAR").Visible = False
        End If
    End Sub

    Private Sub cargaGrid()
        sqlStr = "SELECT id as ID, UPPER(descripcion) as DESCRIPCION,
                    activo as ACTIVO,
                    controlaVencimiento as 'CONTROLA VENCIMIENTO',  
                    UPPER(nombreEnMenu) as 'NOMBRE EN MENU COLABOLADORES', 
                    activoEnMenu as 'VISIBLE EN MENU COLABORADORES',
                    UPPER(nombreEnAuditoria) as 'NOMBRE EN AUDITORÍA', 
                    activoEnAuditoria as 'VISIBLE EN AUDITORÍA',
                    id as ELIMINAR
                    FROM ccTipoDocto
                    ORDER BY activo DESC, ID ASC"
        Call spllenaGridView(gridCrearDocumentos, sqlStr)
    End Sub

    Protected Function imgEstado_A_Load(sender As Object, grid As ASPxGridView)
        'aca es el evento de la imagen dentro del itemtemplate

        Dim image As ASPxImage = sender
        Dim container As GridViewDataItemTemplateContainer = image.NamingContainer
        Dim myValue As String = grid.GetRowValues(container.ItemIndex, container.Column.FieldName)

        If myValue.Equals("1") Then
            image.Visible = True
            image.ImageUrl = "images/15.png"
        ElseIf myValue.Equals("0") Then
            image.Visible = True
            image.ImageUrl = "images/12.png"
        Else
            image.Visible = False
        End If
        Return True
    End Function

    Protected Function btnEliminar_Load(sender As Object, grid As ASPxGridView)
        Dim btn As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = btn.NamingContainer
        Dim myValue As String = grid.GetRowValues(container.ItemIndex, container.Column.FieldName)

        btn.CssClass = "btnGridSinFondo"
        btn.Image.ToolTip = "Eliminar"
        btn.Image.Url = "images/03.png"
        Return True
    End Function

    Public Sub btnEliminar_Click(sender As Object, grid As ASPxGridView)
        Dim btn As ASPxButton = sender
        Dim container As GridViewDataItemTemplateContainer = btn.NamingContainer
        Dim myValue As String = grid.GetRowValues(container.ItemIndex, container.Column.FieldName)
        'Me.Page = Page
        'ELIMINAR ARCHIVOS DE CCTIPODOCTO
        If EliminaCcTipoDocto(myValue) Then
            'ELIMINO LAS COLUMNAS DE AUDITORÍA
            If EliminaAuditoria(myValue) Then
                gridCrearDocumentos = grid
                Call cargaGrid()
                ShowPopUpMsg("COLUMNAS ELIMINADAS DE BASE DE DATOS")
            Else
                ShowPopUpMsg("ERROR AL ELIMINAR COLUMNAS EN AUDITORIA")
            End If
        Else
            ShowPopUpMsg("ERROR AL ELIMINAR DE COLUMNA CCTIPODOCTO")
        End If
        'LAMBDA TOMA LA GRID COMO NUEVA ASÍ QUE HAY QUE ASIGNARLE LA QUE SE LE PASA COMO PARÁMETRO
        'ES UNA FORMA DE VOLVER A ENLAZARLA

    End Sub

    Protected Sub cbpCreaDocumento_Callback(sender As Object, e As CallbackEventArgsBase)
        If e.Parameter = 0 Then
            txtId.ReadOnly = False
            popUpCreaDocumento.HeaderText = "Crear Nuevo Documento"
            Session("pModoPopupCreaDoc") = "C"
            Call LimpiarPopUp()
            txtId.BackColor = txtDescripcion.BackColor
        ElseIf e.Parameter = 1 Then
            txtId.ReadOnly = True
            Dim Id = gridCrearDocumentos.GetRowValues(gridCrearDocumentos.FocusedRowIndex, "ID")
            popUpCreaDocumento.HeaderText = "Modificar Documento"
            Session("pModoPopupCreaDoc") = "M"
            Call RellenarPopUP(Id)
            txtId.BackColor = System.Drawing.Color.Gray
        End If


    End Sub

    Protected Sub btnGrabarArchivo_Click(sender As Object, e As EventArgs)
        Dim id = ""
        Dim descripcion = ""
        Dim nombreEnAuditoria = ""
        Dim nombreEnMenuColaboradores = ""
        Dim controlaVencimiento = 0
        Dim activoEnMenu = 0
        Dim activo = 0
        Dim activoEnAuditoria = 0
        If txtId.Text.Trim.Length > 0 And
            txtDescripcion.Text.Trim.Length > 0 And
            txtNombreEnAuditoria.Text.Trim.Length > 0 And
            txtNombreEnMenuColaboradores.Text.Trim.Length > 0 Then

            id = txtId.Text.Trim.ToUpper()
            descripcion = txtDescripcion.Text.Trim.ToUpper()
            nombreEnAuditoria = txtNombreEnAuditoria.Text.Trim.ToUpper()
            nombreEnMenuColaboradores = txtNombreEnMenuColaboradores.Text.Trim.ToUpper()

            If cbVencimiento.Value Then
                controlaVencimiento = 1
            Else
                controlaVencimiento = 0
            End If

            If cbActivoEnMenuColaboradores.Value Then
                activoEnMenu = 1
            Else
                activoEnMenu = 0
            End If

            If cbActivo.Value Then
                activo = 1
            Else
                activo = 0
            End If

            If cbActivoEnAuditoria.Value Then
                activoEnAuditoria = 1
            Else
                activoEnAuditoria = 0

            End If

            If Session("pModoPopupCreaDoc") = "C" Then
                Dim valor = 0

                'BUSCO EL ID EN LA BD
                Try
                    If retornaId(id) Is Nothing Then
                        'INSERTO EL DOCUMENTO SOLO SI EL ID NO EXISTE
                        If insertarEnCcTipoDocto(id,
                                                 descripcion,
                                                 controlaVencimiento,
                                                 activo,
                                                 nombreEnMenuColaboradores,
                                                 activoEnMenu,
                                                 nombreEnAuditoria,
                                                 activoEnAuditoria) = 1 Then
                            'INSERT EN CCDOCEMPLEADO SOLO SI SE PUDO INSERTAR EN TIPODOCTO
                            Try
                                Call InsertaCcDocEmpleado(id)
                                Call LimpiarPopUp()
                                Call cargaGrid()
                                popUpCreaDocumento.ShowOnPageLoad = False
                                ShowPopUpMsg("DOCUMENTO CREADO EXITOSAMENTE")
                            Catch ex As Exception
                                ShowPopUpMsg("ERROR AL INSERTAR COLUMNAS EN AUDITORÍA")
                                Exit Sub
                            End Try

                        Else
                            ShowPopUpMsg("ERROR AL INSERTAR EN TIPO DOCUMENTO")
                            Exit Sub
                        End If
                    Else
                        ShowPopUpMsg("ERROR: ID YA EXISTENTE EN LA BASE DE DATOS")
                        Exit Sub
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("ERROR DE CONEXIÓN, INTENTE NUEVAMENTE")
                End Try

                'AL MODIFICAR SE ALTERAN LAS TABLAS
            Else
                Try
                    'VERIFICO QUE EL ID EXISTA
                    If (retornaId(id) Is Nothing) = False Then
                        Try
                            ModificarDatos(id,
                                        descripcion,
                                        controlaVencimiento,
                                        activo,
                                        nombreEnMenuColaboradores,
                                        activoEnMenu,
                                        nombreEnAuditoria,
                                        activoEnAuditoria)
                            Call LimpiarPopUp()
                            Call cargaGrid()
                            popUpCreaDocumento.ShowOnPageLoad = False
                            ShowPopUpMsg("DOCUMENTO MODIFICADO EXITOSAMENTE")
                        Catch ex As Exception
                            ShowPopUpMsg("ERROR AL MODIFICAR")
                            Exit Sub
                        End Try

                    Else
                        ShowPopUpMsg("ERROR ID NO EXISTE EN BASE DE DATOS")
                        Exit Sub
                    End If
                Catch ex As Exception
                    ShowPopUpMsg("ERROR DE CONEXIÓN, INTENTE NUEVAMENTE")
                End Try
            End If

        Else
            ShowPopUpMsg("ERROR: NO DEBEN EXISTIR CAMPOS DE TEXTO VACIOS")
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

    'ELIMINA CCTIPODOCTO
    Protected Function EliminaCcTipoDocto(ByVal id As String)
        Dim eliminado = False
        sqlStr = $"DELETE 
                FROM ccTipoDocto 
                WHERE iD = '{id}'"
        Try
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.ExecuteNonQuery()
                        eliminado = True
                    Catch ex As Exception
                        eliminado = False
                    End Try
                End Using
            End If
        Catch ex As Exception
            ShowPopUpMsg("ERROR: NO HAY CONEXIÓN A BASE DE DATOS")
            eliminado = False
        End Try
        Return eliminado
    End Function

    'ELIMINA DE AUDITORÍA
    Protected Function EliminaAuditoria(ByVal id As String)
        Dim eliminado = False
        'DROP CONSTRAINT
        sqlStr = $"ALTER TABLE ccDocEmpleado
                    DROP CONSTRAINT nuevo_doc_{id}, nuevo_doc_fechavencimiento_{id}"

        Try
            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.ExecuteNonQuery()
                End Using

            End If
        Catch ex As Exception
            ShowPopUpMsg("ERROR AL ELIMINAR CONSTRAINTS")
        End Try

        sqlStr = $"ALTER TABLE ccDocEmpleado
                    DROP COLUMN {id}, {id}_FCREACION, {id}_FVENCIMIENTO"
        Try
            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.ExecuteNonQuery()
                    eliminado = True
                End Using
            End If
        Catch ex As Exception
            eliminado = False
        End Try
        Return eliminado
    End Function

    'FUNCION QUE RETORNA ID
    Private Function retornaId(ByVal id As String)
        Dim existe = ""
        Dim valor = 0
        sqlStr = $"SELECT id FROM ccTipoDocto WHERE id = '{id}'"
        Try
            If ConectaSQLServer() Then
                Using conn
                    Try
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        existe = sqlcmd.ExecuteScalar
                    Catch ex As Exception

                    End Try
                End Using
            End If

        Catch ex As Exception
            ShowPopUpMsg("ERROR CONEXIÓN A BASE DE DATOS")
        End Try
        Return existe
    End Function

    'FUNCIÓN QUE INSERTA EN CCTIPODOCTO
    Private Function insertarEnCcTipoDocto(ByVal ID As String,
                                           ByVal descripcion As String,
                                           ByVal controlaVencimiento As Int16,
                                           ByVal activo As Int16,
                                           ByVal nombreEnMenuColaboradores As String,
                                           ByVal activoEnMenu As Int16,
                                           ByVal nombreEnAuditoria As String,
                                           ByVal activoEnAuditoria As Int16)
        Dim valor = 0
        Try
            'INSERTO EN CCTIPODOCTO
            sqlStr = $"INSERT INTO ccTipoDocto(id, descripcion, clasif, cvDigitalDoc, controlaVencimiento, activo, nombreEnMenu, activoEnMenu, nombreEnAuditoria, activoEnAuditoria)
                            VALUES('{ID}', '{descripcion}', 0, '', {controlaVencimiento}, {activo}, '{nombreEnMenuColaboradores}', {activoEnMenu}, '{nombreEnAuditoria}', {activoEnAuditoria})"

            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    valor = sqlcmd.ExecuteNonQuery()
                End Using
            End If
        Catch ex As Exception
            ShowPopUpMsg("ERROR AL INSERTAR NUEVO DOCUMENTO. INTENTE NUEVAMENTE")
        End Try
        Return valor
    End Function

    'MODIFICAR DATOS
    Private Sub ModificarDatos(ByVal ID As String,
                                           ByVal descripcion As String,
                                           ByVal controlaVencimiento As Int16,
                                           ByVal activo As Int16,
                                           ByVal nombreEnMenuColaboradores As String,
                                           ByVal activoEnMenu As Int16,
                                           ByVal nombreEnAuditoria As String,
                                           ByVal activoEnAuditoria As Int16)
        Dim valor = 0
        Try
            'INSERTO EN CCTIPODOCTO
            sqlStr = $"UPDATE ccTipoDocto
                                    SET descripcion = '{descripcion}',
                                        controlaVencimiento = {controlaVencimiento},
                                        activo = {activo},
                                        nombreEnMenu = '{nombreEnMenuColaboradores}',
                                        activoEnMenu = {activoEnMenu},
                                        nombreEnAuditoria = '{nombreEnAuditoria}',
                                        activoEnAuditoria = {activoEnAuditoria}
                                    WHERE id = '{ID}'"
            If ConectaSQLServer() Then
                Using conn
                    Dim sqlcmd As New SqlCommand(sqlStr, conn)
                    sqlcmd.ExecuteNonQuery()
                End Using
            End If
        Catch ex As Exception
            ShowPopUpMsg("ERROR AL MODIFICAR DOCUMENTO. INTENTE NUEVAMENTE")
        End Try
    End Sub

    'FUNCION QUE INSERTA EN CCDOCEMPLEADO
    Private Sub InsertaCcDocEmpleado(ByVal id As String)
        Dim existeColumna = ""
        If ConectaSQLServer() Then
            Using conn
                Try
                    sqlStr = $" IF COL_LENGTH('dbo.ccDocEmpleado', '{id}') IS NOT NULL 
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

        If existeColumna = "0" Then
            sqlStr = $" ALTER TABLE ccDocEmpleado
                                        ADD {id} VARCHAR(250) CONSTRAINT nuevo_doc_{id} DEFAULT '' NOT NULL, 
                                        {id}_FCREACION DATETIME,
                                        {id}_FVENCIMIENTO VARCHAR(10) CONSTRAINT nuevo_doc_fechavencimiento_{id} DEFAULT '' NOT NULL"
            Try
                If ConectaSQLServer() Then
                    Using conn
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        sqlcmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                ShowPopUpMsg("ERROR AL CREAR COLUMNAS EN AUDITORÍA")
            End Try
        End If

    End Sub

    'MÉTODO QUE RELLENA EL POPUP
    Private Sub RellenarPopUP(ByVal id As String)

        'vERIFICO QUE EXISTE EL ID
        If (retornaId(id) Is Nothing) = False Then
            sqlStr = $"SELECT 
                        descripcion, 
                        controlaVencimiento, 
                        activo, 
                        nombreEnMenu, 
                        activoEnMenu, 
                        nombreEnAuditoria, 
                        activoEnAuditoria
                    FROM ccTipoDocto
                    WHERE id = '{id}'"
            Try
                If ConectaSQLServer() Then
                    Using conn
                        Dim sqlcmd As New SqlCommand(sqlStr, conn)
                        Using reader = sqlcmd.ExecuteReader
                            While reader.Read()
                                'TEXTOS
                                txtId.Text = id
                                txtDescripcion.Text = reader("descripcion").ToString()
                                txtNombreEnAuditoria.Text = reader("nombreEnAuditoria").ToString()
                                txtNombreEnMenuColaboradores.Text = reader("nombreEnMenu").ToString()
                                'VALORES NUMÉRICOS
                                If reader("controlaVencimiento").ToString() = "1" Then
                                    cbVencimiento.Value = True
                                Else
                                    cbVencimiento.Value = Nothing
                                End If
                                If reader("activo").ToString() = "1" Then
                                    cbActivo.Value = True
                                Else
                                    cbActivo.Value = Nothing
                                End If
                                If reader("activoEnMenu").ToString() = "1" Then
                                    cbActivoEnMenuColaboradores.Value = True
                                Else
                                    cbActivoEnMenuColaboradores.Value = Nothing
                                End If
                                If reader("activoEnAuditoria").ToString() = "1" Then
                                    cbActivoEnAuditoria.Value = True
                                Else
                                    cbActivoEnAuditoria = Nothing
                                End If
                            End While
                        End Using
                    End Using
                End If
                popUpCreaDocumento.ShowOnPageLoad = True
            Catch ex As Exception
                ShowPopUpMsg("ERROR AL CARGAR DATOS. INTENTE NUEVAMENTE")
            End Try
        End If
    End Sub

    'MÉTODO QUE LIMPIA EL POPUP
    Private Sub LimpiarPopUp()
        txtId.Text = ""
        txtDescripcion.Text = ""
        txtNombreEnAuditoria.Text = ""
        txtNombreEnMenuColaboradores.Text = ""
        cbVencimiento.Value = Nothing
        cbActivoEnAuditoria.Value = Nothing
        cbActivoEnMenuColaboradores.Value = Nothing
        cbActivo.Value = Nothing
    End Sub

    'CREA EL TEMPLATE DE IMAGEN
    Friend Class ImagenesEstado
        Implements ITemplate
        Dim gridantigua As ASPxGridView
        Public Sub New(ByVal grid As ASPxGridView)
            gridantigua = grid
        End Sub

        Public Sub InstantiateIn(ByVal container As UI.Control) Implements ITemplate.InstantiateIn
            Dim pagina As New pag_creacionDocumentos()
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

    'CREA EL TEMPLATE DE BOTON
    Friend Class BotonEliminar
        Implements ITemplate
        Dim gridantigua As ASPxGridView
        Dim pagina As pag_creacionDocumentos
        Public Sub New(ByVal grid As ASPxGridView, ByVal page As Page)
            gridantigua = grid
            pagina = page
        End Sub

        Public Sub InstantiateIn(ByVal container As UI.Control) Implements ITemplate.InstantiateIn

            Dim gridContainer As GridViewDataItemTemplateContainer = CType(container, GridViewDataItemTemplateContainer)
            Dim btn As New ASPxButton()
            btn.ID = "btnEliminar"
            btn.ClientSideEvents.Click = "Mensaje"
            'link.Value = "Sin Imagen"
            'AÑADO EL EVENTO Y UTILIZO LAMBDA PARA PASAR PARÁMETROS
            'SE DEBE CREAR UNA FUNCION QUE DEVUELVA ALGUN VALOR PARA LAMBDA OBLIGATORIAMENTE!!!
            'EN ESTE CASO, SE DEVUELVE TRUE
            AddHandler btn.Init, Function(sender, e) pagina.btnEliminar_Load(sender, gridantigua)
            'AddHandler btn.Click, Sub(sender, e) pagina.btnEliminar_Click(sender, gridantigua, pagina)
            AddHandler btn.Click, Sub(sender, e) pagina.btnEliminar_Click(sender, gridantigua)
            'AddHandler btn.Click, AddressOf pagina.btnEliminar_Click
            container.Controls.Add(btn)
        End Sub
    End Class
End Class