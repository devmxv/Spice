using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; } 

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //---Get Index
        public async Task<IActionResult> Index()
        {
            //---MXV: Working with FK to include the Subcategory info
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);

        }

        //---Get - Create
        public async Task<IActionResult> Create()
        {
            //---create object of subcatAndCatViewModel
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                //---get all cats
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                //---List of all cats names
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //---Post - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                //---check if subcat exists within cat
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if(doesSubCategoryExists.Count() > 0)
                {
                    //---Error
                    StatusMessage = "Error : Sub Category exists under " + 
                        doesSubCategoryExists.First().Category.Name + " category. Try again!";

                } else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                //---setting props
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.
                    OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        //---Get - Details
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            //---Subcat
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if(subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                //---List of all cats names
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }



        //---Get - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            //---Retrieve subcat from DB and add it to var subcat
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if(subCategory == null)
            {
                return NotFound();
            }

            //---create object of subcatAndCatViewModel
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                //---get all cats
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                //---List of all cats names
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //---Post - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                //---check if subcat exists within cat
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count() > 0)
                {
                    //---Error
                    StatusMessage = "Error : Sub Category exists under " +
                        doesSubCategoryExists.First().Category.Name + " category. Try again!";

                }
                else
                {
                    //---Uses id to find the subCat
                    var subCatFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    //---get the name and updates
                    //---use it to change certain properties
                    subCatFromDb.Name = model.SubCategory.Name;

                    //_db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                //---setting props
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.
                    OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            //modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        //---Returns all the subcats in a list as an API
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            //---Returns a select list of categories
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.FindAsync(id);
            if(subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }

        //---Post - Delete 

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var subCategory = await _db.SubCategory.FindAsync(id);
            if(subCategory == null)
            {
                return NotFound();
            }

            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
