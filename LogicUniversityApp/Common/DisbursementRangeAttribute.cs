using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.Common
{
    public class DisbursementRangeAttribute : ValidationAttribute
    {
        private readonly string _qtyRequired;
        private readonly string _qtyActual;
        private readonly string _qtyDamaged;
        private readonly string _qtyMissing;
        public DisbursementRangeAttribute(
            string _qtyRequired
            , string _qtyActual
            , string _qtyDamaged
            , string _qtyMissing
            )
        {
            this._qtyRequired = _qtyRequired;
            this._qtyActual = _qtyActual;
            this._qtyDamaged = _qtyDamaged;
            this._qtyMissing = _qtyMissing;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _qtyRequiredProperty = validationContext.ObjectType.GetProperty(_qtyRequired);
            if (_qtyRequiredProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _qtyRequired));
            }

            var _qtyActualProperty = validationContext.ObjectType.GetProperty(_qtyActual);
            if (_qtyActualProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _qtyActual));
            }

            var _damagedProperty = validationContext.ObjectType.GetProperty(_qtyDamaged);
            if (_damagedProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _qtyDamaged));
            }

            var _missingProperty = validationContext.ObjectType.GetProperty(_qtyMissing);
            if (_missingProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _qtyMissing));
            }

            int qtyPrepared = (int)_qtyRequiredProperty.GetValue(validationContext.ObjectInstance, null);
            int qtyActual = (int)_qtyActualProperty.GetValue(validationContext.ObjectInstance, null);
            int qtyDamaged = (int)_damagedProperty.GetValue(validationContext.ObjectInstance, null);
            int qtyMissing = (int)_missingProperty.GetValue(validationContext.ObjectInstance, null);

            if (qtyActual < 0)
            {
                return new ValidationResult("quantity should bigger than zero");
            }

            var temp = qtyPrepared - qtyDamaged - qtyMissing;
            if (temp < 0)
            {
                return new ValidationResult("invalid lump sum");
            }
            else
            {
                if (temp != qtyActual)
                {
                    return new ValidationResult(string.Format("quantity should be: {0}", temp));
                }
            }
            return null;
        }
    }
}