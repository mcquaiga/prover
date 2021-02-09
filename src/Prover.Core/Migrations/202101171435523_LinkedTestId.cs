namespace Prover.Core.Migrations {
	using System.Data.Entity.Migrations;

	public partial class LinkedTestId : DbMigration {
		public override void Up() {

			AddColumn("dbo.Instruments", "LinkedTestId", col => col.Guid());
		}

		public override void Down() {
		}
	}
}
