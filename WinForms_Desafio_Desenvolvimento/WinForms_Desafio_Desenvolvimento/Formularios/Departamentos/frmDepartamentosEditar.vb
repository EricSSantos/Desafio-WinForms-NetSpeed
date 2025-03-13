Public Class frmDepartamentosEditar
    Public Sub AbrirDepartamento(ByVal idDepartamento As Integer)
        Dim departamento As Departamento = DepartamentosController.ObterDepartamento(idDepartamento)

        Me.txtID.Text = departamento.ID.ToString()
        Me.txtDescricao.Text = departamento.Descricao
    End Sub

    Private Function ExibirMensagemCampo(campo As String, mensagem As String) As Boolean
        Utils.ExibirMensagem($"O campo '{campo}' {mensagem}", TipoMensagem.Aviso)
        Return False
    End Function

    Private Function ValidarDescricao() As Boolean
        If String.IsNullOrWhiteSpace(Me.txtDescricao.Text) Then
            Return ExibirMensagemCampo("Descrição", "é obrigatório.")
        End If

        If Me.txtDescricao.Text.Length > 100 Then
            Return ExibirMensagemCampo("Descrição", "não pode ter mais de 100 caracteres.")
        End If

        If Not Utils.PermitirSomenteLetrasEEspacos(Me.txtDescricao.Text) Then
            Return ExibirMensagemCampo("Descrição", "deve conter apenas letras e espaços.")
        End If

        Return True
    End Function

    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        If Not ValidarDescricao() Then Exit Sub

        Dim departamento As New Departamento() With {
            .ID = If(String.IsNullOrEmpty(Me.txtID.Text), 0, Convert.ToInt32(Me.txtID.Text)),
            .Descricao = Me.txtDescricao.Text
        }

        Dim sucesso As Boolean = DepartamentosController.GravarDepartamento(departamento)

        If Not sucesso Then
            Me.DialogResult = DialogResult.Cancel
        Else
            Me.DialogResult = DialogResult.OK
        End If

        Me.Close()
    End Sub
End Class