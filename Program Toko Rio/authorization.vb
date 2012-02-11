Public Class authorization

    Function unchecked_all(ByVal chk_status)

        Try
            Dim var_chk = True

            If chk_status = 0 Then
                var_chk = False
            End If

            chk_bonus_sales.Checked = var_chk
            chk_faktur.Checked = var_chk
            chk_laba_rugi.Checked = var_chk
            chk_penerimaan_barang.Checked = var_chk
            chk_penyesuaian_stok.Checked = var_chk
            chk_produksi.Checked = var_chk
            chk_purchase_order.Checked = var_chk
            chk_purchase_payment.Checked = var_chk
            chk_purchase_return.Checked = var_chk
            chk_sales_order.Checked = var_chk
            chk_sales_payment.Checked = var_chk
            chk_sales_return.Checked = var_chk
            chk_transfer_stok.Checked = var_chk
            chk_backup.Checked = var_chk
            chk_register.Checked = var_chk
            chk_report_daftar_tagih.Checked = var_chk
            chk_report_pembayaran.Checked = var_chk
            chk_report_penagihan.Checked = var_chk
            chk_report_produksi.Checked = var_chk
            chk_report_trans.Checked = var_chk
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return True
    End Function

    Function get_access()
        Try
            Dim access = execute_reader("Select Username,Access From authorization " _
                                        & " Where Username = '" & cmb_username.Text & "'")
            Dim table(20)
            Dim table_access(1)
            Dim Access_name = ""
            unchecked_all(0)
            While access.Read
                table = access.Item(1).ToString.Split(",")
                For i As Integer = 0 To table.Length - 1
                    table_access = table(i).ToString.Split(":")
                    If table_access(0) = "po" Then
                        chk_purchase_order.Checked = True
                        If table_access(1) = "1" Then
                            rdo_purchase_order_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_purchase_order_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_purchase_order_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_purchase_order_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_purchase_order_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "pb" Then
                        chk_penerimaan_barang.Checked = True
                        If table_access(1) = "1" Then
                            rdo_penerimaan_barang_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_penerimaan_barang_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_penerimaan_barang_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_penerimaan_barang_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_penerimaan_barang_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "returBeli" Then
                        chk_purchase_return.Checked = True
                        If table_access(1) = "1" Then
                            rdo_purchase_return_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_purchase_return_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_purchase_return_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_purchase_return_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_purchase_return_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "paymentBeli" Then
                        chk_purchase_payment.Checked = True
                        If table_access(1) = "1" Then
                            rdo_purchase_payment_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_purchase_payment_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_purchase_payment_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_purchase_payment_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_purchase_payment_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "so" Then
                        chk_sales_order.Checked = True
                        If table_access(1) = "1" Then
                            rdo_sales_order_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_sales_order_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_sales_order_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_sales_order_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_sales_order_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "faktur" Then
                        chk_faktur.Checked = True
                        If table_access(1) = "1" Then
                            rdo_faktur_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_faktur_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_faktur_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_faktur_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_faktur_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "returJual" Then
                        chk_sales_return.Checked = True
                        If table_access(1) = "1" Then
                            rdo_sales_return_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_sales_return_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_sales_return_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_sales_return_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_sales_return_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "paymentJual" Then
                        chk_sales_payment.Checked = True
                        If table_access(1) = "1" Then
                            rdo_sales_payment_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_sales_payment_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_sales_payment_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_sales_payment_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_sales_payment_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "bonus" Then
                        chk_bonus_sales.Checked = True
                        If table_access(1) = "1" Then
                            rdo_bonus_sales_view.Checked = True
                        End If
                    ElseIf table_access(0) = "adj" Then
                        chk_penyesuaian_stok.Checked = True
                        If table_access(1) = "1" Then
                            rdo_penyesuaian_stok_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_penyesuaian_stok_insert.Checked = True
                        End If
                    ElseIf table_access(0) = "trans" Then
                        chk_transfer_stok.Checked = True
                        If table_access(1) = "1" Then
                            rbo_transfer_stok_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rbo_transfer_stok_insert.Checked = True
                        End If
                    ElseIf table_access(0) = "prod" Then
                        chk_produksi.Checked = True
                        If table_access(1) = "1" Then
                            rbo_produksi_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rbo_produksi_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rbo_produksi_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rbo_produksi_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rbo_produksi_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "labarugi" Then
                        chk_laba_rugi.Checked = True
                        If table_access(1) = "1" Then
                            rdo_laba_rugi_view.Checked = True
                        End If
                    ElseIf table_access(0) = "backup" Then
                        chk_backup.Checked = True
                        If table_access(1) = "1" Then
                            rdo_backup_view.Checked = True
                        End If
                    ElseIf table_access(0) = "regis" Then
                        chk_register.Checked = True
                        If table_access(1) = "1" Then
                            rdo_register_view.Checked = True
                        ElseIf table_access(1) = "2" Then
                            rdo_register_insert.Checked = True
                        ElseIf table_access(1) = "3" Then
                            rdo_register_update.Checked = True
                        ElseIf table_access(1) = "4" Then
                            rdo_register_ins_up.Checked = True
                        ElseIf table_access(1) = "5" Then
                            rdo_register_delete.Checked = True
                        End If
                    ElseIf table_access(0) = "utangPiutang" Then
                        chk_report_pembayaran.Checked = True
                        If table_access(1) = "1" Then
                            rdo_report_pembayaran.Checked = True
                        End If
                    ElseIf table_access(0) = "penagihan" Then
                        chk_report_penagihan.Checked = True
                        If table_access(1) = "1" Then
                            rdo_report_penagihan.Checked = True
                        End If
                    ElseIf table_access(0) = "daftarTagih" Then
                        chk_report_daftar_tagih.Checked = True
                        If table_access(1) = "1" Then
                            rdo_report_daftar_tagih.Checked = True
                        End If
                    ElseIf table_access(0) = "reportProd" Then
                        chk_report_produksi.Checked = True
                        If table_access(1) = "1" Then
                            rdo_report_produksi.Checked = True
                        End If
                    ElseIf table_access(0) = "reportTrans" Then
                        chk_report_trans.Checked = True
                        If table_access(1) = "1" Then
                            rdo_report_trans.Checked = True
                        End If
                    End If
                Next
            End While
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return True
    End Function

    Function get_access_name_for_insert_to_table()
        Dim access_name = ""
        Try
            If chk_purchase_order.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "po:"
                If rdo_purchase_order_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_purchase_order_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_purchase_order_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_purchase_order_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_purchase_order_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_penerimaan_barang.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "pb:"
                If rdo_penerimaan_barang_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_penerimaan_barang_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_penerimaan_barang_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_penerimaan_barang_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_penerimaan_barang_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_purchase_return.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "returBeli:"
                If rdo_purchase_return_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_purchase_return_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_purchase_return_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_purchase_return_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_purchase_return_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_purchase_payment.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "paymentBeli:"
                If rdo_purchase_payment_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_purchase_payment_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_purchase_payment_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_purchase_payment_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_purchase_payment_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_sales_order.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "so:"
                If rdo_sales_order_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_sales_order_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_sales_order_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_sales_order_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_sales_order_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_faktur.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "faktur:"
                If rdo_faktur_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_faktur_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_faktur_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_faktur_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_faktur_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_sales_return.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "returJual:"
                If rdo_sales_return_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_sales_return_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_sales_return_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_sales_return_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_sales_return_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_sales_payment.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "paymentJual:"
                If rdo_sales_payment_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_sales_payment_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_sales_payment_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_sales_payment_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_sales_payment_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_bonus_sales.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "bonus:"
                If rdo_bonus_sales_view.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_penyesuaian_stok.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "adj:"
                If rdo_penyesuaian_stok_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_penyesuaian_stok_insert.Checked = True Then
                    access_name += "2"
                End If
            End If
            If chk_transfer_stok.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "trans:"
                If rbo_transfer_stok_view.Checked = True Then
                    access_name += "1"
                ElseIf rbo_transfer_stok_insert.Checked = True Then
                    access_name += "2"
                End If
            End If
            If chk_produksi.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "prod:"
                If rbo_produksi_view.Checked = True Then
                    access_name += "1"
                ElseIf rbo_produksi_insert.Checked = True Then
                    access_name += "2"
                ElseIf rbo_produksi_update.Checked = True Then
                    access_name += "3"
                ElseIf rbo_produksi_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rbo_produksi_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_laba_rugi.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "labarugi:"
                If rdo_laba_rugi_view.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_backup.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "backup:"
                If rdo_backup_view.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_register.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "regis:"
                If rdo_register_view.Checked = True Then
                    access_name += "1"
                ElseIf rdo_register_insert.Checked = True Then
                    access_name += "2"
                ElseIf rdo_register_update.Checked = True Then
                    access_name += "3"
                ElseIf rdo_register_ins_up.Checked = True Then
                    access_name += "4"
                ElseIf rdo_register_delete.Checked = True Then
                    access_name += "5"
                End If
            End If
            If chk_report_pembayaran.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "utangPiutang:"
                If rdo_report_pembayaran.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_report_penagihan.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "penagihan:"
                If rdo_report_penagihan.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_report_daftar_tagih.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "daftarTagih:"
                If rdo_report_daftar_tagih.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_report_produksi.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "reportProd:"
                If rdo_report_produksi.Checked = True Then
                    access_name += "1"
                End If
            End If
            If chk_report_trans.Checked = True Then
                If access_name <> "" Then
                    access_name += ","
                End If
                access_name += "reportTrans:"
                If rdo_report_trans.Checked = True Then
                    access_name += "1"
                End If
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return access_name
    End Function

    Public Function refresh_data()
        Try
            Dim username = execute_reader("Select username From msuser " _
                                          & " Order By username")

            cmb_username.Items.Clear()
            While username.Read
                cmb_username.Items.Add(username("username"))
            End While
            cmb_username.SelectedIndex = 0
            get_access()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return True
    End Function

    Private Sub authorization_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        refresh_data()
    End Sub

    Private Sub cmb_username_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmb_username.SelectedIndexChanged
        get_access()
    End Sub

    Private Sub chk_purchase_order_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_purchase_order.CheckedChanged
        Try
            If chk_purchase_order.Checked = True Then
                grb_purchase_order.Visible = True
            Else
                grb_purchase_order.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_save.Click
        Try
            Dim access_name = get_access_name_for_insert_to_table()
            execute_update("Delete from authorization Where Username = '" & cmb_username.Text & "'")
            execute_update("insert into authorization(Username, Access) values " _
                          & " ('" & cmb_username.Text & "','" & access_name & "')")
            MsgBox("Access has been successfully saved.", MsgBoxStyle.Information, "Success")
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub chk_penerimaan_barang_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_penerimaan_barang.CheckedChanged
        Try
            If chk_penerimaan_barang.Checked = True Then
                grb_penerimaan_barang.Visible = True
            Else
                grb_penerimaan_barang.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_purchase_return_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_purchase_return.CheckedChanged
        Try
            If chk_purchase_return.Checked = True Then
                grb_purchase_return.Visible = True
            Else
                grb_purchase_return.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_purchase_payment_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_purchase_payment.CheckedChanged
        Try
            If chk_purchase_payment.Checked = True Then
                grb_purchase_payment.Visible = True
            Else
                grb_purchase_payment.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_sales_order_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_sales_order.CheckedChanged
        Try
            If chk_sales_order.Checked = True Then
                grb_sales_order.Visible = True
            Else
                grb_sales_order.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_faktur_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_faktur.CheckedChanged
        Try
            If chk_faktur.Checked = True Then
                grb_faktur.Visible = True
            Else
                grb_faktur.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_sales_return_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_sales_return.CheckedChanged
        Try
            If chk_sales_return.Checked = True Then
                grb_sales_return.Visible = True
            Else
                grb_sales_return.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_sales_payment_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_sales_payment.CheckedChanged
        Try
            If chk_sales_payment.Checked = True Then
                grb_sales_payment.Visible = True
            Else
                grb_sales_payment.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_bonus_sales_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_bonus_sales.CheckedChanged
        Try
            If chk_bonus_sales.Checked = True Then
                grb_bonus_sales.Visible = True
            Else
                grb_bonus_sales.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_penyesuaian_stok_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_penyesuaian_stok.CheckedChanged
        Try
            If chk_penyesuaian_stok.Checked = True Then
                grb_penyesuaian_stok.Visible = True
            Else
                grb_penyesuaian_stok.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_transfer_stok_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_transfer_stok.CheckedChanged
        Try
            If chk_transfer_stok.Checked = True Then
                grb_transfer_stok.Visible = True
            Else
                grb_transfer_stok.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_produksi_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_produksi.CheckedChanged
        Try
            If chk_produksi.Checked = True Then
                grb_produksi.Visible = True
            Else
                grb_produksi.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_laba_rugi_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_laba_rugi.CheckedChanged
        Try
            If chk_laba_rugi.Checked = True Then
                grb_laba_rugi.Visible = True
            Else
                grb_laba_rugi.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_all_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_all.CheckedChanged
        Try
            If chk_all.Checked = True Then
                unchecked_all(1)
            Else
                unchecked_all(0)
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_backup_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_backup.CheckedChanged
        Try
            If chk_backup.Checked = True Then
                grb_backup.Visible = True
            Else
                grb_backup.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_register_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_register.CheckedChanged
        Try
            If chk_register.Checked = True Then
                grb_register.Visible = True
            Else
                grb_register.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_report_pembayaran_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_report_pembayaran.CheckedChanged
        Try
            If chk_report_pembayaran.Checked = True Then
                grd_report_pembayaran.Visible = True
            Else
                grd_report_pembayaran.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_report_penagihan_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_report_penagihan.CheckedChanged
        Try
            If chk_report_penagihan.Checked = True Then
                grd_report_penagihan.Visible = True
            Else
                grd_report_penagihan.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_report_daftar_tagih_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_report_daftar_tagih.CheckedChanged
        Try
            If chk_report_daftar_tagih.Checked = True Then
                grd_report_daftar_tagih.Visible = True
            Else
                grd_report_daftar_tagih.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_report_produksi_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_report_produksi.CheckedChanged
        Try
            If chk_report_produksi.Checked = True Then
                grd_report_produksi.Visible = True
            Else
                grd_report_produksi.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub chk_report_trans_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_report_trans.CheckedChanged
        Try
            If chk_report_trans.Checked = True Then
                grd_report_trans.Visible = True
            Else
                grd_report_trans.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub
End Class