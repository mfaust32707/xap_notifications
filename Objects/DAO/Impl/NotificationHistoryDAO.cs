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
    public class NotificationHistoryDAO : AbstractBaseDAO, INotificationHistoryDAO
    {
        public NotificationHistoryDAO()
        {
        }

        public int CreateNotificationHistory(NotificationHistoryDTO notificationHistoryToCreate)
        {
            int id = -1;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_CreateNotificationHistory", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@notificationId", notificationHistoryToCreate.NotificationId);
                    command.Parameters.AddWithValue("@notificationMethodId", notificationHistoryToCreate.NotificationMethodId);
                    command.Parameters.AddWithValue("@recipientUserId", notificationHistoryToCreate.RecipientUserId);
                    command.Parameters.AddWithValue("@applicationId", notificationHistoryToCreate.ApplicationId);
                    command.Parameters.AddWithValue("@readFlag", notificationHistoryToCreate.ReadFlag);
                    command.Parameters.AddWithValue("@messageBody", notificationHistoryToCreate.MessageBody);
                    command.Parameters.AddWithValue("@expiredFlag", notificationHistoryToCreate.ExpiredFlag);
                    command.Parameters.AddWithValue("@expirationDateTime", notificationHistoryToCreate.ExpirationDateTime);

                    id = Convert.ToInt32(command.ExecuteScalar());

                    if (id < 0)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (InvalidOperationException rbEx)
                        {
                            Log.Error("Exception rolling back transaction: " + rbEx.Message);
                            Log.Error(rbEx.StackTrace);
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception creating notification History entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return id;
        }

        public bool DeleteNotificationHistory(NotificationHistoryDTO notificationHistoryToDelete)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_DeleteNotificationHistory", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationHistoryToDelete.Id);

                    success = Convert.ToInt32(command.ExecuteNonQuery()) == 1;

                    if (!success)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (InvalidOperationException rbEx)
                        {
                            Log.Error("Exception rolling back transaction: " + rbEx.Message);
                            Log.Error(rbEx.StackTrace);
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception deleting notification History with Id [" + notificationHistoryToDelete.Id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }

        public NotificationHistoryDTO GetNotificationHistory(int? id)
        {
            NotificationHistoryDTO notificationHistory = null;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  NotificationId, ";
                    sqlString += "  RecipientUserId, ";
                    sqlString += "  ApplicationId, ";
                    sqlString += "  ReadFlag, ";
                    sqlString += "  MessageBody, ";
                    sqlString += "  ExpiredFlag, ";
                    sqlString += "  ExpirationDateTime ";
                    sqlString += "FROM WebApplications.Notifications.NotificationHistory ";
                    sqlString += "WHERE ";
                    sqlString += "  Id = @id ";

                    command.Parameters.AddWithValue("@id", id);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (notificationHistory == null)
                            {
                                notificationHistory = new NotificationHistoryDTO()
                                {
                                    Id = Convert.ToInt32(dataReader["Id"]),
                                    NotificationId = Convert.ToInt32(dataReader["Id"]),
                                    RecipientUserId = Convert.ToInt32(dataReader["RecipientUserId"]),
                                    ApplicationId = Convert.ToInt32(dataReader["ApplicationId"]),
                                    MessageBody = Convert.ToString(dataReader["MessageBody"]),
                                    ReadFlag = Convert.ToBoolean(dataReader["ReadFlag"]),
                                    ExpiredFlag = Convert.ToBoolean(dataReader["ExpiredFlag"]),
                                    ExpirationDateTime = Convert.ToDateTime(dataReader["ExpirationDateTime"])

                                };
                            }

                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving notification History by Id [" + id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationHistory;
        }


        public List<NotificationHistoryDTO> GetAllNotificationHistory()
        {
           

            return GetAllNotificationHistory(null, null);
        }


        public List<NotificationHistoryDTO> GetAllNotificationHistory(int? appId, int? userId)
        {
            List<NotificationHistoryDTO> notificationHistoryList = new List<NotificationHistoryDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "WebApplications.Notifications.sp_GetNotificationHistory";

                    command.Parameters.AddWithValue("@appId", appId);
                    command.Parameters.AddWithValue("@userId", userId);
                    
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationHistoryDTO notificationHistory = new NotificationHistoryDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificationId"]),
                                NotificationMethodId = Convert.ToInt32(dataReader["NotificationMethodId"]),
                                MethodDescription = Convert.ToString(dataReader["MethodDescription"]),
                                RecipientUserId = Convert.ToInt32(dataReader["RecipientUserId"]),
                                Subject = Convert.ToString(dataReader["Subject"]),
                                ApplicationId = Convert.ToInt32(dataReader["ApplicationId"]),
                                MessageBody = Convert.ToString(dataReader["MessageBody"]),
                                ReadFlag = Convert.ToBoolean(dataReader["ReadFlag"]),
                                ExpiredFlag = Convert.ToBoolean(dataReader["ExpiredFlag"]),
                                ExpirationDateTime = Convert.ToDateTime(dataReader["ExpirationDateTime"]),
                                SentDateTime = Convert.ToDateTime(dataReader["SentDateTime"])
                            };
                            notificationHistoryList.Add(notificationHistory);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationHistoryList;
        }

        public bool UpdateNotificationHistory(NotificationHistoryDTO notificationHistoryToUpdate)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_UpdateNotificationHistory", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationHistoryToUpdate.Id);
                    command.Parameters.AddWithValue("@notificationId", notificationHistoryToUpdate.NotificationId);
                    command.Parameters.AddWithValue("@recipientUserId", notificationHistoryToUpdate.RecipientUserId);
                    command.Parameters.AddWithValue("@applicationId", notificationHistoryToUpdate.ApplicationId);
                    command.Parameters.AddWithValue("@readFlag", notificationHistoryToUpdate.ReadFlag);
                    command.Parameters.AddWithValue("@messageBody", notificationHistoryToUpdate.MessageBody);
                    command.Parameters.AddWithValue("@expiredFlag", notificationHistoryToUpdate.ExpiredFlag);
                    command.Parameters.AddWithValue("@expirationDateTime", notificationHistoryToUpdate.ExpirationDateTime);
                    command.Parameters.AddWithValue("@notificationMethodId", notificationHistoryToUpdate.NotificationMethodId);

            success = Convert.ToInt32(command.ExecuteNonQuery()) == 1;

                    if (!success)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (InvalidOperationException rbEx)
                        {
                            Log.Error("Exception rolling back transaction: " + rbEx.Message);
                            Log.Error(rbEx.StackTrace);
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception updating notification History entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }
    }
}

