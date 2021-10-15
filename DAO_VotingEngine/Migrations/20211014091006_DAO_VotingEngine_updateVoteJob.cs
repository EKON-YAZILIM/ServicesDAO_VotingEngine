using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_VotingEngine.Migrations
{
    public partial class DAO_VotingEngine_updateVoteJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "VoteJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "VoteJobs");
        }
    }
}
