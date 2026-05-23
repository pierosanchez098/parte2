Imports System.Net.Http
Imports System.Text
Imports System.Web.Script.Serialization

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim ColorPrimario As Color = Color.FromArgb(37, 99, 235)
        Dim ColorFondo As Color = Color.FromArgb(248, 250, 252)

        Me.BackColor = ColorFondo
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "Evalis"
        Me.Font = New Font("Segoe UI", 10)

        txtPass.PasswordChar = "●"c

        btnLogin.FlatStyle = FlatStyle.Flat
        btnLogin.FlatAppearance.BorderSize = 0
        btnLogin.BackColor = ColorPrimario
        btnLogin.ForeColor = Color.White
        btnLogin.Cursor = Cursors.Hand
        btnLogin.Text = "Iniciar sesión"

        txtUser.BorderStyle = BorderStyle.FixedSingle
        txtPass.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim user As String = txtUser.Text.Trim()
        Dim pass As String = txtPass.Text.Trim()

        If String.IsNullOrEmpty(user) OrElse String.IsNullOrEmpty(pass) Then
            MessageBox.Show("Rellene todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim baseUrl As String = "https://localhost/login_desktop.php"
        Dim userAgentHash As String = GenerarHardwareHash()


        Dim handler As New HttpClientHandler()
        handler.ServerCertificateCustomValidationCallback = Function(req, cert, chain, errors) True

        Using client As New HttpClient(handler)
            Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"user", user},
                {"pass", pass},
                {"user_agent_hash", userAgentHash}
            })

            Try
                Dim response As HttpResponseMessage = Await client.PostAsync(baseUrl, content)
                Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Dim serializer As New JavaScriptSerializer()
                    Dim result As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(jsonResponse)

                    If result IsNot Nothing AndAlso result.ContainsKey("pot_entrar") AndAlso Convert.ToBoolean(result("pot_entrar")) Then

                        Dim rol As String = If(result.ContainsKey("rol"), result("rol").ToString().ToLower().Trim(), "estudiant")

                        Dim token As String = If(result.ContainsKey("token"), result("token").ToString(), "")
                        Dim dniPersona As String = If(result.ContainsKey("dni_persona"), result("dni_persona").ToString(), "")
                        Dim username As String = user

                        My.Settings.Token = token
                        My.Settings.DniPersona = dniPersona
                        My.Settings.Rol = rol
                        My.Settings.Username = username
                        My.Settings.Save()

                        MessageBox.Show("Bienvenido a Evalis Escritorio", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        Dim principal As New FormPrincipal()
                        principal.Show()
                        Me.Hide()
                    Else
                        Dim errorMsg As String = If(result.ContainsKey("tipus_derror"), result("tipus_derror").ToString(), "Usuario o contraseña incorrectos")
                        MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Else
                    MessageBox.Show("Error de conexión con el servidor: " & response.StatusCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            Catch ex As Exception
                MessageBox.Show("Error crítico en el proceso de Login: " & ex.Message, "Excepción", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Function GenerarHardwareHash() As String
        Dim infoHardware As String = Environment.MachineName & Environment.UserName & Environment.ProcessorCount.ToString()
        Using sha256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(infoHardware))
            Dim sb As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1
                sb.Append(bytes(i).ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function

End Class