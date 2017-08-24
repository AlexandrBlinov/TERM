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
using Yst.Context;
using Yst.ViewModels;
using YstIdentity.Models;

namespace Term.Web.Controllers
{
    /// <summary>
    /// Контроллер для управления группами пользователей
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        //
        // GET: /RolesAdmin/
        private readonly RoleStore<IdentityRole> _roleStore;
        private readonly RoleManager<IdentityRole> _roleMngr;
        private readonly UserManager<ApplicationUser> _userManager;

        private AppDbContext _db;

        public RolesAdminController():this (new AppDbContext())
        {
            
        }

        public RolesAdminController(AppDbContext appDbContext)
        {
            // TODO: Complete member initialization
            this._db = appDbContext;
            _roleStore= new RoleStore<IdentityRole>(_db);
            _roleMngr = new RoleManager<IdentityRole>(_roleStore); 
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
        }

        
      
        /// <summary>
        /// Список ролей пользователей
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {   

          var roles = await _roleMngr.Roles.ToListAsync();

            return View(roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]

        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
              /*  var roleStore = new RoleStore<IdentityRole>(_db);
                var roleMngr = new RoleManager<IdentityRole>(roleStore); */

                var roleresult = await _roleMngr.CreateAsync(new IdentityRole { Name = roleViewModel.Name });
               
                 
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        public async Task<ActionResult> Details(string id)
        {

            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } */
            var role = await _roleMngr.FindByIdAsync(id);


            var model =new RoleViewModelWithUsers
            {
                Id = id,
                Name = role.Name
            };
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in await _userManager.Users.ToListAsync())
            {
                if (await _userManager.IsInRoleAsync(user.Id, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

           /* ViewBag.Users = users;
            ViewBag.UserCount = users.Count(); */
            return View(model);
        }

        //
        // GET: /Roles/Delete/5

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await _roleMngr.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
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
                var role = await _roleMngr.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }

                foreach (var user in await _userManager.Users.ToListAsync())
            {
                if (await _userManager.IsInRoleAsync(user.Id, role.Name))
                {
                     ModelState.AddModelError("", "Данная роль содержит пользователей");
                    return View("Delete", role);
                }
            }
                
                IdentityResult result = await _roleMngr.DeleteAsync(role);
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


  
