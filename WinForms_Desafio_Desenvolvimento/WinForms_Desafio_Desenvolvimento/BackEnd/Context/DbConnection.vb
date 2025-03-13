Imports System.Data.SQLite

Public Class DbConnection
    Implements IDisposable

    Private Const CONNECTION_STRING As String = "Data Source=Dados/DesafioDB.db;Version=3;"

    Private _connection As SQLiteConnection
    Private _transaction As SQLiteTransaction
    Private _disposed As Boolean = False

    Public Sub New()
        _connection = New SQLiteConnection(CONNECTION_STRING)
    End Sub

    ' Gerenciamento de transações
    Public Sub IniciarTransacao()
        AbrirConexao()
        _transaction = _connection.BeginTransaction()
    End Sub

    Public Sub Commit()
        If _transaction IsNot Nothing Then
            _transaction.Commit()
            _transaction.Dispose()
            _transaction = Nothing
        End If
    End Sub

    Public Sub Rollback()
        If _transaction IsNot Nothing Then
            _transaction.Rollback()
            _transaction.Dispose()
            _transaction = Nothing
        End If
    End Sub

    ' Executa um comando SQL baseado no tipo de operação especificado (COUNT, INSERT, UPDATE, DELETE ou UPDATE_OR_INSERT).
    ' Essa função é útil para executar comandos SQL mais simples, como contagens ou modificações de dados, 
    ' sem a necessidade de escrever o SQL completo repetidamente. A operação é determinada pelo tipo fornecido.
    Public Function ExecutarComando(tipoOperacao As TipoComandoSQL,
                                tabela As String, parametros As List(Of SQLiteParameter)) As Integer
        Select Case tipoOperacao
            Case TipoComandoSQL.Count
                Return ExecutarCount(tabela, parametros)
            Case TipoComandoSQL.Insert
                Return ExecutarInsert(tabela, parametros)
            Case TipoComandoSQL.Update
                Return ExecutarUpdate(tabela, parametros)
            Case TipoComandoSQL.Delete
                Return ExecutarDelete(tabela, parametros)
            Case TipoComandoSQL.UpdateOrInsert
                Return ExecutarUpdateOrInsert(tabela, parametros)
            Case Else
                Throw New ArgumentException("Tipo de operação inválido.")
        End Select
    End Function

    ' Função para executar uma consulta SQL e retornar um SQLiteDataReader.
    ' Pode ser utilizada quando for necessário executar uma consulta SELECT mais complexa.
    Public Function ExecutarConsulta(sql As String, Optional parametros As List(Of SQLiteParameter) = Nothing) As SQLiteDataReader
        Try
            Dim comando As New SQLiteCommand(sql, _connection)
            If parametros IsNot Nothing Then AdicionarParametros(comando, parametros)

            AbrirConexao()

            If _transaction IsNot Nothing Then
                comando.Transaction = _transaction
            End If

            Return comando.ExecuteReader(CommandBehavior.CloseConnection)
        Catch ex As Exception
            Throw New Exception("Erro ao executar consulta SQL.", ex)
        End Try
    End Function

    ' Função para executar um comando SQL que não retorna dados (como INSERT, UPDATE ou DELETE).
    ' Pode ser utilizada quando for necessário executar um comando SQL que modifica dados no banco.
    Public Function ExecutarComando(sql As String, Optional parametros As List(Of SQLiteParameter) = Nothing) As Integer
        Try
            Using comando As New SQLiteCommand(sql, _connection)
                AdicionarParametros(comando, parametros)
                AbrirConexao()

                Dim resultado As Integer = comando.ExecuteNonQuery()

                If _transaction Is Nothing Then
                    _connection.Close()
                End If

                Return resultado
            End Using
        Catch ex As Exception
            Throw New Exception("Erro ao executar comando SQL.", ex)
        End Try
    End Function

    ' Função para executar uma consulta SQL que retorna um valor único (como um COUNT ou SUM).
    ' Pode ser utilizada quando for necessário obter um valor escalar (como um número ou string) de uma consulta SQL.
    Public Function ExecutarScalar(sql As String, Optional parametros As List(Of SQLiteParameter) = Nothing) As Object
        Try
            Using comando As New SQLiteCommand(sql, _connection)
                AdicionarParametros(comando, parametros)
                AbrirConexao()

                If _transaction IsNot Nothing Then
                    comando.Transaction = _transaction
                End If

                Return comando.ExecuteScalar(CommandBehavior.CloseConnection)
            End Using
        Catch ex As Exception
            Throw New Exception("Erro ao executar consulta escalar.", ex)
        End Try
    End Function

    ' Métodos auxiliares
    Private Function ExecutarCount(tabela As String, parametros As List(Of SQLiteParameter)) As Integer
        Dim sql As String = $"SELECT COUNT(*) FROM {tabela} {AdicionarWhere(parametros, True)}"
        Dim count As Object = ExecutarScalar(sql, parametros)

        Return If(count IsNot Nothing, Convert.ToInt32(count), 0)
    End Function

    Private Function ExecutarUpdateOrInsert(tabela As String, parametros As List(Of SQLiteParameter)) As Integer
        Dim registrosAfetados As Integer
        Dim idParametro As SQLiteParameter = ObterIdParametro(parametros)

        If idParametro Is Nothing OrElse idParametro.Value Is DBNull.Value Then
            registrosAfetados = ExecutarInsert(tabela, parametros)
        Else
            registrosAfetados = ExecutarUpdate(tabela, parametros)
        End If

        Return registrosAfetados
    End Function

    Private Function ExecutarInsert(tabela As String, parametros As List(Of SQLiteParameter)) As Integer
        Dim colunas = String.Join(", ", parametros.Select(Function(p) p.ParameterName.TrimStart("@"c)))
        Dim valores = String.Join(", ", parametros.Select(Function(p) p.ParameterName))

        Dim sql As String = $"INSERT INTO {tabela} ({colunas}) VALUES ({valores});"

        IniciarTransacao()
        Try
            Dim registrosAfetados As Integer = ExecutarComando(sql, parametros)
            Commit()

            Return registrosAfetados
        Catch ex As Exception
            Rollback()
            Throw New Exception("Erro ao executar INSERT.", ex)
        End Try
    End Function

    Private Function ExecutarUpdate(tabela As String, parametros As List(Of SQLiteParameter)) As Integer
        Dim idParametro As SQLiteParameter = ObterIdParametro(parametros)
        If idParametro Is Nothing OrElse idParametro.Value Is DBNull.Value Then
            Throw New Exception("ID não fornecido para o UPDATE.")
        End If

        Dim sql As String = $"UPDATE {tabela} SET {AdicionarSets(parametros)} {AdicionarWhere(idParametro)}"

        IniciarTransacao()
        Try
            Dim registrosAfetados As Integer = ExecutarComando(sql, parametros)
            Commit()

            Return registrosAfetados
        Catch ex As Exception
            Rollback()
            Throw New Exception("Erro ao executar UPDATE.", ex)
        End Try
    End Function

    Private Function ExecutarDelete(tabela As String, parametros As List(Of SQLiteParameter)) As Integer
        Dim sql As String = $"DELETE FROM {tabela} {AdicionarWhere(parametros)}"

        IniciarTransacao()
        Try
            Dim regsAfetados As Integer = ExecutarComando(sql, parametros)
            Commit()

            Return regsAfetados
        Catch ex As Exception
            Rollback()
            Throw New Exception("Erro ao executar DELETE.", ex)
        End Try
    End Function

    Private Sub AbrirConexao()
        If _connection.State = ConnectionState.Closed Then _connection.Open()
    End Sub

    Private Sub AdicionarParametros(comando As SQLiteCommand, parametros As List(Of SQLiteParameter))
        If parametros IsNot Nothing Then
            For Each param As SQLiteParameter In parametros
                comando.Parameters.Add(param)
            Next
        End If
    End Sub

    Private Function AdicionarWhere(parametros As List(Of SQLiteParameter), Optional usarNoCase As Boolean = False) As String
        If parametros Is Nothing OrElse parametros.Count = 0 Then
            Return String.Empty
        End If

        ' Retorna a cláusula WHERE com as condições para múltiplos parâmetros
        Return $" WHERE " & String.Join(" AND ", parametros.Select(Function(p) AdicionarNoCase(p, usarNoCase)))
    End Function

    Private Function AdicionarWhere(parametro As SQLiteParameter, Optional usarNoCase As Boolean = False) As String
        If parametro Is Nothing OrElse parametro.Value Is DBNull.Value Then
            Return String.Empty
        End If

        ' Retorna a cláusula WHERE para um parâmetro específico
        ' Certifique-se de que estamos usando o parâmetro corretamente
        Return $"WHERE {parametro.ParameterName.TrimStart("@"c)} = @{parametro.ParameterName.TrimStart("@"c)}"
    End Function

    Private Function AdicionarNoCase(p As SQLiteParameter, usarNoCase As Boolean) As String
        If p.Value.GetType() = GetType(String) AndAlso usarNoCase Then
            Return $"{p.ParameterName.TrimStart("@"c)} COLLATE NOCASE = {p.ParameterName}"
        Else
            Return $"{p.ParameterName.TrimStart("@"c)} = {p.ParameterName}"
        End If
    End Function

    Private Function AdicionarSets(parametros As List(Of SQLiteParameter)) As String
        Return String.Join(", ", parametros _
            .Where(Function(p) p.ParameterName <> "@ID") _
            .Select(Function(p) $"{p.ParameterName.TrimStart("@"c)} = {p.ParameterName}"))
    End Function

    Private Function ObterIdParametro(parametros As List(Of SQLiteParameter)) As SQLiteParameter
        Dim id As SQLiteParameter = parametros.FirstOrDefault(Function(p) p.ParameterName = "@ID")
        If id IsNot Nothing AndAlso id.Value IsNot DBNull.Value Then
            Return id
        End If

        Return Nothing
    End Function

    Private Sub Dispose() Implements IDisposable.Dispose
        If Not _disposed Then
            _transaction?.Dispose()
            _connection?.Dispose()
            _disposed = True
        End If
    End Sub
End Class
