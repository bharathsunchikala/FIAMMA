namespace Fiamma.Web.Extensions;

public static class CacheHelpers
{
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(30);
    private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}-{3}-{4}";

    public static string GenerateCatalogItemCacheKey(int pageIndex, int itemsPage, int? brandId, int? typeId, string? searchTerm = null)
    {
        var normalizedSearch = string.IsNullOrWhiteSpace(searchTerm)
            ? string.Empty
            : searchTerm.Trim().ToLowerInvariant();

        return string.Format(_itemsKeyTemplate, pageIndex, itemsPage, brandId, typeId, normalizedSearch);
    }

    public static string GenerateBrandsCacheKey()
    {
        return "brands";
    }

    public static string GenerateTypesCacheKey()
    {
        return "types";
    }
}

