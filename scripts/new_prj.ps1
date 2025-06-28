[string]$sln = 'gmafffff.training.hotel'
$prjs = @{
	'infrastructure.data.pay' = 'classlib';
	'domain.pay' = 'classlib';
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

# Создаем проекты по списку
$prjs.Keys | % {dotnet new $prjs[$_] -o "${sln}.$_"}

# Настраиваем выходные каталоги проекта
$prjs.Keys | % {$dirBldContent -f $sln, $_ >"${sln}.$_\$dirBldName"}

# Удаляем «мусор»
Get-ChildItem -Recurse -Filter Class1.cs | Remove-Item
Get-ChildItem -Recurse -Filter UnitTest1.cs | Remove-Item
Get-ChildItem -Recurse -Filter obj -Directory | Remove-Item  -Recurse

# Добавляем проекты в решение
$prjs.Keys | % {dotnet sln add "${sln}.$_" }