Imports System.Data.SQLite

Public Class ChamadosController
    Public Shared Function ListarChamados() As List(Of Chamado)
        Dim chamados As New List(Of Chamado)()

        Using dbConnection As New DbConnection()
            Dim sql As String = "SELECT chamados.ID, " &
                            "       Assunto, " &
                            "       Solicitante, " &
                            "       departamentos.Descricao AS Departamento, " &
                            "       DataAbertura " &
                            "FROM chamados " &
                            "INNER JOIN departamentos " &
                            "   ON chamados.Departamento = departamentos.ID"

            Using reader As SQLiteDataReader = dbConnection.ExecutarConsulta(sql)
                While reader.Read()
                    Dim chamado As New Chamado With {
                        .ID = Convert.ToInt32(reader("ID")),
                        .Assunto = reader("Assunto").ToString(),
                        .Solicitante = reader("Solicitante").ToString(),
                        .Departamento = reader("Departamento").ToString(),
                        .DataAbertura = reader("DataAbertura").ToString()
                    }
                    chamados.Add(chamado)
                End While
            End Using
        End Using

        Return chamados
    End Function

    Public Shared Function ObterChamado(ByVal idChamado As Integer) As Chamado
        Dim chamado As Chamado = Nothing

        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@ID", idChamado)
        }

        Using dbConnection As New DbConnection()
            Dim sql As String = "SELECT * FROM chamados WHERE ID = @ID"

            Using reader As SQLiteDataReader = dbConnection.ExecutarConsulta(sql, parametros)
                If reader.HasRows Then
                    reader.Read()
                    chamado = New Chamado With {
                        .ID = Convert.ToInt32(reader("ID")),
                        .Assunto = reader("Assunto").ToString(),
                        .Solicitante = reader("Solicitante").ToString(),
                        .Departamento = reader("Departamento").ToString(),
                        .DataAbertura = reader("DataAbertura").ToString()
                    }
                End If
            End Using
        End Using

        Return chamado
    End Function

    Public Shared Function GravarChamado(ByVal chamado As Chamado) As Boolean
        Dim regsAfetados As Integer = -1
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@Assunto", chamado.Assunto),
            New SQLiteParameter("@Solicitante", chamado.Solicitante),
            New SQLiteParameter("@Departamento", Convert.ToInt32(chamado.Departamento)),
            New SQLiteParameter("@DataAbertura", chamado.DataAbertura)
        }

        If chamado.ID <> 0 Then
            parametros.Add(New SQLiteParameter("@ID", chamado.ID))
        End If

        If chamado.ID = 0 And VerificarChamadosDuplicados(chamado.Assunto, chamado.Solicitante, Convert.ToInt32(chamado.Departamento)) Then
            Utils.ExibirMensagem($"Já existe um registro com os mesmos dados informados.", TipoMensagem.Aviso)
            Return False
        End If

        Using dbConnection As New DbConnection()
            regsAfetados = dbConnection.ExecutarComando(TipoComandoSQL.UpdateOrInsert, "chamados", parametros)
        End Using

        Return (regsAfetados > 0)
    End Function

    Public Shared Function ExcluirChamado(ByVal idChamado As Integer) As Boolean
        Dim regsAfetados As Integer = -1
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@ID", idChamado)
        }

        Using dbConnection As New DbConnection()
            regsAfetados = dbConnection.ExecutarComando(TipoComandoSQL.Delete, "chamados", parametros)
        End Using

        Return (regsAfetados > 0)
    End Function

    Private Shared Function VerificarChamadosDuplicados(assunto As String, solicitante As String, departamento As Integer) As Boolean
        Dim resultado As Integer
        Dim parametros As New List(Of SQLiteParameter) From {
            New SQLiteParameter("@Assunto", assunto),
            New SQLiteParameter("@Solicitante", solicitante),
            New SQLiteParameter("@Departamento", departamento)
        }

        Using dbConnection As New DbConnection()
            resultado = dbConnection.ExecutarComando(TipoComandoSQL.Count, "chamados", parametros)
        End Using

        Return resultado > 0
    End Function
End Class
