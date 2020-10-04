<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_reportes.aspx.vb" Inherits="gestionEST.pag_reportes" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register Assembly="DevExpress.XtraCharts.v18.1.Web, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts.Web" TagPrefix="dxchartsui" %>

<%@ Register assembly="DevExpress.XtraCharts.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraCharts" tagprefix="cc1" %>

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

    <script type="text/javascript">

        function OnClickVerBotones(s, e) {
            e.processOnServer = true;
        }

        function OnClickConsultaEmpresa(s, e) {
            if (ASPxClientEdit.ValidateGroup('entryGroup')) {
                lpCallBack.Show();
                e.processOnServer = true;
            }
            else {
                e.processOnServer = false;
            }
        }

        function ReiniciarDotacionMensual(s, e) {
            cbckDotacionmensual.PerformCallback();
        }

        function CambiarUnidadesPagina() {
            cbpPanelIzquierdo.PerformCallback();
        }

</script>

    <%--BARRA AZUL--%>
    <dx:ASPxCallbackPanel runat="server" ID="cbpPanelIzquierdo" ClientInstanceName="cbpPanelIzquierdo" OnCallback="cbpPanelIzquierdo_Callback">
        <PanelCollection>
            <dx:PanelContent>
                <table>
                    <tr>
                        <td>
                            <label class="text-white font-weight-bold">Reportes e Informes</label>
                            <dx:ASPxCallback runat="server" ID="cbckDotacionmensual" ClientInstanceName="cbckDotacionmensual" OnCallback="cbckDotacionmensual_Callback">
                            </dx:ASPxCallback>
                        </td>
                    </tr>

                    <tr>
                        <td class="d-flex">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold">
                                    Tipo 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" SelectedIndex="0" Theme="MetropolisBlue" Width="74%"
                                    EnableTheming="true" CssClass="rounded" ID="cbxTipo" runat="server"
                                    ClientInstanceName="cbxTipo"
                                    OnSelectedIndexChanged="cbxTipo_SelectedIndexChanged">
                                    <ClientSideEvents SelectedIndexChanged="OnClickVerBotones" />
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                    <Items>
                                        <dx:ListEditItem Text="SELECCIONE" Value="0"></dx:ListEditItem>
                                        <dx:ListEditItem Text="DOTACION SEMANAL" Value="1"></dx:ListEditItem>
                                        <dx:ListEditItem Text="DOTACION MENSUAL" Value="2"></dx:ListEditItem>
                                        <dx:ListEditItem Text="DOTACION ONLINE" Value="3"></dx:ListEditItem>
                                        <dx:ListEditItem Text="DOTACION CAUSAL ONLINE" Value="4"></dx:ListEditItem>
                                        <dx:ListEditItem Text="POR GÉNERO" Value="5"></dx:ListEditItem>
                                        <dx:ListEditItem Text="HISTORIAL CONTRATOS" Value="6"></dx:ListEditItem>
                                        <dx:ListEditItem Text="REPORTE ARAUCO" Value="7"></dx:ListEditItem>
                                        <%--<dx:ListEditItem Text="REMUNERACIONES" Value="6"></dx:ListEditItem>--%>
                                        
                                    </Items>
                                </dx:ASPxComboBox>
                            </div>
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

                    <tr id="tr_unidad" runat="server">
                        <td class="d-flex">

                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold">
                                    Unidad 
                                </label>
                            </div>
                            <div id="divUnidad" class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" NullText="SELECCIONE" Theme="MetropolisBlue" Width="77%" EnableTheming="true" CssClass="rounded" ID="txtUnidadEmpresa" runat="server"
                                    ValueType="System.String" ClientInstanceName="txtUnidadEmpresa" OnLoad="txtUnidadEmpresa_Load">
                                    <ClientSideEvents SelectedIndexChanged="ReiniciarDotacionMensual" />
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                    <ValidationSettings
                                        EnableCustomValidation="true"
                                        ValidationGroup="entryGroup"
                                        SetFocusOnError="true"
                                        ErrorDisplayMode="Text"
                                        ErrorTextPosition="Bottom"
                                        CausesValidation="true">
                                        <RequiredField ErrorText="Seleccione Unidad" IsRequired="true" />
                                        <RegularExpression ErrorText="Faltan Datos" />
                                        <ErrorFrameStyle ForeColor="Red" Font-Size="10px">
                                            <ErrorTextPaddings PaddingLeft="0px" />
                                        </ErrorFrameStyle>
                                    </ValidationSettings>
                                </dx:ASPxComboBox>
                            </div>


                        </td>
                    </tr>
                    <tr>
                        <td class="d-flex ">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold" runat="server" id="lblDesde">
                                     
                                </label>
                            </div>
                        </td>
                    </tr>

                    <tr id="tr_mes" runat="server">
                        <td class="d-flex ">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold">
                                    Mes 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" Theme="MetropolisBlue" Width="74%" EnableTheming="true" CssClass="rounded" ID="txtMesEmpresa" runat="server"
                                    ValueType="System.String" ClientInstanceName="txtMesEmpresa">
                                    <ClientSideEvents SelectedIndexChanged="ReiniciarDotacionMensual" />
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>
                    <tr id="tr_ano" runat="server">
                        <td class="d-flex ">
                            <div style="width: 20%;">
                                <label class="text-white font-weight-bold">
                                    Año 
                                </label>
                            </div>
                            <div class="d-flex justify-content-end">
                                <dx:ASPxComboBox ForeColor="#00489e" Theme="MetropolisBlue" Width="74%" EnableTheming="true" CssClass="rounded" ID="txtAnoEmpresa" runat="server"
                                    ValueType="System.String" ClientInstanceName="txtAnoEmpresa">
                                    <ClientSideEvents SelectedIndexChanged="ReiniciarDotacionMensual" />
                                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                                </dx:ASPxComboBox>
                            </div>
                        </td>
                    </tr>
                    <tr id="tr_consultar" runat="server">
                        <td class="d-flex justify-content-end" style="padding: 7px">
                            <dx:ASPxButton ID="btnConsultar" runat="server" EnableTheming="false"
                                CssClass="estiloBtnBuscar"
                                Style="background-color: #ff1049; width: 60%;" Font-Size="Small" Text="Buscar"
                                AutoPostBack="False" OnClick="btnConsultar_Click">
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
        <tr id="tr_excel" runat="server">
            <td class="rounded d-inline-flex align-content-center">
                <dx:ASPxImage ID="ASPxImage1" runat="server" ShowLoadingImage="true" ImageUrl="images/16.png">
                </dx:ASPxImage>
            </td>
            <td class="d-inline-flex align-content-center">
                <dx:aspxbutton ID="btnExcel" runat="server" ImagePosition="Left" EnableTheming="false"
                    CssClass="estiloBtnSubmenusExportar" 
                    Style="background-color: #00489e; width: 100%;" Text="Exportar a Excel" 
                    OnClick="btnExcel_Click" AutoPostBack="False" >
                    
                </dx:aspxbutton>
            </td>
        </tr>
    </table>
    
