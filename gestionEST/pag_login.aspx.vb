Imports System.Data
Imports System.Data.SqlClient
Public Class pag_inicio
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


        txtUser.Focus()
        Session("Validado") = 0
    End Sub

    Protected Sub btnLoginIngresar_ServerClick(sender As Object, e As EventArgs) Handles btnLoginIngresar.ServerClick
        Session("pubIdUsuario") = Nothing
        Session("Validado") = 0
        If Not (fnAccesoEmpresa(txtUser.Value, txtPassword.Value)) Then
            Literal1.Text = ("<script>alert('¡No tiene Acceso al Sistema!')</script>")
            txtUser.Value = Nothing
            txtPassword.Value = Nothing
            Return
        End If

        Session("pubIdUsuario") = UCase(txtUser.Value)
        Session("Validado") = 1

        Response.Redirect("/pag_index.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub


    Function fnAccesoEmpresa(ByVal sId As String, ByVal sPassword As String) As Boolean

        fnAccesoEmpresa = False
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
        Session("pModoPopupSolicitud") = ""
        Session("pModoPopupCreaDoc") = ""
        Session("RecargarEvaluacion") = Nothing
        sqlStr = "SELECT * FROM ccUsuarioWeb WHERE usuario='" & sId & "' AND Password='" & sPassword & "'"

        Dim esAdmin = 0
        Dim esAdminEST = 0
        Dim esAdminUsuarios = 0

        If ConectaSQLServer() Then
            Using conn
                Try

                    Dim cmd As New SqlCommand(sqlStr, conn)
                    Dim rdr As SqlDataReader = cmd.ExecuteReader()

                    If rdr.HasRows Then
                        While rdr.Read

                            Session("pubEmpUsuariaUsuario") = rdr("empUsuaria")
                            Session("pubNombreUsuario") = rdr("nombre")
                            fnAccesoEmpresa = True

                            esAdmin = rdr("esAdmin")
                            esAdminEST = rdr("esAdminEST")
                            esAdminUsuarios = rdr("esAdminUsuarios")

                            'SUPER ADMIN
                            If esAdmin = 1 And esAdminEST = 1 And esAdminUsuarios = 4 Then
                                Session("SuperAdmin") = True
                                Session("pubUnidadesUsuario") = fnCargaTodasUnidades()
                            Else
                                Session("SuperAdmin") = False
                            End If

                            'ADMINISTRADOR
                            If esAdmin = 1 And esAdminEST = 1 And esAdminUsuarios = 1 Then
                                Session("Administrador") = True
                                Session("pubUnidadesUsuario") = fnCargaTodasUnidades()
                            Else
                                Session("Administrador") = False
                            End If

                            'AUDITOR
                            If esAdmin = 1 And esAdminEST = 1 And esAdminUsuarios = 2 Then
                                Session("Auditor") = True
                                Session("pubUnidadesUsuario") = fnCargaTodasUnidades()
                            Else
                                Session("Auditor") = False
                            End If

                            'WEB
                            If esAdmin = 1 And esAdminEST = 0 And esAdminUsuarios = 1 Then
                                Session("Web") = True
                                Session("pubUnidadesUsuario") = fnCargaTodasUnidades()
                            Else
                                Session("Web") = False
                            End If

                            'OPERACIONES
                            If esAdmin = 0 And esAdminEST = 0 And esAdminUsuarios = 3 Then
                                Session("Operaciones") = True
                                Session("pubUnidadesUsuario") = rdr("unidades")
                            Else
                                Session("Operaciones") = False
                            End If

                            'CLIENTE
                            If esAdmin = 0 And esAdminEST = 0 And esAdminUsuarios = 0 Then
                                Session("Cliente") = True
                                Session("pubUnidadesUsuario") = rdr("unidades")
                            Else
                                Session("Cliente") = False
                            End If

                            'If Session("Administrador") = True Or Session("Auditor") = True Or Session("Web") = True Then
                            '    'ESTAS SE USAN EN LO RELACIONADO A CONTRATO
                            '    Session("pubUnidadesUsuario") = fnCargaUnidades(Session("pubEmpUsuariaUsuario"))
                            '    Session("pubIsAdmin") = True

                            'Else
                            '    Session("pubIsAdmin") = False
                            '    Session("pubUnidadesUsuario") = rdr("unidades")
                            'End If

                        End While
                    End If
                    rdr.Close()

                Catch ex As Exception
                    ShowPopUpMsg(ex.ToString)
                End Try
            End Using

        End If

        Return fnAccesoEmpresa

    End Function

    Sub ShowPopUpMsg(ByVal msg As String)

        Dim sb As New StringBuilder
        sb.Append("alert('")
        sb.Append(msg)
        sb.Append("');")
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Gestion EST", sb.ToString, True)

    End Sub

    Protected Sub btnIngresar_ServerClick()

    End Sub
End Class