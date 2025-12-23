using System.Text;
using System.Xml.Linq;

namespace GameBooster.Service.Services
{
    public interface ISoapService
    {
        Task<string> NumberToWordsAsync(int number);
    }

    public class SoapService : ISoapService
    {
        private readonly HttpClient _httpClient;

        public SoapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> NumberToWordsAsync(int number)
        {
            // 1. SOAP Zarfını (Envelope) Hazırlıyoruz
            // Bu XML formatı, karşıdaki servisin beklediği standart formattır.
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                    <NumberToWords xmlns=""http://www.dataaccess.com/webservicesserver/"">
                      <ubiNum>{number}</ubiNum>
                    </NumberToWords>
                  </soap:Body>
                </soap:Envelope>";

            // 2. İsteği Hazırlıyoruz
            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.dataaccess.com/webservicesserver/NumberConversion.wso")
            {
                Content = content
            };

            // 3. Gönder ve Cevabı Al
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // 4. Gelen XML Cevabını Parçala (Parsing)
            // Cevap şuna benzer gelir: <m:NumberToWordsResult>sixteen</m:NumberToWordsResult>
            XDocument doc = XDocument.Parse(responseString);
            XNamespace ns = "http://www.dataaccess.com/webservicesserver/";
            
            var resultNode = doc.Descendants(ns + "NumberToWordsResult").FirstOrDefault();

            return resultNode?.Value ?? "Hata";
        }
    }
}