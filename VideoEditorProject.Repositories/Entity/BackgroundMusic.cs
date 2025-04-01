using System;
using System.Collections.Generic;

namespace VideoEditorProject.Repositories.Entity;

public partial class BackgroundMusic
{
    public int MusicId { get; set; }

    public int? VideoId { get; set; }

    public string Name { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public double Duration { get; set; }

    public virtual Video? Video { get; set; }
}
