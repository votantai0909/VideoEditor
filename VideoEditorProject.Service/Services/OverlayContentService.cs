using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;
using VideoEditorProject.Repositories.Repositories;

namespace VideoEditorProject.Services.Services
{
    public class OverlayContentService
    {
        private readonly OverlayContentRepository _repository;

        public OverlayContentService(OverlayContentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OverlayContent>> GetAllOverlaysAsync()
        {
            return await _repository.GetAllOverlaysAsync();
        }

        public async Task<OverlayContent?> GetOverlayByIdAsync(int id)
        {
            return await _repository.GetOverlayByIdAsync(id);
        }

        public async Task AddOverlayAsync(OverlayContent overlay)
        {
            await _repository.AddOverlayAsync(overlay);
        }

        public async Task UpdateOverlayAsync(OverlayContent overlay)
        {
            await _repository.UpdateOverlayAsync(overlay);
        }

        public async Task DeleteOverlayAsync(int id)
        {
            await _repository.DeleteOverlayAsync(id);
        }
    }
}
