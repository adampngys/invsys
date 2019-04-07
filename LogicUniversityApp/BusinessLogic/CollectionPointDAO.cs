using LogicUniversityApp.Models;
using LogicUniversityApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversityApp
{
    public class CollectionPointDAO
    {
        /// <summary>
        /// Find the collection points
        /// </summary>
        /// <returns></returns>
        public static List<CollectionPointViewModel> GetCollectionPoints()
        {
            var cps = DbFactory.Instance.context.CollectionPoints.Select(x =>
             new CollectionPointViewModel()
             {
                 collectionPointId = x.collectionPointId,
                 collectionPointDescription = x.collectionPointDescription
             }).ToList();
            return cps;
        }

        /// <summary>
        /// Find the collection points
        /// </summary>
        /// <returns></returns>
        public static List<CollectionPointViewModel> GetCollectionPointViewModels()
        {
            var cps = DbFactory.Instance.context.CollectionPoints.Select(x =>
             new CollectionPointViewModel()
             {
                 collectionPointId = x.collectionPointId,
                 collectionPointDescription = x.collectionPointDescription
             }).ToList();
            return cps;
        }

        /// <summary>
        /// Update the collection point
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="collectionPointId"></param>
        public static void UpdateCollectionPoint(string departmentId, int collectionPointId)
        {
            var dep = DbFactory.Instance.context.Departments.Where(x => x.departmentId == departmentId).First();
            dep.collectionPointId = collectionPointId;
            DbFactory.Instance.context.SaveChanges();

            string newPoint = DbFactory.Instance.context.CollectionPoints.AsNoTracking().Where(x => x.collectionPointId == collectionPointId).Select(x => x.collectionPointDescription).FirstOrDefault();

            string mailSubject = "Notification for changing the collection point";
            string mailContent = string.Format("{0} department: the new collection point is {1}", dep.departmentName, newPoint);

            bool result = MyEmail.SendEmail("sa47team1@gmail.com", mailSubject, mailContent);
        }
    }
}