using Fiamma.Web.Extensions;
using Xunit;

namespace Fiamma.UnitTests.Web.Extensions.CacheHelpersTests;

public class GenerateTypesCacheKey
{
    [Fact]
    public void ReturnsTypesCacheKey()
    {
        var result = CacheHelpers.GenerateTypesCacheKey();

        Assert.Equal("types", result);
    }
}


