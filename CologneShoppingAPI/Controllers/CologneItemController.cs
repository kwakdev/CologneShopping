using CaseStudyAPI.DAL;
using CaseStudyAPI.DAL.DAO;
using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CologneItemController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public CologneItemController(AppDbContext context) // injected here
        {
            _ctx = context;
        }
        [HttpGet]
        [Route("{catid}")]
        public async Task<ActionResult<List<CologneItem>>> Index(int catid)
        {
            CologneItemDAO dao = new(_ctx!);
            List<CologneItem> itemsForCategory = await dao.GetAllByCategory(catid);
            return itemsForCategory;
        }
    }
}
