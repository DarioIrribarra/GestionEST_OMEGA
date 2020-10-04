Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports DevExpress.Web.Office
Imports DevExpress.XtraRichEdit
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Linq
Imports DevExpress.Web

Public Class pag_contrato
    Inherits System.Web.UI.Page
    Protected Sub pag_contrato(sender As Object, e As EventArgs) Handles Me.Init

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

        'PODER VER ÍTEM DE EVALUACIONES
        If Session("pubIdUsuario") <> Nothing Then
            If verItemEvaluacionArauco(Session("pubIdUsuario")) = False Then
                ASPxMenu1.Items.FindByName("evaluacionArauco").Visible = False
            End If
        End If

        'ACCESO DE PERFILES A MENU
        If Session("SuperAdmin") = True Then
            ASPxMenu1.Items.FindByName("auditoria").Visible = True
        End If

        If Session("Administrador") = True Then
            ASPxMenu1.Items.FindByName("auditoria").Visible = False
        End If

        If Session("Web") = True Or Session("Operaciones") = True Then
            ASPxMenu1.Items.FindByName("contrataciones").Visible = False
        End If

        If Session("Cliente") = True Then
            ASPxMenu1.Items.FindByName("contrataciones").Visible = False
            ASPxMenu1.Items.FindByName("crearDocs").Visible = False
            ASPxMenu1.Items.FindByName("auditoria").Visible = False
            ASPxMenu1.Items.FindByName("descarga").Visible = False
        End If
    End Sub

    Private Const documentId As String = "ContratoEST"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Page.IsPostBack() Then
            If Request("__EVENTTARGET") <> Nothing And Request("__EVENTTARGET").ToString() = "DescargarPDF" Then
                DescargarPDF()
            End If

        End If

        Dim connExtra As New SqlConnection

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
                        MsgBox("Error en Reporte 3 - Drop Table " + ex.ToString)
                        Return
                    End Try
                End Using
            End If
        End If


        'IMAGEN
        Dim datosimagen As String = ""
        If ConectaSQLServer() Then
            If conn.State <> ConnectionState.Open Then
                Using connExtra
                    Try
                        sqlStr = "SELECT imagen 
                            FROM ccUsuarioWeb 
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"
                        cmd = New SqlCommand(sqlStr, connExtra)
                        datosimagen = cmd.ExecuteScalar()
                    Catch ex As Exception
                        MsgBox("Error imgn: " + ex.ToString)
                    End Try
                End Using
            Else
                Using conn
                    Try
                        sqlStr = "SELECT imagen 
                            FROM ccUsuarioWeb 
                            WHERE usuario = '" & Session("pubIdUsuario") & "'"
                        cmd = New SqlCommand(sqlStr, conn)
                        datosimagen = cmd.ExecuteScalar()
                    Catch ex As Exception
                        MsgBox("Error imgn: " + ex.ToString)
                    End Try
                End Using
            End If
        End If

        If datosimagen = "" Then
            Image1.ImageUrl = "images/10.png"
            Image1.Height = 50
            Image1.Width = 80
        Else
            Image1.ImageUrl = datosimagen
            Image1.Height = 50
            Image1.Width = 80
        End If

        form1.DefaultButton = btnDefault.UniqueID
        txtUsuario.InnerText = Session("pubNombreUsuario")
        lblNombreEmpresa.InnerText = Session("pubEmpUsuariaUsuario")




        Dim pSqlStr As String = ""
        If (Not IsCallback) Then

            Dim codContrato As Integer = 0
            Dim sqlCmd As SqlCommand
            Dim documentServer As New RichEditDocumentServer()

