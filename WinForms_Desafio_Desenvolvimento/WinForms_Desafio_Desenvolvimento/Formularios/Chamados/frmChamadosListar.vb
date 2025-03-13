Public Class frmChamadosListar
    Private Sub frmChamadosListar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CarregarChamados()
    End Sub

    Private Sub CarregarChamados()
        Dim chamados As List(Of Chamado) = ChamadosController.ListarChamados()
        Me.dgvChamados.DataSource = chamados
    End Sub

    Private Sub AbrirChamado()
        If Me.dgvChamados.SelectedRows.Count = 0 Then
            Utils.ExibirMensagem("Selecione um chamado para abrir.", TipoMensagem.Aviso)
            Exit Sub
        End If

        Dim chamado As Chamado = CType(Me.dgvChamados.SelectedRows(0).DataBoundItem, Chamado)

        Dim frm As New frmChamadosEditar()
        frm.AbrirChamado(chamado.ID, True)

        If frm.ShowDialog() = DialogResult.OK Then
            CarregarChamados()
        End If
    End Sub

    Private Sub ExcluirChamado()
        If Me.dgvChamados.SelectedRows.Count = 0 Then
            Utils.ExibirMensagem("Selecione um chamado para excluir.", TipoMensagem.Aviso)
            Exit Sub
        End If

        Dim dgvr As DataGridViewRow = Me.dgvChamados.SelectedRows(0)
        Dim chamado As Chamado = CType(dgvr.DataBoundItem, Chamado)
        Dim idChamado As Integer = chamado.ID

        Dim dlgResult As DialogResult = Utils.ExibirMensagem($"Deseja confirmar a exclusão do Chamado nº {idChamado}?", TipoMensagem.ConfirmacaoSimNao)

        If dlgResult <> DialogResult.Yes Then Exit Sub

        Dim sucesso As Boolean = ChamadosController.ExcluirChamado(idChamado)

        If sucesso Then
            CarregarChamados()
        End If
    End Sub

    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click
        Dim frm As New frmChamadosEditar()
        If frm.ShowDialog() = DialogResult.OK Then
            CarregarChamados()
        End If
    End Sub

    Private Sub btnAbrir_Click(sender As Object, e As EventArgs) Handles btnAbrir.Click
        AbrirChamado()
    End Sub

    Private Sub btnExcluir_Click(sender As Object, e As EventArgs) Handles btnExcluir.Click
        ExcluirChamado()
    End Sub

    Private Sub btnRelatorio_Click(sender As Object, e As EventArgs) Handles btnRelatorio.Click
        If Me.dgvChamados.SelectedRows.Count = 0 Then
            Utils.ExibirMensagem("Não há chamados disponíveis para gerar o relatório.", TipoMensagem.Aviso)
            Exit Sub
        End If

        Dim frm As New frmChamadosRelatorio()
        frm.ShowDialog()
    End Sub

    Private Sub dgvChamados_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvChamados.CellDoubleClick
        AbrirChamado()
    End Sub
End Class
