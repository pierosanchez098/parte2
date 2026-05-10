<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPrincipal
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.pnlSidebar = New System.Windows.Forms.Panel()
        Me.btnProfesores = New System.Windows.Forms.Button()
        Me.btnCerrarSesion = New System.Windows.Forms.Button()
        Me.btnGestionPeriodos = New System.Windows.Forms.Button()
        Me.btnInicio = New System.Windows.Forms.Button()
        Me.pnlContenido = New System.Windows.Forms.Panel()
        Me.btnExportacion = New System.Windows.Forms.Button()
        Me.btnCerrar = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlSidebar.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.pnlSidebar)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlContenido)
        Me.SplitContainer1.Size = New System.Drawing.Size(2003, 1058)
        Me.SplitContainer1.SplitterDistance = 300
        Me.SplitContainer1.TabIndex = 0
        '
        'pnlSidebar
        '
        Me.pnlSidebar.Controls.Add(Me.btnCerrar)
        Me.pnlSidebar.Controls.Add(Me.btnExportacion)
        Me.pnlSidebar.Controls.Add(Me.btnProfesores)
        Me.pnlSidebar.Controls.Add(Me.btnCerrarSesion)
        Me.pnlSidebar.Controls.Add(Me.btnGestionPeriodos)
        Me.pnlSidebar.Controls.Add(Me.btnInicio)
        Me.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlSidebar.Location = New System.Drawing.Point(0, 0)
        Me.pnlSidebar.Name = "pnlSidebar"
        Me.pnlSidebar.Size = New System.Drawing.Size(300, 1058)
        Me.pnlSidebar.TabIndex = 0
        '
        'btnProfesores
        '
        Me.btnProfesores.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnProfesores.Location = New System.Drawing.Point(0, 250)
        Me.btnProfesores.Name = "btnProfesores"
        Me.btnProfesores.Size = New System.Drawing.Size(300, 78)
        Me.btnProfesores.TabIndex = 3
        Me.btnProfesores.Text = "Profesores"
        Me.btnProfesores.UseVisualStyleBackColor = True
        '
        'btnCerrarSesion
        '
        Me.btnCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCerrarSesion.Location = New System.Drawing.Point(0, 977)
        Me.btnCerrarSesion.Name = "btnCerrarSesion"
        Me.btnCerrarSesion.Size = New System.Drawing.Size(300, 78)
        Me.btnCerrarSesion.TabIndex = 2
        Me.btnCerrarSesion.Text = "Cerrar sesión"
        Me.btnCerrarSesion.UseVisualStyleBackColor = True
        '
        'btnGestionPeriodos
        '
        Me.btnGestionPeriodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnGestionPeriodos.Location = New System.Drawing.Point(0, 177)
        Me.btnGestionPeriodos.Name = "btnGestionPeriodos"
        Me.btnGestionPeriodos.Size = New System.Drawing.Size(300, 78)
        Me.btnGestionPeriodos.TabIndex = 1
        Me.btnGestionPeriodos.Text = "Gestión evaluación"
        Me.btnGestionPeriodos.UseVisualStyleBackColor = True
        '
        'btnInicio
        '
        Me.btnInicio.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnInicio.Location = New System.Drawing.Point(0, 104)
        Me.btnInicio.Name = "btnInicio"
        Me.btnInicio.Size = New System.Drawing.Size(300, 78)
        Me.btnInicio.TabIndex = 0
        Me.btnInicio.Text = "Inicio"
        Me.btnInicio.UseVisualStyleBackColor = True
        '
        'pnlContenido
        '
        Me.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContenido.Location = New System.Drawing.Point(0, 0)
        Me.pnlContenido.Name = "pnlContenido"
        Me.pnlContenido.Size = New System.Drawing.Size(1699, 1058)
        Me.pnlContenido.TabIndex = 0
        '
        'btnExportacion
        '
        Me.btnExportacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExportacion.Location = New System.Drawing.Point(0, 324)
        Me.btnExportacion.Name = "btnExportacion"
        Me.btnExportacion.Size = New System.Drawing.Size(300, 78)
        Me.btnExportacion.TabIndex = 5
        Me.btnExportacion.Text = "Exportación de datos"
        Me.btnExportacion.UseVisualStyleBackColor = True
        '
        'btnCerrar
        '
        Me.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCerrar.Location = New System.Drawing.Point(0, 807)
        Me.btnCerrar.Name = "btnCerrar"
        Me.btnCerrar.Size = New System.Drawing.Size(300, 78)
        Me.btnCerrar.TabIndex = 6
        Me.btnCerrar.Text = "Cerrar sesión"
        Me.btnCerrar.UseVisualStyleBackColor = True
        '
        'FormPrincipal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(2003, 1058)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "FormPrincipal"
        Me.Text = "FormPrincipal"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlSidebar.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents pnlSidebar As Panel
    Friend WithEvents btnInicio As Button
    Friend WithEvents pnlContenido As Panel
    Friend WithEvents btnCerrarSesion As Button
    Friend WithEvents btnGestionPeriodos As Button
    Friend WithEvents btnProfesores As Button
    Friend WithEvents btnExportacion As Button
    Friend WithEvents btnCerrar As Button
End Class
