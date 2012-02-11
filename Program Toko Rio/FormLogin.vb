Imports MySql.Data.MySqlClient

Public Class FormLogin

    Private Sub createDB()
        Try

            Dim sql As String = " create database  " & dbName
            execute_update(sql)

        Catch ex As Exception
        End Try
    End Sub
    Private Sub importData()
        Try
            Shell("cmd /c IMPORT DATA.bat")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub addModal()
        Dim sql As String = "alter table trdetailfaktur add HargaBeli int default 0"
        Try
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub addField(ByVal sql)
        Try
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub changeDataType(ByVal sql)
        Try
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub addHargaRetur()
        Dim sql As String = "alter table trdetailReturPenjualan add HargaRetur int default 0"
        Try
            execute_update(sql)

        Catch ex As Exception

        End Try

        sql = "alter table trdetailReturPembelian add HargaRetur int default 0"
        Try
            execute_update(sql)
        Catch ex As Exception

        End Try
    End Sub

    Function unvisible_all(ByVal status)
        Try
            Dim var_admin = True
            If status = 0 Then
                var_admin = False
            End If
            FormMain.menuPO.Visible = var_admin
            FormMain.menuPO2.Visible = var_admin
            FormMain.menuPB.Visible = var_admin
            FormMain.menuPB2.Visible = var_admin
            FormMain.menuReturPO.Visible = var_admin
            FormMain.menuReturPO2.Visible = var_admin
            FormMain.menuPaymentPO.Visible = var_admin
            FormMain.menuPaymentPO2.Visible = var_admin
            FormMain.menuSO.Visible = var_admin

            FormMain.menuFaktur.Visible = var_admin
            FormMain.menuFaktur2.Visible = var_admin
            FormMain.menuReturSO.Visible = var_admin
            FormMain.menuReturSO2.Visible = var_admin
            FormMain.menuPaymentSO.Visible = var_admin
            FormMain.menuPaymentSO2.Visible = var_admin
            FormMain.menuBonus.Visible = var_admin
            FormMain.menuAdj.Visible = var_admin
            
            FormMain.menuReportLabaRugi.Visible = var_admin
            FormMain.menuSettingBackup.Visible = var_admin
            FormMain.menuSettingRegister.Visible = var_admin
            FormMain.menuReportPO.Visible = var_admin
            FormMain.menuReportPB.Visible = var_admin
            FormMain.menuReportRetur.Visible = var_admin
            FormMain.menuReportSO.Visible = var_admin
            FormMain.menuReportFaktur.Visible = var_admin
            FormMain.menuReportReturSO.Visible = var_admin
            FormMain.menuReportDaftarPenagihan.Visible = var_admin
            FormMain.menuReportPenagihan.Visible = var_admin
            FormMain.menuReportProd.Visible = var_admin
            FormMain.menuReportTrans.Visible = var_admin
            FormMain.menuUtangPiutang.Visible = var_admin
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
        Return True
    End Function

    Private Sub FormLogin_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtID.Text = "sa"
        txtPass.Text = "a"
        txtID.Select()
        Me.txtID.Focus()
    End Sub
    Private Function checkEmpty() As Boolean
        If txtID.Text = "" Then
            MsgBox("Username harus diisi", MsgBoxStyle.Critical, "Warning")
            txtID.Focus()
            Return False
        ElseIf txtPass.Text = "" Then
            MsgBox("Password harus diisi", MsgBoxStyle.Critical, "Warning")
            txtPass.Focus()
            Return False
        End If
        Return True
    End Function

    Sub get_status_access()
        Try
            Dim access = execute_reader("Select Username,Access From authorization " & _
                                        " Where Username = '" & txtID.Text & "'")
            Dim table(20)
            Dim table_access(1)
            Dim Access_name = ""
            Dim status_po = 0
            Dim status_so = 0
            While access.Read
                table = access.Item(1).ToString.Split(",")
                For i As Integer = 0 To table.Length - 1
                    table_access = table(i).ToString.Split(":")

                    If table_access(0) = "faktur" Then
                        FormMain.menuFaktur.Visible = True
                        FormMain.menuFaktur2.Visible = True
                        FormMain.menuReportFaktur.Visible = True
                        status_so = 1
                    ElseIf table_access(0) = "returJual" Then
                        FormMain.menuReturSO.Visible = True
                        FormMain.menuReturSO2.Visible = True
                        FormMain.menuReportReturSO.Visible = True
                        status_so = 1
                    ElseIf table_access(0) = "paymentJual" Then
                        FormMain.menuPaymentSO.Visible = True
                        FormMain.menuPaymentSO2.Visible = True
                        status_so = 1
                    ElseIf table_access(0) = "po" Then
                        FormMain.menuPO.Visible = True
                        FormMain.menuPO2.Visible = True
                        FormMain.menuReportPO.Visible = True
                        status_po = 1
                    ElseIf table_access(0) = "pb" Then
                        FormMain.menuPB.Visible = True
                        FormMain.menuPB2.Visible = True
                        FormMain.menuReportPB.Visible = True
                        status_po = 1
                    ElseIf table_access(0) = "returBeli" Then
                        FormMain.menuReturPO.Visible = True
                        FormMain.menuReturPO2.Visible = True
                        FormMain.menuReportRetur.Visible = True
                        status_po = 1
                    ElseIf table_access(0) = "paymentBeli" Then
                        FormMain.menuPaymentPO.Visible = True
                        FormMain.menuPaymentPO2.Visible = True
                        status_po = 1
                    ElseIf table_access(0) = "bonus" Then
                        FormMain.menuBonus.Visible = True
                        FormMain.seperatorBonus.Visible = True
                    ElseIf table_access(0) = "adj" Then
                        FormMain.menuAdj.Visible = True
                    
                    ElseIf table_access(0) = "backup" Then
                        FormMain.menuSettingBackup.Visible = True
                    ElseIf table_access(0) = "regis" Then
                        FormMain.menuSettingRegister.Visible = True
                    ElseIf table_access(0) = "labarugi" Then
                        FormMain.menuReportLabaRugi.Visible = True
                    ElseIf table_access(0) = "utangPiutang" Then
                        FormMain.menuUtangPiutang.Visible = True
                    ElseIf table_access(0) = "penagihan" Then
                        FormMain.menuReportPenagihan.Visible = True
                    ElseIf table_access(0) = "daftarTagih" Then
                        FormMain.menuReportDaftarPenagihan.Visible = True
                    ElseIf table_access(0) = "reportProd" Then
                        FormMain.menuReportProd.Visible = True
                    ElseIf table_access(0) = "reportTrans" Then
                        FormMain.menuReportTrans.Visible = True
                    End If
                Next
            End While
            FormMain.btn_sales.Visible = True
            FormMain.ToolStripMenuItemSales.Visible = True
            FormMain.ToolStripMenuItemSales.Visible = True
            FormMain.btn_purchase.Visible = True
            FormMain.ToolStripMenuItemPurchase.Visible = True
            FormMain.ToolStripMenuItemPurchase.Visible = True
            FormMain.seperatorPO.Visible = True
            FormMain.seperatorSO.Visible = True
            If status_so = 0 Then
                FormMain.btn_sales.Visible = False
                FormMain.ToolStripMenuItemSales.Visible = False
                FormMain.ToolStripMenuItemSales.Visible = False
                FormMain.seperatorSO.Visible = False
            End If
            If status_po = 0 Then
                FormMain.btn_purchase.Visible = False
                FormMain.ToolStripMenuItemPurchase.Visible = False
                FormMain.ToolStripMenuItemPurchase.Visible = False
                FormMain.seperatorPO.Visible = False
            End If
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Information)
        End Try
    End Sub

    Private Sub checkLogin()
        If checkEmpty() Then
            Try
                sql = " SELECT userid, username, PASSWORD, " & _
                      " NamaLengkap, LEVEL, Alamat, Keahlian, " & _
                      " Created, LastUpdate, IsAktif " & _
                      " FROM msuser " & _
                      " WHERE username='" & txtID.Text & "' " & _
                      " AND password='" & EnkripsiCode(txtPass.Text & "hr") & "'"
                Dim reader = execute_reader(sql)
                If reader.Read Then
                    kdKaryawan = reader("userid")
                    namaKaryawan = reader("NamaLengkap")
                    lvlKaryawan = reader("LEVEL").ToString
                    Dim admin = reader("LEVEL")
                    If admin = "admin" Then
                        FormMain.UserAuthorizationToolStripMenuItem.Visible = True
                    Else
                        FormMain.UserAuthorizationToolStripMenuItem.Visible = False
                    End If
                    MsgBox("Selamat datang " & namaKaryawan, MsgBoxStyle.Information, "Welcome Message")
                    reader.Close()
                    FormMain.Show()
                    Me.Hide()
                Else
                    reader.Close()
                    MsgBox("Username atau Password tidak valid !", MsgBoxStyle.Critical, "Warning")
                    txtPass.Text = ""
                    txtID.Text = ""
                    txtID.Select()
                    txtID.Focus()
                End If
            Catch ex As Exception
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
        End If
    End Sub
    Private Sub txtPass_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPass.KeyPress
        If AscW(e.KeyChar) = 13 Then
            checkLogin()
        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            checkLogin()
        Catch ex As Exception
            MsgBox(ex, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        If MsgBox("Anda yakin ingin keluar dari aplikasi?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            End
        End If
    End Sub
  
    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtID.KeyPress
        If AscW(e.KeyChar) = 13 Then
            checkLogin()
        End If
    End Sub
End Class
