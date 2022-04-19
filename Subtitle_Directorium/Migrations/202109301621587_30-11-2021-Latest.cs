namespace Subtitle_Directorium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _30112021Latest : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subtitles", "raw", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subtitles", "raw", c => c.String(nullable: false));
        }
    }
}
