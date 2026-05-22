<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlVisorDirectiu
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
        Me.dgvDetalleCentro = New System.Windows.Forms.DataGridView()
        Me.cmbCentros = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnCambiarLogo = New System.Windows.Forms.Button()
        Me.btnModificarPorExcel = New System.Windows.Forms.Button()
        CType(Me.dgvDetalleCentro, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvDetalleCentro
        '
        Me.dgvDetalleCentro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvDetalleCentro.Location = New System.Drawing.Point(121, 122)
        Me.dgvDetalleCentro.Name = "dgvDetalleCentro"
        Me.dgvDetalleCentro.Size = New System.Drawing.Size(668, 327)
        Me.dgvDetalleCentro.TabIndex = 0
        '
        'cmbCentros
        '
        Me.cmbCentros.FormattingEnabled = True
        Me.cmbCentros.Location = New System.Drawing.Point(348, 65)
        Me.cmbCentros.Name = "cmbCentros"
        Me.cmbCentros.Size = New System.Drawing.Size(334, 21)
        Me.cmbCentros.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(269, 68)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(46, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Centros:"
        '
        'btnCambiarLogo
        '
        Me.btnCambiarLogo.Location = New System.Drawing.Point(236, 484)
        Me.btnCambiarLogo.Name = "btnCambiarLogo"
        Me.btnCambiarLogo.Size = New System.Drawing.Size(143, 47)
        Me.btnCambiarLogo.TabIndex = 3
        Me.btnCambiarLogo.Text = "Cambiar logo"
        Me.btnCambiarLogo.UseVisualStyleBackColor = True
        '
        'btnModificarPorExcel
        '
        Me.btnModificarPorExcel.Location = New System.Drawing.Point(511, 484)
        Me.btnModificarPorExcel.Name = "btnModificarPorExcel"
        Me.btnModificarPorExcel.Size = New System.Drawing.Size(143, 47)
        Me.btnModificarPorExcel.TabIndex = 4
        Me.btnModificarPorExcel.Text = "Importar por Excel"
        Me.btnModificarPorExcel.UseVisualStyleBackColor = True
        '
        'UserControlVisorDirectiu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnModificarPorExcel)
        Me.Controls.Add(Me.btnCambiarLogo)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbCentros)
        Me.Controls.Add(Me.dgvDetalleCentro)
        Me.Name = "UserControlVisorDirectiu"
        Me.Size = New System.Drawing.Size(962, 575)
        CType(Me.dgvDetalleCentro, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvDetalleCentro As DataGridView
    Friend WithEvents cmbCentros As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnCambiarLogo As Button
    Friend WithEvents btnModificarPorExcel As Button
End Class
