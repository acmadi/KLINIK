Imports MySql.Data.MySqlClient
Public Class FormMsBahanMentah
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim OldQty As Double = 0
    Dim StockAwal As Double = 0
    Dim StockAkhir As Double = 0
    Dim PK As String

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        txtCari.Text = ""
        txtCari2.Text = ""
        txtNama.Text = ""
        cmbTipe.SelectedIndex = 0
        cmbSubKategori.SelectedIndex = 0
        txtQty.Text = ""
        txtHarga.Text = ""
        cmbSupplier.SelectedIndex = 0
        txtNote.Text = ""
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtID.Enabled = False
        txtNama.Enabled = status
        cmbTipe.Enabled = status
        cmbSubKategori.Enabled = status
        txtQty.Enabled = status
        txtHarga.Enabled = status
        cmbSupplier.Enabled = status
        txtNote.Enabled = status

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

    End Sub

    Private Sub setData()

        If jumData > 0 Then
            Try
                txtID.Text = DataGridView1.CurrentRow.Cells(0).Value.ToString
                txtNama.Text = DataGridView1.CurrentRow.Cells(1).Value.ToString
                cmbTipe.Text = DataGridView1.CurrentRow.Cells(2).Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells(3).Value.ToString
                cmbSubKategori.Text = DataGridView1.CurrentRow.Cells(4).Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells(5).Value.ToString
                txtQty.Text = DataGridView1.CurrentRow.Cells(6).Value.ToString
                OldQty = DataGridView1.CurrentRow.Cells(6).Value.ToString
                cmbSupplier.Text = DataGridView1.CurrentRow.Cells(8).Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells(9).Value.ToString
                txtNote.Text = DataGridView1.CurrentRow.Cells(10).Value.ToString
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Nama Barang")
        cmbCari.Items.Add("Tipe")
        cmbCari.Items.Add("Sub Kategori")
        cmbCari.Items.Add("Supplier")
        cmbCari.SelectedIndex = 0

        cmbCari2.Items.Clear()
        cmbCari2.Items.Add("Nama Barang")
        cmbCari2.Items.Add("Tipe")
        cmbCari2.Items.Add("Sub Kategori")
        cmbCari2.Items.Add("Supplier")
        cmbCari2.SelectedIndex = 2
    End Sub

    Private Sub viewAllData(Optional ByVal cr As String = "", Optional ByVal cr2 As String = "", Optional ByVal opt As String = "", Optional ByVal opt2 As String = "")
        sql = " select kdBahanMentah Kode,NamaBahanMentah Nama,MsBahanMentahTipe.KdBahanMentahTipe, " & _
              " NamaBahanMentahTipe Tipe,MsBahanMentahSubKategori.KdBahanMentahSubKategori, " & _
              " BahanMentahSubKategori 'Sub Kategori', ifnull(( " & _
              " Select StockAkhir from BahanMentahHistory " & _
              " Where KdBahanMentah = " & tab & ".KdBahanMentah " & _
              " order by KdHistory desc limit 1 ),0) Stock, " & _
              " ifnull(( Select DATE_FORMAT( TanggalHistory,'%d/%m/%Y') TanggalHistory " & _
              " from BahanMentahHistory " & _
              " Where KdBahanMentah = " & tab & ".KdBahanMentah " & _
              " order by KdHistory desc limit 1 ),'00/00/0000') `Tanggal Stok`, " & _
              " MsSupplier.KdSupplier,MsSupplier.Nama Supplier,NamaBahanMentah 'Nama Barang', " & _
              " Note, CASE WHEN LevelID = 1 THEN 'Staff rio' WHEN LevelID = 2 THEN 'Staff lain' " & _
              " WHEN LevelID = 3 THEN 'Superuser' End 'Level User' " & _
              " from  " & tab & _
              " Join MsBahanMentahTipe On MsBahanMentahTipe.KdBahanMentahTipe = " & tab & ".KdBahanMentahTipe" & _
              " Join MsBahanMentahSubKategori On MsBahanMentahSubKategori.KdBahanMentahSubKategori = " & tab & ".KdBahanMentahSubKategori" & _
              " Join MsSupplier On MsSupplier.KdSupplier = " & tab & ".KdSupplier " & _
              " where 1 "
        sql &= QueryLevel(lvlKaryawan)

        Dim col As String = ""
        Dim col2 As String = ""

        If opt = "Nama Barang" Then
            col = "NamaBahanMentah"
        ElseIf opt = "Tipe" Then
            col = "NamaBahanMentahTipe"
        ElseIf opt = "Sub Kategori" Then
            col = "BahanMentahSubKategori"
        ElseIf opt = "Supplier" Then
            col = "NamaSupplier"
        End If

        If opt2 = "Nama Barang" Then
            col2 = "NamaBahanMentah"
        ElseIf opt2 = "Tipe" Then
            col2 = "NamaBahanMentahTipe"
        ElseIf opt2 = "Sub Kategori" Then
            col2 = "BahanMentahSubKategori"
        ElseIf opt2 = "Supplier" Then
            col2 = "NamaSupplier"
        End If

        If opt <> "" Then
            sql &= "  And " & col & "  like '%" & cr & "%'  "
        End If
        If opt2 <> "" Then
            sql &= "  And " & col2 & "  like '%" & cr2 & "%' "
        End If
        sql &= " Order by kdBahanMentah desc "

        DataGridView1.DataSource = execute_datatable(sql)
        jumData = DataGridView1.RowCount
        If jumData > 0 Then
            posisi = jumData
            setData()
        End If

        Dim reader = execute_reader("Select StockAwal,StockAkhir from BahanMentahHistory order by KdHistory desc limit 1")
        Do While reader.Read
            StockAwal = reader(0)
            StockAkhir = reader(1)
        Loop
        reader.Close()
    End Sub

    Private Sub setGrid()
        'With DataGridView1.ColumnHeadersDefaultCellStyle
        '    .Alignment = DataGridViewContentAlignment.MiddleCenter
        '    .Font = New Font(.Font.FontFamily, .Font.Size, _
        '      .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
        '    .ForeColor = Color.Gold

        'End With
        'DataGridView1.Columns(0).Width = 100
        'DataGridView1.Columns(1).Width = 200
        'DataGridView1.Columns(2).Visible = False
        'DataGridView1.Columns(3).Width = 100
        'DataGridView1.Columns(4).Visible = False
        'DataGridView1.Columns(5).Width = 100
        'DataGridView1.Columns(6).Width = 50
        'DataGridView1.Columns(7).Width = 100
        'DataGridView1.Columns(8).Visible = False
        'DataGridView1.Columns(9).Width = 100
        'DataGridView1.Columns(10).Width = 100
        DataGridView1.Columns(1).Width = 200
        DataGridView1.Columns(2).Visible = False
        DataGridView1.Columns(4).Visible = False
        DataGridView1.Columns(8).Visible = False

    End Sub

    Public Sub setCmbTipe()
        Dim reader = execute_reader("Select * from MsBahanMentahTipe  where NamaBahanMentahTipe <>'' order by NamaBahanMentahTipe asc")
        Dim varT As String = ""
        cmbTipe.Items.Clear()
        cmbTipe.Items.Add("- Pilih Tipe -")
        Do While reader.Read
            cmbTipe.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbTipe.Items.Count > 0 Then
            cmbTipe.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbSubKategori()
        Dim reader = execute_reader("Select * from MsBahanMentahSubKategori  where BahanMentahSubKategori <>'' order by BahanMentahSubKategori asc")
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

    Function VisibleQty(ByVal status As Boolean)
        lblQty.Visible = status
        txtQty.Visible = status
        lblQtyBintang.Visible = status
        txtHarga.Visible = status
        lblHargaBintang.Visible = status
        lblHarga.Visible = status
        Return False
    End Function

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsBahanMentah "
        PK = "  KdBahanMentah  "
        setCmbTipe()
        setCmbSubKategori()
        setCmbSupplier()
        ubahAktif(False)
        viewAllData()
        posisi = 0
        setData()
        setGrid()
        setCmbCari()
    End Sub

    Private Sub generateCode()
        Dim code As String = "BM"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        'code += Today.Year.ToString.Substring(2, 2)
        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = String.Format("{0:yyMMdd}", Convert.ToDateTime(Today.Date)) 'Format(currentTime, "yyMMdd")

        'If bulan < 10 Then
        '    code += "0" + bulan.ToString
        'Else
        '    code += bulan.ToString
        'End If
        code += FormatDate

        Dim reader = getCode("MsBahanMentah", "KdBahanMentah")

        If reader.read Then
            kode = Trim(reader(0).ToString())
            temp = kode.Substring(0, 8)
            If temp = code Then
                angka = CInt(kode.Substring(8, 4))
            Else
                angka = 0
            End If
        Else
            angka = 0
        End If
        reader.close()

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

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        txtCari.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        emptyField()
        generateCode()
        VisibleQty(True)
        ubahAktif(True)
        status = "add"
        txtNama.Focus()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If (DataGridView1.RowCount) Then
            VisibleQty(False)
            OldQty = DataGridView1.CurrentRow.Cells(6).Value.ToString
            ubahAktif(True)
            status = "update"
            txtNama.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            If (DataGridView1.RowCount) Then
                Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
                If MsgBox("Anda yakin ingin menghapus kode bahan mentah " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
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
        If txtNama.Text = "" Then
            msgInfo("Nama bahan mentah harus diisi")
            txtNama.Focus()
        ElseIf cmbTipe.SelectedIndex = 0 Then
            msgInfo("Tipe harus dipilih")
            cmbTipe.Focus()
        ElseIf cmbSubKategori.SelectedIndex = 0 Then
            msgInfo("Sub kategori harus dipilih")
            cmbSubKategori.Focus()
        ElseIf txtQty.Text = "" Then
            msgInfo("Kuantitas harus diisi")
            txtQty.Focus()
        ElseIf cmbSupplier.SelectedIndex = 0 Then
            msgInfo("Supplier harus dipilih")
            cmbSupplier.Focus()
            'ElseIf txtNote.Text = "" Then
            '    msgInfo("Note harus diisi")
            '    cmbSupplier.Focus()
        Else
            Dim tipeID = cmbTipe.Text.ToString.Split(" ~ ")
            Dim SubKategoriID = cmbSubKategori.Text.ToString.Split(" ~ ")
            Dim supplierID = cmbSupplier.Text.ToString.Split(" ~ ")
            Dim QtyUpdate_Plus = 0
            Dim QtyUpdate_Min = ""
            Dim QtyUpdate = 0
            Dim OP = ""
            Dim Attribute = ""
            StockAwal = StockAkhir

            'If (OldQty < txtQty.Text) Then
            '    QtyUpdate_Plus = Val(txtQty.Text) - Val(OldQty)
            '    QtyUpdate = QtyUpdate_Plus
            '    StockAkhir += Val(QtyUpdate)
            '    OP = "Min"
            '    Attribute = "QtyUpdate_Plus"
            'ElseIf (OldQty > txtQty.Text) Then
            'QtyUpdate_Min = Val(OldQty) - Val(txtQty.Text)
            'QtyUpdate = QtyUpdate_Min
            'StockAkhir -= Val(QtyUpdate)
            'OP = "Min"
            'Attribute = "QtyUpdate_Min"
            'End If

            If status = "add" Then
                If Val(txtQty.Text) = 0 Then
                    msgInfo("Kuantitas harus diisi dan lebih besar dari 0")
                    txtQty.Focus()
                ElseIf Val(txtHarga.Text) = 0 Then
                    msgInfo("Harga harus diisi dan lebih besar dari 0")
                    txtQty.Focus()
                Else
                    sql = " insert into  " & tab & "  values('" + Trim(txtID.Text) + "','" & Trim(tipeID(0)) & "', " & _
                          " '" & Trim(SubKategoriID(0)) & "','" & Trim(supplierID(0)) & "', " & _
                          " '" & Trim(txtNama.Text) & "','" & Trim(txtNote.Text) & "', " & _
                          " " & Trim(lvlKaryawan) & ")"

                    QtyUpdate_Plus = Val(txtQty.Text)
                    QtyUpdate = QtyUpdate_Plus
                    OP = "Plus"
                    Attribute = "QtyUpdate_Plus"

                    dbconmanual.Open()
                    Dim trans As MySql.Data.MySqlClient.MySqlTransaction

                    trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)
                    Try
                        execute_update_manual(sql)
                        StockBahanMentah(QtyUpdate, OP, txtHarga.Text, Trim(txtID.Text), Attribute, "", "Form BahanMentah")
                        ubahAktif(False)
                        trans.Commit()
                        msgInfo("Data berhasil disimpan")
                        viewAllData("", "")
                    Catch ex As Exception
                        trans.Rollback()
                        MsgBox(ex, MsgBoxStyle.Information)
                    End Try
                    dbconmanual.Close()
                End If
            ElseIf status = "update" Then
                Try
                    sql = "update   " & tab & "  set  KdBahanMentahTipe='" & Trim(tipeID(0)) & "'," & _
                    " KdBahanMentahSubKategori='" & Trim(SubKategoriID(0)) & "'," & _
                    " KdSupplier='" & Trim(supplierID(0)) & "'," & _
                    " NamaBahanMentah='" & Trim(txtNama.Text) & "', " & _
                    " Note='" & Trim(txtNote.Text) & "', " & _
                    " LevelID = " & Trim(lvlKaryawan) & " " & _
                    " where  " & PK & "='" + txtID.Text + "' "
                    execute_update(sql)
                    ubahAktif(False)

                    msgInfo("Data berhasil disimpan")
                    viewAllData("", "")
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
            cmbTipe.Focus()
        End If
    End Sub

    Private Sub txtQty_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtQty.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtQty.Text <> "" Then
            txtHarga.Focus()
        End If
    End Sub

    Private Sub txtNote_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNote.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtHarga_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtHarga.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtHarga.Text <> "" Then
            cmbSupplier.Focus()
        End If
    End Sub

    Private Sub cmbSupplier_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbSupplier.SelectedIndexChanged
        txtNote.Focus()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        FormMsBahanMentahTipe.Show()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        FormMsBahanMentahSubKategori.Show()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        FormMsSupplier.Show()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        setCmbTipe()
        setCmbSubKategori()
        setCmbSupplier()
    End Sub

    Private Sub btnHistory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHistory.Click
        Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
        open_subpage("FormBahanMentahHistory", selected_cell)
    End Sub

    Private Sub ToolStripSeparator6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripSeparator6.Click

    End Sub

    Private Sub btnList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnList.Click
        Dim selected_cell = DataGridView1.CurrentRow.Cells("Kode").Value
        open_subpage("FormBahanMentahList", selected_cell)
    End Sub


    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        txtCari.Text = ""
        txtCari2.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub txtCari2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari2.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub
End Class
