using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DocSasReporting.Models;
using DocSasReporting.Concrete;

namespace DocSasReporting.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdvanceReportProvider _advanceReportProvider;
        public HomeController(IAdvanceReportProvider advanceReportProvider)
        {
            _advanceReportProvider = advanceReportProvider;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
