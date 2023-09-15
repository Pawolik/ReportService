using ReportService.Core.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Core.Repositories
{
    public class ReportRepository
    {
        public Report GetLastNotSentReport()
        {
            //pobieranie z bazy danych ostatniego raportu


            return new Report
            {
                ID = 1,
                Title = "R/1/2023",
                Date = new DateTime(2023, 1, 1, 12, 0, 0),
                Positions = new List<ReportPosition>
                {
                    new ReportPosition
                    {
                        ID = 1,
                        ReportID = 1,
                        Title = "Position 1",
                        Description = "Description 1",
                        Value = 43.01M,
                    },
                    new ReportPosition
                    {
                        ID = 2,
                        ReportID = 1,
                        Title = "Position 2",
                        Description = "Description 2",
                        Value = 4311M,
                    },
                    new ReportPosition
                    {
                        ID = 3,
                        ReportID = 1,
                        Title = "Position 3",
                        Description = "Description 3",
                        Value = 1.99M,
                    }
                }
            };
        }

        public void ReportSent (Report report)
        {
            report.IsSend = true;
            //zapis w bazie danych
        }
    }
}
