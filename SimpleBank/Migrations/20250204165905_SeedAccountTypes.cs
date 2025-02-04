using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBank.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccountTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "TypeName", "MaxWithdrawalPerDay", "MinAccountBalance" },
                values: new object[,]
                {
            { "Savings", 1000.00m, 100.00m },
            { "Checking", 2000.00m, 0.00m },
            { "Business", 5000.00m, 1000.00m },
            { "Investment", 10000.00m, 5000.00m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccountTypes",
                keyColumn: "TypeName",
                keyValues: new object[] { "Savings", "Checking", "Business", "Investment" });
        }
      

    
    }
}
