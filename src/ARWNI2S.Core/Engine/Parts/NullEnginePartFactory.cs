using ARWNI2S.Engine.EngineParts;
using System.Reflection;

namespace ARWNI2S.Engine.Parts
{
    /// <summary>
    /// An <see cref="EnginePartFactory"/> that produces no parts.
    /// <para>
    /// This factory may be used to to preempt Mvc's default part discovery allowing for custom configuration at a later stage.
    /// </para>
    /// </summary>
    public class NullEnginePartFactory : EnginePartFactory
    {
        /// <inheritdoc />
        public override IEnumerable<EnginePart> GetEngineParts(Assembly assembly)
        {
            return Enumerable.Empty<EnginePart>();
        }
    }
}
