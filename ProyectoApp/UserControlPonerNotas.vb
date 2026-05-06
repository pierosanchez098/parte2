Imports MySql.Data.MySqlClient

Public Class UserControlPonerNotas

    Private Const ConnString As String = "Server=localhost;Database=plataforma_evalis;Uid=root;Pwd=;"

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

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Dim sql As String = "
                    SELECT DISTINCT gc.nom, gc.aula 
                    FROM professor_grup_classe pgc
                    INNER JOIN grup_classe gc ON pgc.nom_grup = gc.nom
                    WHERE pgc.dni_persona = @dni
                    ORDER BY gc.nom"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@dni", My.Settings.DniPersona)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            cmbGrupo.Items.Add(reader("nom").ToString() & " - " & reader("aula").ToString())
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error al cargar grupos: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub cmbGrupo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbGrupo.SelectedIndexChanged
        If cmbGrupo.SelectedIndex = -1 Then Return
        Dim nomGrupo As String = cmbGrupo.Text.Split("-"c)(0).Trim()
        CargarUnidades(nomGrupo)
    End Sub

    Private Sub CargarUnidades(nomGrupo As String)
        cmbUnidad.Items.Clear()

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Dim sql As String = "
                SELECT u.id, u.num, 
                       CONCAT(m.nom_complet, ' - ', u.tipus, ' ', u.nom) AS unidad
                FROM unitat u
                INNER JOIN modul m ON u.modul = m.nom
                INNER JOIN grupclasse_assignatura gca ON m.codi_assignatura = gca.codi_assignatura
                WHERE gca.nom_grupclasse = @nomGrupo
                ORDER BY m.nom_complet, u.num"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@nomGrupo", nomGrupo)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim displayText As String = reader("unidad").ToString()

                            If Not reader.IsDBNull(reader.GetOrdinal("num")) Then
                                displayText &= " (RA " & reader("num").ToString() & ")"
                            End If

                            Dim item As New UnidadItem With {
                            .Text = displayText,
                            .Value = Convert.ToInt32(reader("id"))
                        }

                            cmbUnidad.Items.Add(item)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error al cargar unidades: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub cmbUnidad_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbUnidad.SelectedIndexChanged
        If cmbUnidad.SelectedIndex = -1 Then Return

        Dim item As UnidadItem = DirectCast(cmbUnidad.SelectedItem, UnidadItem)
        Dim idUnidad As Integer = item.Value

        CargarAlumnos(idUnidad)
    End Sub

    Private Sub CargarAlumnos(idUnidad As Integer)
        dgvNotas.Rows.Clear()

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Dim sql As String = "
                    SELECT 
                        e.nia,
                        CONCAT(p.nom, ' ', p.cognom) AS alumno,
                        n.nota
                    FROM estudiants_grupclasse eg
                    INNER JOIN estudiants e ON eg.nia = e.nia
                    INNER JOIN persona p ON e.dni_persona = p.dni
                    LEFT JOIN notes n ON e.nia = n.nia AND n.uf_id = @idUnidad
                    WHERE eg.nom_grup = @nomGrupo
                    ORDER BY p.nom, p.cognom"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@idUnidad", idUnidad)
                    cmd.Parameters.AddWithValue("@nomGrupo", cmbGrupo.Text.Split("-"c)(0).Trim())

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            dgvNotas.Rows.Add(
                                reader("nia"),
                                reader("alumno").ToString(),
                                If(reader("nota") Is DBNull.Value, "", reader("nota"))
                            )
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error al cargar alumnos: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnGuardarNotas_Click(sender As Object, e As EventArgs) Handles btnGuardarNotas.Click
        If cmbUnidad.SelectedIndex = -1 Then
            MessageBox.Show("Selecciona primero una unidad", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim idUnidad As Integer = DirectCast(cmbUnidad.SelectedItem, Object).Value

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()

                For Each row As DataGridViewRow In dgvNotas.Rows
                    If row.IsNewRow Then Continue For

                    Dim nia As Integer = Convert.ToInt32(row.Cells("nia").Value)
                    Dim nota As String = row.Cells("nota").Value?.ToString().Trim()

                    If String.IsNullOrEmpty(nota) Then Continue For

                    Dim sql As String = "
                        INSERT INTO notes (nia, uf_id, nota, data_nota)
                        VALUES (@nia, @uf_id, @nota, NOW())
                        ON DUPLICATE KEY UPDATE nota = @nota, data_nota = NOW()"

                    Using cmd As New MySqlCommand(sql, conn)
                        cmd.Parameters.AddWithValue("@nia", nia)
                        cmd.Parameters.AddWithValue("@uf_id", idUnidad)
                        cmd.Parameters.AddWithValue("@nota", nota)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                MessageBox.Show("Notas guardadas correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Error al guardar notas: " & ex.Message)
            End Try
        End Using
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