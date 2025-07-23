using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAuctionHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_ImageUrl_And_IsAdultOnly_And_CreatedDate_To_Artwork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "Artworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Artworks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Artworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdultOnly",
                table: "Artworks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "IsAdultOnly",
                table: "Artworks");
        }
    }
}
