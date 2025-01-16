using ARWNI2S.Builder;

namespace ARWNI2S.Extensibility
{
    public interface IConfigureEngine : IOrdered
    {
        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="builder">Builder for configuring an application's request pipeline</param>
        void Configure(INiisBuilder builder);
    }
}
