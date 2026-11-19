using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DonationHub.Api.OpenApi.Transformers
{
    internal sealed class VersionInfoTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var version = context.DocumentName;

            document.Info.Version = version;

            document.Info.Title = $"DonationHub API {version}";

            return Task.CompletedTask;
        }
    }
}
