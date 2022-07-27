using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;


namespace NotificationsService.Objects.DAO
{
	public interface INotificationDAO: IBaseDAO
	{
		NotificationDTO GetNotification(int? notificationId);
		List<NotificationDTO> GetAllNotifications();
		int CreateNotification(string notificationDescription);
        bool DeleteNotification(NotificationDTO notification);
		bool UpdateNotification(NotificationDTO notification);

	}
}
