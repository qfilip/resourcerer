<Query Kind="Program">
  <Reference Relative="..\src\Resourcerer.DataAccess\bin\Debug\net7.0\Resourcerer.DataAccess.dll">D:\Riznice\resourcerer\Resourcerer_Sol\src\Resourcerer.DataAccess\bin\Debug\net7.0\Resourcerer.DataAccess.dll</Reference>
  <Reference Relative="..\src\Resourcerer.Dtos\bin\Debug\net7.0\Resourcerer.Dtos.dll">D:\Riznice\resourcerer\Resourcerer_Sol\src\Resourcerer.Dtos\bin\Debug\net7.0\Resourcerer.Dtos.dll</Reference>
  <Namespace>Resourcerer.DataAccess.Entities</Namespace>
  <Namespace>Resourcerer.Dtos</Namespace>
</Query>

// add DataAccess.dll
// add Dtos.dll
void Main()
{
	var admin = new Resourcerer.DataAccess.Entities.AppUser();
	Permission.GetAllPermissionsDictionary();
}

