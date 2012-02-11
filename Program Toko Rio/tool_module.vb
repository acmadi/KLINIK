Imports System.Data.SqlClient
Module tool_module
    Public form 'variable for main form management
    Public opened_page As String 'variable for current opened page
    Public sub_form 'variable for sub form management

    'function for closing main page management
    Function close_allpage()
        Try
            form.close()
        Catch ex As NullReferenceException
            MsgBox("There's an error occur while trying to close the page. Please try again.", MsgBoxStyle.OkOnly, "Error")
            opened_page = ""
            Return False
        End Try
        Return True
    End Function

    Function clear_variable_array()
        For i As Integer = 1 To 7
            data_carier(i) = ""
        Next
        Return True
    End Function

    'function for opening main page management
    Function open_page(ByVal page_to_open As String)
        If opened_page <> "" Then
            Try
                form.Close()
            Catch ex As NullReferenceException
                MsgBox("There's an error occur while trying to open the page. Please try again.", MsgBoxStyle.OkOnly, "Error")
                opened_page = ""
                Return False
            End Try
        End If
        If page_to_open = "FormMsDistributor" Then
            form = New FormMsDistributor
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsDistributor"
        ElseIf page_to_open = "FormMsReagen" Then
            form = New FormMsReagen
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsReagen"
        ElseIf page_to_open = "FormMsObat" Then
            form = New FormMsObat
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsObat"
        ElseIf page_to_open = "FormMsDokter" Then
            form = New FormMsDokter
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsDokter"
        ElseIf page_to_open = "FormMsPasien" Then
            form = New FormMsPasien
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsPasien"
        ElseIf page_to_open = "FormMsSupplier" Then
            form = New FormMsSupplier
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsSupplier"
        ElseIf page_to_open = "FormMsFarmasi" Then
            form = New FormMsFarmasi
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsFarmasi"
        ElseIf page_to_open = "FormMsElektromedik" Then
            form = New FormMsElektromedik
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsElektromedik"
        ElseIf page_to_open = "FormMsJenisPemeriksaan" Then
            form = New FormMsJenisPemeriksaan
            form.MdiParent = FormMain
            form.Show()
            opened_page = "FormMsJenisPemeriksaan"
        End If
        Return True
    End Function

    'function for opening sub page management
    Function open_subpage(ByVal subpage_to_open As String, Optional ByVal detail_id As String = "")
        If subpage_to_open = "FormBrowseBarang" Then
            sub_form = New FormBrowseBarang
            data_carier(0) = detail_id
            sub_form.ShowDialog(FormMain)
            data_carier(0) = ""
            sub_form = ""
        ElseIf subpage_to_open = "FormFakturManagamen" Then
            sub_form = New FormFakturManagamen
            data_carier(0) = detail_id
            If (detail_id <> "") Then
                data_carier(1) = "update"
            Else
                data_carier(1) = "add"
            End If
            sub_form.ShowDialog(FormMain)
            data_carier(0) = ""
            data_carier(1) = ""
            sub_form = ""
        End If
        Return True
    End Function

    'Qty yang akan di input ke history
    'Op : Operator Min Atau Plus
    'Attribute : Attribute Table yang akan di insert
    Function BarangHistory(ByVal Qty As String, ByVal Op As String, ByVal Attribute As String, ByVal KdBarang As String, ByVal HargaAwal As String, ByVal HargaAkhir As String, Optional ByVal KdTrans As String = "", Optional ByVal Modul As String = "", Optional ByVal StatusBarangList As String = "0", Optional ByVal StockAkhir As Integer = 0)
        If Qty <> 0 Then
            Dim StockAwal = StockAkhir

            If Op = "Min" Then
                StockAkhir = Val(StockAkhir) - Val(Qty)
            ElseIf Op = "Plus" Then
                If StatusBarangList = 0 Then
                    StockAkhir = Val(StockAkhir) + Val(Qty)
                Else
                    StockAkhir = Val(StockAkhir)
                End If
            End If

            Dim sqlHistory = "insert into BarangHistory(KdBarang,UserID,TanggalHistory,StockAwal, " & _
                              Attribute & ",StockAkhir,HargaAwal,HargaAkhir,Module,RefNumber,StatusBarangList) " & _
                              "values('" + KdBarang + "','" & kdKaryawan & "',now(), " & _
                              "'" & StockAwal & "','" & Qty & "', " & _
                              "'" & Trim(StockAkhir) & "','" & Trim(HargaAwal) & "','" & Trim(HargaAkhir) & "', " & _
                              "'" & Modul & "','" & Trim(KdTrans) & "','" & Trim(StatusBarangList) & "')"

            execute_update_manual(sqlHistory)
        End If
        Return True
    End Function

    'Qty yang akan di input ke history
    'Op : Operator Min Atau Plus
    'Attribute : Attribute Table yang akan di insert
    Function BahanMentahHistory(ByVal Qty As String, ByVal Op As String, ByVal Attribute As String, ByVal KdBahanMentah As String, ByVal HargaAwal As String, ByVal HargaAkhir As String, Optional ByVal KdTrans As String = "", Optional ByVal Modul As String = "", Optional ByVal StatusBarangList As String = "0", Optional ByVal StockAkhir As Integer = 0)
        If Qty <> 0 Then
            Dim StockAwal = StockAkhir

            If Op = "Min" Then
                StockAkhir = Val(StockAkhir) - Val(Qty)
            ElseIf Op = "Plus" Then
                StockAkhir = Val(StockAkhir) + Val(Qty)
            End If

            Dim sqlHistory = "insert into BahanMentahHistory(KdBahanMentah,UserID,TanggalHistory,StockAwal, " & _
                              Attribute & ",StockAkhir,HargaAwal,HargaAkhir,Module,RefNumber,StatusBahanMentahList) " & _
                              "values('" + KdBahanMentah + "','" & kdKaryawan & "',now(), " & _
                              "'" & StockAwal & "','" & Qty & "', " & _
                              "'" & Trim(StockAkhir) & "','" & Trim(HargaAwal) & "','" & Trim(HargaAkhir) & "', " & _
                              "'" & Modul & "','" & Trim(KdTrans) & "','" & Trim(StatusBarangList) & "')"

            execute_update_manual(sqlHistory)
        End If
        Return True
    End Function

    'Qty yang akan di input ke history
    'Op : Operator Min Atau Plus
    'Attribute : Attribute Table yang akan di insert
    Function StockBarang(ByVal Qty As String, ByVal Op As String, ByVal Harga As String, ByVal KdBarang As String, ByVal Attribute As String, Optional ByVal KdTrans As String = "", Optional ByVal Modul As String = "", Optional ByVal StatusBarangList As String = "0")
        Dim CurrentQty = 0
        Dim sqlHistory = ""
        Dim cnt = 0
        Dim StockAkhir = 0
        Dim readerCnt = execute_reader("Select count(*) cnt from MsBarangList Where KdBarang = '" & KdBarang & "' And StatusBarangList = 0 ")

        If readerCnt.Read Then
            cnt = readerCnt(0)
        End If
        readerCnt.Close()
        Dim qty_arr(cnt)
        Dim KdList_arr(cnt)
        Dim harga_arr(cnt)

        readerCnt.Close()

        Dim readerStock = execute_reader("Select StockAwal,StockAkhir from baranghistory where isActive = 1 And KdBarang = '" & KdBarang & "' order by KdBarangHistory desc limit 1")
        Do While readerStock.Read
            StockAkhir = readerStock(1)
        Loop
        readerStock.Close()

        If Op = "Min" Then        
            Dim reader = execute_reader("Select Qty,KdList,harga from MsBarangList Where KdBarang = '" & KdBarang & "' And StatusBarangList = 0 order by KdList asc")
            Dim no = 0
            Do While reader.Read
                qty_arr(no) = reader(0)
                KdList_arr(no) = reader(1)
                harga_arr(no) = reader(2)
                no += 1
            Loop
            reader.Close()
            For i As Integer = 0 To no - 1
                If Val(Qty) >= Val(qty_arr(i)) Then
                    execute_update_manual("Delete from MsBarangList where KdList = " & KdList_arr(i))
                    CurrentQty += qty_arr(i)
                ElseIf Val(Qty) < Val(qty_arr(i)) Then
                    sqlHistory = "Update MsBarangList set Qty = Qty - " & Val(Qty) & " where KdList = '" & KdList_arr(i) & "'"
                    execute_update_manual(sqlHistory)
                    CurrentQty = Qty
                End If
                BarangHistory(Math.Abs(Val(CurrentQty)), Op, Trim(Attribute), KdBarang, harga_arr(i), Harga, KdTrans, Modul, 0, StockAkhir)
                If Qty = CurrentQty Then
                    Return True
                Else
                    Qty = Qty - Val(qty_arr(i))
                    StockAkhir -= qty_arr(i)
                End If
            Next
        ElseIf Op = "Plus" Then
            sqlHistory = "Insert into MsBarangList(KdBarang,Harga,Qty,StatusBarangList) values('" & KdBarang & "'," & Harga & "," & Qty & "," & StatusBarangList & ")"
            execute_update_manual(sqlHistory)
            BarangHistory(Val(Qty), Op, Trim(Attribute), KdBarang, Harga, 0, KdTrans, Modul, StatusBarangList, StockAkhir)
        End If
        Return True
    End Function

    Function StockBahanMentah(ByVal Qty As String, ByVal Op As String, ByVal Harga As String, ByVal KdBahanMentah As String, ByVal Attribute As String, Optional ByVal KdTrans As String = "", Optional ByVal Modul As String = "", Optional ByVal StatusBarangList As String = "0")
        Dim CurrentQty = 0
        Dim sqlHistory = ""
        Dim cnt = 0
        Dim StockAkhir = 0
        Dim readerCnt = execute_reader("Select count(*) cnt from MsBahanMentahList Where KdBahanMentah = '" & KdBahanMentah & "'  And StatusBahanMentahList = 0 ")

        If readerCnt.Read Then
            cnt = readerCnt(0)
        End If
        readerCnt.Close()
        Dim qty_arr(cnt)
        Dim KdList_arr(cnt)
        Dim harga_arr(cnt)

        readerCnt.Close()

        Dim readerStock = execute_reader("Select StockAwal,StockAkhir from bahanmentahhistory where isActive = 1 And KdBahanMentah = '" & KdBahanMentah & "' And StatusBahanMentahList = 0 order by KdHistory desc limit 1")
        Do While readerStock.Read
            StockAkhir = readerStock(1)
        Loop
        readerStock.Close()

        If Op = "Min" Then
            Dim reader = execute_reader("Select Qty,KdBahanMentahList,harga from MsBahanMentahList Where KdBahanMentah = '" & KdBahanMentah & "' order by KdBahanMentahList asc")
            Dim no = 0
            Do While reader.Read
                qty_arr(no) = reader(0)
                KdList_arr(no) = reader(1)
                harga_arr(no) = reader(2)
                no += 1
            Loop
            reader.Close()

            For i As Integer = 0 To no - 1
                If Val(Qty) >= Val(qty_arr(i)) Then
                    execute_update_manual("Delete from MsBahanMentahList where KdBahanMentahList = " & KdList_arr(i))
                    CurrentQty += qty_arr(i)
                ElseIf Val(Qty) < Val(qty_arr(i)) Then
                    sqlHistory = "Update MsBahanMentahList set Qty = Qty - " & Val(Qty) & " where KdBahanMentahList = '" & KdList_arr(i) & "'"
                    execute_update_manual(sqlHistory)
                    CurrentQty = Qty
                End If
                BahanMentahHistory(Math.Abs(Val(CurrentQty)), Op, Trim(Attribute), KdBahanMentah, harga_arr(i), Harga, KdTrans, Modul, 0, StockAkhir)
                If Qty = CurrentQty Then
                    Return True
                Else
                    Qty = Qty - Val(qty_arr(i))
                    StockAkhir -= qty_arr(i)
                End If
            Next
        ElseIf Op = "Plus" Then
            sqlHistory = "Insert into MsBahanMentahList(KdBahanMentah,Harga,Qty,StatusBahanMentahList) values('" & KdBahanMentah & "'," & Harga & "," & Qty & "," & StatusBarangList & ")"
            execute_update_manual(sqlHistory)

            BahanMentahHistory(Val(Qty), Op, Trim(Attribute), KdBahanMentah, Harga, 0, KdTrans, Modul, StatusBarangList, StockAkhir)
        End If
        Return True
    End Function

    Function check_code(ByVal ID As String, ByVal CurrentID As String, ByVal NamaTable As String, ByVal PrimaryKey As String, ByVal type As String)
        Dim addQuery = ""
        If type.ToString.ToLower = "update" Then
            addQuery = " And " & PrimaryKey & " <> '" & CurrentID & "' "
        End If
        Dim reader = execute_reader(" Select 1 from " & NamaTable & " " & _
                                " where " & PrimaryKey & " = '" & ID & "' " & _
                                addQuery)
        If reader.Read Then
            reader.Close()
            Return True
        Else
            reader.Close()
            Return False
        End If
        Return False
    End Function

    Function getCode(ByVal NamaTable As String, ByVal PrimaryKey As String)
        Dim sql As String = "Select " & PrimaryKey & " from " & NamaTable & " Order By " & PrimaryKey & " desc limit 1"
        Dim reader = execute_reader(sql)
        Return reader
    End Function
    
    Function QueryLevel(ByVal lvlID As Integer, Optional ByVal clmName As String = "", Optional ByVal Attribute As String = "LevelID")
        'Dim query As String = ""
        'If lvlID = lvlStaffRio Then
        '    query = " And IfNull(" & clmName & Attribute & "," & lvlID & ") in (" & lvlID & "," & lvlSuperuser & ") "
        'ElseIf lvlID = lvlStaffLain Then
        '    query = " And IfNull(" & clmName & Attribute & "," & lvlID & ") = " & lvlID & " "
        'End If
        'Return query
        Return True
    End Function

    Public Sub dropviewM(ByVal viewname As String)
        Try
            Dim sql As String = " drop view  " & viewname
            execute_update(sql)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub createviewM(ByVal q As String, ByVal viewName As String)
        Try
            Dim sql As String
            sql = " create view " & viewName & " as (" & q & " )"
            '    TextBox1.Text = sql
            execute_update(sql)
            ' MsgBox("1")
        Catch ex As Exception
        End Try
    End Sub


    Function FisingHistory(ByVal Qty As String, ByVal Op As String, ByVal Attribute As String, ByVal KdBarang As String, ByVal HargaAwal As String, ByVal HargaAkhir As String, Optional ByVal KdTrans As String = "", Optional ByVal Modul As String = "", Optional ByVal StatusBarangList As String = "0", Optional ByVal StockAkhir As Integer = 0)
        If Qty <> 0 Then
            Dim StockAwal = StockAkhir

            If Op = "Min" Then
                StockAkhir = Val(StockAkhir) - Val(Qty)
            ElseIf Op = "Plus" Then
                If StatusBarangList = 0 Then
                    StockAkhir = Val(StockAkhir) + Val(Qty)
                Else
                    StockAkhir = Val(StockAkhir)
                End If
            End If

            Dim sqlHistory = "insert into FisingHistory(KdFising,UserID,TanggalHistory,StockAwal, " & _
                              Attribute & ",StockAkhir,HargaAwal,HargaAkhir,Module,RefNumber,StatusFisingList) " & _
                              "values('" + KdBarang + "','" & kdKaryawan & "',now(), " & _
                              "'" & StockAwal & "','" & Qty & "', " & _
                              "'" & Trim(StockAkhir) & "','" & Trim(HargaAwal) & "','" & Trim(HargaAkhir) & "', " & _
                              "'" & Modul & "','" & Trim(KdTrans) & "','" & Trim(StatusBarangList) & "')"

            execute_update_manual(sqlHistory)
        End If
        Return True
    End Function
    Function StockFising(ByVal Qty As String, ByVal Op As String, ByVal Harga As String, ByVal KdBarang As String, ByVal Attribute As String, Optional ByVal KdTrans As String = "", Optional ByVal Modul As String = "", Optional ByVal StatusBarangList As String = "0")
        Dim CurrentQty = 0
        Dim sqlHistory = ""
        Dim cnt = 0
        Dim StockAkhir = 0
        Dim readerCnt = execute_reader("Select count(*) cnt from MsFisingList Where KdFising = '" & KdBarang & "' And StatusFisingList = 0 ")

        If readerCnt.Read Then
            cnt = readerCnt(0)
        End If
        readerCnt.Close()
        Dim qty_arr(cnt)
        Dim KdList_arr(cnt)
        Dim harga_arr(cnt)

        readerCnt.Close()

        Dim readerStock = execute_reader("Select StockAwal,StockAkhir from fisinghistory where isActive = 1 And KdFising = '" & KdBarang & "' order by KdFisingHistory desc limit 1")
        Do While readerStock.Read
            StockAkhir = readerStock(1)
        Loop
        readerStock.Close()

        If Op = "Min" Then
            Dim reader = execute_reader("Select Qty,KdList,harga from MsFisingList Where KdFising = '" & KdBarang & "' And StatusFisingList = 0 order by KdList asc")
            Dim no = 0
            Do While reader.Read
                qty_arr(no) = reader(0)
                KdList_arr(no) = reader(1)
                harga_arr(no) = reader(2)
                no += 1
            Loop
            reader.Close()

            For i As Integer = 0 To no - 1
                If Val(Qty) >= Val(qty_arr(i)) Then
                    execute_update_manual("Delete from MsFisingList where KdList = " & KdList_arr(i))
                    CurrentQty += qty_arr(i)
                ElseIf Val(Qty) < Val(qty_arr(i)) Then
                    sqlHistory = "Update MsFisingList set Qty = Qty - " & Val(Qty) & " where KdList = '" & KdList_arr(i) & "'"
                    execute_update_manual(sqlHistory)
                    CurrentQty = Qty
                End If
                FisingHistory(Math.Abs(Val(CurrentQty)), Op, Trim(Attribute), KdBarang, harga_arr(i), Harga, KdTrans, Modul, 0, StockAkhir)
                If Qty = CurrentQty Then
                    Return True
                Else
                    Qty = Qty - Val(qty_arr(i))
                    StockAkhir -= qty_arr(i)
                End If
            Next
        ElseIf Op = "Plus" Then
            sqlHistory = "Insert into MsFisingList(KdFising,Harga,Qty,StatusFisingList) values('" & KdBarang & "'," & Harga & "," & Qty & "," & StatusBarangList & ")"
            execute_update_manual(sqlHistory)
            FisingHistory(Val(Qty), Op, Trim(Attribute), KdBarang, Harga, 0, KdTrans, Modul, StatusBarangList, StockAkhir)
        End If
        Return True
    End Function

    Public Function get_access(ByVal Access_name)
        Dim sql_access = execute_reader("Select Access From authorization " _
                                        & " Join msuser On msuser.username = authorization.Username " _
                                        & " Where userid = '" & kdKaryawan & "'")

        Dim table(20)
        Dim table_access(1)
        Dim is_access = ""
        While sql_access.Read
            table = sql_access.Item(0).ToString.Split(",")
            For i As Integer = 0 To table.Length - 1
                table_access = table(i).ToString.Split(":")
                If table_access(0) = Access_name Then
                    is_access = table_access(1)
                End If
            Next
        End While
        Return is_access
    End Function

    'FlagStatus : StatusSO,Stock
    Public Function general_check_stock(ByVal KdBarang As String, ByVal addQty As String, ByVal FlagStatus As String)
        Dim readerStock = execute_reader(" Select sum(Qty) - IfNull(( Select sum(Qty) from TrSalesOrderDetail " & _
                                         " Join TrSalesOrder On TrSalesOrderDetail.kdso = TrSalesOrder.kdso " & _
                                         " Where KdBarang = MsBarangList.KdBarang " & _
                                         " And StatusSo = 3 " & _
                                         " Group By KdBarang ),0) As Stock From MsBarangList " & _
                                         " Where KdBarang = '" & KdBarang & "' " & _
                                         " And StatusBarangList = 0 " & _
                                         " Group By KdBarang ")
        If readerStock.read Then
            If FlagStatus = "StatusSO" And (Val(readerStock(0)) < Val(addQty)) Then
                readerStock.close()
                Return "Pending"
            ElseIf FlagStatus = "Stock" Then
                Dim readyStock = Val(readerStock(0))
                If Val(readerStock(0)) < 0 Then
                    readyStock = 0
                End If
                Return Val(readyStock)
            End If
        Else
            If FlagStatus = "StatusSO" And (0 < Val(addQty)) Then
                readerStock.close()
                Return "Pending"
            ElseIf FlagStatus = "Stock" Then
                Return 0
            End If
        End If
        readerStock.close()
        If FlagStatus = "Stock" Then
            Return 0
        End If
        Return "Ready"
    End Function

    Public Function EnkripsiCode(ByVal textToHash As String) As String
        '//dibawah ini adalah sebuah fungsi untuk mengenkripsi text dengan metode MD5
        Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim Bytes() As Byte = MD5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(textToHash))
        Dim s As String = Nothing
        For Each by As Byte In Bytes
            s += by.ToString("x2")
        Next
        Return s
    End Function

    Public Function validate_email(ByVal email As String, Optional ByVal is_required As Boolean = False) As Boolean
        If is_required And email = "" Then
            MsgBox("This field is required.", MsgBoxStyle.Information, "Validation Error")
            Return False
        End If
        If email <> "" Then
            Dim emailRegex As System.Text.RegularExpressions.Regex
            emailRegex = New System.Text.RegularExpressions.Regex("^([\w-]+)@([\w-]+\.)+[A-Za-z]{2,3}$")
            Dim isMacth As Boolean = emailRegex.IsMatch(email)
            If Not isMacth Then
                'MsgBox("Invalid email address.", MsgBoxStyle.Information, "Validation Error")
                Return False
            End If
            Return True
        End If
    End Function
End Module
