<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlPonerNotas
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
        Me.cmbGrupo = New System.Windows.Forms.ComboBox()
        Me.cmbUnidad = New System.Windows.Forms.ComboBox()
        Me.dgvNotas = New System.Windows.Forms.DataGridView()
        Me.btnGuardarNotas = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        CType(Me.dgvNotas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmbGrupo
        '
        Me.cmbGrupo.FormattingEnabled = True
        Me.cmbGrupo.Location = New System.Drawing.Point(406, 64)
        Me.cmbGrupo.Name = "cmbGrupo"
        Me.cmbGrupo.Size = New System.Drawing.Size(196, 28)
        Me.cmbGrupo.TabIndex = 0
        '
        'cmbUnidad
        '
        Me.cmbUnidad.FormattingEnabled = True
        Me.cmbUnidad.Location = New System.Drawing.Point(406, 128)
        Me.cmbUnidad.Name = "cmbUnidad"
        Me.cmbUnidad.Size = New System.Drawing.Size(601, 28)
        Me.cmbUnidad.TabIndex = 1
        '
        'dgvNotas
        '
        Me.dgvNotas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvNotas.Location = New System.Drawing.Point(127, 226)
        Me.dgvNotas.Name = "dgvNotas"
        Me.dgvNotas.RowHeadersWidth = 62
        Me.dgvNotas.RowTemplate.Height = 28
        Me.dgvNotas.Size = New System.Drawing.Size(1085, 549)
        Me.dgvNotas.TabIndex = 2
        '
        'btnGuardarNotas
        '
        Me.btnGuardarNotas.Location = New System.Drawing.Point(496, 797)
        Me.btnGuardarNotas.Name = "btnGuardarNotas"
        Me.btnGuardarNotas.Size = New System.Drawing.Size(255, 44)
        Me.btnGuardarNotas.TabIndex = 3
        Me.btnGuardarNotas.Text = "Guardar notas"
        Me.btnGuardarNotas.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(225, 64)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(121, 20)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Grupo de clase:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(225, 131)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(165, 20)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Unidad de asignatura:"
        '
        'UserControlPonerNotas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnGuardarNotas)
        Me.Controls.Add(Me.dgvNotas)
        Me.Controls.Add(Me.cmbUnidad)
        Me.Controls.Add(Me.cmbGrupo)
        Me.Name = "UserControlPonerNotas"
        Me.Size = New System.Drawing.Size(1315, 881)
        CType(Me.dgvNotas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmbGrupo As ComboBox
    Friend WithEvents cmbUnidad As ComboBox
    Friend WithEvents dgvNotas As DataGridView
    Friend WithEvents btnGuardarNotas As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
End Class
