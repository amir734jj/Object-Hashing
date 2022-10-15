# Object-Hashing

Simple library to auto-generate hash code (base64 hashcode) for a class.

```csharp
var model = new DummyModel();
model.GenerateHash();       // SHA1 hash encoded base64 string of all properties.

// Just extend abstract class ObjectHash
public class DummyModel : ObjectHash<DummyModel>
{
    // Optionally, If you want to modify the default behavior, override this function.
    // Default behavior is: Sha1, all properties, JSON.net serialization
    protected override void ConfigureObjectSha(IConfigureObjectHashConfig<DummyModel> config)
    {
        config
            .Algorithm(HashAlgorithm.Sha256)
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
```

