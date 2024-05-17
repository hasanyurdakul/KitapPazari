using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using KitapPazariModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KitapPazariWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
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
