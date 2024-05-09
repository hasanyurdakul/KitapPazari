using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using KitapPazariModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KitapPazariWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productViewModel);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile?   formFile)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (formFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);    
                    string productPath = Path.Combine(wwwRootPath, @"images\product\");
                    using (var filestream = new FileStream(Path.Combine(productPath + fileName),FileMode.Create))
                    {
                        formFile.CopyTo(filestream);
                    }
                    productViewModel.Product.ImageURL = @"images\product\"+fileName;
                }

                _unitOfWork.Product.Add(productViewModel.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product Created Succesfully";
                return RedirectToAction("Index");
            }
            else
            {
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productViewModel);

            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }
            Product? retrievedProduct = _unitOfWork.Product.Get(c => c.Id == id);
            if (retrievedProduct == null)
            {
                return NotFound();
            }
            return View(retrievedProduct);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? product = _unitOfWork.Product.Get(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Product.Remove(product);
                _unitOfWork.Save();
                TempData["success"] = "Product Deleted Succesfully";
                return RedirectToAction("Index");
            }
        }
    }
}
