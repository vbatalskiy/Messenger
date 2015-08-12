using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using WpfClient.Common;
using WpfClient.Properties;
using WpfClient.ViewModels;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindowModel viewModel;
        
        private void OnStart(object sender, StartupEventArgs e)
        {
            var model = new CommunicationParams();
            viewModel = new MainWindowModel(model);
            var view = ViewFactory.Build(viewModel);
            view.Show();
        }

        private void OnClose(object sender, ExitEventArgs e)
        {
            try
            {
                var serializer = GetSerializer<CommunicationParams>();
                using (var stringWriter = new StringWriter())
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    serializer.WriteObject(writer, viewModel.Model);
                    Settings.Default.MainParam = stringWriter.ToString();
                    Settings.Default.Save();
                }
            }
            finally
            {
                viewModel.Dispose();
            }
        }

        private DataContractSerializer GetSerializer<T>()
        {
            return new DataContractSerializer(typeof(T), null, Int32.MaxValue, true, true, null);
        }
    }
}
