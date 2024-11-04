Option Explicit

'章を追加するためのファイルが対象
'na6dl32でダウンロードしたテキストの挿絵リンクを置き換える
'"［＃リンクの図（//[数字5桁のユーザーコード].mitemin.net/userpageimage/viewimagebig/icode/[i+数字6桁の画像ID]/）入る］"
'"［＃挿絵（[画像ファイル名]）入る］"
'ファイル先頭から最初の"［＃中見出し］"までは作品紹介なので不要
'罫線囲みは作者コメントなので削除する
'

Private AUTO_FLAG 'As Boolean

'********************************************************************************
'TXTファイルの文字セットの判定
'NonSoft氏のサンプルソースを参考にVBScriptで動作するよう改変しています。
'http://homepage2.nifty.com/nonnon/index.html
'http://homepage2.nifty.com/nonnon/SoftSample/SampleModJUDG.html
'
'****************************************************************************
' 機能名    : Module1.bas
' 機能説明  : 文字コード判定
' 備考      :
' 著作権    : Copyright(C) 2008 - 2009 のん All rights reserved
' ---------------------------------------------------------------------------
' 使用条件  : このサイトの内容を使用(流用/改変/転載/等全て)した成果物を不特定
'           : 多数に公開/配布する場合は、このサイトを参考にした旨を記述してく
'           : ださい。(例)WEBページやReadMeにリンクを貼ってください
' ---------------------------------------------------------------------------
'****************************************************************************

Private Const JUDGEFIX = 9999       '文字コード決定％
Private Const JUDGESIZEMAX = 1000   '文字コード判定バイト数
Private Const SingleByteWeight = 1  '１バイト文字コードの一致重み
Private Const Multi_ByteWeight = 2  '複数バイト文字コードの一致重み

Private Const ctrl = 0		'制御コード
Private Const asci = 1		'ASCII
Private Const roma = 2		'JISローマ字
Private Const kana = 3		'JISカナ（半角カナ）
Private Const kanO = 4		'旧JIS漢字 (1978)
Private Const kanN = 5		'新JIS漢字 (1983/1990)
Private Const kanH = 6		'JIS補助漢字

'----文字コード判定
' 関数名    : JudgeCode
' 返り値    : 判定結果文字コード名（ADODB.StreamのCharset用）
' 引き数    : FilePath : 判定文字テキストファイル
' 機能説明  : 文字コードを判定する
' 備考      :
Public Function JudgeCode(FilePath,ByRef probability)
	JudgeCode = "Shift_JIS"
	Dim lngSJIS 'As Long
	Dim lngJIS 'As Long
	Dim lngEUC 'As Long
	Dim lngUNI 'As Long
	Dim lngUTF7 'As Long
	Dim lngUTF8 'As Long

	Dim sIn
	Dim bytCode(1000)
	Dim CodeMax

	Set sIn = CreateObject("ADODB.Stream")
	sIn.Type = 1 ' バイナリーモード
	sIn.Open
	sIn.LoadFromFile(FilePath)
	For CodeMax = 0 to JUDGESIZEMAX -1
		bytCode(CodeMax) = AscB(sIn.Read(1))
		If sIn.EOS Then
			Exit For
		End If
    Next
	Set sIn = Nothing
'	Call Editor.InfoMsg("CodeMax=" & CodeMax )
	lngJIS = JudgeJIS(bytCode,CodeMax, True)
'	Call Editor.InfoMsg("lngJIS=" & lngJIS )
	If lngJIS >= JUDGEFIX Then
		probability = lngJIS
		JudgeCode = "ISO-2022-JP"		'JIS
		Exit Function
	End If

	lngUNI = JudgeUNI(bytCode,CodeMax, True)
'	Call Editor.InfoMsg("lngUNI=" & lngUNI )
	If lngUNI >= JUDGEFIX Then
		probability = lngUNI
		JudgeCode = "UNICODE"
		Exit Function
	End If

	lngUTF8 = JudgeUTF8(bytCode,CodeMax, True)
'	Call Editor.InfoMsg("lngUTF8=" & lngUTF8 )
	If lngUTF8 >= JUDGEFIX Then
		probability = lngUTF8
		JudgeCode = "UTF-8"
		Exit Function
	End If

	lngUTF7 = JudgeUTF7(bytCode,CodeMax, True)
'	Call Editor.InfoMsg("lngUTF7=" & lngUTF7 )
	If lngUTF7 >= JUDGEFIX Then
		probability = lngUTF7
		JudgeCode = "UTF-7"
		Exit Function
	End If

	lngSJIS = JudgeSJIS(bytCode,CodeMax, True)
'	Call Editor.InfoMsg("lngSJIS=" & lngSJIS )
	If lngSJIS >= JUDGEFIX Then
		probability = lngUNI
		JudgeCode = "Shift_JIS"
		Exit Function
	End If

	lngEUC = JudgeEUC(bytCode,CodeMax, True)
