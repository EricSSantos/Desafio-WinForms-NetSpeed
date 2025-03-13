Public Class Utils
    Public Shared Function ExibirMensagem(mensagem As String, tipo As TipoMensagem) As DialogResult
        Dim icone As MessageBoxIcon
        Dim botoes As MessageBoxButtons
        Dim titulo As String = tipo.ToString()

        ' Verifica se é uma mensagem de confirmação e altera o título
        If tipo = TipoMensagem.ConfirmacaoSimNao OrElse tipo = TipoMensagem.ConfirmacaoSimNaoCancelar Then
            titulo = "Confirmação"
        End If

        Select Case tipo
            Case TipoMensagem.Aviso
                icone = MessageBoxIcon.Warning
                botoes = MessageBoxButtons.OK
            Case TipoMensagem.Erro
                icone = MessageBoxIcon.Error
                botoes = MessageBoxButtons.OK
            Case TipoMensagem.Sucesso
                icone = MessageBoxIcon.Information
                botoes = MessageBoxButtons.OK
            Case TipoMensagem.ConfirmacaoSimNao
                icone = MessageBoxIcon.Question
                botoes = MessageBoxButtons.YesNo
            Case TipoMensagem.ConfirmacaoSimNaoCancelar
                icone = MessageBoxIcon.Question
                botoes = MessageBoxButtons.YesNoCancel
        End Select

        Return MessageBox.Show(mensagem, titulo, botoes, icone)
    End Function

    Public Shared Function PermitirSomenteLetrasEEspacos(texto As String) As Boolean
        Dim regex As New System.Text.RegularExpressions.Regex("^[a-zA-ZÀ-ÿ\s]+$")
        Return regex.IsMatch(texto)
    End Function
End Class
