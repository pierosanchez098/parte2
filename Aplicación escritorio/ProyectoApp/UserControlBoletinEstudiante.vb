Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UserControlBoletinEstudiante

    Private Const BaseUrl As String = "http://localhost/"
    Private CursoReporte As String = "2025-2026"
    Private datosBoletinActual As Dictionary(Of String, Object)

    Private Sub UserControlBoletinEstudiante_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarDataGridViewBoletin()
        EstilizarDataGridView(dgvBoletin)
        CargarBoletinNotas()
    End Sub

    Private Sub ConfigurarDataGridViewBoletin()
        If dgvBoletin.Columns.Count > 0 Then Return

        dgvBoletin.Columns.Clear()
        dgvBoletin.Columns.Add("modul", "Módulo")
        dgvBoletin.Columns.Add("unitat", "Unidad")
        dgvBoletin.Columns.Add("nota", "Nota")
        dgvBoletin.Columns.Add("data_nota", "Fecha de Evaluación")
    End Sub

    Private Sub CargarBoletinNotas()
        dgvBoletin.Rows.Clear()
        datosBoletinActual = Nothing

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&dni_persona={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(My.Settings.DniPersona)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_boletin_estudiante.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            datosBoletinActual = jsonRespuesta

            If jsonRespuesta.ContainsKey("curso") Then
                CursoReporte = jsonRespuesta("curso").ToString()
            End If

            If jsonRespuesta.ContainsKey("notas") Then
                Dim listaNotas As Object = jsonRespuesta("notas")

                For Each n As Object In CType(listaNotas, IEnumerable)
                    Dim notaDict As Dictionary(Of String, Object) = CType(n, Dictionary(Of String, Object))

                    Dim modul As String = notaDict("modul").ToString()
                    Dim unitat As String = notaDict("unitat").ToString()
                    Dim nota As String = notaDict("nota").ToString()
                    Dim fecha As String = notaDict("data_nota").ToString()

                    dgvBoletin.Rows.Add(modul, unitat, nota, fecha)
                Next
            End If
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("Error al cargar las calificaciones: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs) Handles btnExportarPDF.Click
        If dgvBoletin.Rows.Count = 0 Then
            MessageBox.Show("No hay calificaciones disponibles en el boletín para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Documento PDF (*.pdf)|*.pdf"
        sfd.FileName = "Boletin_Notas_" & My.Settings.DniPersona & ".pdf"

        If sfd.ShowDialog() = DialogResult.OK Then
            Dim doc As New Document(PageSize.A4, 36, 36, 36, 36)

            Try
                Using fs As New FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None)
                    PdfWriter.GetInstance(doc, fs)
                    doc.Open()

                    Dim colorAzulInstitucional As New BaseColor(0, 122, 204)
                    Dim fontTitulo As Font = FontFactory.GetFont("Segoe UI", 18, iTextSharp.text.Font.BOLD, colorAzulInstitucional)
                    Dim fontCentro As Font = FontFactory.GetFont("Segoe UI", 11, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)
                    Dim fontSubtitulo As Font = FontFactory.GetFont("Segoe UI", 9, iTextSharp.text.Font.ITALIC, BaseColor.GRAY)
                    Dim fontTexto As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                    Dim fontDestacado As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)
                    Dim fontCabecera As Font = FontFactory.GetFont("Segoe UI", 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE)
                    Dim fontFirmas As Font = FontFactory.GetFont("Segoe UI", 9, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)

                    Dim tablaCabecera As New PdfPTable(2)
                    tablaCabecera.WidthPercentage = 100
                    tablaCabecera.SetWidths({70.0F, 30.0F})

                    Dim celdaTextos As New PdfPCell()
                    celdaTextos.Border = iTextSharp.text.Rectangle.NO_BORDER

                    Dim pTitulo As New Paragraph("BOLETÍN DE CALIFICACIONES GENERALES", fontTitulo)
                    pTitulo.SpacingAfter = 2
                    celdaTextos.AddElement(pTitulo)

                    Dim nombreCentro As String = "Centro no asignado"
                    If datosBoletinActual IsNot Nothing AndAlso datosBoletinActual.ContainsKey("centro_educativo") Then
                        nombreCentro = datosBoletinActual("centro_educativo").ToString()
                    End If

                    Dim pCentro As New Paragraph("Centro Educativo: " & nombreCentro, fontCentro)
                    pCentro.SpacingAfter = 2
                    celdaTextos.AddElement(pCentro)

                    Dim pSub As New Paragraph("EVALIS - Reporte Informativo Académico", fontSubtitulo)
                    celdaTextos.AddElement(pSub)
                    tablaCabecera.AddCell(celdaTextos)

                    Dim celdaLogo As New PdfPCell()
                    celdaLogo.Border = iTextSharp.text.Rectangle.NO_BORDER
                    celdaLogo.HorizontalAlignment = Element.ALIGN_RIGHT

                    If datosBoletinActual IsNot Nothing AndAlso datosBoletinActual.ContainsKey("logo_centro") AndAlso datosBoletinActual("logo_centro") IsNot Nothing AndAlso Not String.IsNullOrEmpty(datosBoletinActual("logo_centro").ToString()) Then
                        Try
                            Dim logoStr As String = datosBoletinActual("logo_centro").ToString()
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
                                celdaLogo.AddElement(imgLogo)
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                    tablaCabecera.AddCell(celdaLogo)

                    tablaCabecera.SpacingAfter = 20
                    doc.Add(tablaCabecera)

                    Dim infoAlumno As New Paragraph()
                    infoAlumno.Alignment = Element.ALIGN_LEFT
                    infoAlumno.SpacingAfter = 20
                    infoAlumno.Add(New Chunk("Documento de Identidad (DNI): ", fontDestacado))
                    infoAlumno.Add(New Chunk(My.Settings.DniPersona & vbCrLf, fontTexto))
                    infoAlumno.Add(New Chunk("Año Académico / Curso: ", fontDestacado))
                    infoAlumno.Add(New Chunk(CursoReporte & vbCrLf, fontTexto))
                    infoAlumno.Add(New Chunk("Fecha de Emisión: ", fontDestacado))
                    infoAlumno.Add(New Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fontTexto))
                    doc.Add(infoAlumno)

                    Dim tablaPdf As New PdfPTable(dgvBoletin.Columns.Count)
                    tablaPdf.WidthPercentage = 100
                    tablaPdf.SetWidths({35.0F, 35.0F, 12.0F, 18.0F})

                    For Each col As DataGridViewColumn In dgvBoletin.Columns
                        Dim cellHeader As New PdfPCell(New Phrase(col.HeaderText, fontCabecera))
                        cellHeader.BackgroundColor = colorAzulInstitucional
                        cellHeader.HorizontalAlignment = Element.ALIGN_CENTER
                        cellHeader.VerticalAlignment = Element.ALIGN_MIDDLE
                        cellHeader.Padding = 8
                        tablaPdf.AddCell(cellHeader)
                    Next

                    For Each row As DataGridViewRow In dgvBoletin.Rows
                        If row.IsNewRow Then Continue For

                        For Each cell As DataGridViewCell In row.Cells
                            Dim valor As String = If(cell.Value IsNot Nothing, cell.Value.ToString(), "-")
                            Dim cellPdf As New PdfPCell(New Phrase(valor, fontTexto))
                            cellPdf.Padding = 6
                            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE

                            If cell.ColumnIndex = 0 Or cell.ColumnIndex = 1 Then
                                cellPdf.HorizontalAlignment = Element.ALIGN_LEFT
                            Else
                                cellPdf.HorizontalAlignment = Element.ALIGN_CENTER
                            End If

                            tablaPdf.AddCell(cellPdf)
                        Next
                    Next
                    doc.Add(tablaPdf)

                    Dim tablaFirmas As New PdfPTable(3)
                    tablaFirmas.WidthPercentage = 100
                    tablaFirmas.SpacingBefore = 55
                    tablaFirmas.SetWidths({32.0F, 36.0F, 32.0F})

                    Dim cellProf As New PdfPCell(New Paragraph("Firma del Tutor / Profesor" & Environment.NewLine & "_______________________", fontFirmas))
                    cellProf.Border = iTextSharp.text.Rectangle.NO_BORDER
                    cellProf.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaFirmas.AddCell(cellProf)

                    Dim cellJefe As New PdfPCell(New Paragraph("Firma del Jefe de Estudios" & Environment.NewLine & "_______________________", fontFirmas))
                    cellJefe.Border = iTextSharp.text.Rectangle.NO_BORDER
                    cellJefe.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaFirmas.AddCell(cellJefe)

                    Dim cellCent As New PdfPCell(New Paragraph("Sello del Centro Educativo" & Environment.NewLine & "_______________________", fontFirmas))
                    cellCent.Border = iTextSharp.text.Rectangle.NO_BORDER
                    cellCent.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaFirmas.AddCell(cellCent)

                    doc.Add(tablaFirmas)

                    Dim pPie As New Paragraph(Environment.NewLine & "Este boletín de calificaciones tiene carácter estrictamente informativo para el alumno y tutores legales, careciendo de validez como certificación académica oficial.", fontSubtitulo)
                    pPie.Alignment = Element.ALIGN_CENTER
                    pPie.SpacingBefore = 20
                    doc.Add(pPie)

                    doc.Close()
                    MessageBox.Show("El boletín de notas se ha exportado correctamente a PDF.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            Catch ex As Exception
                If doc.IsOpen Then doc.Close()
                MessageBox.Show("Error al generar el reporte PDF: " & ex.Message, "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersHeight = 40
            .EnableHeadersVisualStyles = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 253)
        End With
    End Sub
End Class