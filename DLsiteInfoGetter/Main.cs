//
// DLsiteInfoGetter：DLsiteから情報を取得するライブラリ
//
// 作者：小倉照孤（Ogura Deko）
// バージョン：Ver.1.2
// メール：apps★fanet.work
// Webサイト：https://fanet.work （日本限定）
//
// Ver.1.0：初回リリース
// Ver.1.1：-
// Ver.1.2：値の返却方法の変更
// Ver.1.3：販売日、シナリオ、イラスト、声優、ジャンルに対応
//


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
		/// <summary>
		/// 作品ID
		/// </summary>
		public string ProductId { get; private set; }
		/// <summary>
		/// 作品名
		/// </summary>
		public string Title { get; private set; }
		/// <summary>
		/// サークル名
		/// </summary>
		public string Circle { get; private set; }
		/// <summary>
		/// 作品種別
		/// </summary>
		public string ProductType { get; private set; }
		/// <summary>
		/// 作品画像のURL
		/// </summary>
		public string ImageUrl { get; private set; }
		/// <summary>
		/// 販売日
		/// </summary>
		public DateTime SellDate { get; private set; }
		/// <summary>
		/// シナリオ
		/// </summary>
		public string[] ScenarioWriter { get; private set; }
		/// <summary>
		/// イラスト
		/// </summary>
		public string[] Illustrator { get; private set; }
		/// <summary>
		/// 声優
		/// </summary>
		public string[] VoiceActor { get; private set; }
		/// <summary>
		/// ジャンル
		/// </summary>
		public string[] Genre { get; private set; }

		// コンストラクタ
		/// <summary>
		/// 検索結果を返却します。
		/// </summary>
		/// <param name="productId">作品ID</param>
		/// <param name="title">作品名</param>
		/// <param name="circle">サークル名</param>
		/// <param name="productType">作品種別</param>
		/// <param name="imageUrl">作品画像のURL</param>
		/// <param name="voiceActor">声優</param>
		/// <param name="sellDate">販売日</param>
		/// <param name="genre">ジャンル</param>
		private DLsiteInfo(string productId, string title, string circle, string productType, string imageUrl, DateTime sellDate, string[] scenarioWriter, string[] illustrator, string[] voiceActor, string[] genre)
		{
			ProductId = productId;
			Title = title;
			Circle = circle;
			ProductType = productType;
			ImageUrl = imageUrl;
			SellDate = sellDate;
			ScenarioWriter = scenarioWriter;
			Illustrator = illustrator;
			VoiceActor = voiceActor;
			Genre = genre;
		}

		// 作品情報を取得するメソッド
		/// <summary>
		/// 作品情報を取得するメソッド
		/// </summary>
		/// <param name="URL_or_ID">DLsiteのURLもしくは作品ID</param>
		/// <returns>検索結果</returns>
		/// <exception cref="ArgumentException"></exception>
		public static DLsiteInfo GetInfo(string URL_or_ID)
		{
			// 入力されたテキストを作品ID形式に整形する
			string productId = TextAnalyze(URL_or_ID);

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

				// 作品名
				var titleElement = doc.QuerySelector("#work_name");

				// サークル名
				var circleElement = doc.QuerySelector("span.maker_name > a:nth-child(1)");

				// サムネ画像
				var imageElement = doc.QuerySelector("meta[property='og:image']");

				// 販売日
				var sellDateElement = doc.QuerySelector("tr > th:contains('販売日') + td > a");

				// シナリオ
				var scElements = doc.QuerySelectorAll("tr > th:contains('シナリオ') + td > a");

				// イラスト
				var ilElements = doc.QuerySelectorAll("tr > th:contains('イラスト') + td > a");

				// 声優
				var vaElements = doc.QuerySelectorAll("tr > th:contains('声優') + td > a");

				// ジャンル
				var genreElements = doc.QuerySelectorAll("tr > th:contains('ジャンル') + td > a");

				/* 値確定 */
				string title = titleElement.TextContent;
				string circle = circleElement.TextContent;
				string imageUrl = imageElement.GetAttribute("content");
				DateTime sellDate = Convert.ToDateTime(sellDateElement.TextContent);
				// 取得した要素のテキスト部分をstring[]配列に格納する（シナリオ）
				string[] scenarioArray = new string[scElements.Length];
				int i = 0;
				foreach (var element in scElements)
				{
					scenarioArray[i] = element.TextContent;
					i++;
				}
				// 取得した要素のテキスト部分をstring[]配列に格納する（イラスト）
				string[] illustArray = new string[ilElements.Length];
				i = 0;
				foreach (var element in ilElements)
				{
					illustArray[i] = element.TextContent;
					i++;
				}
				// 取得した要素のテキスト部分をstring[]配列に格納する（声優）
				string[] voiceActor = new string[vaElements.Length];
				i = 0;
				foreach (var element in vaElements)
				{
					voiceActor[i] = element.TextContent;
					i++;
				}
				// 取得した要素のテキスト部分をstring[]配列に格納する（ジャンル）
				string[] genreArray = new string[genreElements.Length];
				i = 0;
				foreach (var element in genreElements)
				{
					genreArray[i] = element.TextContent;
					i++;
				}

				// 作品情報のインスタンスを返す
				return new DLsiteInfo(productId, title, circle, productType, imageUrl, sellDate, scenarioArray, illustArray, voiceActor, genreArray);
			}
		}

		/// <summary>
		/// 入力されたテキストを作品ID形式に整形するメソッド
		/// </summary>
		/// <param name="rawText">DLsiteのURLもしくは作品ID</param>
		/// <returns></returns>
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
