Public Class frmChamadosEditar
    ' Variável para indicar se o formulário está em modo de edição
    Private editMode As Boolean

    Public Sub New()
        InitializeComponent()

        Dim departamentos As List(Of Departamento) = DepartamentosController.ListarDepartamentos()
        Me.cmbDepartamento.DataSource = departamentos
        Me.cmbDepartamento.DisplayMember = "Descricao"
        Me.cmbDepartamento.ValueMember = "ID"
    End Sub

    Public Sub AbrirChamado(ByVal idChamado As Integer, ByVal editMode As Boolean)
        Dim chamado As Chamado = ChamadosController.ObterChamado(idChamado)

        Me.txtID.Text = chamado.ID.ToString()
        Me.txtAssunto.Text = chamado.Assunto
        Me.txtSolicitante.Text = chamado.Solicitante
        Me.cmbDepartamento.SelectedValue = Convert.ToInt32(chamado.Departamento)

        ' Bloqueia a edição da data se for modo de edição
        Me.editMode = editMode
        Me.dtpDataAbertura.Enabled = Not editMode

        ' Mantém a data original apenas se for edição e a data não for nula
        If editMode AndAlso Not String.IsNullOrEmpty(chamado.DataAbertura) Then
            Me.dtpDataAbertura.Value = DateTime.Parse(chamado.DataAbertura)
        Else
            Me.dtpDataAbertura.Value = DateTime.Now
        End If
    End Sub

    Private Function ExibirMensagemCampo(campo As String, mensagem As String) As Boolean
        Utils.ExibirMensagem($"O campo '{campo}' {mensagem}", TipoMensagem.Aviso)
        Return False
    End Function

    Private Function ValidarAssunto() As Boolean
        If String.IsNullOrWhiteSpace(Me.txtAssunto.Text) Then
            Return ExibirMensagemCampo("Assunto", "é obrigatório.")
        End If

        If Me.txtAssunto.Text.Length > 100 Then
            Return ExibirMensagemCampo("Assunto", "não pode ter mais de 100 caracteres.")
        End If

        Return True
    End Function

    Private Function ValidarSolicitante() As Boolean
        If String.IsNullOrWhiteSpace(Me.txtSolicitante.Text) Then
            Return ExibirMensagemCampo("Solicitante", "é obrigatório.")
        End If

        If Me.txtSolicitante.Text.Length > 50 Then
            Return ExibirMensagemCampo("Solicitante", "não pode ter mais de 50 caracteres.")
        End If

        If Not Utils.PermitirSomenteLetrasEEspacos(Me.txtSolicitante.Text) Then
            Return ExibirMensagemCampo("Solicitante", "deve conter apenas letras e espaços.")
        End If

        Return True
    End Function

    Private Function ValidarDepartamento() As Boolean
        If Me.cmbDepartamento.SelectedValue Is Nothing Then
            Return ExibirMensagemCampo("Departamento", "é obrigatório.")
        End If
        Return True
    End Function

    Private Function ValidarDataAbertura() As Boolean
        If Not editMode AndAlso dtpDataAbertura.Value.Date < DateTime.Now.Date Then
            Utils.ExibirMensagem("A data de abertura não pode ser retroativa.", TipoMensagem.Aviso)
            dtpDataAbertura.Value = DateTime.Now
            Return False
        End If
        Return True
    End Function

    Private Function ValidarEntradas() As Boolean
        If Not ValidarAssunto() Then Return False
        If Not ValidarSolicitante() Then Return False
        If Not ValidarDepartamento() Then Return False
        If Not ValidarDataAbertura() Then Return False

        Return True
    End Function

    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        If Not ValidarEntradas() Then Exit Sub

        Dim chamado As New Chamado() With {
            .ID = If(String.IsNullOrEmpty(Me.txtID.Text), 0, Convert.ToInt32(Me.txtID.Text)),
            .Assunto = Me.txtAssunto.Text,
            .Solicitante = Me.txtSolicitante.Text,
            .Departamento = Me.cmbDepartamento.SelectedValue.ToString(),
            .DataAbertura = Me.dtpDataAbertura.Value.ToString("dd/MM/yyyy")
        }

        Dim sucesso As Boolean = ChamadosController.GravarChamado(chamado)

        If Not sucesso Then
            Me.DialogResult = DialogResult.Cancel
        Else
            Me.DialogResult = DialogResult.OK
        End If

        Me.Close()
    End Sub

    Private Sub dtpDataAbertura_ValueChanged(sender As Object, e As EventArgs) Handles dtpDataAbertura.ValueChanged
        ValidarDataAbertura()
    End Sub
End Class
