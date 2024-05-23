using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using KitapPazariModels.ViewModels;
using KitapPazariUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace KitapPazariWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCardList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCardList)
            {
                shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }
            return View(ShoppingCartViewModel);
        }

        public IActionResult Plus(int cartId)
        {
            var shoppingCartFromDatabase = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (shoppingCartFromDatabase != null)
            {
                shoppingCartFromDatabase.Count++;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDatabase);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var shoppingCartFromDatabase = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (shoppingCartFromDatabase != null)
            {
                if (shoppingCartFromDatabase.Count <= 1)
                {
                    _unitOfWork.ShoppingCart.Remove(shoppingCartFromDatabase);
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCartFromDatabase.ApplicationUserId).Count() - 1);
                }
                else
                {
                    shoppingCartFromDatabase.Count--;
                    _unitOfWork.ShoppingCart.Update(shoppingCartFromDatabase);

                }
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var shoppingCartFromDatabase = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (shoppingCartFromDatabase != null)
            {
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCartFromDatabase.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDatabase);
                _unitOfWork.Save();

            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCardList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCardList)
            {
                shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }
            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartViewModel.ShoppingCardList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

            //Mapping properties
            ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            //Calculating the total price.
            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCardList)
            {
                shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }

            //Checking the account for possible company relation
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //It's a regular customer account. 
                ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusPending;
                ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            }
            else
            {
                //It's a company account.  
                ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
                ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
            _unitOfWork.Save();

            //Binding orderDetails to orderHeader
            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCardList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = shoppingCart.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = shoppingCart.Price,
                    Count = shoppingCart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            //Getting the payment
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //It's a regular customer account. Need to take payment. 
                //Stripe logic TBA.
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions> { },
                    Mode = "payment",
                };

                foreach (var shoppingCartItem in ShoppingCartViewModel.ShoppingCardList)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = Convert.ToInt64(shoppingCartItem.Price * 100),
                            Currency = "try",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = shoppingCartItem.Product.Title,
                            }
                        },
                        Quantity = shoppingCartItem.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return StatusCode(303);

            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                //This is an order coming from a Customer account.
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }



    }
}
