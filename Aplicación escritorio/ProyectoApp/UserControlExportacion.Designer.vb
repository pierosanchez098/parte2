<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlExportacion
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
        Me.btnExportarLogsXML = New System.Windows.Forms.Button()
        Me.btnGenerarActaPDF = New System.Windows.Forms.Button()
        Me.cmbGrupoActa = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnExportarAlumnoscsv = New System.Windows.Forms.Button()
        Me.btnExportarProfesoresenJSON = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnExportarLogsXML
        '
        Me.btnExportarLogsXML.Location = New System.Drawing.Point(457, 111)
        Me.btnExportarLogsXML.Name = "btnExportarLogsXML"
        Me.btnExportarLogsXML.Size = New System.Drawing.Size(264, 66)
        Me.btnExportarLogsXML.TabIndex = 0
        Me.btnExportarLogsXML.Text = "Exportar XML de logs"
        Me.btnExportarLogsXML.UseVisualStyleBackColor = True
        '
        'btnGenerarActaPDF
        '
        Me.btnGenerarActaPDF.Location = New System.Drawing.Point(457, 283)
        Me.btnGenerarActaPDF.Name = "btnGenerarActaPDF"
        Me.btnGenerarActaPDF.Size = New System.Drawing.Size(264, 66)
        Me.btnGenerarActaPDF.TabIndex = 1
        Me.btnGenerarActaPDF.Text = "Generar acta de evaluación"
        Me.btnGenerarActaPDF.UseVisualStyleBackColor = True
        '
        'cmbGrupoActa
        '
        Me.cmbGrupoActa.FormattingEnabled = True
        Me.cmbGrupoActa.Location = New System.Drawing.Point(360, 40)
        Me.cmbGrupoActa.Name = "cmbGrupoActa"
        Me.cmbGrupoActa.Size = New System.Drawing.Size(187, 28)
        Me.cmbGrupoActa.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(222, 43)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(121, 20)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Grupo de clase:"
        '
        'btnExportarAlumnoscsv
        '
        Me.btnExportarAlumnoscsv.Location = New System.Drawing.Point(86, 283)
        Me.btnExportarAlumnoscsv.Name = "btnExportarAlumnoscsv"
        Me.btnExportarAlumnoscsv.Size = New System.Drawing.Size(264, 66)
        Me.btnExportarAlumnoscsv.TabIndex = 4
        Me.btnExportarAlumnoscsv.Text = "Exportar alumnos en CSV"
        Me.btnExportarAlumnoscsv.UseVisualStyleBackColor = True
        '
        'btnExportarProfesoresenJSON
        '
        Me.btnExportarProfesoresenJSON.Location = New System.Drawing.Point(86, 111)
        Me.btnExportarProfesoresenJSON.Name = "btnExportarProfesoresenJSON"
        Me.btnExportarProfesoresenJSON.Size = New System.Drawing.Size(264, 66)
        Me.btnExportarProfesoresenJSON.TabIndex = 5
        Me.btnExportarProfesoresenJSON.Text = "Exportar profesores en JSON"
        Me.btnExportarProfesoresenJSON.UseVisualStyleBackColor = True
        '
        'UserControlExportacion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnExportarProfesoresenJSON)
        Me.Controls.Add(Me.btnExportarAlumnoscsv)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbGrupoActa)
        Me.Controls.Add(Me.btnGenerarActaPDF)
        Me.Controls.Add(Me.btnExportarLogsXML)
        Me.Name = "UserControlExportacion"
        Me.Size = New System.Drawing.Size(800, 500)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnExportarLogsXML As Button
    Friend WithEvents btnGenerarActaPDF As Button
    Friend WithEvents cmbGrupoActa As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnExportarAlumnoscsv As Button
    Friend WithEvents btnExportarProfesoresenJSON As Button
End Class
