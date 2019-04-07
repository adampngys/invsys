using LogicUniversityApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.BusinessLogic
{
    public class SupplierDAO
    {
        /// <summary>
        /// Find the suppliers
        /// </summary>
        /// <returns></returns>
        public static List<Supplier> GetSupplierList()
        {
            return DbFactory.Instance.context.Suppliers.ToList();
        }

        /// <summary>
        /// Find the supplier by supplier id
        /// </summary>
        /// <param name="id">supplier id</param>
        /// <returns></returns>
        public static Supplier FindSupplierById(string id)
        {
            return DbFactory.Instance.context.Suppliers.Find(id);
        }

        /// <summary>
        /// Create a new supplier
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Boolean CreateSupplier(Supplier s)
        {
            try
            {
                DbFactory.Instance.context.Suppliers.Add(s);
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Update the supplier info
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public static Boolean EditSupplier(Supplier update)
        {
            try
            {
                Supplier supplier = FindSupplierById(update.supplierId);
                supplier.address = update.address;
                supplier.contactName = update.contactName;
                supplier.email = update.email;
                supplier.supplierName = update.supplierName;
                supplier.phone = update.phone;
                supplier.fax = update.fax;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete the existed supplier
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Boolean DeleteSupplier(Supplier s)
        {
            try
            {
                DbFactory.Instance.context.Suppliers.Remove(s);
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}