using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_VotingEngine.Migrations
{
    public partial class quorum_count_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "Auctions");

            migrationBuilder.AddColumn<int>(
                name: "QuorumCount",
                table: "Votings",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuorumCount",
                table: "Votings");

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Auctions",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
