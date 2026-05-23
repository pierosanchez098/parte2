Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UserControlExpedienteProfesor

    Private Const BaseUrl As String = "http://localhost/"
    Private datosAlumnoActual As Dictionary(Of String, Object)

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
            datosAlumnoActual = jsonRespuesta

            lblNombre.Text = jsonRespuesta("nombre_alumno").ToString()
            lblDni.Text = jsonRespuesta("dni_alumno").ToString()
            lblEmail.Text = jsonRespuesta("email_alumno").ToString()


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
        datosAlumnoActual = Nothing
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

                    Dim colorPrimario As New BaseColor(30, 30, 35)
                    Dim fontTitulo As Font = FontFactory.GetFont("Segoe UI", 18, iTextSharp.text.Font.BOLD, colorPrimario)
                    Dim fontCentro As Font = FontFactory.GetFont("Segoe UI", 11, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)
                    Dim fontSubtitulo As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.ITALIC, BaseColor.GRAY)
                    Dim fontSeccion As Font = FontFactory.GetFont("Segoe UI", 12, iTextSharp.text.Font.BOLD, colorPrimario)
                    Dim fontTexto As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                    Dim fontCabecera As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)
                    Dim fontFirmas As Font = FontFactory.GetFont("Segoe UI", 9, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)

                    Dim tablaCabecera As New PdfPTable(2)
                    tablaCabecera.WidthPercentage = 100
                    tablaCabecera.SetWidths({70.0F, 30.0F})

                    Dim celdaTextosCab As New PdfPCell()
                    celdaTextosCab.Border = iTextSharp.text.Rectangle.NO_BORDER

                    Dim pTitulo As New Paragraph("EXPEDIENTE ACADÉMICO DE ESTUDIANTE", fontTitulo)
                    pTitulo.SpacingAfter = 2
                    celdaTextosCab.AddElement(pTitulo)

                    Dim nombreCentro As String = "Centro no asignado"
                    If datosAlumnoActual IsNot Nothing AndAlso datosAlumnoActual.ContainsKey("centro_educativo") Then
                        nombreCentro = datosAlumnoActual("centro_educativo").ToString()
                    End If

                    Dim pCentro As New Paragraph("Centro Educativo: " & nombreCentro, fontCentro)
                    pCentro.SpacingAfter = 2
                    celdaTextosCab.AddElement(pCentro)

                    Dim pSub As New Paragraph("EVALIS - Resguardo Informativo Oficial", fontSubtitulo)
                    celdaTextosCab.AddElement(pSub)
                    tablaCabecera.AddCell(celdaTextosCab)

                    Dim celdaLogoCab As New PdfPCell()
                    celdaLogoCab.Border = iTextSharp.text.Rectangle.NO_BORDER
                    celdaLogoCab.HorizontalAlignment = Element.ALIGN_RIGHT

                    If datosAlumnoActual IsNot Nothing AndAlso datosAlumnoActual.ContainsKey("logo_centro") AndAlso datosAlumnoActual("logo_centro") IsNot Nothing AndAlso Not String.IsNullOrEmpty(datosAlumnoActual("logo_centro").ToString()) Then
                        Try
                            Dim logoStr As String = datosAlumnoActual("logo_centro").ToString()
                            Dim imgLogo As iTextSharp.text.Image = Nothing

                            If logoStr.StartsWith("data:image") OrElse logoStr.Length > 200 Then
                                Dim base64Data As String = logoStr.Substring(logoStr.IndexOf(",") + 1)
                                Dim imageBytes As Byte() = Convert.FromBase64String(base64Data)
                                imgLogo = iTextSharp.text.Image.GetInstance(imageBytes)
                            Else
                                imgLogo = iTextSharp.text.Image.GetInstance(logoStr)
                            End If

                            If imgLogo IsNot Nothing Then
                                imgLogo.ScaleToFit(110.0F, 50.0F)
                                imgLogo.Alignment = Element.ALIGN_RIGHT
                                celdaLogoCab.AddElement(imgLogo)
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                    tablaCabecera.AddCell(celdaLogoCab)

                    tablaCabecera.SpacingAfter = 25
                    doc.Add(tablaCabecera)

                    Dim tablaFicha As New PdfPTable(1)
                    tablaFicha.WidthPercentage = 100
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
                    doc.Add(tablaFicha)

                    Dim subtitulo As New Paragraph("EXPEDIENTE ACADÉMICO", fontSeccion)
                    subtitulo.SpacingAfter = 10
                    doc.Add(subtitulo)

                    Dim tablaEstudios As New PdfPTable(dgvEstudios.Columns.Count)
                    tablaEstudios.WidthPercentage = 100
                    tablaEstudios.SetWidths({40.0F, 15.0F, 15.0F, 15.0F, 15.0F})

                    For Each col As DataGridViewColumn In dgvEstudios.Columns
                        Dim cellHeader As New PdfPCell(New Phrase(col.HeaderText, fontCabecera))
                        cellHeader.BackgroundColor = colorPrimario
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

                    Dim tablaFirmas As New PdfPTable(3)
                    tablaFirmas.WidthPercentage = 100
                    tablaFirmas.SpacingBefore = 60
                    tablaFirmas.SetWidths({30.0F, 35.0F, 30.0F})

                    Dim cellProf As New PdfPCell(New Paragraph("Firma del Profesor" & Environment.NewLine & "_______________________", fontFirmas))
                    cellProf.Border = iTextSharp.text.Rectangle.NO_BORDER
                    cellProf.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaFirmas.AddCell(cellProf)

                    Dim cellJefe As New PdfPCell(New Paragraph("Firma del Jefe de Estudios" & Environment.NewLine & "_______________________", fontFirmas))
                    cellJefe.Border = iTextSharp.text.Rectangle.NO_BORDER
                    cellJefe.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaFirmas.AddCell(cellJefe)

                    Dim cellCent As New PdfPCell(New Paragraph("Sello / Firma del Centro" & Environment.NewLine & "_______________________", fontFirmas))
                    cellCent.Border = iTextSharp.text.Rectangle.NO_BORDER
                    cellCent.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaFirmas.AddCell(cellCent)

                    doc.Add(tablaFirmas)

                    Dim pPie As New Paragraph(Environment.NewLine & "Este documento sirve como resguardo informativo del expediente académico del alumno en la fecha indicada y carece de validez legal de certificación arancelaria.", fontSubtitulo)
                    pPie.Alignment = Element.ALIGN_CENTER
                    pPie.SpacingBefore = 25
                    doc.Add(pPie)

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