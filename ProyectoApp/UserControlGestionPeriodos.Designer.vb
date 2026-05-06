<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControlGestionPeriodos
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
        Me.lblEstado = New System.Windows.Forms.Label()
        Me.lblAnuncio = New System.Windows.Forms.Label()
        Me.btnActivar = New System.Windows.Forms.Button()
        Me.btnFinalizar = New System.Windows.Forms.Button()
        Me.btnPonerCalificacion = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblEstado
        '
        Me.lblEstado.AutoSize = True
        Me.lblEstado.Location = New System.Drawing.Point(200, 208)
        Me.lblEstado.Name = "lblEstado"
        Me.lblEstado.Size = New System.Drawing.Size(0, 20)
        Me.lblEstado.TabIndex = 0
        '
        'lblAnuncio
        '
        Me.lblAnuncio.AutoSize = True
        Me.lblAnuncio.Location = New System.Drawing.Point(112, 49)
        Me.lblAnuncio.Name = "lblAnuncio"
        Me.lblAnuncio.Size = New System.Drawing.Size(168, 20)
        Me.lblAnuncio.TabIndex = 1
        Me.lblAnuncio.Text = "Periodo de evaluación:"
        '
        'btnActivar
        '
        Me.btnActivar.Location = New System.Drawing.Point(143, 330)
        Me.btnActivar.Name = "btnActivar"
        Me.btnActivar.Size = New System.Drawing.Size(173, 68)
        Me.btnActivar.TabIndex = 2
        Me.btnActivar.Text = "Activar"
        Me.btnActivar.UseVisualStyleBackColor = True
        '
        'btnFinalizar
        '
        Me.btnFinalizar.Location = New System.Drawing.Point(386, 330)
        Me.btnFinalizar.Name = "btnFinalizar"
        Me.btnFinalizar.Size = New System.Drawing.Size(173, 68)
        Me.btnFinalizar.TabIndex = 3
        Me.btnFinalizar.Text = "Finalizar"
        Me.btnFinalizar.UseVisualStyleBackColor = True
        '
        'btnPonerCalificacion
        '
        Me.btnPonerCalificacion.Location = New System.Drawing.Point(264, 256)
        Me.btnPonerCalificacion.Name = "btnPonerCalificacion"
        Me.btnPonerCalificacion.Size = New System.Drawing.Size(173, 68)
        Me.btnPonerCalificacion.TabIndex = 4
        Me.btnPonerCalificacion.Text = "Poner notas"
        Me.btnPonerCalificacion.UseVisualStyleBackColor = True
        '
        'UserControlGestionPeriodos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnPonerCalificacion)
        Me.Controls.Add(Me.btnFinalizar)
        Me.Controls.Add(Me.btnActivar)
        Me.Controls.Add(Me.lblAnuncio)
        Me.Controls.Add(Me.lblEstado)
        Me.Name = "UserControlGestionPeriodos"
        Me.Size = New System.Drawing.Size(800, 500)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblEstado As Label
    Friend WithEvents lblAnuncio As Label
    Friend WithEvents btnActivar As Button
    Friend WithEvents btnFinalizar As Button
    Friend WithEvents btnPonerCalificacion As Button
End Class
