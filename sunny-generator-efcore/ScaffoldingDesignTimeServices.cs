using System.Text.Json;
using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace Sunny.Generator.EFCore;

public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        services.AddHandlebarsScaffolding(options =>
        {
            options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;
            options.TemplateData = new Dictionary<string, object>();
        });

        var jsonHelper = (helperName: "json", helperFunction: (Action<EncodedTextWriter, Context, Arguments>)JsonHbsHelper);
        var ifEqualsHelper = (helperName: "ifEquals", helperFunction: (Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments>)EqHbsHelper);
        services.AddHandlebarsHelpers(jsonHelper);
        services.AddHandlebarsBlockHelpers(ifEqualsHelper);
        services.AddSingleton<IModelCodeGenerator, MyModelGenerator>();
        services.AddSingleton<ICSharpEntityTypeGenerator, MyEntityTypeGenerator>();
        services.AddHandlebarsTransformers2(
            t => t + "PO",
            t => t + "PO",
            propertyTransformer: (m, p) =>
            {
                var property = m.GetProperties().Where(t => t.Name == p.PropertyName).FirstOrDefault();
                if (property != null)
                {
                    if (property.GetColumnType().StartsWith("bit")) return new EntityPropertyInfo("bool", p.PropertyName, p.PropertyIsNullable);
                    if (property.GetColumnType().StartsWith("tinyint")) return new EntityPropertyInfo("int", p.PropertyName, p.PropertyIsNullable);
                }

                return new EntityPropertyInfo(p.PropertyType, p.PropertyName, p.PropertyIsNullable);
            });
    }

    private void JsonHbsHelper(EncodedTextWriter writer, Context context, Arguments arguments)
    {
        var json = JsonSerializer.Serialize(arguments[0], new JsonSerializerOptions { WriteIndented = true });
        writer.WriteSafeString(json);
    }

    private void EqHbsHelper(EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments)
    {
        if (arguments.Length != 2)
        {
            writer.Write("<!-- Error: ifEquals needs two arguments -->");
            return;
        }

        var val1 = arguments[0]?.ToString();
        var val2 = arguments[1]?.ToString();

        if (val1 == val2)
            options.Template(writer, context); // render the block
        else
            options.Inverse(writer, context); // render the {{else}} block
    }
}