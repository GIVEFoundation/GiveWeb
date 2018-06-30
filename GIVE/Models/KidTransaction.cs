using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIVE.Models
{
    public class KidTransaction
    {
        public int TransactionId { get; set; }

        public string Sender { get; set; }
        public string Recepient { get; set; }

        public string RecepientComment { get; set; }
        public string SenderComment { get; set; }
        public string TransactionDate { get; set; }

    }
}