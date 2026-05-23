Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UserControlExpedienteProfesor

    Private Const BaseUrl As String = "http://localhost/"

    Private Class AlumnoItem
        Public Property DisplayName As String
        Public Property NiaValue As String
    End Class

    Private Sub UserControlExpedienteProfesor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarDataGridViewEstudios()
        EstilizarDataGridView(dgvEstudios)
        CargarGruposDelProfesor()
    End Sub

    Private Sub ConfigurarDataGridViewEstudios()
        If dgvEstudios.Columns.Count > 0 Then Return

        dgvEstudios.Columns.Clear()
        dgvEstudios.Columns.Add("nom_estudi", "Estudio")
        dgvEstudios.Columns.Add("curs_inici", "Año Inicio")
        dgvEstudios.Columns.Add("curs_fi", "Año Fin")
        dgvEstudios.Columns.Add("status", "Estado")
        dgvEstudios.Columns.Add("nota_final", "Nota Final")
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
        CargarAlumnosDelGrupo(nomGrupo)
    End Sub


    Private Sub CargarAlumnosDelGrupo(nomGrupo As String)
        lstAlumnos.DataSource = Nothing
        lstAlumnos.Items.Clear()
        dgvEstudios.Rows.Clear()
        LimpiarFichaDetalle()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&nom_grup={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(nomGrupo)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_alumnos.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            If jsonRespuesta.ContainsKey("alumnos") Then
                Dim listaAlumnos As Object = jsonRespuesta("alumnos")
                Dim datasourceAlumnos As New List(Of AlumnoItem)()

                For Each al As Object In CType(listaAlumnos, IEnumerable)
                    Dim alumnoDict As Dictionary(Of String, Object) = CType(al, Dictionary(Of String, Object))

                    Dim item As New AlumnoItem With {
                        .NiaValue = alumnoDict("nia").ToString(),
                        .DisplayName = alumnoDict("cognom").ToString() & ", " & alumnoDict("nom").ToString()
                    }
                    datasourceAlumnos.Add(item)
                Next

                lstAlumnos.DisplayMember = "DisplayName"
                lstAlumnos.ValueMember = "NiaValue"
                lstAlumnos.DataSource = datasourceAlumnos
                lstAlumnos.SelectedIndex = -1
            End If
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("Error al cargar alumnos: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub lstAlumnos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAlumnos.SelectedIndexChanged
        If lstAlumnos.SelectedIndex = -1 OrElse lstAlumnos.SelectedValue Is Nothing Then Return

        Dim niaSeleccionado As String = lstAlumnos.SelectedValue.ToString()
        CargarExpedienteAlumno(niaSeleccionado)
    End Sub



    Private Sub CargarExpedienteAlumno(nia As String)
        dgvEstudios.Rows.Clear()
        LimpiarFichaDetalle()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&nia={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(nia)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_expediente_alumno.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            lblNombre.Text = jsonRespuesta("nombre_alumno").ToString()
            lblDni.Text = jsonRespuesta("dni_alumno").ToString()
            lblEmail.Text = jsonRespuesta("email_alumno").ToString()

            If jsonRespuesta.ContainsKey("foto") AndAlso jsonRespuesta("foto") IsNot Nothing AndAlso Not String.IsNullOrEmpty(jsonRespuesta("foto").ToString()) Then
                Try
                    Dim fotoBase64 As String = jsonRespuesta("foto").ToString()
                    Dim imageBytes As Byte() = Convert.FromBase64String(fotoBase64)
                    Using ms As New MemoryStream(imageBytes)
                        picFoto.Image = System.Drawing.Image.FromStream(ms)
                    End Using
                Catch ex As Exception
                    picFoto.Image = Nothing
                End Try
            Else
                picFoto.Image = Nothing
            End If

            If jsonRespuesta.ContainsKey("estudis") Then
                Dim listaEstudios As Object = jsonRespuesta("estudis")

                For Each est As Object In CType(listaEstudios, IEnumerable)
                    Dim estudioDict As Dictionary(Of String, Object) = CType(est, Dictionary(Of String, Object))

                    Dim nomEstudi As String = estudioDict("nom_estudi").ToString()
                    Dim cursInici As String = estudioDict("curs_inici").ToString()
                    Dim cursFi As String = estudioDict("curs_fi").ToString()
                    Dim status As String = estudioDict("status").ToString()
                    Dim notaFinal As String = If(estudioDict("nota_final") IsNot Nothing, estudioDict("nota_final").ToString(), "-")

                    dgvEstudios.Rows.Add(nomEstudi, cursInici, cursFi, status, notaFinal)
                Next
            End If
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("Error al cargar el expediente: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub LimpiarFichaDetalle()
        lblNombre.Text = "-"
        lblDni.Text = "-"
        lblEmail.Text = "-"
        picFoto.Image = Nothing
    End Sub


    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs) Handles btnExportarPDF.Click
        If lblNombre.Text = "-" OrElse String.IsNullOrEmpty(lblNombre.Text) Then
            MessageBox.Show("Por favor, seleccione un alumno válido para exportar su expediente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Documento PDF (*.pdf)|*.pdf"
        sfd.FileName = "Expediente_" & lblNombre.Text.Replace(" ", "_") & ".pdf"

        If sfd.ShowDialog() = DialogResult.OK Then
            Dim doc As New Document(PageSize.A4, 36, 36, 36, 36)

            Try
                Using fs As New FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None)
                    PdfWriter.GetInstance(doc, fs)
                    doc.Open()

                    Dim fontTitulo As Font = FontFactory.GetFont("Segoe UI", 18, iTextSharp.text.Font.BOLD, BaseColor.BLACK)
                    Dim fontSubtitulo As Font = FontFactory.GetFont("Segoe UI", 12, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)
                    Dim fontTexto As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                    Dim fontCabecera As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)

                    Dim titulo As New Paragraph("EXPEDIENTE ACADÉMICO DE ESTUDIANTE", fontTitulo)
                    titulo.Alignment = Element.ALIGN_CENTER
                    titulo.SpacingAfter = 20
                    doc.Add(titulo)

                    Dim tablaFicha As New PdfPTable(2)
                    tablaFicha.WidthPercentage = 100
                    tablaFicha.SetWidths({75.0F, 25.0F})
                    tablaFicha.SpacingAfter = 25

                    Dim celdaDatos As New PdfPCell()
                    celdaDatos.Border = iTextSharp.text.Rectangle.NO_BORDER
                    celdaDatos.AddElement(New Paragraph("Nombre Completo: " & lblNombre.Text, fontTexto))
                    celdaDatos.AddElement(New Paragraph("DNI / Identificación: " & lblDni.Text, fontTexto))
                    celdaDatos.AddElement(New Paragraph("Correo Electrónico: " & lblEmail.Text, fontTexto))
                    If lstAlumnos.SelectedValue IsNot Nothing Then
                        celdaDatos.AddElement(New Paragraph("NIA Estudiante: " & lstAlumnos.SelectedValue.ToString(), fontTexto))
                    End If
                    tablaFicha.AddCell(celdaDatos)

                    Dim celdaFoto As New PdfPCell()
                    celdaFoto.Border = iTextSharp.text.Rectangle.NO_BORDER
                    celdaFoto.HorizontalAlignment = Element.ALIGN_RIGHT

                    If picFoto.Image IsNot Nothing Then
                        Try
                            Using ms As New MemoryStream()
                                picFoto.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                                Dim imgPdf As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ms.ToArray())
                                imgPdf.ScaleToFit(85.0F, 110.0F)
                                celdaFoto.AddElement(imgPdf)
                            End Using
                        Catch ex As Exception
                        End Try
                    End If
                    tablaFicha.AddCell(celdaFoto)
                    doc.Add(tablaFicha)

                    Dim subtitulo As New Paragraph("HISTORIAL ACADÉMICO DE ASIGNATURAS Y ESTUDIOS", fontSubtitulo)
                    subtitulo.SpacingAfter = 10
                    doc.Add(subtitulo)

                    Dim tablaEstudios As New PdfPTable(dgvEstudios.Columns.Count)
                    tablaEstudios.WidthPercentage = 100
                    tablaEstudios.SetWidths({40.0F, 15.0F, 15.0F, 15.0F, 15.0F})
                    For Each col As DataGridViewColumn In dgvEstudios.Columns
                        Dim cellHeader As New PdfPCell(New Phrase(col.HeaderText, fontCabecera))
                        cellHeader.BackgroundColor = New BaseColor(30, 30, 35)
                        cellHeader.HorizontalAlignment = Element.ALIGN_CENTER
                        cellHeader.Padding = 6
                        tablaEstudios.AddCell(cellHeader)
                    Next

                    For Each row As DataGridViewRow In dgvEstudios.Rows
                        If row.IsNewRow Then Continue For

                        For Each cell As DataGridViewCell In row.Cells
                            Dim valorCelda As String = If(cell.Value IsNot Nothing, cell.Value.ToString(), "-")
                            Dim cellPdf As New PdfPCell(New Phrase(valorCelda, fontTexto))
                            cellPdf.Padding = 5
                            cellPdf.HorizontalAlignment = Element.ALIGN_CENTER

                            If cell.ColumnIndex = 0 Then
                                cellPdf.HorizontalAlignment = Element.ALIGN_LEFT
                            End If

                            tablaEstudios.AddCell(cellPdf)
                        Next
                    Next

                    doc.Add(tablaEstudios)
                    doc.Close()

                    MessageBox.Show("El expediente se ha exportado correctamente a PDF.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            Catch ex As Exception
                If doc.IsOpen Then doc.Close()
                MessageBox.Show("Error al generar el archivo PDF: " & ex.Message, "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
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
            MessageBox.Show("Error de red HTTP: " & ex.Message, "Error de comunicación", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            .RowHeadersVisible = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeRows = False
            .DefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 35)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersHeight = 40
            .EnableHeadersVisualStyles = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250)
        End With
    End Sub
End Class