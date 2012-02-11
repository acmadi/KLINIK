Public Class CRDaftarTagih

    Private Sub CrystalReportViewer1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CrystalReportViewer1.Load
        Dim rp As New CrystalReportDaftarTagih
        rp.SetParameterValue("tg1", CDate(tglMulai))
        rp.SetParameterValue("tg2", CDate(tglAkhir))
        rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        rp.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape
        CrystalReportViewer1.ReportSource = rp
        CrystalReportViewer1.PrintReport()
    End Sub
End Class