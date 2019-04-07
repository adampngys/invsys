using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.Common
{
    public class MySpecialRangeAttribute : ValidationAttribute
    {
        private readonly string _quantityTotalNeed;
        private readonly string _balance;
        private readonly string _quantityRetrieval;
        private readonly string _quantityInstoreDamaged;
        private readonly string _quantityInstoreMissing;
        public MySpecialRangeAttribute(
            string _quantityTotalNeed
            , string _balance
            , string _quantityRetrieval
            , string _quantityInstoreDamaged
            , string _quantityInstoreMissing
            )
        {
            this._quantityTotalNeed = _quantityTotalNeed;
            this._balance = _balance;
            this._quantityRetrieval = _quantityRetrieval;
            this._quantityInstoreDamaged = _quantityInstoreDamaged;
            this._quantityInstoreMissing = _quantityInstoreMissing;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _TotalNeedProperty = validationContext.ObjectType.GetProperty(_quantityTotalNeed);
            if (_TotalNeedProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _quantityTotalNeed));
            }

            var _balanceProperty = validationContext.ObjectType.GetProperty(_balance);
            if (_balanceProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _balance));
            }

            var _retrievalProperty = validationContext.ObjectType.GetProperty(_quantityRetrieval);
            if (_retrievalProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _quantityRetrieval));
            }

            var _damagedProperty = validationContext.ObjectType.GetProperty(_quantityInstoreDamaged);
            if (_damagedProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _quantityInstoreDamaged));
            }

            var _missingProperty = validationContext.ObjectType.GetProperty(_quantityInstoreMissing);
            if (_missingProperty == null)
            {
                return new ValidationResult(string.Format("Unknown property {0}", _quantityInstoreMissing));
            }

            int totalNeed = (int)_TotalNeedProperty.GetValue(validationContext.ObjectInstance, null);
            int balance = (int)_balanceProperty.GetValue(validationContext.ObjectInstance, null);
            int retrieval = (int)_retrievalProperty.GetValue(validationContext.ObjectInstance, null);
            int damaged = (int)_damagedProperty.GetValue(validationContext.ObjectInstance, null);
            int missing = (int)_missingProperty.GetValue(validationContext.ObjectInstance, null);

            if (retrieval < 0)
            {
                return new ValidationResult("quantity should bigger than zero");
            }

            if (balance < (totalNeed + damaged + missing))
            {
                // cannot cover
                int maxPossible = balance - damaged - missing;

                if (maxPossible >= 0)
                {
                    if (maxPossible != retrieval)
                    {
                        return new ValidationResult(
                            string.Format("please take the max possible amount: {0}", maxPossible));
                    }
                }
                else
                {
                    return new ValidationResult("please correct the damaged or missing amount");
                }
            }
            else
            {
                // can cover
                if (totalNeed != retrieval)
                {
                    return new ValidationResult(
                        string.Format("please take the max possible amount: {0}", totalNeed));
                }
            }
            return null;
        }
    }
}