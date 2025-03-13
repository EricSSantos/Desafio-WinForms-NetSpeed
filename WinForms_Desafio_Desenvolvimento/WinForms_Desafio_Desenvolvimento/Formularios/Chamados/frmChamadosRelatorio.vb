Imports Microsoft.Reporting.WinForms

Public Class frmChamadosRelatorio
    Private Sub frmChamadosRelatorio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim chamados As List(Of Chamado) = ChamadosController.ListarChamados()
        Dim rds As New ReportDataSource("dsChamados", chamados)

        Me.ReportViewer1.LocalReport.DataSources.Add(rds)
        Me.ReportViewer1.SetDisplayMode(DisplayMode.PrintLayout)
        Me.ReportViewer1.ZoomMode = ZoomMode.PageWidth
        Me.ReportViewer1.RefreshReport()
    End Sub
End Class