using Core.Interfaces;
using Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.WorkWithFiles.Export;

public class YamlExporter : IDataExporter
{
    public void Export(FinanceContext context, string filePath)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yamlString = serializer.Serialize(context);
        File.WriteAllText(filePath, yamlString);
    }
}