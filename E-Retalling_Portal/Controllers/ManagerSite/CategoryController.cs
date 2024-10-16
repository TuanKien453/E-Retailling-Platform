using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using E_Retalling_Portal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace E_Retalling_Portal.Controllers.Manager
{
    public class CategoryController : Controller
    {

        public IActionResult Index(int? page)
        {
            List<Category> categories;
            using (var context = new Context())
            {
                categories = context.Categories.GetCategories().ToList();
            }
            List<Category> builedCategoies = BuildCategoryTree(categories);

            //paging
            var pageNumber = page ?? 1;
            var pageSize = 20;
            var pagedCategories = builedCategoies.ToPagedList(pageNumber, pageSize);

            ViewBag.categoies = categories;
            return View("/Views/ManagerSite/Category/ViewCategory.cshtml", pagedCategories);
        }
        [HttpPost]
        public IActionResult Add(Category cate)
        {
            Console.WriteLine(cate.parentCategoryId);
            if (ModelState.IsValid)
            {
                if (cate.parentCategoryId == 0)
                {
                    cate.parentCategoryId = null;
                }
                using (var context = new Context())
                {
                    context.Categories.Add(cate);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCategory(Category cate)
        {   
            if(cate.parentCategoryId == 0)
            {
                cate.parentCategoryId = null;
            }
            using (var context = new Context())
            {
                context.Categories.Update(cate);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteCategory(int id)
        {
            using (var context = new Context())
            {
                context.Categories.DeleteCategoryWithChildren(id,context);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //create and order by level 
        private List<Category> BuildCategoryTree(List<Category> list, int level = 0, int? parentid = null)
        {
            List<Category> result = new();
            foreach (var category in list.Where(c => c.parentCategoryId == parentid))
            {
                string newName = new string('-', level) + category.name;
                result.Add(new Category { id = category.id, name = newName, parentCategoryId = category.parentCategoryId });

                var children = BuildCategoryTree(list, level + 1, category.id);
                result.AddRange(children);
            }
            return result;
        }
    }
}
