[string]$sln = 'gmafffff.training.hotel'
$prjs = @{
	'application' = 'classlib';
	'application.tests' = 'xunit';
	'business' = 'classlib';
	'business.tests' = 'xunit';
	'domain' = 'classlib';
	'domain.tests' = 'xunit';
	'infrastructure.data' = 'classlib';
	'infrastructure.data.tests' = 'xunit';
	'infrastructure.mapper' = 'classlib';
	'api' = 'webapi';
	'webSite' = 'razor';
}

[string]$dirBldName = 'Directory.Build.props'
[string]$dirBldContent = @'
<Project>
    <PropertyGroup>
        <BaseIntermediateOutputPath>$(TEMP)\gmafffff\{0}\{1}\obj\</BaseIntermediateOutputPath>
        <BaseOutputPath>$(TEMP)\gmafffff\{0}\{1}\bin\</BaseOutputPath>
    </PropertyGroup>
</Project>
'@

# Создаем каталог для решения
New-Item -ItemType Directory -Name $sln
cd $sln

# Создаем новое решение
dotnet new sln --name $sln

# Создаем проекты по списку
$prjs.Keys | % {dotnet new $prjs[$_] -o "${sln}.$_"}

# Настраиваем выходные каталоги проекта
$prjs.Keys | % {$dirBldContent -f $sln, $_ >"${sln}.$_\$dirBldName"}

# Удаляем «мусор»
Get-ChildItem -Recurse -Filter Class1.cs | Remove-Item
Get-ChildItem -Recurse -Filter UnitTest1.cs | Remove-Item
Get-ChildItem -Recurse -Filter obj -Directory | Remove-Item  -Recurse

# Добавляем в проекты с тестами ссылки на тестируемые проекты
Get-ChildItem -Recurse -Filter *.tests*.csproj | % {dotnet add $_.FullName reference (Get-ChildItem ($_.BaseName -replace '.tests','') -Filter *.csproj).FullName}

# Добавляем проекты в решение
dotnet sln add (Get-ChildItem -Directory)

# Создаем список используемых nuget утилит
dotnet new tool-manifest
#  Установка пакетов по умолчанию
# TODO: Вынести список пакетов отдельно
dotnet tool install dotnet-ef
dotnet tool install Mapster.Tool

# Сохраняем текущий скрипт 
New-Item -ItemType Directory -Name .\scripts
Copy-Item $PSCommandPath .\scripts\init_prj.ps1 -Recurse

# Создаем репозиторий
hg init
hg add --include *.ps1
hg commit --message "chore: Добавлен скрипт инициализации проекта"

