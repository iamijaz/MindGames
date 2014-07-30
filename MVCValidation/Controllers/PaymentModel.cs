using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MVCValidation.Controllers
{
    public class PaymentModel
    {
        [Required]
        public string Name { get; set; }
        
        public DateTime Date { get; set; }
        [Required]
        public string CreditCardNumber { get; set; }

    }
}
