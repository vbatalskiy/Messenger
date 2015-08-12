using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace WpfClient.Common
{
    [Serializable]
    public abstract class ViewModelBase<TModel> : INotifyPropertyChanged where TModel : class
    {
        #region Private Fields

        private static readonly IDictionary<string, PropertyChangedEventArgs> eventArgCache;
        private static readonly object eventArgCacheSync = new object();

        #endregion

        #region Properties

        public TModel Model { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Static constructor.
        /// </summary>
        static ViewModelBase()
        {
            eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
        }

        /// <summary>
        ///   Default constructor.
        /// </summary>
        internal ViewModelBase(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            Model = model;
        }

        #endregion

        #region Externally Visible Methods

        protected static T Clone<T>(T obj) where T : class
        {
            if (obj == null)
                return null;

            var serializer = new DataContractSerializer(typeof(T), null, Int32.MaxValue, true, true, null);
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        ///   Derived classes can override this method to execute logic after a property is set. The base implementation does nothing.
        /// </summary>
        /// <param name="propertyName"> The property which was changed. </param>
        protected virtual void AfterPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        ///   Attempts to raise the <see cref="PropertyChanged" /> event, and invokes the virtual <see cref="AfterPropertyChanged" /> method, regardless of whether the event was raised or not.
        /// </summary>
        /// <param name="propertyName"> The property which was changed. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChangedEventHandler propertyChanged = PropertyChanged;

            if (propertyChanged != null)
                propertyChanged(this, GetPropertyChangedEventArgs(propertyName));

            AfterPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }
            OnPropertyChanged(memberExpression.Member.Name);
        }


        #endregion



        #region Validation

        protected static bool ValidateFolderExists(string toValidate, bool invalidThrowEx = false)
        {
            bool result = Directory.Exists(toValidate);

            if (!result && invalidThrowEx)
                throw new ApplicationException("Folder doesn't exist: " + toValidate);

            return result;
        }

        protected static bool ValidateNotEmpty(string toValidate, bool invalidThrowEx = false)
        {
            bool result = !String.IsNullOrEmpty(toValidate);

            if (!result && invalidThrowEx)
                throw new ApplicationException("Required value");

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///   Returns an instance of <see cref="PropertyChangedEventArgs" /> for the specified property name.
        /// </summary>
        /// <param name="propertyName"> The name of the property to create event args for. </param>
        /// <returns> The instance of <see cref="PropertyChangedEventArgs" /> </returns>
        private static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            PropertyChangedEventArgs result;

            lock (eventArgCacheSync)
            {
                if (!eventArgCache.ContainsKey(propertyName))
                    eventArgCache.Add(propertyName, new PropertyChangedEventArgs(propertyName));

                result = eventArgCache[propertyName];
            }

            return result;
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            if (propertyName == string.Empty)
                return;

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                //if (this.ThrowOnInvalidPropertyName)
                throw new Exception(msg);
                //Debug.Fail(msg);
            }
        }

        #endregion

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
