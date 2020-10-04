
<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_colaboradores.aspx.vb" Inherits="gestionEST.pag_colaboradores" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Barra_Azul" runat="server">
    <%--Barra AZUL--%>

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

        //CALL BACK 
        var postponedCallbackRequired = false;

        function OnGridFocusedRowChanged() {

            //ENCUENTRO AL ELEMENTO DENTRO DEL SPLITTER
            //div

            if (CallbackPanel.InCallback())
                postponedCallbackRequired = true;
            else
                CallbackPanel.PerformCallback();
            //PASAR VALORES AL POPUP
            var grid = (<%= SpliterPadre.GetPaneByName("splitterSuperior").Controls.Item(5).ID%>);
            grid.GetRowValues(grid.GetFocusedRowIndex(), 'rut;nombre', OnGetRowValues);

        }
        // EL ARREGLO "values" CONTIENE LOS DATOS DE RUT Y NOMBRE
        function OnGetRowValues(values) {
            txtIdEmpleado.SetText(values[0]);
            txtEmpleado.SetText(values[1]);
        }

        function OnListBoxIndexChanged(s, e) {
            if (CallbackPanel.InCallback())
                postponedCallbackRequired = true;
            else
                CallbackPanel.PerformCallback();
        }
        function OnEndCallback(s, e) {
            if (postponedCallbackRequired) {
                CallbackPanel.PerformCallback();
                postponedCallbackRequired = false;

            }
        }

        function ValidaRangoFechas(s, e) {
            //Día actual
            var today = new Date();
            var dd = String(today.getDate()).padStart(2, '0');
            var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
            var yyyy = today.getFullYear();
            var limitevencimiento = yyyy + 10
            //Valor ingresado
            valor = txtVenceDocto.GetText();
            var valorIngresado = valor;
            valorIngresadoDia = valorIngresado.slice(0, 2);
            valorIngresadoMes = valorIngresado.slice(3, 5);
            valorIngresadoAno = valorIngresado.slice(6);
            var valorIngresadoJunto = parseInt(valorIngresadoDia + valorIngresadoMes + valorIngresadoAno);
            //Limito la fecha de ingreso y restauro valores
            if (valorIngresadoAno > limitevencimiento) {
                alert('Fecha de vencimiento no puede ser mayor al año: ' + limitevencimiento);
                txtVenceDocto.SetValue(null);
            }

            //today = mm + '-' + dd + '-' + yyyy;
            //juntos = mm+dd+yyyy;
            //today = parseInt(juntos)
            //console.info(yyyy + 20);
            //console.info(today);
            //console.info(valorIngresadoJunto);
        }

        //OBTENER LOS VALORES DEL CBXTIPODCTO
        function valorTIpoDcto(s, e) {
            cbxDescripcion.ClearItems();
            //var index = txtTipoDocto.GetSelectedIndex().toString();
            var valueTest = txtTipoDocto.GetValue();

            if (txtTipoDocto.GetSelectedItem() != null) {
                var texto = txtTipoDocto.GetSelectedItem().GetColumnText(0);
                //var texto = txtTipoDocto.GetSelectedItem();
                //alert(texto);
                //alert(texto)
                if (valueTest == 'CHAMAS') {
                    cbxDescripcion.AddItem("CHARLA MASSO");
                    cbxDescripcion.AddItem("PROCEDIMIENTOS ESPECÍFICOS");
                    cbxDescripcion.AddItem("FICHA SALUD Y SEGURIDAD");
                    cbxDescripcion.SetText("CHARLA MASSO");
                }
                else if (valueTest == 'DERSAB') {
                    cbxDescripcion.AddItem("OBLIGACIÓN DE INFORMAR");
                    cbxDescripcion.AddItem("DERECHO A SABER");
                    cbxDescripcion.SetText("DERECHO A SABER");
                }
                else {
                    cbxDescripcion.SetText(txtTipoDocto.GetSelectedItem().GetColumnText(1));
                }

                //alert(txtTipoDocto.GetSelectedItem().GetColumnText(3));

                var tr_vencimiento = <%= tr_vencimiento.ClientID%>;
                if (txtTipoDocto.GetSelectedItem().GetColumnText(2) == "1") {
                    tr_vencimiento.style.visibility = 'visible';
                    //var myDate = new Date(2011, 1, 1);
                    //txtVenceDocto.SetEnabled(true);
                    txtVenceDocto.SetVisible(true);
                    //txtVenceDocto.SetDate(myDate);
                    //lblVenveDcto.SetVisible(true);
                } else {
                    //txtVenceDocto.SetEnabled(false);
                    txtVenceDocto.SetVisible(false);
                    tr_vencimiento.style.visibility = 'hidden';
                    //txtVenceDocto.SetVisible(false);
                    //lblVenveDcto.SetVisible(false);
                }

            }

            //alert(valueTest);



            <%--if (index == 0 || index == 2 || index == 3 || index == 5 || index == 6 ||
                index == 7 || index == 8 || index == 9 || index == 10 || index == 11) {
                cbxDescripcion.ClearItems();
                cbxDescripcion.SetText(txtTipoDocto.GetText());
            }

            var tr_vencimiento = <%= tr_vencimiento.ClientID%>;
            if (index == 1 || index == 2 || index == 7 || index == 8 || index == 10) {
                tr_vencimiento.style.visibility = 'visible';
                //var myDate = new Date(2011, 1, 1);
                //txtVenceDocto.SetEnabled(true);
                txtVenceDocto.SetVisible(true);
                //txtVenceDocto.SetDate(myDate);
                //lblVenveDcto.SetVisible(true);
                
            } else {
                //txtVenceDocto.SetEnabled(false);
                txtVenceDocto.SetVisible(false);
                tr_vencimiento.style.visibility = 'hidden';
                //txtVenceDocto.SetVisible(false);
                //lblVenveDcto.SetVisible(false);
            }--%>
        }



        function OnClickConsultaEmpresa(s, e) {
            var validado = false;
            if (ASPxClientEdit.ValidateGroup('entryGroup') == true) {
                if (txtArchivoCargado.GetText() != 'Falta Cargar Archivo') {
                    validado = true;
                } else {
                    alert('FALTA CARGAR ARCHIVO')
                    validado = false;
                }
            }

            if (validado == true) {
                e.processOnServer = true;
            } else {
                e.processOnServer = false;
            }
        }

        function Mensaje(s, e) {
            var result = confirm('¡¿Seguro que desea borrar el Archivo Seleccionado?!');
            //console.log("CONFIRM: " + result);
            e.processOnServer = result;
        }

        function LimpiarPopUp(s, e) {
            lpCallBack.Show();
            cbpEditar.PerformCallback();
        }

        function AbrirPopUpDescarga(s, e) {
            lpCallBack.Show();
            cbpDescargarArchivos.PerformCallback();
        }

        function CambiarUnidadesPagina() {
            cbpPanelIzquierdo.PerformCallback();
        }

        function AbrirPopUpEvaluacion(s, e) {
            lpCallBack.Show();
            //OBTENGO INDEX
            var regIndexFila = /.+_cell([0-9]+)/
            var resultadoIndexFila = regIndexFila.exec(s.name);
            var fila = resultadoIndexFila[1];
            //console.log(fila);
            cbpverevaluaciones.PerformCallback(fila);
        }

    </script>

    <dx:ASPxCallbackPanel runat="server" ID="cbpPanelIzquierdo" ClientInstanceName="cbpPanelIzquierdo" OnCallback="cbpPanelIzquierdo_Callback">
        <PanelCollection>
            <dx:PanelContent>
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <label class="text-white font-weight-bold" style="font-family: Verdana; padding-bottom: 10px">
                                Buscar Por
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
                            <div style="width: 25%;">
                                <label class="text-white font-weight-bold" style="font-family: Verdana;">
                                    Unidad 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" AllowNull="true" Width="83%" CssClass="rounded" ID="cbxUnidad" runat="server" ValueType="System.String" ClientInstanceName="" Theme="MetropolisBlue" OnLoad="cbxUnidad_Load">
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px; margin-right: -7px">
                            <dx:ASPxButton ID="btnConsultar" CssClass="estiloBtnBuscar" Text="Buscar" runat="server" AutoPostBack="False" OnClick="btnConsultar_Click">
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
                <dx:ASPxButton ID="btnSubeArchivo" ImagePosition="Left" EnableTheming="false" CssClass="estiloBtnSubmenusExportar" Text="Cargar Archivos"  runat="server" Visible="true" AutoPostBack="False" UseSubmitBehavior="false" >
                    <ClientSideEvents Click="LimpiarPopUp" />
                </dx:ASPxButton>
            </td>

        </tr>
        <tr>
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage2" runat="server" ShowLoadingImage="true" ImageUrl="images/16.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:ASPxButton ID="btnDocumentos" ImagePosition="Left" CssClass="estiloBtnSubmenusExportar" Text="Descargar"  runat="server" Visible="true" AutoPostBack="False" UseSubmitBehavior="false" >
                    <ClientSideEvents Click="AbrirPopUpDescarga" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr style="visibility:hidden">
            <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px">
                <dx:ASPxButton ID="btnVerContrato" EnableTheming="false" CssClass="rounded txtbox text-white font-weight-bold" Style="background-color: #ff1049; width: 100%;" Font-Size="Small" Text="Ver Contrato" runat="server" Visible="true" OnClick="btnVerContrato_Click" AutoPostBack="False" UseSubmitBehavior="false">
                    <Image ToolTip="Ver Contrato" Url="images/verArchivo.png">
                    </Image>
                    <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContenidoPagina" runat="server">

    <dx:ASPxSplitter
        
        ShowCollapseBackwardButton="true"
        Orientation="Vertical"
        ID="SpliterPadre"
        runat="server"
        Height="100%"
        >

        <Panes>
            
            <dx:SplitterPane MinSize="540" ScrollBars="Auto" Name="splitterSuperior" >
                <%--GRID SUPERIOR--%>
                <ContentCollection >
                    <dx:SplitterContentControl>
                        <div id="lblDefinicionAdministrador" runat="server" style="color: #00489e" visible="false">
                            <label>
                                <span>
                                    <img src="images/15.png" />
                                </span>: Activo
                                <span>
                                    <img src="images/12.png" />
                                </span>: Eliminado 
                                <span>
                                    <img src="images/14.png" />
                                </span>: Suspendido
                                <span>
                                    <img src="images/13.png" />
                                </span>: En Observación |
                                <span>
                                    <img src="images/04.png" />
                                </span>: Contrato de Colaborador |
                                <span>
                                    <img src="images/search.png" />
                                </span>: Detalles de Colaborador |
                            </label>
                        </div>
                        <div id="lblDefinicionCliente" runat="server" style="color: #00489e" visible="false">
                            <label>
                                <span>
                                    <img src="images/15.png" />
                                </span>: Activo |
                                <span>
                                    <img src="images/search.png" />
                                </span>: Detalles de Colaborador |
                            </label>
                        </div>
                        <div style="width:100%">
                            <dx:ASPxGridView
                                Width="100%"
                                ForeColor="#00489e"
                                ID="gridEmpleado"
                                ClientInstanceName="gridEmpleado"
                                runat="server"
                                EnableCallBacks="true"
                                KeyFieldName="rut"
                                Theme="Default"
                                AutoGenerateColumns="False"
                                OnBeforeGetCallbackResult="gridEmpleado_BeforeGetCallbackResult">

                            <%--EVENTO DE CAMBIO DE ROW SELECCIONADA--%>
                            <ClientSideEvents FocusedRowChanged="function(s, e) { OnGridFocusedRowChanged(); }"/>
                            <Settings
                                
                                ShowFilterRow="False"
                                ShowGroupPanel="true"
                                VerticalScrollBarMode="Hidden"
                                VerticalScrollableHeight="300"/>


                            <Styles>
                                
                                <FixedColumn ForeColor="#00489E" HorizontalAlign="Center"></FixedColumn>
                                <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" Font-Size="Small" BackColor="#F2F2F2" ForeColor="#00489e"></Header>
                                <Cell Font-Size="Small" HorizontalAlign="Left"></Cell>
                                <FocusedRow BackColor="#EAEAEA" ForeColor="#00489e" Font-Bold="true"></FocusedRow>
                                <RowHotTrack ForeColor="#00489e" BackColor="#EAEAEA"></RowHotTrack>
                            </Styles>
                            <SettingsPager PageSize="15">
                            </SettingsPager>

                            <SettingsBehavior
                                
                                EnableRowHotTrack="true"
                                AllowFocusedRow="True"
                                AllowSelectByRowClick="false"
                                
                                AllowSelectSingleRowOnly="true" />

                            <SettingsSearchPanel Visible="True" ColumnNames="rut; nombre" Delay="1500" ShowApplyButton="False" ShowClearButton="False" />
                            
                            <Columns>
                                <dx:GridViewDataTextColumn Width="10%" Caption="Rut" FieldName="rut" Name="rut" ShowInCustomizationForm="True" >
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn Width="30%" Caption="Nombre" FieldName="nombre" Name="nombre" ShowInCustomizationForm="True" >
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn CellStyle-HorizontalAlign="Center" Caption="ID" FieldName="idUnidad" Name="idUnidad" ShowInCustomizationForm="True" >
                                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn Width="25%"  Caption="Unidad" FieldName="descUnidad" Name="descUnidad" ShowInCustomizationForm="True">
                                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn  CellStyle-HorizontalAlign="Center" Caption="Estado" FieldName="estado" Name="estado" ShowInCustomizationForm="True"  >
                                    <Settings AllowHeaderFilter="True" FilterMode="DisplayText" AllowSort="False" AllowGroup="False" />
                                    <DataItemTemplate>
                                        <dx:ASPxImage ID="imgEstado_A" FieldName="imgEstado_A" Name="imgEstado_A" runat="server" OnLoad="imgEstado_A_Load" Visible="false"></dx:ASPxImage>
                                    </DataItemTemplate>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn CellStyle-HorizontalAlign="Center" Caption="Contrato" FieldName="contrato" Visible="false" Name="contrato" ShowInCustomizationForm="True">
                                    <Settings/>
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnVerContratoGrid" CssClass="btnGridSinFondo" runat="server" Visible="true"  OnClick="btnVerContrato_Click" AutoPostBack="False" UseSubmitBehavior="false">
                                            <Image ToolTip="Ver Contrato" Url="images/04.png">
                                            </Image>
                                            <ClientSideEvents Click="function (s, e) {e.processOnServer = true;}" />
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn  CellStyle-HorizontalAlign="Center" Caption="Detalle" FieldName="detalle" Name="detalle" ShowInCustomizationForm="True">
                                    <Settings/>
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnVerDetalle" runat="server" CssClass="btnGridSinFondo" Visible="true" AutoPostBack="False" UseSubmitBehavior="false">
                                            <Image Url="images/search.png">
                                            </Image>
                                            <ClientSideEvents Click="function(s, e) { OnGridFocusedRowChanged(); }" />
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn  CellStyle-HorizontalAlign="Center" Caption="Ver Evaluaciones" FieldName="evaluacion" Name="evaluacion" ShowInCustomizationForm="True">
                                    <Settings/>
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnVerEvaluacion" runat="server" CssClass="btnGridSinFondo" Visible="true" AutoPostBack="False" UseSubmitBehavior="false">
                                            <Image Url="images/copy.png">
                                            </Image>
                                            <ClientSideEvents Click="AbrirPopUpEvaluacion" />
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataTextColumn>
                            </Columns>

                        </dx:ASPxGridView>
                        </div>
                        
                    </dx:SplitterContentControl>
                </ContentCollection>
            </dx:SplitterPane>

            <dx:SplitterPane Name="spliterInferior" AutoHeight="true" PaneStyle-BackColor="#f8f8f9">
                <ContentCollection>
                    <dx:SplitterContentControl BorderColor="Transparent">
                        <%--MENU Y PANEL INFERIOR--%>
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                        <div id="divPanelInferior" runat="server">
                            <table border="0" style="width:100%">
                            <tr>
                                <td>
                                 <div id="divlistBoxInferior" runat="server" style="border:none">
                                        <dx:ASPxListBox Visible="true" BorderLeft-BorderStyle="None" Font-Bold="false" Width="100%" runat="server" ID="listBoxInferior" ClientInstanceName="ListBox" Paddings-PaddingLeft="0px" CssClass="font-weight-bold" Height="220" ForeColor="#00489e" BackColor="Transparent" Border-BorderStyle="None" SelectedIndex="0">
                                            <ClientSideEvents SelectedIndexChanged="OnListBoxIndexChanged" />
                                            <ItemStyle
                                                Border-BorderStyle="None"
                                                Font-Size="Small"
                                                Paddings-PaddingBottom="0"
                                                Paddings-PaddingRight="10"
                                                Paddings-PaddingLeft="20"
                                                HoverStyle-BackColor="#d0d0d0" 
                                                HoverStyle-ForeColor="#00489e"
                                                SelectedStyle-Font-Bold="true"
                                                SelectedStyle-ForeColor="#00489e"
                                                SelectedStyle-Border-BorderStyle="None"
                                                SelectedStyle-BackColor="#d0d0d0"
                                                SelectedStyle-BackgroundImage-HorizontalPosition="left"
                                                SelectedStyle-BackgroundImage-ImageUrl="images/01.png"
                                                SelectedStyle-BackgroundImage-Repeat="NoRepeat"
                                                SelectedStyle-BackgroundImage-VerticalPosition="center" />
                                            
