using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace Sunny.Generator.EFCore;

public class MyEntityTypeGenerator : HbsCSharpEntityTypeGenerator
{
    private readonly Dictionary<IEntityType, Dictionary<string, object>> EntityTypeTemplateProperties = new();

    public MyEntityTypeGenerator(IAnnotationCodeGenerator annotationCodeGenerator, ICSharpHelper cSharpHelper, IEntityTypeTemplateService entityTypeTemplateService, IEntityTypeTransformationService entityTypeTransformationService, IOptions<HandlebarsScaffoldingOptions> options) : base(annotationCodeGenerator, cSharpHelper, entityTypeTemplateService, entityTypeTransformationService, options)
    {
    }

    protected override void GenerateClass(IEntityType entityType)
    {
        base.GenerateClass(entityType);
        TemplateData.Add("entity-name", entityType.Name);
        TemplateData.Add("table-name", entityType.GetTableName());
        EntityTypeTemplateProperties.Add(entityType, TemplateData);
    }

    protected override void GenerateProperties(IEntityType entityType)
    {
        base.GenerateProperties(entityType);
        var propertyNameColumnNameDict = entityType.GetProperties().ToDictionary(p => p.Name, p => p.GetColumnName());
        var properties = (List<Dictionary<string, object>>)TemplateData["properties"];
        foreach (var t in propertyNameColumnNameDict)
        foreach (var p in properties)
        {
            var propertyName = (string)p["property-name"];
            var propertyType = (string)p["property-type"];
            if (propertyName == t.Key) p["column-name"] = t.Value;

            if (!propertyType.EndsWith("?")) p["property-type"] = propertyType + "?";
            if (propertyType.StartsWith("string")) p["property-type"] = "string";
            p["field-type"] = ((string)p["property-type"]).Replace("?", "");
        }

        TemplateData.Add("primary-key-type", entityType.GetProperties().Where(t => t.IsPrimaryKey()).Select(t => CSharpHelper.Reference(t.ClrType)).FirstOrDefault() + "?");

        var propertiesAll = new List<Dictionary<string, object>>((List<Dictionary<string, object>>)TemplateData["properties"]);

        TemplateData.Add("propertiesAll", propertiesAll);

        List<string> excludePropertyNames = ["Id", "CreateTime", "CreateUser", "UpdateTime", "UpdateUser", "Deleted"];
        properties.RemoveAll(t => excludePropertyNames.Contains(t["property-name"]));
    }

    public Dictionary<IEntityType, Dictionary<string, object>> GetEntityTypeTemplateProperties()
    {
        return EntityTypeTemplateProperties;
    }
}