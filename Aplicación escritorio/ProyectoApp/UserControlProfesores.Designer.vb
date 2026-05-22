<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlProfesores
    Inherits System.Windows.Forms.UserControl

    'UserControl reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.flpProfesores = New System.Windows.Forms.FlowLayoutPanel()
        Me.cmbCentro = New System.Windows.Forms.ComboBox()
        Me.lblSeleccioneCentro = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'flpProfesores
        '
        Me.flpProfesores.AutoScroll = True
        Me.flpProfesores.Location = New System.Drawing.Point(0, 54)
        Me.flpProfesores.Margin = New System.Windows.Forms.Padding(2)
        Me.flpProfesores.Name = "flpProfesores"
        Me.flpProfesores.Size = New System.Drawing.Size(947, 521)
        Me.flpProfesores.TabIndex = 0
        '
        'cmbCentro
        '
        Me.cmbCentro.FormattingEnabled = True
        Me.cmbCentro.Location = New System.Drawing.Point(260, 16)
        Me.cmbCentro.Name = "cmbCentro"
        Me.cmbCentro.Size = New System.Drawing.Size(121, 21)
        Me.cmbCentro.TabIndex = 0
        '
        'lblSeleccioneCentro
        '
        Me.lblSeleccioneCentro.AutoSize = True
        Me.lblSeleccioneCentro.Location = New System.Drawing.Point(153, 19)
        Me.lblSeleccioneCentro.Name = "lblSeleccioneCentro"
        Me.lblSeleccioneCentro.Size = New System.Drawing.Size(46, 13)
        Me.lblSeleccioneCentro.TabIndex = 1
        Me.lblSeleccioneCentro.Text = "Centros."
        '
        'UserControlProfesores
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblSeleccioneCentro)
        Me.Controls.Add(Me.cmbCentro)
        Me.Controls.Add(Me.flpProfesores)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "UserControlProfesores"
        Me.Size = New System.Drawing.Size(962, 575)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents flpProfesores As FlowLayoutPanel
    Friend WithEvents cmbCentro As ComboBox
    Friend WithEvents lblSeleccioneCentro As Label
End Class
