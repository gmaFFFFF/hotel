// Тесты

global using FluentAssertions;
global using FluentAssertions.Execution;
global using Xunit;
global using Xunit.Abstractions;

// Общие
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Data.Sqlite;
global using System.Collections.Immutable;

// Проект
global using gmafffff.training.hotel.domain.Model;
global using gmafffff.training.hotel.infrastructure.data.Repositories;
global using gmafffff.training.hotel.infrastructure.data.Sessions;
global using gmafffff.training.hotel.infrastructure.data.tests.Fixtures;
global using gmafffff.training.hotel.sampleModel.FakeModel;