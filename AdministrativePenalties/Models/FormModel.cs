using System;
using System.Collections.Generic;
using System.Text;

namespace AdministrativePenalties
{
  public class FormModel
  {
    public string ProtocolNo { get; set; }

    public string VehicleNo1 { get; set; }

    public string DocumentNo { get; set; }

    public string VehicleNo2 { get; set; }

    public string CaptchaCode { get; set; }

    public string Lang { get; set; }

    public string CsrfToken { get; set; }

    public override string ToString()
    {
      return $"protocolNo=&vehicleNo1=&documentNo={DocumentNo}&vehicleNo2={VehicleNo2}&captcha_code={CaptchaCode}&lang=en&csrf_token={CsrfToken}";
    }
  }
}
