// Стандартные пространства имен

global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Threading;
global using System.Threading.Tasks;

// Ef Core
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.ValueGeneration;

// Модель
global using gmafffff.training.hotel.domain.Model;
global using gmafffff.training.hotel.domain.Contracts.Repositories;
global using gmafffff.training.hotel.domain.Contracts.Mappers;

// Конфигурация
global using gmafffff.training.hotel.infrastructure.data.Configs;