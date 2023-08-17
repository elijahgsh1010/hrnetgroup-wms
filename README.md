# hrnetgroup-wms

### setup
- update the connectionString in Hrnetgroup.Wms.DbMigrator/appsettings.json
- update the connectionString in Hrnetgroup.Wms.Host/appsettings.json
- run Hrnetgroup.Wms.DbMigrator project
  or
- change directory to Hrnetgroup.Wms.EntityframeworkCore
  - dotnet ef database update
 
### xUnit test
- contains the given sample test case

### auth
- use the SimpleTokenAuth endpoint to get a jwt token
- paste the token inside swagger authorize ( no need prepend Bearer)
