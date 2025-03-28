using System;
using System.Collections.Generic;

namespace VideoEditorProject.Repositories.Entity;

public partial class OverlayContent
{
    public int OverlayId { get; set; }

    public int? VideoId { get; set; }

    public string? ContentType { get; set; }

    public string? ContentText { get; set; }

    public string? FilePath { get; set; }

    public double? PositionX { get; set; }

    public double? PositionY { get; set; }

    public double? SizeWidth { get; set; }

    public double? SizeHeight { get; set; }

    public string? Effect { get; set; }

    public virtual Video? Video { get; set; }
}
