<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_contrataciones.aspx.vb" Inherits="gestionEST.pag_contrataciones" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Barra_Azul" runat="server">

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

    <script type="text/javascript">

        function Mensaje(s, e) {
            var result = confirm('¡¿Seguro que desea borrar el Archivo Seleccionado?!');
            //console.log("CONFIRM: " + result);
            e.processOnServer = result;
        }

        //VER CONTRATO
        function verContrato(s, e) {
            lpCallBack.Show();
            cbpGridMaster.PerformCallback();
        }

        function TerminaCargarContrato(s, e) {
            if (s.cpNewWindowUrl != null) window.open(s.cpNewWindowUrl);
            lpCallBack.Hide();
        }

        function OnInit(s, e) {
            AdjustSize();
            document.getElementById("gridContainer").style.visibility = "";
        }

        function NuevoContrato(s, e) {
            e.processOnServer = true;
        }

        function ModificaContrato(s, e) {
            e.processOnServer = true;
        }

        function AsociaContrato(s, e) {
            e.processOnServer = true;
        }


        function BorraContrato(s, e) {
            if (confirm('¿Seguro que desea borrar contrato seleccionado?')) {
                e.processOnServer = true;
            }
            else {
                e.processOnServer = false;
            }
        }


        function OnClickGrabar(s, e) {
            if (ASPxClientEdit.ValidateGroup('entryGroup')) {
                e.processOnServer = true;
            }
            else {
                e.processOnServer = false;
            }
        }

        function OnClickOkAsociar(s, e) {
            e.processOnServer = true;
        }

        function OnClickGrabarAsociar(s, e) {
            e.processOnServer = true;
        }

        function DuplicaContrato(s, e) {
            if (confirm('¿Desea Duplicar el Contrato Seleccionado?') == true) {
                e.processOnServer = true;
            } else {
                e.processOnServer = false;
            }
        }

        function jsCalculaFechaFin() {

            var days = txtDias.GetNumber();
            if (days > 0) {
                var dateObject = new Date();
                dateObject = txtFechaIni.GetDate();
                dateObject = new Date(dateObject.valueOf());
                dateObject.setDate(dateObject.getDate() + days - 1);
                txtFechaFin.SetDate(dateObject);
            }
        }

        function jsCalculaDias() {

            var fechaini = new Date();
            var fechafin = new Date();
            fechaini = txtFechaIni.GetDate();
            fechafin = txtFechaFin.GetDate();
            var timeDiff = (fechafin.getTime() - fechaini.getTime());
            var dias = Math.ceil(timeDiff / (1000 * 3600 * 24)) + 1;
            if (dias < 1) {
                dias = 1;
                txtFechaFin.SetDate(fechaini);
            }
            else {
                txtDias.SetText(dias);
            }

        }

        function RecargarUnidades(s, e) {
            lpPopup.Show();
            cbpUnidades.PerformCallback();
        }

        function EsconderCarga(s, e) {
            lpPopup.Hide();
        }

        function CambiarUnidadesPagina(s, e) {
            cbpPanelPagina.PerformCallback();

        }

        function recargarGrid(s, e) {
            cbpGrid.PerformCallback();

        }


    </script>
    <dx:ASPxCallbackPanel runat="server" ID="cbpPanelPagina" ClientInstanceName="cbpPanelPagina" OnCallback="cbpPanelPagina_Callback">
        <ClientSideEvents EndCallback="recargarGrid" />
        <PanelCollection>
            <dx:PanelContent>
                <table style="width: 155px" border="0">
                    <tr>
                        <td>
                            <label class="text-white font-weight-bold">Contrato Marco</label>
                        </td>
                    </tr>
                    <tr>
                        <td class="d-flex">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold">
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
                                <label class="text-white font-weight-bold">
                                    Unidad 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" NullText="SELECCIONE" AllowNull="true" Width="77%" CssClass="rounded" ID="cbxUnidad" runat="server" ValueType="System.String" ClientInstanceName="cbxUnidad" Theme="MetropolisBlue" OnLoad="cbxUnidad_Load">
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px; margin-right: -7px">
                            <dx:ASPxButton ID="btnConsultar" EnableTheming="false" CssClass="estiloBtnBuscar" Text="Buscar" runat="server" AutoPostBack="False" OnClick="btnConsultar_Click">
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    
    <hr style="background-color:red" />
    <table id="tb_BotonesCargar" runat="server" >
        <tr >
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage1" runat="server" ShowLoadingImage="true" ImageUrl="images/05.png">
                </dx:ASPxImage>
            </td>
            <td  class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnNuevo" EnableTheming="false" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" runat="server" Text="Nuevo Contrato" OnClick="btnNuevo_Click" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False">
                    
                    <ClientSideEvents Click="NuevoContrato" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td  class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage2" runat="server" ShowLoadingImage="true" ImageUrl="images/06.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton Wrap="True" ID="btnModifica" BorderRight-BorderStyle="None" EnableTheming="false" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" Font-Size="Small" runat="server" OnClick="btnModifica_Click" Text="Modificar Contrato" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False">
                    <ClientSideEvents Click="ModificaContrato" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td  class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage4" runat="server" ShowLoadingImage="true" ImageUrl="images/02.png">
                </dx:ASPxImage>
            </td>
            <td  class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnAsociar" EnableTheming="false" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" runat="server" OnClick="btnAsociar_Click" Text="Asociar Contrato" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False">
                    <ClientSideEvents Click="AsociaContrato" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td  class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage5" runat="server" ShowLoadingImage="true" ImageUrl="images/08.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnDuplicar" EnableTheming="false" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" Font-Size="Small" runat="server" Text="Duplicar Contrato" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False" OnClick="btnDuplicar_Click">
                    <ClientSideEvents Click="DuplicaContrato"  />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td  class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage6" runat="server" ShowLoadingImage="true" ImageUrl="images/04.png">
                </dx:ASPxImage>
            </td>
            <td  class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="ASPxButton1" EnableTheming="false" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" Font-Size="Small" runat="server" Text="Ver Contrato" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False">
                    <ClientSideEvents Click="verContrato" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr style="visibility:hidden">
            <td style="margin-bottom:5px;" class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage3" runat="server" ShowLoadingImage="true" ImageUrl="images/07.png">
                </dx:ASPxImage>
            </td>
            <td style="margin-bottom:5px" class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnBorra" EnableTheming="false" ImagePosition="Left" CssClass="rounded txtbox text-white font-weight-bold" Style="background-color: #00489e; width: 100%;" Font-Size="Small" runat="server" OnClick="btnBorra_Click" Text="Borrar Contrato" AutoPostBack="False" UseSubmitBehavior="false" EncodeHtml="False">
                    <ClientSideEvents Click="BorraContrato" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContenidoPagina" runat="server">

    <%--CALLBACK Y LOADING PANEL--%>
    <dx:ASPxLoadingPanel runat="server" ID="lpCallBack" Text="Cargando Contrato Marco" ClientInstanceName="lpCallBack" Modal="true" Theme="MetropolisBlue" ></dx:ASPxLoadingPanel>
    <dx:ASPxCallback runat="server" ID="cbpGridMaster"  ClientInstanceName="cbpGridMaster" OnCallback="cbpGridMaster_Callback">
        <ClientSideEvents EndCallback="TerminaCargarContrato" />
    </dx:ASPxCallback>

    <dx:ASPxCallbackPanel runat="server" ID="cbpGrid" ClientInstanceName="cbpGrid" OnCallback="cbpGrid_Callback">
        <PanelCollection>
            <dx:PanelContent>

                <%--GRID VIEW--%>
                <table style="width: 100%">
                    <tr>
                        <td>

                            <dx:ASPxGridView Width="100%" ForeColor="#00489e" ClientInstanceName="gridmaster" ID="gridMaster" runat="server" KeyFieldName="id"
                                AutoGenerateColumns="False" Theme="Default" OnPageIndexChanged="gridMaster_PageIndexChanged" OnBeforeGetCallbackResult="gridMaster_BeforeGetCallbackResult">
                                <ClientSideEvents RowDblClick="verContrato"></ClientSideEvents>
                                <Styles>
                                    <FixedColumn ForeColor="#00489E" HorizontalAlign="Center"></FixedColumn>
                                    <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" Font-Size="9" BackColor="#F2F2F2" ForeColor="#00489e"></Header>
                                    <Cell Font-Size="8" HorizontalAlign="Left"></Cell>
                                    <%--            <FocusedRow ForeColor="#00489e" BackColor="#d0d0d0"></FocusedRow>--%>
                                    <FocusedRow BackColor="#EAEAEA" ForeColor="#00489e" Font-Bold="true"></FocusedRow>
                                    <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
                                </Styles>

                                <Settings 
                                    GridLines="Vertical" 
                                    HorizontalScrollBarMode="Visible"
                                    ShowFilterRow="False"
                                    ShowGroupPanel="true"
                                    VerticalScrollBarMode="Hidden"
                                    VerticalScrollableHeight="800" />

                                <SettingsAdaptivity
                                    AdaptiveColumnPosition="right"
                                    AdaptiveDetailLayoutProperties-AlignItemCaptionsInAllGroups="true"
                                    AdaptivityMode="off" AllowOnlyOneAdaptiveDetailExpanded="false">
                                </SettingsAdaptivity>

                                <SettingsPager PageSize="20" Mode="ShowPager"></SettingsPager>

                                <SettingsBehavior EnableRowHotTrack="true" FilterRowMode="Auto" AllowFocusedRow="true" AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" />


                                <SettingsSearchPanel Visible="True" />

                                <Columns>

                                    <dx:GridViewDataTextColumn Caption="Id" FieldName="id" Name="id" ShowInCustomizationForm="True" Visible="False">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="rutasociado" FieldName="rutasociado" Name="rutasociado" ShowInCustomizationForm="True" Visible="false">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Asociado" Width="8%" FieldName="asociado" Name="asociado" ShowInCustomizationForm="true" Visible="true">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Contrato" Width="13%" FieldName="codContrato" Name="codContrato" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="empUsuaria" FieldName="empUsuaria" Name="empUsuaria" ShowInCustomizationForm="True" Visible="False">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Id" Width="5%" FieldName="unidad" Name="unidad" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Unidad" Width="11%" CellStyle-HorizontalAlign="Left" FieldName="descUnidad" Name="descUnidad">
                                        <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Fecha" Width="9%" CellStyle-HorizontalAlign="Left" FieldName="fecha" Name="fecha">
                                        <PropertiesTextEdit DisplayFormatString="dd-mm-yyyy">
                                        </PropertiesTextEdit>
                                        <Settings SortMode="Value" AllowSort="True" />
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="codCausa" FieldName="causaLegal" Name="causaLegal" ShowInCustomizationForm="True" Visible="False">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Causa <br> Legal" Width="9%" CellStyle-HorizontalAlign="Left" FieldName="descCausa" Name="descCausa" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="detalleCausal" FieldName="detalleCausal" Name="detalleCausal" ShowInCustomizationForm="True" Visible="False">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="codCargo" FieldName="cargo" Name="cargo" ShowInCustomizationForm="True" Visible="False">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Cargo" Width="13%" CellStyle-HorizontalAlign="Left" FieldName="descCargo" Name="descCargo" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="N° T" Width="6%" CellStyle-HorizontalAlign="center" FieldName="cantidadTrab" Name="cantidadTrab" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Inicio" Width="9%" CellStyle-HorizontalAlign="Left" FieldName="fini" Name="fini">
                                        <PropertiesTextEdit DisplayFormatString="dd-mm-yyyy">
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Fin" Width="9%" CellStyle-HorizontalAlign="Left" FieldName="ffin" Name="ffin">
                                        <PropertiesTextEdit DisplayFormatString="dd-mm-yyyy">
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Dias" Width="5%" FieldName="cantDias" Name="cantDias" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>

                                    <dx:GridViewDataTextColumn Caption="Valor" Width="7%" CellStyle-HorizontalAlign="Left" FieldName="valorPagar" Name="valorPagar" ShowInCustomizationForm="True">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="Borrar" Width="6%" CellStyle-HorizontalAlign="Center" Visible="false" FieldName="eliminar" Name="eliminar" ShowInCustomizationForm="True">
                                        <DataItemTemplate>
                                            <dx:ASPxButton ID="btnEliminarArchivo" EnableTheming="false" Font-Size="Small" CssClass="btnGridSinFondo" runat="server" Visible="true" AutoPostBack="false" OnClick="btnEliminarArchivo_Click" UseSubmitBehavior="false">
                                                <Image ToolTip="Eliminar" Url="images/03.png">
                                                </Image>
                                                <ClientSideEvents Click="Mensaje" />
                                            </dx:ASPxButton>
                                        </DataItemTemplate>
                                    </dx:GridViewDataTextColumn>

                                </Columns>
                            </dx:ASPxGridView>
                        </td>
                    </tr>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    

    <!-- FORMULARIO NUEVO CONTRATO -->
    <dx:ASPxCallbackPanel runat="server" ID="cbpUnidades" ClientInstanceName="cbpUnidades" OnCallback="cbpUnidades_Callback" Visible="true">
        <SettingsLoadingPanel Enabled="false" />
        <ClientSideEvents EndCallback="EsconderCarga" />
        <PanelCollection>
            <dx:PanelContent>

                <dx:ASPxLoadingPanel runat="server" ID="lpPopup" Text="Cargando Unidades" ClientInstanceName="lpPopup" Modal="true" Theme="MetropolisBlue" ></dx:ASPxLoadingPanel>
                <table>
                    <tr>
                        <td>
                            <dx:ASPxPopupControl ID="popupContrato" runat="server" CloseAction="CloseButton" ClientInstanceName="popupContrato" EnableViewState="False"
                                Modal="True" Width="100%" HeaderText="Nuevo Contrato" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis"
                                ShowPageScrollbarWhenModal="True" EnableHierarchyRecreation="True">
                                <ClientSideEvents PopUp="function(s, e) {  txtUnidad.Focus(); }" />
                                <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                                <ContentCollection>
                                    <dx:PopupControlContentControl runat="server">
                                        <dx:ASPxPanel ID="PanelContrato" runat="server">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">

                                                    <table border="0">
                                                        <%--COMBO EMPRESA--%>
                                                        <tr>
                                                            <td rowspan="10">
                                                                <div class="pcmSideSpacer">
                                                                </div>
                                                            </td>

                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel1" runat="server" Text="Empresa">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="cbxEmpresa" runat="server" Width="350px" ValueType="System.String" ClientInstanceName="cbxEmpresa">
                                                                    <ClientSideEvents SelectedIndexChanged="RecargarUnidades" />
                                                                    <ValidationSettings ValidationGroup="entryGroup">
                                                                        <RequiredField IsRequired="true" ErrorText="Seleccione Empresa" />
                                                                    </ValidationSettings>
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxComboBox>
                                                            </td>

                                                            <td rowspan="10" style="width: 10px">
                                                                <div class="pcmSideSpacer ">
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <!-- COMBOBOX UNIDAD -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel2" runat="server" Text="Unidad">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtUnidad" runat="server" Width="350px" ValueType="System.String" ClientInstanceName="txtUnidad">
                                                                    <ValidationSettings ValidationGroup="entryGroup">
                                                                        <RequiredField IsRequired="true" ErrorText="Seleccione Unidad" />
                                                                    </ValidationSettings>
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxComboBox>
                                                            </td>
                                                        </tr>

                                                        <!-- FECHA CONTRATO -->
                                                        <tr>

                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel15" runat="server" Text="Fecha">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxDateEdit ForeColor="#00489e" CssClass="rounded" Width="350px" Theme="MetropolisBlue" ID="txtFecha" ClientInstanceName="txtFecha" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" MinDate="2017-01-01">
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                        </tr>

                                                        <!-- COMBOBOX CAUSAL LEGAL-->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel10" runat="server" Text="Causa Legal">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtCausa" runat="server" Width="350px" ValueType="System.String" ClientInstanceName="txtCausa">
                                                                    <ValidationSettings ValidationGroup="entryGroup">
                                                                        <RequiredField IsRequired="true" ErrorText="Seleccione Causa " />
                                                                    </ValidationSettings>
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxComboBox>
                                                            </td>

                                                        </tr>

                                                        <!-- DETALLE CAUSA LEGAL -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel21" runat="server" Text="Detalle Causa Legal">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxMemo ForeColor="#00489e" Theme="MetropolisBlue" ID="txtDetalleCausal" runat="server" ClientInstanceName="txtDetalleCausal" Width="350px" Height="120px">
                                                                    <ValidationSettings ValidationGroup="entryGroup">
                                                                        <RequiredField IsRequired="true" ErrorText="Detalle la Causa"/>
                                                                    </ValidationSettings>
                                                                </dx:ASPxMemo>
                                                            </td>
                                                        </tr>

                                                        <!-- COMBOBOX CARGO-->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel8" runat="server" Text="Cargo">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtCargo" runat="server" Width="350px" ValueType="System.String" ClientInstanceName="txtCargo">
                                                                    <ValidationSettings ValidationGroup="entryGroup">
                                                                        <RequiredField IsRequired="true" ErrorText="Seleccione un Cargo" />
                                                                    </ValidationSettings>
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxComboBox>
                                                            </td>

                                                        </tr>

                                                        <!-- CANTIDAD TRABAJADORES -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel16" runat="server" Text="Cant. Trabajadores">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxSpinEdit ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="txtTrabajadores" ClientInstanceName="txtTrabajadores" Width="350px" runat="server" Number="1" MinValue="1" MaxValue="300" NumberType="Integer">
                                                                    <ClientSideEvents KeyDown="function(s, e) {  if(e.htmlEvent.keyCode === 13)  ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);  }" />
                                                                    
                                                                </dx:ASPxSpinEdit>
                                                            </td>
                                                        </tr>

                                                        <!-- FECHA INICIO -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel4" runat="server" Text="Fecha Inicio">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxDateEdit ForeColor="#00489e" CssClass="rounded" Width="350px" Theme="MetropolisBlue" ID="txtFechaIni" ClientInstanceName="txtFechaIni" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" MinDate="2017-04-01">
                                                                    <ClientSideEvents ValueChanged="jsCalculaDias" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                        </tr>

                                                        <!-- CANTIDAD DIAS -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel5" runat="server" Text="Cant. Dias">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxSpinEdit ForeColor="#00489e" Theme="MetropolisBlue" Width="350px" ID="txtDias" ClientInstanceName="txtDias" runat="server" Number="1" MinValue="1" MaxValue="300" NumberType="Integer">
                                                                    <ClientSideEvents ValueChanged="jsCalculaFechaFin" />

                                                                </dx:ASPxSpinEdit>
                                                            </td>
                                                        </tr>

                                                        <!-- FECHA FIN -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel17" runat="server" Text="Fecha Fin">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxDateEdit ForeColor="#00489e" CssClass="rounded" Width="350px" Theme="MetropolisBlue" ID="txtFechaFin" ClientInstanceName="txtFechaFin" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" MinDate="2017-04-01">
                                                                    <ClientSideEvents ValueChanged="jsCalculaDias" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                                                                </dx:ASPxDateEdit>
                                                            </td>

                                                        </tr>

                                                        <!-- VALOR A PAGAR -->
                                                        <tr>
                                                            <td></td>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel12" runat="server" Text="Valor a Pagar">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxSpinEdit ForeColor="#00489e" Theme="MetropolisBlue" ID="txtValor" Width="350px" ClientInstanceName="txtValor" runat="server" Number="1" MinValue="257500" MaxValue="3000000" NumberType="Integer">
                                                                </dx:ASPxSpinEdit>
                                                            </td>
                                                        </tr>


                                                        <!-- BOTONES -->
                                                        <tr>
                                                            <td style="width: 10px;">
                                                                <div class="pcmSideSpacer ">
                                                                </div>
                                                            </td>
                                                            <td colspan="3">
                                                                <div class="pcmButton d-flex justify-content-center">
                                                                    <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px; background-color: #ff1049;" Width="80px" Height="30px" ID="btnGrabar" runat="server" Text="Guardar" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnGrabar_Click">
                                                                        <ClientSideEvents Click="OnClickGrabar" />
                                                                    </dx:ASPxButton>

                                                                    <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px; background-color: #ff1049;" Width="80px" Height="30px" ID="btnCancelar" runat="server" Text="Cancelar" AutoPostBack="false" UseSubmitBehavior="false">
                                                                        <ClientSideEvents Click="function(s, e) { popupContrato.Hide();}" />
                                                                    </dx:ASPxButton>
                                                                </div>
                                                            </td>

                                                        </tr>
                                                        <%--BUSQUEDA DE TRABAJADOR--%>
                                                        <tr>
                                                            <td class="pcmCellCaption" colspan="5" style="text-align: center">
                                                                <table style="width:100%;" border="0">
                                                                    <tr>
                                                                        <td style="width:90%;">
                                                                            <div style="padding-left:5%">
                                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="lblBusquedaTrabajador" runat="server" Text="Historia Contrato">
                                                                                </dx:ASPxLabel>
                                                                            </div>
                                                                            
                                                                        </td>
                                                                        <td>
                                                                            <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: right; margin-right: 15px; background-color: #ff1049;" Width="80px" Height="30px" ID="btnExportarGrid" runat="server" Text="Exportar" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnExportarGrid_Click">
                                                                            </dx:ASPxButton>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="pcmCellText" style="width: 100%" colspan="5">
                                                                <%--GRID CONTRATOS TRABAJADORES--%>
                                                                <dx:ASPxGridView Width="100%" ForeColor="#00489e" ClientInstanceName="gridBucarTrabajadores" ID="gridBucarTrabajadores" runat="server" KeyFieldName="codigo"
                                                                    AutoGenerateColumns="False" Theme="Default" OnInit="gridBucarTrabajadores_Init" EnableCallBacks="true">
                                                                    <Settings GridLines="Vertical" HorizontalScrollBarMode="Visible" />

                                                                    <Styles>
                                                                        <FixedColumn ForeColor="#00489E" HorizontalAlign="Center"></FixedColumn>
                                                                        <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" Font-Size="9" BackColor="#F2F2F2" ForeColor="#00489e"></Header>
                                                                        <Cell Font-Size="8" HorizontalAlign="Left"></Cell>
                                                                        <FocusedRow BackColor="#EAEAEA" ForeColor="#00489e" Font-Bold="true"></FocusedRow>
                                                                        <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
                                                                    </Styles>

                                                                    <Settings
                                                                        ShowFilterRow="False"
                                                                        ShowGroupPanel="true"
                                                                        VerticalScrollBarMode="Hidden"
                                                                        VerticalScrollableHeight="200" />

                                                                    <SettingsAdaptivity
                                                                        AdaptiveColumnPosition="right"
                                                                        AdaptiveDetailLayoutProperties-AlignItemCaptionsInAllGroups="true"
                                                                        AdaptivityMode="off" AllowOnlyOneAdaptiveDetailExpanded="false">
                                                                    </SettingsAdaptivity>

                                                                    <SettingsPager PageSize="6" Mode="ShowPager"></SettingsPager>

                                                                    <SettingsBehavior EnableRowHotTrack="true" FilterRowMode="Auto" AllowFocusedRow="true" AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" />


                                                                    <SettingsSearchPanel Visible="True" />

                                                                    <Columns>

                                                                        <dx:GridViewDataTextColumn Caption="Rut" Width="16%" FieldName="rutasociado" Name="rutasociado" ShowInCustomizationForm="True" Visible="true">
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                        </dx:GridViewDataTextColumn>

                                                                        <dx:GridViewDataTextColumn Caption="Trabajador" Width="15%" FieldName="Nombre" Name="Nombre" ShowInCustomizationForm="True" Visible="true">
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                        </dx:GridViewDataTextColumn>

                                                                        <dx:GridViewDataTextColumn Caption="Causa</br>Legal" Width="14%" FieldName="descrip" Name="descrip" ShowInCustomizationForm="True" Visible="true">
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                        </dx:GridViewDataTextColumn>

                                                                        <dx:GridViewDataTextColumn Caption="Detalle" Width="24%" FieldName="detalleCausal" Name="detalleCausal" ShowInCustomizationForm="True" Visible="true">
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                        </dx:GridViewDataTextColumn>

                                                                        <dx:GridViewDataTextColumn Caption="Inicio</br>Contrato" Width="13%" FieldName="fini" Name="fini" ShowInCustomizationForm="true" Visible="true">
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                        </dx:GridViewDataTextColumn>

                                                                        <dx:GridViewDataTextColumn Caption="Fin</br>Contrato" Width="13%" FieldName="ffin" Name="ffin" ShowInCustomizationForm="True">
                                                                        </dx:GridViewDataTextColumn>

                                                                        <dx:GridViewDataTextColumn Caption="Dias" Width="6%" FieldName="cantDias" Name="cantDias" ShowInCustomizationForm="True">
                                                                        </dx:GridViewDataTextColumn>
                                                                    </Columns>
                                                                </dx:ASPxGridView>
                                                                <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="gridBucarTrabajadores">
                                                                    <Styles>
                                                                        <Default Font-Names="Arial" Font-Size="Medium">
                                                                        </Default>
                                                                        <Header Font-Names="Arial" Font-Size="Medium">
                                                                        </Header>
                                                                    </Styles>

                                                                </dx:ASPxGridViewExporter>
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
                <!-- FORMULARIO ASOCIA-->
    <table>
        <tr>
            <td>

                <dx:ASPxPopupControl ID="popupAsocia" runat="server" CloseAction="CloseButton" ClientInstanceName="popupAsocia" EnableViewState="False"
                    Modal="True" Width="600px" HeaderText="Asociar Contrato" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" 
                    ShowPageScrollbarWhenModal="True">
                    <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                    <ClientSideEvents PopUp="function(s, e) {  txtCodigoTrabajador.Focus(); }" />
                    <ContentCollection>
                        <dx:PopupControlContentControl runat="server">
                            <dx:ASPxPanel ID="ASPxPanel1" runat="server">
                                <PanelCollection>
                                    <dx:PanelContent runat="server">
                                        <table>

                                            <!-- CODIGO TRABAJADOR-->
                                            <tr>
                                                <td rowspan="8">
                                                    <div class="pcmSideSpacer">
                                                    </div>
                                                </td>


                                                <td class="pcmCellCaption">
                                                    <dx:ASPxLabel  CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel3" runat="server" Text="Código Trabajador:">
                                                    </dx:ASPxLabel>
                                                </td>
                                                <td class="pcmCellText">
                                                    <dx:ASPxTextBox ForeColor="#00489e" Theme="MetropolisBlue" ID="txtCodigoTrabajador" runat="server" Width="350px" ClientInstanceName="txtCodigoTrabajador">
                                                    </dx:ASPxTextBox>
                                                </td>

                                                <td rowspan="8" style="width: 10px">
                                                    <div class="pcmSideSpacer ">
                                                    </div>
                                                </td>
                                            </tr>


                                            <!-- BOTONES -->
                                            <tr>
                                                <td style="width: 10px">
                                                    <div class="pcmSideSpacer ">
                                                    </div>
                                                </td>
                                                <td colspan="2">
                                                    <div class="pcmButton">
                                                        <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px;background-color: #ff1049;" Width="80px" Height="30px"  ID="btnOkAsociar" runat="server" Text="Ok" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnOkAsociar_Click">
                                                            <ClientSideEvents Click="OnClickOkAsociar" />
                                                        </dx:ASPxButton>

                                                        <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px;background-color: #ff1049;" Width="80px" Height="30px"  ID="btnCancelarAsociar" runat="server" Text="Cancelar" AutoPostBack="false" UseSubmitBehavior="false">
                                                            <ClientSideEvents Click="function(s, e) { popupAsocia.Hide();}" />
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

                <!-- FORMULARIO GRABAR ASOCIAR-->
    <table>
        <tr>
            <td>

                <dx:ASPxPopupControl ID="popupGrabarAsociar" runat="server" CloseAction="CloseButton" ClientInstanceName="popupGrabarAsociar" EnableViewState="False"
                    Modal="True" Width="600px" HeaderText="Asociar Contrato" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
                    <ClientSideEvents PopUp="function(s, e) {  btnCerrarAsociar.Focus(); }" />
                    <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                    <ContentCollection>
                        <dx:PopupControlContentControl runat="server">
                            <dx:ASPxPanel ID="ASPxPanel2" runat="server">
                                <PanelCollection>
                                    <dx:PanelContent runat="server">
                                        <table>

                                            <!-- DATOS -->
                                            <tr>
                                                <td rowspan="8">
                                                    <div class="pcmSideSpacer">
                                                    </div>
                                                </td>


                                                <td class="pcmCellCaption">
                                                    <dx:ASPxLabel  CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="lblEmpleado" runat="server" Text="Empleado:">
                                                    </dx:ASPxLabel>
                                                </td>
                                                <td class="pcmCellCaption">:</td>
                                                <td class="pcmCellText">
                                                    <dx:ASPxTextBox ForeColor="#00489e" Theme="MetropolisBlue" ID="txtIdEmpleado" Font-Bold="true" runat="server" Width="100px" ClientInstanceName="txtIdEmpleado" ReadOnly="true">
                                                        <Border BorderStyle="None" />
                                                    </dx:ASPxTextBox>
                                                    <dx:ASPxTextBox ForeColor="#00489e" Theme="MetropolisBlue" ID="txtNombreEmpleado" Font-Bold="true" runat="server" Width="350px" ClientInstanceName="txtNombreEmpleado" ReadOnly="true">
                                                        <Border BorderStyle="None" />
                                                    </dx:ASPxTextBox>
                                                </td>

                                                <td rowspan="8" style="width: 10px">
                                                    <div class="pcmSideSpacer ">
                                                    </div>
                                                </td>
                                            </tr>


                                            <!-- BOTONES -->
                                            <tr>
                                                <td style="width: 10px">
                                                    <div class="pcmSideSpacer ">
                                                    </div>
                                                </td>
                                                <td colspan="3">
                                                    <div class="pcmButton">
                                                        <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px;background-color: #ff1049;" Width="80px" Height="30px" ID="btnGrabarAsociar" runat="server" Text="Grabar" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnGrabarAsociar_Click">
                                                            <ClientSideEvents Click="OnClickGrabarAsociar" />
                                                        </dx:ASPxButton>

                                                        <dx:ASPxButton EnableTheming="false" CssClass="estiloBtnBuscar" Style="float: left; margin-right: 15px;background-color: #ff1049;" Width="80px" Height="30px" ID="btnCerrarAsociar" runat="server" Text="Cancelar" AutoPostBack="false" UseSubmitBehavior="false">
                                                            <ClientSideEvents Click="function(s, e) { popupGrabarAsociar.Hide();}" />
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
    
</asp:Content>
