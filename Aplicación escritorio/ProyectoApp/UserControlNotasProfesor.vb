Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UserControlNotasProfesor

    Private Const BaseUrl As String = "http://localhost/"
    Private CursoReporte As String = "2025-2026"
    Private datosBoletinActual As Dictionary(Of String, Object)

    Private Class AlumnoItem
        Public Property DisplayName As String
        Public Property NiaValue As String
    End Class

    Private Sub UserControlNotasProfesor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarDataGridViewBoletin()
        EstilizarDataGridView(dgvBoletinAlumno)
        CargarGruposDelProfesor()
    End Sub

    Private Sub ConfigurarDataGridViewBoletin()
        If dgvBoletinAlumno.Columns.Count > 0 Then Return

        dgvBoletinAlumno.Columns.Clear()
        dgvBoletinAlumno.Columns.Add("modul", "Módulo")
        dgvBoletinAlumno.Columns.Add("unitat", "Unidad")
        dgvBoletinAlumno.Columns.Add("nota", "Nota")
        dgvBoletinAlumno.Columns.Add("data_nota", "Fecha de Evaluación")
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
                    cmbGrupo.Items.Add(grupoDict("nom").ToString() & " - " & grupoDict("aula").ToString())
                Next
            End If
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
        dgvBoletinAlumno.Rows.Clear()
        datosBoletinActual = Nothing

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
                    datasourceAlumnos.Add(New AlumnoItem With {
                        .NiaValue = alumnoDict("nia").ToString(),
                        .DisplayName = alumnoDict("cognom").ToString() & ", " & alumnoDict("nom").ToString()
                    })
                Next

                lstAlumnos.DisplayMember = "DisplayName"
                lstAlumnos.ValueMember = "NiaValue"
                lstAlumnos.DataSource = datasourceAlumnos
                lstAlumnos.SelectedIndex = -1
            End If
        End If
    End Sub

    Private Sub lstAlumnos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAlumnos.SelectedIndexChanged
        If lstAlumnos.SelectedIndex = -1 OrElse lstAlumnos.SelectedValue Is Nothing Then Return
        CargarBoletinAlumno(lstAlumnos.SelectedValue.ToString())
    End Sub

    Private Sub CargarBoletinAlumno(nia As String)
        dgvBoletinAlumno.Rows.Clear()
        datosBoletinActual = Nothing

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&nia={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(nia)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_boletin_alumno_por_profesor.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            datosBoletinActual = jsonRespuesta

            If jsonRespuesta.ContainsKey("curso") Then CursoReporte = jsonRespuesta("curso").ToString()

            If jsonRespuesta.ContainsKey("notas") Then
                Dim listaNotas As Object = jsonRespuesta("notas")
                For Each n As Object In CType(listaNotas, IEnumerable)
                    Dim notaDict As Dictionary(Of String, Object) = CType(n, Dictionary(Of String, Object))
                    dgvBoletinAlumno.Rows.Add(notaDict("modul").ToString(), notaDict("unitat").ToString(), notaDict("nota").ToString(), notaDict("data_nota").ToString())
                Next
            End If
        End If
    End Sub

    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs) Handles btnExportarPDF.Click
        If dgvBoletinAlumno.Rows.Count = 0 OrElse lstAlumnos.SelectedIndex = -1 Then
            MessageBox.Show("Seleccione un alumno con calificaciones para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim nombreAlumno As String = lstAlumnos.Text

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Documento PDF (*.pdf)|*.pdf"
        sfd.FileName = "Boletin_" & nombreAlumno.Replace(" ", "_").Replace(",", "") & ".pdf"

        If sfd.ShowDialog() = DialogResult.OK Then
            Dim doc As New Document(PageSize.A4, 36, 36, 36, 36)
            Try
                Using fs As New FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None)
                    PdfWriter.GetInstance(doc, fs)
                    doc.Open()

                    Dim colorCabeceraGris As New BaseColor(30, 30, 35)
                    Dim fontTitulo As Font = FontFactory.GetFont("Segoe UI", 18, iTextSharp.text.Font.BOLD, colorCabeceraGris)
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

                    Dim pTitulo As New Paragraph("BOLETÍN DE CALIFICACIONES DE ESTUDIANTE", fontTitulo)
                    pTitulo.SpacingAfter = 2
                    celdaTextos.AddElement(pTitulo)

                    Dim nombreCentro As String = "Centro no asignado"
                    If datosBoletinActual IsNot Nothing AndAlso datosBoletinActual.ContainsKey("centro_educativo") Then
                        nombreCentro = datosBoletinActual("centro_educativo").ToString()
                    End If

                    Dim pCentro As New Paragraph("Centro Educativo: " & nombreCentro, fontCentro)
                    pCentro.SpacingAfter = 2
                    celdaTextos.AddElement(pCentro)

                    Dim pSub As New Paragraph("EVALIS - Reporte de Rendimiento Académico", fontSubtitulo)
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

                    Dim info As New Paragraph()
                    info.SpacingAfter = 20
                    info.Add(New Chunk("Estudiante: ", fontDestacado))
                    info.Add(New Chunk(nombreAlumno & vbCrLf, fontTexto))
                    info.Add(New Chunk("NIA: ", fontDestacado))
                    info.Add(New Chunk(lstAlumnos.SelectedValue.ToString() & vbCrLf, fontTexto))
                    info.Add(New Chunk("Curso Académico: ", fontDestacado))
                    info.Add(New Chunk(CursoReporte & vbCrLf, fontTexto))
                    info.Add(New Chunk("Fecha de Emisión: ", fontDestacado))
                    info.Add(New Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fontTexto))
                    doc.Add(info)

                    Dim tablaPdf As New PdfPTable(dgvBoletinAlumno.Columns.Count)
                    tablaPdf.WidthPercentage = 100
                    tablaPdf.SetWidths({35.0F, 35.0F, 12.0F, 18.0F})

                    For Each col As DataGridViewColumn In dgvBoletinAlumno.Columns
                        Dim cellHeader As New PdfPCell(New Phrase(col.HeaderText, fontCabecera))
                        cellHeader.BackgroundColor = colorCabeceraGris
                        cellHeader.HorizontalAlignment = Element.ALIGN_CENTER
                        cellHeader.Padding = 7
                        tablaPdf.AddCell(cellHeader)
                    Next

                    For Each row As DataGridViewRow In dgvBoletinAlumno.Rows
                        If row.IsNewRow Then Continue For
                        For Each cell As DataGridViewCell In row.Cells
                            Dim cellPdf As New PdfPCell(New Phrase(If(cell.Value IsNot Nothing, cell.Value.ToString(), "-"), fontTexto))
                            cellPdf.Padding = 6
                            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE
                            cellPdf.HorizontalAlignment = If(cell.ColumnIndex < 2, Element.ALIGN_LEFT, Element.ALIGN_CENTER)
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

                    Dim pPie As New Paragraph(Environment.NewLine & "Este documento se expide a efectos puramente informativos y de seguimiento académico, careciendo de validez como certificación oficial arancelaria.", fontSubtitulo)
                    pPie.Alignment = Element.ALIGN_CENTER
                    pPie.SpacingBefore = 20
                    doc.Add(pPie)

                    doc.Close()
                    MessageBox.Show("Boletín exportado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            Catch ex As Exception
                If doc.IsOpen Then doc.Close()
                MessageBox.Show("Error al exportar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
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
                    Dim resultado As Dictionary(Of String, Object) = (New JavaScriptSerializer()).Deserialize(Of Dictionary(Of String, Object))(reader.ReadToEnd())
                    If resultado IsNot Nothing AndAlso resultado.ContainsKey("new_token") Then
                        My.Settings.Token = resultado("new_token")?.ToString()
                        My.Settings.Save()
                    End If
                    Return resultado
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function GenerarHardwareHash() As String
        Dim info As String = Environment.MachineName & Environment.UserName & Environment.ProcessorCount.ToString()
        Using sha As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
            Dim bytes As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(info))
            Dim sb As New StringBuilder()
            For i As Integer = 0 To bytes.Length - 1 : sb.Append(bytes(i).ToString("x2")) : Next
            Return sb.ToString()
        End Using
    End Function

    Private Sub EstilizarDataGridView(dgv As DataGridView)
        With dgv
            .BackgroundColor = Color.White : .BorderStyle = BorderStyle.None : .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .RowHeadersVisible = False : .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill : .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AllowUserToAddRows = False : .AllowUserToDeleteRows = False : .DefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 35) : .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10, FontStyle.Bold) : .ColumnHeadersHeight = 40 : .EnableHeadersVisualStyles = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250)
        End With
    End Sub
End Class