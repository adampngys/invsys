using LogicUniversityApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp.Service
{
    public interface IService
    {
        #region department

        // department staff raises request
        //void raiseRequest(Staff s1, int quantityNeed);


        // dh approves or rejects the request from department staff
        void DH_processRequest(Request request, bool bApprove);

        #endregion


        #region store

        // clerk sets the due date to process the requests from all departments
        List<RequestDetail> filterRequest(DateTime dueDate);


        #endregion
    }
}