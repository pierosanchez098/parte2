<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlBoletinEstudiante
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
        Me.dgvBoletin = New System.Windows.Forms.DataGridView()
        Me.btnExportarPDF = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.dgvBoletin, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvBoletin
        '
        Me.dgvBoletin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBoletin.Location = New System.Drawing.Point(88, 120)
        Me.dgvBoletin.Name = "dgvBoletin"
        Me.dgvBoletin.Size = New System.Drawing.Size(682, 312)
        Me.dgvBoletin.TabIndex = 0
        '
        'btnExportarPDF
        '
        Me.btnExportarPDF.Location = New System.Drawing.Point(354, 462)
        Me.btnExportarPDF.Name = "btnExportarPDF"
        Me.btnExportarPDF.Size = New System.Drawing.Size(170, 52)
        Me.btnExportarPDF.TabIndex = 1
        Me.btnExportarPDF.Text = "Exportar en PDF"
        Me.btnExportarPDF.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(411, 63)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(54, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Mi boletín"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'UserControlBoletinEstudiante
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnExportarPDF)
        Me.Controls.Add(Me.dgvBoletin)
        Me.Name = "UserControlBoletinEstudiante"
        Me.Size = New System.Drawing.Size(962, 575)
        CType(Me.dgvBoletin, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvBoletin As DataGridView
    Friend WithEvents btnExportarPDF As Button
    Friend WithEvents Label1 As Label
End Class
