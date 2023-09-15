using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Core.Models.Domains
{
    public class Error
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
