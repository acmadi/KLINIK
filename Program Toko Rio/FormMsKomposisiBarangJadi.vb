Imports System.Data, System.Data.SqlClient

Public Class FormMsKomposisiBarangJadi
    Dim posisi As Integer = 0
    Dim IDROW As String
    Dim kd As String
    Dim act As String
    Dim jumData As Integer
    Dim query As String
    Dim tabel As String
    Dim idmenu As String
    Dim idbarang As String
    Dim PK As String
    Private Sub initGrid()

        query = "SELECT * " & _
           "  from  " & tabel
        If kd <> "" Then
            query &= "  where KdBarang='" & kd & "'"
        End If
        DataGridView1.DataSource = execute_datatable(query)
    End Sub

    Private Sub setTextEnabled(ByVal b As Boolean)
        txtNama.Enabled = False
        cmbBarang.Enabled = b
        txtJUmlah.Enabled = b
        GroupBox1.Enabled = Not b
        DataGridView1.Enabled = Not b
    End Sub

    Private Sub clearFields()
        txtNama.Text = ""
        txtJUmlah.Text = ""
        cmbBarang.Text = ""
        cmbmenu.Text = ""
    End Sub

    Private Sub setTextEmpty()
        txtNama.Text = ""
        txtJUmlah.Text = ""
    End Sub

    Private Sub setData(ByVal row As String)
        Try
            txtNama.Text = DataGridView1.Rows(row).Cells(0).Value.ToString
            IDROW = txtNama.Text
            cmbBarang.Text = DataGridView1.Rows(row).Cells(2).Value.ToString
            txtJUmlah.Text = DataGridView1.Rows(row).Cells(3).Value.ToString
            idmenu = DataGridView1.Rows(row).Cells(1).Value.ToString
            idbarang = DataGridView1.Rows(row).Cells(2).Value.ToString
        Catch ex As Exception
        End Try
    End Sub

    Private Sub fillCMBM()
        cmbProduk.Items.Clear()
        Dim reader = execute_reader("Select kdbarang,namabarang from msbarang order by NamaBarang asc  ")
        Do While reader.Read
            cmbProduk.Items.Add(reader.GetString(0) & " -  " & reader.GetString(1))
        Loop
        reader.Close()
        If cmbProduk.Items.Count > 0 Then
            cmbProduk.SelectedIndex = 0
        End If
    End Sub


    Private Sub fillCMBBRG()
        cmbBarang.Items.Clear()
        Dim reader = execute_reader("Select * from msfising order by TypeFising asc  ")
        Do While reader.Read
            cmbBarang.Items.Add(reader.GetString(0) & " -  " & reader.GetString(1))
        Loop
        reader.Close()
        If cmbBarang.Items.Count > 0 Then
            cmbBarang.SelectedIndex = 0
        End If
    End Sub
    Private Sub awal()
        tabel = " MsKomposisiBarang "
        setTextEnabled(False)

        initGrid()
        fillCMBM()
        Button3.Enabled = False
        Button14.Enabled = False
        txtNama.Text = ""
        txtJUmlah.Text = ""
        cmbBarang.Text = ""
        txtNama.Enabled = False
        txtJUmlah.Enabled = False
        cmbBarang.Enabled = False
        cmbmenu.Visible = False
    End Sub
    Private Sub FormStrukturProduk_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        awal()
    End Sub

    Private Sub DataGridView1_CellClick1(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            setData(e.RowIndex)
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Me.Hide()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If IDROW = "" Then
            MsgBox("Barang jadi yang ingin diubah harus dipilih", MsgBoxStyle.Information)
        Else
            act = "u"
            setTextEnabled(True)
            Button3.Enabled = True
            Button14.Enabled = True
            Button1.Enabled = False
            Button2.Enabled = False
            cmbBarang.Enabled = False
            Button10.Enabled = False
            GroupBox2.Text = "Ubah Komposisi Barang Jadi"
        End If
    End Sub
    Private Function checkEmpty() As Boolean
        If act = "t" And cmbmenu.Text = "" Then
            MsgBox("Menu harus dipilih !", MsgBoxStyle.Critical)
            cmbmenu.Focus()
            Return False
        ElseIf act = "t" And cmbBarang.Text = "" Then
            MsgBox("Barang harus dipilih !", MsgBoxStyle.Critical)
            cmbBarang.Focus()
            Return False
            'ElseIf act = "u" And cmbBarang.Text = "" Then
            '    MsgBox("Barang harus dipilih !", MsgBoxStyle.Critical)
            '    cmbBarang.Focus()
            '    Return False
        ElseIf txtJUmlah.Text = "" Then
            MsgBox("Jumlah harus diisi !", MsgBoxStyle.Critical)
            txtJUmlah.Focus()
            Return False
        ElseIf txtJUmlah.Text = 0 Then
            MsgBox("Jumlah tidak boleh 0 !", MsgBoxStyle.Critical)
            txtJUmlah.Text = ""
            txtJUmlah.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        If act = "u" Then
            query = "update   " & tabel & "   set " & _
            " qty=" & txtJUmlah.Text & "" & _
            "   where kdfising ='" & idbarang & "' and  kdbarang= '" & idmenu & "'"
            If checkEmpty() Then
                execute_update(query)
                initGrid()
                Button1.Enabled = True
                Button2.Enabled = True
                Button3.Enabled = False
                Button14.Enabled = False
                setTextEnabled(False)
                Button10.Enabled = True
                GroupBox2.Text = ""
                IDROW = ""
                act = ""
                clearFields()
                txtNama.Visible = True
                MsgBox("Data berhasil diupdate")
            End If
        Else

            Dim reader = execute_reader("Select * from " & tabel & " where kdbarang='" & cmbmenu.Text.Substring(0, 10) & "' and kdfising='" & cmbBarang.Text.Substring(0, 10) & "' ")
            If reader.Read Then
                reader.Close()
                txtJUmlah.Text = ""
                MsgBox("Data telah ada", MsgBoxStyle.Information)
            Else
                reader.Close()
                query = "insert into  " & tabel & " (KdBarang,kdfising,qty)  values('" & cmbmenu.Text.Substring(0, 10) & "'," & _
                "'" & cmbBarang.Text.Substring(0, 5) & "'," & txtJUmlah.Text & ")"
                MsgBox(query)
                If checkEmpty() Then
                    execute_update(query)
                    initGrid()
                    Button1.Enabled = True
                    Button2.Enabled = True
                    Button3.Enabled = False
                    Button14.Enabled = False
                    setTextEnabled(False)
                    txtNama.Visible = True
                    cmbmenu.Visible = False
                    Button10.Enabled = True
                    GroupBox2.Text = ""
                    IDROW = ""
                    act = ""
                    clearFields()
                    MsgBox("Data berhasil disimpan")
                End If
            End If
        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Button3.Enabled = False
        Button14.Enabled = False
        Button1.Enabled = True
        Button2.Enabled = True
        setTextEnabled(False)
        Button10.Enabled = True
        IDROW = ""
        cmbmenu.Visible = False
    End Sub
    Private Sub fillCMBMenu()
        cmbmenu.Items.Clear()
        Dim reader = execute_reader("Select kdbarang,namabarang from msbarang order by namabarang asc  ")
        Do While reader.Read
            cmbmenu.Items.Add(reader.GetString(0) & " -  " & reader.GetString(1))
        Loop
        reader.Close()
        If cmbmenu.Items.Count > 0 Then
            cmbmenu.SelectedIndex = 0
        End If
    End Sub
    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        act = "t"
        setTextEmpty()
        setTextEnabled(True)
        Button3.Enabled = True

        Button1.Enabled = False
        Button2.Enabled = False
        Button14.Enabled = True
        fillCMBBRG()
        txtNama.Visible = False
        cmbmenu.Visible = True
        fillCMBMenu()
        Button10.Enabled = False
        GroupBox2.Text = "Tambah kompisisi barang"
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        If txtNama.Text = "" Then
            MsgBox("KOmposisi yang ingin dihapus harus dipilih", MsgBoxStyle.Information)
        Else

            If MsgBox("Apakah Anda yakin ingin menghapus data ini ?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                If idmenu = "" Or idbarang = "" Then
                    MsgBox("Menu yang ingin dihapus harus dipilih", MsgBoxStyle.Information)
                Else
                    query = "delete from " & tabel & " where kdbarang='" & idmenu & "' and kdfising='" & idbarang & "'"
                    execute_update(query)
                    initGrid()
                    clearFields()
                    IDROW = ""
                    idmenu = ""
                    idbarang = ""
                    MsgBox("Data berhasil dihapus")


                End If
            End If
        End If
    End Sub

    Private Sub cmbProduk_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbProduk.SelectedIndexChanged
        kd = cmbProduk.Text.Substring(0, 10)
        initGrid()
    End Sub
    Private Sub txtJUmlah_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtJUmlah.KeyPress
        If AscW(e.KeyChar) <> 8 And (Asc(e.KeyChar) < Asc("0") Or Asc(e.KeyChar) > Asc("9")) Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub
End Class