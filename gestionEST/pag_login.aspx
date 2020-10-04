<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="pag_login.aspx.vb" Inherits="gestionEST.pag_inicio" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<link href="Content/pag_inicio.css" rel="stylesheet"/>
<link href="Content/bootstrap.css" rel="stylesheet"/>
<script src="//maxcdn.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js"></script>
<script src="//cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <title></title>
     <style type="text/css">
            .txtbox {
                -webkit-border-radius: 50px;
                -moz-border-radius: 50px;
                border-radius: 50px;
            }
        </style>
</head>
<body>
    <div class="container login-container">
        <div class="row" style="margin-left: 15%; margin-top: 10%; margin-bottom: 15%;">
            <div class="col-md-4 login-form-1 d-flex align-items-center " style="background-color: #00489e">
                <form>
                    <div class="form-group d-flex justify-content-center">
                        <img src="images/logoNuevo.png" style="width:100%; height:50%" class="imagen" />
                    </div>
                </form>
            </div>
            <div class="col-md-6 login-form-2" style="background-color: #ffffff;">
                <form runat="server">
                    <p style="color: #00489e; text-align: center; font-size: larger">
                        <b>Iniciar Sesión</b>
                    </p>

                    <div runat="server" class="form-group">
                        <input id="txtUser" name="txtUser" required="" type="text" runat="server"
                            class="form-control txtbox font-italic text-center" style="background-color: #f8f8f9"
                            placeholder="Usuario" />
                    </div>

                    <div runat="server" class="form-group">
                        <input id="txtPassword" name="txtUser" required="" type="password" runat="server"
                            class="form-control txtbox font-italic text-center" style="background-color: #f8f8f9"
                            placeholder="Contraseña" />
                    </div>

                    <div class="d-flex justify-content-center" runat="server">
                        <input id="btnLoginIngresar" name="btnLoginIngresar" type="submit" value="Ingresar"
                            class="btn w-50 txtbox text-light font-weight-bold" style="background-color: #ff1049"
                            runat="server" onserverclick="btnLoginIngresar_ServerClick" />
                    </div>

                    <%--<div class="d-flex justify-content-center" style="font-style: italic; font-size: small; color: #00489e">
                        <a>¿Olvidaste tu contraseña?</a>
                        
                    </div>--%>
                    <asp:Literal ID="Literal1" runat="server"></asp:Literal>

                    <div class="container fondo" style="color: #00489e; margin-left:50px; ">
                        <a href="https://www.sancristobalrrhh.cl/">visítanos en www.sancristobalrrhh.cl
                        </a>
                    </div>

                </form>
            </div>
        </div>
    </div>
</body>
</html>
