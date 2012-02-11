Public Class CRLaporan
    Private Sub CrystalReportViewer1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CrystalReportViewer1.Load
        Dim rp = Nothing
        Dim no As String = idPrint
        'MsgBox(tglMulai)
        If flagLaporan = "lap_penagihan" Then
            If jenisReport = "area" Then
                rp = New CrystalReportLapPenagihanArea
                rp.SetParameterValue("area", areaReport)
                Me.Text = "Laporan Penagihan Wilayah " & areaReport
            Else
                rp = New CrystalReportLapPenagihanToko
                rp.SetParameterValue("toko", areaReport)
                Me.Text = "Laporan Penagihan " & areaReport
            End If
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "faktur" Then
            rp = New CrystalReportFaktur
            rp.SetParameterValue("kd", no)
            Me.Text = "Faktur " & no
        ElseIf flagLaporan = "retur_jual" Then
            rp = New CrystalReportReturJual
            rp.SetParameterValue("kd", no)
            Me.Text = "Retur Penjualan " & no
        ElseIf flagLaporan = "po" Then
            rp = New CrystalReportPO
            rp.SetParameterValue("kd", no)
            Me.Text = "Purchase Order " & no
        ElseIf flagLaporan = "pb" Then
            rp = New CrystalReportPO
            rp.SetParameterValue("kd", no)
            Me.Text = "Penerimaan Barang " & no
        ElseIf flagLaporan = "lap_po" Then
            rp = New CrystalReportLapPO
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Pemesanan Barang periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_po_mentah" Then
            rp = New CrystalReportLapPOMentah
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Pemesanan Bahan Mentah periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_po_fising" Then
            rp = New CrystalReportLapPOFising
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Pemesanan Barang Mentah periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_pb" Then
            rp = New CrystalReportLapPB
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Penerimaan Barang periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_pb_mentah" Then
            rp = New CrystalReportLapPBMentah
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Penerimaan Bahan Mentah periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_pb_fising" Then
            rp = New CrystalReportLapPBFising
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Penerimaan Fising " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_so" Then
            rp = New CrystalReportLapSO
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Sales Order periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_jual" Then
            If jenisReport = "area" Then
                rp = New CrystalReportLapPenjualanArea
                rp.SetParameterValue("area", areaReport)
                Me.Text = "Laporan Penjualan Wilayah " & areaReport

            Else
                rp = New CrystalReportLapPenjualanToko
                rp.SetParameterValue("toko", areaReport)
                Me.Text = "Laporan Penjualan " & areaReport
            End If
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_retur_jual" Then
            rp = New CrystalReportLapReturJualNew
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Retur Penjualan periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_retur_beli" Then
            rp = New CrystalReportLapreturBeli
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Retur Pembelian periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_retur_beli_mentah" Then
            rp = New CrystalReportLapReturBeliMentah
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Retur Pembelian Bahan Mentah periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_retur_beli_fising" Then
            rp = New CrystalReportLapReturBeliFising
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Retur Pembelian Fising periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_pembayaran_utang" Then
            rp = New CrystalReportLapPembayaranUtang
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Pembayaran Utang periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_pembayaran_piutang" Then
            rp = New CrystalReportLapPembayaranPiutang
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Pembayaran Piutang periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_produksi" Then
            rp = New CrystalReportLapProduksi
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Produksi periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_transfer_stok" Then
            rp = New CrystalReportLapTransferStok
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Transfer Stok periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
        ElseIf flagLaporan = "lap_komisi_sales" Then
            rp = New CrystalReportLapKomisiSalesNew
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Komisi Sales periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4
            rp.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape
        ElseIf flagLaporan = "lap_laba_rugi" Then
            rp = New CrystalReportLapLR
            rp.SetParameterValue("tg1", CDate(tglMulai))
            rp.SetParameterValue("tg2", CDate(tglAkhir))
            Me.Text = "Laporan Laba Rugi periode " & tglMulai & "-" & tglAkhir
            rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4

        End If
        'rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize
        CrystalReportViewer1.Zoom(100)
        CrystalReportViewer1.ReportSource = rp
    End Sub
End Class