using System;
using System.Collections.Generic;

namespace VideoEditorProject.Repositories.Entity;

public partial class VideoEffect
{
    public int EffectId { get; set; }

    public int? VideoId { get; set; }

    public string? EffectType { get; set; }

    public double StartTime { get; set; }

    public double EndTime { get; set; }

    public virtual Video? Video { get; set; }
}
