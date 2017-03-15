﻿using CrossStitch.Core.Messages.Data;
using CrossStitch.Core.Models;
using CrossStitch.Core.Modules.Data;
using CrossStitch.Core.Modules.Data.InMemory;
using FluentAssertions;
using NUnit.Framework;

namespace CrossStitch.Core.Tests.Modules.Data
{
    [TestFixture]
    public class DataServiceTests
    {
        private static DataService CreateTarget()
        {
            var storage = new InMemoryDataStorage();
            storage.Save(new Application()
            {
                Name = "A"
            }, true);
            storage.Save(new Application()
            {
                Name = "B"
            }, true);
            storage.Save(new Application()
            {
                Name = "C"
            }, true);
            return new DataService(storage, null);
        }

        [Test]
        public void HandleRequest_GetAll_Test()
        {
            var target = CreateTarget();
            var result = target.HandleRequest(DataRequest<Application>.GetAll());
            result.Type.Should().Be(DataResponseType.Success);
            result.Entities.Count.Should().Be(3);
        }

        [Test]
        public void HandleRequest_Get_Test()
        {
            var storage = new InMemoryDataStorage();
            var entity = new Application()
            {
                Name = "A"
            };
            storage.Save(entity, true);
            var id = entity.Id;
            var target = new DataService(storage, null);

            var result = target.HandleRequest(DataRequest<Application>.Get(id));
            result.Type.Should().Be(DataResponseType.Success);
            result.Entity.Name.Should().Be("A");
        }

        [Test]
        public void HandleRequest_Delete_Test()
        {
            var storage = new InMemoryDataStorage();
            var entity = new Application()
            {
                Name = "A"
            };
            storage.Save(entity, true);
            var id = entity.Id;
            var target = new DataService(storage, null);

            var result = target.HandleRequest(DataRequest<Application>.Delete(id));
            result.Type.Should().Be(DataResponseType.Success);

            var result2 = storage.Get<Application>(id);
            result2.Should().BeNull();
        }

        [Test]
        public void HandleRequest_Save_Test()
        {
            var target = new DataService(new InMemoryDataStorage(), null);

            var entity = new Application()
            {
                Name = "A"
            };
            var result = target.HandleRequest(DataRequest<Application>.Save(entity));
            result.Type.Should().Be(DataResponseType.Success);
            result.Entity.Name.Should().Be("A");
        }
    }
}