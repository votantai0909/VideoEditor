using System;
using System.Collections.Generic;

namespace VideoEditorProject.Repositories.Entity;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
