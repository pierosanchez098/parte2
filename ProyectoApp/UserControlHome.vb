Public Class UserControlHome
    Private Sub UserControlHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not String.IsNullOrEmpty(My.Settings.Username) Then
            lblUsuario.Text = My.Settings.Username
            lblUsuario.Font = New Font("Segoe UI", 16, FontStyle.Bold)
        ElseIf Not String.IsNullOrEmpty(My.Settings.DniPersona) Then
            lblUsuario.Text = My.Settings.DniPersona
            lblUsuario.Font = New Font("Segoe UI", 14, FontStyle.Regular)
        Else
            lblUsuario.Text = "Usuario no identificado"
            lblUsuario.Font = New Font("Segoe UI", 14, FontStyle.Italic)
        End If

        If Not String.IsNullOrEmpty(My.Settings.Rol) Then
            lblRol.Text = "Rol: " & My.Settings.Rol.ToUpper()
            lblRol.ForeColor = Color.FromArgb(0, 122, 204)
            lblRol.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        Else
            lblRol.Text = "Rol no disponible"
            lblRol.ForeColor = Color.Gray
        End If

        lblMensaje.Text = "Has iniciado sesión correctamente." & vbCrLf & vbCrLf &
                      "Selecciona una opción del menú lateral para continuar."

        lblMensaje.Font = New Font("Segoe UI", 11)
        lblMensaje.ForeColor = Color.FromArgb(80, 80, 80)

    End Sub
End Class
