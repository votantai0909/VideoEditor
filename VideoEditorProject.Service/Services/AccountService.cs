using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoEditorProject.Repositories.Entity;
using VideoEditorProject.Repositories.Repositories;

namespace VideoEditorProject.Services.Services
{
    public class AccountService
    {
        private AccountRepository _repo = new();

        public Account? Authentication(string email, string password)
        {
            return _repo.GetOne(email, password);
        }
    }
}
