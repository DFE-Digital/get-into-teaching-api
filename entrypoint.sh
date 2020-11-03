# See https://www.benday.com/2017/03/17/deploy-entity-framework-core-migrations-from-a-dll/
EfMigrationsNamespace=GetIntoTeachingApi
EfMigrationsDllName=GetIntoTeachingApi.dll
EfMigrationsDllDepsJson=GetIntoTeachingApi.deps.json
DllDir=$PWD
EfMigrationsDllDepsJsonPath=$PWD/bin/$BuildFlavor/netcoreapp1.0/$EfMigrationsDllDepsJson
PathToNuGetPackages=$HOME/.nuget/packages
PathToEfDll=$PathToNuGetPackages/microsoft.entityframeworkcore.tools.dotnet/3.1.9/tools/netcoreapp1.0/ef.dll

dotnet exec --depsfile ./$EfMigrationsDllDepsJson --additionalprobingpath $PathToNuGetPackages $PathToEfDll database update --assembly ./$EfMigrationsDllName --startup-assembly ./$EfMigrationsDllName --project-dir . --content-root $DllDir --data-dir $DllDir --verbose --root-namespace $EfMigrationsNamespace

dotnet GetIntoTeachingApi.dll