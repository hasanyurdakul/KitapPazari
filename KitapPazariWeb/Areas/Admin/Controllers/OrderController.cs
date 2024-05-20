using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using KitapPazariModels.ViewModels;
using KitapPazariUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Security.Claims;

namespace KitapPazariWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderViewModel _orderViewModel { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            _orderViewModel = new OrderViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(o => o.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(_orderViewModel);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Employee + "," + StaticDetails.Role_Admin)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDatabase = _unitOfWork.OrderHeader.Get(u => u.Id == _orderViewModel.OrderHeader.Id); orderHeaderFromDatabase.Name = _orderViewModel.OrderHeader.Name;
            orderHeaderFromDatabase.PhoneNumber = _orderViewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDatabase.StreetAddress = _orderViewModel.OrderHeader.StreetAddress;
            orderHeaderFromDatabase.City = _orderViewModel.OrderHeader.City;
            orderHeaderFromDatabase.State = _orderViewModel.OrderHeader.State;
            orderHeaderFromDatabase.PostalCode = _orderViewModel.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(_orderViewModel.OrderHeader.Carrier))
            {
                orderHeaderFromDatabase.Carrier = _orderViewModel.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(_orderViewModel.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDatabase.TrackingNumber = _orderViewModel.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDatabase);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new
            {
                orderId = orderHeaderFromDatabase.Id
            });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Employee + "," + StaticDetails.Role_Admin)]
        public IActionResult StartProcessing()
        {
           _unitOfWork.OrderHeader.UpdateStatus(_orderViewModel.OrderHeader.Id, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Detail Updated Successfully";
            return RedirectToAction(nameof(Details), new
            {
                orderId = _orderViewModel.OrderHeader.Id
            });
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaderList;

            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
            }



            switch (status)
            {
                case "pending":
                    objOrderHeaderList = objOrderHeaderList.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaderList = objOrderHeaderList.Where(o => o.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaderList = objOrderHeaderList.Where(o => o.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaderList = objOrderHeaderList.Where(o => o.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;
            }








            return Json(new { data = objOrderHeaderList });
        }
        #endregion

    }
}
