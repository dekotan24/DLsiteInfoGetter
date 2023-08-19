using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DLsiteInfoGetter
{
	public class Main
	{
		// 共通変数
		private bool foundFlg = true;
		private string prodID = string.Empty;
		private string prodTitle = string.Empty;
		private string prodCircle = string.Empty;
		private string prodType = string.Empty;
		private string prodImage = string.Empty;
		private string errMsg = string.Empty;

		// 共通関数
		/// <summary>
		/// DLsiteから作品情報を取得します。
		/// </summary>
		/// <param name="rawText">作品IDもしくはURL</param>
		/// <param name="productID">作品ID</param>
		/// <param name="title">作品名</param>
		/// <param name="circle">サークル名</param>
		/// <param name="productType">作品種別</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>True：作品が存在する / False：作品が存在しない</returns>
		public bool GetInfo(string rawText, out string productID, out string title, out string circle, out string productType, out string imageUrl, out string errorMessage)
		{
			string ansText = string.Empty;

			if (!string.IsNullOrEmpty(rawText))
			{
				ansText = textAnalyze(rawText);

				if (!string.IsNullOrEmpty(ansText))
				{
					searchExecute(ansText, rawText);
				}
				else
				{
					setErrMsg("切り出し結果が不正です。(E002)");
				}
			}
			else
			{
				setErrMsg("URLが不正です。(E001)");
			}

			// 結果格納
			productID = prodID;
			title = prodTitle;
			circle = prodCircle;
			productType = prodType;
			imageUrl = prodImage;
			errorMessage = errMsg;

			return foundFlg;
		}

		/// <summary>
		/// 入力されたテキストを作品ID形式に整形します。
		/// </summary>
		/// <param name="rawText">生の値（URLもしくは作品ID形式）</param>
		/// <returns>作品ID形式</returns>
		private string textAnalyze(string rawText)
		{
			string ansText = string.Empty;
			int beginID = -1;
			string productTypeID = string.Empty;
			string upperText = rawText.ToUpper();

			// RJ：一般向け作品のID。例: RJ123456
			if (upperText.Contains("RJ"))
			{
				productTypeID = "RJ";
			}
			// VJ：18禁作品のID。例: VJ123456
			else if (upperText.Contains("VJ"))
			{
				productTypeID = "VJ";
			}
			// RE：一般向け作品の体験版のID。例: RE123456
			else if (upperText.Contains("RE"))
			{
				productTypeID = "RE";
			}
			// VE：18禁作品の体験版のID。例: VE123456
			else if (upperText.Contains("VE"))
			{
				productTypeID = "VE";
			}
			// BJ：ブックマーク用のID。例: BJ123456
			else if (upperText.Contains("BJ"))
			{
				productTypeID = "BJ";
			}
			// AJ：アフィリエイト用のID。例: AJ123456
			else if (upperText.Contains("AJ"))
			{
				productTypeID = "AJ";
			}
			// 識別不可
			else
			{
				productTypeID = "NA";
			}

			// 作品ID計算処理
			beginID = upperText.IndexOf((productTypeID));
			if (beginID < 0)
			{
				return string.Empty;
			}

			// 文字切り出し
			ansText = upperText.Substring(beginID);

			// URLの可能性を考慮し、不要文字列を削除する
			ansText = ansText.Replace("/", "").Replace(".HTML", "");

			// 大文字化
			ansText = ansText.ToUpper();

			// (RJ|VJ|RE|VE|BJ|AJ)xxxxx形式かチェックする
			if (!Convert.ToBoolean(new Regex("^(" + productTypeID + ")+[0-9]+$").IsMatch(ansText)))
			{
				setErrMsg("正規表現チェックでエラーが発生しました。文字列の切り出し結果が不正です。(E003)[" + ansText + "]");
				return string.Empty;
			}

			// 結果反映
			prodType = productTypeID;
			prodID = ansText;

			return ansText;
		}

		/// <summary>
		/// 作品情報取得処理
		/// </summary>
		/// <param name="rawText">作品ID形式の値</param>
		/// <param name="rawUrl">生の値</param>
		private void searchExecute(string rawText, string rawUrl)
		{
			// イメージ作成用のURL構築
			string url = string.Empty;
			string str = Regex.Replace(rawText, @"[^0-9]", "");
			int id = int.Parse(str);
			string fullID = prodType;
			int kuriage = (id / 1000) * 1000 + 1000;
			string kuriage2 = kuriage.ToString().PadLeft(rawText.Length - prodType.Length, '0');

			fullID = prodType + id.ToString().PadLeft(rawText.Length - prodType.Length, '0');

			prodImage = "https://img.dlsite.jp/modpub/images2/work/" + (prodType.Equals("RJ") ? "doujin" : "professional") + "/" + prodType + kuriage2 + "/" + fullID + "_img_main.jpg";

			// 作品名取得
			if (!rawUrl.Contains("dlsite.com"))
			{
				rawUrl = "https://www.dlsite.com/" + (prodType.Equals("RJ") ? "maniax" : "pro") + "/work/=/product_id/" + rawUrl.ToUpper();
			}
			if (!rawUrl.Contains(".html"))
			{
				rawUrl += ".html";
			}

			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				WebClient wc = new WebClient();
				System.IO.Stream st = wc.OpenRead(rawUrl);

				var parser = new AngleSharp.Html.Parser.HtmlParser();
				var doc = parser.ParseDocument(st);

				var title2 = doc.QuerySelector("#work_name");
				var circle2 = doc.QuerySelector("span.maker_name > a:nth-child(1)");
				string title = title2.TextContent;
				string circle = circle2.TextContent;
				prodTitle = title;
				prodCircle = circle;
			}
			catch (Exception ex)
			{
				setErrMsg("作品詳細取得中にエラーが発生しました。(E004)[" + ex.Message + "]");
			}

			return;
		}

		/// <summary>
		/// エラーメッセージをセットします。
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		private void setErrMsg(string message)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(errMsg))
			{
				sb.Append(errMsg).Append("\n").Append(message);
			}
			else
			{
				sb.Append(message);
			}

			errMsg = sb.ToString();
			foundFlg = false;
		}
	}
}