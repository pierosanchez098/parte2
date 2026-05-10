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
        Me.SuspendLayout()
        '
        'flpProfesores
        '
        Me.flpProfesores.AutoScroll = True
        Me.flpProfesores.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpProfesores.Location = New System.Drawing.Point(0, 0)
        Me.flpProfesores.Name = "flpProfesores"
        Me.flpProfesores.Size = New System.Drawing.Size(800, 500)
        Me.flpProfesores.TabIndex = 0
        '
        'UserControlProfesores
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.flpProfesores)
        Me.Name = "UserControlProfesores"
        Me.Size = New System.Drawing.Size(800, 500)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents flpProfesores As FlowLayoutPanel
End Class
