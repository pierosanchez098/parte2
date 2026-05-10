Imports MySql.Data.MySqlClient
Imports System.Drawing

Public Class UserControlProfesores

    Private Const ConnString As String = "Server=localhost;Database=plataforma_evalis;Uid=root;Pwd=;"
    Private Const DefaultPhotoUrl As String = "https://cdn.pixabay.com/photo/2023/02/18/11/00/icon-7797704_640.png"

    Private Sub UserControlProfesores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarProfesores()
    End Sub

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        If Me.Visible Then
            CargarProfesores()
        End If
    End Sub

    Private Sub CargarProfesores()
        flpProfesores.Controls.Clear()

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()

                Dim sql As String = "
                    SELECT 
                        CONCAT(p.nom, ' ', p.cognom) AS nombre_completo,
                        p.email,
                        pr.especialitzacio,
                        p.foto
                    FROM persona p
                    INNER JOIN professor pr ON p.dni = pr.dni_persona
                    WHERE p.rol = 'professor'
                    ORDER BY p.nom, p.cognom"

                Using cmd As New MySqlCommand(sql, conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim panel As New Panel With {
                                .Size = New Size(280, 340),
                                .Margin = New Padding(10),
                                .BackColor = Color.White,
                                .BorderStyle = BorderStyle.FixedSingle
                            }

                            Dim pb As New PictureBox With {
                                .Size = New Size(260, 200),
                                .Location = New Point(10, 10),
                                .SizeMode = PictureBoxSizeMode.Zoom,
                                .BorderStyle = BorderStyle.FixedSingle
                            }

                            Dim fotoUrl As String = reader("foto").ToString().Trim()

                            If Not String.IsNullOrEmpty(fotoUrl) Then
                                Try
                                    pb.Load(fotoUrl)
                                Catch
                                    pb.Load(DefaultPhotoUrl)
                                End Try
                            Else
                                pb.Load(DefaultPhotoUrl)
                            End If

                            Dim lblNombre As New Label With {
                                .Text = reader("nombre_completo").ToString(),
                                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                                .Location = New Point(10, 220),
                                .AutoSize = True
                            }

                            Dim lblEmail As New Label With {
                                .Text = "Email: " & reader("email").ToString(),
                                .Font = New Font("Segoe UI", 10),
                                .Location = New Point(10, 245),
                                .AutoSize = True
                            }

                            Dim lblEspecial As New Label With {
                                .Text = "Especialización: " & reader("especialitzacio").ToString(),
                                .Font = New Font("Segoe UI", 10),
                                .Location = New Point(10, 270),
                                .AutoSize = True
                            }

                            panel.Controls.Add(pb)
                            panel.Controls.Add(lblNombre)
                            panel.Controls.Add(lblEmail)
                            panel.Controls.Add(lblEspecial)

                            flpProfesores.Controls.Add(panel)
                        End While
                    End Using
                End Using

            Catch ex As Exception
                MessageBox.Show("Error al cargar profesores: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

End Class