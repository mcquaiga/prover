namespace Prover.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyFrequencyTests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FrequencyTests", "ItemsType", c => c.String());
            AddColumn("dbo.FrequencyTests", "PreTestItemData", c => c.String());
            AddColumn("dbo.FrequencyTests", "PostTestItemData", c => c.String());
            DropColumn("dbo.FrequencyTests", "AfterTestData");
            DropColumn("dbo.FrequencyTests", "InstrumentData");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FrequencyTests", "InstrumentData", c => c.String(maxLength: 4000));
            AddColumn("dbo.FrequencyTests", "AfterTestData", c => c.String(maxLength: 4000));
            DropColumn("dbo.FrequencyTests", "PostTestItemData");
            DropColumn("dbo.FrequencyTests", "PreTestItemData");
            DropColumn("dbo.FrequencyTests", "ItemsType");
        }
    }
}
