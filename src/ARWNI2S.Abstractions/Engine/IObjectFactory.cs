/* Cambio no fusionado mediante combinación del proyecto 'ARWNI2S.Abstractions (net8.0)'
Antes:
namespace ARWNI2S.Engine.Object
{
Después:
using ARWNI2S;
using ARWNI2S.Engine;
using ARWNI2S.Engine;
using ARWNI2S.Engine.Object;

namespace ARWNI2S.Engine
{
*/
namespace ARWNI2S.Engine
{
    public interface IObjectFactory
    {
        TObject CreateInstance<TObject>() where TObject : INiisObject;

        INiisObject CreateInstance(Type type);
    }

    public interface IObjectFactory<TObject> : IObjectFactory where TObject : INiisObject
    {
        TObject CreateInstance();
    }
}