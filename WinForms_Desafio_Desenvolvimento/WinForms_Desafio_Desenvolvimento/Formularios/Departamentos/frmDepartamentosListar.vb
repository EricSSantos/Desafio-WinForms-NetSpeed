Imports System.Data
Imports System.Windows.Forms

Public Class frmDepartamentosListar

    Private Sub frmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim dtDepartamentos As DataTable = Dados.ListarDepartamentos()
        Me.dgvDepartamentos.DataSource = dtDepartamentos

    End Sub

    Private Sub btnExcluir_Click(sender As Object, e As EventArgs) Handles btnExcluir.Click


    End Sub

    Private Sub btnAbrir_Click(sender As Object, e As EventArgs) Handles btnAbrir.Click


    End Sub

    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click


    End Sub

    Private Sub btnRelatorio_Click(sender As Object, e As EventArgs) Handles btnRelatorio.Click

        Dim frm As New frmDepartamentosRelatorio()
        frm.ShowDialog()

    End Sub
End Class
