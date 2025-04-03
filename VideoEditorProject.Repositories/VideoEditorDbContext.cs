using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VideoEditorProject.Repositories.Entity;

namespace VideoEditorProject.Repositories;

public partial class VideoEditorDbContext : DbContext
{
    public VideoEditorDbContext()
    {
    }

    public VideoEditorDbContext(DbContextOptions<VideoEditorDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<BackgroundMusic> BackgroundMusics { get; set; }

    public virtual DbSet<EditedVideo> EditedVideos { get; set; }

    public virtual DbSet<OverlayContent> OverlayContents { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<VideoEffect> VideoEffects { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true).Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];
        return strConn;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A674FD2926");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4E7009660").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Account__A9D10534865E0E7D").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<BackgroundMusic>(entity =>
        {
            entity.HasKey(e => e.MusicId).HasName("PK__Backgrou__11F840003D0A6140");

            entity.ToTable("BackgroundMusic");

            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Video).WithMany(p => p.BackgroundMusics)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Backgroun__Video__33D4B598");
        });

        modelBuilder.Entity<EditedVideo>(entity =>
        {
            entity.HasKey(e => e.EditedVideoId).HasName("PK__EditedVi__533BDD26E55F46B1");

            entity.ToTable("EditedVideo");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(255);

            entity.HasOne(d => d.OriginalVideo).WithMany(p => p.EditedVideos)
                .HasForeignKey(d => d.OriginalVideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__EditedVid__Origi__300424B4");
        });

        modelBuilder.Entity<OverlayContent>(entity =>
        {
            entity.HasKey(e => e.OverlayId).HasName("PK__OverlayC__8BDC3702B48F7A97");

            entity.ToTable("OverlayContent");

            entity.Property(e => e.ContentText).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(50);
            entity.Property(e => e.Effect).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(255);

            entity.HasOne(d => d.Video).WithMany(p => p.OverlayContents)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OverlayCo__Video__36B12243");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK__Video__BAE5126ACF44A312");

            entity.ToTable("Video");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(255);

            entity.HasOne(d => d.Account).WithMany(p => p.Videos)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Video__AccountId__29572725");
        });

        modelBuilder.Entity<VideoEffect>(entity =>
        {
            entity.HasKey(e => e.EffectId).HasName("PK__VideoEff__6B859F235F3EF195");

            entity.ToTable("VideoEffect");

            entity.Property(e => e.EffectType).HasMaxLength(50);

            entity.HasOne(d => d.Video).WithMany(p => p.VideoEffects)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__VideoEffe__Video__2C3393D0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
