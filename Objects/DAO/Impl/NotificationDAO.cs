using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ChainLinkUtils.Base.DAO;
using ChainLinkUtils.Utils;
using Serilog;
using NotificationsService.Objects.DTO;
using ChainLinkUtils.Utils.Objects.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace NotificationsService.Objects.DAO.Impl
{
    public class NotificationDAO : AbstractBaseDAO, INotificationDAO    
    {
        public NotificationDAO()
        {
        }

        public int CreateNotification(NotificationDTO notificationToCreate)
        {
            return CreateNotification(notificationToCreate.Description);
        }

        public int CreateNotification(string description)
        {
            int id = -1;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_CreateNotificationHeader", connection))
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
                Log.Error("Exception creating notification entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return id;
        }

        public bool DeleteNotification(NotificationDTO notificationToDelete)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_DeleteNotificationHeader", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationToDelete.Id);

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
                Log.Error("Exception deleting notification with Id [" + notificationToDelete.Id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }

        public NotificationDTO GetNotification(int? id)
        {
            NotificationDTO notification = null;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Description ";
                    sqlString += "FROM WebApplications.Notifications.NotificationHeader ";
                    sqlString += "WHERE ";
                    sqlString += "  Id = @id ";

                    command.Parameters.AddWithValue("@id", id);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (notification == null)
                            {
                                notification = new NotificationDTO()
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
                Log.Error("Exception retrieving notification by Id [" + id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notification;
        }

        
        public List<NotificationDTO> GetAllNotifications()
        {
            List<NotificationDTO> notificationList = new List<NotificationDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Description ";
                    sqlString += "FROM WebApplications.system.NotificationHeader ";
                   
                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationDTO notification = new NotificationDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                Description = Convert.ToString(dataReader["Description"])
                            };
                            notificationList.Add(notification);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return notificationList;
        }

        public bool UpdateNotification(NotificationDTO notificationToUpdate)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_UpdateNotificationHeader", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", notificationToUpdate.Id);
                    command.Parameters.AddWithValue("@descro[topm", notificationToUpdate.Description);

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