'	Call Editor.InfoMsg("lngEUC=" & lngEUC )
	If lngEUC >= JUDGEFIX Then
		probability = lngEUC
		JudgeCode = "EUC-JP"
		Exit Function
	End If

	If lngSJIS >= lngSJIS And lngSJIS >= lngUNI And lngSJIS >= lngJIS And _
	lngSJIS >= lngUTF7 And lngSJIS >= lngUTF8 And lngSJIS >= lngEUC Then
		probability = lngSJIS
		JudgeCode = "Shift_JIS"
		Exit Function
	End If

	If lngUTF8 >= lngSJIS And lngUTF8 >= lngUNI And lngUTF8 >= lngJIS And _
	lngUTF8 >= lngUTF7 And lngUTF8 >= lngUTF8 And lngUTF8 >= lngEUC Then
		probability = lngUTF8
		JudgeCode = "UTF-8"
		Exit Function
	End If

	If lngUNI >= lngSJIS And lngUNI >= lngUNI And lngUNI >= lngJIS And _
	lngUNI >= lngUTF7 And lngUNI >= lngUTF8 And lngUNI >= lngEUC Then
		probability = lngUNI
		JudgeCode = "UNICODE"
		Exit Function
	End If

	If lngJIS >= lngSJIS And lngJIS >= lngUNI And lngJIS >= lngJIS And _
	lngJIS >= lngUTF7 And lngJIS >= lngUTF8 And lngJIS >= lngEUC Then
		probability = lngJIS
		JudgeCode = "ISO-2022-JP"		'JIS
		Exit Function
	End If

	If lngUTF7 >= lngSJIS And lngUTF7 >= lngUNI And lngUTF7 >= lngJIS And _
	lngUTF7 >= lngUTF7 And lngUTF7 >= lngUTF8 And lngUTF7 >= lngEUC Then
		probability = lngUTF7
		JudgeCode = "UTF-7"
		Exit Function
	End If

	If lngEUC >= lngSJIS And lngEUC >= lngUNI And lngEUC >= lngJIS And _
	lngEUC >= lngUTF7 And lngEUC >= lngUTF8 And lngEUC >= lngEUC Then
		probability = lngEUC
		JudgeCode = "EUC-JP"
		Exit Function
	End If

End Function

'----SJIS関係
' 関数名    : JudgeSJIS
' 返り値    : 判定結果確率（％）
' 引き数    : bytCode : 判定文字データ
'           : fixFlag : 確定判断有無
' 機能説明  : SJISの文字コード判定(可能性)確率を計算する
' 備考      :
Private Function JudgeSJIS(ByRef bytCode(),CodeMax, fixFlag )
	Dim i 'As Long
	Dim lngFit 'As Long
	Dim lngUB 'As Long

	lngUB = JUDGESIZEMAX - 1
	If lngUB > CodeMax Then
		lngUB = CodeMax
	End If
	For i = 0 To lngUB
		'81-9F,E0-EF(1バイト目)
		If (bytCode(i) >= &H81 And bytCode(i) <= &H9F) Or _
			(bytCode(i) >= &HE0 And bytCode(i) <= &HEF) Then
			If i <= UBound(bytCode) - 1 Then
				'40-7E,80-FC(2バイト目)
				If (bytCode(i + 1) >= &H40 And bytCode(i + 1) <= &H7E) Or _
					(bytCode(i + 1) >= &H80 And bytCode(i + 1) <= &HFC) Then
					lngFit = lngFit + (2 * Multi_ByteWeight)
					i = i + 1
				End If
			End If

			'A1-DF(1バイト目)
			ElseIf (bytCode(i) >= &HA1 And bytCode(i) <= &HDF) Then
				lngFit = lngFit + (1 * SingleByteWeight)

			'20-7E(1バイト目)
			ElseIf (bytCode(i) >= &H20 And bytCode(i) <= &H7E) Then
				lngFit = lngFit + (1 * SingleByteWeight)

			'00-1F, 7F(1バイト目)
			ElseIf (bytCode(i) >= &H0 And bytCode(i) <= &H1F) Or _
				bytCode(i) = &H7F Then
				lngFit = lngFit + (1 * SingleByteWeight)
		End If
	Next 
	JudgeSJIS = (lngFit * 100) / ((lngUB + 1) * Multi_ByteWeight)
End Function

