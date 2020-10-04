<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_seguridad.aspx.vb" Inherits="gestionEST.pag_seguridad" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Barra_Azul" runat="server">

    <style type="text/css">
        .estiloBtnBuscar {
            background-image: none;
            background-color: #ff1049;
            border: 1px solid transparent;
            padding: 0px 0px 0px 0px !important;
            color: white;
            font-weight: bold;
            border-radius: .25rem !important;
            /*TIEMPO DE RETRASO ANTES DE ACTIVACIÓN DE EFECTO*/
            /* Safari */
            -webkit-transition-duration: 0.4s;
            transition-duration: 0.4s;
        }

            .estiloBtnBuscar:hover {
                border: 2px solid lightgrey;
                border-color:#00489E;
                /*background-color:white;*/
                /*color:#ff1049;*/
            }


        .estiloBtnSubmenusExportar {
            background: none;
            border: 1px solid transparent;
            font-weight: bold;
            color: white;
            /*TIEMPO DE RETRASO ANTES DE ACTIVACIÓN DE EFECTO*/
            /* Safari */
            -webkit-transition-duration: 0.4s;
            transition-duration: 0.4s;
        }

            .estiloBtnSubmenusExportar:hover {
                /*border:1px solid white;*/
                /*background-color:ThreeDShadow;*/
                border-bottom: 1px solid white;
            }

        /*Style="background-color: #ff1049; width: 60%;"*/

        .btnGridSinFondo {
            background: none;
            border: none;
        }
    </style>

    <dx:ASPxMenu AllowSelectItem="true" Paddings-PaddingLeft="0px" Width="150" Orientation="Vertical" CssClass="font-weight-bold" ForeColor="White" BackColor="Transparent" Border-BorderStyle="None" ID="menuSeguridad" 
        runat="server" OnItemClick="menuSeguridad_ItemClick">

        <ItemStyle
            HoverStyle-BackgroundImage-Repeat="NoRepeat"
            HoverStyle-BackgroundImage-HorizontalPosition="right"
            HoverStyle-BackgroundImage-VerticalPosition="center"
            HoverStyle-Border-BorderStyle="None"
            HoverStyle-BackColor="#d0d0d0" HoverStyle-ForeColor="#00489e"
            SelectedStyle-BackgroundImage-HorizontalPosition="right"
            SelectedStyle-BackColor="Transparent"
            SelectedStyle-BackgroundImage-ImageUrl="images/01.png"
            SelectedStyle-BackgroundImage-Repeat="NoRepeat"
            SelectedStyle-Border-BorderStyle="None"
            SelectedStyle-BackgroundImage-VerticalPosition="center" />
        
        <Items>
            <dx:MenuItem Name="perfil" Text="Mi Perfil"></dx:MenuItem>
            <dx:MenuItem Name="clave" Text="Cambio de Clave"></dx:MenuItem>
            <dx:MenuItem Name="usuarios" Text="Usuarios"></dx:MenuItem>
            <%--<dx:MenuItem Name="solicitudes" Text="Historial Solicitudes"></dx:MenuItem>--%>
        </Items>
    </dx:ASPxMenu>
    <hr style="background-color:red;"/>

    <%-- BOTON DE POPUPS --%>

    <table>
        <tr id="tr_popupUsuarios" runat="server" visible="false">
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage3" runat="server" ShowLoadingImage="true" ImageUrl="images/05.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnPopUpUsuarios" ImagePosition="Left" EnableTheming="false" Font-Names="verdana" CssClass="estiloBtnSubmenusExportar" Style="background-color: #00489e; width: 100%;" Text="Cargar Archivos" runat="server" Visible="true" OnClick="btnPopUpUsuarios_Click" AutoPostBack="False" UseSubmitBehavior="false" >
<%--                    <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />--%>

                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <%--POPUP MODIFICAR--%>
    <table>
        <tr id="tr_popupModificarUsuario" runat="server" visible="false">
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage5" runat="server" ShowLoadingImage="true" ImageUrl="images/06.png">
                </dx:ASPxImage>
            </td>
            <td style="margin-bottom:5px" class="d-inline-flex align-content-center">
                <dx:ASPxButton Wrap="True" ID="btnModificaUsuarios" BorderRight-BorderStyle="None" EnableTheming="false" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" Style="background-color: #00489e; width: 100%;" Font-Size="Small" runat="server" OnClick="btnModificaUsuarios_Click" Text="Modificar Usuario" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False">
                    
                </dx:ASPxButton>
            </td>
        </tr>
    </table>


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContenidoPagina" runat="server">

    <script type="text/javascript">
        //MOSTRAR IMAGEN
        function OnFileUploadComplete(s, e) {
            document.getElementById('<%=image.ClientId%>').src = e.callbackData;
        }

        function OnClickGrabaCambio(s, e) {
            if (ASPxClientEdit.ValidateGroup('entryGroup')) {
                e.processOnServer = true;
            }
            else {
                e.processOnServer = false;
            }
        }

        function OnClickCargarCheckbox(s, e) {
            e.processOnServer = true;
        }

        function OnClickCrearUsuario(s, e) {
            if (ASPxClientEdit.ValidateGroup('entryGroup')) {
                e.processOnServer = true;
            }
            else {
                e.processOnServer = false;
            }

        }
        function requerido(s, e) {
            var nombreReferido = document.getElementById('lblNombreReferido');
            var UploadControl = document.getElementById('td_UploadControl');
            var td_NombreReferido = document.getElementById('td_NombreReferido');
          <%--var UploadControl = "<%= UploadControl.ClientInstanceName %>";--%>

            if (s.GetChecked() == true) {
                nombreReferido.style.visibility = "visible";
                UploadControl.style.visibility = "visible";
                td_NombreReferido.style.visibility = "visible";

            }
            else {
                nombreReferido.style.visibility = 'hidden';
                UploadControl.style.visibility = 'hidden';
                td_NombreReferido.style.visibility = 'hidden';
            }
        }

        function Eliminar(s, e) {
            var result = confirm('¡¿Seguro que desea borrar el Usuario Seleccionado?!');
            //console.log("CONFIRM: " + result);
            e.processOnServer = result;
        }

        function jsCalculaFechaFin() {

            var days = txtDias.GetNumber();
            if (days > 0) {
                var dateObject = new Date();
                dateObject = dtFechaDesde.GetDate();
                dateObject = new Date(dateObject.valueOf());
                dateObject.setDate(dateObject.getDate() + days - 1);
                dtFechaHasta.SetDate(dateObject);
            }
        }

        function jsCalculaDias() {

            var fechaini = new Date();
            var fechafin = new Date();
            fechaini = dtFechaDesde.GetDate();
            fechafin = dtFechaHasta.GetDate();
            var timeDiff = (dtFechaHasta.GetDate() - dtFechaDesde.GetDate());
            var dias = Math.ceil(timeDiff / (1000 * 3600 * 24)) + 1;
            if (dias < 1) {
                dias = 1;
                dtFechaHasta.SetDate(fechaini);
            }
            else {
                txtDias.SetValue(dias);
            }

        }

        function seleccionUnidades(s,e) {
            cbpUnidades.PerformCallback();
        }

    </script>

    <style type="text/css">  
        .select {  
            width:110px;
            height:25px;
            color:darkblue;
            -webkit-appearance: none; 
            background:url("images/20.png");
            background-position:right;
            background-repeat:no-repeat;
            font-family:Verdana;
        }  

        .ajuste_img {
            object-fit:scale-down;
        }
    </style> 

    <%--<%---------------------------------------------------PAGINA PERFIL-<%----------------------------------------------------%>
    <div id="div_perfil" runat="server" visible ="False">
        <label style="color: #00489e; font-family:Verdana; font-size:large; padding-left:40px">
            <b>DATOS PERSONALES</b>
        </label>
        <hr style="background-color: red;" />
        <table border="0" style="color:#00489e; width:95%; margin: 0px auto;">
            <tr>
                <td style="width: 100px;">
                    <dx:ASPxLabel ID="ASPxLabel3" runat="server" Text="Nombre" Font-Bold="true">
                    </dx:ASPxLabel>
                </td>
            </tr>
            <tr>
                <td style="width:80%" >
                    <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtNombrePerfil" runat="server" Width="100%" ClientInstanceName="txtNombrePerfil" >
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="true">
                            <RequiredField ErrorText="Debe ingresar Nombre" IsRequired="true" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle ForeColor="Red" Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
                <td rowspan="5" style="width:10px; padding-left:40px;">
                    <div style="max-width:150px; text-align:center">
                        <img src="images/10.png" id="image" runat="server" alt="Please load image" style="max-height:120px; max-width:150px;" />
                    </div>
                    
                    <dx:ASPxUploadControl ID="ASPxUploadControl1" runat="server" OnFileUploadComplete="ASPxUploadControl1_FileUploadComplete1" 
                        ForeColor="#00489e" Font-Names="verdana" Theme="MetropolisBlue" EnableTheming="True" 
                        CssClass="rounded txtbox text-white font-weight-bold" 
                        ClientInstanceName="UploadControl" Width="100%" NullText="Seleccione Archivo" FileUploadMode="OnPageLoad" 
                        UploadMode="Advanced" ShowProgressPanel="True" AutoStartUpload="true" ShowTextBox="false">
                        <BrowseButton Text="&nbsp;&nbsp;Cambiar Imagen">
                        </BrowseButton>
                        <BrowseButtonStyle Width="132">
                            
                        </BrowseButtonStyle>
                        <ValidationSettings AllowedFileExtensions=".jpg,.jpeg,.png,.gif">                            
                        </ValidationSettings>

                        <ClientSideEvents FileUploadComplete="OnFileUploadComplete" />
                       
                    </dx:ASPxUploadControl>
                </td>
            </tr>
            <tr>
                <td style="width: 100px;">
                    <dx:ASPxLabel ID="ASPxLabel4" runat="server" Text="Email" Font-Bold="true">
                    </dx:ASPxLabel>
                </td>
            </tr>
            <tr>
                <td >
                    <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtEmailPerfil" runat="server" Width="100%" ClientInstanceName="txtEmailPerfil" >
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="true">
                            <RequiredField ErrorText="Debe ingresar un Correo Electrónico" IsRequired="true" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle ForeColor="Red" Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 100px;">
                    <dx:ASPxLabel ID="ASPxLabel5" runat="server" Text="Teléfono" Font-Bold="true">
                    </dx:ASPxLabel>
                </td>
            </tr>
            <tr>
                <td >
                    <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtTelefonoPerfil" runat="server" Width="100%" ClientInstanceName="txtTelefonoPerfil" >
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="False">
                            <RequiredField ErrorText="Debe ingresar Teléfono" IsRequired="True" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle ForeColor="Red" Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 100px;">
                    <dx:ASPxLabel ID="ASPxLabel6" runat="server" Text="Sitio Web Empresa" Font-Bold="true">
                    </dx:ASPxLabel>
                </td>
            </tr>
            <tr>
                <td >
                    <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtSitioWebEmpresaPerfil" runat="server" Width="100%" ClientInstanceName="txtSitioWebEmpresaPerfil" >
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="False">
                            <RequiredField ErrorText="Debe ingresar Sitio Web de Empresa" IsRequired="True" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle ForeColor="Red" Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 100px;">
                    <dx:ASPxLabel ID="ASPxLabel8" runat="server" Text="Dirección Empresa" Font-Bold="true">
                    </dx:ASPxLabel>
                </td>
            </tr>
            <tr>
                <td >
                    <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtDireccionEmpresaPerfil" runat="server" Width="100%" ClientInstanceName="txtDireccionEmpresaPerfil" >
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="False">
                            <RequiredField ErrorText="Debe ingresar Dirección de Empresa" IsRequired="True" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle ForeColor="Red" Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align:right">
                    <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="background-color: #ff1049;" Width="80" Height="20" ID="btnAgregarDatosPerfil" runat="server" Text="Guardar" OnClick="btnAgregarDatosPerfil_Click">
                        <ClientSideEvents Click="OnClickGrabaCambio" />
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>
    </div>

    <%---------------------------------------------------PAGINA CAMBIO DE CLAVE-<%----------------------------------------------------%>
    <div id="div_clave" runat="server" visible="False">
        <div style="padding-left: 71px;" class="d-inline-flex">
            <asp:Image CssClass="ajuste_img" ID="imagenCambioClave" runat="server" ImageUrl="images/10.png"/>

            <table style="padding-left: 40px; color: #00489e" class="d-inline-flex">
                <tr>
                    <td>
                        <label><b>USUARIO</b><span style="padding-left: 30px">:</span></label></td>
                    <td>
                        <label id="lblNombreUsuario" runat="server" style="padding-left: 30px">NOMBRE DE ALGUN USUARIO</label></td>
                </tr>
                <tr id="tr_rut" runat="server">
                    <td>
                        <label><b>RUT</b><span style="padding-left: 62px">:</span></label></td>
                    <td>
                        <label style="padding-left: 30px">NUMERO DE RUT</label></td>
                </tr>
                <tr>
                    <td>
                        <label><b>EMPRESA</b><span style="padding-left: 27px">:</span></label></td>
                    <td>
                        <label id="lblNombreEmpresa" runat="server" style="padding-left: 30px">NOMBRE DE ALGUNA EMPRESA</label></td>
                </tr>
            </table>
        </div>

        <hr style="background-color: red;" />

        <table style="table-layout: fixed; color: #00489e" border="0">
            <tr>
                <td rowspan="4" style="width: 70px">
                    <div class="pcmSideSpacer">
                    </div>
                </td>

                <!-- contraseña anterior-->
                <td class="pcmCellCaption" style="width: 100px;">
                    <dx:ASPxLabel ID="ASPxLabel7" runat="server" Text="Contraseña Anterior">
                    </dx:ASPxLabel>
                </td>

                <td class="pcmCellText">
                    <dx:ASPxTextBox CssClass="rounded" Theme="MetropolisBlue" ID="txtAnterior" runat="server" Width="250px" ClientInstanceName="txtAnterior" Password="True">
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="true">
                            <RequiredField ErrorText="Debe ingresar Contraseña anterior" IsRequired="true" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>

            </tr>

            <tr>
                <!-- contraseña nueva-->
                <td class="pcmCellCaption">
                    <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Contraseña Nueva">
                    </dx:ASPxLabel>
                </td>

                <td class="pcmCellText">
                    <dx:ASPxTextBox CssClass="rounded" Theme="MetropolisBlue" ID="txtNueva" runat="server" Width="250px" ClientInstanceName="txtNueva" Password="True">
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="true">
                            <RequiredField ErrorText="Debe ingresar Contraseña Nueva" IsRequired="true" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
            </tr>

            <tr>
                <!-- reingresar contraseña nueva-->
                <td class="pcmCellCaption">
                    <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="Reingresar Contraseña">
                    </dx:ASPxLabel>
                </td>

                <td class="pcmCellText">
                    <dx:ASPxTextBox CssClass="rounded" Theme="MetropolisBlue" ID="txtNuevaRe" runat="server" Width="250px" ClientInstanceName="txtNuevaRe" Password="True">
                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                            ErrorTextPosition="Bottom" CausesValidation="true">
                            <RequiredField ErrorText="Debe ingresar reingresar Contraseña Nueva" IsRequired="true" />
                            <RegularExpression ErrorText="Faltan Datos" />
                            <ErrorFrameStyle Font-Size="12px">
                                <ErrorTextPaddings PaddingLeft="0px" />
                            </ErrorFrameStyle>

                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </td>
            </tr>

            <tr>
                <!-- Boton Grabar-->
                <td style="text-align:right" colspan="2">
                    <div class="pcmButton">
                        <dx:ASPxButton ID="btnCambiar" runat="server" Text="Guardar" Width="80" Height="20" EnableTheming="false" CssClass="estiloBtnBuscar" Style="background-color: #ff1049; float: right;" Font-Size="Small" AutoPostBack="false" OnClick="btnCambiar_Click">
                            <ClientSideEvents Click="OnClickGrabaCambio" />
                        </dx:ASPxButton>

                    </div>
                </td>
            </tr>
        </table>
    </div>

    <%--<%---------------------------------------------------PAGINA USUARIOS Y PERMISOS-<%----------------------------------------------------%>
    <div id="div_usuarios"  runat="server" visible ="False">

        <label style="color: #00489e; font-family:Verdana; font-size:large">
            <b>EDITAR USUARIOS</b>
        </label>
        <hr style="background-color: red;" />
        <%--GRID USUARIOS--%>

        <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridUsuariosWeb" OnPageIndexChanged="gridUsuariosWeb_PageIndexChanged"
            runat="server" ClientInstanceName="gridUsuariosWeb" 
            KeyFieldName="usuario" AutoGenerateColumns="False" Theme="Default">
            <Settings GridLines="Vertical" />
            <SettingsSearchPanel Visible="true" />
            <SettingsLoadingPanel Mode="Default" />
            <SettingsBehavior
                EnableRowHotTrack="true"
                AllowFocusedRow="True"
                AllowSelectByRowClick="false"
                AllowSelectSingleRowOnly="true" />
            <Styles>
                
                <Header Font-Bold="true" Font-Size="Small" ForeColor="#00489e" BackColor="#F2F2F2" HorizontalAlign="Center"></Header>
                <Cell Font-Size="Small"></Cell>
                <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#EAEAEA"></FocusedRow>
                <FixedColumn ForeColor="#00489E"></FixedColumn>
                <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
            </Styles>

            <Columns>
                <dx:GridViewDataColumn Caption="Nombre de Usuario" Name="usuario" FieldName="usuario">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Empresa<br/>Usuaria" Name="empUsuaria" FieldName="empUsuaria">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Correo<br/>Electrónico" Name="email" FieldName="email">
                </dx:GridViewDataColumn>

                <dx:GridViewDataComboBoxColumn Caption="Permisos" Name="permisos" FieldName="permisos">
                    <DataItemTemplate>
                        <asp:DropDownList CssClass="alert-heading" ForeColor="#00489e" Font-Names="verdana" ID="ddlPermisos" runat="server" OnInit="ddlPermisos_Init" OnSelectedIndexChanged="ddlPermisos_SelectedIndexChanged" Width="100%" AutoPostBack="True">
                            
                            <asp:ListItem Value ="0" Text="Cliente"></asp:ListItem>
                            <asp:ListItem Value ="1" Text="Administrador"></asp:ListItem>
                            <asp:ListItem Value ="2" Text="Web"></asp:ListItem>                     
                            <asp:ListItem Value ="3" Text="Auditor"></asp:ListItem>                     
                            <asp:ListItem Value ="4" Text="Operaciones"></asp:ListItem>                     
                        </asp:DropDownList>                     
                    </DataItemTemplate>
                </dx:GridViewDataComboBoxColumn>

                <dx:GridViewDataColumn CellStyle-HorizontalAlign="Center" Caption="Eliminar" FieldName="eliminar" Name="eliminar" Visible="True">
                <DataItemTemplate>
                    <dx:ASPxButton ID="btnEliminarUsuario" CssClass="btnGridSinFondo" EnableTheming="false" Font-Size="Small" runat="server" AutoPostBack="false" OnClick="btnEliminarUsuario_Click" UseSubmitBehavior="false">
                        <Image ToolTip="Eliminar" Url="images/03.png">
                        </Image>
                        <ClientSideEvents Click="Eliminar" />
                    </dx:ASPxButton>
                </DataItemTemplate>
            </dx:GridViewDataColumn>

            </Columns>
        </dx:ASPxGridView>
        
        <%--REGISTRO DENTRO DEL POPUP--%>
        <dx:ASPxHiddenField runat="server" ID="ASPxHiddenField1" ClientInstanceName="HiddenField"></dx:ASPxHiddenField>

        <%--'LOADING PANEL Y CALLBACK--%>
        <dx:ASPxLoadingPanel runat="server" ID="lpUnidades" ClientInstanceName="lpUnidades"></dx:ASPxLoadingPanel>

        <dx:ASPxPopupControl ID="popUpCrearUsuarios" runat="server" CloseAction="CloseButton" ClientInstanceName="popUpCrearUsuarios" EnableViewState="False"
            Modal="True" Width="100%" HeaderText="CREAR USUARIO" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
            <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="ASPxPanel1" runat="server">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <table border="0" style="color:#00489e">
                                    <tr>
                                        <td class="pcmCellCaption">
                                            <label>Empresa Usuaria</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxComboBox ForeColor="#00489e" NullText="SELECCIONE" CssClass="rounded" Theme="MetropolisBlue" ID="cbxEmpresaUsuaria" runat="server" ValueType="System.String" OnLoad="cbxEmpresaUsuaria_Load1">
                                                <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                <%--<ClientSideEvents SelectedIndexChanged="seleccionUnidades" />--%>
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Elija una unidad" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxComboBox>
                                        </td>

                                        <td id="td_unidadesHabilitadas" runat="server" visible="true" class="pcmCellCaption">
                                            
                                        </td>
                                        <td id="td_puntos" runat="server" visible="true" class="pcmCellCaption"></td>
                                        <td id="td_CheckListUnidades" runat="server" visible="true" rowspan="7" class="pcmCellText">
                                            <label>Unidades Habilitadas :</label>
                                            <%--RESULTADOS DE CHECKBOX--%>
                                            <dx:ASPxCheckBoxList ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="checkListPanl" ClientInstanceName="checkListPanl" runat="server" ValueType="System.String" RepeatColumns="6" OnInit="checkListPanl_Init">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Elija al menos una unidad" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxCheckBoxList>
                                        </td>
                                    </tr>
                                    <tr id="tr_nombre" runat="server" visible="true">
                                        <td class="pcmCellCaption">Nombre
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtNombrePermisos" runat="server" Width="170px">

                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Nombre no puede estar vacío" IsRequired="true" />
                                                    <RegularExpression ErrorText="Faltan Datos" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr_nombreUsuario" runat="server" visible="true">
                                        <td class="pcmCellCaption">Username
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtUsername" runat="server" Width="170px">

                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Nombre no puede estar vacío" IsRequired="true" />
                                                    <RegularExpression ErrorText="Faltan Datos" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr_correo" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>
                                                Correo Usuario
                                            </label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtCorreo" runat="server" Width="170px">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Ingrese un correo válido" IsRequired="true" />
                                                    <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorText="Ingrese un correo válido" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr1" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>
                                                Correo Supervisor
                                            </label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtCorreoSupervisorUsuarios" runat="server" Width="170px">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Ingrese un correo válido" IsRequired="true" />
                                                    <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorText="Ingrese un correo válido" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr_contraseña" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>
                                                Contraseña
                                            </label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtContraseñaPermisosUsuario" runat="server" Width="170px" Password="True">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Ingrese una contraseña" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr_confirmarContraseña" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>Confirmar Contraseña</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtConfirmarContraseñaPermisosUsuario" runat="server" Width="170px" Password="True">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Re ingrese la contraseña" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr_permisos" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>Permisos</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxComboBox ForeColor="#00489e" Theme="MetropolisBlue" AllowNull="false" NullText="SELECCIONE" ID="cbxPermisoCrearUsuario" CssClass="rounded" runat="server" ValueType="System.String">
                                                <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Seleccione un nivel de Permisos" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                                <Items>
                                                    <dx:ListEditItem Value="0" Text="CLIENTE" />
                                                    <dx:ListEditItem Value="1" Text="ADMINISTRADOR" />
                                                    <dx:ListEditItem Value="2" Text="WEB" />
                                                    <dx:ListEditItem Value="3" Text="AUDITOR" />
                                                    <dx:ListEditItem Value="4" Text="OPERACIONES" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>

                                    <tr id="tr10" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>Ve Evaluación</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxCheckBox runat="server" ID="chkEvaluacion" ClientInstanceName="chkevaluacion">
                                            </dx:ASPxCheckBox>
                                        </td>
                                    </tr>

                                    <tr id="tr_btnCrearUsuario" runat="server" visible="true">
                                        <td colspan="3" style="text-align:right">
                                            <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="background-color: #ff1049;" Width="100" Height="20" ID="btnCrear" runat="server" Text="Crear Usuario" OnClick="btnCrear_Click" AutoPostBack="true">
                                                <ClientSideEvents Click="OnClickCrearUsuario" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>

            </ContentCollection>
        </dx:ASPxPopupControl>

        <%--MODIFICAR DENTRO DEL POPUP--%>

        <dx:ASPxPopupControl ID="popUpModificar" runat="server" CloseAction="CloseButton" ClientInstanceName="popUpModificarUsuarios" EnableViewState="False"
            Modal="True" Width="100%" HeaderText="MODIFICAR USUARIO" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
            <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="ASPxPanel2" runat="server">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <table border="0" style="color:#00489e">
                                    <tr>
                                        <td class="pcmCellCaption">
                                            <label>Empresa Usuaria</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="cbxUnidadModificar" runat="server" ValueType="System.String" OnLoad="cbxEmpresaUsuaria_Load1">
                                                <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Elija una unidad" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                                <%--<ClientSideEvents SelectedIndexChanged="OnClickCargarCheckbox" />--%>
                                            </dx:ASPxComboBox>
                                        </td>

                                        <td id="td1" runat="server" visible="true" class="pcmCellCaption">
                                        </td>
                                        <td id="td2" runat="server" visible="true" class="pcmCellCaption"></td>
                                        <td id="td3" runat="server" visible="true" rowspan="7" class="pcmCellText">
                                            <label>Unidades Habilitadas :</label>
                                            <%--RESULTADOS DE CHECKBOX--%>
                                            <dx:ASPxCheckBoxList ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="checkListPanlModificar" runat="server" ValueType="System.String" RepeatColumns="6" OnInit="checkListPanl_Init">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Elija al menos una unidad" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxCheckBoxList>
                                        </td>
                                    </tr>
                                    <tr id="tr2" runat="server" visible="true">
                                        <td class="pcmCellCaption">Nombre
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtNombreUsuarioModificar" runat="server" Width="170px">

                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Nombre no puede estar vacío" IsRequired="true" />
                                                    <RegularExpression ErrorText="Faltan Datos" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr3" runat="server" visible="true">
                                        <td class="pcmCellCaption">Username
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" ReadOnly="true" Theme="MetropolisBlue" BackColor="Gray" ID="txtUsernameModificar" runat="server" Width="170px">

                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Nombre no puede estar vacío" IsRequired="true" />
                                                    <RegularExpression ErrorText="Faltan Datos" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr4" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>
                                                Correo Usuario
                                            </label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtCorreoUsuarioModificar" runat="server" Width="170px">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Ingrese un correo válido" IsRequired="true" />
                                                    <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorText="Ingrese un correo válido" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr5" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>
                                                Correo Supervisor
                                            </label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtCorreoSupervisorModificar" runat="server" Width="170px">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Ingrese un correo válido" IsRequired="true" />
                                                    <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorText="Ingrese un correo válido" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr6" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>
                                                Contraseña
                                            </label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" ReadOnly="true" BackColor="Gray" CssClass="rounded" Theme="MetropolisBlue" ID="txtContraseñaModificar" runat="server" Width="170px" Password="True">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="False">

                                                    <RequiredField ErrorText="Ingrese una contraseña" IsRequired="False" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr7" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>Confirmar Contraseña</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxTextBox ForeColor="#00489e" ReadOnly="true" BackColor="Gray"  CssClass="rounded" Theme="MetropolisBlue" ID="txtContraseñaConfirmarModificar" runat="server" Width="170px" Password="True">
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="False">

                                                    <RequiredField ErrorText="Re ingrese la contraseña" IsRequired="False" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr id="tr8" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>Permisos</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxComboBox ForeColor="#00489e" Theme="MetropolisBlue" AllowNull="false" NullText="SELECCIONE" ID="cbxPermisosModificar" CssClass="rounded" runat="server" ValueType="System.String">
                                                <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                <ValidationSettings
                                                    EnableCustomValidation="true"
                                                    ValidationGroup="entryGroup"
                                                    SetFocusOnError="true"
                                                    ErrorDisplayMode="Text"
                                                    ErrorTextPosition="Bottom"
                                                    CausesValidation="true">

                                                    <RequiredField ErrorText="Seleccione un nivel de Permisos" IsRequired="true" />
                                                    <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                        <ErrorTextPaddings PaddingLeft="0px" />
                                                    </ErrorFrameStyle>
                                                </ValidationSettings>
                                                <Items>
                                                    <dx:ListEditItem Value="0" Text="CLIENTE" />
                                                    <dx:ListEditItem Value="1" Text="ADMINISTRADOR" />
                                                    <dx:ListEditItem Value="2" Text="WEB" />
                                                    <dx:ListEditItem Value="3" Text="AUDITOR" />
                                                    <dx:ListEditItem Value="4" Text="OPERACIONES" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>

                                    <tr id="tr11" runat="server" visible="true">
                                        <td class="pcmCellCaption">
                                            <label>Ve Evaluación</label>
                                        </td>
                                        <td class="pcmCellCaption">:</td>
                                        <td class="pcmCellText">
                                            <dx:ASPxCheckBox runat="server" ID="chkEvaluacionModificar" ClientInstanceName="chkevaluacion">
                                            </dx:ASPxCheckBox>
                                        </td>
                                    </tr>

                                    <tr id="tr9" runat="server" visible="true">
                                        <td colspan="3" style="text-align:right">
                                            <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="background-color: #ff1049;" Width="100" Height="20" ID="btnGuardarModificar" runat="server" Text="Modificar Usuario" OnClick="btnGuardarModificar_Click" AutoPostBack="true">
                                                <ClientSideEvents Click="OnClickCrearUsuario" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>

            </ContentCollection>
        </dx:ASPxPopupControl>
    </div>
    <%--<%---------------------------------------------------PAGINA SOLICITUDES-<%----------------------------------------------------%>

</asp:Content>
