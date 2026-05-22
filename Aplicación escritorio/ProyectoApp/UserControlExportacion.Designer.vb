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
        Me.cmbCSVGrupo = New System.Windows.Forms.ComboBox()
        Me.lblGrupoClaseCSV = New System.Windows.Forms.Label()
        Me.btnExportarAlumnoscsv = New System.Windows.Forms.Button()
        Me.btnExportarProfesoresenJSON = New System.Windows.Forms.Button()
        Me.lblCentros = New System.Windows.Forms.Label()
        Me.cmbCentroJson = New System.Windows.Forms.ComboBox()
        Me.cmbGrupoExcel = New System.Windows.Forms.ComboBox()
        Me.lblGrupoClaseExcel = New System.Windows.Forms.Label()
        Me.btnExportarAlumnosExcel = New System.Windows.Forms.Button()
        Me.lblGrupoClaseActa = New System.Windows.Forms.Label()
        Me.cmbActa = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'btnExportarLogsXML
        '
        Me.btnExportarLogsXML.Location = New System.Drawing.Point(311, 122)
        Me.btnExportarLogsXML.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExportarLogsXML.Name = "btnExportarLogsXML"
        Me.btnExportarLogsXML.Size = New System.Drawing.Size(176, 43)
        Me.btnExportarLogsXML.TabIndex = 0
        Me.btnExportarLogsXML.Text = "Exportar XML de logs"
        Me.btnExportarLogsXML.UseVisualStyleBackColor = True
        '
        'btnGenerarActaPDF
        '
        Me.btnGenerarActaPDF.Location = New System.Drawing.Point(485, 211)
        Me.btnGenerarActaPDF.Margin = New System.Windows.Forms.Padding(2)
        Me.btnGenerarActaPDF.Name = "btnGenerarActaPDF"
        Me.btnGenerarActaPDF.Size = New System.Drawing.Size(176, 43)
        Me.btnGenerarActaPDF.TabIndex = 1
        Me.btnGenerarActaPDF.Text = "Generar acta de evaluación"
        Me.btnGenerarActaPDF.UseVisualStyleBackColor = True
        '
        'cmbCSVGrupo
        '
        Me.cmbCSVGrupo.FormattingEnabled = True
        Me.cmbCSVGrupo.Location = New System.Drawing.Point(336, 36)
        Me.cmbCSVGrupo.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbCSVGrupo.Name = "cmbCSVGrupo"
        Me.cmbCSVGrupo.Size = New System.Drawing.Size(126, 21)
        Me.cmbCSVGrupo.TabIndex = 2
        '
        'lblGrupoClaseCSV
        '
        Me.lblGrupoClaseCSV.AutoSize = True
        Me.lblGrupoClaseCSV.Location = New System.Drawing.Point(235, 39)
        Me.lblGrupoClaseCSV.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblGrupoClaseCSV.Name = "lblGrupoClaseCSV"
        Me.lblGrupoClaseCSV.Size = New System.Drawing.Size(82, 13)
        Me.lblGrupoClaseCSV.TabIndex = 3
        Me.lblGrupoClaseCSV.Text = "Grupo de clase:"
        '
        'btnExportarAlumnoscsv
        '
        Me.btnExportarAlumnoscsv.Location = New System.Drawing.Point(485, 24)
        Me.btnExportarAlumnoscsv.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExportarAlumnoscsv.Name = "btnExportarAlumnoscsv"
        Me.btnExportarAlumnoscsv.Size = New System.Drawing.Size(176, 43)
        Me.btnExportarAlumnoscsv.TabIndex = 4
        Me.btnExportarAlumnoscsv.Text = "Exportar alumnos en CSV"
        Me.btnExportarAlumnoscsv.UseVisualStyleBackColor = True
        '
        'btnExportarProfesoresenJSON
        '
        Me.btnExportarProfesoresenJSON.Location = New System.Drawing.Point(485, 425)
        Me.btnExportarProfesoresenJSON.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExportarProfesoresenJSON.Name = "btnExportarProfesoresenJSON"
        Me.btnExportarProfesoresenJSON.Size = New System.Drawing.Size(176, 43)
        Me.btnExportarProfesoresenJSON.TabIndex = 5
        Me.btnExportarProfesoresenJSON.Text = "Exportar profesores en JSON"
        Me.btnExportarProfesoresenJSON.UseVisualStyleBackColor = True
        '
        'lblCentros
        '
        Me.lblCentros.AutoSize = True
        Me.lblCentros.Location = New System.Drawing.Point(244, 439)
        Me.lblCentros.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblCentros.Name = "lblCentros"
        Me.lblCentros.Size = New System.Drawing.Size(46, 13)
        Me.lblCentros.TabIndex = 7
        Me.lblCentros.Text = "Centros:"
        '
        'cmbCentroJson
        '
        Me.cmbCentroJson.FormattingEnabled = True
        Me.cmbCentroJson.Location = New System.Drawing.Point(336, 437)
        Me.cmbCentroJson.Margin = New System.Windows.Forms.Padding(2)
        Me.cmbCentroJson.Name = "cmbCentroJson"
        Me.cmbCentroJson.Size = New System.Drawing.Size(126, 21)
        Me.cmbCentroJson.TabIndex = 6
        '
        'cmbGrupoExcel
        '
        Me.cmbGrupoExcel.FormattingEnabled = True
        Me.cmbGrupoExcel.Location = New System.Drawing.Point(336, 326)
        Me.cmbGrupoExcel.Name = "cmbGrupoExcel"
        Me.cmbGrupoExcel.Size = New System.Drawing.Size(121, 21)
        Me.cmbGrupoExcel.TabIndex = 8
        '
        'lblGrupoClaseExcel
        '
        Me.lblGrupoClaseExcel.AutoSize = True
        Me.lblGrupoClaseExcel.Location = New System.Drawing.Point(235, 329)
        Me.lblGrupoClaseExcel.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblGrupoClaseExcel.Name = "lblGrupoClaseExcel"
        Me.lblGrupoClaseExcel.Size = New System.Drawing.Size(82, 13)
        Me.lblGrupoClaseExcel.TabIndex = 9
        Me.lblGrupoClaseExcel.Text = "Grupo de clase:"
        '
        'btnExportarAlumnosExcel
        '
        Me.btnExportarAlumnosExcel.Location = New System.Drawing.Point(485, 314)
        Me.btnExportarAlumnosExcel.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExportarAlumnosExcel.Name = "btnExportarAlumnosExcel"
        Me.btnExportarAlumnosExcel.Size = New System.Drawing.Size(176, 43)
        Me.btnExportarAlumnosExcel.TabIndex = 10
        Me.btnExportarAlumnosExcel.Text = "Exportar alumnos en Excel"
        Me.btnExportarAlumnosExcel.UseVisualStyleBackColor = True
        '
        'lblGrupoClaseActa
        '
        Me.lblGrupoClaseActa.AutoSize = True
        Me.lblGrupoClaseActa.Location = New System.Drawing.Point(235, 226)
        Me.lblGrupoClaseActa.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblGrupoClaseActa.Name = "lblGrupoClaseActa"
        Me.lblGrupoClaseActa.Size = New System.Drawing.Size(82, 13)
        Me.lblGrupoClaseActa.TabIndex = 12
        Me.lblGrupoClaseActa.Text = "Grupo de clase:"
        '
        'cmbActa
        '
        Me.cmbActa.FormattingEnabled = True
        Me.cmbActa.Location = New System.Drawing.Point(336, 223)
        Me.cmbActa.Name = "cmbActa"
        Me.cmbActa.Size = New System.Drawing.Size(121, 21)
        Me.cmbActa.TabIndex = 11
        '
        'UserControlExportacion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblGrupoClaseActa)
        Me.Controls.Add(Me.cmbActa)
        Me.Controls.Add(Me.btnExportarAlumnosExcel)
        Me.Controls.Add(Me.lblGrupoClaseExcel)
        Me.Controls.Add(Me.cmbGrupoExcel)
        Me.Controls.Add(Me.lblCentros)
        Me.Controls.Add(Me.cmbCentroJson)
        Me.Controls.Add(Me.btnExportarProfesoresenJSON)
        Me.Controls.Add(Me.btnExportarAlumnoscsv)
        Me.Controls.Add(Me.lblGrupoClaseCSV)
        Me.Controls.Add(Me.cmbCSVGrupo)
        Me.Controls.Add(Me.btnGenerarActaPDF)
        Me.Controls.Add(Me.btnExportarLogsXML)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "UserControlExportacion"
        Me.Size = New System.Drawing.Size(934, 572)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnExportarLogsXML As Button
    Friend WithEvents btnGenerarActaPDF As Button
    Friend WithEvents cmbCSVGrupo As ComboBox
    Friend WithEvents lblGrupoClaseCSV As Label
    Friend WithEvents btnExportarAlumnoscsv As Button
    Friend WithEvents btnExportarProfesoresenJSON As Button
    Friend WithEvents lblCentros As Label
    Friend WithEvents cmbCentroJson As ComboBox
    Friend WithEvents cmbGrupoExcel As ComboBox
    Friend WithEvents lblGrupoClaseExcel As Label
    Friend WithEvents btnExportarAlumnosExcel As Button
    Friend WithEvents lblGrupoClaseActa As Label
    Friend WithEvents cmbActa As ComboBox
End Class
