using ARWNI2S.Engine.Configuration;
using Microsoft.Extensions.Options;

namespace ARWNI2S.Core.Configuration
{
    internal class NI2SCoreNI2SOptionsSetup : IConfigureOptions<NI2SOptions>, IPostConfigureOptions<NI2SOptions>
    {
        public void Configure(NI2SOptions options)
        {

        }

        public void PostConfigure(string name, NI2SOptions options)
        {

        }
    }
}