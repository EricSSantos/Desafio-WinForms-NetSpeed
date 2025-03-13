# **Desafio NetSpeed - Sistema de Chamados e Departamentos**

### **1. Refatoração da Lógica de CRUD**: 
   - A lógica de manipulação de dados, que antes estava centralizada em uma única camada, foi dividida em duas classes:

     - **`ChamadosController`**
       - **Listar**: Retorna todos os chamados cadastrados no sistema.
       - **Obter**: Recupera um chamado específico pelo ID.
       - **Gravar**: Insere ou atualiza um chamado, com verificação de duplicidade antes de qualquer operação.
       - **Excluir**: Remove um chamado pelo ID.

     - **`DepartamentosController`**
        - **Listar**: Recupera todos os departamentos cadastrados no sistema.
        - **Obter**: Retorna um departamento específico pelo ID.
        - **Gravar**: Insere ou atualiza um departamento, com verificação de duplicidade antes de qualquer operação.
        - **Excluir**: Exclui um departamento do banco de dados.

     A separação das responsabilidades em **ChamadosController** e **DepartamentosController** melhora a modularidade do sistema, facilitando a manutenção e a adição de novos recursos.

### **2. Classe `DbConnection`**: 
   - A classe DbConnection centraliza o gerenciamento de conexões com o banco de dados SQLite, cuidando da abertura e do fechamento da conexão. Ela abstrai a execução de comandos SQL (como INSERT, UPDATE, DELETE e SELECT), garantindo que as operações sejam realizadas de maneira eficiente. Além disso, oferece suporte a transações, permitindo operações atômicas com commit e rollback, garantindo a integridade dos dados em caso de falhas.

### **Documentação dos Métodos da Classe DbConnection**

#### **1. ExecutarComando**:
   - **Descrição**: Executa um comando SQL baseado no tipo de operação especificado (COUNT, INSERT, UPDATE, DELETE ou UPDATE_OR_INSERT). Essa função centraliza as operações SQL mais comuns e ajuda a evitar a repetição do SQL completo.
   - **Parâmetros**:
     - `tipoOperacao` (TipoComandoSQL): Tipo de operação (COUNT, INSERT, UPDATE, DELETE ou UPDATE_OR_INSERT).
     - `tabela` (String): Nome da tabela para a operação.
     - `parametros` (List(Of SQLiteParameter)): Lista de parâmetros para a consulta SQL.
   - **Retorno**: Número de registros afetados pela operação.
   - **Exemplo de Uso**:
     ```vb
     Dim resultado As Integer = dbConnection.ExecutarComando(TipoComandoSQL.Insert, "departamentos", parametros)
     ```

#### **2. ExecutarConsulta**:
   - **Descrição**: Executa uma consulta SQL e retorna um `SQLiteDataReader` com os resultados. É útil para consultas que retornam múltiplos registros.
   - **Parâmetros**:
     - `sql` (String): Consulta SQL a ser executada.
     - `parametros` (Optional List(Of SQLiteParameter)): Lista de parâmetros (opcional).
   - **Retorno**: Um `SQLiteDataReader` contendo os resultados da consulta.
   - **Exemplo de Uso**:
     ```vb
     Dim reader As SQLiteDataReader = dbConnection.ExecutarConsulta("SELECT * FROM departamentos")
     While reader.Read()
         ' Processa cada linha de dados
     End While
     ```

#### **3. ExecutarScalar**:
   - **Descrição**: Executa uma consulta SQL que retorna um único valor (como `COUNT`, `SUM` ou qualquer outro valor escalar).
   - **Parâmetros**:
     - `sql` (String): Consulta SQL a ser executada.
     - `parametros` (Optional List(Of SQLiteParameter)): Lista de parâmetros (opcional).
   - **Retorno**: Um objeto contendo o valor escalar retornado pela consulta SQL.
   - **Exemplo de Uso**:
     ```vb
     Dim totalDepartamentos As Integer = dbConnection.ExecutarScalar("SELECT COUNT(*) FROM departamentos")
     ```
