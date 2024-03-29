using AutoFixture;
using Core.Tests.Models;
using ObjectHashing.Models;
using Xunit;

namespace Core.Tests;

public class ObjectHashTest
{
    private readonly Fixture _fixture = new();

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

    [Theory]
    [InlineData(HashAlgorithm.Md5)]
    [InlineData(HashAlgorithm.Sha1)]
    [InlineData(HashAlgorithm.Sha256)]
    [InlineData(HashAlgorithm.Sha384)]
    [InlineData(HashAlgorithm.Sha512)]
    public void Test__Hash_Uniqueness(HashAlgorithm algorithm)
    {
        // Arrange
        var models = _fixture.Build<DummyModel>()
            .FromFactory<int>(_ => new DummyModel(algorithm))
            .CreateMany(100)
            .ToList();
        
        // Act
        var hashes = models.Select(x => x.GenerateHash()).ToHashSet();

        // Assert
        Assert.Equal(models.Count, hashes.Count);
    }
}