</asp:Content>
<asp:Content ID="Content4"  ContentPlaceHolderID="ContenidoPagina" runat="server">
    
    <dx:ASPxLoadingPanel runat="server" ID="lpCallBack" Text="Cargando Reporte..." ClientInstanceName="lpCallBack" Modal="true" Theme="MetropolisBlue"></dx:ASPxLoadingPanel>

    <!-- GRID INFO -->
    <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridDatos" runat="server"
        ClientInstanceName="gridDatos" KeyFieldName="id" Font-Size="Small" Settings-HorizontalScrollBarMode="Visible" OnPageIndexChanged="gridDatos_PageIndexChanged">
        <Styles>
            <Row Wrap="True" HorizontalAlign="Center" ></Row>
            <Cell Wrap="True" HorizontalAlign="Center" Paddings-Padding="0" ></Cell>
            <FixedColumn Wrap="True" HorizontalAlign="Center" Paddings-Padding="0"></FixedColumn>
            <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" ForeColor="#00489e"></Header>
            <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#d0d0d0"></FocusedRow>
            <RowHotTrack ForeColor="#00489e"  BackColor="#d0d0d0"></RowHotTrack>
        </Styles>
        <SettingsPager PageSize="8">
        </SettingsPager>


    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="gridDatos">
        <Styles>
            <Default Font-Names="Arial" Font-Size="Medium">
            </Default>
            <Header Font-Names="Arial" Font-Size="Medium">
            </Header>
        </Styles>

    </dx:ASPxGridViewExporter>

    <%--GRID SEMANAL--%>
    <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridSemanal"  runat="server" 
        ClientInstanceName="gridSemanal" KeyFieldName="id" Font-Size="Small" Settings-HorizontalScrollBarMode="Visible" OnPageIndexChanged="gridDatos_PageIndexChanged">
        <Styles>
            <Row Wrap="True" HorizontalAlign="Center" ></Row>
            <Cell Wrap="True" HorizontalAlign="Center" Paddings-Padding="0" ></Cell>
            <FixedColumn Wrap="True" HorizontalAlign="Center" Paddings-Padding="0"></FixedColumn>
            <Header Wrap="True" HorizontalAlign="Left" Font-Bold="true" ForeColor="#00489e"></Header>
            <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#d0d0d0"></FocusedRow>
            <RowHotTrack ForeColor="#00489e"  BackColor="#d0d0d0"></RowHotTrack>
        </Styles>
        <SettingsPager PageSize="8">
        </SettingsPager>


    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter ID="gridExportarSemanal" runat="server" GridViewID="gridSemanal">
        <Styles>
            <Default Font-Names="Arial" Font-Size="Medium">
            </Default>
            <Header Font-Names="Arial" Font-Size="Medium">
            </Header>
        </Styles>

    </dx:ASPxGridViewExporter>

    <%--GRID DOTACION MENSUAL--%>
    <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridDotacionMensual"  runat="server" 
        ClientInstanceName="gridDotacionMensual" KeyFieldName="id" Font-Size="Small" Settings-HorizontalScrollBarMode="Visible" 
        OnPageIndexChanged="gridDatos_PageIndexChanged"  >
        <Styles>
            <Row Wrap="True" HorizontalAlign="Center" ></Row>
            <Cell Wrap="True" HorizontalAlign="Center" Paddings-Padding="0" ></Cell>
            <FixedColumn Wrap="True" HorizontalAlign="Center" Paddings-Padding="0"></FixedColumn>
            <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" ForeColor="#00489e"></Header>
            <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#d0d0d0"></FocusedRow>
            <RowHotTrack ForeColor="#00489e"  BackColor="#d0d0d0"></RowHotTrack>
        </Styles>
        <SettingsPager PageSize="8">
        </SettingsPager>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter ID="GridExportarDotacionMensual" runat="server" GridViewID="gridDotacionMensual">
        <Styles>
            <Default Font-Names="Arial" Font-Size="Medium">
            </Default>
            <Header Font-Names="Arial" Font-Size="Medium">
            </Header>
        </Styles>
    </dx:ASPxGridViewExporter>

        <%--REPORTE ONLINE DOTACION--%>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" OnUnload="UpdatePanel1_Unload">
        <ContentTemplate>
            <table class="OptionsTable OptionsBottomMargin">
                <tr>
                    <td colspan="2">
                        <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="ChartType" runat="server" OnValueChanged="ChartType_ValueChanged" 
                            AutoPostBack="True" ValueType="System.String" Caption="Tipo Gráfico">
                            <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" runat="server" Text="Mostrar columna Total General" AutoPostBack="True" Theme="MetropolisBlue"
                            ID="ShowColumnGrandTotals" OnCheckedChanged="ShowColumnGrandTotals_CheckedChanged"
                            Wrap="False" />
                    </td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" runat="server" Text="Generar Series de Columnas" AutoPostBack="True" Theme="MetropolisBlue"
                            ID="ChartDataVertical" OnCheckedChanged="ChartDataVertical_CheckedChanged" Wrap="False" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" ID="PointLabels" runat="server" Text="Mostrar rotulos de Datos" Theme="MetropolisBlue" OnCheckedChanged="PointLabels_CheckedChanged"
                            AutoPostBack="True" Wrap="False" />
                    </td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" runat="server" Checked="True" Text="Mostrar Fila Total General" AutoPostBack="True" Theme="MetropolisBlue"
                            ID="ShowRowGrandTotals" OnCheckedChanged="ShowRowGrandTotals_CheckedChanged"
                            Wrap="False" />
                    </td>
                </tr>
            </table>
            <dx:ASPxPivotGrid ID="ASPxPivotGrid" runat="server" DataSourceID="SqlDataSource1"
                EnableCallBacks="False" OnPreRender="ASPxPivotGrid_PreRender" Width="100%" ClientIDMode="AutoID">
                <Styles>
                    

                    <CellsAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center"></CellsAreaStyle>
                    <ColumnFieldValuesAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center" ></ColumnFieldValuesAreaStyle>
                     <FilterItemsAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center"  ></FilterItemsAreaStyle>
                    <RowFieldValuesAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Left" ></RowFieldValuesAreaStyle>
                    <DataAreaStyle>
                        
                    </DataAreaStyle>
                    <HeaderStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center" />
                </Styles>
                <Fields>
                    <dx:PivotGridField FieldName="dotacion" ID="fielddotacion" Area="DataArea"
                        AreaIndex="0" Caption="Dotacion" />
                    <dx:PivotGridField FieldName="descripcion" ID="fielddescripcion" Area="RowArea"
                        AreaIndex="0" Caption="Unidad" />
                    <dx:PivotGridField FieldName="ames" ID="fieldOrderMonth" Area="ColumnArea" AreaIndex="0" UnboundFieldName="fieldOrderDateMonth" Caption="Año/Mes" />
                    <dx:PivotGridField FieldName="OrderDate" ID="fieldOrderYear" Area="FilterArea" AreaIndex="0"
                        GroupInterval="DateYear" UnboundFieldName="fieldOrderDateYear" Caption="Order Year" Visible="False" />
                </Fields>
                <OptionsView HorizontalScrollBarMode="Auto" />
                <OptionsFilter NativeCheckBoxes="False" />
                <OptionsChartDataSource DataProvideMode="UseCustomSettings" />
            </dx:ASPxPivotGrid>

            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:EST2016ConnectionString %>" SelectCommand="SELECT [id], [ames], [descripcion], [dotacion] FROM [ccRptDotacion] WHERE "></asp:SqlDataSource>

            <dxchartsui:WebChartControl ID="WebChart" runat="server" CrosshairEnabled="True" Height="300px" Width="1000px" DataSourceID="ASPxPivotGrid" SeriesDataMember="Series">
                <Legend MaxHorizontalPercentage="50"></Legend>

                <DiagramSerializable>
                    <cc1:XYDiagram>
                        <AxisX Title-Text="Unidad" VisibleInPanesSerializable="-1">
                        </AxisX>
                        <AxisY Title-Text="Dotacion" VisibleInPanesSerializable="-1">
                        </AxisY>
                    </cc1:XYDiagram>
                </DiagramSerializable>

                <Legend MaxHorizontalPercentage="30"></Legend>

                <SeriesTemplate ArgumentDataMember="Arguments" ArgumentScaleType="Qualitative" ValueDataMembersSerializable="Values" />


            </dxchartsui:WebChartControl>

        </ContentTemplate>
    </asp:UpdatePanel>

    <%--REPORTE ONLINE DOTACION-CAUSAL--%>
    <asp:UpdatePanel runat="server" ID="updatePanel2" OnUnload="UpdatePanel1_Unload">

        <ContentTemplate>
            <table class="OptionsTable OptionsBottomMargin">
                <tr>
                    <td colspan="2">
                        <dx:ASPxComboBox ForeColor="#00489e" CssClass="rounded" Theme="MetropolisBlue" ID="ChartTypeCausal" runat="server" OnValueChanged="ChartTypeCausal_ValueChanged" 
                            AutoPostBack="True" ValueType="System.String" Caption="Tipo Gráfico">
                            <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>

                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" runat="server" Text="Mostrar columna Total General" AutoPostBack="True" Theme="MetropolisBlue"
                            ID="ShowColumnGrandTotalsCausal" OnCheckedChanged="ShowColumnGrandTotalsCausal_CheckedChanged"
                            Wrap="False" />
                    </td>
                    <td>
                        <dx:ASPxCheckBox  ForeColor="#00489e" runat="server" Text="Generar Series de Columnas" AutoPostBack="True" Theme="MetropolisBlue"
                            ID="ChartDataVerticalCausal" OnCheckedChanged="ChartDataVerticalCausal_CheckedChanged" Wrap="False" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" ID="PointLabelsCausal" runat="server" Text="Mostrar rotulos de Datos" OnCheckedChanged="PointLabelsCausal_CheckedChanged" Theme="MetropolisBlue"
                            AutoPostBack="True" Wrap="False" />
                    </td>
                    <td>
                        <dx:ASPxCheckBox ForeColor="#00489e" runat="server" Checked="True" Text="Mostrar Fila Total General" AutoPostBack="True" Theme="MetropolisBlue"
                            ID="ShowRowGrandTotalsCausal" OnCheckedChanged="ShowRowGrandTotalsCausal_CheckedChanged"
                            Wrap="False" />
                    </td>
                </tr>
            </table>
            <dx:ASPxPivotGrid ID="ASPxPivotGridCausal" runat="server" DataSourceID="SqlDataSource2"
                EnableCallBacks="False" OnPreRender="ASPxPivotGridCausal_PreRender" Width="100%" ClientIDMode="AutoID">
                <Styles>

                    <CellsAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center"></CellsAreaStyle>
                    <ColumnFieldValuesAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center" ></ColumnFieldValuesAreaStyle>
                     <FilterItemsAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center"  ></FilterItemsAreaStyle>
                    <RowFieldValuesAreaStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Left" ></RowFieldValuesAreaStyle>
                    <HeaderStyle ForeColor="#00489e" BackColor="#d0d0d0" HorizontalAlign="Center" />
                </Styles>
                <Fields>
                    <dx:PivotGridField FieldName="dotacion" ID="fielddotacioncausal" Area="DataArea"
                        AreaIndex="0" Caption="Dotacion" />
                    <dx:PivotGridField FieldName="Descrip" ID="fieldDescrip" Area="RowArea"
                        AreaIndex="0" Caption="Causal" />
                    <dx:PivotGridField FieldName="ames" ID="fieldOrderMonthcausal" Area="ColumnArea" AreaIndex="0" UnboundFieldName="fieldOrderDateMonth" Caption="Año/Mes" />
                    <dx:PivotGridField FieldName="descripcion" ID="fieldOrderYearcausal" Area="FilterArea" AreaIndex="0" UnboundFieldName="fieldOrderDateYear" Caption="Unidad" Width="200" />
                </Fields>
                <OptionsView HorizontalScrollBarMode="Auto" />
                <OptionsPager Visible="False">
                </OptionsPager>
                <OptionsFilter NativeCheckBoxes="False" />
                <OptionsChartDataSource DataProvideMode="UseCustomSettings" />
            </dx:ASPxPivotGrid>

            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:EST2016ConnectionString %>" SelectCommand="SELECT [id], [ames], [descripcion], [Descrip], [dotacion] FROM [ccRptDotacionCausal]"></asp:SqlDataSource>

            <dxchartsui:WebChartControl ID="WebChartCausal" runat="server" CrosshairEnabled="True" Height="300px" Width="1000px" DataSourceID="ASPxPivotGridCausal" SeriesDataMember="Series">
                <Legend MaxHorizontalPercentage="30"></Legend>

                <DiagramSerializable>
                    <cc1:XYDiagram>
                    <axisx title-text="Causal" visibleinpanesserializable="-1">
                    </axisx>
                    <axisy title-text="Dotacion" visibleinpanesserializable="-1">
                    </axisy>
                    </cc1:XYDiagram>
                </DiagramSerializable>

                <Legend MaxHorizontalPercentage="30"></Legend>

                <SeriesTemplate ArgumentDataMember="Arguments" ArgumentScaleType="Qualitative" ValueDataMembersSerializable="Values" />

            </dxchartsui:WebChartControl>

            </ContentTemplate>

