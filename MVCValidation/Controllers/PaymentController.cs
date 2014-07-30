using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCValidation.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/
        /*
        public ActionResult Index()
        {
            var model=new PaymentModel();
            return View(model);
        }*/

        public ActionResult Index(PaymentModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PaymentModel payment)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", payment);
            }

            return RedirectToAction("Index", payment);
        }
    }
}
