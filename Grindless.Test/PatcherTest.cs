using HarmonyLib;
using Xunit;

namespace Grindless.Test
{
    public class PatcherTest
    {
        /// <summary>
        /// Test that the patches actually compile when running the game.
        /// This is a lengthy process, so only run the tests when necessary.
        /// </summary>
        [Fact]
        public void TestPatchCompilation()
        {
            new Harmony("Grindless").PatchAll(typeof(Globals).Assembly);
        }
    }
}