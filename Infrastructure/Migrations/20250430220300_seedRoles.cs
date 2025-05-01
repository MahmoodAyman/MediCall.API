using Core.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Role.Admin.ToString(), Role.Admin.ToString().ToUpper() },
                    { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Role.Nurse.ToString(), Role.Nurse.ToString().ToUpper() },
                    { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Role.Patient.ToString(), Role.Patient.ToString().ToUpper() }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Name IN ('Admin', 'Nurse', 'Patient')");
        }
    }
}
