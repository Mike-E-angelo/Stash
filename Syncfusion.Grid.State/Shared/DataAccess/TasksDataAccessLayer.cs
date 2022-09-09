using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFGrid.Shared.Models;

namespace EFGrid.Server.DataAccess
{
    public class TasksDataAccessLayer
    {
        TasksContext treedb = new TasksContext();

        //To Get all Task details   
        public IEnumerable<Tasks> GetAllRecords()
        {
            try
            {
                return treedb.Task.ToList();
            }
            catch
            {
                throw;
            }
        }

        //To Add new Tasks record     
        public void AddTask(Tasks task)
        {
            try
            {
                treedb.Task.Add(task);
                treedb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //To Update the records of a particluar Tasks    
        public void UpdateTask(Tasks task)
        {
            try
            {
                treedb.Entry(task).State = EntityState.Modified;
                treedb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        //Get the details of a particular Tasks    
        public Tasks GetTaskData(int id)
        {
            try
            {
                Tasks task = treedb.Task.Find(id);
                return task;
            }
            catch
            {
                throw;
            }
        }

        //To Delete the record of a particular Tasks    
        public void DeleteTask(int id)
        {
            try
            {
                Tasks emp = treedb.Task.Find(id);
                treedb.Task.Remove(emp);
                treedb.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
