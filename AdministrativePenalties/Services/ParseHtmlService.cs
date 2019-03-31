using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json; 

namespace AdministrativePenalties
{
  public class ParseHtmlService
  {
    public void ParseProtocolsPage(string documentNo, string vehicleNo2, string protocols)
    {
      var doc = new HtmlDocument();
      doc.LoadHtml(protocols);

      var rows = doc.DocumentNode.SelectNodes("//div[@id='content']/div/div/*[contains(@class,'row')]");
      var headers = doc.DocumentNode.SelectNodes($"//div[@class='headrow']/span").Select(i => i.InnerText).ToArray();

      var items = new List<Dictionary<string, string>>();
      for (int i = 1; i < rows.Count(); i++)
      {
        var item = new Dictionary<string, string>();
        for (int j = 1; j < headers.Count(); j++)
        {
          var value = rows[i].ChildNodes[j];
          item.Add(headers[j], value.InnerText);
        }
        items.Add(item);
      }

      var result = JsonConvert.SerializeObject(items);

      this.WriteToFile(documentNo, vehicleNo2, result);
    }

    public void WriteToFile(string documentNo, string vehicleNo2, string result)
    {
      using (StreamWriter outputFile = new StreamWriter($@"..\..\..\Json\{documentNo}_{vehicleNo2}.json"))
      {
        outputFile.Write(result);
      }
    }
  }

}
