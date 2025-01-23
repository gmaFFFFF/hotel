// Тесты

global using FluentAssertions;
global using FluentAssertions.Execution;
global using Xunit.Abstractions;

// Общие
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Data.Sqlite;
global using System.Collections.Immutable;

// Модель
global using gmafffff.training.hotel.domain.Model;
global using gmafffff.training.hotel.domain.tests.FakeModel;
global using gmafffff.training.hotel.infrastructure.data.Repositories;
global using gmafffff.training.hotel.infrastructure.data.Sessions;

//Внутреннее
global using gmafffff.training.hotel.infrastructure.data.tests.Fixtures;