Imports System.Data.SqlClient

Public Class FormTRProduksiBarangJadi
    Dim query As String = ""
    Dim PK As String = ""
    Dim tabelH As String = ""
    Dim tabelD As String = ""
    Dim tab As String
    Dim SubKategoriID(2) As String
    Dim barangID(2) As String
    Dim SubKategoriID2(2) As String
    Dim barangID2(2) As String
    Dim is_access As Integer

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub
    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub setCode() 'As String
        Dim code As String = "KP"
        Dim angka As Integer
        Dim kode As String = ""
        Dim temp As String
        Dim bulan As Integer = CInt(Today.Month.ToString)
        code += Today.Year.ToString.Substring(2, 2)
        If bulan < 10 Then
            code += "0" + bulan.ToString
        Else
            code += bulan.ToString
        End If

        'generate code
        query = " select KdProduksi from  TrProduksi order by KdProduksi  desc"
        Dim reader = execute_reader(query)
        If reader.Read() Then
            kode = reader(0)
        Else
            reader.Close()
            query = " select KdProduksi from  TrProduksi_T  order by KdProduksi  desc limit 1"
            Dim reader2 = execute_reader(query)
            If reader2.Read() Then
                kode = reader2(0)
            Else
                kode = ""
            End If
            reader2.Close()
        End If
        reader.Close()
        reader = Nothing

        If kode <> "" Then
            temp = kode.Substring(0, 6)
            If temp = code Then
                angka = CInt(kode.Substring(6, 4))
            Else
                angka = 0
            End If

        Else
            angka = 0
        End If


        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka)
        txtID.Text = code
        'Return code
    End Sub
 
    Private Sub setCmbTypeFising()
        cmbTypeFising.Items.Clear()
        If Trim(cmbJenisFising.Text) <> "" Then
            Dim reader = execute_reader("Select distinct(TypeFising) from msfising where jenisfising='" & Trim(cmbJenisFising.Text) & "' order by typefising ")
            Dim varT As String = ""
            Do While reader.Read
                cmbTypeFising.Items.Add(reader(0))
            Loop
            reader.Close()
            If cmbTypeFising.Items.Count > 0 Then
                cmbTypeFising.SelectedIndex = 0
            End If
            reader.Close()
        End If
    End Sub

    Private Sub setCmbJenisFising()
        cmbJenisFising.Items.Clear()

        Dim reader = execute_reader("Select distinct(jenisFising) from msfising order by jenisfising ")
        Dim varT As String = ""
        Do While reader.Read
            cmbJenisFising.Items.Add(reader(0))
        Loop
        reader.Close()
        If cmbJenisFising.Items.Count > 0 Then
            cmbJenisFising.SelectedIndex = 0
        End If
        reader.Close()

    End Sub

    Private Sub add()
       
        tab = "TrProduksi"
        txtQty.Text = ""
        setCode()
        cmbTypeFising.Enabled = False
        txtPartNo.Text = ""
        KdFising.Text = ""
        setCmbJenisFising()
    End Sub
    Private Sub FormProduksiBarangJadi_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("prod")
        If is_access = 2 Then
            btnAdd.Enabled = True
            btnProd.Enabled = True
        End If
        add()
    End Sub

    Private Sub cmbSubKategori_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'SubKategoriID = cmbSubKategori.Text.ToString.Split(" - ")
        'cmbNamaBarang.Items.Clear()
        'Dim reader = execute_reader("Select kdbarang,namabarang from msbarang where kdsub='" & SubKategoriID(0) & "' order by namabarang asc")
        'Do While reader.Read
        '    cmbNamaBarang.Items.Add(reader(0) & " - " & reader(1))
        'Loop
        'reader.Close()
        'If cmbNamaBarang.Items.Count > 0 Then cmbNamaBarang.SelectedIndex = 0
    End Sub

    Private Sub cmbNamaBarang_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'barangID = cmbNamaBarang.Text.ToString.Split(" - ")
        'cmbDari.Text = barangID(0)

        'sql = " Select StockAkhir from BarangHistory where isActive = 1 And kdBarang = '" & cmbDari.Text & "' order by KdBarangHistory desc limit 1"
        'Dim reader = execute_reader(sql)
        'If reader.Read Then
        '    lblStock.Text = reader(0)
        'End If
        'reader.Close()
        'reader = Nothing
    End Sub

    Private Sub cmbJenisFising_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbJenisFising.SelectedIndexChanged
        cmbTypeFising.Enabled = True
        setCmbTypeFising()
    End Sub

    Private Sub cmbTypeFising_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbTypeFising.SelectedIndexChanged
        sql = " Select KdFising,Kdbarang, ifnull(( " & _
              " Select StockAkhir " & _
              " from FisingHistory " & _
              " where isActive = 1 " & _
              " And KdFising = msfising.KdFising " & _
              " order by KdFisingHistory desc limit 1  " & _
              " ),0) Stock " & _
              " from msfising " & _
              " where jenisfising='" & Trim(cmbJenisFising.Text) & "' " & _
              " And typefising = '" & Trim(cmbTypeFising.Text) & "' "
        Dim reader = execute_reader(sql)
        If reader.Read Then
            KdFising.Text = reader(0)
            txtPartNo.Text = reader(1)
            lblStockFising.Text = reader("Stock")
        Else
            KdFising.Text = ""
            txtPartNo.Text = ""
            lblStockFising.Text = 0
        End If
        reader.Close()
        reader = Nothing
        If txtPartNo.Text <> "" Then
            sql = " select   KdIdentifikasi 'No. Identifikasi', " & _
             " NamaBarang Nama,MsMerk.Merk, " & _
             " MsKategori.KdKategori,Kategori Kategori, " & _
             " MsSubkategori.KdSub,SubKategori 'Sub Kategori', " & _
             " IfNull(( Select IfNull(sum(Qty),0) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
             " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
             " Where KdBarang = MsBarangList.KdBarang " & _
             " And StatusSo = 3 " & _
             " Group By KdBarang ),0) As Stock From MsBarangList " & _
             " Where KdBarang = b.KdBarang And StatusBarangList = 0 " & _
             " Group By KdBarang ),0) Stock " & _
             " from msbarang b  " & _
             " Join Mskategori On Mskategori.KdKategori = b.KdKategori" & _
             " Join MsSubkategori On MsSubkategori.KdSub = b.KdSub" & _
             " Join MsSupplier On MsSupplier.KdSupplier = b.KdSupplier" & _
             " Join MsMerk On MsMerk.KdMerk = b.KdMerk where kdbarang='" & txtPartNo.Text & "' "
            reader = execute_reader(sql)
            If reader.Read Then
                lblMerk.Text = reader("Merk")
                lblNamabarang.Text = reader("Nama")
                lblKategori.Text = reader("Kategori")
                lblSub.Text = reader("Sub Kategori")
                lblStock.Text = reader("Stock")
            End If
            reader.close()
            reader = Nothing
        End If
        If KdFising.Text <> "" Then
            sql = " select kdBahanMentah Kode,NamaBahanMentah Nama,NamaBahanMentahTipe Tipe," & _
                  " BahanMentahSubKategori `Sub Kategori`, ifnull((  Select StockAkhir from BahanMentahHistory  Where KdBahanMentah =  a.KdBahanMentah  order by KdHistory desc limit 1 ),0) Stock " & _
                  " from msbahanmentah a " & _
                  " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe = a.KdBahanMentahTipe " & _
                  " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = a.KdBahanMentahSubKategori " & _
                  " where kdBahanMentah  in (select kdbahanmentah from msfising_detail where kdfising = '" & Trim(KdFising.Text) & "') "



            DataGridView3.DataSource = execute_datatable(sql)
        End If
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If AscW(e.KeyChar) = 13 Then
            Button1_Click(Nothing, Nothing)
        End If
        If AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtQty_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtQty.TextChanged

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProd.Click
        If txtPartNo.Text = "" Then
            msgInfo("Part No masih kosong.Barang belum dipilih")
            '            cmbDari.Focus()
        ElseIf cmbJenisFising.Text = "" Then
            msgInfo("Jenis fising harus dipilih")
            cmbJenisFising.Focus()
        ElseIf cmbTypeFising.Text = "" Then
            msgInfo("Type fising harus dipilih")
            cmbTypeFising.Focus()
        ElseIf KdFising.Text = "" Then
            msgInfo("No Fising masih kosong.Fising belum dipilih")
        ElseIf txtQty.Text = "" Then
            msgInfo("Quantity harus diisi")
            txtQty.Focus()
        ElseIf lblStockFising.Text = 0 Then
            msgInfo("Tidak ada stock barang fising ")
            txtQty.Focus()
        ElseIf CDbl(Trim(lblStockFising.Text)) < CDbl(Trim(txtQty.Text)) Then
            msgInfo("Stock barang fising tidak mencukupi.")
            txtQty.Focus()
        Else
            Dim stockReady As Boolean = True
            Dim kdNotReady As String = ""
            Dim j As Integer = 0

            For j = 0 To DataGridView3.RowCount - 1
                Dim KdBahanMentah = DataGridView3.Rows.Item(j).Cells("Kode").Value
                Dim stock = DataGridView3.Rows.Item(j).Cells("Stock").Value
                If stock < CDbl(Trim(txtQty.Text)) Then
                    stockReady = False
                    kdNotReady &= KdBahanMentah & " "
                End If
            Next
            If stockReady = False Then
                msgInfo("Stock bahan mentah " & kdNotReady & "  tidak mencukupi.")
                txtQty.Focus()
            Else


                dbconmanual.Open()
                Dim trans As MySql.Data.MySqlClient.MySqlTransaction

                trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

                Try
                    If MsgBox("Anda yakin ?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                        Dim Qty = Trim(txtQty.Text)
                        Dim jumAr As Integer = 0
                        'kurangi bahan mentah
                        sql = " select kdBahanMentah Kode,NamaBahanMentah Nama,NamaBahanMentahTipe Tipe," & _
                              " BahanMentahSubKategori `Sub Kategori` " & _
                              " from msbahanmentah a " & _
                              " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe = a.KdBahanMentahTipe " & _
                              " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = a.KdBahanMentahSubKategori " & _
                              " where kdBahanMentah  in (select kdbahanmentah from msfising_detail where kdfising = '" & kdFisingView & "') "

                        Dim reader = execute_reader(sql)

                        Do While reader.Read
                            jumAr = jumAr + 1
                        Loop
                        reader.Close()

                        reader = Nothing
                        Dim kdMentah(jumAr) As String
                        Dim idAr As Integer = 0
                        reader = execute_reader(sql)
                        Do While reader.Read
                            kdMentah(idAr) = reader(0)
                            idAr = idAr + 1
                        Loop
                        reader.Close()
                        Dim x As Integer = 0
                        Dim OPM = "Min"
                        Dim AttributeM = "QtyProd_Min"

                        For x = 0 To DataGridView3.RowCount - 1
                            Dim KdBahanMentah = DataGridView3.Rows.Item(x).Cells("Kode").Value
                            StockBahanMentah(txtQty.Text, OPM, 0, KdBahanMentah, AttributeM, Trim(txtID.Text), "Form Produksi")
                        Next


                        'kurangi fising
                        Dim OP = "Min"
                        Dim Attribute = "QtyProd_Min"

                        StockFising(Qty, OP, 0, KdFising.Text, Attribute, Trim(txtID.Text), "Form Produksi")

                        Dim KdBarang = txtPartNo.Text

                        sql = " Select Harga  As HargaAwal From MsBarangList " & _
                              " where KdBarang ='" & Trim(KdBarang) & "'  And StatusBarangList = 0 " & _
                              " Order By KdList asc Limit 1   "
                        reader = Nothing
                        reader = execute_reader(sql)
                        Dim harga As Double = 0
                        If reader.Read Then
                            harga = reader(0)
                        End If
                        reader.Close()

                        Qty = Trim(txtQty.Text)
                        OP = "Plus"
                        Attribute = "QtyProd_Plus"
                        StockBarang(Qty, OP, harga, KdBarang, Attribute, Trim(txtID.Text), "Form Produksi")

                        'insert into history produksi
                        sql = "insert into  " & tab & " values('" & txtID.Text & "',now(),'" & Trim(txtPartNo.Text) & "','" & Trim(KdFising.Text) & "'," & Trim(txtQty.Text) & ")"
                        ' TextBox1.Text = sql
                        execute_update_manual(sql)
                        msgInfo("Produksi berhasil")

                        sql = "delete from  " & tab & "_T  where kdproduksi= '" & txtID.Text & "'"
                        execute_update_manual(sql)

                        trans.Commit()
                        add()
                    End If
                Catch ex As Exception
                    trans.Rollback()
                    MsgBox(ex, MsgBoxStyle.Information)
                End Try
                dbconmanual.Close()
            End If
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        add()
    End Sub
End Class