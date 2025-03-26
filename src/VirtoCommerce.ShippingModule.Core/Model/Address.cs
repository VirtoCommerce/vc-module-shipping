using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.ShippingModule.Core.Model;

[SwaggerSchemaId("ShippingAddress")]
public class Address : CoreModule.Core.Common.Address
{
    public string Id { get; set; }
}