#Region "COPIAR"
            Dim connInicial As New SqlConnection
            If ConectaSQLServerConn(connInicial) Then
                Using connInicial
                    Try
                        sqlStr = String.Format("SELECT horario from remples WHERE REMPLES.ESTADO='A' AND REMPLES.rut='{0}' AND REMPLES.fecha_ing IN (SELECT MAX(fecha_ing) FROM REMPLES WHERE ESTADO='A' AND RUT='{0}')", Session("pubRutContrato"))
                        sqlCmd = New SqlCommand(sqlStr, connInicial)

                        codContrato = Convert.ToInt32(sqlCmd.ExecuteScalar())


                        If Session("pubEmpUsuariaUsuario") = "MASO" Then
                            If codContrato <> 22 Then
                                Dim pasaporte As String = ""
                                Dim conn2 As New SqlConnection
                                sqlStr = String.Format("Select dbo.trim(apc) from REMPLES WHERE ESTADO='A' AND RUT='{0}'", Session("pubRutContrato"))
                                Try
                                    If ConectaSQLServerConn(conn2) Then
                                        Using conn2
                                            sqlCmd = New SqlCommand(sqlStr, conn2)
                                            pasaporte = (sqlCmd.ExecuteScalar())
                                        End Using
                                    End If
                                Catch ex As Exception
                                    MsgBox("Error contrato 1.1: " + ex.ToString)
                                End Try


                                If Not IsNumeric(pasaporte) Then
                                    documentServer.LoadDocument(MapPath("/documentos/FormContrato-MASO.docx"))
                                Else
                                    documentServer.LoadDocument(MapPath("/documentos/FormContrato-MASO-PASA.docx"))
                                End If

                            Else
                                documentServer.LoadDocument(MapPath("/documentos/FormContratoArt22-MASO.docx"))
                            End If
                        End If

                        If Session("pubEmpUsuariaUsuario") = "PANL" Then
                            If codContrato <> 22 Then
                                documentServer.LoadDocument(MapPath("/documentos/ARAU-Contrato.docx"))
                            Else
                                documentServer.LoadDocument(MapPath("/documentos/ARAU-Contrato-22.docx"))
                            End If
                        End If

                        If Session("pubEmpUsuariaUsuario") = "RMDK" Then

                            Dim ubicaRMDH As Integer = 0
                            Try
                                sqlStr = String.Format("Select unidad from REMPLES WHERE ESTADO='A' AND RUT='{0}'", Session("pubRutContrato"))
                                Dim conn3 As New SqlConnection
                                If ConectaSQLServerConn(conn3) Then
                                    Using conn3
                                        sqlCmd = New SqlCommand(sqlStr, conn3)
                                        ubicaRMDH = (sqlCmd.ExecuteScalar())
                                    End Using
                                End If

                            Catch ex As Exception
                                MsgBox("Error contrato 1.2: " + ex.ToString)
                            End Try

                            Dim pasaporte As String = ""
                            Try
                                sqlStr = String.Format("Select dbo.trim(apc) from REMPLES WHERE ESTADO='A' AND RUT='{0}'", Session("pubRutContrato"))
                                Dim conn4 As New SqlConnection
                                If ConectaSQLServerConn(conn4) Then
                                    sqlCmd = New SqlCommand(sqlStr, conn4)
                                    pasaporte = (sqlCmd.ExecuteScalar())
                                End If

                            Catch ex As Exception
                                MsgBox("Error contrato 1.3: " + ex.ToString)
                            End Try

                            If ubicaRMDH = 6005 And Not IsNumeric(pasaporte) Then
                                documentServer.LoadDocument(MapPath("/documentos/RMDK-Contrato-STGO.docx"))
                            Else
                                documentServer.LoadDocument(MapPath("/documentos/RMDK-Contrato-STGO-EXTRANJERO.docx"))
                            End If

                            If ubicaRMDH = 6003 Then
                                documentServer.LoadDocument(MapPath("/documentos/RMDK-Contrato-TALCA.docx"))
                            End If

                            If ubicaRMDH = 6004 Then
                                documentServer.LoadDocument(MapPath("/documentos/RMDK-Contrato-COPIAPO.docx"))
                            End If

                        End If

                        If Session("pubEmpUsuariaUsuario") = "OXIQ" Then
                            documentServer.LoadDocument(MapPath("/documentos/OXIQUIM-Contrato.docx"))
                        End If

                        If Session("pubEmpUsuariaUsuario") = "BALL" Then
                            documentServer.LoadDocument(MapPath("/documentos/FormContrato-BALL.docx"))
                        End If

                    Catch ex As Exception
                        MsgBox("Error en contrato: " + ex.ToString)
                        Return
                    End Try
                End Using

            End If

