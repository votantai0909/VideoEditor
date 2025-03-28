using System;
using System.Collections.Generic;

namespace VideoEditorProject.Repositories.Entity;

public partial class EditedVideo
{
    public int EditedVideoId { get; set; }

    public int? OriginalVideoId { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Video? OriginalVideo { get; set; }
}
