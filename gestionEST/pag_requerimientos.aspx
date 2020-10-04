﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_requerimientos.aspx.vb" Inherits="gestionEST.pag_requerimientos" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Barra_Azul" runat="server">

        <style type="text/css">
        .txtbox {
            -webkit-border-radius: 50px;
            -moz-border-radius: 50px;
            border-radius: 50px;
        }

        .uploadContainer {
            float: left;
            margin-right: 80px;
        }

        .contentFooter {
            clear: both;
            padding-top: 20px;
        }

        .imagenSplitter {
            background-image: url("images/31.png");
            background-repeat: no-repeat;
            display: inline-block;
            width: 35px;
            height: 34px;
        }

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

    <table>
        <tr>
            <td>
                <label class="text-white font-weight-bold" style="font-family:Verdana; padding-bottom:10px">
                    Historial de Requerimientos
                </label>
            </td>
        </tr>
        <tr>
            <td class="d-flex">
                <div style="width: 20%;">
                    <label class="text-white font-weight-bold" style="font-family:Verdana">
                        Planta 
                    </label>
                </div>
                <div class="d-flex justify-content-end">
                    <dx:ASPxComboBox Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" AllowNull="true" Theme="MetropolisBlue" AutoPostBack="false" CssClass="rounded" Width="82%" ID="cbxUnidad" runat="server" ValueType="System.String" ClientInstanceName="cbxUnidad" >
                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                    </dx:ASPxComboBox>
                </div>
            </td>
        </tr>
        <tr>
            <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px">
                <dx:ASPxButton ID="btnConsultar" EnableTheming="false"  CssClass="estiloBtnBuscar" Style="background-color: #ff1049; width: 60%;" Font-Size="Small" Text="Buscar" runat="server" AutoPostBack="False" OnClick="btnConsultar_Click">
                </dx:ASPxButton>
            </td>
        </tr>
        
    </table>
    <hr style="background-color:red" />
    <table>
        <tr id="tr_popupSolicitud" runat="server" visible="false"> 
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage4" runat="server" ShowLoadingImage="true" ImageUrl="images/05.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnpopUpSolicitud" ImagePosition="Left" EnableTheming="false" Font-Names="verdana" CssClass="estiloBtnSubmenusExportar" Style="background-color: #00489e; width: 100%;" Text="Cargar Archivos" Paddings-Padding="3px" runat="server" Visible="true" OnClick="btnpopUp_Click" AutoPostBack="False" UseSubmitBehavior="false" >
<%--                    <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />--%>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr id="tr_excel" runat="server" visible="false">
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage2" runat="server" ShowLoadingImage="true" ImageUrl="images/16.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:aspxbutton ID="btnExcel" runat="server" ImagePosition="Left" EnableTheming="false" Paddings-Padding="3px"
                    CssClass="estiloBtnSubmenusExportar" 
                    Style="background-color: #00489e; width: 100%;" Text="Exportar a Excel" 
                    OnClick="btnExcel_Click" AutoPostBack="False" >
                </dx:aspxbutton>
            </td>
        </tr>
    </table>

    

        
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContenidoPagina" runat="server">

    <script type="text/javascript">
       
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

        function requerido(s, e) {
            var nombreReferido = <%= lblNombreReferido.ClientID%>;
            var UploadControl = <%= td_UploadControl.ClientID%>;
            var td_NombreReferido = <% = td_NombreReferido.ClientID%>;
            var td_txtArchivoCargado = <%= td_txtArchivoCargado.ClientID%>;
          <%--var UploadControl = "<%= UploadControl.ClientInstanceName %>";--%>

            if (s.GetChecked() == true) {
                nombreReferido.style.visibility = "visible";
                UploadControl.style.visibility = "visible";
                td_NombreReferido.style.visibility = "visible";
                td_txtArchivoCargado.style.visibility = "visible";
            }
            else {
                    nombreReferido.style.visibility = 'hidden';
                    UploadControl.style.visibility = 'hidden';
                    td_NombreReferido.style.visibility = 'hidden';
                    td_txtArchivoCargado.style.visibility = "hidden";
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

    </script>

    <div id="div_solicitudes" runat="server" visible="false">
        <label style="color: #00489e; font-family:Verdana; font-size:large">
            <b>REQUERIMIENTOS</b>
        </label>
        <hr style="background-color: red;" />
        <%--GRIDVIEW--%>
        <dx:ASPxCallback runat="server" ID="cbFocusedRow" ClientInstanceName="cbFocusedRow" >
            
        </dx:ASPxCallback>
        <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridSolicitudes"
            runat="server" ClientInstanceName="gridSolicitudes" Paddings-Padding="0" 
            AutoGenerateColumns="False" Theme="Default" KeyFieldName="idSolicitud" OnPageIndexChanged="gridSolicitudes_PageIndexChanged" OnBeforeGetCallbackResult="gridSolicitudes_BeforeGetCallbackResult"> 
            <Settings GridLines="Vertical" HorizontalScrollBarMode="Auto" />
            <SettingsSearchPanel Visible="true" />
            <SettingsLoadingPanel Mode="Default" />
            <Styles>

                <Header Font-Bold="true" Font-Size="Small" ForeColor="#00489e" BackColor="#F2F2F2" HorizontalAlign="Center"></Header>
                <Cell Font-Size="Small" HorizontalAlign="Center"></Cell>
                <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#EAEAEA"></FocusedRow>
                <FixedColumn ForeColor="#00489E"></FixedColumn>
                <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
            </Styles>
            <SettingsBehavior EnableRowHotTrack="true" AllowFocusedRow="true" AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" AllowSort="False" />
            <Columns>
                <dx:GridViewDataColumn Caption="Código" Name="idSolicitud" FieldName="idSolicitud">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Número <br/> Solicitud" Name="numeroSolicitudes" FieldName="numeroSolicitudes">
                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Empresa <br/> Usuaria" Name="empUsuaria" FieldName="empUsuaria" Visible="False">
                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Cantidad<br/>EST<br/>Requerido" Name="cantRequerida" FieldName="cantRequerida" Visible="true">
                    <Settings AllowHeaderFilter="True" FilterMode="Value" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataDateColumn Caption="Fecha <br/> Creación <br/> Solicitud" Name="fechaCreacion" FieldName="fechaCreacion">
                    <Settings AllowHeaderFilter="True" FilterMode="Value" />
                </dx:GridViewDataDateColumn>

                <dx:GridViewDataColumn Caption="Unidad" Name="idUnidad" FieldName="idUnidad">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Planta" Name="planta" FieldName="planta">
                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Centro <br/> Costo" Name="centroCosto" FieldName="centroCosto">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Area" Name="area" FieldName="area">
                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Cargo" Name="cargo" FieldName="cargo">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Causal" Name="causal" FieldName="causal">
                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Motivo <br/> Causal" Name="motivoCausal" FieldName="motivoCausal">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Fecha <br/> Inicio" Name="fechaInicio" FieldName="fechaInicio">
                    <Settings AllowHeaderFilter="True" FilterMode="Value" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Días" Name="dias" FieldName="dias">
                    <Settings AllowHeaderFilter="True" FilterMode="Value" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Fecha <br/> Término" Name="fechaTermino" FieldName="fechaTermino">
                    <Settings AllowHeaderFilter="True" FilterMode="Value" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Sueldo" Name="sueldo" FieldName="sueldo">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Jornada" Name="turno" FieldName="turno">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Jefe <br/> a Cargo" Name="jefe" FieldName="jefe">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Referido" Name="referido" FieldName="referido">
                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Datos <br/> Referido" Name="datosReferido" FieldName="datosReferido">
                </dx:GridViewDataColumn>

                <dx:GridViewDataColumn Caption="Archivo <br/> Referido" Name="archivo" FieldName="archivo">
                    <DataItemTemplate>
                        <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="SIN ARCHIVO" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                    </DataItemTemplate>
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn Caption="Editar" VisibleIndex="20">
                    <DataItemTemplate>
                        <dx:ASPxButton ID="btnEditarDocumento" EnableTheming="false" Font-Size="Small" CssClass="btnGridSinFondo" runat="server" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnEditarDocumento_Click" OnLoad="btnEditarDocumento_Load">
                            <Image ToolTip="Editar" Url="images/39.png">
                            </Image>
                        </dx:ASPxButton>
                    </DataItemTemplate>
                </dx:GridViewDataColumn>

            </Columns>
        </dx:ASPxGridView>
        
        

        <%--EXPORTAR A EXCEL--%>

        <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="gridSolicitudes" >
            <Styles>
                <Default Font-Names="Arial" Font-Size="Medium" HorizontalAlign="Center">
                </Default>
                 <Header Font-Names="Arial" Font-Size="Medium" HorizontalAlign="Center">
                </Header>
            </Styles>
        </dx:ASPxGridViewExporter>

        <%--POPUP CREAR SOLICITUD--%>
        <dx:ASPxCallbackPanel runat="server">
            <PanelCollection>
                <dx:PanelContent>
                    <dx:ASPxHiddenField runat="server" ID="HiddenField" ClientInstanceName="HiddenField"></dx:ASPxHiddenField>
                    <dx:ASPxPopupControl ID="popUpCrearSolicitud" runat="server" CloseAction="CloseButton" ClientInstanceName="popUpCrearSolicitud" EnableViewState="False"
                        Modal="True" Width="650" HeaderText="CREAR SOLICITUD" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
                        <ClientSideEvents PopUp="function(s, e) {  cbxPlanta.Focus(); }" />
                        <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                        <ContentCollection>
                            <dx:PopupControlContentControl runat="server">
                                <script type="text/javascript">
                                    function cargaMasiva(s, e) {
                                        var td_CargaMasiva = <%= td_CargaMasiva.ClientID%>;
                                        var spnCargaMasiva = spncargamasiva;
                                        if (s.GetChecked() == true) {
                                            td_CargaMasiva.style.visibility = 'visible';
                                        }
                                        else {
                                            spnCargaMasiva.SetValue(1);
                                            td_CargaMasiva.style.visibility = 'hidden';
                                        }
                                    }

                                    function LlenarCentroCosto(s,e) {
                                        cbpCentroCosto.PerformCallback();
                                    }

                                </script>
                                <dx:ASPxPanel ID="PanelCargaArchivo" runat="server">
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">
                                            <table style="width: 100%; color: #00489e" border="0">
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Planta
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxComboBox Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" AllowNull="true" AutoPostBack="false" Width="100%" ID="cbxPlanta" runat="server" ValueType="System.String" ClientInstanceName="cbxPlanta">
                                                            <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                            <ClientSideEvents SelectedIndexChanged="LlenarCentroCosto" />
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Seleccione una Planta" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 50%">
                                                        <label>
                                                            Centro de Costo
                                                        </label>
                                                    </td>
                                                    <td style="width: 100%; padding-left: 5%" colspan="2">
                                                        <label>
                                                            Area
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxCallbackPanel runat="server" ID="cbpCentroCosto" ClientInstanceName="cbpCentroCosto" OnCallback="cbpCentroCosto_Callback">
                                                            <PanelCollection>
                                                                <dx:PanelContent>
                                                                    <dx:ASPxComboBox Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" AllowNull="true" AutoPostBack="false" Width="100%" ID="cbxCentroCosto" runat="server" ValueType="System.String" ClientInstanceName="cbxCentroCosto">
                                                                        <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                        <ValidationSettings
                                                                            EnableCustomValidation="true"
                                                                            ValidationGroup="entryGroup"
                                                                            SetFocusOnError="true"
                                                                            ErrorDisplayMode="Text"
                                                                            ErrorTextPosition="Bottom"
                                                                            CausesValidation="true">
                                                                            <RequiredField ErrorText="Seleccione un Centro de Costo" IsRequired="true" />
                                                                            <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                                <ErrorTextPaddings PaddingLeft="0px" />
                                                                            </ErrorFrameStyle>
                                                                        </ValidationSettings>
                                                                    </dx:ASPxComboBox>
                                                                </dx:PanelContent>
                                                            </PanelCollection>
                                                        </dx:ASPxCallbackPanel>
                                                    </td>
                                                    <td style="width: 100%; padding-left: 5%" colspan="2">
                                                        <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" runat="server" ID="txtArea" Width="100%">
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Ingrese un Area de trabajo" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Cargo
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" runat="server" ID="txtCargo" Width="100%">
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Ingrese un Cargo" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Causal
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxComboBox Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" runat="server" ID="cbxCausal" Width="100%">
                                                            <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Seleccione una Causal" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Motivo Causal
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxMemo ForeColor="#00489e" CssClass="rounded" ID="txtMotivoCausal" runat="server" Height="50px" Width="100%">
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Ingrese un Motivo Causal" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxMemo>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <label>
                                                            Ingreso Masivo
                                                        </label>
                                                    </td>
                                                    <td style="width: 100%; padding-left: 5%" colspan="2">
                                                        <label>
                                                            Fecha Desde
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <dx:ASPxCheckBox runat="server" ID="chkCargaMasiva" CssClass="rounded" Theme="MetropolisBlue" ClientInstanceName="chkCargaMasiva">
                                                                        <ClientSideEvents CheckedChanged="cargaMasiva" />
                                                                        <ValidationSettings
                                                                            EnableCustomValidation="true"
                                                                            ValidationGroup="entryGroup"
                                                                            SetFocusOnError="true"
                                                                            ErrorDisplayMode="Text"
                                                                            ErrorTextPosition="Bottom"
                                                                            CausesValidation="False">

                                                                            <RequiredField ErrorText="Seleccione una Cantidad de Trabajadores" IsRequired="False" />
                                                                            <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                                <ErrorTextPaddings PaddingLeft="0px" />
                                                                            </ErrorFrameStyle>
                                                                        </ValidationSettings>
                                                                    </dx:ASPxCheckBox>
                                                                </td>
                                                                <td colspan="1" runat="server" id="td_CargaMasiva" style="width: 100%; visibility: hidden">
                                                                    <dx:ASPxSpinEdit Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" ID="spnCargaMasiva" runat="server" ClientInstanceName="spncargamasiva" Number="1" MinValue="1" MaxValue="300" NumberType="Integer" Width="100%">

                                                                        <ValidationSettings
                                                                            EnableCustomValidation="true"
                                                                            ValidationGroup="entryGroup"
                                                                            SetFocusOnError="true"
                                                                            ErrorDisplayMode="Text"
                                                                            ErrorTextPosition="Bottom"
                                                                            CausesValidation="False">

                                                                            <RequiredField ErrorText="Seleccione una Cantidad de Trabajadores" IsRequired="False" />
                                                                            <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                                <ErrorTextPaddings PaddingLeft="0px" />
                                                                            </ErrorFrameStyle>
                                                                        </ValidationSettings>
                                                                    </dx:ASPxSpinEdit>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 100%; padding-left: 5%" colspan="2">
                                                        <dx:ASPxDateEdit Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" ID="dtFechaDesde" ClientInstanceName="dtFechaDesde" runat="server" Width="100%">
                                                            <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Seleccione una Fecha de Inicio" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                            <ClientSideEvents ValueChanged="jsCalculaDias" />
                                                        </dx:ASPxDateEdit>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <label>
                                                            Días
                                                        </label>
                                                    </td>
                                                    <td style="width: 100%; padding-left: 5%" colspan="2">
                                                        <label>
                                                            Fecha Hasta
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxSpinEdit Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" ID="txtDias" runat="server" ClientInstanceName="txtDias" Number="1" MinValue="1" MaxValue="300" NumberType="Integer" Width="100%">

                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="False">

                                                                <RequiredField ErrorText="Seleccione una Fecha de Inicio" IsRequired="False" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                            <ClientSideEvents ValueChanged="jsCalculaFechaFin" />
                                                        </dx:ASPxSpinEdit>
                                                    </td>
                                                    <td style="width: 100%; padding-left: 5%" colspan="2">
                                                        <dx:ASPxDateEdit Theme="MetropolisBlue" CssClass="rounded" Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" ID="dtFechaHasta" ClientInstanceName="dtFechaHasta" runat="server" Width="100%">
                                                            <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Seleccione una Fecha de Término" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                            <ClientSideEvents ValueChanged="jsCalculaDias" />
                                                        </dx:ASPxDateEdit>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Sueldo Base
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" runat="server" ID="txtSuedoBase" Width="100%" NullText="$">
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">
                                                                <RequiredField ErrorText="Ingrese un Sueldo Base" IsRequired="true" />
                                                                <RegularExpression ValidationExpression="^[0-9.,]+$" ErrorText="Sueldo solo puede contener números, puntos y/o comas" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Jornada
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxMemo ForeColor="#00489e" CssClass="rounded" ID="txtTurno" runat="server" Height="50px" Width="100%">
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Ingrese una Jornada" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxMemo>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <label>
                                                            Nombre Jefe a Cargo
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" runat="server" ID="txtJefeACargo" Width="100%">
                                                            <ValidationSettings
                                                                EnableCustomValidation="true"
                                                                ValidationGroup="entryGroup"
                                                                SetFocusOnError="true"
                                                                ErrorDisplayMode="Text"
                                                                ErrorTextPosition="Bottom"
                                                                CausesValidation="true">

                                                                <RequiredField ErrorText="Ingrese un Nombre de Jefe" IsRequired="true" />
                                                                <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: center; width: 10%">
                                                        <label>
                                                            Referido / Documento Respaldo
                                                        </label>
                                                    </td>
                                                    <td colspan="2" style="width: 90%">
                                                        <label runat="server" id="lblNombreReferido" style="visibility: hidden">
                                                            Nombre / Contacto Referido / Documento Respaldo
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: center; width: 10%">
                                                        <dx:ASPxCheckBox runat="server" ID="chkReferido" CssClass="rounded" Theme="MetropolisBlue" ClientInstanceName="chkReferido">
                                                            <ClientSideEvents CheckedChanged="requerido" />
                                                        </dx:ASPxCheckBox>
                                                    </td>
                                                    <td colspan="1" id="td_NombreReferido" runat="server" style="width: 100%; visibility: hidden">
                                                        <dx:ASPxTextBox ForeColor="#00489e" CssClass="rounded" runat="server" ID="txtNombreReferido" Width="100%">
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                    <td id="td_UploadControl" runat="server" style="visibility: hidden; text-align: right">
                                                        <dx:ASPxUploadControl ID="UploadControl" runat="server" ClientInstanceName="UploadControl" ShowTextBox="false"
                                                            NullText="Selecciona Archivo" FileUploadMode="OnPageLoad" UploadMode="Advanced" ShowProgressPanel="True" AutoStartUpload="true"
                                                            OnFileUploadComplete="UploadControl_FileUploadComplete" Theme="MetropolisBlue" EnableTheming="True" CssClass="rounded txtbox text-white font-weight-bold">
                                                            <BrowseButton Text="Adjuntar Archivo"></BrowseButton>
                                                            <BrowseButtonStyle BackColor="#ff1049" ForeColor="White" CssClass="rounded txtbox text-white font-weight-bold">
                                                            </BrowseButtonStyle>
                                                            <AdvancedModeSettings EnableMultiSelect="false" EnableFileList="True" EnableDragAndDrop="True" />
                                                            <ValidationSettings MaxFileSize="4194304" AllowedFileExtensions=".jpg, .png, .pdf, .xlsx, .xlsm, .xls, .msg">
                                                            </ValidationSettings>
                                                            <ClientSideEvents FileUploadComplete="function (s, e) {
                                                        var text = s.GetText(e.inputIndex).replace(/\s|C:\\fakepath\\/g, '');
                                                        txtArchivoCargado.SetText('¡ARCHIVO ' + text + ' CARGADO!'); }" />
                                                        </dx:ASPxUploadControl>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td colspan="3" id="td_txtArchivoCargado" runat="server" style="text-align: right; visibility: hidden;">
                                                        <dx:ASPxTextBox ForeColor="#00489e" HorizontalAlign="Right" Font-Bold="true" ID="txtArchivoCargado" ReadOnly="true" runat="server" Width="100%" ClientInstanceName="txtArchivoCargado" Text="Falta Cargar Archivo">
                                                            <Border BorderStyle="None" />
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="text-align: right">
                                                        <dx:ASPxButton runat="server" ID="btnCrearSolicitud" CssClass="estiloBtnBuscar" Text="Crear Solicitud" OnClick="btnCrearSolicitud_Click">
                                                            <ClientSideEvents Click="OnClickGrabaCambio" />
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
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
        
    </div>
</asp:Content>