'----JIS関係
' 関数名    : JudgeJIS
' 返り値    : 判定結果確率（％）
' 引き数    : bytCode : 判定文字データ
'           : fixFlag : 確定判断有無
' 機能説明  : JISの文字コード判定(可能性)確率を計算する
' 備考      :
Private Function JudgeJIS(ByRef bytCode(),CodeMax, fixFlag )
	Dim i 'As Long
	Dim lngFit 'As Long
	Dim lngMode 'As JISMODE
	Dim lngUB 'As Long

	lngUB = JUDGESIZEMAX - 1
	If lngUB > CodeMax Then
		lngUB = CodeMax
	End If
	For i = 0 To lngUB
		'1B(1バイト目)
		If bytCode(i) = &H1B Then
			If i <= UBound(bytCode) - 2 Then
				'28 42(2・3バイト目)
				If bytCode(i + 1) = &H28 And bytCode(i + 1) <= &H42 Then
					lngMode = asci
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
					If fixFlag Then
						JudgeJIS = JUDGEFIX
						Exit Function
					End If
				End If
				'28 4A(2・3バイト目)
				If bytCode(i + 1) = &H28 And bytCode(i + 1) <= &H4A Then
					lngMode = roma
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
					If fixFlag Then
						JudgeJIS = JUDGEFIX
						Exit Function
					End If
				End If
				'28 49(2・3バイト目)
				If bytCode(i + 1) = &H28 And bytCode(i + 1) <= &H49 Then
					lngMode = kana
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
					If fixFlag Then
						JudgeJIS = JUDGEFIX
						Exit Function
					End If
				End If
				'24 40(2・3バイト目)
				If bytCode(i + 1) = &H24 And bytCode(i + 1) <= &H40 Then
					lngMode = kanO
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
					If fixFlag Then
						JudgeJIS = JUDGEFIX
						Exit Function
					End If
				End If
				'24 42(2・3バイト目)
				If bytCode(i + 1) = &H24 And bytCode(i + 1) <= &H42 Then
					lngMode = kanN
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
					If fixFlag Then
						JudgeJIS = JUDGEFIX
						Exit Function
					End If
				End If
				'24 44(2・3バイト目)
				If bytCode(i + 1) = &H24 And bytCode(i + 1) <= &H44 Then
					lngMode = kanH
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
					If fixFlag Then
						JudgeJIS = JUDGEFIX
						Exit Function
					End If
				End If
			End If
		Else
			Select Case lngMode
			Case ctrl, asci, roma
				'00-1F,7F
				If (bytCode(i) >= &H0 And bytCode(i) <= &H1F) Or _
					bytCode(i) = &H7F Then
					lngFit = lngFit + (1 * SingleByteWeight)
				End If
				'20-7E
				If (bytCode(i) >= &H20 And bytCode(i) <= &H7E) Then
					lngFit = lngFit + (1 * SingleByteWeight)
				End If
			Case kana
				'21-5F
				If (bytCode(i) >= &H21 And bytCode(i) <= &H5F) Then
					lngFit = lngFit + (1 * SingleByteWeight)
				End If
			Case kanO, kanN, kanH
				If i <= UBound(bytCode) - 1 Then
					'21-7E
					If (bytCode(i) >= &H21 And bytCode(i) <= &H7E) And _
						(bytCode(i - 1) >= &H21 And bytCode(i - 1) <= &H7E) Then
						lngFit = lngFit + (2 * Multi_ByteWeight)
						i = i + 1
					End If
				End If
			End Select
		End If
	Next 
	JudgeJIS = (lngFit * 100) / ((lngUB + 1) * Multi_ByteWeight)
End Function

'----EUC関係
' 関数名    : JudgeEUC
' 返り値    : 判定結果確率（％）
' 引き数    : bytCode : 判定文字データ
'           : fixFlag : 確定判断有無
' 機能説明  : EUCの文字コード判定(可能性)確率を計算する
' 備考      :
Private Function JudgeEUC(ByRef bytCode(),CodeMax, fixFlag) 
	Dim i 'As Long
	Dim lngFit 'As Long
	Dim lngUB 'As Long

	lngUB = JUDGESIZEMAX - 1
	If lngUB > CodeMax Then
		lngUB = CodeMax
	End If
	For i = 0 To lngUB
		'8E(1バイト目) + A1-DF(2バイト目)
		If bytCode(i) = &H8E Then
			If i <= UBound(bytCode) - 1 Then
				If bytCode(i + 1) >= &HA1 And bytCode(i + 1) <= &HDF Then
					lngFit = lngFit + (2 * Multi_ByteWeight)
					i = i + 1
				End If
			End If

		'8F(1バイト目) + A1-0xFE(2・3バイト目)
		ElseIf bytCode(i) = &H8F Then
			If i <= UBound(bytCode) - 2 Then
				If (bytCode(i + 1) >= &HA1 And bytCode(i + 1) <= &HFE) And _
				(bytCode(i + 2) >= &HA1 And bytCode(i + 2) <= &HFE) Then
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
				End If
			End If

		'A1-FE(1バイト目) + A1-FE(2バイト目)
		ElseIf bytCode(i) >= &HA1 And bytCode(i) <= &HFE Then
			If i <= UBound(bytCode) - 1 Then
				If bytCode(i + 1) >= &HA1 And bytCode(i + 1) <= &HFE Then
					lngFit = lngFit + (2 * Multi_ByteWeight)
					i = i + 1
				End If
			End If

		'20-7E(1バイト目)
		ElseIf (bytCode(i) >= &H20 And bytCode(i) <= &H7E) Then
			lngFit = lngFit + (1 * SingleByteWeight)

		'00-1F, 7F(1バイト目)
		ElseIf (bytCode(i) >= &H0 And bytCode(i) <= &H1F) Or _
			bytCode(i) = &H7F Then
			lngFit = lngFit + (1 * SingleByteWeight)
		End If
	Next 
	JudgeEUC = (lngFit * 100) / ((lngUB + 1) * Multi_ByteWeight)
End Function

'----UNICODE関係
' 関数名    : JudgeUNI
' 返り値    : 判定結果確率（％）
' 引き数    : bytCode : 判定文字データ
'           : fixFlag : 確定判断有無
' 機能説明  : UTF16の文字コード判定(可能性)確率を計算する
' 備考      :
Private Function JudgeUNI(ByRef bytCode(),CodeMax, fixFlag )
	Dim i 'As Long
	Dim lngFit 'As Long
	Dim lngUB 'As Long

	lngUB = JUDGESIZEMAX - 1
	If lngUB > CodeMax Then
		lngUB = CodeMax
	End If
	For i = 0 To lngUB
		If fixFlag Then
			'BOM
			If bytCode(i) = &HFF Then
				If i <= UBound(bytCode) - 1 Then
					If bytCode(i + 1) = &HFE Then
						JudgeUNI = JUDGEFIX
						Exit Function
					End If
				End If
			End If
