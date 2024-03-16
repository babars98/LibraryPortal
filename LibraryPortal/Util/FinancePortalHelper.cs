using LibraryPortal.Interface;
using LibraryPortal.Models;
using Newtonsoft.Json;

namespace LibraryPortal.Util
{
    public class FinancePortalHelper : IFinancePortalHelper
    {
        private readonly IConfigurationRoot configuration;
        private readonly string API_URL = string.Empty;

        public FinancePortalHelper()
        {
            configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            //Read API url from config file
            API_URL = $"{configuration.GetValue<string>("FinanceAppUrl")}api/";
        }

        /// <summary>
        /// Create Invoice if the student is enrolled in a course or borrows a book
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="fee"></param>
        /// <returns></returns>
        public string CreateInvoice(string studentId, double fee)
        {
            var obj = new Invoice
            {
                StudentId = studentId,
                Fee = fee,
                InvoiceType = InvoiceType.Fine,
                DueDate = DateTime.Now.AddDays(7)
            };

            var url = $"{API_URL}CreateInvoice";

            var apiHelper = new APIHelper();

            var response = apiHelper.PostMethod(url, obj);

            var result = JsonConvert.DeserializeObject<string>(response);

            return result;
        }
    }
}
