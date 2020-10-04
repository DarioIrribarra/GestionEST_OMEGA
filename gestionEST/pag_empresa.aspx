﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_empresa.aspx.vb" Inherits="gestionEST.pag_empresa" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>

<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="Barra_Azul">
    <script type="text/javascript">

        function OnClickConsultaEmpresa(s, e) {
            if (ASPxClientEdit.ValidateGroup('entryGroup')) {
                e.processOnServer = true;
            }
            else {
                e.processOnServer = false;
            }
        }

        function OnInit(s, e) {
            AdjustSize();
            document.getElementById("gridContainer").style.visibility = "";
        }

        function gridEmpresa_OnCustomButtonClick(s, e) {

            if (e.buttonID == "btnBorrar") {
                popupBorrar.Show();
            }

        }

        function CambiarUnidadesPagina() {
            cbpPanelIzquierdo.PerformCallback();
        }

        function recargarGrid(s,e) {
            cbpEmpresa.PerformCallback();
        }

        function recargarLinkContrato(s,e) {
            cbpContrato.PerformCallback();
        }

    </script>
    <%--Barra AZUL--%>

    <dx:ASPxCallbackPanel runat="server" ID="cbpPanelIzquierdo" ClientInstanceName="cbpPanelIzquierdo" OnCallback="cbpPanelIzquierdo_Callback">
        <%--<ClientSideEvents EndCallback="recargarGrid" />--%>
        <PanelCollection>
            <dx:PanelContent>
                <table style="width: 155px" border="0">
                    <tr>
                        <td>
                            <label class="text-white font-weight-bold" style="font-family: Verdana; padding-bottom: 10px">
                                Documentación Legal
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="d-flex">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold" runat="server" id="lblEmpresa">
                                    Empresa 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" Width="77%" CssClass="rounded" ID="cbxEmpresaPagina" runat="server" ValueType="System.String" ClientInstanceName="cbxEmpresaPagina" Theme="MetropolisBlue">
                                    <ClientSideEvents SelectedIndexChanged="CambiarUnidadesPagina" />
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="d-flex">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold" style="font-family: Verdana">
                                    Planta 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox Font-Names="verdana" ForeColor="#00489e" Font-Bold="False" AllowNull="true" Theme="MetropolisBlue" AutoPostBack="false" CssClass="rounded" Width="77%" ID="cbxPlanta" runat="server" ValueType="System.String" ClientInstanceName="cbxPlanta" OnLoad="cbxPlanta_Load">
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="d-flex">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold" style="font-family: Verdana">
                                    Mes 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox Font-Names="verdana" ForeColor="#00489e" Theme="MetropolisBlue" Width="75%" CssClass="rounded" ID="txtMesEmpresa" runat="server" ValueType="System.String" ClientInstanceName="txtMesEmpresa">
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="d-flex ">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold" style="font-family: Verdana">
                                    Año 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox Font-Names="verdana" ForeColor="#00489e" Theme="MetropolisBlue" Width="75%" CssClass="rounded" ID="txtAnoEmpresa" runat="server" ValueType="System.String" ClientInstanceName="txtAnoEmpresa">
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px">
                            <dx:ASPxButton ID="btnConsultar" EnableTheming="false" CssClass="estiloBtnBuscar" Style="background-color: #ff1049; width: 60%;" Font-Size="Small" Text="Buscar" runat="server" AutoPostBack="False" OnClick="btnConsultar_Click">

                                <ClientSideEvents Click="OnClickConsultaEmpresa" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    
    <hr style="background-color:red" />
    <table id="tb_BotonesCargar" runat="server">

        <tr>
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage1" runat="server" ShowLoadingImage="true" ImageUrl="images/02.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnSubeArchivo" ImagePosition="Left" EnableTheming="false" Font-Names="verdana" CssClass="estiloBtnSubmenusExportar" Style="background-color: #00489e; width: 100%;" Text="Cargar Archivos" Paddings-Padding="3px" runat="server" Visible="true" OnClick="btnSubeArchivo_Click" AutoPostBack="False" UseSubmitBehavior="false" >
<%--                    <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />--%>
                </dx:ASPxButton>
            </td>
        </tr>

        <%--CAGAR CONTRATO A TRAVES DE LA PLATAFORMA--%>
        <tr style="visibility:visible">
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage2" runat="server" ShowLoadingImage="true" ImageUrl="images/02.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center" >
                <dx:ASPxButton ID="btnSubirContrato" ImagePosition="Left" EnableTheming="false" Font-Names="verdana" CssClass="estiloBtnSubmenusExportar" Style="background-color: #00489e; width: 100%;" Text="Cargar Contrato" Paddings-Padding="3px" runat="server" Visible="true" OnClick="btnSubirContrato_Click" AutoPostBack="False" UseSubmitBehavior="false" >
