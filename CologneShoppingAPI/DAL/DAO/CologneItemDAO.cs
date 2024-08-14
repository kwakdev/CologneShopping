using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace CaseStudyAPI.DAL.DAO
{
    public class CologneItemDAO
    {

        private readonly AppDbContext _db;
        public CologneItemDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<CologneItem>> GetAllByCategory(int id)
        {
            return await _db.CologneItems!.Where(item => item.Category!.Id == id).ToListAsync();
        }
    }
}
