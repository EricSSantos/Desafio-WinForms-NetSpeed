Public Class frmDepartamentosListar
    Private Sub frmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CarregarDepartamentos()
    End Sub

    Private Sub CarregarDepartamentos()
        Dim departamentos As List(Of Departamento) = DepartamentosController.ListarDepartamentos()
        Me.dgvDepartamentos.DataSource = departamentos
    End Sub

    Private Sub AbrirDepartamento()
        If Me.dgvDepartamentos.SelectedRows.Count = 0 Then
            Utils.ExibirMensagem("Selecione um departamento para abrir.", TipoMensagem.Aviso)
            Exit Sub
        End If

        Dim departamento As Departamento = CType(Me.dgvDepartamentos.SelectedRows(0).DataBoundItem, Departamento)

        Dim frm As New frmDepartamentosEditar()
        frm.AbrirDepartamento(departamento.ID)

        If frm.ShowDialog() = DialogResult.OK Then
            CarregarDepartamentos()
        End If
    End Sub

    Private Sub ExcluirDepartamento()
        If Me.dgvDepartamentos.SelectedRows.Count = 0 Then
            Utils.ExibirMensagem("Selecione um deparamento para excluir.", TipoMensagem.Aviso)
            Exit Sub
        End If

        Dim dgvr As DataGridViewRow = Me.dgvDepartamentos.SelectedRows(0)
        Dim departamento As Departamento = CType(dgvr.DataBoundItem, Departamento)
        Dim idDepartamento As Integer = departamento.ID

        Dim dlgResult As DialogResult = Utils.ExibirMensagem($"Deseja realizar a exclusão do departamento nº {idDepartamento}?",
                                                             TipoMensagem.ConfirmacaoSimNao)

        If dlgResult <> DialogResult.Yes Then Exit Sub

        Dim sucesso As Boolean = DepartamentosController.ExcluirDepartamento(idDepartamento)

        If sucesso Then
            CarregarDepartamentos()
        End If
    End Sub

    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click
        Dim frm As New frmDepartamentosEditar()
        If frm.ShowDialog() = DialogResult.OK Then
            CarregarDepartamentos()
        End If
    End Sub

    Private Sub btnAbrir_Click(sender As Object, e As EventArgs) Handles btnAbrir.Click
        AbrirDepartamento()
    End Sub

    Private Sub btnExcluir_Click(sender As Object, e As EventArgs) Handles btnExcluir.Click
        ExcluirDepartamento()
    End Sub

    Private Sub btnRelatorio_Click(sender As Object, e As EventArgs) Handles btnRelatorio.Click
        If Me.dgvDepartamentos.SelectedRows.Count = 0 Then
            Utils.ExibirMensagem("Não há departamentos disponíveis para gerar o relatório.", TipoMensagem.Aviso)
            Exit Sub
        End If

        Dim frm As New frmDepartamentosRelatorio()
        frm.ShowDialog()
    End Sub

    Private Sub dgvDepartamentos_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDepartamentos.CellDoubleClick
        AbrirDepartamento()
    End Sub
End Class
