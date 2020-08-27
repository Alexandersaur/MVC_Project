using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MVC_Project.Helpers;
using MVC_Project.Models;
using PagedList;
using PagedList.Mvc;

namespace MVC_Project.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 10; //Specifies the number of poasts per page
            int pageNumber = (page ?? 1); //?? is a null coalescing operator

            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize);
            //return View(db.BlogPosts.ToList());
            return View(publishedBlogPosts);
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if(searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(b => b.Title.Contains(searchStr) ||
                                           b.Body.Contains(searchStr) || 
                                           b.Comments.Any(c => c.CommentBody.Contains(searchStr) ||
                                                               c.Author.FirstName.Contains(searchStr) ||
                                                               c.Author.LastName.Contains(searchStr) ||
                                                               c.Author.DisplayName.Contains(searchStr) ||
                                                               c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blogPost = db.BlogPosts.FirstOrDefault(b => b.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Title,Slug,Body,Abstract,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                blogPost.Created = DateTime.Now;
                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                    return View(blogPost);
                }
                if (db.BlogPosts.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The Title must be unique.");
                    return View(blogPost);
                }

                blogPost.Slug = slug;
                blogPost.Created = DateTime.Now;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var justFileName = Path.GetFileNameWithoutExtension(fileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    fileName = $"{justFileName}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaURL = "/Uploads/" + fileName;
                }

                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ID,Created,Updated,Title,Slug,Body,Abstract,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                    return View(blogPost);
                }
                //if (db.BlogPosts.Any(p => p.Slug == slug))
                //{
                //    ModelState.AddModelError("Title", "The Title must be unique.");
                //    return View(blogPost);
                //}

                blogPost.Slug = slug;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaURL = "/Uploads/" + fileName;
                }

                blogPost.Updated = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