'			'半角の証
'			If bytCode(i) = &H0 Then
'				JudgeUNI = JUDGEFIX
'				Exit Function
'			End If
		End If

		If i <= UBound(bytCode) - 1 Then
			'00(2バイト目)
			If (bytCode(i + 1) = &H0) Then
				'00-FF(1バイト目)
				lngFit = lngFit + (2 * Multi_ByteWeight)

			'01-33(2バイト目)
			ElseIf (bytCode(i + 1) >= &H1 And bytCode(i + 1) <= &H33) Then
				'00-FF(1バイト目)
				lngFit = lngFit + (2 * Multi_ByteWeight)

			'34-4D(2バイト目)
			ElseIf (bytCode(i + 1) >= &H34 And bytCode(i + 1) <= &H4D) Then
				'00-FF(1バイト目)----空き----
				lngFit = 0
				Exit For

			'4E-9F(2バイト目)
			ElseIf (bytCode(i + 1) >= &H4E And bytCode(i + 1) <= &H9F) Then
				'00-FF(1バイト目)
				lngFit = lngFit + (2 * Multi_ByteWeight)

			'A0-AB(2バイト目)
			ElseIf (bytCode(i + 1) >= &HA0 And bytCode(i + 1) <= &HAB) Then
				'00-FF(1バイト目)----空き----
				lngFit = 0
				Exit For

			'AC-D7(2バイト目)
			ElseIf (bytCode(i + 1) >= &HAC And bytCode(i + 1) <= &HD7) Then
				'00-FF(1バイト目)----ハングル----
				lngFit = 0
				Exit For

			'D8-DF(2バイト目)
			ElseIf (bytCode(i + 1) >= &HD8 And bytCode(i + 1) <= &HDF) Then
				'00-FF(1バイト目)
				lngFit = lngFit + (2 * Multi_ByteWeight)

			'E0-F7(2バイト目)
			ElseIf (bytCode(i + 1) >= &HE0 And bytCode(i + 1) <= &HF7) Then
				'00-FF(1バイト目)----外字----
				lngFit = 0
				Exit For

			'F8-FF(2バイト目)
			ElseIf (bytCode(i + 1) >= &HF8 And bytCode(i + 1) <= &HFF) Then
				'00-FF(1バイト目)
				lngFit = lngFit + (2 * Multi_ByteWeight)

			End If
			i = i + 1
		End If
	Next 
	JudgeUNI = (lngFit * 100) / ((lngUB + 1) * Multi_ByteWeight)
End Function

'----UTF7関係
' 関数名    : JudgeUTF7
' 返り値    : 判定結果確率（％）
' 引き数    : bytCode : 判定文字データ
'           : fixFlag : 確定判断有無
' 機能説明  : UTF7の文字コード判定(可能性)確率を計算する
' 備考      :
Private Function JudgeUTF7(ByRef bytCode(),CodeMax, fixFlag ) 
	Dim i 'As Long
	Dim lngFit 'As Long
	Dim lngWrk 'As Long
	Dim str64 'As String
	Dim bln64 'As Boolean
	str64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
	Dim lngUB 'As Long
	Dim lngBY 'As Long
	Dim lngXB 'As Long
	Dim lngXX 'As Long

	lngUB = JUDGESIZEMAX - 1
	If lngUB > CodeMax Then
		lngUB = CodeMax
	End If
	lngWrk = 0

	For i = 0 To lngUB
		'+～-まではBASE64ENCODE
		If bytCode(i) = Asc("+") And bln64 = False Then
			lngWrk = 1
			bln64 = True
		ElseIf bytCode(i) = Asc("-") Then
			If lngWrk <= 0 Then
				lngWrk = lngWrk + 1
				lngFit = lngFit + (lngWrk * SingleByteWeight)
			ElseIf lngWrk = 1 Then
				lngWrk = lngWrk + 1
				lngFit = lngFit + (lngWrk * Multi_ByteWeight)
			ElseIf lngWrk >= 4 And lngXB < 6 And _
				((InStr(str64, Chr(bytCode(i - 1))) - 1) And lngXX) = 0 Then
				lngWrk = lngWrk + 1
				lngFit = lngFit + (lngWrk * Multi_ByteWeight)
			End If
			lngWrk = 0
			bln64 = False
		Else
			If bln64 = True Then
				'BASE64ENCODE中
				If InStr(str64, Chr(bytCode(i))) > 0 Then
					lngBY = Int((lngWrk * 6) / 8)
					lngXB = (lngWrk * 6) - (lngBY * 8)
					lngXX = (2 ^ lngXB) - 1
					lngWrk = lngWrk + 1
				Else
					lngWrk = 0
					bln64 = False
				End If
			Else
				'20-7E(1バイト目)
				If (bytCode(i) >= &H20 And bytCode(i) <= &H7E) Then
					lngFit = lngFit + (1 * SingleByteWeight)

				'00-1F, 7F(1バイト目)
				ElseIf (bytCode(i) >= &H0 And bytCode(i) <= &H1F) Or _
					bytCode(i) = &H7F Then
					lngFit = lngFit + (1 * SingleByteWeight)
				End If
			End If
		End If
	Next 
	JudgeUTF7 = (lngFit * 100) / ((lngUB + 1) * Multi_ByteWeight)
