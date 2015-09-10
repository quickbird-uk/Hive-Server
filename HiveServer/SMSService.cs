using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiveServer
{
    public class SMSService
    {
        private const string AzureConnectorURL = @"https://twilioconnector1b82e2c377ca479682946ffb3ed55bd8.azurewebsites.net/";
        private const string SubscribtionID = "f7687fca-13f6-4973-8853-25796dec7d8c";
        private const string APIMessages = "twilio/Messages";



        public static async Task<bool> SendMessage(string number, string message)
        {      
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, AzureConnectorURL + APIMessages);
            SMSMessageOut sms = new SMSMessageOut { To = number, Body = message };
            string serialisedSms = JsonConvert.SerializeObject(sms); 
            requestMessage.Content = new StringContent(serialisedSms, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            HttpResponseMessage httpResponce = await client.SendAsync(requestMessage);
            if (httpResponce.IsSuccessStatusCode)
            {
                string serialisedResponce = await httpResponce.Content.ReadAsStringAsync(); 
                SMSMessageResponce reponce = JsonConvert.DeserializeObject<SMSMessageResponce>(serialisedResponce);
             }

            return true;
        }
    }

    public class SMSMessageOut
    {
        public string From = "+441473379434";
        public string To = string.Empty;
        public string Body = string.Empty; 
    }

    public class SMSMessageResponce
    {
        public string status;
        public string sid;
        public string error_code;
        public string error_message;
        public string date_created;
        public string date_sent;
        public string direction;
        public string price; 
    }

    public class SentMessagePast :SMSMessageResponce
    {
        public string From;
        public string To;
        public string Body;
    }
}