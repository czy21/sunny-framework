using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.Options;

namespace Sunny.Generator.EFCore;

public class MyModelGenerator : HbsCSharpModelGenerator
{
    private readonly MyEntityTypeGenerator _entityTypeGenerator;
    private readonly IOptions<HandlebarsScaffoldingOptions> _options;

    public MyModelGenerator(ModelCodeGeneratorDependencies dependencies, ICSharpDbContextGenerator cSharpDbContextGenerator, ICSharpEntityTypeGenerator cSharpEntityTypeGenerator, IHbsHelperService handlebarsHelperService, IHbsBlockHelperService handlebarsBlockHelperService, IDbContextTemplateService dbContextTemplateService, IEntityTypeTemplateService entityTypeTemplateService, IEntityTypeTransformationService entityTypeTransformationService, IContextTransformationService contextTransformationService, ICSharpHelper cSharpHelper, IOptions<HandlebarsScaffoldingOptions> options) : base(dependencies, cSharpDbContextGenerator, cSharpEntityTypeGenerator, handlebarsHelperService, handlebarsBlockHelperService, dbContextTemplateService, entityTypeTemplateService, entityTypeTransformationService, contextTransformationService, cSharpHelper, options)
    {
        _options = options;
        _entityTypeGenerator = (MyEntityTypeGenerator)cSharpEntityTypeGenerator;
    }

    public override ScaffoldedModel GenerateModel(IModel model, ModelCodeGenerationOptions options)
    {
        var scaffoldedModel = base.GenerateModel(model, options);

        foreach (var entityType in model.GetScaffoldEntityTypes(_options.Value))
        {
            scaffoldedModel.AdditionalFiles.Add(new ScaffoldedFile(Path.Combine("../Repository", $"I{entityType.Name}Repository.cs"), GenerateIRepositoryCode(entityType, _entityTypeGenerator.GetEntityTypeTemplateProperties()[entityType])));
            scaffoldedModel.AdditionalFiles.Add(new ScaffoldedFile(Path.Combine("../Repository/impl", $"{entityType.Name}Repository.cs"), GenerateRepositoryCode(entityType, _entityTypeGenerator.GetEntityTypeTemplateProperties()[entityType])));
        }

        return scaffoldedModel;
    }

    private string GenerateIRepositoryCode(IEntityType entityType, Dictionary<string, object> data)
    {
        return Handlebars.Compile(File.ReadAllText("CodeTemplates/Repository/IRepository.hbs"))(data);
    }

    private string GenerateRepositoryCode(IEntityType entityType, Dictionary<string, object> data)
    {
        return Handlebars.Compile(File.ReadAllText("CodeTemplates/Repository/Repository.hbs"))(data);
    }
}