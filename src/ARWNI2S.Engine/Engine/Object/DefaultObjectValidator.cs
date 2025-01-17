using ARWNI2S.Core.Models;
using ARWNI2S.Engine.Configuration;

namespace ARWNI2S.Engine.Object
{
    internal class DefaultObjectValidator : IObjectModelValidator
    {
        private IModelMetadataProvider modelMetadataProvider;
        private object modelValidatorProviders;
        private NI2SOptions options;

        public DefaultObjectValidator(IModelMetadataProvider modelMetadataProvider, object modelValidatorProviders, NI2SOptions options)
        {
            this.modelMetadataProvider = modelMetadataProvider;
            this.modelValidatorProviders = modelValidatorProviders;
            this.options = options;
        }
    }
}