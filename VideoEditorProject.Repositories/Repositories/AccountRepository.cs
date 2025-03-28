using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;
namespace VideoEditorProject.Repositories.Repositories
{
    public class AccountRepository
    {
        private VideoEditorDbContext _context;

        public Account? GetOne(string email, string password)
        {
            _context = new VideoEditorDbContext();
            return _context.Accounts.FirstOrDefault(x => x.Email == email && x.Password == password);
        }
    }
}
