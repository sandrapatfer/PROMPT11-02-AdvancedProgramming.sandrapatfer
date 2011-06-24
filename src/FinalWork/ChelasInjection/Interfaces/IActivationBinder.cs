namespace ChelasInjection
{
    public interface IActivationBinder<T>
    {
        IActivationPlugIn ActivationPlugIn { set; }
        ITypeBinder<T> GetCurrentTypeBinder { get; }
    }
}