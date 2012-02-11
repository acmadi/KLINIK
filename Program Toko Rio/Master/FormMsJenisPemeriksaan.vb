Imports System.Data.SqlClient
Public Class FormMsJenisPemeriksaan
    Dim LastKode = ""

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub emptyField()
        txtCari.Text = ""
        txtNama.Text = ""
        txtKd.Text = ""
        gridInfoPemeriksaan.Rows.Clear()
        LastKode = ""
        EmptyInfoPemeriksaan()
    End Sub

    Private Function EmptyInfoPemeriksaan()
        txtInfoPemeriksaan.Text = ""
        txtNilaiRujukan.Text = ""
        Return True
    End Function

    Private Sub ubahAktif(ByVal status As Boolean)
        txtKd.Enabled = status
        txtNama.Enabled = status
        txtInfoPemeriksaan.Enabled = status
        txtNilaiRujukan.Enabled = status
        gridInfoPemeriksaan.Enabled = status
        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAddDetail.Enabled = status
        btnRemoveDetail.Enabled = status

        btnExit.Enabled = Not status
        GroupBox1.Enabled = Not status

        btnSave.Enabled = status
        btnCancel.Enabled = status
        btnAdd.Enabled = Not status
        btnUpdate.Enabled = Not status
        btnDelete.Enabled = Not status
        grdDokter.Enabled = Not status
    End Sub

    Private Sub setData()

        If grdDokter.RowCount > 0 Then
            Try
                txtKd.Text = grdDokter.CurrentRow.Cells("Call ID").Value.ToString
                txtNama.Text = grdDokter.CurrentRow.Cells("Jenis Pemeriksaan").Value.ToString
                sql = " SELECT KdJenisPemeriksaan, " & _
                      " NamaDetailJenisPemeriksaan, NilaiRujukan " & _
                      " FROM msdetailjenispemeriksaan " & _
                      " WHERE IsAktif = 1 " & _
                      " AND KdJenisPemeriksaan = '" & txtKd.Text & "' "
                Dim reader = execute_reader(sql)
                EmptyInfoPemeriksaan()
                gridInfoPemeriksaan.Rows.Clear()
                Do While reader.Read
                    gridInfoPemeriksaan.Rows.Add()
                    gridInfoPemeriksaan.Rows.Item(gridInfoPemeriksaan.RowCount - 1).Cells("clmInfoPemeriksaan").Value = reader("NamaDetailJenisPemeriksaan")
                    gridInfoPemeriksaan.Rows.Item(gridInfoPemeriksaan.RowCount - 1).Cells("clmNilaiRujukan").Value = reader("NilaiRujukan")
                Loop
                LastKode = txtKd.Text
            Catch ex As Exception
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
        End If
    End Sub

    Private Sub setCmbCari()
        cmbCari.Items.Clear()
        cmbCari.Items.Add("Call ID")
        cmbCari.Items.Add("Jenis Pemeriksaan")
        cmbCari.SelectedIndex = 0
    End Sub

    Private Sub viewAllData(ByVal cr As String, ByVal opt As String)
        sql = " SELECT 	KdJenisPemeriksaan 'Call ID', " & _
              " NamaJenisPemeriksaan 'Jenis Pemeriksaan' " & _
              " FROM msjenispemeriksaan " & _
              " WHERE IsAktif = 1 "

        If opt <> "" Then
            Dim col As String = ""
            If opt = "Call ID" Then
                col = "KdJenisPemeriksaan"
            ElseIf opt = "Jenis Pemeriksaan" Then
                col = "NamaJenisPemeriksaan"
            End If
            sql &= " AND " & col & "  like '%" & cr & "%' "
        End If
        sql &= " ORDER BY NamaJenisPemeriksaan "

        grdDokter.DataSource = execute_datatable(sql)
        setGrid()
        setData()
    End Sub

    Private Sub setGrid()
        With grdDokter.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
        grdDokter.Columns("Jenis Pemeriksaan").Width = 150

    End Sub

    Private Sub FormMsSales_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        emptyField()
        ubahAktif(False)
        viewAllData("", "")
        setCmbCari()
    End Sub

    Private Sub generateCode()
        Dim code As String = "JP"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        code += Today.Year.ToString.Substring(2, 2)
        Dim bulan As Integer = CInt(Today.Month.ToString)

        If bulan < 10 Then
            code += "0" + bulan.ToString
        Else
            code += bulan.ToString
        End If

        Dim reader = getCode("msjenispemeriksaan", "KdJenisPemeriksaan")

        If reader.read Then
            kode = Trim(reader(0).ToString())
            temp = kode.Substring(0, 6)
            If temp = code Then
                angka = CInt(kode.Substring(6, 4))
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
        txtKd.Text = Trim(code)
    End Sub

    Private Sub txtCari_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtCari.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
    End Sub

    Private Sub txtCari_TextChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCari.TextChanged
        viewAllData(txtCari.Text, cmbCari.Text)
    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdDokter.CellClick, grdDokter.CellClick
        If e.RowIndex >= 0 And e.RowIndex <= grdDokter.Rows.Count - 1 Then
            setData()
        End If
    End Sub

    Private Sub btnReset_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        txtCari.Text = ""
        viewAllData("", "")
        txtCari.Focus()
    End Sub

    Private Sub txtNama_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNama.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNama.Text <> "" Then
            txtInfoPemeriksaan.Focus()
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        emptyField()
        ubahAktif(True)
        generateCode()
        txtKd.Focus()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If grdDokter.RowCount Then
            ubahAktif(True)
            txtNama.Focus()
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If grdDokter.RowCount Then
            Dim selected_cell = grdDokter.CurrentRow.Cells("Call ID").Value
            If MsgBox("Anda yakin ingin menghapus Call ID " & selected_cell & "?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                execute_update(" UPDATE msjenispemeriksaan SET " & _
                               " IsAktif = 0 " & _
                               " WHERE KdJenisPemeriksaan = '" & selected_cell & "' ")
                msgInfo("Data berhasil dihapus")
                viewAllData("", "")
            End If
        Else
            msgInfo("Data tidak ditemukan.")
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim addQuery = ""
        Dim readUsedCode = execute_reader(" SELECT 1 FROM msjenispemeriksaan " & _
                                           " WHERE KdJenisPemeriksaan = '" & txtKd.Text & "' " & _
                                           " AND KdJenisPemeriksaan <> '" & LastKode & "' ")
        If txtKd.Text = "" Then
            msgInfo("Call ID harus diisi")
            txtKd.Focus()
        ElseIf readUsedCode.read Then
            msgInfo("Call ID sudah terdaftar. Silakan ganti yang lain.")
            txtKd.Focus()
        ElseIf txtNama.Text = "" Then
            msgInfo("Jenis Pemeriksaan harus diisi")
            txtNama.Focus()
        Else
            If LastKode = "" Then
                LastKode = txtKd.Text
            End If

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction
            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            sql = " INSERT INTO msjenispemeriksaan ( " & _
                  "     KdJenisPemeriksaan, NamaJenisPemeriksaan, " & _
                  "     Created " & _
                  " ) values( " & _
                  "     '" & Trim(LastKode) & "', '" & Trim(txtNama.Text) & "', " & _
                  "     now() " & _
                  " )  ON DUPLICATE KEY UPDATE " & _
                  " KdJenisPemeriksaan = '" & Trim(txtKd.Text) & "', " & _
                  " NamaJenisPemeriksaan = '" & Trim(txtNama.Text) & "' "
            Try
                execute_update_manual(sql)

                sql = " UPDATE msdetailjenispemeriksaan SET " & _
                      " IsAktif = 0 " & _
                      " WHERE KdJenisPemeriksaan = '" & Trim(txtKd.Text) & "' "
                execute_update_manual(sql)

                For i As Integer = 0 To gridInfoPemeriksaan.RowCount - 1
                    Dim InfoPemeriksaan = gridInfoPemeriksaan.Rows.Item(i).Cells("clmInfoPemeriksaan").Value
                    Dim NilaiRujukan = gridInfoPemeriksaan.Rows.Item(i).Cells("clmNilaiRujukan").Value
                    Dim sqlDetail = " INSERT INTO msdetailjenispemeriksaan ( " & _
                                    "   KdJenisPemeriksaan, " & _
                                    "   NamaDetailJenisPemeriksaan, " & _
                                    "   NilaiRujukan, Created " & _
                                    " ) values( " & _
                                    " '" & Trim(txtKd.Text) & "', " & _
                                    " '" & Trim(InfoPemeriksaan) & "', " & _
                                    " '" & Trim(NilaiRujukan) & "', now() ) " & _
                                    " ON DUPLICATE KEY UPDATE " & _
                                    " KdJenisPemeriksaan = '" & Trim(txtKd.Text) & "', " & _
                                    " NamaDetailJenisPemeriksaan = '" & Trim(InfoPemeriksaan) & "', " & _
                                    " NilaiRujukan = '" & Trim(NilaiRujukan) & "', " & _
                                    " IsAktif = '1' "
                    execute_update_manual(sqlDetail)
                Next

                trans.Commit()
                viewAllData("", "")
                ubahAktif(False)
                msgInfo("Data berhasil disimpan")
            Catch ex As Exception
                trans.Rollback()
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
            readUsedCode.close()
        End If
        dbconmanual.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ubahAktif(False)
        setData()
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub txtKd_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtKd.KeyPress, txtKd.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtKd.Text <> "" Then
            txtNama.Focus()
        End If
    End Sub

    Private Sub cmbCari_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbCari.SelectedIndexChanged
        btnReset.PerformClick()
    End Sub

    Private Sub txtInfoPemeriksaan_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtInfoPemeriksaan.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtInfoPemeriksaan.Text <> "" Then
            txtNilaiRujukan.Focus()
        End If
    End Sub

    Private Sub txtNilaiRujukan_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNilaiRujukan.KeyPress
        If Asc(e.KeyChar) = Asc("'") And AscW(e.KeyChar) <> 13 And AscW(e.KeyChar) <> 8 Then
            e.KeyChar = Nothing
        End If
        If AscW(e.KeyChar) = 13 And txtNilaiRujukan.Text <> "" Then
            btnAddDetail.PerformClick()
        End If
    End Sub

    Private Sub btnAddDetail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddDetail.Click
        Try
            If Trim(txtInfoPemeriksaan.Text) = "" Then
                msgInfo("Informasi Pemeriksaan harus diisi")
                txtInfoPemeriksaan.Focus()
            Else
                gridInfoPemeriksaan.Rows.Add()
                gridInfoPemeriksaan.Rows.Item(gridInfoPemeriksaan.RowCount - 1).Cells("clmInfoPemeriksaan").Value = Trim(txtInfoPemeriksaan.Text)
                gridInfoPemeriksaan.Rows.Item(gridInfoPemeriksaan.RowCount - 1).Cells("clmNilaiRujukan").Value = Trim(txtNilaiRujukan.Text)
                EmptyInfoPemeriksaan()
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnRemoveDetail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveDetail.Click
        If gridInfoPemeriksaan.RowCount <> 0 Then
            gridInfoPemeriksaan.Rows.RemoveAt(gridInfoPemeriksaan.CurrentRow.Index)
        End If
    End Sub
End Class
