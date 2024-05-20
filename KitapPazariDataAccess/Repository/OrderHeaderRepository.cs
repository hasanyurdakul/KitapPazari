using KitapPazariDataAccess.Data;
using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KitapPazariDataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }



        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDatabase = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDatabase != null)
            {
                orderFromDatabase.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDatabase.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDatabase = _context.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDatabase.SessionId=sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDatabase.PaymentIntentId = paymentIntentId;
                orderFromDatabase.PaymentDate = DateTime.Now;
            }
        }
    }
}
