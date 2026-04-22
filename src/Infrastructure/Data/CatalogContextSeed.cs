using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiamma.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fiamma.Infrastructure.Data;

public class CatalogContextSeed
{
    public static async Task SeedAsync(
        CatalogContext catalogContext,
        ILogger logger,
        int retry = 0)
    {
        var retryForAvailability = retry;
        try
        {
            if (catalogContext.Database.IsSqlServer())
            {
                catalogContext.Database.Migrate();
            }

            var preconfiguredBrands = GetPreconfiguredCatalogBrands().ToList();
            var existingBrands = await catalogContext.CatalogBrands.ToListAsync();
            var missingBrands = preconfiguredBrands
                .Where(preconfiguredBrand => !existingBrands.Any(existingBrand =>
                    existingBrand.Brand.Equals(preconfiguredBrand.Brand, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (missingBrands.Count > 0)
            {
                await catalogContext.CatalogBrands.AddRangeAsync(missingBrands);
                await catalogContext.SaveChangesAsync();
            }

            var preconfiguredTypes = GetPreconfiguredCatalogTypes().ToList();
            var existingTypes = await catalogContext.CatalogTypes.ToListAsync();
            var missingTypes = preconfiguredTypes
                .Where(preconfiguredType => !existingTypes.Any(existingType =>
                    existingType.Type.Equals(preconfiguredType.Type, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (missingTypes.Count > 0)
            {
                await catalogContext.CatalogTypes.AddRangeAsync(missingTypes);
                await catalogContext.SaveChangesAsync();
            }

            var brands = await catalogContext.CatalogBrands.ToListAsync();
            var types = await catalogContext.CatalogTypes.ToListAsync();
            var preconfiguredItems = GetPreconfiguredItems(brands, types);

            var existingItemNames = await catalogContext.CatalogItems
                .Select(catalogItem => catalogItem.Name)
                .ToListAsync();
            var existingItemNamesSet = new HashSet<string>(existingItemNames, StringComparer.OrdinalIgnoreCase);

            var missingItems = preconfiguredItems
                .Where(preconfiguredItem => !existingItemNamesSet.Contains(preconfiguredItem.Name))
                .ToList();

            if (missingItems.Count > 0)
            {
                await catalogContext.CatalogItems.AddRangeAsync(missingItems);
                await catalogContext.SaveChangesAsync();
            }

            var preconfiguredItemNameSet = new HashSet<string>(
                preconfiguredItems.Select(preconfiguredItem => preconfiguredItem.Name),
                StringComparer.OrdinalIgnoreCase);
            var preconfiguredItemsInDatabase = await catalogContext.CatalogItems
                .Where(catalogItem => preconfiguredItemNameSet.Contains(catalogItem.Name))
                .ToListAsync();
            var preconfiguredItemIds = preconfiguredItemsInDatabase
                .Select(catalogItem => catalogItem.Id)
                .ToList();

            var itemIdsWithOptions = new HashSet<int>();
            if (preconfiguredItemIds.Count > 0)
            {
                itemIdsWithOptions = new HashSet<int>(await catalogContext.ProductOptions
                    .Where(option => preconfiguredItemIds.Contains(option.CatalogItemId))
                    .Select(option => option.CatalogItemId)
                    .Distinct()
                    .ToListAsync());
            }

            var itemsWithoutOptions = preconfiguredItemsInDatabase
                .Where(catalogItem => !itemIdsWithOptions.Contains(catalogItem.Id))
                .ToList();

            if (itemsWithoutOptions.Count > 0)
            {
                var options = GetPreconfiguredProductOptions(itemsWithoutOptions);
                await catalogContext.ProductOptions.AddRangeAsync(options);
                await catalogContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvailability >= 10) throw;

            retryForAvailability++;
            logger.LogError(ex.Message);
            await SeedAsync(catalogContext, logger, retryForAvailability);
            throw;
        }
    }

    static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
    {
        return new List<CatalogBrand>
        {
            new("Artisan Collective"),
            new("Crafted & Co"),
            new("Silk & Cotton"),
            new("Woodland Works"),
            new("Handmade Studio"),
            new("Other")
        };
    }

    static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
    {
        return new List<CatalogType>
        {
            new("Clothing"),
            new("Jackets"),
            new("Bottoms"),
            new("Pottery"),
            new("Woodwork")
        };
    }

    static List<CatalogItem> GetPreconfiguredItems(
        IReadOnlyCollection<CatalogBrand> brands,
        IReadOnlyCollection<CatalogType> types)
    {
        var clothingTypeId = types.FirstOrDefault(t => t.Type.Equals("Clothing", StringComparison.OrdinalIgnoreCase))?.Id
            ?? types.First().Id;
        var bottomsTypeId = types.FirstOrDefault(t => t.Type.Equals("Bottoms", StringComparison.OrdinalIgnoreCase))?.Id
            ?? clothingTypeId;
        var jacketsTypeId = types.FirstOrDefault(t => t.Type.Equals("Jackets", StringComparison.OrdinalIgnoreCase))?.Id
            ?? clothingTypeId;

        int BrandId(string preferredName) =>
            brands.FirstOrDefault(b => b.Brand.Equals(preferredName, StringComparison.OrdinalIgnoreCase))?.Id
            ?? brands.First().Id;

        return new List<CatalogItem>
        {
            new(
                clothingTypeId,
                BrandId("Handmade Studio"),
                "Relaxed black street tee with subtle flame pocket detail. Perfect for everyday wear.",
                "Fiamma Ember Pocket T-Shirt",
                1199M,
                "/images/products/fiamma-ember-pocket-tee.png"),
            new(
                clothingTypeId,
                BrandId("Artisan Collective"),
                "Bold red cotton crew with clean Fiamma chest branding and modern drop-shoulder fit.",
                "Fiamma Crimson Classic T-Shirt",
                1349M,
                "/images/products/fiamma-crimson-classic-tee.png"),
            new(
                clothingTypeId,
                BrandId("Crafted & Co"),
                "Electric blue oversized tee with front flame graphic and soft premium jersey fabric.",
                "Fiamma Electric Flame T-Shirt",
                1499M,
                "/images/products/fiamma-electric-flame-tee.png"),
            new(
                clothingTypeId,
                BrandId("Silk & Cotton"),
                "White oversized graphic tee with inferno print and breathable heavyweight cotton feel.",
                "Fiamma Inferno Graphic T-Shirt",
                1599M,
                "/images/products/fiamma-inferno-graphic-tee.png"),
            new(
                clothingTypeId,
                BrandId("Woodland Works"),
                "Navy premium polo shirt with minimal branding and tailored athletic silhouette.",
                "Fiamma Midnight Polo Shirt",
                1899M,
                "/images/products/fiamma-midnight-polo-shirt.png"),
            new(
                bottomsTypeId,
                BrandId("Handmade Studio"),
                "Black utility cargo bottoms with oversized pockets and adjustable ankle cuffs.",
                "Fiamma Shadow Cargo Bottoms",
                2100M,
                "/images/products/fiamma-shadow-cargo-bottoms.png"),
            new(
                bottomsTypeId,
                BrandId("Artisan Collective"),
                "White street cargo bottoms with multi-pocket storage and clean tapered cut.",
                "Fiamma Frost Cargo Bottoms",
                2100M,
                "/images/products/fiamma-frost-cargo-bottoms.png"),
            new(
                bottomsTypeId,
                BrandId("Crafted & Co"),
                "Graphite gray utility bottoms built for movement with strap pocket detailing.",
                "Fiamma Graphite Utility Bottoms",
                2100M,
                "/images/products/fiamma-graphite-utility-bottoms.png"),
            new(
                bottomsTypeId,
                BrandId("Silk & Cotton"),
                "Brown cargo bottoms with relaxed fit and side utility compartments.",
                "Fiamma Mocha Cargo Bottoms",
                2100M,
                "/images/products/fiamma-mocha-cargo-bottoms.png"),
            new(
                jacketsTypeId,
                BrandId("Woodland Works"),
                "Black insulated hooded jacket with weatherproof shell and tonal FIAMMA back branding.",
                "Fiamma Obsidian Hooded Jacket",
                3200M,
                "/images/products/fiamma-obsidian-hooded-jacket.png"),
            new(
                jacketsTypeId,
                BrandId("Crafted & Co"),
                "Navy storm jacket with zip chest pocket and clean technical silhouette.",
                "Fiamma Storm Navy Jacket",
                3200M,
                "/images/products/fiamma-storm-navy-jacket.png"),
            new(
                jacketsTypeId,
                BrandId("Artisan Collective"),
                "Olive hooded utility jacket with insulated lining and minimalist embroidered FIAMMA mark.",
                "Fiamma Olive Ridge Jacket",
                3200M,
                "/images/products/fiamma-olive-ridge-jacket.png")
        };
    }

    static IEnumerable<ProductOption> GetPreconfiguredProductOptions(List<CatalogItem> items)
    {
        var options = new List<ProductOption>();

        foreach (var item in items)
        {
            options.Add(new ProductOption(item.Id, ProductOption.OptionType.Size, "M", "Medium"));
            options.Add(new ProductOption(item.Id, ProductOption.OptionType.Size, "L", "Large"));
            options.Add(new ProductOption(item.Id, ProductOption.OptionType.Size, "XL", "Extra Large", 100M));
        }

        return options;
    }
}