End Function

'----UTF8関係
' 関数名    : JudgeUTF8
' 返り値    : 判定結果確率（％）
' 引き数    : bytCode : 判定文字データ
'           : fixFlag : 確定判断有無
' 機能説明  : UTF8の文字コード判定(可能性)確率を計算する
' 備考      :
Private Function JudgeUTF8(ByRef bytCode(),CodeMax, fixFlag )
	Dim i 
	Dim lngFit
	Dim lngUB

	lngUB = JUDGESIZEMAX - 1
	If lngUB > CodeMax Then
		lngUB = CodeMax
	End If
	For i = 0 To lngUB
		If fixFlag Then
		'BOM
			If bytCode(i) = &HEF Then
				If i <= UBound(bytCode) - 2 Then
					If bytCode(i + 1) = &HBB And _
						bytCode(i + 2) = &HBF Then
						JudgeUTF8 = JUDGEFIX
						Exit Function
					End If
				End If
			End If
		End If

		'AND FC(1バイト目) + 80-BF(2-6バイト目)
		If (bytCode(i) And &HFC) = &HFC Then
			If i <= UBound(bytCode) - 5 Then
				If (bytCode(i + 1) >= &H80 And bytCode(i + 1) <= &HBF) And _
					(bytCode(i + 2) >= &H80 And bytCode(i + 2) <= &HBF) And _
					(bytCode(i + 3) >= &H80 And bytCode(i + 3) <= &HBF) And _
					(bytCode(i + 4) >= &H80 And bytCode(i + 4) <= &HBF) And _
					(bytCode(i + 5) >= &H80 And bytCode(i + 5) <= &HBF) Then
					lngFit = lngFit + (6 * Multi_ByteWeight)
					i = i + 5
				End If
			End If

		'AND F8(1バイト目) + 80-BF(2-5バイト目)
		ElseIf (bytCode(i) And &HF8) = &HF8 Then
			If i <= UBound(bytCode) - 4 Then
				If (bytCode(i + 1) >= &H80 And bytCode(i + 1) <= &HBF) And _
					(bytCode(i + 2) >= &H80 And bytCode(i + 2) <= &HBF) And _
					(bytCode(i + 3) >= &H80 And bytCode(i + 3) <= &HBF) And _
					(bytCode(i + 4) >= &H80 And bytCode(i + 4) <= &HBF) Then
					lngFit = lngFit + (5 * Multi_ByteWeight)
					i = i + 4
				End If
			End If

		'AND F0(1バイト目) + 80-BF(2-4バイト目)
		ElseIf (bytCode(i) And &HF0) = &HF0 Then
			If i <= UBound(bytCode) - 3 Then
				If (bytCode(i + 1) >= &H80 And bytCode(i + 1) <= &HBF) And _
					(bytCode(i + 2) >= &H80 And bytCode(i + 2) <= &HBF) And _
					(bytCode(i + 3) >= &H80 And bytCode(i + 3) <= &HBF) Then
					lngFit = lngFit + (4 * Multi_ByteWeight)
					i = i + 3
				End If
			End If

		'AND E0(1バイト目) + 80-BF(2-3バイト目)
		ElseIf (bytCode(i) And &HE0) = &HE0 Then
			If i <= UBound(bytCode) - 2 Then
				If (bytCode(i + 1) >= &H80 And bytCode(i + 1) <= &HBF) And _
					(bytCode(i + 2) >= &H80 And bytCode(i + 2) <= &HBF) Then
					lngFit = lngFit + (3 * Multi_ByteWeight)
					i = i + 2
				End If
			End If

		'AND C0(1バイト目) + 80-BF(2バイト目)
		ElseIf (bytCode(i) And &HC0) = &HC0 Then
			If i <= UBound(bytCode) - 1 Then
				If (bytCode(i + 1) >= &H80 And bytCode(i + 1) <= &HBF) Then
					lngFit = lngFit + (2 * Multi_ByteWeight)
					i = i + 1
				End If
			End If

		'20-7E(1バイト目)
		ElseIf (bytCode(i) >= &H20 And bytCode(i) <= &H7E) Then
			lngFit = lngFit + (1 * SingleByteWeight)

		'00-1F, 7F(1バイト目)
		ElseIf (bytCode(i) >= &H0 And bytCode(i) <= &H1F) Or _
			bytCode(i) = &H7F Then
			lngFit = lngFit + (1 * SingleByteWeight)
		End If
	Next 
	JudgeUTF8 = (lngFit * 100) / ((lngUB + 1) * Multi_ByteWeight)
End Function

'********************************************************************************

