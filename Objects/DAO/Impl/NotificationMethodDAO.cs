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
    public class NotificationMethodDAO : AbstractBaseDAO, INotificationMethodDAO
    {
        public NotificationMethodDAO()
        {
        }

        public int CreateNotificationMethod(NotificationMethodDTO notificationMethodToCreate)
        {
            return CreateNotificationMethod(notificationMethodToCreate.Description);
        }

        public int CreateNotificationMethod(string description)
        {
            int id = -1;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_CreateNotificationMethod", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@description", description);

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
                Log.Error("Exception creating notification method entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return id;
        }

        public bool DeleteNotificationMethod(NotificationMethodDTO notificationMethodToDelete)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_DeleteNotificationMethod", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationMethodToDelete.Id);

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
                Log.Error("Exception deleting notification method with Id [" + notificationMethodToDelete.Id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }

        public NotificationMethodDTO GetNotificationMethod(int? id)
        {
            NotificationMethodDTO notificationMethod = null;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Description ";
                    sqlString += "FROM WebApplications.Notifications.NotificationMethod ";
                    sqlString += "WHERE ";
                    sqlString += "  Id = @id ";

                    command.Parameters.AddWithValue("@id", id);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (notificationMethod == null)
                            {
                                notificationMethod = new NotificationMethodDTO()
                                {
                                    Id = Convert.ToInt32(dataReader["Id"]),
                                    Description = Convert.ToString(dataReader["Description"])

                                };
                            }

                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving notification method by Id [" + id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationMethod;
        }


        public List<NotificationMethodDTO> GetAllNotificationMethods()
        {
            List<NotificationMethodDTO> notificationMethodList = new List<NotificationMethodDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Description ";
                    sqlString += "FROM WebApplications.Notifications.NotificationMethod ";

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationMethodDTO notificationMethod = new NotificationMethodDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                Description = Convert.ToString(dataReader["Description"])
                            };
                            notificationMethodList.Add(notificationMethod);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationMethodList;
        }

        public List<NotificationMethodDTO> GetNotificationMethodsByNotificationId(int? notificationId)
        {
            List<NotificationMethodDTO> notificationMethodList = new List<NotificationMethodDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Description ";
                    sqlString += "FROM WebApplications.Notifications.vw_MethodWithNotification ";
                    sqlString += "WHERE ";
                    sqlString += "  NotificationId = @id ";

                    command.Parameters.AddWithValue("@id", notificationId);


                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationMethodDTO notificationMethod = new NotificationMethodDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                Description = Convert.ToString(dataReader["Description"])
                            };
                            notificationMethodList.Add(notificationMethod);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationMethodList;
        }

        public bool UpdateNotificationMethod(NotificationMethodDTO notificationMethodToUpdate)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_UpdateNotificationMethod", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationMethodToUpdate.Id);
                    command.Parameters.AddWithValue("@description", notificationMethodToUpdate.Description);

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
                Log.Error("Exception updating notification method entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }
    }
}

