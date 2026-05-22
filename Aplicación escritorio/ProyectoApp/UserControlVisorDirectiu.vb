Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization

Public Class UserControlVisorDirectiu

    ' Cambia esta URL por la dirección real de tu servidor local o remoto
    Private Const BaseUrl As String = "http://localhost/"

    ' Variables globales para la lectura del Excel y modificación del centro
    Private RutaExcelModificacion As String = ""
    Private RutaNuevoLogo As String = ""
    Private InfoCentroExcel_Nom As String = ""
    Private InfoCentroExcel_Dir As String = ""
    Private InfoCentroExcel_Pla As String = ""
    Private ListaNuevosEstudios As New List(Of String)()

    ' Estructura auxiliar para almacenar de forma emparejada el ID y el Nombre en el ComboBox
    Private Class CentroItem
        Public Property Id As String
        Public Property Nombre As String
    End Class

    Private Sub UserControlVisorDirectiu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarDataGridView()
        EstilizarDataGridView(dgvDetalleCentro)
        CargarCentrosEnComboBox()
    End Sub

    ''' <summary>
    ''' Inicializa las columnas del DataGridView estructurando la información del centro y sus estudios.
    ''' </summary>
    Private Sub ConfigurarDataGridView()
        If dgvDetalleCentro.Columns.Count > 0 Then Return

        dgvDetalleCentro.Columns.Clear()
        dgvDetalleCentro.Columns.Add("centro_nom", "Centro")
        dgvDetalleCentro.Columns.Add("centro_dir", "Dirección")
        dgvDetalleCentro.Columns.Add("centro_pla", "Plan Escolar")
        dgvDetalleCentro.Columns.Add("estudio_nom", "Estudio / Ciclo Ofertado")
        dgvDetalleCentro.Columns.Add("estudio_curso", "Curso")
    End Sub

    ' ==========================================================
    ' PASO 1: TRAER TODOS LOS CENTROS AL COMBOBOX (INICIALIZAR)
    ' ==========================================================
    Private Sub CargarCentrosEnComboBox()
        cmbCentros.DataSource = Nothing
        cmbCentros.Items.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash())
        )

        Dim json As Dictionary(Of String, Object) = EnviarPeticionWeb("get_centros_combobox.php", postData)

        If json IsNot Nothing AndAlso json("status").ToString() = "success" Then
            If json.ContainsKey("centros") Then
                Dim listaCentros As Object = json("centros")
                Dim datasourceCentros As New List(Of CentroItem)()

                For Each c As Object In CType(listaCentros, IEnumerable)
                    Dim cDict As Dictionary(Of String, Object) = CType(c, Dictionary(Of String, Object))

                    ' Validación estricta para evitar punteros nulos de claves
                    If cDict IsNot Nothing AndAlso cDict.ContainsKey("id") AndAlso cDict.ContainsKey("nom") Then
                        datasourceCentros.Add(New CentroItem With {
                            .Id = If(cDict("id") IsNot Nothing, cDict("id").ToString(), ""),
                            .Nombre = If(cDict("nom") IsNot Nothing, cDict("nom").ToString(), "Sin Nombre")
                        })
                    End If
                Next

                cmbCentros.DisplayMember = "Nombre"
                cmbCentros.ValueMember = "Id"
                cmbCentros.DataSource = datasourceCentros

                ' Forzamos estado deseleccionado por defecto
                cmbCentros.SelectedIndex = -1
            End If
        ElseIf json IsNot Nothing Then
            MessageBox.Show("Error al cargar centros: " & json("motivo").ToString(), "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' ==========================================================
    ' PASO 2: DETECTAR CAMBIO EN EL COMBO Y CARGAR GRID
    ' ==========================================================
    Private Sub cmbCentros_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCentros.SelectedIndexChanged
        If cmbCentros.SelectedIndex = -1 OrElse cmbCentros.SelectedValue Is Nothing Then Return

        ' Evitamos falsos positivos cuando el control está cargando enlazando objetos temporales
        If Not TypeOf cmbCentros.SelectedValue Is String AndAlso Not TypeOf cmbCentros.SelectedValue Is Integer Then Return

        Dim idCentroSeleccionado As String = cmbCentros.SelectedValue.ToString()
        If String.IsNullOrEmpty(idCentroSeleccionado) OrElse idCentroSeleccionado.Contains("Item") Then Return

        CargarTablaDetalleCentro(idCentroSeleccionado)
    End Sub

    Private Sub CargarTablaDetalleCentro(idCentro As String)
        dgvDetalleCentro.Rows.Clear()

        Dim postData As String = String.Format(
            "token={0}&user_agent_hash={1}&id_centre={2}",
            Uri.EscapeDataString(My.Settings.Token),
            Uri.EscapeDataString(GenerarHardwareHash()),
            Uri.EscapeDataString(idCentro)
        )

        Dim json As Dictionary(Of String, Object) = EnviarPeticionWeb("get_detalle_centro.php", postData)

        If json IsNot Nothing AndAlso json("status").ToString() = "success" Then
            Dim infoCentro As Dictionary(Of String, Object) = CType(json("info_centro"), Dictionary(Of String, Object))
            Dim cNom As String = infoCentro("nom").ToString()
            Dim cDir As String = infoCentro("direccio").ToString()
            Dim cPla As String = infoCentro("pla").ToString()

            If json.ContainsKey("estudios") Then
                Dim listaEstudios As Object = json("estudios")

                For Each est As Object In CType(listaEstudios, IEnumerable)
                    Dim estDict As Dictionary(Of String, Object) = CType(est, Dictionary(Of String, Object))
                    Dim eNom As String = estDict("nom_estudio").ToString()
                    Dim eCurso As String = estDict("curso").ToString()

                    dgvDetalleCentro.Rows.Add(cNom, cDir, cPla, eNom, eCurso)
                Next
            End If
        ElseIf json IsNot Nothing Then
            MessageBox.Show("No se pudieron obtener los detalles: " & json("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' ==========================================================
    ' PASO 3: SELECCIONAR LOGO ADICIONAL (OPCIONAL)
    ' ==========================================================
    Private Sub btnCambiarLogo_Click(sender As Object, e As EventArgs) Handles btnCambiarLogo.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
        If ofd.ShowDialog() = DialogResult.OK Then
            RutaNuevoLogo = ofd.FileName
            MessageBox.Show("Logotipo listo para subida: " & Path.GetFileName(RutaNuevoLogo), "Logo Cargado", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' ==========================================================
    ' PASO 4: LEER ARCHIVO EXCEL (.XLSX) MEDIANTE OLEDB E IMPORTAR
    ' ==========================================================
    Private Sub btnModificarPorExcel_Click(sender As Object, e As EventArgs) Handles btnModificarPorExcel.Click
        If cmbCentros.SelectedIndex = -1 OrElse cmbCentros.SelectedValue Is Nothing Then
            MessageBox.Show("Por favor, seleccione primero en el ComboBox qué centro desea modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim idCentroSeleccionado As String = cmbCentros.SelectedValue.ToString()

        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Archivos de Excel (*.xlsx)|*.xlsx|Archivos de Excel Antiguos (*.xls)|*.xls"

        If ofd.ShowDialog() = DialogResult.OK Then
            RutaExcelModificacion = ofd.FileName
            ListaNuevosEstudios.Clear()

            ' Driver de conexión OLEDB nativo de Microsoft para abrir el binario .xlsx
            Dim conString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & RutaExcelModificacion & ";Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1;'"

            If Path.GetExtension(RutaExcelModificacion).ToLower() = ".xls" Then
                conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & RutaExcelModificacion & ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'"
            End If

            Try
                Using con As New System.Data.OleDb.OleDbConnection(conString)
                    ' Leemos el contenido mapeado de la primera pestaña, por defecto llamada 'Hoja1'
                    Dim query As String = "SELECT * FROM [Hoja1$]"

                    Using cmd As New System.Data.OleDb.OleDbCommand(query, con)
                        con.Open()
                        Using reader As System.Data.OleDb.OleDbDataReader = cmd.ExecuteReader()
                            Dim esPrimeraFila As Boolean = True

                            While reader.Read()
                                ' Capturamos los datos generales en la primera lectura de registro
                                If esPrimeraFila Then
                                    InfoCentroExcel_Nom = If(reader("Nombre") IsNot DBNull.Value, reader("Nombre").ToString().Trim(), "")
                                    InfoCentroExcel_Dir = If(reader("Direccion") IsNot DBNull.Value, reader("Direccion").ToString().Trim(), "")
                                    InfoCentroExcel_Pla = If(reader("Plan") IsNot DBNull.Value, reader("Plan").ToString().Trim(), "")
                                    esPrimeraFila = False
                                End If

                                ' Colección vertical dinámica para la columna de los Ciclos
                                If reader("Estudios") IsNot DBNull.Value Then
                                    Dim estudio As String = reader("Estudios").ToString().Trim()
                                    If Not String.IsNullOrEmpty(estudio) Then
                                        ListaNuevosEstudios.Add(estudio)
                                    End If
                                End If
                            End While
                        End Using
                    End Using
                End Using

                ' Si las columnas no se llamaban igual que los índices del Reader, cancelamos la subida
                If String.IsNullOrEmpty(InfoCentroExcel_Nom) OrElse ListaNuevosEstudios.Count = 0 Then
                    MessageBox.Show("El Excel no posee el formato de columnas esperado (Nombre, Direccion, Plan, Estudios).", "Error de Mapeo", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' Enviar el bloque de datos al servidor
                EjecutarModificacionServidor(idCentroSeleccionado)

            Catch ex As Exception
                MessageBox.Show("Error al analizar el Excel: " & ex.Message & vbCrLf & "Comprueba que la pestaña se llame 'Hoja1'.", "Error OLEDB", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' ==========================================================
    ' PASO 5: ENVÍO MULTIPART AL BACKEND PHP
    ' ==========================================================
    Private Sub EjecutarModificacionServidor(idCentro As String)
        Dim jss As New JavaScriptSerializer()
        Dim estudiosJson As String = jss.Serialize(ListaNuevosEstudios)

        Dim boundary As String = "---------------------------" & DateTime.Now.Ticks.ToString("x")
        Dim request As HttpWebRequest = CType(WebRequest.Create(BaseUrl & "modificar_centro_excel.php"), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "multipart/form-data; boundary=" & boundary
        ServicePointManager.ServerCertificateValidationCallback = Function() True

        Using memStream As New MemoryStream()
            EscribirCampoMultipart(memStream, boundary, "token", My.Settings.Token)
            EscribirCampoMultipart(memStream, boundary, "user_agent_hash", GenerarHardwareHash())
            EscribirCampoMultipart(memStream, boundary, "id_centre", idCentro)
            EscribirCampoMultipart(memStream, boundary, "nom_centro", InfoCentroExcel_Nom)
            EscribirCampoMultipart(memStream, boundary, "direccio_centro", InfoCentroExcel_Dir)
            EscribirCampoMultipart(memStream, boundary, "pla_centro", InfoCentroExcel_Pla)
            EscribirCampoMultipart(memStream, boundary, "estudios", estudiosJson)

            ' Adjuntar archivo binario del logo si existe
            If Not String.IsNullOrEmpty(RutaNuevoLogo) AndAlso File.Exists(RutaNuevoLogo) Then
                Dim fileBytes As Byte() = File.ReadAllBytes(RutaNuevoLogo)
                Dim fileHeader As String = String.Format("--{0}" & vbCrLf & "Content-Disposition: form-data; name=""logo_file""; filename=""{1}""" & vbCrLf & "Content-Type: image/png" & vbCrLf & vbCrLf, boundary, Path.GetFileName(RutaNuevoLogo))
                Dim fileHeaderBytes As Byte() = Encoding.UTF8.GetBytes(fileHeader)

                memStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length)
                memStream.Write(fileBytes, 0, fileBytes.Length)
                memStream.Write(Encoding.UTF8.GetBytes(vbCrLf), 0, 2)
            End If

            Dim footerBytes As Byte() = Encoding.UTF8.GetBytes("--" & boundary & "--" & vbCrLf)
            memStream.Write(footerBytes, 0, footerBytes.Length)
            request.ContentLength = memStream.Length

            Try
                Using requestStream As Stream = request.GetRequestStream()
                    memStream.Position = 0
                    memStream.CopyTo(requestStream)
                End Using

                Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                    Using reader As New StreamReader(response.GetResponseStream())
                        Dim jsonRespuesta As Dictionary(Of String, Object) = jss.Deserialize(Of Dictionary(Of String, Object))(reader.ReadToEnd())

                        If jsonRespuesta IsNot Nothing AndAlso jsonRespuesta("status").ToString() = "success" Then
                            ' Actualización y salvado transparente del nuevo token rotado
                            My.Settings.Token = jsonRespuesta("new_token").ToString()
                            My.Settings.Save()

                            MessageBox.Show(jsonRespuesta("motivo").ToString(), "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

                            ' Limpieza e impacto inmediato en el Grid
                            RutaNuevoLogo = ""
                            CargarTablaDetalleCentro(idCentro)
                        Else
                            MessageBox.Show("El servidor rechazó los datos: " & jsonRespuesta("motivo").ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Fallo de conexión HTTP: " & ex.Message, "Error de Red", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    ' ==========================================================
    ' MÉTODOS INTERNOS COMPLEMENTARIOS (RED, HASH, DISEÑO)
    ' ==========================================================
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
            MessageBox.Show("Error de red: " & ex.Message, "Error HTTP", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    Private Sub EscribirCampoMultipart(stream As Stream, boundary As String, nombre As String, valor As String)
        Dim formato As String = String.Format("--{0}" & vbCrLf & "Content-Disposition: form-data; name=""{1}""" & vbCrLf & vbCrLf & "{2}" & vbCrLf, boundary, nombre, valor)
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(formato)
        stream.Write(bytes, 0, bytes.Length)
    End Sub

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
            .AllowUserToAddRows = False : .AllowUserToDeleteRows = False : .DefaultCellStyle.Font = New Font("Segoe UI", 9.5!)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 50, 60) : .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.5!, FontStyle.Bold) : .ColumnHeadersHeight = 38 : .EnableHeadersVisualStyles = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255)
        End With
    End Sub
End Class