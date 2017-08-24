using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using Yst.Context;
using Yst.ViewModels;
using YstIdentity.Models;

namespace Term.Web.Controllers
{
    /// <summary>
    /// Управление  пользователями
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        // GET: /RolesAdmin/
        private readonly RoleStore<IdentityRole> _roleStore;
        private readonly RoleManager<IdentityRole> _roleMngr;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly AppDbContext _db;

        public UsersAdminController():this (new AppDbContext())
        {
            
        }

        public UsersAdminController(AppDbContext appDbContext)
        {
            // TODO: Complete member initialization
            this._db = appDbContext;
            _roleStore= new RoleStore<IdentityRole>(_db);
            _roleMngr = new RoleManager<IdentityRole>(_roleStore); 
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
        }


        public ActionResult Index(UsersViewModel model)
        {

            var query = _userManager.Users;

            if (!String.IsNullOrEmpty(model.FilterByName)) query = query.Where(p => p.UserName.StartsWith(model.FilterByName));

            if (model.OrderBy == OrderByUserRole.NameAsc) query = query.OrderBy(p => p.UserName);
            else query = query.OrderByDescending(p => p.UserName);

            model.UsersPaged = query.ToPagedList(model.PageNumber, model.ItemsPerPage);


            return View(model);
           
        }


        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await _roleMngr.Roles.ToListAsync(), "Name", "Name");
            return View();
        }


        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.UserName, 
                    Email = userViewModel.Email, 
                    IsPartner = userViewModel.IsPartner,
                 //   IsSupplier = userViewModel.IsSupplier,
                //   IsSaleRep = userViewModel.IsSaleRep,
                   DepartmentId = userViewModel.DepartmentId,
                      PartnerId = userViewModel.PartnerId,
                    PartnerPointId = userViewModel.PartnerPointId,
                   SupplierId = userViewModel.SupplierId
                };
                var adminresult = await _userManager.CreateAsync(user, userViewModel.Password);

                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        foreach (var role in selectedRoles)
                        {
                            var result = await _userManager.AddToRoleAsync(user.Id, role);    
                        
                        
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await _roleMngr.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(_roleMngr.Roles, "Name", "Name");
                    return View();

                }
                //Add User to the selected Roles 
               /* if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await _userService.UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await _userService.RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(_userService.RoleManager.Roles, "Name", "Name");
                    return View();

                } */
                return RedirectToAction("Index");
            }
           // ViewBag.RoleId = new SelectList(_userService.RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PartnerPointId = user.PartnerPointId,
                PartnerId = user.PartnerId,
                IsPartner = user.IsPartner,
                
                DepartmentId = user.DepartmentId,
                SupplierId = user.SupplierId,

                RolesList = _roleMngr.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }


       

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( EditUserViewModel editUser, params string[] selectedRole)
        {
            
            if (ModelState.IsValid)
            {
                IdentityResult result=null;

                var user = await _userManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // не меняем имя пользователя
                user.Email = editUser.Email;
                user.DepartmentId = editUser.DepartmentId;
                user.PartnerId = editUser.PartnerId;
                user.IsPartner = editUser.IsPartner;
                user.SupplierId = editUser.SupplierId;

                await _db.SaveChangesAsync();

                var userRoles = await _userManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };


                foreach (var role in  userRoles)
                {
                    result = await _userManager.RemoveFromRoleAsync(user.Id, role);

                }

                
                foreach (var role in selectedRole)
                {
                    result = await _userManager.AddToRoleAsync(user.Id, role);    
                }
                
                
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                } 
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var user = await _userManager.FindByIdAsync(id);

            ViewBag.RoleNames = await _userManager.GetRolesAsync(user.Id);

            return View(user);
        }

        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }


                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        

    }
}
