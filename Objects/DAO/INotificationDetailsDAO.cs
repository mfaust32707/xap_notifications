using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;

namespace NotificationsService.Objects.DAO
{
    public interface INotificationDetailsDAO: IBaseDAO
    {
        NotificationDetailDTO GetNotificationDetails(int? notificationDetailId);
        List<NotificationDetailDTO> GetAllNotificationDetails();
        List<NotificationDetailDTO> GetNotificationDetailsByNotificationId(int? notificationId);
        int CreateNotificationDetails(NotificationDetailDTO notificationDetail);
        bool DeleteNotificationDetails(NotificationDetailDTO notificationDetail);
        bool UpdateNotificationDetails(NotificationDetailDTO notificationDetail);
    }
}
