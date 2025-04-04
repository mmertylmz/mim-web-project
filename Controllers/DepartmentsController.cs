﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MIM.Config;
using MIM.Models;
using PagedList;

namespace MIM.Controllers
{
    public class DepartmentsController : Controller
    {
        private MIMDBContext db = new MIMDBContext();

        // GET: Departments
        public async Task<ActionResult> Index()
        {
            var departments = db.Departments.Include(d => d.Organization);
            
            return View(await departments.ToListAsync());
        }

        public IPagedList<Department> GetDepartmentList(Department department, int? page)
        {
            var _page = page ?? 1;
            var departments = db.Departments.Include(u => u.Organization).Where(x => x.OrganizationID == Organization.current.OrganizationID);
            IPagedList<Department> filtering_department = departments.ToList().ToPagedList(_page, MvcApplication.ListPerPage);
            if (department.DepartmentID > 0) filtering_department = filtering_department.Where(x => x.DepartmentID == department.DepartmentID).ToList().ToPagedList(_page, MvcApplication.ListPerPage);
            if (department.Name != null) filtering_department = filtering_department.Where(x => x.Name.Contains(department.Name)).ToList().ToPagedList(_page, MvcApplication.ListPerPage);
            if (department.Description != null) filtering_department = filtering_department.Where(x => x.Description.Contains(department.Description)).ToList().ToPagedList(_page, MvcApplication.ListPerPage);
            if (department.UserID != null) filtering_department = filtering_department.Where(x => x.UserID == department.UserID).ToList().ToPagedList(_page, MvcApplication.ListPerPage);
            return filtering_department;
        }

        // GET: /Departments/Table
        public ActionResult Table(Department department, int? page)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Table", "Departments");
            if (!grant) return View("");
            var _page = page ?? 1;
            var departments = GetDepartmentList(department, page);            
            ViewBag.UserID = new SelectList(db.Users.Where(x => x.OrganizationID == Organization.current.OrganizationID), "UserID", "fullname");
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<ActionResult> Show(int? id)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Show", "Departments");
            if (!grant) return View("");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Create", "Departments");
            if (!grant) return View("");
            ViewBag.UserID = new SelectList(db.Users.Where(x => x.OrganizationID == Organization.current.OrganizationID), "UserID", "fullname");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentID,Name,Description,UserID")] Department department)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Create", "Departments");
            if (!grant) return View("");
            department.OrganizationID = Organization.current.OrganizationID;
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users.Where(x => x.OrganizationID == Organization.current.OrganizationID), "UserID", "fullname");
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Edit", "Departments");
            if (!grant) return View("");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users.Where(x => x.OrganizationID == Organization.current.OrganizationID), "UserID", "fullname");
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentID,Name,Description,UserID")] Department department)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Edit", "Departments");
            if (!grant) return View("");
            department.OrganizationID = Organization.current.OrganizationID;
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users.Where(x => x.OrganizationID == Organization.current.OrganizationID), "UserID", "fullname");
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Delete", "Departments");
            if (!grant) return View("");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            bool grant = ViewBag.grant = MIM.Models.User.Current().isGranted("Delete", "Departments");
            if (!grant) return View("");
            Department department = await db.Departments.FindAsync(id);
            db.Departments.Remove(department);
            await db.SaveChangesAsync();
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
