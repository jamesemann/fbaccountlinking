using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebViews.Controllers
{
    public class FakeLoginController : Controller
    {
        // GET: FakeLogin
        public ActionResult Index()
        {
            // in a real login application you would validate first, here i dont care
            ViewBag.fbAccountLinkingToken = base.Request.Params["account_linking_token"];
            ViewBag.fbRedirectUri = base.Request.Params["redirect_uri"];
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string formRedirectUrl)
        {
            // validate password

            return Redirect(formRedirectUrl + "&authorization_code=AUTHORIZATION_CODE");
        }
    }
}