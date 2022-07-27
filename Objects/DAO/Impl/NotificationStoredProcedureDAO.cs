using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ChainLinkUtils.Base.DAO;
using ChainLinkUtils.Utils;
using Serilog;
using NotificationsService.Objects.DTO;
using ChainLinkUtils.Utils.Objects.DTO;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

namespace NotificationsService.Objects.DAO.Impl
{
    public class NotificationStoredProcedureDAO : AbstractBaseDAO, INotificationStoredProcedureDAO
    {
        public NotificationStoredProcedureDAO()
        {
        }

        public List<NotificationStoredProcedureDTO> GetNotificationStoredProcedures(int? notificationId)
        {

            List<NotificationStoredProcedureDTO> storedProcs = new List<NotificationStoredProcedureDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Notification, ";
                    sqlString += "  StoredProcedureName ";
                    sqlString += "FROM WebApplications.Notifications.NotificationStoredProcedures ";
                    sqlString += " WHERE 1=1 ";

                    if (notificationId != null)
                    {
                       
                        sqlString += " AND NotificationId = @notificationId";
                        command.Parameters.AddWithValue("@notificationId", notificationId);
                    }

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationStoredProcedureDTO storedProc = new NotificationStoredProcedureDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificationId"]),
                                StoredProcedureName = Convert.ToString(dataReader["StoredProcedureName"])
                            };
                            storedProcs.Add(storedProc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notification/stored procedures: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return storedProcs;
        }

        public List<NotificationStoredProcedureDTO> GetNotificationsForStoredProcedure(string storedProcedure)
        {

            List<NotificationStoredProcedureDTO> storedProcs = new List<NotificationStoredProcedureDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Notification, ";
                    sqlString += "  StoredProcedureName ";
                    sqlString += "FROM WebApplications.Notifications.NotificationStoredProcedures ";
                    sqlString += " WHERE 1=1 ";

                    if (storedProcedure != null)
                    {

                        sqlString += " AND StoredProcedureName = @storedProc";
                        command.Parameters.AddWithValue("@storedProc", storedProcedure);
                    }

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            NotificationStoredProcedureDTO storedProc = new NotificationStoredProcedureDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificationId"]),
                                StoredProcedureName = Convert.ToString(dataReader["StoredProcedureName"])
                            };
                            storedProcs.Add(storedProc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notification/stored procedures: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return storedProcs;
        }

        public NotificationStoredProcedureDTO GetNotificationStoredProcedure(int? Id)
        {

            NotificationStoredProcedureDTO storedProc = new NotificationStoredProcedureDTO();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  Notification, ";
                    sqlString += "  StoredProcedureName ";
                    sqlString += "FROM WebApplications.Notifications.NotificationStoredProcedures ";
                    sqlString += " WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", Id);
                    
                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            storedProc = new NotificationStoredProcedureDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificationId"]),
                                StoredProcedureName = Convert.ToString(dataReader["StoredProcedureName"])
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notification/stored procedure: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return storedProc;
        }

        public NotificationStoredProcedureDTO GetNotificationStoredProcedure(int? NotificationId, string StoredProcedure)
        {

            NotificationStoredProcedureDTO storedProc = new NotificationStoredProcedureDTO();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  NotificationId, ";
                    sqlString += "  StoredProcedureName ";
                    sqlString += "FROM WebApplications.Notifications.NotificationStoredProcedures ";
                    sqlString += " WHERE NotificationId = @notificationId AND StoredProcedureName = @storedProc";
                    command.Parameters.AddWithValue("@notificationId", NotificationId);
                    command.Parameters.AddWithValue("@storedProc", StoredProcedure);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            storedProc = new NotificationStoredProcedureDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                NotificationId = Convert.ToInt32(dataReader["NotificationId"]),
                                StoredProcedureName = Convert.ToString(dataReader["StoredProcedureName"])
                            };
                        }
                        else
                        {
                            storedProc = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notification/stored procedure: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return storedProc;
        }

        public int CreateNotificationStoredProcedure(NotificationStoredProcedureDTO notificationToCreate)
        {
            int id = -1;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_CreateNotificationStoredProc", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@notificationId", notificationToCreate.NotificationId);
                    command.Parameters.AddWithValue("@storedProcedure", notificationToCreate.StoredProcedureName);

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
                Log.Error("Exception creating notification stored procedure entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return id;
        }

        public bool UpdateNotificationStoredProcedure(NotificationStoredProcedureDTO storedProcToUpdate)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_UpdateNotificationStoredProc", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", storedProcToUpdate.Id);
                    command.Parameters.AddWithValue("@notificationId", storedProcToUpdate.NotificationId);
                    command.Parameters.AddWithValue("@storedProcedure", storedProcToUpdate.StoredProcedureName);

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
                Log.Error("Exception updating notification stored procedure entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }


        public bool DeleteNotificationStoredProcedure(NotificationStoredProcedureDTO storedProcedureToDelete)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_DeleteNotificationStoredProcedure", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", storedProcedureToDelete.Id);

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
                Log.Error("Exception deleting notification stored procedure with Id [" + storedProcedureToDelete.Id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }
 
    }

}

