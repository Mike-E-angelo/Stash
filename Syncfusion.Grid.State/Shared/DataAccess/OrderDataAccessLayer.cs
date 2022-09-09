using EFGrid.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EFGrid.Shared.DataAccess
{
	public class OrderDataAccessLayer
	{
		readonly OrderContext db;

		public OrderDataAccessLayer() : this(new OrderContext()) {}

		public OrderDataAccessLayer(OrderContext db)
		{
			this.db = db;
			this.db.Database.EnsureCreated();
		}

		//To Get all Orders details   
		public DbSet<Order> GetAllOrders() => db.Orders;

		// To Add new Order record
		public void AddOrder(Order Order)
		{
			db.Orders.Add(Order);
			db.SaveChanges();
		}

		//To Update the records of a particluar Order    
		public void UpdateOrder(Order Order)
		{
			db.Entry(Order).State = EntityState.Modified;
			db.SaveChanges();
		}

		//Get the details of a particular Order    
		public Order GetOrderData(int id)
		{
			Order Order = db.Orders.Find(id);
			return Order;
		}

		//To Delete the record of a particular Order    
		public void DeleteOrder(int id)
		{
			Order ord = db.Orders.Find(id);
			db.Orders.Remove(ord);
			db.SaveChanges();
		}
	}
}