﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarRental.EntityFramework;
using CarRentalWebSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CarRentalWebSite
{
    public class ReviewsController : Controller
    {
        private CarRentalWebSiteContext db = new CarRentalWebSiteContext();
        private ApplicationUserManager _userManager;

        public ReviewsController()
        {
        }

        public ReviewsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Reviews
        public ActionResult Index()
        {
            var reviews = db.ReviewSet.ToList();
            if (!User.IsInRole(CustomRoles.Administrator))
            {
                reviews = reviews.Where(review => review.Reservation != null && review.Reservation.Client_Id == User.Identity.GetUserId()).ToList();
            }
            return View(reviews);
            return View(db.ReviewSet.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.ReviewSet.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create(int? carId, int? reservationId)
        {
            ViewBag.Cars = new SelectList(db.CarSet, "Id", "FullName", carId);
            // Pass on a Review object with predefined Car field.
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Rating,Comment")] Review review, int carId, int? reservationId)
        {
            if (ModelState.IsValid)
            {
                review.Car = db.CarSet.Find(carId);
                review.Reservation = db.ReservationSet.Find(reservationId.GetValueOrDefault(0));
                db.ReviewSet.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.ReviewSet.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Rating,Comment,Car_Id,Reservation_Id")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.ReviewSet.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.ReviewSet.Find(id);
            db.ReviewSet.Remove(review);
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

        /// <summary>
        /// Used to retrieve user profile information, like FirstName, LastName and City.
        /// </summary>
        /// <param name="userId">Reservation.Client_Id</param>
        /// <returns>ApplicationUser</returns>
        public ApplicationUser GetUser(String userId)
        {
            return UserManager.FindById(userId);
        }

        public String UserName(String userId)
        {
            var u = GetUser(userId);
            if (string.IsNullOrEmpty(u.FirstName) && string.IsNullOrEmpty(u.LastName))
                return u.UserName;
            return u.FirstName + " " + u.LastName;
        }
    }
}
