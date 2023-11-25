<Query Kind="Program">
  <Connection>
    <ID>c3dbaed0-055b-497a-868b-465a57212a9a</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AttachFileName>D:\Riznice\resourcerer\Resourcerer_Sol\src\Resourcerer.Api\wwwroot\resourcerer.db3</AttachFileName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.Sqlite</EFProvider>
    </DriverData>
  </Connection>
  <Reference Relative="..\src\Resourcerer.DataAccess\bin\Debug\net7.0\Resourcerer.DataAccess.dll">D:\Riznice\resourcerer\Resourcerer_Sol\src\Resourcerer.DataAccess\bin\Debug\net7.0\Resourcerer.DataAccess.dll</Reference>
  <Reference Relative="..\src\Resourcerer.Dtos\bin\Debug\net7.0\Resourcerer.Dtos.dll">D:\Riznice\resourcerer\Resourcerer_Sol\src\Resourcerer.Dtos\bin\Debug\net7.0\Resourcerer.Dtos.dll</Reference>
  <Reference Relative="..\src\Resourcerer.Utilities\bin\Debug\net7.0\Resourcerer.Utilities.dll">D:\Riznice\resourcerer\Resourcerer_Sol\src\Resourcerer.Utilities\bin\Debug\net7.0\Resourcerer.Utilities.dll</Reference>
  <Namespace>Resourcerer.DataAccess.Entities</Namespace>
  <Namespace>Resourcerer.Dtos</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>Resourcerer.Utilities.Cryptography</Namespace>
</Query>

// add DataAccess.dll
// add Dtos.dll
// add Utilities.dll
void Main()
{
	var permissionDict = Permissions.GetAllPermissionsDictionary();
	var permissions = JsonSerializer.Serialize(permissionDict);
	
	var passwordHash = Hasher.GetSha256Hash("admin");
	
	var now = new DateTime(2000, 1, 1);
	var id = Guid.Parse(Guid.Empty.ToString().Replace('0', '1'));

	var admin = new Resourcerer.DataAccess.Entities.AppUser()
	{
		Id = id,
		Name = "admin",
		PasswordHash = passwordHash,
		Permissions = permissions,
		CreatedAt = now,
		ModifiedAt = now
	};

	var command = @$"
	INSERT INTO AppUser
	(Id, Name, PasswordHash, Permissions, EntityStatus, CreatedAt, ModifiedAt)
    VALUES (
        '{admin.Id}',
        '{admin.Name}',
        '{admin.PasswordHash}',
        '{admin.Permissions}',
        '0',
        '{admin.CreatedAt}',
        '{admin.ModifiedAt}');
	";

	command.Dump();
}

