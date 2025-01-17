namespace ARWNI2S.Core.Models
{
    internal class DefaultCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
    {
        private object modelMetadataDetailsProviders;

        public DefaultCompositeMetadataDetailsProvider(object modelMetadataDetailsProviders)
        {
            this.modelMetadataDetailsProviders = modelMetadataDetailsProviders;
        }
    }
}