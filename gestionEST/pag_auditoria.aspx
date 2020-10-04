<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/pag_maestra.Master" CodeBehind="pag_auditoria.aspx.vb" Inherits="gestionEST.pag_auditoria" %>

<%@ Register Assembly="DevExpress.Web.v18.1, Version=18.1.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Barra_Azul" runat="server">
    <script type="text/javascript">
        //LLAMAR A PANEL CALLBACK DEL CHECKBOX
        function ClickAuditado(s, e) {
            //SE MUESTRA EL MENSAJE DE CARGANDO
            lpCallBack.Show();
            //OBTENGO EL STRING CON NUMERO DE CELDA
            var stringCelda = Object.values(s)[1];
            //HAGO EL REGEX PARA OBTENER LA CELDA
            var pattern = /cell([0-9]+)/;
            //LE PASO EL VALOR AL BALLBACK
            CBGridAuditoria.PerformCallback(stringCelda.match(pattern)[1] + "-" + s.GetValue());
            //console.log(stringCelda.match(pattern)[1] + s.GetValue());
            //console.log(s.GetValue());
            //CBGridAuditoria.PerformCallback();
        }

        function TerminaActualizacionAuditoria(s, e) {
            alert("Auditoría Modificada")
            lpCallBack.Hide();
        }
    </script>
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
            background:none;
            border:1px solid transparent;
            font-weight:bold;
            color:white;
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
    </style>


    <table>
        <tr id="tr_unidad" runat="server">
            <td class="d-flex">
                <div style="width: 20%;">
                    <label class="text-white font-weight-bold">
                        Unidad 
                    </label>
                </div>
                <div  id="divUnidad" class="d-flex justify-content-end">
                    <dx:ASPxComboBox ForeColor="#00489e" NullText="SELECCIONE" Theme="MetropolisBlue" Width="83%" EnableTheming="true" CssClass="rounded" ID="cbxUnidad" runat="server" 
                        ValueType="System.String" ClientInstanceName="cbxUnidad"  OnInit="txtUnidadEmpresa_Init"
                        >
                        
                    <DropDownButton Image-Url="images/arrow-point-to-down.png"></DropDownButton>
                    </dx:ASPxComboBox>
                </div>
            </td>
        </tr>

        <tr id="tr_consultar" runat="server">
            <td class="d-flex justify-content-end" >
                <dx:ASPxButton 
                    ID="btnConsultar" runat="server" AutoPostBack="False" Text="Buscar" CssClass="estiloBtnBuscar "
                    OnClick="btnConsultar_Click" >
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
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
    <div id="lblDefinicionAuditado" runat="server" style="color: #00489e">
        <label>
            Documentación: 
            <span>
                <img src="images/15.png" />
            </span>: Vigente |
            <span>
                <img src="images/36.png" />
            </span>: Menos de 20 Días de Vigencia |
            <span>
                <img src="images/12.png" />
            </span>: Caducado |
            <span>
                <img src="images/14.png" />
            </span>: Sin Documento |
        </label>
    </div>
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>

    <dx:ASPxLoadingPanel runat="server" ID="lpCallBack" Text="Modificando Auditoría" ClientInstanceName="lpCallBack" Modal="true" Theme="MetropolisBlue" ></dx:ASPxLoadingPanel>
    <dx:ASPxCallback runat="server" ID="ASPxCallback1" ClientInstanceName="CBGridAuditoria" OnCallback="chkAuditado_CheckedChanged">
        <ClientSideEvents EndCallback="TerminaActualizacionAuditoria"/>
    </dx:ASPxCallback>

    <dx:ASPxGridView ForeColor="#00489e" Width="100%" ID="gridDocumentacion" 
        OnPageIndexChanged="gridDocumentacion_PageIndexChanged" EnableCallBacks="true" 
        OnLoad="gridDocumentacion_Load"
        OnInit="gridDocumentacion_Init"
        runat="server" ClientInstanceName="gridDocumentacion"
        KeyFieldName="rut" AutoGenerateColumns="True" Theme="Default">
        <Settings GridLines="Vertical" HorizontalScrollBarMode="Auto" />
        <SettingsPager PageSize="20" />
        <SettingsSearchPanel Visible="true" />
        <SettingsLoadingPanel Mode="Default" />
        <SettingsBehavior
            
            AllowSort="false"
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
<%--        

        <Columns>

            <dx:GridViewDataTextColumn Width="12%"  Caption="Rut" Name="rut" FieldName="rut" Visible="false">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Width="12%"  Caption="Rut" Name="rut" FieldName="rut">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Width="30%"  Caption="Nombre" Name="nombre" FieldName="nombre">
                <HeaderStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="ID" Name="id" FieldName="planta">
                <HeaderStyle HorizontalAlign="Center" />
                <CellStyle HorizontalAlign="Center" />
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  CellStyle-HorizontalAlign="Center" Caption="Estado" FieldName="estado" Name="estado" >
                <Settings AllowHeaderFilter="True" FilterMode="DisplayText" AllowSort="False" AllowGroup="False" />
                <DataItemTemplate>
                    <dx:ASPxImage ID="imgEstado_A" FieldName="imgEstado_A" Name="imgEstado_A" runat="server" OnLoad="imgEstado_A_Load" Visible="false"></dx:ASPxImage>
                </DataItemTemplate>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Liquidaciones" FieldName="liquidacion" Name="liquidacion">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Pacto</br>H. Extras" FieldName="pactohoraextra" Name="pactohoraextra">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="C.Estudios" FieldName="estudios" Name="estudios">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Masso" FieldName="chamas" Name="chamas">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Contratos" FieldName="contratos" Name="contratos">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Curriculum" FieldName="curriculum" Name="curriculum">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="ODI</br>Derecho a</br>Saber" FieldName="derechosaber" Name="derechosaber">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Entrega</br>EPP" FieldName="epp" Name="epp">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Reglamento</br>Interno" FieldName="reglamentointerno" Name="reglamentointerno">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Examen</br>Médico" FieldName="examensalud" Name="examensalud">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn  Caption="Evaluación</br>Psicolaboral" FieldName="examenpsicologico" Name="examenpsicologico">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataTextColumn Caption="Finiquito" FieldName="finiquito" Name="finiquito">
                <HeaderStyle HorizontalAlign="Center" />
                <DataItemTemplate>
                    <dx:ASPxHyperLink ID="ASPxHyperLink1" runat="server" Text="ASPxHyperLink" OnInit="ASPxHyperLink1_Init"></dx:ASPxHyperLink>
                </DataItemTemplate>
                <CellStyle HorizontalAlign="Center">
                </CellStyle>
            </dx:GridViewDataTextColumn>

            <dx:GridViewDataCheckColumn Caption="" Name="auditado" FieldName="auditado">
                <HeaderCaptionTemplate>
                    <img src="images/37.png" />
                </HeaderCaptionTemplate>
                <DataItemTemplate>
                    <dx:ASPxCheckBox runat="server" ID="chkAuditado" Theme="MetropolisBlue" ClientInstanceName="chkAuditado" AutoPostBack="true" OnInit="chkAuditado_Init" OnCheckedChanged="chkAuditado_CheckedChanged">
                    </dx:ASPxCheckBox>
                </DataItemTemplate>
            </dx:GridViewDataCheckColumn>
        </Columns>--%>
        
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="gridDocumentacion">
        <Styles>
            <Default Font-Names="Arial" Font-Size="Medium">
            </Default>
            <Header Font-Names="Arial" Font-Size="Medium">
            </Header>
        </Styles>

    </dx:ASPxGridViewExporter>

</asp:Content>
