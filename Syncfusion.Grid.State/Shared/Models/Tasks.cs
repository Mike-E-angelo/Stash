using System;
using System.Collections.Generic;
using System.Text;

namespace EFGrid.Shared.Models
{
    public class Tasks
    {
        public int TaskID
        {
            get;
            set;
        }

        public string TaskName
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public int Progress
        {
            get;
            set;
        }
        public string Priority
        {
            get;
            set;
        }
        public bool Approved
        {
            get;
            set;
        }

        public DateTime FilterStartDate
        {
            get;
            set;
        }
        public DateTime FilterEndDate
        {
            get;
            set;
        }

        public List<Tasks> Children
        {
            get;
            set;
        }

        public int? ParentID
        {
            get;
            set;
        }
        public bool IsParent
        {
            get;
            set;
        }

    }
}
