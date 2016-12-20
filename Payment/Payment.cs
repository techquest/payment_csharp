using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interswitch;


namespace Interswitch
{
    public class Payment
    {

        Interswitch interswitch;

        public Payment(String clientId, String clientSecret, String environment = null)
        {
            interswitch = new Interswitch(clientId, clientSecret, environment);
        }

        public Dictionary<string, string> Authorize(string pan, string expDate, string cvv, string pin, string amt, string currency, string customerId, string reqRef)
        {
            string authData = interswitch.GetAuthData(pan, expDate, cvv, pin);
            var paymentReq = new 
            {
               customerId = customerId,
               amount = amt,
               transactionRef = reqRef,
               currency = currency,
               authData = authData
            };

            return interswitch.Send("/api/v2/purchases", "POST", paymentReq);
        }


        public Dictionary<string, string> ValidateCard(string pan, string expDate, string cvv, string pin, string reqRef)
        {
            string authData = interswitch.GetAuthData(pan, expDate, cvv, pin);
            var validateReq = new
            {
                transactionRef = reqRef,
                authData = authData
            };

            return interswitch.Send("/api/v2/purchases/validations", "POST", validateReq);
        }


        public Dictionary<string, string> VerifyOTP(string tranId, string otp)
        {
            var verifyOTPReq = new
            {
                paymentId = tranId,
                otp = otp
            };

            return interswitch.Send("/api/v2/purchases/otps/auths", "POST", verifyOTPReq);
        }

        public Dictionary<string, string> GetStatus(string reqRef, string amt)
        {
            Dictionary<string, string> httpHeader = new Dictionary<string, string>();
            httpHeader.Add("amount", amt);
            httpHeader.Add("transactionRef", reqRef);

            return interswitch.Send("/api/v2/purchases", "GET", null, httpHeader);
        }

    }
}
