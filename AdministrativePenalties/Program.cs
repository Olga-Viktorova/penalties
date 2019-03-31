using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AdministrativePenalties
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Enter Domain");
      string domain = Console.ReadLine();

      var dataService = new DataService();
      var parseHtmlService = new ParseHtmlService();

      var fileStream = new FileStream(@"..\..\..\Temp\Penalties.txt", FileMode.Open, FileAccess.Read);
      using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
      {
        string line;
        while ((line = streamReader.ReadLine()) != null)
        {
          var documentNo = line.Split(" ").FirstOrDefault();
          var vehicleNo2 = line.Split(" ").LastOrDefault();

          Console.WriteLine($"Tech-passport number: {documentNo}");
          Console.WriteLine($"Car state number: {vehicleNo2}");

          var protocolsResult = Task.Run(() => dataService.GetProtocols(documentNo, vehicleNo2, domain));
          protocolsResult.Wait();
          var protocols = protocolsResult.Result;

          var isCsrfTokenExist = protocols.Contains("csrf_token");
          if (isCsrfTokenExist)
          {
            Console.WriteLine("invalid capchure");
          }
          else
          {
            parseHtmlService.ParseProtocolsPage(documentNo, vehicleNo2, protocols);

          }
        }
      }


      //Console.WriteLine("Enter Tech-passport number: ");
      //var documentNo = "AM0272491"; //Console.ReadLine();

      //Console.WriteLine("Enter Car state number:");
      //var vehicleNo2 = "QU770EN"; //Console.ReadLine();

      //var protocolsResult = Task.Run(() => dataService.GetProtocols(documentNo, vehicleNo2));
      //protocolsResult.Wait();
      //var protocols = protocolsResult.Result;

      //parseHtmlService.ParseProtocolsPage(documentNo, vehicleNo2, protocols);

      Console.ReadLine();
    }
  }
}
