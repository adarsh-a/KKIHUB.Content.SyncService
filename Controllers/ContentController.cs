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
        [Route("Sync")]
        public async Task<IActionResult> SyncContentAsync(int days, string hubId)
        {
            var content = await ContentService.FetchContentAsync(days, hubId);

            return Json(content);
        }
    }
}