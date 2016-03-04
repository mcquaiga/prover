namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pressuretests : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PressureTests", new[] { "Pressure_Id" });
            RenameColumn(table: "dbo.PressureTests", name: "Pressure_Id", newName: "PressureId");
            AddColumn("dbo.PressureTests", "IsVolumeTestPressure", c => c.Boolean(nullable: false));
            AddColumn("dbo.PressureTests", "GasGauge", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.PressureTests", "AtmosphericGauge", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.TemperatureTests", "Gauge", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Volumes", "AppliedInput", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PressureTests", "PressureId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PressureTests", "PressureId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PressureTests", new[] { "PressureId" });
            AlterColumn("dbo.PressureTests", "PressureId", c => c.Guid());
            AlterColumn("dbo.Volumes", "AppliedInput", c => c.Double(nullable: false));
            AlterColumn("dbo.TemperatureTests", "Gauge", c => c.Double(nullable: false));
            DropColumn("dbo.PressureTests", "AtmosphericGauge");
            DropColumn("dbo.PressureTests", "GasGauge");
            DropColumn("dbo.PressureTests", "IsVolumeTestPressure");
            RenameColumn(table: "dbo.PressureTests", name: "PressureId", newName: "Pressure_Id");
            CreateIndex("dbo.PressureTests", "Pressure_Id");
        }
    }
}
