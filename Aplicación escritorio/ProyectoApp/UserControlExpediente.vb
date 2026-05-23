Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports System.Web.Script.Serialization
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UserControlExpediente

    Private dtEstudios As DataTable
    Private datosAlumno As Dictionary(Of String, Object)

    Private Sub UserControlExpediente_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarDiseñoControles()
        CargarDatosExpediente()
    End Sub

    Private Sub ConfigurarDiseñoControles()
        btnExportarPDF.BackColor = Color.FromArgb(37, 99, 235)
        btnExportarPDF.ForeColor = Color.White
        btnExportarPDF.FlatStyle = FlatStyle.Flat
        btnExportarPDF.FlatAppearance.BorderSize = 0
        btnExportarPDF.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        btnExportarPDF.Cursor = Cursors.Hand

        dgvExpediente.BackgroundColor = Color.White
        dgvExpediente.BorderStyle = BorderStyle.None
        dgvExpediente.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250)
        dgvExpediente.EnableHeadersVisualStyles = False
        dgvExpediente.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 99, 235)
        dgvExpediente.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgvExpediente.ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        dgvExpediente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub

    Private Async Sub CargarDatosExpediente()
        Dim baseUrl As String = "http://localhost/get_expediente_desktop.php"
        Dim token As String = My.Settings.Token
        Dim userAgentHash As String = GenerarHardwareHash()

        Dim handler As New HttpClientHandler()
        handler.ServerCertificateCustomValidationCallback = Function(req, cert, chain, errors) True

        Using client As New HttpClient(handler)
            Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"token", token},
                {"user_agent_hash", userAgentHash}
            })

            Try
                Dim response As HttpResponseMessage = Await client.PostAsync(baseUrl, content)
                Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Dim serializer As New JavaScriptSerializer()
                    Dim result As Dictionary(Of String, Object) = serializer.Deserialize(Of Dictionary(Of String, Object))(jsonResponse)

                    If result IsNot Nothing AndAlso result.ContainsKey("status") AndAlso result("status").ToString() = "success" Then
                        If result.ContainsKey("new_token") Then
                            My.Settings.Token = result("new_token").ToString()
                            My.Settings.Save()
                        End If

                        datosAlumno = result

                        lblNombre.Text = "Alumno: " & result("nombre_alumno").ToString()
                        lblDni.Text = "DNI: " & result("dni_alumno").ToString()
                        lblEmail.Text = "Email: " & result("email_alumno").ToString()

                        If result.ContainsKey("foto") AndAlso result("foto") IsNot Nothing AndAlso Not String.IsNullOrEmpty(result("foto").ToString()) Then
                            Try
                                Dim fotoStr As String = result("foto").ToString()
                                If fotoStr.StartsWith("data:image") OrElse fotoStr.Length > 200 Then
                                    Dim base64Data As String = fotoStr.Substring(fotoStr.IndexOf(",") + 1)
                                    Dim imageBytes As Byte() = Convert.FromBase64String(base64Data)
                                    Using ms As New MemoryStream(imageBytes)
                                        picFoto.Image = System.Drawing.Image.FromStream(ms)
                                    End Using
                                Else
                                    picFoto.ImageLocation = fotoStr
                                End If
                            Catch ex As Exception
                                picFoto.Image = Nothing
                            End Try
                        End If

                        Dim listaEstudios As System.Collections.ArrayList = CType(result("estudis"), System.Collections.ArrayList)
                        dtEstudios = New DataTable()
                        dtEstudios.Columns.Add("Estudio")
                        dtEstudios.Columns.Add("Año Inicio")
                        dtEstudios.Columns.Add("Año Fin")
                        dtEstudios.Columns.Add("Estado")
                        dtEstudios.Columns.Add("Nota Final")

                        For Each item As Dictionary(Of String, Object) In listaEstudios
                            dtEstudios.Rows.Add(item("nom_estudi"), item("curs_inici"), item("curs_fi"), item("status"), item("nota_final"))
                        Next

                        dgvExpediente.DataSource = dtEstudios
                    Else
                        Dim motivo As String = If(result.ContainsKey("motivo"), result("motivo").ToString(), "Error desconocido.")
                        MessageBox.Show(motivo, "Error de Expediente", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show("Error al conectar con el servidor: " & ex.Message, "Excepción", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub btnExportarPDF_Click(sender As Object, e As EventArgs) Handles btnExportarPDF.Click
        If dgvExpediente.Rows.Count = 0 Then
            MessageBox.Show("No hay datos disponibles para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Documento PDF (*.pdf)|*.pdf"
        sfd.FileName = "Expediente_" & My.Settings.Username & ".pdf"

        If sfd.ShowDialog() = DialogResult.OK Then
            Dim doc As New Document(PageSize.A4, 30, 30, 30, 30)

            Try
                Dim fileStream As New FileStream(sfd.FileName, FileMode.Create)
                PdfWriter.GetInstance(doc, fileStream)
                doc.Open()

                Dim colorPrimario As New BaseColor(37, 99, 235)
                Dim colorTexto As New BaseColor(51, 65, 85)
                Dim colorGrisClaro As New BaseColor(241, 245, 249)

                Dim fTitulo As Font = FontFactory.GetFont("Helvetica", 18, Font.Bold, colorPrimario)
                Dim fCentro As Font = FontFactory.GetFont("Helvetica", 11, Font.Bold, colorTexto)
                Dim fSubtitulo As Font = FontFactory.GetFont("Helvetica", 10, Font.Italic, BaseColor.GRAY)
                Dim fSeccion As Font = FontFactory.GetFont("Helvetica", 12, Font.Bold, colorPrimario)
                Dim fContenido As Font = FontFactory.GetFont("Helvetica", 10, Font.Bold, colorTexto)
                Dim fCabeceraTabla As Font = FontFactory.GetFont("Helvetica", 10, Font.Bold, BaseColor.WHITE)
                Dim fFirmas As Font = FontFactory.GetFont("Helvetica", 9, Font.Bold, colorTexto)


                Dim tablaCabecera As New PdfPTable(2)
                tablaCabecera.WidthPercentage = 100
                tablaCabecera.SetWidths({70, 30})

                Dim cellTextosCab As New PdfPCell()
                cellTextosCab.Border = PdfPCell.NO_BORDER

                Dim pTitulo As New Paragraph("EVALIS - PLATAFORMA EDUCATIVA", fTitulo)
                pTitulo.SpacingAfter = 2
                cellTextosCab.AddElement(pTitulo)

                Dim nombreCentro As String = "Centro no asignado"
                If datosAlumno IsNot Nothing AndAlso datosAlumno.ContainsKey("centro_educativo") Then
                    nombreCentro = datosAlumno("centro_educativo").ToString()
                End If

                Dim pCentro As New Paragraph("Centro Educativo: " & nombreCentro, fCentro)
                pCentro.SpacingAfter = 2
                cellTextosCab.AddElement(pCentro)

                Dim pSub As New Paragraph("Expediente Académico Oficial de Estudios", fSubtitulo)
                cellTextosCab.AddElement(pSub)
                tablaCabecera.AddCell(cellTextosCab)

                Dim cellLogoCab As New PdfPCell()
                cellLogoCab.Border = PdfPCell.NO_BORDER
                cellLogoCab.HorizontalAlignment = Element.ALIGN_RIGHT

                If datosAlumno IsNot Nothing AndAlso datosAlumno.ContainsKey("logo_centro") AndAlso datosAlumno("logo_centro") IsNot Nothing AndAlso Not String.IsNullOrEmpty(datosAlumno("logo_centro").ToString()) Then
                    Try
                        Dim logoStr As String = datosAlumno("logo_centro").ToString()
                        Dim imgLogo As iTextSharp.text.Image = Nothing

                        If logoStr.StartsWith("data:image") OrElse logoStr.Length > 200 Then
                            Dim base64Data As String = logoStr.Substring(logoStr.IndexOf(",") + 1)
                            Dim imageBytes As Byte() = Convert.FromBase64String(base64Data)
                            imgLogo = iTextSharp.text.Image.GetInstance(imageBytes)
                        Else
                            imgLogo = iTextSharp.text.Image.GetInstance(logoStr)
                        End If

                        If imgLogo IsNot Nothing Then
                            imgLogo.ScaleToFit(110, 50)
                            imgLogo.Alignment = Element.ALIGN_RIGHT
                            cellLogoCab.AddElement(imgLogo)
                        End If
                    Catch ex As Exception
                    End Try
                End If
                tablaCabecera.AddCell(cellLogoCab)

                tablaCabecera.SpacingAfter = 20
                doc.Add(tablaCabecera)

                Dim tablaInfo As New PdfPTable(2)
                tablaInfo.WidthPercentage = 100
                tablaInfo.SetWidths({75, 25})

                Dim cellDatos As New PdfPCell()
                cellDatos.Border = PdfPCell.NO_BORDER

                Dim pNombre As New Paragraph(lblNombre.Text, FontFactory.GetFont("Helvetica", 11, Font.Bold, colorTexto))
                pNombre.SpacingAfter = 5
                Dim pDni As New Paragraph(lblDni.Text, fContenido)
                pDni.SpacingAfter = 5
                Dim pEmail As New Paragraph(lblEmail.Text, fContenido)
                pEmail.SpacingAfter = 5
                Dim pFecha As New Paragraph("Fecha de generación: " & DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fContenido)

                cellDatos.AddElement(pNombre)
                cellDatos.AddElement(pDni)
                cellDatos.AddElement(pEmail)
                cellDatos.AddElement(pFecha)
                tablaInfo.AddCell(cellDatos)

                Dim cellFoto As New PdfPCell()
                cellFoto.Border = PdfPCell.NO_BORDER
                cellFoto.HorizontalAlignment = Element.ALIGN_RIGHT

                If picFoto.Image IsNot Nothing Then
                    Try
                        Using ms As New MemoryStream()
                            picFoto.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                            Dim imgPdf As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ms.ToArray())
                            imgPdf.ScaleToFit(90, 110)
                            imgPdf.Alignment = Element.ALIGN_RIGHT
                            cellFoto.AddElement(imgPdf)
                        End Using
                    Catch ex As Exception
                        cellFoto.AddElement(New Paragraph("[Sin Foto]", fSubtitulo))
                    End Try
                Else
                    cellFoto.AddElement(New Paragraph("[Sin Foto]", fSubtitulo))
                End If

                tablaInfo.AddCell(cellFoto)
                tablaInfo.SpacingAfter = 30
                doc.Add(tablaInfo)

                Dim pSec As New Paragraph("EXPEDIENTE DE ESTUDIOS", fSeccion)
                pSec.SpacingAfter = 10
                doc.Add(pSec)

                Dim tablaNotas As New PdfPTable(5)
                tablaNotas.WidthPercentage = 100
                tablaNotas.SetWidths({40, 15, 15, 15, 15})

                Dim cabeceras() As String = {"Estudio", "Año Inicio", "Año Fin", "Estado", "Nota Final"}
                For Each cab As String In cabeceras
                    Dim cellCab As New PdfPCell(New Phrase(cab, fCabeceraTabla))
                    cellCab.BackgroundColor = colorPrimario
                    cellCab.HorizontalAlignment = Element.ALIGN_CENTER
                    cellCab.Padding = 6
                    cellCab.BorderColor = BaseColor.WHITE
                    tablaNotas.AddCell(cellCab)
                Next

                For Each row As DataRow In dtEstudios.Rows
                    Dim c1 As New PdfPCell(New Phrase(row(0).ToString(), fContenido))
                    c1.Padding = 6
                    c1.HorizontalAlignment = Element.ALIGN_LEFT
                    tablaNotas.AddCell(c1)

                    Dim c2 As New PdfPCell(New Phrase(row(1).ToString(), fContenido))
                    c2.Padding = 6
                    c2.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaNotas.AddCell(c2)

                    Dim c3 As New PdfPCell(New Phrase(row(2).ToString(), fContenido))
                    c3.Padding = 6
                    c3.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaNotas.AddCell(c3)

                    Dim c4 As New PdfPCell(New Phrase(row(3).ToString(), fContenido))
                    c4.Padding = 6
                    c4.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaNotas.AddCell(c4)

                    Dim esAprobado As Boolean = IsNumeric(row(4)) AndAlso Convert.ToDouble(row(4)) >= 5.0
                    Dim fNota As Font = If(esAprobado, FontFactory.GetFont("Helvetica", 10, Font.Bold, colorTexto), fContenido)

                    Dim c5 As New PdfPCell(New Phrase(row(4).ToString(), fNota))
                    c5.Padding = 6
                    c5.HorizontalAlignment = Element.ALIGN_CENTER
                    tablaNotas.AddCell(c5)
                Next

                doc.Add(tablaNotas)

                Dim tablaFirmas As New PdfPTable(3)
                tablaFirmas.WidthPercentage = 100
                tablaFirmas.SpacingBefore = 60
                tablaFirmas.SetWidths({30, 35, 30})

                Dim cellProf As New PdfPCell(New Paragraph("Firma del Profesor" & Environment.NewLine & "_______________________", fFirmas))
                cellProf.Border = PdfPCell.NO_BORDER
                cellProf.HorizontalAlignment = Element.ALIGN_CENTER
                tablaFirmas.AddCell(cellProf)

                Dim cellJefe As New PdfPCell(New Paragraph("Firma del Jefe de Estudios" & Environment.NewLine & "_______________________", fFirmas))
                cellJefe.Border = PdfPCell.NO_BORDER
                cellJefe.HorizontalAlignment = Element.ALIGN_CENTER
                tablaFirmas.AddCell(cellJefe)

                Dim cellCent As New PdfPCell(New Paragraph("Sello / Firma del Centro" & Environment.NewLine & "_______________________", fFirmas))
                cellCent.Border = PdfPCell.NO_BORDER
                cellCent.HorizontalAlignment = Element.ALIGN_CENTER
                tablaFirmas.AddCell(cellCent)

                doc.Add(tablaFirmas)

                Dim pPie As New Paragraph(Environment.NewLine & "Este documento sirve como resguardo informativo del expediente académico del alumno en la fecha indicada y carece de validez legal de certificación arancelaria.", fSubtitulo)
                pPie.Alignment = Element.ALIGN_CENTER
                pPie.SpacingBefore = 30
                doc.Add(pPie)

                MessageBox.Show("El expediente en formato PDF ha sido generado y guardado con éxito.", "PDF Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Ocurrió un error al intentar estructurar el PDF: " & ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                doc.Close()
            End Try
        End If
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