'********************************************************************************
'青空文庫形式のルビを振る関数
'引数の書式は、"漢字列《ルビ》"または"｜文字列《ルビ》"
'"漢字列《ルビ》"の場合の処理
'まだルビの付いていない"漢字列"を検索して、"漢字列《ルビ》"に置換する。
'ルビ付きの判定は"《"が付いている事で判断する。
'"漢字列"の直前の文字が漢字の場合、"漢字列"の前に"｜"を付けて置換する。
'"｜文字列《ルビ》"の場合の処理
'まだルビの付いていない"文字列"を検索して、"｜文字列《ルビ》"に置換する。
'　例　ルビを振る文字列を"｜黒い雲"、ルビを"あまぐも"とすると、"黒い雲"で検索し"｜黒い雲《あまぐも》"に置換する。
'
'"魔力《マナ》"と"魔力貯蔵量《マナ･プール》"の様に一方が他方の一部である場合、リスト内の順番を考慮しないと、
'"魔力《マナ》貯蔵量《マナ･プール》"のように置換される場合がある。
'正しく置換するには、ルビを振る文字列の長さが長い方をリストの前に登録し、短い物を後に登録する。
'上の例では魔力貯蔵量《マナ･プール》を先に、魔力《マナ》を後に置換する様、リストに記述する。
'********************************************************************************
Private Function AddRubiAll(ByVal strLine)
	Dim str1	'As String
	Dim str2	'As String
	Dim str3	'As String
	Dim pos1 	'As Integer
	Dim pos2 	'As Integer

	AddRubiAll = false
	'指定文字列の書式"文字列《ルビ》"で文字列、ルビ共に1文字以上あるかを判定
	pos1 = InStr(strLine,"《")
	pos2 = InStr(strLine,"》")
	If 1 < pos1 And  pos1 + 1 < pos2 Then
		str1 = left(strLine,pos1 -1 )					'ルビを振る漢字の取得
		str2 = mid(strLine,pos1 + 1,pos2 - pos1 - 1)	'ルビの取得
		str3 = left(str1,1)
		If str3 = "｜" or str3 = "|" Then
			str1 = mid(str1,2)
			'ルビを振る文字列の先頭が"｜"の場合（半角|だったら全角で置換する）
			' 直後が《で無い"文字列"を検索し、"｜文字列《ルビ》"に置換する。
			Call Editor.ReplaceAll(str1 & "(\p{Han}*+[^《])", "｜" & str1 & "《" & str2 & "》$1", 20)	' すべて置換
		else
			' 直前の文字が漢字で直後が《で無い"文字列"を検索し、"｜文字列《ルビ》"に置換する。
			Call Editor.ReplaceAll("(\p{Han})" & str1 & "(\p{Han}*+[^《])", "$1｜" & str1 & "《" & str2 & "》$2", 20)	' すべて置換
			'直後が《で無い"文字列"を検索し、"文字列《ルビ》"に置換する。
			Call Editor.ReplaceAll(str1 & "(\p{Han}*+[^《])", str1 & "《" & str2 & "》$1", 20)	' すべて置換
		End If
		AddRubiAll = true
	End If
End Function

