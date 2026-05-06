Imports System.Drawing.Printing
Imports System.IO
Imports System.Text
Imports System.Xml
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Public Class UserControlExportacion

    Private Const ConnString As String = "Server=localhost;Database=plataforma_evalis;Uid=root;Pwd=;Convert Zero Datetime=True;"

    Private Sub UserControlExportacion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarGruposParaActa()


        btnExportarLogsXML.Visible = (My.Settings.Rol = "admin")
        btnExportarProfesoresenJSON.Visible = (My.Settings.Rol = "admin")
        btnGenerarActaPDF.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "admin")
        btnExportarAlumnoscsv.Visible = (My.Settings.Rol = "admin")
    End Sub



    Private Sub btnExportarAlumnoscsv_Click(sender As Object, e As EventArgs) Handles btnExportarAlumnoscsv.Click
        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                        $"Alumnos_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.csv")

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Dim sql As String = "
                    SELECT 
                        p.dni,
                        p.nom,
                        p.cognom,
                        p.data_naix,
                        p.esMajor,
                        p.poblacio,
                        p.codi_postal,
                        p.nacionalitat,
                        p.mun_naixement,
                        p.tel_mobil,
                        p.tel_fix,
                        p.email,
                        p.rol,
                        p.edat,
                        p.foto,
                        e.nia,
                        eg.nom_grup AS grupo
                    FROM persona p
                    INNER JOIN estudiants e ON p.dni = e.dni_persona
                    LEFT JOIN estudiants_grupclasse eg ON e.nia = eg.nia
                    ORDER BY p.cognom, p.nom"

                Using cmd As New MySqlCommand(sql, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        Dim sb As New StringBuilder()

                        For i = 0 To reader.FieldCount - 1
                            sb.Append(reader.GetName(i)).Append(";")
                        Next
                        sb.AppendLine()

                        While reader.Read()
                            For i = 0 To reader.FieldCount - 1
                                Dim valor = If(reader(i) Is DBNull.Value, "", reader(i).ToString().Replace(";", ","))
                                sb.Append(valor).Append(";")
                            Next
                            sb.AppendLine()
                        End While

                        File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8)
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error al exportar alumnos: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        MessageBox.Show("Archivo CSV de alumnos exportado correctamente en el Escritorio:" & vbCrLf & ruta, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnExportarProfesoresJSON_Click(sender As Object, e As EventArgs) Handles btnExportarProfesoresenJSON.Click
        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                        $"Profesores_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.json")

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Dim sql As String = "
                    SELECT 
                        p.dni,
                        p.nom,
                        p.cognom,
                        p.data_naix,
                        p.esMajor,
                        p.poblacio,
                        p.codi_postal,
                        p.nacionalitat,
                        p.mun_naixement,
                        p.tel_mobil,
                        p.tel_fix,
                        p.email,
                        p.rol,
                        p.edat,
                        p.foto,
                        pr.especialitzacio,
                        pr.id_intern
                    FROM persona p
                    INNER JOIN professor pr ON p.dni = pr.dni_persona
                    ORDER BY p.cognom, p.nom"

                Using cmd As New MySqlCommand(sql, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        Dim lista As New List(Of Object)

                        While reader.Read()
                            lista.Add(New With {
                                .dni = reader("dni").ToString(),
                                .nombre = reader("nom").ToString(),
                                .apellidos = reader("cognom").ToString(),
                                .fechaNacimiento = reader("data_naix").ToString(),
                                .email = reader("email").ToString(),
                                .telefonoMovil = reader("tel_mobil").ToString(),
                                .telefonoFijo = reader("tel_fix").ToString(),
                                .especializacion = reader("especialitzacio").ToString(),
                                .foto = If(reader("foto") Is DBNull.Value, Nothing, reader("foto").ToString())
                            })
                        End While

                        Dim json As String = JsonConvert.SerializeObject(lista, Newtonsoft.Json.Formatting.Indented)
                        File.WriteAllText(ruta, json, Encoding.UTF8)
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error al exportar profesores: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
        End Using

        MessageBox.Show("Archivo JSON de profesores exportado correctamente en el Escritorio:" & vbCrLf & ruta, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnExportarLogsXML_Click(sender As Object, e As EventArgs) Handles btnExportarLogsXML.Click
        Dim ruta As String = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "Logs_Login_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".xml"
        )

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()

                Dim sql As String = "
                    SELECT login_timestamp, username, ip_direccio, login_success 
                    FROM login_logs 
                    ORDER BY login_timestamp DESC"

                Using cmd As New MySqlCommand(sql, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()

                        Dim settings As New XmlWriterSettings() With {
                            .Indent = True,
                            .Encoding = Encoding.UTF8
                        }

                        Using writer As XmlWriter = XmlWriter.Create(ruta, settings)
                            writer.WriteStartDocument()
                            writer.WriteStartElement("RegistrosLogin")

                            While reader.Read()
                                writer.WriteStartElement("Login")

                                Dim fechaStr As String = reader("login_timestamp").ToString()
                                writer.WriteElementString("Fecha", fechaStr)

                                writer.WriteElementString("Usuario", reader("username").ToString())
                                writer.WriteElementString("IP", reader("ip_direccio").ToString())
                                writer.WriteElementString("Exito", If(Convert.ToInt32(reader("login_success")) = 1, "Sí", "No"))

                                writer.WriteEndElement()
                            End While

                            writer.WriteEndElement()
                            writer.WriteEndDocument()
                        End Using
                    End Using
                End Using

                MessageBox.Show("Archivo XML exportado correctamente en el Escritorio:" & vbCrLf & ruta,
                               "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Error al exportar logs: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub


    Private Sub CargarGruposParaActa()
        cmbGrupoActa.Items.Clear()
        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Using cmd As New MySqlCommand("SELECT nom, aula FROM grup_classe ORDER BY nom", conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            cmbGrupoActa.Items.Add(reader("nom").ToString() & " - " & reader("aula").ToString())
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error al cargar grupos: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnGenerarActaPDF_Click(sender As Object, e As EventArgs) Handles btnGenerarActaPDF.Click
        If cmbGrupoActa.SelectedIndex = -1 Then
            MessageBox.Show("Selecciona un grupo de clase", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim nomGrupo As String = cmbGrupoActa.Text.Split("-"c)(0).Trim()

        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                        $"Acta_Evaluacion_{nomGrupo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf")

        Try
            GenerarActaPDF(nomGrupo, ruta)
            MessageBox.Show("Acta generada correctamente en el Escritorio:" & vbCrLf & ruta,
                           "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error al generar el PDF: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub GenerarActaPDF(nomGrupo As String, rutaPDF As String)
        Dim doc As New Document(PageSize.A4.Rotate(), 30, 30, 30, 30)

        Try
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(rutaPDF, FileMode.Create))
            doc.Open()

            Dim fontTitulo As Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18)
            Dim fontSubtitulo As Font = FontFactory.GetFont(FontFactory.HELVETICA, 12)
            Dim fontHeader As Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)
            Dim fontRows As Font = FontFactory.GetFont(FontFactory.HELVETICA, 9)

            Dim titulo As New Paragraph("Acta de evaluación", fontTitulo)
            titulo.Alignment = Element.ALIGN_CENTER
            doc.Add(titulo)

            Dim subtitulo As New Paragraph("Grupo: " & nomGrupo & "   |   Curso Académico: 2025-2026", fontSubtitulo)
            subtitulo.Alignment = Element.ALIGN_CENTER
            doc.Add(subtitulo)
            doc.Add(New Paragraph(" "))


            Dim tabla As New PdfPTable(12) With {.WidthPercentage = 100}

            Dim anchos() As Single = {25, 200, 30, 30, 30, 30, 30, 30, 30, 30, 30, 60}
            tabla.SetWidths(anchos)

            Dim cellN As New PdfPCell(New Phrase("Nº", fontHeader)) With {.BackgroundColor = BaseColor.LIGHT_GRAY, .HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 5}
            tabla.AddCell(cellN)

            Dim cellNom As New PdfPCell(New Phrase("Apellidos y Nombres", fontHeader)) With {.BackgroundColor = BaseColor.LIGHT_GRAY, .HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 5}
            tabla.AddCell(cellNom)

            For i As Integer = 1 To 9
                Dim cellNota As New PdfPCell(New Phrase(i.ToString(), fontHeader)) With {.BackgroundColor = BaseColor.LIGHT_GRAY, .HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 5}
                tabla.AddCell(cellNota)
            Next

            Dim cellProm As New PdfPCell(New Phrase("Promoción", fontHeader)) With {.BackgroundColor = BaseColor.LIGHT_GRAY, .HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 5}
            tabla.AddCell(cellProm)

            Using conn As New MySqlConnection(ConnString)
                conn.Open()
                Dim sql As String = "SELECT " &
                                "ROW_NUMBER() OVER (ORDER BY p.cognom, p.nom) AS numero, " &
                                "CONCAT(p.cognom, ', ', p.nom) AS nombre_completo " &
                                "FROM estudiants_grupclasse eg " &
                                "INNER JOIN estudiants e ON eg.nia = e.nia " &
                                "INNER JOIN persona p ON e.dni_persona = p.dni " &
                                "WHERE eg.nom_grup = @nomGrupo " &
                                "ORDER BY p.cognom, p.nom"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@nomGrupo", nomGrupo)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        Dim rnd As New Random()

                        While reader.Read()
                            tabla.AddCell(New PdfPCell(New Phrase(reader("numero").ToString(), fontRows)) With {.HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 4})

                            tabla.AddCell(New PdfPCell(New Phrase(reader("nombre_completo").ToString(), fontRows)) With {.Padding = 4})

                            For i As Integer = 1 To 9
                                Dim notaHardcoded As String = rnd.Next(5, 11).ToString()
                                tabla.AddCell(New PdfPCell(New Phrase(notaHardcoded, fontRows)) With {.HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 4})
                            Next

                            tabla.AddCell(New PdfPCell(New Phrase("SÍ", fontRows)) With {.HorizontalAlignment = Element.ALIGN_CENTER, .Padding = 4})
                        End While
                    End Using
                End Using
            End Using

            doc.Add(tabla)


            doc.Add(New Paragraph(vbCrLf & "Firma del Jefe de Estudios: ___________________________" &
                          "Firma del Director: ___________________________", fontSubtitulo))

        Catch ex As Exception
            MsgBox("Error crítico al generar el acta: " & ex.Message, MsgBoxStyle.Critical)
        Finally
            If doc.IsOpen() Then doc.Close()
        End Try
    End Sub

End Class


