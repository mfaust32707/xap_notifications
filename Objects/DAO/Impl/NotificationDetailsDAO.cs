using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ChainLinkUtils.Base.DAO;
using ChainLinkUtils.Utils;
using NotificationsService.Objects.DTO;
using Serilog;

namespace NotificationsService.Objects.DAO.Impl
{
    public class NotificationDetailsDAO : AbstractBaseDAO, INotificationDetailsDAO
    {
        public NotificationDetailsDAO()
        {
        }

        public int CreateNotificationDetails(NotificationDetailDTO notificationDetailsToCreate)
        {

            int id = -1;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_CreateNotificationDetails", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@notificationId", notificationDetailsToCreate.NotificationId);
                    command.Parameters.AddWithValue("@messageFormat", notificationDetailsToCreate.MessageFormat);
                    command.Parameters.AddWithValue("@messageTemplate", notificationDetailsToCreate.MessageTemplate);

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
                Log.Error("Exception creating notification entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return id;
        }

        public bool DeleteNotificationDetails(NotificationDetailDTO notificationDetailsToDelete)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.NotificationDetailss.sp_DeleteNotificationDetails", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationDetailsToDelete.Id);

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
                Log.Error("Exception deleting notification with Id [" + notificationDetailsToDelete.Id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }

        public NotificationDetailDTO GetNotificationDetails(int? id)
        {
            NotificationDetailDTO notificationDetails = null;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Description ";
                    sqlString += "FROM WebApplications.Notifications.NotificationDetails ";
                    sqlString += "WHERE ";
                    sqlString += "  Id = @id ";

                    command.Parameters.AddWithValue("@id", id);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (notificationDetails == null)
                            {
                                notificationDetails = new NotificationDetailDTO()
                                {
                                    Id = Convert.ToInt32(dataReader["Id"]),
                                    MessageFormat = Convert.ToString(dataReader["MessageFormat"]),
                                    MessageTemplate = Convert.ToString(dataReader["MessageTemplate"]),
                                    NotificationId = Convert.ToInt32(dataReader["NotificaitonId"])

                                };
                            }

                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving notification by Id [" + id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationDetails;
        }


        public List<NotificationDetailDTO> GetAllNotificationDetails()
        {
            List<NotificationDetailDTO> notificationDetailsList = new List<NotificationDetailDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  MessageFormat, ";
                    sqlString += "  MessageTemplate, ";
                    sqlString += "  NotificationId ";
                    sqlString += "FROM WebApplications.Notifications.NotificationDetails ";

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationDetailDTO notificationDetails = new NotificationDetailDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                MessageFormat = Convert.ToString(dataReader["MessageFormat"]),
                                MessageTemplate = Convert.ToString(dataReader["MessageTemplate"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificaitonId"])

                            };
                            notificationDetailsList.Add(notificationDetails);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationDetailsList;
        }

        public List<NotificationDetailDTO> GetNotificationDetailsByNotificationId(int? notificationId)
        {
            List<NotificationDetailDTO> notificationDetailsList = new List<NotificationDetailDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  MessageFormat, ";
                    sqlString += "  MessageTemplate, ";
                    sqlString += "  NotificationId ";
                    sqlString += "FROM WebApplications.Notifications.NotificationDetail ";
                    sqlString += "WHERE NotificationId = @notificationId ";

                    command.Parameters.AddWithValue("@notificationId", notificationId);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationDetailDTO notificationDetails = new NotificationDetailDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                MessageFormat = Convert.ToString(dataReader["MessageFormat"]),
                                MessageTemplate = Convert.ToString(dataReader["MessageTemplate"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificationId"])

                            };
                            notificationDetailsList.Add(notificationDetails);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationDetailsList;
        }
        public bool UpdateNotificationDetails(NotificationDetailDTO notificationDetailsToUpdate)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_UpdateNotificationDetails", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationDetailsToUpdate.Id);
                    command.Parameters.AddWithValue("@messageFormat", notificationDetailsToUpdate.MessageFormat);
                    command.Parameters.AddWithValue("@messageTemplate", notificationDetailsToUpdate.MessageTemplate);
                    command.Parameters.AddWithValue("@notificationId", notificationDetailsToUpdate.NotificationId);

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
                Log.Error("Exception updating notification entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }
    }
}

