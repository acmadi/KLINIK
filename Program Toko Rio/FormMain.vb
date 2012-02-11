Public Class FormMain

    Private Sub MerkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FormMsMerk.Show()
    End Sub

    Private Sub MerkTipeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FormMsMerkTipe.Show()
    End Sub

    Private Sub KategoriToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FormMsKategori.Show()
    End Sub

    Private Sub SubKategoriToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FormMsKategoriSub.Show()
    End Sub

    Private Sub PropinsiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsProvinsi")
    End Sub

    Private Sub JenisFisingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FormMsJenisFishing.Show()
    End Sub

    Private Sub EkspedisiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsEkspedisi")
    End Sub

    Private Sub SalesToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsSales")
    End Sub

    Private Sub SupplierToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SupplierToolStripMenuItem.Click
        open_page("FormMsSupplier")
    End Sub

    Private Sub TokoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FarmasiToolStripMenuItem.Click
        open_page("FormMsFarmasi")
    End Sub

    Private Sub btn_logout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_logout.Click
        Me.Hide()

        FormLogin.txtID.Text = ""
        FormLogin.txtPass.Text = ""
        FormLogin.txtID.Select()
        FormLogin.txtID.Focus()
        FormLogin.Show()
    End Sub

    Private Sub BarangJadiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub KomposisiBarangJadiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub TipeToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsJenisFishing")
    End Sub

    Private Sub AreaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsArea")
    End Sub

    Private Sub TipeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsBahanMentahTipe")
    End Sub

    Private Sub SubKategoriToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsBahanMentahSubKategori")
    End Sub

    Private Sub BahanMentahToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub MerkToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsMerk")
    End Sub

    Private Sub MerkTipeToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsMerkTipe")
    End Sub

    Private Sub KategoriToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsKategori")
    End Sub

    Private Sub SubKategoriToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsKategoriSub")
    End Sub

    Private Sub JenisFisingToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
    End Sub

    Private Sub FormMain_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        End
    End Sub

    Private Sub FormMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub BarangJadiToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsBarang")
    End Sub

    Private Sub PenyesuaiaStokToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuAdj.Click
        open_page("FormBarangAdjusment")
    End Sub

    Private Sub MerkToolStripMenuItem_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsMerk")
    End Sub

    

    Private Sub BahanMentahToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsBahanMentah")
    End Sub

    Private Sub SalesOrderToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormSO")
    End Sub

    Private Sub FisingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormMsFising")
    End Sub
 
    Private Sub SalesOrderToolStripMenuItem1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSO.Click
        open_page("FormSO")
    End Sub

    Private Sub SalesInvoiceToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuFaktur.Click
        open_page("FormFaktur")
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReturSO.Click
        open_page("FormRetur")
    End Sub

    

    Private Sub SalesReturnToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuFaktur2.Click
        open_page("FormFaktur")
    End Sub

    Private Sub SalesPaymentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReturSO2.Click
        open_page("FormRetur")
    End Sub

    Private Sub SalesPaymentToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPaymentSO.Click
        open_page("FormSalesPayment")
    End Sub

    Private Sub PurchaseOrderToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPO2.Click

    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPB2.Click

    End Sub

    Private Sub PurchaseOrderToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPO.Click
    End Sub

    Private Sub ProfileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSettingProfile.Click
        open_page("FormMsProfile")
    End Sub

    Private Sub RegisterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSettingRegister.Click
        open_page("FormMsUser")
    End Sub

    Private Sub PenagihanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportPenagihan.Click
        open_page("FormLapPenagihan")
    End Sub

    Private Sub PenjualanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportFaktur.Click
        open_page("FormLapPenjualan")
    End Sub

   
    Private Sub GoodReceiveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormTrPenerimaanBarangM")
    End Sub

     

    Private Sub ToolStripMenuItemSales_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPenjualan.Click

    End Sub

    Private Sub ToolStripMenuItemSalesOrder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportSO.Click
        open_page("FormLapSO")
    End Sub

    Private Sub PurchaseReturnToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPB.Click

    End Sub

    Private Sub LabaRugiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportLabaRugi.Click
        open_page("FormLapLabaRugi")
    End Sub

    Private Sub BackupDataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSettingBackup.Click

        open_page("FormBackUpDB")
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReturPO.Click

    End Sub

    Private Sub ReturToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportReturSO.Click
        open_page("FormLapReturJual")
    End Sub

    Private Sub ProduksiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportProd.Click
        open_page("FormLapProduksi")
    End Sub

    Private Sub TransferStokToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportTrans.Click
        open_page("FormLapTransferStock")
    End Sub

    
 

    Private Sub SalesPaymentToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPaymentSO2.Click
        open_page("FormSalesPayment")
    End Sub

    Private Sub ReturToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        open_page("FormTrPenerimaanBarangM")
    End Sub

     

    Private Sub ReturToolStripMenuItem1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportRetur.Click

    End Sub

    Private Sub PurchasePaymentToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPaymentPO.Click

    End Sub

    Private Sub UtangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UtangToolStripMenuItem.Click
        open_page("FormLapPembayaranUtang")
    End Sub

    Private Sub PurchasePaymentToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPaymentPO2.Click

    End Sub

    Private Sub PiutangToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PiutangToolStripMenuItem.Click
        open_page("FormLapPembayaranPiutang")
    End Sub

    Private Sub DaftarPenagihanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuReportDaftarPenagihan.Click
        open_page("FormLapPenagihan2")
    End Sub

    Private Sub BahanJadiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanJadiToolStripMenuItem.Click
        open_page("FormTrPO")
    End Sub

    Private Sub BahanMentahToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem2.Click
        open_page("FormTrPOBahanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem1.Click
        open_page("FormTrPOFising")
    End Sub

    Private Sub BarangJadiToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarangJadiToolStripMenuItem1.Click
        open_page("FormTrPenerimaanBarangM")
    End Sub

    Private Sub BahanMentahToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem3.Click
        open_page("FormTrPenerimaanBahanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem2.Click
        open_page("FormTrPenerimaanFising")
    End Sub

    Private Sub ToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem5.Click
        open_page("FormTrReturPembelian")
    End Sub

    Private Sub ToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem6.Click
        open_page("FormTrReturPembelianBahanMentah")
    End Sub

    Private Sub ToolStripMenuItem7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem7.Click
        open_page("FormTrReturPembelianFising")
    End Sub

    Private Sub ToolStripMenuItem8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem8.Click
        open_page("FormPurchasePayment")
    End Sub

    Private Sub ToolStripMenuItem9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem9.Click
        open_page("FormPurchasePaymentBahanMentah")
    End Sub

    Private Sub ToolStripMenuItem10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem10.Click
        open_page("FormPurchasePaymentFising")
    End Sub

    Private Sub UserAuthorizationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserAuthorizationToolStripMenuItem.Click
        open_page("Authorization")
    End Sub

    Private Sub menuBonus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuBonus.Click
        open_page("FormKomisiSales")
    End Sub

    Private Sub BahanJadiToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanJadiToolStripMenuItem1.Click
        open_page("FormLapPO")
    End Sub

    Private Sub BahanMentahToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem4.Click
        open_page("FormLapPOMentah")
    End Sub

    Private Sub FisingToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem3.Click
        open_page("FormLapPOFising")
    End Sub

    Private Sub BarangJadiToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarangJadiToolStripMenuItem2.Click
        open_page("FormLapPenerimaan")
    End Sub

    Private Sub BahanMentahToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem5.Click
        open_page("FormLapPenerimaanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem4.Click
        open_page("FormLapPenerimaanFising")
    End Sub

    Private Sub BarangJadiToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarangJadiToolStripMenuItem3.Click
        open_page("FormLapReturBeli")
    End Sub

    Private Sub BahanMentahToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem6.Click
        open_page("FormLapReturBeliMentah")
    End Sub

    Private Sub FisingToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem5.Click
        open_page("FormLapReturBeliFising")
    End Sub

    
    Private Sub BarangJadiToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarangJadiToolStripMenuItem4.Click
        open_page("FormTrPO")
    End Sub

    Private Sub BahanMentahToolStripMenuItem7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem7.Click
        open_page("FormTrPOBahanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem6.Click
        open_page("FormTrPOFising")
    End Sub

    Private Sub BarangJadiToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarangJadiToolStripMenuItem5.Click
        open_page("FormTrPenerimaanBarangM")
    End Sub

    Private Sub BahanMentahToolStripMenuItem8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem8.Click
        open_page("FormTrPenerimaanBahanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem7.Click
        open_page("FormTrPenerimaanFising")
    End Sub

    Private Sub BarangJadiToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarangJadiToolStripMenuItem6.Click
        open_page("FormTrReturPembelian")
    End Sub

    Private Sub BahanMentahToolStripMenuItem9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem9.Click
        open_page("FormTrReturPembelianBahanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem8.Click
        open_page("FormTrReturPembelianFising")
    End Sub

    Private Sub BahanJadiToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanJadiToolStripMenuItem2.Click
        open_page("FormPurchasePayment")
    End Sub

    Private Sub BahanMentahToolStripMenuItem10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BahanMentahToolStripMenuItem10.Click
        open_page("FormPurchasePaymentBahanMentah")
    End Sub

    Private Sub FisingToolStripMenuItem9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FisingToolStripMenuItem9.Click
        open_page("FormPurchasePaymentFising")
    End Sub

    Private Sub PasienToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasienToolStripMenuItem.Click
        open_page("FormMsPasien")
    End Sub

    Private Sub ToolStripMenuItem1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        open_page("FormMsDokter")
    End Sub

    Private Sub ObatToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ObatToolStripMenuItem.Click
        open_page("FormMsObat")
    End Sub

    Private Sub DistributorReagenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DistributorReagenToolStripMenuItem.Click
        open_page("FormMsDistributor")
    End Sub

    Private Sub ReagenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReagenToolStripMenuItem.Click
        open_page("FormMsReagen")
    End Sub

    Private Sub ElektromedikToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ElektromedikToolStripMenuItem.Click
        open_page("FormMsElektromedik")
    End Sub

    Private Sub JenisPemeriksaanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JenisPemeriksaanToolStripMenuItem.Click
        open_page("FormMsJenisPemeriksaan")
    End Sub
End Class