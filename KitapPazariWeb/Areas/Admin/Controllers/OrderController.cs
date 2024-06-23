using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using KitapPazariModels.ViewModels;
using KitapPazariUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Stripe;
using Stripe.Checkout;
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

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Employee + "," + StaticDetails.Role_Admin)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == _orderViewModel.OrderHeader.Id);
            orderHeader.TrackingNumber = _orderViewModel.OrderHeader.TrackingNumber;
            orderHeader.Carrier = _orderViewModel.OrderHeader.Carrier;
            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.OrderStatus = StaticDetails.StatusShipped;
            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new
            {
                orderId = _orderViewModel.OrderHeader.Id
            });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Employee + "," + StaticDetails.Role_Admin)]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == _orderViewModel.OrderHeader.Id);
            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new
            {
                orderId = _orderViewModel.OrderHeader.Id
            });
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult Detail_PAY_NOW()
        {
            _orderViewModel.OrderHeader = _unitOfWork.OrderHeader
                .Get(u => u.Id == _orderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
            _orderViewModel.OrderDetails = _unitOfWork.OrderDetail
                .GetAll(o => o.OrderHeaderId == _orderViewModel.OrderHeader.Id, includeProperties: "Product");

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={_orderViewModel.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={_orderViewModel.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions> { },
                Mode = "payment",
            };

            foreach (var orderDetail in _orderViewModel.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = Convert.ToInt64(orderDetail.Price * 100),
                        Currency = "try",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = orderDetail.Product.Title,
                            Description = orderDetail.Product.Description,
                        }
                    },
                    Quantity = orderDetail.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(_orderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return StatusCode(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                //This is an order coming from a Company account.
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(orderHeaderId);
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
