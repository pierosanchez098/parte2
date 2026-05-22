Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

Public Class UserControlGestionPeriodos

    Private Const BaseUrl As String = "http://localhost/"

    Private Sub UserControlGestionPeriodos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarEstadoActual()
    End Sub

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        If Me.Visible Then
            CargarEstadoActual()
        End If
    End Sub

    Private Sub CargarEstadoActual()
        Dim rol As String = My.Settings.Rol.ToLower().Trim()

        btnActivar.Visible = (rol = "directiu" OrElse rol = "admin")
        btnFinalizar.Visible = (rol = "directiu" OrElse rol = "admin")

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_estado_periodo.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Dim abierto As Boolean = Convert.ToBoolean(jsonRespuesta("periodo_abierto"))

            If abierto Then
                lblEstado.Text = "ESTADO ACTUAL: ABIERTO"
                lblEstado.ForeColor = Color.Green
                btnPonerCalificacion.Visible = (rol = "professor")
            Else
                lblEstado.Text = "ESTADO ACTUAL: CERRADO"
                lblEstado.ForeColor = Color.Red
                btnPonerCalificacion.Visible = False
            End If
        Else
            lblEstado.Text = "Error al cargar estado"
            lblEstado.ForeColor = Color.Red
            btnPonerCalificacion.Visible = False
        End If
    End Sub


    Private Sub btnActivar_Click(sender As Object, e As EventArgs) Handles btnActivar.Click
        CambiarEstadoPeriodo(True)
    End Sub


    Private Sub btnFinalizar_Click(sender As Object, e As EventArgs) Handles btnFinalizar.Click
        CambiarEstadoPeriodo(False)
    End Sub


    Private Sub CambiarEstadoPeriodo(abrir As Boolean)
        Dim accion As String = If(abrir, "abrir", "cerrar")

        Dim result As DialogResult = MessageBox.Show(String.Format("¿Está seguro de que desea {0} el periodo de evaluación?", accion), "Confirmar Acción", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.No Then Return

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&abrir={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            If(abrir, "true", "false")
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("toggle_periodo.php", postData)

        If jsonRespuesta IsNot Nothing Then
            If jsonRespuesta("status").ToString() = "success" Then
                MessageBox.Show(jsonRespuesta("motivo").ToString(), "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                CargarEstadoActual()
            Else
                MessageBox.Show("Error: " & jsonRespuesta("motivo").ToString(), "Operación Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        End If
    End Sub

    Private Sub btnPonerCalificacion_Click(sender As Object, e As EventArgs) Handles btnPonerCalificacion.Click
        Dim ucNotas As New UserControlPonerNotas()
        Dim principal As FormPrincipal = CType(Me.ParentForm, FormPrincipal)
        principal.CargarContenido(ucNotas)
    End Sub


    Private Function EnviarPeticionWeb(endpoint As String, postData As String) As Dictionary(Of String, Object)
        Try
            Dim urlCompleta As String = BaseUrl & endpoint
            Dim request As HttpWebRequest = CType(WebRequest.Create(urlCompleta), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"

            ServicePointManager.ServerCertificateValidationCallback = Function() True

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentLength = byteArray.Length

            Using dataStream As Stream = request.GetRequestStream()
                dataStream.Write(byteArray, 0, byteArray.Length)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseFromServer As String = reader.ReadToEnd()
                    Dim serializer As New JavaScriptSerializer()
                    Dim resultado As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(responseFromServer)

                    If resultado IsNot Nothing AndAlso resultado.ContainsKey("new_token") Then
                        Dim nuevoToken As String = resultado("new_token")?.ToString()
                        If Not String.IsNullOrEmpty(nuevoToken) Then
                            My.Settings.Token = nuevoToken
                            My.Settings.Save()
                        End If
                    End If

                    Return resultado
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

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