﻿using FY111.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using FY111.Models.FY111User;
using System;
using System.Linq;

namespace FY111.Controllers
{
    public class UserManageController : Controller
    {
        private UserManager<FY111User> _userManager;
        

        public UserManageController(UserManager<FY111User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["RoleParm"] = String.IsNullOrEmpty(sortOrder) ? "role_desc" : "";
            ViewData["OrganizationParm"] = sortOrder == "organization" ? "organization_desc" : "organization";
            var Users = await _userManager.Users.ToListAsync();
            List<ManageModel> manageModel = new List<ManageModel>();
            for (int i = 0; i < Users.Count; i++)
            {
                ManageModel model = new ManageModel();
                model.UserName = Users[i].UserName;
                model.Id = Users[i].Id;
                model.Email = Users[i].Email;
                model.Avatar = Users[i].Avatar;
                model.Organization = Users[i].Organization;
                model.Role = (await _userManager.GetRolesAsync(Users[i]))[0];
                manageModel.Add(model);
            }
            switch (sortOrder)
            {
                case "role_desc":
                    manageModel = manageModel.OrderByDescending(x => x.Role).ToList();
                    break;
                case "organization":
                    manageModel = manageModel.OrderBy(x => x.Organization).ToList();
                    break;
                case "organization_desc":
                    manageModel = manageModel.OrderByDescending(x => x.Organization).ToList();
                    break;
                default:
                    manageModel = manageModel.OrderBy(x => x.Role).ToList();
                    break;
            }

            return View(manageModel);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ManageModel model = new ManageModel();
            model.UserName = user.UserName;
            model.Id = user.Id;
            model.Email = user.Email;
            model.Avatar = user.Avatar;
            model.Organization = user.Organization;
            model.Role = (await _userManager.GetRolesAsync(user))[0];
            return View(model);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ManageModel model = new ManageModel();
            model.UserName = user.UserName;
            model.Id = user.Id;
            model.Email = user.Email;
            model.Avatar = user.Avatar;
            model.Organization = user.Organization;
            model.Role = (await _userManager.GetRolesAsync(user))[0];
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ManageModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                FY111User user = await _userManager.FindByIdAsync(model.Id);
                string prevRole = (await _userManager.GetRolesAsync(user))[0];
                if(prevRole != model.Role)
                {
                    await _userManager.RemoveFromRoleAsync(user, prevRole);
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                user.Organization = model.Organization;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ManageModel model = new ManageModel();
            model.UserName = user.UserName;
            model.Id = user.Id;
            model.Email = user.Email;
            model.Avatar = user.Avatar;
            model.Organization = user.Organization;
            model.Role = (await _userManager.GetRolesAsync(user))[0];
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}