Public Class frmChamadosEditar

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim dtDepartamentos As DataTable = Dados.ListarDepartamentos()
        Me.cmbDepartamento.DataSource = dtDepartamentos
        Me.cmbDepartamento.DisplayMember = "Descricao"
        Me.cmbDepartamento.ValueMember = "ID"

    End Sub

    Public Sub AbrirChamado(ByVal idChamado As Integer)

        Dim drChamado As DataRow = Dados.ObterChamado(idChamado)

        Me.txtID.Text = CInt(drChamado("ID")).ToString()
        Me.txtAssunto.Text = CStr(drChamado("Assunto"))
        Me.txtSolicitante.Text = CStr(drChamado("Solicitante"))

        Me.cmbDepartamento.SelectedValue = CInt(drChamado("Departamento"))

        Dim strDataAbertura As String = CStr(drChamado("DataAbertura"))
        Me.dtpDataAbertura.Value = DateTime.Parse(strDataAbertura)

    End Sub

    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click

        Dim ID As Integer = Integer.Parse(Me.txtID.Text)
        Dim Assunto As String = Me.txtAssunto.Text
        Dim Solicitante As String = Me.txtSolicitante.Text
        Dim Departamento As Integer = CInt(Me.cmbDepartamento.SelectedValue)
        Dim DataAbertura As DateTime = Me.dtpDataAbertura.Value

        Dim sucesso As Boolean = Dados.GravarChamado(ID, Assunto, Solicitante, Departamento, DataAbertura)

        If Not sucesso Then

            MessageBox.Show(Me, "Falha ao gravar o chamado", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.DialogResult = DialogResult.Cancel

        Else

            Me.DialogResult = DialogResult.OK

        End If

        Me.Close()

    End Sub
End Class