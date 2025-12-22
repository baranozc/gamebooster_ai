using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq; // XML okumak için gerekli kütüphane

namespace GameBooster.Web.ViewComponents
{
    public class ExchangeRateViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string exchangeRate = "Bağlantı Hatası";

            try
            {
                // 1. TCMB'nin Günlük Kur Servisine Bağlan (XML)
                string url = "https://www.tcmb.gov.tr/kurlar/today.xml";
                XDocument xmlDoc = XDocument.Load(url);

                // 2. XML içinden 'USD' kodlu satırı bul ve 'ForexSelling' (Satış) değerini al
                var usdRate = xmlDoc.Descendants("Currency")
                    .FirstOrDefault(x => x.Attribute("Kod")?.Value == "USD")?
                    .Element("ForexSelling")?.Value;

                if (usdRate != null)
                {
                    // Gelen veri "34.5000" gibi olabilir, sonundaki sıfırları atalım
                    exchangeRate = usdRate.Substring(0, 5); 
                }
            }
            catch
            {
                // İnternet yoksa veya TCMB sitesi çöktüyse site patlamasın, "-" yazsın.
                exchangeRate = "-";
            }

            // 3. Veriyi View'a gönder
            return View("Default", exchangeRate);
        }
    }
}