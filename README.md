# DLsite Information Getter
## 概要
DLsiteのURL or 作品IDを元に、作品名、サークル名、作品種別等を取得するDLLです。

解析に[AngleSharp](https://github.com/AngleSharp/AngleSharp)を使用していますので、nugetで取得してください。

.NET Framework 4.8.1で開発しました。



## 使い方
	using DLsiteInfoGetter;
 
	string searchTarget = "RJ162718";  // "https://www.dlsite.com/maniax/work/=/product_id/RJ162718.html" などでも可
	DLsiteInfo result = DLsiteInfo.GetInfo(searchTarget);


| I/O | パラメータ | 型 | 概要 |
|-----|-------------|----|----|
|  I  | searchTarget | string | 検索を行う作品IDもしくはURL |
|  O  | result | DLsiteInfo | 返却値（パラメータは下記を参照） |
|  -  | DLsiteInfo.ProductId | string | 作品ID |
|  -  | DLsiteInfo.Title | string | 作品名称 |
|  -  | DLsiteInfo.Circle | string | サークル名 |
|  -  | DLsiteInfo.ProductType | string | 作品種別（RJ、VJなど） |
|  -  | DLsiteInfo.ImageUrl | string | 作品のサムネ画像URL |



## 実装例（C#であればtry-catchで例外を検知できる…らしい。）
	private void SearchButton_Click(object sender, EventArgs e)
	{
		SearchResultText.Text = string.Empty;
		resultText = string.Empty;
		resultImagePath = string.Empty;

		if (!string.IsNullOrEmpty(SearchTargetText.Text.Trim()))
		{
			try
			{
				DLsiteInfo result = DLsiteInfo.GetInfo(SearchTargetText.Text.Trim());
				resultText = result.Title;
				SearchResultText.Text = result.Title;
				ImageText.Text = result.ImageUrl;
				ImageBox.ImageLocation = result.ImageUrl;
			}
			catch (Exception ex)
			{
				ImageBox.ImageLocation = null;
				MessageBox.Show(ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			SaveButton.Focus();
		}
		else
		{
			SearchTargetText.Focus();
		}
	}

 
![image](https://github.com/dekotan24/DLsiteInfoGetter/assets/27037519/47761f44-78d6-45e5-8b2d-fef498ab74f2)

![image](https://github.com/dekotan24/DLsiteInfoGetter/assets/27037519/4720e3ff-fad5-44da-a963-6d2bc3a8b136)

