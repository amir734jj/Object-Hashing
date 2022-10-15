using ObjectHashing;
using ObjectHashing.Interfaces;
using ObjectHashing.Models;

namespace Core.Tests.Models;

public class DummyModel : ObjectHash<DummyModel>
{
    private readonly HashAlgorithm _algorithm;

    public DummyModel(HashAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    protected override void ConfigureObjectSha(IConfigureObjectHashConfig<DummyModel> config)
    {
        config
            .Algorithm(_algorithm)
            .AllProperties()
            .DefaultSerialization()
            .Build();
    }

    public string Id { get; set; }
    
    public string Firstname { get; set; }
    
    public string Lastname { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public List<string> Sessions { get; set; }
    
    public int Year { get; set; }
}