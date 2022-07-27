using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;

namespace NotificationsService.Objects.DAO
{
    public interface INotificationMethodDAO: IBaseDAO

    {
        NotificationMethodDTO GetNotificationMethod(int? notificationMethodId);
        List<NotificationMethodDTO> GetAllNotificationMethods();
        List<NotificationMethodDTO> GetNotificationMethodsByNotificationId(int? notificationId);
        int CreateNotificationMethod(NotificationMethodDTO notificationMethod);
        bool DeleteNotificationMethod(NotificationMethodDTO notificationMethod);
        bool UpdateNotificationMethod(NotificationMethodDTO notificationMethod);
    }
}
