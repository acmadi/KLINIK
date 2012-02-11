Imports System.Data.SqlClient
Public Class FormMsBarang
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim OldQty As Double = 0
    Dim StockAwal As Double = 0
    Dim StockAkhir As Double = 0
    Dim KodeTemp As String = ""
    Dim IdentifikasiTemp As String = ""
    Dim PK As String

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        txtID.Text = ""
        txtIdentifikasi.Text = ""
        txtNama.Text = ""
        cmbMerk.SelectedIndex = 0
        cmbKategori.SelectedIndex = 0
        cmbSubKategori.SelectedIndex = 0
        cmbSupplier.SelectedIndex = 0
        txtNote.Text = ""
        txtCari.Text = ""
        txtCari2.Text = ""
        txtHargaList.Text = 0
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtID.Enabled = status
        txtIdentifikasi.Enabled = status
        txtNama.Enabled = status
        cmbMerk.Enabled = status
        cmbKategori.Enabled = status
        cmbSubKategori.Enabled = status
        'txtQty.Enabled = status
        'txtHarga.Enabled = status
        cmbSupplier.Enabled = status
        txtNote.Enabled = status
        txtHargaList.Enabled = status

        btnSave.Enabled = status
        btnCancel.Enabled = status

        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAdd.Enabled = Not status
        btnHistory.Enabled = Not status
        btnList.Enabled = Not status
        btnUpdate.Enabled = Not status
        btnDelete.Enabled = Not status
        DataGridView1.Enabled = Not status

        Button2.Enabled = status
        Button3.Enabled = status
        Button4.Enabled = status
        Button5.Enabled = status
        Button6.Enabled = status

    End Sub

    Private Sub setData()

        If jumData > 0 Then
            Try
                KodeTemp = DataGridView1.CurrentRow.Cells("No. Part").Value.ToString
                txtID.Text = DataGridView1.CurrentRow.Cells("No. Part").Value.ToString
                txtIdentifikasi.Text = DataGridView1.CurrentRow.Cells("No. Identifikasi").Value.ToString
                IdentifikasiTemp = DataGridView1.CurrentRow.Cells("No. Identifikasi").Value.ToString
                txtNama.Text = DataGridView1.CurrentRow.Cells("Nama Part").Value.ToString
                cmbMerk.Text = DataGridView1.CurrentRow.Cells("KdMerk").Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells("Merk").Value.ToString
                cmbKategori.Text = DataGridView1.CurrentRow.Cells("KdKategori").Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells("Kategori").Value.ToString
                cmbSubKategori.Text = DataGridView1.CurrentRow.Cells("KdSub").Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells("Sub Kategori").Value.ToString
                OldQty = DataGridView1.CurrentRow.Cells("Stock").Value.ToString
                cmbSupplier.Text = DataGridView1.CurrentRow.Cells("KdSupplier").Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells("Supplier").Value.ToString
                txtNote.Text = DataGridView1.CurrentRow.Cells("Note").Value.ToString
                StockAwal = DataGridView1.CurrentRow.Cells("StockAwal").Value.ToString
                StockAkhir = DataGridView1.CurrentRow.Cells("Stock").Value.ToString
                txtHargaList.Text = DataGridView1.CurrentRow.Cells("HargaList").Value.ToString
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("No.Identifikasi")
        cmbCari.Items.Add("Nama Barang")
        cmbCari.Items.Add("Merk")
        cmbCari.Items.Add("Kategori")
        cmbCari.Items.Add("Sub Kategori")
        cmbCari.Items.Add("Supplier")
        cmbCari.SelectedIndex = 0

        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("No.Identifikasi")
        cmbCari2.Items.Add("Nama Barang")
        cmbCari2.Items.Add("Merk")
        cmbCari2.Items.Add("Kategori")
        cmbCari2.Items.Add("Sub Kategori")
        cmbCari2.Items.Add("Supplier")
        cmbCari2.SelectedIndex = 4
    End Sub

    Private Sub viewAllData(Optional ByVal cr As String = "", Optional ByVal cr2 As String = "", Optional ByVal opt As String = "", Optional ByVal opt2 As String = "")
        'VisibleQty(False)
        sql = " select kdBarang 'No. Part', KdIdentifikasi 'No. Identifikasi', " & _
              " NamaBarang 'Nama Part',MsMerk.Merk, " & _
              " MsKategori.KdKategori,Kategori Kategori, " & _
              " MsSubkategori.KdSub,SubKategori 'Sub Kategori', " & _
              " ifnull(( Select StockAkhir from BarangHistory where isActive = 1 And KdBarang = " & tab & ".KdBarang order by KdBarangHistory desc limit 1 ),0) Stock, " & _
              " HargaList,MsSupplier.KdSupplier,MsSupplier.Nama Supplier,Note, " & _
              " ifnull(( Select StockAwal from BarangHistory where isActive = 1 And KdBarang = " & tab & ".KdBarang order by KdBarangHistory desc limit 1 ),0) StockAwal, " & _
              " MsMerk.KdMerk, " & _
              " CASE WHEN LevelID = 1 THEN 'Staff rio' WHEN LevelID = 2 THEN 'Staff lain' " & _
              " WHEN LevelID = 3 THEN 'Superuser' End 'Level User' " & _
              " from  " & tab & _
              " Join Mskategori On Mskategori.KdKategori = " & tab & ".KdKategori" & _
              " Join MsSubkategori On MsSubkategori.KdSub = " & tab & ".KdSub" & _
              " Join MsSupplier On MsSupplier.KdSupplier = " & tab & ".KdSupplier" & _
              " Join MsMerk On MsMerk.KdMerk = " & tab & ".KdMerk where 1 "
        sql &= QueryLevel(lvlKaryawan)

        Dim col As String = ""
        Dim col2 As String = ""

        If opt = "No.Identifikasi" Then
            col = "KdIdentifikasi"
        ElseIf opt = "Nama Barang" Then
            col = "NamaBarang"
        ElseIf opt = "Merk" Then
            col = "Merk"
        ElseIf opt = "Kategori" Then
            col = "Kategori"
        ElseIf opt = "Sub Kategori" Then
            col = "SubKategori"
        ElseIf opt = "Supplier" Then
            col = "MsSupplier.Nama"
        End If

        If opt2 = "No.Identifikasi" Then
            col2 = "KdIdentifikasi"
        ElseIf opt2 = "Nama Barang" Then
            col2 = "NamaBarang"
        ElseIf opt2 = "Merk" Then
            col2 = "Merk"
        ElseIf opt2 = "Kategori" Then
            col2 = "Kategori"
        ElseIf opt2 = "Sub Kategori" Then
            col2 = "SubKategori"
        ElseIf opt2 = "Supplier" Then
            col2 = "MsSupplier.Nama"
        End If

        If opt <> "" Then
            sql &= "  And " & col & "  like '%" & cr & "%'  "
        End If
        If opt2 <> "" Then
            sql &= "  And " & col2 & "  like '%" & cr2 & "%' "
        End If

        DataGridView1.DataSource = execute_datatable(sql)
        jumData = DataGridView1.RowCount
        If jumData > 0 Then
            posisi = jumData
            setData()
        End If
    End Sub

    Private Sub setGrid()
        With DataGridView1.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(1).Width = 100
        DataGridView1.Columns(2).Width = 200
        DataGridView1.Columns(3).Width = 100
        DataGridView1.Columns(4).Visible = False
        DataGridView1.Columns(5).Width = 100
        DataGridView1.Columns(6).Visible = False
        DataGridView1.Columns(7).Width = 100
        DataGridView1.Columns(8).Width = 100
        DataGridView1.Columns(9).Width = 100
        DataGridView1.Columns(10).Visible = False
        DataGridView1.Columns(11).Visible = False
        DataGridView1.Columns(13).Width = 100
        DataGridView1.Columns(14).Visible = False
        DataGridView1.Columns("StockAwal").Visible = False
    End Sub

    Public Sub setCmbKategori()
        Dim reader = execute_reader("Select * from MsKategori  where Kategori <>'' order by Kategori asc")
        Dim varT As String = ""
        cmbKategori.Items.Clear()
        cmbKategori.Items.Add("- Pilih Kategori -")
        Dim i As Integer = 1
        Do While reader.Read
            cmbKategori.Items.Add(reader(0) & " ~ " & reader(1))
           
        Loop
        reader.Close()
        If cmbKategori.Items.Count > 0 Then
            cmbKategori.SelectedIndex = 0
        End If

    End Sub

    Public Sub setCmbSubKategori()
        Dim reader = execute_reader("Select * from MsSubkategori  where SubKategori <>'' order by SubKategori asc")
        Dim varT As String = ""
        cmbSubKategori.Items.Clear()
        cmbSubKategori.Items.Add("- Pilih Sub Kategori -")
        Do While reader.Read
            cmbSubKategori.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbSubKategori.Items.Count > 0 Then
            cmbSubKategori.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbSupplier()
        Dim reader = execute_reader("Select * from MsSupplier  where Nama <>'' order by Nama asc")
        Dim varT As String = ""
        cmbSupplier.Items.Clear()
        cmbSupplier.Items.Add("- Pilih Supplier -")
        Do While reader.Read
            cmbSupplier.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbSupplier.Items.Count > 0 Then
            cmbSupplier.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbMerk()
        Dim reader = execute_reader("Select * from MsMerk  where merk <>'' order by merk asc")
        Dim varT As String = ""
        cmbMerk.Items.Clear()
        cmbMerk.Items.Add("- Pilih Merk -")
        Do While reader.Read
            cmbMerk.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbMerk.Items.Count > 0 Then
            cmbMerk.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    'Function VisibleQty(ByVal status As Boolean)
    '    lblQty.Visible = status
    '    txtQty.Visible = status
    '    lblQtyBintang.Visible = status
    '    txtHarga.Visible = status
    '    lblHargaBintang.Visible = status
    '    lblHarga.Visible = status
    '    Return False
    'End Function

    Public Sub FormMsBarang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBarang "
        PK = "  KdBarang "
        setCmbKategori()
        setCmbSubKategori()
        setCmbSupplier()
        setCmbMerk()
        ubahAktif(False)
        viewAllData("", "")
        posisi = 0
        setData()
        setGrid()
        setCmbCari()
        If lvlKaryawan <> 3 Then
            btnList.Visible = False
            lblSeperatorList.Visible = False
        End If
    End Sub

    Private Sub generateCode()
        Dim code As String = "BR"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        'code += Today.Year.ToString.Substring(2, 2)
        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, "yyMMdd")

        'If bulan < 10 Then
        '    code += "0" + bulan.ToString
        'Else
        '    code += bulan.ToString
        'End If
        code += FormatDate

        If DataGridView1.Rows.Count > 0 Then
            kode = Trim(DataGridView1.Rows(DataGridView1.Rows.Count - 1).Cells("No. Part").Value.ToString())
            temp = kode.Substring(0, 8)
            If temp = code Then
                angka = CInt(kode.Substring(8, 4))
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
        txtID.Text = Trim(code)
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= DataGridView1.Rows.Count - 1 Then
            posisi = e.RowIndex
            setData()
        End If
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtCari.Text = ""
        txtCari2.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        'generateCode()
        'VisibleQty(True)
        ubahAktif(True)
        status = "add"
        emptyField()
        StockAkhir = 0
        OldQty = 0
        txtID.Focus()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If (DataGridView1.RowCount) Then
            OldQty = DataGridView1.CurrentRow.Cells("Stock").Value.ToString
            ubahAktif(True)
            status = "update"
            'VisibleQty(False)
            txtID.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If (DataGridView1.RowCount) Then
                Dim selected_cell = DataGridView1.CurrentRow.Cells("No. Part").Value
                If MsgBox("Anda yakin ingin menghapus kode barang " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                    execute_update("delete from " & tab & " where  " & PK & " = '" & selected_cell & "'")
                    msgInfo("Data berhasil dihapus")
                    posisi = 0
                    viewAllData("", "")
                    setData()
                End If
            Else
                msgInfo("Data tidak ditemukan.")
            End If
        Catch ex As Exception
            msgInfo("Data tidak dapat dihapus.")
        End Try
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If txtID.Text = "" Then
            msgInfo("No. Part harus diisi")
            txtID.Focus()
        ElseIf txtIdentifikasi.Text = "" Then
            msgInfo("No. Identifikasi harus diisi")
            txtIdentifikasi.Focus()
        ElseIf txtNama.Text = "" Then
            msgInfo("Nama bahan mentah harus diisi")
            txtNama.Focus()
        ElseIf cmbMerk.SelectedIndex = 0 Then
            msgInfo("Merk harus dipilih")
            cmbMerk.Focus()
        ElseIf cmbKategori.SelectedIndex = 0 Then
            msgInfo("Kategori harus dipilih")
            cmbSubKategori.Focus()
        ElseIf cmbSubKategori.SelectedIndex = 0 Then
            msgInfo("Sub kategori harus dipilih")
            cmbSubKategori.Focus()
        ElseIf cmbSupplier.SelectedIndex = 0 Then
            msgInfo("Supplier harus dipilih")
            cmbSupplier.Focus()
            'ElseIf txtNote.Text = "" Then
            '    msgInfo("Note harus diisi")
            '    txtNote.Focus()
        Else
            Dim merkID = cmbMerk.Text.ToString.Split("~")
            Dim KategoriID = cmbKategori.Text.ToString.Split(" ~ ")
            Dim SubKategoriID = cmbSubKategori.Text.ToString.Split(" ~ ")
            Dim supplierID = cmbSupplier.Text.ToString.Split(" ~ ")
            Dim QtyUpdate_Plus = 0
            Dim QtyUpdate_Min = ""
            Dim QtyUpdate = 0
            Dim OP = ""
            Dim Attribute = ""
            StockAwal = StockAkhir

            If status = "add" Then
                sql = " insert into  " & tab & "  values('" + Trim(txtID.Text) + "','" + Trim(txtIdentifikasi.Text) + "', " & _
                      " '" & Trim(merkID(0)) & "', " & _
                      " '" & Trim(KategoriID(0)) & "','" & Trim(SubKategoriID(0)) & "'," & _
                      " '" & Trim(supplierID(0)) & "','" & Trim(txtNama.Text) & "', " & _
                      " '" & Trim(txtNote.Text) & "','" & Val(txtHargaList.Text) & "','" & Trim(lvlKaryawan) & "')"

                'If (OldQty < txtQty.Text) Then
                '    QtyUpdate_Plus = Val(txtQty.Text) - Val(OldQty)
                '    QtyUpdate = QtyUpdate_Plus
                '    StockAkhir += Val(QtyUpdate)
                '    OP = "Plus"
                '    Attribute = "QtyUpdate_Plus"
                'ElseIf (OldQty > txtQty.Text) Then
                '    QtyUpdate_Min = Val(OldQty) - Val(txtQty.Text)
                '    QtyUpdate = QtyUpdate_Min
                '    StockAkhir -= Val(QtyUpdate)
                '    OP = "Min"
                '    Attribute = "QtyUpdate_Min"
                'End If

                dbconmanual.Open()
                Dim trans As MySql.Data.MySqlClient.MySqlTransaction

                trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)
                Try
                    execute_update_manual(sql)
                    'If QtyUpdate <> 0 Then
                    '    StockBarang(QtyUpdate, OP, txtHarga.Text, Trim(txtID.Text), Attribute, "", "Form Barang")
                    'End If
                    trans.Commit()

                    viewAllData("", "")
                    ubahAktif(False)
                    msgInfo("Data berhasil disimpan")
                Catch ex As Exception
                    trans.Rollback()
                    msgWarning("Data tidak valid")
                End Try
                dbconmanual.Close()
            ElseIf status = "update" Then
            Try
                    sql = "update   " & tab & "  set  KdBarang = '" & Trim(txtID.Text) & "', " & _
                          " KdIdentifikasi = '" & Trim(txtIdentifikasi.Text) & "'," & _
                          " kdMerk = '" & Trim(merkID(0)) & "', " & _
                          " KdKategori='" & Trim(KategoriID(0)) & "', " & _
                          " KdSub='" & Trim(SubKategoriID(0)) & "', " & _
                          " KdSupplier='" & Trim(supplierID(0)) & "', " & _
                          " NamaBarang='" & Trim(txtNama.Text) & "', " & _
                          " HargaList='" & Val(txtHargaList.Text) & "', " & _
                          " LevelID='" & Trim(lvlKaryawan) & "', " & _
                          " Note='" & Trim(txtNote.Text) & "' where  " & PK & "='" + txtID.Text + "' "
                execute_update(sql)
                viewAllData("", "")
                ubahAktif(False)
                msgInfo("Data berhasil disimpan")
            Catch ex As Exception
                msgWarning(" Data tidak valid")
                txtNama.Text = ""
            End Try
        End If
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If status = "add" Then
            posisi = 0
        End If
        ubahAktif(False)
        setData()
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub txtNama_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNama.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNama.Text <> "" Then
            cmbMerk.Focus()
        End If
    End Sub

    'Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
    '    If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
    '        e.KeyChar = Nothing
    '    End If
    '    If AscW(e.KeyChar) = 13 And txtQty.Text <> "" Then
    '        txtHarga.Focus()
    '    End If
    'End Sub

    Private Sub txtNote_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNote.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtID.KeyPress
        If (Asc(e.KeyChar) = Asc("'") Or Asc(e.KeyChar) = 32) And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtID.Text <> "" Then
            txtIdentifikasi.Focus()
        End If
    End Sub

    Private Sub txtID_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtID.Leave
        If check_code(txtID.Text, KodeTemp, "MsBarang", "KdBarang", status) Then
            msgInfo("No. Part telah terdaftar. Silahkan coba yang lain!!!")
            txtID.Text = ""
            txtID.Focus()
        End If
    End Sub

    Private Sub txtIdentifikasi_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtIdentifikasi.KeyPress
        If (Asc(e.KeyChar) = Asc("'") Or Asc(e.KeyChar) = 32) And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtIdentifikasi.Text <> "" Then
            txtNama.Focus()
        End If
    End Sub

    Private Sub txtIdentifikasi_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtIdentifikasi.Leave
        If check_code(txtIdentifikasi.Text, IdentifikasiTemp, "MsBarang", "KdIdentifikasi", status) Then
            msgInfo("No. identifikasi telah terdaftar. Silahkan coba yang lain!!!")
            txtIdentifikasi.Text = ""
            txtIdentifikasi.Focus()
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        FormMsMerk.Show()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        FormMsKategori.Show()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        FormMsKategoriSub.Show()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        FormMsSupplier.Show()
    End Sub
 

    Public Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        setCmbKategori()
        setCmbSubKategori()
        setCmbSupplier()
        setCmbMerk()

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        Button6_Click(Nothing, Nothing)
        Timer1.Enabled = False

    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHistory.Click
        Dim selected_cell = DataGridView1.CurrentRow.Cells("No. Part").Value
        open_subpage("FormBarangHistory", selected_cell)
    End Sub

    'Private Sub txtHarga_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
    '    If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
    '        e.KeyChar = Nothing
    '    End If
    '    If AscW(e.KeyChar) = 13 And txtHarga.Text <> "" Then
    '        btnSave_Click(Nothing, Nothing)
    '    End If
    'End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged

    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub txtHargaList_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtHargaList.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 Then
            btnSave_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnList.Click
        Dim selected_cell = DataGridView1.CurrentRow.Cells("No. Part").Value
        open_subpage("FormBarangList", selected_cell)
    End Sub

    Private Sub txtCari2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari2.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub

    Private Sub cmbCari2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari2.SelectedIndexChanged

    End Sub
End Class
