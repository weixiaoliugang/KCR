using GalaSoft.MvvmLight;
using System.ComponentModel.DataAnnotations;

namespace KinectControlRobot.Application.Validation
{
    /// <summary>
    /// Base class for the classes need to be validated
    /// </summary>
    public abstract class BaseValidateObject : ObservableObject
    {
        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void validateProperty<T>(T value, string propertyName)
        {
            Validator.ValidateProperty(value,
                new ValidationContext(this, null, null) { MemberName = propertyName });
        }

        /// <summary>
        /// Validate this class
        /// </summary>
        /// <returns>
        /// If this class have error
        /// </returns>
        public bool Valid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null), null, true);
        }
    }
}
