using PersistentKeyValueStorage.Repository;
using System.Collections.Generic;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Xbehave;
using FluentAssertions;
using PersistentKeyValueStorage.Model;
using System;

namespace PersistentKeyValueStorageTest
{
  public class PersistentKeyValueStorageTest
    {
        private const string MONGO_EMULATOR_CONNECTION = "mongodb://localhost:C2y6yDjf5%2FR%2Bob0N8A7Cgv30VRDJIWEHLM%2B4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw%2FJw%3D%3D@localhost:10255/admin?ssl=true&replicaSet=globaldb";
        private static IConfiguration ConfigurationMongo()
        {
            Mock<IConfiguration> configMock = new();
            configMock.Setup(x => x["Connections:PersistentKeyValueStorage:ConnectionString"]).Returns(MONGO_EMULATOR_CONNECTION);
            configMock.Setup(x => x["Connections:PersistentKeyValueStorage:Database"]).Returns("UnitTest");

            return configMock.Object;
        }

        public static IEnumerable<object[]> Strategies()
        {
            yield return new object[] { new Mongo(ConfigurationMongo())};
            yield return new object[] { new Memory()};
        }

        [Scenario]
        [MemberData(nameof(Strategies))]
        public void Test_DeleteAsync_WhenRowNotExists_ReturnsFalse(IPersistentKeyValueStorage storage, bool actual)
        {
            "When calling deleteAsync for not existing entry"
                .x(async () => actual = await storage.DeleteAsync("i really not exist"));

            "Then it returns false"
                .x(() => actual.Should().BeFalse(because: "When no row exists, no delete happens"));
        }

        [Scenario]
        [MemberData(nameof(Strategies))]
        public void Test_DeleteAsync_WhenRowExists_ReturnsTrue(IPersistentKeyValueStorage storage, bool actual)
        {
            "Given a valid entry"
                .x(async () => await storage.CreateOrUpdateAsync(new() { Key = "i exist", Value = "testing is great"}) );
            "When calling deleteAsync for not existing entry"
                .x(async () => actual = await storage.DeleteAsync("i exist"));

            "Then it returns false"
                .x(() => actual.Should().BeTrue(because: "When row exists,the delete happend"));
        }

        [Scenario]
        [MemberData(nameof(Strategies))]
        public void Test_CreateOrUpdateAsync_Given_A_New_Entry_WillBeAdded(IPersistentKeyValueStorage storage,Entry toCreate, string actualValue)
        {
            "Given a valid entry"
                .x(() => toCreate = new(){
                    Key = "unittest" + Guid.NewGuid().ToString(),
                    Value = "i am a great value"
                });
            "When calling CreateOrUpdateAsync"
                .x(async() => await storage.CreateOrUpdateAsync(toCreate))
                .Teardown(() => storage.DeleteAsync(toCreate.Key));
                "and GetValueAsync"
                    .x(async() => actualValue = await storage.GetValueAsync(toCreate.Key));
            
            "Then the value is correct"
                .x(() => actualValue.Should().Be(toCreate.Value, "the data has been saved"));
        }

        [Scenario]
        [MemberData(nameof(Strategies))]
        public void Test_CreateOrUpdateAsync_Given_An_Existing_Entry_WillBeAdded(IPersistentKeyValueStorage storage,Entry toCreate, string actualValue)
        {
            "Given an already existing entry with same key"
                .x(() => toCreate = new(){
                    Key = "unittest_existing",
                    Value = "val1"
                });
                "and calling CreateOrUpdateAsync"
                    .x(async() => await storage.CreateOrUpdateAsync(toCreate));

            "When calling CreateOrUpdateAsync with same key but another value"
                .x(async() => {
                    toCreate.Value ="val2";
                    await storage.CreateOrUpdateAsync(toCreate);
                }).Teardown(() => storage.DeleteAsync("unittest_existing"));
                "and GetValueAsync"
                    .x(async() => actualValue = await storage.GetValueAsync(toCreate.Key));
            
            "Then the value is val2"
                .x(() => actualValue.Should().Be(toCreate.Value, "the data has been overriden"));
        }

        [Scenario]
        [MemberData(nameof(Strategies))]
        public void Test_GetValueAsync_Given_NotExisting_Key_Returns_EmptyString(IPersistentKeyValueStorage storage, string actual)
        {
            "When calling GetValueAsync for not existing key"
                .x(async() => actual = await storage.GetValueAsync(Guid.NewGuid().ToString()));

            "Then an empty string has been returned"
                .x(() => actual.Should().BeEmpty("I dont want exceptions ;)"));
        }
        
    }
}
