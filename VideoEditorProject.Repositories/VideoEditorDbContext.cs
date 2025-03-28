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
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];

        return strConn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6C8AED212");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4CF3DF774").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Account__A9D105343BCCBD11").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<BackgroundMusic>(entity =>
        {
            entity.HasKey(e => e.MusicId).HasName("PK__Backgrou__11F840007B1C0DCD");

            entity.ToTable("BackgroundMusic");

            entity.Property(e => e.FilePath).HasMaxLength(255);

            entity.HasOne(d => d.Video).WithMany(p => p.BackgroundMusics)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Backgroun__Video__32E0915F");
        });

        modelBuilder.Entity<EditedVideo>(entity =>
        {
            entity.HasKey(e => e.EditedVideoId).HasName("PK__EditedVi__533BDD26E13B6357");

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
            entity.HasKey(e => e.OverlayId).HasName("PK__OverlayC__8BDC3702345EC9A7");

            entity.ToTable("OverlayContent");

            entity.Property(e => e.ContentText).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(50);
            entity.Property(e => e.Effect).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(255);

            entity.HasOne(d => d.Video).WithMany(p => p.OverlayContents)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OverlayCo__Video__35BCFE0A");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK__Video__BAE5126A742EBB9D");

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
            entity.HasKey(e => e.EffectId).HasName("PK__VideoEff__6B859F23C266E1F5");

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
