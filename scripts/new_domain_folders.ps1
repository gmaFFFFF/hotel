$ContextName = 'ContextName'


New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Contracts\Mappers"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Contracts\Repositories"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "DomainEvents"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Dto"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Errors"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Models"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Services"
New-Item  -ItemType Directory -Force -Path ".\$ContextName" -Name "Validations"