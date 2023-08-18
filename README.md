# DLsiteInfoGetter
## 使い方
	var main = new DLsiteInfoGetter.Main();
	string searchTarget = "RJ162718";  // "https://www.dlsite.com/maniax/work/=/product_id/RJ162718.html" でも可
	bool result = main.GetInfo(searchTarget, out string prodID, out string title, out string circle, out string prodType, out string errMsg);

 result：true:エラーなし／false:エラーあり
 
 prodID：作品ID
 
 title：作品名
 
 circle：サークル名
 
 prodType：作品種別（RJ|VJ|RE|VE|BJ|AJ）
 
 errMsg：エラーが発生した場合のエラーエッセージ
![image](https://github.com/dekotan24/DLsiteInfoGetter/assets/27037519/3483d1b9-9dae-481c-b06c-7c1477707547)
