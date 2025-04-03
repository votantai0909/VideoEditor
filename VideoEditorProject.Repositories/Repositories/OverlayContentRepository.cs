using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;

namespace VideoEditorProject.Repositories.Repositories
{
    public class OverlayContentRepository
    {
        private readonly VideoEditorDbContext _context;

        public OverlayContentRepository(VideoEditorDbContext context)
        {
            _context = context;
        }

        public async Task<List<OverlayContent>> GetAllOverlaysAsync()
        {
            return await _context.OverlayContents.ToListAsync();
        }

        public async Task<OverlayContent?> GetOverlayByIdAsync(int id)
        {
            return await _context.OverlayContents.FindAsync(id);
        }

        public async Task AddOverlayAsync(OverlayContent overlay)
        {
            await _context.OverlayContents.AddAsync(overlay);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOverlayAsync(OverlayContent overlay)
        {
            _context.OverlayContents.Update(overlay);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOverlayAsync(int id)
        {
            var overlay = await _context.OverlayContents.FindAsync(id);
            if (overlay != null)
            {
                _context.OverlayContents.Remove(overlay);
                await _context.SaveChangesAsync();
            }
        }
    }
}
