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
    public class AssetController : Controller
    {
        private IContentService ContentService { get; set; }
        public AssetController(IContentService contentService)
        {
            this.ContentService = contentService;
        }


        [HttpGet]
        [Route("SyncUpdated")]
        public async Task<IActionResult> SyncContentUpdated(int days, string sourceHub, string targetHub)
        {
            var content = await ContentService.FetchAssetAsync(days, sourceHub, true, false);

            return Json(content);
        }
    }
}