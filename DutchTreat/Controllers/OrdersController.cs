using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository repository;
        private readonly ILogger<OrdersController> logger;
        private readonly IMapper mapper;

        public OrdersController(IDutchRepository repository, 
            ILogger<OrdersController> logger,
            IMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                var results = repository.GetAllOrders(includeItems);
                return Ok(mapper.Map<IEnumerable<Order>,IEnumerable<OrderViewModel>>(results));
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to get orders: ", ex);
                return BadRequest("Failed to get orders");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = repository.GetOrderById(id);
                if (order != null)
                {
                    return Ok(mapper.Map<Order, OrderViewModel>(order));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to get orders: ", ex);
                return BadRequest("Failed to get orders");
            }
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] OrderViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    //var newOrder = new Order
                    //{
                    //    OrderDate = model.OrderDate,
                    //    OrderNumber = model.OrderNumber,
                    //    Id = model.OrderId
                    //};

                    var newOrder = mapper.Map<OrderViewModel, Order>(model);

                    repository.AddEntity(newOrder);
                    if(repository.SaveAll())
                    {
                        return Created($"/api/orders/{newOrder.Id}", mapper.Map<Order, OrderViewModel>(newOrder));
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }    
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to save a new order: ", ex);
            }
            return BadRequest("Failed to save a new order");
        }


    }
}
