using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Core.Models.Domains
{
    public class Report
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public bool IsSend { get; set; }
        public List<ReportPosition> Positions { get; set; }
    }
}
