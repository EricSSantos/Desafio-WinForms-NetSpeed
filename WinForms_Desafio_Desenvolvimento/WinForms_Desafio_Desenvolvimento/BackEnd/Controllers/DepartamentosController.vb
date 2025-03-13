Imports System.Data.SQLite

Public Class DepartamentosController
    Public Shared Function ListarDepartamentos() As List(Of Departamento)
        Dim departamentos As New List(Of Departamento)()

        Using dbConnection As New DbConnection()
            Dim sql As String = "SELECT * FROM departamentos"

            Using reader As SQLiteDataReader = dbConnection.ExecutarConsulta(sql)
                While reader.Read()
                    Dim departamento As New Departamento With {
                        .ID = Convert.ToInt32(reader("ID")),
                        .Descricao = reader("Descricao").ToString()
                    }
                    departamentos.Add(departamento)
                End While
            End Using
        End Using

        Return departamentos
    End Function

    Public Shared Function ObterDepartamento(ByVal idDepartamento As Integer) As Departamento
        Dim departamento As Departamento = Nothing
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@ID", idDepartamento)
        }

        Using dbConnection As New DbConnection()
            Dim sql As String = "SELECT * FROM departamentos WHERE ID = @ID"

            Using reader As SQLiteDataReader = dbConnection.ExecutarConsulta(sql, parametros)
                If reader.HasRows Then
                    reader.Read()
                    departamento = New Departamento With {
                        .ID = Convert.ToInt32(reader("ID")),
                        .Descricao = reader("Descricao").ToString()
                    }
                End If
            End Using
        End Using

        Return departamento
    End Function

    Public Shared Function GravarDepartamento(ByVal departamento As Departamento) As Boolean
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@Descricao", departamento.Descricao)
        }

        If departamento.ID <> 0 Then
            parametros.Add(New SQLiteParameter("@ID", departamento.ID))
        End If

        If departamento.ID = 0 And VerificarDepartamentosDuplicados(departamento.Descricao) Then
            Utils.ExibirMensagem($"Já existe um registro com os mesmos dados informados.", TipoMensagem.Aviso)
            Return False
        End If

        Using dbConnection As New DbConnection()
            Dim regsAfetados As Integer = dbConnection.ExecutarComando(TipoComandoSQL.UpdateOrInsert, "departamentos", parametros)
            Return (regsAfetados > 0)
        End Using
    End Function

    Public Shared Function ExcluirDepartamento(ByVal idDepartamento As Integer) As Boolean
        Dim regsAfetados As Integer = -1
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@ID", idDepartamento)
        }

        Using dbConnection As New DbConnection()
            regsAfetados = dbConnection.ExecutarComando(TipoComandoSQL.Delete, "departamentos", parametros)
        End Using

        Return (regsAfetados > 0)
    End Function

    Private Shared Function VerificarDepartamentosDuplicados(descricao As String) As Boolean
        Dim resultado As Integer
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@Descricao", descricao)
        }

        Using dbConnection As New DbConnection()
            resultado = dbConnection.ExecutarComando(TipoComandoSQL.Count, "departamentos", parametros)
        End Using

        Return resultado > 0
    End Function
End Class
