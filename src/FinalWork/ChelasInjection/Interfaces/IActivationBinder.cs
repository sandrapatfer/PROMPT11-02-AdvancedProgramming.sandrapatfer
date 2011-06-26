namespace ChelasInjection
{
    /// <summary>
    /// Interface to configure the activation plug in in a type configuration binding
    /// </summary>
    /// <typeparam name="T">Type being configured</typeparam>
    public interface IActivationBinder<T>
    {
        IActivationPlugIn ActivationPlugIn { set; }
        ITypeBinder<T> GetCurrentTypeBinder { get; }
    }
}