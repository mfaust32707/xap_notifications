using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;

namespace NotificationsService.Objects.DAO
{
    public interface INotificationRecipientDAO: IBaseDAO

    {
        List<NotificationRecipientDTO> GetNotificationRecipients(List<int> recipientIds, List<int> applicationIds);
       
    }
}
