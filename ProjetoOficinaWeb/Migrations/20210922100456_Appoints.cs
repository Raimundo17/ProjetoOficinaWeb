﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjetoOficinaWeb.Migrations
{
    public partial class Appoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetails_Services_PriceId",
                table: "AppointmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetailsTemp_Services_PriceId",
                table: "AppointmentDetailsTemp");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDetailsTemp_PriceId",
                table: "AppointmentDetailsTemp");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDetails_PriceId",
                table: "AppointmentDetails");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "AppointmentDetailsTemp");

            migrationBuilder.DropColumn(
                name: "PriceId",
                table: "AppointmentDetails");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "AppointmentDetailsTemp",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "AppointmentDetailsTemp",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "AppointmentDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "AppointmentDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "AppointmentDetailsTemp");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AppointmentDetailsTemp");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "AppointmentDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AppointmentDetails");

            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                table: "AppointmentDetailsTemp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                table: "AppointmentDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetailsTemp_PriceId",
                table: "AppointmentDetailsTemp",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetails_PriceId",
                table: "AppointmentDetails",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetails_Services_PriceId",
                table: "AppointmentDetails",
                column: "PriceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetailsTemp_Services_PriceId",
                table: "AppointmentDetailsTemp",
                column: "PriceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}