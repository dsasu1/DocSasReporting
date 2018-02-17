using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSasReporting.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocSasReporting.Models;
using DocSasReporting.managers;

namespace DocSasReporting.Controllers
{
    [Produces("application/json")]
    [Route("api/report")]
    public class ReportAPIController : Controller
    {
        private readonly IAdvanceReportProvider _advanceReportProvider;
        public ReportAPIController(IAdvanceReportProvider advanceReportProvider)
        {
            _advanceReportProvider = advanceReportProvider;
        }

        [HttpGet]
        [Route("LoadTables")]
        public async Task<IActionResult> GetTables()
        {
            var result = await _advanceReportProvider.LoadReportTablesAsync();
            return new JsonResult(result);
        }


        [HttpGet]
        [Route("LoadFields")]
        public async Task<IActionResult> GetFields(int dataId)
        {
            var result = await _advanceReportProvider.GetTableSchemaAsync(dataId);
            return new JsonResult(result);
        }

        [HttpDelete]
        [Route("DeleteReport")]
        public async Task<IActionResult> DeleteSavedReport(int id)
        {
             await _advanceReportProvider.DeleteReportAsync(id);
            return new JsonResult(true);
        }

        [HttpGet]
        [Route("LoadSavedReports")]
        public async Task<IActionResult> GetSavedReports()
        {
            var result = await _advanceReportProvider.LoadSavedReportsAsync();
            return new JsonResult(result);
        }


        [HttpPost]
        [Route("RunReport")]
        public  async Task<IActionResult> PostRunReport([FromBody] SavedReport savedReport)
        {
            var result =  await _advanceReportProvider.RunReportAsync(savedReport);
            return Ok(result.ToJson());
        }

        [HttpPost]
        [Route("SaveReport")]
        public async Task<IActionResult> PostSaveReport([FromBody] SavedReport savedReport)
        {
             await _advanceReportProvider.SaveReportAsync(savedReport);
            return new JsonResult(true);
        }


    }
}