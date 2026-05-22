Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization

Public Class UserControlPonerNotas

    Private Const BaseUrl As String = "http://localhost/"

    Private Sub UserControlPonerNotas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarDataGridView()
        EstilizarDataGridView(dgvNotas)
        CargarGruposDelProfesor()
    End Sub

    Private Sub ConfigurarDataGridView()
        If dgvNotas.Columns.Count > 0 Then Return

        dgvNotas.Columns.Clear()

        dgvNotas.Columns.Add("nia", "NIA")
        dgvNotas.Columns("nia").Visible = False

        dgvNotas.Columns.Add("alumno", "Alumno")
        dgvNotas.Columns("alumno").Width = 300
        dgvNotas.Columns("alumno").ReadOnly = True

        dgvNotas.Columns.Add("nota", "Nota")
        dgvNotas.Columns("nota").Width = 100
    End Sub


    Private Sub CargarGruposDelProfesor()
        cmbGrupo.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&dni_profesor={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(My.Settings.DniPersona)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_grupos.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            If jsonRespuesta.ContainsKey("grupos") Then
                Dim listaGrupos As Object = jsonRespuesta("grupos")

                For Each g As Object In CType(listaGrupos, IEnumerable)
                    Dim grupoDict As Dictionary(Of String, Object) = CType(g, Dictionary(Of String, Object))
                    Dim nom As String = grupoDict("nom").ToString()
                    Dim aula As String = grupoDict("aula").ToString()
                    cmbGrupo.Items.Add(nom & " - " & aula)
                Next
            End If
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("Error al cargar grupos: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub cmbGrupo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbGrupo.SelectedIndexChanged
        If cmbGrupo.SelectedIndex = -1 Then Return
        Dim nomGrupo As String = cmbGrupo.Text.Split("-"c)(0).Trim()
        CargarUnidades(nomGrupo)
    End Sub


    Private Sub CargarUnidades(nomGrupo As String)
        cmbUnidad.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&nom_grup={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(nomGrupo)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_unidades.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            If jsonRespuesta.ContainsKey("unidades") Then
                Dim listaUnidades As Object = jsonRespuesta("unidades")

                For Each u As Object In CType(listaUnidades, IEnumerable)
                    Dim unidadDict As Dictionary(Of String, Object) = CType(u, Dictionary(Of String, Object))
                    Dim item As New UnidadItem With {
                        .Text = unidadDict("unidad").ToString(),
                        .Value = Convert.ToInt32(unidadDict("id"))
                    }
                    cmbUnidad.Items.Add(item)
                Next
            End If
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("Error al cargar unidades: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub cmbUnidad_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbUnidad.SelectedIndexChanged
        If cmbUnidad.SelectedIndex = -1 Then Return

        Dim item As UnidadItem = DirectCast(cmbUnidad.SelectedItem, UnidadItem)
        CargarAlumnos(item.Value)
    End Sub


    Private Sub CargarAlumnos(idUnidad As Integer)
        dgvNotas.Rows.Clear()
        Dim nomGrupo As String = cmbGrupo.Text.Split("-"c)(0).Trim()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&uf_id={2}&nom_grup={3}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            idUnidad.ToString(),
            Uri.EscapeDataString(nomGrupo)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_alumnos_notas.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            If jsonRespuesta.ContainsKey("alumnos") Then
                Dim listaAlumnos As Object = jsonRespuesta("alumnos")

                For Each al As Object In CType(listaAlumnos, IEnumerable)
                    Dim alumnoDict As Dictionary(Of String, Object) = CType(al, Dictionary(Of String, Object))

                    Dim nia As String = alumnoDict("nia").ToString()
                    Dim alumno As String = alumnoDict("alumno").ToString()

                    Dim notaStr As String = ""
                    If alumnoDict.ContainsKey("nota") AndAlso alumnoDict("nota") IsNot Nothing Then
                        notaStr = alumnoDict("nota").ToString()
                    End If

                    dgvNotas.Rows.Add(nia, alumno, notaStr)
                Next
            End If
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("Error al cargar alumnos: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub btnGuardarNotas_Click(sender As Object, e As EventArgs) Handles btnGuardarNotas.Click
        If cmbUnidad.SelectedIndex = -1 Then
            MessageBox.Show("Selecciona primero una unidad", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim itemUnidad As UnidadItem = DirectCast(cmbUnidad.SelectedItem, UnidadItem)
        Dim idUnidad As Integer = itemUnidad.Value

        Dim listaNotas As New List(Of Dictionary(Of String, Object))()

        For Each row As DataGridViewRow In dgvNotas.Rows
            If row.IsNewRow Then Continue For

            Dim nia As String = row.Cells("nia").Value?.ToString()
            Dim nota As String = row.Cells("nota").Value?.ToString().Trim()

            If Not String.IsNullOrEmpty(nia) AndAlso Not String.IsNullOrEmpty(nota) Then
                Dim notaDict As New Dictionary(Of String, Object) From {
                    {"nia", Convert.ToInt32(nia)},
                    {"nota", Convert.ToDecimal(nota)}
                }
                listaNotas.Add(notaDict)
            End If
        Next

        If listaNotas.Count = 0 Then
            MessageBox.Show("No hay notas válidas para guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim jss As New JavaScriptSerializer()
        Dim jsonNotas As String = jss.Serialize(listaNotas)

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&dni_profesor={2}&uf_id={3}&notas={4}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(My.Settings.DniPersona),
            idUnidad.ToString(),
            Uri.EscapeDataString(jsonNotas)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("guardar_notas.php", postData)

        If jsonRespuesta IsNot Nothing Then
            If jsonRespuesta("status").ToString() = "success" Then
                MessageBox.Show(jsonRespuesta("motivo").ToString(), "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("Error: " & jsonRespuesta("motivo").ToString(), "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        End If
    End Sub

    Private Sub dgvNotas_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvNotas.CellValidating
        If dgvNotas.Columns(e.ColumnIndex).Name = "nota" Then
            Dim valor As String = e.FormattedValue?.ToString().Trim()

            If String.IsNullOrEmpty(valor) Then Return

            If Not IsNumeric(valor) Then
                MessageBox.Show("La nota debe ser un número", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.Cancel = True
                Return
            End If

            Dim nota As Decimal = Convert.ToDecimal(valor)

            If nota < 0 OrElse nota > 10 Then
                MessageBox.Show("La nota debe estar entre 0 y 10", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                e.Cancel = True
            End If
        End If
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

                    Dim jss As New JavaScriptSerializer()
                    Dim resultado As Dictionary(Of String, Object) = jss.Deserialize(Of Dictionary(Of String, Object))(responseFromServer)

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
            MessageBox.Show("Error crítico de comunicación con el servidor: " & ex.Message, "Error de red", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Sub EstilizarDataGridView(dgv As DataGridView)
        With dgv
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            .RowHeadersVisible = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False

            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204)
            .DefaultCellStyle.SelectionForeColor = Color.White
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)

            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 35)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 8, 8, 8)
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
            .ColumnHeadersHeight = 45

            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250)

            .EnableHeadersVisualStyles = False
        End With
    End Sub
End Class