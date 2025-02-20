namespace Thavyra.Rest.Documentation;

[AttributeUsage(AttributeTargets.Class)]
public class SchemaNameAttribute : Attribute
{
    public SchemaNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}