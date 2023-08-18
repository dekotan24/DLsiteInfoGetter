# DLsiteInfoGetter
## 使い方
	var main = new DLsiteInfoGetter.Main();
	string searchTarget = "RJ162718";  // "https://www.dlsite.com/maniax/work/=/product_id/RJ162718.html" でも可
	bool result = main.GetInfo(searchTarget, out string prodID, out string searchResult, out string circle, out string prodType, out string errMsg);
![image](https://github.com/dekotan24/DLsiteInfoGetter/assets/27037519/3483d1b9-9dae-481c-b06c-7c1477707547)
