using EFGrid.Shared.DataAccess;
using EFGrid.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TestGridApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        //public int i = 100;
        OrderDataAccessLayer db = new OrderDataAccessLayer();
        // GET: api/Default
        [HttpGet]
        public object Get()
        {

            IQueryable<Order> data = db.GetAllOrders().AsQueryable();
            var count = data.Count();
            var queryString = Request.Query;
            if (queryString.Keys.Contains("$inlinecount"))
            {
                StringValues Skip;
                StringValues Take;
                int skip = (queryString.TryGetValue("$skip", out Skip)) ? Convert.ToInt32(Skip[0]) : 0;
                int top = (queryString.TryGetValue("$top", out Take)) ? Convert.ToInt32(Take[0]) : data.Count();
                return new { Items = data.Skip(skip).Take(top), Count = count };
            }
            else
            {
                return data;
            }
        }

        // GET: api/Default/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        //// POST: api/Default
        [HttpPost]
        public void Post([FromBody]Order Order)
        {
            /// code for Insert operation   
            /// 

            Random rand = new Random();


            db.AddOrder(Order);

        }

        //// PUT: api/Default/5
        [HttpPut]
        public object Put([FromBody]Order Order)
        {
            /// code for Update operation
            db.UpdateOrder(Order);
            return Order;
        }


        //// DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            /// code for delete operation
            db.DeleteOrder(id);

        }

        //    [HttpPost("[action]")]
        //    public object Insert([FromBody]Dataresult order)
        //    {
        //        db.AddOrder(order.Value);
        //        return order.Value;
        //    }

        //    [HttpPost("[action]")]
        //    public object Update([FromBody]Dataresult order)
        //    {
        //        db.UpdateOrder(order.Value);
        //        return order.Value;
        //    }

        //    [HttpPost("[action]")]
        //    public void Remove([FromBody]Dataresult order)
        //    {
        //        db.DeleteOrder(order.key);
        //    }
        //}

        //public class Dataresult : OrdersDetails
        //{
        //    public Order Value { get; set; }

        //    public string action { get; set; }
        //    public int key { get; set; }

    }
}
