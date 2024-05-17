using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitapPazariModels.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCardList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
