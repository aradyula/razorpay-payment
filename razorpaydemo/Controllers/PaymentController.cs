using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace razorpaydemo.Controllers
{
    public class PaymentController : Controller
    {
        jayaEntities db = new jayaEntities();
        RazorpayClient client = new RazorpayClient("rzp_test_ILZ3agjpKO4BsN", "SPBpHnHOuWl9dcmcXe9bv3Z6");
        public ActionResult GetCustomerDetails()
        {
            var customer = db.RazorPayCustomers.ToList();
            return View(customer);
        }


        // GET: Payment
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult SendingPaymentLink(int id)
        {
            var Customer = db.RazorPayCustomers.Where(c => c.CID == id).FirstOrDefault();
            bool IsPaymentSent = false;
            try
            {
                Dictionary<string, object> options = new Dictionary<string, object>();
                Dictionary<string, string> customer = new Dictionary<string, string>();
                customer.Add("name",Customer.CustomerName);
                customer.Add("email", Customer.Email);
                customer.Add("contact", Convert.ToString(Customer.MobileNumber));
                options.Add("customer", customer);
                options.Add("type", "link");
                options.Add("amount", Customer.Amount);
                options.Add("currency", "INR");
                options.Add("description", "demo description");

                Invoice ObjInvoice = new Invoice().Create(options);
                var InvoiceId = Convert.ToString(ObjInvoice["id"]);

                if (!string.IsNullOrEmpty(InvoiceId))
                {
                    IsPaymentSent = true;
                }
                if (IsPaymentSent == true)
                {
                    TempData["SuccessMessage"] = "Payment Link Sent successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment Link Was Not Sent";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ViewBag.IsPaymentSent = IsPaymentSent;
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateOrder(Models.PaymentInitiateModel _requestData)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(10000000, 100000000).ToString();

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_heaWcmQVmBCVaD", "FBL5dhABLQVqHXEBZcdZ9zve");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", _requestData.amount * 100);  // Amount will in paise
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0"); // 1 - automatic  , 0 - manual
                                                 //options.Add("notes", "-- You can put any notes here --");
            Razorpay.Api.Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();

            // Create order model for return on view
            OrderModel orderModel = new OrderModel
            {
                orderId = orderResponse.Attributes["id"],
                razorpayKey = "rzp_test_heaWcmQVmBCVaD",
                amount = _requestData.amount * 100,
                currency = "INR",
                name = _requestData.name,
                email = _requestData.email,
                contactNumber = _requestData.contactNumber,
                address = _requestData.address,
                description = "Testing description"
            };

            // Return on PaymentPage with Order data
            return View("PaymentPage", orderModel);
        }

        public class OrderModel
        {
            public string orderId { get; set; }
            public string razorpayKey { get; set; }
            public int amount { get; set; }
            public string currency { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string contactNumber { get; set; }
            public string address { get; set; }
            public string description { get; set; }
        }


        [HttpPost]
        public ActionResult Complete()
        {
            // Payment data comes in url so we have to get it from url

            // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
            string paymentId = Request.Params["rzp_paymentid"];

            // This is orderId
            string orderId = Request.Params["rzp_orderid"];

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_heaWcmQVmBCVaD", "FBL5dhABLQVqHXEBZcdZ9zve");

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
            string amt = paymentCaptured.Attributes["amount"];

            //// Check payment made successfully

            if (paymentCaptured.Attributes["status"] == "captured")
            {
                // Create these action method
                return RedirectToAction("Success");
            }
            else
            {
                return RedirectToAction("Failed");
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Failed()
        {
            return View();
        }
    }
}