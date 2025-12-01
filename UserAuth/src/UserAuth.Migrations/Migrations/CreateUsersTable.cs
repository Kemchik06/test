using FluentMigrator;

namespace UserAuth.Migrations.Migrations;

[Migration(1)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        var sql = @"
        CREATE TABLE IF NOT EXISTS users (
            id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            email TEXT NOT NULL,
            hashed_password TEXT NOT NULL,
            name TEXT NOT NULL,
            created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            updated_at TIMESTAMPTZ,
            is_active BOOLEAN NOT NULL DEFAULT true
        );

            CREATE UNIQUE INDEX uq_users_email_lower_idx ON users ((lower(email)));";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        var sql = @"
            DROP INDEX IF EXISTS uq_users_email_lower_idx;
            DROP TABLE IF EXISTS users;";
        Execute.Sql(sql);
    }
}