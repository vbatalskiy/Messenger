using System;
using System.Linq;
using System.Windows;
using WpfClient.Common;
using WpfClient.Views;

namespace WpfClient.ViewModels
{
    class ViewFactory
    {
        public static IView Build<T>(ViewModelBase<T> viewModel) where T : class
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            var view = CreateView(viewModel.GetType());
            view.Owner = Application.Current.Windows.Cast<Window>().SingleOrDefault(w => w.IsActive);
            view.DataContext = viewModel;
            return (IView)view;
        }

        private static Window CreateView(Type vmType)
        {
            if (vmType == typeof(MainWindowModel))
                return new MainWindowView();
            if (vmType == typeof(LogonWindowViewModel))
                return new LogonWindowView();
            throw new NotSupportedException("ViewModel is not supported: " + vmType);
        }
    }
}
