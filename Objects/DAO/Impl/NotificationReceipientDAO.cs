using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ChainLinkUtils.Base.DAO;
using ChainLinkUtils.Utils;
using Serilog;
using NotificationsService.Objects.DTO;
using ChainLinkUtils.Utils.Objects.DTO;

namespace NotificationsService.Objects.DAO.Impl
{
    public class NotificationRecipientDAO : AbstractBaseDAO, INotificationRecipientDAO
    {
        public NotificationRecipientDAO()
        {
        }

        public List<NotificationRecipientDTO> GetNotificationRecipients(List<int> recipientIds, List<int> applicationIds)
        {
       
            List<NotificationRecipientDTO> recipients = new List<NotificationRecipientDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  UserId, ";
                    sqlString += "  ApplicationId, ";
                    sqlString += "  FirstName, ";
                    sqlString += "  LastName, ";
                    sqlString += "  EmailAddress ";
                    sqlString += "FROM WebApplications.system.vw_UserApplicationRef ";
                    sqlString += " WHERE 1=1 ";

                    if (recipientIds != null && recipientIds.Count>0)
                    {
                        string users = String.Join(",", recipientIds);

                        sqlString += " AND UserId in (SELECT * FROM ChainLink.system.SplitString(@users, ',')) ";
                        command.Parameters.AddWithValue("@users", users);
                    }

                    if (applicationIds != null && applicationIds.Count > 0)
                    {
                        string apps = String.Join(",", applicationIds);

                        sqlString += " AND ApplicationId in (SELECT * FROM ChainLink.system.SplitString(@apps, ',')) ";
                        command.Parameters.AddWithValue("@apps", apps);
                    }

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationRecipientDTO recipient = new NotificationRecipientDTO()
                            {
                                UserId = Convert.ToInt32(dataReader["UserId"]),
                                ApplicationId = Convert.ToInt32(dataReader["ApplicationId"]),
                                FirstName = Convert.ToString(dataReader["FirstName"]),
                                LastName = Convert.ToString(dataReader["LastName"]),
                                EmailAddress = Convert.ToString(dataReader["EmailAddress"])
                            };
                            recipients.Add(recipient);                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return recipients;
        }

       
    }
}

