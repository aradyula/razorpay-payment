//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace razorpaydemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class RazorPayCustomer
    {
        public int CID { get; set; }
        public string CustomerName { get; set; }
        public Nullable<long> MobileNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public Nullable<System.DateTime> PaymentDate { get; set; }
        
        public string Email { get; set; }
    }
}