using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using Microsoft.AspNetCore.Mvc;

namespace KitapPazariWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();
                TempData["success"] = "Product Created Succesfully";
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Edit(int? id)
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
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Product Edited Succesfully";
                return RedirectToAction("Index");
            }
            return View();
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
