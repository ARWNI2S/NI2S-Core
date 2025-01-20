using System.Collections;

namespace ARWNI2S.Engine.Actor.Components
{
    public class ComponentTree : IEnumerable<IActorComponent>, IEnumerator<IActorComponent>
    {
        public IActorComponent Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IActorComponent> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}