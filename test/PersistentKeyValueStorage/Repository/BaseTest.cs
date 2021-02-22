using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using PersistentKeyValueStorage.Model;
using PersistentKeyValueStorage.Repository;
using Xbehave;
using Xunit;

namespace PersistentKeyValueStorageTest.Repository
{
  public class BaseTest
  {
    private BaseImpl _sUT;

    [Background]
    public void Background()
    {
      "Given an initialized sUT"
        .x(() => _sUT = new BaseImpl());
    }

    public static IEnumerable<object[]> InvalidEntries()
    {
      yield return new object[] { null, "entry"};
      yield return new object[] { new Entry() { Key = String.Empty, Value = "existing" }, "Key" };
      yield return new object[] { new Entry() { Key = "     ", Value = "existing" }, "Key" };
      yield return new object[] { new Entry() { Key = null, Value = "existing" } , "Key" };
      yield return new object[] { new Entry() { Value = String.Empty, Key = "existing" }, "Value" };
      yield return new object[] { new Entry() { Value = "     ", Key = "existing" }, "Value" };
      yield return new object[] { new Entry() { Value = null, Key = "existing" } , "Value" };
    }
    
    [Scenario]
    [MemberData(nameof(InvalidEntries))]  
    public void Test_CreateOrUpdateAsync_Given_Invalid_Entry_Throws_ArgumentNullWithProperMessage(Entry entry, string message, Func<Task> createOrUpdateAsync)
    {
      "When calling CreateOrUpdateAsync with missing values"
        .x(() => createOrUpdateAsync = async() => await this._sUT.CreateOrUpdateAsync(entry));
      
      "Then an ArgumentNullException with proper message has been thrown"
        .x(() => createOrUpdateAsync.Should().ThrowExactlyAsync<ArgumentNullException>()
                                             .WithMessage($"*{message}*"));
    }


    [Scenario]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Test_DeleteAsync_Given_Invalid_KeyName_Throws_ArgumentNullWithProperMessage(string keyName, Func<Task> deleteAsync)
    {
      "When calling DeleteAsync with invalid keyName"
        .x(() => deleteAsync = async() => await this._sUT.DeleteAsync(keyName));
      
      "Then an ArgumentNullException with proper message has been thrown"
        .x(() => deleteAsync.Should().ThrowExactlyAsync<ArgumentNullException>()
                                             .WithMessage($"*keyName*"));
    }   

    [Scenario]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Test_GetValueAsync_Given_Invalid_KeyName_Throws_ArgumentNullWithProperMessage(string keyName, Func<Task> getValueAsync)
    {
      "When calling DeleteAsync with invalid keyName"
        .x(() => getValueAsync = async() => await this._sUT.GetValueAsync(keyName));
      
      "Then an ArgumentNullException with proper message has been thrown"
        .x(() => getValueAsync.Should().ThrowExactlyAsync<ArgumentNullException>()
                                             .WithMessage($"*keyName*"));
    }     

    public static IEnumerable<object[]> ValidDeleteKeys()
    {
      yield return new object[] { "fail", false };
      yield return new object[] { "success", true };
    }
    [Scenario]
    [MemberData(nameof(ValidDeleteKeys))]
    public void Test_DeleteAsync_Given_ValidKey_Returns_DeleteSucceededTrueOrFalse(string keyName, bool expected, bool actual)
    {
      "When calling DeleteAsync with valid keyName"
        .x(async() => actual = await this._sUT.DeleteAsync(keyName));
      "Then it returns the correct delete status"
        .x(() => actual.Should().Be(expected));
    }  

    [Scenario]
    public void Test_GetValueAsync_Given_ValidKey_Returns_CorrectValue(string expected, string actual)
    {
      "Given an expected string"
        .x(() => expected = "some value");
      "When calling GetValueAsync with valid keyName"
        .x(async() => actual = await this._sUT.GetValueAsync("keyName"));
      "Then it returns the correct delete status"
        .x(() => actual.Should().Be(expected));
    }  
  }

  public class BaseImpl : Base
  {
    public BaseImpl()
    {
    }

    protected override Task ExecuteCreateOrUpdateAsync(Entry entry)
    {
      return Task.CompletedTask;
    }

    protected override Task<bool> ExecuteDeleteAsync(string keyName)
    {
      if(keyName == "fail")
        return Task.FromResult(false);
      return Task.FromResult(true);
    }

    protected override Task<string> ExecuteGetValueAsync(string keyName)
    {
      return Task.FromResult("some value");
    }
  }
}