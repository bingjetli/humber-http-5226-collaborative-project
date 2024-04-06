namespace humber_http_5226_collaborative_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_setup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "CafeId", "dbo.Cafes");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ItemId", "dbo.Items");
            DropIndex("dbo.Orders", new[] { "CafeId" });
            DropIndex("dbo.OrderItems", new[] { "ItemId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            AlterColumn("dbo.Orders", "CafeId", c => c.Int());
            AlterColumn("dbo.OrderItems", "ItemId", c => c.Int());
            AlterColumn("dbo.OrderItems", "OrderId", c => c.Int());
            CreateIndex("dbo.Orders", "CafeId");
            CreateIndex("dbo.OrderItems", "ItemId");
            CreateIndex("dbo.OrderItems", "OrderId");
            AddForeignKey("dbo.Orders", "CafeId", "dbo.Cafes", "CafeId");
            AddForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders", "OrderId");
            AddForeignKey("dbo.OrderItems", "ItemId", "dbo.Items", "ItemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "ItemId", "dbo.Items");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CafeId", "dbo.Cafes");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ItemId" });
            DropIndex("dbo.Orders", new[] { "CafeId" });
            AlterColumn("dbo.OrderItems", "OrderId", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderItems", "ItemId", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "CafeId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderItems", "OrderId");
            CreateIndex("dbo.OrderItems", "ItemId");
            CreateIndex("dbo.Orders", "CafeId");
            AddForeignKey("dbo.OrderItems", "ItemId", "dbo.Items", "ItemId", cascadeDelete: true);
            AddForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "CafeId", "dbo.Cafes", "CafeId", cascadeDelete: true);
        }
    }
}
