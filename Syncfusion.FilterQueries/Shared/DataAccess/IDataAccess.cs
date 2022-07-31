using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EFGrid.Shared.Models;

namespace EFGrid.Shared.DataAccess
{
    public interface IDataAccess
    {
        Task<List<Order>> GetAllRecords();
    }
}
