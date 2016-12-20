using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interswitch;


namespace SampleProject
{
    public class Example
    {
        static string clientId = "IKIA9614B82064D632E9B6418DF358A6A4AEA84D7218";
        static string clientSecret = "XCTiBtLy1G9chAnyg0z3BcaFK4cVpwDg/GTw2EmjTZ8=";

        static void Main(string[] args)
        {
            // Payment
            bool hasRespCode = false;
            bool hasRespMsg = false;
            string httpRespCode = "400";
            string httpRespMsg = "Failed";
            Random rand = new Random();

            string amt = "35000";
            string currency = "NGN";
            string custId = "customer@myshop.com";
            var id = rand.Next(99999999);

            var pan = "6280511000000095";
            var expDate = "5004";
            var cvv = "111";
            var pin = "1111";
            var otpPan = "5061020000000000011";
            var otpExpDate = "1801";
            var otpCvv = "350";
            var otpPin = "1111";
            
            Payment payment = new Payment(clientId, clientSecret);
            
            // Payment - No OTP
            var paymentReqRef = "ISW-SDK-PAYMENT-" + id;
            var authorizeResp = payment.Authorize(pan, expDate, cvv, pin, amt, currency, custId, paymentReqRef);
            hasRespCode = authorizeResp.TryGetValue("CODE", out httpRespCode);
            hasRespMsg = authorizeResp.TryGetValue("RESPONSE", out httpRespMsg);
            Console.WriteLine("Authorize HTTP Code: " + httpRespCode);
            Console.WriteLine("Authorize HTTP Data: " + httpRespMsg);


            var getStatusResp = payment.GetStatus(paymentReqRef, amt);
            hasRespCode = getStatusResp.TryGetValue("CODE", out httpRespCode);
            hasRespMsg = getStatusResp.TryGetValue("RESPONSE", out httpRespMsg);
            Console.WriteLine("Get status HTTP Code: " + httpRespCode);
            Console.WriteLine("Get status HTTP Data: " + httpRespMsg);


            // Card Validation - No OTP
            var validateReqRef = "ISW-SDK-VALIDATE-" + id;
            var validateResp = payment.ValidateCard(pan, expDate, cvv, pin, validateReqRef);
            hasRespCode = validateResp.TryGetValue("CODE", out httpRespCode);
            hasRespMsg = validateResp.TryGetValue("RESPONSE", out httpRespMsg);
            Console.WriteLine("Validate HTTP Code: " + httpRespCode);
            Console.WriteLine("Validate HTTP Data: " + httpRespMsg);


            // Payment requires OTP
            var otpPaymentReqRef = "ISW-SDK-PAYMENT-OTP-" + id;
            var otpAuthorizeResp = payment.Authorize(otpPan, otpExpDate, otpCvv, otpPin, amt, currency, custId, otpPaymentReqRef);
            hasRespCode = otpAuthorizeResp.TryGetValue("CODE", out httpRespCode);
            hasRespMsg = otpAuthorizeResp.TryGetValue("RESPONSE", out httpRespMsg);
            Console.WriteLine("OTP Card Authorize HTTP Code: " + httpRespCode);
            Console.WriteLine("OTP Card Authorize HTTP Data: " + httpRespMsg);
            if (hasRespCode && hasRespMsg && (httpRespCode == "201" || httpRespCode == "202"))
            {
                Response response = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Response>(httpRespMsg);
                var otp = "123456";
                var otpResp = payment.VerifyOTP(response.paymentId, otp);
                hasRespCode = otpResp.TryGetValue("CODE", out httpRespCode);
                hasRespMsg = otpResp.TryGetValue("RESPONSE", out httpRespMsg);
                Console.WriteLine("Payment OTP HTTP Code: " + httpRespCode);
                Console.WriteLine("Payment OTP HTTP Data: " + httpRespMsg);
            }

            // Card Validation - requires OTP
            var otpValidateReqRef = "ISW-SDK-VALIDATE-OTP-" + id;
            var otpValidateResp = payment.ValidateCard(otpPan, otpExpDate, otpCvv, otpPin, otpValidateReqRef);
            hasRespCode = otpValidateResp.TryGetValue("CODE", out httpRespCode);
            hasRespMsg = otpValidateResp.TryGetValue("RESPONSE", out httpRespMsg);
            Console.WriteLine("OTP Card Validate HTTP Code: " + httpRespCode);
            Console.WriteLine("OTP Card Validate HTTP Data: " + httpRespMsg);


            Console.ReadKey();
        }
                
    }

     public class Response
    {
        public string paymentId { get; set; }
        public string transactionRef { get; set; }
        public PaymentMethod[] paymentMethods { get; set; }
    }

     public class PaymentMethod
     {
         public string paymentMethodTypeCode { get; set; }
         public string paymentMethodCode { get; set; }
         public string cardProduct { get; set; }
         public string panLast4Digits { get; set; }
         public string token { get; set; }
     }
}
