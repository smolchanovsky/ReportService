using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Common.Testing;

public class AutoInjectAttribute : AutoDataAttribute
{
    public AutoInjectAttribute()
        :base(() => new Fixture().Customize(new AutoMoqCustomization()))
    {
    }
}