<%--                    <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />--%>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContenidoPagina" runat="server">

    <%--Contenido GRID--%>
    <div id="lblDefinicion" runat="server" style="color: #00489e">
        <label>
                <span>
                    <img src="images/15.png" />
                </span>: Con Documento
                <span>
                    <img src="images/12.png" />
                </span>: Sin Documento

        </label>
    </div>
    <dx:ASPxCallbackPanel runat="server" ID="cbpEmpresa" ClientInstanceName="cbpEmpresa" OnCallback="cbpEmpresa_Callback">
        <PanelCollection>
            <dx:PanelContent>
                <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridEmpresa"
                    runat="server" ClientInstanceName="gridEmpresa"
                    KeyFieldName="unidad" AutoGenerateColumns="False" Theme="Default">
                    <Settings GridLines="Vertical"
                        HorizontalScrollBarMode="Auto" />
                    <SettingsLoadingPanel Mode="Default" />
                    <Styles>
                        <Header Font-Bold="true" Font-Size="Small" ForeColor="#00489e" BackColor="#F2F2F2"></Header>
                        <Cell Font-Size="Small" Paddings-PaddingTop="12" Paddings-PaddingBottom="12"></Cell>
                        <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#EAEAEA"></FocusedRow>
                        <FixedColumn ForeColor="#00489E"></FixedColumn>
                        <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
                    </Styles>

                    <SettingsAdaptivity
                        AdaptiveColumnPosition="Left"
                        AdaptiveDetailLayoutProperties-AlignItemCaptionsInAllGroups="true"
                        AdaptivityMode="off" AllowOnlyOneAdaptiveDetailExpanded="false">
                    </SettingsAdaptivity>
                    <ClientSideEvents CustomButtonClick="gridEmpresa_OnCustomButtonClick" />
                    <SettingsPager Visible="False">
                    </SettingsPager>
                    <SettingsBehavior EnableRowHotTrack="true" AllowSelectByRowClick="false" AllowSelectSingleRowOnly="false" AllowSort="False" />

                    <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />


                    <Columns>

                        <dx:GridViewDataTextColumn Width="20%" Caption="Centro o Planta" Name="Centro" VisibleIndex="0" FieldName="centro">
                            <HeaderStyle HorizontalAlign="Center" />

                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="Contrato Marco" FieldName="contrato" VisibleIndex="1">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="Asistencia" FieldName="asiste" VisibleIndex="2" Visible="false">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="14%" Caption="Libro <br/> Remuneraciones" FieldName="librem" VisibleIndex="3">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="Cotizaciones" FieldName="cotiza" VisibleIndex="4">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="Liquidaciones" FieldName="sueldo" VisibleIndex="5">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="Finiquitos" FieldName="finpag" VisibleIndex="6">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="F-30</br>(IDT)" FieldName="form30" VisibleIndex="7">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="F-29</br>(PAGO IVA)" FieldName="form29" VisibleIndex="8">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>

                        <dx:GridViewDataHyperLinkColumn Width="11%" Caption="Deuda Fiscal" FieldName="cerdeu" VisibleIndex="9">
                            <HeaderStyle HorizontalAlign="Center" />
                            <DataItemTemplate>
                                <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                            </DataItemTemplate>
                            <CellStyle HorizontalAlign="Center">
                            </CellStyle>
                        </dx:GridViewDataHyperLinkColumn>


                        <dx:GridViewDataTextColumn Caption="unidad" Name="Centro" VisibleIndex="10" FieldName="unidad" Visible="false">
                            <HeaderStyle HorizontalAlign="Center" />

                        </dx:GridViewDataTextColumn>

                    </Columns>

                </dx:ASPxGridView>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

    <%--POP UP CARGA CONTRATO--%>
    <dx:ASPxCallbackPanel runat="server" ID="cbpContrato" ClientInstanceName="cbpContrato" OnCallback="cbpContrato_Callback" >
        <PanelCollection>
            <dx:PanelContent>
                <table>
                    <tr>
                        <td>
                            <dx:ASPxPopupControl Font-Names="Verdana" ID="popUpContrato" runat="server" CloseAction="CloseButton" ClientInstanceName="popUpContrato" EnableViewState="False"
                                Modal="True" Width="600px" HeaderText="Carga Contrato" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis"
                                ShowPageScrollbarWhenModal="True">
                                <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                                <ContentCollection>
                                    <dx:PopupControlContentControl runat="server">
                                        <dx:ASPxPanel ID="ASPxPanel1" runat="server">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">
                                                    <table border="0" style="width:100%">
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" runat="server" ID="lblEmpresaContrato" Text="Empresa">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:
                                                            </td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" Width="100%" CssClass="rounded" ID="cbxEmpresaContrato" runat="server" ValueType="System.String" ClientInstanceName="cbxEmpresaContrato" Theme="MetropolisBlue">
                                                                    <%--<ClientSideEvents SelectedIndexChanged="CambiarUnidadesPagina" />--%>
                                                                    <ClientSideEvents SelectedIndexChanged="recargarLinkContrato" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                </dx:ASPxComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" runat="server" ID="lblMesContrato" Text="Mes">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox Font-Names="verdana" ForeColor="#00489e" Theme="MetropolisBlue" Width="100%" CssClass="rounded" ID="cbxMesContrato" runat="server" ValueType="System.String" ClientInstanceName="cbxMesContrato">
                                                                    <ClientSideEvents SelectedIndexChanged="recargarLinkContrato" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                </dx:ASPxComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" runat="server" ID="lblAñoContrato" Text="Año">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox Font-Names="verdana" ForeColor="#00489e" Theme="MetropolisBlue" Width="100%" CssClass="rounded" ID="cbxAñoContrato" runat="server" ValueType="System.String" ClientInstanceName="cbxAñoContrato">
                                                                    <ClientSideEvents SelectedIndexChanged="recargarLinkContrato" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                </dx:ASPxComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" runat="server" ID="lblContrato" Text="Contrato"></dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxUploadControl ForeColor="#00489e" Font-Names="verdana" Theme="MetropolisBlue" EnableTheming="True" CssClass="rounded txtbox text-white font-weight-bold" ID="uploadControlContrato"
                                                                    runat="server" ClientInstanceName="uploadControlContrato" Width="100%"
                                                                    NullText="Selecciona Contrato" FileUploadMode="OnPageLoad" UploadMode="Advanced" ShowProgressPanel="True" AutoStartUpload="true"
                                                                    OnFileUploadComplete="uploadControlContrato_FileUploadComplete">
                                                                    <BrowseButtonStyle BackColor="#ff1049" ForeColor="White" CssClass="rounded txtbox text-white font-weight-bold">
                                                                    </BrowseButtonStyle>
                                                                    <AdvancedModeSettings EnableMultiSelect="false" EnableFileList="True" EnableDragAndDrop="True" />
                                                                    <ValidationSettings MaxFileSize="4194304" AllowedFileExtensions=".jpg, .png, .pdf, .xlsx, .xls, .xml, .xlsm">
                                                                    </ValidationSettings>
                                                                    <ClientSideEvents FileUploadComplete="function (s, e) {
                                                        var text = s.GetText(e.inputIndex).replace(/\s|C:\\fakepath\\/g, '');
                                                        txtContrato.SetText('¡ARCHIVO ' + text + ' CARGADO!');}" />
                                                                </dx:ASPxUploadControl>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3" style="text-align:center">
                                                                <dx:ASPxHyperLink runat="server" ID="hlVerArchivo" Text=""></dx:ASPxHyperLink>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3">
                                                                <dx:ASPxTextBox ForeColor="#00489e" Font-Bold="true" ID="txtContrato" ReadOnly="true" runat="server" Width="350px" ClientInstanceName="txtContrato" Text="Contrato Antiguo Cargado" CssClass="float-right">
                                                                    <Border BorderStyle="None" />
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3">
                                                                <%--BOTONES GUARDAR Y CERRAR--%>
                                                                <div class="pcmButton d-flex justify-content-center">
                                                                    <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px; background-color: #ff1049;" Width="80px" Height="30px" Text="Guardar" ID="btnGuardarContrato" runat="server" OnClick="btnGuardarContrato_Click" AutoPostBack="false" UseSubmitBehavior="false">

                                                                        <ClientSideEvents Click="OnClickConsultaEmpresa" />
                                                                    </dx:ASPxButton>
                                                                    <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px; background-color: #ff1049;" Width="80px" Height="30px" Text="Cerrar" ID="btnCancelarContrato" runat="server" AutoPostBack="false" UseSubmitBehavior="false">
                                                                        <ClientSideEvents Click="function(s, e) { popUpContrato.Hide();}" />
                                                                    </dx:ASPxButton>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxPanel>
                                    </dx:PopupControlContentControl>
                                </ContentCollection>
                            </dx:ASPxPopupControl>
                        </td>
                    </tr>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    
    
        <!-- POPUP CARGA ARCHIVOS-->
    <table>
        <tr>
            <td>
                <dx:ASPxHiddenField runat="server" ID="HiddenField" ClientInstanceName="HiddenField"></dx:ASPxHiddenField>
                <dx:ASPxPopupControl Font-Names="Verdana" ID="popupCargaArchivo" runat="server" CloseAction="CloseButton" ClientInstanceName="popupCargaArchivo" EnableViewState="False"
                    Modal="True" Width="600px" HeaderText="Carga Archivos" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis"
                    ShowPageScrollbarWhenModal="True">
                    <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                    <ClientSideEvents PopUp="function(s, e) {  txtTipoDocto.Focus(); }" />
                    <ContentCollection>
                        <dx:PopupControlContentControl runat="server">
                            <dx:ASPxPanel ID="PanelCargaArchivo" runat="server">
                                <PanelCollection>
                                    <dx:PanelContent runat="server">
                                        <table style="margin: 0px auto;" border="0">
                                            <tr>
                                                <td rowspan="4"></td>
                                                
                                                <td class="pcmCellCaption">
                                                    <%--NOMBRE UNIDAD--%>
                                                    <div style="width: 20%;">
                                                        <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="lblUnidad" runat="server" Text="Unidad">
                                                        </dx:ASPxLabel>
                                                    </div>
                                                </td>
                                                <td class="pcmCellCaption">:</td>

                                                <td class="pcmCellText">
                                                    
                                                    <dx:ASPxComboBox ValidateRequestMode="Enabled" ForeColor="#00489e" Font-Names="verdana" AllowNull="False" Theme="MetropolisBlue" AutoPostBack="false" CssClass="rounded" Width="350px" ID="cbxUnidadCarga" runat="server" ValueType="System.String" ClientInstanceName="cbxPlanta" OnSelectedIndexChanged="cbxUnidadCarga_SelectedIndexChanged">
                                                        <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                                                            ErrorTextPosition="Bottom" CausesValidation="true">
                                                            <RequiredField ErrorText="Debe Seleccionar Tipo Documento" IsRequired="true" />
                                                            <RegularExpression ErrorText="Faltan Datos" />
                                                            <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                <ErrorTextPaddings PaddingLeft="0px" />
                                                            </ErrorFrameStyle>

                                                        </ValidationSettings>
                                                        <ClientSideEvents SelectedIndexChanged="function (s,e){e.processOnServer = true;}" />
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td rowspan="4"></td>
                                            </tr>
                                            <tr>
                                                <td class="pcmCellCaption">
                                                    <%--FECHA--%>
                                                    <div style="width: 20%;">
                                                        <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="lblAmes" runat="server" Text="Fecha">
                                                        </dx:ASPxLabel>
                                                    </div>
                                                </td>
                                                <td class="pcmCellCaption">:</td>
                                                <td class="pcmCellText">
                                                    <dx:ASPxTextBox ForeColor="#00489e" Font-Names="verdana" Theme="MetropolisBlue" CssClass="rounded" ID="txtAnoMes" runat="server" Width="350px" ClientInstanceName="txtAnoMes" ReadOnly="true">
                                                        <Border BorderStyle="None" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="pcmCellCaption">
                                                    <%--ID DE UNIDAD--%>
                                                    <div style="width: 20%;">
                                                        <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="lblId" runat="server" Text="ID">
                                                        </dx:ASPxLabel>
                                                    </div>
                                                </td>
                                                <td>
                                                    <label>:</label>
                                                </td>
                                                <td class="pcmCellText">
                                                    <dx:ASPxTextBox ID="txtIdUnidad" CssClass="rounded" ForeColor="#00489e" Font-Names="verdana" Theme="MetropolisBlue"  runat="server" Width="350px" ClientInstanceName="txtIdUnidad" ReadOnly="true">
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="pcmCellCaption">
                                                    <%--TIPO DOCUMENTO--%>
                                                    <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel2" runat="server" Text="Tipo Docto">
                                                    </dx:ASPxLabel>
                                                </td>
                                                <td class="pcmCellCaption">:</td>
                                                <td class="pcmCellText">
                                                    
                                                    <dx:ASPxComboBox ForeColor="#00489e" Font-Names="verdana"  AllowNull="true" Theme="MetropolisBlue" AutoPostBack="false" CssClass="rounded" Width="350px" ID="txtTipoDocto" runat="server" ValueType="System.String" ClientInstanceName="txtTipoDocto">
                                                        <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                        <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                                                            ErrorTextPosition="Bottom" CausesValidation="true" RequiredField-IsRequired="true">
                                                            <RequiredField ErrorText="Debe Seleccionar Tipo Documento" IsRequired="true" />
                                                            <RegularExpression ErrorText="Faltan Datos" />
                                                            <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                <ErrorTextPaddings PaddingLeft="0px" />
                                                            </ErrorFrameStyle>

                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>

                                                </td>
                                            </tr>

                                            <tr>
                                                <td></td>
                                                <td class="pcmCellCaption">
                                                    <%--SUBIR ARCHIVO--%>
                                                    <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel5" runat="server" Text="Archivo">
                                                    </dx:ASPxLabel>
                                                </td>
                                                <td class="pcmCellCaption">:</td>
                                                <td class="pcmCellText">
                                                    <dx:ASPxUploadControl ForeColor="#00489e" Font-Names="verdana" Theme="MetropolisBlue" EnableTheming="True" CssClass="rounded txtbox text-white font-weight-bold" ID="UploadControl"
                                                        runat="server" ClientInstanceName="UploadControl" Width="100%"
                                                        NullText="Selecciona Archivo" FileUploadMode="OnPageLoad" UploadMode="Advanced" ShowProgressPanel="True" AutoStartUpload="true"
                                                        OnFileUploadComplete="UploadControl_FileUploadComplete">
                                                        <BrowseButtonStyle BackColor="#ff1049" ForeColor="White" CssClass="rounded txtbox text-white font-weight-bold">
                                                        </BrowseButtonStyle>
                                                        <AdvancedModeSettings EnableMultiSelect="false" EnableFileList="True" EnableDragAndDrop="True" />
                                                        <ValidationSettings MaxFileSize="4194304" AllowedFileExtensions=".jpg, .png, .pdf, .xlsx, .xls, .xml, .xlsm">
                                                        </ValidationSettings>
                                                        <ClientSideEvents FileUploadComplete="function (s, e) {
                                                        var text = s.GetText(e.inputIndex).replace(/\s|C:\\fakepath\\/g, '');
                                                        txtArchivoCargado.SetText('¡ARCHIVO ' + text + ' CARGADO!'); }" />
                                                    </dx:ASPxUploadControl>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td colspan="3" >
                                                    <dx:ASPxTextBox HorizontalAlign="Right" CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="txtArchivoCargado" ReadOnly="true" runat="server" Width="350px" ClientInstanceName="txtArchivoCargado" Text="Falta Cargar Archivo">
                                                        <Border BorderStyle="None" />
                                                    </dx:ASPxTextBox>

                                                </td>
                                                <td></td>
                                                <td></td>


                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td colspan="3">
                                                    <%--BOTONES GUARDAR Y CERRAR--%>
                                                    <div class="pcmButton d-flex justify-content-center">
                                                        <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px;background-color: #ff1049;" Width="80px" Height="30px"  Text="Guardar" ID="btnGrabarArchivo" runat="server" OnClick="btnGrabarArchivo_Click" AutoPostBack="false" UseSubmitBehavior="false">

                                                            <ClientSideEvents Click="OnClickConsultaEmpresa" />
                                                        </dx:ASPxButton>
                                                        <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px;background-color: #ff1049;" Width="80px" Height="30px" Text="Cerrar" ID="btnCerrar" runat="server" AutoPostBack="false" UseSubmitBehavior="false">
                                                            <ClientSideEvents Click="function(s, e) { popupCargaArchivo.Hide();}" />
                                                        </dx:ASPxButton>
                                                    </div>

                                                </td>
                                                <td></td>
                                                <td></td>

                                            </tr>
                                        </table>


                                    </dx:PanelContent>


                                </PanelCollection>
                            </dx:ASPxPanel>
                        </dx:PopupControlContentControl>

                    </ContentCollection>
                </dx:ASPxPopupControl>

            </td>

        </tr>
    </table>

</asp:Content>
