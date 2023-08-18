# DLsiteInfoGetter
## 概要
DLsiteのURL or 作品IDを元に、作品名、サークル名、作品種別を取得するDLLです。

解析に[AngleSharp](https://github.com/AngleSharp/AngleSharp)を使用していますので、nugetで取得してください。

.NET Framework 4.8.1で開発しました。



## 使い方
	var main = new DLsiteInfoGetter.Main();
	string searchTarget = "RJ162718";  // "https://www.dlsite.com/maniax/work/=/product_id/RJ162718.html" でも可
	bool result = main.GetInfo(searchTarget, out string prodID, out string title, out string circle, out string prodType, out string imageUrl, out string errMsg);

 result：true:エラーなし／false:エラーあり
 
 prodID：作品ID
 
 title：作品名
 
 circle：サークル名
 
 prodType：作品種別（RJ|VJ|RE|VE|BJ|AJ）

 imageUrl：サムネイル画像URL（maniaxのRJのみ対応）
 
 errMsg：エラーが発生した場合のエラーエッセージ



## 実装例
	private void searchButton_Click(object sender, EventArgs e)
	{
		searchResultText.Text = string.Empty;
		resultText = string.Empty;
		var main = new DLsiteInfoGetter.Main();
		if (!string.IsNullOrEmpty(searchTargetText.Text.Trim()))
		{
			bool result = main.GetInfo(searchTargetText.Text.Trim(), out string prodID, out string searchResult, out string circle, out string prodType, out string imageUrl, out string errMsg);
			if (!string.IsNullOrEmpty(errMsg))
			{
				MessageBox.Show(errMsg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				searchButton.Focus();
				return;
			}
			// 結果がtrueの場合、結果を格納する。
			if (result)
			{
				resultText = searchResult;
				searchResultText.Text = searchResult;
				saveButton.Focus();
			}
		}
		else
		{
			searchTargetText.Focus();
		}
	}

