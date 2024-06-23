using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using KitapPazariModels.ViewModels;
using KitapPazariUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KitapPazariWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                Company companyObject = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObject);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company companyObject)
        {
            if (ModelState.IsValid)
            {
                if (companyObject.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObject);
                }
                else
                {
                    _unitOfWork.Company.Update(companyObject);
                }
                _unitOfWork.Save();
                TempData["success"] = "Company Created Succesfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(companyObject);
            }
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company companyTobeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyTobeDeleted == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _unitOfWork.Company.Remove(companyTobeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
