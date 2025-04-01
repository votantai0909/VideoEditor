using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;

namespace VideoEditorProject.Repositories.Repositories
{
    public class BackgroundMusicRepository
    {
        private readonly VideoEditorDbContext _context = new();

        // Lấy danh sách tất cả nhạc
        public async Task<List<BackgroundMusic>> GetAllMusicAsync()
        {
            return await Task.Run(() => _context.BackgroundMusics.ToList());
        }

        // Lấy nhạc theo ID
        public async Task<BackgroundMusic?> GetMusicByIdAsync(int musicId)
        {
            return await Task.Run(() => _context.BackgroundMusics.FirstOrDefault(m => m.MusicId == musicId));
        }

        // Thêm nhạc mới
        public async Task AddMusicAsync(BackgroundMusic music)
        {
            await Task.Run(() =>
            {
                _context.BackgroundMusics.Add(music);
                _context.SaveChanges();
            });
        }

        // Xóa nhạc theo ID
        public async Task DeleteMusicAsync(int musicId)
        {
            await Task.Run(() =>
            {
                var music = _context.BackgroundMusics.FirstOrDefault(m => m.MusicId == musicId);
                if (music != null)
                {
                    _context.BackgroundMusics.Remove(music);
                    _context.SaveChanges();
                }
            });
        }

        public async Task<string?> GetMusicFilePathByIdAsync(int musicId)
        {
            return await Task.Run(() =>
            {
                var music = _context.BackgroundMusics.FirstOrDefault(m => m.MusicId == musicId);
                return music?.FilePath; // Trả về FilePath nếu tìm thấy, nếu không trả về null
            });
        }

    }
}