<%--                                            <Items>
                                                <dx:ListEditItem Selected="true" Value="Liquidaciones" Text="LIQUIDACIONES" />
                                                <dx:ListEditItem Value="Pacto_HE" Text="PACTO HE" />
                                                <dx:ListEditItem Value="Estudios" Text="ESTUDIOS" />
                                                <dx:ListEditItem Value="Masso" Text="MASSO/PE/S&S" />
                                                <dx:ListEditItem Value="Contratos" Text="CONTRATOS Y ANEXOS" />
                                                <dx:ListEditItem Value="Curriculum" Text="CURRICULUM" />
                                                <dx:ListEditItem Value="Der_Saber" Text="ODI/DER. A SABER" />
                                                <dx:ListEditItem Value="EPP" Text="EPP" />
                                                <dx:ListEditItem Value="Reg_Interno" Text="REG. INTERNO" />
                                                <dx:ListEditItem Value="Ex_Preoc" Text="EX. PREOCUPACIONAL" />
                                                <dx:ListEditItem Value="Ex_Psico" Text="EV. PSICOLÓGICA" />
                                            </Items>--%>
                                        </dx:ASPxListBox>
                                    </div>
                                </td>
                                <td style="width:80%">
                                    <div>
                                        <dx:ASPxCallbackPanel runat="server" ID="cbpInferior" ClientInstanceName="CallbackPanel" RenderMode="Div" OnCallback="cbpInferior_Callback">
                                            
                                            <ClientSideEvents EndCallback="OnEndCallback"></ClientSideEvents>
                                            <PanelCollection>
                                                <dx:PanelContent>
                                                    <dx:ASPxLoadingPanel runat="server" ID="lpCallBack" Text="Cargando..." ClientInstanceName="lpCallBack" Modal="true"  Theme="MetropolisBlue"></dx:ASPxLoadingPanel>
                                                    <div style="width: 100%" id="divGridInferior" runat="server">
                                                        <dx:ASPxGridView ID="GridInferior" Width="100%" ClientInstanceName="gSueldo" runat="server" KeyFieldName="id" AutoGenerateColumns="False" ForeColor="#00489e" OnPageIndexChanged="GridInferior_PageIndexChanged" OnBeforeGetCallbackResult="GridInferior_BeforeGetCallbackResult"
                                                            Theme="Default">
                                                            <SettingsBehavior AllowFocusedRow="true" AllowHeaderFilter="false" AllowSort="false" EnableRowHotTrack="true" />
                                                            <SettingsPager PageSize="10" />
                                                            <Settings GridLines="Vertical" VerticalScrollBarMode="Visible" />
                                                            <Styles>
                                                                <Cell ForeColor="#00489e" HorizontalAlign="Center"></Cell>
                                                                <Header BackColor="#00489e" Font-Bold="true" ForeColor="White" HorizontalAlign="Center"></Header>
                                                                <FocusedRow BackColor="#EAEAEA" ForeColor="#00489e" Font-Bold="true"></FocusedRow>
                                                                <RowHotTrack BackColor="#d0d0d0" ></RowHotTrack>
                                                                
                                                            </Styles>
                                                            <Columns>
                                                                
                                                                <dx:GridViewDataHyperLinkColumn Caption="Documento" FieldName="ruta" Name="ruta" ShowInCustomizationForm="True" VisibleIndex="0">
                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                    <DataItemTemplate>
                                                                        <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                                                                    </DataItemTemplate>
                                                                    <CellStyle HorizontalAlign="Center">
                                                                    </CellStyle>
                                                                </dx:GridViewDataHyperLinkColumn>
                                                                <dx:GridViewDataTextColumn Width="35%" Caption="Descripcion" CellStyle-HorizontalAlign="Left" FieldName="descdocto" Name="descdocto" ShowInCustomizationForm="True" VisibleIndex="1">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataTextColumn Caption="Creación" FieldName="fechadcto" Name="fechadcto" ShowInCustomizationForm="True" VisibleIndex="2">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataTextColumn Caption="Id" FieldName="id" Name="id" ShowInCustomizationForm="True" VisibleIndex="3" Visible="false">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataColumn Caption="Eliminar" FieldName="eliminar" Visible="false" Name="eliminar" VisibleIndex="4">
                                                                    <Settings />
                                                                    <DataItemTemplate>
                                                                        <dx:ASPxButton ID="btnEliminarArchivo" CssClass="btnGridSinFondo" runat="server" AutoPostBack="false" OnClick="btnEliminarArchivo_Click" UseSubmitBehavior="false">
                                                                            <Image ToolTip="Eliminar" Url="images/03.png">
                                                                            </Image>
                                                                            <ClientSideEvents Click="Mensaje" />
                                                                        </dx:ASPxButton>
                                                                    </DataItemTemplate>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn Caption="Editar" FieldName="editar" Visible="false" Name="editar" VisibleIndex="5">
                                                                    <Settings />
                                                                    <DataItemTemplate>
                                                                        <dx:ASPxButton ID="btnEditarDocumento" CssClass="btnGridSinFondo" runat="server" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnEditarDocumento_Click">
                                                                            <Image ToolTip="Editar" Url="images/39.png">
                                                                            </Image>
                                                                            <%--<ClientSideEvents Click="function(s,e){cbEditar.PerformCallback();}" />--%>
                                                                        </dx:ASPxButton>
                                                                    </DataItemTemplate>
                                                                </dx:GridViewDataColumn>
                                                            </Columns>

                                                        </dx:ASPxGridView>

                                                        <dx:ASPxGridView ID="GridInferiorVencimiento" Width="100%" ClientInstanceName="gSueldo" runat="server" KeyFieldName="id" AutoGenerateColumns="False" ForeColor="#00489e" Theme="Default" OnPageIndexChanged="GridInferior_PageIndexChanged" OnBeforeGetCallbackResult="GridInferior_BeforeGetCallbackResult" >
                                                            <SettingsBehavior AllowFocusedRow="true" AllowHeaderFilter="false" AllowSort="false" EnableRowHotTrack="true" />
                                                            <Settings GridLines="Vertical" VerticalScrollBarMode="Visible" />
                                                            <SettingsPager PageSize="10"  />
                                                            <Styles>
                                                                <Cell ForeColor="#00489e" HorizontalAlign="Center"></Cell>
                                                                <Header BackColor="#00489e" Font-Bold="true" ForeColor="White" HorizontalAlign="Center"></Header>
                                                                <FocusedRow BackColor="#EAEAEA" ForeColor="#00489e" Font-Bold="true"></FocusedRow>
                                                                <RowHotTrack BackColor="#d0d0d0" ></RowHotTrack>
                                                            </Styles>
                                                            <Columns>
                                                                <dx:GridViewDataHyperLinkColumn Caption="Documento" FieldName="ruta" Name="ruta" ShowInCustomizationForm="True" VisibleIndex="0">
                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                    <DataItemTemplate>
                                                                        <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnLoad="ASPxHyperLink1_Load"></dx:ASPxHyperLink>
                                                                    </DataItemTemplate>
                                                                    <CellStyle HorizontalAlign="Center">
                                                                    </CellStyle>
                                                                </dx:GridViewDataHyperLinkColumn>
                                                                <dx:GridViewDataTextColumn Width="35%" Caption="Descripcion" CellStyle-HorizontalAlign="Left" FieldName="descdocto" Name="descdocto" ShowInCustomizationForm="True">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataTextColumn Caption="Creación" FieldName="fechadcto" Name="fechadcto" ShowInCustomizationForm="True">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataTextColumn Caption="Vencimiento" FieldName="vence" Name="vence" ShowInCustomizationForm="True">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataTextColumn Caption="Id" FieldName="id" Name="id" ShowInCustomizationForm="True" Visible="false">
                                                                </dx:GridViewDataTextColumn>
                                                                <dx:GridViewDataColumn Caption="Eliminar" FieldName="eliminar" Name="eliminar" Visible="false">
                                                                    <Settings />
                                                                    <DataItemTemplate>
                                                                        <dx:ASPxButton ID="btnEliminarArchivo" CssClass="btnGridSinFondo" runat="server" AutoPostBack="false" OnClick="btnEliminarArchivo_Click" UseSubmitBehavior="false" VisibleIndex="6">
                                                                            <Image ToolTip="Eliminar" Url="images/03.png">
                                                                            </Image>
                                                                            <ClientSideEvents Click="Mensaje" />
                                                                        </dx:ASPxButton>
                                                                    </DataItemTemplate>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn Caption="Editar" FieldName="editar" Name="editar">
                                                                    <Settings />
                                                                    <DataItemTemplate>
                                                                        <dx:ASPxButton ID="btnEditarDocumento" CssClass="btnGridSinFondo" runat="server" AutoPostBack="false" UseSubmitBehavior="false" OnClick="btnEditarDocumento_Click">
                                                                            <Image ToolTip="Editar" Url="images/39.png">
                                                                            </Image>
                                                                            <%--<ClientSideEvents Click="function(s,e){cbEditar.PerformCallback();}" />--%>
                                                                        </dx:ASPxButton>
                                                                    </DataItemTemplate>
                                                                </dx:GridViewDataColumn>

                                                            </Columns>

                                                        </dx:ASPxGridView>
                                                    </div>
                                                </dx:PanelContent>
                                            </PanelCollection>

                                        </dx:ASPxCallbackPanel>
                                    </div>
                                    
                                </td>
                            </tr>
                        </table>
                        </div>
                        
                        
                        <%--GRID INFERIOR--%>
                        
                        

                    </dx:SplitterContentControl>
                        
                </ContentCollection>
            </dx:SplitterPane>
        </Panes>

    </dx:ASPxSplitter>

    <!-- POPUP CARGA ARCHIVOS-->
    <table>
        <tr>
            <td>
                <dx:ASPxHiddenField runat="server" ID="HiddenField" ClientInstanceName="HiddenField"></dx:ASPxHiddenField>
                <dx:ASPxCallbackPanel runat="server" ID="cbpEditar" ClientInstanceName="cbpEditar" OnCallback="cbpEditar_Callback">
                    <ClientSideEvents EndCallback="function(s,e) {popupCargaArchivo.Show(); lpCallBack.Hide();}" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxPopupControl ID="popupCargaArchivo" runat="server" CloseAction="CloseButton" ClientInstanceName="popupCargaArchivo" EnableViewState="False"
                                Modal="True" Width="600px" HeaderText="Carga Archivos" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
                                <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                                <ContentCollection>
                                    <dx:PopupControlContentControl runat="server">
                                        <dx:ASPxPanel ID="PanelCargaArchivo" runat="server">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">
                                                    <table>
                                                        <tr>
                                                            <td rowspan="8">
                                                                <div class="pcmSideSpacer">
                                                                </div>
                                                            </td>

                                                            <!-- NOMBRE EMPLEADO -->
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="lblEmpleado" runat="server" Text="Empleado">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxTextBox ForeColor="#00489e" Font-Names="verdana" ID="txtIdEmpleado" Font-Bold="False" runat="server" Width="100px" ClientInstanceName="txtIdEmpleado" ReadOnly="true">
                                                                    <Border BorderStyle="None" />
                                                                </dx:ASPxTextBox>
                                                                <dx:ASPxTextBox ForeColor="#00489e" Font-Names="verdana" ID="txtEmpleado" Font-Bold="False" runat="server" Width="350px" ClientInstanceName="txtEmpleado" ReadOnly="true">
                                                                    <Border BorderStyle="None" />
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                            <td rowspan="8" style="width: 10px">
                                                                <div class="pcmSideSpacer ">
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <!-- COMBOBOX TIPO DOCUMENTO-->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel2" runat="server" Text="Tipo Docto">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" ID="txtTipoDocto" runat="server" Width="350px" ValueType="System.String" ClientInstanceName="txtTipoDocto">
                                                                    <ClientSideEvents SelectedIndexChanged="valorTIpoDcto" Init="valorTIpoDcto" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                    <ValidationSettings EnableCustomValidation="true" ValidationGroup="entryGroup" SetFocusOnError="true" ErrorDisplayMode="Text"
                                                                        ErrorTextPosition="Bottom" CausesValidation="true">
                                                                        <RequiredField ErrorText="Debe Seleccionar Tipo Documento" IsRequired="true" />
                                                                        <RegularExpression ErrorText="Faltan Datos" />
                                                                        <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                                                            <ErrorTextPaddings PaddingLeft="0px" />
                                                                        </ErrorFrameStyle>

                                                                    </ValidationSettings>
                                                                </dx:ASPxComboBox>
                                                            </td>

                                                        </tr>

                                                        <!-- FECHA DOCUMENTO -->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel17" runat="server" Text="Fecha Docto">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxDateEdit ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" Width="350px" ID="txtFechaDocto" ClientInstanceName="txtFechaDocto" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" MinDate="2016-01-01">
                                                                    <ValidationSettings ValidationGroup ="entryGroup">
                                                                        <RequiredField IsRequired ="True" ErrorText="Fecha incorrecta"/>
                                                                    </ValidationSettings>
                                                                    <ClearButton DisplayMode="Always"></ClearButton>

                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                </dx:ASPxDateEdit>
                                                            </td>

                                                        </tr>


                                                        <!-- FECHA VENCIMIENTO DOCUMENTO -->
                                                        <tr id="tr_vencimiento" runat="server">
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel3" ClientInstanceName="lblVenveDcto" runat="server" Text="Vencimiento Docto">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxDateEdit ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Width="350px" Theme="MetropolisBlue" ID="txtVenceDocto" ClientInstanceName="txtVenceDocto" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" MinDate="2016-01-01">
                                                                    <ValidationSettings ValidationGroup="entryGroup">
                                                                       <RequiredField IsRequired="True" ErrorText="Fecha incorrecta"/>
                                                                    </ValidationSettings>
                                                                    <ClearButton DisplayMode="Always"></ClearButton>
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                    <ClientSideEvents DateChanged="ValidaRangoFechas" />
                                                                    
                                                                </dx:ASPxDateEdit>
                                                            </td>

                                                        </tr>

                                                        <!-- COMBOBOX DESCRIPCION-->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel1" ClientInstanceName="ASPxLabel1" runat="server" Text="Descripción">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText ">
                                                                <dx:ASPxComboBox ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" ID="cbxDescripcion" runat="server" NullText="Ingrese descripción del Archivo" Width="350px" ClientInstanceName="cbxDescripcion">
                                                                <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                    
                                                                </dx:ASPxComboBox>
                                                                <%--<dx:ASPxTextBox ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" ID="txtDescripcion" runat="server" NullText="Ingrese descripción del Archivo" Width="350px" ClientInstanceName="txtDescripcion">
                                                                </dx:ASPxTextBox>--%>
                                                            </td>

                                                        </tr>

                                                        <!-- CHECKBOX SI ES DOCUMENTO VIGENTE -->
                                                        <%--<tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel18" runat="server" Text="Docto Vigente">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCheckBox">
                                                                <dx:ASPxCheckBox ForeColor="#00489e" Font-Names="verdana" ID="cbVigente" runat="server" CheckState="Unchecked" ClientInstanceName="cbVigente">
                                                                </dx:ASPxCheckBox>
                                                            </td>
                                                        </tr>--%>
                                                        <tr>
                                                            <!-- CARGA DE ARCHIVO -->
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel4" runat="server" Text="Archivo">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td>
                                                                <dx:ASPxUploadControl ForeColor="#00489e" Font-Names="verdana" Theme="MetropolisBlue" EnableTheming="True" CssClass="rounded txtbox text-white font-weight-bold" ID="UploadControl" runat="server" ClientInstanceName="UploadControl" Width="350"
                                                                    NullText="Selecciona Archivo" FileUploadMode="OnPageLoad" UploadMode="Advanced" ShowProgressPanel="True" AutoStartUpload="true"
                                                                    OnFileUploadComplete="UploadControl_FileUploadComplete">
                                                                    <BrowseButtonStyle BackColor="#ff1049" ForeColor="White" CssClass="rounded txtbox text-white font-weight-bold">
                                                                    </BrowseButtonStyle>
                                                                    <Paddings PaddingLeft="10" />
                                                                    <AdvancedModeSettings EnableMultiSelect="false" EnableFileList="True" EnableDragAndDrop="True" />
                                                                    <ValidationSettings MaxFileSize="8000000" AllowedFileExtensions=".jpg, .png, .pdf">
                                                                    </ValidationSettings>
                                                                    <ClientSideEvents FileUploadComplete="function (s, e) {
                                                        var text = s.GetText(e.inputIndex).replace(/\s|C:\\fakepath\\/g, '');
                                                        txtArchivoCargado.SetText('¡ARCHIVO ' + text + ' CARGADO!'); }" />
                                                                </dx:ASPxUploadControl>
                                                            </td>
                                                        </tr>
                                                        <%--HYPERLINK PARA VER EL ARCHIVO--%>
                                                        <tr>
                                                            <td><dx:ASPxHyperLink runat="server" ID="hlVerArchivo" Text=""></dx:ASPxHyperLink></td>
                                                            <td colspan="2">
                                                                <dx:ASPxTextBox ForeColor="#00489e" Font-Bold="true" ID="txtArchivoCargado" ReadOnly="true" runat="server" Width="350px" ClientInstanceName="txtArchivoCargado" Text="Falta Cargar Archivo">
                                                                    <Border BorderStyle="None" />
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>


                                                        <!-- BOTONES -->
                                                        <tr>
                                                            <td colspan="2" style="width: 10px">
                                                                <div class="pcmSideSpacer ">
                                                                </div>
                                                            </td>
                                                            <td >
                                                                <div class="container-fluid">
                                                                    <table style="width: 100%">
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <dx:ASPxButton Font-Names="verdana" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" ID="btnGrabarArchivo" runat="server" Text="Guardar" OnClick="btnGrabarArchivo_Click" AutoPostBack="false" UseSubmitBehavior="false">
                                                                                    <ClientSideEvents Click="OnClickConsultaEmpresa" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                            <td style="text-align: left;">
                                                                                <dx:ASPxButton Font-Names="verdana" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" ID="btnCerrar" runat="server" Text="Cerrar" AutoPostBack="false" UseSubmitBehavior="false">
                                                                                    <ClientSideEvents Click="function(s, e) { popupCargaArchivo.Hide();}" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
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
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
                

            </td>

        </tr>
    </table>

    <!-- POPUP CARGA ARCHIVOS-->
    <table>
        <tr>
            <td>
                <dx:ASPxHiddenField runat="server" ID="ASPxHiddenField1" ClientInstanceName="HiddenField"></dx:ASPxHiddenField>
                <dx:ASPxCallbackPanel runat="server" ID="cbpDescargarArchivos" ClientInstanceName="cbpDescargarArchivos" OnCallback="cbpDescargarArchivos_Callback">
                    <ClientSideEvents EndCallback="function(s,e) {popUpDescargarArchivos.Show(); lpCallBack.Hide();}" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxPopupControl ID="popUpDescargarArchivos" runat="server" CloseAction="CloseButton" ClientInstanceName="popUpDescargarArchivos" EnableViewState="False"
                                Modal="True" Width="600px" HeaderText="Descarga de Documentos" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Metropolis" ShowPageScrollbarWhenModal="True">
                                <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                                <ContentCollection>
                                    <dx:PopupControlContentControl runat="server">
                                        <dx:ASPxPanel ID="ASPxPanel1" runat="server">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">
                                                    <table>
                                                        <tr>
                                                            <td rowspan="8">
                                                                <div class="pcmSideSpacer">
                                                                </div>
                                                            </td>

                                                            <!-- NOMBRE EMPLEADO -->
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel5" runat="server" Text="Empleado">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxTextBox ForeColor="#00489e" Font-Names="verdana" ID="txtIdEmpleadoDescargarDocumento" Font-Bold="False" runat="server" Width="100px" ClientInstanceName="txtIdEmpleadoDescargarDocumento" ReadOnly="true">
                                                                    <Border BorderStyle="None" />
                                                                </dx:ASPxTextBox>
                                                                <dx:ASPxTextBox ForeColor="#00489e" Font-Names="verdana" ID="txtEmpleadoDescargarDocumento" Font-Bold="False" runat="server" Width="350px" ClientInstanceName="txtEmpleadoDescargarDocumento" ReadOnly="true">
                                                                    <Border BorderStyle="None" />
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                            <td rowspan="8" style="width: 10px">
                                                                <div class="pcmSideSpacer ">
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <!-- COMBOBOX TIPO DOCUMENTO-->
                                                        <tr>
                                                            <td class="pcmCellCaption">
                                                                <dx:ASPxLabel CssClass="font-weight-bold" ForeColor="#00489e" Font-Names="verdana" ID="ASPxLabel6" runat="server" Text="Tipo Docto">
                                                                </dx:ASPxLabel>
                                                            </td>
                                                            <td class="pcmCellCaption">:</td>
                                                            <td class="pcmCellText">
                                                                <dx:ASPxComboBox ForeColor="#00489e" Font-Names="verdana" CssClass="rounded" Theme="MetropolisBlue" ID="cbxDescargarDocumento" runat="server" Width="350px" ValueType="System.String" ClientInstanceName="cbxDescargarDocumento">
                                                                    <ClientSideEvents SelectedIndexChanged="valorTIpoDcto" />
                                                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                                                </dx:ASPxComboBox>
                                                            </td>

                                                        </tr>

                                                        <!-- BOTONES -->
                                                        <tr>
                                                            <td colspan="2" style="width: 10px">
                                                                <div class="pcmSideSpacer ">
                                                                </div>
                                                            </td>
                                                            <td>
                                                                <div class="container-fluid">
                                                                    <table style="width: 100%">
                                                                        <tr>
                                                                            <td style="text-align: center;">
                                                                                <dx:ASPxButton Font-Names="verdana" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" ID="btnDescargar" runat="server" Text="Descargar" OnClick="btnDescargar_Click" AutoPostBack="false" UseSubmitBehavior="false">
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                            <td style="text-align: left;">
                                                                                <dx:ASPxButton Font-Names="verdana" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" ID="ASPxButton2" runat="server" Text="Cerrar" AutoPostBack="false" UseSubmitBehavior="false">
                                                                                    <ClientSideEvents Click="function(s, e) { popUpDescargarArchivos.Hide();}" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
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
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
                

            </td>

        </tr>
    </table>

    <%--POPUP EVALUACIONES--%>
    <table>
        <tr>
            <td>
                <dx:ASPxCallbackPanel runat="server" ID="cbpVerEvaluaciones" ClientInstanceName="cbpverevaluaciones" OnCallback="cbpVerEvaluaciones_Callback">
                    <ClientSideEvents EndCallback="function(s,e) {popupevaluaciones.Show();}" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxPopupControl ID="popUpEvaluaciones" runat="server" CloseAction="CloseButton" ClientInstanceName="popupevaluaciones" EnableViewState="False"
                                Modal="True" Width="800px" HeaderText="Registro de evaluaciones" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" 
                                Theme="Metropolis" ShowPageScrollbarWhenModal="True">
                                <ClientSideEvents Init="function(s,e){lpCallBack.Hide();}" />
                                <HeaderStyle Font-Names="verdana" BackColor="#00489e" HorizontalAlign="Center" Font-Bold="true" ForeColor="White" />
                                <ContentCollection>
                                    <dx:PopupControlContentControl runat="server">
                                        <dx:ASPxPanel ID="ASPxPanel2" runat="server">
                                            <PanelCollection>
                                                <dx:PanelContent runat="server">
                                                    <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridVerEvaluacion" runat="server"
                                                        ClientInstanceName="gridverevaluacion" KeyFieldName="rut" Font-Size="Small" 
                                                        Settings-HorizontalScrollBarMode="Visible" OnPageIndexChanged="gridVerEvaluacion_PageIndexChanged">
                                                        <Styles>
                                                            <Row Wrap="True" HorizontalAlign="Center"></Row>
                                                            <Cell Wrap="True" HorizontalAlign="Center" Paddings-Padding="0"></Cell>
                                                            <FixedColumn Wrap="True" HorizontalAlign="Center" Paddings-Padding="0"></FixedColumn>
                                                            <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" ForeColor="#00489e"></Header>
                                                            <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#d0d0d0"></FocusedRow>
                                                            <RowHotTrack ForeColor="#00489e" BackColor="#d0d0d0"></RowHotTrack>
                                                        </Styles>
                                                        <SettingsPager PageSize="8">
                                                        </SettingsPager>
                                                    </dx:ASPxGridView>
                                                    <div style="width:600px; margin-left:500px">
                                                        <dx:ASPxButton runat="server" ID="btnOkEvaluacion" EnableTheming="false" CssClass="estiloBtnBuscar" Font-Size="Small" Text="Cerrar" AutoPostBack="false" UseSubmitBehavior="false">
                                                            <ClientSideEvents Click="function(s,e){popupevaluaciones.Hide();}" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                    
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

    <style type="text/css">
        .uploadContainer {
            float: left;
            margin-right: 80px;
        }

        .contentFooter {
            clear: both;
            padding-top: 20px;
        }
    </style>

</asp:Content>