#End Region

            pSqlStr = String.Format("SELECT

                            REMPLES.Codigo, dbo.TRIM(REMPLES.nombre) As nombreEmpleado,  
                            dbo.trim(REMPLES.apc) as pasaporte,
                            dbo.fn_RutconPuntos(REMPLES.rut) As rutEmpleado,                                
                            dbo.fn_fechaPalabras(REMPLES.fecha_ing) As fechaIngreso,                                
                            ccREMPLEC03.detalleCausal as detalleCausal, dbo.TRIM(UPPER(Est_civil)) As estadoCivil,                                
                            dbo.fn_fechaPalabras(fecha_nac) As fechaNacimiento,                                 
                            (SELECT dbo.TRIM(UPPER(Descrip)) FROM RTABLAS WHERE COTAB=16 And RTABLAS.codigo=REMPLES.nacion) as nacionalidad,                                
                            dbo.fn_direccionCompleta(remples.direccion) As dirTrabajador,                                
                            (SELECT UPPER(causalContrato) FROM cpCausasLegales WHERE cpCausasLegales.codigo=REMPLES.CLASIF) as causaLegal,                                
                            dbo.TRIM(Celular) As numeroTelefonico,
                            (SELECT descripcionContrato FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as nombrePlanta,                                
                            (SELECT lugarContrato FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as ubicaPlanta,                                
                            (SELECT ciudadContrato FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as ciudadPlanta,                                
                            (SELECT detalleJornada FROM ccHorario WHERE ccHorario.codigo=REMPLES.horario) as jornadaTrab,                                
                            (SELECT dbo.TRIM(UPPER(Descrip)) FROM RTABLAS WHERE COTAB=3 And RTABLAS.codigo=REMPLES.cargo) as cargoTrab,                                
                            Replace(Replace(Convert(VARCHAR, Convert(money, rmapitm.Monto), 1),'.00',''),',','.') as SUBASE,

                            Replace(Replace(Convert(VARCHAR, Convert(money, tmpCOLACI.Monto), 1),'.00',''),',','.') as COLACI,
                            Replace(Replace(Convert(VARCHAR, Convert(money, tmpMOVILI.Monto), 1),'.00',''),',','.') as MOVILI,
                            Replace(Replace(Convert(VARCHAR, Convert(money, tmpBONPRO.Monto), 1),'.00',''),',','.') as BONPRO,
                            Replace(Replace(Convert(VARCHAR, Convert(money, tmpBOTANO.Monto), 1),'.00',''),',','.') as BOTANO,
                            
														
				'Mas Bono EST de $ ' + REPLACE(REPLACE(CONVERT(VARCHAR,CONVERT(money, ccItem.Monto),1),'.00',''),',','.') as BONARA, 
														
                            (SELECT descCon FROM ccTipoContrato WHERE ccTipoContrato.tipoCon=REMPLES.tipcon) as tipoContrato,       
                             dbo.fn_fechaPalabras(fecha_ret) As fechaTermino,                                
						     (SELECT dbo.TRIM(UPPER(Descrip)) FROM RTABLAS WHERE COTAB=8 And RTABLAS.codigo=REMPLES.cod_AFP) as nombreAFP,                                
						     (SELECT dbo.TRIM(UPPER(Descrip)) FROM RTABLAS WHERE COTAB=4 And RTABLAS.codigo=REMPLES.cod_ISA) as nombreISAPRE,                                
						     ccContratoMarco.codContrato as codContratoMarco,
                             dbo.fn_fechaPalabras(ccContratoMarco.fechaContrato) As fechaContratoMarco,                                
                             ccEmpUsuaria.razonSocial as nombreUsuaria,
                 dbo.fn_RutconPuntos(ccEmpUsuaria.rut) As rutUsuaria,

								 (SELECT rutRepresentanteMarco FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as rutRepUsuaria,
								 (SELECT nombreRepresentanteMarco FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as reprUsuaria,
								 (SELECT direccionMarco FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as dirUsuaria,
								 (SELECT ciudadMarco FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as ciudadUsuaria,
								 (SELECT dbo.fn_fechaPalabras(fechaMarco) FROM ccUnidades WHERE ccUnidades.cvPayroll=REMPLES.unidad) as fechacontratoUsuaria,
								 
								 
								 (SELECT UPPER(detalleCausalContrato) FROM cpCausasLegales WHERE cpCausasLegales.codigo=REMPLES.CLASIF) as descDetalleCausal,                                
			                 ccContratoMarco.detalleCausal as detalleCausalContratoMarco,
                             dbo.fn_fechaPalabras(ccContratoMarco.fechaInicio) As fechaIniContratoMarco,                                
                             dbo.fn_fechaPalabras(ccContratoMarco.fechaFin) As fechaFinContratoMarco,                                
			                 ccContratoMarco.cantDias as cantDiasContratoMarco,
                             Replace(Replace(Convert(VARCHAR, Convert(money, ccContratoMarco.valorPagar), 1),'.00',''),',','.')as valorContratoMarco,
                             dbo.fn_NumeroALetras(ccContratoMarco.valorPagar) as valorEnPalabrasContratoMarco
                       From REMPLES
                            Left Join ccREMPLEC02 ON ccREMPLEC02.codigo=REMPLES.Codigo
                            Left Join ccREMPLEC03 ON ccREMPLEC03.codigo=REMPLES.Codigo
                            Left Join ccContratoMarco ON ccContratoMarco.codContrato=dbo.TRIM(REMPLES.CREDENC)
                            Left Join ccEmpUsuaria ON ccEmpUsuaria.id=ccContratoMarco.empUsuaria
                            Left Join RMAPITM ON (RMAPITM.codigo=REMPLES.Codigo And RMAPITM.COHADE='SUBASE')

                            Left Join RMAPITM as tmpCOLACI ON (tmpCOLACI.codigo=REMPLES.Codigo And tmpCOLACI.COHADE='COLACI')
                            Left Join RMAPITM as tmpMOVILI ON (tmpMOVILI.codigo=REMPLES.Codigo And tmpMOVILI.COHADE='MOVILI')
                            Left Join RMAPITM as tmpBONPRO ON (tmpBONPRO.codigo=REMPLES.Codigo And tmpBONPRO.COHADE='BONPRO')
                            Left Join RMAPITM as tmpBOTANO ON (tmpBOTANO.codigo=REMPLES.Codigo And tmpBOTANO.COHADE='BOTANO')
                            Left Join RMAPITM as ccItem ON (ccItem.codigo=REMPLES.Codigo And ccItem.COHADE='BONARA')

                WHERE REMPLES.ESTADO ='A' AND REMPLES.rut='{0}' AND REMPLES.fecha_ing IN (SELECT MAX(fecha_ing) FROM REMPLES WHERE ESTADO='A' AND RUT='{0}')", Session("pubRutContrato"))

            Dim ds As New DataSet

            Dim conexionContrato As New SqlConnection
            If ConectaSQLServerConn(conexionContrato) Then
                Using conexionContrato
                    Try
                        da = New SqlDataAdapter(pSqlStr, conexionContrato)
                        da.Fill(ds, "Datos")
                        documentServer.Options.MailMerge.DataSource = ds
                        documentServer.Options.MailMerge.DataMember = "Datos"
                        documentServer.Options.MailMerge.ViewMergedData = False
                    Catch ex As Exception
                        MsgBox("Error")
                    End Try
                End Using
            End If

            Using Stream As New MemoryStream()
                documentServer.MailMerge(Stream, DocumentFormat.OpenXml)
                Stream.Position = 0
                DocumentManager.CloseDocument(documentId)
                RichEdit.Open(documentId, DocumentFormat.OpenXml, Function() Stream)
            End Using
        End If
    End Sub

    Protected Sub btnCerrarSesion_ServerClick(sender As Object, e As EventArgs)
        Session("DotacionMensual") = Nothing
        Session("pubRutContrato") = Nothing
        Session("Validado") = 0
        Session("pubIdUsuario") = Nothing
        Session("pubEmpUsuariaUsuario") = Nothing
        Session("pubNombreUsuario") = Nothing
        Session("pubUnidadesUsuario") = Nothing
        Session("pubEsAdminEst") = Nothing
        Session("pubIsAdmin") = Nothing
        Session("CambiarClave") = 0
        Session("Administrador") = Nothing
        Session("Auditor") = Nothing
        Session("Web") = Nothing
        Session("Operaciones") = Nothing
        Session("Cliente") = Nothing
        Session("SuperAdmin") = Nothing
        Session("pModoPopup") = ""
        Session("pModoPopupSolicitud") = ""
        Session("pModoPopupCreaDoc") = ""
        Session("pubFileNameContrato") = Nothing
        Response.Redirect("~/pag_login.aspx")
    End Sub

    Protected Sub btnCambioClave_ServerClick(sender As Object, e As EventArgs)
        Session("CambiarClave") = 1
        Response.Redirect("/pag_seguridad.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Sub DescargarPDF()
        Using stream As New MemoryStream
            RichEdit.ExportToPdf(stream)
            Page.Response.Clear()
            Page.Response.Buffer = False
            Page.Response.AppendHeader("Content-Type", "application/pdf")
            Page.Response.AppendHeader("Content-Transfer-Encoding", "binary")
            Page.Response.BinaryWrite(stream.ToArray())
            Page.Response.End()
        End Using
    End Sub


    Protected Sub cbpDescargaPDF_Callback(source As Object, e As CallbackEventArgs)
        Using stream As New MemoryStream
            RichEdit.ExportToPdf(stream)
            Page.Response.Clear()
            Page.Response.Buffer = False
            Page.Response.AppendHeader("Content-Type", "application/pdf")
            Page.Response.AppendHeader("Content-Transfer-Encoding", "binary")
            Page.Response.BinaryWrite(stream.ToArray())
            Page.Response.End()
        End Using

    End Sub
End Class