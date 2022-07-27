using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;

namespace NotificationsService.Objects.DAO
{
    public interface INotificationHistoryDAO: IBaseDAO

    {
        NotificationHistoryDTO GetNotificationHistory(int? notificationHistoryId);
        List<NotificationHistoryDTO> GetAllNotificationHistory(int? appId, int? userId);
        int CreateNotificationHistory(NotificationHistoryDTO notificationHistory);
        bool DeleteNotificationHistory(NotificationHistoryDTO notificationHistory);
        bool UpdateNotificationHistory(NotificationHistoryDTO notificationHistory);
    }
}
