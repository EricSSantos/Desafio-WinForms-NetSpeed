Public Class frmPrincipal

    Public Sub New()
        InitializeComponent()

        ChamadosController.ListarChamados()
        DepartamentosController.ListarDepartamentos()
    End Sub

    Private Sub btnChamados_Click(sender As Object, e As EventArgs) Handles btnChamados.Click
        Dim frm As New frmChamadosListar()
        frm.Show(Me)
    End Sub

    Private Sub btnDepartamentos_Click(sender As Object, e As EventArgs) Handles btnDepartamentos.Click
        Dim frm As New frmDepartamentosListar()
        frm.Show(Me)
    End Sub
End Class