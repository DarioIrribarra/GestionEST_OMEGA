<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="pag_contrato.aspx.vb" Inherits="gestionEST.pag_contrato" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxRichEdit.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxRichEdit" tagprefix="dx" %>

<!DOCTYPE html>

<html>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="description" content="">
    <meta name="author" content="">
    
    <title>Contratos - San Cristóbal RRHH</title>

    <!-- Bootstrap core CSS -->
    <link href="vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="Content/sidebar.css" rel="stylesheet">

    <%--Font Awesome--%>
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.5.0/css/all.css" integrity="sha384-B4dIYHKNBt8Bc12p+WXckhzcICo0wtJAoU8YZTY5qE0Id1GSseTk6S+L3BlXeVIU" crossorigin="anonymous">

    <script>
        function DescargarPDF(s, e) {  
            if (e.commandName == "DescargarPDF") {  
                __doPostBack(e.commandName, ''); 
            }  
            //cbpdescargapdf.PerformCallback();
        }
    </script>

    <style type="text/css">
        
        body {
            padding: 0;
            margin: 0;
            overflow-x: hidden;
            min-height: 240px;
            min-width: 250px;
        }

        .title {
        }

        .expandedPanel .title {
            display: none;
        }

        .mainMenu {
            float: right !important;
            margin: 8px 0 4px;
        }

        .expandedPanel .mainMenu {
            width: 100%;
        }

        .grid,
        .grid .dxgvHSDC,
        .grid .dxgvCSD {
            border-left: 0 !important;
            border-right: 0 !important;
            border-bottom: 0 !important;
        }

        .ajuste_img {
            object-fit:contain;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button Visible="false" UseSubmitBehavior="false" ID="btnDefault" runat="server" />
        <div>
                <%--PARTE SUPERIOR--%>
            <dx:ASPxPanel ID="TopPanel" BorderBottom-BorderStyle="none" runat="server" FixedPosition="WindowTop"
                ClientInstanceName="TopPanel" CssClass="topPanel" Collapsible="true">
                <Styles ExpandedPanel-Wrap="True">
                    <ExpandBar Wrap="True">
                    </ExpandBar>
                    <ExpandedPanel CssClass="expandedPanel">
                        <Paddings Padding="0" />
                    </ExpandedPanel>
                </Styles>
                <SettingsAdaptivity CollapseAtWindowInnerWidth="1050" />
                <PanelCollection>
                    <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                        <div class="w-100">
                            <div class="d-inline-flex">
                                <%--                        <div class="title d-inline-flex" style="margin-top: -10px; margin-left: -10px;">--%>
                                <div class="title  p-0 " style="width: 182px; background-color: white; margin-left: -10px; margin-top: -10px; margin-bottom: -10px">
                                    <img class="title w-100 h-100" style="padding-top: 20px; padding-bottom: 20px; padding-left: 10px; padding-right: 20px"
                                        src="images/logo_azul_Gestion.png" />
                                    <%--                            </div>--%>
                                </div>
                            </div>
                            <div class=" d-inline-flex">
                                <div class="title" style="margin-left: 6px; padding-left: 10px; padding-bottom: 0px; margin-bottom: -8px; padding-top: 0px; background-color: transparent; color: #00489e">
                                    <h3 class="title p-0"><b>Software Gestión de Documentos</b></h3>
                                </div>

                            </div>
                            <div class="d-inline-flex float-right">
                                <div class="">
                                    <table class=" table-responsive-sm" border="0">
                                        <tr>
                                            <td>
                                                <asp:Image CssClass="ajuste_img" ID="Image1" runat="server" ImageUrl="images/10.png" style="padding-right: 10px;" />
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                            <td style="margin-right: 5px">
                                                <span style="color: #00489e">Empresa&nbsp: </span>
                                            </td>
                                            <td style="margin-right: 5px">
                                                <label style="color: #00489e; margin-bottom: 0px" id="lblNombreEmpresa" runat="server">Nombre de la Empresa</label>
                                            </td>
                                            <td style="border-left: 1px solid; border-left-color: #00489e; padding-left: 20px">
                                                <button type="button" class="boton_sesion" style="color: #00489e; margin-bottom: 3px" runat="server" id="btnCerrarSesion" name="btnCerrarSesion" onserverclick="btnCerrarSesion_ServerClick">
                                                    <i class="fas fa-user"></i>&nbsp Cerrar Sesión
                                                </button>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="margin-right: 5px">
                                                <span style="color: #00489e">Usuario &nbsp&nbsp: </span>
                                            </td>
                                            <td style="margin-right: 5px; padding-right: 10px">
                                                <label style="color: #00489e; margin-bottom: 0px" id="txtUsuario" runat="server">Nombre del Usuario</label>

                                            </td>
                                            <td style="border-left: 1px solid; border-left-color: #00489e; padding-left: 20px">
                                                <button type="button" id="btnCambioClave" runat="server" style="color: #00489e" class="boton_sesion" onserverclick="btnCambioClave_ServerClick"><i class="fas fa-key"></i>&nbsp Cambiar Clave</button></td>
                                        </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        
                                    </table>
                                </div>
                            </div>
                        </div>

                    </dx:PanelContent>
                </PanelCollection>
                <ExpandBarTemplate>
                    <img width="150" style="padding-right: 3px;" src="images/logo1.png" />
                    <div class="title d-inline-flex" style="background-color: #f8f8f9; color: #00489e">
                        <h4 class=""><b>Gestión Documentos</b></h4>
                    </div>

                </ExpandBarTemplate>
            </dx:ASPxPanel>
            <%--PARTE IZQUIERDA--%>
            <dx:ASPxPanel Styles-Panel-Wrap="True" Width="180" BorderRight-BorderStyle="None" ID="LeftPanel" BackColor="White" runat="server" FixedPosition="WindowLeft" 
                Collapsible="true">
                <SettingsAdaptivity CollapseAtWindowInnerWidth="1050" />
                
                <PanelCollection>
                    <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
                        <dx:ASPxMenu Font-Names="verdana" ItemWrap="true" 
                            ItemStyle-Border-BorderStyle="None"
                            Border-BorderStyle="none" 
                            Border-BorderColor="Transparent"
                            ItemStyle-HoverStyle-Border-BorderStyle="None"
                            ItemStyle-HoverStyle-BackColor="White"
                            ItemStyle-Paddings-PaddingBottom="10px"
                            BackColor="Transparent" 
                            ID="ASPxMenu1" Orientation="Vertical" 
                            ItemStyle-CheckedStyle-Font-Bold="true"
                            ItemStyle-Font-Size="Medium" ItemStyle-ForeColor="#00489e" runat="server">

                            <ItemStyle 
                                HorizontalAlign="Left"
                                VerticalAlign="Middle"
                                SelectedStyle-Font-Bold="true"
                                SelectedStyle-Font-Size="Medium"
                                SelectedStyle-BackgroundImage-HorizontalPosition="right"
                                SelectedStyle-BackgroundImage-ImageUrl="images/01.png"
                                SelectedStyle-BackgroundImage-Repeat="NoRepeat"
                                SelectedStyle-BackgroundImage-VerticalPosition="center"
                                Border-BorderColor="White" />
                            <%--AL PASAR POR ENCIMA--%>
                            <ItemStyle 
                                HoverStyle-BackColor="#d0d0d0"/>
                            <%--<ItemStyle
                                HoverStyle-BackgroundImage-HorizontalPosition="right"
                                HoverStyle-BackgroundImage-VerticalPosition="center"
                                HoverStyle-BackgroundImage-Repeat="NoRepeat"
                                HoverStyle-BackgroundImage-ImageUrl="images/01.png" />--%>
                            <Items >
                                <dx:MenuItem Text="Empresa" NavigateUrl="pag_empresa.aspx">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Requerimientos" NavigateUrl="pag_requerimientos.aspx">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Crear Documentos" NavigateUrl="pag_creacionDocumentos.aspx" Name="crearDocs">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Evaluación" NavigateUrl="pag_EvaluacionArauco.aspx" Name="evaluacionArauco">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Colaboradores" NavigateUrl="pag_colaboradores.aspx">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Contrataciones" NavigateUrl="pag_contrataciones.aspx" Name="contrataciones">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Reportabilidad" NavigateUrl="pag_reportes.aspx">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Auditoría" NavigateUrl="pag_auditoria.aspx" Name="auditoria">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Perfil" NavigateUrl="pag_seguridad.aspx">
                                </dx:MenuItem>
                                <dx:MenuItem Text="Documentos RRHH" Name="descarga">
                                </dx:MenuItem>
                            </Items>
                            
                        </dx:ASPxMenu>
                    </dx:PanelContent>
                </PanelCollection>
            </dx:ASPxPanel>
            <%--PARTE DERECHA--%>
            
            <dx:ASPxRichEdit ID="RichEdit" runat="server" Width="100%" Height="800" 
                ReadOnly="True" RibbonMode="OneLineRibbon" ShowStatusBar="False"
                Settings-HorizontalRuler-Visibility="Hidden" ShowConfirmOnLosingChanges="False"
                WorkDirectory="~\App_Data\documentos" ClientSideEvents-CustomCommandExecuted="DescargarPDF" >

                <RibbonTabs>
                    <dx:RERFileTab>
                        <Groups>
                            <dx:RERFileCommonGroup>
                                <Items>
                                    <dx:RERPrintCommand Size="Large" >
                                    </dx:RERPrintCommand>
                                   <%-- <dx:RibbonButtonItem Size ="Large" Text="DescargarPDF">
                                    </dx:RibbonButtonItem>--%>
                                </Items>
                            </dx:RERFileCommonGroup>
                        </Groups>
                    </dx:RERFileTab>
                </RibbonTabs>
                <SettingsDocumentSelector >
                    <EditingSettings DownloadedArchiveName="contrato" />
                </SettingsDocumentSelector>
                <Settings Unit="Centimeter">
                    <HorizontalRuler Visibility="Hidden"></HorizontalRuler>
                </Settings>
            </dx:ASPxRichEdit>

            <dx:ASPxCallback runat="server" ID="cbpDescargaPDF" ClientInstanceName="cbpdescargapdf" OnCallback="cbpDescargaPDF_Callback">

            </dx:ASPxCallback>
            
            <!-- Bootstrap core JavaScript -->
            <script src="vendor/jquery/jquery.min.js"></script>
            <script src="vendor/bootstrap/js/bootstrap.bundle.min.js"></script>

        </div>
        <%--<asp:TextBox Style="text-align: right; font-size: 13px; position: absolute; left: 900px; width: 400px; margin-top: 29px;"
            ID="txtUsuario" runat="server"
            BackColor="Transparent" BorderStyle="None" ForeColor="White" ReadOnly="True">

        </asp:TextBox>--%>
    </form>
</body>
</html>
