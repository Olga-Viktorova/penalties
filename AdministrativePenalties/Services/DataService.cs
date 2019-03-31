using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AdministrativePenalties
{
  public class DataService
  {
    public async Task<string> GetProtocols(string documentNo, string vehicleNo2, string domain)
    {
      var protocols = string.Empty;
      var indexUri = new Uri($"{domain}/index.php?lang=en");
      var submitUri = new Uri($"{domain}/submit-index.php");
      var protocolUri = new Uri($"{domain}/protocols.php?lang=en");

      var indexResponse = await this.GetResponseFromURI(indexUri, null);
      if (indexResponse.IsSuccessStatusCode)
      {
        var indexDocument = await indexResponse.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(indexDocument);

        var htmlInputCsrfToken = doc.DocumentNode.SelectSingleNode("//input[@name='csrf_token']");
        var htmlCaptureUrl = doc.DocumentNode.SelectSingleNode("//img[@id='captcha_code_img']");

        var csrfToken = htmlInputCsrfToken != null ? htmlInputCsrfToken.Attributes["value"].Value : string.Empty;
        var captureUrl = htmlCaptureUrl != null ? htmlCaptureUrl.Attributes["src"].Value : string.Empty;

        var cookie = indexResponse.Headers.GetValues("Set-cookie").FirstOrDefault();
        var a = cookie.Split(';');
        cookie = a.FirstOrDefault();

        this.DownloadCaptchaCode(cookie, captureUrl);
        Console.WriteLine("Enter Captcha Code");
        string captchaCode = Console.ReadLine();

        var formModel = new FormModel
        {
          ProtocolNo = string.Empty,
          VehicleNo1 = string.Empty,
          DocumentNo = documentNo,
          VehicleNo2 = vehicleNo2,
          CaptchaCode = captchaCode,
          Lang = "en",
          CsrfToken = csrfToken
        };

        var submitResponse = await this.PostResponseFromURI(submitUri, formModel, cookie);
        if (submitResponse.IsSuccessStatusCode)
        {
          var protocolsResponce = await this.GetResponseFromURI(protocolUri, cookie);
          protocols = await protocolsResponce.Content.ReadAsStringAsync();
        }
      }

      return protocols;
    }

    private async Task<HttpResponseMessage> GetResponseFromURI(Uri uri, string cookie)
    {
      var response = new HttpResponseMessage();
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Add("Host", "videos.police.ge");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
        client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
        client.DefaultRequestHeaders.Add("Referer", "http://videos.police.ge/index.php?lang=en");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        if (string.IsNullOrEmpty(cookie) == false)
        {
          client.DefaultRequestHeaders.Add("Cookie", cookie);
        }

        response = await client.GetAsync(uri);

        return response;
      }
    }

    private async Task<HttpResponseMessage> PostResponseFromURI(Uri uri, FormModel formModel, string cookie)
    {
      var response = new HttpResponseMessage();
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Add("Host", "videos.police.ge");
        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
        client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
        client.DefaultRequestHeaders.Add("Referer", "http://videos.police.ge/index.php?lang=en");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        client.DefaultRequestHeaders.Add("Origin", "http://videos.police.ge");
        client.DefaultRequestHeaders.Add("Cookie", cookie);

        response = await client.PostAsync(
          uri,
          new StringContent(formModel.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded"));

        return response;
      }
    }

    private void DownloadCaptchaCode(string cookie, string captureUrl)
    {
      using (WebClient wc = new WebClient())
      {
        wc.Headers.Add("cookie", cookie);
        wc.Headers.Add("Host", "videos.police.ge");
        wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");
        wc.Headers.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
        wc.Headers.Add("Referer", "http://videos.police.ge/index.php?lang=en");
        wc.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        wc.DownloadFile(captureUrl, "D:\\temp\\savedImage.png");
      }
    }
  }
}
