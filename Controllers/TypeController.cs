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
    public class TypeController : Controller
    {
        private IContentService ContentService { get; set; }
        public TypeController(IContentService contentService)
        {
            this.ContentService = contentService;
        }


        [HttpGet]
        [Route("Sync")]
        public async Task<IActionResult> SyncContentUpdated(string sourceHub)
        {
            var content = await ContentService.FetchTypeAsync(0, sourceHub, true, false);

            return Json(content);
        }
    }
}