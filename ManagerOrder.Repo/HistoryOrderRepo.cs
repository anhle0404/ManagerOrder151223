using ManagerOrder.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerOrder.Repo
{
    public class HistoryOrderRepo:GenericRepo<HistoryOrder>
    {
        HistoryOrderDetailRepo detailRepo = new HistoryOrderDetailRepo();
        public object GetDataReport(DateTime dateStart, DateTime dateEnd)
        {
            var listDate = Enumerable.Range(0, 1 + dateEnd.Subtract(dateStart).Days)
                                                .Select(offset => dateStart.AddDays(offset))
                                                .ToList();

            List<double> dataImports = new List<double>();
            List<double> dataRevenues = new List<double>();
            List<double> dataInterests = new List<double>();

            foreach (var item in listDate)
            {

                var dataReports = (from o in GetAll()
                                   join d in detailRepo.GetAll() on o.Id equals d.HistoryOrderId into od
                                   from d in od.DefaultIfEmpty()
                                   where Convert.ToDateTime(o.CreatedDate).Year == item.Year &&
                                         Convert.ToDateTime(o.CreatedDate).Month == item.Month &&
                                         Convert.ToDateTime(o.CreatedDate).Day == item.Day && o.IsApproved == 1
                                   group new { o, d } by d.HistoryOrderId into g
                                   select new
                                   {
                                       TotalIntoMoney = g.First().o.TotalIntoMoney,
                                       IntoMoney = g.Sum(x => x.d.IntoMoney),
                                       Interest = g.First().o.TotalIntoMoney - g.Sum(x => x.d.IntoMoney),
                                   }).ToList();

                dataImports.Add((double)dataReports.Sum(x => x.TotalIntoMoney));
                dataRevenues.Add((double)dataReports.Sum(x => x.IntoMoney));
                dataInterests.Add((double)dataReports.Sum(x => x.Interest));

            }
            string[] categories = Enumerable.Range(0, 1 + dateEnd.Subtract(dateStart).Days)
                                            .Select(offset => dateStart.AddDays(offset).ToString("dd/MM/yyyy"))
                                            .ToArray();


            var seriesImport = new
            {
                name = "Tiền nhập",
                data = dataImports.ToArray()
            };

            var seriesRevenue = new
            {
                name = "Doanh thu",
                data = dataRevenues.ToArray()
            };

            var seriesInterest = new
            {
                name = "Tiền lãi",
                data = dataInterests.ToArray()
            };

            var reports = new
            {
                data = new List<object>() { seriesImport, seriesRevenue, seriesInterest },
                categories = categories,
            };

            return reports;
        }
    }
}
