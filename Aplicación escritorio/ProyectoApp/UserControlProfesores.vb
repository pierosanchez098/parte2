Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

Public Class UserControlProfesores

    Private Const BaseUrl As String = "http://localhost/"
    Private Const DefaultPhotoUrl As String = "https://cdn.pixabay.com/photo/2023/02/18/11/00/icon-7797704_640.png"

    Private _datosCargados As Boolean = False

    Private Sub UserControlProfesores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not _datosCargados Then
            DeterminarFlujoPorRol()
            _datosCargados = True
        End If
    End Sub


    Private Sub DeterminarFlujoPorRol()
        Dim rol As String = My.Settings.Rol.ToLower().Trim()

        If rol = "admin" OrElse rol = "administrador" Then
            lblSeleccioneCentro.Visible = True
            cmbCentro.Visible = True

            If cmbCentro.Items.Count = 0 Then
                CargarCentrosAsignados()
            End If
        Else
            lblSeleccioneCentro.Visible = False
            cmbCentro.Visible = False

            CargarOrlaProfesores("auto")
        End If
    End Sub


    Private Sub CargarCentrosAsignados()
        cmbCentro.Items.Clear()

        Dim postData As String = String.Format(
        "token={0}&user_agent_hash={1}",
        Uri.EscapeDataString(My.Settings.Token),
        Uri.EscapeDataString(GenerarHardwareHash())
    )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_centros_asignados.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then

            Dim listaCentros As System.Collections.ArrayList = CType(jsonRespuesta("centros"), System.Collections.ArrayList)

            cmbCentro.DisplayMember = "Value"
            cmbCentro.ValueMember = "Key"

            For Each centro As Dictionary(Of String, Object) In listaCentros
                Dim idCentro As String = centro("id_centre").ToString()
                Dim nombreCentro As String = centro("nom").ToString()

                cmbCentro.Items.Add(New KeyValuePair(Of String, String)(idCentro, nombreCentro))
            Next

            If cmbCentro.Items.Count > 0 Then
                cmbCentro.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub cmbCentro_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCentro.SelectedIndexChanged
        If cmbCentro.SelectedIndex = -1 Then Return

        Dim idCentro As String = DirectCast(cmbCentro.SelectedItem, KeyValuePair(Of String, String)).Key
        CargarOrlaProfesores(idCentro)
    End Sub

    Private Sub CargarOrlaProfesores(idCentro As String)
        flpProfesores.SuspendLayout()
        flpProfesores.Controls.Clear()

        Dim postData As String = String.Format(
        "token={0}&user_agent_hash={1}&id_centre={2}",
        Uri.EscapeDataString(My.Settings.Token),
        Uri.EscapeDataString(GenerarHardwareHash()),
        Uri.EscapeDataString(idCentro)
    )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_profesors_desktop.php", postData)

        If jsonRespuesta Is Nothing Then
            MessageBox.Show("No se recibió respuesta del servidor.", "Error de comunicación", MessageBoxButtons.OK, MessageBoxIcon.Error)
            flpProfesores.ResumeLayout()
            Return
        End If

        If jsonRespuesta.ContainsKey("status") AndAlso jsonRespuesta("status").ToString() = "error" Then
            Dim motivo As String = If(jsonRespuesta.ContainsKey("motivo"), jsonRespuesta("motivo").ToString(), "Error desconocido en el servidor.")
            MessageBox.Show(motivo, "Aviso del Servidor", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            flpProfesores.ResumeLayout()
            Return
        End If

        If jsonRespuesta.ContainsKey("status") AndAlso jsonRespuesta("status").ToString() = "success" AndAlso jsonRespuesta.ContainsKey("profesores") Then
            Dim listaProfesores As IEnumerable = CType(jsonRespuesta("profesores"), IEnumerable)

            For Each item As Object In listaProfesores
                Dim prof As Dictionary(Of String, Object) = CType(item, Dictionary(Of String, Object))

                Dim panel As New Panel With {.Size = New Size(290, 370), .Margin = New Padding(12), .BackColor = Color.White, .BorderStyle = BorderStyle.FixedSingle}
                Dim pb As New PictureBox With {.Size = New Size(270, 190), .Location = New Point(10, 10), .SizeMode = PictureBoxSizeMode.Zoom, .BorderStyle = BorderStyle.FixedSingle}

                Dim fotoUrl As String = If(prof.ContainsKey("foto"), prof("foto").ToString().Trim(), "")
                Try
                    pb.Load(If(String.IsNullOrEmpty(fotoUrl), DefaultPhotoUrl, fotoUrl))
                Catch
                    pb.Load(DefaultPhotoUrl)
                End Try

                Dim lblNombre As New Label With {.Text = prof("nombre_completo").ToString(), .Font = New Font("Segoe UI", 11, FontStyle.Bold), .Location = New Point(10, 210), .Size = New Size(270, 25)}
                Dim lblEmail As New Label With {.Text = "✉ " & prof("email").ToString(), .Font = New Font("Segoe UI", 9), .Location = New Point(10, 240), .Size = New Size(270, 20), .ForeColor = Color.DarkSlateGray}
                Dim lblCargo As New Label With {.Text = "💼 Cargo: " & prof("carrec").ToString(), .Font = New Font("Segoe UI", 9, FontStyle.Italic), .Location = New Point(10, 265), .Size = New Size(270, 40), .ForeColor = Color.Navy}
                Dim lblDept As New Label With {.Text = "🏢 Dept: " & prof("departament").ToString(), .Font = New Font("Segoe UI", 9, FontStyle.Bold), .Location = New Point(10, 310), .Size = New Size(270, 20), .ForeColor = Color.DarkGreen}

                panel.Controls.Add(pb)
                panel.Controls.Add(lblNombre)
                panel.Controls.Add(lblEmail)
                panel.Controls.Add(lblCargo)
                panel.Controls.Add(lblDept)

                flpProfesores.Controls.Add(panel)
            Next
        Else
            MessageBox.Show("La respuesta del servidor no tiene un formato reconocido.", "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        flpProfesores.ResumeLayout()
    End Sub

    Private Function EnviarPeticionWeb(endpoint As String, postData As String) As Dictionary(Of String, Object)
        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create(BaseUrl & endpoint), HttpWebRequest)
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
                    Dim jsonString As String = reader.ReadToEnd()
                    Dim serializer As New JavaScriptSerializer()
                    Dim resultado As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(jsonString)

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
        Catch ex As WebException
            If ex.Response IsNot Nothing Then
                Using errorStream As Stream = ex.Response.GetResponseStream()
                    Using reader As New StreamReader(errorStream)
                        Dim errorDelServidor As String = reader.ReadToEnd()
                        MessageBox.Show("El servidor PHP devolvió un error:" & vbCrLf & errorDelServidor,
                                    "Error Interno del Servidor (500)",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error)
                    End Using
                End Using
            Else
                MessageBox.Show("No se pudo conectar con el servidor: " & ex.Message, "Error de Red")
            End If
            Return Nothing
        Catch ex As Exception
            MessageBox.Show("Error inesperado: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Private Function GenerarHardwareHash() As String
        Dim infoHardware As String = Environment.MachineName & Environment.UserName & Environment.ProcessorCount.ToString()
        Using sha256 As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(infoHardware))
            Dim sb As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1 : sb.Append(bytes(i).ToString("x2")) : Next
            Return sb.ToString()
        End Using
    End Function

End Class