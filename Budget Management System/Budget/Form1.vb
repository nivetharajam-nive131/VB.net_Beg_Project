Public Class Form1
    Dim maxrow As Integer
    Dim balance As Double = 0
    Dim id As Integer = 0
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim money As Double

        If cboType.Text = "Select" Then
            MsgBox("Pls choose the transaction type", MsgBoxStyle.Exclamation)
            Return
        End If
        If txtMoney.Text = "" Then
            MsgBox("Money cannot be null", MsgBoxStyle.Exclamation)
            Return
        End If
        If txtRemarks.Text = "" Then
            MsgBox("Remarks cannot be null", MsgBoxStyle.Exclamation)
            Return
        End If

        sql = "SELECT *  FROM `tblbudget` 
                WHERE BugetID = " & id
        maxrow = loadSingleResult(sql)
        If maxrow > 0 Then

            If MessageBox.Show("Do you want to update this record?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then

                If cboType.Text = "Received" Then
                    money = Double.Parse(txtMoney.Text) - Double.Parse(dt.Rows(0).Item(1))
                    balance = Double.Parse(dt.Rows(0).Item(3)) + money
                Else
                    money = Double.Parse(dt.Rows(0).Item(2)) - Double.Parse(txtMoney.Text)
                    balance = Double.Parse(dt.Rows(0).Item(3)) + money
                End If
                'MsgBox(money)
                'MsgBox(balance)
                Try
                    sql = "SELECT BugetID,BudgetBalance FROM `tblbudget` 
                        WHERE BugetID > " & id & "  ORDER BY `BugetID` asc"
                    loadSingleResult(sql)
                    For Each r As DataRow In dt.Rows
                        sql = "UPDATE tblbudget SET BudgetBalance = BudgetBalance + '" & money & "' WHERE BugetID = " & r.Item(0)
                        executeQuery(sql)
                    Next
                Catch ex As Exception
                End Try

                Select Case cboType.Text
                    Case "Received"
                        sql = "UPDATE `tblbudget` SET `BudgetIn`='" & txtMoney.Text & "',`BudgetBalance`= '" & balance & "',
                                `Remarks`='" & txtRemarks.Text & "',`TrasactionDate`='" & dtpTransDate.Text & "',`Type`='" & cboType.Text & "' 
                                WHERE `BugetID`=" & id
                        executeQuery(sql)

                    Case "Withdraw"
                        sql = "UPDATE `tblbudget` SET `BudgetOut`='" & txtMoney.Text & "',`BudgetBalance`= '" & balance & "',
                                `Remarks`='" & txtRemarks.Text & "',`TrasactionDate`='" & dtpTransDate.Text & "',`Type`='" & cboType.Text & "' 
                                WHERE `BugetID`=" & id
                        executeQuery(sql)
                End Select
            End If




        Else
            If MessageBox.Show("Do you want to Save this record?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then

                Select Case cboType.Text
                    Case "Received"

                        sql = "SELECT BudgetBalance FROM `tblbudget` ORDER BY `BugetID` DESC"
                        maxrow = loadSingleResult(sql)
                        If maxrow > 0 Then
                            balance = Double.Parse(dt.Rows(0).Item(0)) + Double.Parse(txtMoney.Text)
                        Else
                            balance = txtMoney.Text
                        End If


                        sql = "INSERT INTO `tblbudget`(`BudgetIn`, `BudgetOut`, `BudgetBalance`, `Remarks`, `TrasactionDate`,Type) 
                        VALUES ('" & txtMoney.Text & "',0,'" & balance & "','" & txtRemarks.Text & "','" & dtpTransDate.Text & "',
                            '" & cboType.Text & "')"
                        executeQuery(sql)

                    Case "Withdraw"

                        sql = "SELECT BudgetBalance FROM `tblbudget` ORDER BY `BugetID` DESC"
                        maxrow = loadSingleResult(sql)
                        If maxrow > 0 Then

                            balance = Double.Parse(dt.Rows(0).Item(0)) - Double.Parse(txtMoney.Text)

                        Else
                            MsgBox("transaction cannot be proccess", MsgBoxStyle.Exclamation)
                            Return
                        End If

                        sql = "INSERT INTO `tblbudget`(`BudgetIn`, `BudgetOut`, `BudgetBalance`, `Remarks`, `TrasactionDate`,Type) 
                        VALUES (0,'" & txtMoney.Text & "','" & balance & "','" & txtRemarks.Text & "','" & dtpTransDate.Text & "',
                            '" & cboType.Text & "')"
                        executeQuery(sql)
                End Select
            End If

        End If

        clear()
    End Sub
    Private Sub clear()
        id = 0
        txtMoney.Text = 0
        txtRemarks.Clear()
        cboType.Text = "Select"
        dtpTransDate.Text = Now()

        sql = "SELECT `BugetID`,`TrasactionDate` as 'Date', `BudgetIn` as 'Recieved', `BudgetOut` as 'Withdraw', `BudgetBalance` as 'Balance', `Remarks`,  `Type` FROM `tblbudget` ORDER BY BugetID ASC"
        loadResultList(sql, dtglist)

        Dim maxrow As Integer
        Dim recieve, withdraw, bal As Double
        maxrow = dtglist.RowCount()


        For i As Integer = 0 To maxrow - 1
            recieve += dtglist.Rows(i).Cells(2).Value
            withdraw += dtglist.Rows(i).Cells(3).Value
        Next

        bal = recieve - withdraw
        txtRecieved.Text = recieve.ToString("N2")
        txtWidthraw.Text = withdraw.ToString("N2")
        txtBalance.Text = bal.ToString("N2")


        'sql = "SELECT BudgetBalance FROM `tblbudget` ORDER BY `BugetID` DESC"
        'maxrow = loadSingleResult(sql)
        'If maxrow > 0 Then
        '    bal = Double.Parse(dt.Rows(0).Item(0)) + Double.Parse(txtMoney.Text)
        '    txtBalance.Text = bal.ToString("N2")
        'Else
        '    txtBalance.Text = 0
        'End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        clear()
    End Sub

    Private Sub txtMoney_Click(sender As Object, e As EventArgs) Handles txtMoney.Click
        txtMoney.SelectAll()
    End Sub
    Private Sub txtRemarks_Click(sender As Object, e As EventArgs) Handles txtRemarks.Click
        txtRemarks.SelectAll()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        sql = "SELECT `BugetID`,`TrasactionDate` as 'Date', `BudgetIn` as 'Received', `BudgetOut` as 'Withdraw', `BudgetBalance` as 'Balance', `Remarks`,  `Type` 
                FROM `tblbudget` WHERE DATE(TrasactionDate) BETWEEN '" & dtpfrom.Text & "' AND '" & dtpto.Text & "' ORDER BY BugetID ASC"
        loadResultList(sql, dtglist)

        Dim maxrow As Integer
        Dim recieve, withdraw, bal As Double
        maxrow = dtglist.RowCount()


        For i As Integer = 0 To maxrow - 1
            recieve += dtglist.Rows(i).Cells(2).Value
            withdraw += dtglist.Rows(i).Cells(3).Value
        Next

        bal = recieve - withdraw
        txtRecieved.Text = recieve.ToString("N2")
        txtWidthraw.Text = withdraw.ToString("N2")
        txtBalance.Text = bal.ToString("N2")




        'sql = "SELECT BudgetBalance FROM `tblbudget` ORDER BY `BugetID` DESC"
        'maxrow = loadSingleResult(sql)
        'If maxrow > 0 Then
        '    bal = Double.Parse(dt.Rows(0).Item(0))
        '    txtBalance.Text = bal.ToString("N2")
        'Else
        '    txtBalance.Text = 0
        'End If

    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        clear()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        Dim money As Double

        If dtglist.CurrentRow.Cells(6).Value = "Received" Then
            money = dtglist.CurrentRow.Cells(2).Value
        Else
            money = dtglist.CurrentRow.Cells(3).Value
        End If

        If MessageBox.Show("Are you sure you want to delete this record?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then



            sql = "SELECT BugetID,BudgetBalance FROM `tblbudget` 
                WHERE BugetID > " & dtglist.CurrentRow.Cells(0).Value & "  ORDER BY `BugetID` asc"
            maxrow = loadSingleResult(sql)
            If maxrow > 0 Then
                Try
                    For Each r As DataRow In dt.Rows

                        'bal = r.Item(1) - money 


                        sql = "UPDATE tblbudget SET BudgetBalance = BudgetBalance + '" & money & "' WHERE BugetID = " & r.Item(0)
                        executeQuery(sql)
                    Next
                    'For i As Integer = 0 To maxrow
                    '    sql = "UPDATE tblbudget SET BudgetBalance = BudgetBalance - '" & dt.Rows(i).Item(1) & "' WHERE BugetID = " & dt.Rows(i).Item(0)
                    '    executeQuery(sql)
                    'Next

                Catch ex As Exception

                End Try

            End If

            sql = "DELETE FROM `tblbudget` WHERE `BugetID` = " & dtglist.CurrentRow.Cells(0).Value
            executeQuery(sql)

            clear()
        End If
    End Sub

    Private Sub dtglist_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dtglist.CellClick
        Try
            With dtglist.CurrentRow
                If .Cells(6).Value = "Received" Then
                    txtMoney.Text = .Cells(2).Value
                Else
                    txtMoney.Text = .Cells(3).Value
                End If
                id = .Cells(0).Value

                txtRemarks.Text = .Cells(5).Value
                cboType.Text = .Cells(6).Value
                dtpTransDate.Text = .Cells(1).Value
            End With
        Catch ex As Exception

        End Try
    End Sub
End Class
