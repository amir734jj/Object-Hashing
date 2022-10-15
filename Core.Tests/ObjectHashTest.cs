using AutoFixture;
using Core.Models;
using Core.Tests.Models;
using Xunit;

namespace Core.Tests;

public class ObjectHashTest
{
    private readonly Fixture _fixture;

    public ObjectHashTest()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [InlineData(HashAlgorithm.Md5)]
    [InlineData(HashAlgorithm.Sha1)]
    [InlineData(HashAlgorithm.Sha256)]
    [InlineData(HashAlgorithm.Sha384)]
    [InlineData(HashAlgorithm.Sha512)]
    public void Test__Hash_Default(HashAlgorithm algorithm)
    {
        // Arrange
        var model = _fixture.Build<DummyModel>()
            .FromFactory<int>(_ => new DummyModel(algorithm))
            .Create();
        
        // Act
        var hash = model.GenerateHash();

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }
}