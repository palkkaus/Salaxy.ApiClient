using NSwag.CodeGeneration.CSharp;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using NJsonSchema.CodeGeneration.TypeScript;

namespace Tools.ApiCodegen
{
    /// <summary>
    /// Tools for generating code from the Salaxy APIs.
    /// </summary>
    public class NSwagGenerator
    {
        private readonly string serverUrl;
        private readonly string targetFolder;
        private readonly string swaggerSpecUrl;

        /// <summary>
        /// Creates a new code generator wrapper for NSwag.
        /// </summary>
        /// <param name="serverUrl">Salaxy API server URL</param>
        public NSwagGenerator(string serverUrl, string targetFolder)
        {
            this.serverUrl = serverUrl;
            this.targetFolder = targetFolder;
            this.swaggerSpecUrl = serverUrl + "/docs/swagger/v03";
        }

        /// <summary>
        /// Generates the C# code for connecting to Salaxy main API.
        /// </summary>
        public async Task GenerateCSharp()
        {
            var document = await OpenApiDocument.FromUrlAsync(swaggerSpecUrl);
            foreach (var path in document.Paths)
            {
                // Remove "application/x-www-form-urlencoded" beccause of this issue: https://github.com/RicoSuter/NSwag/issues/3414
                foreach (var method in path.Value)
                {
                    if (method.Key == "post")
                    {
                        method.Value.Consumes = method.Value.Consumes.Where(x => x != "application/x-www-form-urlencoded").ToList();
                    }
                }
            }
            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = "{controller}Client",
                OperationNameGenerator = new MultipleClientsFromOperationIdOperationNameGenerator(),
                CSharpGeneratorSettings =
                {
                    Namespace = "Salaxy.Client.Api"
                }
            };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();
            code = code.Replace("Newtonsoft.Json.Converters.StringEnumConverter", "Salaxy.Client.SafeEnumConverter");
            File.WriteAllText(targetFolder + "/Salaxy.Client.Api.cs", code);
        }

        /// <summary>
        /// Generates the typescript code for connecting to Salaxy main API.
        /// TODO: This is just a demo => Change this to only generate the Model and save that to Monorepo.
        /// </summary>
        /// <returns></returns>
        public async Task GenerateTypescript()
        {
            // HACK: Can we move to 03 here or not?
            var url = swaggerSpecUrl.Replace("swagger/v03", "swagger/v02") + "?showObsolete=true";
            var document = await OpenApiDocument.FromUrlAsync(url);
            // HACK: Go through all the config from NSwagStudio file to assure compatibility.
            var settings = new TypeScriptClientGeneratorSettings
            {
                ClassName = "{controller}Client",
                GenerateClientClasses = false,
                GenerateClientInterfaces = false,
                GenerateOptionalParameters = false,
                OperationNameGenerator = new MultipleClientsFromOperationIdOperationNameGenerator(),
                ImportRequiredTypes = true,
                BaseUrlTokenName = "API_BASE_URL",
                TypeScriptGeneratorSettings = {
                    TypeStyle = TypeScriptTypeStyle.Interface,
                }
            };

            var generator = new TypeScriptClientGenerator(document, settings);
            var code = generator.GenerateFile();
            File.WriteAllText(targetFolder + "/Salaxy.Client.Api.ts", code);
        }

    }
}
