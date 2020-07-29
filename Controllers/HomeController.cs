using MVC_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    { 
        private ApplicationDbContext db = new ApplicationDbContext();
    
        public ActionResult Index()
        {
            //this is for ALL blog posts
            //var allBlogPosts = db.BlogPosts.ToList();
            //this is for just the published ones

            var allBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToList();
            return View(allBlogPosts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}