Imports System.Data.SqlClient
Public Class FormMsToko
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
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
        txtNamaToko.Text = ""
        txtOwner.Text = ""
        txtAlamat.Text = ""
        cmbArea.SelectedIndex = 0
        txtDaerah.Text = ""
        cmbEkspedisi.SelectedIndex = 0
        txtNoTelp.Text = ""
        txtNoHp.Text = ""
        txtNoFax.Text = ""
        txtJatuhTempo.Text = ""
    End Sub

    Private Sub ubahAktif(ByVal status As Boolean)
        txtID.Enabled = False
        txtNamaToko.Enabled = status
        txtOwner.Enabled = status
        txtAlamat.Enabled = status
        cmbArea.Enabled = status
        txtDaerah.Enabled = status
        cmbEkspedisi.Enabled = status
        txtNoTelp.Enabled = status
        txtNoHp.Enabled = status
        txtNoFax.Enabled = status
        txtJatuhTempo.Enabled = status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAdd.Enabled = Not status
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
                txtNamaToko.Text = DataGridView1.CurrentRow.Cells(1).Value.ToString
                txtOwner.Text = DataGridView1.CurrentRow.Cells(2).Value.ToString
                txtAlamat.Text = DataGridView1.CurrentRow.Cells(3).Value.ToString
                cmbArea.Text = DataGridView1.CurrentRow.Cells(4).Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells(5).Value.ToString
                txtDaerah.Text = DataGridView1.CurrentRow.Cells(6).Value.ToString
                cmbEkspedisi.Text = DataGridView1.CurrentRow.Cells(7).Value.ToString & " ~ " & DataGridView1.CurrentRow.Cells(8).Value.ToString
                txtNoTelp.Text = DataGridView1.CurrentRow.Cells(9).Value.ToString
                txtNoHp.Text = DataGridView1.CurrentRow.Cells(10).Value.ToString
                txtNoFax.Text = DataGridView1.CurrentRow.Cells(11).Value.ToString
                txtJatuhTempo.Text = DataGridView1.CurrentRow.Cells(12).Value.ToString
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Nama Toko")
        cmbCari.Items.Add("Nama Owner")
        cmbCari.Items.Add("Nama Area")
        cmbCari.Items.Add("Nama Dearah")
        cmbCari.Items.Add("Ekpedisi")
        cmbCari.SelectedIndex = 0

        cmbCari2.Items.Clear()
        '  cmbCari2.Items.Add("Nama Toko")
        cmbCari2.Items.Add("Nama Owner")
        cmbCari2.Items.Add("Nama Area")
        cmbCari2.Items.Add("Nama Dearah")
        cmbCari2.Items.Add("Ekpedisi")
        cmbCari2.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(Optional ByVal cr As String = "", Optional ByVal cr2 As String = "", Optional ByVal opt As String = "", Optional ByVal opt2 As String = "")
        sql = " select kdtoko Kode,NamaToko 'Nama Toko',NamaOwner 'Nama Owner', " & _
              " AlamatToko 'Alamat Toko',MsArea.KdArea, NamaArea Area, " & _
              " Daerah,MsEkspedisi.KdEkspedisi, NamaEkspedisi Ekspedisi,NoTelp 'Nomor Telepon', " & _
              " NoHP 'Nomor HP',NoFax 'Nomor Fax', JatuhTempo 'Jatuh Tempo' " & _
              " from  " & tab & _
              " Join MsEkspedisi On MsEkspedisi.KdEkspedisi = MsToko.KdEkspedisi " & _
              " Join MsArea On MsArea.KdArea = MsToko.KdArea " & _
              " where 1 "

        Dim col As String = ""
        Dim col2 As String = ""

        If opt = "Nama Toko" Then
            col = "NamaToko"
        ElseIf opt = "Nama Owner" Then
            col = "NamaOwner"
        ElseIf opt = "Nama Area" Then
            col = "NamaArea"
        ElseIf opt = "Nama Dearah" Then
            col = "Daerah"
        ElseIf opt = "Ekpedisi" Then
            col = "NamaEkspedisi"
        End If

        If opt2 = "Nama Toko" Then
            col2 = "NamaToko"
        ElseIf opt2 = "Nama Owner" Then
            col2 = "NamaOwner"
        ElseIf opt2 = "Nama Area" Then
            col2 = "NamaArea"
        ElseIf opt2 = "Nama Dearah" Then
            col2 = "Daerah"
        ElseIf opt2 = "Ekpedisi" Then
            col2 = "NamaEkspedisi"
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
        DataGridView1.Columns(1).Width = 150
        DataGridView1.Columns(2).Width = 150
        DataGridView1.Columns(3).Width = 150
        DataGridView1.Columns(4).Visible = False
        DataGridView1.Columns(5).Width = 100
        DataGridView1.Columns(6).Width = 100
        DataGridView1.Columns(7).Visible = False
        DataGridView1.Columns(8).Width = 100
        DataGridView1.Columns(9).Width = 100
        DataGridView1.Columns(10).Width = 100
        DataGridView1.Columns(11).Width = 100
        DataGridView1.Columns(12).Width = 100

    End Sub

    Public Sub setCmbArea1()
        Dim reader = execute_reader("Select * from MsArea where NamaArea <>'' order by NamaArea asc")
        Dim varT As String = ""
        cmbArea.Items.Clear()
        cmbArea.Items.Add("- Pilih Area -")
        Do While reader.Read
            cmbArea.Items.Add(reader(0) & " ~ " & reader(1))
        Loop
        reader.Close()
        If cmbArea.Items.Count > 0 Then
            cmbArea.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Public Sub setCmbEkspedisi()
        Dim reader = execute_reader("Select * from MsEkspedisi  where NamaEkspedisi <>'' order by NamaEkspedisi asc")
        Dim varT As String = ""
        cmbEkspedisi.Items.Clear()
        cmbEkspedisi.Items.Add("- Pilih Ekspedisi -")
        Do While reader.Read
            cmbEkspedisi.Items.Add(reader(0) & " ~ " & reader(1)) ' & "-" & reader(0))
        Loop
        reader.Close()
        If cmbEkspedisi.Items.Count > 0 Then
            cmbEkspedisi.SelectedIndex = 0
        End If
        reader.Close()
    End Sub

    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tab = " MsToko "
        PK = "  KdToko  "
        setCmbArea1()
        setCmbEkspedisi()
        ubahAktif(False)
        viewAllData("", "")
        posisi = 0
        setData()
        setGrid()
        setCmbCari()
    End Sub

    Private Sub generateCode()
        Dim code As String = "TK"
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

        Dim reader = getCode("MsToko", "KdToko")

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

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= DataGridView1.Rows.Count - 1 Then
            posisi = e.RowIndex
            setData()
        End If
    End Sub

    Private Sub btnAdd_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        emptyField()
        generateCode()
        ubahAktif(True)
        status = "add"
        txtNamaToko.Focus()
    End Sub

    Private Sub btnUpdate_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        ubahAktif(True)
        status = "update"
        txtNamaToko.Focus()
    End Sub

    Private Sub btnDelete_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If (DataGridView1.RowCount) Then
            Dim selected_cell = DataGridView1.CurrentRow.Cells(0).Value
            If MsgBox("Anda yakin ingin menghapus kode toko " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                execute_update("delete from " & tab & " where  " & PK & " = '" & selected_cell & "'")
                msgInfo("Data berhasil dihapus")
                posisi = 0
                viewAllData("", "")
                setData()
            End If
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If txtNamaToko.Text = "" Then
            msgInfo("Nama toko harus diisi")
            txtNamaToko.Focus()
        ElseIf txtOwner.Text = "" Then
            msgInfo("Nama Owner harus diisi")
            txtOwner.Focus()
        ElseIf txtAlamat.Text = "" Then
            msgInfo("Nama alamat harus diisi")
            txtAlamat.Focus()
        ElseIf cmbArea.Text = "" Then
            msgInfo("Area harus dipilih")
            cmbArea.Focus()
        ElseIf txtDaerah.Text = "" Then
            msgInfo("Daerah harus diisi")
            txtDaerah.Focus()
        ElseIf cmbEkspedisi.SelectedIndex = 0 Then
            msgInfo("Ekspedisi harus dipilih")
            cmbEkspedisi.Focus()
        ElseIf txtNoTelp.Text = "" Then
            msgInfo("No telepon harus diisi")
            txtNoTelp.Focus()
        ElseIf txtJatuhTempo.Text = "" Then
            msgInfo("Jatuh Tempo harus diisi")
            txtJatuhTempo.Focus()
        Else
            Dim areaID = cmbArea.Text.ToString.Split(" ~ ")
            Dim EkspedisiID = cmbEkspedisi.Text.ToString.Split(" ~ ")
            If status = "add" Then
                sql = " insert into  " & tab & "  values('" + Trim(txtID.Text) + "','" & Trim(txtNamaToko.Text) & "','" & Trim(txtOwner.Text) & "','" & Trim(txtAlamat.Text) & "', " & _
                      " '" & areaID(0) & "','" & Trim(txtDaerah.Text) & "','" & Trim(EkspedisiID(0)) & "', " & _
                      " '" & Trim(txtNoTelp.Text) & "','" & Trim(txtNoHp.Text) & "','" & Trim(txtNoFax.Text) & "','" & Trim(txtJatuhTempo.Text) & "')"
                Try
                    execute_update(sql)
                    viewAllData("", "")
                    ubahAktif(False)
                    msgInfo("Data berhasil disimpan")
                Catch ex As Exception
                    msgWarning("Data tidak valid")
                    'txtNoPolisi.Text = ""
                End Try
            ElseIf status = "update" Then
                Try
                    sql = " update   " & tab & "  set  NamaToko='" & Trim(txtNamaToko.Text) & "',NamaOwner='" & Trim(txtOwner.Text) & "', " & _
                          " AlamatToko='" & Trim(txtAlamat.Text) & "', " & _
                          " Daerah='" & Trim(txtDaerah.Text) & "', " & _
                          " KdEkspedisi='" & Trim(EkspedisiID(0)) & "',NoTelp='" & Trim(txtNoTelp.Text) & "', " & _
                          " NoHP='" & Trim(txtNoHp.Text) & "',NoFax='" & Trim(txtNoFax.Text) & "', " & _
                          " JatuhTempo='" & Trim(txtJatuhTempo.Text) & "', " & _
                          " kdArea='" & Trim(areaID(0)) & "' where  " & PK & "='" + txtID.Text + "' "
                    execute_update(sql)
                    viewAllData("", "")
                    ubahAktif(False)

                    msgInfo("Data berhasil disimpan")
                Catch ex As Exception
                    msgWarning(" Data tidak valid")
                    txtNamaToko.Text = ""
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

    Private Sub txtNamaToko_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNamaToko.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNamaToko.Text <> "" Then
            txtOwner.Focus()
        End If
    End Sub

    Private Sub txtOwner_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtOwner.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtOwner.Text <> "" Then
            txtAlamat.Focus()
        End If
    End Sub

    Private Sub txtAlamat_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAlamat.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtDaerah_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDaerah.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtDaerah.Text <> "" Then
            cmbEkspedisi.Focus()
        End If
    End Sub

    Private Sub txtNoTelp_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNoTelp.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNoTelp.Text <> "" Then
            txtNoHp.Focus()
        End If
    End Sub

    Private Sub txtNoHp_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNoHp.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNoHp.Text <> "" Then
            txtNoFax.Focus()
        End If
    End Sub

    Private Sub txtNoFax_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNoFax.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNoFax.Text <> "" Then
            txtJatuhTempo.Focus()
        End If
    End Sub

    Private Sub txtJatuhTempo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtJatuhTempo.KeyPress
        If Asc(e.KeyChar) <> Asc("-") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtJatuhTempo.Text <> "" Then
            btnSave_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub txtCari_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtCari.Text = ""
        txtCari2.Text = ""
        viewAllData("", "")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        FormMsArea.Show()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        FormMsEkspedisi.Show()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        setCmbArea1()
        setCmbEkspedisi()
    End Sub

    Private Sub txtCari2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCari2.TextChanged
        viewAllData(txtCari.Text, txtCari2.Text, cmbCari.Text, cmbCari2.Text)
    End Sub
End Class
