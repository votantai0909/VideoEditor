using System;
using System.Collections.Generic;

namespace VideoEditorProject.Repositories.Entity;

public partial class Video
{
    public int VideoId { get; set; }

    public int? AccountId { get; set; }

    public string FilePath { get; set; } = null!;

    public double Duration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<BackgroundMusic> BackgroundMusics { get; set; } = new List<BackgroundMusic>();

    public virtual ICollection<EditedVideo> EditedVideos { get; set; } = new List<EditedVideo>();

    public virtual ICollection<OverlayContent> OverlayContents { get; set; } = new List<OverlayContent>();

    public virtual ICollection<VideoEffect> VideoEffects { get; set; } = new List<VideoEffect>();
}