'********************************************************************************
'指定されたオプションフラグで
'指定された検索文字列を
'指定された置換文字で
'確認しながら全て置換する
'戻り値：置換回数（キャンセル時には1の補数になる）
'********************************************************************************
Private Function ReplaceAllWithConfirmation(strSrc, strDst, intOpt)
	Dim posx 	'As Integer
	Dim posy 	'As Integer
	Dim repCnt	'As Integer
	Dim str1	'As String

	ReplaceAllWithConfirmation = 0
	Call Editor.GoFileTop			'ファイル先頭に移動
	Call Editor.SearchNext(strSrc, CLng(intOpt))	'次を検索
	posy = Editor.GetSelectLineTo()	'現在の選択終了行位置を取得する
	posx = Editor.GetSelectColmTo()	'現在の選択終了桁位置を取得する
	Call Editor.SetDrawSwitch(1)	' 描画再開
	Call Editor.ReDraw(0)			' 再描画
	repCnt = 0
	Do While posy <> 0 And posx <> 0 		'検索後の選択終了位置が(0,0)で無い時（次が見つかった時）
		str1 = Editor.GetSelectedString()	'選択されている文字列
		Select Case Editor.MessageBox("""" & str1 & """" & "を置換しますか？", 35)	'Yes/No/Cancel + 確認アイコン
		Case 6			'YES
			Call Editor.Replace(strSrc, strDst, CLng(intOpt))	'置換（次を検索と同じ処理も行う）
			repCnt = repCnt + 1
		Case 7			'No
			Call Editor.SearchNext(strSrc, CLng(intOpt))	'次を検索
		Case 2			'キャンセル
			repCnt = (repCnt + 1) * -1 
			Exit Do				'終了
		End Select
		posy = Editor.GetSelectLineTo()	'現在の選択終了行位置を取得する
		posx = Editor.GetSelectColmTo()	'現在の選択終了桁位置を取得する
		Call Editor.ReDraw(0)			' 再描画
	Loop
	Call Editor.SetDrawSwitch(0)	' 描画停止
	ReplaceAllWithConfirmation = repCnt
End Function

'********************************************************************************
'Speeeeedの変換リストの書式を使用して置換する
'lstファイルの書式 : "検索文字列\t置換文字列\t置換オプション"（\tはタブ Chr(9)）
'置換オプションは以下の組み合わせ
'"S":エスケープシーケンスを使用しない
'"C":英字の大文字小文字を区別しない
'"確":1つ1つについて置換するかを確認する（このスクリプトの為の拡張置換オプション）
'"E":正規表現、以下のオプションは正規表現時のオプション
'"k":日本語（全角文字）も対象（このスクリプトでは常に日本語も対象）
'"m":複数行（"."が改行も含む様になる）（このスクリプトでは無効）
'"i":英字の大文字小文字を区別しない
'********************************************************************************
Private Function ReplaceAllSpe5d(strLine)
	Dim str1	'As String
	Dim str2	'As String
	Dim str3	'As String
	Dim pos1 	'As Integer
	Dim pos2 	'As Integer
	Dim opt		'As Long

	ReplaceAllSpe5d = false
	pos1 = InStr(strLine,Chr(9))		'タブ区切り
	If pos1 > 0 Then
		str1 = Left(strLine,pos1-1)
		pos2 = InStr(pos1 + 1,strLine,Chr(9))		'タブ区切り
		If pos1 < pos2 Then
			str2 = Mid(strLine,pos1 + 1,pos2 - pos1 - 1)
			str3 = Mid(StrLine,pos2 + 1)
			opt = 16		'ダイアログを閉じる
			If InStr(str3,"E") > 0 Then
				'正規表現のオプション
				opt = opt + 4								'正規表現（サクラエディタでは正規表現時のみ"\n"等を認識する）
'				If InStr(str3,"k") > 0 Then opt = opt + 0	'日本語（WindowsXP以降はUnicodeなので常に対応）
'				If InStr(str3,"m") > 0 Then opt = opt + 0	'複数行（サクラエディタでは指定できない）
				If InStr(str3,"i") <= 0 Then opt = opt + 2 	'"i"が無い時、英大文字と小文字を区別する
			Else
				'正規表現で無い時のオプション
'				If InStr(str3,"S") > 0 Then opt = opt + 0	'エスケープシーケンスを使用しない。（サクラエディタでは正規表現で無い場合、常に使用できない）
				If InStr(str3,"C") <= 0 Then opt = opt + 2 	'"C"が無い時、英大文字と小文字を区別する
			End If
			If Instr(str3,"確") > 0 Then
				Call ReplaceAllWithConfirmation(str1, str2, opt)	' すべて置換
			Else
				Call Editor.ReplaceAll(str1, str2, opt)	' すべて置換
			End If
			ReplaceAllSpe5d = true
		End If
	End If
End Function

'********************************************************************************
'文字列を判定して、実行する処理を振り分ける
'********************************************************************************
Private Function SelectOperation(strLine)
	Dim pos1 	'As Integer
	Dim pos2 	'As Integer

	SelectOperation = false
	If Trim(strLine) = "" Or left(strLine,1) = "'" Then
		'何も無い行か、行頭が"'"ならコメント行なので何もしないで正常終了
		SelectOperation = true
		Exit Function
	End If

	pos1 = InStr(strLine,Chr(9))			'タブ検索
	pos2 = InStr(pos1 + 1,strLine,Chr(9))	'タブ検索
	If pos1 > 1 And pos2 >= 3 And  pos1 < pos2  Then		'タブ区切り、第２引数は""が有りうる
		'Speeeed置換リスト互換
		SelectOperation = ReplaceAllSpe5d(strLine)
		Exit Function
	End If
	'ルビ振り
	SelectOperation = AddRubiAll(strLine)

End Function

'********************************************************************************
'置換リストを使用した置換
'********************************************************************************
Private Function ReplaceByList(ByVal FilePath)
	Dim intCnt			'As Long
	Dim LineNo			'As Long
	Dim strRead			'As String
	Dim CharSetOfText	'As String
	Dim fProBability	'As Single
	Dim objFso			'As Object

	If FilePath = "" Then		'ファイルパスが無い時
		Exit Function
	End If
	Set objFso = CreateObject("Scripting.FileSystemObject")
	if objFso.FileExists(FilePath) = false Then
		Set objFso = Nothing
		Exit Function
	End If
	Set objFso = Nothing

	'リストファイルの文字セットを判定、VBのデフォルト文字セットはSJIS
	CharSetOfText = JudgeCode(FilePath, fProBability)

	'ファイルオープンする
	With CreateObject("ADODB.Stream")
		.Charset = CharSetOfText
		.Open
		.LoadFromFile(FilePath)
		If Err.Number > 0 Then		'エラー発生時
			'エラーを表示する
			Call Editor.MessageBox("Open Error : " & FilePath )
		Else
'			' ちらつき防止
			LineNo = CLng(Editor.ExpandParameter("$y"))
			intCnt = 1
			Do While Not .EOS
		    	'処理できない行が有れば終了
		    	strRead = .ReadText(-2)
		        If SelectOperation( strRead ) = false Then
					Call Editor.ErrorMsg("Replace Error. Line = " & intCnt & ":""" & strRead & """" )
		        	Exit Do
		        End If
				intCnt = intCnt + 1
			Loop
			Call Editor.MoveCursor( LineNo, 1, 0)
			Call Editor.SearchClearMark()	' 検索マークをクリアする
		End If
		'ファイルクローズ
		.Close
	End With

End Function

'*******************************************************************************
'画像のサイズ取得
'*******************************************************************************
Private Function GetFigureSize(imgFilePath, ByRef sizeX, ByRef sizeY)
	Dim objImg 'As Object
	Dim strMsg 'As String

	GetFigureSize = False
	' インスタンス作成
	Set objImg = CreateObject("WIA.ImageFile")
	If Not objImg Is Nothing Then
		' ファイルパス指定
		objImg.LoadFile(imgFilePath)
'		Call Editor.MessageBox(imgFilePath , 32)
		' 幅x高さで表示
		'WScript.Echo(objImg.Width & "x" & objImg.Height & "(ピクセル)")
		sizeX = objImg.Width
		sizeY = objImg.Height
		' インスタンス初期化
		Set objImg = Nothing

'		'### または ###
'		With CreateObject("WIA.imagefile")
'		    .LoadFile(imgFilePath)
'		'    WScript.Echo(.Width & "x" & .Height & "(ピクセル)")
'			sizeX = .Width
'			sizeY = .Height
'		End With

		GetFigureSize = True
	End If

End Function

'*******************************************************************************
'*******************************************************************************
Private Function main()
	Dim LineNo 'As Lng
	Dim LatestPos 'As Lng
	Dim strLine 'As String
	Dim tmpstr 'As String
	Dim fileName 'As String
	Dim dirPath 'As String
	Dim strSize 'As String
	Dim sx,sy
	Dim pos1,Pos2 ' As Integer
	Dim strFilePath 'As String
	Dim ReplaceFile

With Editor
	' ちらつき防止
	Call Editor.SetDrawSwitch(0)	' 以降描画停止(ReplaceAllのプログレスバーは表示される)
	Call Editor.AddRefUndoBuffer()	' これ以降、Undoバッファをまとめる

	'マクロのファイル名末尾が"-自動"なら自動スクリプト用にフラグセットする
	strFilePath = .ExpandParameter("$M")	'このマクロのフルパス
	If Mid(strFilePath, InStrRev(strFilePath,".") -3, 3) = "-自動" Then
		AUTO_FLAG = True
	Else
		AUTO_FLAG = False
	End If

	LatestPos = CLng(.ExpandParameter("$y"))
	tmpstr = Editor.ExpandParameter("$F")	'開いているファイルのフルパス
	dirPath = Left(tmpstr, InStrRev(tmpstr,"\"))
	'リンクの図を"［＃挿絵（画像ファイル名、横Xsize×縦Ysize）入る］"に変換
'	.GoFileTop
'	LineNo = CLng(.ExpandParameter("$y"))
'	Call .SearchNext("［＃ここから罫囲み］", 20)	'検索
'	On Error Resume Next
'	do Until (CLng(Editor.ExpandParameter("$y")) = LineNo)
'		LineNo = CLng(.ExpandParameter("$y"))
'		Call .GoLineTop(8)
'		Call .BeginSelect
'		Call .SearchNext("［＃ここで罫囲み終わり］", 20)	'検索
'		Call .GoLineEnd_Sel(8)
''		strLine = .GetSelectedString(0) '選択文字列を取得
''		Call .MessageBox(strLine, 32)
'		Call .Delete
'		Call .SearchNext("［＃ここから罫囲み］", 20)	'検索
''		Call .MessageBox(CStr(Editor.ExpandParameter("$y")) & ";"& CStr(LineNo), 32)
'	Loop
'	On Error GoTo 0
	'リンクの図を［＃挿絵（画像ファイル名）入る］"に変換
	Call .ReplaceAll("^[ 　\t]*［＃リンクの図（//([0-9]+)\.mitemin\..*icode/([i0-9]+)/(.*$)","［＃挿絵（$1_$2.jpg$3", 20) ' すべて置換

	'ファイル先頭から最初の"［＃中見出し］"までは作品紹介なので不要
	.GoFileTop
	Call .BeginSelect
	Call .SearchNext("［＃中見出し］", 20)	'検索
	Call .GoLineTop_Sel(8)
'	strLine = .GetSelectedString(0) '選択文字列を取得
'	Call .MessageBox(strLine, 32)
	Call .Delete

	'罫線囲みは作者コメントなので削除する
'	.GoFileTop
	LineNo = CLng(.ExpandParameter("$y"))
	Call .SearchNext("［＃ここから罫囲み］", 20)	'検索
	On Error Resume Next
	do Until (CLng(Editor.ExpandParameter("$y")) = LineNo)
		LineNo = CLng(.ExpandParameter("$y"))
		Call .GoLineTop(8)
		Call .BeginSelect
		Call .SearchNext("［＃ここで罫囲み終わり］", 20)	'検索
		Call .GoLineEnd_Sel(8)
'		strLine = .GetSelectedString(0) '選択文字列を取得
'		Call .MessageBox(strLine, 32)
		Call .Delete
		Call .SearchNext("［＃ここから罫囲み］", 20)	'検索
'		Call .MessageBox(CStr(Editor.ExpandParameter("$y")) & ";"& CStr(LineNo), 32)
	Loop
	On Error GoTo 0

	ReplaceFile = dirPath & "_Replace.lst"
	Call ReplaceByList(ReplaceFile)		'リストファイルによる置き換えを実行

	Call Editor.SetUndoBuffer()		' ここでまとめてUndoバッファのリストに登録される
	Call Editor.MoveCursor( LatestPos, 1, 0)
	Call .FileSave						' 上書き保存
	.SearchClearMark	' 検索マークをクリアする
	.SetDrawSwitch(1)	' 描画再開
	.ReDraw(0)			' 再描画
End With
End Function

Call main()
If AUTO_FLAG Then
	ExitAll()
End If

