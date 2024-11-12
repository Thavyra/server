using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    display_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scopes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    display_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scopes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    balance = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    display_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    scope_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_permissions_scopes_scope_id",
                        column: x => x.scope_id,
                        principalTable: "scopes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    client_secret_hash = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_applications_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    provider_account_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    provider_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    provider_avatar_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logins", x => x.id);
                    table.ForeignKey(
                        name: "FK_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_permissions",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_permissions", x => new { x.application_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_application_permissions_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "authorizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subject = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_authorizations_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "objectives",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    display_name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objectives", x => x.id);
                    table.ForeignKey(
                        name: "FK_objectives_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "redirects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uri = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_redirects", x => x.id);
                    table.ForeignKey(
                        name: "FK_redirects_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "system",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system", x => x.id);
                    table.ForeignKey(
                        name: "FK_system_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transactions_users_subject_id",
                        column: x => x.subject_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_id = table.Column<Guid>(type: "uuid", nullable: false),
                    succeeded = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_attempts", x => x.id);
                    table.ForeignKey(
                        name: "FK_login_attempts_logins_login_id",
                        column: x => x.login_id,
                        principalTable: "logins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "authorization_scopes",
                columns: table => new
                {
                    authorization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authorization_scopes", x => new { x.authorization_id, x.scope_id });
                    table.ForeignKey(
                        name: "FK_authorization_scopes_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "authorizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_authorization_scopes_scopes_scope_id",
                        column: x => x.scope_id,
                        principalTable: "scopes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    authorization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subject = table.Column<Guid>(type: "uuid", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    payload = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    redeemed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "applications",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tokens_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "authorizations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "scores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    objective_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scores", x => x.id);
                    table.ForeignKey(
                        name: "FK_scores_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scores_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "display_name", "name", "scope_id" },
                values: new object[,]
                {
                    { new Guid("08b21900-d3a2-d85e-42d1-08dce1858470"), "Authorization", "ept:authorization", null },
                    { new Guid("08b21a00-d3a2-d85e-42d1-08dce1858470"), "Token", "ept:token", null },
                    { new Guid("08b21b00-d3a2-d85e-42d1-08dce1858470"), "Logout", "ept:logout", null },
                    { new Guid("08b21c00-d3a2-d85e-42d1-08dce1858470"), "Implicit", "cst:implicit", null },
                    { new Guid("08b21d00-d3a2-d85e-42d1-08dce1858470"), "Code", "rst:code", null },
                    { new Guid("08b21e00-d3a2-d85e-42d1-08dce1858470"), "Id Token", "rst:id_token", null },
                    { new Guid("08b21f00-d3a2-d85e-42d1-08dce1858470"), "Authorization Code", "gt:authorization_code", null },
                    { new Guid("08b22000-d3a2-d85e-42d1-08dce1858470"), "Client Credentials", "gt:client_credentials", null },
                    { new Guid("08b22100-d3a2-d85e-42d1-08dce1858470"), "Refresh Token", "gt:refresh_token", null },
                    { new Guid("08b22200-d3a2-d85e-42d1-08dce1858470"), "Implicit", "gt:implicit", null },
                    { new Guid("08b22400-d3a2-d85e-42d1-08dce1858470"), "None", "rst:none", null }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "display_name", "name" },
                values: new object[] { new Guid("08b22300-d3a2-d85e-42d1-08dce1858470"), "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "scopes",
                columns: new[] { "id", "description", "display_name", "name" },
                values: new object[,]
                {
                    { new Guid("08b20000-d3a2-d85e-42d1-08dce1858470"), "Perform privileged operations.", "Sudo", "sudo" },
                    { new Guid("08b20100-d3a2-d85e-42d1-08dce1858470"), "Perform administrator operations.", "Admin", "admin" },
                    { new Guid("08b20200-d3a2-d85e-42d1-08dce1858470"), "View, manage and delete your profile, logins and account.", "Full Account Access", "account" },
                    { new Guid("08b20300-d3a2-d85e-42d1-08dce1858470"), "View and modify your profile and avatar.", "Profile", "account.profile" },
                    { new Guid("08b20400-d3a2-d85e-42d1-08dce1858470"), "View your profile and avatar.", "View Profile", "account.profile.read" },
                    { new Guid("08b20500-d3a2-d85e-42d1-08dce1858470"), "View transactions sent to your virtual balance.", "View Transactions", "account.transactions" },
                    { new Guid("08b20600-d3a2-d85e-42d1-08dce1858470"), "Manage your logins and change your password.", "Logins", "account.logins" },
                    { new Guid("08b20700-d3a2-d85e-42d1-08dce1858470"), "Manage your OAuth applications.", "Applications", "applications" },
                    { new Guid("08b20800-d3a2-d85e-42d1-08dce1858470"), "View your OAuth applications.", "View Applications", "applications.read" },
                    { new Guid("08b20900-d3a2-d85e-42d1-08dce1858470"), "Manage your authorized third party apps.", "Authorizations", "authorizations" },
                    { new Guid("08b20a00-d3a2-d85e-42d1-08dce1858470"), "View your authorized third party apps.", "Read Authorizations", "authorizations.read" },
                    { new Guid("08b20b00-d3a2-d85e-42d1-08dce1858470"), "Send transactions and view your virtual balance.", "Transactions", "transactions" },
                    { new Guid("08b22700-d3a2-d85e-42d1-08dce1858470"), "Link a third party login provider to your account.", "Link Provider", "link_provider" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "display_name", "name", "scope_id" },
                values: new object[,]
                {
                    { new Guid("08b20d00-d3a2-d85e-42d1-08dce1858470"), "Sudo", "scp:sudo", new Guid("08b20000-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b20e00-d3a2-d85e-42d1-08dce1858470"), "Admin", "scp:admin", new Guid("08b20100-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b20f00-d3a2-d85e-42d1-08dce1858470"), "Full Account Access", "scp:account", new Guid("08b20200-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21000-d3a2-d85e-42d1-08dce1858470"), "Profile", "scp:account.profile", new Guid("08b20300-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21100-d3a2-d85e-42d1-08dce1858470"), "Read Profile", "scp:account.profile.read", new Guid("08b20400-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21200-d3a2-d85e-42d1-08dce1858470"), "Read Transactions", "scp:account.transactions", new Guid("08b20500-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21300-d3a2-d85e-42d1-08dce1858470"), "Logins", "scp:account.logins", new Guid("08b20600-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21400-d3a2-d85e-42d1-08dce1858470"), "Applications", "scp:applications", new Guid("08b20700-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21500-d3a2-d85e-42d1-08dce1858470"), "View Applications", "scp:applications.read", new Guid("08b20800-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21600-d3a2-d85e-42d1-08dce1858470"), "Authorizations", "scp:authorizations", new Guid("08b20900-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21700-d3a2-d85e-42d1-08dce1858470"), "Read Authorizations", "scp:authorizations.read", new Guid("08b20a00-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b21800-d3a2-d85e-42d1-08dce1858470"), "Transactions", "scp:transactions", new Guid("08b20b00-d3a2-d85e-42d1-08dce1858470") },
                    { new Guid("08b22800-d3a2-d85e-42d1-08dce1858470"), "Link Provider", "scp:link_provider", new Guid("08b22700-d3a2-d85e-42d1-08dce1858470") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_permissions_permission_id",
                table: "application_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_client_id",
                table: "applications",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_owner_id",
                table: "applications",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorization_scopes_scope_id",
                table: "authorization_scopes",
                column: "scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_application_id",
                table: "authorizations",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_login_attempts_login_id",
                table: "login_attempts",
                column: "login_id");

            migrationBuilder.CreateIndex(
                name: "IX_logins_provider_account_id",
                table: "logins",
                column: "provider_account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_logins_provider_username",
                table: "logins",
                column: "provider_username");

            migrationBuilder.CreateIndex(
                name: "IX_logins_user_id",
                table: "logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_objectives_application_id",
                table: "objectives",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_objectives_name",
                table: "objectives",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_scope_id",
                table: "permissions",
                column: "scope_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_redirects_application_id",
                table: "redirects",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_redirects_uri",
                table: "redirects",
                column: "uri");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scopes_name",
                table: "scopes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scores_objective_id",
                table: "scores",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_scores_user_id",
                table: "scores",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_system_application_id",
                table: "system",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_application_id",
                table: "tokens",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_authorization_id",
                table: "tokens",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_reference_id",
                table: "tokens",
                column: "reference_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_application_id",
                table: "transactions",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_recipient_id",
                table: "transactions",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_subject_id",
                table: "transactions",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_permissions");

            migrationBuilder.DropTable(
                name: "authorization_scopes");

            migrationBuilder.DropTable(
                name: "login_attempts");

            migrationBuilder.DropTable(
                name: "redirects");

            migrationBuilder.DropTable(
                name: "scores");

            migrationBuilder.DropTable(
                name: "system");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "logins");

            migrationBuilder.DropTable(
                name: "objectives");

            migrationBuilder.DropTable(
                name: "authorizations");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "scopes");

            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
