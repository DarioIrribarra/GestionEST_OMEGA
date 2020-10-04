<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_creacionDocumentos.aspx.vb" Inherits="gestionEST.pag_creacionDocumentos" %>

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
                border: 1px solid lightgrey;
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

    <script>
        
        
        function AbrirPopUpCreaDocumento(s, e) {
            lpCallBack.Show();
            //Limpia validaciones
            ASPxClientEdit.ClearGroup('entryGroup');
            //LE PASO VALOR 1 ALL CALLBACK PARA USARLO COMO IDENTIFICADOR DE INGRESAR
            cbpCreaDocumento.PerformCallback(0);
        }

        function validaLargoId() {
            var validadorLargoId = false;
            var texto = txtId.GetText().trim();
            var cuentaEspacios = (texto.split(" ").length);
            if (cuentaEspacios > 1 || texto.length < 6) {
                validadorLargoId = false;
            } else {
                validadorLargoId = true;
            }
            return validadorLargoId;
        }

        function validaLargoNombreEnAuditoria() {
            var validadorLargoNombreEnAuditoria = false;
            var texto = txtNombreEnAuditoria.GetText().trim();
            var cuentaEspacios = (texto.split(" ").length);
            if (cuentaEspacios > 3) {
                 validadorLargoNombreEnAuditoria = false;
            } else {
                validadorLargoNombreEnAuditoria = true;
            }
            //alert(cuentaEspacios);
            //alert(texto);
            return validadorLargoNombreEnAuditoria;
        }

        function Validaciones(s, e) {
            var validado = false;
            //VALIDO LARGO DEL ID

            if (ASPxClientEdit.ValidateGroup('entryGroup') == true) {
                if (validaLargoId()) {
                    if (validaLargoNombreEnAuditoria()) {
                        validado = true;
                    } else {
                        alert("ERROR: NOMBRE EN AUDITORÍA NO PUEDE TENER MÁS DE 3 PALABRAS")
                        txtNombreEnAuditoria.Focus();
                        validado = false;
                    }
                }
                else {
                    alert("ERROR: ID DEBE TENER UN LARGO DE 6 CARACTERES SIN ESPACIOS")
                    txtId.Focus();
                    validado = false;
                }
                //validado = true;
            } 
            if (validado == true) {
                e.processOnServer = true;
            } else {
                e.processOnServer = false;
            }
        }

        //function NombreEnAuditoria(s, e) {
        //    validadorLargoNombreEnAuditoria = false;
        //    var texto = s.GetText().trim();
        //    var cuentaEspacios = (texto.split(" ").length);
        //    if (cuentaEspacios > 3) {
        //        alert("ERROR: NOMBRE EN AUDITORÍA NO PUEDE TENER MÁS DE 3 PALABRAS")
        //        txtNombreEnAuditoria.Focus();
        //    } else {
        //        validadorLargoNombreEnAuditoria = true;
        //    }
        //    //console.log(cuentaEspacios)            
        //}

        //function ValidaLargoId(s, e) { 
        //    validadorLargoId = false;
        //    var texto = s.GetText().trim();
        //    var cuentaEspacios = (texto.split(" ").length);
        //    if (cuentaEspacios > 1 || texto.length < 6) {
        //        alert("ERROR: ID DEBE TENER UN LARGO DE 6 CARACTERES SIN ESPACIOS")
        //        txtId.Focus();
        //    } else {
        //        validadorLargoId = true;
        //    }
        //}

        // EL ARREGLO "values" CONTIENE LOS DATOS DE RUT Y NOMBRE
        function OnGetRowValues(values) {
            alert('ID: ' + values.toString());
        }

        function EditarTipoDocumento(s, e) {
            //var grid = gridCrearDocumentos;
            //var dato = grid.GetRowValues(grid.GetFocusedRowIndex(), 'ID', OnGetRowValues);
            //alert(dato);
            lpCallBack.Show();
            //Limpia validaciones
            //ASPxClientEdit.ClearGroup('entryGroup');
            //LE PASO VALOR 1 AL CALLBACK PARA INDICAR EDICIÓN
            cbpCreaDocumento.PerformCallback(1);
        }

        function Mensaje(s, e) {
            var result = confirm('¡¿Seguro que desea borrar el Archivo Seleccionado?!\nTODAS LAS COLUMNAS EN BASE DE DATOS SE ELIMINARÁN');
            //console.log("CONFIRM: " + result);
            e.processOnServer = result;
        }

    </script>
    
    <table id="tb_BotonesCargar" runat="server">
        <tr>
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage1" runat="server" ShowLoadingImage="true" ImageUrl="images/02.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnSubeArchivo" ImagePosition="Left" EnableTheming="false" CssClass="estiloBtnSubmenusExportar" Text="Crear Documento"  runat="server" Visible="true" AutoPostBack="False" UseSubmitBehavior="false" >
                    <ClientSideEvents Click="AbrirPopUpCreaDocumento" />
                </dx:ASPxButton>
            </td>

        </tr>
        <tr>
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage2" runat="server" ShowLoadingImage="true" ImageUrl="images/16.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnDocumentos" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" Text="Modificar"  runat="server" Visible="true" AutoPostBack="False" UseSubmitBehavior="false" >
                    <ClientSideEvents Click="EditarTipoDocumento" />
                </dx:ASPxButton>
            </td>
        </tr>
       <%-- <tr style="visibility:hidden">
            <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px">
                <dx:ASPxButton ID="btnVerContrato" EnableTheming="false" CssClass="rounded txtbox text-white font-weight-bold" Style="background-color: #ff1049; width: 100%;" Font-Size="Small" Text="Ver Contrato" runat="server" Visible="true" OnClick="btnVerContrato_Click" AutoPostBack="False" UseSubmitBehavior="false">
                    <Image ToolTip="Ver Contrato" Url="images/verArchivo.png">
                    </Image>
                    <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />
                </dx:ASPxButton>
            </td>
        </tr>--%>
    </table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContenidoPagina" runat="server">
    <label style="color: #00489e; font-family: Verdana; font-size: large;">
        <b>TIPOS DE DOCUMENTOS</b>
    </label>
    <hr style="background-color: red;" />

    <%--GIF CARGANDO--%>
    <dx:ASPxLoadingPanel runat="server" ID="lpCallBack" Text="Cargando..." ClientInstanceName="lpCallBack" Modal="true"  Theme="MetropolisBlue"></dx:ASPxLoadingPanel>

    <dx:ASPxGridView
        ID="gridCrearDocumentos"
        ClientInstanceName="gridCrearDocumentos"
        runat="server"
        ForeColor="#00489e"
        Width="100%"
        KeyFieldName="id"
        OnInit="gridCrearDocumentos_Init"
        OnLoad="gridCrearDocumentos_Load"
        >

        <Settings GridLines="Vertical"/>
        <SettingsPager PageSize="20" />
        <SettingsSearchPanel Visible="true" />
        <SettingsLoadingPanel Mode="Default" />
        <SettingsBehavior
            AllowSort="true"
            EnableRowHotTrack="true"
            AllowFocusedRow="True"
            AllowSelectByRowClick="false"
            AllowSelectSingleRowOnly="true" />
        <Styles>

            <Header Font-Bold="true" Font-Size="11px" ForeColor="#00489e" BackColor="#F2F2F2" HorizontalAlign="Center"></Header>
            <Cell Font-Size="11px"></Cell>
            <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#EAEAEA"></FocusedRow>
            <FixedColumn ForeColor="#00489E"></FixedColumn>
            <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
        </Styles>
        <ClientSideEvents RowDblClick="EditarTipoDocumento" />

    </dx:ASPxGridView>

    <%--POP UP CREA DOCUMENTO--%>
    <table>
        <tr>
            <td>
                <dx:ASPxHiddenField runat="server" ID="HiddenField" ClientInstanceName="HiddenField"></dx:ASPxHiddenField>
                <dx:ASPxCallbackPanel runat="server" ID="cbpCreaDocumento" ClientInstanceName="cbpCreaDocumento" OnCallback="cbpCreaDocumento_Callback">
                    <ClientSideEvents EndCallback="function(s,e) {popUpCreaDocumento.Show(); lpCallBack.Hide();}" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxPopupControl ID="popUpCreaDocumento" runat="server" CloseAction="CloseButton" ClientInstanceName="popUpCreaDocumento" EnableViewState="False"
                                Modal="True" Width="600px" HeaderText="Crear Nuevo Documento" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
                                <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                                <ContentCollection>
                                    <dx:PopupControlContentControl runat="server">
                                        <dx:ASPxPanel ID="panelCreaDocumento" runat="server">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">
                                                    <dx:ASPxFormLayout runat="server" ID="ASPxFormLayout1" Width="100%" Height="100%">
                                                        <Items>
                                                            <%--ID--%>
                                                            <dx:LayoutItem Caption="ID DOCUMENTO">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana"/>
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxTextBox ID="txtId" runat="server" Width="100%" ClientInstanceName="txtId" 
                                                                            ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" MaxLength="6">

                                                                            <ValidationSettings EnableCustomValidation="True" ValidationGroup="entryGroup" SetFocusOnError="True"
                                                                                ErrorDisplayMode="Text" ErrorTextPosition="Bottom" CausesValidation="True">
                                                                                <RequiredField ErrorText="ID Requerido" IsRequired="True" />
                                                                                <ErrorFrameStyle Font-Size="10px">
                                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                                </ErrorFrameStyle>
                                                                            </ValidationSettings>
                                                                            <%--<ClientSideEvents TextChanged="ValidaLargoId"/>--%>
                                                                        </dx:ASPxTextBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <%--DESCRIPCIÓN--%>
                                                            <dx:LayoutItem Caption="DESCRIPCIÓN DOCUMENTO">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana"/>
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxTextBox ID="txtDescripcion" runat="server" Width="100%" ClientInstanceName="txtDescripcion" 
                                                                            ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" MaxLength="50">

                                                                            <ValidationSettings EnableCustomValidation="True" ValidationGroup="entryGroup" SetFocusOnError="True"
                                                                                ErrorDisplayMode="Text" ErrorTextPosition="Bottom" CausesValidation="True">
                                                                                <RequiredField ErrorText="Descripción Requerida" IsRequired="True" />
                                                                                <ErrorFrameStyle Font-Size="10px">
                                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                                </ErrorFrameStyle>
                                                                            </ValidationSettings>

                                                                        </dx:ASPxTextBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <%--NOMBRE EN MENU COLABORADORES--%>
                                                            <dx:LayoutItem Caption="NOMBRE EN MENU COLABORADORES">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" />
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxTextBox ID="txtNombreEnMenuColaboradores" runat="server" Width="100%" ClientInstanceName="txtNombreEnMenuColaboradores"
                                                                            ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" MaxLength="20">

                                                                            <ValidationSettings EnableCustomValidation="True" ValidationGroup="entryGroup" SetFocusOnError="True"
                                                                                ErrorDisplayMode="Text" ErrorTextPosition="Bottom" CausesValidation="True">
                                                                                <RequiredField ErrorText="Nombre en Menú Requerido" IsRequired="True" />
                                                                                <ErrorFrameStyle Font-Size="10px">
                                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                                </ErrorFrameStyle>
                                                                            </ValidationSettings>

                                                                        </dx:ASPxTextBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <%--NOMBRE EN AUDITORÍA--%>
                                                            <dx:LayoutItem Caption="NOMBRE EN AUDITORÍA">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" />
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxTextBox ID="txtNombreEnAuditoria" runat="server" Width="100%" ClientInstanceName="txtNombreEnAuditoria"
                                                                            ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" MaxLength="25">

                                                                            <ValidationSettings EnableCustomValidation="True" ValidationGroup="entryGroup" SetFocusOnError="True"
                                                                                ErrorDisplayMode="Text" ErrorTextPosition="Bottom" CausesValidation="True">
                                                                                <RequiredField ErrorText="Nombre en Auditoría Requerido" IsRequired="True" />
                                                                                <ErrorFrameStyle Font-Size="10px">
                                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                                </ErrorFrameStyle>
                                                                            </ValidationSettings>
                                                                            <%--<ClientSideEvents TextChanged="NombreEnAuditoria"/>--%>
                                                                        </dx:ASPxTextBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <%--CONTROLA VENCIMIENTO--%>
                                                            <dx:LayoutItem Caption="CONTROLA VENCIMIENTO">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" />
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxCheckBox ID="cbVencimiento" runat="server" ClientInstanceName="cbVencimiento" Theme="MetropolisBlue">
                                                                        </dx:ASPxCheckBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>

                                                            <%--ACTIVO--%>
                                                            <dx:LayoutItem Caption="ACTIVADO DOCUMENTO EN COLABORADORES">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" />
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxCheckBox ID="cbActivo" runat="server" ClientInstanceName="cbActivo" Theme="MetropolisBlue">
                                                                        </dx:ASPxCheckBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>

                                                            <%--VISIBLE EN MENÚ COLABORADORES--%>
                                                            <dx:LayoutItem Caption="VISIBLE EN MENÚ COLABORADORES">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" />
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxCheckBox ID="cbActivoEnMenuColaboradores" runat="server" ClientInstanceName="cbActivoEnMenuColaboradores" Theme="MetropolisBlue">
                                                                        </dx:ASPxCheckBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>

                                                            <%--ACTIVO EN AUDITORÍA--%>
                                                            <dx:LayoutItem Caption="VISIBLE EN AUDITORÍA">
                                                                <CaptionStyle CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" />
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxCheckBox ID="cbActivoEnAuditoria" runat="server" ClientInstanceName="cbActivoEnAuditoria" Theme="MetropolisBlue">
                                                                        </dx:ASPxCheckBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>

                                                            <%--BOTONES--%>
                                                            <dx:LayoutItem ShowCaption="False" Paddings-PaddingLeft="180px">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer>
                                                                        <dx:ASPxButton Font-Names="verdana" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" ID="btnGrabarArchivo" runat="server" Text="Guardar" OnClick="btnGrabarArchivo_Click" AutoPostBack="false" UseSubmitBehavior="false">
                                                                            <ClientSideEvents Click="Validaciones" />
                                                                        </dx:ASPxButton>
                                                                        <dx:ASPxLabel ID="lblEspacioBotones" runat="server" Width="50px"></dx:ASPxLabel>
                                                                        <dx:ASPxButton Font-Names="verdana" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" ID="btnCerrar" runat="server" Text="Cerrar" AutoPostBack="false" UseSubmitBehavior="false">
                                                                            <ClientSideEvents Click="function(s, e) { popUpCreaDocumento.Hide();}" />
                                                                        </dx:ASPxButton>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:ASPxFormLayout>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PopupControlContentControl>
                                </ContentCollection>
                            </dx:ASPxPopupControl>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </td>
        </tr>
    </table>
</asp:Content>
