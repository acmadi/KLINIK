Imports System.Data.SqlClient
Public Class FormFakturManagamen
    Dim status As String
    Dim posisi As Integer = 0
    Dim tab As String
    Dim jumData As Integer = 0
    Dim PK As String = ""
    Dim NamaBarang As String = ""
    Dim Merk As String = ""
    Dim SubCat As String = ""
    Dim kdsales As String = ""
    Dim kdtoko As String = ""
    Dim KomisiSales = 0
    Dim DiscKomisiSales = 0
    Dim jatuhTempo As Integer
    Dim tJatuhTempo As String
    Dim totalKomisiSales = 0
    Dim totalDiscKomisiSales = 0
    Dim is_access As Integer
    Dim tempDisc = 0
    Dim tempHargaDisc = 0
    Dim tempHarga = 0

    Private Sub msgWarning(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Critical, "Warning")
    End Sub

    Private Sub msgInfo(ByVal str As String)
        MsgBox(str, MsgBoxStyle.Information, "Information")
    End Sub

    'FlagStatus : StatusSO,Stock
    Function check_stock(ByVal KdBarang As String, ByVal addQty As String, ByVal FlagStatus As String)
        Return 1
    End Function

    Private Sub emptyField()
        dtpFaktur.Text = Now()
        
    End Sub

    Function getFaktur(Optional ByVal KdFaktur As String = "")
        Dim sql As String = " Select KdFaktur from trfaktur order by no_increment desc "

        If KdFaktur <> "" Then
            sql &= "kdfaktur = '" & KdFaktur & "'"
        End If
        Dim reader = execute_reader(sql)
        Return reader
    End Function

    Private Sub setData()
        Try
            Dim Sales = ""
            Dim Toko = ""
            Dim kdso = ""
            Dim readerSO = execute_reader(" select kdfaktur,DATE_FORMAT(Tanggalfaktur, '%m/%d/%Y') Tanggal, " & _
                                          " so.kdso, MS.KdSales, NamaSales, MT.KdToko, " & _
                                          " NamaToko, MT.Daerah, NamaArea, StatusFaktur " & _
                                          " from trfaktur faktur " & _
                                          " Join TrSalesOrder SO On  SO.kdso = faktur.kdso " & _
                                          " Join MsSales MS On MS.KdSales = SO.KdSales " & _
                                          " Join MsToko MT On MT.KdToko = SO.KdToko " & _
                                          " Join MsArea MP On MP.KdArea = MT.KdArea " & _
                                          " Where kdfaktur = '" & PK & "' ")
            If readerSO.Read Then
                txtID.Text = readerSO.Item("kdfaktur")
                dtpFaktur.Text = readerSO.Item("Tanggal")

                'lblProvinsi.Text = readerSO.Item(8)
                kdso = readerSO.Item("kdso")
                Sales = readerSO.Item("KdSales") & " - " & readerSO.Item("NamaSales")
                Toko = readerSO.Item("KdToko") & " - " & readerSO.Item("NamaToko")
                If readerSO.Item("StatusFaktur") <> 0 Then
                    btnSave.Enabled = False
                    btnConfirms.Enabled = False
                End If
            End If
            readerSO.Close()
          

            Dim reader = execute_reader(" Select MB.KdBarang,NamaBarang,Subkategori, " & _
                                    " Harga,Qty,Disc, " & _
                                    " ifnull(( Select StockAkhir " & _
                                    " from BarangHistory where isActive = 1 " & _
                                    " And KdBarang = faktur.KdBarang " & _
                                    " order by KdBarangHistory desc limit 1 ),0) Stock, " & _
                                    " faktur.StatusBarang,Merk,KomisiSales,HargaDisc,DiscKomisiSales " & _
                                    " from TrFakturDetail faktur " & _
                                    " Join MsBarang MB On faktur.KdBarang = MB.KdBarang " & _
                                    " Join MsMerk On MsMerk.kdMerk = MB.kdMerk " & _
                                    " Join MsSubkategori On MsSubkategori.KdSub = MB.KdSub " & _
                                    " where kdfaktur = '" & PK & "' " & _
                                    " order by NamaBarang asc ")

            gridBarang.Rows.Clear()
            Do While reader.Read
                Dim Subtotal = Val(reader("HargaDisc")) * Val(reader("Qty"))
                Dim StatusBarang = "Ready"

                If Val(reader("StatusBarang")) = 1 Then
                    StatusBarang = "Pending"
                End If

                gridBarang.Rows.Add()
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmPartNo").Value = reader("KdBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmNamaBarang").Value = reader("NamaBarang")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmMerk").Value = reader("Merk")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubKategori").Value = reader("Subkategori")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHarga").Value = FormatNumber(reader("Harga"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmHargaDisc").Value = FormatNumber(reader("HargaDisc"), 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDisc").Value = reader("Disc")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmSubtotal").Value = FormatNumber(Subtotal, 0)
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStock").Value = reader("Stock")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmQty").Value = reader("Qty")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmStatus").Value = StatusBarang
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmKomisi").Value = reader("KomisiSales")
                gridBarang.Rows.Item(gridBarang.RowCount - 1).Cells("clmDiscKomisi").Value = reader("DiscKomisiSales")
            Loop
            reader.Close()

            HitungTotal()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub setGrid()
        With gridBarang.ColumnHeadersDefaultCellStyle
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = New Font(.Font.FontFamily, .Font.Size, _
              .Font.Style Or FontStyle.Bold, GraphicsUnit.Point)
            .ForeColor = Color.Gold

        End With
    End Sub

    Public Sub setCmbSO()
       
    End Sub
    Public Sub setCmbDiskon()
        cmbDiskon.Items.Clear()
        cmbDiskon.Items.Add("- Pilih Diskon -")
        cmbDiskon.Items.Add("0")
        cmbDiskon.Items.Add("5")
        cmbDiskon.Items.Add("10")
        cmbDiskon.Items.Add("15")
        cmbDiskon.SelectedIndex = 0
    End Sub
    Private Sub FormMsSupplier_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        is_access = get_access("faktur")
        PK = data_carier(0)
        status = data_carier(1)
        clear_variable_array()

        emptyField()

        If PK = "" Then
            If is_access = 3 Or is_access = 4 Or is_access = 5 Then
                btnSave.Enabled = True
                btnConfirms.Enabled = True
            End If
            generateCode()
        Else
            If is_access = 2 Or is_access = 4 Or is_access = 5 Then
                btnSave.Enabled = True
                btnConfirms.Enabled = True
            End If
            setData()
            txtID.Text = PK
        End If

        setCmbDiskon()
    End Sub

    Private Sub generateCode()
        Dim code As String = "FK"
        Dim angka As Integer
        Dim kode As String
        Dim temp As String
        Dim tempCompare As String = ""

        Dim bulan As Integer = CInt(Today.Month.ToString)
        Dim currentTime As System.DateTime = System.DateTime.Now
        Dim FormatDate As String = Format(currentTime, ".dd.MM.yy")
        tempCompare &= FormatDate

        Dim reader = getFaktur()

        If reader.read Then
            kode = Trim(reader("KdFaktur").ToString())
            temp = kode.Substring(6, 9)
            If temp = tempCompare Then
                angka = CInt(kode.Substring(2, 4))
            Else
                angka = 0
            End If
            reader.Close()
        Else
            angka = 0
            reader.Close()
        End If
        angka = angka + 1
        Dim len As Integer = angka.ToString().Length
        For i As Integer = 1 To 4 - len
            code += "0"
        Next
        code = code & (angka) & FormatDate
        txtID.Text = Trim(code)
    End Sub

    Function SaveFakturDetail(ByVal flag As String)
        Return 1
    End Function

    Function save(ByVal flag As String)
        If 1 Then


        Else
            Dim Grandtotal = ""

            If (lblGrandtotal.Text <> "") Then
                Grandtotal = lblGrandtotal.Text.ToString.Replace(".", "").Replace(",", "")
            End If

            dbconmanual.Open()
            Dim trans As MySql.Data.MySqlClient.MySqlTransaction

            trans = dbconmanual.BeginTransaction(IsolationLevel.ReadCommitted)

            Try
                If PK = "" Then
                    sql = " insert into trfaktur ( KdFaktur, KdSO, TanggalFaktur, " & _
                          " KdSales, KdToko, GrandTotal, StatusFaktur, " & _
                          " UserID, TotalKomisiSales, TanggalJT, TotalDiscKomisiSales ) values ( " & _
                          " '" + Trim(txtID.Text) + "', " & _
                          " '" & dtpFaktur.Value.ToString("yyyy/MM/dd HH:mm:ss") & "', " & _
                          " '" & Trim(kdsales) & "','" & Trim(kdtoko) & "'," & _
                          " '" & Trim(Grandtotal) & "','" & flag & "','" & kdKaryawan & "', " & _
                          " '" & totalKomisiSales & "','" & tJatuhTempo & "', " & _
                          " '" & totalDiscKomisiSales & "')"
                    execute_update_manual(sql)
                Else
                    sql = " update   trfaktur  set  " & _
                          " TanggalFaktur = '" & dtpFaktur.Value.ToString("yyyy/MM/dd HH:mm:ss") & "'," & _
                          " KdSales = '" & Trim(kdsales) & "'," & _
                          " KdToko = '" & Trim(kdtoko) & "'," & _
                          " GrandTotal = '" & Trim(Grandtotal) & "', " & _
                          " UserID = '" & Trim(kdKaryawan) & "', " & _
                          " TotalKomisiSales = '" & Trim(totalKomisiSales) & "', " & _
                          " StatusFaktur = '" & flag & "', " & _
                          " TotalDiscKomisiSales = '" & Trim(totalDiscKomisiSales) & "', " & _
                          " where  KdFaktur = '" + txtID.Text + "' "
                    execute_update_manual(sql)
                End If

                execute_update_manual("delete from Trfakturdetail where  kdfaktur = '" & txtID.Text & "'")
                SaveFakturDetail(flag)

                trans.Commit()
                msgInfo("Data berhasil disimpan")
                Me.Close()
            Catch ex As Exception
                trans.Rollback()
                MsgBox(ex, MsgBoxStyle.Information)
            End Try
            dbconmanual.Close()
        End If
        Return True
    End Function

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        'save(0)
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub HitungTotal()
        Try
            Dim Grandtotal = 0
            totalKomisiSales = 0
            totalDiscKomisiSales = 0
            If gridBarang.Rows.Count <> 0 Then
                For i As Integer = 0 To (gridBarang.Rows.Count - 1)
                    Dim total = gridBarang.Rows.Item(i).Cells("clmSubtotal").Value.ToString.Replace(".", "").Replace(",", "")
                    Grandtotal = Val(Grandtotal) + Val(total)
                    totalKomisiSales += Val(gridBarang.Rows.Item(i).Cells("clmKomisi").Value)
                    totalDiscKomisiSales += Val(gridBarang.Rows.Item(i).Cells("clmDiscKomisi").Value)
                Next
                totalDiscKomisiSales = Math.Round(totalDiscKomisiSales / gridBarang.Rows.Count, 2)
            End If
            lblGrandtotal.Text = FormatNumber(Grandtotal, 0)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub gridBarang_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs)
        tempDisc = Val(gridBarang.CurrentRow.Cells("clmDisc").Value)
        tempDisc = Math.Round(tempDisc, 2)
        tempHargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
        tempHarga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
    End Sub

    Private Sub gridBarang_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        tempDisc = gridBarang.CurrentRow.Cells("clmDisc").Value * 1
        tempDisc = Math.Round(tempDisc, 2)
        tempHargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))
        tempHarga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
    End Sub

    Private Sub gridBarang_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Try
            Dim BarangID = gridBarang.CurrentRow.Cells("clmPartNo").Value
            Dim harga = Val(gridBarang.CurrentRow.Cells("clmHarga").Value.Replace(".", "").Replace(",", ""))
            Dim qty = Val(gridBarang.CurrentRow.Cells("clmQty").Value)
            Dim disc = Val(gridBarang.CurrentRow.Cells("clmDisc").Value)
            Dim stok = check_stock(BarangID, 0, "Stock")
            Dim hargaDisc = Val(gridBarang.CurrentRow.Cells("clmHargaDisc").Value.Replace(".", "").Replace(",", ""))

            If IsNumeric(qty) = False Or qty < 1 Then
                MsgBox("Jumlah harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                qty = 1
                gridBarang.CurrentRow.Cells("clmQty").Value = 1
                gridBarang.CurrentRow.Cells("clmQty").Selected = True
            ElseIf qty > stok Then
                MsgBox("Jumlah tidak mencukupi persedian. Persedian saat ini adalah " & stok, MsgBoxStyle.Information, "Validation Error")
                qty = 1
                gridBarang.CurrentRow.Cells("clmQty").Value = 1
                gridBarang.CurrentRow.Cells("clmQty").Selected = True
            ElseIf IsNumeric(harga) = False Or harga < 1 Then
                MsgBox("Harga harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                harga = 1
                gridBarang.CurrentRow.Cells("clmHarga").Value = 1
                gridBarang.CurrentRow.Cells("clmHarga").Selected = True
            ElseIf IsNumeric(hargaDisc) = False Or hargaDisc < 1 Then
                MsgBox("Harga Disc harus lebih besar dari 0 dan berupa angka.", MsgBoxStyle.Information, "Validation Error")
                hargaDisc = 1
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = 1
                gridBarang.CurrentRow.Cells("clmHargaDisc").Selected = True
            ElseIf IsNumeric(disc) = False Then
                MsgBox("Diskon harus berupa angka.", MsgBoxStyle.Information, "Validation Error")
                disc = 0
                gridBarang.CurrentRow.Cells("clmDisc").Value = 0
                gridBarang.CurrentRow.Cells("clmDisc").Selected = True
            Else
                Dim TempHarga = FormatNumber(harga, 0)
                Dim TempHargaDisc = FormatNumber(hargaDisc, 0)
                gridBarang.CurrentRow.Cells("clmHarga").Value = TempHarga
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = TempHargaDisc
            End If

            If hargaDisc <> tempHargaDisc Or harga <> tempHarga Then
                Dim calcDisc = 100 - (hargaDisc / harga * 100)
                gridBarang.CurrentRow.Cells("clmDisc").Value = Math.Round(calcDisc, 2)
            ElseIf disc <> tempDisc Then
                Dim discProses = 100 - Math.Round(disc * 1, 2)
                Dim calcHarga = harga * (discProses / 100)
                gridBarang.CurrentRow.Cells("clmHargaDisc").Value = FormatNumber(calcHarga, 0)
            End If

            gridBarang.CurrentRow.Cells("clmSubtotal").Value = FormatNumber(Val(hargaDisc) * Val(qty), 0)

            HitungTotal()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical, "Warning!!!")
        End Try
    End Sub

    Private Sub btnConfirms_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirms.Click
        'save(1)
    End Sub
End Class
