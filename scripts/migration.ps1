[CmdletBinding()]
Param
(
	[string]$action,
	[string]$title
)

$data_prj = Get-ChildItem -Recurse -File -Filter *.data.csproj
$sln =  (Get-Item $PSScriptRoot).Parent.Name
$prj_name = $data_prj.Directory.Name
$prj_fname = $data_prj.FullName
$prj_abbr = $prj_name -replace "$sln.", ''
$dll_dir = "$env:TEMP\gmafffff\$sln\$prj_abbr\obj\"
$migration = Join-Path $data_prj.Directory.FullName Migrations
$sql  = Join-Path $data_prj.Directory.FullName Sql | Join-Path -ChildPath "$title.sql"

if($title -eq $null -and $action -ne 'db'){
	$title = $(Get-Date -Format "yyyy-MM-dd_HH-mm")
}

Write-Verbose "migration title: $title"
Write-Verbose "project: $prj_fname"
Write-Verbose "dll dir: $dll_dir"
Write-Verbose "migration dir: $migration"
Write-Verbose "migration dir: $sql"

if($action -eq 'add'){
	dotnet ef migrations add $title --project $prj_fname --msbuildprojectextensionspath $dll_dir --output-dir $migration 
}
elseif ($action -eq 'remove') {
	dotnet ef migrations remove --project $prj_fname --msbuildprojectextensionspath $dll_dir
}
elseif ($action -eq 'script') {
	dotnet ef migrations script --project $prj_fname --msbuildprojectextensionspath $dll_dir --output $sql
}
elseif ($action -eq 'db') {
	dotnet ef database update --project $prj_fname --msbuildprojectextensionspath $dll_dir $title 
}