using System;
using System.Windows;
using WpfClient.Common;

namespace WpfClient
{
    class MainWindowModel : ViewModelBase<CommunicationParams>, IDisposable
    {
        public MainWindowModel(CommunicationParams model)
            : base(model)
        {

        }

        #region Start command

        private RelayCommand _sendCmd;
        public RelayCommand SendCmd
        {
            get
            {
                return this._sendCmd ??
                    (this._sendCmd = new RelayCommand(Send, Validate));
            }
        }

        private string messageText;
        public string MessageText {
            get { return messageText; }
            set
            {
                if (value != messageText)
                {
                    messageText = value;
                    OnPropertyChanged("MessageText");
                }        
            } 
        }

        private void Send(object obj)
        {
            var p = (CommunicationParams)obj;

            StartSend(p);
        }

        private bool Validate(object obj)
        {
            return Model != null;
        }

        #endregion

        #region Private
        /// <summary>
        /// TODO implement sending functionality
        /// </summary>
        /// <param name="fttParams"></param>
        private void StartSend(CommunicationParams fttParams)
        {
            try
            {
                var textToSend = MessageText;
                MessageText = string.Empty;
                MessageBox.Show(string.Format("Message sent:{0}", textToSend));

            }
            catch (Exception ex)
            {
                ShowError(ex.ToString());
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        public void Dispose()
        {

        }
    }
}
