using System;
using Xunit;
using Xunit.Gherkin.Quick;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/tuples.feature")]
    public class UnitTest1 : Feature
    {
        [Given(@"a ← tuple({float}, {float}, {float}, {float})")]
        public void Tuple_a(float t1, float t2, float t3, float t4)
        {
        }

        [Then(@"Then a.x = {float}")]
        public void Then_x(float expectedX)
        {
        }
    }
}
