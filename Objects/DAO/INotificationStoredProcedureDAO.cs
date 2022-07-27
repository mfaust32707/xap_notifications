using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;

namespace NotificationsService.Objects.DAO
{
    public interface INotificationStoredProcedureDAO: IBaseDAO

    {
        NotificationStoredProcedureDTO GetNotificationStoredProcedure(int? Id);
        NotificationStoredProcedureDTO GetNotificationStoredProcedure(int? NotificationId, string StoredProcedure);
        List<NotificationStoredProcedureDTO> GetNotificationStoredProcedures(int? NotificationId);
        List<NotificationStoredProcedureDTO> GetNotificationsForStoredProcedure(string StoredProcedure);
        int CreateNotificationStoredProcedure(NotificationStoredProcedureDTO NotificationStoredProcedure);
        bool DeleteNotificationStoredProcedure(NotificationStoredProcedureDTO NotificationStoredProcedure);
        bool UpdateNotificationStoredProcedure(NotificationStoredProcedureDTO NotificationStoredProcedure);

    }
}
