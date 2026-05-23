<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlExpedienteProfesor
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
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.lblDni = New System.Windows.Forms.Label()
        Me.lblNombre = New System.Windows.Forms.Label()
        Me.dgvEstudios = New System.Windows.Forms.DataGridView()
        Me.btnExportarPDF = New System.Windows.Forms.Button()
        Me.lstAlumnos = New System.Windows.Forms.ListBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbGrupo = New System.Windows.Forms.ComboBox()
        CType(Me.dgvEstudios, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblEmail
        '
        Me.lblEmail.AutoSize = True
        Me.lblEmail.Location = New System.Drawing.Point(827, 372)
        Me.lblEmail.Name = "lblEmail"
        Me.lblEmail.Size = New System.Drawing.Size(39, 13)
        Me.lblEmail.TabIndex = 11
        Me.lblEmail.Text = "Label1"
        '
        'lblDni
        '
        Me.lblDni.AutoSize = True
        Me.lblDni.Location = New System.Drawing.Point(827, 328)
        Me.lblDni.Name = "lblDni"
        Me.lblDni.Size = New System.Drawing.Size(39, 13)
        Me.lblDni.TabIndex = 10
        Me.lblDni.Text = "Label1"
        '
        'lblNombre
        '
        Me.lblNombre.AutoSize = True
        Me.lblNombre.Location = New System.Drawing.Point(827, 281)
        Me.lblNombre.Name = "lblNombre"
        Me.lblNombre.Size = New System.Drawing.Size(39, 13)
        Me.lblNombre.TabIndex = 9
        Me.lblNombre.Text = "Label1"
        '
        'dgvEstudios
        '
        Me.dgvEstudios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEstudios.Location = New System.Drawing.Point(179, 72)
        Me.dgvEstudios.Name = "dgvEstudios"
        Me.dgvEstudios.Size = New System.Drawing.Size(532, 372)
        Me.dgvEstudios.TabIndex = 7
        '
        'btnExportarPDF
        '
        Me.btnExportarPDF.Location = New System.Drawing.Point(381, 488)
        Me.btnExportarPDF.Name = "btnExportarPDF"
        Me.btnExportarPDF.Size = New System.Drawing.Size(158, 34)
        Me.btnExportarPDF.TabIndex = 6
        Me.btnExportarPDF.Text = "Exportar en PDF"
        Me.btnExportarPDF.UseVisualStyleBackColor = True
        '
        'lstAlumnos
        '
        Me.lstAlumnos.FormattingEnabled = True
        Me.lstAlumnos.Location = New System.Drawing.Point(24, 72)
        Me.lstAlumnos.Name = "lstAlumnos"
        Me.lstAlumnos.Size = New System.Drawing.Size(120, 368)
        Me.lstAlumnos.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(221, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Grupo de clase:"
        '
        'cmbGrupo
        '
        Me.cmbGrupo.FormattingEnabled = True
        Me.cmbGrupo.Location = New System.Drawing.Point(337, 28)
        Me.cmbGrupo.Name = "cmbGrupo"
        Me.cmbGrupo.Size = New System.Drawing.Size(175, 21)
        Me.cmbGrupo.TabIndex = 14
        '
        'UserControlExpedienteProfesor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.cmbGrupo)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lstAlumnos)
        Me.Controls.Add(Me.lblEmail)
        Me.Controls.Add(Me.lblDni)
        Me.Controls.Add(Me.lblNombre)
        Me.Controls.Add(Me.dgvEstudios)
        Me.Controls.Add(Me.btnExportarPDF)
        Me.Name = "UserControlExpedienteProfesor"
        Me.Size = New System.Drawing.Size(962, 575)
        CType(Me.dgvEstudios, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblEmail As Label
    Friend WithEvents lblDni As Label
    Friend WithEvents lblNombre As Label
    Friend WithEvents dgvEstudios As DataGridView
    Friend WithEvents btnExportarPDF As Button
    Friend WithEvents lstAlumnos As ListBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cmbGrupo As ComboBox
End Class
