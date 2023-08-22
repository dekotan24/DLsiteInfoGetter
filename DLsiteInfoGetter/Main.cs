using AngleSharp.Html.Parser;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DLsiteInfoGetter
{
	public class DLsiteInfo
	{
		// プロパティ
		public string ProductId { get; private set; } // 作品ID
		public string Title { get; private set; } // 作品名
		public string Circle { get; private set; } // サークル名
		public string ProductType { get; private set; } // 作品種別
		public string ImageUrl { get; private set; } // 作品画像のURL

		// コンストラクタ
		private DLsiteInfo(string productId, string title, string circle, string productType, string imageUrl)
		{
			ProductId = productId;
			Title = title;
			Circle = circle;
			ProductType = productType;
			ImageUrl = imageUrl;
		}

		// 作品情報を取得するメソッド
		public static DLsiteInfo GetInfo(string rawText)
		{
			// 入力されたテキストを作品ID形式に整形する
			string productId = TextAnalyze(rawText);

			if (string.IsNullOrEmpty(productId))
			{
				throw new ArgumentException("入力されたテキストが作品ID形式ではありません。");
			}

			// 作品種別を判定する
			string productType = productId.Substring(0, 2);

			// 作品情報を取得するURLを構築する
			string url = "https://www.dlsite.com/" + (productType == "RJ" ? "maniax" : "pro") + "/work/=/product_id/" + productId + ".html";

			// 作品情報を取得する
			using (WebClient wc = new WebClient())
			{
				wc.Encoding = Encoding.UTF8;
				string html = wc.DownloadString(url);

				// HTMLからタイトル、サークル名、画像URLを抽出する
				var parser = new HtmlParser();
				var doc = parser.ParseDocument(html);
				var titleElement = doc.QuerySelector("#work_name");
				var circleElement = doc.QuerySelector("span.maker_name > a:nth-child(1)");
				var imageElement = doc.QuerySelector("meta[property='og:image']");
				string title = titleElement.TextContent;
				string circle = circleElement.TextContent;
				string imageUrl = imageElement.GetAttribute("content");

				// 作品情報のインスタンスを返す
				return new DLsiteInfo(productId, title, circle, productType, imageUrl);
			}
		}

		// 入力されたテキストを作品ID形式に整形するメソッド
		private static string TextAnalyze(string rawText)
		{
			string upperText = rawText.ToUpper();

			// URLから一番最初のRJもしくはVJなどの作品種別+数字（6桁以上）を抽出する
			Match match = Regex.Match(upperText, @"(RJ|VJ|RE|VE|BJ|AJ)[0-9]{6,}");
			if (!match.Success)
			{
				return string.Empty;
			}

			// 抽出した文字列を作品IDとして返す
			string productId = match.Value;

			return productId;
		}
	}
}
