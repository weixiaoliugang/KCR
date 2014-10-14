using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.ComponentModel.DataAnnotations;

namespace KinectControlRobot.Application.Validation
{
    public abstract class BaseValidateObject : ObservableObject
    {
        protected void ValidateProperty<T>(T value, string propertyName)
        {
            Validator.ValidateProperty(value,
                new ValidationContext(this, null, null) { MemberName = propertyName });
        }

        public bool Valid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null), null, true);
        }
    }
}
