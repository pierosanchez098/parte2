Imports System.Collections.Specialized
Imports System.Drawing.Printing
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Xml
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

Public Class UserControlExportacion

    Private Const ConnString As String = "Server=localhost;Database=plataforma_evalis;Uid=root;Pwd=;Convert Zero Datetime=True;"

    Private Sub UserControlExportacion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarGruposParaCSV()
        CargarCentrosParaJson()
        CargarGruposParaExcel()
        CargarGruposParaActa()


        btnExportarLogsXML.Visible = (My.Settings.Rol = "admin" OrElse My.Settings.Rol = "directiu")
        btnExportarProfesoresenJSON.Visible = (My.Settings.Rol = "admin")
        btnGenerarActaPDF.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "admin")
        btnExportarAlumnoscsv.Visible = (My.Settings.Rol = "admin")

        btnExportarAlumnoscsv.Visible = (My.Settings.Rol = "admin")
        cmbCSVGrupo.Visible = (My.Settings.Rol = "admin")
        lblGrupoClaseCSV.Visible = (My.Settings.Rol = "admin")

        cmbCentroJson.Visible = (My.Settings.Rol = "admin")
        lblCentros.Visible = (My.Settings.Rol = "admin")

        lblGrupoClaseExcel.Visible = (My.Settings.Rol = "admin")
        cmbGrupoExcel.Visible = (My.Settings.Rol = "admin")
        btnExportarAlumnosExcel.Visible = (My.Settings.Rol = "admin")

        lblGrupoClaseActa.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "admin")
        cmbActa.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "admin")



    End Sub



    Private Sub btnExportarAlumnoscsv_Click(sender As Object, e As EventArgs) Handles btnExportarAlumnoscsv.Click
        If cmbCSVGrupo.SelectedIndex = -1 Then
            MessageBox.Show("Por favor, seleccione primero un grupo de clase del ComboBox.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim nomGrupo As String = cmbCSVGrupo.Text.Split("-"c)(0).Trim()

        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                         $"Alumnos_Grupo_{nomGrupo}_{DateTime.Now:yyyyMMdd_HHmmss}.csv")

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&nom_grup={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(nomGrupo)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("exportar_alumnos_csv.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim cuerpoCSV As String = jsonRespuesta("csv_data").ToString()

                Dim contenidoBytes As Byte() = Encoding.UTF8.GetBytes(cuerpoCSV)
                Dim bomBytes As Byte() = Encoding.UTF8.GetPreamble()

                Using fs As New FileStream(ruta, FileMode.Create, FileAccess.Write)
                    fs.Write(bomBytes, 0, bomBytes.Length)
                    fs.Write(contenidoBytes, 0, contenidoBytes.Length)
                End Using

                MessageBox.Show("Archivo CSV generado con éxito" & vbCrLf &
                                "Ubicación: " & ruta, "Exportación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Error al escribir el archivo físico en el escritorio: " & ex.Message, "Error de Archivo", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("No se pudo exportar el grupo: " & jsonRespuesta("motivo").ToString(), "Error del Servidor", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnExportarProfesoresJSON_Click(sender As Object, e As EventArgs) Handles btnExportarProfesoresenJSON.Click
        If cmbCentroJson.SelectedIndex = -1 Then
            MessageBox.Show("Por favor, seleccione primero un centro educativo del ComboBox.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim centroSeleccionado As ComboboxItem = CType(cmbCentroJson.SelectedItem, ComboboxItem)
        Dim idCentro As String = centroSeleccionado.Value
        Dim nomCentro As String = centroSeleccionado.Text.Replace(" ", "_")

        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                         $"Profesores_{nomCentro}_{DateTime.Now:yyyyMMdd_HHmmss}.json")

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&id_centro={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(idCentro)
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("exportar_profesores_centro_json.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim listaProfesores As Object = jsonRespuesta("lista")

                Dim jsonIndentado As String = JsonConvert.SerializeObject(listaProfesores, Newtonsoft.Json.Formatting.Indented)

                File.WriteAllText(ruta, jsonIndentado, Encoding.UTF8)

                MessageBox.Show($"Listado del centro: '{centroSeleccionado.Text}' exportado correctamente" & vbCrLf & vbCrLf &
                                "Ubicación: " & ruta, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Error al estructurar o guardar el archivo JSON: " & ex.Message, "Error de Archivo", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("El servidor rechazó la exportación: " & jsonRespuesta("motivo").ToString(), "Error de Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub


    Private Sub btnExportarLogsXML_Click(sender As Object, e As EventArgs) Handles btnExportarLogsXML.Click
        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                         $"Logs_Login_{DateTime.Now:yyyyMMdd_HHmmss}.xml")

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("exportar_logs_xml.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim contenidoXML As String = jsonRespuesta("xml_data").ToString()

                File.WriteAllText(ruta, contenidoXML, Encoding.UTF8)

                MessageBox.Show("Historial de logins exportado con éxito" & vbCrLf &
                                "Ubicación: " & ruta, "Exportación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Error al escribir el archivo XML en el almacenamiento local: " & ex.Message, "Error de Archivo", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("El servidor denegó la exportación de logs: " & jsonRespuesta("motivo").ToString(), "Error de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub



    Private Sub btnExportarAlumnosExcel_Click(sender As Object, e As EventArgs) Handles btnExportarAlumnosExcel.Click
        If cmbGrupoExcel.SelectedIndex = -1 Then
            MessageBox.Show("Por favor, seleccione un grupo de clase del ComboBox de Excel.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim nomGrupo As String = cmbGrupoExcel.Text.Split("-"c)(0).Trim()

        Dim ruta As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                         $"Alumnos_Grupo_{nomGrupo}_{DateTime.Now:yyyyMMdd_HHmmss}.xls")

        Try
            Dim parametros As New NameValueCollection()
            parametros.Add("token", My.Settings.Token)
            parametros.Add("user_agent_hash", GenerarHardwareHash())
            parametros.Add("nom_grup", nomGrupo)

            Dim urlApi As String = "http://localhost/exportar_alumnos_excel.php"

            Using client As New WebClient()
                client.Encoding = Encoding.UTF8

                Dim respuestaBytes As Byte() = client.UploadValues(urlApi, "POST", parametros)

                File.WriteAllBytes(ruta, respuestaBytes)
            End Using

            MessageBox.Show($"Listado de'{nomGrupo}' se ha exportado correctamente" & vbCrLf & vbCrLf &
                            "Ubicación: " & ruta, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Error al descargar el archivo Excel desde el servidor web: " & ex.Message, "Error de Red", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub CargarGruposParaCSV()
        cmbCSVGrupo.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_todos_grupos.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim jss As New JavaScriptSerializer()
                Dim gruposRaw As String = jss.Serialize(jsonRespuesta("grupos"))
                Dim listaGrupos As List(Of Dictionary(Of String, String)) = jss.Deserialize(Of List(Of Dictionary(Of String, String)))(gruposRaw)

                For Each grupo As Dictionary(Of String, String) In listaGrupos
                    Dim item As String = grupo("nom") & " - " & grupo("aula")
                    cmbCSVGrupo.Items.Add(item)
                Next

            Catch ex As Exception
                MessageBox.Show("Error al estructurar el listado global de grupos: " & ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("No se pudieron cargar los grupos institucionales: " & jsonRespuesta("motivo").ToString(), "Error de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub CargarGruposParaActa()
        cmbActa.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_todos_grupos.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim jss As New JavaScriptSerializer()
                Dim gruposRaw As String = jss.Serialize(jsonRespuesta("grupos"))
                Dim listaGrupos As List(Of Dictionary(Of String, String)) = jss.Deserialize(Of List(Of Dictionary(Of String, String)))(gruposRaw)

                For Each grupo As Dictionary(Of String, String) In listaGrupos
                    Dim item As String = grupo("nom") & " - " & grupo("aula")
                    cmbActa.Items.Add(item)
                Next

            Catch ex As Exception
                MessageBox.Show("Error al estructurar el listado global de grupos: " & ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("No se pudieron cargar los grupos institucionales: " & jsonRespuesta("motivo").ToString(), "Error de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub CargarGruposParaExcel()
        cmbGrupoExcel.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_todos_grupos.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim jss As New JavaScriptSerializer()
                Dim gruposRaw As String = jss.Serialize(jsonRespuesta("grupos"))
                Dim listaGrupos As List(Of Dictionary(Of String, String)) = jss.Deserialize(Of List(Of Dictionary(Of String, String)))(gruposRaw)

                For Each grupo As Dictionary(Of String, String) In listaGrupos
                    Dim item As String = grupo("nom") & " - " & grupo("aula")
                    cmbGrupoExcel.Items.Add(item)
                Next

            Catch ex As Exception
                MessageBox.Show("Error al estructurar el listado global de grupos: " & ex.Message, "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf jsonRespuesta IsNot Nothing Then
            MessageBox.Show("No se pudieron cargar los grupos institucionales: " & jsonRespuesta("motivo").ToString(), "Error de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub




    Private Sub CargarCentrosParaJson()
        cmbCentroJson.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim jsonRespuesta As Dictionary(Of String, Object) = EnviarPeticionWeb("get_centros_combobox.php", postData)

        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
            Try
                Dim jss As New JavaScriptSerializer()
                Dim centrosRaw As String = jss.Serialize(jsonRespuesta("centros"))
                Dim listaCentros As List(Of Dictionary(Of String, Object)) = jss.Deserialize(Of List(Of Dictionary(Of String, Object)))(centrosRaw)

                For Each centro As Dictionary(Of String, Object) In listaCentros
                    Dim item As New ComboboxItem()
                    item.Text = centro("nom").ToString()
                    item.Value = centro("id").ToString()
                    cmbCentroJson.Items.Add(item)
                Next

            Catch ex As Exception
                MessageBox.Show("Error al procesar el listado de centros: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnGenerarActaPDF_Click(sender As Object, e As EventArgs) Handles btnGenerarActaPDF.Click
        If cmbCSVGrupo.SelectedIndex = -1 Then
            MessageBox.Show("Selecciona un grupo de clase", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim nomGrupo As String = cmbCSVGrupo.Text.Split("-"c)(0).Trim()

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

    Private Function EnviarPeticionWeb(endpoint As String, postData As String) As Dictionary(Of String, Object)
        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create("http://localhost/" & endpoint), HttpWebRequest)
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
                    Dim jss As New JavaScriptSerializer()
                    Dim resultado As Dictionary(Of String, Object) = jss.Deserialize(Of Dictionary(Of String, Object))(reader.ReadToEnd())

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
            MessageBox.Show("Error al conectar con el servicio de exportación: " & ex.Message, "Error de Red", MessageBoxButtons.OK, MessageBoxIcon.Error)
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


End Class

Public Class ComboboxItem
    Public Property Text As String
    Public Property Value As String

    Public Overrides Function ToString() As String
        Return Text
    End Function
End Class


