using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext ctx;

        public DutchRepository(DutchContext ctx)
        {
            this.ctx = ctx;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return ctx.Products
                    .OrderBy(p => p.Title)
                    .ToList();
        }
        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return ctx.Products
                    .Where(p => p.Category == category)
                    .ToList();
        }

        //including to URL: ?includeItems=false
        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if(includeItems)
            {
                return ctx.Orders
                    .Include(o => o.Items)
                    .ThenInclude(o => o.Product)
                    .ToList();
            }
            else
            {
                return ctx.Orders
                    .ToList();
            }
        }

        public bool SaveAll() 
        {
            return ctx.SaveChanges() > 0;
        }

        public Order GetOrderById(int id)
        {
            return ctx.Orders
                .Include(o => o.Items)
                .ThenInclude(o => o.Product)
                .Where(o => o.Id == id)
                .FirstOrDefault();
        }

        public void AddEntity(object model)
        {
            ctx.Add(model);
        }
    }
}