</asp:UpdatePanel>

    <%--GRID SEXO--%>
    <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridSexo"  runat="server" 
        ClientInstanceName="gridSexo" KeyFieldName="id" Font-Size="Small"
        OnPageIndexChanged="gridDatos_PageIndexChanged"  >
        <Styles>
            <Row  HorizontalAlign="Center" ></Row>
            <Cell HorizontalAlign="Center" Paddings-Padding="5" ></Cell>
            <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" ForeColor="#00489e"></Header>
            <FocusedRow ForeColor="#00489e" Font-Bold="true" BackColor="#d0d0d0"></FocusedRow>
            <RowHotTrack ForeColor="#00489e"  BackColor="#d0d0d0"></RowHotTrack>
        </Styles>
        <SettingsPager PageSize="25">
        </SettingsPager>
        <Columns>
            <dx:GridViewDataColumn Caption ="Sexo" FieldName="sexo" Name="sexo">
                <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Nombre" FieldName="NOMBRE" Name="NOMBRE">
                <CellStyle HorizontalAlign="Left"/>
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Rut" FieldName="RUT" Name="RUT">
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Nacionalidad" FieldName="NACIONALIDAD" Name="NACIONALIDAD">
                <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Localidad" FieldName="LOCALIDAD" Name="LOCALIDAD">
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Cargo" FieldName="CARGO" Name="CARGO">
                <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                <CellStyle HorizontalAlign="Left"/>
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Planta" FieldName="PLANTA" Name="PLANTA">
                <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                <CellStyle HorizontalAlign="Left"/>
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Centro Costo" FieldName="CCOSTO" Name="CCOSTO">
                <Settings AllowHeaderFilter="True" FilterMode="DisplayText" />
                <CellStyle HorizontalAlign="Left"/>
            </dx:GridViewDataColumn>

            <dx:GridViewDataColumn Caption ="Área" FieldName="AREA" Name="AREA">
                <CellStyle HorizontalAlign="Left"/>
            </dx:GridViewDataColumn>
        </Columns>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter ID="GridExportarSexo" runat="server" GridViewID="gridSexo">
        <Styles>
            <Default Font-Names="Arial" Font-Size="Medium">
            </Default>
            <Header Font-Names="Arial" Font-Size="Medium">
            </Header>
        </Styles>
    </dx:ASPxGridViewExporter>

    <%--GRID REPORTE TOTAL POR TRABAJADOR--%>
    <dx:ASPxGridView Width="100%" ForeColor="#00489e" ClientInstanceName="gridBucarTrabajadores" ID="gridBucarTrabajadores" runat="server" KeyFieldName="codigo"
        AutoGenerateColumns="False" Theme="Default" EnableCallBacks="true" OnPageIndexChanged="gridBucarTrabajadores_PageIndexChanged">
        <Settings GridLines="Vertical" HorizontalScrollBarMode="Visible" />

        <Styles>
            <FixedColumn ForeColor="#00489E" HorizontalAlign="Center"></FixedColumn>
            <Header Wrap="True" HorizontalAlign="Center" Font-Bold="true" Font-Size="10" BackColor="#F2F2F2" ForeColor="#00489e"></Header>
            <Cell Font-Size="10" HorizontalAlign="Left"></Cell>
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

        <SettingsPager PageSize="25" Mode="ShowPager"></SettingsPager>

        <SettingsBehavior EnableRowHotTrack="true" FilterRowMode="Auto" AllowFocusedRow="true" AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" />


        <SettingsSearchPanel Visible="True" />

        <Columns>

            <dx:GridViewDataTextColumn Caption="Rut" FieldName="rutasociado" Name="rutasociado" ShowInCustomizationForm="True" Visible="true">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Trabajador" Width="25%" FieldName="Nombre" Name="Nombre" ShowInCustomizationForm="True" Visible="true">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Mes Activo" FieldName="ames" Name="ames" ShowInCustomizationForm="True" Visible="true">
                <Settings AllowHeaderFilter="True" FilterMode="Value" />
                <CellStyle HorizontalAlign="Center"/>
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Unidad" FieldName="id" Name="id" ShowInCustomizationForm="True">
                <Settings AllowHeaderFilter="True" FilterMode="Value" />
                <CellStyle HorizontalAlign="Center"/>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Causa Legal" Width="14%" FieldName="descrip" Name="descrip" ShowInCustomizationForm="True" Visible="true">
                <Settings AllowHeaderFilter="True" FilterMode="Value" />
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Detalle Causal" Width="35%" FieldName="detalleCausal" Name="detalleCausal" ShowInCustomizationForm="True" Visible="true">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Inicio Contrato" FieldName="fini" Name="fini" ShowInCustomizationForm="true" Visible="true">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Fin Contrato" FieldName="ffin" Name="ffin" ShowInCustomizationForm="True">
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Dias" FieldName="cantDias" Name="cantDias" ShowInCustomizationForm="True">
                <CellStyle HorizontalAlign="Center"/>
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>
        </Columns>
    </dx:ASPxGridView>


    <dx:ASPxGridViewExporter ID="GridExportarBuscarTrabajadores" runat="server" GridViewID="gridBucarTrabajadores">
        <Styles>
            <Default Font-Names="Arial" Font-Size="Medium">
            </Default>
            <Header Font-Names="Arial" Font-Size="Medium">
            </Header>
        </Styles>

    </dx:ASPxGridViewExporter>

</asp:Content>