using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    public class StaffDepartmentDAO
    {
        /// <summary>
        /// Converting Department to Department view model
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public static DepartmentViewModel ConvertDepartmentToDepartmentViewModel(Department department)
        {
            DepartmentViewModel depView = new DepartmentViewModel()
            {
                departmentId = department.departmentId,
                departmentName = department.departmentName,
                collectionPointId = department.collectionPointId,
                collectionPointDescription = department.CollectionPoint.collectionPointDescription,
                staffIdDH = department.staffIdDH,
                DHName = department.Staff1.staffName,
                staffIdDR = department.staffIdDR,
                DRName = department.Staff2.staffName,
                staffIdContact = department.staffIdContact,
                ContactName = department.Staff.staffName,
                departmentFax = department.departmentFax,
                departmentPhone = department.departmentPhone
            };
            return depView;
        }

        /// <summary>
        /// Find department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public static Department getDepartmentById(string departmentId)
        {
            var dep = DbFactory.Instance.context.Departments.Where(x => x.departmentId == departmentId).FirstOrDefault();
            return dep;
        }

        /// <summary>
        /// Parse to staff view model
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static StaffViewModel staffViewModelConverter(Staff s)
        {
            if (s == null)
                return null;

            var svm = new StaffViewModel()
            {
                staffId = s.staffId,
                staffName = s.staffName,
                departmentId = s.departmentId,
                delegatedStatus = s.delegatedStatus,
                delegatedStartDate = s.delegatedStartDate,
                delegatedEndDate = s.delegatedEndDate
            };
            return svm;
        }

        /// <summary>
        /// Update the role of temp department head when today's date reach the assigned date
        /// </summary>
        /// <returns></returns>
        public static Boolean CheckAllTempDH()
        {
            try
            {
                List<Department> depList = DbFactory.Instance.context.Departments.ToList();
                foreach (Department department in depList)
                {
                    Staff staff = GetDelegatedStaff(department);
                    if (staff != null)
                    {
                        // Make sure he got an authority for the today
                        if (staff.delegatedEndDate < DateTime.Today)
                        {
                            RemoveAuthority(staff);
                        }
                        else if (staff.delegatedStartDate <= DateTime.Today)
                        {
                            AssignDepRole(staff.staffId, "Temp DH");
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Find the temp department head of this department
        /// </summary>
        /// <param name="dep">department info</param>
        /// <returns></returns>
        public static Staff GetDelegatedStaff(Department dep)
        {
            try
            {
                Staff result = DbFactory.Instance.context.Staffs.Where(x => x.departmentId == dep.departmentId && x.delegatedStatus == true).First();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Remove the management position of this staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static bool RemoveAuthority(Staff staff)
        {
            try
            {
                // If the staff's department dr is him, assign him as DR
                if (staff.Department.staffIdDR == staff.staffId)
                {
                    AssignDepRole(staff.staffId, "DR");
                }
                // If not assign him as staff
                else
                {
                    AssignDepRole(staff.staffId, "Staff");
                }

                staff.delegatedStatus = false;
                staff.delegatedStartDate = null;
                staff.delegatedEndDate = null;
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Assign the management position to this staff
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool AssignDepRole(int staffId, string roleName)
        {
            using (var dbContextTransaction = DbFactory.Instance.context.Database.BeginTransaction())
            {
                try
                {
                    var staff = DbFactory.Instance.context.Staffs.Where(x => x.staffId == staffId).First();

                    // Find the roles by the user id
                    var user = DbFactory.Instance.context.AspNetUsers.Where(x => x.Id == staff.userId).First();

                    var role = DbFactory.Instance.context.AspNetRoles.Where(x => x.Name == roleName).First();

                    user.AspNetRoles = new HashSet<AspNetRole>();
                    user.AspNetRoles.Add(role);
                    DbFactory.Instance.context.SaveChanges();

                    // if this guy is DR or DH, we also update the department table                
                    switch (roleName)
                    {
                        case "DR":
                            var dep = DbFactory.Instance.context.Departments.Where(x => x.departmentId == staff.departmentId).First();
                            dep.staffIdDR = staff.staffId;
                            DbFactory.Instance.context.SaveChanges();
                            break;

                        default:
                            break;
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Assign the temp department head position to this staff
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool GiveAuthority(int? staffId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                Staff staff = GetStaffByStaffId(staffId);
                //AssignDepRole(staff.staffId, "Temp DH");
                staff.delegatedStatus = true;
                staff.delegatedStartDate = startDate;
                staff.delegatedEndDate = endDate;
                DbFactory.Instance.context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Find the role of this staff
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static AspNetRole FindStaffRole(int staffId)
        {
            var staff = DbFactory.Instance.context.Staffs.Where(x => x.staffId == staffId).First();

            // Find the roles by the user id
            var user = DbFactory.Instance.context.AspNetUsers.Where(x => x.Id == staff.userId).First();
            var role = user.AspNetRoles.First();

            return role;
        }

        /// <summary>
        /// Find the staff who is in the given role of this department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static Staff FindDepartmentRole(string departmentId, string roleName)
        {
            var staffs = DbFactory.Instance.context.Staffs.Where(x => x.departmentId == departmentId).ToList();

            Staff sRes = null;

            foreach (var s in staffs)
            {
                var role = FindStaffRole(s.staffId);
                if (role != null && role.Name == roleName)
                {
                    sRes = s;
                    break;
                }
            }
            return sRes;
        }

        /// <summary>
        /// Find staffs in this department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public static List<Staff> FindAllStaffInDepartment(string departmentId)
        {
            return DbFactory.Instance.context.Staffs.Where(x => x.departmentId == departmentId).ToList();
        }

        /// <summary>
        /// Find the possible department rep list
        /// </summary>
        /// <param name="s1"></param>
        /// <returns></returns>
        public static List<Staff> FindPossibleDRList(Staff s1)
        {
            if (s1 == null || s1.departmentId == null)
                return null;

            Staff depDH = FindDepartmentRole(s1.departmentId, "DH");
            Staff depTempDH = FindDepartmentRole(s1.departmentId, "Temp DH");

            List<Staff> staffList = FindAllStaffInDepartment(s1.departmentId).
                Where(x => x.staffId != (depDH == null ? -1 : depDH.staffId) &&
                    x.staffId != (depTempDH == null ? -1 : depTempDH.staffId)
                ).ToList();
            return staffList;
        }

        /// <summary>
        /// Find the staff info by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Staff GetStaffByUserId(string userId)
        {
            var staff = DbFactory.Instance.context.Staffs.Where(x => x.userId == userId).FirstOrDefault();
            return staff;
        }

        /// <summary>
        /// Find the staff info by staff id
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static Staff GetStaffByStaffId(int? staffId)
        {
            var result = DbFactory.Instance.context.Staffs.Where(x => x.staffId == staffId).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Assign department rep from one staff to another
        /// </summary>
        /// <param name="staffId_Assigner"></param>
        /// <param name="staffId_newDR"></param>
        /// <returns></returns>
        public static bool changeDR(int staffId_Assigner, int staffId_newDR)
        {
            Staff sAssigner = GetStaffByStaffId(staffId_Assigner);
            Staff sNewDR = GetStaffByStaffId(staffId_newDR);

            // Must in same department
            if (sAssigner == null || sNewDR == null || sAssigner.departmentId != sNewDR.departmentId)
            {
                return false;
            }

            var dh = FindDepartmentRole(sAssigner.departmentId, "DH");
            var dhTemp = FindDepartmentRole(sAssigner.departmentId, "Temp DH");

            // Assigner must be DH or Temp DH
            bool bPass = false;
            if (dh != null && sAssigner.staffId == dh.staffId && staffId_newDR != dh.staffId)
            {
                bPass = true;
            }

            if (dhTemp != null && sAssigner.staffId == dhTemp.staffId)
            {
                bPass = true;
            }

            if (bPass == false)
            {
                return false;
            }

            var sOldDR = FindDepartmentRole(sAssigner.departmentId, "DR");

            if (sOldDR != null)
            {
                AssignDepRole(sOldDR.staffId, "Staff");
            }

            AssignDepRole(sNewDR.staffId, "DR");
            return true;
        }

    }
}