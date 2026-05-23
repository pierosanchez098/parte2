<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlExpediente
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
        Me.btnExportarPDF = New System.Windows.Forms.Button()
        Me.dgvExpediente = New System.Windows.Forms.DataGridView()
        Me.picFoto = New System.Windows.Forms.PictureBox()
        Me.lblNombre = New System.Windows.Forms.Label()
        Me.lblDni = New System.Windows.Forms.Label()
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.dgvExpediente, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picFoto, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnExportarPDF
        '
        Me.btnExportarPDF.Location = New System.Drawing.Point(383, 520)
        Me.btnExportarPDF.Name = "btnExportarPDF"
        Me.btnExportarPDF.Size = New System.Drawing.Size(158, 34)
        Me.btnExportarPDF.TabIndex = 0
        Me.btnExportarPDF.Text = "Exportar en PDF"
        Me.btnExportarPDF.UseVisualStyleBackColor = True
        '
        'dgvExpediente
        '
        Me.dgvExpediente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvExpediente.Location = New System.Drawing.Point(49, 84)
        Me.dgvExpediente.Name = "dgvExpediente"
        Me.dgvExpediente.Size = New System.Drawing.Size(640, 392)
        Me.dgvExpediente.TabIndex = 1
        '
        'picFoto
        '
        Me.picFoto.Location = New System.Drawing.Point(740, 115)
        Me.picFoto.Name = "picFoto"
        Me.picFoto.Size = New System.Drawing.Size(178, 159)
        Me.picFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picFoto.TabIndex = 2
        Me.picFoto.TabStop = False
        '
        'lblNombre
        '
        Me.lblNombre.AutoSize = True
        Me.lblNombre.Location = New System.Drawing.Point(800, 324)
        Me.lblNombre.Name = "lblNombre"
        Me.lblNombre.Size = New System.Drawing.Size(39, 13)
        Me.lblNombre.TabIndex = 3
        Me.lblNombre.Text = "Label1"
        '
        'lblDni
        '
        Me.lblDni.AutoSize = True
        Me.lblDni.Location = New System.Drawing.Point(800, 371)
        Me.lblDni.Name = "lblDni"
        Me.lblDni.Size = New System.Drawing.Size(39, 13)
        Me.lblDni.TabIndex = 4
        Me.lblDni.Text = "Label1"
        '
        'lblEmail
        '
        Me.lblEmail.AutoSize = True
        Me.lblEmail.Location = New System.Drawing.Point(800, 415)
        Me.lblEmail.Name = "lblEmail"
        Me.lblEmail.Size = New System.Drawing.Size(39, 13)
        Me.lblEmail.TabIndex = 5
        Me.lblEmail.Text = "Label1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(405, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Mi expediente"
        '
        'UserControlExpediente
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblEmail)
        Me.Controls.Add(Me.lblDni)
        Me.Controls.Add(Me.lblNombre)
        Me.Controls.Add(Me.picFoto)
        Me.Controls.Add(Me.dgvExpediente)
        Me.Controls.Add(Me.btnExportarPDF)
        Me.Name = "UserControlExpediente"
        Me.Size = New System.Drawing.Size(962, 575)
        CType(Me.dgvExpediente, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picFoto, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnExportarPDF As Button
    Friend WithEvents dgvExpediente As DataGridView
    Friend WithEvents picFoto As PictureBox
    Friend WithEvents lblNombre As Label
    Friend WithEvents lblDni As Label
    Friend WithEvents lblEmail As Label
    Friend WithEvents Label1 As Label
End Class
