using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KKIHUB.Content.SyncService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KKIHUB.Content.SyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : Controller
    {
        private IContentService ContentService { get; set; }
        public ContentController(IContentService contentService)
        {
            this.ContentService = contentService;
        }


        [HttpGet]
        [Route("SyncComplete")]
        public async Task<IActionResult> SyncContentAsync(int days, string sourceHub,string targetHub)
        {
            var content = await ContentService.FetchContentAsync(days, sourceHub, false, false);

            return Json(content);
        }


        [HttpGet]
        [Route("SyncRecursive")]
        public async Task<IActionResult> SyncContentRecursive(int days, string sourceHub, string targetHub)
        {
            var content = await ContentService.FetchContentAsync(days, sourceHub, true, false);

            return Json(content);
        }

        [HttpGet]
        [Route("SyncUpdated")]
        public async Task<IActionResult> SyncContentUpdated(int days, string sourceHub, string targetHub)
        {
            var content = await ContentService.FetchContentAsync(days, sourceHub, true, true);

            return Json(content);
        }
    }
}