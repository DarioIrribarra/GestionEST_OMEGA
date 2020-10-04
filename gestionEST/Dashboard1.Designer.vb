Namespace Win_Dashboards
    Partial Public Class Dashboard1
        ''' <summary> 
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Component Designer generated code"

        ''' <summary> 
        ''' Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Dim DashboardLayoutGroup1 As DevExpress.DashboardCommon.DashboardLayoutGroup = New DevExpress.DashboardCommon.DashboardLayoutGroup()
            Dim DashboardLayoutGroup2 As DevExpress.DashboardCommon.DashboardLayoutGroup = New DevExpress.DashboardCommon.DashboardLayoutGroup()
            Dim DashboardLayoutItem1 As DevExpress.DashboardCommon.DashboardLayoutItem = New DevExpress.DashboardCommon.DashboardLayoutItem()
            Dim DashboardLayoutItem2 As DevExpress.DashboardCommon.DashboardLayoutItem = New DevExpress.DashboardCommon.DashboardLayoutItem()
            Me.TextBoxDashboardItem1 = New DevExpress.DashboardCommon.TextBoxDashboardItem()
            Me.TextBoxDashboardItem2 = New DevExpress.DashboardCommon.TextBoxDashboardItem()
            CType(Me.TextBoxDashboardItem1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.TextBoxDashboardItem2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
            '
            'TextBoxDashboardItem1
            '
            Me.TextBoxDashboardItem1.ComponentName = "TextBoxDashboardItem1"
            Me.TextBoxDashboardItem1.DataItemRepository.Clear()
            Me.TextBoxDashboardItem1.InteractivityOptions.IgnoreMasterFilters = False
            Me.TextBoxDashboardItem1.Name = "Cuadro de Texto 1"
            Me.TextBoxDashboardItem1.ShowCaption = True
            '
            'TextBoxDashboardItem2
            '
            Me.TextBoxDashboardItem2.ComponentName = "TextBoxDashboardItem2"
            Me.TextBoxDashboardItem2.DataItemRepository.Clear()
            Me.TextBoxDashboardItem2.InteractivityOptions.IgnoreMasterFilters = False
            Me.TextBoxDashboardItem2.Name = "Cuadro de Texto 1"
            Me.TextBoxDashboardItem2.ShowCaption = True
            '
            'Dashboard1
            '
            Me.Items.AddRange(New DevExpress.DashboardCommon.DashboardItem() {Me.TextBoxDashboardItem1, Me.TextBoxDashboardItem2})
            DashboardLayoutItem1.DashboardItem = Me.TextBoxDashboardItem1
            DashboardLayoutItem1.Weight = 100.0R
            DashboardLayoutItem2.DashboardItem = Me.TextBoxDashboardItem2
            DashboardLayoutItem2.Weight = 100.0R
            DashboardLayoutGroup2.ChildNodes.AddRange(New DevExpress.DashboardCommon.DashboardLayoutNode() {DashboardLayoutItem1, DashboardLayoutItem2})
            DashboardLayoutGroup2.DashboardItem = Nothing
            DashboardLayoutGroup2.Weight = 100.0R
            DashboardLayoutGroup1.ChildNodes.AddRange(New DevExpress.DashboardCommon.DashboardLayoutNode() {DashboardLayoutGroup2})
            DashboardLayoutGroup1.DashboardItem = Nothing
            DashboardLayoutGroup1.Orientation = DevExpress.DashboardCommon.DashboardLayoutGroupOrientation.Vertical
            Me.LayoutRoot = DashboardLayoutGroup1
            Me.Title.Text = "Tablero de control"
            CType(Me.TextBoxDashboardItem1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.TextBoxDashboardItem2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

        End Sub

        Friend WithEvents TextBoxDashboardItem1 As DevExpress.DashboardCommon.TextBoxDashboardItem
        Friend WithEvents TextBoxDashboardItem2 As DevExpress.DashboardCommon.TextBoxDashboardItem

#End Region
    End Class
End Namespace