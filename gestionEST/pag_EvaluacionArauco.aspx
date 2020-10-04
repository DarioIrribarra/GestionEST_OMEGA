<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_EvaluacionArauco.aspx.vb" Inherits="gestionEST.pag_EvaluacionArauco" %>

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

        .estiloBtnGuardarEvaluacion {
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

            .estiloBtnGuardarEvaluacion:hover {
                /*border: 2px solid #007bff;*/
                background-color:#00489E;
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

        .btnGridSinFondo {
            background: none;
            border: none;
        }
    </style>

    <script type="text/javascript">
        function OnClickConsultaEmpresa(s, e) {
            var validado = false;
            if (ASPxClientEdit.ValidateGroup('entryGroup') == true) {
                if (cbxunidaduevaluacion.GetSelectedIndex() > 0) {
                    validado = true;
                } else {
                    alert('Debe seleccionar una unidad')
                    validado = false;
                }
            }

            if (validado == true) {
                e.processOnServer = true;
            } else {
                e.processOnServer = false;
            }
        }


        function guardaMemo(s, e) {
            //MUESTRO POPUP DE CARGA
            lpCallBack.Show();

            //OBTENGO INDEX
            var regIndexFila = /.+_cell([0-9]+)/
            var resultadoIndexFila = regIndexFila.exec(s.name);
            var fila = resultadoIndexFila[1];

            //Variables para tomar valores por javascript
            var Nota01 = "";
            var Nota02 = "";
            var Nota03 = "";
            var Nota04 = "";
            var NotaC01 = "";
            var NotaC02 = "";
            var NotaC03 = "";
            var NotaC04 = "";
            var esRecomendable = "";
            var Observacion = "";
            var Identificador = "";
            var Index = resultadoIndexFila[1];
            
            //Try catch para ver si la columna nota01 está visible. Si no lo está, solo se capturan las de concepto
            //LAS LLENO CON VALORES DEL LADO CLIENTE
            try {
                Nota01 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_9_Nota01_" + fila + "_I").value;
                Nota02 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_10_Nota02_" + fila + "_I").value;
                Nota03 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_11_Nota03_" + fila + "_I").value;
                Nota04 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_12_Nota04_" + fila + "_I").value;
                
            } catch (e) {
                Identificador = "Conceptos";
            }

            try {
                NotaC01 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_14_NotaC01_" + fila + "_I").value;
                NotaC02 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_15_NotaC02_" + fila + "_I").value;
                NotaC03 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_16_NotaC03_" + fila + "_I").value;
                NotaC04 = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_17_NotaC04_" + fila + "_I").value;
            } catch (e) {
                Identificador = "Numeros";
            }
            esRecomendable = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_18_esRecomendado_" + fila + "_I").value;
            Observacion = document.getElementById("ASPxSplitter1_ContenidoPagina_cbpGrid_gridEvaluaciones_cell" + fila + "_19_Observacion_" + fila + "_I").value;

            //ARREGLO QUE ALMACENA LOS VALORES
            var arregloValores = []
            if (Identificador == "Numeros") {
                arregloValores = [Identificador, Nota01, Nota02, Nota03, Nota04, esRecomendable, Observacion, Index]
            } else {
                arregloValores = [Identificador, NotaC01, NotaC02, NotaC03, NotaC04, esRecomendable, Observacion, Index]
            }

            //JOIN PARA QUE EL SERVIDOR NO SEPARE EN COMAS
            var valoresEnString = arregloValores.join("~")

            //SE LLAMA A CALLBACK PANE ENTREGANDO STRING DE VALORES
            cbpGrid.PerformCallback(valoresEnString);

        }

        //FUNCIÓN QUE VERIFICA QUE NO SE INGRESE '~'
        function teclaIngresada(s, e) {
            //Obtengo valor de caja de texto
            var textoIngresado = s.GetInputElement().value;

            //COMPARO VALOR CON LETRA
            var regTeclaIngresada = /([~])/
            var resultadoTeclaIngresada = regTeclaIngresada.exec(textoIngresado);
            try {
                var valor = resultadoTeclaIngresada[0];
                alert("El caracter '~' no está permitido\nEdite el texto de Observación por favor");
            } catch (e) {

            }
            
        }

        //FUNCIÓN QUE ESCONDE EL POPUP DE CARGANDO
        function finalizaGuardado(s, e) {
            lpCallBack.Hide();
            alert("Evaluación registrrada");
        }
    </script>

        <dx:ASPxCallbackPanel runat="server" ID="cbpPanelIzquierdo" ClientInstanceName="cbpPanelIzquierdo">
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
                            <div style="width: 25%;">
                                <label class="text-white font-weight-bold" style="font-family: Verdana;">
                                    Unidad 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" AllowNull="true" Width="83%" CssClass="rounded" ID="cbxUnidad" runat="server" ValueType="System.String" ClientInstanceName="cbxunidaduevaluacion" Theme="MetropolisBlue" OnLoad="cbxUnidad_Load">
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="d-flex justify-content-end" style="padding: 5px 7px 5px 7px; margin-right: -7px">
                            <dx:ASPxButton ID="btnConsultar" CssClass="estiloBtnBuscar" Text="Buscar" runat="server" AutoPostBack="False" OnClick="btnConsultar_Click" UseSubmitBehavior="false">
                                <ClientSideEvents Click="OnClickConsultaEmpresa" />
                            </dx:ASPxButton>
                        </td>

                    </tr>
                </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
        <hr style="background-color:red" />
    <table>
        <tr id="tr_excel" runat="server" style="visibility:hidden">
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage1" runat="server" ShowLoadingImage="true" ImageUrl="images/16.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:aspxbutton ID="btnExcel" runat="server" ImagePosition="Left" EnableTheming="false"
                    CssClass="estiloBtnSubmenusExportar" Text="Exportar a Excel" 
                    AutoPostBack="False" OnClick="btnExcel_Click">
                    
                </dx:aspxbutton>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContenidoPagina" runat="server">
<dx:ASPxLoadingPanel runat="server" ID="lpCallBack" Text="Guardando Evaluación" ClientInstanceName="lpCallBack" Modal="true" Theme="MetropolisBlue"></dx:ASPxLoadingPanel>
    <dx:ASPxCallbackPanel runat="server" ID="cbpGrid" ClientInstanceName="cbpGrid" OnCallback="cbpGrid_Callback" EnableCallbackAnimation="false">
        <ClientSideEvents EndCallback="finalizaGuardado" />
        <PanelCollection>
            <dx:PanelContent>
                <dx:ASPxGridView
                    Width="100%"
                    ForeColor="#00489e"
                    ID="gridEvaluaciones"
                    ClientInstanceName="gridevaluaciones"
                    runat="server"
                    KeyFieldName="rut"
                    Theme="Default"
                    Visible ="false"
                    AutoGenerateColumns="False" 
                    EnableCallbackAnimation="false">
                    <Settings
                        ShowFilterRow="False"
                        ShowGroupPanel="true"
                        VerticalScrollBarMode="Hidden"
                        VerticalScrollableHeight="300" />
                    <Styles>

                        <FixedColumn ForeColor="#00489E" HorizontalAlign="Center"></FixedColumn>
                        <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" Font-Size="Small" BackColor="#F2F2F2" ForeColor="#00489e"></Header>
                        <Cell Font-Size="Small" HorizontalAlign="Left"></Cell>
                        <FocusedRow BackColor="#EAEAEA" ForeColor="#00489e"></FocusedRow>
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
                        <dx:GridViewDataTextColumn Caption="Rut" FieldName="rut" Name="rut" ShowInCustomizationForm="True" VisibleIndex="0" Width="7%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Nombre" FieldName="nombre" Name="nombre" ShowInCustomizationForm="True" VisibleIndex="1" Width="150%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Fecha" FieldName="ames" Name="ames" ShowInCustomizationForm="True" VisibleIndex="2" Width="60%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Id Planta" FieldName="id" Name="id" ShowInCustomizationForm="True" VisibleIndex="3" Width="60%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="idArea" Name="idArea" ShowInCustomizationForm="True" Visible ="false">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Area" FieldName="area" Name="area" ShowInCustomizationForm="True" VisibleIndex="4" Width="60%" >
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Cargo" FieldName="cargo" Name="cargo" ShowInCustomizationForm="True" VisibleIndex="5" Width="60%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Evaluador" FieldName="userEvalua" Name="userEvalua" ShowInCustomizationForm="True" VisibleIndex="6" Width="60%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Tipo Evaluacion" FieldName="tipoEvaluacion" Name="tipoEvaluacion" ShowInCustomizationForm="True" VisibleIndex="7" Visible="false" Width="60%">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Procedimientos</br>de</br>seguridad" FieldName="Nota01" Name="Nota01" ShowInCustomizationForm="True" VisibleIndex="8" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="Nota01" ClientInstanceName="cbxevaluacion" ValueType="System.String" Theme="Aqua" Width="120px"  HorizontalAlign="Center" OnInit="cbxEvaluacion_Init" OnLoad="cbxEvaluacion_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Responsabilidad</br>en</br>tareas" FieldName="Nota02" Name="Nota02" ShowInCustomizationForm="True" VisibleIndex="9" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="Nota02" ClientInstanceName="cbxevaluacion" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Center" OnInit="cbxEvaluacion_Init" OnLoad="cbxEvaluacion_Load">
                                    <ItemStyle HorizontalAlign="Center" />
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Realización</br>de</br>labores" FieldName="Nota03" Name="Nota03" ShowInCustomizationForm="True" VisibleIndex="10" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="Nota03" ClientInstanceName="cbxevaluacion" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Center" OnInit="cbxEvaluacion_Init" OnLoad="cbxEvaluacion_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Relación</br>respetuosa" FieldName="Nota04" Name="Nota04" ShowInCustomizationForm="True" VisibleIndex="11" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="Nota04" ClientInstanceName="cbxevaluacion" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Center" OnInit="cbxEvaluacion_Init" OnLoad="cbxEvaluacion_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Promedio" FieldName="Promedio" Name="Promedio" ShowInCustomizationForm="True" VisibleIndex="12" Width="100%">
                            <CellStyle HorizontalAlign="Center"/>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Dominio</br>técnico" FieldName="NotaC01" Name="NotaC01" ShowInCustomizationForm="True" VisibleIndex="13">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="NotaC01" ClientInstanceName="cbxevaluacionconceptual" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Left" OnInit="cbxEvaluacionConceptual_Init" OnLoad="cbxEvaluacionConceptual_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Superación</br>de</br>obstáculos" FieldName="NotaC02" Name="NotaC02" ShowInCustomizationForm="True" VisibleIndex="14" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="NotaC02" ClientInstanceName="cbxevaluacionconceptual" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Left" OnInit="cbxEvaluacionConceptual_Init" OnLoad="cbxEvaluacionConceptual_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Cumplimiento</br>de</br>reglas" FieldName="NotaC03" Name="NotaC03" ShowInCustomizationForm="True" VisibleIndex="15" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="NotaC03" ClientInstanceName="cbxevaluacionconceptual" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Left" OnInit="cbxEvaluacionConceptual_Init" OnLoad="cbxEvaluacionConceptual_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Actitud</br>respetuosa" FieldName="NotaC04" Name="NotaC04" ShowInCustomizationForm="True" VisibleIndex="16" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="NotaC04" ClientInstanceName="cbxevaluacionconceptual" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Left" OnInit="cbxEvaluacionConceptual_Init" OnLoad="cbxEvaluacionConceptual_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Recomendado" FieldName="esRecomendado" Name="esRecomendado" ShowInCustomizationForm="True" VisibleIndex="17" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxComboBox runat="server" ID="esRecomendado" ClientInstanceName="cbxrecomendado" ValueType="System.String" Theme="Aqua" Width="120px" HorizontalAlign="Left" OnInit="cbxRecomendado_Init" OnLoad="cbxRecomendado_Load">
                                </dx:ASPxComboBox>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Observacion" FieldName="Observacion" Name="Observacion" ShowInCustomizationForm="True" VisibleIndex="18" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxMemo runat="server" ID="Observacion" ClientInstanceName="txtobservacion" ValueType="System.String" Theme="Aqua" Width="150px" OnLoad="txtObservacion_Load">
                                    <ClientSideEvents UserInput="teclaIngresada"  />
                                </dx:ASPxMemo>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Guardar" FieldName="btnGuardar" Name="btnGuardar" ShowInCustomizationForm="True" VisibleIndex="19" Width="100%">
                            <DataItemTemplate>
                                <dx:ASPxButton runat="server" ID="btnGuardarEvaluacion" CssClass="estiloBtnGuardarEvaluacion" Text="Guardar" AutoPostBack="false">
                                    <ClientSideEvents Click="guardaMemo"  />
                                </dx:ASPxButton>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>                        
                    </Columns>
                </dx:ASPxGridView>

                <dx:ASPxGridView runat="server" ID="EvaluacionExcel" AutoGenerateColumns="true" Visible="false">

                </dx:ASPxGridView>

                <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="EvaluacionExcel">
                    <Styles>
                        <Default Font-Names="Arial" Font-Size="Medium">
                        </Default>
                        <Header Font-Names="Arial" Font-Size="Medium">
                        </Header>
                    </Styles>

                </dx:ASPxGridViewExporter>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
</asp:Content>
