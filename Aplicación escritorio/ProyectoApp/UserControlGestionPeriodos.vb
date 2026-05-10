Imports MySql.Data.MySqlClient

Public Class UserControlGestionPeriodos

    Private Const ConnString As String = "Server=localhost;Database=plataforma_evalis;Uid=root;Pwd=;"

    Private Sub UserControlGestionPeriodos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CargarEstadoActual()
    End Sub

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        If Me.Visible Then
            CargarEstadoActual()
        End If
    End Sub

    Private Sub CargarEstadoActual()
        Dim rol As String = My.Settings.Rol.ToLower()

        btnActivar.Visible = (rol = "directiu" OrElse My.Settings.Rol = "admin")
        btnFinalizar.Visible = (rol = "directiu" OrElse My.Settings.Rol = "admin")

        Using conn As New MySqlConnection(ConnString)
            Try
                conn.Open()
                Dim cmd As New MySqlCommand("SELECT periodo_abierto FROM estado_evaluacion WHERE id = 1", conn)
                Dim abierto As Boolean = Convert.ToBoolean(cmd.ExecuteScalar())

                If abierto Then
                    lblEstado.Text = "ESTADO ACTUAL: ABIERTO"
                    lblEstado.ForeColor = Color.Green

                    btnPonerCalificacion.Visible = (rol = "professor")
                Else
                    lblEstado.Text = "ESTADO ACTUAL: CERRADO"
                    lblEstado.ForeColor = Color.Red
                    btnPonerCalificacion.Visible = False
                End If

            Catch ex As Exception
                lblEstado.Text = "Error al cargar estado"
                lblEstado.ForeColor = Color.Red
                btnPonerCalificacion.Visible = False
            End Try
        End Using
    End Sub

    Private Sub btnPonerCalificacion_Click(sender As Object, e As EventArgs) Handles btnPonerCalificacion.Click
        Dim ucNotas As New UserControlPonerNotas()

        Dim principal As FormPrincipal = CType(Me.ParentForm, FormPrincipal)
        principal.CargarContenido(ucNotas)
    End Sub

End Class