Public Class MPSistema
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtUsuario.Text = "Usuario: " + Session("pubNombreUsuario")
    End Sub

    Protected Sub btnCerrarSesion_Click(sender As Object, e As EventArgs) Handles btnCerrarSesion.Click
        Session("Validado") = 0
        Session("pubIdUsuario") = Nothing
        Session("pubEmpUsuariaUsuario") = Nothing
        Session("pubNombreUsuario") = Nothing
        Session("pubUnidadesUsuario") = Nothing
        Session("pubEsAdminEst") = Nothing
        Session("pubIsAdmin") = Nothing
        Response.Redirect("~/pag_inicio.aspx")
    End Sub
End Class