Public Class FormPrincipal

    Private Sub FormPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnExportacion.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "admin")
        btnGestionPeriodos.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "professor" OrElse My.Settings.Rol = "admin")

        pnlSidebar.BackColor = Color.FromArgb(30, 30, 35)
        pnlSidebar.Padding = New Padding(10)

        Dim botones = {btnInicio, btnProfesores, btnGestionPeriodos, btnExportacion, btnCerrarSesion}

        For Each btn As Button In botones
            btn.FlatStyle = FlatStyle.Flat
            btn.FlatAppearance.BorderSize = 0
            btn.BackColor = Color.FromArgb(30, 30, 35)
            btn.ForeColor = Color.White
            btn.Font = New Font("Segoe UI", 10.5, FontStyle.Regular)
            btn.TextAlign = ContentAlignment.MiddleLeft
            btn.Padding = New Padding(15, 0, 0, 0)
            btn.Height = 50
            btn.Cursor = Cursors.Hand

            AddHandler btn.MouseEnter, AddressOf Btn_MouseEnter
        Next

        btnCerrarSesion.BackColor = Color.FromArgb(200, 50, 50)
        btnCerrarSesion.ForeColor = Color.White

        btnExportacion.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "admin")
        btnGestionPeriodos.Visible = (My.Settings.Rol = "directiu" OrElse My.Settings.Rol = "professor" OrElse My.Settings.Rol = "admin")
    End Sub

    Private Sub Btn_MouseEnter(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        btn.BackColor = Color.FromArgb(0, 122, 204)
    End Sub





    Public Sub CargarContenido(control As UserControl)
        pnlContenido.Controls.Clear()
        control.Dock = DockStyle.Fill
        pnlContenido.Controls.Add(control)
    End Sub

    Private Sub btnInicio_Click(sender As Object, e As EventArgs) Handles btnInicio.Click
        CargarContenido(New UserControlHome())
    End Sub


    Private Sub btnProfesores_Click(sender As Object, e As EventArgs) Handles btnProfesores.Click
        CargarContenido(New UserControlProfesores())
    End Sub



    Private Sub btnGestionPeriodos_Click(sender As Object, e As EventArgs) Handles btnGestionPeriodos.Click
        CargarContenido(New UserControlGestionPeriodos())
    End Sub

    Private Sub btnCerrarSesion_Click(sender As Object, e As EventArgs) Handles btnCerrarSesion.Click
        My.Settings.Token = ""
        My.Settings.DniPersona = ""
        My.Settings.Rol = ""
        My.Settings.Save()

        Dim login As New Form1()
        login.Show()
        Me.Close()
    End Sub

    Private Sub btnExportacion_Click(sender As Object, e As EventArgs) Handles btnExportacion.Click
        CargarContenido(New UserControlExportacion())
    End Sub

    Private Sub btnCerrar_Click(sender As Object, e As EventArgs) Handles btnCerrar.Click
        Dim resultado As DialogResult = MessageBox.Show(
        "¿Estás seguro de que deseas cerrar la sesión?",
        "Cerrar Sesión",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button2)

        If resultado = DialogResult.Yes Then
            My.Settings.Token = ""
            My.Settings.DniPersona = ""
            My.Settings.Rol = ""
            My.Settings.Username = ""
            My.Settings.Save()

            Dim login As New Form1()
            login.Show()
            Me.Close()
        End If
    End Sub
End Class