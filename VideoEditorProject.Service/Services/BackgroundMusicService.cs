using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;
using VideoEditorProject.Repositories.Repositories;

namespace VideoEditorProject.Services.Services
{
    public class BackgroundMusicService
    {
        private BackgroundMusicRepository _repo = new();

        // Lấy danh sách nhạc
        public async Task<List<BackgroundMusic>> GetAllMusicAsync()
        {
            return await _repo.GetAllMusicAsync();
        }

        // Lấy nhạc theo ID
        public async Task<BackgroundMusic?> GetMusicByIdAsync(int musicId)
        {
            return await _repo.GetMusicByIdAsync(musicId);
        }

        // Thêm nhạc mới
        public async Task AddMusicAsync(BackgroundMusic music)
        {
            await _repo.AddMusicAsync(music);
        }

        // Xóa nhạc
        public async Task DeleteMusicAsync(int musicId)
        {
            await _repo.DeleteMusicAsync(musicId);
        }

        public async Task<string?> GetMusicFilePathByIdAsync(int musicId)
        {
            return await _repo.GetMusicFilePathByIdAsync(musicId);
        }

    }
}
